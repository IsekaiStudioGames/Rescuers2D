using UnityEngine;

public class GroundCheck : MonoBehaviour {

    [Header("Check Settings")]
    [SerializeField] LayerMask groundLayer = 1 << 3;
    [SerializeField] private float skinWidth = 0.05f;
    [SerializeField] private float sphereRadius = 0.22f;
    [SerializeField] private float maxSlopeAngle = 45f;

    public float SlopeAngle { get; private set; }
    public Vector2 GroundNormal { get; private set; }
    public bool IsGround { get; private set; }

    private void FixedUpdate() => CheckForGround();

    private void CheckForGround() {
        
        var origin = transform.position + Vector3.down * skinWidth;
        IsGround = Physics2D.OverlapCircle(origin, sphereRadius, groundLayer);

        if (IsGround ) {

            var hit = Physics2D.Raycast(origin, Vector2.down, 0.3f, groundLayer);
            GroundNormal = hit ? hit.normal : Vector2.up;
            SlopeAngle = Vector2.Angle(GroundNormal, Vector2.up);
            if (SlopeAngle > maxSlopeAngle) IsGround = false;

        } else {
            GroundNormal = Vector2.up;
            SlopeAngle = 0f;
        }
    }
}