//----- InventoryHUDController.cs START -----

using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryHUDController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TeamInventory teamInventory;

    [SerializeField]
    private InventoryGridController inventoryGridController;

    private bool isSubscribed;

    private void Awake()
    {
        if (inventoryGridController == null)
        {
            inventoryGridController =
                GetComponent<InventoryGridController>();
        }
    }

    private void Start()
    {
        ResolveTeamInventory();
        RefreshHUD();
    }

    private void OnEnable()
    {
        ResolveTeamInventory();
    }

    private void OnDisable()
    {
        UnsubscribeFromInventory();
    }

    public void RefreshHUD()
    {
        if (teamInventory == null)
        {
            ResolveTeamInventory();
        }

        if (teamInventory == null ||
            inventoryGridController == null)
        {
            return;
        }

        for (int index = 0;
             index < teamInventory.TotalSlotCount;
             index++)
        {
            InventorySlot slot =
                teamInventory.GetSlot(index);

            bool occupied =
                slot != null && !slot.IsEmpty;

            inventoryGridController.RefreshSlot(
                index,
                occupied
                    ? slot.Item.InventoryIcon
                    : null,
                occupied
                    ? slot.Quantity
                    : 0,
                occupied);
        }
    }

    private void ResolveTeamInventory()
    {
        if (teamInventory == null)
        {
            teamInventory =
                FindFirstObjectByType<TeamInventory>();
        }

        if (teamInventory == null)
        {
            Debug.LogWarning(
                $"{nameof(InventoryHUDController)} on '{name}' " +
                $"could not find a {nameof(TeamInventory)}. " +
                "Make sure the game was started through the Bootstrap scene.",
                this);

            return;
        }

        SubscribeToInventory();
    }

    private void SubscribeToInventory()
    {
        if (isSubscribed || teamInventory == null)
        {
            return;
        }

        teamInventory.OnInventoryChanged += RefreshHUD;
        isSubscribed = true;
    }

    private void UnsubscribeFromInventory()
    {
        if (!isSubscribed || teamInventory == null)
        {
            return;
        }

        teamInventory.OnInventoryChanged -= RefreshHUD;
        isSubscribed = false;
    }
}

//----- InventoryHUDController.cs END -----