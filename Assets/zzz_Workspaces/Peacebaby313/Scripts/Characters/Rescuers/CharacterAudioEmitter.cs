//----- CharacterAudioEmitter.cs START -----

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class CharacterAudioEmitter
    : MonoBehaviour
{
    [Header("Character Audio")]
    [SerializeField]
    private CharacterAudioProfileData profile;

    [Tooltip(
        "World-space origin followed by attached sounds. " +
        "Defaults to this character root.")]
    [SerializeField]
    private Transform audioOrigin;

    [Tooltip(
        "Optional direct reference for isolated testing. " +
        "Normal scene characters resolve the persistent bootstrap SfxPlayer.")]
    [SerializeField]
    private SfxPlayer sfxPlayer;

    [Header("Diagnostics")]
    [SerializeField]
    private bool logMissingProfile = true;

    [SerializeField]
    private bool logMissingCues;

    [SerializeField]
    private bool logMissingPlayer = true;

    private readonly HashSet<CharacterAudioEvent>
        loggedMissingCueEvents =
            new HashSet<CharacterAudioEvent>();

    private SfxPlaybackHandle swimLoopHandle =
        SfxPlaybackHandle.Invalid;

    private bool hasPlayedDeath;
    private bool hasLoggedMissingProfile;
    private bool hasLoggedMissingPlayer;

    public CharacterAudioProfileData Profile =>
        profile;

    public Transform AudioOrigin =>
        audioOrigin != null
            ? audioOrigin
            : transform;

    public bool HasProfile =>
        profile != null;

    public bool IsSwimLoopPlaying =>
        sfxPlayer != null &&
        swimLoopHandle.IsValid &&
        sfxPlayer.IsPlaying(
            swimLoopHandle);

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

    private void OnDisable()
    {
        StopSwimLoop();
    }

    public void SetProfile(
        CharacterAudioProfileData newProfile)
    {
        if (profile == newProfile)
            return;

        StopSwimLoop();

        profile =
            newProfile;

        hasPlayedDeath =
            false;

        hasLoggedMissingProfile =
            false;

        loggedMissingCueEvents.Clear();
    }

    public void ResetLifecycleAudio()
    {
        StopSwimLoop();

        hasPlayedDeath =
            false;
    }

    public SfxPlaybackHandle Play(
        CharacterAudioEvent audioEvent)
    {
        switch (audioEvent)
        {
            case CharacterAudioEvent.Death:
                return PlayDeathOnce();

            case CharacterAudioEvent.SwimLoop:
                return StartSwimLoopInternal();

            default:
                return PlayAttachedOneShot(
                    audioEvent);
        }
    }

    [ContextMenu("Audio Test/Play Jump")]
    public void PlayJump()
    {
        Play(
            CharacterAudioEvent.Jump);
    }

    [ContextMenu("Audio Test/Play Land")]
    public void PlayLand()
    {
        Play(
            CharacterAudioEvent.Land);
    }

    [ContextMenu("Audio Test/Play Hurt")]
    public void PlayHurt()
    {
        Play(
            CharacterAudioEvent.Hurt);
    }

    [ContextMenu("Audio Test/Play Death")]
    public void PlayDeath()
    {
        Play(
            CharacterAudioEvent.Death);
    }

    [ContextMenu("Audio Test/Play Footstep")]
    public void PlayFootstep()
    {
        Play(
            CharacterAudioEvent.Footstep);
    }

    [ContextMenu("Audio Test/Play Swim Stroke")]
    public void PlaySwimStroke()
    {
        Play(
            CharacterAudioEvent.SwimStroke);
    }

    [ContextMenu("Audio Test/Start Swim Loop")]
    public void StartSwimLoop()
    {
        Play(
            CharacterAudioEvent.SwimLoop);
    }

    [ContextMenu("Audio Test/Stop Swim Loop")]
    public void StopSwimLoopFromEvent()
    {
        StopSwimLoop();
    }

    [ContextMenu("Audio Test/Play Climb Step")]
    public void PlayClimbStep()
    {
        Play(
            CharacterAudioEvent.ClimbStep);
    }

    public void PlayInteract()
    {
        Play(
            CharacterAudioEvent.Interact);
    }

    public void PlayPickup()
    {
        Play(
            CharacterAudioEvent.Pickup);
    }

    public void PlayDrop()
    {
        Play(
            CharacterAudioEvent.Drop);
    }

    [ContextMenu("Audio Test/Play Primary Action")]
    public void PlayPrimaryAction()
    {
        Play(
            CharacterAudioEvent.PrimaryAction);
    }

    [ContextMenu("Audio Test/Play Secondary Action")]
    public void PlaySecondaryAction()
    {
        Play(
            CharacterAudioEvent.SecondaryAction);
    }

    [ContextMenu("Audio Test/Play Special Action")]
    public void PlaySpecialAction()
    {
        Play(
            CharacterAudioEvent.SpecialAction);
    }

    public bool StopSwimLoop()
    {
        if (!swimLoopHandle.IsValid)
            return false;

        bool stopped =
            false;

        if (TryResolveSfxPlayer())
        {
            stopped =
                sfxPlayer.Stop(
                    swimLoopHandle);
        }

        swimLoopHandle =
            SfxPlaybackHandle.Invalid;

        return stopped;
    }

    private SfxPlaybackHandle PlayAttachedOneShot(
        CharacterAudioEvent audioEvent)
    {
        if (!TryResolveRequest(
                audioEvent,
                out SfxCueData cue))
        {
            return SfxPlaybackHandle.Invalid;
        }

        return sfxPlayer.PlayAttached(
            cue,
            AudioOrigin,
            profile.VolumeMultiplier,
            profile.PitchMultiplier);
    }

    private SfxPlaybackHandle PlayDeathOnce()
    {
        if (hasPlayedDeath)
            return SfxPlaybackHandle.Invalid;

        StopSwimLoop();

        if (!TryResolveRequest(
                CharacterAudioEvent.Death,
                out SfxCueData cue))
        {
            return SfxPlaybackHandle.Invalid;
        }

        SfxPlaybackHandle handle =
            sfxPlayer.PlayAtPosition(
                cue,
                AudioOrigin.position,
                profile.VolumeMultiplier,
                profile.PitchMultiplier);

        if (handle.IsValid)
        {
            hasPlayedDeath =
                true;
        }

        return handle;
    }

    private SfxPlaybackHandle StartSwimLoopInternal()
    {
        if (swimLoopHandle.IsValid &&
            TryResolveSfxPlayer() &&
            sfxPlayer.IsPlaying(
                swimLoopHandle))
        {
            return swimLoopHandle;
        }

        swimLoopHandle =
            SfxPlaybackHandle.Invalid;

        if (!TryResolveRequest(
                CharacterAudioEvent.SwimLoop,
                out SfxCueData cue))
        {
            return SfxPlaybackHandle.Invalid;
        }

        swimLoopHandle =
            sfxPlayer.PlayAttached(
                cue,
                AudioOrigin,
                profile.VolumeMultiplier,
                profile.PitchMultiplier);

        return swimLoopHandle;
    }

    private bool TryResolveRequest(
        CharacterAudioEvent audioEvent,
        out SfxCueData cue)
    {
        cue =
            null;

        if (profile == null)
        {
            LogMissingProfileOnce();
            return false;
        }

        if (!profile.TryGetCue(
                audioEvent,
                out cue))
        {
            LogMissingCueOnce(
                audioEvent);

            return false;
        }

        return TryResolveSfxPlayer();
    }

    private bool TryResolveSfxPlayer()
    {
        if (sfxPlayer != null)
            return true;

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
                $"[CHARACTER AUDIO] '{name}' could not resolve " +
                "the persistent SfxPlayer.",
                this);

            hasLoggedMissingPlayer =
                true;
        }

        return false;
    }

    private void LogMissingProfileOnce()
    {
        if (!logMissingProfile ||
            hasLoggedMissingProfile)
        {
            return;
        }

        Debug.LogWarning(
            $"[CHARACTER AUDIO] '{name}' has no " +
            "CharacterAudioProfileData assigned.",
            this);

        hasLoggedMissingProfile =
            true;
    }

    private void LogMissingCueOnce(
        CharacterAudioEvent audioEvent)
    {
        if (!logMissingCues ||
            !loggedMissingCueEvents.Add(
                audioEvent))
        {
            return;
        }

        Debug.LogWarning(
            $"[CHARACTER AUDIO] Profile '{profile.name}' has no cue " +
            $"assigned for {audioEvent}.",
            profile);
    }
}

//----- CharacterAudioEmitter.cs END -----