//----- LevelTimerHUD.cs START -----

using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class LevelTimerHUD : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private LevelTimer levelTimer;

    [SerializeField]
    private TMP_Text timerText;

    [Header("Display")]
    [SerializeField]
    private string timerPrefix = "TIME ";

    private void OnEnable()
    {
        if (levelTimer != null)
        {
            levelTimer.OnTimeChanged +=
                HandleTimeChanged;

            RefreshDisplay();
        }
    }

    private void OnDisable()
    {
        if (levelTimer != null)
        {
            levelTimer.OnTimeChanged -=
                HandleTimeChanged;
        }
    }

    private void HandleTimeChanged(
        float timeRemaining)
    {
        DisplayTime(
            timeRemaining);
    }

    private void RefreshDisplay()
    {
        if (levelTimer == null)
        {
            DisplayTime(0f);
            return;
        }

        DisplayTime(
            levelTimer.TimeRemaining);
    }

    private void DisplayTime(
        float timeRemaining)
    {
        if (timerText == null)
        {
            return;
        }

        int totalSeconds =
            Mathf.CeilToInt(
                Mathf.Max(0f, timeRemaining));

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;

        timerText.text =
            $"{timerPrefix}{minutes:00}:{seconds:00}";
    }

    private void OnValidate()
    {
        if (timerText == null)
        {
            Debug.LogWarning(
                "[LEVEL TIMER HUD] Assign a TMP_Text component.",
                this);
        }
    }
}

//----- LevelTimerHUD.cs END -----