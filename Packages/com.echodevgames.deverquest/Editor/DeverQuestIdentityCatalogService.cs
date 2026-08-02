using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestIdentityCatalogService
    {
        private const string RegistryPath =
            "Assets/DeverQuest/IdentityCatalogs/" +
            "GuildIdentityRegistry.asset";
        private static List<DeverQuestAncestry> ancestries;
        private static List<DeverQuestClassDefinition> classes;
        private static List<DeverQuestDeity> faiths;
        private static List<DeverQuestIdentityCatalog> catalogs;
        private static DeverQuestIdentityCatalogRegistry registry;
        private static DeverQuestIdentityCatalog defaultCatalog;

        static DeverQuestIdentityCatalogService()
        {
            EditorApplication.projectChanged -= Clear;
            EditorApplication.projectChanged += Clear;
        }

        public static IReadOnlyList<DeverQuestAncestry> Ancestries
        {
            get
            {
                Ensure();
                return defaultCatalog != null &&
                       (defaultCatalog.ancestries?.Count ?? 0) > 0
                    ? defaultCatalog.ancestries
                    : ancestries;
            }
        }

        public static IReadOnlyList<DeverQuestClassDefinition> Classes
        {
            get
            {
                Ensure();
                return defaultCatalog != null &&
                       (defaultCatalog.classes?.Count ?? 0) > 0
                    ? defaultCatalog.classes
                    : classes;
            }
        }

        public static IReadOnlyList<DeverQuestDeity> Faiths
        {
            get
            {
                Ensure();
                return defaultCatalog != null &&
                       (defaultCatalog.faiths?.Count ?? 0) > 0
                    ? defaultCatalog.faiths
                    : faiths;
            }
        }

        public static IReadOnlyList<DeverQuestIdentityCatalog> Catalogs
        {
            get
            {
                Ensure();
                return catalogs;
            }
        }

        public static DeverQuestIdentityCatalog ActiveCatalog
        {
            get
            {
                Ensure();
                return defaultCatalog;
            }
        }

        public static void SetActiveCatalog(
            DeverQuestIdentityCatalog catalog)
        {
            if (catalog == null)
            {
                defaultCatalog = null;
                if (registry != null)
                {
                    registry.activeCatalog = null;
                    EditorUtility.SetDirty(registry);
                    AssetDatabase.SaveAssets();
                }
                Ensure();
                return;
            }
            EnsureRegistry();
            registry.activeCatalog = catalog;
            EditorUtility.SetDirty(registry);
            AssetDatabase.SaveAssets();
            defaultCatalog = catalog;
        }

        public static DeverQuestAncestry FindAncestry(
            string id,
            string legacyName = "")
        {
            Ensure();
            return ancestries.FirstOrDefault(value =>
                       value.IdentityId == id) ??
                   ancestries.FirstOrDefault(value =>
                       string.Equals(
                           value.displayName,
                           legacyName,
                           StringComparison.OrdinalIgnoreCase));
        }

        public static DeverQuestClassDefinition FindClass(
            string id,
            string legacyName = "")
        {
            Ensure();
            return classes.FirstOrDefault(value =>
                       value.IdentityId == id) ??
                   classes.FirstOrDefault(value =>
                       string.Equals(
                           value.displayName,
                           legacyName,
                           StringComparison.OrdinalIgnoreCase));
        }

        public static DeverQuestDeity FindFaith(
            string id,
            string legacyName = "")
        {
            Ensure();
            return faiths.FirstOrDefault(value =>
                       value.IdentityId == id) ??
                   faiths.FirstOrDefault(value =>
                       string.Equals(
                           value.displayName,
                           legacyName,
                           StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsEligible(
            DeverQuestAncestry ancestry,
            DeverQuestClassDefinition classDefinition,
            out string reason)
        {
            Ensure();
            reason = string.Empty;
            if (ancestry == null || classDefinition == null)
            {
                reason = "Select an Ancestry and Class.";
                return false;
            }
            if (!ancestry.playable || !ancestry.sapient)
            {
                reason =
                    "That Ancestry is not available for Adventurers.";
                return false;
            }
            if (defaultCatalog != null &&
                (!(defaultCatalog.ancestries ??
                   new List<DeverQuestAncestry>())
                    .Contains(ancestry) ||
                 !(defaultCatalog.classes ??
                   new List<DeverQuestClassDefinition>())
                    .Contains(classDefinition)))
            {
                reason =
                    "That identity is not offered by the active Guild " +
                    "Identity Catalog.";
                return false;
            }
            if ((ancestry.restrictedClassIds ??
                 new List<string>()).Contains(
                    classDefinition.IdentityId))
            {
                reason =
                    $"{classDefinition.displayName} is restricted for " +
                    $"{ancestry.displayName}.";
                return false;
            }
            if ((ancestry.eligibleClassIds?.Count ?? 0) > 0 &&
                !ancestry.eligibleClassIds.Contains(
                    classDefinition.IdentityId))
            {
                reason =
                    $"{classDefinition.displayName} is not in this " +
                    "Ancestry's eligibility list.";
                return false;
            }
            return true;
        }

        public static bool IsFaithEligible(
            DeverQuestDeity faith,
            DeverQuestAncestry ancestry,
            DeverQuestClassDefinition classDefinition,
            DeverQuestAlignment alignment,
            out string reason)
        {
            Ensure();
            reason = string.Empty;
            if (faith == null)
            {
                return true;
            }
            if (defaultCatalog != null &&
                !(defaultCatalog.faiths ??
                  new List<DeverQuestDeity>()).Contains(faith))
            {
                reason =
                    "That Faith is not offered by the active Guild " +
                    "Identity Catalog.";
                return false;
            }
            if ((faith.allowedAlignments?.Count ?? 0) > 0 &&
                !faith.allowedAlignments.Contains(alignment))
            {
                reason =
                    $"{faith.displayName} does not accept that Alignment.";
                return false;
            }
            if (ancestry != null &&
                (faith.restrictedAncestryIds ??
                 new List<string>()).Contains(
                    ancestry.IdentityId))
            {
                reason =
                    $"{faith.displayName} is unavailable to that Ancestry.";
                return false;
            }
            return true;
        }

        public static void ApplyIdentityFoundation(
            DeverQuestAdventurer target,
            DeverQuestAncestry ancestry,
            DeverQuestClassDefinition classDefinition,
            DeverQuestDeity faith,
            DeverQuestAlignment alignment,
            bool resetVitals)
        {
            if (target == null || classDefinition == null)
            {
                return;
            }
            target.characterClass = classDefinition.displayName;
            target.classId = classDefinition.IdentityId;
            target.ancestryName =
                ancestry?.displayName ?? string.Empty;
            target.ancestryId =
                ancestry?.IdentityId ?? string.Empty;
            target.deityName =
                faith?.displayName ?? "Agnostic";
            target.deityId =
                faith?.IdentityId ?? string.Empty;
            target.alignment = alignment;
            target.homeDepartment = classDefinition.department;
            target.strength = classDefinition.strength;
            target.dexterity = classDefinition.dexterity;
            target.constitution = classDefinition.constitution;
            target.intelligence = classDefinition.intelligence;
            target.wisdom = classDefinition.wisdom;
            target.charisma = classDefinition.charisma;
            target.luck = classDefinition.luck;
            target.agility = target.dexterity;
            target.stamina = target.constitution;
            target.hitDie = classDefinition.hitDie;
            target.proficientSaves =
                new List<string>(
                    classDefinition.proficientSaves ??
                    new List<string>());
            if (ancestry != null)
            {
                foreach (DeverQuestAbilityAdjustment adjustment
                         in ancestry.abilityAdjustments ??
                            new List<DeverQuestAbilityAdjustment>())
                {
                    if (adjustment != null)
                    {
                        AdjustAbility(
                            target,
                            adjustment.ability,
                            adjustment.amount);
                    }
                }
            }
            int constitutionModifier =
                DeverQuestRulesService.AbilityModifier(
                    target.constitution);
            target.maximumHitPoints = Math.Max(
                1,
                classDefinition.hitDie +
                constitutionModifier +
                (ancestry?.hitPointBonus ?? 0));
            target.maximumMana = classDefinition.usesMana
                ? Math.Max(
                    1,
                    target.level * 5 +
                    DeverQuestRulesService.AbilityModifier(
                        target.intelligence) * 2 +
                    (ancestry?.manaBonus ?? 0))
                : 0;
            if (resetVitals ||
                target.currentHitPoints <= 0)
            {
                target.currentHitPoints =
                    target.maximumHitPoints;
                target.currentMana =
                    target.maximumMana;
            }
        }

        public static void Migrate(DeverQuestAdventurer adventurer)
        {
            if (adventurer == null)
            {
                return;
            }
            DeverQuestClassDefinition classDefinition =
                FindClass(
                    adventurer.classId,
                    adventurer.characterClass);
            if (classDefinition != null)
            {
                adventurer.classId =
                    classDefinition.IdentityId;
                adventurer.characterClass =
                    classDefinition.displayName;
            }
            DeverQuestAncestry ancestry =
                FindAncestry(
                    adventurer.ancestryId,
                    adventurer.ancestryName);
            if (ancestry == null &&
                string.IsNullOrWhiteSpace(adventurer.ancestryId) &&
                string.IsNullOrWhiteSpace(adventurer.ancestryName))
            {
                ancestry = defaultCatalog?.defaultAncestry;
            }
            if (ancestry != null)
            {
                adventurer.ancestryId = ancestry.IdentityId;
                adventurer.ancestryName = ancestry.displayName;
            }
            DeverQuestDeity faith =
                FindFaith(
                    adventurer.deityId,
                    adventurer.deityName);
            if (faith == null &&
                string.IsNullOrWhiteSpace(adventurer.deityId))
            {
                faith = defaultCatalog?.defaultFaith;
            }
            if (faith != null)
            {
                adventurer.deityId = faith.IdentityId;
                adventurer.deityName = faith.displayName;
            }
        }

        public static void Migrate(DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return;
            }
            DeverQuestClassDefinition classDefinition =
                FindClass(account.classId, account.characterClass);
            if (classDefinition != null)
            {
                account.classId = classDefinition.IdentityId;
                account.characterClass =
                    classDefinition.displayName;
            }
            DeverQuestAncestry ancestry =
                FindAncestry(account.ancestryId, account.ancestryName);
            if (ancestry == null &&
                string.IsNullOrWhiteSpace(account.ancestryId) &&
                string.IsNullOrWhiteSpace(account.ancestryName))
            {
                ancestry = defaultCatalog?.defaultAncestry;
            }
            if (ancestry != null)
            {
                account.ancestryId = ancestry.IdentityId;
                account.ancestryName = ancestry.displayName;
            }
            DeverQuestDeity faith =
                FindFaith(account.deityId, account.deityName);
            if (faith == null &&
                string.IsNullOrWhiteSpace(account.deityId))
            {
                faith = defaultCatalog?.defaultFaith;
            }
            if (faith != null)
            {
                account.deityId = faith.IdentityId;
                account.deityName = faith.displayName;
            }
        }

        public static void Clear()
        {
            ancestries = null;
            classes = null;
            faiths = null;
            catalogs = null;
            registry = null;
            defaultCatalog = null;
        }

        private static void Ensure()
        {
            if (ancestries == null)
            {
                ancestries =
                    LoadAll<DeverQuestAncestry>()
                        .Where(value => value != null)
                        .OrderBy(value => value.displayName)
                        .ToList();
            }
            if (classes == null)
            {
                classes =
                    LoadAll<DeverQuestClassDefinition>()
                        .Where(value => value != null)
                        .OrderBy(value => value.displayName)
                        .ToList();
            }
            if (faiths == null)
            {
                faiths =
                    LoadAll<DeverQuestDeity>()
                        .Where(value => value != null)
                        .OrderBy(value => value.displayName)
                        .ToList();
            }
            if (catalogs == null)
            {
                catalogs =
                    LoadAll<DeverQuestIdentityCatalog>()
                        .Where(value => value != null)
                        .OrderBy(value => value.displayName)
                        .ToList();
            }
            if (defaultCatalog == null)
            {
                if (registry == null)
                {
                    registry =
                        AssetDatabase.LoadAssetAtPath<
                            DeverQuestIdentityCatalogRegistry>(
                            RegistryPath);
                }
                defaultCatalog =
                    registry?.activeCatalog ??
                    catalogs.FirstOrDefault(value =>
                        value.displayName ==
                        "Original Guild Identity Catalog") ??
                    catalogs.FirstOrDefault();
            }
        }

        private static void EnsureRegistry()
        {
            if (registry != null)
            {
                return;
            }

            registry =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestIdentityCatalogRegistry>(
                    RegistryPath);
            if (registry != null)
            {
                return;
            }

            EnsureFolder("Assets/DeverQuest/IdentityCatalogs");

            // Earlier Beta builds could leave a Missing Script asset at the
            // canonical registry path. Unity will not overwrite that file,
            // and CreateAsset may invalidate the temporary object instead of
            // producing a usable registry. Remove only an incompatible asset
            // occupying the reserved registry path.
            string existingGuid =
                AssetDatabase.AssetPathToGUID(RegistryPath);
            if (!string.IsNullOrWhiteSpace(existingGuid))
            {
                if (!AssetDatabase.DeleteAsset(RegistryPath))
                {
                    throw new InvalidOperationException(
                        "DeverQuest could not replace the invalid Guild " +
                        "Identity Registry at " + RegistryPath + ".");
                }
            }

            DeverQuestIdentityCatalogRegistry createdRegistry =
                UnityEngine.ScriptableObject.CreateInstance<
                    DeverQuestIdentityCatalogRegistry>();
            createdRegistry.name = "GuildIdentityRegistry";

            try
            {
                AssetDatabase.CreateAsset(
                    createdRegistry, RegistryPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(
                    RegistryPath,
                    ImportAssetOptions.ForceSynchronousImport);
            }
            catch
            {
                if (createdRegistry != null &&
                    !AssetDatabase.Contains(createdRegistry))
                {
                    UnityEngine.Object.DestroyImmediate(
                        createdRegistry);
                }
                throw;
            }

            registry =
                AssetDatabase.LoadAssetAtPath<
                    DeverQuestIdentityCatalogRegistry>(
                    RegistryPath);
            if (registry == null)
            {
                throw new InvalidOperationException(
                    "Unity created no usable Guild Identity Registry at " +
                    RegistryPath + ". Check the Console for asset import " +
                    "errors, then retry generation.");
            }
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

        private static IEnumerable<T> LoadAll<T>()
            where T : UnityEngine.Object
        {
            foreach (string guid in AssetDatabase.FindAssets(
                         $"t:{typeof(T).Name}"))
            {
                T asset = AssetDatabase.LoadAssetAtPath<T>(
                    AssetDatabase.GUIDToAssetPath(guid));
                if (asset != null)
                {
                    yield return asset;
                }
            }
        }

        private static void AdjustAbility(
            DeverQuestAdventurer target,
            DeverQuestAbility ability,
            int amount)
        {
            switch (ability)
            {
                case DeverQuestAbility.Strength:
                    target.strength += amount;
                    break;
                case DeverQuestAbility.Dexterity:
                    target.dexterity += amount;
                    break;
                case DeverQuestAbility.Constitution:
                    target.constitution += amount;
                    break;
                case DeverQuestAbility.Intelligence:
                    target.intelligence += amount;
                    break;
                case DeverQuestAbility.Wisdom:
                    target.wisdom += amount;
                    break;
                case DeverQuestAbility.Charisma:
                    target.charisma += amount;
                    break;
                case DeverQuestAbility.Agility:
                    target.agility += amount;
                    break;
                case DeverQuestAbility.Stamina:
                    target.stamina += amount;
                    break;
                case DeverQuestAbility.Luck:
                    target.luck += amount;
                    break;
            }
        }
    }
}
