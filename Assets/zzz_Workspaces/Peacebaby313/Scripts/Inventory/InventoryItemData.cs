using UnityEngine;


public enum InventoryItemCategory
{
    Consumable,
    MissionItem
}

[CreateAssetMenu(
    fileName = "Item_NewItem",
    menuName = "Rescuers2D/Inventory/Item Data")]
public sealed class InventoryItemData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string itemId = "new_item";
    [SerializeField] private string displayName = "New Item";
    [SerializeField, TextArea] private string description;

    [Header("Classification")]
    [SerializeField]
    private InventoryItemCategory category =
        InventoryItemCategory.MissionItem;

    [Header("Presentation")]
    [SerializeField] private Sprite inventoryIcon;

    [Header("Inventory Rules")]
    [SerializeField, Min(1)] private int maximumStackSize = 1;

    public string ItemId => itemId;
    public string DisplayName => displayName;
    public string Description => description;
    public InventoryItemCategory Category => category;
    public Sprite InventoryIcon => inventoryIcon;
    public int MaximumStackSize => maximumStackSize;

    private void OnValidate()
    {
        itemId = itemId.Trim();
        maximumStackSize = Mathf.Max(1, maximumStackSize);
    }
}