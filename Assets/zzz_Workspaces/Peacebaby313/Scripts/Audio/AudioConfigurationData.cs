//----- AudioMixerConfigurationData.cs START -----

using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName = "AudioMixerConfiguration_New",
    menuName = "Rescuers2D/Audio/Audio Mixer Configuration")]
public sealed class AudioMixerConfigurationData
    : ScriptableObject
{
    [Header("Mixer")]
    [SerializeField]
    private AudioMixer audioMixer;

    [Header("Exposed Parameters")]
    [SerializeField]
    private string masterVolumeParameter =
        "MasterVolume";

    [SerializeField]
    private string musicVolumeParameter =
        "MusicVolume";

    [SerializeField]
    private string sfxVolumeParameter =
        "SFXVolume";

    [SerializeField]
    private bool useAmbience;

    [SerializeField]
    private string ambienceVolumeParameter =
        "AmbienceVolume";

    [Header("Conversion")]
    [SerializeField, Range(-100f, -20f)]
    private float minimumDecibels = -80f;

    [SerializeField, Range(-10f, 10f)]
    private float maximumDecibels;

    [SerializeField, Range(0.00001f, 0.1f)]
    private float silenceThreshold = 0.0001f;

    public AudioMixer AudioMixer =>
        audioMixer;

    public string MasterVolumeParameter =>
        masterVolumeParameter;

    public string MusicVolumeParameter =>
        musicVolumeParameter;

    public string SfxVolumeParameter =>
        sfxVolumeParameter;

    public bool UseAmbience =>
        useAmbience;

    public string AmbienceVolumeParameter =>
        ambienceVolumeParameter;

    public float MinimumDecibels =>
        minimumDecibels;

    public float MaximumDecibels =>
        maximumDecibels;

    public float SilenceThreshold =>
        silenceThreshold;

    public bool IsConfigured
    {
        get
        {
            if (audioMixer == null)
                return false;

            if (string.IsNullOrWhiteSpace(
                    masterVolumeParameter))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    musicVolumeParameter))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(
                    sfxVolumeParameter))
            {
                return false;
            }

            if (useAmbience &&
                string.IsNullOrWhiteSpace(
                    ambienceVolumeParameter))
            {
                return false;
            }

            return true;
        }
    }

    private void OnValidate()
    {
        minimumDecibels =
            Mathf.Min(
                minimumDecibels,
                maximumDecibels);

        maximumDecibels =
            Mathf.Max(
                maximumDecibels,
                minimumDecibels);

        silenceThreshold =
            Mathf.Clamp(
                silenceThreshold,
                0.00001f,
                0.1f);
    }
}

//----- AudioMixerConfigurationData.cs END -----