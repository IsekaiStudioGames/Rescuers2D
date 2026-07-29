//----- SfxPlayerTester.cs START -----

using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class SfxPlayerTester : MonoBehaviour
{
    [Header("Test Cues")]
    [SerializeField]
    private SfxCueData twoDimensionalCue;

    [SerializeField]
    private SfxCueData positionalCue;

    [SerializeField]
    private SfxCueData attachedCue;

    [SerializeField]
    private SfxCueData stressCue;

    [Header("World Test")]
    [SerializeField]
    private Vector3 positionalOffset =
        new Vector3(5f, 0f, 0f);

    [SerializeField]
    private Transform attachmentTarget;

    [Min(0f)]
    [SerializeField]
    private float attachmentMoveSpeed = 4f;

    [Header("Stress Test")]
    [Min(1)]
    [SerializeField]
    private int burstCount = 40;

    [Min(0f)]
    [SerializeField]
    private float burstRadius = 3f;

    private SfxPlaybackHandle lastHandle =
        SfxPlaybackHandle.Invalid;

    private SfxPlayer Player
    {
        get
        {
            if (ApplicationBootstrap.Instance == null)
                return null;

            return ApplicationBootstrap.Instance.SfxPlayer;
        }
    }

    private void Update()
    {
        Keyboard keyboard =
            Keyboard.current;

        if (keyboard == null)
            return;

        MoveAttachmentTarget(keyboard);

        if (keyboard.digit1Key.wasPressedThisFrame)
            PlayTwoDimensional();

        if (keyboard.digit2Key.wasPressedThisFrame)
            PlayAtPosition();

        if (keyboard.digit3Key.wasPressedThisFrame)
            PlayAttached();

        if (keyboard.rKey.wasPressedThisFrame)
            RunStressBurst();

        if (keyboard.sKey.wasPressedThisFrame)
            StopLastVoice();

        if (keyboard.cKey.wasPressedThisFrame)
            StopTwoDimensionalCue();

        if (keyboard.aKey.wasPressedThisFrame)
            StopAllVoices();

        if (keyboard.gKey.wasPressedThisFrame)
            LogPoolState();

        if (keyboard.deleteKey.wasPressedThisFrame)
            DestroyAttachmentTarget();
    }

    public void PlayTwoDimensional()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        lastHandle =
            player.Play(
                twoDimensionalCue);
    }

    public void PlayAtPosition()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        lastHandle =
            player.PlayAtPosition(
                positionalCue,
                transform.position +
                positionalOffset);
    }

    public void PlayAttached()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        lastHandle =
            player.PlayAttached(
                attachedCue,
                attachmentTarget);
    }

    public void RunStressBurst()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        for (int i = 0;
             i < burstCount;
             i++)
        {
            Vector2 circle =
                Random.insideUnitCircle *
                burstRadius;

            Vector3 position =
                transform.position +
                new Vector3(
                    circle.x,
                    circle.y,
                    0f);

            lastHandle =
                player.PlayAtPosition(
                    stressCue,
                    position);
        }

        LogPoolState();
    }

    public void StopLastVoice()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        bool stopped =
            player.Stop(
                lastHandle);

        Debug.Log(
            $"[SFX TESTER] Stop last handle: {stopped}.",
            this);

        if (stopped)
        {
            lastHandle =
                SfxPlaybackHandle.Invalid;
        }
    }

    public void StopTwoDimensionalCue()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        int stoppedCount =
            player.Stop(
                twoDimensionalCue);

        Debug.Log(
            $"[SFX TESTER] Stopped {stoppedCount} " +
            "voice(s) for the 2D cue.",
            this);
    }

    public void StopAllVoices()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        int stoppedCount =
            player.StopAll();

        Debug.Log(
            $"[SFX TESTER] Stopped {stoppedCount} " +
            "total voice(s).",
            this);
    }

    public void LogPoolState()
    {
        if (!TryGetPlayer(
                out SfxPlayer player))
        {
            return;
        }

        Debug.Log(
            $"[SFX TESTER] Pool: {player.PoolSize}, " +
            $"Active: {player.ActiveVoiceCount}, " +
            $"Available: {player.AvailableVoiceCount}.",
            this);
    }

    public void DestroyAttachmentTarget()
    {
        if (attachmentTarget == null)
        {
            Debug.LogWarning(
                "[SFX TESTER] No attachment target is assigned.",
                this);

            return;
        }

        Destroy(
            attachmentTarget.gameObject);
    }

    private void MoveAttachmentTarget(
        Keyboard keyboard)
    {
        if (attachmentTarget == null)
            return;

        Vector3 direction =
            Vector3.zero;

        if (keyboard.leftArrowKey.isPressed)
            direction.x -= 1f;

        if (keyboard.rightArrowKey.isPressed)
            direction.x += 1f;

        if (keyboard.downArrowKey.isPressed)
            direction.y -= 1f;

        if (keyboard.upArrowKey.isPressed)
            direction.y += 1f;

        if (direction.sqrMagnitude <= 0f)
            return;

        attachmentTarget.position +=
            direction.normalized *
            attachmentMoveSpeed *
            Time.unscaledDeltaTime;
    }

    private bool TryGetPlayer(
        out SfxPlayer player)
    {
        player = Player;

        if (player != null)
            return true;

        Debug.LogWarning(
            "[SFX TESTER] SfxPlayer is unavailable.",
            this);

        return false;
    }
}

//----- SfxPlayerTester.cs END -----