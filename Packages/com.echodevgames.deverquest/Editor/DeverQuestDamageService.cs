using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestAttackProfile
    {
        public string DisplayName = "Guild Strike";
        public string DamageDice = "1d8";
        public DeverQuestDamageType DamageType =
            DeverQuestDamageType.Slashing;
    }

    internal sealed class DeverQuestDamageResolution
    {
        public DeverQuestDamageType DamageType;
        public DeverQuestDamageResponse Response;
        public int RawDamage;
        public int FinalDamage;
        public int AbsorbedHealing;

        public string Summary
        {
            get
            {
                string type = Friendly(DamageType);
                switch (Response)
                {
                    case DeverQuestDamageResponse.Vulnerable:
                        return $"{FinalDamage} {type} (vulnerable; " +
                               $"raw {RawDamage})";
                    case DeverQuestDamageResponse.Resistant:
                        return $"{FinalDamage} {type} (resisted; " +
                               $"raw {RawDamage})";
                    case DeverQuestDamageResponse.Immune:
                        return $"0 {type} (immune; raw {RawDamage})";
                    case DeverQuestDamageResponse.Absorbs:
                        return $"0 {type} (absorbed {AbsorbedHealing})";
                    default:
                        return $"{FinalDamage} {type}";
                }
            }
        }

        private static string Friendly(DeverQuestDamageType value)
        {
            return value.ToString().ToLowerInvariant();
        }
    }

    internal static class DeverQuestDamageService
    {
        public static DeverQuestDamageResolution Resolve(
            int rawDamage,
            DeverQuestDamageType damageType,
            IEnumerable<DeverQuestDamageAffinity> affinities)
        {
            int raw = Math.Max(0, rawDamage);
            bool vulnerable = false;
            bool resistant = false;
            bool immune = false;
            bool absorbs = false;
            foreach (DeverQuestDamageAffinity affinity in
                     affinities ??
                     Enumerable.Empty<DeverQuestDamageAffinity>())
            {
                if (affinity == null ||
                    affinity.damageType != damageType)
                {
                    continue;
                }
                switch (affinity.response)
                {
                    case DeverQuestDamageResponse.Vulnerable:
                        vulnerable = true;
                        break;
                    case DeverQuestDamageResponse.Resistant:
                        resistant = true;
                        break;
                    case DeverQuestDamageResponse.Immune:
                        immune = true;
                        break;
                    case DeverQuestDamageResponse.Absorbs:
                        absorbs = true;
                        break;
                }
            }

            DeverQuestDamageResponse response;
            int finalDamage;
            int healing = 0;
            if (absorbs)
            {
                response = DeverQuestDamageResponse.Absorbs;
                finalDamage = 0;
                healing = raw;
            }
            else if (immune)
            {
                response = DeverQuestDamageResponse.Immune;
                finalDamage = 0;
            }
            else if (resistant && !vulnerable)
            {
                response = DeverQuestDamageResponse.Resistant;
                finalDamage = raw == 0 ? 0 : Math.Max(1, (raw + 1) / 2);
            }
            else if (vulnerable && !resistant)
            {
                response = DeverQuestDamageResponse.Vulnerable;
                finalDamage = raw * 2;
            }
            else
            {
                response = DeverQuestDamageResponse.Normal;
                finalDamage = raw;
            }
            return new DeverQuestDamageResolution
            {
                DamageType = damageType,
                Response = response,
                RawDamage = raw,
                FinalDamage = finalDamage,
                AbsorbedHealing = healing
            };
        }

        public static DeverQuestAttackProfile AdventurerAttack(
            DeverQuestAdventurer adventurer)
        {
            DeverQuestEquipment weapon =
                DeverQuestRulesService.EquippedAssets(adventurer)
                    .Where(item =>
                        item.slot == DeverQuestEquipmentSlot.MainHand &&
                        !string.IsNullOrWhiteSpace(item.damageDice))
                    .OrderBy(item => item.displayName)
                    .FirstOrDefault();
            if (weapon != null)
            {
                return new DeverQuestAttackProfile
                {
                    DisplayName = weapon.displayName,
                    DamageDice = weapon.damageDice,
                    DamageType = weapon.weaponDamageType
                };
            }
            DeverQuestSpell spell =
                DeverQuestRulesService.KnownSpellAssets(adventurer)
                    .Where(item =>
                        !string.IsNullOrWhiteSpace(item.damageDice))
                    .OrderBy(item => item.spellLevel)
                    .ThenBy(item => item.displayName)
                    .FirstOrDefault();
            if (spell != null)
            {
                return new DeverQuestAttackProfile
                {
                    DisplayName = spell.displayName,
                    DamageDice = spell.damageDice,
                    DamageType = spell.damageType
                };
            }
            return new DeverQuestAttackProfile();
        }

        public static IEnumerable<DeverQuestDamageAffinity>
            AdventurerAffinities(DeverQuestAdventurer adventurer)
        {
            DeverQuestAncestry ancestry =
                DeverQuestIdentityCatalogService.FindAncestry(
                    adventurer.ancestryId,
                    adventurer.ancestryName);
            foreach (DeverQuestDamageAffinity affinity in
                     ancestry?.damageAffinities ??
                     new List<DeverQuestDamageAffinity>())
            {
                if (affinity != null)
                {
                    yield return affinity;
                }
            }
            foreach (DeverQuestEquipment equipment in
                     DeverQuestRulesService.EquippedAssets(adventurer))
            {
                foreach (DeverQuestDamageAffinity affinity in
                         equipment.damageAffinities ??
                         new List<DeverQuestDamageAffinity>())
                {
                    if (affinity != null)
                    {
                        yield return affinity;
                    }
                }
            }
        }

        public static string DescribeAdventurerAffinities(
            DeverQuestAdventurer adventurer)
        {
            List<DeverQuestDamageAffinity> affinities =
                AdventurerAffinities(adventurer).ToList();
            List<string> descriptions = new List<string>();
            foreach (DeverQuestDamageType type in
                     Enum.GetValues(typeof(DeverQuestDamageType)))
            {
                DeverQuestDamageResolution resolution =
                    Resolve(10, type, affinities);
                if (resolution.Response ==
                    DeverQuestDamageResponse.Normal)
                {
                    continue;
                }
                descriptions.Add(
                    $"{type}: {resolution.Response}");
            }
            return descriptions.Count == 0
                ? "None"
                : string.Join(", ", descriptions);
        }

        public static string DescribeBattle(
            IEnumerable<DeverQuestDamageEvent> events)
        {
            List<DeverQuestDamageEvent> values =
                (events ?? Enumerable.Empty<DeverQuestDamageEvent>())
                .Where(item => item != null)
                .ToList();
            if (values.Count == 0)
            {
                return "No typed damage recorded.";
            }
            return string.Join(
                " · ",
                values.GroupBy(item => item.damageType)
                    .OrderBy(group => group.Key.ToString())
                    .Select(group =>
                        $"{group.Key}: " +
                        $"{group.Sum(item => item.finalDamage)} dealt"));
        }
    }
}
