//----- SfxVariation.cs START -----

using System;
using UnityEngine;

[Serializable]
public sealed class SfxVariation
{
    [Tooltip(
        "Audio clip used by this variation.")]
    [SerializeField]
    private AudioClip clip;

    [Tooltip(
        "Per-clip volume correction multiplied by the cue volume.")]
    [Range(0f, 2f)]
    [SerializeField]
    private float volumeMultiplier = 1f;

    [Tooltip(
        "Per-clip pitch correction added to the cue's randomized pitch.")]
    [Range(-2f, 2f)]
    [SerializeField]
    private float pitchOffset;

    [Tooltip(
        "Relative chance of selection in weighted random modes. " +
        "A value of zero disables random selection for this variation.")]
    [Min(0f)]
    [SerializeField]
    private float selectionWeight = 1f;

    public AudioClip Clip => clip;
    public float VolumeMultiplier => volumeMultiplier;
    public float PitchOffset => pitchOffset;
    public float SelectionWeight => selectionWeight;

    public bool HasClip => clip != null;

    public bool CanBeRandomlySelected =>
        clip != null &&
        selectionWeight > 0f;

    public void Validate()
    {
        volumeMultiplier =
            Mathf.Clamp(
                volumeMultiplier,
                0f,
                2f);

        pitchOffset =
            Mathf.Clamp(
                pitchOffset,
                -2f,
                2f);

        selectionWeight =
            Mathf.Max(
                0f,
                selectionWeight);
    }
}

//----- SfxVariation.cs END -----