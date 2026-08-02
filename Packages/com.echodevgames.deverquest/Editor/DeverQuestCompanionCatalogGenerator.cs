using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestCompanionGenerationReport
    {
        public int Created;
        public int Preserved;
        public string RootPath = string.Empty;
        public DeverQuestCompanionCatalog Catalog;

        public string Summary =>
            $"Companion Stable ready: {Created} created, " +
            $"{Preserved} preserved.";
    }

    internal static class DeverQuestCompanionCatalogGenerator
    {
        private const string Root =
            "Assets/DeverQuest/Companions/OriginalStarter";

        public static DeverQuestCompanionGenerationReport
            GenerateOriginalStarterCatalog()
        {
            DeverQuestCompanionGenerationReport report =
                new DeverQuestCompanionGenerationReport
                {
                    RootPath = Root
                };
            EnsureFolder(Root + "/Profiles");

            DeverQuestCompanionProfile gravebound =
                CreateProfile(
                    "Gravebound_Wisp",
                    report,
                    "Gravebound Wisp",
                    "A patient lantern-spirit bound by mutual oath, " +
                    "never by stolen will.",
                    DeverQuestCompanionKind.BoundMinion,
                    DeverQuestCompanionRole.Controller,
                    DeverQuestCreatureType.Spirit,
                    DeverQuestDamageType.Shadow,
                    profile =>
                    {
                        profile.damageAffinities =
                            Affinities(
                                DeverQuestDamageType.Poison,
                                DeverQuestDamageResponse.Immune);
                    },
                    "Necromancer");

            DeverQuestCompanionProfile trailclaw =
                CreateProfile(
                    "Trailclaw_Lynx",
                    report,
                    "Trailclaw Lynx",
                    "A swift woodland hunter that chooses its " +
                    "Adventurer through earned trust.",
                    DeverQuestCompanionKind.BondedBeast,
                    DeverQuestCompanionRole.Striker,
                    DeverQuestCreatureType.Beast,
                    DeverQuestDamageType.Slashing,
                    profile => profile.damageDice = "1d6",
                    "Ranger",
                    "Wildwarden");

            DeverQuestCompanionProfile verdant =
                CreateProfile(
                    "Verdant_Mote",
                    report,
                    "Verdant Mote",
                    "A tiny seed-light that mends wounds and hums " +
                    "when a Quest returns life to the world.",
                    DeverQuestCompanionKind.Familiar,
                    DeverQuestCompanionRole.Support,
                    DeverQuestCreatureType.Fey,
                    DeverQuestDamageType.Radiant,
                    null,
                    "Druid");

            DeverQuestCompanionProfile ancestor =
                CreateProfile(
                    "Ancestor_Echo",
                    report,
                    "Ancestor Echo",
                    "A willing memory-spirit that guides its companion " +
                    "through rhythm, warning, and song.",
                    DeverQuestCompanionKind.Spirit,
                    DeverQuestCompanionRole.Support,
                    DeverQuestCreatureType.Spirit,
                    DeverQuestDamageType.Sonic,
                    profile =>
                    {
                        profile.damageAffinities =
                            Affinities(
                                DeverQuestDamageType.Psychic,
                                DeverQuestDamageResponse.Resistant);
                    },
                    "Shaman");

            DeverQuestCompanionProfile brasswing =
                CreateProfile(
                    "Brasswing_Sentry",
                    report,
                    "Brasswing Sentry",
                    "A Guild-built clockwork guardian available to any " +
                    "Adventurer able to earn its charter.",
                    DeverQuestCompanionKind.Construct,
                    DeverQuestCompanionRole.Guardian,
                    DeverQuestCreatureType.Construct,
                    DeverQuestDamageType.Piercing,
                    profile =>
                    {
                        profile.requiresCompanionClass = false;
                        profile.recruitCopperCost = 50;
                        profile.maximumHitPoints = 12;
                        profile.armorClass = 13;
                        profile.damageAffinities =
                            Affinities(
                                DeverQuestDamageType.Poison,
                                DeverQuestDamageResponse.Immune);
                    });

            List<DeverQuestCompanionProfile> profiles =
                new List<DeverQuestCompanionProfile>
                {
                    gravebound,
                    trailclaw,
                    verdant,
                    ancestor,
                    brasswing
                };
            report.Catalog = GetOrCreate<
                DeverQuestCompanionCatalog>(
                Root + "/Original_Guild_Companion_Catalog.asset",
                report,
                catalog =>
                {
                    catalog.displayName =
                        "Original Guild Companion Stable";
                    catalog.description =
                        "Commercially clean starter Companions for " +
                        "DeverQuest's supported traditions.";
                });
            report.Catalog.companions =
                report.Catalog.companions ??
                new List<DeverQuestCompanionProfile>();
            foreach (DeverQuestCompanionProfile profile in profiles)
            {
                if (!report.Catalog.companions.Contains(profile))
                {
                    report.Catalog.companions.Add(profile);
                }
            }
            EditorUtility.SetDirty(report.Catalog);

            AssignStarter("Necromancer", gravebound);
            AssignStarter("Ranger", trailclaw);
            AssignStarter("Wildwarden", trailclaw);
            AssignStarter("Druid", verdant);
            AssignStarter("Shaman", ancestor);

            foreach (DeverQuestCompanionProfile profile in profiles)
            {
                EditorUtility.SetDirty(profile);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return report;
        }

        private static DeverQuestCompanionProfile CreateProfile(
            string fileName,
            DeverQuestCompanionGenerationReport report,
            string displayName,
            string lore,
            DeverQuestCompanionKind kind,
            DeverQuestCompanionRole role,
            DeverQuestCreatureType creatureType,
            DeverQuestDamageType damageType,
            Action<DeverQuestCompanionProfile> customize,
            params string[] allowedClasses)
        {
            return GetOrCreate<DeverQuestCompanionProfile>(
                Root + "/Profiles/" + fileName + ".asset",
                report,
                profile =>
                {
                    profile.displayName = displayName;
                    profile.lore = lore;
                    profile.kind = kind;
                    profile.role = role;
                    profile.creatureType = creatureType;
                    profile.damageType = damageType;
                    profile.allowedClassNames =
                        allowedClasses.ToList();
                    foreach (string className in allowedClasses)
                    {
                        DeverQuestClassDefinition classDefinition =
                            DeverQuestIdentityCatalogService.Classes
                                .FirstOrDefault(value =>
                                    string.Equals(
                                        value.displayName,
                                        className,
                                        StringComparison
                                            .OrdinalIgnoreCase));
                        if (classDefinition != null)
                        {
                            profile.allowedClassIds.Add(
                                classDefinition.IdentityId);
                        }
                    }
                    customize?.Invoke(profile);
                });
        }

        private static List<DeverQuestDamageAffinity> Affinities(
            DeverQuestDamageType damageType,
            DeverQuestDamageResponse response)
        {
            return new List<DeverQuestDamageAffinity>
            {
                new DeverQuestDamageAffinity
                {
                    damageType = damageType,
                    response = response
                }
            };
        }

        private static void AssignStarter(
            string className,
            DeverQuestCompanionProfile profile)
        {
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.Classes
                    .FirstOrDefault(value =>
                        string.Equals(
                            value.displayName,
                            className,
                            StringComparison.OrdinalIgnoreCase));
            if (classDefinition == null)
            {
                return;
            }
            classDefinition.supportsCompanion = true;
            classDefinition.starterCompanion = profile;
            EditorUtility.SetDirty(classDefinition);
        }

        private static T GetOrCreate<T>(
            string path,
            DeverQuestCompanionGenerationReport report,
            Action<T> initialize)
            where T : ScriptableObject
        {
            T asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null)
            {
                report.Preserved++;
                return asset;
            }
            asset = ScriptableObject.CreateInstance<T>();
            initialize?.Invoke(asset);
            AssetDatabase.CreateAsset(asset, path);
            report.Created++;
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
                        current,
                        parts[index]);
                }
                current = next;
            }
        }
    }
}
