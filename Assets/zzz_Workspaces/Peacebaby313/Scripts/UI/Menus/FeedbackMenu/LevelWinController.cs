//----- LevelWinController.cs START -----

using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class LevelWinController : MonoBehaviour
{
    public event Action OnLevelWon;

    [Header("Win Condition")]
    [SerializeField]
    private RescueZone rescueZone;

    [Header("Presentation")]
    [SerializeField]
    private FeedbackMenuController feedbackMenuController;

    [Header("Gameplay Lock")]
    [SerializeField]
    private bool pauseTimeOnWin = true;

    [SerializeField]
    private bool pauseAudioOnWin = true;

    [SerializeField]
    private PlayerInputReader playerInputReader;

    [Tooltip(
        "Optional gameplay components to disable after winning. " +
        "Do not add this controller or the FeedbackMenuController.")]
    [SerializeField]
    private Behaviour[] behavioursDisabledOnWin;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onLevelWon;

    public bool HasWon { get; private set; }

    private float previousTimeScale = 1f;
    private bool previousAudioPause;

    private void OnEnable()
    {
        if (rescueZone != null)
        {
            rescueZone.OnAllTargetsSafe +=
                HandleAllTargetsSafe;
        }
    }

    private void Start()
    {
        if (rescueZone == null)
        {
            Debug.LogError(
                "[LEVEL WIN] No RescueZone has been assigned.",
                this);
        }

        if (feedbackMenuController == null)
        {
            Debug.LogError(
                "[LEVEL WIN] No FeedbackMenuController has " +
                "been assigned.",
                this);
        }

        if (playerInputReader == null)
        {
            Debug.LogWarning(
                "[LEVEL WIN] No PlayerInputReader has been assigned. " +
                "Gameplay input will remain enabled after winning.",
                this);
        }
    }

    private void OnDisable()
    {
        if (rescueZone != null)
        {
            rescueZone.OnAllTargetsSafe -=
                HandleAllTargetsSafe;
        }
    }

    private void HandleAllTargetsSafe()
    {
        TriggerWin();
    }

    [ContextMenu("Trigger Win")]
    public void TriggerWin()
    {
        if (HasWon)
        {
            return;
        }

        if (feedbackMenuController == null)
        {
            Debug.LogError(
                "[LEVEL WIN] Cannot show the Win screen because " +
                "FeedbackMenuController is missing.",
                this);

            return;
        }

        HasWon = true;

        LockGameplay();

        feedbackMenuController.ShowWin();

        OnLevelWon?.Invoke();
        onLevelWon?.Invoke();

        Debug.Log(
            "[LEVEL WIN] Mission complete. All required rescuers " +
            "and civilians reached safety.",
            this);
    }

    private void LockGameplay()
    {
        playerInputReader
            ?.SetGameplayInputEnabled(false);

        if (pauseTimeOnWin)
        {
            previousTimeScale =
                Time.timeScale;

            Time.timeScale =
                0f;
        }

        if (pauseAudioOnWin)
        {
            previousAudioPause =
                AudioListener.pause;

            AudioListener.pause =
                true;
        }

        if (behavioursDisabledOnWin == null)
        {
            return;
        }

        foreach (Behaviour behaviour in behavioursDisabledOnWin)
        {
            if (behaviour == null)
            {
                continue;
            }

            if (behaviour == this ||
                behaviour == feedbackMenuController)
            {
                Debug.LogWarning(
                    $"[LEVEL WIN] {behaviour.name} was not disabled " +
                    "because the Win state requires it.",
                    behaviour);

                continue;
            }

            behaviour.enabled =
                false;
        }
    }

    private void RestoreRuntimeState()
    {
        if (!HasWon)
        {
            return;
        }

        if (pauseTimeOnWin)
        {
            Time.timeScale =
                previousTimeScale;
        }

        if (pauseAudioOnWin)
        {
            AudioListener.pause =
                previousAudioPause;
        }
    }

    private void OnDestroy()
    {
        RestoreRuntimeState();
    }

    private void OnValidate()
    {
        if (rescueZone == null)
        {
            Debug.LogWarning(
                "[LEVEL WIN] Assign the scene's RescueZone.",
                this);
        }

        if (feedbackMenuController == null)
        {
            Debug.LogWarning(
                "[LEVEL WIN] Assign the FeedbackMenuController.",
                this);
        }
    }
}

//----- LevelWinController.cs END -----