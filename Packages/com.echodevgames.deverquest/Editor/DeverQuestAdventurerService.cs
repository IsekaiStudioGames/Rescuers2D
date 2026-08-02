//----- DeverQuestAdventurerService.cs START -----

using System;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestAdventurer
    {
        public int dataVersion = 1;
        public string characterName = string.Empty;
        public string guildName = "Isekai Studios";
        public string guildRank = "Member";
        public string characterClass = "Warrior";
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public long copperBalance;
        public long totalCopperEarned;
        public long totalCopperSpent;

        public void Sanitize()
        {
            characterName = characterName?.Trim() ?? string.Empty;
            guildName = guildName?.Trim() ?? string.Empty;
            guildRank = guildRank?.Trim() ?? string.Empty;
            characterClass = characterClass?.Trim() ?? string.Empty;
            level = Math.Max(1, level);
            currentExperience = Math.Max(0L, currentExperience);
            lifetimeExperience = Math.Max(0L, lifetimeExperience);
            copperBalance = Math.Max(0L, copperBalance);
            totalCopperEarned = Math.Max(0L, totalCopperEarned);
            totalCopperSpent = Math.Max(0L, totalCopperSpent);
            dataVersion = 1;
        }
    }

    internal readonly struct DeverQuestProgressionResult
    {
        public readonly int StartingLevel;
        public readonly int EndingLevel;
        public readonly long ExperienceAwarded;

        public bool LeveledUp => EndingLevel > StartingLevel;

        public DeverQuestProgressionResult(
            int startingLevel,
            int endingLevel,
            long experienceAwarded)
        {
            StartingLevel = startingLevel;
            EndingLevel = endingLevel;
            ExperienceAwarded = experienceAwarded;
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestAdventurerService
    {
        private const string AdventurerKey =
            "EchoDevGames.DeverQuest.Adventurer.v1";

        private static DeverQuestAdventurer adventurer;

        public static readonly string[] Classes =
        {
            "Warrior", "Paladin", "Ranger", "Rogue", "Cleric", "Druid",
            "Wizard", "Sorcerer", "Necromancer", "Bard", "Monk", "Barbarian"
        };

        public static readonly string[] GuildRanks =
        {
            "Member", "Project Leader", "Boss", "CEO"
        };

        static DeverQuestAdventurerService()
        {
            Load();
        }

        public static DeverQuestAdventurer Adventurer
        {
            get
            {
                if (adventurer == null)
                {
                    Load();
                }

                return adventurer;
            }
        }

        public static long ExperienceForNextLevel(int level)
        {
            return Math.Max(100L, level * 100L);
        }

        public static DeverQuestProgressionResult Award(
            long copper,
            long experience)
        {
            DeverQuestAdventurer target = Adventurer;
            int startingLevel = target.level;
            copper = Math.Max(0L, copper);
            experience = Math.Max(0L, experience);

            target.copperBalance += copper;
            target.totalCopperEarned += copper;
            target.currentExperience += experience;
            target.lifetimeExperience += experience;

            while (target.currentExperience >=
                   ExperienceForNextLevel(target.level))
            {
                target.currentExperience -=
                    ExperienceForNextLevel(target.level);
                target.level++;
            }

            Save();
            return new DeverQuestProgressionResult(
                startingLevel,
                target.level,
                experience);
        }

        public static bool SpendCopper(long copper, out string error)
        {
            error = string.Empty;
            copper = Math.Max(0L, copper);
            if (copper <= 0L)
            {
                error = "Enter a coin cost greater than zero.";
                return false;
            }

            if (copper > Adventurer.copperBalance)
            {
                error =
                    $"Only {FormatCoins(Adventurer.copperBalance)} is available.";
                return false;
            }

            Adventurer.copperBalance -= copper;
            Adventurer.totalCopperSpent += copper;
            Save();
            return true;
        }

        public static void MigrateLegacyCopper(long copper)
        {
            copper = Math.Max(0L, copper);
            if (copper <= 0L)
            {
                return;
            }

            Adventurer.copperBalance += copper;
            Adventurer.totalCopperEarned += copper;
            Save();
        }

        public static string FormatCoins(long totalCopper)
        {
            totalCopper = Math.Max(0L, totalCopper);
            long platinum = totalCopper / 1000000L;
            long remainder = totalCopper % 1000000L;
            long gold = remainder / 10000L;
            remainder %= 10000L;
            long silver = remainder / 100L;
            long copper = remainder % 100L;

            return $"{platinum}p {gold}g {silver}s {copper}c";
        }

        public static void Save()
        {
            Adventurer.Sanitize();
            EditorPrefs.SetString(
                AdventurerKey,
                JsonUtility.ToJson(Adventurer));
        }

        private static void Load()
        {
            string json = EditorPrefs.GetString(AdventurerKey, string.Empty);
            try
            {
                adventurer = string.IsNullOrWhiteSpace(json)
                    ? new DeverQuestAdventurer()
                    : JsonUtility.FromJson<DeverQuestAdventurer>(json) ??
                      new DeverQuestAdventurer();
                adventurer.Sanitize();
            }
            catch
            {
                Debug.LogWarning(
                    "[DeverQuest] Adventurer profile could not be loaded.");
                adventurer = new DeverQuestAdventurer();
            }
        }
    }
}

//----- DeverQuestAdventurerService.cs END -----
