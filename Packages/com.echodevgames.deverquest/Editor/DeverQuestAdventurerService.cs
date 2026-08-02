//----- DeverQuestAdventurerService.cs START -----

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestAdventurer
    {
        public int dataVersion = 9;
        public string characterName = string.Empty;
        public string guildName = "Isekai Studios";
        public string guildRank = "Member";
        public string characterClass = "Warrior";
        public string classId = string.Empty;
        public string ancestryName = string.Empty;
        public string ancestryId = string.Empty;
        public string deityName = "Agnostic";
        public string deityId = string.Empty;
        public DeverQuestAlignment alignment =
            DeverQuestAlignment.TrueNeutral;
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public long copperBalance;
        public long platinumCoins;
        public long goldCoins;
        public long silverCoins;
        public long copperCoins;
        public long totalCopperEarned;
        public long totalCopperSpent;
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int charisma = 10;
        public int agility = 10;
        public int stamina = 10;
        public int luck = 10;
        public int hitDie = 8;
        public int maximumHitPoints = 8;
        public int currentHitPoints = 8;
        public int maximumMana;
        public int currentMana;
        public int hunger = 100;
        public int rest = 100;
        public int happiness = 100;
        public bool isFallen;
        public int defeats;
        public string homeDepartment = "Programming";
        public List<string> proficientSaves =
            new List<string>();
        public List<string> statusEffects =
            new List<string>();
        public List<string> equippedEquipmentIds =
            new List<string>();
        public List<string> knownSpellIds =
            new List<string>();
        public string activeCompanionInstanceId = string.Empty;
        public List<DeverQuestCompanionState> companions =
            new List<DeverQuestCompanionState>();
        public List<DeverQuestInventoryEntry> inventory =
            new List<DeverQuestInventoryEntry>();

        public void Sanitize()
        {
            if (dataVersion < 6)
            {
                alignment = DeverQuestAlignment.TrueNeutral;
            }
            if (dataVersion < 2)
            {
                DeverQuestAdventurerService.ApplyClassFoundation(
                    this, characterClass, false);
            }
            if (dataVersion < 3)
            {
                agility = dexterity;
                stamina = constitution;
                luck = 10;
                homeDepartment =
                    DeverQuestAdventurerService
                        .DefaultDepartment(characterClass);
                maximumMana =
                    Math.Max(0,
                        DeverQuestRulesService.AbilityModifier(
                            intelligence) * 2 + level * 5);
                currentMana = maximumMana;
                hunger = 100;
                rest = 100;
                happiness = 100;
            }
            characterName = characterName?.Trim() ?? string.Empty;
            guildName = guildName?.Trim() ?? string.Empty;
            guildRank = guildRank?.Trim() ?? string.Empty;
            characterClass = characterClass?.Trim() ?? string.Empty;
            classId = classId?.Trim() ?? string.Empty;
            ancestryName = ancestryName?.Trim() ?? string.Empty;
            ancestryId = ancestryId?.Trim() ?? string.Empty;
            deityName = deityName?.Trim() ?? string.Empty;
            deityId = deityId?.Trim() ?? string.Empty;
            level = Math.Max(1, level);
            currentExperience = Math.Max(0L, currentExperience);
            lifetimeExperience = Math.Max(0L, lifetimeExperience);
            copperBalance = Math.Max(0L, copperBalance);
            if (dataVersion < 8)
            {
                DeverQuestAdventurerService.NormalizeCoinPurse(this);
            }
            platinumCoins = Math.Max(0L, platinumCoins);
            goldCoins = Math.Max(0L, goldCoins);
            silverCoins = Math.Max(0L, silverCoins);
            copperCoins = Math.Max(0L, copperCoins);
            totalCopperEarned = Math.Max(0L, totalCopperEarned);
            totalCopperSpent = Math.Max(0L, totalCopperSpent);
            strength = ClampAbility(strength);
            dexterity = ClampAbility(dexterity);
            constitution = ClampAbility(constitution);
            intelligence = ClampAbility(intelligence);
            wisdom = ClampAbility(wisdom);
            charisma = ClampAbility(charisma);
            agility = ClampAbility(agility);
            stamina = ClampAbility(stamina);
            luck = ClampAbility(luck);
            hitDie = Math.Max(4, hitDie);
            maximumHitPoints = Math.Max(1, maximumHitPoints);
            currentHitPoints =
                Math.Min(maximumHitPoints,
                    Math.Max(0, currentHitPoints));
            maximumMana = Math.Max(0, maximumMana);
            currentMana =
                Math.Min(maximumMana, Math.Max(0, currentMana));
            hunger = Math.Min(100, Math.Max(0, hunger));
            rest = Math.Min(100, Math.Max(0, rest));
            happiness = Math.Min(100, Math.Max(0, happiness));
            defeats = Math.Max(0, defeats);
            homeDepartment =
                homeDepartment?.Trim() ?? string.Empty;
            proficientSaves = proficientSaves ??
                               new List<string>();
            statusEffects = statusEffects ?? new List<string>();
            equippedEquipmentIds = equippedEquipmentIds ??
                                   new List<string>();
            knownSpellIds = knownSpellIds ?? new List<string>();
            activeCompanionInstanceId =
                activeCompanionInstanceId?.Trim() ?? string.Empty;
            companions = companions ??
                         new List<DeverQuestCompanionState>();
            companions.RemoveAll(value => value == null);
            foreach (DeverQuestCompanionState companion in companions)
            {
                companion.Sanitize();
            }
            if (!string.IsNullOrWhiteSpace(
                    activeCompanionInstanceId) &&
                !companions.Exists(value =>
                    value.instanceId ==
                    activeCompanionInstanceId &&
                    !value.isFallen))
            {
                activeCompanionInstanceId = string.Empty;
            }
            foreach (DeverQuestCompanionState companion in companions)
            {
                companion.isActive =
                    companion.instanceId ==
                    activeCompanionInstanceId &&
                    !companion.isFallen;
            }
            inventory = inventory ??
                        new List<DeverQuestInventoryEntry>();
            inventory.RemoveAll(item =>
                item == null || item.quantity <= 0);
            foreach (DeverQuestInventoryEntry item in inventory)
            {
                item.EnsureOwnership(string.Empty);
            }
            DeverQuestIdentityCatalogService.Migrate(this);
            DeverQuestAdventurerService.EnsureCoinPurseValue(this);
            dataVersion = 9;
        }

        private static int ClampAbility(int value)
        {
            return Math.Min(30, Math.Max(1, value));
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

        public static void ApplyClassFoundation(
            DeverQuestAdventurer target,
            string characterClass,
            bool resetVitals)
        {
            if (target == null)
            {
                return;
            }
            target.characterClass =
                string.IsNullOrWhiteSpace(characterClass)
                    ? "Warrior"
                    : characterClass;
            int[] scores;
            string[] saves;
            switch (target.characterClass)
            {
                case "Necromancer":
                case "Wizard":
                    scores = new[] { 8, 14, 14, 16, 12, 10 };
                    target.hitDie = 6;
                    saves = new[] { "Intelligence", "Wisdom" };
                    break;
                case "Sorcerer":
                case "Bard":
                    scores = new[] { 8, 14, 14, 10, 12, 16 };
                    target.hitDie = 6;
                    saves = new[] { "Constitution", "Charisma" };
                    break;
                case "Rogue":
                case "Ranger":
                    scores = new[] { 10, 16, 14, 12, 14, 8 };
                    target.hitDie = 8;
                    saves = new[] { "Dexterity", "Intelligence" };
                    break;
                case "Cleric":
                case "Druid":
                    scores = new[] { 10, 12, 14, 8, 16, 10 };
                    target.hitDie = 8;
                    saves = new[] { "Wisdom", "Charisma" };
                    break;
                case "Paladin":
                    scores = new[] { 16, 10, 14, 8, 10, 14 };
                    target.hitDie = 10;
                    saves = new[] { "Wisdom", "Charisma" };
                    break;
                case "Monk":
                    scores = new[] { 12, 16, 14, 8, 16, 8 };
                    target.hitDie = 8;
                    saves = new[] { "Strength", "Dexterity" };
                    break;
                default:
                    scores = new[] { 16, 12, 16, 8, 10, 10 };
                    target.hitDie = 10;
                    saves = new[] { "Strength", "Constitution" };
                    break;
            }
            target.strength = scores[0];
            target.dexterity = scores[1];
            target.constitution = scores[2];
            target.intelligence = scores[3];
            target.wisdom = scores[4];
            target.charisma = scores[5];
            target.agility = target.dexterity;
            target.stamina = target.constitution;
            target.luck = 10;
            target.homeDepartment =
                DefaultDepartment(target.characterClass);
            target.proficientSaves =
                new List<string>(saves);
            int constitutionModifier =
                DeverQuestRulesService.AbilityModifier(
                    target.constitution);
            int calculatedMaximum =
                Math.Max(1,
                    target.hitDie + constitutionModifier +
                    Math.Max(0, target.level - 1) *
                    (target.hitDie / 2 + 1 +
                     constitutionModifier));
            target.maximumHitPoints = calculatedMaximum;
            bool caster =
                target.characterClass == "Necromancer" ||
                target.characterClass == "Wizard" ||
                target.characterClass == "Sorcerer" ||
                target.characterClass == "Cleric" ||
                target.characterClass == "Druid" ||
                target.characterClass == "Bard" ||
                target.characterClass == "Paladin";
            target.maximumMana = caster
                ? Math.Max(1,
                    target.level * 5 +
                    DeverQuestRulesService.AbilityModifier(
                        target.intelligence) * 2)
                : 0;
            if (resetVitals || target.currentHitPoints <= 0)
            {
                target.currentHitPoints = calculatedMaximum;
                target.currentMana = target.maximumMana;
            }
        }

        public static string DefaultDepartment(string characterClass)
        {
            switch (characterClass)
            {
                case "Ranger":
                case "Rogue":
                case "Bard":
                    return "Art";
                case "Paladin":
                case "Cleric":
                    return "Design";
                case "Druid":
                    return "Audio";
                case "Warrior":
                case "Barbarian":
                case "Monk":
                    return "Quality Assurance";
                default:
                    return "Programming";
            }
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
            target.copperCoins += copper;
            target.totalCopperEarned += copper;
            target.currentExperience += experience;
            target.lifetimeExperience += experience;

            while (target.currentExperience >=
                   ExperienceForNextLevel(target.level))
            {
                target.currentExperience -=
                    ExperienceForNextLevel(target.level);
                target.level++;
                int constitutionModifier =
                    DeverQuestRulesService.AbilityModifier(
                        target.constitution);
                int hitPointGain = Math.Max(
                    1,
                    target.hitDie / 2 + 1 +
                    constitutionModifier);
                target.maximumHitPoints += hitPointGain;
                target.currentHitPoints += hitPointGain;
                int manaGain =
                    target.maximumMana > 0 ? 5 : 0;
                target.maximumMana += manaGain;
                target.currentMana += manaGain;
            }

            Save();
            if (target.level > startingLevel &&
                DeverQuestSettingsStore.Profile
                    .notificationSoundsEnabled)
            {
                DeverQuestAudioDirector.PlayCue(
                    DeverQuestAudioCue.LevelUp);
            }
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
            NormalizeCoinPurse(Adventurer);
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

        public static void ExchangeCoinAtGuildHall()
        {
            ExchangeCoinAtGuildHall(out _, out _);
        }

        public static bool ExchangeCoinAtGuildHall(
            out long piecesBefore,
            out long piecesAfter)
        {
            DeverQuestAdventurer target = Adventurer;
            piecesBefore =
                Math.Max(0L, target.platinumCoins) +
                Math.Max(0L, target.goldCoins) +
                Math.Max(0L, target.silverCoins) +
                Math.Max(0L, target.copperCoins);
            NormalizeCoinPurse(target);
            piecesAfter =
                Math.Max(0L, target.platinumCoins) +
                Math.Max(0L, target.goldCoins) +
                Math.Max(0L, target.silverCoins) +
                Math.Max(0L, target.copperCoins);
            Save();
            DeverQuestEconomyService.RecordDenominationExchange(
                piecesBefore, piecesAfter);
            return piecesAfter < piecesBefore;
        }

        public static long CoinPieceCount(
            DeverQuestAdventurer target = null)
        {
            target = target ?? Adventurer;
            EnsureCoinPurseValue(target);
            return target.platinumCoins + target.goldCoins +
                   target.silverCoins + target.copperCoins;
        }

        internal static void NormalizeCoinPurse(
            DeverQuestAdventurer target)
        {
            if (target == null)
            {
                return;
            }
            long total = Math.Max(0L, target.copperBalance);
            target.platinumCoins = total / 1000000L;
            total %= 1000000L;
            target.goldCoins = total / 10000L;
            total %= 10000L;
            target.silverCoins = total / 100L;
            target.copperCoins = total % 100L;
        }

        internal static void EnsureCoinPurseValue(
            DeverQuestAdventurer target)
        {
            if (target == null)
            {
                return;
            }
            long purseValue =
                Math.Max(0L, target.platinumCoins) * 1000000L +
                Math.Max(0L, target.goldCoins) * 10000L +
                Math.Max(0L, target.silverCoins) * 100L +
                Math.Max(0L, target.copperCoins);
            if (purseValue != Math.Max(0L, target.copperBalance))
            {
                NormalizeCoinPurse(target);
            }
        }

        public static void Save()
        {
            Adventurer.Sanitize();
            EditorPrefs.SetString(
                AdventurerKey,
                JsonUtility.ToJson(Adventurer));
            DeverQuestGuildAccountService.SyncFromAdventurer();
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
