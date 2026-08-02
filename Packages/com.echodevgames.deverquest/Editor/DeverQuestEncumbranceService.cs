using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal readonly struct DeverQuestCarrySummary
    {
        public readonly float InventoryWeight;
        public readonly float CoinWeight;
        public readonly float TotalWeight;
        public readonly float Capacity;
        public readonly float RemainingCapacity;
        public readonly float LoadPercent;
        public readonly string Status;

        public bool IsEncumbered => TotalWeight > Capacity;

        public DeverQuestCarrySummary(
            float inventoryWeight,
            float coinWeight,
            float capacity)
        {
            InventoryWeight = Math.Max(0f, inventoryWeight);
            CoinWeight = Math.Max(0f, coinWeight);
            TotalWeight = InventoryWeight + CoinWeight;
            Capacity = Math.Max(0f, capacity);
            RemainingCapacity =
                Math.Max(0f, Capacity - TotalWeight);
            LoadPercent = Capacity <= 0f
                ? 0f
                : TotalWeight / Capacity * 100f;

            if (TotalWeight > Capacity)
            {
                Status = "Encumbered";
            }
            else if (LoadPercent >= 90f)
            {
                Status = "Near Limit";
            }
            else if (LoadPercent >= 70f)
            {
                Status = "Heavy";
            }
            else if (LoadPercent >= 40f)
            {
                Status = "Comfortable";
            }
            else
            {
                Status = "Light";
            }
        }
    }

    internal static class DeverQuestEncumbranceService
    {
        private const float CoinWeight = 0.01f;

        public static float CarryCapacity(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return Math.Max(
                20f,
                30f + adventurer.strength * 2f +
                adventurer.level);
        }

        public static float InventoryWeight(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return (adventurer.inventory ??
                    new List<DeverQuestInventoryEntry>())
                .Where(item => item != null && item.quantity > 0)
                .Sum(item =>
                    Math.Max(0f, item.unitWeight) * item.quantity);
        }

        public static float CoinCarryWeight(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return DeverQuestAdventurerService.CoinPieceCount(
                adventurer) * CoinWeight;
        }

        public static DeverQuestCarrySummary Summary(
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return new DeverQuestCarrySummary(
                InventoryWeight(adventurer),
                CoinCarryWeight(adventurer),
                CarryCapacity(adventurer));
        }

        public static float CarriedWeight(
            DeverQuestAdventurer adventurer = null)
        {
            return Summary(adventurer).TotalWeight;
        }

        public static bool IsEncumbered(
            DeverQuestAdventurer adventurer = null)
        {
            return Summary(adventurer).IsEncumbered;
        }

        public static bool DropInventory(
            string ownershipId,
            int quantity,
            out string message)
        {
            return DeverQuestInventoryService.TryDrop(
                ownershipId, quantity, out message);
        }
    }
}
