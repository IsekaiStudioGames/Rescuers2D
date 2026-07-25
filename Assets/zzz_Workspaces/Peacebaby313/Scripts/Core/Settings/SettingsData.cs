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
            return;

        EnsureCategoriesExist();

        audio.ResetToDefaults(
            defaults);

        graphics.ResetToDefaults(
            defaults);
    }

    public void Sanitize(
        SettingsDefaultsData defaults)
    {
        if (defaults == null)
            return;

        if (audio == null)
        {
            audio =
                new AudioSettingsData();

            audio.ResetToDefaults(
                defaults);
        }
        else
        {
            audio.Sanitize(
                defaults);
        }

        if (graphics == null)
        {
            graphics =
                new GraphicsSettingsData();

            graphics.ResetToDefaults(
                defaults);
        }
        else
        {
            graphics.Sanitize(
                defaults);
        }

        settingsVersion =
            Mathf.Max(
                1,
                settingsVersion);
    }

    public void MarkSaved()
    {
        lastSavedUtc =
            DateTime.UtcNow.ToString(
                "O");
    }

    private void EnsureCategoriesExist()
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

    public void SetMasterVolume(
        float value)
    {
        masterVolume =
            Mathf.Clamp01(
                value);
    }

    public void SetMusicVolume(
        float value)
    {
        musicVolume =
            Mathf.Clamp01(
                value);
    }

    public void SetSfxVolume(
        float value)
    {
        sfxVolume =
            Mathf.Clamp01(
                value);
    }

    public void SetAmbienceVolume(
        float value)
    {
        ambienceVolume =
            Mathf.Clamp01(
                value);
    }

    public void ResetToDefaults(
        SettingsDefaultsData defaults)
    {
        if (defaults == null)
            return;

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
        masterVolume =
            Mathf.Clamp01(
                masterVolume);

        musicVolume =
            Mathf.Clamp01(
                musicVolume);

        sfxVolume =
            Mathf.Clamp01(
                sfxVolume);

        ambienceVolume =
            Mathf.Clamp01(
                ambienceVolume);
    }
}

[Serializable]
public sealed class GraphicsSettingsData
{
    [SerializeField, Min(640)]
    private int resolutionWidth = 1920;

    [SerializeField, Min(360)]
    private int resolutionHeight = 1080;

    [SerializeField]
    private FullScreenMode fullscreenMode =
        FullScreenMode.FullScreenWindow;

    [SerializeField, Range(0, 4)]
    private int vSyncCount = 1;

    [SerializeField, Min(0)]
    private int qualityLevel;

    [SerializeField, Min(-1)]
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
            Mathf.Max(
                640,
                width);

        resolutionHeight =
            Mathf.Max(
                360,
                height);
    }

    public void SetFullscreenMode(
        FullScreenMode value)
    {
        fullscreenMode =
            value;
    }

    public void SetVSyncCount(
        int value)
    {
        vSyncCount =
            Mathf.Clamp(
                value,
                0,
                4);
    }

    public void SetQualityLevel(
        int value)
    {
        qualityLevel =
            Mathf.Max(
                0,
                value);
    }

    public void SetTargetFrameRate(
        int value)
    {
        targetFrameRate =
            Mathf.Max(
                -1,
                value);
    }

    public void ResetToDefaults(
        SettingsDefaultsData defaults)
    {
        if (defaults == null)
            return;

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
        resolutionWidth =
            Mathf.Max(
                640,
                resolutionWidth);

        resolutionHeight =
            Mathf.Max(
                360,
                resolutionHeight);

        vSyncCount =
            Mathf.Clamp(
                vSyncCount,
                0,
                4);

        qualityLevel =
            Mathf.Max(
                0,
                qualityLevel);

        targetFrameRate =
            Mathf.Max(
                -1,
                targetFrameRate);
    }
}

//----- SettingsData.cs END -----