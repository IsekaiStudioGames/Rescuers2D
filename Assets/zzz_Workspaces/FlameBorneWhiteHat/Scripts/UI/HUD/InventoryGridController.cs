using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class InventoryGridController : MonoBehaviour
{
    [Header("Grid References")]
    [SerializeField] private RectTransform fireFighterSlots;

    [SerializeField] private RectTransform riotOfficerSlots;

    [SerializeField] private RectTransform rescueSpecialistSlots;

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

        BuildSlotsForContainer(
            fireFighterSlots,
            "FireFighter");

        BuildSlotsForContainer(
            riotOfficerSlots,
            "RiotOfficer");

        BuildSlotsForContainer(
            rescueSpecialistSlots,
            "RescueSpecialist");
    }

    private void BuildSlotsForContainer(
    RectTransform slotContainer,
    string rescuerName)
    {
        for (int localIndex = 0;
             localIndex < slotsPerRescuer;
             localIndex++)
        {
            InventoryElementView newSlot = Instantiate(
                inventoryElementPrefab,
                slotContainer);

            newSlot.name =
                $"{rescuerName}_InventoryElement_{localIndex + 1:00}";

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
        ClearContainerChildren(fireFighterSlots);
        ClearContainerChildren(riotOfficerSlots);
        ClearContainerChildren(rescueSpecialistSlots);
    }

    private static void ClearContainerChildren(
        RectTransform container)
    {
        if (container == null)
        {
            return;
        }

        for (int index = container.childCount - 1;
             index >= 0;
             index--)
        {
            Destroy(
                container.GetChild(index).gameObject);
        }
    }

    private bool ValidateReferences()
    {
        bool valid = true;

        if (inventoryElementPrefab == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its Inventory Element Prefab reference.",
                this);

            valid = false;
        }

        if (fireFighterSlots == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its FireFighter Slots reference.",
                this);

            valid = false;
        }

        if (riotOfficerSlots == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its Riot Officer Slots reference.",
                this);

            valid = false;
        }

        if (rescueSpecialistSlots == null)
        {
            Debug.LogError(
                $"{nameof(InventoryGridController)} on '{name}' is missing its Rescue Specialist Slots reference.",
                this);

            valid = false;
        }

        return valid;
    }
}