using System.Collections.Generic;
using UnityEngine;

public class DamageTrigger2D : MonoBehaviour
{
    [Header("Damage")]
    //[SerializeField] private int damage = 1;
    [SerializeField] private LayerMask targetLayers = ~0;

    [Header("Temporary Debris Support")]
    [SerializeField] private bool destroyDebris = true;
    [SerializeField] private string debrisTag = "Debris";

    [Header("Components")]
    [SerializeField] private Collider2D damageCollider;

    private readonly HashSet<GameObject> hitObjects =
        new HashSet<GameObject>();

    private bool damageEnabled;

    private void Awake()
    {
        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider2D>();
        }

        DisableDamage();
    }

    public void BeginNewAttack()
    {
        hitObjects.Clear();
        DisableDamage();
    }

    public void EnableDamage()
    {
        damageEnabled = true;

        if (damageCollider != null)
        {
            damageCollider.enabled = true;
        }
    }

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
        if (!damageEnabled || other == null)
        {
            return;
        }

        if (!IsLayerIncluded(other.gameObject.layer))
        {
            return;
        }

        GameObject target =
            other.attachedRigidbody != null
                ? other.attachedRigidbody.gameObject
                : other.transform.root.gameObject;

        if (hitObjects.Contains(target))
        {
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
        //damageable.TakeDamage(damage);
    }

    private bool IsLayerIncluded(int layer)
    {
        return (targetLayers.value & (1 << layer)) != 0;
    }
}