using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestCompanionService
    {
        private static Dictionary<string, DeverQuestCompanionProfile>
            profileCache;

        static DeverQuestCompanionService()
        {
            EditorApplication.projectChanged -= ClearCache;
            EditorApplication.projectChanged += ClearCache;
        }

        public static IReadOnlyList<DeverQuestCompanionProfile> Profiles
        {
            get
            {
                EnsureCache();
                return profileCache.Values
                    .Where(value => value != null)
                    .OrderBy(value => value.displayName)
                    .ToList();
            }
        }

        public static DeverQuestCompanionProfile FindProfile(
            string profileId)
        {
            EnsureCache();
            return profileCache.TryGetValue(
                profileId ?? string.Empty,
                out DeverQuestCompanionProfile profile)
                ? profile
                : null;
        }

        public static DeverQuestCompanionState ActiveCompanion(
            DeverQuestAdventurer adventurer)
        {
            if (adventurer == null)
            {
                return null;
            }
            adventurer.companions =
                adventurer.companions ??
                new List<DeverQuestCompanionState>();
            DeverQuestCompanionState active =
                adventurer.companions.FirstOrDefault(value =>
                    value != null &&
                    value.instanceId ==
                    adventurer.activeCompanionInstanceId &&
                    !value.isFallen);
            if (active == null)
            {
                adventurer.activeCompanionInstanceId = string.Empty;
            }
            return active;
        }

        public static bool CanRecruit(
            DeverQuestAdventurer adventurer,
            DeverQuestCompanionProfile profile,
            out string reason)
        {
            reason = string.Empty;
            if (adventurer == null || profile == null)
            {
                reason = "Select a Companion Profile.";
                return false;
            }
            if (adventurer.level < profile.minimumAdventurerLevel)
            {
                reason =
                    $"Requires Adventurer level " +
                    $"{profile.minimumAdventurerLevel}.";
                return false;
            }
            if ((adventurer.companions ??
                 new List<DeverQuestCompanionState>())
                .Any(value =>
                    value != null &&
                    value.profileId == profile.CompanionId))
            {
                reason = "This Companion is already in the roster.";
                return false;
            }
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.FindClass(
                    adventurer.classId,
                    adventurer.characterClass);
            bool supportsCompanion =
                classDefinition?.supportsCompanion == true ||
                SupportsLegacyClass(adventurer.characterClass);
            if (profile.requiresCompanionClass &&
                !supportsCompanion)
            {
                reason =
                    "This Class does not have a Companion tradition.";
                return false;
            }
            bool hasClassRestrictions =
                (profile.allowedClassIds?.Count ?? 0) > 0 ||
                (profile.allowedClassNames?.Count ?? 0) > 0;
            bool classAllowed =
                (profile.allowedClassIds ??
                 new List<string>())
                .Contains(adventurer.classId) ||
                (profile.allowedClassNames ??
                 new List<string>())
                .Any(value => string.Equals(
                    value,
                    adventurer.characterClass,
                    StringComparison.OrdinalIgnoreCase));
            if (hasClassRestrictions && !classAllowed)
            {
                reason =
                    $"{profile.displayName} does not answer to the " +
                    $"{adventurer.characterClass} tradition.";
                return false;
            }
            if (profile.recruitCopperCost >
                adventurer.copperBalance)
            {
                reason =
                    $"Recruitment costs " +
                    $"{DeverQuestAdventurerService.FormatCoins(profile.recruitCopperCost)}.";
                return false;
            }
            return true;
        }

        public static bool Recruit(
            DeverQuestCompanionProfile profile,
            out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (!CanRecruit(adventurer, profile, out message))
            {
                return false;
            }
            if (profile.recruitCopperCost > 0 &&
                !DeverQuestAdventurerService.SpendCopper(
                    profile.recruitCopperCost,
                    out message))
            {
                return false;
            }
            DeverQuestCompanionState state =
                CreateState(profile);
            bool makeActive =
                ActiveCompanion(adventurer) == null;
            state.isActive = makeActive;
            adventurer.companions.Add(state);
            if (makeActive)
            {
                adventurer.activeCompanionInstanceId =
                    state.instanceId;
            }
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Companion Recruited",
                state.customName,
                $"{profile.kind} · {profile.role}");
            message =
                $"{state.customName} joined the Companion roster" +
                (makeActive ? " and is now active." : ".");
            return true;
        }

        public static void GrantStarter(
            DeverQuestAdventurer adventurer,
            DeverQuestCompanionProfile profile)
        {
            if (adventurer == null || profile == null ||
                (adventurer.companions ??
                 new List<DeverQuestCompanionState>())
                .Any(value =>
                    value != null &&
                    value.profileId == profile.CompanionId))
            {
                return;
            }
            DeverQuestCompanionState state = CreateState(profile);
            state.isActive = true;
            adventurer.companions.Add(state);
            adventurer.activeCompanionInstanceId =
                state.instanceId;
        }

        public static bool Activate(
            DeverQuestCompanionState state,
            out string message)
        {
            message = string.Empty;
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (state == null ||
                !adventurer.companions.Contains(state))
            {
                message = "That Companion is not in this roster.";
                return false;
            }
            if (state.isFallen || state.currentHitPoints <= 0)
            {
                message =
                    "Recover this Companion before making it active.";
                return false;
            }
            foreach (DeverQuestCompanionState companion in
                     adventurer.companions)
            {
                companion.isActive = companion == state;
            }
            adventurer.activeCompanionInstanceId = state.instanceId;
            DeverQuestAdventurerService.Save();
            message = $"{DisplayName(state)} is now active.";
            return true;
        }

        public static void Dismiss(
            DeverQuestCompanionState state)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (state == null)
            {
                return;
            }
            state.isActive = false;
            if (adventurer.activeCompanionInstanceId ==
                state.instanceId)
            {
                adventurer.activeCompanionInstanceId =
                    string.Empty;
            }
            DeverQuestAdventurerService.Save();
        }

        public static bool Recover(
            DeverQuestCompanionState state,
            out string message)
        {
            message = string.Empty;
            DeverQuestCompanionProfile profile =
                FindProfile(state?.profileId);
            if (state == null || profile == null)
            {
                message = "The Companion Profile is unavailable.";
                return false;
            }
            int maximumHitPoints =
                MaximumHitPoints(state, profile);
            if (!state.isFallen &&
                state.currentHitPoints >= maximumHitPoints)
            {
                message = $"{DisplayName(state)} is already healthy.";
                return false;
            }
            if (profile.recoveryCopperCost > 0 &&
                !DeverQuestAdventurerService.SpendCopper(
                    profile.recoveryCopperCost,
                    out message))
            {
                return false;
            }
            state.isFallen = false;
            state.currentHitPoints = maximumHitPoints;
            state.loyalty = Math.Max(1, state.loyalty - 1);
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Companion Recovered",
                DisplayName(state),
                DeverQuestAdventurerService.FormatCoins(
                    profile.recoveryCopperCost));
            message = $"{DisplayName(state)} has recovered.";
            return true;
        }

        public static int RecoveryCost(
            DeverQuestCompanionState state)
        {
            DeverQuestCompanionProfile profile =
                FindProfile(state?.profileId);
            if (state == null || profile == null)
            {
                return 0;
            }
            int maximumHitPoints =
                MaximumHitPoints(state, profile);
            return state.isFallen ||
                   state.currentHitPoints < maximumHitPoints
                ? Math.Max(0, profile.recoveryCopperCost)
                : 0;
        }

        public static bool RecoverAll(out string message)
        {
            message = string.Empty;
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            List<DeverQuestCompanionState> targets =
                (adventurer.companions ??
                 new List<DeverQuestCompanionState>())
                .Where(state =>
                {
                    DeverQuestCompanionProfile profile =
                        FindProfile(state?.profileId);
                    return state != null &&
                           profile != null &&
                           (state.isFallen ||
                            state.currentHitPoints <
                            MaximumHitPoints(state, profile));
                })
                .ToList();
            if (targets.Count == 0)
            {
                message = "Every Companion is already ready.";
                return false;
            }

            int totalCost = targets.Sum(RecoveryCost);
            if (totalCost > 0 &&
                !DeverQuestAdventurerService.SpendCopper(
                    totalCost,
                    out message))
            {
                return false;
            }

            foreach (DeverQuestCompanionState state in targets)
            {
                DeverQuestCompanionProfile profile =
                    FindProfile(state.profileId);
                state.isFallen = false;
                state.currentHitPoints =
                    MaximumHitPoints(state, profile);
                state.loyalty = Math.Max(1, state.loyalty - 1);
            }
            DeverQuestAdventurerService.Save();
            DeverQuestGuildAccountService.AddAudit(
                "Companion Roster Recovered",
                $"{targets.Count} Companion(s)",
                DeverQuestAdventurerService.FormatCoins(totalCost));
            message =
                $"Recovered {targets.Count} Companion" +
                (targets.Count == 1 ? string.Empty : "s") +
                $" for {DeverQuestAdventurerService.FormatCoins(totalCost)}.";
            return true;
        }

        public static void CompleteBattle(
            DeverQuestCompanionState state,
            bool encounterVictory,
            long encounterExperience)
        {
            if (state == null)
            {
                return;
            }
            state.battles++;
            if (encounterVictory && !state.isFallen)
            {
                state.victories++;
                state.loyalty = Math.Min(100, state.loyalty + 2);
                AwardExperience(
                    state,
                    Math.Max(1L, encounterExperience / 2L));
            }
            else
            {
                state.loyalty = Math.Max(0, state.loyalty - 1);
            }
        }

        public static int MaximumHitPoints(
            DeverQuestCompanionState state,
            DeverQuestCompanionProfile profile)
        {
            if (state == null || profile == null)
            {
                return 1;
            }
            return Math.Max(
                1,
                profile.maximumHitPoints +
                Math.Max(0, state.level - 1) *
                profile.hitPointsPerLevel);
        }

        public static int ArmorClass(
            DeverQuestCompanionState state,
            DeverQuestCompanionProfile profile)
        {
            return Math.Max(
                1,
                (profile?.armorClass ?? 1) +
                Math.Max(0, (state?.level ?? 1) - 1) / 4);
        }

        public static int AttackModifier(
            DeverQuestCompanionState state,
            DeverQuestCompanionProfile profile)
        {
            int loyaltyBonus =
                (state?.loyalty ?? 0) >= 75 ? 1 : 0;
            return (profile?.attackModifier ?? 0) +
                   Math.Max(0, (state?.level ?? 1) - 1) / 3 +
                   loyaltyBonus;
        }

        public static long ExperienceForNextLevel(int level)
        {
            return Math.Max(50L, level * 50L);
        }

        public static string DisplayName(
            DeverQuestCompanionState state)
        {
            if (state == null)
            {
                return "No Companion";
            }
            if (!string.IsNullOrWhiteSpace(state.customName))
            {
                return state.customName;
            }
            return FindProfile(state.profileId)?.displayName ??
                   "Unknown Companion";
        }

        private static DeverQuestCompanionState CreateState(
            DeverQuestCompanionProfile profile)
        {
            DeverQuestCompanionState state =
                new DeverQuestCompanionState
                {
                    instanceId = Guid.NewGuid().ToString("N"),
                    profileId = profile.CompanionId,
                    customName = profile.displayName,
                    level = 1,
                    loyalty = profile.startingLoyalty,
                    currentHitPoints = profile.maximumHitPoints,
                    recruitedUtc = DateTime.UtcNow.ToString("O")
                };
            state.Sanitize();
            return state;
        }

        private static void AwardExperience(
            DeverQuestCompanionState state,
            long experience)
        {
            state.currentExperience += Math.Max(0L, experience);
            state.lifetimeExperience += Math.Max(0L, experience);
            while (state.currentExperience >=
                   ExperienceForNextLevel(state.level))
            {
                long required =
                    ExperienceForNextLevel(state.level);
                state.currentExperience -= required;
                state.level++;
                DeverQuestCompanionProfile profile =
                    FindProfile(state.profileId);
                state.currentHitPoints +=
                    Math.Max(1, profile?.hitPointsPerLevel ?? 1);
            }
        }

        private static bool SupportsLegacyClass(
            string characterClass)
        {
            switch (characterClass)
            {
                case "Necromancer":
                case "Ranger":
                case "Druid":
                case "Shaman":
                case "Wildwarden":
                    return true;
                default:
                    return false;
            }
        }

        private static void EnsureCache()
        {
            if (profileCache != null)
            {
                return;
            }
            profileCache =
                new Dictionary<string, DeverQuestCompanionProfile>();
            foreach (string guid in AssetDatabase.FindAssets(
                         "t:DeverQuestCompanionProfile"))
            {
                DeverQuestCompanionProfile profile =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestCompanionProfile>(
                        AssetDatabase.GUIDToAssetPath(guid));
                if (profile != null)
                {
                    profileCache[profile.CompanionId] = profile;
                }
            }
        }

        private static void ClearCache()
        {
            profileCache = null;
        }
    }
}
