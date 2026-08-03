//----- C4Charge2D.cs START -----

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(WorldAudioEmitter))]
public sealed class C4Charge2D : MonoBehaviour
{
    private static readonly int ArmedHash =
        Animator.StringToHash("Armed");

    private static readonly int ExplodeHash =
        Animator.StringToHash("Explode");

    private static readonly List<C4Charge2D>
        ActiveCharges = new();

    [Header("Components")]
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private WorldAudioEmitter worldAudio;

    [Header("Explosion")]
    [SerializeField, Min(0f)]
    private float fuseDuration = 2f;

    [SerializeField, Min(0.01f)]
    private float explosionRadius = 1.5f;

    [SerializeField, Min(1)]
    private int explosionDamage = 1;

    [Tooltip(
        "The center of the explosion. The C4 position is used " +
        "when this is unassigned.")]
    [SerializeField]
    private Transform explosionPoint;

    [Tooltip(
        "The Tilemap selected during placement. Other destructible " +
        "Tilemaps inside the blast are also checked.")]
    [SerializeField]
    private DestructibleTilemap2D destructibleTilemap;

    [Header("Physical Blast")]
    [SerializeField, Min(0f)]
    private float explosionForce = 800f;

    [SerializeField]
    private LayerMask physicsBlastLayers = ~0;

    [Header("Explosion Effects")]
    [SerializeField]
    private GameObject flashPrefab;

    [SerializeField]
    private GameObject smokeParticlePrefab;

    [SerializeField, Min(0.1f)]
    private float effectCleanupDelay = 2f;

    [Header("Audio")]
    [Tooltip(
        "Played once after placement and inventory consumption succeed.")]
    [SerializeField]
    private SfxCueData placementCue;

    [Tooltip(
        "Played once when the fuse begins.")]
    [SerializeField]
    private SfxCueData armedCue;

    [Tooltip(
        "Played at the world-space explosion position.")]
    [SerializeField]
    private SfxCueData explosionCue;

    [Header("Cleanup")]
    [Tooltip(
        "How long the C4 object remains after exploding. " +
        "Set this to the length of its explosion animation.")]
    [SerializeField, Min(0.1f)]
    private float cleanupDelay = 1f;

    private bool hasPlayedPlacementFeedback;
    private bool hasBeenArmed;
    private bool hasExploded;

    public bool HasBeenArmed =>
        hasBeenArmed;

    public bool HasExploded =>
        hasExploded;

    public event Action OnExploded;

