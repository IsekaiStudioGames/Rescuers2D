//----- LockedDoor2D.cs START -----

using UnityEngine;
using static PlayerInputReader;

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
    [SerializeField]
    private Collider2D blockingCollider;

    [Header("Item Requirement")]
    [SerializeField]
    private string requiredItemId = "key_01";

    [SerializeField, Min(1)]
    private int requiredQuantity = 1;

    [SerializeField]
    private bool consumeItemWhenOpened = true;

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

    public LockedDoorState CurrentState =>
        currentState;

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

    public void TryOpen(
        RescuerInventoryOwner requestingRescuer)
    {
        if (!CanInteract)
        {
            return;
        }

        ResolveReferences();

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(LockedDoor2D)} on '{name}' could not " +
                $"find a {nameof(TeamInventory)}.",
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

        bool canUseRequiredItem =
            teamInventory.CanUseItem(
                requiredItemId,
                requiredQuantity,
                requestingRescuer);

        if (!canUseRequiredItem)
        {
            ShowMissingItemMessage(requestingRescuer);
            return;
        }

        if (consumeItemWhenOpened)
        {
            bool itemUsed =
                teamInventory.TryUseItem(
                    requiredItemId,
                    requiredQuantity,
                    requestingRescuer);

            if (!itemUsed)
            {
                Debug.LogError(
                    $"The door validated '{requiredItemId}' " +
                    "but could not consume it.",
                    this);

                return;
            }
        }

        BeginOpening(requestingRescuer);
    }

    private void BeginOpening(
        RescuerInventoryOwner requestingRescuer)
    {
        currentState = LockedDoorState.Opening;

        if (animator == null)
        {
            Debug.LogError(
                $"{nameof(LockedDoor2D)} on '{name}' has no " +
                "Animator.",
                this);

            FinishOpening();
            return;
        }

        animator.SetTrigger(OpenTrigger);

        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowSuccess(
                $"{GetOwnerDisplayName(requestingRescuer)} " +
                "unlocked the door.");
        }

        Debug.Log(
            $"Door '{name}' opened by {requestingRescuer} " +
            $"using '{requiredItemId}'.",
            this);
    }

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

    private void ShowMissingItemMessage(
        RescuerInventoryOwner requestingRescuer)
    {
        if (feedbackPresenter == null)
        {
            return;
        }

        feedbackPresenter.ShowWarning(
            $"{GetOwnerDisplayName(requestingRescuer)} " +
            "cannot access the required item.");
    }

    private static string GetOwnerDisplayName(
        RescuerInventoryOwner owner)
    {
        return owner switch
        {
            RescuerInventoryOwner.Firefighter =>
                "The Firefighter",

            RescuerInventoryOwner.RiotOfficer =>
                "The Riot Officer",

            RescuerInventoryOwner.Specialist =>
                "The Specialist",

            _ => "This rescuer"
        };
    }

    private void OnValidate()
    {
        requiredItemId =
            requiredItemId?.Trim() ?? string.Empty;

        requiredQuantity =
            Mathf.Max(1, requiredQuantity);
    }
    //public RescuerInventoryOwner CurrentInventoryOwner =>
    //currentCharacter switch
    //{
    //    ActiveCharacter.Firefighter =>
    //        RescuerInventoryOwner.Firefighter,

    //    ActiveCharacter.RiotOfficer =>
    //        RescuerInventoryOwner.RiotOfficer,

    //    ActiveCharacter.Specialist =>
    //        RescuerInventoryOwner.Specialist,

    //    _ => RescuerInventoryOwner.Firefighter
    //};
}

//----- LockedDoor2D.cs END -----