
//----- SettingsDefaultsData.cs START -----

using UnityEngine;

[CreateAssetMenu(
    fileName = "SettingsDefaults_New",
    menuName = "Rescuers2D/Settings/Settings Defaults")]
public sealed class SettingsDefaultsData : ScriptableObject
{
    [Header("Audio Defaults")]
    [SerializeField, Range(0f, 1f)]
    private float masterVolume = 1f;

    [SerializeField, Range(0f, 1f)]
    private float musicVolume = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float sfxVolume = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float ambienceVolume = 0.8f;

    [Header("Graphics Defaults")]
    [SerializeField, Min(320)]
    private int resolutionWidth = 1920;

    [SerializeField, Min(180)]
    private int resolutionHeight = 1080;

    [SerializeField]
    private FullScreenMode fullscreenMode =
        FullScreenMode.FullScreenWindow;

    [SerializeField, Range(0, 4)]
    private int vSyncCount = 1;

    [SerializeField, Min(0)]
    private int qualityLevel = 2;

    [SerializeField, Min(-1)]
    private int targetFrameRate = 60;

    public float MasterVolume =>
        masterVolume;

    public float MusicVolume =>
        musicVolume;

    public float SfxVolume =>
        sfxVolume;

    public float AmbienceVolume =>
        ambienceVolume;

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

    public SettingsData CreateRuntimeData()
    {
        SettingsData data =
            SettingsData.CreateEmpty();

        data.ResetToDefaults(this);

        return data;
    }

    private void OnValidate()
    {
        masterVolume =
            Mathf.Clamp01(masterVolume);

        musicVolume =
            Mathf.Clamp01(musicVolume);

        sfxVolume =
            Mathf.Clamp01(sfxVolume);

        ambienceVolume =
            Mathf.Clamp01(ambienceVolume);

        resolutionWidth =
            Mathf.Max(320, resolutionWidth);

        resolutionHeight =
            Mathf.Max(180, resolutionHeight);

        vSyncCount =
            Mathf.Clamp(vSyncCount, 0, 4);

        qualityLevel =
            Mathf.Max(0, qualityLevel);

        targetFrameRate =
            Mathf.Max(-1, targetFrameRate);
    }
}

//----- SettingsDefaultsData.cs END -----