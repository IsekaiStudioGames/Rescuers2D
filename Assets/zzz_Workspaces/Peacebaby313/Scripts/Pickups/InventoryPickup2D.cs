//----- InventoryPickup2D.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider2D))]
public sealed class InventoryPickup2D : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private InventoryItemData itemData;
    [SerializeField, Min(1)] private int quantity = 1;

    [Header("Inventory Owner")]
    [SerializeField]
    private RescuerInventoryOwner inventoryOwner =
        RescuerInventoryOwner.Firefighter;

    [Header("Optional Feedback")]
    [SerializeField]
    private HUDFeedbackPresenter feedbackPresenter;

    [Header("Pickup Rules")]
    [Tooltip(
        "When enabled, only the rescuer matching Inventory Owner " +
        "can collect this item.")]
    [SerializeField]
    private bool requireMatchingRescuer = true;

    private TeamInventory teamInventory;
    private bool hasBeenCollected;

    private void Awake()
    {
        Collider2D pickupCollider = GetComponent<Collider2D>();

        if (!pickupCollider.isTrigger)
        {
            Debug.LogWarning(
                $"{nameof(InventoryPickup2D)} on '{name}' requires " +
                "its Collider2D to be marked Is Trigger. " +
                "It has been corrected automatically.",
                this);

            pickupCollider.isTrigger = true;
        }
    }

    private void Start()
    {
        ResolveReferences();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasBeenCollected)
        {
            return;
        }

        if (!TryResolveEnteringRescuer(
                other,
                out RescuerInventoryOwner enteringRescuer))
        {
            return;
        }

        if (requireMatchingRescuer &&
            enteringRescuer != inventoryOwner)
        {
            ShowWrongRescuerMessage();
            return;
        }

        RescuerInventoryOwner resolvedOwner =
            requireMatchingRescuer
                ? inventoryOwner
                : enteringRescuer;

        TryCollect(resolvedOwner);
    }

    private void TryCollect(RescuerInventoryOwner resolvedOwner)
    {
        if (itemData == null)
        {
            Debug.LogError(
                $"{nameof(InventoryPickup2D)} on '{name}' " +
                "has no Item Data assigned.",
                this);

            return;
        }

        ResolveReferences();

        if (teamInventory == null)
        {
            Debug.LogError(
                $"{nameof(InventoryPickup2D)} on '{name}' could not " +
                $"find a {nameof(TeamInventory)}. Start the game " +
                "through the Splash scene.",
                this);

            return;
        }

        if (!teamInventory.TryAddItem(
                itemData,
                quantity,
                resolvedOwner))
        {
            ShowInventoryFullMessage();
            return;
        }

        hasBeenCollected = true;

        if (feedbackPresenter != null)
        {
            string quantityText =
                quantity > 1
                    ? $" x{quantity}"
                    : string.Empty;

            feedbackPresenter.ShowSuccess(
                $"{itemData.DisplayName}{quantityText} added.");
        }

        Debug.Log(
            $"Picked up {quantity}x '{itemData.DisplayName}' " +
            $"for {resolvedOwner}.",
            this);

        Destroy(gameObject);
    }

    private bool TryResolveEnteringRescuer(
        Collider2D other,
        out RescuerInventoryOwner enteringRescuer)
    {
        if (other.GetComponentInParent<FirefighterController>() != null)
        {
            enteringRescuer =
                RescuerInventoryOwner.Firefighter;

            return true;
        }

        if (other.GetComponentInParent<RiotOfficerController>() != null)
        {
            enteringRescuer =
                RescuerInventoryOwner.RiotOfficer;

            return true;
        }

        if (other.GetComponentInParent<RescueSpecialistController>() != null)
        {
            enteringRescuer =
                RescuerInventoryOwner.Specialist;

            return true;
        }

        enteringRescuer = default;
        return false;
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

    private void ShowInventoryFullMessage()
    {
        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowWarning(
                $"{GetOwnerDisplayName(inventoryOwner)} has no room.");
        }
    }

    private void ShowWrongRescuerMessage()
    {
        if (feedbackPresenter != null)
        {
            feedbackPresenter.ShowWarning(
                $"This item belongs to " +
                $"{GetOwnerDisplayName(inventoryOwner)}.");
        }
    }

    private static string GetOwnerDisplayName(
        RescuerInventoryOwner owner)
    {
        return owner switch
        {
            RescuerInventoryOwner.Firefighter =>
                "the Firefighter",

            RescuerInventoryOwner.RiotOfficer =>
                "the Riot Officer",

            RescuerInventoryOwner.Specialist =>
                "the Specialist",

            _ => "this rescuer"
        };
    }

    private void OnValidate()
    {
        quantity = Mathf.Max(1, quantity);
    }
}

//----- InventoryPickup2D.cs END -----