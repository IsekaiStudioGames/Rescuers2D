using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestScaffoldReport
    {
        public int FoldersCreated;
        public int AssetsCreated;
        public int ExistingItemsSkipped;
        public string SelectedFolder = string.Empty;
        public DeverQuestQuestContract TutorialContract;
        public DeverQuestShopProfile TutorialShop;

        public string Summary =>
            $"Created {FoldersCreated} folder(s) and " +
            $"{AssetsCreated} asset(s); preserved " +
            $"{ExistingItemsSkipped} existing item(s).";
    }

    internal static class DeverQuestCampaignScaffolder
    {
        internal const string Root = "Assets/DeverQuest";
        internal const string TemplatesRoot =
            Root + "/Templates";
        internal const string DemoRoot =
            Root + "/DemoCampaign";

        private static readonly string[] ProductionFolders =
        {
            "Audio/Ambience",
            "Audio/Music",
            "Audio/WarningProfiles",
            "Characters/Classes",
            "Characters/Equipment",
            "Characters/Companions",
            "Characters/Spells",
            "Characters/Techniques",
            "Characters/AbilityProfiles",
            "Characters/StarterLoadouts",
            "Combat/Codices",
            "Companions/Profiles",
            "Companions/Catalogs",
            "IdentityCatalogs/Ancestries",
            "IdentityCatalogs/Classes",
            "IdentityCatalogs/Faiths",
            "IdentityCatalogs/Catalogs",
            "Guild/Shops",
            "Guild/ShopItems",
            "Guild/Rewards",
            "Quests/Profiles",
            "Quests/Contracts",
            "Quests/Encounters",
            "Quests/Monsters",
            "ActivityProfiles",
            "Playlists",
            "Templates"
        };

        private static readonly string[] DemoFolders =
        {
            "Audio",
            "Characters/Equipment",
            "Characters/Spells",
            "Characters/Techniques",
            "Characters/AbilityProfiles",
            "Characters/StarterLoadouts",
            "Guild",
            "Quests",
            "Encounters",
            "Activity"
        };

        public static DeverQuestScaffoldReport
            CreateProductionStructure()
        {
            DeverQuestScaffoldReport report =
                new DeverQuestScaffoldReport
                {
                    SelectedFolder = TemplatesRoot
                };
            EnsureFolderPath(Root, report);
            foreach (string folder in ProductionFolders)
            {
                EnsureFolderPath(
                    Root + "/" + folder,
                    report);
            }
            CreateBlankTemplates(report);
            Finish(report);
            return report;
        }

        public static DeverQuestScaffoldReport
            CreateTutorialCampaign()
        {
            DeverQuestScaffoldReport report =
                CreateProductionStructureInternal();
            report.SelectedFolder = DemoRoot;
            EnsureFolderPath(DemoRoot, report);
            foreach (string folder in DemoFolders)
            {
                EnsureFolderPath(
                    DemoRoot + "/" + folder,
                    report);
            }

            DeverQuestEquipment graveWand =
                CreateAsset<DeverQuestEquipment>(
                    DemoRoot +
                    "/Characters/Equipment/Grave_Wand.asset",
                    report,
                    item =>
                    {
                        item.displayName = "Initiate's Grave Wand";
                        item.description =
                            "A tutorial focus for a new Necromancer.";
                        item.slot =
                            DeverQuestEquipmentSlot.MainHand;
                        item.equipmentFamily =
                            DeverQuestEquipmentFamily.Wand;
                        item.requiredSkillId = "Wand";
                        item.tags = new List<string>
                        {
                            "Tutorial",
                            "Necromancer",
                            "Arcane Focus"
                        };
                        item.materialTier = "Bone";
                        item.rarity = "Common";
                        item.minimumLevel = 1;
                        item.damageDice = "1d6";
                        item.weaponDamageType =
                            DeverQuestDamageType.Shadow;
                        item.abilityBonusType =
                            DeverQuestAbility.Intelligence;
                        item.abilityBonus = 1;
                        item.copperValue = 20;
                    });
            DeverQuestEquipment rareRing =
                CreateAsset<DeverQuestEquipment>(
                    DemoRoot +
                    "/Characters/Equipment/Ring_of_Focused_Embers.asset",
                    report,
                    item =>
                    {
                        item.displayName =
                            "Ring of Focused Embers";
                        item.description =
                            "Rare tutorial loot recovered from the Crypt.";
                        item.slot =
                            DeverQuestEquipmentSlot.RingLeft;
                        item.equipmentFamily =
                            DeverQuestEquipmentFamily.Trinket;
                        item.tags = new List<string>
                        {
                            "Tutorial",
                            "Rare Loot",
                            "Fire Resistance"
                        };
                        item.materialTier = "Silver";
                        item.rarity = "Rare";
                        item.abilityBonusType =
                            DeverQuestAbility.Intelligence;
                        item.abilityBonus = 1;
                        item.minimumLevel = 1;
                        item.copperValue = 125;
                        item.damageAffinities.Add(
                            new DeverQuestDamageAffinity
                            {
                                damageType =
                                    DeverQuestDamageType.Fire,
                                response =
                                    DeverQuestDamageResponse.Resistant
                            });
                    });
            DeverQuestSpell boneSpark =
                CreateAsset<DeverQuestSpell>(
                    DemoRoot +
                    "/Characters/Spells/Bone_Spark.asset",
                    report,
                    spell =>
                    {
                        spell.displayName = "Bone Spark";
                        spell.description =
                            "A harmless tutorial cantrip for testing " +
                            "spell ownership.";
                        spell.spellLevel = 0;
                        spell.castingAbility =
                            DeverQuestAbility.Intelligence;
                        spell.damageDice = "1d6";
                        spell.damageType =
                            DeverQuestDamageType.Shadow;
                        spell.minimumCharacterLevel = 1;
                    });
            CreateAsset<DeverQuestStarterLoadout>(
                DemoRoot +
                "/Characters/StarterLoadouts/" +
                "Tutorial_Necromancer.asset",
                report,
                loadout =>
                {
                    loadout.displayName =
                        "Tutorial Necromancer";
                    loadout.characterClass = "Necromancer";
                    loadout.department = "Programming";
                    loadout.equipment.Add(graveWand);
                    loadout.spells.Add(boneSpark);
                });

            DeverQuestShopItem rations =
                CreateShopItem(
                    "Crypt_Rations",
                    report,
                    item =>
                    {
                        item.displayName = "Crypt Rations";
                        item.description =
                            "A basic tutorial provision.";
                        item.itemType =
                            DeverQuestShopItemType.Food;
                        item.itemCategory =
                            DeverQuestItemCategory.Provision;
                        item.subcategory = "Rations";
                        item.tags = new List<string>
                        {
                            "Tutorial",
                            "Provision"
                        };
                        item.copperCost = 5;
                        item.merchantSellValueCopper = 2;
                        item.hungerChange = 20;
                    });
            DeverQuestShopItem rareRingLoot =
                CreateShopItem(
                    "Ring_of_Focused_Embers",
                    report,
                    item =>
                    {
                        item.displayName =
                            "Ring of Focused Embers";
                        item.description =
                            "Rare, tradable equipment used by the " +
                            "tutorial loot and trading flow.";
                        item.itemType =
                            DeverQuestShopItemType.Equipment;
                        item.itemCategory =
                            DeverQuestItemCategory.Equipment;
                        item.subcategory = "Ring";
                        item.tags = new List<string>
                        {
                            "Tutorial",
                            "Rare Loot"
                        };
                        item.equipment = rareRing;
                        item.copperCost = 125;
                        item.maximumOwned = 1;
                        item.rarity =
                            DeverQuestItemRarity.Rare;
                        item.binding =
                            DeverQuestItemBinding.BindOnEquip;
                        item.tradable = true;
                    });
            DeverQuestShopItem rewardVoucher =
                CreateShopItem(
                    "Tutorial_Real_Reward_Voucher",
                    report,
                    item =>
                    {
                        item.displayName =
                            "Tutorial Reward Voucher";
                        item.description =
                            "A non-delivering example of the leadership " +
                            "approval and fulfillment workflow.";
                        item.itemType =
                            DeverQuestShopItemType.Redemption;
                        item.itemCategory =
                            DeverQuestItemCategory.Service;
                        item.subcategory = "Real Reward";
                        item.tags = new List<string>
                        {
                            "Tutorial",
                            "Leadership Approval"
                        };
                        item.copperCost = 500;
                        item.realRewardType =
                            DeverQuestRealRewardType.Custom;
                        item.fulfillmentInstructions =
                            "Tutorial only. Do not fulfill.";
                        item.requiresLeadershipApproval = true;
                        item.tradable = false;
                    });
            report.TutorialShop =
                CreateAsset<DeverQuestShopProfile>(
                    DemoRoot +
                    "/Guild/Tutorial_Quartermaster.asset",
                    report,
                    shop =>
                    {
                        shop.displayName =
                            "Tutorial Quartermaster";
                        shop.welcomeMessage =
                            "Test provisions, rare gear, trading, and " +
                            "manual real-reward fulfillment here.";
                        shop.items.Add(rations);
                        shop.items.Add(rareRingLoot);
                        shop.items.Add(rewardVoucher);
                    });

            DeverQuestMonsterProfile skeleton =
                CreateAsset<DeverQuestMonsterProfile>(
                    DemoRoot +
                    "/Encounters/Tutorial_Skeleton.asset",
                    report,
                    monster =>
                    {
                        monster.displayName =
                            "Tutorial Skeleton";
                        monster.description =
                            "A low-risk opponent for the complete demo.";
                        monster.level = 1;
                        monster.maximumHitPoints = 7;
                        monster.armorClass = 10;
                        monster.attackModifier = 1;
                        monster.damageDice = "1d4";
                        monster.attackDamageType =
                            DeverQuestDamageType.Bludgeoning;
                        monster.creatureType =
                            DeverQuestCreatureType.Undead;
                        monster.damageAffinities.Add(
                            new DeverQuestDamageAffinity
                            {
                                damageType =
                                    DeverQuestDamageType.Radiant,
                                response =
                                    DeverQuestDamageResponse.Vulnerable
                            });
                        monster.damageAffinities.Add(
                            new DeverQuestDamageAffinity
                            {
                                damageType =
                                    DeverQuestDamageType.Poison,
                                response =
                                    DeverQuestDamageResponse.Immune
                            });
                        monster.victoryCopper = 8;
                        monster.victoryExperience = 12;
                        monster.dropTable.Add(
                            new DeverQuestDropEntry
                            {
                                displayName =
                                    "Ring of Focused Embers",
                                dropChancePercent = 100,
                                shopItem = rareRingLoot
                            });
                    });
            DeverQuestEncounterProfile encounter =
                CreateAsset<DeverQuestEncounterProfile>(
                    DemoRoot +
                    "/Encounters/Tutorial_Crypt.asset",
                    report,
                    value =>
                    {
                        value.displayName =
                            "Tutorial Crypt Encounter";
                        value.storyIntroduction =
                            "A training skeleton guards the final " +
                            "deliverable in the tutorial crypt.";
                        value.allowInjury = true;
                        value.allowCharacterDeath = false;
                        value.victoryCopperBonus = 10;
                        value.victoryExperienceBonus = 20;
                        value.waves.Add(
                            new DeverQuestEncounterWave
                            {
                                waveTitle =
                                    "The Tutorial Guardian",
                                monster = skeleton,
                                count = 1,
                                bossWave = true
                            });
                    });
            DeverQuestQuestProfile questProfile =
                CreateAsset<DeverQuestQuestProfile>(
                    DemoRoot +
                    "/Quests/Trouble_in_the_Tutorial_Crypt_Profile.asset",
                    report,
                    quest =>
                    {
                        quest.displayName =
                            "Trouble in the Tutorial Crypt";
                        quest.description =
                            "A complete DeverQuest systems walkthrough.";
                        quest.availableToMembers = true;
                        quest.minimumAdventurerLevel = 1;
                        quest.projectName = "DeverQuest Tutorial";
                        quest.taskName =
                            "Clear the Tutorial Crypt";
                        quest.department = "Programming";
                        quest.goalTemplate =
                            "Complete the preparation and implementation " +
                            "stages, record a Quest Log note, commit work, " +
                            "resolve the encounter, collect loot, and turn " +
                            "in the Chronicle.";
                        quest.suggestedFocusMinutes = 10;
                        quest.baseCopper = 10;
                        quest.baseExperience = 20;
                        quest.workBlockMinutes = 5;
                        quest.copperPerWorkBlock = 15;
                        quest.experiencePerWorkBlock = 25;
                    });
            report.TutorialContract =
                CreateAsset<DeverQuestQuestContract>(
                    DemoRoot +
                    "/Quests/Trouble_in_the_Tutorial_Crypt_Contract.asset",
                    report,
                    contract =>
                    {
                        contract.InitializeFromProfile(
                            questProfile,
                            "Tutorial Dungeon Master");
                        contract.contractTitle =
                            "Trouble in the Tutorial Crypt";
                        contract.status =
                            DeverQuestContractStatus.Offered;
                        contract.priority =
                            DeverQuestContractPriority.Normal;
                        contract.openToAnyMember = true;
                        contract.questStory =
                            "The Guild's tutorial crypt has begun emitting " +
                            "compiler warnings. Enter, document the fault, " +
                            "and defeat the skeletal regression.";
                        contract.deliverables =
                            "One Quest Log note, one Git commit or linked " +
                            "commit note, and a finalized Chronicle.";
                        contract.encounterProfileId =
                            encounter.EncounterId;
                        contract.encounterNotes =
                            "Resolve after the implementation stage.";
                        contract.focusStages.Add(
                            CreateStage(
                                "Prepare the Expedition",
                                "Review the Quest and plan the change.",
                                5,
                                5,
                                10,
                                null));
                        contract.focusStages.Add(
                            CreateStage(
                                "Confront the Regression",
                                "Implement and verify the tutorial change.",
                                5,
                                10,
                                20,
                                encounter));
                    });

            CreateAsset<DeverQuestPlaylist>(
                DemoRoot +
                "/Audio/Tutorial_Playlist.asset",
                report,
                playlist =>
                {
                    playlist.Shuffle = false;
                    playlist.RepeatMode =
                        DeverQuestRepeatMode.All;
                    playlist.Volume = 0.65f;
                });
            CreateAsset<DeverQuestAmbienceProfile>(
                DemoRoot +
                "/Audio/Tutorial_Crypt_Ambience.asset",
                report,
                ambience =>
                {
                    ambience.displayName =
                        "Tutorial Crypt Ambience";
                    ambience.description =
                        "Drop compatible AudioClips here.";
                    ambience.volume = 0.3f;
                    ambience.playDuringActiveQuest = true;
                });
            CreateAsset<DeverQuestWarningAudioProfile>(
                DemoRoot +
                "/Audio/Tutorial_Warnings.asset",
                report,
                warnings => warnings.volume = 0.8f);
            CreateAsset<DeverQuestExternalActivityProfile>(
                DemoRoot +
                "/Activity/Tutorial_Creative_Tools.asset",
                report,
                activity =>
                {
                    activity.displayName =
                        "Tutorial Creative Tools";
                    activity.providers.Add(
                        new DeverQuestExternalActivityProvider
                        {
                            displayName = "Aseprite",
                            processName = "aseprite",
                            inputFreshnessSeconds = 30
                        });
                });

            Finish(report);
            return report;
        }

        private static DeverQuestScaffoldReport
            CreateProductionStructureInternal()
        {
            DeverQuestScaffoldReport report =
                new DeverQuestScaffoldReport();
            EnsureFolderPath(Root, report);
            foreach (string folder in ProductionFolders)
            {
                EnsureFolderPath(
                    Root + "/" + folder,
                    report);
            }
            CreateBlankTemplates(report);
            return report;
        }

        private static void CreateBlankTemplates(
            DeverQuestScaffoldReport report)
        {
            CreateAsset<DeverQuestQuestProfile>(
                TemplatesRoot + "/QuestProfile_Template.asset",
                report);
            CreateAsset<DeverQuestQuestContract>(
                TemplatesRoot + "/QuestContract_Template.asset",
                report);
            CreateAsset<DeverQuestEncounterProfile>(
                TemplatesRoot + "/Encounter_Template.asset",
                report);
            CreateAsset<DeverQuestMonsterProfile>(
                TemplatesRoot + "/Monster_Template.asset",
                report);
            CreateAsset<DeverQuestEquipment>(
                TemplatesRoot + "/Equipment_Template.asset",
                report);
            CreateAsset<DeverQuestSpell>(
                TemplatesRoot + "/Spell_Template.asset",
                report);
            CreateAsset<DeverQuestAttackTechnique>(
                TemplatesRoot + "/AttackTechnique_Template.asset",
                report);
            CreateAsset<DeverQuestAbilityProfile>(
                TemplatesRoot + "/AbilityProfile_Template.asset",
                report);
            CreateAsset<DeverQuestCompanionProfile>(
                TemplatesRoot + "/CompanionProfile_Template.asset",
                report);
            CreateAsset<DeverQuestCompanionCatalog>(
                TemplatesRoot + "/CompanionCatalog_Template.asset",
                report);
            CreateAsset<DeverQuestCombatTypeCatalog>(
                TemplatesRoot + "/CombatTypeCatalog_Template.asset",
                report);
            CreateAsset<DeverQuestStarterLoadout>(
                TemplatesRoot + "/StarterLoadout_Template.asset",
                report);
            CreateAsset<DeverQuestAncestry>(
                TemplatesRoot + "/Ancestry_Template.asset",
                report);
            CreateAsset<DeverQuestClassDefinition>(
                TemplatesRoot + "/ClassDefinition_Template.asset",
                report);
            CreateAsset<DeverQuestDeity>(
                TemplatesRoot + "/Faith_Template.asset",
                report);
            CreateAsset<DeverQuestIdentityCatalog>(
                TemplatesRoot + "/IdentityCatalog_Template.asset",
                report);
            CreateAsset<DeverQuestShopItem>(
                TemplatesRoot + "/ShopItem_Template.asset",
                report);
            CreateAsset<DeverQuestShopProfile>(
                TemplatesRoot + "/ShopProfile_Template.asset",
                report);
            CreateAsset<DeverQuestPlaylist>(
                TemplatesRoot + "/Playlist_Template.asset",
                report);
            CreateAsset<DeverQuestWarningAudioProfile>(
                TemplatesRoot + "/WarningAudio_Template.asset",
                report);
            CreateAsset<DeverQuestAmbienceProfile>(
                TemplatesRoot + "/Ambience_Template.asset",
                report);
            CreateAsset<DeverQuestExternalActivityProfile>(
                TemplatesRoot +
                "/ExternalActivity_Template.asset",
                report);
        }

        private static DeverQuestShopItem CreateShopItem(
            string name,
            DeverQuestScaffoldReport report,
            Action<DeverQuestShopItem> initialize)
        {
            return CreateAsset(
                DemoRoot + "/Guild/" + name + ".asset",
                report,
                initialize);
        }

        private static DeverQuestFocusStage CreateStage(
            string title,
            string objective,
            int minutes,
            int copper,
            int experience,
            DeverQuestEncounterProfile encounter)
        {
            DeverQuestFocusStage stage =
                new DeverQuestFocusStage
                {
                    stageTitle = title,
                    workObjective = objective,
                    focusedMinutesRequired = minutes,
                    copperReward = copper,
                    experienceReward = experience,
                    encounterProfile = encounter
                };
            stage.Sanitize();
            return stage;
        }

        private static T CreateAsset<T>(
            string path,
            DeverQuestScaffoldReport report,
            Action<T> initialize = null)
            where T : ScriptableObject
        {
            T existing =
                AssetDatabase.LoadAssetAtPath<T>(path);
            if (existing != null)
            {
                report.ExistingItemsSkipped++;
                return existing;
            }
            T asset = ScriptableObject.CreateInstance<T>();
            initialize?.Invoke(asset);
            AssetDatabase.CreateAsset(asset, path);
            EditorUtility.SetDirty(asset);
            report.AssetsCreated++;
            return asset;
        }

        private static void EnsureFolderPath(
            string path,
            DeverQuestScaffoldReport report)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1;
                 index < parts.Length;
                 index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(
                        current,
                        parts[index]);
                    report.FoldersCreated++;
                }
                else
                {
                    report.ExistingItemsSkipped++;
                }
                current = next;
            }
        }

        private static void Finish(
            DeverQuestScaffoldReport report)
        {
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

}
