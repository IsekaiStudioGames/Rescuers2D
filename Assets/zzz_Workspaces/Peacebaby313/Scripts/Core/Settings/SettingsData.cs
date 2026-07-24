//----- SettingsData.cs START -----

using System;
using UnityEngine;

[Serializable]
public sealed class SettingsData
{
    [SerializeField]
    private int settingsVersion = 1;

    [SerializeField]
    private AudioSettingsData audio =
        new AudioSettingsData();

    [SerializeField]
    private GraphicsSettingsData graphics =
        new GraphicsSettingsData();

    [SerializeField]
    private string lastSavedUtc =
        string.Empty;

    public int SettingsVersion =>
        settingsVersion;

    public AudioSettingsData Audio =>
        audio;

    public GraphicsSettingsData Graphics =>
        graphics;

    public string LastSavedUtc =>
        lastSavedUtc;

    public static SettingsData CreateEmpty()
    {
        return new SettingsData();
    }

    public void ResetToDefaults(
        SettingsDefaultsData defaults)
    {
        if (defaults == null)
        {
            Debug.LogError(
                "[SETTINGS DATA] Cannot reset without defaults.");

            return;
        }

        EnsureNestedData();

        settingsVersion = 1;

        audio.ResetToDefaults(defaults);
        graphics.ResetToDefaults(defaults);

        UpdateTimestamp();
    }

    public void Sanitize(
        SettingsDefaultsData defaults)
    {
        if (defaults == null)
        {
            Debug.LogError(
                "[SETTINGS DATA] Cannot sanitize without defaults.");

            return;
        }

        EnsureNestedData();

        settingsVersion =
            Mathf.Max(1, settingsVersion);

        audio.Sanitize(defaults);
        graphics.Sanitize(defaults);
    }

    public void MarkSaved()
    {
        UpdateTimestamp();
    }

    private void EnsureNestedData()
    {
        if (audio == null)
        {
            audio =
                new AudioSettingsData();
        }

        if (graphics == null)
        {
            graphics =
                new GraphicsSettingsData();
        }
    }

    private void UpdateTimestamp()
    {
        lastSavedUtc =
            DateTime.UtcNow.ToString("O");
    }
}

[Serializable]
public sealed class AudioSettingsData
{
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float ambienceVolume = 0.8f;

    public float MasterVolume =>
        masterVolume;

    public float MusicVolume =>
        musicVolume;

    public float SfxVolume =>
        sfxVolume;

    public float AmbienceVolume =>
        ambienceVolume;

    public void SetMasterVolume(float value)
    {
        masterVolume =
            Mathf.Clamp01(value);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume =
            Mathf.Clamp01(value);
    }

    public void SetSfxVolume(float value)
    {
        sfxVolume =
            Mathf.Clamp01(value);
    }

    public void SetAmbienceVolume(float value)
    {
        ambienceVolume =
            Mathf.Clamp01(value);
    }

    public void ResetToDefaults(
        SettingsDefaultsData defaults)
    {
        masterVolume =
            defaults.MasterVolume;

        musicVolume =
            defaults.MusicVolume;

        sfxVolume =
            defaults.SfxVolume;

        ambienceVolume =
            defaults.AmbienceVolume;
    }

    public void Sanitize(
        SettingsDefaultsData defaults)
    {
        if (float.IsNaN(masterVolume) ||
            float.IsInfinity(masterVolume))
        {
            masterVolume =
                defaults.MasterVolume;
        }

        if (float.IsNaN(musicVolume) ||
            float.IsInfinity(musicVolume))
        {
            musicVolume =
                defaults.MusicVolume;
        }

        if (float.IsNaN(sfxVolume) ||
            float.IsInfinity(sfxVolume))
        {
            sfxVolume =
                defaults.SfxVolume;
        }

        if (float.IsNaN(ambienceVolume) ||
            float.IsInfinity(ambienceVolume))
        {
            ambienceVolume =
                defaults.AmbienceVolume;
        }

        masterVolume =
            Mathf.Clamp01(masterVolume);

        musicVolume =
            Mathf.Clamp01(musicVolume);

        sfxVolume =
            Mathf.Clamp01(sfxVolume);

        ambienceVolume =
            Mathf.Clamp01(ambienceVolume);
    }
}

[Serializable]
public sealed class GraphicsSettingsData
{
    [SerializeField]
    private int resolutionWidth = 1920;

    [SerializeField]
    private int resolutionHeight = 1080;

    [SerializeField]
    private FullScreenMode fullscreenMode =
        FullScreenMode.FullScreenWindow;

    [SerializeField]
    private int vSyncCount = 1;

    [SerializeField]
    private int qualityLevel = 2;

    [SerializeField]
    private int targetFrameRate = 60;

    public int ResolutionWidth =>
        resolutionWidth;

    public int ResolutionHeight =>
        resolutionHeight;

    public FullScreenMode FullscreenMode =>
        fullscreenMode;

    public int VSyncCount =>
        vSyncCount;

    public int QualityLevel =>
        qualityLevel;

    public int TargetFrameRate =>
        targetFrameRate;

    public void SetResolution(
        int width,
        int height)
    {
        resolutionWidth =
            Mathf.Max(320, width);

        resolutionHeight =
            Mathf.Max(180, height);
    }

    public void SetFullscreenMode(
        FullScreenMode mode)
    {
        fullscreenMode =
            mode;
    }

    public void SetVSyncCount(int value)
    {
        vSyncCount =
            Mathf.Clamp(value, 0, 4);
    }

    public void SetQualityLevel(int value)
    {
        qualityLevel =
            Mathf.Max(0, value);
    }

    public void SetTargetFrameRate(int value)
    {
        targetFrameRate =
            Mathf.Max(-1, value);
    }

    public void ResetToDefaults(
        SettingsDefaultsData defaults)
    {
        resolutionWidth =
            defaults.ResolutionWidth;

        resolutionHeight =
            defaults.ResolutionHeight;

        fullscreenMode =
            defaults.FullscreenMode;

        vSyncCount =
            defaults.VSyncCount;

        qualityLevel =
            defaults.QualityLevel;

        targetFrameRate =
            defaults.TargetFrameRate;
    }

    public void Sanitize(
        SettingsDefaultsData defaults)
    {
        if (resolutionWidth < 320)
        {
            resolutionWidth =
                defaults.ResolutionWidth;
        }

        if (resolutionHeight < 180)
        {
            resolutionHeight =
                defaults.ResolutionHeight;
        }

        if (!Enum.IsDefined(
                typeof(FullScreenMode),
                fullscreenMode))
        {
            fullscreenMode =
                defaults.FullscreenMode;
        }

        vSyncCount =
            Mathf.Clamp(vSyncCount, 0, 4);

        qualityLevel =
            Mathf.Max(0, qualityLevel);

        targetFrameRate =
            Mathf.Max(-1, targetFrameRate);
    }
}

//----- SettingsData.cs END -----