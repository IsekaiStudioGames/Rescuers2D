//----- UIAudioProfileData.cs START -----

using UnityEngine;

public enum UIAudioCue
{
    Navigate,
    Submit,
    Cancel,
    ValueChanged
}

[CreateAssetMenu(
    fileName = "UIAudioProfile_New",
    menuName = "Rescuers2D/Audio/UI Audio Profile")]
public sealed class UIAudioProfileData
    : ScriptableObject
{
    [Header("Clips")]
    [SerializeField]
    private AudioClip navigateClip;

    [SerializeField]
    private AudioClip submitClip;

    [SerializeField]
    private AudioClip cancelClip;

    [SerializeField]
    private AudioClip valueChangedClip;

    [Header("Volumes")]
    [SerializeField, Range(0f, 1f)]
    private float navigateVolume = 0.55f;

    [SerializeField, Range(0f, 1f)]
    private float submitVolume = 0.8f;

    [SerializeField, Range(0f, 1f)]
    private float cancelVolume = 0.7f;

    [SerializeField, Range(0f, 1f)]
    private float valueChangedVolume = 0.4f;

    [Header("Playback")]
    [SerializeField, Min(0f)]
    private float minimumCueInterval = 0.045f;

    public float MinimumCueInterval =>
        minimumCueInterval;

    public AudioClip GetClip(
        UIAudioCue cue)
    {
        switch (cue)
        {
            case UIAudioCue.Navigate:
                return navigateClip;

            case UIAudioCue.Submit:
                return submitClip;

            case UIAudioCue.Cancel:
                return cancelClip;

            case UIAudioCue.ValueChanged:
                return valueChangedClip;

            default:
                return null;
        }
    }

    public float GetVolume(
        UIAudioCue cue)
    {
        switch (cue)
        {
            case UIAudioCue.Navigate:
                return navigateVolume;

            case UIAudioCue.Submit:
                return submitVolume;

            case UIAudioCue.Cancel:
                return cancelVolume;

            case UIAudioCue.ValueChanged:
                return valueChangedVolume;

            default:
                return 1f;
        }
    }

    private void OnValidate()
    {
        minimumCueInterval =
            Mathf.Max(
                0f,
                minimumCueInterval);
    }
}

//----- UIAudioProfileData.cs END -----