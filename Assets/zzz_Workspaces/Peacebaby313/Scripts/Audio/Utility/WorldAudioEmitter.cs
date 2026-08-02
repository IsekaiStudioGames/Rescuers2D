//----- WorldAudioEmitter.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class WorldAudioEmitter : MonoBehaviour
{
    private const float MinimumPitch = 0.01f;
    private const float MaximumPitch = 3f;

    [Header("World Audio")]
    [Tooltip(
        "Default position used by detached world sounds. " +
        "Defaults to this GameObject.")]
    [SerializeField]
    private Transform audioOrigin;

    [Tooltip(
        "Optional direct reference for isolated testing. " +
        "Normal gameplay resolves the persistent bootstrap SfxPlayer.")]
    [SerializeField]
    private SfxPlayer sfxPlayer;

    [Header("Emitter Tuning")]
    [SerializeField, Range(0f, 2f)]
    private float volumeMultiplier = 1f;

    [SerializeField, Range(MinimumPitch, MaximumPitch)]
    private float pitchMultiplier = 1f;

    [Header("Diagnostics")]
    [SerializeField]
    private bool logMissingPlayer = true;

    private bool hasLoggedMissingPlayer;

    public Transform AudioOrigin =>
        audioOrigin != null
            ? audioOrigin
            : transform;

    public float VolumeMultiplier =>
        volumeMultiplier;

    public float PitchMultiplier =>
        pitchMultiplier;

    private void Reset()
    {
        audioOrigin =
            transform;
    }

    private void Awake()
    {
        if (audioOrigin == null)
        {
            audioOrigin =
                transform;
        }

        TryResolveSfxPlayer();
    }

    public SfxPlaybackHandle PlayAtOrigin(
        SfxCueData cue)
    {
        return PlayAtPosition(
            cue,
            AudioOrigin.position);
    }

    public SfxPlaybackHandle PlayAtPosition(
        SfxCueData cue,
        Vector3 position)
    {
        if (cue == null ||
            !TryResolveSfxPlayer())
        {
            return SfxPlaybackHandle.Invalid;
        }

        return sfxPlayer.PlayAtPosition(
            cue,
            position,
            volumeMultiplier,
            pitchMultiplier);
    }

    public SfxPlaybackHandle PlayAttached(
        SfxCueData cue)
    {
        return PlayAttached(
            cue,
            AudioOrigin);
    }

    public SfxPlaybackHandle PlayAttached(
        SfxCueData cue,
        Transform target)
    {
        if (cue == null ||
            target == null ||
            !TryResolveSfxPlayer())
        {
            return SfxPlaybackHandle.Invalid;
        }

        return sfxPlayer.PlayAttached(
            cue,
            target,
            volumeMultiplier,
            pitchMultiplier);
    }

    private bool TryResolveSfxPlayer()
    {
        if (sfxPlayer != null)
        {
            return true;
        }

        ApplicationBootstrap bootstrap =
            ApplicationBootstrap.Instance;

        if (bootstrap != null)
        {
            sfxPlayer =
                bootstrap.SfxPlayer;
        }

        if (sfxPlayer != null)
        {
            hasLoggedMissingPlayer =
                false;

            return true;
        }

        if (logMissingPlayer &&
            !hasLoggedMissingPlayer)
        {
            Debug.LogWarning(
                $"[WORLD AUDIO] '{name}' could not resolve " +
                "the persistent SfxPlayer.",
                this);

            hasLoggedMissingPlayer =
                true;
        }

        return false;
    }

    private void OnValidate()
    {
        volumeMultiplier =
            Mathf.Max(
                0f,
                volumeMultiplier);

        pitchMultiplier =
            Mathf.Clamp(
                pitchMultiplier,
                MinimumPitch,
                MaximumPitch);
    }
}

//----- WorldAudioEmitter.cs END -----