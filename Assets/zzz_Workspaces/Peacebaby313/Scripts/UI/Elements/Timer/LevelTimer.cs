//----- LevelTimer.cs START -----

using System;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public sealed class LevelTimer : MonoBehaviour
{
    public event Action<float> OnTimeChanged;
    public event Action OnTimeExpired;

    [Header("Configuration")]
    [SerializeField]
    private LevelConfigurationData levelConfiguration;

    [Header("Timer State")]
    [SerializeField]
    private bool startAutomatically = true;

    [Header("Events")]
    [SerializeField]
    private UnityEvent onTimeExpired;

    public float TimeRemaining { get; private set; }

    public float TimeLimit =>
        levelConfiguration != null
            ? levelConfiguration.LevelTimeLimit
            : 0f;

    public bool IsRunning { get; private set; }

    public bool HasExpired { get; private set; }

    private void Awake()
    {
        ResetTimer();
    }

    private void Start()
    {
        if (startAutomatically)
        {
            StartTimer();
        }
    }

    private void Update()
    {
        if (!IsRunning || HasExpired)
        {
            return;
        }

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining <= 0f)
        {
            TimeRemaining = 0f;

            OnTimeChanged?.Invoke(
                TimeRemaining);

            ExpireTimer();
            return;
        }

        OnTimeChanged?.Invoke(
            TimeRemaining);
    }

    public void StartTimer()
    {
        if (HasExpired || TimeRemaining <= 0f)
        {
            return;
        }

        IsRunning = true;

        OnTimeChanged?.Invoke(
            TimeRemaining);
    }

    public void StopTimer()
    {
        IsRunning = false;
    }

    public void ResetTimer()
    {
        IsRunning = false;
        HasExpired = false;

        TimeRemaining = TimeLimit;

        OnTimeChanged?.Invoke(
            TimeRemaining);
    }

    private void ExpireTimer()
    {
        if (HasExpired)
        {
            return;
        }

        HasExpired = true;
        IsRunning = false;

        Debug.Log(
            "[LEVEL TIMER] The mission timer expired.",
            this);

        OnTimeExpired?.Invoke();
        onTimeExpired?.Invoke();
    }

    private void OnValidate()
    {
        if (levelConfiguration == null)
        {
            Debug.LogWarning(
                "[LEVEL TIMER] Assign a LevelConfigurationData asset.",
                this);
        }
    }
}

//----- LevelTimer.cs END -----