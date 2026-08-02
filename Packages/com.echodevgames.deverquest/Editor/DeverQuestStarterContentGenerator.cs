using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestStarterContentGenerator
    {
        private const string Root = "Assets/DeverQuest";
        private const string GearRoot =
            "Assets/DeverQuest/StarterGear";
        private const string ShopRoot =
            "Assets/DeverQuest/GuildShop";
        private const string EncounterRoot =
            "Assets/DeverQuest/Encounters";
        private const string CombatRoot =
            "Assets/DeverQuest/Combat";

        public static DeverQuestCombatTypeCatalog
            GenerateCombatCodex()
        {
            EnsureFolder("Assets", "DeverQuest");
            EnsureFolder(Root, "Combat");
            string path = CombatRoot + "/GuildCombatCodex.asset";
            DeverQuestCombatTypeCatalog catalog =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestCombatTypeCatalog>(path);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<
                    DeverQuestCombatTypeCatalog>();
                AssetDatabase.CreateAsset(catalog, path);
            }
            catalog.displayName = "Guild Combat Codex";
            catalog.description =
                "Original, studio-owned combat vocabulary for typed " +
                "damage, creature families, and encounter authoring.";
            catalog.creatureTypes.Clear();
            foreach (DeverQuestCreatureType type in
                     System.Enum.GetValues(
                         typeof(DeverQuestCreatureType)))
            {
                catalog.creatureTypes.Add(type);
            }
            catalog.damageTypes.Clear();
            foreach (DeverQuestDamageType type in
                     System.Enum.GetValues(
                         typeof(DeverQuestDamageType)))
            {
                catalog.damageTypes.Add(type);
            }
            EditorUtility.SetDirty(catalog);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return catalog;
        }

        public static int GenerateBasicGear()
        {
            EnsureFolder("Assets", "DeverQuest");
            EnsureFolder(Root, "StarterGear");
            string[] materials =
            {
                "Copper", "Bronze", "Iron", "Steel"
            };
            int created = 0;
            foreach (string material in materials)
            {
                string folder = GearRoot + "/" + material;
                EnsureFolder(GearRoot, material);
                foreach (DeverQuestEquipmentSlot slot in
                         System.Enum.GetValues(
                             typeof(DeverQuestEquipmentSlot)))
                {
                    string assetPath =
                        $"{folder}/{material}_{slot}.asset";
                    if (AssetDatabase.LoadAssetAtPath<
                            DeverQuestEquipment>(assetPath) != null)
                    {
                        continue;
                    }
                    DeverQuestEquipment item =
                        ScriptableObject.CreateInstance<
                            DeverQuestEquipment>();
                    item.displayName =
                        $"{material} {ReadableSlot(slot)}";
                    item.description =
                        $"Starter {material.ToLowerInvariant()} equipment " +
                        $"for the {ReadableSlot(slot)} slot.";
                    item.slot = slot;
                    item.equipmentFamily =
                        DefaultEquipmentFamily(slot);
                    item.tags = new List<string>
                    {
                        "Starter Gear",
                        material,
                        ReadableSlot(slot)
                    };
                    item.materialTier = material;
                    int tier = System.Array.IndexOf(
                        materials, material) + 1;
                    item.armorClassBonus =
                        IsArmorSlot(slot) ? tier : 0;
                    item.copperValue = tier * 25;
                    item.minimumLevel = tier;
                    item.damageDice =
                        slot == DeverQuestEquipmentSlot.MainHand
                            ? $"{tier}d4"
                            : string.Empty;
                    AssetDatabase.CreateAsset(item, assetPath);
                    created++;
                }
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return created;
        }

        public static DeverQuestShopProfile GenerateBasicShop()
        {
            EnsureFolder("Assets", "DeverQuest");
            EnsureFolder(Root, "GuildShop");
            DeverQuestShopProfile profile =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestShopProfile>(
                    ShopRoot + "/GuildQuartermaster.asset");
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<
                    DeverQuestShopProfile>();
                profile.displayName = "Guild Quartermaster";
                profile.welcomeMessage =
                    "Provisions, rest, potions, and sanctioned breaks " +
                    "for working Adventurers.";
                AssetDatabase.CreateAsset(
                    profile,
                    ShopRoot + "/GuildQuartermaster.asset");
            }
            AddShopItem(
                profile, "Trail_Rations", "Trail Rations",
                DeverQuestShopItemType.Food, 10, 25, 0, 5, 0);
            AddShopItem(
                profile, "Fresh_Water", "Fresh Water",
                DeverQuestShopItemType.Drink, 5, 0, 0, 3, 20);
            AddShopItem(
                profile, "Inn_Rest", "A Night at the Inn",
                DeverQuestShopItemType.InnRest, 35, 10, 60, 15, 0);
            AddShopItem(
                profile, "Healing_Draught", "Minor Healing Draught",
                DeverQuestShopItemType.Consumable, 20, 0, 0, 3, 0,
                8, 0);
            AddShopItem(
                profile, "Meditation_Tonic", "Meditation Tonic",
                DeverQuestShopItemType.Consumable, 20, 0, 0, 3, 0,
                0, 8);
            AddShopItem(
                profile, "Smoke_Break_Permit",
                "Sanctioned Smoke Break",
                DeverQuestShopItemType.BreakPermit, 15, 0, 0, 5, 0,
                0, 0, 10);
            AddShopItem(
                profile, "Privy_Break_Permit",
                "Sanctioned Privy Break",
                DeverQuestShopItemType.BreakPermit, 5, 0, 0, 2, 0,
                0, 0, 5);
            EditorUtility.SetDirty(profile);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return profile;
        }

        public static DeverQuestEncounterProfile
            GenerateTrainingEncounter()
        {
            EnsureFolder("Assets", "DeverQuest");
            EnsureFolder(Root, "Encounters");
            DeverQuestMonsterProfile rat =
                CreateMonster(
                    "Training_Rat",
                    "Training Rat",
                    6, 10, 1, "1d4", 3, 5);
            DeverQuestMonsterProfile goblin =
                CreateMonster(
                    "Goblin_Foreman",
                    "Goblin Foreman",
                    12, 12, 3, "1d6", 8, 15);
            rat.creatureType = DeverQuestCreatureType.Beast;
            rat.attackDamageType =
                DeverQuestDamageType.Piercing;
            goblin.creatureType =
                DeverQuestCreatureType.Humanoid;
            goblin.attackDamageType =
                DeverQuestDamageType.Slashing;
            EditorUtility.SetDirty(rat);
            EditorUtility.SetDirty(goblin);
            string path =
                EncounterRoot + "/Guildhall_Training.asset";
            DeverQuestEncounterProfile encounter =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestEncounterProfile>(path);
            if (encounter == null)
            {
                encounter = ScriptableObject.CreateInstance<
                    DeverQuestEncounterProfile>();
                AssetDatabase.CreateAsset(encounter, path);
            }
            encounter.displayName = "Guildhall Training Bout";
            encounter.storyIntroduction =
                "A controlled Guildhall trial for new Adventurers.";
            encounter.allowInjury = true;
            encounter.allowCharacterDeath = false;
            encounter.victoryCopperBonus = 10;
            encounter.victoryExperienceBonus = 20;
            encounter.waves.Clear();
            encounter.waves.Add(
                new DeverQuestEncounterWave
                {
                    waveTitle = "Vermin Drill",
                    monster = rat,
                    count = 2
                });
            encounter.waves.Add(
                new DeverQuestEncounterWave
                {
                    waveTitle = "Training Captain",
                    monster = goblin,
                    count = 1,
                    bossWave = true
                });
            EditorUtility.SetDirty(encounter);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return encounter;
        }

        private static DeverQuestMonsterProfile CreateMonster(
            string fileName,
            string displayName,
            int hitPoints,
            int armorClass,
            int attackModifier,
            string damageDice,
            int copper,
            int experience)
        {
            string path =
                $"{EncounterRoot}/{fileName}.asset";
            DeverQuestMonsterProfile monster =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestMonsterProfile>(path);
            if (monster == null)
            {
                monster = ScriptableObject.CreateInstance<
                    DeverQuestMonsterProfile>();
                AssetDatabase.CreateAsset(monster, path);
            }
            monster.displayName = displayName;
            monster.maximumHitPoints = hitPoints;
            monster.armorClass = armorClass;
            monster.attackModifier = attackModifier;
            monster.damageDice = damageDice;
            monster.victoryCopper = copper;
            monster.victoryExperience = experience;
            EditorUtility.SetDirty(monster);
            return monster;
        }

        private static void AddShopItem(
            DeverQuestShopProfile profile,
            string fileName,
            string displayName,
            DeverQuestShopItemType itemType,
            int copperCost,
            int hunger,
            int rest,
            int happiness,
            int mana,
            int hitPoints = 0,
            int extraMana = 0,
            int breakMinutes = 0)
        {
            string path = $"{ShopRoot}/{fileName}.asset";
            DeverQuestShopItem item =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestShopItem>(path);
            if (item == null)
            {
                item = ScriptableObject.CreateInstance<
                    DeverQuestShopItem>();
                AssetDatabase.CreateAsset(item, path);
            }
            item.displayName = displayName;
            item.description =
                $"A Guild-approved {displayName.ToLowerInvariant()}.";
            item.itemType = itemType;
            item.itemCategory =
                DeverQuestInventoryEntry.InferCategory(itemType);
            item.subcategory = itemType.ToString();
            item.tags = new List<string>
            {
                "Starter",
                "Quartermaster"
            };
            item.merchantSellValueCopper =
                Math.Max(0, copperCost / 2);
            item.copperCost = copperCost;
            item.hungerChange = hunger;
            item.restChange = rest;
            item.happinessChange = happiness;
            item.restoreMana = mana + extraMana;
            item.restoreHitPoints = hitPoints;
            item.approvedBreakMinutes = breakMinutes;
            EditorUtility.SetDirty(item);
            if (!profile.items.Contains(item))
            {
                profile.items.Add(item);
            }
        }

        private static DeverQuestEquipmentFamily
            DefaultEquipmentFamily(
                DeverQuestEquipmentSlot slot)
        {
            switch (slot)
            {
                case DeverQuestEquipmentSlot.MainHand:
                    return DeverQuestEquipmentFamily.Sword;
                case DeverQuestEquipmentSlot.OffHand:
                    return DeverQuestEquipmentFamily.Shield;
                case DeverQuestEquipmentSlot.Trinket:
                case DeverQuestEquipmentSlot.Neck:
                case DeverQuestEquipmentSlot.EarLeft:
                case DeverQuestEquipmentSlot.EarRight:
                case DeverQuestEquipmentSlot.RingLeft:
                case DeverQuestEquipmentSlot.RingRight:
                    return DeverQuestEquipmentFamily.Trinket;
                case DeverQuestEquipmentSlot.Shirt:
                    return DeverQuestEquipmentFamily.Clothing;
                default:
                    return DeverQuestEquipmentFamily.Armor;
            }
        }

        private static bool IsArmorSlot(
            DeverQuestEquipmentSlot slot)
        {
            return slot == DeverQuestEquipmentSlot.Helm ||
                   slot == DeverQuestEquipmentSlot.Chest ||
                   slot == DeverQuestEquipmentSlot.Shoulders ||
                   slot == DeverQuestEquipmentSlot.Back ||
                   slot == DeverQuestEquipmentSlot.Legs ||
                   slot == DeverQuestEquipmentSlot.Boots ||
                   slot == DeverQuestEquipmentSlot.OffHand;
        }

        private static string ReadableSlot(
            DeverQuestEquipmentSlot slot)
        {
            return slot.ToString()
                .Replace("Left", " Left")
                .Replace("Right", " Right")
                .Replace("MainHand", "Main Hand")
                .Replace("OffHand", "Off Hand");
        }

        private static void EnsureFolder(
            string parent,
            string child)
        {
            string path = parent + "/" + child;
            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder(parent, child);
            }
        }
    }
}
