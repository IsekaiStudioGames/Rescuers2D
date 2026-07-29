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
    [SerializeField]
    private float moveSpeed = 5f;

    [SerializeField]
    private float shieldMoveSpeed = 2.5f;

    [Header("Shield Actions")]
    [SerializeField, Min(0f)]
    private float bashCooldown = 0.5f;

    [Header("C4")]
    [SerializeField]
    private TeamInventory teamInventory;

    [SerializeField]
    private C4Charge2D c4Prefab;

    [Tooltip(
        "The point where the Riot Officer places C4. " +
        "Place this child near the character's feet and slightly forward.")]
    [SerializeField]
    private Transform c4PlacementPoint;

    [SerializeField]
    private string c4ItemId = "c4";

    [SerializeField, Min(1)]
    private int c4QuantityPerPlacement = 1;

    [SerializeField, Min(0f)]
    private float c4PlacementCooldown = 0.5f;

    [Header("Optional Feedback")]
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    [Header("Audio")]
    [SerializeField]
    private CharacterAudioEmitter characterAudio;

    private Vector2 currentMoveInput;

    private bool isFacingRight = true;
    private bool isHoldingShield;
    private bool isBracing;

    private float nextBashTime;
    private float nextC4PlacementTime;

    public bool IsHoldingShield => isHoldingShield;
    public bool IsBracing => isBracing;

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
        if (characterAudio == null)
        {
            characterAudio =
                GetComponent<CharacterAudioEmitter>();
        }
        ResolveC4References();
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
        if (rb == null)
        {
            return;
        }

        if (isBracing)
        {
            rb.linearVelocity =
                new Vector2(0f, rb.linearVelocity.y);

            return;
        }

        float activeMoveSpeed =
            isHoldingShield
                ? shieldMoveSpeed
                : moveSpeed;

        Vector2 velocity = rb.linearVelocity;

        velocity.x =
            currentMoveInput.x * activeMoveSpeed;

        rb.linearVelocity = velocity;

        if (Mathf.Abs(currentMoveInput.x) > 0.1f)
        {
            FlipSprite(currentMoveInput.x);
        }
    }

    public void SetShield(bool holdingShield)
    {
        if (isBracing)
        {
            holdingShield = false;
        }

        if (isHoldingShield == holdingShield)
        {
            return;
        }

        isHoldingShield = holdingShield;

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

        isBracing = bracing;

        if (isBracing)
        {
            isHoldingShield = false;
            currentMoveInput = Vector2.zero;

            if (rb != null)
            {
                rb.linearVelocity =
                    new Vector2(0f, rb.linearVelocity.y);
            }
        }
        if (isBracing)
        {
            isHoldingShield = false;
            currentMoveInput = Vector2.zero;

            if (rb != null)
            {
                rb.linearVelocity =
                    new Vector2(
                        0f,
                        rb.linearVelocity.y);
            }

            characterAudio?.PlaySecondaryAction();
        }
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

        isHoldingShield = false;

        if (animator != null)
        {
            animator.SetBool(
                IsHoldingUpHash,
                false);

            animator.SetTrigger(BashHash);
        }
        if (animator != null)
        {
            animator.SetBool(
                IsHoldingUpHash,
                false);

            animator.SetTrigger(BashHash);
        }

        characterAudio?.PlayPrimaryAction();
    }

    public void PlaceC4()
    {
        if (isBracing ||
            Time.time < nextC4PlacementTime)
        {
            return;
        }

        ResolveC4References();

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(RiotOfficerController)} on '{name}' " +
                $"could not find a {nameof(TeamInventory)}.",
                this);

            return;
        }

        if (c4Prefab == null)
        {
            Debug.LogError(
                $"{nameof(RiotOfficerController)} on '{name}' " +
                "has no C4 Prefab assigned.",
                this);

            return;
        }

        if (string.IsNullOrWhiteSpace(c4ItemId))
        {
            Debug.LogError(
                $"{nameof(RiotOfficerController)} on '{name}' " +
                "has no C4 Item ID.",
                this);

            return;
        }

        bool canUseC4 =
            teamInventory.CanUseItem(
                c4ItemId,
                c4QuantityPerPlacement,
                RescuerInventoryOwner.RiotOfficer);

        if (!canUseC4)
        {
            ShowMissingC4Message();

            Debug.LogWarning(
                "The Riot Officer cannot use C4. Verify that the " +
                "C4 item allows Riot Officer use and cross-inventory use.",
                this);

            return;
        }

        bool consumedC4 =
            teamInventory.TryUseItem(
                c4ItemId,
                c4QuantityPerPlacement,
                RescuerInventoryOwner.RiotOfficer);

        if (!consumedC4)
        {
            Debug.LogError(
                "C4 validation succeeded, but the item could not " +
                "be consumed.",
                this);

            return;
        }

        SpawnAndArmC4();

        nextC4PlacementTime =
            Time.time + c4PlacementCooldown;
    }

    private void SpawnAndArmC4()
    {
        Vector3 spawnPosition =
            c4PlacementPoint != null
                ? c4PlacementPoint.position
                : transform.position;

        Quaternion spawnRotation =
            c4PlacementPoint != null
                ? c4PlacementPoint.rotation
                : Quaternion.identity;

        C4Charge2D placedC4 =
            Instantiate(
                c4Prefab,
                spawnPosition,
                spawnRotation);

        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowSuccess(
                "The Riot Officer placed the C4.");
        }

        Debug.Log(
            $"Riot Officer placed C4 at {spawnPosition}.",
            this);

        placedC4.Arm();
    }

    private void ShowMissingC4Message()
    {
        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowWarning(
                "The team does not have usable C4.");
        }
    }

    private void ResolveC4References()
    {
        if (teamInventory == null)
        {
            teamInventory =
                FindFirstObjectByType<TeamInventory>();
        }

        if (feedbackPresenter == null)
        {
            feedbackPresenter =
                FindFirstObjectByType<HUDFeedbackPresenter>();
        }
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

    private void OnDisable()
    {
        currentMoveInput = Vector2.zero;
        isHoldingShield = false;
        isBracing = false;

        UpdateAnimator();
    }

    private void OnValidate()
    {
        bashCooldown =
            Mathf.Max(0f, bashCooldown);

        c4QuantityPerPlacement =
            Mathf.Max(1, c4QuantityPerPlacement);

        c4PlacementCooldown =
            Mathf.Max(0f, c4PlacementCooldown);

        c4ItemId =
            c4ItemId?.Trim() ?? string.Empty;
    }
}

//----- RiotOfficerController.cs END -----