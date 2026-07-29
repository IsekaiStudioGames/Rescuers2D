//----- LevelBriefingController.cs START -----

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

[Serializable]
public sealed class RescuerBriefingUI
{
    [SerializeField]
    private Image characterPortrait;

    [SerializeField]
    private TMP_Text characterNameText;

    [SerializeField]
    private TMP_Text abilityDescriptionText;

    public void Apply(RescuerBriefingEntry entry)
    {
        bool hasEntry = entry != null;

        if (characterPortrait != null)
        {
            characterPortrait.sprite =
                hasEntry ? entry.CharacterPortrait : null;

            characterPortrait.enabled =
                hasEntry && entry.CharacterPortrait != null;
        }

        if (characterNameText != null)
        {
            characterNameText.text =
                hasEntry ? entry.CharacterName : string.Empty;
        }

        if (abilityDescriptionText != null)
        {
            abilityDescriptionText.text =
                hasEntry
                    ? entry.AbilityDescription
                    : string.Empty;
        }
    }
}

[DisallowMultipleComponent]
public sealed class LevelBriefingController : MonoBehaviour
{
    public event Action OnBriefingStarted;
    public event Action OnBriefingClosed;

    [Header("Level Data")]
    [SerializeField]
    private LevelConfigurationData levelConfiguration;

    [Header("Menu Controller")]
    [SerializeField]
    private FeedbackMenuController feedbackMenuController;

    [Header("Briefing Text")]
    [SerializeField]
    private TMP_Text welcomeText;

    [SerializeField]
    private TMP_Text levelNumberText;

    [SerializeField]
    private TMP_Text levelNameText;

    [SerializeField]
    private TMP_Text levelGoalText;

    [SerializeField]
    private TMP_Text briefingCountdownText;

    [SerializeField]
    private TMP_Text levelOverviewText;

    [SerializeField]
    private TMP_Text currentLevelPasswordText;

    [SerializeField]
    private TMP_Text continuePromptText;

    [Header("Rescuer Panels")]
    [SerializeField]
    private RescuerBriefingUI[] rescuerPanels =
        new RescuerBriefingUI[3];

    [Header("Briefing Behavior")]
    [SerializeField]
    private bool showOnStart = true;

    [SerializeField]
    private bool allowAnyButtonSkip = true;

    [SerializeField]
    private bool pauseTimeDuringBriefing = true;

    [Header("Optional Gameplay Behaviours")]
    [Tooltip(
        "Optional gameplay components to disable during the briefing. " +
        "Do not add this controller or the FeedbackMenuController.")]
    [SerializeField]
    private Behaviour[] behavioursDisabledDuringBriefing;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onBriefingStarted;

    [SerializeField]
    private UnityEvent onBriefingClosed;

    public bool IsBriefingActive { get; private set; }
    public float RemainingTime { get; private set; }

    private bool waitingForInputRelease;
    private bool briefingHasClosed;
    private float previousTimeScale = 1f;
    private bool[] previousBehaviourStates;

    private void Start()
    {
        if (showOnStart)
        {
            BeginBriefing();
        }
    }

    private void Update()
    {
        if (!IsBriefingActive)
        {
            return;
        }

        RemainingTime -= Time.unscaledDeltaTime;
        RemainingTime = Mathf.Max(0f, RemainingTime);

        UpdateCountdownText();

        if (RemainingTime <= 0f)
        {
            CloseBriefing();
            return;
        }

        if (!allowAnyButtonSkip)
        {
            return;
        }

        if (waitingForInputRelease)
        {
            if (!IsAnySupportedButtonPressed())
            {
                waitingForInputRelease = false;
            }

            return;
        }

        if (WasAnySupportedButtonPressedThisFrame())
        {
            CloseBriefing();
        }
    }

    [ContextMenu("Begin Briefing")]
    public void BeginBriefing()
    {
        if (levelConfiguration == null)
        {
            Debug.LogError(
                "[LEVEL BRIEFING] No LevelConfigurationData " +
                "has been assigned.",
                this);

            return;
        }

        if (feedbackMenuController == null)
        {
            Debug.LogError(
                "[LEVEL BRIEFING] No FeedbackMenuController " +
                "has been assigned.",
                this);

            return;
        }

        if (IsBriefingActive)
        {
            return;
        }

        briefingHasClosed = false;
        IsBriefingActive = true;
        RemainingTime = levelConfiguration.BriefingDuration;

        PopulateBriefing();
        LockGameplay();

        waitingForInputRelease =
            IsAnySupportedButtonPressed();

        feedbackMenuController.ShowPlayerFeedback();

        UpdateCountdownText();

        OnBriefingStarted?.Invoke();
        onBriefingStarted?.Invoke();

        //Debug.Log(
        //    $"[LEVEL BRIEFING] Began briefing for " +
        //    $"{levelConfiguration.LevelName}.",
        //    this);
    }

