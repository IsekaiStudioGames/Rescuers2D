//----- SettingsMenuController.cs START -----

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public sealed class SettingsMenuController
    : MonoBehaviour
{
    private enum SettingsPanel
    {
        Home,
        Audio,
        Graphics
    }

    [Header("Menu Roots")]
    [SerializeField]
    private GameObject hostMenuRoot;

    [SerializeField]
    private GameObject settingsPanelRoot;

    [Header("Settings Panels")]
    [SerializeField]
    private GameObject settingsHomePanel;

    [SerializeField]
    private GameObject audioSettingsPanel;

    [SerializeField]
    private GameObject graphicsSettingsPanel;

    [Header("Panel Controllers")]
    [SerializeField]
    private GraphicsSettingsController
        graphicsSettingsController;

    [Header("Home Buttons")]
    [SerializeField]
    private Button audioSettingsButton;

    [SerializeField]
    private Button graphicsSettingsButton;

    [SerializeField]
    private Button resetDefaultsButton;

    [SerializeField]
    private Button closeSettingsButton;

    [Header("Submenu Buttons")]
    [SerializeField]
    private Button audioBackButton;

    [SerializeField]
    private Button graphicsBackButton;

    [Header("Selection")]
    [SerializeField]
    private GameObject hostReturnSelection;

    [SerializeField]
    private GameObject settingsHomeFirstSelection;

    [SerializeField]
    private GameObject audioFirstSelection;

    [SerializeField]
    private GameObject graphicsFirstSelection;

    [Header("Feedback")]
    [SerializeField]
    private TMP_Text statusText;

    private ApplicationBootstrap bootstrap;
    private SettingsService settingsService;

    private InputAction cancelAction;

    private SettingsPanel activePanel =
        SettingsPanel.Home;

    private bool menuOpen;

    public bool IsOpen =>
        menuOpen;

    private void Awake()
    {
        BuildInputActions();
        AddButtonListeners();

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }
    }

    private void Update()
    {
        if (!menuOpen)
            return;

        if (cancelAction.WasPressedThisFrame())
        {
            HandleBackRequest();
        }
    }

    public void OpenMenu()
    {
        bootstrap =
            ApplicationBootstrap.Instance;

        if (bootstrap == null ||
            !bootstrap.IsInitialized ||
            bootstrap.SettingsService == null ||
            !bootstrap.SettingsService.IsInitialized)
        {
            Debug.LogError(
                "[SETTINGS MENU] Settings services are not ready.");

            return;
        }

        settingsService =
            bootstrap.SettingsService;

        menuOpen = true;

        if (hostMenuRoot != null)
        {
            hostMenuRoot.SetActive(false);
        }

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(true);
        }

        cancelAction.Enable();

        ShowHomePanel();
    }

    public void CloseMenu()
    {
        if (!menuOpen)
            return;

        if (activePanel ==
            SettingsPanel.Graphics)
        {
            graphicsSettingsController
                ?.RevertPendingChanges(
                    showFeedback: false);
        }

        menuOpen = false;

        cancelAction.Disable();

        if (settingsPanelRoot != null)
        {
            settingsPanelRoot.SetActive(false);
        }

        if (hostMenuRoot != null)
        {
            hostMenuRoot.SetActive(true);
        }

        SelectObject(
            hostReturnSelection);
    }

    public void ShowHomePanel()
    {
        if (activePanel ==
            SettingsPanel.Graphics)
        {
            graphicsSettingsController
                ?.RevertPendingChanges(
                    showFeedback: false);
        }

        activePanel =
            SettingsPanel.Home;

        SetPanelVisibility(
            showHome: true,
            showAudio: false,
            showGraphics: false);

        SetStatus(
            "Choose a settings category.");

        SelectObject(
            settingsHomeFirstSelection);
    }

    public void ShowAudioPanel()
    {
        activePanel =
            SettingsPanel.Audio;

        SetPanelVisibility(
            showHome: false,
            showAudio: true,
            showGraphics: false);

        SetStatus(
            "Audio controls arrive in Milestone 1F.");

        SelectObject(
            audioFirstSelection);
    }

    public void ShowGraphicsPanel()
    {
        activePanel =
            SettingsPanel.Graphics;

        SetPanelVisibility(
            showHome: false,
            showAudio: false,
            showGraphics: true);

        graphicsSettingsController
            ?.RefreshFromCurrentSettings();

        SelectObject(
            graphicsFirstSelection);
    }

    public void ResetDefaults()
    {
        if (settingsService == null)
        {
            SetStatus(
                "Settings services are unavailable.");

            return;
        }

        bool resetSucceeded =
            settingsService.ResetToDefaults(
                saveImmediately: true);

        SetStatus(
            resetSucceeded
                ? "All settings reset to defaults."
                : "Settings could not be reset.");
    }

    private void HandleBackRequest()
    {
        if (activePanel ==
            SettingsPanel.Home)
        {
            CloseMenu();
            return;
        }

        ShowHomePanel();
    }

    private void SetPanelVisibility(
        bool showHome,
        bool showAudio,
        bool showGraphics)
    {
        if (settingsHomePanel != null)
        {
            settingsHomePanel.SetActive(
                showHome);
        }

        if (audioSettingsPanel != null)
        {
            audioSettingsPanel.SetActive(
                showAudio);
        }

        if (graphicsSettingsPanel != null)
        {
            graphicsSettingsPanel.SetActive(
                showGraphics);
        }
    }

    private void AddButtonListeners()
    {
        AddListener(
            audioSettingsButton,
            ShowAudioPanel);

        AddListener(
            graphicsSettingsButton,
            ShowGraphicsPanel);

        AddListener(
            resetDefaultsButton,
            ResetDefaults);

        AddListener(
            closeSettingsButton,
            CloseMenu);

        AddListener(
            audioBackButton,
            ShowHomePanel);

        AddListener(
            graphicsBackButton,
            ShowHomePanel);
    }

    private void RemoveButtonListeners()
    {
        RemoveListener(
            audioSettingsButton,
            ShowAudioPanel);

        RemoveListener(
            graphicsSettingsButton,
            ShowGraphicsPanel);

        RemoveListener(
            resetDefaultsButton,
            ResetDefaults);

        RemoveListener(
            closeSettingsButton,
            CloseMenu);

        RemoveListener(
            audioBackButton,
            ShowHomePanel);

        RemoveListener(
            graphicsBackButton,
            ShowHomePanel);
    }

    private static void AddListener(
        Button button,
        UnityAction action)
    {
        if (button != null)
        {
            button.onClick.AddListener(
                action);
        }
    }

    private static void RemoveListener(
        Button button,
        UnityAction action)
    {
        if (button != null)
        {
            button.onClick.RemoveListener(
                action);
        }
    }

    private void BuildInputActions()
    {
        cancelAction =
            new InputAction(
                "Settings Cancel",
                InputActionType.Button);

        cancelAction.AddBinding(
            "<Keyboard>/escape");

        cancelAction.AddBinding(
            "<Gamepad>/buttonEast");
    }

    private void SelectObject(
        GameObject selection)
    {
        if (EventSystem.current == null)
            return;

        EventSystem.current.SetSelectedGameObject(
            null);

        if (selection != null &&
            selection.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(
                selection);
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
        cancelAction?.Disable();
    }

    private void OnDestroy()
    {
        RemoveButtonListeners();

        cancelAction?.Disable();
        cancelAction?.Dispose();
    }
}

//----- SettingsMenuController.cs END -----