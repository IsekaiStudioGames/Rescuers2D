//----- SfxCueData.cs START -----

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[CreateAssetMenu(
    fileName = "SFX_",
    menuName = "Rescuers2D/Audio/SFX Cue")]
public sealed class SfxCueData : ScriptableObject
{
    private const float MinimumPitch = 0.01f;
    private const float MaximumPitch = 3f;
    private const float MinimumMaxDistance = 0.01f;

    [Header("Identity")]
    [Tooltip(
        "Permanent unique identifier used by tools and optional lookups.")]
    [SerializeField]
    private string cueId;

    [Tooltip(
        "Player-facing or debug-facing cue name.")]
    [SerializeField]
    private string displayName;

    [Header("Variations")]
    [SerializeField]
    private SfxSelectionMode selectionMode =
        SfxSelectionMode.RandomWithoutImmediateRepeat;

    [SerializeField]
    private SfxVariation[] variations =
        new SfxVariation[0];

    [Header("Volume And Pitch")]
    [Tooltip(
        "Base cue volume before variation and mixer volume are applied.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float cueVolume = 1f;

    [Tooltip(
        "Random volume multiplier range applied per playback.")]
    [SerializeField]
    private Vector2 randomVolumeRange =
        new Vector2(1f, 1f);

    [Tooltip(
        "Random base pitch range applied per playback.")]
    [SerializeField]
    private Vector2 randomPitchRange =
        new Vector2(1f, 1f);

    [Header("World Audio")]
    [Tooltip(
        "Zero is fully 2D. One is fully 3D.")]
    [Range(0f, 1f)]
    [SerializeField]
    private float spatialBlend;

    [Min(0f)]
    [SerializeField]
    private float minimumDistance = 1f;

    [Min(MinimumMaxDistance)]
    [SerializeField]
    private float maximumDistance = 20f;

    [Header("Playback Rules")]
    [Min(0f)]
    [SerializeField]
    private float cooldown;

    [Tooltip(
        "Maximum voices from this cue. Zero means no cue-specific limit.")]
    [Min(0)]
    [SerializeField]
    private int maximumSimultaneousVoices;

    [Tooltip(
        "Unity AudioSource priority. Zero is highest priority; 256 is lowest.")]
    [Range(0, 256)]
    [SerializeField]
    private int priority = 128;

    [Tooltip(
        "Allows the source to continue while AudioListener.pause is true.")]
    [SerializeField]
    private bool ignoreListenerPause;

    [Tooltip(
        "Loops the selected variation until its voice is stopped.")]
    [SerializeField]
    private bool loop;

    [Tooltip(
        "Stops attached playback when its target is destroyed.")]
    [SerializeField]
    private bool stopWhenAttachedTargetIsDestroyed = true;

    [Header("Routing")]
    [SerializeField]
    private AudioMixerGroup mixerGroup;

    public string CueId => cueId;

    public string DisplayName =>
        string.IsNullOrWhiteSpace(displayName)
            ? name
            : displayName;

    public SfxSelectionMode SelectionMode =>
        selectionMode;

    public IReadOnlyList<SfxVariation> Variations =>
        variations;

    public int VariationCount =>
        variations != null
            ? variations.Length
            : 0;

    public float CueVolume => cueVolume;
    public Vector2 RandomVolumeRange => randomVolumeRange;
    public Vector2 RandomPitchRange => randomPitchRange;
    public float SpatialBlend => spatialBlend;
    public float MinimumDistance => minimumDistance;
    public float MaximumDistance => maximumDistance;
    public float Cooldown => cooldown;

    public int MaximumSimultaneousVoices =>
        maximumSimultaneousVoices;

    public int Priority => priority;
    public bool IgnoreListenerPause => ignoreListenerPause;
    public bool Loop => loop;

    public bool StopWhenAttachedTargetIsDestroyed =>
        stopWhenAttachedTargetIsDestroyed;

    public AudioMixerGroup MixerGroup => mixerGroup;

    public bool HasPlayableVariation
    {
        get
        {
            if (variations == null)
                return false;

            for (int i = 0; i < variations.Length; i++)
            {
                SfxVariation variation =
                    variations[i];

                if (variation != null &&
                    variation.HasClip)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public bool IsPlayable =>
        !string.IsNullOrWhiteSpace(cueId) &&
        HasPlayableVariation;

    public bool TryGetVariation(
        int index,
        out SfxVariation variation)
    {
        variation = null;

        if (variations == null ||
            index < 0 ||
            index >= variations.Length)
        {
            return false;
        }

        variation =
            variations[index];

        return variation != null &&
               variation.HasClip;
    }

    private void OnValidate()
    {
        cueId =
            cueId?.Trim();

        displayName =
            displayName?.Trim();

        cueVolume =
            Mathf.Clamp01(cueVolume);

        randomVolumeRange =
            NormalizeRange(
                randomVolumeRange,
                0f,
                2f);

        randomPitchRange =
            NormalizeRange(
                randomPitchRange,
                MinimumPitch,
                MaximumPitch);

        spatialBlend =
            Mathf.Clamp01(spatialBlend);

        minimumDistance =
            Mathf.Max(
                0f,
                minimumDistance);

        maximumDistance =
            Mathf.Max(
                minimumDistance,
                maximumDistance,
                MinimumMaxDistance);

        cooldown =
            Mathf.Max(
                0f,
                cooldown);

        maximumSimultaneousVoices =
            Mathf.Max(
                0,
                maximumSimultaneousVoices);

        priority =
            Mathf.Clamp(
                priority,
                0,
                256);

        ValidateVariations();

        if (string.IsNullOrWhiteSpace(cueId))
        {
            Debug.LogWarning(
                $"[SFX CUE] {name} has no Cue ID.",
                this);
        }

        if (!HasPlayableVariation)
        {
            Debug.LogWarning(
                $"[SFX CUE] {name} has no playable variation.",
                this);
        }

        if (mixerGroup == null)
        {
            Debug.LogWarning(
                $"[SFX CUE] {name} has no mixer group. " +
                "Audio will use the AudioSource's current output.",
                this);
        }
    }

    private void ValidateVariations()
    {
        if (variations == null)
        {
            variations =
                new SfxVariation[0];

            return;
        }

        bool hasWeightedVariation = false;

        for (int i = 0; i < variations.Length; i++)
        {
            SfxVariation variation =
                variations[i];

            if (variation == null)
            {
                Debug.LogWarning(
                    $"[SFX CUE] {name} has a null variation " +
                    $"at index {i}.",
                    this);

                continue;
            }

            variation.Validate();

            if (!variation.HasClip)
            {
                Debug.LogWarning(
                    $"[SFX CUE] {name} has a variation with " +
                    $"no AudioClip at index {i}.",
                    this);
            }

            if (variation.CanBeRandomlySelected)
            {
                hasWeightedVariation = true;
            }
        }

        bool usesWeightedSelection =
            selectionMode == SfxSelectionMode.Random ||
            selectionMode ==
                SfxSelectionMode.RandomWithoutImmediateRepeat ||
            selectionMode == SfxSelectionMode.ShuffleBag;

        if (usesWeightedSelection &&
            HasPlayableVariation &&
            !hasWeightedVariation)
        {
            Debug.LogWarning(
                $"[SFX CUE] {name} uses {selectionMode}, but " +
                "every playable variation has zero selection weight.",
                this);
        }
    }

    private static Vector2 NormalizeRange(
        Vector2 range,
        float minimum,
        float maximum)
    {
        float low =
            Mathf.Clamp(
                Mathf.Min(
                    range.x,
                    range.y),
                minimum,
                maximum);

        float high =
            Mathf.Clamp(
                Mathf.Max(
                    range.x,
                    range.y),
                minimum,
                maximum);

        return new Vector2(
            low,
            high);
    }
}

//----- SfxCueData.cs END -----