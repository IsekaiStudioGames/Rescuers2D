using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryGridController : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private RectTransform inventoryGrid;
    [SerializeField] private InventoryElementView inventoryElementPrefab;

    [Header("Documented Inventory Layout")]
    [SerializeField, Min(1)] private int rescuerCount = 3;
    [SerializeField, Min(1)] private int slotsPerRescuer = 4;

    private readonly List<InventoryElementView> slotViews = new();

    public int TotalSlotCount => rescuerCount * slotsPerRescuer;

    private void Awake()
    {
        BuildGrid();
    }

    public void BuildGrid()
    {
        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        ClearExistingGridChildren();
        slotViews.Clear();

        for (int index = 0; index < TotalSlotCount; index++)
        {
            InventoryElementView newSlot = Instantiate(
                inventoryElementPrefab,
                inventoryGrid);

            newSlot.name = $"InventoryElement_{index + 1:00}";
            newSlot.SetEmpty();

            slotViews.Add(newSlot);
        }
    }

    public void ClearAllSlots()
    {
        EnsureGridExists();

        foreach (InventoryElementView slotView in slotViews)
        {
            if (slotView != null)
            {
                slotView.SetEmpty();
            }
        }
    }

    public void RefreshSlot(
        int slotIndex,
        Sprite itemSprite,
        int stackCount,
        bool occupied)
    {
        if (!TryGetSlot(slotIndex, out InventoryElementView slotView))
        {
            return;
        }

        if (!occupied)
        {
            slotView.SetEmpty();
            return;
        }

        slotView.SetItem(itemSprite, Mathf.Max(1, stackCount));
    }

    public void SetSlotFeedbackState(
    int slotIndex,
    InventorySlotFeedbackState feedbackState)
    {
        if (!TryGetSlot(
                slotIndex,
                out InventoryElementView slotView))
        {
            return;
        }

        slotView.SetFeedbackState(feedbackState);
    }
    public void SetSelectedSlot(int slotIndex)
    {
        EnsureGridExists();

        for (int index = 0; index < slotViews.Count; index++)
        {
            InventoryElementView slotView = slotViews[index];

            if (slotView != null)
            {
                slotView.SetSelected(index == slotIndex);
            }
        }
    }

    public void ClearSelection()
    {
        EnsureGridExists();

        foreach (InventoryElementView slotView in slotViews)
        {
            if (slotView != null)
            {
                slotView.SetSelected(false);
            }
        }
    }

    private bool TryGetSlot(
        int slotIndex,
        out InventoryElementView slotView)
    {
        EnsureGridExists();

        if (slotIndex < 0 || slotIndex >= slotViews.Count)
        {
            Debug.LogWarning(
                $"Inventory slot index {slotIndex} is outside the valid range " +
                $"0 through {slotViews.Count - 1}.",
                this);

            slotView = null;
            return false;
        }

        slotView = slotViews[slotIndex];
        return slotView != null;
    }

    private void EnsureGridExists()
    {
        if (slotViews.Count != TotalSlotCount)
        {
            BuildGrid();
        }
    }

    private void ClearExistingGridChildren()
    {
        for (int index = inventoryGrid.childCount - 1; index >= 0; index--)
        {
            GameObject childObject =
                inventoryGrid.GetChild(index).gameObject;

            Destroy(childObject);
        }
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (inventoryGrid == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its Inventory Grid reference.",
                this);

            valid = false;
        }

        if (inventoryElementPrefab == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its Inventory Element Prefab reference.",
                this);

            valid = false;
        }

        return valid;
    }
}