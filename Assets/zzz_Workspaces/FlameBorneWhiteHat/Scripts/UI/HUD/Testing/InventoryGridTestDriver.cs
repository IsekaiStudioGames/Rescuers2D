using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryGridTestDriver : MonoBehaviour
{
    [Header("Test Scene Reference")]
    [SerializeField]
    private InventoryGridController inventoryGridController;

    [Header("Optional Temporary Test Sprites")]
    [SerializeField] private Sprite bandageTestSprite;
    [SerializeField] private Sprite medkitTestSprite;
    [SerializeField] private Sprite keyTestSprite;

    private void Start()
    {
        if (inventoryGridController == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridTestDriver)} on '{name}' is missing its Inventory Grid Controller reference.",
                this);

            enabled = false;
            return;
        }

        RunShowcase();
    }

    [ContextMenu("Run HUD Showcase")]
    public void RunShowcase()
    {
        inventoryGridController.ClearAllSlots();

        inventoryGridController.RefreshSlot(
            0,
            bandageTestSprite,
            3,
            true);

        inventoryGridController.RefreshSlot(
            1,
            medkitTestSprite,
            1,
            true);

        inventoryGridController.RefreshSlot(
            4,
            keyTestSprite,
            1,
            true);

        inventoryGridController.SetSlotFeedbackState(
            4,
            InventorySlotFeedbackState.Transferable);

        for (int slotIndex = 8;
             slotIndex <= 11;
             slotIndex++)
        {
            inventoryGridController.RefreshSlot(
                slotIndex,
                null,
                1,
                true);

            inventoryGridController.SetSlotFeedbackState(
                slotIndex,
                InventorySlotFeedbackState.Blocked);
        }

        inventoryGridController.SetSelectedSlot(0);
    }

    private void Reset()
    {
        inventoryGridController =
            FindFirstObjectByType<InventoryGridController>();
    }
}