    [ContextMenu("Close Briefing")]
    public void CloseBriefing()
    {
        if (!IsBriefingActive || briefingHasClosed)
        {
            return;
        }

        briefingHasClosed = true;
        IsBriefingActive = false;
        RemainingTime = 0f;

        feedbackMenuController.HideAll();
        UnlockGameplay();

        OnBriefingClosed?.Invoke();
        onBriefingClosed?.Invoke();

        Debug.Log(
            "[LEVEL BRIEFING] Briefing closed. Gameplay released.",
            this);
    }

    private void PopulateBriefing()
    {
        if (welcomeText != null)
        {
            welcomeText.text =
                "Welcome to Rescuers2D";
        }

        if (levelNumberText != null)
        {
            levelNumberText.text =
                levelConfiguration.LevelNumber;
        }

        if (levelNameText != null)
        {
            levelNameText.text =
                levelConfiguration.LevelName;
        }

        if (levelGoalText != null)
        {
            levelGoalText.text =
                levelConfiguration.LevelGoal;
        }

        if (levelOverviewText != null)
        {
            levelOverviewText.text =
                levelConfiguration.LevelOverview;
        }

        if (currentLevelPasswordText != null)
        {
            currentLevelPasswordText.text =
            //    "Password to return to this level:\n" +
                levelConfiguration.CurrentLevelPassword;
        }

        if (continuePromptText != null)
        {
            continuePromptText.text =
                allowAnyButtonSkip
                    ? "Press any button to continue"
                    : string.Empty;
        }

        for (int i = 0; i < rescuerPanels.Length; i++)
        {
            if (rescuerPanels[i] == null)
            {
                continue;
            }

            levelConfiguration.TryGetRescuer(
                i,
                out RescuerBriefingEntry entry);

            rescuerPanels[i].Apply(entry);
        }
    }

    private void UpdateCountdownText()
    {
        if (briefingCountdownText == null)
        {
            return;
        }

        int totalSeconds =
            Mathf.CeilToInt(RemainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        briefingCountdownText.text =
            $"Closing in {minutes:00}:{seconds:00}";
    }

    private void LockGameplay()
    {
        if (pauseTimeDuringBriefing)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        if (behavioursDisabledDuringBriefing == null)
        {
            return;
        }

        previousBehaviourStates =
            new bool[behavioursDisabledDuringBriefing.Length];

        for (int i = 0;
             i < behavioursDisabledDuringBriefing.Length;
             i++)
        {
            Behaviour behaviour =
                behavioursDisabledDuringBriefing[i];

            if (behaviour == null)
            {
                continue;
            }

            if (behaviour == this ||
                behaviour == feedbackMenuController)
            {
                Debug.LogWarning(
                    $"[LEVEL BRIEFING] {behaviour.name} was not " +
                    "disabled because it is required by the briefing.",
                    behaviour);

                continue;
            }

            previousBehaviourStates[i] = behaviour.enabled;
            behaviour.enabled = false;
        }
    }

    private void UnlockGameplay()
    {
        if (pauseTimeDuringBriefing)
        {
            Time.timeScale = previousTimeScale;
        }

        if (behavioursDisabledDuringBriefing == null ||
            previousBehaviourStates == null)
        {
            return;
        }

        int restoreCount = Mathf.Min(
            behavioursDisabledDuringBriefing.Length,
            previousBehaviourStates.Length);

        for (int i = 0; i < restoreCount; i++)
        {
            Behaviour behaviour =
                behavioursDisabledDuringBriefing[i];

            if (behaviour != null)
            {
                behaviour.enabled =
                    previousBehaviourStates[i];
            }
        }

        previousBehaviourStates = null;
    }

    private static bool IsAnySupportedButtonPressed()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            foreach (InputControl control in device.allControls)
            {
                if (control is ButtonControl button &&
                    button.isPressed)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private static bool WasAnySupportedButtonPressedThisFrame()
    {
        foreach (InputDevice device in InputSystem.devices)
        {
            foreach (InputControl control in device.allControls)
            {
                if (control is ButtonControl button &&
                    button.wasPressedThisFrame)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnDisable()
    {
        if (!IsBriefingActive)
        {
            return;
        }

        IsBriefingActive = false;
        UnlockGameplay();
    }

    private void OnValidate()
    {
        if (rescuerPanels == null ||
            rescuerPanels.Length != 3)
        {
            Debug.LogWarning(
                "[LEVEL BRIEFING] Exactly three rescuer UI " +
                "panels should be configured.",
                this);
        }
    }
}

//----- LevelBriefingController.cs END -----
