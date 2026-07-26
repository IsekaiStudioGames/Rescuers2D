//----- TeamInventory.cs START -----

using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class TeamInventory : MonoBehaviour
{
    private const int RequiredRescuerCount = 3;
    private const int RequiredSlotsPerRescuer = 4;

    private readonly List<InventorySlot> slots = new();

    public event Action OnInventoryChanged;

    public int RescuerCount => RequiredRescuerCount;
    public int SlotsPerRescuer => RequiredSlotsPerRescuer;

    public int TotalSlotCount =>
        RescuerCount * SlotsPerRescuer;

    private void Awake()
    {
        InitializeSlots();
    }

    public bool CanAddItem(
        InventoryItemData item,
        int quantity,
        RescuerInventoryOwner owner)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        EnsureInitialized();

        if (!TryGetOwnerRange(
                owner,
                out int startIndex,
                out int endIndex))
        {
            return false;
        }

        int availableSpace = 0;

        for (int index = startIndex;
             index < endIndex;
             index++)
        {
            availableSpace +=
                slots[index].GetAvailableSpace(item);

            if (availableSpace >= quantity)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryAddItem(
        InventoryItemData item,
        int quantity,
        RescuerInventoryOwner owner)
    {
        if (!CanAddItem(item, quantity, owner))
        {
            return false;
        }

        TryGetOwnerRange(
            owner,
            out int startIndex,
            out int endIndex);

        int remainingQuantity = quantity;

        // Add to existing stacks first.
        for (int index = startIndex;
             index < endIndex &&
             remainingQuantity > 0;
             index++)
        {
            InventorySlot slot = slots[index];

            if (slot.IsEmpty || slot.Item != item)
            {
                continue;
            }

            remainingQuantity -=
                slot.Add(item, remainingQuantity);
        }

        // Then fill empty slots.
        for (int index = startIndex;
             index < endIndex &&
             remainingQuantity > 0;
             index++)
        {
            InventorySlot slot = slots[index];

            if (!slot.IsEmpty)
            {
                continue;
            }

            remainingQuantity -=
                slot.Add(item, remainingQuantity);
        }

        if (remainingQuantity > 0)
        {
            Debug.LogError(
                $"Inventory capacity validation succeeded, " +
                $"but {remainingQuantity} item(s) could not be added.",
                this);

            return false;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool ContainsItem(
        string itemId,
        int quantity = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId) ||
            quantity <= 0)
        {
            return false;
        }

        EnsureInitialized();

        int foundQuantity = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty ||
                slot.Item.ItemId != itemId)
            {
                continue;
            }

            foundQuantity += slot.Quantity;

            if (foundQuantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryConsumeItem(
        string itemId,
        int quantity = 1)
    {
        if (!ContainsItem(itemId, quantity))
        {
            return false;
        }

        int remainingQuantity = quantity;

        foreach (InventorySlot slot in slots)
        {
            if (remainingQuantity <= 0)
            {
                break;
            }

            if (slot.IsEmpty ||
                slot.Item.ItemId != itemId)
            {
                continue;
            }

            remainingQuantity -=
                slot.Remove(remainingQuantity);
        }

        OnInventoryChanged?.Invoke();
        return remainingQuantity == 0;
    }

    public InventorySlot GetSlot(int globalSlotIndex)
    {
        EnsureInitialized();

        if (globalSlotIndex < 0 ||
            globalSlotIndex >= slots.Count)
        {
            Debug.LogWarning(
                $"Inventory slot index {globalSlotIndex} " +
                $"is outside the valid range.",
                this);

            return null;
        }

        return slots[globalSlotIndex];
    }

    public int GetGlobalSlotIndex(
        RescuerInventoryOwner owner,
        int localSlotIndex)
    {
        if (localSlotIndex < 0 ||
            localSlotIndex >= SlotsPerRescuer)
        {
            return -1;
        }

        return ((int)owner * SlotsPerRescuer) +
               localSlotIndex;
    }

    private bool TryGetOwnerRange(
        RescuerInventoryOwner owner,
        out int startIndex,
        out int endIndex)
    {
        int ownerIndex = (int)owner;

        if (ownerIndex < 0 ||
            ownerIndex >= RescuerCount)
        {
            Debug.LogWarning(
                $"Inventory owner '{owner}' is invalid.",
                this);

            startIndex = -1;
            endIndex = -1;
            return false;
        }

        startIndex = ownerIndex * SlotsPerRescuer;
        endIndex = startIndex + SlotsPerRescuer;
        return true;
    }

    private void InitializeSlots()
    {
        slots.Clear();

        for (int index = 0;
             index < TotalSlotCount;
             index++)
        {
            slots.Add(new InventorySlot());
        }

        OnInventoryChanged?.Invoke();
    }

    private void EnsureInitialized()
    {
        if (slots.Count != TotalSlotCount)
        {
            InitializeSlots();
        }
    }

    public bool ContainsItem(
    string itemId,
    int quantity,
    RescuerInventoryOwner owner)
    {
        if (string.IsNullOrWhiteSpace(itemId) ||
            quantity <= 0)
        {
            return false;
        }

        EnsureInitialized();

        if (!TryGetOwnerRange(
                owner,
                out int startIndex,
                out int endIndex))
        {
            return false;
        }

        int foundQuantity = 0;

        for (int index = startIndex;
             index < endIndex;
             index++)
        {
            InventorySlot slot = slots[index];

            if (slot.IsEmpty ||
                slot.Item.ItemId != itemId)
            {
                continue;
            }

            foundQuantity += slot.Quantity;

            if (foundQuantity >= quantity)
            {
                return true;
            }
        }

        return false;
    }

    public bool TryConsumeItem(
        string itemId,
        int quantity,
        RescuerInventoryOwner owner)
    {
        if (!ContainsItem(itemId, quantity, owner))
        {
            return false;
        }

        TryGetOwnerRange(
            owner,
            out int startIndex,
            out int endIndex);

        int remainingQuantity = quantity;

        for (int index = startIndex;
             index < endIndex &&
             remainingQuantity > 0;
             index++)
        {
            InventorySlot slot = slots[index];

            if (slot.IsEmpty ||
                slot.Item.ItemId != itemId)
            {
                continue;
            }

            remainingQuantity -=
                slot.Remove(remainingQuantity);
        }

        OnInventoryChanged?.Invoke();
        return remainingQuantity == 0;
    }
    public bool CanUseItem(
    string itemId,
    int quantity,
    RescuerInventoryOwner requestingRescuer)
    {
        return GetUsableItemQuantity(
                   itemId,
                   requestingRescuer) >= quantity;
    }

    public int GetUsableItemQuantity(
        string itemId,
        RescuerInventoryOwner requestingRescuer)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        EnsureInitialized();

        int usableQuantity = 0;

        // Search the requesting rescuer's inventory first.
        usableQuantity += GetUsableItemQuantityFromOwner(
            itemId,
            requestingRescuer,
            requestingRescuer,
            isCrossInventorySearch: false);

        // Then search the other rescuers' inventories.
        for (int ownerIndex = 0;
             ownerIndex < RescuerCount;
             ownerIndex++)
        {
            RescuerInventoryOwner inventoryOwner =
                (RescuerInventoryOwner)ownerIndex;

            if (inventoryOwner == requestingRescuer)
            {
                continue;
            }

            usableQuantity += GetUsableItemQuantityFromOwner(
                itemId,
                inventoryOwner,
                requestingRescuer,
                isCrossInventorySearch: true);
        }

        return usableQuantity;
    }

    public bool TryUseItem(
        string itemId,
        int quantity,
        RescuerInventoryOwner requestingRescuer)
    {
        if (string.IsNullOrWhiteSpace(itemId) ||
            quantity <= 0)
        {
            return false;
        }

        EnsureInitialized();

        if (!CanUseItem(
                itemId,
                quantity,
                requestingRescuer))
        {
            return false;
        }

        int remainingQuantity = quantity;

        // Consume from the requesting rescuer first.
        remainingQuantity = ConsumeUsableItemFromOwner(
            itemId,
            remainingQuantity,
            requestingRescuer,
            requestingRescuer,
            isCrossInventorySearch: false);

        // Then consume shareable copies from other inventories.
        for (int ownerIndex = 0;
             ownerIndex < RescuerCount &&
             remainingQuantity > 0;
             ownerIndex++)
        {
            RescuerInventoryOwner inventoryOwner =
                (RescuerInventoryOwner)ownerIndex;

            if (inventoryOwner == requestingRescuer)
            {
                continue;
            }

            remainingQuantity = ConsumeUsableItemFromOwner(
                itemId,
                remainingQuantity,
                inventoryOwner,
                requestingRescuer,
                isCrossInventorySearch: true);
        }

        if (remainingQuantity > 0)
        {
            Debug.LogError(
                $"Item-use validation succeeded, but " +
                $"{remainingQuantity} '{itemId}' item(s) could " +
                $"not be consumed.",
                this);

            return false;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    private int GetUsableItemQuantityFromOwner(
        string itemId,
        RescuerInventoryOwner inventoryOwner,
        RescuerInventoryOwner requestingRescuer,
        bool isCrossInventorySearch)
    {
        if (!TryGetOwnerRange(
                inventoryOwner,
                out int startIndex,
                out int endIndex))
        {
            return 0;
        }

        int foundQuantity = 0;

        for (int index = startIndex;
             index < endIndex;
             index++)
        {
            InventorySlot slot = slots[index];

            if (!CanUseSlotItem(
                    slot,
                    itemId,
                    requestingRescuer,
                    isCrossInventorySearch))
            {
                continue;
            }

            foundQuantity += slot.Quantity;
        }

        return foundQuantity;
    }

    private int ConsumeUsableItemFromOwner(
        string itemId,
        int quantity,
        RescuerInventoryOwner inventoryOwner,
        RescuerInventoryOwner requestingRescuer,
        bool isCrossInventorySearch)
    {
        if (quantity <= 0)
        {
            return 0;
        }

        if (!TryGetOwnerRange(
                inventoryOwner,
                out int startIndex,
                out int endIndex))
        {
            return quantity;
        }

        int remainingQuantity = quantity;

        for (int index = startIndex;
             index < endIndex &&
             remainingQuantity > 0;
             index++)
        {
            InventorySlot slot = slots[index];

            if (!CanUseSlotItem(
                    slot,
                    itemId,
                    requestingRescuer,
                    isCrossInventorySearch))
            {
                continue;
            }

            remainingQuantity -=
                slot.Remove(remainingQuantity);
        }

        return remainingQuantity;
    }

    private static bool CanUseSlotItem(
        InventorySlot slot,
        string itemId,
        RescuerInventoryOwner requestingRescuer,
        bool isCrossInventorySearch)
    {
        if (slot == null ||
            slot.IsEmpty ||
            slot.Item.ItemId != itemId)
        {
            return false;
        }

        InventoryItemData item = slot.Item;

        if (!item.CanBeUsedBy(requestingRescuer))
        {
            return false;
        }

        if (isCrossInventorySearch &&
            !item.AllowCrossInventoryUse)
        {
            return false;
        }

        return true;
    }
}

//----- TeamInventory.cs END -----