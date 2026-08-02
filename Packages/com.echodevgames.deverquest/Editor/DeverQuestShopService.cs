using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestPurchaseStatus
    {
        Requested = 0,
        Purchased = 1,
        Approved = 2,
        Denied = 3,
        Redeemed = 4
    }

    [Serializable]
    internal sealed class DeverQuestPurchaseRecord
    {
        public string purchaseId = string.Empty;
        public string accountId = string.Empty;
        public string developerName = string.Empty;
        public string adventurerName = string.Empty;
        public string shopItemId = string.Empty;
        public string itemName = string.Empty;
        public DeverQuestShopItemType itemType;
        public long copperCost;
        public DeverQuestPurchaseStatus status;
        public string requestedUtc = string.Empty;
        public string resolvedUtc = string.Empty;
        public string resolvedBy = string.Empty;
        public string note = string.Empty;
        public DeverQuestRealRewardType realRewardType;
        public string fulfillmentInstructions = string.Empty;
        public string fulfillmentReference = string.Empty;
        public string fulfilledUtc = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestPurchaseLedger
    {
        public List<DeverQuestPurchaseRecord> records =
            new List<DeverQuestPurchaseRecord>();
    }

    [InitializeOnLoad]
    internal static class DeverQuestShopService
    {
        private const string LedgerKey =
            "EchoDevGames.DeverQuest.GuildShopLedger.v1";
        private const string ActiveProfileGuidKey =
            "EchoDevGames.DeverQuest.ActiveShopProfileGuid.v1";
        private static DeverQuestPurchaseLedger ledger;
        private static Dictionary<string, DeverQuestShopItem>
            itemCache;
        private static DeverQuestShopProfile activeProfile;

        static DeverQuestShopService()
        {
            Load();
            EditorApplication.projectChanged -= ClearItemCache;
            EditorApplication.projectChanged += ClearItemCache;
        }

        public static IReadOnlyList<DeverQuestPurchaseRecord> Records =>
            ledger.records;

        public static DeverQuestShopProfile ActiveProfile
        {
            get
            {
                if (activeProfile != null)
                {
                    return activeProfile;
                }

                string guid = EditorPrefs.GetString(
                    ActiveProfileGuidKey, string.Empty);
                if (!string.IsNullOrWhiteSpace(guid))
                {
                    activeProfile = AssetDatabase.LoadAssetAtPath<
                        DeverQuestShopProfile>(
                        AssetDatabase.GUIDToAssetPath(guid));
                }
                if (activeProfile == null)
                {
                    string first = AssetDatabase.FindAssets(
                        "t:DeverQuestShopProfile").FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(first))
                    {
                        activeProfile = AssetDatabase.LoadAssetAtPath<
                            DeverQuestShopProfile>(
                            AssetDatabase.GUIDToAssetPath(first));
                    }
                }
                return activeProfile;
            }
        }

        public static void SetActiveProfile(
            DeverQuestShopProfile profile)
        {
            activeProfile = profile;
            if (profile == null)
            {
                EditorPrefs.DeleteKey(ActiveProfileGuidKey);
                return;
            }
            string path = AssetDatabase.GetAssetPath(profile);
            string guid = AssetDatabase.AssetPathToGUID(path);
            EditorPrefs.SetString(ActiveProfileGuidKey, guid);
        }

        public static bool CanBrowse(
            DeverQuestShopProfile profile,
            out string message)
        {
            return CanTransact(profile, false, out message);
        }

        public static bool CanSellAtActiveShop(out string message)
        {
            DeverQuestShopProfile profile = ActiveProfile;
            if (!CanTransact(profile, false, out message))
            {
                return false;
            }
            if (!profile.buyItemsFromMembers)
            {
                message =
                    "The active Quartermaster is not buying items from " +
                    "members.";
                return false;
            }
            return true;
        }

        public static bool Purchase(
            DeverQuestShopItem item,
            out string message)
        {
            return Purchase(ActiveProfile, item, out message);
        }

        public static bool Purchase(
            DeverQuestShopProfile profile,
            DeverQuestShopItem item,
            out string message)
        {
            message = string.Empty;
            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.CurrentAccount;
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (item == null || account == null)
            {
                message = "Select a valid Shop Item.";
                return false;
            }
            if (!CanTransact(profile, true, out message))
            {
                return false;
            }
            if (!(profile.items ?? new List<DeverQuestShopItem>())
                    .Contains(item))
            {
                message =
                    "This item is not stocked by the active Quartermaster.";
                return false;
            }
            if (adventurer.level < item.minimumLevel)
            {
                message = $"Requires Level {item.minimumLevel}.";
                return false;
            }
            if (OwnedQuantity(adventurer, item.ShopItemId) >=
                item.maximumOwned)
            {
                message = "Maximum owned quantity reached.";
                return false;
            }

            DeverQuestPurchaseRecord record = CreateRecord(
                account, item);
            bool thresholdApproval =
                profile.leadershipApprovalThresholdCopper > 0 &&
                item.copperCost >=
                profile.leadershipApprovalThresholdCopper;
            bool requiresApproval =
                (item.requiresLeadershipApproval || thresholdApproval) &&
                !DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (requiresApproval)
            {
                record.status = DeverQuestPurchaseStatus.Requested;
                ledger.records.Insert(0, record);
                Save();
                DeverQuestEconomyService.RecordPurchaseRequest(record);
                DeverQuestGuildAccountService.AddAudit(
                    "Purchase Requested",
                    item.displayName,
                    adventurer.characterName);
                message = "Purchase sent to Guild leadership for approval.";
                return true;
            }
            if (!DeverQuestAdventurerService.SpendCopper(
                    item.copperCost, out message))
            {
                return false;
            }
            GrantToAdventurer(adventurer, item);
            DeverQuestAdventurerService.Save();
            record.status = DeverQuestPurchaseStatus.Purchased;
            record.resolvedUtc = DateTime.UtcNow.ToString("O");
            ledger.records.Insert(0, record);
            Save();
            DeverQuestEconomyService.RecordPurchase(record);
            RecordCoinSpend(item);
            DeverQuestGuildAccountService.AddAudit(
                "Shop Purchase",
                item.displayName,
                DeverQuestAdventurerService.FormatCoins(
                    item.copperCost));
            DeverQuestAudioDirector.PlayCue(
                DeverQuestAudioCue.Purchase);
            message = $"{item.displayName} added to inventory.";
            return true;
        }

        public static bool Resolve(
            DeverQuestPurchaseRecord record,
            bool approve,
            out string message)
        {
            message = string.Empty;
            if (record == null ||
                record.status != DeverQuestPurchaseStatus.Requested)
            {
                message = "This request is no longer pending.";
                return false;
            }
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                message = "Guild leadership permission is required.";
                return false;
            }
            record.resolvedUtc = DateTime.UtcNow.ToString("O");
            record.resolvedBy =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.developerName ?? string.Empty;
            if (!approve)
            {
                record.status = DeverQuestPurchaseStatus.Denied;
                Save();
                DeverQuestEconomyService.RecordPurchaseDenied(record);
                message = "Purchase request denied.";
                return true;
            }
            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.FindAccount(
                    record.accountId);
            DeverQuestShopItem item = FindItem(record.shopItemId);
            if (account == null || item == null)
            {
                message = "Account or Shop Item could not be resolved.";
                return false;
            }
            if (account.copperBalance < record.copperCost)
            {
                message = "The Adventurer no longer has enough coin.";
                return false;
            }
            account.copperBalance -= record.copperCost;
            account.totalCopperSpent += record.copperCost;
            NormalizeAccountCoinPurse(account);
            GrantToAccount(account, item);
            record.status = DeverQuestPurchaseStatus.Approved;
            DeverQuestGuildAccountService.CommitAccountChanges(account);
            Save();
            DeverQuestEconomyService.RecordPurchaseApproved(record);
            DeverQuestGuildAccountService.AddAudit(
                "Purchase Approved",
                item.displayName,
                account.characterName);
            message = "Purchase approved and delivered.";
            return true;
        }

        public static bool Use(
            DeverQuestShopItem item,
            out string message)
        {
            message = string.Empty;
            if (item == null)
            {
                message = "Select a valid inventory item.";
                return false;
            }

            DeverQuestInventoryEntry entry =
                DeverQuestAdventurerService.Adventurer.inventory
                    .FirstOrDefault(value =>
                        value != null &&
                        value.shopItemId == item.ShopItemId &&
                        value.quantity > 0);
            return UseResolvedEntry(entry, item, out message);
        }

        public static bool UseInventoryEntry(
            string ownershipId,
            out string message)
        {
            message = string.Empty;
            DeverQuestInventoryEntry entry =
                DeverQuestAdventurerService.Adventurer.inventory
                    .FirstOrDefault(value =>
                        value != null &&
                        value.ownershipId == ownershipId &&
                        value.quantity > 0);
            if (entry == null)
            {
                message = "This inventory entry was not found.";
                return false;
            }

            DeverQuestShopItem item =
                FindItem(entry.shopItemId);
            return UseResolvedEntry(entry, item, out message);
        }

        private static bool UseResolvedEntry(
            DeverQuestInventoryEntry entry,
            DeverQuestShopItem item,
            out string message)
        {
            message = string.Empty;
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (entry == null || entry.quantity <= 0)
            {
                message = "This item is not in your inventory.";
                return false;
            }
            if (item == null)
            {
                message =
                    "The source Shop Item asset could not be resolved.";
                return false;
            }
            if (item.itemType ==
                DeverQuestShopItemType.Redemption)
            {
                message =
                    "This real-world reward remains in the fulfillment " +
                    "queue until Guild leadership marks it delivered.";
                return false;
            }
            if (item.itemType == DeverQuestShopItemType.BreakPermit &&
                !DeverQuestSessionStore.PauseForApprovedBreak(
                    item.approvedBreakMinutes,
                    item.displayName))
            {
                message =
                    "Begin a running Quest before using a break permit.";
                return false;
            }

            ApplyWellness(adventurer, item);
            if (!item.reusable)
            {
                entry.quantity--;
                adventurer.inventory.RemoveAll(
                    value => value == null || value.quantity <= 0);
            }
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Inventory Used",
                item.displayName,
                adventurer.characterName);
            message = item.itemType ==
                      DeverQuestShopItemType.BreakPermit
                ? $"Approved break started for " +
                  $"{item.approvedBreakMinutes} minute(s)."
                : $"{item.displayName} used.";
            return true;
        }

        public static bool MarkFulfilled(
            DeverQuestPurchaseRecord record,
            string fulfillmentReference,
            out string message)
        {
            message = string.Empty;
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                message =
                    "Guild leadership permission is required.";
                return false;
            }
            if (record == null ||
                record.itemType !=
                DeverQuestShopItemType.Redemption ||
                record.status !=
                DeverQuestPurchaseStatus.Approved)
            {
                message =
                    "Select an approved real-reward redemption.";
                return false;
            }

            record.status =
                DeverQuestPurchaseStatus.Redeemed;
            record.fulfilledUtc =
                DateTime.UtcNow.ToString("O");
            record.resolvedBy =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.developerName ?? string.Empty;
            record.fulfillmentReference =
                fulfillmentReference?.Trim() ?? string.Empty;

            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.FindAccount(
                    record.accountId);
            if (account != null)
            {
                DeverQuestInventoryEntry entry =
                    account.inventory.FirstOrDefault(value =>
                        value.shopItemId == record.shopItemId);
                if (entry != null)
                {
                    entry.quantity--;
                    account.inventory.RemoveAll(
                        value => value.quantity <= 0);
                    DeverQuestGuildAccountService
                        .CommitAccountChanges(account);
                }
            }
            Save();
            DeverQuestEconomyService.RecordRedemptionFulfilled(record);
            DeverQuestGuildAccountService.AddAudit(
                "Real Reward Fulfilled",
                record.itemName,
                string.IsNullOrWhiteSpace(
                    record.fulfillmentReference)
                    ? record.realRewardType.ToString()
                    : record.fulfillmentReference);
            message =
                $"{record.itemName} marked fulfilled.";
            return true;
        }

        private static DeverQuestPurchaseRecord CreateRecord(
            DeverQuestGuildAccount account,
            DeverQuestShopItem item)
        {
            return new DeverQuestPurchaseRecord
            {
                purchaseId = Guid.NewGuid().ToString("N"),
                accountId = account.accountId,
                developerName = account.developerName,
                adventurerName = account.characterName,
                shopItemId = item.ShopItemId,
                itemName = item.displayName,
                itemType = item.itemType,
                copperCost = item.copperCost,
                realRewardType = item.realRewardType,
                fulfillmentInstructions =
                    item.fulfillmentInstructions,
                requestedUtc = DateTime.UtcNow.ToString("O")
            };
        }

        private static void GrantToAdventurer(
            DeverQuestAdventurer adventurer,
            DeverQuestShopItem item)
        {
            DeverQuestInventoryService.AddItem(
                adventurer.inventory,
                item,
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty,
                DeverQuestItemOriginKind.GuildShop,
                "Guild Shop");
            if (item.itemType == DeverQuestShopItemType.Equipment &&
                item.equipment != null &&
                item.autoEquipOnAcquire)
            {
                DeverQuestRulesService.Equip(
                    adventurer, item.equipment);
            }
            if (item.itemType == DeverQuestShopItemType.Spell &&
                item.spell != null &&
                !adventurer.knownSpellIds.Contains(item.spell.SpellId))
            {
                adventurer.knownSpellIds.Add(item.spell.SpellId);
            }
        }

        private static void GrantToAccount(
            DeverQuestGuildAccount account,
            DeverQuestShopItem item)
        {
            DeverQuestInventoryService.AddItem(
                account.inventory,
                item,
                account.accountId,
                DeverQuestItemOriginKind.GuildShop,
                "Guild Shop Approval");
            if (item.itemType == DeverQuestShopItemType.Equipment &&
                item.equipment != null &&
                item.autoEquipOnAcquire)
            {
                account.equippedEquipmentIds.RemoveAll(
                    id =>
                    {
                        DeverQuestEquipment existing =
                            DeverQuestRulesService.FindEquipment(id);
                        return existing == null ||
                               existing.slot == item.equipment.slot;
                    });
                account.equippedEquipmentIds.Add(
                    item.equipment.EquipmentId);
            }
            if (item.itemType == DeverQuestShopItemType.Spell &&
                item.spell != null &&
                !account.knownSpellIds.Contains(item.spell.SpellId))
            {
                account.knownSpellIds.Add(item.spell.SpellId);
            }
        }

        private static void NormalizeAccountCoinPurse(
            DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return;
            }
            long total = Math.Max(0L, account.copperBalance);
            account.platinumCoins = total / 1000000L;
            total %= 1000000L;
            account.goldCoins = total / 10000L;
            total %= 10000L;
            account.silverCoins = total / 100L;
            account.copperCoins = total % 100L;
        }

        private static int OwnedQuantity(
            DeverQuestAdventurer adventurer,
            string itemId)
        {
            return adventurer.inventory
                .Where(item => item.shopItemId == itemId)
                .Sum(item => item.quantity);
        }

        private static void ApplyWellness(
            DeverQuestAdventurer adventurer,
            DeverQuestShopItem item)
        {
            adventurer.currentHitPoints = Math.Min(
                adventurer.maximumHitPoints,
                adventurer.currentHitPoints + item.restoreHitPoints);
            adventurer.currentMana = Math.Min(
                adventurer.maximumMana,
                adventurer.currentMana + item.restoreMana);
            adventurer.hunger = ClampNeed(
                adventurer.hunger + item.hungerChange);
            adventurer.rest = ClampNeed(
                adventurer.rest + item.restChange);
            adventurer.happiness = ClampNeed(
                adventurer.happiness + item.happinessChange);
        }

        private static int ClampNeed(int value)
        {
            return Math.Min(100, Math.Max(0, value));
        }

        private static void RecordCoinSpend(
            DeverQuestShopItem item)
        {
            DeverQuestSessionStore.AddRewardTransaction(
                new DeverQuestRewardTransaction
                {
                    categoryName = "Guild Shop",
                    transactionType = "Purchased",
                    copper = -item.copperCost,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    note = item.displayName
                });
        }

        public static DeverQuestShopItem FindItem(string itemId)
        {
            if (itemCache == null)
            {
                itemCache =
                    new Dictionary<string, DeverQuestShopItem>();
                foreach (string guid in AssetDatabase.FindAssets(
                             "t:DeverQuestShopItem"))
                {
                    DeverQuestShopItem item =
                        AssetDatabase.LoadAssetAtPath<
                            DeverQuestShopItem>(
                            AssetDatabase.GUIDToAssetPath(guid));
                    if (item != null)
                    {
                        itemCache[item.ShopItemId] = item;
                    }
                }
            }
            return itemCache.TryGetValue(
                itemId ?? string.Empty,
                out DeverQuestShopItem found)
                ? found
                : null;
        }

        private static void ClearItemCache()
        {
            itemCache = null;
            activeProfile = null;
        }

        private static bool CanTransact(
            DeverQuestShopProfile profile,
            bool purchase,
            out string message)
        {
            if (profile == null)
            {
                message =
                    "Select an active Shop Profile in Guild Economy.";
                return false;
            }
            bool leadership =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (!profile.shopOpen && !leadership)
            {
                message = string.IsNullOrWhiteSpace(profile.closedMessage)
                    ? "The Quartermaster is currently closed."
                    : profile.closedMessage;
                return false;
            }
            if (!profile.availableToMembers && !leadership)
            {
                message =
                    "This Quartermaster is restricted to Guild leadership.";
                return false;
            }
            if (purchase && !profile.allowPurchases)
            {
                message =
                    "Purchases are currently disabled for this Shop Profile.";
                return false;
            }
            message = string.Empty;
            return true;
        }

        private static void Load()
        {
            ledger = JsonUtility.FromJson<DeverQuestPurchaseLedger>(
                         EditorPrefs.GetString(
                             LedgerKey, string.Empty)) ??
                     new DeverQuestPurchaseLedger();
            ledger.records = ledger.records ??
                             new List<DeverQuestPurchaseRecord>();
        }

        private static void Save()
        {
            EditorPrefs.SetString(
                LedgerKey, JsonUtility.ToJson(ledger));
        }
    }
}
