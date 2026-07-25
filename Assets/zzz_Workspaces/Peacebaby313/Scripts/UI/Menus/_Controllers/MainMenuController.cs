//----- MainMenuController.cs START -----

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class MainMenuController
    : MonoBehaviour
{
    [Header("Menu System")]
    [SerializeField]
    private MenuSystemPresenter
        menuSystemPresenter;

    [Header("Buttons")]
    [SerializeField]
    private Button newGameButton;

    [SerializeField]
    private Button passwordButton;

    [SerializeField]
    private Button settingsButton;

    [SerializeField]
    private Button quitButton;

    [Header("Submenus")]
    [SerializeField]
    private PasswordMenuController
        passwordMenuController;

    [SerializeField]
    private SettingsMenuController
        settingsMenuController;

    [Header("Optional Feedback")]
    [SerializeField]
    private TMP_Text statusText;

    [Header("Initialization")]
    [SerializeField, Min(0.1f)]
    private float bootstrapWaitTimeout = 5f;

    private ApplicationBootstrap bootstrap;

    private bool menuReady;
    private bool transitionInProgress;

    private void Awake()
    {
        AddButtonListeners();
        SetButtonsInteractable(
            false);
    }

    private IEnumerator Start()
    {
        yield return
            WaitForBootstrap();

        if (bootstrap == null)
        {
            SetStatus(
                "Startup services could not be initialized.");

            Debug.LogError(
                "[MAIN MENU] ApplicationBootstrap was not found.");

            yield break;
        }

        menuReady =
            true;

        transitionInProgress =
            false;

        RefreshMenuState();
    }

    private IEnumerator WaitForBootstrap()
    {
        float elapsedTime =
            0f;

        while (ApplicationBootstrap.Instance == null &&
               elapsedTime < bootstrapWaitTimeout)
        {
            elapsedTime +=
                Time.unscaledDeltaTime;

            yield return null;
        }

        bootstrap =
            ApplicationBootstrap.Instance;
    }

    public void StartNewGame()
    {
        if (!CanAcceptRequest())
            return;

        bool accepted =
            bootstrap.TryStartNewGame(
                out string feedback);

        SetStatus(
            feedback);

        if (!accepted)
        {
            RefreshMenuState();
            return;
        }

        BeginSceneTransition();
    }

    public void OpenPasswordMenu()
    {
        if (!CanAcceptRequest())
            return;

        if (passwordMenuController == null)
        {
            SetStatus(
                "Password menu is unavailable.");

            return;
        }

        passwordMenuController.OpenMenu();
    }

    public void OpenSettingsMenu()
    {
        if (!CanAcceptRequest())
            return;

        if (settingsMenuController == null)
        {
            SetStatus(
                "Settings menu is unavailable.");

            return;
        }

        settingsMenuController.OpenMenu();
    }

    public void QuitGame()
    {
        if (transitionInProgress)
            return;

        transitionInProgress =
            true;

        SetButtonsInteractable(
            false);

        SetStatus(
            "Exiting Rescuers2D...");

        menuSystemPresenter
            ?.PrepareForSceneTransition();

#if UNITY_EDITOR
        EditorApplication.isPlaying =
            false;
#else
        Application.Quit();
#endif
    }

    public void RefreshMenuState()
    {
        if (bootstrap == null ||
            !bootstrap.IsInitialized)
        {
            SetButtonsInteractable(
                false);

            SetStatus(
                "Initializing...");

            return;
        }

        bool passwordsReady =
            bootstrap.LevelCodes != null &&
            bootstrap.LevelCodes.IsReady;

        if (newGameButton != null)
        {
            newGameButton.interactable =
                bootstrap.CanStartNewGame;
        }

        if (passwordButton != null)
        {
            passwordButton.interactable =
                passwordsReady;
        }

        if (settingsButton != null)
        {
            settingsButton.interactable =
                bootstrap.SettingsService != null &&
                bootstrap.SettingsService.IsInitialized;
        }

        if (quitButton != null)
        {
            quitButton.interactable =
                true;
        }

        if (!passwordsReady)
        {
            SetStatus(
                "Level password data is unavailable.");

            return;
        }

        if (!bootstrap.CanStartNewGame)
        {
            SetStatus(
                "The first level is unavailable in this build.");

            return;
        }

        SetStatus(
            "Start a new game, enter a password, " +
            "or adjust settings.");
    }

    private bool CanAcceptRequest()
    {
        return
            menuReady &&
            !transitionInProgress &&
            bootstrap != null &&
            bootstrap.IsInitialized;
    }

    private void BeginSceneTransition()
    {
        transitionInProgress =
            true;

        menuReady =
            false;

        SetButtonsInteractable(
            false);

        menuSystemPresenter
            ?.PrepareForSceneTransition();
    }

    private void AddButtonListeners()
    {
        AddListener(
            newGameButton,
            StartNewGame);

        AddListener(
            passwordButton,
            OpenPasswordMenu);

        AddListener(
            settingsButton,
            OpenSettingsMenu);

        AddListener(
            quitButton,
            QuitGame);
    }

    private void RemoveButtonListeners()
    {
        RemoveListener(
            newGameButton,
            StartNewGame);

        RemoveListener(
            passwordButton,
            OpenPasswordMenu);

        RemoveListener(
            settingsButton,
            OpenSettingsMenu);

        RemoveListener(
            quitButton,
            QuitGame);
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

    private void SetButtonsInteractable(
        bool interactable)
    {
        if (newGameButton != null)
        {
            newGameButton.interactable =
                interactable;
        }

        if (passwordButton != null)
        {
            passwordButton.interactable =
                interactable;
        }

        if (settingsButton != null)
        {
            settingsButton.interactable =
                interactable;
        }

        if (quitButton != null)
        {
            quitButton.interactable =
                interactable;
        }
    }

    private void SetStatus(
        string message)
    {
        if (statusText != null)
        {
            statusText.text =
                message;
        }
    }

    private void OnDestroy()
    {
        RemoveButtonListeners();
    }
}

//----- MainMenuController.cs END -----