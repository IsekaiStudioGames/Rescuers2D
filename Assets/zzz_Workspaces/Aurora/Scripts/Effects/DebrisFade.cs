using UnityEngine;

public class DebrisFade : MonoBehaviour {

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float lifetime = 2f;
    [SerializeField] private float timer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        GetComponent<Rigidbody2D>().AddTorque(Random.Range(-50f, 50f));
    }
    private void Update() {

        timer += Time.deltaTime;
        if (timer >= lifetime) {
            Destroy(gameObject);
            return;
        }
        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(1f, 0f, timer / lifetime);
        spriteRenderer.color = color;
    }
}