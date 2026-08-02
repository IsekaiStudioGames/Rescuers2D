//----- C4Charge2D.cs START -----

using System;
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(WorldAudioEmitter))]
public sealed class C4Charge2D : MonoBehaviour
{
    private static readonly int ArmedHash =
        Animator.StringToHash("Armed");

    private static readonly int ExplodeHash =
        Animator.StringToHash("Explode");

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
        "The point used as the center of the tile explosion. " +
        "If omitted, this object's position is used.")]
    [SerializeField]
    private Transform explosionPoint;

    [Tooltip(
        "Optional direct reference to the destructible Tilemap. " +
        "If omitted, the charge searches the scene when it explodes.")]
    [SerializeField]
    private DestructibleTilemap2D destructibleTilemap;

    [Header("Audio")]
    [Tooltip("Played once after the placement transaction succeeds.")]
    [SerializeField]
    private SfxCueData placementCue;

    [Tooltip("Optional one-shot played when the fuse is armed.")]
    [SerializeField]
    private SfxCueData armedCue;

    [Tooltip("Detached cue played at the exact explosion position.")]
    [SerializeField]
    private SfxCueData explosionCue;

    [Header("Cleanup")]
    [Tooltip(
        "Fallback destruction delay when no animation event is used.")]
    [SerializeField, Min(0.1f)]
    private float cleanupDelay = 1f;

    private bool hasPlayedPlacementFeedback;
    private bool hasBeenArmed;
    private bool hasExploded;

    public event Action OnExploded;

    private void Reset()
    {
        ResolveReferences();
    }

    private void Awake()
    {
        ResolveReferences();
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
        if (hasBeenArmed)
        {
            return;
        }

        hasBeenArmed =
            true;

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
            new WaitForSeconds(fuseDuration);

        Explode();
    }

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

        Debug.Log(
            $"C4 exploded at {explosionPosition} with radius " +
            $"{explosionRadius}.",
            this);

        if (animator != null)
        {
            animator.SetTrigger(
                ExplodeHash);
        }

        // Detached pooled playback survives either cleanup path.
        worldAudio?.PlayAtPosition(
            explosionCue,
            explosionPosition);

        DamageDestructibleTiles(
            explosionPosition);

        OnExploded?.Invoke();

        Destroy(
            gameObject,
            cleanupDelay);
    }

    private void DamageDestructibleTiles(
        Vector2 explosionPosition)
    {
        if (destructibleTilemap != null)
        {
            DamageTilemap(
                destructibleTilemap,
                explosionPosition);

            return;
        }

        DestructibleTilemap2D[] tilemaps =
            FindObjectsByType<DestructibleTilemap2D>(
                FindObjectsSortMode.None);

        Debug.Log(
            $"C4 found {tilemaps.Length} destructible tilemap(s).",
            this);

        foreach (DestructibleTilemap2D currentTilemap
                 in tilemaps)
        {
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
                $"C4 damaged {damagedTiles} destructible tile(s) " +
                $"on '{targetTilemap.name}'.",
                this);
        }
    }

    public void Anim_DestroyCharge()
    {
        Destroy(gameObject);
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
                GetComponent<Animator>();
        }

        if (worldAudio == null)
        {
            worldAudio =
                GetComponent<WorldAudioEmitter>();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(
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
            Mathf.Max(0f, fuseDuration);

        explosionRadius =
            Mathf.Max(0.01f, explosionRadius);

        explosionDamage =
            Mathf.Max(1, explosionDamage);

        cleanupDelay =
            Mathf.Max(0.1f, cleanupDelay);

        ResolveReferences();
    }
}

//----- C4Charge2D.cs END -----