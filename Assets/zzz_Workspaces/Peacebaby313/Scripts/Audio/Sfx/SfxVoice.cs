
//----- SfxVoice.cs START -----

using UnityEngine;
using UnityEngine.Audio;

public sealed class SfxVoice
{
    private const float DefaultPitch = 1f;

    private readonly GameObject voiceObject;
    private readonly Transform voiceTransform;
    private readonly AudioSource audioSource;

    private Transform attachedTarget;
    private bool wasAttached;
    private bool stopWhenTargetDestroyed;
    private uint generation;

    public int VoiceId { get; }

    public bool IsActive { get; private set; }

    public SfxCueData Cue { get; private set; }

    public int Priority { get; private set; }

    public long StartOrder { get; private set; }

    public AudioSource Source =>
        audioSource;

    public SfxPlaybackHandle CurrentHandle =>
        IsActive
            ? new SfxPlaybackHandle(
                VoiceId,
                generation)
            : SfxPlaybackHandle.Invalid;

    internal SfxVoice(
        int voiceId,
        Transform poolOwner)
    {
        VoiceId = voiceId;

        voiceObject =
            new GameObject(
                $"SFX Voice {voiceId:00}");

        voiceTransform =
            voiceObject.transform;

        voiceTransform.SetParent(
            poolOwner,
            false);

        audioSource =
            voiceObject.AddComponent<AudioSource>();

        PrepareReusableSource();

        voiceObject.SetActive(false);
    }

    internal SfxPlaybackHandle Play(
        SfxCueData cue,
        AudioClip clip,
        AudioMixerGroup mixerGroup,
        Vector3 position,
        Transform target,
        bool forceTwoDimensional,
        float volume,
        float pitch,
        long startOrder)
    {
        Release();

        generation =
            generation == uint.MaxValue
                ? 1u
                : generation + 1u;

        Cue = cue;
        Priority = cue.Priority;
        StartOrder = startOrder;

        attachedTarget = target;
        wasAttached = target != null;

        stopWhenTargetDestroyed =
            cue.StopWhenAttachedTargetIsDestroyed;

        voiceObject.SetActive(true);

        voiceTransform.position =
            target != null
                ? target.position
                : position;

        audioSource.clip = clip;

        audioSource.outputAudioMixerGroup =
            mixerGroup;

        audioSource.volume =
            Mathf.Clamp01(volume);

        audioSource.pitch = pitch;
        audioSource.loop = cue.Loop;
        audioSource.priority = cue.Priority;

        audioSource.ignoreListenerPause =
            cue.IgnoreListenerPause;

        audioSource.spatialBlend =
            forceTwoDimensional
                ? 0f
                : cue.SpatialBlend;

        audioSource.minDistance =
            cue.MinimumDistance;

        audioSource.maxDistance =
            cue.MaximumDistance;

        IsActive = true;

        audioSource.Play();

        return CurrentHandle;
    }

    internal bool ShouldRelease()
    {
        if (!IsActive)
            return false;

        if (attachedTarget != null)
        {
            voiceTransform.position =
                attachedTarget.position;
        }
        else if (wasAttached)
        {
            wasAttached = false;
            attachedTarget = null;

            if (stopWhenTargetDestroyed)
                return true;
        }

        if (AudioListener.pause &&
            !audioSource.ignoreListenerPause)
        {
            return false;
        }

        return !audioSource.isPlaying;
    }

    internal bool Matches(
        SfxPlaybackHandle handle)
    {
        return IsActive &&
               handle.IsValid &&
               handle.VoiceId == VoiceId &&
               handle.Generation == generation;
    }

    internal void Release()
    {
        audioSource.Stop();

        audioSource.clip = null;

        audioSource.outputAudioMixerGroup =
            null;

        audioSource.volume = 0f;
        audioSource.pitch = DefaultPitch;
        audioSource.loop = false;
        audioSource.priority = 128;

        audioSource.ignoreListenerPause =
            false;

        audioSource.spatialBlend = 0f;

        attachedTarget = null;
        wasAttached = false;

        stopWhenTargetDestroyed = true;

        Cue = null;
        Priority = 128;
        StartOrder = 0;
        IsActive = false;

        voiceObject.SetActive(false);
    }

    private void PrepareReusableSource()
    {
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.volume = 0f;
        audioSource.pitch = DefaultPitch;
        audioSource.spatialBlend = 0f;
        audioSource.dopplerLevel = 0f;

        audioSource.rolloffMode =
            AudioRolloffMode.Logarithmic;

        audioSource.priority = 128;
    }
}

//----- SfxVoice.cs END -----