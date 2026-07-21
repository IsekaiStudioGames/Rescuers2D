//----- MainMenuController.cs START -----

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class MainMenuController : MonoBehaviour
{
    [Header("Scene Configuration")]
    [SerializeField]
    private string firstLevelSceneName =
        "R2D-UR-01_collapsed_block";

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    [Header("Optional Feedback")]
    [SerializeField] private TMP_Text statusText;

    [Header("Initialization")]
    [SerializeField, Min(0.1f)]
    private float bootstrapWaitTimeout = 5f;

    private ApplicationBootstrap bootstrap;
    private bool menuReady;

    private void Awake()
    {
        AddButtonListeners();
        SetButtonsInteractable(false);
    }

    private IEnumerator Start()
    {
        yield return WaitForBootstrap();

        if (bootstrap == null)
        {
            SetStatus(
                "Startup services could not be initialized.");

            Debug.LogError(
                "[MAIN MENU] ApplicationBootstrap was not found.");

            yield break;
        }

        menuReady = true;

        RefreshMenuState();
    }

    private IEnumerator WaitForBootstrap()
    {
        float elapsedTime = 0f;

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

    private void AddButtonListeners()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(
                StartNewGame);
        }

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(
                ContinueGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(
                QuitGame);
        }
    }

    private void RemoveButtonListeners()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.RemoveListener(
                StartNewGame);
        }

        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(
                ContinueGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveListener(
                QuitGame);
        }
    }

    public void StartNewGame()
    {
        if (!menuReady ||
            bootstrap == null)
        {
            return;
        }

        bool requestAccepted =
            bootstrap.TryStartNewGame(
                firstLevelSceneName);

        if (!requestAccepted)
        {
            RefreshMenuState();
            return;
        }

        menuReady = false;

        SetButtonsInteractable(false);

        SetStatus(
            "Starting new rescue operation...");
    }

    public void ContinueGame()
    {
        if (!menuReady ||
            bootstrap == null)
        {
            return;
        }

        bool requestAccepted =
            bootstrap.TryContinueGame();

        if (!requestAccepted)
        {
            RefreshMenuState();
            return;
        }

        menuReady = false;

        SetButtonsInteractable(false);

        SetStatus(
            "Continuing rescue operation...");
    }

    public void QuitGame()
    {
        SetButtonsInteractable(false);

        SetStatus(
            "Exiting Rescuers2D...");

#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void RefreshMenuState()
    {
        if (bootstrap == null ||
            !bootstrap.IsInitialized)
        {
            SetButtonsInteractable(false);

            SetStatus(
                "Initializing...");

            return;
        }

        bool firstSceneAvailable =
            !string.IsNullOrWhiteSpace(
                firstLevelSceneName) &&
            Application.CanStreamedLevelBeLoaded(
                firstLevelSceneName);

        if (newGameButton != null)
        {
            newGameButton.interactable =
                firstSceneAvailable;
        }

        if (continueButton != null)
        {
            continueButton.interactable =
                bootstrap.CanContinue;
        }

        if (quitButton != null)
        {
            quitButton.interactable = true;
        }

        if (!firstSceneAvailable)
        {
            SetStatus(
                $"First mission '{firstLevelSceneName}' " +
                "is not enabled in the Build Profile.");

            return;
        }

        if (bootstrap.CanContinue)
        {
            string lastScene =
                bootstrap
                    .SaveService
                    .CurrentData
                    .LastSceneName;

            SetStatus(
                $"Continue available: {lastScene}");

            return;
        }

        SetStatus(
            "No previous rescue operation found.");
    }

    private void SetButtonsInteractable(
        bool interactable)
    {
        if (newGameButton != null)
        {
            newGameButton.interactable =
                interactable;
        }

        if (continueButton != null)
        {
            continueButton.interactable =
                interactable;
        }

        if (quitButton != null)
        {
            quitButton.interactable =
                interactable;
        }
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
            statusText.text = message;
    }

    private void OnDestroy()
    {
        RemoveButtonListeners();
    }
}

//----- MainMenuController.cs END -----