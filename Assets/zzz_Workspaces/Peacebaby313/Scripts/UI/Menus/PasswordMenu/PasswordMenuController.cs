//----- PasswordMenuController.cs START -----

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class PasswordMenuController : MonoBehaviour
{
    [Header("Menu Roots")]
    [SerializeField] private GameObject mainMenuRoot;
    [SerializeField] private GameObject passwordPanelRoot;

    [Header("Selection Restoration")]
    [SerializeField]
    private GameObject mainMenuReturnSelection;

    [Header("Slots")]
    [SerializeField] private Transform slotParent;
    [SerializeField] private PasswordSlotUI slotPrefab;

    [Header("Buttons")]
    [SerializeField] private Button submitButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button backButton;

    [Header("Feedback")]
    [SerializeField] private TMP_Text statusText;

    [Header("Navigation Repeat")]
    [SerializeField, Min(0.05f)]
    private float initialRepeatDelay = 0.35f;

    [SerializeField, Min(0.03f)]
    private float repeatRate = 0.1f;

    private readonly List<PasswordSlotUI> slots =
        new List<PasswordSlotUI>();

    private int[] selectedTokenIndices =
        Array.Empty<int>();

    private ApplicationBootstrap bootstrap;

    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;
    private InputAction clearAction;

    private Keyboard subscribedKeyboard;

    private int selectedSlotIndex;
    private Vector2Int lastNavigationDirection;
    private float nextNavigationRepeatTime;

    private bool menuOpen;
    private bool requestInProgress;
    private bool inputEnabled;

    public bool IsOpen =>
        menuOpen;

    private void Awake()
    {
        BuildInputActions();
        AddButtonListeners();

        if (passwordPanelRoot != null)
        {
            passwordPanelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!menuOpen ||
            requestInProgress)
        {
            return;
        }

        HandleNavigationInput();

        if (submitAction.WasPressedThisFrame())
        {
            SubmitPassword();
        }

        if (cancelAction.WasPressedThisFrame())
        {
            CloseMenu();
        }

        if (clearAction.WasPressedThisFrame())
        {
            ClearCode();
        }
    }

    public void OpenMenu()
    {
        bootstrap =
            ApplicationBootstrap.Instance;

        if (bootstrap == null ||
            !bootstrap.IsInitialized ||
            bootstrap.LevelCodes == null ||
            !bootstrap.LevelCodes.IsReady)
        {
            Debug.LogError(
                "[PASSWORD MENU] Level-code services are not ready.");

            return;
        }

        if (!BuildSlotsIfNeeded())
            return;

        ResetTokenSelections();

        requestInProgress = false;
        menuOpen = true;

        if (mainMenuRoot != null)
        {
            mainMenuRoot.SetActive(false);
        }

        if (passwordPanelRoot != null)
        {
            passwordPanelRoot.SetActive(true);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        EnableMenuInput();
        SetButtonsInteractable(true);

        SetStatus(
            "Enter the eight-token level password.");

        RefreshAllSlots();
    }

    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        menuOpen = false;
        requestInProgress = false;

        DisableMenuInput();

        if (passwordPanelRoot != null)
        {
            passwordPanelRoot.SetActive(false);
        }

        if (mainMenuRoot != null)
        {
            mainMenuRoot.SetActive(true);
        }

        if (EventSystem.current != null &&
            mainMenuReturnSelection != null)
        {
            EventSystem.current.SetSelectedGameObject(
                mainMenuReturnSelection);
        }
    }

    public void SubmitPassword()
    {
        if (!menuOpen ||
            requestInProgress ||
            bootstrap == null ||
            bootstrap.LevelCodes == null)
        {
            return;
        }

        if (!IsCodeComplete())
        {
            SetStatus(
                "Complete all eight password slots.");

            return;
        }

        string[] submittedTokenIds =
            new string[selectedTokenIndices.Length];

        for (int index = 0;
             index < selectedTokenIndices.Length;
             index++)
        {
            PasswordTokenDefinition token =
                bootstrap.LevelCodes.GetTokenAtWrappedIndex(
                    selectedTokenIndices[index]);

            submittedTokenIds[index] =
                token?.TokenId ??
                string.Empty;
        }

        bool accepted =
            bootstrap.TryLoadLevelByPassword(
                submittedTokenIds,
                out string feedback);

        SetStatus(feedback);

        if (!accepted)
            return;

        requestInProgress = true;

        SetButtonsInteractable(false);
        DisableMenuInput();
    }

    public void ClearCode()
    {
        if (!menuOpen ||
            requestInProgress)
        {
            return;
        }

        ResetTokenSelections();
        RefreshAllSlots();

        SetStatus(
            "Password cleared.");
    }

    private bool BuildSlotsIfNeeded()
    {
        if (bootstrap == null ||
            bootstrap.LevelCodes == null)
        {
            return false;
        }

        if (slotParent == null)
        {
            Debug.LogError(
                "[PASSWORD MENU] Slot parent is missing.");

            return false;
        }

        if (slotPrefab == null)
        {
            Debug.LogError(
                "[PASSWORD MENU] Slot prefab is missing.");

            return false;
        }

        int requiredSlotCount =
            bootstrap.LevelCodes.PasswordLength;

        if (requiredSlotCount <= 0)
        {
            Debug.LogError(
                "[PASSWORD MENU] Password length is invalid.");

            return false;
        }

        if (slots.Count == requiredSlotCount)
            return true;

        foreach (PasswordSlotUI existingSlot in slots)
        {
            if (existingSlot != null)
            {
                Destroy(existingSlot.gameObject);
            }
        }

        slots.Clear();

        selectedTokenIndices =
            new int[requiredSlotCount];

        for (int index = 0;
             index < requiredSlotCount;
             index++)
        {
            PasswordSlotUI newSlot =
                Instantiate(
                    slotPrefab,
                    slotParent);

            newSlot.name =
                $"PasswordSlot_{index + 1:00}";

            slots.Add(newSlot);
        }

        return true;
    }

    private void ResetTokenSelections()
    {
        int requiredCount =
            bootstrap.LevelCodes.PasswordLength;

        if (selectedTokenIndices.Length != requiredCount)
        {
            selectedTokenIndices =
                new int[requiredCount];
        }

        for (int index = 0;
             index < selectedTokenIndices.Length;
             index++)
        {
            selectedTokenIndices[index] = -1;
        }

        selectedSlotIndex = 0;
        lastNavigationDirection = Vector2Int.zero;
    }

    private bool IsCodeComplete()
    {
        foreach (int tokenIndex in selectedTokenIndices)
        {
            if (tokenIndex < 0)
                return false;
        }

        return true;
    }

    private void RefreshAllSlots()
    {
        for (int index = 0;
             index < slots.Count;
             index++)
        {
            RefreshSlot(index);
        }
    }

    private void RefreshSlot(int slotIndex)
    {
        if (slotIndex < 0 ||
            slotIndex >= slots.Count)
        {
            return;
        }

        PasswordTokenDefinition token = null;

        if (selectedTokenIndices[slotIndex] >= 0)
        {
            token =
                bootstrap.LevelCodes.GetTokenAtWrappedIndex(
                    selectedTokenIndices[slotIndex]);
        }

        slots[slotIndex].SetToken(token);

        slots[slotIndex].SetSelected(
            slotIndex == selectedSlotIndex);
    }

    private void MoveSelection(int direction)
    {
        if (slots.Count == 0)
            return;

        int previousIndex =
            selectedSlotIndex;

        selectedSlotIndex =
            ((selectedSlotIndex + direction) %
             slots.Count +
             slots.Count) %
            slots.Count;

        RefreshSlot(previousIndex);
        RefreshSlot(selectedSlotIndex);
    }

    private void CycleSelectedToken(int direction)
    {
        if (slots.Count == 0 ||
            bootstrap == null ||
            bootstrap.LevelCodes == null)
        {
            return;
        }

        int tokenCount =
            bootstrap.LevelCodes.TokenSet.Count;

        if (tokenCount == 0)
            return;

        int currentIndex =
            selectedTokenIndices[selectedSlotIndex];

        if (currentIndex < 0)
        {
            selectedTokenIndices[selectedSlotIndex] =
                direction >= 0
                    ? 0
                    : tokenCount - 1;
        }
        else
        {
            currentIndex += direction;

            selectedTokenIndices[selectedSlotIndex] =
                ((currentIndex % tokenCount) +
                 tokenCount) %
                tokenCount;
        }

        RefreshSlot(selectedSlotIndex);
    }

    private void HandleNavigationInput()
    {
        Vector2 navigation =
            navigateAction.ReadValue<Vector2>();

        Vector2Int direction =
            ToCardinalDirection(navigation);

        if (direction == Vector2Int.zero)
        {
            lastNavigationDirection =
                Vector2Int.zero;

            return;
        }

        bool directionChanged =
            direction != lastNavigationDirection;

        bool repeatReady =
            Time.unscaledTime >=
            nextNavigationRepeatTime;

        if (!directionChanged &&
            !repeatReady)
        {
            return;
        }

        if (direction.x != 0)
        {
            MoveSelection(
                direction.x);
        }
        else if (direction.y != 0)
        {
            CycleSelectedToken(
                direction.y);
        }

        lastNavigationDirection =
            direction;

        nextNavigationRepeatTime =
            Time.unscaledTime +
            (directionChanged
                ? initialRepeatDelay
                : repeatRate);
    }

    private static Vector2Int ToCardinalDirection(
        Vector2 navigation)
    {
        const float threshold = 0.5f;

        if (navigation.sqrMagnitude <
            threshold * threshold)
        {
            return Vector2Int.zero;
        }

        if (Mathf.Abs(navigation.x) >=
            Mathf.Abs(navigation.y))
        {
            return new Vector2Int(
                navigation.x > 0f ? 1 : -1,
                0);
        }

        return new Vector2Int(
            0,
            navigation.y > 0f ? 1 : -1);
    }

    private void HandleTextInput(char character)
    {
        if (!menuOpen ||
            requestInProgress ||
            bootstrap == null ||
            bootstrap.LevelCodes == null)
        {
            return;
        }

        char normalizedCharacter =
            char.ToUpperInvariant(character);

        if (normalizedCharacter < 'A' ||
            normalizedCharacter > 'Z')
        {
            return;
        }

        string tokenId =
            normalizedCharacter.ToString();

        int tokenIndex =
            bootstrap.LevelCodes.GetTokenIndex(
                tokenId);

        if (tokenIndex < 0)
            return;

        selectedTokenIndices[selectedSlotIndex] =
            tokenIndex;

        RefreshSlot(selectedSlotIndex);
        MoveSelection(1);
    }

    private void BuildInputActions()
    {
        navigateAction =
            new InputAction(
                "Password Navigate",
                InputActionType.Value);

        navigateAction
            .AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");

        navigateAction.AddBinding(
            "<Gamepad>/dpad");

        navigateAction.AddBinding(
            "<Gamepad>/leftStick");

        submitAction =
            new InputAction(
                "Password Submit",
                InputActionType.Button);

        submitAction.AddBinding(
            "<Keyboard>/enter");

        submitAction.AddBinding(
            "<Keyboard>/numpadEnter");

        submitAction.AddBinding(
            "<Gamepad>/buttonSouth");

        cancelAction =
            new InputAction(
                "Password Cancel",
                InputActionType.Button);

        cancelAction.AddBinding(
            "<Keyboard>/escape");

        cancelAction.AddBinding(
            "<Gamepad>/buttonEast");

        clearAction =
            new InputAction(
                "Password Clear",
                InputActionType.Button);

        clearAction.AddBinding(
            "<Keyboard>/backspace");

        clearAction.AddBinding(
            "<Gamepad>/buttonWest");
    }

    private void EnableMenuInput()
    {
        if (inputEnabled)
            return;

        navigateAction.Enable();
        submitAction.Enable();
        cancelAction.Enable();
        clearAction.Enable();

        subscribedKeyboard =
            Keyboard.current;

        if (subscribedKeyboard != null)
        {
            subscribedKeyboard.onTextInput +=
                HandleTextInput;
        }

        inputEnabled = true;
    }

    private void DisableMenuInput()
    {
        if (!inputEnabled)
            return;

        navigateAction.Disable();
        submitAction.Disable();
        cancelAction.Disable();
        clearAction.Disable();

        if (subscribedKeyboard != null)
        {
            subscribedKeyboard.onTextInput -=
                HandleTextInput;

            subscribedKeyboard = null;
        }

        inputEnabled = false;
    }

    private void AddButtonListeners()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(
                SubmitPassword);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(
                ClearCode);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(
                CloseMenu);
        }
    }

    private void RemoveButtonListeners()
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(
                SubmitPassword);
        }

        if (clearButton != null)
        {
            clearButton.onClick.RemoveListener(
                ClearCode);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveListener(
                CloseMenu);
        }
    }

    private void SetButtonsInteractable(bool interactable)
    {
        if (submitButton != null)
        {
            submitButton.interactable =
                interactable;
        }

        if (clearButton != null)
        {
            clearButton.interactable =
                interactable;
        }

        if (backButton != null)
        {
            backButton.interactable =
                interactable;
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text =
                message;
        }
    }

    private void OnDisable()
    {
        DisableMenuInput();
    }

    private void OnDestroy()
    {
        DisableMenuInput();
        RemoveButtonListeners();

        navigateAction?.Dispose();
        submitAction?.Dispose();
        cancelAction?.Dispose();
        clearAction?.Dispose();
    }
}

//----- PasswordMenuController.cs END -----