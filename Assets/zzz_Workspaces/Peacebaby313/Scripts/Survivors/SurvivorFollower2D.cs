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

    [Header("Player Party")]
    [Tooltip(
        "Used to detect every player and determine which " +
        "character is currently active.")]
    [SerializeField]
    private PlayerInputReader playerInputReader;

    [Header("Rescue Activation")]
    [Tooltip(
        "The survivor remains idle until any playable " +
        "character enters this distance.")]
    [SerializeField, Min(0.1f)]
    private float activationDistance = 4f;

    [Tooltip(
        "If enabled, the survivor begins following immediately. " +
        "Normally leave this disabled.")]
    [SerializeField]
    private bool startFollowingImmediately;

    [Header("Following")]
    [SerializeField, Min(0f)]
    private float stoppingDistance = 1.25f;

    [SerializeField, Min(0f)]
    private float resumeDistance = 1.75f;

    [SerializeField, Min(0f)]
    private float maximumWalkingHeightDifference = 1.5f;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float moveSpeed = 3f;

    [SerializeField, Min(0f)]
    private float acceleration = 20f;

    [SerializeField, Min(0f)]
    private float deceleration = 30f;

    [Header("Ladder Movement")]
    [Tooltip(
        "How quickly the survivor climbs upward after following " +
        "the active player into a ladder.")]
    [SerializeField, Min(0f)]
    private float ladderClimbSpeed = 2.5f;

    [Tooltip(
        "The active player must be at least this far above the " +
        "survivor before upward climbing begins.")]
    [SerializeField, Min(0f)]
    private float minimumClimbHeight = 0.75f;

    [Tooltip(
        "Horizontal speed used to align the survivor with " +
        "the middle of the ladder.")]
    [SerializeField, Min(0f)]
    private float ladderAlignmentSpeed = 4f;

    [Tooltip(
        "Small upward hop applied when the survivor leaves " +
        "the top of a ladder.")]
    [SerializeField, Min(0f)]
    private float ladderTopJumpForce = 5f;

    [Header("Facing")]
    [SerializeField]
    private bool spriteFacesRight = true;

    [Header("Ground Detection")]
    [SerializeField]
    private Transform groundCheck;

    [SerializeField, Min(0.01f)]
    private float groundCheckRadius = 0.15f;

    [SerializeField]
    private LayerMask groundLayers;

    [Header("Animator Parameters")]
    [SerializeField]
    private string movementSpeedParameter = "MoveSpeed";

    [SerializeField]
    private string groundedParameter = "IsGrounded";

    [SerializeField]
    private string climbingParameter = "IsClimbing";

    [SerializeField]
    private string verticalSpeedParameter = "VerticalSpeed";

    [Header("Runtime State")]
    [SerializeField]
    private bool hasBeenRescued;

    [SerializeField]
    private bool isFollowing;

    [SerializeField]
    private bool isMoving;

    [SerializeField]
    private bool isGrounded;

    [SerializeField]
    private bool isClimbing;

    private Transform followTarget;
    private CarryableLadder nearbyLadder;
    private Transform nearbyLadderCenter;

    private float movementDirection;
    private float normalGravityScale;

    public Transform FollowTarget => followTarget;
    public bool HasBeenRescued => hasBeenRescued;
    public bool IsFollowing => isFollowing;
    public bool IsMoving => isMoving;
    public bool IsGrounded => isGrounded;
    public bool IsClimbing => isClimbing;

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

        if (playerInputReader == null)
        {
            playerInputReader =
                FindFirstObjectByType<PlayerInputReader>();
        }

        if (rb != null)
        {
            normalGravityScale = rb.gravityScale;
        }

        if (startFollowingImmediately)
        {
            ActivateSurvivor();
        }
    }

    private void Update()
    {
        UpdateGroundedState();

        if (!hasBeenRescued)
        {
            CheckForNearbyPlayer();
        }

        if (hasBeenRescued)
        {
            UpdateActiveFollowTarget();
            UpdateFollowState();
        }
        else
        {
            movementDirection = 0f;
            isMoving = false;
        }

        UpdateFacing();
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            ApplyLadderMovement();
        }
        else
        {
            ApplyHorizontalMovement();
        }
    }

    private void CheckForNearbyPlayer()
    {
        if (playerInputReader == null)
        {
            return;
        }

        if (playerInputReader.IsAnyCharacterWithinDistance(
                transform.position,
                activationDistance))
        {
            ActivateSurvivor();
        }
    }

    public void ActivateSurvivor()
    {
        hasBeenRescued = true;
        isFollowing = true;

        UpdateActiveFollowTarget();

        Debug.Log(
            $"'{name}' has been rescued and is now following.",
            this);
    }

    private void UpdateActiveFollowTarget()
    {
        if (playerInputReader == null)
        {
            followTarget = null;
            return;
        }

        followTarget =
            playerInputReader.ActiveCharacterTransform;
    }

    private void UpdateFollowState()
    {
        bool wasMoving = isMoving;

        movementDirection = 0f;
        isMoving = false;

        if (!isFollowing ||
            followTarget == null)
        {
            StopClimbing();
            return;
        }

        Vector2 difference =
            followTarget.position - transform.position;

        TryBeginUpwardClimb(difference);

        if (isClimbing)
        {
            return;
        }

        if (Mathf.Abs(difference.y) >
            maximumWalkingHeightDifference)
        {
            return;
        }

        float horizontalDistance =
            Mathf.Abs(difference.x);

        float requiredDistance =
            wasMoving
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

    private void TryBeginUpwardClimb(
        Vector2 targetDifference)
    {
        if (isClimbing ||
            nearbyLadder == null ||
            !nearbyLadder.CanBeClimbed)
        {
            return;
        }

        // The survivor only uses a ladder if the active
        // character is above them. Descending characters
        // do not cause the survivor to climb downward.
        if (targetDifference.y < minimumClimbHeight)
        {
            return;
        }

        isClimbing = true;
        isMoving = false;
        movementDirection = 0f;

        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void ApplyLadderMovement()
    {
        if (rb == null)
        {
            return;
        }

        if (nearbyLadder == null ||
            !nearbyLadder.CanBeClimbed ||
            followTarget == null)
        {
            StopClimbing();
            return;
        }

        Vector2 difference =
            followTarget.position - transform.position;

        // Never climb downward after a player who is below.
        if (difference.y < -minimumClimbHeight)
        {
            StopClimbing();
            return;
        }

        float ladderCenterX =
            nearbyLadderCenter != null
                ? nearbyLadderCenter.position.x
                : nearbyLadder.transform.position.x;

        float newHorizontalPosition =
            Mathf.MoveTowards(
                rb.position.x,
                ladderCenterX,
                ladderAlignmentSpeed *
                Time.fixedDeltaTime);

        rb.position = new Vector2(
            newHorizontalPosition,
            rb.position.y);

        rb.linearVelocity = new Vector2(
            0f,
            ladderClimbSpeed);
    }

    private void ApplyHorizontalMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.gravityScale = normalGravityScale;

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

    public void EnterLadderZone(
        CarryableLadder ladder,
        Transform ladderCenter)
    {
        if (ladder == null ||
            !ladder.CanBeClimbed)
        {
            return;
        }

        nearbyLadder = ladder;
        nearbyLadderCenter = ladderCenter;
    }

    public void ExitLadderZone(
        CarryableLadder ladder)
    {
        if (nearbyLadder != ladder)
        {
            return;
        }

        bool wasClimbingUpward =
            isClimbing &&
            rb != null &&
            rb.linearVelocity.y > 0f;

        nearbyLadder = null;
        nearbyLadderCenter = null;

        StopClimbing();

        if (wasClimbingUpward &&
            rb != null)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                ladderTopJumpForce);
        }
    }

    private void StopClimbing()
    {
        if (!isClimbing)
        {
            return;
        }

        isClimbing = false;

        if (rb != null)
        {
            rb.gravityScale = normalGravityScale;
            rb.linearVelocity = Vector2.zero;
        }
    }

    public void StopFollowing()
    {
        isFollowing = false;
        movementDirection = 0f;
        isMoving = false;

        StopClimbing();
    }

    public void CompleteRescue()
    {
        StopFollowing();

        Debug.Log(
            $"'{name}' reached the rescue tent.",
            this);
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
        if (animator == null ||
            rb == null)
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

        if (!string.IsNullOrWhiteSpace(
                climbingParameter))
        {
            animator.SetBool(
                climbingParameter,
                isClimbing);
        }

        if (!string.IsNullOrWhiteSpace(
                verticalSpeedParameter))
        {
            animator.SetFloat(
                verticalSpeedParameter,
                rb.linearVelocity.y);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            activationDistance);

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
        activationDistance =
            Mathf.Max(0.1f, activationDistance);

        stoppingDistance =
            Mathf.Max(0f, stoppingDistance);

        resumeDistance =
            Mathf.Max(
                stoppingDistance,
                resumeDistance);

        moveSpeed = Mathf.Max(0f, moveSpeed);
        acceleration = Mathf.Max(0f, acceleration);
        deceleration = Mathf.Max(0f, deceleration);

        ladderClimbSpeed =
            Mathf.Max(0f, ladderClimbSpeed);

        minimumClimbHeight =
            Mathf.Max(0f, minimumClimbHeight);

        ladderAlignmentSpeed =
            Mathf.Max(0f, ladderAlignmentSpeed);

        ladderTopJumpForce =
            Mathf.Max(0f, ladderTopJumpForce);

        groundCheckRadius =
            Mathf.Max(0.01f, groundCheckRadius);
    }
}

//----- SurvivorFollower2D.cs END -----