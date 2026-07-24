using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(InteractableWater), typeof(Rigidbody2D))]
public class WaterTriggerHandler : MonoBehaviour{

    [SerializeField] private LayerMask water;
    [SerializeField] private GameObject splashParticles;

    private EdgeCollider2D edgeCollider;

    private InteractableWater interactableWater;

    private void Awake() {
        
        edgeCollider = GetComponent<EdgeCollider2D>();
        interactableWater = GetComponent<InteractableWater>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        
        if ((water.value & (1 << other.gameObject.layer)) > 0) {
            Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();

            if (rb != null) {

                Vector2 localPosition = gameObject.transform.localPosition;
                Vector2 hitPosition = other.transform.position;
                Bounds hitBounds = other.bounds;

                Vector3 spawnPosition = Vector3.zero;
                if (other.transform.position.y >= edgeCollider.points[1].y + edgeCollider.offset.y + localPosition.y) {

                    spawnPosition = hitPosition - new Vector2(0f, hitBounds.extents.y);
                } else {

                    spawnPosition = hitPosition + new Vector2(0f, hitBounds.extents.y);
                }
                Instantiate(splashParticles, spawnPosition, Quaternion.identity);

                int multiplier = 1;
                if (rb.linearVelocity.y < 0) { multiplier = -1; }
                else { multiplier = 1; }

                float _velocity = rb.linearVelocity.y * interactableWater.ForceMultiplier;
                _velocity = Mathf.Clamp(Mathf.Abs(_velocity), 0f, interactableWater.MaxForce);
                _velocity *= multiplier;

                interactableWater.Splash(other, _velocity);
            }
        }
    }
}