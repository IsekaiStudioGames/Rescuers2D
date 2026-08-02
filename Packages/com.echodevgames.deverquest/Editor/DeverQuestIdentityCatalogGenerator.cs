using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestIdentityGenerationReport
    {
        public int Created;
        public int Preserved;
        public string RootPath = string.Empty;
        public DeverQuestIdentityCatalog Catalog;
        public string Error = string.Empty;

        public bool Succeeded =>
            Catalog != null && string.IsNullOrWhiteSpace(Error);

        public string Summary =>
            Succeeded
                ? $"Identity catalog ready: {Created} created, " +
                  $"{Preserved} preserved."
                : "Identity catalog generation failed: " +
                  (string.IsNullOrWhiteSpace(Error)
                      ? "No catalog asset was produced."
                      : Error);
    }

    internal static class DeverQuestIdentityCatalogGenerator
    {
        private const string Root =
            "Assets/DeverQuest/IdentityCatalogs/OriginalStarter";

        public static DeverQuestIdentityGenerationReport
            GenerateOriginalStarterCatalog()
        {
            DeverQuestIdentityGenerationReport report =
                new DeverQuestIdentityGenerationReport
                {
                    RootPath = Root
                };
            EnsureFolder(Root + "/Ancestries");
            EnsureFolder(Root + "/Classes");
            EnsureFolder(Root + "/Faiths");

            List<DeverQuestAncestry> ancestries =
                new List<DeverQuestAncestry>
                {
                    Ancestry(
                        "Freefolk", report,
                        "Adaptable people found throughout the Guild realms.",
                        DeverQuestAbility.Charisma, 1,
                        DeverQuestAbility.Luck, 1,
                        "Adaptable", "Guild Tongue"),
                    Ancestry(
                        "Stonekin", report,
                        "Stout mountain folk shaped by craft and endurance.",
                        DeverQuestAbility.Constitution, 2,
                        DeverQuestAbility.Strength, 1,
                        "Stonecunning", "Deep Speech"),
                    Ancestry(
                        "Sylvan", report,
                        "Long-lived woodland scholars with precise senses.",
                        DeverQuestAbility.Dexterity, 2,
                        DeverQuestAbility.Wisdom, 1,
                        "Keen Senses", "Sylvan"),
                    Ancestry(
                        "Hearthling", report,
                        "Small, resilient travelers blessed with good fortune.",
                        DeverQuestAbility.Dexterity, 1,
                        DeverQuestAbility.Luck, 2,
                        "Brave", "Hearth Cant"),
                    Ancestry(
                        "Mirekin", report,
                        "Sapient amphibious folk from the singing marshes.",
                        DeverQuestAbility.Agility, 2,
                        DeverQuestAbility.Wisdom, 1,
                        "Amphibious", "Marshsong"),
                    Ancestry(
                        "Ashscale", report,
                        "Disciplined scaled folk of volcanic badlands.",
                        DeverQuestAbility.Stamina, 2,
                        DeverQuestAbility.Constitution, 1,
                        "Heatwise", "Ash Tongue"),
                    Ancestry(
                        "Moonclaw", report,
                        "Sapient feline folk guided by lunar traditions.",
                        DeverQuestAbility.Agility, 2,
                        DeverQuestAbility.Strength, 1,
                        "Night Eyes", "Pride Cant"),
                    Ancestry(
                        "High Scholar", report,
                        "Tower-raised researchers devoted to reason and magic.",
                        DeverQuestAbility.Intelligence, 2,
                        DeverQuestAbility.Wisdom, 1,
                        "Arcane Education", "Scholarly"),
                    Ancestry(
                        "Northlander", report,
                        "Hardy coastal clans renowned for courage.",
                        DeverQuestAbility.Strength, 2,
                        DeverQuestAbility.Stamina, 1,
                        "Coldwise", "Northern")
                };

            activeReport = report;
            List<DeverQuestClassDefinition> classes =
                new List<DeverQuestClassDefinition>
                {
                    Class("Warrior", "Quality Assurance",
                        DeverQuestAbility.Strength, 10, false,
                        false, 16, 12, 16, 8, 10, 10,
                        "Strength", "Constitution"),
                    Class("Paladin", "Design",
                        DeverQuestAbility.Charisma, 10, true,
                        false, 16, 10, 14, 8, 10, 14,
                        "Wisdom", "Charisma"),
                    Class("Ranger", "Art",
                        DeverQuestAbility.Dexterity, 8, true,
                        true, 10, 16, 14, 12, 14, 8,
                        "Strength", "Dexterity"),
                    Class("Rogue", "Art",
                        DeverQuestAbility.Dexterity, 8, false,
                        false, 10, 16, 14, 12, 14, 8,
                        "Dexterity", "Intelligence"),
                    Class("Cleric", "Design",
                        DeverQuestAbility.Wisdom, 8, true,
                        false, 10, 12, 14, 8, 16, 10,
                        "Wisdom", "Charisma"),
                    Class("Druid", "Audio",
                        DeverQuestAbility.Wisdom, 8, true,
                        true, 10, 12, 14, 8, 16, 10,
                        "Intelligence", "Wisdom"),
                    Class("Wizard", "Programming",
                        DeverQuestAbility.Intelligence, 6, true,
                        false, 8, 14, 14, 16, 12, 10,
                        "Intelligence", "Wisdom"),
                    Class("Sorcerer", "Programming",
                        DeverQuestAbility.Charisma, 6, true,
                        false, 8, 14, 14, 10, 12, 16,
                        "Constitution", "Charisma"),
                    Class("Necromancer", "Programming",
                        DeverQuestAbility.Intelligence, 6, true,
                        true, 8, 14, 14, 16, 12, 10,
                        "Intelligence", "Wisdom"),
                    Class("Monk", "Quality Assurance",
                        DeverQuestAbility.Wisdom, 8, false,
                        false, 12, 16, 14, 8, 16, 8,
                        "Strength", "Dexterity"),
                    Class("Bard", "Art",
                        DeverQuestAbility.Charisma, 8, true,
                        false, 8, 14, 14, 10, 12, 16,
                        "Dexterity", "Charisma"),
                    Class("Shaman", "Audio",
                        DeverQuestAbility.Wisdom, 8, true,
                        true, 10, 12, 14, 10, 16, 10,
                        "Wisdom", "Constitution"),
                    Class("Berserker", "Quality Assurance",
                        DeverQuestAbility.Strength, 12, false,
                        false, 17, 12, 16, 8, 9, 8,
                        "Strength", "Constitution"),
                    Class("Barbarian", "Quality Assurance",
                        DeverQuestAbility.Strength, 12, false,
                        false, 17, 12, 16, 8, 10, 8,
                        "Strength", "Constitution"),
                    Class("Wildwarden", "Art",
                        DeverQuestAbility.Wisdom, 10, true,
                        true, 14, 14, 15, 8, 15, 8,
                        "Strength", "Wisdom")
                };
            activeReport = null;
            SetCompanionTradition(
                classes, "Necromancer", "Bound Minion");
            SetCompanionTradition(
                classes, "Wildwarden", "Bonded Beast");
            SetCompanionTradition(
                classes, "Ranger", "Animal Companion");
            SetCompanionTradition(
                classes, "Druid", "Primal Familiar");
            SetCompanionTradition(
                classes, "Shaman", "Spirit Companion");
            foreach (DeverQuestClassDefinition value in classes)
            {
                if (value == null ||
                    (value.classFeatures?.Count ?? 0) > 0)
                {
                    continue;
                }
                value.classFeatures =
                    new List<string>
                    {
                        value.displayName + " Training"
                    };
                if (value.usesMana)
                {
                    value.classFeatures.Add("Spellcraft");
                }
                if (value.supportsCompanion)
                {
                    value.classFeatures.Add("Companion Affinity");
                }
                EditorUtility.SetDirty(value);
            }

            List<DeverQuestDeity> faiths =
                new List<DeverQuestDeity>
                {
                    Faith(
                        "Agnostic", report,
                        "No declared patron; conscience and Guild oath guide " +
                        "the Adventurer.",
                        DeverQuestAlignment.TrueNeutral,
                        "Freedom"),
                    Faith(
                        "The Lantern", report,
                        "A guardian ideal of truth, refuge, and duty.",
                        DeverQuestAlignment.LawfulGood,
                        "Light", "Protection", "Knowledge"),
                    Faith(
                        "The Verdant Chorus", report,
                        "A living harmony of growth, seasons, and renewal.",
                        DeverQuestAlignment.NeutralGood,
                        "Nature", "Life", "Storm"),
                    Faith(
                        "The Keeper Below", report,
                        "A solemn warden of memory, endings, and ancestors.",
                        DeverQuestAlignment.LawfulNeutral,
                        "Grave", "Knowledge", "Order"),
                    Faith(
                        "The Unbound Star", report,
                        "A wandering light of invention and self-determination.",
                        DeverQuestAlignment.ChaoticGood,
                        "Arcana", "Travel", "Freedom")
                };

            report.Catalog = GetOrCreate<DeverQuestIdentityCatalog>(
                Root + "/Original_Guild_Identity_Catalog.asset",
                report,
                catalog =>
                {
                    catalog.displayName =
                        "Original Guild Identity Catalog";
                });
            if (report.Catalog == null)
            {
                report.Error =
                    "Unity did not create or load the catalog asset.";
                return report;
            }

            report.Catalog.ancestries =
                report.Catalog.ancestries ??
                new List<DeverQuestAncestry>();
            report.Catalog.classes =
                report.Catalog.classes ??
                new List<DeverQuestClassDefinition>();
            report.Catalog.faiths =
                report.Catalog.faiths ??
                new List<DeverQuestDeity>();
            AddUnique(report.Catalog.ancestries, ancestries);
            AddUnique(report.Catalog.classes, classes);
            AddUnique(report.Catalog.faiths, faiths);
            if (report.Catalog.defaultAncestry == null)
            {
                report.Catalog.defaultAncestry = ancestries[0];
            }
            if (report.Catalog.defaultClass == null)
            {
                report.Catalog.defaultClass =
                    classes.Find(value =>
                        value.displayName == "Warrior");
            }
            if (report.Catalog.defaultFaith == null)
            {
                report.Catalog.defaultFaith = faiths[0];
            }
            EditorUtility.SetDirty(report.Catalog);
            AssetDatabase.SaveAssets();
            DeverQuestIdentityCatalogService.Clear();
            DeverQuestIdentityCatalogService.SetActiveCatalog(
                report.Catalog);
            DeverQuestGuildAccountService.MigrateIdentityCatalogs();
            return report;
        }

        private static DeverQuestAncestry Ancestry(
            string name,
            DeverQuestIdentityGenerationReport report,
            string lore,
            DeverQuestAbility first,
            int firstAmount,
            DeverQuestAbility second,
            int secondAmount,
            string trait,
            string language)
        {
            return GetOrCreate<DeverQuestAncestry>(
                Root + "/Ancestries/" + Safe(name) + ".asset",
                report,
                ancestry =>
                {
                    ancestry.displayName = name;
                    ancestry.lore = lore;
                    ancestry.playable = true;
                    ancestry.sapient = true;
                    SetAbilityAdjustment(
                        ancestry.abilityAdjustments,
                        first,
                        firstAmount);
                    SetAbilityAdjustment(
                        ancestry.abilityAdjustments,
                        second,
                        secondAmount);
                    AddUniqueString(
                        ancestry.innateTraits,
                        trait);
                    AddUniqueString(
                        ancestry.languages,
                        "Guild Tongue");
                    AddUniqueString(
                        ancestry.languages,
                        language);
                });
        }

        private static DeverQuestClassDefinition Class(
            string name,
            string department,
            DeverQuestAbility primary,
            int hitDie,
            bool mana,
            bool companion,
            int strength,
            int dexterity,
            int constitution,
            int intelligence,
            int wisdom,
            int charisma,
            params string[] saves)
        {
            DeverQuestIdentityGenerationReport report =
                activeReport;
            return GetOrCreate<DeverQuestClassDefinition>(
                Root + "/Classes/" + Safe(name) + ".asset",
                report,
                value =>
                {
                    value.displayName = name;
                    value.lore =
                        $"An original DeverQuest {name} tradition.";
                    value.department = department;
                    value.primaryAbility = primary;
                    value.hitDie = hitDie;
                    value.usesMana = mana;
                    value.supportsCompanion = companion;
                    value.strength = strength;
                    value.dexterity = dexterity;
                    value.constitution = constitution;
                    value.intelligence = intelligence;
                    value.wisdom = wisdom;
                    value.charisma = charisma;
                    AddUniqueStrings(
                        value.proficientSaves,
                        saves);
                });
        }

        private static DeverQuestIdentityGenerationReport activeReport;

        private static DeverQuestDeity Faith(
            string name,
            DeverQuestIdentityGenerationReport report,
            string lore,
            DeverQuestAlignment alignment,
            params string[] domains)
        {
            return GetOrCreate<DeverQuestDeity>(
                Root + "/Faiths/" + Safe(name) + ".asset",
                report,
                faith =>
                {
                    faith.displayName = name;
                    faith.lore = lore;
                    faith.alignment = alignment;
                    AddUniqueStrings(faith.domains, domains);
                    faith.grantedTrait =
                        domains.Length == 0
                            ? string.Empty
                            : domains[0] + " Devotion";
                });
        }

        private static T GetOrCreate<T>(
            string path,
            DeverQuestIdentityGenerationReport report,
            Action<T> initialize)
            where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            bool existed = asset != null;
            if (!existed)
            {
                asset = ScriptableObject.CreateInstance<T>();
            }

            EnsureGeneratedAssetCollections(asset);
            initialize(asset);

            if (!existed)
            {
                AssetDatabase.CreateAsset(asset, path);
            }
            EditorUtility.SetDirty(asset);
            if (report != null)
            {
                if (existed)
                {
                    report.Preserved++;
                }
                else
                {
                    report.Created++;
                }
            }
            return asset;
        }

        private static void SetAbilityAdjustment(
            List<DeverQuestAbilityAdjustment> adjustments,
            DeverQuestAbility ability,
            int amount)
        {
            DeverQuestAbilityAdjustment first = null;
            for (int index = adjustments.Count - 1; index >= 0; index--)
            {
                DeverQuestAbilityAdjustment adjustment =
                    adjustments[index];
                if (adjustment == null)
                {
                    adjustments.RemoveAt(index);
                    continue;
                }
                if (adjustment.ability != ability)
                {
                    continue;
                }
                if (first == null)
                {
                    first = adjustment;
                    first.amount = amount;
                }
                else
                {
                    adjustments.RemoveAt(index);
                }
            }
            if (first == null)
            {
                adjustments.Add(
                    new DeverQuestAbilityAdjustment
                    {
                        ability = ability,
                        amount = amount
                    });
            }
        }

        private static void AddUniqueStrings(
            List<string> target,
            IEnumerable<string> values)
        {
            foreach (string value in values ??
                     Enumerable.Empty<string>())
            {
                AddUniqueString(target, value);
            }
        }

        private static void AddUniqueString(
            List<string> target,
            string value)
        {
            if (target == null || string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            if (!target.Any(existing =>
                    string.Equals(
                        existing,
                        value,
                        StringComparison.OrdinalIgnoreCase)))
            {
                target.Add(value);
            }
        }

        private static void EnsureGeneratedAssetCollections(
            ScriptableObject asset)
        {
            DeverQuestAncestry ancestry =
                asset as DeverQuestAncestry;
            if (ancestry != null)
            {
                ancestry.abilityAdjustments =
                    ancestry.abilityAdjustments ??
                    new List<DeverQuestAbilityAdjustment>();
                ancestry.languages =
                    ancestry.languages ?? new List<string>();
                ancestry.innateTraits =
                    ancestry.innateTraits ?? new List<string>();
                ancestry.damageAffinities =
                    ancestry.damageAffinities ??
                    new List<DeverQuestDamageAffinity>();
                ancestry.eligibleClassIds =
                    ancestry.eligibleClassIds ?? new List<string>();
                ancestry.restrictedClassIds =
                    ancestry.restrictedClassIds ?? new List<string>();
                return;
            }

            DeverQuestClassDefinition classDefinition =
                asset as DeverQuestClassDefinition;
            if (classDefinition != null)
            {
                classDefinition.proficientSaves =
                    classDefinition.proficientSaves ??
                    new List<string>();
                classDefinition.classFeatures =
                    classDefinition.classFeatures ??
                    new List<string>();
                return;
            }

            DeverQuestDeity deity = asset as DeverQuestDeity;
            if (deity != null)
            {
                deity.allowedAlignments =
                    deity.allowedAlignments ??
                    new List<DeverQuestAlignment>();
                deity.domains =
                    deity.domains ?? new List<string>();
                deity.favoredClassIds =
                    deity.favoredClassIds ?? new List<string>();
                deity.restrictedAncestryIds =
                    deity.restrictedAncestryIds ??
                    new List<string>();
                return;
            }

            DeverQuestIdentityCatalog catalog =
                asset as DeverQuestIdentityCatalog;
            if (catalog != null)
            {
                catalog.ancestries =
                    catalog.ancestries ??
                    new List<DeverQuestAncestry>();
                catalog.classes =
                    catalog.classes ??
                    new List<DeverQuestClassDefinition>();
                catalog.faiths =
                    catalog.faiths ??
                    new List<DeverQuestDeity>();
            }
        }

        private static void AddUnique<T>(
            List<T> target,
            IEnumerable<T> values)
            where T : UnityEngine.Object
        {
            foreach (T value in values)
            {
                if (value != null && !target.Contains(value))
                {
                    target.Add(value);
                }
            }
        }

        private static void SetCompanionTradition(
            IEnumerable<DeverQuestClassDefinition> classes,
            string className,
            string tradition)
        {
            DeverQuestClassDefinition value =
                classes.FirstOrDefault(item =>
                    item != null &&
                    item.displayName == className);
            if (value == null)
            {
                return;
            }
            value.companionTradition = tradition;
            EditorUtility.SetDirty(value);
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
                        current,
                        parts[index]);
                }
                current = next;
            }
        }

        private static string Safe(string value)
        {
            return value.Replace(" ", "_");
        }
    }
}
