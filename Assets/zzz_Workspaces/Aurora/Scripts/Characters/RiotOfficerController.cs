using UnityEngine;

public class RiotOfficerController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float shieldMoveSpeed = 2.5f;

    [Header("Shield Actions")]
    [SerializeField] private float bashCooldown = 0.5f;

    private bool isBracing;
    private float nextBashTime;

    private static readonly int IsBracingHash =
        Animator.StringToHash("IsBracing");

    private static readonly int BashHash =
        Animator.StringToHash("Bash");

    private Vector2 currentMoveInput;

    private bool isFacingRight = true;
    private bool isHoldingShield;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int IsHoldingUpHash =
        Animator.StringToHash("IsHoldingUp");

    private void Awake()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void FixedUpdate()
    {
        HandleMovement();
        UpdateAnimator();
    }

    public void Move(Vector2 moveDirection)
    {
        currentMoveInput = moveDirection;
    }

    private void HandleMovement()
    {
        if (isBracing) 
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }


        float activeMoveSpeed = isHoldingShield
            ? shieldMoveSpeed
            : moveSpeed;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentMoveInput.x * activeMoveSpeed;
        rb.linearVelocity = velocity;

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            FlipSprite(currentMoveInput.x);
        }
    }

    public void SetShield(bool holdingShield)
    {
        isHoldingShield = holdingShield;

        if (animator == null)
        {
            Debug.LogError(
                "[Riot Officer] Animator reference is missing.",
                this
            );

            return;
        }

        animator.SetBool(IsHoldingUpHash, holdingShield);

        Debug.Log(
            $"[Riot Officer] Shield: {holdingShield} | " +
            $"Animator: {animator.name} | " +
            $"Parameter: {animator.GetBool(IsHoldingUpHash)}",
            this
        );
    }
    public void SetBrace(bool bracing)
    {
        if (isBracing == bracing)
        {
            return;
        }

        isBracing = bracing;

        if (isBracing)
        {
            isHoldingShield = false;
            currentMoveInput = Vector2.zero;
            rb.linearVelocity =
                new Vector2(0f, rb.linearVelocity.y);
        }

        UpdateAnimator();
    }

    public void Bash()
    {
        if (isBracing || Time.time < nextBashTime)
        {
            return;
        }

        nextBashTime = Time.time + bashCooldown;

        // Bash overrides the raised-shield state.
        isHoldingShield = false;

        animator.SetBool(IsHoldingUpHash, false);
        animator.SetTrigger(BashHash);
    }
    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        bool isMoving =
            Mathf.Abs(currentMoveInput.x) > 0.1f;

        animator.SetBool(
            IsMovingHash,
            isMoving && !isHoldingShield
        );

        animator.SetBool(
            IsHoldingUpHash,
            isHoldingShield
        );

        animator.SetBool(
            IsMovingHash,
            isMoving && !isHoldingShield && !isBracing
        );

        animator.SetBool(
            IsHoldingUpHash,
            isHoldingShield && !isBracing
        );

        animator.SetBool(
            IsBracingHash,
            isBracing
        );


    }

    private void FlipSprite(float direction)
    {
        if ((direction > 0f && !isFacingRight) ||
            (direction < 0f && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            Vector3 scale = transform.localScale;
            scale.x =
                Mathf.Abs(scale.x) *
                (isFacingRight ? 1f : -1f);

            transform.localScale = scale;
        }
    }

    public bool IsHoldingShield => isHoldingShield;
}