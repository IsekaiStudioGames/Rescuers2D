//----- LockedDoor2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public sealed class LockedDoor2D : MonoBehaviour
{
    private static readonly int OpenTrigger =
        Animator.StringToHash("Open");

    [Header("Door Components")]
    [SerializeField] private Animator animator;

    [Tooltip(
        "The solid collider that prevents characters " +
        "from walking through the closed door.")]
    [SerializeField] private Collider2D blockingCollider;

    [Header("Key Requirement")]
    [SerializeField] private string requiredItemId = "key_01";

    [SerializeField, Min(1)]
    private int requiredQuantity = 1;

    [Tooltip(
        "The rescuer whose inventory must contain the key.")]
    [SerializeField]
    private RescuerInventoryOwner keyOwner =
        RescuerInventoryOwner.Specialist;

    [SerializeField]
    private bool consumeKeyWhenOpened = true;

    [Header("Optional Feedback")]
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    [Header("Runtime State")]
    [SerializeField]
    private LockedDoorState currentState =
        LockedDoorState.Locked;

    private TeamInventory teamInventory;

    public enum LockedDoorState
    {
        Locked,
        Opening,
        Open
    }

    public LockedDoorState CurrentState => currentState;

    public bool CanInteract =>
        currentState == LockedDoorState.Locked;

    public bool IsOpen =>
        currentState == LockedDoorState.Open;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        ResolveReferences();

        if (currentState == LockedDoorState.Open)
        {
            SetBlockingColliderEnabled(false);
        }
    }

    public void TryOpen()
    {
        if (!CanInteract)
        {
            return;
        }

        ResolveReferences();

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(LockedDoor2D)} on '{name}' could not find " +
                $"a {nameof(TeamInventory)}. Start the game through " +
                "the proper bootstrap scene.",
                this);

            return;
        }

        if (string.IsNullOrWhiteSpace(requiredItemId))
        {
            Debug.LogError(
                $"{nameof(LockedDoor2D)} on '{name}' has no " +
                "Required Item Id.",
                this);

            return;
        }

        bool hasRequiredKey =
            teamInventory.ContainsItem(
                requiredItemId,
                requiredQuantity
                );

        if (!hasRequiredKey)
        {
            ShowMissingKeyMessage();
            return;
        }

        if (consumeKeyWhenOpened)
        {
            bool consumedKey =
                teamInventory.TryConsumeItem(
                    requiredItemId,
                    requiredQuantity
                    );

            if (!consumedKey)
            {
                Debug.LogError(
                    $"The door found '{requiredItemId}' but could " +
                    "not consume it from the specified inventory.",
                    this);

                return;
            }
        }

        BeginOpening();
    }

    private void BeginOpening()
    {
        currentState = LockedDoorState.Opening;

        if (animator == null)
        {
            Debug.LogError(
                $"{nameof(LockedDoor2D)} on '{name}' has no Animator.",
                this);

            // Avoid permanently trapping the player if the
            // Animator reference was accidentally omitted.
            FinishOpening();
            return;
        }

        animator.SetTrigger(OpenTrigger);

        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowSuccess(
                "The Firefighter unlocked the door.");
        }

        Debug.Log(
            $"Door '{name}' opened using '{requiredItemId}' " +
            $"from {keyOwner}'s inventory.",
            this);
    }

    // Call this using an Animation Event on the final
    // frame of the door-opening animation.
    public void Anim_FinishOpening()
    {
        FinishOpening();
    }

    private void FinishOpening()
    {
        if (currentState == LockedDoorState.Open)
        {
            return;
        }

        currentState = LockedDoorState.Open;
        SetBlockingColliderEnabled(false);
    }

    private void SetBlockingColliderEnabled(bool enabled)
    {
        if (blockingCollider != null)
        {
            blockingCollider.enabled = enabled;
        }
    }

    private void ResolveReferences()
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

    private void ShowMissingKeyMessage()
    {
        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowWarning(
                "The Specialist needs to find the key.");
        }
    }

    private void OnValidate()
    {
        requiredItemId = requiredItemId.Trim();
        requiredQuantity = Mathf.Max(1, requiredQuantity);
    }
}

//----- LockedDoor2D.cs END -----