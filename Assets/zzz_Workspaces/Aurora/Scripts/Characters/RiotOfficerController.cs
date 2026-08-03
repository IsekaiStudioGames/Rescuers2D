//----- RiotOfficerController.cs START -----

using UnityEngine;

public class RiotOfficerController : MonoBehaviour
{
    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int IsHoldingUpHash =
        Animator.StringToHash("IsHoldingUp");

    private static readonly int IsBracingHash =
        Animator.StringToHash("IsBracing");

    private static readonly int BashHash =
        Animator.StringToHash("Bash");

    [Header("Components")]
    [SerializeField]
    private Rigidbody2D rb;

    [SerializeField]
    private Animator animator;

    [Header("Movement")]
    [SerializeField, Min(0f)]
    private float moveSpeed = 5f;

    [Header("Shield Actions")]
    [SerializeField, Min(0f)]
    private float bashCooldown = 0.5f;

    [SerializeField, Min(0f)]
    private float shieldMoveSpeed = 2.5f;

    [Header("Audio")]
    [SerializeField]
    private CharacterAudioEmitter characterAudio;

    private Vector2 currentMoveInput;

    private bool isFacingRight = true;
    private bool isHoldingShield;
    private bool isBracing;

    private float nextBashTime;

    public bool IsHoldingShield =>
        isHoldingShield;

    public bool IsBracing =>
        isBracing;

    private void Awake()
    {
        ResolveReferences();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        UpdateAnimator();
    }

    public void Move(Vector2 moveDirection)
    {
        currentMoveInput =
            moveDirection;
    }

    private void HandleMovement()
    {
        if (rb == null)
        {
            return;
        }

        if (isBracing)
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y);

            return;
        }

        float activeMoveSpeed =
            isHoldingShield
                ? shieldMoveSpeed
                : moveSpeed;

        Vector2 velocity =
            rb.linearVelocity;

        velocity.x =
            currentMoveInput.x *
            activeMoveSpeed;

        rb.linearVelocity =
            velocity;

        if (Mathf.Abs(currentMoveInput.x) >
            0.1f)
        {
            FlipSprite(
                currentMoveInput.x);
        }
    }

    public void SetShield(bool holdingShield)
    {
        if (isBracing)
        {
            holdingShield =
                false;
        }

        if (isHoldingShield ==
            holdingShield)
        {
            return;
        }

        isHoldingShield =
            holdingShield;

        if (isHoldingShield)
        {
            characterAudio?.PlaySpecialAction();
        }

        UpdateAnimator();
    }

    public void SetBrace(bool bracing)
    {
        if (isBracing == bracing)
        {
            return;
        }

        isBracing =
            bracing;

        if (isBracing)
        {
            isHoldingShield =
                false;

            currentMoveInput =
                Vector2.zero;

            if (rb != null)
            {
                rb.linearVelocity =
                    new Vector2(
                        0f,
                        rb.linearVelocity.y);
            }
        }

        // Plays once when raised and once when lowered.
        characterAudio?.PlaySecondaryAction();

        UpdateAnimator();
    }

    public void Bash()
    {
        if (isBracing ||
            Time.time < nextBashTime)
        {
            return;
        }

        nextBashTime =
            Time.time + bashCooldown;

        isHoldingShield =
            false;

        if (animator != null)
        {
            animator.SetBool(
                IsHoldingUpHash,
                false);

            animator.SetTrigger(
                BashHash);
        }

        UpdateAnimator();
    }

    /// <summary>
    /// Requests placement on the nearest destructible tile.
    /// C4PlacementZone2D owns tile selection, inventory consumption,
    /// placement restrictions, spawning, and arming.
    /// </summary>
    public void PlaceC4()
    {
        if (isBracing)
        {
            return;
        }

        C4PlacementZone2D.TryPlaceNearest(
            transform.position,
            RescuerInventoryOwner.RiotOfficer);
    }

    public void Anim_PlayFootstep()
    {
        if (isHoldingShield ||
            isBracing ||
            Mathf.Abs(currentMoveInput.x) <= 0.1f)
        {
            return;
        }

        characterAudio?.PlayFootstep();
    }

    public void Anim_PlayBashImpact()
    {
        characterAudio?.PlayPrimaryAction();
    }

    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        bool isMoving =
            Mathf.Abs(currentMoveInput.x) >
            0.1f;

        animator.SetBool(
            IsMovingHash,
            isMoving &&
            !isHoldingShield &&
            !isBracing);

        animator.SetBool(
            IsHoldingUpHash,
            isHoldingShield &&
            !isBracing);

        animator.SetBool(
            IsBracingHash,
            isBracing);
    }

    private void FlipSprite(float direction)
    {
        bool shouldFaceRight =
            direction > 0f;

        if (shouldFaceRight ==
            isFacingRight)
        {
            return;
        }

        isFacingRight =
            shouldFaceRight;

        Vector3 scale =
            transform.localScale;

        scale.x =
            Mathf.Abs(scale.x) *
            (isFacingRight ? 1f : -1f);

        transform.localScale =
            scale;
    }

    private void ResolveReferences()
    {
        if (rb == null)
        {
            rb =
                GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator =
                GetComponentInChildren<Animator>();
        }

        if (characterAudio == null)
        {
            characterAudio =
                GetComponent<CharacterAudioEmitter>();
        }
    }

    private void OnDisable()
    {
        currentMoveInput =
            Vector2.zero;

        isHoldingShield =
            false;

        isBracing =
            false;

        if (rb != null)
        {
            rb.linearVelocity =
                new Vector2(
                    0f,
                    rb.linearVelocity.y);
        }

        UpdateAnimator();
    }

    private void OnValidate()
    {
        moveSpeed =
            Mathf.Max(
                0f,
                moveSpeed);

        shieldMoveSpeed =
            Mathf.Max(
                0f,
                shieldMoveSpeed);

        bashCooldown =
            Mathf.Max(
                0f,
                bashCooldown);

        ResolveReferences();
    }
}

//----- RiotOfficerController.cs END -----