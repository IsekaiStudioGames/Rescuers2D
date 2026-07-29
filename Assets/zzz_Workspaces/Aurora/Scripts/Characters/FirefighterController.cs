using UnityEngine;

public class FirefighterController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private Transform visuals;

    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float climbSpeed = 3f;

    [Header("Ladder")]
    [SerializeField] private Transform ladderHoldPoint;
    [SerializeField] private CarryableLadder startingLadder;

    [Header("Axe")]
    [SerializeField] private DamageTrigger2D axeDamageTrigger;

    [Header("Audio")]
    [SerializeField]
    private CharacterAudioEmitter characterAudio;

    private Vector2 currentMoveInput;
    private CarryableLadder nearbyLadder;
    private CarryableLadder carriedLadder;
    private CarryableLadder climbingLadder;

    private float normalGravityScale;
    private bool isFacingRight = true;
    private int climbingZoneContactCount;
    //private LockedDoor2D nearbyLockedDoor;

    private bool IsInsideClimbingZone => climbingZoneContactCount >0;

    private static readonly int IsMovingHash =
        Animator.StringToHash("IsMoving");

    private static readonly int IsHoldingHash =
        Animator.StringToHash("IsHolding");

    private static readonly int IsClimbingHash =
        Animator.StringToHash("IsClimbing");

    private static readonly int ClimbSpeedHash =
        Animator.StringToHash("ClimbSpeed");

    private static readonly int SwingAxeHash =
        Animator.StringToHash("SwingAxe");

    public enum FirefighterState
    {
        Idle,
        Walking,
        Holding,
        HoldingWalking,
        Climbing,
        SwingingAxe
    }

    [Header("Runtime State")]
    [SerializeField]
    private FirefighterState currentState =
        FirefighterState.Idle;

    public FirefighterState CurrentState => currentState;

    public bool IsGrounded =>
        groundCheck != null && groundCheck.IsGrounded;

    public bool IsHoldingLadder => carriedLadder != null;

    public bool IsClimbing =>
        currentState == FirefighterState.Climbing;

    public float SlopeAngle =>
        groundCheck != null ? groundCheck.SlopeAngle : 0f;

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

        if (groundCheck == null)
        {
            groundCheck = GetComponentInChildren<GroundCheck>();
        }

        if (visuals == null && animator != null)
        {
            visuals = animator.transform;
        }

        if (rb != null)
        {
            normalGravityScale = rb.gravityScale;
        }

        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.DisableDamage();
        }
        if (characterAudio == null)
        {
            characterAudio =
                GetComponent<CharacterAudioEmitter>();
        }
    }


    private void Start()
    {
        if (startingLadder != null)
        {
            PickUpLadder(
                startingLadder,
                playAudio: false);
        }

        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        if (currentState == FirefighterState.SwingingAxe)
        {
            StopHorizontalMovement();
            UpdateAnimator();
            return;
        }

        if (currentState == FirefighterState.Climbing)
        {
            HandleClimbing();
        }
        else
        {
            HandleGroundMovement();
        }

        UpdateAnimator();
    }

    public void Move(Vector2 moveDirection)
    {
        currentMoveInput = moveDirection;

        if (moveDirection != Vector2.zero)
        {
            Debug.Log(
                $"[FIREFIGHTER {GetInstanceID()}] " +
                $"Received: {currentMoveInput}, " +
                $"State: {currentState}, " +
                $"Rigidbody: {rb?.name}"
            );
        }
    }
    private void HandleGroundMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.gravityScale = normalGravityScale;

        Vector2 velocity = rb.linearVelocity;
        velocity.x = currentMoveInput.x * walkSpeed;
        rb.linearVelocity = velocity;

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            FlipVisuals(currentMoveInput.x);

            ChangeState(
                IsHoldingLadder
                    ? FirefighterState.HoldingWalking
                    : FirefighterState.Walking
            );
        }
        else
        {
            ChangeState(
                IsHoldingLadder
                    ? FirefighterState.Holding
                    : FirefighterState.Idle
            );
        }
    }

    private void HandleClimbing()
    {
        if (rb == null)
        {
            return;
        }

        if (!IsInsideClimbingZone ||
            climbingLadder == null ||
            !climbingLadder.CanBeClimbed)
        {
            StopClimbing();
            return;
        }

        rb.gravityScale = 0f;

        rb.linearVelocity = new Vector2(
            0f,
            currentMoveInput.y * climbSpeed
        );

        ChangeState(FirefighterState.Climbing);
    }

    public void UseLadder()
    {
        if (currentState == FirefighterState.SwingingAxe)
        {
            return;
        }

        if (IsClimbing)
        {
            StopClimbing();
            return;
        }

        if (carriedLadder != null)
        {
            DropLadder();
            return;
        }

        if (nearbyLadder != null)
        {
            PickUpLadder(nearbyLadder);
        }
    }
    public void UseLadderExtension()
    {
        if (IsHoldingLadder ||
            IsClimbing ||
            currentState == FirefighterState.SwingingAxe)
        {
            return;
        }

        CarryableLadder ladderToExtend =
            nearbyLadder != null
                ? nearbyLadder
                : climbingLadder;

        if (ladderToExtend == null ||
            !ladderToExtend.CanChangeExtension)
        {
            return;
        }

        ladderToExtend.ToggleExtension();

        characterAudio?.PlaySecondaryAction();

    }
    private void PickUpLadder(CarryableLadder ladderToPickUp, bool playAudio = true)
    {
        if (ladderToPickUp == null ||
            ladderHoldPoint == null ||
            !ladderToPickUp.CanBePickedUp)
        {
            return;
        }

        if (IsClimbing)
        {
            StopClimbing();
        }

        carriedLadder = ladderToPickUp;
        climbingLadder = null;
        climbingZoneContactCount = 0;

        carriedLadder.AttachTo(ladderHoldPoint);

        if (playAudio)
        {
            characterAudio?.PlayPickup();
        }

        characterAudio?.PlayPickup();
        StopHorizontalMovement();
        ChangeState(FirefighterState.Holding);
        UpdateAnimator();
    }

    private void DropLadder()
    {
        if (carriedLadder == null)
        {
            return;
        }

        CarryableLadder ladderToDrop = carriedLadder;
        carriedLadder = null;

        ladderToDrop.Detach();

        characterAudio?.PlayDrop();
        StopHorizontalMovement();
        ChangeState(FirefighterState.Idle);
        UpdateAnimator();
    }

    public void EnterLadderPickupZone(
        CarryableLadder ladderToRegister)
    {
        if (ladderToRegister == null ||
            ladderToRegister == carriedLadder)
        {
            return;
        }

        nearbyLadder = ladderToRegister;
    }

    public void ExitLadderPickupZone(
        CarryableLadder ladderToUnregister)
    {
        if (nearbyLadder == ladderToUnregister)
        {
            nearbyLadder = null;
        }
    }

    public void EnterLadderClimbingZone(
        CarryableLadder ladderToRegister)
    {
        if (ladderToRegister == null ||
            !ladderToRegister.CanBeClimbed)
        {
            return;
        }
        if(climbingLadder != ladderToRegister)
        {
            climbingLadder = ladderToRegister;
            climbingZoneContactCount = 0;
        }
        climbingZoneContactCount++;
    }

    public void ExitLadderClimbingZone(
        CarryableLadder ladderToUnregister)
    {
        if (climbingLadder != ladderToUnregister)
        {
            return;
        }

        climbingZoneContactCount = Mathf.Max(0, climbingZoneContactCount - 1);

        if (climbingZoneContactCount > 0)
        {
            return;
        }

        climbingLadder = null;

        if (IsClimbing)
        {
            StopClimbing();
        }
    }

    public void StartClimbing()
    {
        if (carriedLadder != null ||
            climbingLadder == null ||
            !IsInsideClimbingZone ||
            !climbingLadder.CanBeClimbed ||
            currentState == FirefighterState.SwingingAxe)
        {
            return;
        }

        rb.gravityScale = 0f;
        rb.linearVelocity = Vector2.zero;

        ChangeState(FirefighterState.Climbing);
        UpdateAnimator();
    }

    public void StopClimbing()
    {
        if (!IsClimbing)
        {
            return;
        }

        rb.gravityScale = normalGravityScale;
        rb.linearVelocity = Vector2.zero;

        ChangeState(FirefighterState.Idle);
        UpdateAnimator();
    }

    public void UseAxe()
    {
        if (IsHoldingLadder ||
            IsClimbing ||
            currentState == FirefighterState.SwingingAxe)
        {
            return;
        }

        ChangeState(FirefighterState.SwingingAxe);

        characterAudio?.PlayPrimaryAction();

        StopHorizontalMovement();

        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.BeginNewAttack();
        }

        if (animator != null)
        {
            animator.ResetTrigger(SwingAxeHash);
            animator.SetTrigger(SwingAxeHash);
        }

        UpdateAnimator();
    }

    // Animation event placed at the start of the impact window.
    public void Anim_EnableAxeDamage()
    {
        if (currentState != FirefighterState.SwingingAxe)
        {
            return;
        }

        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.EnableDamage();
        }
    }

    // Animation event placed at the end of the impact window.
    public void Anim_DisableAxeDamage()
    {
        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.DisableDamage();
        }
    }

    // Animation event placed on the final attack frame.
    public void Anim_AxeFinished()
    {
        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.DisableDamage();
        }

        if (currentState == FirefighterState.SwingingAxe)
        {
            ChangeState(FirefighterState.Idle);
        }

        UpdateAnimator();
    }
    public void Anim_PlayFootstep()
    {
        if (IsGrounded &&
            !IsClimbing &&
            Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            characterAudio?.PlayFootstep();
        }
    }

    public void Anim_PlayClimbStep()
    {
        if (IsClimbing &&
            Mathf.Abs(currentMoveInput.y) > 0.1f)
        {
            characterAudio?.PlayClimbStep();
        }
    }
    private void UpdateAnimator()
    {
        if (animator == null)
        {
            return;
        }

        bool moving =
            Mathf.Abs(currentMoveInput.x) > 0.1f;

        animator.SetBool(
            IsMovingHash,
            moving &&
            currentState != FirefighterState.Climbing &&
            currentState != FirefighterState.SwingingAxe
        );

        animator.SetBool(
            IsHoldingHash,
            IsHoldingLadder
        );

        animator.SetBool(
            IsClimbingHash,
            currentState == FirefighterState.Climbing
        );

        animator.SetFloat(
            ClimbSpeedHash,
            currentState == FirefighterState.Climbing
                ? Mathf.Abs(currentMoveInput.y)
                : 0f
        );
    }

    private void ChangeState(FirefighterState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
    }

    private void FlipVisuals(float direction)
    {
        if ((direction > 0f && !isFacingRight) ||
            (direction < 0f && isFacingRight))
        {
            isFacingRight = !isFacingRight;

            Transform flipTarget =
                visuals != null ? visuals : transform;

            Vector3 scale = flipTarget.localScale;

            scale.x =
                Mathf.Abs(scale.x) *
                (isFacingRight ? 1f : -1f);

            flipTarget.localScale = scale;
        }
    }

    private void StopHorizontalMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity = new Vector2(
            0f,
            rb.linearVelocity.y
        );
    }


 
    private void OnDisable()
    {
        currentMoveInput = Vector2.zero;

        if (axeDamageTrigger != null)
        {
            axeDamageTrigger.DisableDamage();
        }
    }
}