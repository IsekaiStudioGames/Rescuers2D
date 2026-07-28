//----- MusicTrackData.cs START -----

using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName = "MusicTrack_",
    menuName = "Rescuers2D/Audio/Music Track")]
public sealed class MusicTrackData : ScriptableObject
{
    private const float MinimumPlaybackSpeed = 0.25f;
    private const float MaximumPlaybackSpeed = 3f;

    [Header("Identity")]
    [Tooltip(
        "Permanent unique identifier used by tools and optional lookups.")]
    [SerializeField]
    private string trackId;

    [Tooltip(
        "Player-facing or debug-facing track name.")]
    [SerializeField]
    private string displayName;

    [Header("Audio")]
    [SerializeField]
    private AudioClip clip;

    [Tooltip(
        "Individual track volume before Music mixer volume is applied.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float volume = 1f;

    [Tooltip(
        "AudioSource pitch value. This changes both playback speed and pitch.")]
    [Range(MinimumPlaybackSpeed, MaximumPlaybackSpeed)]
    [SerializeField]
    private float playbackSpeed = 1f;

    [Tooltip(
        "Requests reverse playback. Audio B will validate platform support.")]
    [SerializeField]
    private bool playInReverse;

    [SerializeField]
    private bool loop = true;

    [Header("Transitions")]
    [Min(0f)]
    [SerializeField]
    private float fadeInDuration = 1f;

    [Min(0f)]
    [SerializeField]
    private float fadeOutDuration = 1f;

    [Min(0f)]
    [SerializeField]
    private float startDelay;

    [Header("Routing")]
    [SerializeField]
    private AudioMixerGroup mixerGroup;

    public string TrackId => trackId;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? name
            : displayName;

    public AudioClip Clip => clip;
    public float Volume => volume;
    public float PlaybackSpeed => playbackSpeed;
    public bool PlayInReverse => playInReverse;
    public bool Loop => loop;
    public float FadeInDuration => fadeInDuration;
    public float FadeOutDuration => fadeOutDuration;
    public float StartDelay => startDelay;
    public AudioMixerGroup MixerGroup => mixerGroup;

    public bool IsPlayable =>
        !string.IsNullOrWhiteSpace(trackId) &&
        clip != null;

    private void OnValidate()
    {
        trackId =
            trackId?.Trim();

        displayName =
            displayName?.Trim();

        volume =
            Mathf.Clamp01(volume);

        playbackSpeed =
            Mathf.Clamp(
                playbackSpeed,
                MinimumPlaybackSpeed,
                MaximumPlaybackSpeed);

        fadeInDuration =
            Mathf.Max(
                0f,
                fadeInDuration);

        fadeOutDuration =
            Mathf.Max(
                0f,
                fadeOutDuration);

        startDelay =
            Mathf.Max(
                0f,
                startDelay);

        if (string.IsNullOrWhiteSpace(trackId))
        {
            Debug.LogWarning(
                $"[MUSIC TRACK] {name} has no Track ID.",
                this);
        }

        if (clip == null)
        {
            Debug.LogWarning(
                $"[MUSIC TRACK] {name} has no AudioClip.",
                this);
        }

        if (mixerGroup == null)
        {
            Debug.LogWarning(
                $"[MUSIC TRACK] {name} has no mixer group. " +
                "Audio will use the AudioSource's current output.",
                this);
        }
    }
}

//----- MusicTrackData.cs END -----