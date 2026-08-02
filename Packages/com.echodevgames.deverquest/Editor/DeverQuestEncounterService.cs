using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestEncounterService
    {
        private static Dictionary<string, DeverQuestEncounterProfile>
            encounterCache;

        static DeverQuestEncounterService()
        {
            EditorApplication.projectChanged -= ClearCache;
            EditorApplication.projectChanged += ClearCache;
        }

        public static DeverQuestBattleResult Resolve(
            DeverQuestSession session,
            DeverQuestSessionStage stage,
            int decreeModifier)
        {
            if (session == null || stage == null ||
                stage.encounterResolved ||
                string.IsNullOrWhiteSpace(stage.encounterProfileId))
            {
                return null;
            }
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage.encounterProfileId);
            if (encounter == null)
            {
                return null;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            bool survival =
                encounter.encounterMode ==
                DeverQuestEncounterMode.Survival;
            stage.survivalMode = survival;
            DeverQuestCompanionState companion =
                DeverQuestCompanionService.ActiveCompanion(
                    adventurer);
            DeverQuestCompanionProfile companionProfile =
                DeverQuestCompanionService.FindProfile(
                    companion?.profileId);
            if (companionProfile == null ||
                companion?.isFallen == true)
            {
                companion = null;
                companionProfile = null;
            }
            DeverQuestAudioDirector.PlayCue(
                DeverQuestAudioCue.EncounterAttack);
            string seed =
                $"{session.sessionId}:{stage.stageId}:" +
                $"{adventurer.characterName}";
            DeverQuestBattleResult result =
                new DeverQuestBattleResult
                {
                    stageId = stage.stageId,
                    stageTitle = stage.stageTitle,
                    encounterId = encounter.EncounterId,
                    encounterName = encounter.displayName,
                    seed = seed,
                    startingHitPoints =
                        adventurer.currentHitPoints,
                    companionName =
                        companion == null
                            ? string.Empty
                            : DeverQuestCompanionService.DisplayName(
                                companion),
                    companionStartingHitPoints =
                        companion?.currentHitPoints ?? 0,
                    companionLevelBefore =
                        companion?.level ?? 0,
                    parRounds = encounter.parRounds,
                    survivalWave =
                        survival ? stage.survivalWave + 1 : 0,
                    carriedWeight =
                        DeverQuestEncumbranceService.CarriedWeight(
                            adventurer),
                    carryCapacity =
                        DeverQuestEncumbranceService.CarryCapacity(
                            adventurer),
                    resolvedUtcTicks = DateTime.UtcNow.Ticks
                };
            if (!string.IsNullOrWhiteSpace(
                    encounter.storyIntroduction))
            {
                result.combatLog.Add(
                    encounter.storyIntroduction.Trim());
            }
            if (companion != null)
            {
                result.combatLog.Add(
                    $"{result.companionName} joins the encounter " +
                    $"[{companionProfile.kind} · " +
                    $"{companionProfile.role}].");
            }

            long copper = 0L;
            long experience = 0L;
            int combatIndex = 0;
            bool victory = true;
            int survivalTier = survival
                ? stage.survivalWave /
                  Math.Max(
                      1,
                      encounter.difficultyIncreaseEveryWaves)
                : 0;
            IEnumerable<DeverQuestEncounterWave> selectedWaves =
                survival && encounter.waves.Count > 0
                    ? encounter.waves.Skip(
                            stage.survivalWave %
                            encounter.waves.Count)
                        .Take(1)
                    : encounter.waves;
            foreach (DeverQuestEncounterWave wave
                     in selectedWaves)
            {
                if (wave == null || wave.monster == null)
                {
                    continue;
                }
                for (int count = 0; count < Math.Max(1, wave.count);
                     count++)
                {
                    if (!FightMonster(
                            adventurer,
                            companion,
                            companionProfile,
                            wave.monster,
                            encounter,
                            survivalTier,
                            decreeModifier,
                            seed,
                            ref combatIndex,
                            result))
                    {
                        victory = false;
                        break;
                    }
                    result.defeatedMonsters.Add(
                        wave.monster.displayName +
                        (wave.bossWave ? " [Boss]" : string.Empty));
                    copper += wave.monster.victoryCopper;
                    experience += wave.monster.victoryExperience;
                    ResolveDrops(
                        adventurer,
                        wave.monster,
                        seed,
                        ref combatIndex,
                        result,
                        ref copper,
                        ref experience);
                    if (encounter.pauseWhenEncumbered &&
                        DeverQuestEncumbranceService.IsEncumbered(
                            adventurer))
                    {
                        result.safetyPaused = true;
                        result.safetyPauseReason =
                            "Encumbered by carried loot and coin";
                        victory = false;
                        break;
                    }
                }
                if (!victory)
                {
                    break;
                }
            }

            result.victory = victory;
            result.earlyVictory =
                victory &&
                result.rounds <= Math.Max(1, encounter.parRounds);
            if (victory)
            {
                copper += encounter.victoryCopperBonus;
                experience += encounter.victoryExperienceBonus;
                if (survival)
                {
                    int waveNumber = stage.survivalWave + 1;
                    copper +=
                        encounter.survivalCopperGrowthPerWave *
                        Math.Max(0, waveNumber - 1);
                    experience +=
                        encounter.survivalExperienceGrowthPerWave *
                        Math.Max(0, waveNumber - 1);
                    stage.survivalWave = waveNumber;
                    stage.survivalExitOffered =
                        waveNumber %
                        Math.Max(
                            1,
                            encounter.wagonOfferEveryWaves) == 0;
                }
                if (result.earlyVictory)
                {
                    copper += encounter.earlyVictoryCopperBonus;
                    experience +=
                        encounter.earlyVictoryExperienceBonus;
                    AddCombatLine(
                        result,
                        $"Early victory: {result.rounds} rounds " +
                        $"beat par {encounter.parRounds}.");
                }
            }
            else if (!result.safetyPaused)
            {
                adventurer.defeats++;
                result.characterFell = true;
                if (encounter.allowCharacterDeath)
                {
                    adventurer.currentHitPoints = 0;
                    adventurer.isFallen = true;
                    AddStatus(adventurer, "Fallen");
                    result.injury =
                        "The Adventurer fell and requires resurrection.";
                }
                else
                {
                    adventurer.currentHitPoints = 1;
                    if (encounter.allowInjury)
                    {
                        string injury =
                            InjuryForSeed(seed + ":injury");
                        AddStatus(adventurer, injury);
                        result.injury = injury;
                    }
                }
            }
            else
            {
                AddCombatLine(
                    result,
                    "The fight was safely suspended before another " +
                    "enemy turn: " + result.safetyPauseReason + ".");
                DeverQuestAudioDirector.PlayCue(
                    DeverQuestAudioCue.EncounterDanger);
            }
            result.endingHitPoints = adventurer.currentHitPoints;
            if (companion != null)
            {
                bool companionParticipated =
                    result.rounds > 0;
                long companionExperience =
                    companionParticipated &&
                    victory &&
                    !companion.isFallen
                        ? Math.Max(1L, experience / 2L)
                        : 0L;
                if (companionParticipated &&
                    !result.safetyPaused)
                {
                    DeverQuestCompanionService.CompleteBattle(
                        companion,
                        victory,
                        experience);
                }
                result.companionEndingHitPoints =
                    companion.currentHitPoints;
                result.companionFell = companion.isFallen;
                result.companionLevelAfter = companion.level;
                result.companionExperienceEarned =
                    companionExperience;
            }
            result.bonusCopper = copper;
            result.bonusExperience = experience;
            result.typedDamageSummary =
                DeverQuestDamageService.DescribeBattle(
                    result.damageEvents);
            if (companion != null)
            {
                DeverQuestCombatSummaryService.RecordCompanionBattle(
                    companion,
                    result);
            }
            if (copper > 0L || experience > 0L)
            {
                DeverQuestProgressionResult progression =
                    DeverQuestAdventurerService.Award(
                        copper, experience);
                session.rewardTransactions.Add(
                    new DeverQuestRewardTransaction
                    {
                        categoryName = "Encounter",
                        transactionType = victory
                            ? "Battle Victory"
                            : "Battle Spoils",
                        copper = copper,
                        experience = experience,
                        startingLevel = progression.StartingLevel,
                        endingLevel = progression.EndingLevel,
                        createdUtcTicks = DateTime.UtcNow.Ticks,
                        note = encounter.displayName
                    });
            }
            else
            {
                DeverQuestAdventurerService.Save();
            }
            stage.encounterResolved = !survival;
            stage.survivalFightPaused = result.safetyPaused;
            stage.survivalPauseReason =
                result.safetyPauseReason ?? string.Empty;
            result.carriedWeight =
                DeverQuestEncumbranceService.CarriedWeight(adventurer);
            session.battleResults.Add(result);
            DeverQuestTacticalArchiveService.Record(
                session,
                result);
            DeverQuestGuildAccountService.AddAudit(
                victory
                    ? "Encounter Victory"
                    : result.safetyPaused
                    ? "Encounter Safety Pause"
                    : "Encounter Defeat",
                encounter.displayName,
                $"{adventurer.characterName} · seed {seed}");
            DeverQuestAudioDirector.PlayCue(
                victory
                    ? DeverQuestAudioCue.EncounterVictory
                    : result.safetyPaused
                    ? DeverQuestAudioCue.EncounterDanger
                    : DeverQuestAudioCue.EncounterDefeat);
            return result;
        }

        public static bool IsSurvival(
            DeverQuestSessionStage stage)
        {
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage?.encounterProfileId);
            return encounter != null &&
                   encounter.encounterMode ==
                   DeverQuestEncounterMode.Survival;
        }

        public static int SurvivalIntervalMinutes(
            DeverQuestSessionStage stage)
        {
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage?.encounterProfileId);
            return encounter == null
                ? Math.Max(1, stage?.focusedMinutesRequired ?? 15)
                : Math.Max(1, encounter.survivalWaveMinutes);
        }

        public static string EncounterDisplayName(
            DeverQuestSessionStage stage)
        {
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage?.encounterProfileId);
            return encounter == null ||
                   string.IsNullOrWhiteSpace(encounter.displayName)
                ? string.IsNullOrWhiteSpace(stage?.stageTitle)
                    ? "Encounter"
                    : stage.stageTitle
                : encounter.displayName;
        }

        public static string DescribeEncounter(
            DeverQuestSessionStage stage)
        {
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage?.encounterProfileId);
            if (encounter == null)
            {
                return "No tactical Encounter Profile is assigned.";
            }
            int monsterCount = (encounter.waves ??
                                new List<DeverQuestEncounterWave>())
                .Where(value => value != null && value.monster != null)
                .Sum(value => Math.Max(1, value.count));
            string mode = encounter.encounterMode ==
                          DeverQuestEncounterMode.Survival
                ? "Survival"
                : "Fixed";
            return $"{mode} · {monsterCount} configured foe" +
                   (monsterCount == 1 ? string.Empty : "s") +
                   $" · Par {Math.Max(1, encounter.parRounds)} rounds · " +
                   $"Victory bonus " +
                   $"{DeverQuestAdventurerService.FormatCoins(encounter.victoryCopperBonus)} " +
                   $"+ {encounter.victoryExperienceBonus} XP";
        }

        public static string DescribeSurvivalProgress(
            DeverQuestSessionStage stage)
        {
            DeverQuestEncounterProfile encounter =
                FindEncounter(stage?.encounterProfileId);
            if (encounter == null)
            {
                return $"Wave {Math.Max(0, stage?.survivalWave ?? 0)} · " +
                       "Survival profile unavailable.";
            }
            int completedWaves = Math.Max(0, stage?.survivalWave ?? 0);
            int difficultyEvery = Math.Max(
                1,
                encounter.difficultyIncreaseEveryWaves);
            int wagonEvery = Math.Max(1, encounter.wagonOfferEveryWaves);
            int currentTier = completedWaves / difficultyEvery;
            int wavesToTier = difficultyEvery -
                              completedWaves % difficultyEvery;
            int wavesToWagon = wagonEvery -
                               completedWaves % wagonEvery;
            return $"Next wave {completedWaves + 1} · " +
                   $"Difficulty tier {currentTier} · " +
                   $"Tier increases in {wavesToTier} wave" +
                   (wavesToTier == 1 ? string.Empty : "s") +
                   $" · Guild wagon in {wavesToWagon} wave" +
                   (wavesToWagon == 1 ? string.Empty : "s") +
                   $" · {Math.Max(1, encounter.survivalWaveMinutes)} " +
                   "focused minutes per wave";
        }

        public static bool Resurrect(out string message)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (!adventurer.isFallen)
            {
                message = "This Adventurer is not fallen.";
                return false;
            }
            const int resurrectionCost = 50;
            if (!DeverQuestAdventurerService.SpendCopper(
                    resurrectionCost, out message))
            {
                return false;
            }
            adventurer.isFallen = false;
            adventurer.currentHitPoints =
                Math.Max(1, adventurer.maximumHitPoints / 2);
            adventurer.statusEffects.RemoveAll(
                item => item == "Fallen");
            DeverQuestAdventurerService.Save();
            message =
                "Resurrection complete at half Hit Points for " +
                DeverQuestAdventurerService.FormatCoins(
                    resurrectionCost) + ".";
            return true;
        }

        private static bool FightMonster(
            DeverQuestAdventurer adventurer,
            DeverQuestCompanionState companion,
            DeverQuestCompanionProfile companionProfile,
            DeverQuestMonsterProfile monster,
            DeverQuestEncounterProfile encounter,
            int difficultyTier,
            int decreeModifier,
            string seed,
            ref int index,
            DeverQuestBattleResult result)
        {
            int monsterMaximumHitPoints =
                monster.maximumHitPoints +
                Math.Max(0, difficultyTier) *
                Math.Max(1, monster.level * 2);
            int monsterArmorClass =
                monster.armorClass +
                Math.Max(0, difficultyTier) / 2;
            int monsterAttackModifier =
                monster.attackModifier +
                Math.Max(0, difficultyTier);
            int monsterHp = monsterMaximumHitPoints;
            int rounds = 0;
            DeverQuestAttackProfile adventurerAttack =
                DeverQuestDamageService.AdventurerAttack(adventurer);
            DeverQuestCombatState adventurerState =
                new DeverQuestCombatState();
            DeverQuestCombatState monsterState =
                new DeverQuestCombatState();
            List<DeverQuestDamageAffinity> adventurerAffinities =
                DeverQuestDamageService
                    .AdventurerAffinities(adventurer)
                    .ToList();
            AddCombatLine(
                result,
                $"{monster.displayName} enters the encounter " +
                $"[{monster.creatureType}].");
            while (monsterHp > 0 &&
                   adventurer.currentHitPoints > 0 &&
                   rounds < 50)
            {
                rounds++;
                DeverQuestTacticalCombatService.TickStartOfRound(
                    adventurer,
                    monster,
                    seed,
                    rounds,
                    ref monsterHp,
                    adventurerState,
                    monsterState,
                    result);
                if (monsterHp <= 0 ||
                    adventurer.currentHitPoints <= 0)
                {
                    break;
                }
                int openingPauseThreshold = Math.Max(
                    1,
                    adventurer.maximumHitPoints *
                    encounter.lowHitPointPausePercent / 100);
                if (adventurer.currentHitPoints <=
                    openingPauseThreshold)
                {
                    result.safetyPaused = true;
                    result.safetyPauseReason =
                        $"Low health: " +
                        $"{adventurer.currentHitPoints}/" +
                        $"{adventurer.maximumHitPoints} HP";
                    break;
                }
                index++;
                DeverQuestAbility ability =
                    AttackAbility(adventurer.characterClass);
                bool tacticalAction =
                    DeverQuestTacticalCombatService
                        .TryResolveAdventurerTurn(
                            adventurer,
                            monster,
                            decreeModifier,
                            seed,
                            rounds,
                            ref index,
                            ref monsterHp,
                            adventurerState,
                            monsterState,
                            result);
                DeverQuestRuleResult attack = tacticalAction
                    ? null
                    :
                    DeverQuestRulesService.ResolveCheck(
                        adventurer,
                        ability,
                        true,
                        monsterArmorClass,
                        seed + ":hero:" + index,
                        decreeModifier -
                        DeverQuestTacticalCombatService
                            .AdventurerAttackPenalty(
                                adventurerState) -
                        monsterState.Total(
                            DeverQuestCombatEffectType.ArmorBuff) +
                        monsterState.Total(
                            DeverQuestCombatEffectType.ArmorDebuff));
                if (!tacticalAction && attack.Success)
                {
                    int modifier = Math.Max(
                        0,
                        DeverQuestRulesService.AbilityModifier(
                            DeverQuestRulesService.GetAbilityScore(
                                adventurer, ability)));
                    int rawDamage =
                        DeverQuestRulesService.RollDice(
                            adventurerAttack.DamageDice,
                            seed + ":hero-damage:" + index,
                            out _) + modifier;
                    DeverQuestDamageResolution damage =
                        DeverQuestDamageService.Resolve(
                            Math.Max(1, rawDamage),
                            adventurerAttack.DamageType,
                            monster.damageAffinities);
                    monsterHp = Math.Min(
                        monsterMaximumHitPoints,
                        monsterHp - damage.FinalDamage +
                        damage.AbsorbedHealing);
                    RecordDamage(
                        result,
                        rounds,
                        adventurer.characterName,
                        monster.displayName,
                        damage);
                    AddCombatLine(
                        result,
                        $"Round {rounds}: {adventurer.characterName} " +
                        $"uses {adventurerAttack.DisplayName} against " +
                        $"{monster.displayName} for {damage.Summary}.");
                }
                else if (!tacticalAction)
                {
                    AddCombatLine(
                        result,
                        $"Round {rounds}: {adventurer.characterName} " +
                        $"misses {monster.displayName}.");
                }
                if (monsterHp <= 0)
                {
                    break;
                }
                int enemyAttackPenalty =
                    ResolveCompanionTurn(
                        adventurer,
                        companion,
                        companionProfile,
                        monster,
                        seed,
                        rounds,
                        ref index,
                        ref monsterHp,
                        result);
                enemyAttackPenalty +=
                    DeverQuestTacticalCombatService
                        .MonsterAttackPenalty(monsterState);
                if (monsterHp <= 0)
                {
                    break;
                }
                index++;
                if (DeverQuestTacticalCombatService.SkipsTurn(
                        monsterState))
                {
                    AddCombatLine(
                        result,
                        $"{monster.displayName} loses its turn to a " +
                        "control effect.");
                    continue;
                }
                int enemyRoll =
                    DeverQuestRulesService.RollDice(
                        "1d20",
                        seed + ":enemy:" + index,
                        out _) +
                    monsterAttackModifier -
                    enemyAttackPenalty;
                bool targetsCompanion =
                    ShouldTargetCompanion(
                        companion,
                        companionProfile,
                        seed,
                        index);
                int targetArmorClass = targetsCompanion
                    ? DeverQuestCompanionService.ArmorClass(
                        companion,
                        companionProfile)
                    : DeverQuestRulesService.ArmorClass(
                        adventurer) +
                      adventurerState.Total(
                          DeverQuestCombatEffectType.ArmorBuff) -
                      adventurerState.Total(
                          DeverQuestCombatEffectType.ArmorDebuff);
                if (enemyRoll >= targetArmorClass)
                {
                    int rawDamage = Math.Max(
                        1,
                        DeverQuestRulesService.RollDice(
                            monster.damageDice,
                            seed + ":enemy-damage:" + index,
                            out _));
                    DeverQuestDamageResolution damage =
                        DeverQuestDamageService.Resolve(
                            rawDamage,
                            monster.attackDamageType,
                            targetsCompanion
                                ? companionProfile.damageAffinities
                                : adventurerAffinities);
                    if (targetsCompanion)
                    {
                        int maximumCompanionHitPoints =
                            DeverQuestCompanionService
                                .MaximumHitPoints(
                                    companion,
                                    companionProfile);
                        companion.currentHitPoints =
                            Math.Min(
                                maximumCompanionHitPoints,
                                Math.Max(
                                    0,
                                    companion.currentHitPoints -
                                    damage.FinalDamage +
                                    damage.AbsorbedHealing));
                        RecordDamage(
                            result,
                            rounds,
                            monster.displayName,
                            DeverQuestCompanionService
                                .DisplayName(companion),
                            damage);
                        if (companion.currentHitPoints <= 0)
                        {
                            companion.isFallen = true;
                            companion.isActive = false;
                            adventurer.activeCompanionInstanceId =
                                string.Empty;
                            AddCombatLine(
                                result,
                                $"{DeverQuestCompanionService.DisplayName(companion)} " +
                                "falls and must recover at the Stable.");
                        }
                        else
                        {
                            AddCombatLine(
                                result,
                                $"{monster.displayName} hits " +
                                $"{DeverQuestCompanionService.DisplayName(companion)} " +
                                $"for {damage.Summary}; " +
                                $"{companion.currentHitPoints} HP remains.");
                        }
                    }
                    else
                    {
                        int shieldedDamage =
                            DeverQuestTacticalCombatService
                                .AbsorbWithShield(
                                    adventurerState,
                                    damage.FinalDamage);
                        adventurer.currentHitPoints =
                            Math.Min(
                                adventurer.maximumHitPoints,
                                Math.Max(
                                    1,
                                    adventurer.currentHitPoints -
                                    shieldedDamage +
                                    damage.AbsorbedHealing));
                        RecordDamage(
                            result,
                            rounds,
                            monster.displayName,
                            adventurer.characterName,
                            damage);
                        if (adventurer.currentHitPoints > 0 &&
                            adventurer.currentHitPoints <=
                            Math.Max(
                                1,
                                adventurer.maximumHitPoints / 4))
                        {
                            DeverQuestAudioDirector.PlayCue(
                                DeverQuestAudioCue.EncounterDanger);
                        }
                        AddCombatLine(
                            result,
                            $"{monster.displayName} hits for " +
                            $"{damage.Summary}; " +
                            $"{adventurer.currentHitPoints} HP remains.");
                        DeverQuestTacticalCombatService
                            .ApplyMonsterOnHit(
                                monster,
                                adventurer,
                                seed + ":monster-ability:" + index,
                                rounds,
                                adventurerState,
                                result);
                        int pauseThreshold = Math.Max(
                            1,
                            adventurer.maximumHitPoints *
                            encounter.lowHitPointPausePercent / 100);
                        if (adventurer.currentHitPoints > 0 &&
                            adventurer.currentHitPoints <=
                            pauseThreshold)
                        {
                            result.safetyPaused = true;
                            result.safetyPauseReason =
                                $"Low health: " +
                                $"{adventurer.currentHitPoints}/" +
                                $"{adventurer.maximumHitPoints} HP";
                            DeverQuestAudioDirector.PlayCue(
                                DeverQuestAudioCue.EncounterDanger);
                            break;
                        }
                    }
                }
                else
                {
                    AddCombatLine(
                        result,
                        $"{monster.displayName} misses " +
                        (targetsCompanion
                            ? DeverQuestCompanionService
                                .DisplayName(companion)
                            : adventurer.characterName) +
                        ".");
                }
            }
            result.rounds += rounds;
            return monsterHp <= 0;
        }

        private static int ResolveCompanionTurn(
            DeverQuestAdventurer adventurer,
            DeverQuestCompanionState companion,
            DeverQuestCompanionProfile profile,
            DeverQuestMonsterProfile monster,
            string seed,
            int round,
            ref int index,
            ref int monsterHitPoints,
            DeverQuestBattleResult result)
        {
            if (companion == null || profile == null ||
                companion.isFallen ||
                companion.currentHitPoints <= 0)
            {
                return 0;
            }
            string companionName =
                DeverQuestCompanionService.DisplayName(companion);
            index++;
            if (profile.role == DeverQuestCompanionRole.Support &&
                adventurer.currentHitPoints <
                adventurer.maximumHitPoints &&
                DeverQuestRulesService.RollDice(
                    "1d4",
                    seed + ":companion-support:" + index,
                    out _) == 4)
            {
                int healing = Math.Max(
                    1,
                    DeverQuestRulesService.RollDice(
                        "1d4",
                        seed + ":companion-heal:" + index,
                        out _));
                int before = adventurer.currentHitPoints;
                adventurer.currentHitPoints =
                    Math.Min(
                        adventurer.maximumHitPoints,
                        adventurer.currentHitPoints + healing);
                AddCombatLine(
                    result,
                    $"Round {round}: {companionName} restores " +
                    $"{adventurer.currentHitPoints - before} HP to " +
                    $"{adventurer.characterName}.");
                return 0;
            }

            int attackRoll =
                DeverQuestRulesService.RollDice(
                    "1d20",
                    seed + ":companion-attack:" + index,
                    out _) +
                DeverQuestCompanionService.AttackModifier(
                    companion,
                    profile);
            if (attackRoll < monster.armorClass)
            {
                AddCombatLine(
                    result,
                    $"Round {round}: {companionName} misses " +
                    $"{monster.displayName}.");
                return 0;
            }
            int strikerBonus =
                profile.role == DeverQuestCompanionRole.Striker
                    ? 1 + Math.Max(0, companion.level - 1) / 4
                    : 0;
            int rawDamage = Math.Max(
                1,
                DeverQuestRulesService.RollDice(
                    profile.damageDice,
                    seed + ":companion-damage:" + index,
                    out _) + strikerBonus);
            DeverQuestDamageResolution damage =
                DeverQuestDamageService.Resolve(
                    rawDamage,
                    profile.damageType,
                    monster.damageAffinities);
            monsterHitPoints =
                Math.Min(
                    monster.maximumHitPoints,
                    monsterHitPoints -
                    damage.FinalDamage +
                    damage.AbsorbedHealing);
            RecordDamage(
                result,
                round,
                companionName,
                monster.displayName,
                damage);
            AddCombatLine(
                result,
                $"Round {round}: {companionName} hits " +
                $"{monster.displayName} for {damage.Summary}.");
            return profile.role ==
                   DeverQuestCompanionRole.Controller &&
                   damage.FinalDamage > 0
                ? 1
                : 0;
        }

        private static bool ShouldTargetCompanion(
            DeverQuestCompanionState companion,
            DeverQuestCompanionProfile profile,
            string seed,
            int index)
        {
            if (companion == null || profile == null ||
                companion.isFallen ||
                companion.currentHitPoints <= 0)
            {
                return false;
            }
            int chance =
                profile.role == DeverQuestCompanionRole.Guardian
                    ? 50
                    : 25;
            return DeverQuestRulesService.RollDice(
                       "1d100",
                       seed + ":enemy-target:" + index,
                       out _) <= chance;
        }

        private static void ResolveDrops(
            DeverQuestAdventurer adventurer,
            DeverQuestMonsterProfile monster,
            string seed,
            ref int index,
            DeverQuestBattleResult result,
            ref long copper,
            ref long experience)
        {
            foreach (DeverQuestDropEntry drop in
                     monster.dropTable ??
                     new List<DeverQuestDropEntry>())
            {
                index++;
                int roll = DeverQuestRulesService.RollDice(
                    "1d100",
                    seed + ":drop:" + index,
                    out _);
                if (roll > Math.Max(
                        0,
                        Math.Min(100, drop.dropChancePercent)))
                {
                    continue;
                }
                copper += Math.Max(0, drop.copper);
                experience += Math.Max(0, drop.experience);
                if (drop.equipment != null)
                {
                    DeverQuestSession activeSession =
                        DeverQuestSessionStore.ActiveSession;
                    DeverQuestInventoryService.AddEquipmentAsset(
                        adventurer.inventory,
                        drop.equipment,
                        DeverQuestGuildAccountService
                            .CurrentAccount?.accountId ??
                        string.Empty,
                        DeverQuestItemOriginKind.EncounterLoot,
                        "Encounter Loot",
                        activeSession?.questContractId ??
                        string.Empty,
                        activeSession?.questContractRunId ??
                        string.Empty,
                        result.encounterId,
                        monster.MonsterId,
                        monster.displayName);
                    DeverQuestRulesService.Equip(
                        adventurer, drop.equipment);
                }
                if (drop.spell != null &&
                    !adventurer.knownSpellIds.Contains(
                        drop.spell.SpellId))
                {
                    adventurer.knownSpellIds.Add(
                        drop.spell.SpellId);
                }
                if (drop.shopItem != null)
                {
                    DeverQuestSession activeSession =
                        DeverQuestSessionStore.ActiveSession;
                    DeverQuestInventoryService.AddItem(
                        adventurer.inventory,
                        drop.shopItem,
                        DeverQuestGuildAccountService
                            .CurrentAccount?.accountId ??
                        string.Empty,
                        DeverQuestItemOriginKind.EncounterLoot,
                        "Encounter Loot",
                        activeSession?.questContractId ??
                        string.Empty,
                        activeSession?.questContractRunId ??
                        string.Empty,
                        result.encounterId,
                        monster.MonsterId,
                        monster.displayName);
                }
                result.loot.Add(
                    string.IsNullOrWhiteSpace(drop.displayName)
                        ? "Unknown Spoils"
                        : drop.displayName);
            }
        }

        private static DeverQuestAbility AttackAbility(
            string characterClass)
        {
            switch (characterClass)
            {
                case "Necromancer":
                case "Wizard":
                case "Sorcerer":
                    return DeverQuestAbility.Intelligence;
                case "Cleric":
                case "Druid":
                    return DeverQuestAbility.Wisdom;
                case "Rogue":
                case "Ranger":
                case "Bard":
                    return DeverQuestAbility.Dexterity;
                default:
                    return DeverQuestAbility.Strength;
            }
        }

        private static string InjuryForSeed(string seed)
        {
            string[] injuries =
            {
                "Bruised", "Wounded", "Exhausted", "Shaken"
            };
            int roll = DeverQuestRulesService.RollDice(
                "1d4", seed, out _);
            return injuries[Math.Max(0, roll - 1)];
        }

        private static void AddStatus(
            DeverQuestAdventurer adventurer,
            string status)
        {
            if (!adventurer.statusEffects.Contains(status))
            {
                adventurer.statusEffects.Add(status);
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

        public static DeverQuestEncounterProfile FindEncounter(
            string encounterId)
        {
            if (encounterCache == null)
            {
                encounterCache =
                    new Dictionary<
                        string,
                        DeverQuestEncounterProfile>();
                foreach (string guid in AssetDatabase.FindAssets(
                             "t:DeverQuestEncounterProfile"))
                {
                    DeverQuestEncounterProfile encounter =
                        AssetDatabase.LoadAssetAtPath<
                            DeverQuestEncounterProfile>(
                            AssetDatabase.GUIDToAssetPath(guid));
                    if (encounter != null)
                    {
                        encounterCache[encounter.EncounterId] =
                            encounter;
                    }
                }
            }
            return encounterCache.TryGetValue(
                encounterId ?? string.Empty,
                out DeverQuestEncounterProfile found)
                ? found
                : null;
        }

        private static void ClearCache()
        {
            encounterCache = null;
        }
    }
}
