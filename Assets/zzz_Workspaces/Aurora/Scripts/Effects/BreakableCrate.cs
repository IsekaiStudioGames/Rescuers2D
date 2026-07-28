using UnityEngine;

public class BreakableCrate : MonoBehaviour {

    [Header("Debris Settings")]
    public GameObject[] debrisPrefabs; 
    public int totalPiecesToSpawn = 4;
    public float explosionScatterForce = 5f;

    [ContextMenu("Break Crate")]
    public void Break() {

        for (int i = 0; i < totalPiecesToSpawn; i++) {
            GameObject newDebris = Instantiate(debrisPrefabs[i], transform.position, Quaternion.identity);
            Rigidbody2D rb = newDebris.GetComponent<Rigidbody2D>();
            Vector2 randomDirection = Random.insideUnitCircle.normalized;
            rb.AddForce(randomDirection * explosionScatterForce, ForceMode2D.Impulse);
        }
        gameObject.SetActive(false);
        Destroy(gameObject, 10f);
    }
}