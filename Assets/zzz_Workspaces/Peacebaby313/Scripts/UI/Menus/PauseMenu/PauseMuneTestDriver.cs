//----- PauseGameTestDriver.cs START -----

using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-500)]
public sealed class PauseGameTestDriver
    : MonoBehaviour,
      IPauseGameAuthority
{
    public event Action<bool>
        PauseStateChanged;

    [Header("Temporary Pause Input")]
    [SerializeField]
    private bool enableKeyboardP = true;

    [SerializeField]
    private bool enableGamepadStart = true;

    [Header("Temporary Pause Behavior")]
    [SerializeField]
    private bool pauseAudioListener = true;

    [SerializeField]
    private bool pauseOnStart;

    public bool IsPaused
    {
        get;
        private set;
    }

    private InputAction pauseAction;

    private float resumeTimeScale = 1f;
    private bool previousAudioListenerPause;

    private void Awake()
    {
        resumeTimeScale =
            Time.timeScale > 0f
                ? Time.timeScale
                : 1f;

        previousAudioListenerPause =
            AudioListener.pause;

        BuildPauseAction();
    }

    private void OnEnable()
    {
        pauseAction?.Enable();
    }

    private void Start()
    {
        if (pauseOnStart)
        {
            SetPaused(
                true);
        }
    }

    private void Update()
    {
        if (pauseAction == null)
            return;

        if (pauseAction.WasPressedThisFrame())
        {
            SetPaused(
                !IsPaused);
        }
    }

    public void SetPaused(
        bool paused)
    {
        if (IsPaused == paused)
            return;

        IsPaused =
            paused;

        Time.timeScale =
            paused
                ? 0f
                : resumeTimeScale;

        if (pauseAudioListener)
        {
            AudioListener.pause =
                paused
                    ? true
                    : previousAudioListenerPause;
        }

        Debug.Log(
            paused
                ? "[PAUSE TEST] Game paused."
                : "[PAUSE TEST] Game resumed.");

        PauseStateChanged?.Invoke(
            IsPaused);
    }

    [ContextMenu("Debug/Toggle Pause")]
    private void DebugTogglePause()
    {
        SetPaused(
            !IsPaused);
    }

    [ContextMenu("Debug/Pause")]
    private void DebugPause()
    {
        SetPaused(
            true);
    }

    [ContextMenu("Debug/Resume")]
    private void DebugResume()
    {
        SetPaused(
            false);
    }

    private void BuildPauseAction()
    {
        pauseAction =
            new InputAction(
                "Temporary Pause",
                InputActionType.Button);

        if (enableKeyboardP)
        {
            pauseAction.AddBinding(
                "<Keyboard>/p");
        }

        if (enableGamepadStart)
        {
            pauseAction.AddBinding(
                "<Gamepad>/start");
        }
    }

    private void RestoreRuntimeState()
    {
        if (!IsPaused)
            return;

        IsPaused =
            false;

        Time.timeScale =
            resumeTimeScale;

        if (pauseAudioListener)
        {
            AudioListener.pause =
                previousAudioListenerPause;
        }

        PauseStateChanged?.Invoke(
            false);
    }

    private void OnDisable()
    {
        pauseAction?.Disable();

        RestoreRuntimeState();
    }

    private void OnDestroy()
    {
        pauseAction?.Dispose();
    }
}

//----- PauseGameTestDriver.cs END -----