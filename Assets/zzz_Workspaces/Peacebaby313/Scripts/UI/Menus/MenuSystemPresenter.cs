//----- MenuSystemPresenter.cs START -----

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public enum MenuSystemContext
{
    MainMenu,
    PauseMenu
}

public sealed class MenuSystemPresenter
    : MonoBehaviour
{
    [Header("Menu Context")]
    [SerializeField]
    private MenuSystemContext menuContext =
        MenuSystemContext.MainMenu;

    [SerializeField]
    private bool showOnStart = true;

    [Header("Visible Menu Root")]
    [Tooltip(
        "This child is shown and hidden. " +
        "The Canvas root containing this presenter must remain active.")]
    [SerializeField]
    private GameObject menuVisualRoot;

    [Header("Menu Panels")]
    [SerializeField]
    private GameObject mainMenuRoot;

    [SerializeField]
    private GameObject passwordMenuRoot;

    [SerializeField]
    private GameObject settingsMenuRoot;

    [Header("Menu Controllers")]
    [SerializeField]
    private PasswordMenuController passwordMenuController;

    [SerializeField]
    private SettingsMenuController settingsMenuController;

    [Header("Title")]
    [SerializeField]
    private TMP_Text menuTitleText;

    [SerializeField]
    private string mainMenuTitle =
        "MAIN MENU";

    [SerializeField]
    private string pauseMenuTitle =
        "PAUSED";

    [Header("Initial Selection")]
    [SerializeField]
    private GameObject firstSelection;

    [Header("Pause Integration")]
    [Tooltip(
        "Gameplay instance only. Assign a component that " +
        "implements IPauseGameAuthority.")]
    [SerializeField]
    private MonoBehaviour pauseAuthoritySource;

    private IPauseGameAuthority pauseAuthority;

    private Coroutine selectionRoutine;

    private bool subscribedToPauseAuthority;
    private bool sceneTransitionInProgress;

    public MenuSystemContext Context =>
        menuContext;

    public bool IsVisible =>
        menuVisualRoot != null &&
        menuVisualRoot.activeSelf;

    private void Awake()
    {
        ApplyContextTitle();
        ResolvePauseAuthority();
        SetInitialPanelState();

        if (menuVisualRoot != null)
        {
            menuVisualRoot.SetActive(
                showOnStart);
        }
    }

    private void OnEnable()
    {
        SubscribeToPauseAuthority();
    }

    private void Start()
    {
        if (menuContext ==
                MenuSystemContext.PauseMenu &&
            pauseAuthority != null)
        {
            SetMenuVisible(
                pauseAuthority.IsPaused);

            return;
        }

        SetMenuVisible(
            showOnStart);
    }

    public void SetMenuVisible(
        bool visible)
    {
        if (sceneTransitionInProgress)
            return;

        CancelSelectionRoutine();

        if (visible)
        {
            ResetToHomeState();

            if (menuVisualRoot != null)
            {
                menuVisualRoot.SetActive(
                    true);
            }

            selectionRoutine =
                StartCoroutine(
                    SelectFirstControlNextFrame());

            return;
        }

        ResetToHomeState();

        if (menuVisualRoot != null)
        {
            menuVisualRoot.SetActive(
                false);
        }

        ClearSelection();
    }

    public void ShowMenu()
    {
        SetMenuVisible(
            true);
    }

    public void HideMenu()
    {
        SetMenuVisible(
            false);
    }

    public void SetPauseMenuVisible(
        bool paused)
    {
        SetMenuVisible(
            paused);
    }

    public void ResetToHomeState()
    {
        if (passwordMenuController != null &&
            passwordMenuController.IsOpen)
        {
            passwordMenuController.CloseMenu();
        }

        if (settingsMenuController != null &&
            settingsMenuController.IsOpen)
        {
            settingsMenuController.CloseMenu();
        }

        if (mainMenuRoot != null)
        {
            mainMenuRoot.SetActive(
                true);
        }

        if (passwordMenuRoot != null)
        {
            passwordMenuRoot.SetActive(
                false);
        }

        if (settingsMenuRoot != null)
        {
            settingsMenuRoot.SetActive(
                false);
        }
    }

    public void PrepareForSceneTransition()
    {
        if (sceneTransitionInProgress)
            return;

        sceneTransitionInProgress =
            true;

        CancelSelectionRoutine();

        if (menuContext ==
                MenuSystemContext.PauseMenu &&
            pauseAuthority != null &&
            pauseAuthority.IsPaused)
        {
            pauseAuthority.SetPaused(
                false);
        }

        if (menuVisualRoot != null)
        {
            menuVisualRoot.SetActive(
                false);
        }

        ClearSelection();
    }

    private void ResolvePauseAuthority()
    {
        pauseAuthority =
            pauseAuthoritySource as
                IPauseGameAuthority;

        if (menuContext !=
            MenuSystemContext.PauseMenu)
        {
            return;
        }

        if (pauseAuthoritySource == null)
        {
            Debug.LogWarning(
                "[MENU SYSTEM] Pause Menu context has no " +
                "Pause Authority Source assigned.");

            return;
        }

        if (pauseAuthority == null)
        {
            Debug.LogError(
                "[MENU SYSTEM] Pause Authority Source must " +
                "implement IPauseGameAuthority.");
        }
    }

    private void SubscribeToPauseAuthority()
    {
        if (subscribedToPauseAuthority ||
            pauseAuthority == null)
        {
            return;
        }

        pauseAuthority.PauseStateChanged +=
            HandlePauseStateChanged;

        subscribedToPauseAuthority =
            true;
    }

    private void UnsubscribeFromPauseAuthority()
    {
        if (!subscribedToPauseAuthority ||
            pauseAuthority == null)
        {
            return;
        }

        pauseAuthority.PauseStateChanged -=
            HandlePauseStateChanged;

        subscribedToPauseAuthority =
            false;
    }

    private void HandlePauseStateChanged(
        bool isPaused)
    {
        if (sceneTransitionInProgress)
            return;

        SetMenuVisible(
            isPaused);
    }

    private void ApplyContextTitle()
    {
        if (menuTitleText == null)
            return;

        menuTitleText.text =
            menuContext ==
                MenuSystemContext.PauseMenu
                ? pauseMenuTitle
                : mainMenuTitle;
    }

    private void SetInitialPanelState()
    {
        if (mainMenuRoot != null)
        {
            mainMenuRoot.SetActive(
                true);
        }

        if (passwordMenuRoot != null)
        {
            passwordMenuRoot.SetActive(
                false);
        }

        if (settingsMenuRoot != null)
        {
            settingsMenuRoot.SetActive(
                false);
        }
    }

    private IEnumerator
        SelectFirstControlNextFrame()
    {
        yield return null;

        selectionRoutine =
            null;

        if (!IsVisible ||
            EventSystem.current == null)
        {
            yield break;
        }

        EventSystem.current.SetSelectedGameObject(
            null);

        if (firstSelection != null &&
            firstSelection.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(
                firstSelection);
        }
    }

    private static void ClearSelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(
                null);
        }
    }

    private void CancelSelectionRoutine()
    {
        if (selectionRoutine == null)
            return;

        StopCoroutine(
            selectionRoutine);

        selectionRoutine =
            null;
    }

    private void OnValidate()
    {
        ApplyContextTitle();
    }

    private void OnDisable()
    {
        UnsubscribeFromPauseAuthority();
        CancelSelectionRoutine();
    }

    private void OnDestroy()
    {
        UnsubscribeFromPauseAuthority();
        CancelSelectionRoutine();
    }
}

//----- MenuSystemPresenter.cs END -----