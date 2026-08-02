//----- AudioSettingsService.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public sealed class AudioSettingsService
    : IDisposable
{
    private readonly SettingsService settingsService;

    private readonly AudioMixerConfigurationData
        configuration;

    private readonly HashSet<string>
        missingParameterWarnings =
            new HashSet<string>();

    public bool IsInitialized
    {
        get;
        private set;
    }

    public AudioSettingsService(
        SettingsService settingsService,
        AudioMixerConfigurationData configuration)
    {
        this.settingsService =
            settingsService;

        this.configuration =
            configuration;
    }

    public void Initialize()
    {
        if (IsInitialized)
            return;

        if (settingsService == null)
        {
            Debug.LogError(
                "[AUDIO SETTINGS] SettingsService is missing.");

            return;
        }

        if (configuration == null ||
            !configuration.IsConfigured)
        {
            Debug.LogError(
                "[AUDIO SETTINGS] Audio mixer configuration " +
                "is missing or invalid.");

            return;
        }

        settingsService.OnAudioSettingsChanged +=
            HandleAudioSettingsChanged;

        IsInitialized =
            true;
    }

    public void Dispose()
    {
        if (!IsInitialized ||
            settingsService == null)
        {
            return;
        }

        settingsService.OnAudioSettingsChanged -=
            HandleAudioSettingsChanged;

        IsInitialized =
            false;
    }

    public bool ApplyCurrentSettings()
    {
        if (!IsInitialized ||
            settingsService == null ||
            settingsService.CurrentData == null)
        {
            return false;
        }

        return Apply(
            settingsService
                .CurrentData
                .Audio);
    }

    public bool Apply(
        AudioSettingsData audioData)
    {
        if (audioData == null)
        {
            Debug.LogError(
                "[AUDIO SETTINGS] Cannot apply null audio data.");

            return false;
        }

        AudioMixer mixer =
            configuration.AudioMixer;

        if (mixer == null)
        {
            Debug.LogError(
                "[AUDIO SETTINGS] AudioMixer is missing.");

            return false;
        }

        bool success =
            true;

        success &=
            SetLinearVolume(
                mixer,
                configuration.MasterVolumeParameter,
                audioData.MasterVolume);

        success &=
            SetLinearVolume(
                mixer,
                configuration.MusicVolumeParameter,
                audioData.MusicVolume);

        success &=
            SetLinearVolume(
                mixer,
                configuration.SfxVolumeParameter,
                audioData.SfxVolume);

        if (configuration.UseAmbience)
        {
            success &=
                SetLinearVolume(
                    mixer,
                    configuration.AmbienceVolumeParameter,
                    audioData.AmbienceVolume);
        }

        return success;
    }

    public float LinearToDecibels(
        float linearValue)
    {
        linearValue =
            Mathf.Clamp01(
                linearValue);

        if (linearValue <=
            configuration.SilenceThreshold)
        {
            return
                configuration.MinimumDecibels;
        }

        float decibels =
            20f *
            Mathf.Log10(
                linearValue);

        return Mathf.Clamp(
            decibels,
            configuration.MinimumDecibels,
            configuration.MaximumDecibels);
    }

    private bool SetLinearVolume(
        AudioMixer mixer,
        string parameterName,
        float linearValue)
    {
        if (string.IsNullOrWhiteSpace(
                parameterName))
        {
            return false;
        }

        float decibels =
            LinearToDecibels(
                linearValue);

        bool parameterSet =
            mixer.SetFloat(
                parameterName,
                decibels);

        if (!parameterSet &&
            missingParameterWarnings.Add(
                parameterName))
        {
            Debug.LogError(
                $"[AUDIO SETTINGS] Mixer parameter " +
                $"'{parameterName}' was not found. " +
                "Confirm the Volume parameter is exposed " +
                "and renamed exactly.");
        }

        return parameterSet;
    }

    private void HandleAudioSettingsChanged(
        AudioSettingsData audioData)
    {
        Apply(
            audioData);
    }
}

//----- AudioSettingsService.cs END -----