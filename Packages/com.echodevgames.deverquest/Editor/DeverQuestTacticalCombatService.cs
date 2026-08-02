using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestActiveCombatEffect
    {
        public DeverQuestCombatEffectType EffectType;
        public string Source = string.Empty;
        public string Dice = string.Empty;
        public int Amount;
        public int RemainingRounds;
        public DeverQuestDamageType DamageType;
        public bool BreaksOnDamage;
    }

    internal sealed class DeverQuestCombatState
    {
        public readonly List<DeverQuestActiveCombatEffect> Effects =
            new List<DeverQuestActiveCombatEffect>();
        public readonly Dictionary<string, int> Cooldowns =
            new Dictionary<string, int>();

        public bool Has(DeverQuestCombatEffectType effect)
        {
            return Effects.Any(value =>
                value.EffectType == effect &&
                value.RemainingRounds > 0);
        }

        public int Total(DeverQuestCombatEffectType effect)
        {
            return Effects
                .Where(value =>
                    value.EffectType == effect &&
                    value.RemainingRounds > 0)
                .Sum(value => Math.Max(1, value.Amount));
        }
    }

    internal static class DeverQuestTacticalCombatService
    {
        public static bool TryResolveAdventurerTurn(
            DeverQuestAdventurer adventurer,
            DeverQuestMonsterProfile monster,
            int decreeModifier,
            string seed,
            int round,
            ref int index,
            ref int monsterHitPoints,
            DeverQuestCombatState adventurerState,
            DeverQuestCombatState monsterState,
            DeverQuestBattleResult result)
        {
            DeverQuestAbilitySlot selected =
                SelectAbility(
                    adventurer,
                    adventurerState,
                    monsterState);
            if (selected == null)
            {
                return false;
            }
            DeverQuestSpell spell = selected.spell;
            DeverQuestAttackTechnique technique =
                selected.technique;
            string actionName =
                spell == null
                    ? technique.displayName
                    : spell.displayName;
            int manaCost =
                spell == null ? technique.manaCost : spell.manaCost;
            if (manaCost > adventurer.currentMana)
            {
                return false;
            }
            List<DeverQuestCombatEffect> effects =
                Effects(spell, technique);
            bool hostile = effects.Any(IsHostile);
            DeverQuestAbility ability =
                spell == null
                    ? technique.attackAbility
                    : spell.castingAbility;
            if (hostile)
            {
                index++;
                int armorPenalty =
                    monsterState.Total(
                        DeverQuestCombatEffectType.ArmorDebuff);
                int armorBonus =
                    monsterState.Total(
                        DeverQuestCombatEffectType.ArmorBuff);
                DeverQuestRuleResult attack =
                    DeverQuestRulesService.ResolveCheck(
                        adventurer,
                        ability,
                        true,
                        Math.Max(
                            1,
                            monster.armorClass -
                            armorPenalty +
                            armorBonus),
                        seed + ":tactical:" + index,
                        decreeModifier);
                if (!attack.Success)
                {
                    RecordAction(
                        result,
                        round,
                        adventurer.characterName,
                        actionName,
                        monster.displayName,
                        manaCost,
                        "Miss");
                    AddCombatLine(
                        result,
                        $"Round {round}: {adventurer.characterName} " +
                        $"uses {actionName}, but {monster.displayName} " +
                        "resists the attempt.");
                    adventurer.currentMana -= manaCost;
                    return true;
                }
            }

            adventurer.currentMana -= manaCost;
            int cooldown =
                spell == null
                    ? technique.cooldownRounds
                    : spell.cooldownRounds;
            if (cooldown > 0)
            {
                adventurerState.Cooldowns[
                    ActionKey(spell, technique)] = cooldown + 1;
            }
            List<string> applied = new List<string>();
            foreach (DeverQuestCombatEffect effect in effects)
            {
                if (effect == null)
                {
                    continue;
                }
                index++;
                ApplyEffect(
                    effect,
                    adventurer,
                    monster,
                    actionName,
                    seed + ":effect:" + index,
                    round,
                    ref monsterHitPoints,
                    adventurerState,
                    monsterState,
                    result,
                    applied);
            }
            RecordAction(
                result,
                round,
                adventurer.characterName,
                actionName,
                monster.displayName,
                manaCost,
                string.Join(", ", applied));
            AddCombatLine(
                result,
                $"Round {round}: {adventurer.characterName} uses " +
                $"{actionName}" +
                (applied.Count == 0
                    ? "."
                    : $" — {string.Join("; ", applied)}."));
            return true;
        }

        public static void TickStartOfRound(
            DeverQuestAdventurer adventurer,
            DeverQuestMonsterProfile monster,
            string seed,
            int round,
            ref int monsterHitPoints,
            DeverQuestCombatState adventurerState,
            DeverQuestCombatState monsterState,
            DeverQuestBattleResult result)
        {
            TickState(
                monsterState,
                false,
                adventurer,
                monster,
                seed + ":monster-tick",
                round,
                ref monsterHitPoints,
                result);
            TickCooldowns(adventurerState);
            TickCooldowns(monsterState);
            TickState(
                adventurerState,
                true,
                adventurer,
                monster,
                seed + ":hero-tick",
                round,
                ref monsterHitPoints,
                result);
        }

        public static int AdventurerAttackPenalty(
            DeverQuestCombatState state)
        {
            return state.Total(
                       DeverQuestCombatEffectType.AttackDebuff) +
                   (state.Has(DeverQuestCombatEffectType.Snare) ? 2 : 0) +
                   (state.Has(DeverQuestCombatEffectType.Root) ? 2 : 0) -
                   state.Total(
                       DeverQuestCombatEffectType.AttackBuff);
        }

        public static int MonsterAttackPenalty(
            DeverQuestCombatState state)
        {
            return state.Total(
                       DeverQuestCombatEffectType.AttackDebuff) +
                   (state.Has(DeverQuestCombatEffectType.Snare) ? 2 : 0) +
                   (state.Has(DeverQuestCombatEffectType.Root) ? 4 : 0) -
                   state.Total(
                       DeverQuestCombatEffectType.AttackBuff);
        }

        public static bool SkipsTurn(DeverQuestCombatState state)
        {
            return state.Has(DeverQuestCombatEffectType.Stun);
        }

        public static int AbsorbWithShield(
            DeverQuestCombatState state,
            int damage)
        {
            DeverQuestActiveCombatEffect shield =
                state.Effects.FirstOrDefault(value =>
                    value.EffectType ==
                    DeverQuestCombatEffectType.Shield &&
                    value.RemainingRounds > 0 &&
                    value.Amount > 0);
            if (shield == null)
            {
                return damage;
            }
            int absorbed = Math.Min(shield.Amount, Math.Max(0, damage));
            shield.Amount -= absorbed;
            return Math.Max(0, damage - absorbed);
        }

        public static void ApplyMonsterOnHit(
            DeverQuestMonsterProfile monster,
            DeverQuestAdventurer adventurer,
            string seed,
            int round,
            DeverQuestCombatState adventurerState,
            DeverQuestBattleResult result)
        {
            DeverQuestAbilitySlot slot =
                monster?.abilityProfile?.abilities?
                    .Where(value => value != null)
                    .OrderByDescending(value => value.priority)
                    .FirstOrDefault();
            List<DeverQuestCombatEffect> effects =
                Effects(slot?.spell, slot?.technique);
            foreach (DeverQuestCombatEffect effect in effects)
            {
                if (effect == null ||
                    effect.effectType ==
                    DeverQuestCombatEffectType.DirectDamage ||
                    effect.effectType ==
                    DeverQuestCombatEffectType.LifeDrain ||
                    effect.effectType ==
                    DeverQuestCombatEffectType.Heal)
                {
                    continue;
                }
                AddCondition(
                    adventurerState,
                    effect,
                    slot.spell == null
                        ? slot.technique?.displayName
                        : slot.spell.displayName);
                RecordAction(
                    result,
                    round,
                    monster.displayName,
                    slot.spell == null
                        ? slot.technique?.displayName
                        : slot.spell.displayName,
                    adventurer.characterName,
                    0,
                    effect.effectType.ToString());
            }
        }

        public static bool HasReturnAbility(
            DeverQuestAdventurer adventurer)
        {
            return CandidateAbilities(adventurer).Any(slot =>
                Effects(slot.spell, slot.technique).Any(effect =>
                    effect != null &&
                    effect.effectType ==
                    DeverQuestCombatEffectType.ReturnToGuild));
        }

        private static DeverQuestAbilitySlot SelectAbility(
            DeverQuestAdventurer adventurer,
            DeverQuestCombatState adventurerState,
            DeverQuestCombatState monsterState)
        {
            int hitPointPercent =
                adventurer.maximumHitPoints <= 0
                    ? 0
                    : adventurer.currentHitPoints * 100 /
                      adventurer.maximumHitPoints;
            return CandidateAbilities(adventurer)
                .Where(slot =>
                    slot != null &&
                    !adventurerState.Cooldowns.ContainsKey(
                        ActionKey(slot.spell, slot.technique)) &&
                    (!adventurerState.Has(
                         DeverQuestCombatEffectType.Silence) ||
                     slot.spell == null) &&
                    (slot.spell == null ||
                     slot.spell.manaCost <=
                     adventurer.currentMana) &&
                    (slot.technique == null ||
                     slot.technique.manaCost <=
                     adventurer.currentMana))
                .Select(slot => new
                {
                    Slot = slot,
                    Score = Score(
                        slot,
                        hitPointPercent,
                        monsterState)
                })
                .Where(value => value.Score >= 0)
                .OrderByDescending(value => value.Score)
                .ThenBy(value =>
                    value.Slot.spell == null
                        ? value.Slot.technique?.displayName
                        : value.Slot.spell.displayName)
                .Select(value => value.Slot)
                .FirstOrDefault();
        }

        private static IEnumerable<DeverQuestAbilitySlot>
            CandidateAbilities(DeverQuestAdventurer adventurer)
        {
            HashSet<string> known =
                new HashSet<string>(
                    adventurer.knownSpellIds ??
                    new List<string>());
            DeverQuestClassDefinition definition =
                DeverQuestIdentityCatalogService.FindClass(
                    adventurer.classId,
                    adventurer.characterClass);
            HashSet<string> included =
                new HashSet<string>();
            foreach (DeverQuestAbilitySlot slot in
                     definition?.abilityProfile?.abilities ??
                     new List<DeverQuestAbilitySlot>())
            {
                if (slot?.spell != null &&
                    !known.Contains(slot.spell.SpellId))
                {
                    continue;
                }
                string id = slot?.spell == null
                    ? slot?.technique?.TechniqueId
                    : slot.spell.SpellId;
                if (slot != null &&
                    !string.IsNullOrWhiteSpace(id) &&
                    included.Add(id))
                {
                    yield return slot;
                }
            }
            foreach (DeverQuestSpell spell in
                     DeverQuestRulesService.KnownSpellAssets(adventurer))
            {
                if (spell == null || !included.Add(spell.SpellId))
                {
                    continue;
                }
                yield return new DeverQuestAbilitySlot
                {
                    spell = spell,
                    priority = 50,
                    useBelowHitPointPercent = 45,
                    maintainEffect = true
                };
            }
        }

        private static int Score(
            DeverQuestAbilitySlot slot,
            int hitPointPercent,
            DeverQuestCombatState monsterState)
        {
            List<DeverQuestCombatEffect> effects =
                Effects(slot.spell, slot.technique);
            bool healing = effects.Any(effect =>
                effect != null &&
                (effect.effectType ==
                 DeverQuestCombatEffectType.Heal ||
                 effect.effectType ==
                 DeverQuestCombatEffectType.HealOverTime ||
                 effect.effectType ==
                 DeverQuestCombatEffectType.Shield));
            if (healing &&
                hitPointPercent > slot.useBelowHitPointPercent)
            {
                return -1;
            }
            bool alreadyMaintained =
                slot.maintainEffect &&
                effects.Any(effect =>
                    effect != null &&
                    IsCondition(effect.effectType) &&
                    monsterState.Has(effect.effectType));
            if (alreadyMaintained)
            {
                return -1;
            }
            return slot.priority +
                   (healing ? 200 : 0) +
                   (effects.Any(effect =>
                        effect != null &&
                        effect.effectType ==
                        DeverQuestCombatEffectType.DamageOverTime)
                        ? 40
                        : 0);
        }

        private static List<DeverQuestCombatEffect> Effects(
            DeverQuestSpell spell,
            DeverQuestAttackTechnique technique)
        {
            if (technique?.effects?.Count > 0)
            {
                return technique.effects;
            }
            if (spell?.effects?.Count > 0)
            {
                return spell.effects;
            }
            List<DeverQuestCombatEffect> legacy =
                new List<DeverQuestCombatEffect>();
            if (spell != null &&
                !string.IsNullOrWhiteSpace(spell.damageDice))
            {
                legacy.Add(new DeverQuestCombatEffect
                {
                    effectType =
                        DeverQuestCombatEffectType.DirectDamage,
                    target = DeverQuestCombatTarget.Enemy,
                    dice = spell.damageDice,
                    damageType = spell.damageType
                });
            }
            if (spell != null &&
                !string.IsNullOrWhiteSpace(spell.statusEffect))
            {
                legacy.Add(new DeverQuestCombatEffect
                {
                    effectType =
                        DeverQuestCombatEffectType.Snare,
                    target = DeverQuestCombatTarget.Enemy,
                    durationRounds = 2
                });
            }
            return legacy;
        }

        private static bool IsHostile(DeverQuestCombatEffect effect)
        {
            return effect.target ==
                   DeverQuestCombatTarget.Enemy ||
                   effect.target ==
                   DeverQuestCombatTarget.AllEnemies;
        }

        private static bool IsCondition(
            DeverQuestCombatEffectType effect)
        {
            return effect != DeverQuestCombatEffectType.DirectDamage &&
                   effect != DeverQuestCombatEffectType.Heal &&
                   effect !=
                   DeverQuestCombatEffectType.ManaRestore &&
                   effect !=
                   DeverQuestCombatEffectType.ReturnToGuild;
        }

        private static void ApplyEffect(
            DeverQuestCombatEffect effect,
            DeverQuestAdventurer adventurer,
            DeverQuestMonsterProfile monster,
            string actionName,
            string seed,
            int round,
            ref int monsterHitPoints,
            DeverQuestCombatState adventurerState,
            DeverQuestCombatState monsterState,
            DeverQuestBattleResult result,
            List<string> applied)
        {
            int rolled =
                string.IsNullOrWhiteSpace(effect.dice)
                    ? 0
                    : DeverQuestRulesService.RollDice(
                        effect.dice, seed, out _);
            int amount = Math.Max(0, rolled + effect.flatAmount);
            bool selfTarget =
                effect.target == DeverQuestCombatTarget.Self ||
                effect.target == DeverQuestCombatTarget.Ally ||
                effect.target == DeverQuestCombatTarget.AllAllies;
            if (!selfTarget &&
                effect.saveNegates)
            {
                int save = DeverQuestRulesService.RollDice(
                               "1d20",
                               seed + ":save",
                               out _) +
                           Math.Max(0, monster.level / 2);
                if (save >= effect.difficultyClass)
                {
                    applied.Add(
                        $"{effect.effectType} resisted by save " +
                        $"{save} vs DC {effect.difficultyClass}");
                    return;
                }
            }
            switch (effect.effectType)
            {
                case DeverQuestCombatEffectType.DirectDamage:
                case DeverQuestCombatEffectType.LifeDrain:
                    DeverQuestDamageResolution damage =
                        DeverQuestDamageService.Resolve(
                            Math.Max(1, amount),
                            effect.damageType,
                            monster.damageAffinities);
                    monsterHitPoints = Math.Max(
                        0,
                        monsterHitPoints - damage.FinalDamage +
                        damage.AbsorbedHealing);
                    RecordDamage(
                        result,
                        round,
                        adventurer.characterName,
                        monster.displayName,
                        damage);
                    applied.Add(damage.Summary);
                    if (effect.effectType ==
                        DeverQuestCombatEffectType.LifeDrain)
                    {
                        int before = adventurer.currentHitPoints;
                        adventurer.currentHitPoints = Math.Min(
                            adventurer.maximumHitPoints,
                            adventurer.currentHitPoints +
                            damage.FinalDamage);
                        applied.Add(
                            $"{adventurer.currentHitPoints - before} HP drained");
                    }
                    break;
                case DeverQuestCombatEffectType.Heal:
                    int prior = adventurer.currentHitPoints;
                    adventurer.currentHitPoints = Math.Min(
                        adventurer.maximumHitPoints,
                        adventurer.currentHitPoints +
                        Math.Max(1, amount));
                    applied.Add(
                        $"{adventurer.currentHitPoints - prior} HP restored");
                    break;
                case DeverQuestCombatEffectType.ManaRestore:
                    int oldMana = adventurer.currentMana;
                    adventurer.currentMana = Math.Min(
                        adventurer.maximumMana,
                        adventurer.currentMana + amount);
                    applied.Add(
                        $"{adventurer.currentMana - oldMana} mana restored");
                    break;
                case DeverQuestCombatEffectType.Cleanse:
                    adventurerState.Effects.RemoveAll(value =>
                        value.EffectType ==
                        DeverQuestCombatEffectType.DamageOverTime ||
                        value.EffectType ==
                        DeverQuestCombatEffectType.Snare ||
                        value.EffectType ==
                        DeverQuestCombatEffectType.Root ||
                        value.EffectType ==
                        DeverQuestCombatEffectType.Silence);
                    applied.Add("harmful conditions cleansed");
                    break;
                case DeverQuestCombatEffectType.Dispel:
                    monsterState.Effects.RemoveAll(value =>
                        value.EffectType ==
                        DeverQuestCombatEffectType.Shield ||
                        value.EffectType ==
                        DeverQuestCombatEffectType.AttackBuff ||
                        value.EffectType ==
                        DeverQuestCombatEffectType.ArmorBuff);
                    applied.Add("enemy boons dispelled");
                    break;
                case DeverQuestCombatEffectType.ReturnToGuild:
                    applied.Add("return passage prepared");
                    break;
                default:
                    AddCondition(
                        selfTarget ? adventurerState : monsterState,
                        effect,
                        actionName);
                    applied.Add(
                        $"{effect.effectType} " +
                        $"({effect.durationRounds} rounds)");
                    break;
            }
        }

        private static void AddCondition(
            DeverQuestCombatState state,
            DeverQuestCombatEffect effect,
            string source)
        {
            state.Effects.RemoveAll(value =>
                value.EffectType == effect.effectType);
            state.Effects.Add(new DeverQuestActiveCombatEffect
            {
                EffectType = effect.effectType,
                Source = source ?? string.Empty,
                Dice = effect.dice ?? string.Empty,
                Amount = Math.Max(0, effect.flatAmount),
                RemainingRounds = Math.Max(1, effect.durationRounds),
                DamageType = effect.damageType,
                BreaksOnDamage = effect.breaksOnDamage
            });
        }

        private static void TickState(
            DeverQuestCombatState state,
            bool targetsAdventurer,
            DeverQuestAdventurer adventurer,
            DeverQuestMonsterProfile monster,
            string seed,
            int round,
            ref int monsterHitPoints,
            DeverQuestBattleResult result)
        {
            foreach (DeverQuestActiveCombatEffect effect in
                     state.Effects.ToList())
            {
                if (effect.RemainingRounds <= 0)
                {
                    continue;
                }
                if (effect.EffectType ==
                    DeverQuestCombatEffectType.DamageOverTime)
                {
                    int damage = Math.Max(
                        1,
                        DeverQuestRulesService.RollDice(
                            effect.Dice,
                            seed + ":" + effect.Source + ":" + round,
                            out _) + effect.Amount);
                    if (targetsAdventurer)
                    {
                        adventurer.currentHitPoints = Math.Max(
                            1,
                            adventurer.currentHitPoints - damage);
                    }
                    else
                    {
                        monsterHitPoints = Math.Max(
                            0, monsterHitPoints - damage);
                    }
                    AddCombatLine(
                        result,
                        $"Round {round}: {effect.Source} deals " +
                        $"{damage} ongoing " +
                        $"{effect.DamageType.ToString().ToLowerInvariant()} " +
                        "damage.");
                }
                else if (effect.EffectType ==
                         DeverQuestCombatEffectType.HealOverTime &&
                         targetsAdventurer)
                {
                    int healing = Math.Max(
                        1,
                        DeverQuestRulesService.RollDice(
                            effect.Dice,
                            seed + ":" + effect.Source + ":" + round,
                            out _) + effect.Amount);
                    adventurer.currentHitPoints = Math.Min(
                        adventurer.maximumHitPoints,
                        adventurer.currentHitPoints + healing);
                }
                effect.RemainingRounds--;
            }
            state.Effects.RemoveAll(value =>
                value.RemainingRounds <= 0 || value.Amount == 0 &&
                value.EffectType ==
                DeverQuestCombatEffectType.Shield);
        }

        private static void RecordAction(
            DeverQuestBattleResult result,
            int round,
            string actor,
            string action,
            string target,
            int mana,
            string effect)
        {
            result.actionEvents.Add(
                new DeverQuestCombatActionEvent
                {
                    round = round,
                    actor = actor ?? string.Empty,
                    actionName = action ?? string.Empty,
                    target = target ?? string.Empty,
                    manaSpent = Math.Max(0, mana),
                    effects = string.IsNullOrWhiteSpace(effect)
                        ? new List<string>()
                        : new List<string> { effect }
                });
        }

        private static string ActionKey(
            DeverQuestSpell spell,
            DeverQuestAttackTechnique technique)
        {
            return spell == null
                ? technique?.TechniqueId ?? string.Empty
                : spell.SpellId;
        }

        private static void TickCooldowns(
            DeverQuestCombatState state)
        {
            foreach (string key in state.Cooldowns.Keys.ToList())
            {
                int remaining = state.Cooldowns[key] - 1;
                if (remaining <= 0)
                {
                    state.Cooldowns.Remove(key);
                }
                else
                {
                    state.Cooldowns[key] = remaining;
                }
            }
        }

        private static void AddCombatLine(
            DeverQuestBattleResult result,
            string line)
        {
            if (result.combatLog.Count < 100)
            {
                result.combatLog.Add(line);
            }
        }

        private static void RecordDamage(
            DeverQuestBattleResult result,
            int round,
            string source,
            string target,
            DeverQuestDamageResolution resolution)
        {
            result.damageEvents.Add(
                new DeverQuestDamageEvent
                {
                    round = round,
                    source = source ?? string.Empty,
                    target = target ?? string.Empty,
                    damageType = resolution.DamageType,
                    response = resolution.Response,
                    rawDamage = resolution.RawDamage,
                    finalDamage = resolution.FinalDamage,
                    absorbedHealing = resolution.AbsorbedHealing
                });
        }
    }
}
