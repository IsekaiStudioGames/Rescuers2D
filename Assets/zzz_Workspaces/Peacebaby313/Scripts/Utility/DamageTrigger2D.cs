//----- DamageTrigger2D.cs START -----

using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class DamageTrigger2D : MonoBehaviour
{
    [Header("Target Detection")]
    [SerializeField]
    private LayerMask targetLayers = ~0;

    [Header("Destructible Tiles")]
    [SerializeField]
    private DestructionDamageType destructionDamageType =
        DestructionDamageType.Axe;

    [SerializeField, Min(1)]
    private int destructionDamage = 1;

    [Tooltip(
        "Moves the calculated hit point slightly inside the struck " +
        "tile so WorldToCell does not select the neighboring cell.")]
    [SerializeField, Min(0f)]
    private float tileHitInset = 0.02f;

    [Header("Temporary Debris Support")]
    [SerializeField]
    private bool destroyDebris = true;

    [SerializeField]
    private string debrisTag = "Debris";

    [Header("Components")]
    [SerializeField]
    private Collider2D damageCollider;

    private readonly HashSet<GameObject> hitObjects =
        new HashSet<GameObject>();

    private bool damageEnabled;

    private void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
        }

        if (damageCollider == null)
        {
            Debug.LogError(
                $"{nameof(DamageTrigger2D)} on '{name}' requires " +
                $"a {nameof(Collider2D)}.",
                this);
        }

        DisableDamage();
    }

    /// <summary>
    /// Clears all targets hit by the previous attack.
    /// Call this when a new axe swing begins.
    /// </summary>
    public void BeginNewAttack()
    {
        hitObjects.Clear();
        DisableDamage();
    }

    /// <summary>
    /// Opens the attack's active damage window.
    /// Usually called by an Animation Event.
    /// </summary>
    public void EnableDamage()
    {
        damageEnabled = true;

        if (damageCollider != null)
        {
            damageCollider.enabled = true;
        }
    }

    /// <summary>
    /// Closes the attack's active damage window.
    /// Usually called by an Animation Event.
    /// </summary>
    public void DisableDamage()
    {
        damageEnabled = false;

        if (damageCollider != null)
        {
            damageCollider.enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        TryDamage(other);
    }

    private void TryDamage(Collider2D other)
    {
        if (!damageEnabled ||
            damageCollider == null ||
            other == null)
        {
            return;
        }

        if (!IsLayerIncluded(other.gameObject.layer))
        {
            return;
        }

        GameObject target = GetTargetObject(other);

        if (target == null ||
            hitObjects.Contains(target))
        {
            return;
        }

        // Check destructible Tilemaps before temporary debris.
        // This prevents accidentally destroying an entire Tilemap
        // GameObject if it happens to use the Debris tag.
        DestructibleTilemap2D destructibleTilemap =
            other.GetComponentInParent<DestructibleTilemap2D>();

        if (destructibleTilemap != null)
        {
            Vector2 hitPoint =
                CalculateTileHitPoint(other);

            bool tileWasDamaged =
                destructibleTilemap.TryDamageTile(
                    hitPoint,
                    destructionDamageType,
                    destructionDamage);

            if (tileWasDamaged)
            {
                // The Tilemap counts as one target for this swing.
                // BeginNewAttack() clears it before the next swing.
                hitObjects.Add(target);
            }

            return;
        }


        DestructibleContainer2D destructibleContainer =
            other.GetComponentInParent<DestructibleContainer2D>();

        if (destructibleContainer != null)
        {
            bool containerWasDamaged =
                destructibleContainer.TryDamage(
                    destructionDamageType,
                    destructionDamage);

            if (containerWasDamaged)
            {
                hitObjects.Add(target);
            }

            return;
        }



        if (destroyDebris &&
            other.CompareTag(debrisTag))
        {
            hitObjects.Add(target);
            Destroy(target);
            return;
        }

        IDamageable damageable =
            other.GetComponentInParent<IDamageable>();

        if (damageable == null)
        {
            return;
        }

        hitObjects.Add(target);

        // Restore this once your IDamageable damage signature
        // has been confirmed:
        //
        // damageable.TakeDamage(damage);
    }

    private Vector2 CalculateTileHitPoint(
        Collider2D other)
    {
        Vector2 attackCenter =
            damageCollider.bounds.center;

        Vector2 closestPoint =
            other.ClosestPoint(attackCenter);

        Vector2 directionIntoTarget =
            closestPoint - attackCenter;

        if (directionIntoTarget.sqrMagnitude >
            Mathf.Epsilon)
        {
            closestPoint +=
                directionIntoTarget.normalized *
                tileHitInset;
        }

        return closestPoint;
    }

    private GameObject GetTargetObject(
        Collider2D other)
    {
        if (other.attachedRigidbody != null)
        {
            return other.attachedRigidbody.gameObject;
        }

        return other.transform.root.gameObject;
    }

    private bool IsLayerIncluded(int layer)
    {
        return
            (targetLayers.value & (1 << layer)) != 0;
    }

    private void OnValidate()
    {
        destructionDamage =
            Mathf.Max(1, destructionDamage);

        tileHitInset =
            Mathf.Max(0f, tileHitInset);

        if (damageCollider == null)
        {
            damageCollider =
                GetComponent<Collider2D>();
        }
    }
}

//----- DamageTrigger2D.cs END -----