    [RuntimeInitializeOnLoadMethod(
        RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStaticState()
    {
        ActiveCharges.Clear();
    }

    private void Reset()
    {
        ResolveReferences();
    }

    private void Awake()
    {
        ResolveReferences();
    }

    private void OnEnable()
    {
        if (hasBeenArmed)
        {
            RegisterAsActive();
        }
    }

    private void OnDisable()
    {
        UnregisterAsActive();
    }

    private void OnDestroy()
    {
        UnregisterAsActive();
    }

    public void Initialize(
        DestructibleTilemap2D placementTilemap)
    {
        destructibleTilemap =
            placementTilemap;
    }

    /// <summary>
    /// Returns true while an armed C4 still exists within the
    /// supplied blocking radius.
    /// </summary>
    public static bool IsPlacementBlocked(
        Vector2 requestedPosition,
        float blockingRadius)
    {
        float safeRadius =
            Mathf.Max(
                0.01f,
                blockingRadius);

        float squaredRadius =
            safeRadius * safeRadius;

        for (int index = ActiveCharges.Count - 1;
             index >= 0;
             index--)
        {
            C4Charge2D charge =
                ActiveCharges[index];

            if (charge == null ||
                !charge.isActiveAndEnabled)
            {
                ActiveCharges.RemoveAt(index);
                continue;
            }

            Vector2 chargePosition =
                charge.GetExplosionPosition();

            float squaredDistance =
                (chargePosition - requestedPosition)
                .sqrMagnitude;

            if (squaredDistance <= squaredRadius)
            {
                return true;
            }
        }

        return false;
    }

    public void PlayPlacementFeedback()
    {
        if (hasPlayedPlacementFeedback)
        {
            return;
        }

        hasPlayedPlacementFeedback =
            true;

        worldAudio?.PlayAtOrigin(
            placementCue);
    }

    public void Arm()
    {
        if (hasBeenArmed ||
            hasExploded)
        {
            return;
        }

        hasBeenArmed =
            true;

        RegisterAsActive();

        if (animator != null)
        {
            animator.SetTrigger(
                ArmedHash);
        }

        worldAudio?.PlayAtOrigin(
            armedCue);

        StartCoroutine(
            FuseRoutine());
    }

    private IEnumerator FuseRoutine()
    {
        yield return
            new WaitForSeconds(
                fuseDuration);

        Explode();
    }

    [ContextMenu("Detonate")]
    public void Explode()
    {
        if (hasExploded)
        {
            return;
        }

        hasExploded =
            true;

        Vector3 explosionPosition =
            GetExplosionPosition();

        if (animator != null)
        {
            animator.SetTrigger(
                ExplodeHash);
        }

        // Detached playback survives the destruction of the C4.
        worldAudio?.PlayAtPosition(
            explosionCue,
            explosionPosition);

        SpawnExplosionEffect(
            flashPrefab,
            explosionPosition);

        SpawnExplosionEffect(
            smokeParticlePrefab,
            explosionPosition);

        ApplyPhysicalBlast(
            explosionPosition);

        DamageDestructibleTiles(
            explosionPosition);

        Debug.Log(
            $"C4 exploded at {explosionPosition} with radius " +
            $"{explosionRadius}.",
            this);

        // Schedule cleanup before invoking external listeners.
        Destroy(
            gameObject,
            cleanupDelay);

        OnExploded?.Invoke();
    }

    private void ApplyPhysicalBlast(
        Vector2 explosionPosition)
    {
        if (explosionForce <= 0f)
        {
            return;
        }

        Collider2D[] colliders =
            Physics2D.OverlapCircleAll(
                explosionPosition,
                explosionRadius,
                physicsBlastLayers);

        HashSet<Rigidbody2D> affectedBodies =
            new();

        foreach (Collider2D hit in colliders)
        {
            if (hit == null)
            {
                continue;
            }

            Rigidbody2D body =
                hit.attachedRigidbody;

            if (body == null ||
                body.bodyType != RigidbodyType2D.Dynamic)
            {
                continue;
            }

            if (body.transform.IsChildOf(transform))
            {
                continue;
            }

            // Prevent multiple colliders on one body from multiplying force.
            if (!affectedBodies.Add(body))
            {
                continue;
            }

            Vector2 bodyPosition =
                body.worldCenterOfMass;

            Vector2 direction =
                bodyPosition - explosionPosition;

            float distance =
                direction.magnitude;

            if (distance > explosionRadius)
            {
                continue;
            }

            if (direction.sqrMagnitude <=
                Mathf.Epsilon)
            {
                direction =
                    Vector2.up;
            }
            else
            {
                direction.Normalize();
            }

            float forceMultiplier =
                Mathf.Clamp01(
                    1f -
                    distance / explosionRadius);

            body.AddForce(
                direction *
                explosionForce *
                forceMultiplier,
                ForceMode2D.Impulse);
        }
    }

    private void DamageDestructibleTiles(
        Vector2 explosionPosition)
    {
        HashSet<DestructibleTilemap2D>
            checkedTilemaps = new();

        if (destructibleTilemap != null)
        {
            checkedTilemaps.Add(
                destructibleTilemap);

            DamageTilemap(
                destructibleTilemap,
                explosionPosition);
        }

        DestructibleTilemap2D[] sceneTilemaps =
            FindObjectsByType<DestructibleTilemap2D>(
                FindObjectsSortMode.None);

        foreach (DestructibleTilemap2D currentTilemap
                 in sceneTilemaps)
        {
            if (currentTilemap == null ||
                !checkedTilemaps.Add(currentTilemap))
            {
                continue;
            }

            DamageTilemap(
                currentTilemap,
                explosionPosition);
        }
    }

    private void DamageTilemap(
        DestructibleTilemap2D targetTilemap,
        Vector2 explosionPosition)
    {
        if (targetTilemap == null)
        {
            return;
        }

        int damagedTiles =
            targetTilemap.DamageArea(
                explosionPosition,
                explosionRadius,
                DestructionDamageType.Explosion,
                explosionDamage);

        if (damagedTiles > 0)
        {
            Debug.Log(
                $"C4 damaged {damagedTiles} destructible " +
                $"tile(s) on '{targetTilemap.name}'.",
                this);
        }
    }

    private void SpawnExplosionEffect(
        GameObject effectPrefab,
        Vector3 explosionPosition)
    {
        if (effectPrefab == null)
        {
            return;
        }

        GameObject spawnedEffect =
            Instantiate(
                effectPrefab,
                explosionPosition,
                Quaternion.identity);

        Destroy(
            spawnedEffect,
            effectCleanupDelay);
    }

    private void RegisterAsActive()
    {
        if (!ActiveCharges.Contains(this))
        {
            ActiveCharges.Add(this);
        }
    }

    private void UnregisterAsActive()
    {
        ActiveCharges.Remove(this);
    }

    private Vector3 GetExplosionPosition()
    {
        return explosionPoint != null
            ? explosionPoint.position
            : transform.position;
    }

    private void ResolveReferences()
    {
        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }

        if (worldAudio == null)
        {
            worldAudio =
                GetComponent<WorldAudioEmitter>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color =
            new Color(
                1f,
                0.35f,
                0f,
                0.75f);

        Gizmos.DrawWireSphere(
            GetExplosionPosition(),
            explosionRadius);
    }

    private void OnValidate()
    {
        fuseDuration =
            Mathf.Max(
                0f,
                fuseDuration);

        explosionRadius =
            Mathf.Max(
                0.01f,
                explosionRadius);

        explosionDamage =
            Mathf.Max(
                1,
                explosionDamage);

        explosionForce =
            Mathf.Max(
                0f,
                explosionForce);

        effectCleanupDelay =
            Mathf.Max(
                0.1f,
                effectCleanupDelay);

        cleanupDelay =
            Mathf.Max(
                0.1f,
                cleanupDelay);

        ResolveReferences();
    }
}

//----- C4Charge2D.cs END -----