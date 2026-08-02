using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestRuleResult
    {
        public string Seed = string.Empty;
        public int RawRoll;
        public int AbilityModifier;
        public int ProficiencyBonus;
        public int DecreeModifier;
        public int Total;
        public int DifficultyClass;
        public bool Success;
        public string Formula = string.Empty;
    }

    [InitializeOnLoad]
    internal static class DeverQuestRulesService
    {
        private static Dictionary<string, DeverQuestEquipment>
            equipmentCache;
        private static Dictionary<string, DeverQuestSpell>
            spellCache;

        static DeverQuestRulesService()
        {
            EditorApplication.projectChanged -= ClearAssetCaches;
            EditorApplication.projectChanged += ClearAssetCaches;
        }

        public static int AbilityModifier(int score)
        {
            return (int)Math.Floor((score - 10) / 2d);
        }

        public static int ProficiencyBonus(int level)
        {
            return 2 + Math.Max(0, level - 1) / 4;
        }

        public static int GetAbilityScore(
            DeverQuestAdventurer character,
            DeverQuestAbility ability)
        {
            int score;
            switch (ability)
            {
                case DeverQuestAbility.Strength:
                    score = character.strength;
                    break;
                case DeverQuestAbility.Dexterity:
                    score = character.dexterity;
                    break;
                case DeverQuestAbility.Constitution:
                    score = character.constitution;
                    break;
                case DeverQuestAbility.Intelligence:
                    score = character.intelligence;
                    break;
                case DeverQuestAbility.Wisdom:
                    score = character.wisdom;
                    break;
                case DeverQuestAbility.Agility:
                    score = character.agility;
                    break;
                case DeverQuestAbility.Stamina:
                    score = character.stamina;
                    break;
                case DeverQuestAbility.Luck:
                    score = character.luck;
                    break;
                default:
                    score = character.charisma;
                    break;
            }
            foreach (DeverQuestEquipment equipment in EquippedAssets(
                         character))
            {
                if (equipment.abilityBonusType == ability)
                {
                    score += equipment.abilityBonus;
                }
            }
            return score;
        }

        public static int ArmorClass(DeverQuestAdventurer character)
        {
            DeverQuestAncestry ancestry =
                DeverQuestIdentityCatalogService.FindAncestry(
                    character.ancestryId,
                    character.ancestryName);
            return 10 +
                   AbilityModifier(GetAbilityScore(
                       character, DeverQuestAbility.Dexterity)) +
                   (ancestry?.naturalArmorBonus ?? 0) +
                   EquippedAssets(character).Sum(
                       item => item.armorClassBonus);
        }

        public static void Equip(
            DeverQuestAdventurer character,
            DeverQuestEquipment equipment)
        {
            if (character == null || equipment == null)
            {
                return;
            }
            character.equippedEquipmentIds.RemoveAll(
                id =>
                {
                    DeverQuestEquipment existing =
                        FindEquipment(id);
                    return existing == null ||
                           existing.slot == equipment.slot;
                });
            character.equippedEquipmentIds.Add(equipment.EquipmentId);
        }

        public static List<string> EquippedNames(
            DeverQuestAdventurer character)
        {
            return EquippedAssets(character)
                .Select(item => item.displayName)
                .ToList();
        }

        public static List<string> KnownSpellNames(
            DeverQuestAdventurer character)
        {
            HashSet<string> ids = new HashSet<string>(
                character.knownSpellIds ??
                new List<string>());
            List<string> names = new List<string>();
            EnsureAssetCaches();
            foreach (DeverQuestSpell spell in spellCache.Values)
            {
                if (spell != null && ids.Contains(spell.SpellId))
                {
                    names.Add(spell.displayName);
                }
            }
            return names;
        }

        public static List<string> ClassFeatures(
            DeverQuestAdventurer character)
        {
            List<string> features = new List<string>();
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.FindClass(
                    character.classId,
                    character.characterClass);
            if ((classDefinition?.classFeatures?.Count ?? 0) > 0)
            {
                features.AddRange(
                    classDefinition.classFeatures);
            }
            else
            {
                switch (character.characterClass)
                {
                    case "Necromancer":
                        features.Add("Grave Magic");
                        features.Add("Life Siphon");
                        break;
                    case "Wizard":
                    case "Sorcerer":
                        features.Add("Spellcasting");
                        features.Add("Arcane Tradition");
                        break;
                    case "Cleric":
                    case "Druid":
                        features.Add("Divine or Primal Magic");
                        break;
                    case "Rogue":
                        features.Add("Precision Strike");
                        features.Add("Expertise");
                        break;
                    case "Ranger":
                        features.Add("Wilderness Training");
                        break;
                    case "Paladin":
                        features.Add("Sacred Oath");
                        features.Add("Lay on Hands");
                        break;
                    default:
                        features.Add("Martial Training");
                        break;
                }
            }
            if (character.level >= 2)
            {
                features.Add("Class Path");
            }
            if (character.level >= 4)
            {
                features.Add("Ability Advancement");
            }
            if (character.level >= 5)
            {
                features.Add("Veteran Feature");
            }
            return features;
        }

        public static DeverQuestRuleResult ResolveCheck(
            DeverQuestAdventurer character,
            DeverQuestAbility ability,
            bool proficient,
            int difficultyClass,
            string seed,
            int decreeModifier)
        {
            uint state = StableSeed(seed);
            int raw = 1 + (int)(Next(ref state) % 20u);
            int abilityModifier = AbilityModifier(
                GetAbilityScore(character, ability));
            int proficiency = proficient
                ? ProficiencyBonus(character.level)
                : 0;
            int total =
                raw + abilityModifier + proficiency + decreeModifier;
            return new DeverQuestRuleResult
            {
                Seed = seed ?? string.Empty,
                RawRoll = raw,
                AbilityModifier = abilityModifier,
                ProficiencyBonus = proficiency,
                DecreeModifier = decreeModifier,
                Total = total,
                DifficultyClass = Math.Max(1, difficultyClass),
                Success = total >= Math.Max(1, difficultyClass),
                Formula =
                    $"{raw} + {abilityModifier} ability + " +
                    $"{proficiency} proficiency + " +
                    $"{decreeModifier} decree = {total}"
            };
        }

        public static int RollDice(
            string dice,
            string seed,
            out string formula)
        {
            formula = string.Empty;
            if (!TryParseDice(
                    dice, out int count, out int sides, out int bonus))
            {
                formula = "Invalid dice expression.";
                return 0;
            }
            uint state = StableSeed(seed);
            List<int> rolls = new List<int>();
            for (int index = 0; index < count; index++)
            {
                rolls.Add(1 + (int)(Next(ref state) % (uint)sides));
            }
            int total = rolls.Sum() + bonus;
            formula =
                $"{string.Join(" + ", rolls)}" +
                (bonus == 0 ? string.Empty : $" + {bonus}") +
                $" = {total}";
            return total;
        }

        internal static IEnumerable<DeverQuestEquipment> EquippedAssets(
            DeverQuestAdventurer character)
        {
            if (character.equippedEquipmentIds == null)
            {
                yield break;
            }
            HashSet<string> ids =
                new HashSet<string>(
                    character.equippedEquipmentIds);
            EnsureAssetCaches();
            foreach (string id in ids)
            {
                if (equipmentCache.TryGetValue(
                        id,
                        out DeverQuestEquipment item) &&
                    item != null)
                {
                    yield return item;
                }
            }
        }

        internal static DeverQuestEquipment FindEquipment(string id)
        {
            EnsureAssetCaches();
            return equipmentCache.TryGetValue(
                id ?? string.Empty,
                out DeverQuestEquipment item)
                ? item
                : null;
        }

        internal static IEnumerable<DeverQuestSpell> KnownSpellAssets(
            DeverQuestAdventurer character)
        {
            if (character?.knownSpellIds == null)
            {
                yield break;
            }
            HashSet<string> ids =
                new HashSet<string>(character.knownSpellIds);
            EnsureAssetCaches();
            foreach (string id in ids)
            {
                if (spellCache.TryGetValue(
                        id,
                        out DeverQuestSpell spell) &&
                    spell != null)
                {
                    yield return spell;
                }
            }
        }

        private static void EnsureAssetCaches()
        {
            if (equipmentCache == null)
            {
                equipmentCache =
                    new Dictionary<string, DeverQuestEquipment>();
                foreach (string guid in AssetDatabase.FindAssets(
                             "t:DeverQuestEquipment"))
                {
                    DeverQuestEquipment equipment =
                        AssetDatabase.LoadAssetAtPath<
                            DeverQuestEquipment>(
                            AssetDatabase.GUIDToAssetPath(guid));
                    if (equipment != null)
                    {
                        equipmentCache[equipment.EquipmentId] =
                            equipment;
                    }
                }
            }
            if (spellCache == null)
            {
                spellCache =
                    new Dictionary<string, DeverQuestSpell>();
                foreach (string guid in AssetDatabase.FindAssets(
                             "t:DeverQuestSpell"))
                {
                    DeverQuestSpell spell =
                        AssetDatabase.LoadAssetAtPath<
                            DeverQuestSpell>(
                            AssetDatabase.GUIDToAssetPath(guid));
                    if (spell != null)
                    {
                        spellCache[spell.SpellId] = spell;
                    }
                }
            }
        }

        private static void ClearAssetCaches()
        {
            equipmentCache = null;
            spellCache = null;
        }

        private static bool TryParseDice(
            string dice,
            out int count,
            out int sides,
            out int bonus)
        {
            count = 0;
            sides = 0;
            bonus = 0;
            string value = (dice ?? string.Empty)
                .Trim().ToLowerInvariant().Replace(" ", string.Empty);
            int d = value.IndexOf('d');
            if (d <= 0)
            {
                return false;
            }
            int sign = value.IndexOf('+', d + 1);
            if (sign < 0)
            {
                sign = value.IndexOf('-', d + 1);
            }
            string sidesText = sign < 0
                ? value.Substring(d + 1)
                : value.Substring(d + 1, sign - d - 1);
            if (!int.TryParse(value.Substring(0, d),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out count) ||
                !int.TryParse(sidesText,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out sides))
            {
                return false;
            }
            if (sign >= 0 &&
                !int.TryParse(value.Substring(sign),
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture, out bonus))
            {
                return false;
            }
            count = Math.Min(100, Math.Max(1, count));
            sides = Math.Min(1000, Math.Max(2, sides));
            return true;
        }

        private static uint StableSeed(string value)
        {
            uint hash = 2166136261u;
            foreach (char character in value ?? string.Empty)
            {
                hash ^= character;
                hash *= 16777619u;
            }
            return hash == 0u ? 1u : hash;
        }

        private static uint Next(ref uint state)
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return state;
        }
    }
}
