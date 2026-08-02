using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestEconomyTransactionType
    {
        PurchaseRequested = 0,
        Purchase = 1,
        PurchaseApproved = 2,
        PurchaseDenied = 3,
        Sale = 4,
        ItemGrant = 5,
        CoinGrant = 6,
        DenominationExchange = 7,
        RedemptionFulfilled = 8
    }

    [Serializable]
    internal sealed class DeverQuestEconomyTransaction
    {
        public string transactionId = string.Empty;
        public DeverQuestEconomyTransactionType transactionType;
        public string createdUtc = string.Empty;
        public string actorAccountId = string.Empty;
        public string actorName = string.Empty;
        public string targetAccountId = string.Empty;
        public string targetDeveloperName = string.Empty;
        public string targetAdventurerName = string.Empty;
        public string shopItemId = string.Empty;
        public string itemName = string.Empty;
        public int quantity;
        public long copperAmount;
        public long balanceDeltaCopper;
        public long balanceAfterCopper;
        public long coinPiecesBefore;
        public long coinPiecesAfter;
        public string relatedRecordId = string.Empty;
        public string note = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestEconomyLedger
    {
        public int dataVersion = 1;
        public List<DeverQuestEconomyTransaction> records =
            new List<DeverQuestEconomyTransaction>();
    }

    [InitializeOnLoad]
    internal static class DeverQuestEconomyService
    {
        private const string LedgerKey =
            "EchoDevGames.DeverQuest.EconomyLedger.v1";
        private const int MaximumRecords = 1000;
        private static DeverQuestEconomyLedger ledger;

        static DeverQuestEconomyService()
        {
            Load();
        }

        public static IReadOnlyList<DeverQuestEconomyTransaction> Records =>
            ledger.records;

        public static bool GrantItem(
            DeverQuestGuildAccount target,
            DeverQuestShopItem item,
            int quantity,
            string note,
            out string message)
        {
            message = string.Empty;
            if (!CanManage(out message))
            {
                return false;
            }
            if (target == null || target.disabled)
            {
                message = "Select an enabled Guild account.";
                return false;
            }
            if (item == null)
            {
                message = "Select a Shop Item to grant.";
                return false;
            }
            if (item.itemType == DeverQuestShopItemType.Redemption)
            {
                message =
                    "Real-world redemptions must use the approval and " +
                    "fulfillment workflow.";
                return false;
            }

            quantity = Math.Max(1, quantity);
            target.inventory = target.inventory ??
                new List<DeverQuestInventoryEntry>();
            int owned = target.inventory
                .Where(entry =>
                    entry != null &&
                    entry.shopItemId == item.ShopItemId)
                .Sum(entry => entry.quantity);
            if (owned + quantity > item.maximumOwned)
            {
                message =
                    $"The grant would exceed the maximum owned quantity " +
                    $"of {item.maximumOwned}.";
                return false;
            }

            for (int index = 0; index < quantity; index++)
            {
                DeverQuestInventoryService.AddItem(
                    target.inventory,
                    item,
                    target.accountId,
                    DeverQuestItemOriginKind.LeadershipGrant,
                    string.IsNullOrWhiteSpace(note)
                        ? "Guild Leadership Grant"
                        : "Guild Leadership Grant: " + note.Trim());
            }
            if (item.itemType == DeverQuestShopItemType.Spell &&
                item.spell != null)
            {
                target.knownSpellIds = target.knownSpellIds ??
                    new List<string>();
                if (!target.knownSpellIds.Contains(item.spell.SpellId))
                {
                    target.knownSpellIds.Add(item.spell.SpellId);
                }
            }

            DeverQuestGuildAccountService.CommitAccountChanges(target);
            AddRecord(
                DeverQuestEconomyTransactionType.ItemGrant,
                target,
                item,
                quantity,
                0L,
                0L,
                string.Empty,
                note);
            DeverQuestGuildAccountService.AddAudit(
                "Leadership Item Grant",
                item.displayName,
                $"{quantity} to {target.characterName}");
            message =
                $"Granted {quantity} × {item.displayName} to " +
                $"{target.characterName}.";
            return true;
        }

        public static bool GrantCoin(
            DeverQuestGuildAccount target,
            long copper,
            string note,
            out string message)
        {
            message = string.Empty;
            if (!CanManage(out message))
            {
                return false;
            }
            if (target == null || target.disabled)
            {
                message = "Select an enabled Guild account.";
                return false;
            }
            if (copper <= 0L)
            {
                message = "Enter a positive coin grant.";
                return false;
            }

            target.copperBalance += copper;
            target.copperCoins += copper;
            target.totalCopperEarned += copper;
            DeverQuestGuildAccountService.CommitAccountChanges(target);
            AddRecord(
                DeverQuestEconomyTransactionType.CoinGrant,
                target,
                null,
                0,
                copper,
                copper,
                string.Empty,
                note);
            DeverQuestGuildAccountService.AddAudit(
                "Leadership Coin Grant",
                target.characterName,
                DeverQuestAdventurerService.FormatCoins(copper));
            message =
                $"Granted {DeverQuestAdventurerService.FormatCoins(copper)} " +
                $"to {target.characterName}.";
            return true;
        }

        public static void RecordPurchaseRequest(
            DeverQuestPurchaseRecord purchase)
        {
            AddPurchaseRecord(
                DeverQuestEconomyTransactionType.PurchaseRequested,
                purchase,
                0L);
        }

        public static void RecordPurchase(
            DeverQuestPurchaseRecord purchase)
        {
            AddPurchaseRecord(
                DeverQuestEconomyTransactionType.Purchase,
                purchase,
                -Math.Max(0L, purchase?.copperCost ?? 0L));
        }

        public static void RecordPurchaseApproved(
            DeverQuestPurchaseRecord purchase)
        {
            AddPurchaseRecord(
                DeverQuestEconomyTransactionType.PurchaseApproved,
                purchase,
                -Math.Max(0L, purchase?.copperCost ?? 0L));
        }

        public static void RecordPurchaseDenied(
            DeverQuestPurchaseRecord purchase)
        {
            AddPurchaseRecord(
                DeverQuestEconomyTransactionType.PurchaseDenied,
                purchase,
                0L);
        }

        public static void RecordRedemptionFulfilled(
            DeverQuestPurchaseRecord purchase)
        {
            AddPurchaseRecord(
                DeverQuestEconomyTransactionType.RedemptionFulfilled,
                purchase,
                0L);
        }

        public static void RecordSale(
            DeverQuestInventoryEntry entry,
            int quantity,
            long copper)
        {
            DeverQuestEconomyTransaction record = AddRecord(
                DeverQuestEconomyTransactionType.Sale,
                DeverQuestGuildAccountService.CurrentAccount,
                DeverQuestShopService.FindItem(entry?.shopItemId),
                quantity,
                copper,
                copper,
                entry?.ownershipId ?? string.Empty,
                entry?.displayName ?? "Inventory Sale");
            if (string.IsNullOrWhiteSpace(record.itemName))
            {
                record.itemName =
                    entry?.displayName ?? "Inventory Sale";
                Save();
            }
        }

        public static void RecordDenominationExchange(
            long piecesBefore,
            long piecesAfter)
        {
            if (piecesBefore == piecesAfter)
            {
                return;
            }

            DeverQuestEconomyTransaction record = AddRecord(
                DeverQuestEconomyTransactionType.DenominationExchange,
                DeverQuestGuildAccountService.CurrentAccount,
                null,
                0,
                0L,
                0L,
                string.Empty,
                "Coin denominations consolidated at Guild Hall.");
            record.coinPiecesBefore = piecesBefore;
            record.coinPiecesAfter = piecesAfter;
            Save();
        }

        public static bool ExportCsv(string path, out string message)
        {
            message = string.Empty;
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(
                    "CreatedUtc,Type,Actor,TargetDeveloper," +
                    "TargetAdventurer,Item,Quantity,CopperAmount," +
                    "BalanceDelta,BalanceAfter,RelatedId,Note");
                foreach (DeverQuestEconomyTransaction record in ledger.records)
                {
                    builder.Append(Csv(record.createdUtc)).Append(',');
                    builder.Append(Csv(record.transactionType.ToString())).Append(',');
                    builder.Append(Csv(record.actorName)).Append(',');
                    builder.Append(Csv(record.targetDeveloperName)).Append(',');
                    builder.Append(Csv(record.targetAdventurerName)).Append(',');
                    builder.Append(Csv(record.itemName)).Append(',');
                    builder.Append(record.quantity.ToString(CultureInfo.InvariantCulture)).Append(',');
                    builder.Append(record.copperAmount.ToString(CultureInfo.InvariantCulture)).Append(',');
                    builder.Append(record.balanceDeltaCopper.ToString(CultureInfo.InvariantCulture)).Append(',');
                    builder.Append(record.balanceAfterCopper.ToString(CultureInfo.InvariantCulture)).Append(',');
                    builder.Append(Csv(record.relatedRecordId)).Append(',');
                    builder.AppendLine(Csv(record.note));
                }
                File.WriteAllText(path, builder.ToString(), Encoding.UTF8);
                message = $"Exported {ledger.records.Count} economy transaction(s).";
                return true;
            }
            catch (Exception exception)
            {
                message = "Economy export failed: " + exception.Message;
                return false;
            }
        }

        public static int DuplicateIdCount()
        {
            return ledger.records
                .Where(record => record != null)
                .GroupBy(record => record.transactionId ?? string.Empty)
                .Count(group =>
                    string.IsNullOrWhiteSpace(group.Key) ||
                    group.Count() > 1);
        }

        private static void AddPurchaseRecord(
            DeverQuestEconomyTransactionType type,
            DeverQuestPurchaseRecord purchase,
            long delta)
        {
            if (purchase == null)
            {
                return;
            }
            DeverQuestGuildAccount target =
                DeverQuestGuildAccountService.FindAccount(
                    purchase.accountId);
            AddRecord(
                type,
                target,
                DeverQuestShopService.FindItem(purchase.shopItemId),
                1,
                purchase.copperCost,
                delta,
                purchase.purchaseId,
                purchase.note);
        }

        private static DeverQuestEconomyTransaction AddRecord(
            DeverQuestEconomyTransactionType type,
            DeverQuestGuildAccount target,
            DeverQuestShopItem item,
            int quantity,
            long copperAmount,
            long balanceDelta,
            string relatedId,
            string note)
        {
            DeverQuestGuildAccount actor =
                DeverQuestGuildAccountService.CurrentAccount;
            DeverQuestEconomyTransaction record =
                new DeverQuestEconomyTransaction
                {
                    transactionId = Guid.NewGuid().ToString("N"),
                    transactionType = type,
                    createdUtc = DateTime.UtcNow.ToString("O"),
                    actorAccountId = actor?.accountId ?? string.Empty,
                    actorName = actor?.developerName ?? "System",
                    targetAccountId = target?.accountId ?? string.Empty,
                    targetDeveloperName =
                        target?.developerName ?? string.Empty,
                    targetAdventurerName =
                        target?.characterName ?? string.Empty,
                    shopItemId = item?.ShopItemId ?? string.Empty,
                    itemName = item?.displayName ?? string.Empty,
                    quantity = Math.Max(0, quantity),
                    copperAmount = Math.Max(0L, copperAmount),
                    balanceDeltaCopper = balanceDelta,
                    balanceAfterCopper = Math.Max(
                        0L, target?.copperBalance ?? 0L),
                    relatedRecordId =
                        relatedId?.Trim() ?? string.Empty,
                    note = note?.Trim() ?? string.Empty
                };
            ledger.records.Insert(0, record);
            if (ledger.records.Count > MaximumRecords)
            {
                ledger.records.RemoveRange(
                    MaximumRecords,
                    ledger.records.Count - MaximumRecords);
            }
            Save();
            return record;
        }

        private static bool CanManage(out string message)
        {
            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                message = string.Empty;
                return true;
            }
            message = "Guild leadership permission is required.";
            return false;
        }

        private static string Csv(string value)
        {
            value = value ?? string.Empty;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        private static void Load()
        {
            try
            {
                ledger = JsonUtility.FromJson<DeverQuestEconomyLedger>(
                    EditorPrefs.GetString(LedgerKey, string.Empty));
            }
            catch
            {
                ledger = null;
            }
            ledger = ledger ?? new DeverQuestEconomyLedger();
            ledger.records = ledger.records ??
                new List<DeverQuestEconomyTransaction>();
            foreach (DeverQuestEconomyTransaction record in ledger.records)
            {
                if (record != null &&
                    string.IsNullOrWhiteSpace(record.transactionId))
                {
                    record.transactionId = Guid.NewGuid().ToString("N");
                }
            }
            Save();
        }

        private static void Save()
        {
            EditorPrefs.SetString(LedgerKey, JsonUtility.ToJson(ledger));
        }
    }
}
