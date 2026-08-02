using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestCombatSummaryService
    {
        public static string OutcomeTitle(
            DeverQuestBattleResult battle)
        {
            if (battle == null)
            {
                return "No Result";
            }
            if (battle.safetyPaused)
            {
                return "Safety Pause";
            }
            if (!battle.victory)
            {
                return "Defeat";
            }
            return battle.earlyVictory
                ? "Early Victory"
                : "Victory";
        }

        public static string OutcomeSummary(
            DeverQuestBattleResult battle)
        {
            if (battle == null)
            {
                return "No battle result is available.";
            }
            string hp = $"HP {battle.startingHitPoints} → " +
                        $"{battle.endingHitPoints}";
            string rounds = battle.parRounds > 0
                ? $"{battle.rounds} rounds against par " +
                  $"{battle.parRounds}"
                : $"{battle.rounds} rounds";
            string foes = battle.defeatedMonsters.Count == 0
                ? "no foes defeated"
                : $"{battle.defeatedMonsters.Count} foe" +
                  (battle.defeatedMonsters.Count == 1
                      ? " defeated"
                      : "s defeated");
            string rewards =
                $"{DeverQuestAdventurerService.FormatCoins(battle.bonusCopper)} " +
                $"+ {battle.bonusExperience} XP";
            if (battle.safetyPaused)
            {
                return $"Paused safely after {rounds}; {hp}. " +
                       (string.IsNullOrWhiteSpace(
                            battle.safetyPauseReason)
                           ? "The expedition awaits a safe decision."
                           : battle.safetyPauseReason);
            }
            if (!battle.victory)
            {
                return $"Defeat after {rounds}; {hp}; {foes}. " +
                       (string.IsNullOrWhiteSpace(battle.injury)
                           ? "No lasting consequence was recorded."
                           : battle.injury);
            }
            return $"{(battle.earlyVictory ? "Early victory" : "Victory")} " +
                   $"in {rounds}; {foes}; {hp}; " +
                   $"battle rewards {rewards}.";
        }

        public static string DamageSummary(
            DeverQuestBattleResult battle,
            string developerName,
            string adventurerName)
        {
            if (battle?.damageEvents == null ||
                battle.damageEvents.Count == 0)
            {
                return string.IsNullOrWhiteSpace(
                    battle?.typedDamageSummary)
                    ? "No typed damage events were recorded."
                    : battle.typedDamageSummary;
            }
            string hero = string.IsNullOrWhiteSpace(adventurerName)
                ? InferAdventurerName(battle, developerName)
                : adventurerName;
            int heroDamage = SumDamageBySource(
                battle,
                hero);
            int companionDamage = SumDamageBySource(
                battle,
                battle.companionName);
            int heroTaken = SumDamageToTarget(battle, hero);
            int companionTaken = SumDamageToTarget(
                battle,
                battle.companionName);
            int resisted = CountResponse(
                battle,
                DeverQuestDamageResponse.Resistant);
            int vulnerable = CountResponse(
                battle,
                DeverQuestDamageResponse.Vulnerable);
            int immune = CountResponse(
                battle,
                DeverQuestDamageResponse.Immune);
            int absorbed = CountResponse(
                battle,
                DeverQuestDamageResponse.Absorbs);

            List<string> pieces = new List<string>
            {
                $"Adventurer dealt {heroDamage}",
                $"Adventurer took {heroTaken}"
            };
            if (!string.IsNullOrWhiteSpace(battle.companionName))
            {
                pieces.Add($"{battle.companionName} dealt " +
                           companionDamage);
                pieces.Add($"{battle.companionName} took " +
                           companionTaken);
            }
            List<string> reactions = new List<string>();
            if (resisted > 0)
            {
                reactions.Add($"{resisted} resisted");
            }
            if (vulnerable > 0)
            {
                reactions.Add($"{vulnerable} vulnerable");
            }
            if (immune > 0)
            {
                reactions.Add($"{immune} immune");
            }
            if (absorbed > 0)
            {
                reactions.Add($"{absorbed} absorbed");
            }
            if (reactions.Count > 0)
            {
                pieces.Add("reactions: " + string.Join(", ", reactions));
            }
            return string.Join(" · ", pieces);
        }

        public static string CompanionContributionSummary(
            DeverQuestBattleResult battle)
        {
            if (battle == null ||
                string.IsNullOrWhiteSpace(battle.companionName))
            {
                return string.Empty;
            }
            battle.damageEvents = battle.damageEvents ??
                                  new List<DeverQuestDamageEvent>();
            battle.combatLog = battle.combatLog ?? new List<string>();
            int damage = SumDamageBySource(
                battle,
                battle.companionName);
            int taken = SumDamageToTarget(
                battle,
                battle.companionName);
            int hits = battle.damageEvents.Count(value =>
                value != null &&
                Same(value.source, battle.companionName) &&
                value.finalDamage > 0);
            int misses = battle.combatLog.Count(value =>
                Contains(value, battle.companionName + " misses"));
            int healing = CompanionHealing(battle);
            string growth = battle.companionExperienceEarned > 0
                ? $" · +{battle.companionExperienceEarned} XP"
                : string.Empty;
            string level = battle.companionLevelAfter >
                           battle.companionLevelBefore
                ? $" · level {battle.companionLevelBefore} → " +
                  $"{battle.companionLevelAfter}"
                : string.Empty;
            string fall = battle.companionFell
                ? " · fell in battle"
                : string.Empty;
            return $"{battle.companionName}: {damage} damage, " +
                   $"{healing} healing, {taken} damage taken, " +
                   $"{hits} hit" + (hits == 1 ? string.Empty : "s") +
                   $", {misses} miss" +
                   (misses == 1 ? string.Empty : "es") +
                   growth + level + fall + ".";
        }

        public static string ConditionSummary(
            DeverQuestBattleResult battle)
        {
            if (battle == null)
            {
                return string.Empty;
            }
            HashSet<string> conditions =
                new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DeverQuestCombatActionEvent action in
                     battle.actionEvents ??
                     new List<DeverQuestCombatActionEvent>())
            {
                foreach (string effect in action?.effects ??
                         new List<string>())
                {
                    if (string.IsNullOrWhiteSpace(effect))
                    {
                        continue;
                    }
                    foreach (DeverQuestCombatEffectType type in
                             Enum.GetValues(
                                 typeof(DeverQuestCombatEffectType)))
                    {
                        if (!IsCondition(type) ||
                            effect.IndexOf(
                                type.ToString(),
                                StringComparison.OrdinalIgnoreCase) < 0)
                        {
                            continue;
                        }
                        conditions.Add(Friendly(type.ToString()));
                    }
                    if (effect.IndexOf(
                            "resisted by save",
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        conditions.Add("saving throw resisted an effect");
                    }
                }
            }
            foreach (string line in battle.combatLog ??
                     new List<string>())
            {
                if (Contains(line, "loses its turn"))
                {
                    conditions.Add("turn denied");
                }
                if (Contains(line, "falls and must recover"))
                {
                    conditions.Add("Companion fell");
                }
            }
            if (!string.IsNullOrWhiteSpace(battle.injury))
            {
                conditions.Add(battle.injury);
            }
            return conditions.Count == 0
                ? string.Empty
                : string.Join(", ", conditions.OrderBy(value => value));
        }

        public static IReadOnlyList<string> Highlights(
            DeverQuestBattleResult battle,
            int maximum)
        {
            if (battle == null || maximum <= 0)
            {
                return Array.Empty<string>();
            }
            List<string> source = (battle.combatLog ??
                                  new List<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .ToList();
            if (source.Count <= maximum)
            {
                return source;
            }
            List<string> selected = new List<string>();
            string opening = source.FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(opening))
            {
                selected.Add(opening);
            }
            int remaining = maximum - selected.Count;
            foreach (string line in source
                         .Skip(Math.Max(0, source.Count - remaining)))
            {
                if (!selected.Contains(line))
                {
                    selected.Add(line);
                }
            }
            return selected.Take(maximum).ToList();
        }

        public static string GroupedDefeatedMonsters(
            DeverQuestBattleResult battle)
        {
            if (battle?.defeatedMonsters == null ||
                battle.defeatedMonsters.Count == 0)
            {
                return "None";
            }
            return string.Join(
                ", ",
                battle.defeatedMonsters
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .GroupBy(value => value.Trim())
                    .OrderBy(value => value.Key)
                    .Select(value =>
                        value.Count() == 1
                            ? value.Key
                            : $"{value.Key} ×{value.Count()}"));
        }

        public static string BuildFullCombatReport(
            DeverQuestBattleResult battle)
        {
            if (battle == null)
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                $"{OutcomeTitle(battle)} — {battle.encounterName}");
            builder.AppendLine(OutcomeSummary(battle));
            builder.AppendLine(
                DamageSummary(battle, string.Empty, string.Empty));
            string companion = CompanionContributionSummary(battle);
            if (!string.IsNullOrWhiteSpace(companion))
            {
                builder.AppendLine(companion);
            }
            string conditions = ConditionSummary(battle);
            if (!string.IsNullOrWhiteSpace(conditions))
            {
                builder.AppendLine("Conditions: " + conditions);
            }
            builder.AppendLine();
            foreach (string line in battle.combatLog ??
                     new List<string>())
            {
                builder.AppendLine(line);
            }
            return builder.ToString().Trim();
        }

        public static void RecordCompanionBattle(
            DeverQuestCompanionState companion,
            DeverQuestBattleResult battle)
        {
            if (companion == null || battle == null ||
                string.IsNullOrWhiteSpace(battle.companionName))
            {
                return;
            }
            companion.lifetimeDamageDealt += SumDamageBySource(
                battle,
                battle.companionName);
            companion.lifetimeDamageTaken += SumDamageToTarget(
                battle,
                battle.companionName);
            companion.lifetimeHealingDone += CompanionHealing(battle);
            companion.lastBattleSummary =
                CompanionContributionSummary(battle);
            companion.lastBattleUtc = DateTime.UtcNow.ToString("O");
            companion.Sanitize();
        }

        public static string FriendlyExitMethod(string method)
        {
            string value = method?.Trim() ?? string.Empty;
            switch (value)
            {
                case "Return":
                    return "Homeward Passage";
                case "Wagon":
                    return "Guild Wagon";
                case "Flee":
                    return "Flee attempt";
                default:
                    return string.IsNullOrWhiteSpace(value)
                        ? "safe return"
                        : value;
            }
        }


        private static string InferAdventurerName(
            DeverQuestBattleResult battle,
            string fallback)
        {
            string actionActor = (battle.actionEvents ??
                                  new List<DeverQuestCombatActionEvent>())
                .Where(value => value != null &&
                                !string.IsNullOrWhiteSpace(value.actor) &&
                                !Same(value.actor, battle.companionName))
                .Select(value => value.actor.Trim())
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(actionActor))
            {
                return actionActor;
            }
            HashSet<string> defeated = new HashSet<string>(
                (battle.defeatedMonsters ?? new List<string>())
                    .Where(value => !string.IsNullOrWhiteSpace(value))
                    .Select(value => value.Replace(" [Boss]", string.Empty)
                        .Trim()),
                StringComparer.OrdinalIgnoreCase);
            string damageSource = (battle.damageEvents ??
                                   new List<DeverQuestDamageEvent>())
                .Where(value => value != null &&
                                !string.IsNullOrWhiteSpace(value.source) &&
                                !Same(value.source, battle.companionName) &&
                                defeated.Contains(
                                    value.target?.Trim() ?? string.Empty))
                .Select(value => value.source.Trim())
                .FirstOrDefault();
            if (!string.IsNullOrWhiteSpace(damageSource))
            {
                return damageSource;
            }
            return fallback?.Trim() ?? string.Empty;
        }

        private static int SumDamageBySource(
            DeverQuestBattleResult battle,
            string source)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return 0;
            }
            return battle.damageEvents
                .Where(value => value != null &&
                                Same(value.source, source))
                .Sum(value => Math.Max(0, value.finalDamage));
        }

        private static int SumDamageToTarget(
            DeverQuestBattleResult battle,
            string target)
        {
            if (string.IsNullOrWhiteSpace(target))
            {
                return 0;
            }
            return battle.damageEvents
                .Where(value => value != null &&
                                Same(value.target, target))
                .Sum(value => Math.Max(0, value.finalDamage));
        }

        private static int CountResponse(
            DeverQuestBattleResult battle,
            DeverQuestDamageResponse response)
        {
            return battle.damageEvents.Count(value =>
                value != null && value.response == response);
        }

        private static int CompanionHealing(
            DeverQuestBattleResult battle)
        {
            if (battle == null ||
                string.IsNullOrWhiteSpace(battle.companionName))
            {
                return 0;
            }
            int total = 0;
            foreach (string line in battle.combatLog ??
                     new List<string>())
            {
                if (!Contains(line, battle.companionName + " restores"))
                {
                    continue;
                }
                string marker = " restores ";
                int markerIndex = line.IndexOf(
                    marker,
                    StringComparison.OrdinalIgnoreCase);
                if (markerIndex < 0)
                {
                    continue;
                }
                string tail = line.Substring(
                    markerIndex + marker.Length);
                string digits = new string(
                    tail.TakeWhile(char.IsDigit).ToArray());
                if (int.TryParse(digits, out int healing))
                {
                    total += Math.Max(0, healing);
                }
            }
            return total;
        }

        private static bool Same(string left, string right)
        {
            return string.Equals(
                left?.Trim(),
                right?.Trim(),
                StringComparison.OrdinalIgnoreCase);
        }

        private static bool Contains(string source, string value)
        {
            return !string.IsNullOrWhiteSpace(source) &&
                   !string.IsNullOrWhiteSpace(value) &&
                   source.IndexOf(
                       value,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsCondition(
            DeverQuestCombatEffectType type)
        {
            return type != DeverQuestCombatEffectType.DirectDamage &&
                   type != DeverQuestCombatEffectType.Heal &&
                   type != DeverQuestCombatEffectType.ManaRestore &&
                   type != DeverQuestCombatEffectType.ReturnToGuild;
        }

        private static string Friendly(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }
            StringBuilder builder = new StringBuilder();
            foreach (char character in value)
            {
                if (builder.Length > 0 && char.IsUpper(character))
                {
                    builder.Append(' ');
                }
                builder.Append(character);
            }
            return builder.ToString();
        }
    }
}
