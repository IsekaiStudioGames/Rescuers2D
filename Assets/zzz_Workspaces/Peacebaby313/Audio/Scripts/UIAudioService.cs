//----- UIAudioService.cs START -----

using System.Collections.Generic;
using UnityEngine;

public sealed class UIAudioService
{
    private readonly AudioSource audioSource;
    private readonly UIAudioProfileData profile;

    private readonly Dictionary<UIAudioCue, float>
        lastCueTimes =
            new Dictionary<UIAudioCue, float>();

    public bool IsInitialized
    {
        get;
        private set;
    }

    public UIAudioService(
        AudioSource audioSource,
        UIAudioProfileData profile)
    {
        this.audioSource =
            audioSource;

        this.profile =
            profile;
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        if (audioSource == null)
        {
            Debug.LogError(
                "[UI AUDIO] UI AudioSource is missing.");

            return;
        }

        if (profile == null)
        {
            Debug.LogError(
                "[UI AUDIO] UIAudioProfileData is missing.");

            return;
        }

        audioSource.playOnAwake =
            false;

        audioSource.loop =
            false;

        audioSource.spatialBlend =
            0f;

        audioSource.ignoreListenerPause =
            true;

        IsInitialized =
            true;
    }

    public void Play(
        UIAudioCue cue)
    {
        if (!IsInitialized)
            return;

        AudioClip clip =
            profile.GetClip(
                cue);

        if (clip == null)
            return;

        float currentTime =
            Time.unscaledTime;

        if (lastCueTimes.TryGetValue(
                cue,
                out float lastCueTime))
        {
            float elapsed =
                currentTime -
                lastCueTime;

            if (elapsed <
                profile.MinimumCueInterval)
            {
                return;
            }
        }

        lastCueTimes[cue] =
            currentTime;

        audioSource.PlayOneShot(
            clip,
            profile.GetVolume(
                cue));
    }

    public void PlayNavigate()
    {
        Play(
            UIAudioCue.Navigate);
    }

    public void PlaySubmit()
    {
        Play(
            UIAudioCue.Submit);
    }

    public void PlayCancel()
    {
        Play(
            UIAudioCue.Cancel);
    }

    public void PlayValueChanged()
    {
        Play(
            UIAudioCue.ValueChanged);
    }
}

//----- UIAudioService.cs END -----