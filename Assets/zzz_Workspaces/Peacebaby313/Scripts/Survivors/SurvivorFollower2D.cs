//----- SurvivorFollower2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody2D))]
public sealed class SurvivorFollower2D : MonoBehaviour
{
    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private Animator animator;

    [Header("Follow Target")]
    [Tooltip("The Firefighter or other character being followed.")]
    [SerializeField]
    private Transform followTarget;

    [Tooltip(
        "The survivor stops moving when within this " +
        "horizontal distance of the target.")]
    [SerializeField, Min(0f)]
    private float stoppingDistance = 1.25f;

    [Tooltip(
        "The survivor starts moving again when the target " +
        "moves beyond this distance.")]
    [SerializeField, Min(0f)]
    private float resumeDistance = 1.75f;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float moveSpeed = 3f;

    [SerializeField, Min(0f)]
    private float acceleration = 20f;

    [SerializeField, Min(0f)]
    private float deceleration = 30f;

    [Header("Vertical Follow Limits")]
    [Tooltip(
        "The survivor will not walk toward the target while " +
        "the target is significantly above or below them. " +
        "Ladder behavior will handle that later.")]
    [SerializeField, Min(0f)]
    private float maximumWalkingHeightDifference = 1.5f;

    [Header("Facing")]
    [Tooltip(
        "Enable this if the survivor sprite normally faces right.")]
    [SerializeField]
    private bool spriteFacesRight = true;

    [Header("Animator Parameters")]
    [SerializeField]
    private string movementSpeedParameter = "MoveSpeed";

    [SerializeField]
    private string groundedParameter = "IsGrounded";

    [Header("Ground Detection")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField, Min(0.01f)]
    private float groundCheckRadius = 0.15f;

    [SerializeField]
    private LayerMask groundLayers;

    [Header("Runtime State")]
    [SerializeField]
    private bool isFollowing = true;

    [SerializeField]
    private bool isMoving;

    [SerializeField]
    private bool isGrounded;

    private float movementDirection;

    public Transform FollowTarget => followTarget;
    public bool IsFollowing => isFollowing;
    public bool IsMoving => isMoving;
    public bool IsGrounded => isGrounded;

    private void Reset()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer =
            GetComponentInChildren<SpriteRenderer>();
        animator =
            GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer =
                GetComponentInChildren<SpriteRenderer>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        UpdateGroundedState();
        CalculateFollowMovement();
        UpdateFacing();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        ApplyHorizontalMovement();
    }

    public void SetFollowTarget(Transform newTarget)
    {
        followTarget = newTarget;
    }

    public void BeginFollowing(Transform newTarget)
    {
        if (newTarget != null)
        {
            followTarget = newTarget;
        }

        isFollowing = followTarget != null;
    }

    public void StopFollowing()
    {
        isFollowing = false;
        movementDirection = 0f;
        isMoving = false;
    }

    private void CalculateFollowMovement()
    {
        movementDirection = 0f;
        isMoving = false;

        if (!isFollowing ||
            followTarget == null)
        {
            return;
        }

        Vector2 difference =
            followTarget.position - transform.position;

        if (Mathf.Abs(difference.y) >
            maximumWalkingHeightDifference)
        {
            return;
        }

        float horizontalDistance =
            Mathf.Abs(difference.x);

        float requiredDistance =
            isMoving
                ? stoppingDistance
                : resumeDistance;

        if (horizontalDistance <= requiredDistance)
        {
            return;
        }

        movementDirection =
            Mathf.Sign(difference.x);

        isMoving = true;
    }

    private void ApplyHorizontalMovement()
    {
        if (rb == null)
        {
            return;
        }

        float targetVelocity =
            movementDirection * moveSpeed;

        float rate =
            isMoving
                ? acceleration
                : deceleration;

        float newHorizontalVelocity =
            Mathf.MoveTowards(
                rb.linearVelocity.x,
                targetVelocity,
                rate * Time.fixedDeltaTime);

        rb.linearVelocity = new Vector2(
            newHorizontalVelocity,
            rb.linearVelocity.y);
    }

    private void UpdateGroundedState()
    {
        if (groundCheck == null)
        {
            isGrounded = true;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayers);
    }

    private void UpdateFacing()
    {
        if (!isMoving ||
            spriteRenderer == null)
        {
            return;
        }

        bool movingRight =
            movementDirection > 0f;

        spriteRenderer.flipX =
            spriteFacesRight
                ? !movingRight
                : movingRight;
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        if (!string.IsNullOrWhiteSpace(
                movementSpeedParameter))
        {
            animator.SetFloat(
                movementSpeedParameter,
                Mathf.Abs(rb.linearVelocity.x));
        }

        if (!string.IsNullOrWhiteSpace(
                groundedParameter))
        {
            animator.SetBool(
                groundedParameter,
                isGrounded);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.color =
            isGrounded
                ? Color.green
                : Color.red;

        Gizmos.DrawWireSphere(
            groundCheck.position,
            groundCheckRadius);
    }

    private void OnValidate()
    {
        stoppingDistance =
            Mathf.Max(0f, stoppingDistance);

        resumeDistance =
            Mathf.Max(
                stoppingDistance,
                resumeDistance);

        moveSpeed = Mathf.Max(0f, moveSpeed);
        acceleration = Mathf.Max(0f, acceleration);
        deceleration = Mathf.Max(0f, deceleration);

        groundCheckRadius =
            Mathf.Max(0.01f, groundCheckRadius);
    }
}

//----- SurvivorFollower2D.cs END -----