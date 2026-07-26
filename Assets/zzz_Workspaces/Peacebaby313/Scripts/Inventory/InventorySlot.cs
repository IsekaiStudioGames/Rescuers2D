//----- InventorySlot.cs START -----

using System;

[Serializable]
public sealed class InventorySlot
{
    private InventoryItemData item;
    private int quantity;

    public InventoryItemData Item => item;
    public int Quantity => quantity;

    public bool IsEmpty =>
        item == null || quantity <= 0;

    public bool CanAccept(
        InventoryItemData itemToAdd,
        int amount = 1)
    {
        if (itemToAdd == null || amount <= 0)
        {
            return false;
        }

        if (IsEmpty)
        {
            return amount <= itemToAdd.MaximumStackSize;
        }

        return item == itemToAdd &&
               quantity + amount <= item.MaximumStackSize;
    }

    public int GetAvailableSpace(
        InventoryItemData itemToAdd)
    {
        if (itemToAdd == null)
        {
            return 0;
        }

        if (IsEmpty)
        {
            return itemToAdd.MaximumStackSize;
        }

        if (item != itemToAdd)
        {
            return 0;
        }

        return MathfMax(
            0,
            item.MaximumStackSize - quantity);
    }

    public int Add(
        InventoryItemData itemToAdd,
        int amount)
    {
        if (itemToAdd == null || amount <= 0)
        {
            return 0;
        }

        int amountAccepted = Math.Min(
            amount,
            GetAvailableSpace(itemToAdd));

        if (amountAccepted <= 0)
        {
            return 0;
        }

        if (IsEmpty)
        {
            item = itemToAdd;
            quantity = 0;
        }

        quantity += amountAccepted;
        return amountAccepted;
    }

    public int Remove(int amount)
    {
        if (IsEmpty || amount <= 0)
        {
            return 0;
        }

        int amountRemoved =
            Math.Min(amount, quantity);

        quantity -= amountRemoved;

        if (quantity <= 0)
        {
            Clear();
        }

        return amountRemoved;
    }

    public void Clear()
    {
        item = null;
        quantity = 0;
    }

    private static int MathfMax(int first, int second)
    {
        return first > second ? first : second;
    }
}

//----- InventorySlot.cs END -----