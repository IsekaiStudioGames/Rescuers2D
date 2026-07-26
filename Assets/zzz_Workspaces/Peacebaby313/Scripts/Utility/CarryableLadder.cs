using UnityEngine;

public class CarryableLadder : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;

    [Tooltip("The ladder colliders used for physical collision.")]
    [SerializeField] private Collider2D[] physicalColliders;

    [Header("Attachment")]
    [Tooltip(
        "The point on the ladder that snaps to the " +
        "Firefighter's LadderHoldPoint."
    )]
    [SerializeField] private Transform firefighterAttachPoint;

    [Header("Runtime State")]
    [SerializeField]
    private LadderState currentState =
        LadderState.Placed;

    private Transform originalParent;
    private RigidbodyType2D originalBodyType;
    private float originalGravityScale;
    private bool originalSimulated;
    private bool hasCachedPhysicsSettings;

    public enum LadderState
    {
        Placed,
        Carried
    }

    public LadderState CurrentState => currentState;

    public bool IsCarried =>
        currentState == LadderState.Carried;

    public bool CanBePickedUp =>
        currentState == LadderState.Placed;

    public bool CanBeClimbed =>
        currentState == LadderState.Placed;

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        originalParent = transform.parent;
        CachePhysicsSettings();
    }

    public void AttachTo(Transform firefighterHoldPoint)
    {
        if (firefighterHoldPoint == null ||
            firefighterAttachPoint == null ||
            IsCarried)
        {
            return;
        }

        CachePhysicsSettings();

        currentState = LadderState.Carried;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.simulated = false;
        }

        SetPhysicalCollidersEnabled(false);

        transform.SetParent(
            firefighterHoldPoint,
            true
        );

        AlignAttachmentPoints(
            firefighterHoldPoint
        );
    }

    public void Detach()
    {
        if (!IsCarried)
        {
            return;
        }

        Vector3 worldPosition = transform.position;
        Quaternion worldRotation = transform.rotation;
        Vector3 worldScale = transform.lossyScale;

        transform.SetParent(originalParent, true);

        transform.position = worldPosition;
        transform.rotation = worldRotation;

        SetWorldScale(worldScale);

        currentState = LadderState.Placed;

        SetPhysicalCollidersEnabled(true);

        if (rb != null)
        {
            rb.bodyType = originalBodyType;
            rb.gravityScale = originalGravityScale;
            rb.simulated = originalSimulated;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    private void AlignAttachmentPoints(
        Transform firefighterHoldPoint)
    {
        Quaternion rotationDifference =
            firefighterHoldPoint.rotation *
            Quaternion.Inverse(
                firefighterAttachPoint.rotation
            );

        transform.rotation =
            rotationDifference *
            transform.rotation;

        Vector3 positionDifference =
            firefighterHoldPoint.position -
            firefighterAttachPoint.position;

        transform.position += positionDifference;
    }

    private void CachePhysicsSettings()
    {
        if (rb == null || hasCachedPhysicsSettings)
        {
            return;
        }

        originalBodyType = rb.bodyType;
        originalGravityScale = rb.gravityScale;
        originalSimulated = rb.simulated;
        hasCachedPhysicsSettings = true;
    }

    private void SetPhysicalCollidersEnabled(bool enabled)
    {
        if (physicalColliders == null)
        {
            return;
        }

        foreach (Collider2D physicalCollider
                 in physicalColliders)
        {
            if (physicalCollider != null)
            {
                physicalCollider.enabled = enabled;
            }
        }
    }

    private void SetWorldScale(Vector3 desiredWorldScale)
    {
        Transform parent = transform.parent;

        if (parent == null)
        {
            transform.localScale = desiredWorldScale;
            return;
        }

        Vector3 parentScale = parent.lossyScale;

        transform.localScale = new Vector3(
            SafeDivide(
                desiredWorldScale.x,
                parentScale.x
            ),
            SafeDivide(
                desiredWorldScale.y,
                parentScale.y
            ),
            SafeDivide(
                desiredWorldScale.z,
                parentScale.z
            )
        );
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Approximately(divisor, 0f))
        {
            return value;
        }

        return value / divisor;
    }
}