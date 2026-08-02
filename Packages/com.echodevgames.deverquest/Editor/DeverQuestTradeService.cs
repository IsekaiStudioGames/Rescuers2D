using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestTradeStatus
    {
        Offered = 0,
        Accepted = 1,
        Rejected = 2,
        Cancelled = 3,
        Reclaimed = 4
    }

    [Serializable]
    internal sealed class DeverQuestTradeRecord
    {
        public string tradeId = string.Empty;
        public string fromAccountId = string.Empty;
        public string fromName = string.Empty;
        public string toAccountId = string.Empty;
        public string toName = string.Empty;
        public string ownershipId = string.Empty;
        public string shopItemId = string.Empty;
        public string itemName = string.Empty;
        public DeverQuestShopItemType itemType;
        public DeverQuestItemCategory itemCategory =
            DeverQuestItemCategory.Unknown;
        public string subcategory = string.Empty;
        public List<string> tags = new List<string>();
        public DeverQuestItemRarity rarity;
        public DeverQuestItemBinding binding;
        public bool droppable = true;
        public float unitWeight = 0.25f;
        public int unitValueCopper;
        public DeverQuestItemOriginKind originKind =
            DeverQuestItemOriginKind.Unknown;
        public string originSource = string.Empty;
        public string originAcquiredUtc = string.Empty;
        public string sourceContractId = string.Empty;
        public string sourceRunId = string.Empty;
        public string sourceEncounterId = string.Empty;
        public string sourceMonsterId = string.Empty;
        public string sourceMonsterName = string.Empty;
        public string equipmentId = string.Empty;
        public DeverQuestTradeStatus status;
        public string offeredUtc = string.Empty;
        public string resolvedUtc = string.Empty;
        public string resolvedBy = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestTradeLedger
    {
        public List<DeverQuestTradeRecord> records =
            new List<DeverQuestTradeRecord>();
    }

    [InitializeOnLoad]
    internal static class DeverQuestTradeService
    {
        private const string LedgerKey =
            "EchoDevGames.DeverQuest.TradeLedger.v1";
        private static DeverQuestTradeLedger ledger;

        static DeverQuestTradeService()
        {
            Load();
        }

        public static IReadOnlyList<DeverQuestTradeRecord> Records =>
            ledger.records;

        public static bool Offer(
            DeverQuestInventoryEntry entry,
            string targetAccountId,
            out string message)
        {
            message = string.Empty;
            DeverQuestGuildAccount source =
                DeverQuestGuildAccountService.CurrentAccount;
            DeverQuestGuildAccount target =
                DeverQuestGuildAccountService.FindAccount(
                    targetAccountId);
            if (source == null || target == null ||
                source.accountId == target.accountId)
            {
                message = "Choose another enabled Guild account.";
                return false;
            }
            if (entry == null || entry.quantity <= 0 ||
                !entry.tradable ||
                entry.questProtected ||
                entry.itemCategory ==
                DeverQuestItemCategory.QuestItem ||
                entry.itemType == DeverQuestShopItemType.Redemption ||
                entry.binding == DeverQuestItemBinding.BindOnPickup ||
                entry.binding == DeverQuestItemBinding.AccountBound ||
                !string.IsNullOrWhiteSpace(entry.boundAccountId))
            {
                message = "That item is bound or otherwise not tradable.";
                return false;
            }
            DeverQuestShopItem shopItem =
                DeverQuestShopService.FindItem(entry.shopItemId);
            if (entry.binding ==
                    DeverQuestItemBinding.BindOnEquip &&
                shopItem != null &&
                shopItem.equipment != null &&
                DeverQuestAdventurerService.Adventurer
                    .equippedEquipmentIds.Contains(
                        shopItem.equipment.EquipmentId))
            {
                entry.binding =
                    DeverQuestItemBinding.AccountBound;
                entry.boundAccountId = source.accountId;
                entry.tradable = false;
                DeverQuestAdventurerService.Save();
                message =
                    "That equipped item is now bound to this account.";
                return false;
            }

            entry.EnsureOwnership(source.accountId);
            DeverQuestTradeRecord trade = new DeverQuestTradeRecord
            {
                tradeId = Guid.NewGuid().ToString("N"),
                fromAccountId = source.accountId,
                fromName = source.characterName,
                toAccountId = target.accountId,
                toName = target.characterName,
                ownershipId = entry.ownershipId,
                shopItemId = entry.shopItemId,
                itemName = entry.displayName,
                itemType = entry.itemType,
                itemCategory = entry.itemCategory,
                subcategory = entry.subcategory,
                tags = new List<string>(
                    entry.tags ?? new List<string>()),
                rarity = entry.rarity,
                binding = entry.binding,
                droppable = entry.droppable,
                unitWeight = entry.unitWeight,
                unitValueCopper = entry.unitValueCopper,
                originKind = entry.originKind,
                originSource = entry.originSource,
                originAcquiredUtc = entry.originAcquiredUtc,
                sourceContractId = entry.sourceContractId,
                sourceRunId = entry.sourceRunId,
                sourceEncounterId = entry.sourceEncounterId,
                sourceMonsterId = entry.sourceMonsterId,
                sourceMonsterName = entry.sourceMonsterName,
                equipmentId = entry.equipmentId,
                status = DeverQuestTradeStatus.Offered,
                offeredUtc = DateTime.UtcNow.ToString("O")
            };
            entry.quantity--;
            DeverQuestAdventurerService.Adventurer.inventory.RemoveAll(
                value => value.quantity <= 0);
            if (entry.quantity > 0)
            {
                entry.ownershipId = Guid.NewGuid().ToString("N");
            }
            DeverQuestAdventurerService.Save();
            ledger.records.Insert(0, trade);
            Save();
            DeverQuestGuildAccountService.AddAudit(
                "Trade Offered", trade.itemName,
                $"{trade.fromName} → {trade.toName}");
            message = $"{trade.itemName} placed in escrow for " +
                      $"{trade.toName}.";
            return true;
        }

        public static bool Accept(
            DeverQuestTradeRecord trade,
            out string message)
        {
            message = string.Empty;
            DeverQuestGuildAccount current =
                DeverQuestGuildAccountService.CurrentAccount;
            if (trade == null ||
                trade.status != DeverQuestTradeStatus.Offered ||
                current == null ||
                trade.toAccountId != current.accountId)
            {
                message = "Only the intended Adventurer may accept.";
                return false;
            }
            DeverQuestInventoryEntry entry =
                new DeverQuestInventoryEntry
                {
                    ownershipId = trade.ownershipId,
                    shopItemId = trade.shopItemId,
                    displayName = trade.itemName,
                    itemType = trade.itemType,
                    itemCategory = trade.itemCategory,
                    subcategory = trade.subcategory,
                    tags = new List<string>(
                        trade.tags ?? new List<string>()),
                    rarity = trade.rarity,
                    binding = trade.binding,
                    tradable = true,
                    droppable = trade.droppable,
                    acquiredUtc = DateTime.UtcNow.ToString("O"),
                    acquisitionSource =
                        $"Trade from {trade.fromName}",
                    originKind = trade.originKind,
                    originSource = trade.originSource,
                    originAcquiredUtc = trade.originAcquiredUtc,
                    sourceContractId = trade.sourceContractId,
                    sourceRunId = trade.sourceRunId,
                    sourceEncounterId = trade.sourceEncounterId,
                    sourceMonsterId = trade.sourceMonsterId,
                    sourceMonsterName = trade.sourceMonsterName,
                    equipmentId = trade.equipmentId,
                    unitValueCopper =
                        Math.Max(0, trade.unitValueCopper),
                    unitWeight = Math.Max(0f, trade.unitWeight),
                    quantity = 1
                };
            entry.EnsureOwnership(current.accountId);
            DeverQuestAdventurerService.Adventurer.inventory.Add(entry);
            DeverQuestAdventurerService.Save();
            Resolve(trade, DeverQuestTradeStatus.Accepted);
            message = $"{trade.itemName} accepted into your pack.";
            return true;
        }

        public static bool Reject(
            DeverQuestTradeRecord trade,
            out string message)
        {
            message = string.Empty;
            DeverQuestGuildAccount current =
                DeverQuestGuildAccountService.CurrentAccount;
            if (trade == null ||
                trade.status != DeverQuestTradeStatus.Offered ||
                current == null ||
                trade.toAccountId != current.accountId)
            {
                message = "Only the intended Adventurer may reject.";
                return false;
            }
            Resolve(trade, DeverQuestTradeStatus.Rejected);
            message = "Trade rejected; the item awaits its owner.";
            return true;
        }

        public static bool CancelOrReclaim(
            DeverQuestTradeRecord trade,
            out string message)
        {
            message = string.Empty;
            DeverQuestGuildAccount current =
                DeverQuestGuildAccountService.CurrentAccount;
            if (trade == null || current == null ||
                trade.fromAccountId != current.accountId ||
                (trade.status != DeverQuestTradeStatus.Offered &&
                 trade.status != DeverQuestTradeStatus.Rejected))
            {
                message = "This escrow cannot be reclaimed.";
                return false;
            }
            DeverQuestInventoryEntry entry =
                new DeverQuestInventoryEntry
                {
                    ownershipId = trade.ownershipId,
                    shopItemId = trade.shopItemId,
                    displayName = trade.itemName,
                    itemType = trade.itemType,
                    itemCategory = trade.itemCategory,
                    subcategory = trade.subcategory,
                    tags = new List<string>(
                        trade.tags ?? new List<string>()),
                    rarity = trade.rarity,
                    binding = trade.binding,
                    tradable = true,
                    droppable = trade.droppable,
                    acquiredUtc = DateTime.UtcNow.ToString("O"),
                    acquisitionSource = "Trade escrow return",
                    originKind = trade.originKind,
                    originSource = trade.originSource,
                    originAcquiredUtc = trade.originAcquiredUtc,
                    sourceContractId = trade.sourceContractId,
                    sourceRunId = trade.sourceRunId,
                    sourceEncounterId = trade.sourceEncounterId,
                    sourceMonsterId = trade.sourceMonsterId,
                    sourceMonsterName = trade.sourceMonsterName,
                    equipmentId = trade.equipmentId,
                    unitValueCopper =
                        Math.Max(0, trade.unitValueCopper),
                    unitWeight = Math.Max(0f, trade.unitWeight),
                    quantity = 1
                };
            entry.EnsureOwnership(current.accountId);
            DeverQuestAdventurerService.Adventurer.inventory.Add(entry);
            DeverQuestAdventurerService.Save();
            Resolve(
                trade,
                trade.status == DeverQuestTradeStatus.Offered
                    ? DeverQuestTradeStatus.Cancelled
                    : DeverQuestTradeStatus.Reclaimed);
            message = $"{trade.itemName} returned to your pack.";
            return true;
        }

        private static void Resolve(
            DeverQuestTradeRecord trade,
            DeverQuestTradeStatus status)
        {
            trade.status = status;
            trade.resolvedUtc = DateTime.UtcNow.ToString("O");
            trade.resolvedBy =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.developerName ?? string.Empty;
            Save();
            DeverQuestGuildAccountService.AddAudit(
                $"Trade {status}", trade.itemName,
                $"{trade.fromName} → {trade.toName}");
        }

        private static void Load()
        {
            string json = EditorPrefs.GetString(LedgerKey, string.Empty);
            ledger = string.IsNullOrWhiteSpace(json)
                ? new DeverQuestTradeLedger()
                : JsonUtility.FromJson<DeverQuestTradeLedger>(json);
            if (ledger == null)
            {
                ledger = new DeverQuestTradeLedger();
            }
            if (ledger.records == null)
            {
                ledger.records = new List<DeverQuestTradeRecord>();
            }
        }

        private static void Save()
        {
            EditorPrefs.SetString(
                LedgerKey, JsonUtility.ToJson(ledger));
        }
    }
}
