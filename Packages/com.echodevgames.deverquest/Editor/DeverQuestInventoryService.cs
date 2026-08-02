using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal readonly struct DeverQuestEquipmentComparison
    {
        public readonly DeverQuestEquipment Current;
        public readonly DeverQuestEquipment Candidate;
        public readonly int ArmorClassDelta;
        public readonly int AbilityBonusDelta;
        public readonly float WeightDelta;

        public DeverQuestEquipmentComparison(
            DeverQuestEquipment current,
            DeverQuestEquipment candidate)
        {
            Current = current;
            Candidate = candidate;
            ArmorClassDelta =
                (candidate?.armorClassBonus ?? 0) -
                (current?.armorClassBonus ?? 0);
            AbilityBonusDelta =
                (candidate?.abilityBonus ?? 0) -
                (current?.abilityBonus ?? 0);
            WeightDelta =
                (candidate?.weight ?? 0f) -
                (current?.weight ?? 0f);
        }
    }

    internal static class DeverQuestInventoryService
    {
        public static DeverQuestInventoryEntry AddItem(
            List<DeverQuestInventoryEntry> inventory,
            DeverQuestShopItem item,
            string accountId,
            DeverQuestItemOriginKind originKind,
            string source,
            string sourceContractId = "",
            string sourceRunId = "",
            string sourceEncounterId = "",
            string sourceMonsterId = "",
            string sourceMonsterName = "")
        {
            if (inventory == null || item == null)
            {
                return null;
            }

            bool unique =
                item.equipment != null ||
                item.itemType == DeverQuestShopItemType.Redemption ||
                item.rarity >= DeverQuestItemRarity.Rare ||
                item.maximumStackSize <= 1;

            DeverQuestInventoryEntry entry = unique
                ? null
                : inventory.FirstOrDefault(value =>
                    value != null &&
                    value.shopItemId == item.ShopItemId &&
                    value.binding == item.binding &&
                    value.rarity == item.rarity &&
                    value.quantity < item.maximumStackSize);

            if (entry == null)
            {
                entry = new DeverQuestInventoryEntry
                {
                    shopItemId = item.ShopItemId,
                    displayName = item.displayName,
                    itemType = item.itemType,
                    itemCategory = item.itemCategory,
                    subcategory = item.subcategory,
                    tags = new List<string>(
                        item.tags ?? new List<string>()),
                    rarity = item.rarity,
                    binding = item.binding,
                    tradable = item.tradable,
                    droppable = item.droppable,
                    questProtected = item.questProtected,
                    acquiredUtc = DateTime.UtcNow.ToString("O"),
                    acquisitionSource =
                        source?.Trim() ?? string.Empty,
                    originKind = originKind,
                    originSource =
                        source?.Trim() ?? string.Empty,
                    originAcquiredUtc =
                        DateTime.UtcNow.ToString("O"),
                    sourceContractId =
                        sourceContractId?.Trim() ?? string.Empty,
                    sourceRunId =
                        sourceRunId?.Trim() ?? string.Empty,
                    sourceEncounterId =
                        sourceEncounterId?.Trim() ?? string.Empty,
                    sourceMonsterId =
                        sourceMonsterId?.Trim() ?? string.Empty,
                    sourceMonsterName =
                        sourceMonsterName?.Trim() ?? string.Empty,
                    equipmentId = item.equipment == null
                        ? string.Empty
                        : item.equipment.EquipmentId,
                    unitValueCopper =
                        Math.Max(0, item.EffectiveSellValueCopper),
                    unitWeight = item.equipment == null
                        ? item.unitWeight
                        : item.equipment.weight
                };
                inventory.Add(entry);
            }
            else
            {
                SynchronizeEntry(entry, item);
            }

            entry.quantity++;
            entry.EnsureOwnership(accountId);
            return entry;
        }

        public static DeverQuestInventoryEntry AddEquipmentAsset(
            List<DeverQuestInventoryEntry> inventory,
            DeverQuestEquipment equipment,
            string accountId,
            DeverQuestItemOriginKind originKind,
            string source,
            string sourceContractId = "",
            string sourceRunId = "",
            string sourceEncounterId = "",
            string sourceMonsterId = "",
            string sourceMonsterName = "")
        {
            if (inventory == null || equipment == null)
            {
                return null;
            }

            DeverQuestInventoryEntry entry =
                new DeverQuestInventoryEntry
                {
                    shopItemId =
                        "equipment:" + equipment.EquipmentId,
                    displayName = equipment.displayName,
                    itemType = DeverQuestShopItemType.Equipment,
                    itemCategory =
                        DeverQuestItemCategory.Equipment,
                    subcategory =
                        equipment.equipmentFamily ==
                        DeverQuestEquipmentFamily.Unknown
                            ? string.Empty
                            : equipment.equipmentFamily.ToString(),
                    tags = new List<string>(
                        equipment.tags ?? new List<string>()),
                    quantity = 1,
                    rarity = ParseRarity(equipment.rarity),
                    binding = DeverQuestItemBinding.Unbound,
                    tradable = true,
                    droppable = true,
                    acquiredUtc = DateTime.UtcNow.ToString("O"),
                    acquisitionSource =
                        source?.Trim() ?? string.Empty,
                    originKind = originKind,
                    originSource =
                        source?.Trim() ?? string.Empty,
                    originAcquiredUtc =
                        DateTime.UtcNow.ToString("O"),
                    sourceContractId =
                        sourceContractId?.Trim() ?? string.Empty,
                    sourceRunId =
                        sourceRunId?.Trim() ?? string.Empty,
                    sourceEncounterId =
                        sourceEncounterId?.Trim() ?? string.Empty,
                    sourceMonsterId =
                        sourceMonsterId?.Trim() ?? string.Empty,
                    sourceMonsterName =
                        sourceMonsterName?.Trim() ?? string.Empty,
                    equipmentId = equipment.EquipmentId,
                    unitValueCopper =
                        Math.Max(0, equipment.copperValue / 2),
                    unitWeight = Math.Max(0f, equipment.weight)
                };
            entry.EnsureOwnership(accountId);
            inventory.Add(entry);
            return entry;
        }

        public static void SynchronizeEntry(
            DeverQuestInventoryEntry entry,
            DeverQuestShopItem item)
        {
            if (entry == null || item == null)
            {
                return;
            }

            entry.displayName = item.displayName;
            entry.itemType = item.itemType;
            entry.itemCategory = item.itemCategory;
            entry.subcategory = item.subcategory;
            entry.tags = new List<string>(
                item.tags ?? new List<string>());
            entry.rarity = item.rarity;
            entry.binding = entry.binding ==
                DeverQuestItemBinding.AccountBound
                    ? entry.binding
                    : item.binding;
            entry.tradable =
                entry.binding == DeverQuestItemBinding.AccountBound
                    ? false
                    : item.tradable;
            entry.droppable = item.droppable;
            entry.questProtected = item.questProtected;
            entry.equipmentId = item.equipment == null
                ? string.Empty
                : item.equipment.EquipmentId;
            entry.unitValueCopper =
                Math.Max(0, item.EffectiveSellValueCopper);
            entry.unitWeight = item.equipment == null
                ? item.unitWeight
                : item.equipment.weight;
            entry.EnsureOwnership(
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty);
        }

        public static DeverQuestShopItem FindShopItem(
            DeverQuestInventoryEntry entry)
        {
            return entry == null
                ? null
                : DeverQuestShopService.FindItem(entry.shopItemId);
        }

        public static DeverQuestEquipment FindEquipment(
            DeverQuestInventoryEntry entry)
        {
            if (entry == null)
            {
                return null;
            }

            DeverQuestShopItem item = FindShopItem(entry);
            if (item?.equipment != null)
            {
                return item.equipment;
            }

            return string.IsNullOrWhiteSpace(entry.equipmentId)
                ? null
                : DeverQuestRulesService.FindEquipment(
                    entry.equipmentId);
        }

        public static bool IsEquipped(
            DeverQuestInventoryEntry entry,
            DeverQuestAdventurer adventurer = null)
        {
            DeverQuestEquipment equipment = FindEquipment(entry);
            return equipment != null &&
                   IsEquipped(equipment, adventurer);
        }

        public static bool IsEquipped(
            DeverQuestEquipment equipment,
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return equipment != null &&
                   (adventurer.equippedEquipmentIds ??
                    new List<string>())
                   .Contains(equipment.EquipmentId);
        }

        public static DeverQuestEquipment CurrentEquipment(
            DeverQuestEquipmentSlot slot,
            DeverQuestAdventurer adventurer = null)
        {
            adventurer =
                adventurer ?? DeverQuestAdventurerService.Adventurer;
            return DeverQuestRulesService.EquippedAssets(adventurer)
                .FirstOrDefault(value =>
                    value != null && value.slot == slot);
        }

        public static DeverQuestEquipmentComparison Compare(
            DeverQuestEquipment candidate,
            DeverQuestAdventurer adventurer = null)
        {
            DeverQuestEquipment current = candidate == null
                ? null
                : CurrentEquipment(candidate.slot, adventurer);
            return new DeverQuestEquipmentComparison(
                current, candidate);
        }

        public static int RepairEquippedInventory(
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            List<DeverQuestInventoryEntry> inventory =
                adventurer.inventory ??
                (adventurer.inventory =
                    new List<DeverQuestInventoryEntry>());
            HashSet<string> carried = new HashSet<string>(
                inventory
                    .Where(value =>
                        value != null &&
                        !string.IsNullOrWhiteSpace(
                            value.equipmentId) &&
                        value.quantity > 0)
                    .Select(value => value.equipmentId));
            int repaired = 0;
            foreach (DeverQuestEquipment equipment in
                     DeverQuestRulesService.EquippedAssets(adventurer)
                         .Where(value => value != null))
            {
                if (carried.Contains(equipment.EquipmentId))
                {
                    continue;
                }

                AddEquipmentAsset(
                    inventory,
                    equipment,
                    DeverQuestGuildAccountService.CurrentAccount
                        ?.accountId ?? string.Empty,
                    DeverQuestItemOriginKind.LegacyMigration,
                    "Equipped Gear Migration");
                carried.Add(equipment.EquipmentId);
                repaired++;
            }

            if (repaired > 0)
            {
                DeverQuestAdventurerService.Save();
                DeverQuestGuildAccountService.AddAudit(
                    "Inventory Repaired",
                    "Equipped Gear",
                    $"{repaired} missing entr" +
                    (repaired == 1 ? "y" : "ies"));
                message =
                    $"Restored {repaired} equipped item" +
                    (repaired == 1 ? string.Empty : "s") +
                    " to inventory without changing equipped slots.";
            }
            else
            {
                message =
                    "Every equipped item already has an inventory record.";
            }

            return repaired;
        }

        public static bool TryEquip(
            string ownershipId,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestInventoryEntry entry =
                FindEntry(adventurer, ownershipId);
            DeverQuestEquipment equipment = FindEquipment(entry);
            if (entry == null || equipment == null)
            {
                message =
                    "That inventory entry is not usable equipment.";
                return false;
            }
            if (adventurer.level < equipment.minimumLevel)
            {
                message =
                    $"Requires Level {equipment.minimumLevel}.";
                return false;
            }
            if (IsEquipped(equipment, adventurer))
            {
                message = $"{equipment.displayName} is already equipped.";
                return false;
            }

            DeverQuestRulesService.Equip(adventurer, equipment);
            if (entry.binding == DeverQuestItemBinding.BindOnEquip)
            {
                entry.binding = DeverQuestItemBinding.AccountBound;
                entry.boundAccountId =
                    DeverQuestGuildAccountService.CurrentAccount
                        ?.accountId ?? string.Empty;
                entry.tradable = false;
            }
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Equipment Equipped",
                equipment.displayName,
                adventurer.characterName);
            message =
                $"{equipment.displayName} equipped in the " +
                $"{FriendlySlot(equipment.slot)} slot.";
            return true;
        }

        public static bool TryUnequip(
            string ownershipId,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestInventoryEntry entry =
                FindEntry(adventurer, ownershipId);
            DeverQuestEquipment equipment = FindEquipment(entry);
            if (equipment == null)
            {
                message = "That equipment could not be resolved.";
                return false;
            }
            return TryUnequipEquipment(
                equipment.EquipmentId, out message);
        }

        public static bool TryUnequipEquipment(
            string equipmentId,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestEquipment equipment =
                DeverQuestRulesService.FindEquipment(equipmentId);
            if (equipment == null ||
                !adventurer.equippedEquipmentIds.Contains(
                    equipment.EquipmentId))
            {
                message = "That equipment is not currently equipped.";
                return false;
            }

            bool carried =
                (adventurer.inventory ??
                 new List<DeverQuestInventoryEntry>())
                .Any(value =>
                    value != null &&
                    value.equipmentId == equipment.EquipmentId &&
                    value.quantity > 0);
            if (!carried)
            {
                AddEquipmentAsset(
                    adventurer.inventory,
                    equipment,
                    DeverQuestGuildAccountService.CurrentAccount
                        ?.accountId ?? string.Empty,
                    DeverQuestItemOriginKind.LegacyMigration,
                    "Equipped Gear Migration");
            }

            adventurer.equippedEquipmentIds.RemoveAll(
                value => value == equipment.EquipmentId);
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Equipment Unequipped",
                equipment.displayName,
                adventurer.characterName);
            message = $"{equipment.displayName} returned to the pack.";
            return true;
        }

        public static bool CanDrop(
            DeverQuestInventoryEntry entry,
            out string reason)
        {
            if (entry == null || entry.quantity <= 0)
            {
                reason = "That item is not in the pack.";
                return false;
            }
            if (entry.questProtected ||
                entry.itemCategory ==
                DeverQuestItemCategory.QuestItem)
            {
                reason = "Quest-protected items cannot be dropped.";
                return false;
            }
            if (!entry.droppable)
            {
                reason = "This item is marked as non-droppable.";
                return false;
            }
            if (IsEquipped(entry))
            {
                reason = "Unequip this item before dropping it.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        public static bool TryDrop(
            string ownershipId,
            int quantity,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestInventoryEntry entry =
                FindEntry(adventurer, ownershipId);
            if (!CanDrop(entry, out message))
            {
                return false;
            }

            quantity = Math.Max(
                1, Math.Min(quantity, entry.quantity));
            string itemName = entry.displayName;
            entry.quantity -= quantity;
            adventurer.inventory.RemoveAll(
                value => value == null || value.quantity <= 0);
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Inventory Dropped",
                itemName,
                $"{quantity} item(s)");
            DeverQuestCarrySummary summary =
                DeverQuestEncumbranceService.Summary(adventurer);
            message =
                $"Dropped {quantity} × {itemName}. " +
                $"{summary.TotalWeight:0.0}/" +
                $"{summary.Capacity:0.0} weight remains.";
            return true;
        }

        public static bool CanSell(
            DeverQuestInventoryEntry entry,
            out string reason)
        {
            if (DeverQuestSessionStore.HasActiveSession)
            {
                reason =
                    "Finish or abandon the active Quest before selling.";
                return false;
            }
            if (!DeverQuestShopService.CanSellAtActiveShop(
                    out reason))
            {
                return false;
            }
            if (entry == null || entry.quantity <= 0)
            {
                reason = "That item is not in the pack.";
                return false;
            }
            if (entry.questProtected ||
                entry.itemCategory ==
                DeverQuestItemCategory.QuestItem)
            {
                reason = "Quest-protected items cannot be sold.";
                return false;
            }
            if (entry.itemType == DeverQuestShopItemType.Redemption ||
                entry.itemCategory == DeverQuestItemCategory.Service)
            {
                reason = "This service or redemption cannot be sold.";
                return false;
            }
            if (IsEquipped(entry))
            {
                reason = "Unequip this item before selling it.";
                return false;
            }
            if (SellValue(entry) <= 0)
            {
                reason = "The Quartermaster assigns no resale value.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        public static bool TrySell(
            string ownershipId,
            int quantity,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestInventoryEntry entry =
                FindEntry(adventurer, ownershipId);
            if (!CanSell(entry, out message))
            {
                return false;
            }

            quantity = Math.Max(
                1, Math.Min(quantity, entry.quantity));
            int unitValue = SellValue(entry);
            long totalValue = (long)unitValue * quantity;
            string itemName = entry.displayName;
            entry.quantity -= quantity;
            adventurer.inventory.RemoveAll(
                value => value == null || value.quantity <= 0);
            DeverQuestAdventurerService.Award(totalValue, 0);
            DeverQuestAdventurerService.ExchangeCoinAtGuildHall();
            DeverQuestEconomyService.RecordSale(
                entry, quantity, totalValue);
            DeverQuestGuildAccountService.AddAudit(
                "Inventory Sold",
                itemName,
                $"{quantity} item(s) for " +
                DeverQuestAdventurerService.FormatCoins(totalValue));
            message =
                $"Sold {quantity} × {itemName} for " +
                $"{DeverQuestAdventurerService.FormatCoins(totalValue)}.";
            return true;
        }

        public static int SellValue(
            DeverQuestInventoryEntry entry)
        {
            if (entry == null)
            {
                return 0;
            }
            DeverQuestShopItem item = FindShopItem(entry);
            return item == null
                ? Math.Max(0, entry.unitValueCopper)
                : Math.Max(0, item.EffectiveSellValueCopper);
        }

        public static string DescribeClassification(
            DeverQuestInventoryEntry entry)
        {
            if (entry == null)
            {
                return "Unknown item";
            }

            string subcategory =
                string.IsNullOrWhiteSpace(entry.subcategory)
                    ? string.Empty
                    : $" · {entry.subcategory}";
            return $"{entry.itemCategory}{subcategory} · " +
                   $"{entry.rarity} · {entry.binding}";
        }

        public static string DescribeProvenance(
            DeverQuestInventoryEntry entry)
        {
            if (entry == null)
            {
                return "Origin unavailable.";
            }

            List<string> details = new List<string>();
            string source = string.IsNullOrWhiteSpace(
                entry.originSource)
                ? entry.acquisitionSource
                : entry.originSource;
            details.Add(
                string.IsNullOrWhiteSpace(source)
                    ? "Unknown source"
                    : source);

            if (!string.IsNullOrWhiteSpace(entry.sourceMonsterName))
            {
                details.Add($"from {entry.sourceMonsterName}");
            }
            if (!string.IsNullOrWhiteSpace(entry.sourceEncounterId))
            {
                details.Add(
                    $"Encounter {ShortId(entry.sourceEncounterId)}");
            }
            if (!string.IsNullOrWhiteSpace(entry.sourceRunId))
            {
                details.Add($"Run {ShortId(entry.sourceRunId)}");
            }
            if (DateTime.TryParse(
                    entry.originAcquiredUtc,
                    out DateTime acquired))
            {
                details.Add(acquired.ToLocalTime().ToString(
                    "yyyy-MM-dd HH:mm"));
            }

            return string.Join(" · ", details);
        }

        public static string DescribeEquipment(
            DeverQuestEquipment equipment)
        {
            if (equipment == null)
            {
                return "Equipment data unavailable.";
            }

            List<string> details = new List<string>
            {
                FriendlySlot(equipment.slot)
            };
            if (equipment.equipmentFamily !=
                DeverQuestEquipmentFamily.Unknown)
            {
                details.Add(equipment.equipmentFamily.ToString());
            }
            if (equipment.twoHanded)
            {
                details.Add("Two-Handed");
            }
            if (equipment.armorClassBonus != 0)
            {
                details.Add(
                    Signed(equipment.armorClassBonus) + " AC");
            }
            if (equipment.abilityBonus != 0)
            {
                details.Add(
                    $"{Signed(equipment.abilityBonus)} " +
                    $"{equipment.abilityBonusType}");
            }
            if (!string.IsNullOrWhiteSpace(equipment.damageDice))
            {
                details.Add(
                    $"{equipment.damageDice} " +
                    $"{equipment.weaponDamageType}");
            }
            details.Add($"{equipment.weight:0.##} wt");
            return string.Join(" · ", details);
        }

        public static string DescribeComparison(
            DeverQuestEquipment candidate,
            DeverQuestAdventurer adventurer = null)
        {
            if (candidate == null)
            {
                return "No equipment comparison is available.";
            }

            DeverQuestEquipmentComparison comparison =
                Compare(candidate, adventurer);
            if (comparison.Current == null)
            {
                return $"Empty {FriendlySlot(candidate.slot)} slot · " +
                       $"{DescribeEquipment(candidate)}";
            }
            if (comparison.Current.EquipmentId ==
                candidate.EquipmentId)
            {
                return "Currently equipped.";
            }

            List<string> differences = new List<string>
            {
                $"Replaces {comparison.Current.displayName}",
                $"AC {Signed(comparison.ArmorClassDelta)}",
                $"Weight {Signed(comparison.WeightDelta)}"
            };
            if (candidate.abilityBonusType ==
                comparison.Current.abilityBonusType)
            {
                differences.Add(
                    $"{candidate.abilityBonusType} " +
                    $"{Signed(comparison.AbilityBonusDelta)}");
            }
            else
            {
                differences.Add(
                    $"{comparison.Current.abilityBonusType} " +
                    $"{Signed(-comparison.Current.abilityBonus)} → " +
                    $"{candidate.abilityBonusType} " +
                    $"{Signed(candidate.abilityBonus)}");
            }

            return string.Join(" · ", differences);
        }

        public static string FriendlySlot(
            DeverQuestEquipmentSlot slot)
        {
            return slot.ToString()
                .Replace("Left", " Left")
                .Replace("Right", " Right")
                .Replace("MainHand", "Main Hand")
                .Replace("OffHand", "Off Hand");
        }

        private static DeverQuestInventoryEntry FindEntry(
            DeverQuestAdventurer adventurer,
            string ownershipId)
        {
            return (adventurer.inventory ??
                    new List<DeverQuestInventoryEntry>())
                .FirstOrDefault(value =>
                    value != null &&
                    value.ownershipId == ownershipId);
        }

        private static DeverQuestItemRarity ParseRarity(
            string value)
        {
            return Enum.TryParse(
                value?.Trim() ?? string.Empty,
                true,
                out DeverQuestItemRarity rarity)
                ? rarity
                : DeverQuestItemRarity.Common;
        }

        private static string ShortId(string value)
        {
            value = value?.Trim() ?? string.Empty;
            return value.Length <= 8
                ? value
                : value.Substring(0, 8);
        }

        private static string Signed(int value)
        {
            return value > 0 ? $"+{value}" : value.ToString();
        }

        private static string Signed(float value)
        {
            return value > 0f
                ? $"+{value:0.##}"
                : value.ToString("0.##");
        }
    }
}
