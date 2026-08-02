//----- DestructibleTileProfile.cs START -----

using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(
    fileName = "DestructibleTileProfile_New",
    menuName =
        "Rescuers2D/Destruction/Destructible Tile Profile")]
public sealed class DestructibleTileProfile :
    ScriptableObject
{
    [Header("Tiles")]
    [Tooltip("The original tile painted onto the Tilemap.")]
    [SerializeField]
    private TileBase intactTile;

    [Tooltip(
        "Optional cracked tile displayed after the first hit. " +
        "Leave empty for walls without a damaged appearance.")]
    [SerializeField]
    private TileBase damagedTile;

    [Header("Durability")]
    [SerializeField, Min(1)]
    private int hitsToBreak = 1;

    [Header("Allowed Damage")]
    [SerializeField]
    private DestructionDamageType[] allowedDamageTypes =
    {
        DestructionDamageType.Axe
    };

    [Header("Effects")]
    [Tooltip("Optional particle prefab spawned on a non-breaking hit.")]
    [SerializeField]
    private ParticleSystem hitEffectPrefab;

    [Tooltip("Particle prefab spawned when the tile breaks.")]
    [SerializeField]
    private ParticleSystem destructionEffectPrefab;

    [Header("Audio")]
    [Tooltip(
        "Cue played at the cell center after accepted " +
        "non-breaking damage.")]
    [SerializeField]
    private SfxCueData impactCue;

    [Tooltip(
        "Detached cue played at the cell center when the tile breaks.")]
    [SerializeField]
    private SfxCueData destructionCue;

    public TileBase IntactTile =>
        intactTile;

    public TileBase DamagedTile =>
        damagedTile;

    public int HitsToBreak =>
        hitsToBreak;

    public ParticleSystem HitEffectPrefab =>
        hitEffectPrefab;

    public ParticleSystem DestructionEffectPrefab =>
        destructionEffectPrefab;

    public SfxCueData ImpactCue =>
        impactCue;

    public SfxCueData DestructionCue =>
        destructionCue;

    public bool AcceptsDamageType(
        DestructionDamageType damageType)
    {
        if (allowedDamageTypes == null)
        {
            return false;
        }

        foreach (DestructionDamageType allowedType
                 in allowedDamageTypes)
        {
            if (allowedType == damageType)
            {
                return true;
            }
        }

        return false;
    }

    private void OnValidate()
    {
        hitsToBreak =
            Mathf.Max(1, hitsToBreak);
    }
}

//----- DestructibleTileProfile.cs END -----