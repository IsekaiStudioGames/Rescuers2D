//----- CharacterAudioProfileData.cs START -----

using UnityEngine;

public enum CharacterAudioEvent
{
    Jump = 0,
    Land = 1,
    Hurt = 2,
    Death = 3,
    Footstep = 4,
    SwimStroke = 5,
    SwimLoop = 6,
    ClimbStep = 7,
    Interact = 8,
    Pickup = 9,
    Drop = 10,
    PrimaryAction = 11,
    SecondaryAction = 12,
    SpecialAction = 13
}

[CreateAssetMenu(
    fileName = "CharacterAudio_",
    menuName = "Rescuers2D/Audio/Character Audio Profile")]
public sealed class CharacterAudioProfileData
    : ScriptableObject
{
    private const float MinimumPitch = 0.01f;
    private const float MaximumPitch = 3f;

    [Header("Character-Wide Tuning")]
    [Tooltip(
        "Multiplies the selected cue volume for every event in this profile.")]
    [Range(0f, 2f)]
    [SerializeField]
    private float volumeMultiplier = 1f;

    [Tooltip(
        "Multiplies the selected cue pitch for every event in this profile.")]
    [Range(MinimumPitch, MaximumPitch)]
    [SerializeField]
    private float pitchMultiplier = 1f;

    [Header("Movement")]
    [SerializeField]
    private SfxCueData jump;

    [SerializeField]
    private SfxCueData land;

    [SerializeField]
    private SfxCueData footstep;

    [SerializeField]
    private SfxCueData climbStep;

    [SerializeField]
    private SfxCueData swimStroke;

    [Tooltip(
        "Assign an SfxCueData asset whose Loop option is enabled.")]
    [SerializeField]
    private SfxCueData swimLoop;

    [Header("Health")]
    [SerializeField]
    private SfxCueData hurt;

    [SerializeField]
    private SfxCueData death;

    [Header("Interaction")]
    [SerializeField]
    private SfxCueData interact;

    [SerializeField]
    private SfxCueData pickup;

    [SerializeField]
    private SfxCueData drop;

    [Header("Character Actions")]
    [Tooltip(
        "The character's main action, such as axe swing or shield bash.")]
    [SerializeField]
    private SfxCueData primaryAction;

    [Tooltip(
        "The character's supporting action, such as shield brace.")]
    [SerializeField]
    private SfxCueData secondaryAction;

    [Tooltip(
        "A character-specific ability or state transition.")]
    [SerializeField]
    private SfxCueData specialAction;

    public float VolumeMultiplier =>
        volumeMultiplier;

    public float PitchMultiplier =>
        pitchMultiplier;

    public SfxCueData GetCue(
        CharacterAudioEvent audioEvent)
    {
        switch (audioEvent)
        {
            case CharacterAudioEvent.Jump:
                return jump;

            case CharacterAudioEvent.Land:
                return land;

            case CharacterAudioEvent.Hurt:
                return hurt;

            case CharacterAudioEvent.Death:
                return death;

            case CharacterAudioEvent.Footstep:
                return footstep;

            case CharacterAudioEvent.SwimStroke:
                return swimStroke;

            case CharacterAudioEvent.SwimLoop:
                return swimLoop;

            case CharacterAudioEvent.ClimbStep:
                return climbStep;

            case CharacterAudioEvent.Interact:
                return interact;

            case CharacterAudioEvent.Pickup:
                return pickup;

            case CharacterAudioEvent.Drop:
                return drop;

            case CharacterAudioEvent.PrimaryAction:
                return primaryAction;

            case CharacterAudioEvent.SecondaryAction:
                return secondaryAction;

            case CharacterAudioEvent.SpecialAction:
                return specialAction;

            default:
                return null;
        }
    }

    public bool TryGetCue(
        CharacterAudioEvent audioEvent,
        out SfxCueData cue)
    {
        cue =
            GetCue(
                audioEvent);

        return cue != null;
    }

    private void OnValidate()
    {
        volumeMultiplier =
            Mathf.Max(
                0f,
                volumeMultiplier);

        pitchMultiplier =
            Mathf.Clamp(
                pitchMultiplier,
                MinimumPitch,
                MaximumPitch);
    }
}

//----- CharacterAudioProfileData.cs END -----