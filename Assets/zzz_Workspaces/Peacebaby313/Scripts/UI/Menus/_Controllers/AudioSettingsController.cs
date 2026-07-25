//----- AudioSettingsController.cs START -----

using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AudioSettingsController
    : MonoBehaviour
{
    [Header("Volume Sliders")]
    [SerializeField]
    private Slider masterVolumeSlider;

    [SerializeField]
    private Slider musicVolumeSlider;

    [SerializeField]
    private Slider sfxVolumeSlider;

    [SerializeField]
    private Slider ambienceVolumeSlider;

    [Header("Percentage Labels")]
    [SerializeField]
    private TMP_Text masterVolumeText;

    [SerializeField]
    private TMP_Text musicVolumeText;

    [SerializeField]
    private TMP_Text sfxVolumeText;

    [SerializeField]
    private TMP_Text ambienceVolumeText;

    [Header("Optional Ambience")]
    [SerializeField]
    private GameObject ambienceRow;

    [Header("Actions")]
    [SerializeField]
    private Button resetAudioButton;

    [Header("Feedback")]
    [SerializeField]
    private TMP_Text statusText;

    [Header("Persistence")]
    [SerializeField, Min(0f)]
    private float saveDelay = 0.35f;

    private ApplicationBootstrap bootstrap;
    private SettingsService settingsService;
    private AudioSettingsService audioService;

    private AudioMixerConfigurationData
        configuration;

    private Coroutine saveRoutine;

    private bool refreshingControls;
    private bool hasUnsavedChanges;

    private void Awake()
    {
        ConfigureSlider(
            masterVolumeSlider);

        ConfigureSlider(
            musicVolumeSlider);

        ConfigureSlider(
            sfxVolumeSlider);

        ConfigureSlider(
            ambienceVolumeSlider);

        AddListeners();
    }

    public void RefreshFromCurrentSettings(
        bool updateStatus = true)
    {
        if (!ResolveServices())
        {
            SetStatus(
                "Audio services are unavailable.");

            return;
        }

        AudioSettingsData audioData =
            settingsService
                .CurrentData
                .Audio;

        refreshingControls =
            true;

        SetSliderValue(
            masterVolumeSlider,
            audioData.MasterVolume);

        SetSliderValue(
            musicVolumeSlider,
            audioData.MusicVolume);

        SetSliderValue(
            sfxVolumeSlider,
            audioData.SfxVolume);

        SetSliderValue(
            ambienceVolumeSlider,
            audioData.AmbienceVolume);

        refreshingControls =
            false;

        bool showAmbience =
            configuration != null &&
            configuration.UseAmbience;

        if (ambienceRow != null)
        {
            ambienceRow.SetActive(
                showAmbience);
        }

        UpdateAllLabels();

        hasUnsavedChanges =
            false;

        if (updateStatus)
        {
            SetStatus(
                "Audio changes preview immediately and save automatically.");
        }
    }

    public void ResetAudioToDefaults()
    {
        if (!ResolveServices())
        {
            SetStatus(
                "Audio services are unavailable.");

            return;
        }

        CancelScheduledSave();

        bool resetSucceeded =
            settingsService.ResetAudioToDefaults(
                saveImmediately: true);

        RefreshFromCurrentSettings(
            updateStatus: false);

        SetStatus(
            resetSucceeded
                ? "Audio settings reset to defaults."
                : "Audio settings could not be reset.");
    }

    public void CommitPendingChanges()
    {
        CancelScheduledSave();

        if (!hasUnsavedChanges ||
            settingsService == null)
        {
            return;
        }

        bool saveSucceeded =
            settingsService.SaveCurrent();

        hasUnsavedChanges =
            !saveSucceeded;

        SetStatus(
            saveSucceeded
                ? "Audio settings saved."
                : "Audio settings could not be saved.");
    }

    private bool ResolveServices()
    {
        if (bootstrap == null)
        {
            bootstrap =
                ApplicationBootstrap.Instance;
        }

        if (bootstrap == null ||
            !bootstrap.IsInitialized)
        {
            return false;
        }

        settingsService =
            bootstrap.SettingsService;

        audioService =
            bootstrap.AudioService;

        configuration =
            bootstrap.AudioMixerConfiguration;

        return
            settingsService != null &&
            settingsService.IsInitialized &&
            settingsService.CurrentData != null &&
            settingsService.CurrentData.Audio != null &&
            audioService != null &&
            audioService.IsInitialized &&
            configuration != null;
    }

    private void HandleMasterVolumeChanged(
        float value)
    {
        if (refreshingControls ||
            !ResolveServices())
        {
            return;
        }

        settingsService
            .CurrentData
            .Audio
            .SetMasterVolume(
                value);

        SetPercentageText(
            masterVolumeText,
            value);

        ApplyAndScheduleSave();
    }

    private void HandleMusicVolumeChanged(
        float value)
    {
        if (refreshingControls ||
            !ResolveServices())
        {
            return;
        }

        settingsService
            .CurrentData
            .Audio
            .SetMusicVolume(
                value);

        SetPercentageText(
            musicVolumeText,
            value);

        ApplyAndScheduleSave();
    }

    private void HandleSfxVolumeChanged(
        float value)
    {
        if (refreshingControls ||
            !ResolveServices())
        {
            return;
        }

        settingsService
            .CurrentData
            .Audio
            .SetSfxVolume(
                value);

        SetPercentageText(
            sfxVolumeText,
            value);

        ApplyAndScheduleSave();
    }

    private void HandleAmbienceVolumeChanged(
        float value)
    {
        if (refreshingControls ||
            !ResolveServices())
        {
            return;
        }

        settingsService
            .CurrentData
            .Audio
            .SetAmbienceVolume(
                value);

        SetPercentageText(
            ambienceVolumeText,
            value);

        ApplyAndScheduleSave();
    }

    private void ApplyAndScheduleSave()
    {
        settingsService.NotifyAudioSettingsChanged(
            saveImmediately: false);

        hasUnsavedChanges =
            true;

        SetStatus(
            "Previewing audio changes...");

        ScheduleSave();
    }

    private void ScheduleSave()
    {
        CancelScheduledSave();

        saveRoutine =
            StartCoroutine(
                SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        if (saveDelay > 0f)
        {
            yield return
                new WaitForSecondsRealtime(
                    saveDelay);
        }

        saveRoutine =
            null;

        CommitPendingChanges();
    }

    private void CancelScheduledSave()
    {
        if (saveRoutine == null)
            return;

        StopCoroutine(
            saveRoutine);

        saveRoutine =
            null;
    }

    private static void ConfigureSlider(
        Slider slider)
    {
        if (slider == null)
            return;

        slider.minValue =
            0f;

        slider.maxValue =
            1f;

        slider.wholeNumbers =
            false;
    }

    private static void SetSliderValue(
        Slider slider,
        float value)
    {
        if (slider == null)
            return;

        slider.SetValueWithoutNotify(
            Mathf.Clamp01(
                value));
    }

    private void UpdateAllLabels()
    {
        UpdateLabel(
            masterVolumeText,
            masterVolumeSlider);

        UpdateLabel(
            musicVolumeText,
            musicVolumeSlider);

        UpdateLabel(
            sfxVolumeText,
            sfxVolumeSlider);

        UpdateLabel(
            ambienceVolumeText,
            ambienceVolumeSlider);
    }

    private static void UpdateLabel(
        TMP_Text label,
        Slider slider)
    {
        if (label == null ||
            slider == null)
        {
            return;
        }

        SetPercentageText(
            label,
            slider.value);
    }

    private static void SetPercentageText(
        TMP_Text label,
        float value)
    {
        if (label == null)
            return;

        int percentage =
            Mathf.RoundToInt(
                Mathf.Clamp01(
                    value) *
                100f);

        label.text =
            $"{percentage}%";
    }

    private void AddListeners()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider
                .onValueChanged
                .AddListener(
                    HandleMasterVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider
                .onValueChanged
                .AddListener(
                    HandleMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider
                .onValueChanged
                .AddListener(
                    HandleSfxVolumeChanged);
        }

        if (ambienceVolumeSlider != null)
        {
            ambienceVolumeSlider
                .onValueChanged
                .AddListener(
                    HandleAmbienceVolumeChanged);
        }

        if (resetAudioButton != null)
        {
            resetAudioButton
                .onClick
                .AddListener(
                    ResetAudioToDefaults);
        }
    }

    private void RemoveListeners()
    {
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider
                .onValueChanged
                .RemoveListener(
                    HandleMasterVolumeChanged);
        }

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider
                .onValueChanged
                .RemoveListener(
                    HandleMusicVolumeChanged);
        }

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider
                .onValueChanged
                .RemoveListener(
                    HandleSfxVolumeChanged);
        }

        if (ambienceVolumeSlider != null)
        {
            ambienceVolumeSlider
                .onValueChanged
                .RemoveListener(
                    HandleAmbienceVolumeChanged);
        }

        if (resetAudioButton != null)
        {
            resetAudioButton
                .onClick
                .RemoveListener(
                    ResetAudioToDefaults);
        }
    }

    private void SetStatus(
        string message)
    {
        if (statusText != null)
        {
            statusText.text =
                message;
        }
    }

    private void OnDisable()
    {
        CommitPendingChanges();
    }

    private void OnDestroy()
    {
        CancelScheduledSave();
        CommitPendingChanges();
        RemoveListeners();
    }
}

//----- AudioSettingsController.cs END -----