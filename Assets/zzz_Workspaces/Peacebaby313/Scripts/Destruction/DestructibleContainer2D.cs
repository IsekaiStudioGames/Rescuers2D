//----- DestructibleContainer2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class DestructibleContainer2D : MonoBehaviour
{
    [Header("Durability")]
    [SerializeField, Min(1)]
    private int hitsToBreak = 1;

    [SerializeField]
    private DestructionDamageType[] allowedDamageTypes =
    {
        DestructionDamageType.Axe,
        DestructionDamageType.Fire,
        DestructionDamageType.Explosion
    };

    [Header("Contained Item")]
    [Tooltip("The item prefab spawned when this container breaks.")]
    [SerializeField]
    private GameObject containedItemPrefab;

    [Tooltip(
        "Optional spawn location. The container's position is used " +
        "when this is not assigned.")]
    [SerializeField]
    private Transform itemSpawnPoint;

    [Header("Effects")]
    [SerializeField]
    private ParticleSystem hitEffectPrefab;

    [SerializeField]
    private ParticleSystem destructionEffectPrefab;

    [Header("Optional Audio")]
    [SerializeField]
    private AudioClip hitSound;

    [SerializeField]
    private AudioClip destructionSound;

    [SerializeField, Range(0f, 1f)]
    private float soundVolume = 1f;

    private int remainingHealth;
    private bool hasBeenDestroyed;

    private void Awake()
    {
        remainingHealth = hitsToBreak;
    }

    public bool TryDamage(
        DestructionDamageType damageType,
        int damageAmount = 1)
    {
        if (hasBeenDestroyed ||
            damageAmount <= 0 ||
            !AcceptsDamageType(damageType))
        {
            return false;
        }

        remainingHealth -= damageAmount;

        if (remainingHealth <= 0)
        {
            BreakContainer();
        }
        else
        {
            PlayHitFeedback();
        }

        return true;
    }

    private void PlayHitFeedback()
    {
        SpawnParticle(hitEffectPrefab);

        PlaySound(
            hitSound,
            transform.position);

        Debug.Log(
            $"'{name}' was damaged. " +
            $"{remainingHealth} hit(s) remain.",
            this);
    }

    private void BreakContainer()
    {
        if (hasBeenDestroyed)
        {
            return;
        }

        hasBeenDestroyed = true;

        Vector3 spawnPosition =
            itemSpawnPoint != null
                ? itemSpawnPoint.position
                : transform.position;

        if (containedItemPrefab != null)
        {
            Instantiate(
                containedItemPrefab,
                spawnPosition,
                Quaternion.identity);
        }

        SpawnParticle(destructionEffectPrefab);

        PlaySound(
            destructionSound,
            transform.position);

        Debug.Log(
            $"'{name}' was destroyed.",
            this);

        Destroy(gameObject);
    }

    private bool AcceptsDamageType(
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

    private void SpawnParticle(
        ParticleSystem particlePrefab)
    {
        if (particlePrefab == null)
        {
            return;
        }

        ParticleSystem spawnedParticle =
            Instantiate(
                particlePrefab,
                transform.position,
                Quaternion.identity);

        ParticleSystem.MainModule main =
            spawnedParticle.main;

        float lifetime =
            main.duration +
            main.startLifetime.constantMax;

        Destroy(
            spawnedParticle.gameObject,
            Mathf.Max(0.1f, lifetime));
    }

    private void PlaySound(
        AudioClip clip,
        Vector3 position)
    {
        if (clip == null)
        {
            return;
        }

        AudioSource.PlayClipAtPoint(
            clip,
            position,
            soundVolume);
    }

    private void OnValidate()
    {
        hitsToBreak = Mathf.Max(1, hitsToBreak);
    }
}

//----- DestructibleContainer2D.cs END -----