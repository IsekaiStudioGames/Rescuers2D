//----- LevelLoopController.cs START -----

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public sealed class LevelLoopController : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField, Min(1f)]
    private float levelTimeLimit = 300f;

    [SerializeField]
    private string nextSceneName;

    [SerializeField]
    private string nextLevelCode = "RESCUE-002";

    [SerializeField, Min(0f)]
    private float winScreenDuration = 30f;

    [Header("Player Feedback")]
    [SerializeField]
    private GameObject playerFeedbackPanel;

    [SerializeField]
    private TMP_Text playerFeedbackText;

    [TextArea(3, 8)]
    [SerializeField]
    private string levelStartMessage =
        "Find the survivor and escort them safely to the tent.";

    [Tooltip("Use 0 to leave the message visible.")]
    [SerializeField, Min(0f)]
    private float feedbackDisplayDuration = 5f;

    [Header("Timer UI")]
    [SerializeField]
    private TMP_Text timerText;

    [Header("Win Menu")]
    [SerializeField]
    private GameObject winMenu;

    [SerializeField]
    private TMP_Text winMessageText;

    [SerializeField]
    private TMP_Text levelCodeText;

    [SerializeField]
    private TMP_Text nextLevelCountdownText;

    [TextArea(2, 5)]
    [SerializeField]
    private string winMessage =
        "Survivor rescued! Preparing the next mission.";

    [Header("Lose Menu")]
    [SerializeField]
    private GameObject loseMenu;

    [SerializeField]
    private TMP_Text loseMessageText;

    [TextArea(2, 5)]
    [SerializeField]
    private string loseMessage =
        "Time has expired. The rescue mission has failed.";

    private float remainingTime;
    private bool levelEnded;
    private Coroutine feedbackRoutine;
    private Coroutine winRoutine;

    public bool LevelEnded => levelEnded;

    private void Awake()
    {
        remainingTime = levelTimeLimit;

        // Only the end-state menus begin hidden.
        SetPanelActive(winMenu, false);
        SetPanelActive(loseMenu, false);
    }

    private void Start()
    {
        ShowStartingFeedback();
        UpdateTimerDisplay();
    }

    private void Update()
    {
        if (levelEnded)
        {
            return;
        }

        remainingTime -= Time.deltaTime;

        if (remainingTime <= 0f)
        {
            remainingTime = 0f;
            UpdateTimerDisplay();
            TriggerLose();
            return;
        }

        UpdateTimerDisplay();
    }

    public void TriggerWin()
    {
        if (levelEnded)
        {
            return;
        }

        levelEnded = true;

        HidePlayerFeedback();
        SetPanelActive(loseMenu, false);
        SetPanelActive(winMenu, true);

        if (winMessageText != null)
        {
            winMessageText.text = winMessage;
        }

        if (levelCodeText != null)
        {
            levelCodeText.text =
                string.IsNullOrWhiteSpace(nextLevelCode)
                    ? string.Empty
                    : $"Next Level Code: {nextLevelCode}";
        }

        if (winRoutine != null)
        {
            StopCoroutine(winRoutine);
        }

        winRoutine = StartCoroutine(WinCountdownRoutine());
    }

    public void TriggerLose()
    {
        if (levelEnded)
        {
            return;
        }

        levelEnded = true;

        HidePlayerFeedback();
        SetPanelActive(winMenu, false);
        SetPanelActive(loseMenu, true);

        if (loseMessageText != null)
        {
            loseMessageText.text = loseMessage;
        }
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void LoadNextLevelImmediately()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning(
                "Next Scene Name has not been assigned.",
                this);

            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError(
                $"Scene '{nextSceneName}' is not available. " +
                "Add it to the Build Settings.",
                this);

            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene(nextSceneName);
    }

    public void ShowStartingFeedback()
    {
        if (playerFeedbackPanel == null)
        {
            Debug.LogError(
                "Player Feedback Panel is not assigned.",
                this);

            return;
        }

        if (playerFeedbackText != null)
        {
            playerFeedbackText.text = levelStartMessage;
        }

        SetPanelActive(playerFeedbackPanel, true);

        if (!playerFeedbackPanel.activeInHierarchy)
        {
            Debug.LogError(
                $"Feedback panel '{playerFeedbackPanel.name}' was enabled, " +
                "but one of its parent objects is inactive.",
                playerFeedbackPanel);

            return;
        }

        CanvasGroup canvasGroup =
            playerFeedbackPanel.GetComponent<CanvasGroup>();

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        if (feedbackDisplayDuration > 0f)
        {
            feedbackRoutine =
                StartCoroutine(FeedbackRoutine());
        }

        Debug.Log(
            $"Opening feedback panel '{playerFeedbackPanel.name}'.",
            this);
    }

    public void HidePlayerFeedback()
    {
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
            feedbackRoutine = null;
        }

        SetPanelActive(playerFeedbackPanel, false);
    }

    private IEnumerator FeedbackRoutine()
    {
        yield return new WaitForSeconds(feedbackDisplayDuration);

        feedbackRoutine = null;
        SetPanelActive(playerFeedbackPanel, false);
    }

    private IEnumerator WinCountdownRoutine()
    {
        float countdown = winScreenDuration;

        while (countdown > 0f)
        {
            if (nextLevelCountdownText != null)
            {
                nextLevelCountdownText.text =
                    $"Next mission begins in " +
                    $"{Mathf.CeilToInt(countdown)} seconds";
            }

            countdown -= Time.deltaTime;
            yield return null;
        }

        if (nextLevelCountdownText != null)
        {
            nextLevelCountdownText.text =
                "Loading next mission...";
        }

        winRoutine = null;
        LoadNextLevelImmediately();
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
        {
            return;
        }

        int totalSeconds =
            Mathf.Max(0, Mathf.CeilToInt(remainingTime));

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    private static void SetPanelActive(
        GameObject panel,
        bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    private void OnValidate()
    {
        levelTimeLimit =
            Mathf.Max(1f, levelTimeLimit);

        winScreenDuration =
            Mathf.Max(0f, winScreenDuration);

        feedbackDisplayDuration =
            Mathf.Max(0f, feedbackDisplayDuration);
    }
}

//----- LevelLoopController.cs END -----