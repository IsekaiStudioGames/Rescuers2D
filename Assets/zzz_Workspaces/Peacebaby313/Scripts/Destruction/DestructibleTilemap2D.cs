//----- DestructibleTilemap2D.cs START -----

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[DisallowMultipleComponent]
[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public sealed class DestructibleTilemap2D :
    MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField]
    private Tilemap tilemap;

    [Header("Destructible Tile Profiles")]
    [SerializeField]
    private DestructibleTileProfile[] tileProfiles;

    [Header("Effect Placement")]
    [Tooltip(
        "The world-space Z position used when spawning effects.")]
    [SerializeField]
    private float effectZPosition;

    private readonly Dictionary<
        TileBase,
        DestructibleTileProfile> profileLookup = new();

    private readonly Dictionary<
        Vector3Int,
        int> remainingTileHealth = new();

    private void Awake()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }

        BuildProfileLookup();
    }

    public bool TryDamageTile(
        Vector2 worldPosition,
        DestructionDamageType damageType,
        int damageAmount = 1)
    {
        if (tilemap == null ||
            damageAmount <= 0)
        {
            return false;
        }

        Vector3Int cellPosition =
            tilemap.WorldToCell(worldPosition);

        return TryDamageCell(
            cellPosition,
            damageType,
            damageAmount);
    }

    public bool TryDamageCell(
        Vector3Int cellPosition,
        DestructionDamageType damageType,
        int damageAmount = 1)
    {
        if (tilemap == null ||
            damageAmount <= 0)
        {
            return false;
        }

        TileBase currentTile =
            tilemap.GetTile(cellPosition);

        if (currentTile == null)
        {
            return false;
        }

        Debug.Log(
            $"C4 checked cell {cellPosition} on '{tilemap.name}'. " +
            $"Found tile asset: '{currentTile.name}'.",
            this);

        if (!TryGetProfile(
                currentTile,
                out DestructibleTileProfile profile))
        {
            Debug.LogWarning(
                $"Tile '{currentTile.name}' at {cellPosition} has no " +
                "registered destruction profile.",
                this);

            return false;
        }

        if (currentTile == null)
        {
            return false;
        }



        if (!profile.AcceptsDamageType(damageType))
        {
            Debug.Log(
                $"Tile at {cellPosition} cannot be damaged " +
                $"by {damageType}.",
                this);

            return false;
        }

        if (!remainingTileHealth.TryGetValue(
                cellPosition,
                out int remainingHealth))
        {
            remainingHealth = profile.HitsToBreak;
        }

        remainingHealth -= damageAmount;

        if (remainingHealth <= 0)
        {
            BreakTile(cellPosition, profile);
            return true;
        }

        remainingTileHealth[cellPosition] =
            remainingHealth;

        DamageTile(
            cellPosition,
            profile,
            remainingHealth);

        return true;
    }

    public int DamageArea(
        Vector2 worldPosition,
        float radius,
        DestructionDamageType damageType,
        int damageAmount = 1)
    {
        if (tilemap == null ||
            radius <= 0f ||
            damageAmount <= 0)
        {
            return 0;
        }

        Vector3 bottomLeftWorld = new(
            worldPosition.x - radius,
            worldPosition.y - radius,
            tilemap.transform.position.z);

        Vector3 topRightWorld = new(
            worldPosition.x + radius,
            worldPosition.y + radius,
            tilemap.transform.position.z);

        Vector3Int minimumCell =
            tilemap.WorldToCell(bottomLeftWorld);

        Vector3Int maximumCell =
            tilemap.WorldToCell(topRightWorld);

        int occupiedCellCount = 0;
        int cellsInsideRadius = 0;
        int damagedTileCount = 0;

        Debug.Log(
            $"C4 damage scan on '{tilemap.name}': " +
            $"world position {worldPosition}, radius {radius}, " +
            $"cells {minimumCell} through {maximumCell}.",
            this);

        for (int x = minimumCell.x;
             x <= maximumCell.x;
             x++)
        {
            for (int y = minimumCell.y;
                 y <= maximumCell.y;
                 y++)
            {
                Vector3Int cellPosition =
                    new(x, y, minimumCell.z);

                TileBase currentTile =
                    tilemap.GetTile(cellPosition);

                if (currentTile == null)
                {
                    continue;
                }

                occupiedCellCount++;

                Vector3 cellCenter =
                    tilemap.GetCellCenterWorld(cellPosition);

                float distance =
                    Vector2.Distance(
                        worldPosition,
                        cellCenter);

                Debug.Log(
                    $"C4 found occupied cell {cellPosition}: " +
                    $"tile '{currentTile.name}', center {cellCenter}, " +
                    $"distance {distance:F2}.",
                    this);

                if (distance > radius)
                {
                    continue;
                }

                cellsInsideRadius++;

                if (TryDamageCell(
                        cellPosition,
                        damageType,
                        damageAmount))
                {
                    damagedTileCount++;
                }
            }
        }

        Debug.Log(
            $"C4 scan complete on '{tilemap.name}'. " +
            $"Occupied cells in scan bounds: {occupiedCellCount}. " +
            $"Occupied cells inside radius: {cellsInsideRadius}. " +
            $"Damaged tiles: {damagedTileCount}.",
            this);

        return damagedTileCount;
    }

    private void DamageTile(
        Vector3Int cellPosition,
        DestructibleTileProfile profile,
        int remainingHealth)
    {
        if (profile.DamagedTile != null)
        {
            tilemap.SetTile(
                cellPosition,
                profile.DamagedTile);

            tilemap.RefreshTile(cellPosition);
        }

        SpawnEffect(
            profile.HitEffectPrefab,
            cellPosition);

        Debug.Log(
            $"Tile at {cellPosition} took damage. " +
            $"{remainingHealth} hit(s) remain.",
            this);
    }

    private void BreakTile(
        Vector3Int cellPosition,
        DestructibleTileProfile profile)
    {
        SpawnEffect(
            profile.DestructionEffectPrefab,
            cellPosition);

        tilemap.SetTile(cellPosition, null);
        tilemap.RefreshTile(cellPosition);

        remainingTileHealth.Remove(cellPosition);

        Debug.Log(
            $"Tile at {cellPosition} was destroyed.",
            this);
    }

    private void SpawnEffect(
        ParticleSystem effectPrefab,
        Vector3Int cellPosition)
    {
        if (effectPrefab == null)
        {
            return;
        }

        Vector3 spawnPosition =
            tilemap.GetCellCenterWorld(cellPosition);

        spawnPosition.z = effectZPosition;

        ParticleSystem spawnedEffect =
            Instantiate(
                effectPrefab,
                spawnPosition,
                Quaternion.identity);

        ParticleSystem.MainModule main =
            spawnedEffect.main;

        float destroyDelay =
            main.duration +
            main.startLifetime.constantMax;

        Destroy(
            spawnedEffect.gameObject,
            Mathf.Max(0.1f, destroyDelay));
    }

    private bool TryGetProfile(
        TileBase tile,
        out DestructibleTileProfile profile)
    {
        if (profileLookup.TryGetValue(tile, out profile))
        {
            return true;
        }

        profile = null;
        return false;
    }

    private void BuildProfileLookup()
    {
        profileLookup.Clear();

        if (tileProfiles == null)
        {
            return;
        }

        foreach (DestructibleTileProfile profile
                 in tileProfiles)
        {
            if (profile == null)
            {
                continue;
            }

            RegisterTile(
                profile.IntactTile,
                profile);

            // The cracked tile must resolve to the same profile
            // when it receives the next hit.
            RegisterTile(
                profile.DamagedTile,
                profile);
        }
    }

    private void RegisterTile(
        TileBase tile,
        DestructibleTileProfile profile)
    {
        if (tile == null)
        {
            return;
        }

        if (profileLookup.ContainsKey(tile))
        {
            Debug.LogWarning(
                $"Multiple destruction profiles use tile " +
                $"'{tile.name}'. The first profile will be used.",
                this);

            return;
        }

        profileLookup.Add(tile, profile);
    }

    private void OnValidate()
    {
        if (tilemap == null)
        {
            tilemap = GetComponent<Tilemap>();
        }
    }
}

//----- DestructibleTilemap2D.cs END -----