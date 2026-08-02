using System;
using System.Collections.Generic;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    public enum DeverQuestShopItemType
    {
        Equipment = 0,
        Spell = 1,
        Consumable = 2,
        Food = 3,
        Drink = 4,
        InnRest = 5,
        BreakPermit = 6,
        Redemption = 7
    }

    public enum DeverQuestItemCategory
    {
        Unknown = 0,
        Equipment = 1,
        Consumable = 2,
        Provision = 3,
        TradeskillSupply = 4,
        CraftingComponent = 5,
        LoreBook = 6,
        MerchantTrash = 7,
        QuestItem = 8,
        Tool = 9,
        Container = 10,
        Key = 11,
        Trophy = 12,
        CompanionSupply = 13,
        HousingItem = 14,
        EnvironmentalProtection = 15,
        Currency = 16,
        Service = 17,
        Spell = 18,
        Other = 19
    }

    public enum DeverQuestItemOriginKind
    {
        Unknown = 0,
        LegacyMigration = 1,
        GuildShop = 2,
        EncounterLoot = 3,
        Trade = 4,
        StarterLoadout = 5,
        LeadershipGrant = 6,
        QuestReward = 7,
        Imported = 8
    }

    public enum DeverQuestItemRarity
    {
        Common = 0,
        Uncommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        Artifact = 5
    }

    public enum DeverQuestItemBinding
    {
        Unbound = 0,
        BindOnPickup = 1,
        BindOnEquip = 2,
        AccountBound = 3,
        GuildBound = 4
    }

    public enum DeverQuestRealRewardType
    {
        None = 0,
        DiscordNitro = 1,
        Merchandise = 2,
        GiftCard = 3,
        MonetaryBonus = 4,
        Custom = 5
    }

    [Serializable]
    public sealed class DeverQuestInventoryEntry
    {
        public string shopItemId = string.Empty;
        public string displayName = string.Empty;
        public DeverQuestShopItemType itemType;
        public DeverQuestItemCategory itemCategory =
            DeverQuestItemCategory.Unknown;
        public string subcategory = string.Empty;
        public List<string> tags = new List<string>();
        public int quantity;
        public string ownershipId = string.Empty;
        public DeverQuestItemRarity rarity;
        public DeverQuestItemBinding binding;
        public string boundAccountId = string.Empty;
        public bool tradable = true;
        public bool droppable = true;
        public bool questProtected;
        public string acquiredUtc = string.Empty;
        public string acquisitionSource = string.Empty;
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
        public int unitValueCopper;
        [Min(0f)]
        public float unitWeight = 0.25f;

        public void EnsureOwnership(string accountId)
        {
            bool legacyEntry =
                string.IsNullOrWhiteSpace(ownershipId) &&
                string.IsNullOrWhiteSpace(acquiredUtc) &&
                binding == DeverQuestItemBinding.Unbound &&
                string.IsNullOrWhiteSpace(boundAccountId);
            bool legacyClassification =
                itemCategory == DeverQuestItemCategory.Unknown &&
                string.IsNullOrWhiteSpace(originSource) &&
                string.IsNullOrWhiteSpace(originAcquiredUtc) &&
                string.IsNullOrWhiteSpace(sourceContractId) &&
                string.IsNullOrWhiteSpace(sourceRunId) &&
                string.IsNullOrWhiteSpace(sourceEncounterId) &&
                string.IsNullOrWhiteSpace(sourceMonsterId);

            if (string.IsNullOrWhiteSpace(ownershipId))
            {
                ownershipId = Guid.NewGuid().ToString("N");
            }
            if (string.IsNullOrWhiteSpace(acquiredUtc))
            {
                acquiredUtc = DateTime.UtcNow.ToString("O");
            }
            if (legacyEntry)
            {
                tradable = true;
                acquisitionSource = "Legacy Inventory Migration";
                originKind = DeverQuestItemOriginKind.LegacyMigration;
            }

            displayName = displayName?.Trim() ?? string.Empty;
            subcategory = subcategory?.Trim() ?? string.Empty;
            acquisitionSource =
                acquisitionSource?.Trim() ?? string.Empty;
            originSource = originSource?.Trim() ?? string.Empty;
            originAcquiredUtc =
                originAcquiredUtc?.Trim() ?? string.Empty;
            sourceContractId =
                sourceContractId?.Trim() ?? string.Empty;
            sourceRunId = sourceRunId?.Trim() ?? string.Empty;
            sourceEncounterId =
                sourceEncounterId?.Trim() ?? string.Empty;
            sourceMonsterId =
                sourceMonsterId?.Trim() ?? string.Empty;
            sourceMonsterName =
                sourceMonsterName?.Trim() ?? string.Empty;
            equipmentId = equipmentId?.Trim() ?? string.Empty;
            tags = tags ?? new List<string>();
            tags.RemoveAll(value =>
                string.IsNullOrWhiteSpace(value));
            for (int index = 0; index < tags.Count; index++)
            {
                tags[index] = tags[index].Trim();
            }

            if (itemCategory == DeverQuestItemCategory.Unknown)
            {
                itemCategory = InferCategory(itemType);
            }
            if (legacyClassification && !questProtected)
            {
                droppable = true;
            }
            if (originKind == DeverQuestItemOriginKind.Unknown)
            {
                originKind = InferOrigin(acquisitionSource);
            }
            if (string.IsNullOrWhiteSpace(originSource))
            {
                originSource = string.IsNullOrWhiteSpace(
                    acquisitionSource)
                    ? "Unknown source"
                    : acquisitionSource;
            }
            if (string.IsNullOrWhiteSpace(originAcquiredUtc))
            {
                originAcquiredUtc = acquiredUtc;
            }

            if (binding == DeverQuestItemBinding.BindOnPickup ||
                binding == DeverQuestItemBinding.AccountBound)
            {
                boundAccountId =
                    accountId?.Trim() ?? string.Empty;
                tradable = false;
                binding = DeverQuestItemBinding.AccountBound;
            }
            if (questProtected ||
                itemCategory == DeverQuestItemCategory.QuestItem)
            {
                questProtected = true;
                tradable = false;
                droppable = false;
            }

            quantity = Mathf.Max(0, quantity);
            unitWeight = Mathf.Max(0f, unitWeight);
            unitValueCopper = Mathf.Max(0, unitValueCopper);
        }

        public static DeverQuestItemCategory InferCategory(
            DeverQuestShopItemType type)
        {
            switch (type)
            {
                case DeverQuestShopItemType.Equipment:
                    return DeverQuestItemCategory.Equipment;
                case DeverQuestShopItemType.Consumable:
                    return DeverQuestItemCategory.Consumable;
                case DeverQuestShopItemType.Food:
                case DeverQuestShopItemType.Drink:
                    return DeverQuestItemCategory.Provision;
                case DeverQuestShopItemType.Spell:
                    return DeverQuestItemCategory.Spell;
                case DeverQuestShopItemType.InnRest:
                case DeverQuestShopItemType.BreakPermit:
                case DeverQuestShopItemType.Redemption:
                    return DeverQuestItemCategory.Service;
                default:
                    return DeverQuestItemCategory.Other;
            }
        }

        private static DeverQuestItemOriginKind InferOrigin(
            string source)
        {
            source = source?.Trim() ?? string.Empty;
            if (source.IndexOf(
                    "Encounter",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DeverQuestItemOriginKind.EncounterLoot;
            }
            if (source.IndexOf(
                    "Shop",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DeverQuestItemOriginKind.GuildShop;
            }
            if (source.IndexOf(
                    "Trade",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DeverQuestItemOriginKind.Trade;
            }
            if (source.IndexOf(
                    "Starter",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DeverQuestItemOriginKind.StarterLoadout;
            }
            if (source.IndexOf(
                    "Legacy",
                    StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return DeverQuestItemOriginKind.LegacyMigration;
            }
            return DeverQuestItemOriginKind.Unknown;
        }
    }

    [CreateAssetMenu(
        fileName = "NewGuildShopItem",
        menuName = "DeverQuest/Guild Shop/Shop Item")]
    public sealed class DeverQuestShopItem : ScriptableObject
    {
        [SerializeField, HideInInspector]
        private string shopItemId = string.Empty;

        [Header("Identity")]
        public string displayName = "New Guild Shop Item";
        [TextArea(2, 6)]
        public string description = string.Empty;
        [TextArea(2, 6)]
        public string loreText = string.Empty;
        public DeverQuestShopItemType itemType;
        public DeverQuestItemCategory itemCategory =
            DeverQuestItemCategory.Unknown;
        public string subcategory = string.Empty;
        public List<string> tags = new List<string>();

        [Header("Purchase and Inventory")]
        public int copperCost = 10;
        public int merchantSellValueCopper;
        public int minimumLevel = 1;
        public bool requiresLeadershipApproval;
        public bool reusable;
        public int maximumOwned = 99;
        public int maximumStackSize = 99;
        public bool droppable = true;
        public bool questProtected;
        public bool autoEquipOnAcquire = true;
        [Min(0f)]
        public float unitWeight = 0.25f;

        [Header("Rarity, Ownership, and Trading")]
        public DeverQuestItemRarity rarity =
            DeverQuestItemRarity.Common;
        public DeverQuestItemBinding binding =
            DeverQuestItemBinding.Unbound;
        public bool tradable = true;

        [Header("Real Reward Redemption")]
        public DeverQuestRealRewardType realRewardType =
            DeverQuestRealRewardType.None;
        [TextArea(2, 5)]
        public string fulfillmentInstructions = string.Empty;

        [Header("Character Assets")]
        public DeverQuestEquipment equipment;
        public DeverQuestSpell spell;

        [Header("Wellness Effects")]
        public int restoreHitPoints;
        public int restoreMana;
        public int hungerChange;
        public int restChange;
        public int happinessChange;
        public int approvedBreakMinutes;

        public string ShopItemId
        {
            get
            {
                if (string.IsNullOrWhiteSpace(shopItemId))
                {
                    shopItemId = Guid.NewGuid().ToString("N");
                }
                return shopItemId;
            }
        }

        public int EffectiveSellValueCopper =>
            merchantSellValueCopper > 0
                ? merchantSellValueCopper
                : Mathf.Max(0, copperCost / 2);

        private void OnEnable()
        {
            Sanitize();
        }

        private void OnValidate()
        {
            Sanitize();
        }

        private void Sanitize()
        {
            _ = ShopItemId;
            displayName = displayName?.Trim() ?? string.Empty;
            description = description?.Trim() ?? string.Empty;
            loreText = loreText?.Trim() ?? string.Empty;
            subcategory = subcategory?.Trim() ?? string.Empty;
            tags = tags ?? new List<string>();
            tags.RemoveAll(value =>
                string.IsNullOrWhiteSpace(value));
            for (int index = 0; index < tags.Count; index++)
            {
                tags[index] = tags[index].Trim();
            }
            if (itemCategory == DeverQuestItemCategory.Unknown)
            {
                itemCategory =
                    DeverQuestInventoryEntry.InferCategory(itemType);
            }
            copperCost = Mathf.Max(0, copperCost);
            merchantSellValueCopper =
                Mathf.Max(0, merchantSellValueCopper);
            minimumLevel = Mathf.Max(1, minimumLevel);
            maximumOwned = Mathf.Max(1, maximumOwned);
            maximumStackSize = Mathf.Max(1, maximumStackSize);
            unitWeight = Mathf.Max(0f, unitWeight);
            if (itemType == DeverQuestShopItemType.Redemption)
            {
                requiresLeadershipApproval = true;
                tradable = false;
                droppable = false;
                binding = DeverQuestItemBinding.AccountBound;
                maximumOwned = 1;
                maximumStackSize = 1;
                itemCategory = DeverQuestItemCategory.Service;
            }
            if (itemType == DeverQuestShopItemType.Equipment &&
                equipment != null)
            {
                itemCategory = DeverQuestItemCategory.Equipment;
                maximumStackSize = 1;
            }
            if (questProtected ||
                itemCategory == DeverQuestItemCategory.QuestItem)
            {
                questProtected = true;
                tradable = false;
                droppable = false;
            }
            restoreHitPoints = Mathf.Max(0, restoreHitPoints);
            restoreMana = Mathf.Max(0, restoreMana);
            approvedBreakMinutes =
                Mathf.Max(0, approvedBreakMinutes);
        }
    }
}
