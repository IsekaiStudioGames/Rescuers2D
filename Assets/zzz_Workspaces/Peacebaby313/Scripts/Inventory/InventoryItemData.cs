//----- InventoryItemData.cs START -----

using System;
using UnityEngine;

public enum InventoryItemCategory
{
    Consumable,
    MissionItem
}

[Flags]
public enum RescuerItemUser
{
    None = 0,
    Firefighter = 1 << 0,
    RiotOfficer = 1 << 1,
    Specialist = 1 << 2,

    Everyone =
        Firefighter |
        RiotOfficer |
        Specialist
}

[CreateAssetMenu(
    fileName = "Item_NewItem",
    menuName = "Rescuers2D/Inventory/Item Data")]
public sealed class InventoryItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId = "new_item";
    [SerializeField] private string displayName = "New Item";

    [SerializeField, TextArea]
    private string description;

    [Header("Classification")]
    [SerializeField]
    private InventoryItemCategory category =
        InventoryItemCategory.MissionItem;

    [Header("Presentation")]
    [SerializeField] private Sprite inventoryIcon;

    [Header("Inventory Rules")]
    [SerializeField, Min(1)]
    private int maximumStackSize = 1;

    [Tooltip(
        "The rescuers who are qualified to use this item. " +
        "This does not control who can carry or collect it.")]
    [SerializeField]
    private RescuerItemUser allowedUsers =
        RescuerItemUser.Everyone;

    [Tooltip(
        "When enabled, an allowed rescuer may use this item " +
        "even when it is stored in another rescuer's inventory.")]
    [SerializeField]
    private bool allowCrossInventoryUse = true;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;

    public InventoryItemCategory Category =>
        category;

    public Sprite InventoryIcon => inventoryIcon;

    public int MaximumStackSize =>
        maximumStackSize;

    public RescuerItemUser AllowedUsers =>
        allowedUsers;

    public bool AllowCrossInventoryUse =>
        allowCrossInventoryUse;

    public bool CanBeUsedBy(
        RescuerInventoryOwner rescuer)
    {
        RescuerItemUser rescuerFlag =
            GetUserFlag(rescuer);

        return rescuerFlag != RescuerItemUser.None &&
               (allowedUsers & rescuerFlag) != 0;
    }

    private static RescuerItemUser GetUserFlag(
        RescuerInventoryOwner rescuer)
    {
        return rescuer switch
        {
            RescuerInventoryOwner.Firefighter =>
                RescuerItemUser.Firefighter,

            RescuerInventoryOwner.RiotOfficer =>
                RescuerItemUser.RiotOfficer,

            RescuerInventoryOwner.Specialist =>
                RescuerItemUser.Specialist,

            _ => RescuerItemUser.None
        };
    }

    private void OnValidate()
    {
        itemId = itemId?.Trim() ?? string.Empty;
        displayName = displayName?.Trim() ?? string.Empty;

        maximumStackSize =
            Mathf.Max(1, maximumStackSize);
    }
}

//----- InventoryItemData.cs END -----