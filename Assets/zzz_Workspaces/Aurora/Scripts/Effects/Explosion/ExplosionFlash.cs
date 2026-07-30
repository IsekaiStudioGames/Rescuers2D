using UnityEngine;

public class ExplosionFlash : MonoBehaviour {

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private float expansionSpeed = 50f;
    [SerializeField] private float duration = 0.15f;
    private float timer = 0f;

    private void Start() {
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localScale = Vector3.zero;
    }
    private void Update() {

        timer += Time.deltaTime;
        if (timer >= duration) { 
            Destroy(gameObject);
            return;
        }
        transform.localScale += Vector3.one * expansionSpeed * Time.deltaTime;

        Color color = spriteRenderer.color;
        color.a = Mathf.Lerp(1f, 0f, timer / duration);
        spriteRenderer.color = color;
    }
}