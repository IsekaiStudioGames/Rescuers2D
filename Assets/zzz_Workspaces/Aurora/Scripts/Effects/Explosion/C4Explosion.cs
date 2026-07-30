using UnityEngine;

public class C4Explosion : MonoBehaviour {

    [Header("Prefabs")]
    [SerializeField] private GameObject flashPrefab;
    [SerializeField] private GameObject smokeParticlePrefab;

    [SerializeField] private float blastRadius = 5f;
    [SerializeField] private float explosionForce = 800f;

    [ContextMenu("Detonate")]
    public void Detonate() {

        Instantiate(flashPrefab, transform.position, Quaternion.identity);
        Instantiate(smokeParticlePrefab, transform.position, Quaternion.identity);

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius);

        foreach (Collider2D hit in colliders) {
            Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
            if (rb != null) {

                Vector2 direction = (hit.transform.position - transform.position).normalized;

                float distance = Vector2.Distance(transform.position, hit.transform.position);
                float forceMultiplier = Mathf.Clamp01(1f - (distance / blastRadius));

                rb.AddForce(direction * explosionForce * forceMultiplier, ForceMode2D.Impulse);
            }
        }
        Destroy(gameObject);
    }
    private void OnDrawGizmosSelected() {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}