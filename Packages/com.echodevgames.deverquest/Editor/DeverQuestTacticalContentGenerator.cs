using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestTacticalContentReport
    {
        public string RootPath = string.Empty;
        public int Created;
        public int Updated;
    }

    internal static class DeverQuestTacticalContentGenerator
    {
        private const string Root =
            "Assets/DeverQuest/Tactical";

        [MenuItem(
            "Tools/DeverQuest/Content/Generate Tactical Starter Kit")]
        public static DeverQuestTacticalContentReport
            GenerateStarterKit()
        {
            EnsureFolder(Root);
            EnsureFolder(Root + "/Abilities");
            EnsureFolder(Root + "/Profiles");
            EnsureFolder(Root + "/Encounters");
            EnsureFolder(Root + "/Quests");
            DeverQuestTacticalContentReport report =
                new DeverQuestTacticalContentReport
                {
                    RootPath = Root
                };

            DeverQuestSpell ember = Upsert<DeverQuestSpell>(
                Root + "/Abilities/EmberLance.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Ember Lance";
                    spell.description =
                        "A precise lance of guild-forged flame.";
                    spell.manaCost = 2;
                    spell.damageDice = "1d8";
                    spell.damageType = DeverQuestDamageType.Fire;
                    spell.effects = Effects(
                        Effect(
                            DeverQuestCombatEffectType.DirectDamage,
                            DeverQuestCombatTarget.Enemy,
                            "1d8",
                            DeverQuestDamageType.Fire));
                });
            DeverQuestSpell mend = Upsert<DeverQuestSpell>(
                Root + "/Abilities/MendingLight.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Mending Light";
                    spell.description =
                        "Restores an ally before the next enemy turn.";
                    spell.manaCost = 3;
                    spell.cooldownRounds = 2;
                    spell.damageDice = string.Empty;
                    spell.target = DeverQuestCombatTarget.Self;
                    spell.effects = Effects(
                        Effect(
                            DeverQuestCombatEffectType.Heal,
                            DeverQuestCombatTarget.Self,
                            "1d8+2"));
                });
            DeverQuestSpell wither = Upsert<DeverQuestSpell>(
                Root + "/Abilities/WitheringBrand.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Withering Brand";
                    spell.description =
                        "A lingering mark that deals shadow damage.";
                    spell.manaCost = 3;
                    spell.cooldownRounds = 3;
                    spell.damageDice = string.Empty;
                    DeverQuestCombatEffect effect = Effect(
                        DeverQuestCombatEffectType.DamageOverTime,
                        DeverQuestCombatTarget.Enemy,
                        "1d4",
                        DeverQuestDamageType.Shadow);
                    effect.durationRounds = 3;
                    spell.effects = Effects(effect);
                });
            DeverQuestSpell mire = Upsert<DeverQuestSpell>(
                Root + "/Abilities/BindingMire.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Binding Mire";
                    spell.description =
                        "Roots a foe, then leaves it snared.";
                    spell.manaCost = 2;
                    spell.cooldownRounds = 3;
                    spell.damageDice = string.Empty;
                    DeverQuestCombatEffect root = Effect(
                        DeverQuestCombatEffectType.Root,
                        DeverQuestCombatTarget.Enemy,
                        string.Empty);
                    root.durationRounds = 2;
                    DeverQuestCombatEffect snare = Effect(
                        DeverQuestCombatEffectType.Snare,
                        DeverQuestCombatTarget.Enemy,
                        string.Empty);
                    snare.durationRounds = 3;
                    spell.effects = Effects(root, snare);
                });
            DeverQuestSpell siphon = Upsert<DeverQuestSpell>(
                Root + "/Abilities/GraveSiphon.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Grave Siphon";
                    spell.description =
                        "Deals shadow damage and restores equal health.";
                    spell.manaCost = 4;
                    spell.damageDice = string.Empty;
                    spell.effects = Effects(
                        Effect(
                            DeverQuestCombatEffectType.LifeDrain,
                            DeverQuestCombatTarget.Enemy,
                            "1d6+1",
                            DeverQuestDamageType.Shadow));
                });
            DeverQuestSpell homeward = Upsert<DeverQuestSpell>(
                Root + "/Abilities/HomewardSigil.asset",
                ref report,
                spell =>
                {
                    spell.displayName = "Homeward Sigil";
                    spell.description =
                        "Safely returns an expedition to the Guild Hall.";
                    spell.manaCost = 5;
                    spell.damageDice = string.Empty;
                    spell.target = DeverQuestCombatTarget.Self;
                    spell.effects = Effects(
                        Effect(
                            DeverQuestCombatEffectType.ReturnToGuild,
                            DeverQuestCombatTarget.Self,
                            string.Empty));
                });

            DeverQuestAttackTechnique strike = Upsert<DeverQuestAttackTechnique>(
                Root + "/Abilities/GuildSteelArc.asset",
                ref report,
                technique =>
                {
                    technique.displayName = "Guildsteel Arc";
                    technique.description =
                        "A dependable martial sweep.";
                    technique.attackAbility =
                        DeverQuestAbility.Strength;
                    technique.effects = Effects(
                        Effect(
                            DeverQuestCombatEffectType.DirectDamage,
                            DeverQuestCombatTarget.Enemy,
                            "1d8",
                            DeverQuestDamageType.Slashing));
                });
            DeverQuestAttackTechnique hamstring = Upsert<DeverQuestAttackTechnique>(
                Root + "/Abilities/HamperingCut.asset",
                ref report,
                technique =>
                {
                    technique.displayName = "Hampering Cut";
                    technique.description =
                        "A measured cut that slows pursuit.";
                    technique.attackAbility =
                        DeverQuestAbility.Dexterity;
                    DeverQuestCombatEffect damage = Effect(
                        DeverQuestCombatEffectType.DirectDamage,
                        DeverQuestCombatTarget.Enemy,
                        "1d6",
                        DeverQuestDamageType.Slashing);
                    DeverQuestCombatEffect snare = Effect(
                        DeverQuestCombatEffectType.Snare,
                        DeverQuestCombatTarget.Enemy,
                        string.Empty);
                    snare.durationRounds = 2;
                    technique.effects = Effects(damage, snare);
                });
            DeverQuestAttackTechnique guard = Upsert<DeverQuestAttackTechnique>(
                Root + "/Abilities/MeasuredGuard.asset",
                ref report,
                technique =>
                {
                    technique.displayName = "Measured Guard";
                    technique.description =
                        "Raises a short-lived shield against harm.";
                    technique.cooldownRounds = 2;
                    technique.effects = Effects(
                        new DeverQuestCombatEffect
                        {
                            effectType =
                                DeverQuestCombatEffectType.Shield,
                            target = DeverQuestCombatTarget.Self,
                            flatAmount = 5,
                            durationRounds = 2
                        });
                });
            DeverQuestAttackTechnique venom = Upsert<DeverQuestAttackTechnique>(
                Root + "/Abilities/VenomousBite.asset",
                ref report,
                technique =>
                {
                    technique.displayName = "Venomous Bite";
                    technique.description =
                        "A creature attack that leaves ongoing venom.";
                    DeverQuestCombatEffect effect = Effect(
                        DeverQuestCombatEffectType.DamageOverTime,
                        DeverQuestCombatTarget.Enemy,
                        "1d3",
                        DeverQuestDamageType.Poison);
                    effect.durationRounds = 3;
                    technique.effects = Effects(effect);
                });

            DeverQuestAbilityProfile caster = Upsert<DeverQuestAbilityProfile>(
                Root + "/Profiles/WayfarerCaster.asset",
                ref report,
                profile =>
                {
                    profile.displayName = "Wayfarer Spellcraft";
                    profile.tacticalStyle =
                        DeverQuestTacticalStyle.Controller;
                    profile.abilities = new List<
                        DeverQuestAbilitySlot>
                    {
                        Slot(mend, 95, 45),
                        Slot(wither, 85, 100, true),
                        Slot(mire, 75, 100, true),
                        Slot(siphon, 70, 65),
                        Slot(ember, 60),
                        Slot(homeward, 1)
                    };
                });
            DeverQuestAbilityProfile martial = Upsert<DeverQuestAbilityProfile>(
                Root + "/Profiles/GuildVanguard.asset",
                ref report,
                profile =>
                {
                    profile.displayName = "Guild Vanguard";
                    profile.tacticalStyle =
                        DeverQuestTacticalStyle.Balanced;
                    profile.abilities = new List<
                        DeverQuestAbilitySlot>
                    {
                        Slot(guard, 90, 35),
                        Slot(hamstring, 70, 100, true),
                        Slot(strike, 60)
                    };
                });
            DeverQuestAbilityProfile monsterActions = Upsert<DeverQuestAbilityProfile>(
                Root + "/Profiles/BogStalkerActions.asset",
                ref report,
                profile =>
                {
                    profile.displayName = "Bog Stalker Actions";
                    profile.tacticalStyle =
                        DeverQuestTacticalStyle.Aggressive;
                    profile.abilities = new List<
                        DeverQuestAbilitySlot>
                    {
                        Slot(venom, 80)
                    };
                });

            foreach (string guid in AssetDatabase.FindAssets(
                         "t:DeverQuestClassDefinition"))
            {
                DeverQuestClassDefinition classDefinition =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestClassDefinition>(
                        AssetDatabase.GUIDToAssetPath(guid));
                if (classDefinition == null)
                {
                    continue;
                }
                DeverQuestAbilityProfile baseline =
                    classDefinition.usesMana ? caster : martial;
                string className =
                    string.IsNullOrWhiteSpace(
                        classDefinition.displayName)
                        ? classDefinition.name
                        : classDefinition.displayName;
                DeverQuestAbilityProfile classProfile = Upsert<DeverQuestAbilityProfile>(
                    Root + "/Profiles/" +
                    SafeFileName(className) +
                    "Abilities.asset",
                    ref report,
                    profile =>
                    {
                        profile.displayName =
                            className + " Tactical Codex";
                        profile.description =
                            "An editable class-specific copy of the " +
                            "original DeverQuest starter tactics.";
                        profile.tacticalStyle =
                            baseline.tacticalStyle;
                        profile.abilities =
                            baseline.abilities.ConvertAll(slot =>
                                new DeverQuestAbilitySlot
                                {
                                    spell = slot.spell,
                                    technique = slot.technique,
                                    priority = slot.priority,
                                    useBelowHitPointPercent =
                                        slot.useBelowHitPointPercent,
                                    maintainEffect =
                                        slot.maintainEffect
                                });
                    });
                classDefinition.abilityProfile = classProfile;
                EditorUtility.SetDirty(classDefinition);
                report.Updated++;
            }
            DeverQuestAdventurer current =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestClassDefinition currentClass =
                DeverQuestIdentityCatalogService.FindClass(
                    current.classId,
                    current.characterClass);
            if (currentClass != null && currentClass.usesMana)
            {
                foreach (DeverQuestSpell spell in
                         new[]
                         {
                             ember, mend, wither, mire, siphon, homeward
                         })
                {
                    if (!current.knownSpellIds.Contains(spell.SpellId))
                    {
                        current.knownSpellIds.Add(spell.SpellId);
                    }
                }
                DeverQuestAdventurerService.Save();
            }

            DeverQuestShopItem bogScrap = Upsert<DeverQuestShopItem>(
                Root + "/Encounters/BogStalkerScrap.asset",
                ref report,
                item =>
                {
                    item.displayName = "Bog-Stalker Scrap";
                    item.description =
                        "Heavy salvage that can fill an expedition pack.";
                    item.itemType =
                        DeverQuestShopItemType.Equipment;
                    item.itemCategory =
                        DeverQuestItemCategory.MerchantTrash;
                    item.subcategory = "Salvage";
                    item.tags = new List<string>
                    {
                        "Monster Drop",
                        "Marsh",
                        "Salvage"
                    };
                    item.copperCost = 2;
                    item.merchantSellValueCopper = 2;
                    item.unitWeight = 4f;
                    item.maximumOwned = 99;
                    item.maximumStackSize = 99;
                    item.autoEquipOnAcquire = false;
                    item.tradable = true;
                });
            DeverQuestMonsterProfile scout = Upsert<DeverQuestMonsterProfile>(
                Root + "/Encounters/BogStalker.asset",
                ref report,
                monster =>
                {
                    monster.displayName = "Bog Stalker";
                    monster.description =
                        "An original marsh predator used for starter " +
                        "tactical encounters.";
                    monster.level = 1;
                    monster.maximumHitPoints = 9;
                    monster.armorClass = 11;
                    monster.attackModifier = 2;
                    monster.damageDice = "1d4";
                    monster.attackDamageType =
                        DeverQuestDamageType.Poison;
                    monster.victoryCopper = 8;
                    monster.victoryExperience = 12;
                    monster.abilityProfile = monsterActions;
                    monster.dropTable =
                        new List<DeverQuestDropEntry>
                        {
                            new DeverQuestDropEntry
                            {
                                displayName =
                                    "Bog-Stalker Scrap",
                                dropChancePercent = 100,
                                shopItem = bogScrap
                            }
                        };
                });
            DeverQuestEncounterProfile skirmish = Upsert<DeverQuestEncounterProfile>(
                Root + "/Encounters/FifteenMinuteSkirmish.asset",
                ref report,
                encounter =>
                {
                    encounter.displayName =
                        "Fifteen-Minute Border Skirmish";
                    encounter.storyIntroduction =
                        "Hold the old road while completing one focused " +
                        "development objective.";
                    encounter.encounterMode =
                        DeverQuestEncounterMode.Fixed;
                    encounter.parRounds = 5;
                    encounter.earlyVictoryCopperBonus = 10;
                    encounter.earlyVictoryExperienceBonus = 15;
                    encounter.victoryCopperBonus = 10;
                    encounter.victoryExperienceBonus = 20;
                    encounter.waves =
                        new List<DeverQuestEncounterWave>
                        {
                            new DeverQuestEncounterWave
                            {
                                waveTitle = "Roadside Ambush",
                                monster = scout,
                                count = 1
                            }
                        };
                });
            DeverQuestEncounterProfile survival = Upsert<DeverQuestEncounterProfile>(
                Root + "/Encounters/WayfarerSurvival.asset",
                ref report,
                encounter =>
                {
                    encounter.displayName =
                        "Wayfarer Survival Expedition";
                    encounter.storyIntroduction =
                        "Push deeper for growing spoils, then escape, " +
                        "invoke a homeward passage, or take the wagon.";
                    encounter.encounterMode =
                        DeverQuestEncounterMode.Survival;
                    encounter.survivalWaveMinutes = 15;
                    encounter.difficultyIncreaseEveryWaves = 2;
                    encounter.wagonOfferEveryWaves = 3;
                    encounter.survivalCopperGrowthPerWave = 3;
                    encounter.survivalExperienceGrowthPerWave = 6;
                    encounter.lowHitPointPausePercent = 25;
                    encounter.pauseWhenEncumbered = true;
                    encounter.parRounds = 6;
                    encounter.earlyVictoryCopperBonus = 5;
                    encounter.earlyVictoryExperienceBonus = 8;
                    encounter.waves =
                        new List<DeverQuestEncounterWave>
                        {
                            new DeverQuestEncounterWave
                            {
                                waveTitle = "Bog Trail",
                                monster = scout,
                                count = 1
                            },
                            new DeverQuestEncounterWave
                            {
                                waveTitle = "Deep Marsh",
                                monster = scout,
                                count = 2
                            }
                        };
                });

            Upsert<DeverQuestQuestContract>(
                Root + "/Quests/FifteenMinuteSkirmishQuest.asset",
                ref report,
                (DeverQuestQuestContract contract) =>
                {
                    contract.contractTitle =
                        "Fifteen-Minute Skirmish";
                    contract.status =
                        DeverQuestContractStatus.Offered;
                    contract.openToAnyMember = true;
                    contract.projectName = "Current Project";
                    contract.taskName =
                        "Complete one focused development objective";
                    contract.objective =
                        "Finish the assigned development task while " +
                        "the Adventurer resolves the staged encounter.";
                    contract.focusStages =
                        new List<DeverQuestFocusStage>
                        {
                            new DeverQuestFocusStage
                            {
                                stageTitle = "Hold the Old Road",
                                workObjective =
                                    "Complete the assigned task.",
                                focusedMinutesRequired = 15,
                                copperReward = 10,
                                experienceReward = 20,
                                allowEarlyTurnIn = true,
                                earlyCompletionCopperBonus = 10,
                                earlyCompletionExperienceBonus = 20,
                                encounterProfile = skirmish,
                                encounterProfileId =
                                    skirmish.EncounterId
                            }
                        };
                });
            Upsert<DeverQuestQuestContract>(
                Root + "/Quests/WayfarerSurvivalQuest.asset",
                ref report,
                (DeverQuestQuestContract contract) =>
                {
                    contract.contractTitle =
                        "Wayfarer Survival Expedition";
                    contract.status =
                        DeverQuestContractStatus.Offered;
                    contract.openToAnyMember = true;
                    contract.projectName = "Current Project";
                    contract.taskName =
                        "Survive repeating fifteen-minute Focus waves";
                    contract.objective =
                        "Work for as long as the Adventurer can safely " +
                        "survive and carry the earned spoils.";
                    contract.focusStages =
                        new List<DeverQuestFocusStage>
                        {
                            new DeverQuestFocusStage
                            {
                                stageTitle = "Push Beyond the Map",
                                workObjective =
                                    "Continue development through " +
                                    "repeating survival waves.",
                                focusedMinutesRequired = 15,
                                copperReward = 20,
                                experienceReward = 30,
                                allowEarlyTurnIn = false,
                                encounterProfile = survival,
                                encounterProfileId =
                                    survival.EncounterId
                            }
                        };
                });

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return report;
        }

        private static DeverQuestCombatEffect Effect(
            DeverQuestCombatEffectType type,
            DeverQuestCombatTarget target,
            string dice,
            DeverQuestDamageType damageType =
                DeverQuestDamageType.Arcane)
        {
            return new DeverQuestCombatEffect
            {
                effectType = type,
                target = target,
                dice = dice,
                damageType = damageType,
                durationRounds = 1
            };
        }

        private static List<DeverQuestCombatEffect> Effects(
            params DeverQuestCombatEffect[] values)
        {
            return new List<DeverQuestCombatEffect>(values);
        }

        private static DeverQuestAbilitySlot Slot(
            DeverQuestSpell spell,
            int priority,
            int threshold = 100,
            bool maintain = false)
        {
            return new DeverQuestAbilitySlot
            {
                spell = spell,
                priority = priority,
                useBelowHitPointPercent = threshold,
                maintainEffect = maintain
            };
        }

        private static DeverQuestAbilitySlot Slot(
            DeverQuestAttackTechnique technique,
            int priority,
            int threshold = 100,
            bool maintain = false)
        {
            return new DeverQuestAbilitySlot
            {
                technique = technique,
                priority = priority,
                useBelowHitPointPercent = threshold,
                maintainEffect = maintain
            };
        }

        private static T Upsert<T>(
            string path,
            ref DeverQuestTacticalContentReport report,
            System.Action<T> configure)
            where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();
                AssetDatabase.CreateAsset(asset, path);
                report.Created++;
            }
            else
            {
                report.Updated++;
            }
            configure(asset);
            EditorUtility.SetDirty(asset);
            return asset;
        }

        private static void EnsureFolder(string path)
        {
            string[] parts = path.Split('/');
            string current = parts[0];
            for (int index = 1; index < parts.Length; index++)
            {
                string next = current + "/" + parts[index];
                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(
                        current, parts[index]);
                }
                current = next;
            }
        }

        private static string SafeFileName(string value)
        {
            char[] invalid =
                System.IO.Path.GetInvalidFileNameChars();
            foreach (char character in invalid)
            {
                value = value.Replace(character, '_');
            }
            return value.Replace(' ', '_');
        }
    }
}
