//----- MusicJukebox.cs START -----

using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class MusicJukebox : MonoBehaviour
{
    private const float MinimumPlayablePitch = 0.01f;
    private const float MaximumPlayablePitch = 3f;

    [Header("Reusable Music Sources")]
    [SerializeField]
    private AudioSource sourceA;

    [SerializeField]
    private AudioSource sourceB;

    [Header("Defaults")]
    [Tooltip(
        "Fade duration used when a command does not provide one " +
        "and the current track has no useful transition duration.")]
    [Min(0f)]
    [SerializeField]
    private float defaultFadeDuration = 1f;

    [Tooltip(
        "Prevents Play from restarting the track that is already active.")]
    [SerializeField]
    private bool protectCurrentTrack = true;

    [Header("Debug")]
    [SerializeField]
    private bool logPlaybackCommands;

    private AudioSource activeSource;
    private AudioSource inactiveSource;
    private MusicTrackData currentTrack;
    private Coroutine transitionRoutine;

    private bool isTransitionPaused;
    private bool reverseOverride;
    private bool loopOverride;
    private float speedOverride = 1f;
    private int transitionVersion;

    public event Action<MusicTrackData> OnTrackChanged;
    public event Action OnMusicStopped;

    public MusicTrackData CurrentTrack =>
        currentTrack;

    public AudioSource ActiveSource =>
        activeSource;

    public bool HasCurrentTrack =>
        currentTrack != null &&
        activeSource != null &&
        activeSource.clip != null;

    public bool IsPlaying =>
        HasCurrentTrack &&
        activeSource.isPlaying;

    public bool IsPaused =>
        isTransitionPaused;

    public float PlaybackTime =>
        HasCurrentTrack
            ? activeSource.time
            : 0f;

    public float NormalizedPlaybackPosition
    {
        get
        {
            if (!HasCurrentTrack ||
                activeSource.clip.length <= 0f)
            {
                return 0f;
            }

            return Mathf.Clamp01(
                activeSource.time /
                activeSource.clip.length);
        }
    }

    private void Awake()
    {
        if (!ValidateSources())
        {
            enabled = false;
            return;
        }

        activeSource = sourceA;
        inactiveSource = sourceB;

        PrepareReusableSource(sourceA);
        PrepareReusableSource(sourceB);
    }

    private void OnDisable()
    {
        CancelTransition();
    }

    public void Play(
        MusicTrackData track,
        bool restartIfAlreadyPlaying = false)
    {
        if (!CanPlay(track))
            return;

        if (protectCurrentTrack &&
            !restartIfAlreadyPlaying &&
            currentTrack == track &&
            HasCurrentTrack)
        {
            Log(
                $"Ignored Play for current track " +
                $"'{track.DisplayName}'.");

            return;
        }

        float fadeOutDuration =
            HasCurrentTrack
                ? currentTrack.FadeOutDuration
                : 0f;

        BeginPlayTransition(
            track,
            fadeOutDuration,
            track.FadeInDuration);
    }

    public void Play(
        MusicTrackData track,
        float fadeOutDuration,
        float fadeInDuration,
        bool restartIfAlreadyPlaying = false)
    {
        if (!CanPlay(track))
            return;

        if (protectCurrentTrack &&
            !restartIfAlreadyPlaying &&
            currentTrack == track &&
            HasCurrentTrack)
        {
            Log(
                $"Ignored Play for current track " +
                $"'{track.DisplayName}'.");

            return;
        }

        BeginPlayTransition(
            track,
            Mathf.Max(0f, fadeOutDuration),
            Mathf.Max(0f, fadeInDuration));
    }

    public void Restart()
    {
        if (currentTrack == null)
        {
            Debug.LogWarning(
                "[MUSIC JUKEBOX] Restart requested without a current track.",
                this);

            return;
        }

        Play(
            currentTrack,
            true);
    }

    public void Stop()
    {
        float duration =
            currentTrack != null
                ? currentTrack.FadeOutDuration
                : defaultFadeDuration;

        FadeOut(duration);
    }

    public void StopImmediate()
    {
        CancelTransition();

        StopAndClearSource(sourceA);
        StopAndClearSource(sourceB);

        currentTrack = null;
        isTransitionPaused = false;

        Log("Stopped immediately.");
        OnMusicStopped?.Invoke();
    }

    public void FadeOut(float duration)
    {
        if (!HasCurrentTrack)
        {
            StopImmediate();
            return;
        }

        StartTransition(
            FadeOutRoutine(
                Mathf.Max(0f, duration)));
    }

    public void Pause()
    {
        if (!HasCurrentTrack ||
            isTransitionPaused)
        {
            return;
        }

        isTransitionPaused = true;

        PauseIfActive(sourceA);
        PauseIfActive(sourceB);

        Log("Paused.");
    }

    public void Resume()
    {
        if (!HasCurrentTrack ||
            !isTransitionPaused)
        {
            return;
        }

        isTransitionPaused = false;

        UnPauseIfConfigured(sourceA);
        UnPauseIfConfigured(sourceB);

        Log("Resumed.");
    }

    public void SetPlaybackSpeed(
        float playbackSpeed)
    {
        speedOverride =
            Mathf.Clamp(
                Mathf.Abs(playbackSpeed),
                MinimumPlayablePitch,
                MaximumPlayablePitch);

        ApplyRuntimeOverrides(sourceA);
        ApplyRuntimeOverrides(sourceB);
    }

    public void SetReverse(
        bool playInReverse)
    {
        reverseOverride =
            playInReverse;

        ApplyDirection(sourceA);
        ApplyDirection(sourceB);
    }

    public void SetLoop(
        bool shouldLoop)
    {
        loopOverride =
            shouldLoop;

        if (sourceA != null)
            sourceA.loop = shouldLoop;

        if (sourceB != null)
            sourceB.loop = shouldLoop;
    }

    private void BeginPlayTransition(
        MusicTrackData track,
        float fadeOutDuration,
        float fadeInDuration)
    {
        reverseOverride =
            track.PlayInReverse;

        loopOverride =
            track.Loop;

        speedOverride =
            track.PlaybackSpeed;

        StartTransition(
            CrossfadeRoutine(
                track,
                fadeOutDuration,
                fadeInDuration));
    }

    private IEnumerator CrossfadeRoutine(
        MusicTrackData track,
        float fadeOutDuration,
        float fadeInDuration)
    {
        int version =
            transitionVersion;

        AudioSource outgoing =
            HasCurrentTrack
                ? activeSource
                : null;

        AudioSource incoming =
            inactiveSource;

        ConfigureSource(
            incoming,
            track);

        float delayRemaining =
            track.StartDelay;

        while (delayRemaining > 0f)
        {
            if (version != transitionVersion)
                yield break;

            if (!isTransitionPaused)
            {
                delayRemaining -=
                    Time.unscaledDeltaTime;
            }

            yield return null;
        }

        StartConfiguredSource(incoming);

        activeSource =
            incoming;

        inactiveSource =
            outgoing != null
                ? outgoing
                : incoming == sourceA
                    ? sourceB
                    : sourceA;

        currentTrack =
            track;

        OnTrackChanged?.Invoke(
            track);

        float incomingTargetVolume =
            track.Volume;

        float outgoingStartVolume =
            outgoing != null
                ? outgoing.volume
                : 0f;

        float elapsed = 0f;

        float transitionDuration =
            Mathf.Max(
                fadeOutDuration,
                fadeInDuration);

        if (transitionDuration <= 0f)
        {
            incoming.volume =
                incomingTargetVolume;

            if (outgoing != null)
            {
                StopAndClearSource(
                    outgoing);
            }
        }
        else
        {
            while (elapsed < transitionDuration)
            {
                if (version != transitionVersion)
                    yield break;

                if (isTransitionPaused)
                {
                    yield return null;
                    continue;
                }

                elapsed +=
                    Time.unscaledDeltaTime;

                float fadeInProgress =
                    fadeInDuration <= 0f
                        ? 1f
                        : Mathf.Clamp01(
                            elapsed /
                            fadeInDuration);

                float fadeOutProgress =
                    fadeOutDuration <= 0f
                        ? 1f
                        : Mathf.Clamp01(
                            elapsed /
                            fadeOutDuration);

                incoming.volume =
                    incomingTargetVolume *
                    fadeInProgress;

                if (outgoing != null)
                {
                    outgoing.volume =
                        outgoingStartVolume *
                        (1f - fadeOutProgress);
                }

                yield return null;
            }

            incoming.volume =
                incomingTargetVolume;

            if (outgoing != null)
            {
                StopAndClearSource(
                    outgoing);
            }
        }

        transitionRoutine =
            null;

        Log(
            $"Now playing '{track.DisplayName}'.");
    }

    private IEnumerator FadeOutRoutine(
        float duration)
    {
        int version =
            transitionVersion;

        float sourceAStartVolume =
            sourceA != null
                ? sourceA.volume
                : 0f;

        float sourceBStartVolume =
            sourceB != null
                ? sourceB.volume
                : 0f;

        if (!HasCurrentTrack ||
            duration <= 0f)
        {
            StopImmediate();
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            if (version != transitionVersion)
                yield break;

            if (isTransitionPaused)
            {
                yield return null;
                continue;
            }

            elapsed +=
                Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed /
                    duration);

            if (sourceA != null)
            {
                sourceA.volume =
                    Mathf.Lerp(
                        sourceAStartVolume,
                        0f,
                        progress);
            }

            if (sourceB != null)
            {
                sourceB.volume =
                    Mathf.Lerp(
                        sourceBStartVolume,
                        0f,
                        progress);
            }

            yield return null;
        }

        StopAndClearSource(sourceA);
        StopAndClearSource(sourceB);

        currentTrack = null;
        isTransitionPaused = false;
        transitionRoutine = null;

        Log("Fade out completed.");
        OnMusicStopped?.Invoke();
    }

    private void ConfigureSource(
        AudioSource source,
        MusicTrackData track)
    {
        StopAndClearSource(
            source);

        source.clip =
            track.Clip;

        source.outputAudioMixerGroup =
            track.MixerGroup;

        source.volume =
            0f;

        source.loop =
            loopOverride;

        ApplyRuntimeOverrides(
            source);
    }

    private void StartConfiguredSource(
        AudioSource source)
    {
        if (source == null ||
            source.clip == null)
        {
            return;
        }

        if (reverseOverride)
        {
            source.timeSamples =
                Mathf.Max(
                    0,
                    source.clip.samples - 1);
        }
        else
        {
            source.timeSamples =
                0;
        }

        source.Play();
    }

    private void ApplyRuntimeOverrides(
        AudioSource source)
    {
        if (source == null)
            return;

        source.loop =
            loopOverride;

        ApplyDirection(
            source);
    }

    private void ApplyDirection(
        AudioSource source)
    {
        if (source == null)
            return;

        float direction =
            reverseOverride
                ? -1f
                : 1f;

        source.pitch =
            direction *
            Mathf.Clamp(
                speedOverride,
                MinimumPlayablePitch,
                MaximumPlayablePitch);
    }

    private void StartTransition(
        IEnumerator routine)
    {
        CancelTransition();

        transitionVersion++;

        transitionRoutine =
            StartCoroutine(
                routine);
    }

    private void CancelTransition()
    {
        transitionVersion++;

        if (transitionRoutine != null)
        {
            StopCoroutine(
                transitionRoutine);

            transitionRoutine =
                null;
        }
    }

    private bool CanPlay(
        MusicTrackData track)
    {
        if (!enabled)
            return false;

        if (track == null)
        {
            Debug.LogWarning(
                "[MUSIC JUKEBOX] Cannot play a null track.",
                this);

            return false;
        }

        if (!track.IsPlayable)
        {
            Debug.LogWarning(
                $"[MUSIC JUKEBOX] Track '{track.name}' is not playable.",
                track);

            return false;
        }

        return true;
    }

    private bool ValidateSources()
    {
        if (sourceA == null ||
            sourceB == null)
        {
            Debug.LogError(
                "[MUSIC JUKEBOX] Two AudioSources are required.",
                this);

            return false;
        }

        if (sourceA == sourceB)
        {
            Debug.LogError(
                "[MUSIC JUKEBOX] Source A and Source B must be " +
                "different AudioSource components.",
                this);

            return false;
        }

        return true;
    }

    private static void PrepareReusableSource(
        AudioSource source)
    {
        source.playOnAwake =
            false;

        source.spatialBlend =
            0f;

        source.dopplerLevel =
            0f;

        source.volume =
            0f;
    }

    private static void StopAndClearSource(
        AudioSource source)
    {
        if (source == null)
            return;

        source.Stop();
        source.clip = null;
        source.volume = 0f;
    }

    private static void PauseIfActive(
        AudioSource source)
    {
        if (source != null &&
            source.isPlaying)
        {
            source.Pause();
        }
    }

    private static void UnPauseIfConfigured(
        AudioSource source)
    {
        if (source != null &&
            source.clip != null)
        {
            source.UnPause();
        }
    }

    private void Log(
        string message)
    {
        if (!logPlaybackCommands)
            return;

        Debug.Log(
            $"[MUSIC JUKEBOX] {message}",
            this);
    }

    private void OnValidate()
    {
        defaultFadeDuration =
            Mathf.Max(
                0f,
                defaultFadeDuration);

        if (sourceA != null &&
            sourceA == sourceB)
        {
            Debug.LogError(
                "[MUSIC JUKEBOX] Source A and Source B cannot " +
                "reference the same AudioSource.",
                this);
        }
    }
}

//----- MusicJukebox.cs END -----