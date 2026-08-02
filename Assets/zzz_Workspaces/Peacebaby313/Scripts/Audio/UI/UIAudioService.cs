//----- UIAudioService.cs START -----

using System.Collections.Generic;
using UnityEngine;

public sealed class UIAudioService
{
    private readonly SfxPlayer sfxPlayer;
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
        SfxPlayer sfxPlayer,
        UIAudioProfileData profile)
    {
        this.sfxPlayer = sfxPlayer;
        this.profile = profile;
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        if (sfxPlayer == null)
        {
            Debug.LogError(
                "[UI AUDIO] SfxPlayer is missing.");

            return;
        }

        if (profile == null)
        {
            Debug.LogError(
                "[UI AUDIO] UIAudioProfileData is missing.");

            return;
        }

        IsInitialized = true;
    }

    public void Play(
        UIAudioCue cue)
    {
        if (!IsInitialized)
            return;

        if (sfxPlayer == null)
        {
            IsInitialized = false;

            Debug.LogError(
                "[UI AUDIO] The Jukebot SfxPlayer " +
                "is no longer available.");

            return;
        }

        if (profile == null)
        {
            IsInitialized = false;

            Debug.LogError(
                "[UI AUDIO] The UI audio profile " +
                "is no longer available.");

            return;
        }

        SfxCueData cueData =
            profile.GetCue(cue);

        if (cueData == null)
            return;

        float currentTime =
            Time.unscaledTime;

        if (lastCueTimes.TryGetValue(
                cue,
                out float lastCueTime))
        {
            float elapsed =
                currentTime - lastCueTime;

            if (elapsed <
                profile.MinimumCueInterval)
            {
                return;
            }
        }

        lastCueTimes[cue] =
            currentTime;

        sfxPlayer.Play(
            cueData,
            profile.GetVolume(cue));
    }

    public void PlayNavigate()
    {
        Play(UIAudioCue.Navigate);
    }

    public void PlaySubmit()
    {
        Play(UIAudioCue.Submit);
    }

    public void PlayCancel()
    {
        Play(UIAudioCue.Cancel);
    }

    public void PlayValueChanged()
    {
        Play(UIAudioCue.ValueChanged);
    }
}

//----- UIAudioService.cs END -----

