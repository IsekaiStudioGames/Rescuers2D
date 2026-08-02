//----- DeverQuestTimecardWriter.cs START -----

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestDailyRecord
    {
        public string developerName = string.Empty;
        public string localDate = string.Empty;
        public int chronicleIndex = 1;
        public List<DeverQuestSession> sessions =
            new List<DeverQuestSession>();
    }

    internal static class DeverQuestTimecardWriter
    {
        public static bool TryWriteSession(
            DeverQuestProfile profile,
            DeverQuestSession completedSession,
            out string markdownPath,
            out string errorMessage)
        {
            markdownPath = string.Empty;
            errorMessage = string.Empty;

            if (profile == null || completedSession == null)
            {
                errorMessage = "Profile or completed session was missing.";
                return false;
            }

            try
            {
                string developerFolder =
                    DeverQuestPathUtility.GetDeveloperFolder(
                        profile.timecardRootPath,
                        profile.developerName);

                Directory.CreateDirectory(developerFolder);

                DateTime completedLocal =
                    DeverQuestSessionStore.GetLocalCompletionTime(
                        completedSession);

                string dateKey =
                    completedLocal.ToString(
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture);

                string safeDeveloperName =
                    DeverQuestPathUtility.MakeSafeFolderName(
                        profile.developerName);

                int chronicleIndex =
                    DeverQuestChronicleIntegrityService
                        .GetRequestedChronicleIndex(
                            developerFolder, dateKey);
                string baseFileName = BuildBaseFileName(
                    dateKey, safeDeveloperName, chronicleIndex);

                string dataPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".deverquest.json");

                DeverQuestDailyRecord record =
                    LoadRecord(
                        dataPath,
                        profile.developerName,
                        dateKey);
                record.chronicleIndex = chronicleIndex;

                if (ShouldRollOver(profile, dataPath, record,
                        completedSession.sessionId))
                {
                    chronicleIndex =
                        DeverQuestChronicleIntegrityService
                            .StartNewChronicle(
                                developerFolder, dateKey);
                    baseFileName = BuildBaseFileName(
                        dateKey, safeDeveloperName, chronicleIndex);
                    dataPath = Path.Combine(
                        developerFolder,
                        baseFileName + ".deverquest.json");
                    record = CreateRecord(
                        profile.developerName, dateKey);
                    record.chronicleIndex = chronicleIndex;
                }

                markdownPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".md");

                int existingIndex = record.sessions.FindIndex(
                    session => session != null &&
                               session.sessionId ==
                               completedSession.sessionId);

                if (existingIndex >= 0)
                {
                    record.sessions[existingIndex] = completedSession;
                }
                else
                {
                    record.sessions.Add(completedSession);
                }

                record.sessions = record.sessions
                    .Where(session => session != null)
                    .OrderByDescending(
                        session => session.startedUtcTicks)
                    .ToList();

                string json = JsonUtility.ToJson(record, true);
                File.WriteAllText(dataPath, json, Encoding.UTF8);

                string markdown = BuildMarkdown(
                    record,
                    completedLocal.Date,
                    DeverQuestChronicleIntegrityService
                        .LoadCorrections(dataPath));

                File.WriteAllText(
                    markdownPath,
                    markdown,
                    new UTF8Encoding(false));

                if (profile.chronicleIntegrityEnabled)
                {
                    DeverQuestChronicleIntegrityService.Seal(
                        dataPath,
                        profile.developerName,
                        existingIndex >= 0
                            ? "Chronicle Rewritten"
                            : "Quest Appended",
                        completedSession.sessionId);
                }

                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        private static DeverQuestDailyRecord LoadRecord(
            string dataPath,
            string developerName,
            string dateKey)
        {
            if (!File.Exists(dataPath))
            {
                return CreateRecord(developerName, dateKey);
            }

            string json = File.ReadAllText(dataPath);

            DeverQuestDailyRecord record =
                JsonUtility.FromJson<DeverQuestDailyRecord>(json);

            if (record == null)
            {
                throw new InvalidDataException(
                    "The existing DeverQuest daily-data file could not be read.");
            }

            record.developerName = developerName;
            record.localDate = dateKey;

            if (record.sessions == null)
            {
                record.sessions = new List<DeverQuestSession>();
            }

            return record;
        }

        private static DeverQuestDailyRecord CreateRecord(
            string developerName,
            string dateKey)
        {
            return new DeverQuestDailyRecord
            {
                developerName = developerName,
                localDate = dateKey,
                chronicleIndex = 1,
                sessions = new List<DeverQuestSession>()
            };
        }

        internal static bool TryRegenerate(
            string dataPath,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                DeverQuestDailyRecord record =
                    JsonUtility.FromJson<DeverQuestDailyRecord>(
                        File.ReadAllText(dataPath));
                DateTime date = DateTime.ParseExact(
                    record.localDate, "yyyy-MM-dd",
                    CultureInfo.InvariantCulture);
                string suffix = ".deverquest.json";
                string markdownPath = dataPath.EndsWith(suffix)
                    ? dataPath.Substring(
                        0, dataPath.Length - suffix.Length) + ".md"
                    : Path.ChangeExtension(dataPath, ".md");
                File.WriteAllText(markdownPath,
                    BuildMarkdown(record, date,
                        DeverQuestChronicleIntegrityService
                            .LoadCorrections(dataPath)),
                    new UTF8Encoding(false));
                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        private static string BuildMarkdown(
            DeverQuestDailyRecord record,
            DateTime localDate,
            IReadOnlyList<DeverQuestCorrection> corrections)
        {
            double totalFocused = record.sessions.Sum(
                session => session.accumulatedFocusedSeconds);

            double totalPaused = record.sessions.Sum(
                session => session.accumulatedPausedSeconds);

            int totalCommits = record.sessions.Sum(
                session => session.commitEntries?.Count ?? 0);

            int totalBreaks = record.sessions.Sum(
                session => session.wellnessEvents?.Count(
                    wellnessEvent =>
                        wellnessEvent.action == "Break Started") ?? 0);
            int totalAttachments = record.sessions.Sum(
                session => session.mediaAttachments?.Count ?? 0);
            double totalExternalSeconds = record.sessions.Sum(
                session => session.externalActivityEvents?.Where(
                    activity => activity.action == "Activity Ended")
                    .Sum(activity => activity.durationSeconds) ?? 0d);

            double totalRewardMinutes = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.minutes > 0d)
                    .Sum(transaction => transaction.minutes) ?? 0d);
            long totalCopper = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.copper > 0L)
                    .Sum(transaction => transaction.copper) ?? 0L);
            long totalExperience = record.sessions.Sum(
                session => session.rewardTransactions?.Where(
                    transaction => transaction.experience > 0L)
                    .Sum(transaction => transaction.experience) ?? 0L);
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;

            StringBuilder builder = new StringBuilder();

            builder.AppendLine("# DeverQuest Daily Timecard");
            builder.AppendLine();
            builder.AppendLine($"**Developer:** {Escape(record.developerName)}  ");
            builder.AppendLine(
                $"**Adventurer:** {Escape(string.IsNullOrWhiteSpace(adventurer.characterName) ? record.developerName : adventurer.characterName)}  ");
            builder.AppendLine(
                $"**Class:** {Escape(adventurer.characterClass)} · " +
                $"**Level:** {adventurer.level} · " +
                $"**Guild Rank:** {Escape(adventurer.guildRank)}  ");
            builder.AppendLine(
                $"**Ancestry:** " +
                $"{Escape(string.IsNullOrWhiteSpace(adventurer.ancestryName) ? "Legacy / Not Assigned" : adventurer.ancestryName)} · " +
                $"**Alignment:** " +
                $"{Escape(ObjectNames.NicifyVariableName(adventurer.alignment.ToString()))} · " +
                $"**Faith:** " +
                $"{Escape(string.IsNullOrWhiteSpace(adventurer.deityName) ? "Agnostic" : adventurer.deityName)}  ");
            builder.AppendLine(
                $"**Character Rules:** " +
                $"HP {adventurer.currentHitPoints}/" +
                $"{adventurer.maximumHitPoints} · " +
                $"Mana {adventurer.currentMana}/" +
                $"{adventurer.maximumMana} · " +
                $"AC {DeverQuestRulesService.ArmorClass(adventurer)} · " +
                $"Proficiency +" +
                $"{DeverQuestRulesService.ProficiencyBonus(adventurer.level)}  ");
            builder.AppendLine(
                $"**Adventurer Needs:** Hunger {adventurer.hunger} · " +
                $"Rest {adventurer.rest} · " +
                $"Happiness {adventurer.happiness}  ");
            DeverQuestCompanionState activeCompanion =
                DeverQuestCompanionService.ActiveCompanion(
                    adventurer);
            if (activeCompanion != null)
            {
                DeverQuestCompanionProfile companionProfile =
                    DeverQuestCompanionService.FindProfile(
                        activeCompanion.profileId);
                builder.AppendLine(
                    $"**Active Companion:** " +
                    $"{Escape(DeverQuestCompanionService.DisplayName(activeCompanion))} · " +
                    $"Level {activeCompanion.level} · " +
                    $"HP {activeCompanion.currentHitPoints}/" +
                    $"{DeverQuestCompanionService.MaximumHitPoints(activeCompanion, companionProfile)} · " +
                    $"Loyalty {activeCompanion.loyalty}/100  ");
            }
            builder.AppendLine($"**Date:** {localDate:MMMM d, yyyy}  ");
            builder.AppendLine(
                $"**Chronicle:** {Math.Max(1, record.chronicleIndex)}  ");
            builder.AppendLine($"**Sessions:** {record.sessions.Count}");
            builder.AppendLine();
            builder.AppendLine("## Daily Totals");
            builder.AppendLine();
            builder.AppendLine($"- **Focused Work:** {FormatDuration(totalFocused)}");
            builder.AppendLine($"- **Paused Time:** {FormatDuration(totalPaused)}");
            builder.AppendLine($"- **Commit Entries:** {totalCommits}");
            builder.AppendLine($"- **Breaks Taken:** {totalBreaks}");
            builder.AppendLine(
                $"- **External Craft Activity:** " +
                $"{FormatDuration(totalExternalSeconds)}");
            builder.AppendLine(
                $"- **Media Attachments:** {totalAttachments}");
            builder.AppendLine(
                $"- **Coin Earned:** {DeverQuestAdventurerService.FormatCoins(totalCopper)}");
            builder.AppendLine(
                $"- **Experience Earned:** {totalExperience} XP");
            builder.AppendLine(
                $"- **Ending Coin Purse:** " +
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            DeverQuestCarrySummary carry =
                DeverQuestEncumbranceService.Summary(adventurer);
            builder.AppendLine(
                $"- **Carry Weight:** " +
                $"{carry.TotalWeight:0.0} / {carry.Capacity:0.0} " +
                $"({carry.Status}; inventory {carry.InventoryWeight:0.0}, " +
                $"coin {carry.CoinWeight:0.0})");
            builder.AppendLine(
                $"- **Inventory:** " +
                (adventurer.inventory.Count == 0
                    ? "Empty"
                    : string.Join(
                        ", ",
                        adventurer.inventory.Select(
                            item =>
                                $"{item.displayName} ×{item.quantity} " +
                                $"[{item.itemCategory}; " +
                                $"{(string.IsNullOrWhiteSpace(item.originSource) ? item.acquisitionSource : item.originSource)}]"))));
            if (totalRewardMinutes > 0d)
            {
                builder.AppendLine(
                    $"- **Legacy Reward Time:** " +
                    $"{totalRewardMinutes:0.#} minutes");
            }
            builder.AppendLine();
            builder.AppendLine("---");

            for (int index = 0; index < record.sessions.Count; index++)
            {
                AppendSession(
                    builder,
                    record.sessions[index],
                    index + 1,
                    adventurer);
            }

            AppendCorrections(builder, corrections);

            builder.AppendLine();
            builder.AppendLine(
                "_Generated by DeverQuest Developer Companion._");

            return builder.ToString();
        }

        private static void AppendCorrections(
            StringBuilder builder,
            IReadOnlyList<DeverQuestCorrection> corrections)
        {
            if (corrections == null || corrections.Count == 0)
            {
                return;
            }
            builder.AppendLine();
            builder.AppendLine("## Chronicle Corrections");
            builder.AppendLine();
            builder.AppendLine(
                "_Corrections are appended; original Quest records remain unchanged._");
            foreach (DeverQuestCorrection correction in corrections)
            {
                builder.AppendLine();
                builder.AppendLine(
                    $"### {Escape(correction.sessionTitle)} — " +
                    $"{Escape(correction.status)}");
                builder.AppendLine();
                builder.AppendLine(
                    $"- **Requested By:** {Escape(correction.requestedBy)}");
                builder.AppendLine(
                    $"- **Requested:** {Escape(correction.requestedUtc)}");
                builder.AppendLine(
                    $"- **Reason:** {Escape(correction.reason)}");
                builder.AppendLine(
                    $"- **Corrected Record:** " +
                    $"{EscapeMultiline(correction.correctedValue)}");
                if (!string.IsNullOrWhiteSpace(correction.reviewedBy))
                {
                    builder.AppendLine(
                        $"- **Reviewed By:** " +
                        $"{Escape(correction.reviewedBy)}");
                }
            }
        }

        private static string BuildBaseFileName(
            string dateKey,
            string safeDeveloperName,
            int chronicleIndex)
        {
            string baseName =
                $"{dateKey}_{safeDeveloperName}_Timecard";
            return chronicleIndex <= 1
                ? baseName
                : $"{baseName}_Chronicle_{chronicleIndex:00}";
        }

        private static bool ShouldRollOver(
            DeverQuestProfile profile,
            string dataPath,
            DeverQuestDailyRecord record,
            string sessionId)
        {
            if (record.sessions.Any(
                    item => item != null &&
                            item.sessionId == sessionId))
            {
                return false;
            }
            if (record.sessions.Count >= profile.chronicleMaxSessions)
            {
                return true;
            }
            return File.Exists(dataPath) &&
                   new FileInfo(dataPath).Length >=
                   profile.chronicleMaxKilobytes * 1024L;
        }

        private static void AppendSession(
            StringBuilder builder,
            DeverQuestSession session,
            int sessionNumber,
            DeverQuestAdventurer adventurer)
        {
            DateTime start =
                DeverQuestSessionStore.GetLocalStartTime(session);

            DateTime completed =
                DeverQuestSessionStore.GetLocalCompletionTime(session);

            builder.AppendLine();
            builder.AppendLine(
                $"## Session {sessionNumber} — {Escape(session.taskName)}");
            builder.AppendLine();
            builder.AppendLine($"- **Project:** {Escape(session.projectName)}");
            builder.AppendLine($"- **Department:** {Escape(session.category)}");
            if (session.usesQuestProfile)
            {
                builder.AppendLine(
                    $"- **Quest Profile:** " +
                    $"{Escape(session.questProfileName)}");
                builder.AppendLine(
                    $"- **Profile Spoils:** " +
                    $"{DeverQuestAdventurerService.FormatCoins(session.questBaseCopper)} " +
                    $"+ {session.questBaseExperience} XP base; " +
                    $"{DeverQuestAdventurerService.FormatCoins(session.questCopperPerWorkBlock)} " +
                    $"+ {session.questExperiencePerWorkBlock} XP per " +
                    $"{session.questWorkBlockMinutes}m block");
            }
            if (session.usesQuestContract)
            {
                builder.AppendLine(
                    $"- **Quest Contract:** " +
                    $"{Escape(session.questContractTitle)}");
                if (!string.IsNullOrWhiteSpace(
                        session.questContractRunId))
                {
                    builder.AppendLine(
                        $"- **Quest Run:** " +
                        $"`{Escape(session.questContractRunId)}`");
                }
                builder.AppendLine(
                    $"- **Contract Creator:** " +
                    $"{Escape(session.questContractCreator)}");
                builder.AppendLine(
                    $"- **Assigned Adventurer:** " +
                    $"{Escape(session.questContractAssignee)}");
                builder.AppendLine(
                    $"- **Priority:** " +
                    $"{Escape(session.questContractPriority)}");
                builder.AppendLine(
                    $"- **Due Date:** " +
                    $"{Escape(string.IsNullOrWhiteSpace(session.questContractDueDate) ? "Unscheduled" : session.questContractDueDate)}");
                if (session.questIsGroupQuest)
                {
                    builder.AppendLine(
                        $"- **Party Quest:** " +
                        $"{session.questPartyMembers} · " +
                        $"{session.questMaximumParticipants} max");
                }
            }
            builder.AppendLine($"- **Started:** {start:h:mm tt}");
            builder.AppendLine($"- **Ended:** {completed:h:mm tt}");
            builder.AppendLine(
                $"- **Focused Work:** {FormatDuration(session.accumulatedFocusedSeconds)}");
            builder.AppendLine(
                $"- **Paused Time:** {FormatDuration(session.accumulatedPausedSeconds)}");
            double classifiedPaused =
                session.meditationSeconds +
                session.approvedBreakSeconds +
                session.idleUnverifiedSeconds;
            double legacyUnclassified =
                Math.Max(0d,
                    session.accumulatedPausedSeconds -
                    classifiedPaused);
            builder.AppendLine(
                $"- **Time Classification:** Focused " +
                $"{FormatDuration(session.accumulatedFocusedSeconds)} · " +
                $"Meditation {FormatDuration(session.meditationSeconds)} · " +
                $"Approved Break " +
                $"{FormatDuration(session.approvedBreakSeconds)} · " +
                $"Idle/Unverified " +
                $"{FormatDuration(session.idleUnverifiedSeconds)}" +
                (legacyUnclassified > 0d
                    ? $" · Legacy Unclassified " +
                      $"{FormatDuration(legacyUnclassified)}"
                    : string.Empty));
            if (session.meditationHitPointsRestored > 0 ||
                session.meditationManaRestored > 0)
            {
                builder.AppendLine(
                    $"- **Meditation Recovery:** +" +
                    $"{session.meditationHitPointsRestored} HP · +" +
                    $"{session.meditationManaRestored} Mana");
            }

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                builder.AppendLine();
                builder.AppendLine("### Goal");
                builder.AppendLine();
                builder.AppendLine(EscapeMultiline(session.goal));
            }

            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                builder.AppendLine();
                builder.AppendLine("### Quest Story");
                builder.AppendLine();
                builder.AppendLine(
                    EscapeMultiline(session.questStory));
            }

            if (session.questStages != null &&
                session.questStages.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Encounters");
                builder.AppendLine();
                foreach (DeverQuestSessionStage stage
                         in session.questStages)
                {
                    builder.AppendLine(
                        $"- **{(stage.completed ? "Completed" : "Open")}: " +
                        $"{Escape(string.IsNullOrWhiteSpace(stage.stageTitle) ? "Encounter" : stage.stageTitle)}** · " +
                        $"{stage.focusedMinutesRequired}m" +
                        (stage.completedEarly
                            ? $" · Early at " +
                              $"{stage.elapsedFocusedSeconds / 60d:0.0}m"
                            : string.Empty) +
                        (stage.survivalMode
                            ? $" · Survival wave {stage.survivalWave}" +
                              (stage.survivalEndedSafely
                                  ? " · Returned safely" +
                                    (string.IsNullOrWhiteSpace(
                                         stage.survivalExitMethod)
                                        ? string.Empty
                                        : " via " +
                                          Escape(
                                              DeverQuestCombatSummaryService
                                                  .FriendlyExitMethod(
                                                      stage.survivalExitMethod)))
                                  : string.Empty)
                            : string.Empty) +
                        (string.IsNullOrWhiteSpace(
                             stage.assignedPartyRole)
                            ? string.Empty
                            : $" · {Escape(stage.assignedPartyRole)}") +
                        $" · " +
                        $"{DeverQuestAdventurerService.FormatCoins(stage.copperReward)} " +
                        $"+ {stage.experienceReward} XP");
                }
            }

            if (session.battleResults != null &&
                session.battleResults.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Battle Chronicle");
                builder.AppendLine();
                foreach (DeverQuestBattleResult battle
                         in session.battleResults)
                {
                    string battleStatus =
                        battle.safetyPaused
                            ? "Safety Pause"
                            : battle.victory
                                ? battle.earlyVictory
                                    ? "Early Victory"
                                    : "Victory"
                                : "Defeat";
                    builder.AppendLine(
                        $"#### {battleStatus} — " +
                        Escape(battle.encounterName));
                    builder.AppendLine();
                    builder.AppendLine(
                        $"- **Stage:** {Escape(battle.stageTitle)}");
                    builder.AppendLine(
                        $"- **Seed:** `{Escape(battle.seed)}`");
                    builder.AppendLine(
                        $"- **Rounds:** {battle.rounds}" +
                        (battle.parRounds > 0
                            ? $" · Par {battle.parRounds}"
                            : string.Empty));
                    if (battle.survivalWave > 0)
                    {
                        builder.AppendLine(
                            $"- **Survival Wave:** " +
                            $"{battle.survivalWave}");
                    }
                    builder.AppendLine(
                        $"- **Hit Points:** {battle.startingHitPoints} → " +
                        $"{battle.endingHitPoints}");
                    if (!string.IsNullOrWhiteSpace(
                            battle.companionName))
                    {
                        builder.AppendLine(
                            $"- **Companion:** " +
                            $"{Escape(battle.companionName)} · HP " +
                            $"{battle.companionStartingHitPoints} → " +
                            $"{battle.companionEndingHitPoints} · " +
                            $"Level {battle.companionLevelBefore} → " +
                            $"{battle.companionLevelAfter} · " +
                            $"+{battle.companionExperienceEarned} XP" +
                            (battle.companionFell
                                ? " · Fell"
                                : string.Empty));
                    }
                    builder.AppendLine(
                        $"- **Outcome:** " +
                        Escape(
                            DeverQuestCombatSummaryService
                                .OutcomeSummary(battle)));
                    builder.AppendLine(
                        $"- **Damage Report:** " +
                        Escape(
                            DeverQuestCombatSummaryService
                                .DamageSummary(
                                    battle,
                                    session.developerName,
                                    adventurer == null ||
                                    string.IsNullOrWhiteSpace(
                                        adventurer.characterName)
                                        ? session.developerName
                                        : adventurer.characterName)));
                    string conditionSummary =
                        DeverQuestCombatSummaryService
                            .ConditionSummary(battle);
                    if (!string.IsNullOrWhiteSpace(conditionSummary))
                    {
                        builder.AppendLine(
                            $"- **Conditions and Reactions:** " +
                            Escape(conditionSummary));
                    }
                    string companionContribution =
                        DeverQuestCombatSummaryService
                            .CompanionContributionSummary(battle);
                    if (!string.IsNullOrWhiteSpace(
                            companionContribution))
                    {
                        builder.AppendLine(
                            $"- **Companion Contribution:** " +
                            Escape(companionContribution));
                    }
                    builder.AppendLine(
                        $"- **Bonus Rewards:** " +
                        $"{DeverQuestAdventurerService.FormatCoins(battle.bonusCopper)} " +
                        $"+ {battle.bonusExperience} XP");
                    if (battle.defeatedMonsters.Count > 0)
                    {
                        builder.AppendLine(
                            $"- **Defeated:** " +
                            string.Join(
                                ", ",
                                battle.defeatedMonsters.Select(Escape)));
                    }
                    if (battle.damageEvents != null &&
                        battle.damageEvents.Count > 0)
                    {
                        builder.AppendLine(
                            $"- **Typed Damage:** " +
                            Escape(
                                string.IsNullOrWhiteSpace(
                                    battle.typedDamageSummary)
                                    ? DeverQuestDamageService
                                        .DescribeBattle(
                                            battle.damageEvents)
                                    : battle.typedDamageSummary));
                    }
                    if (battle.loot.Count > 0)
                    {
                        builder.AppendLine(
                            $"- **Loot:** " +
                            string.Join(
                                ", ",
                                battle.loot.Select(Escape)));
                    }
                    if (!string.IsNullOrWhiteSpace(battle.injury))
                    {
                        builder.AppendLine(
                            $"- **Consequence:** " +
                            Escape(battle.injury));
                    }
                    if (!string.IsNullOrWhiteSpace(
                            battle.safetyPauseReason))
                    {
                        builder.AppendLine(
                            $"- **Safety Pause:** " +
                            Escape(battle.safetyPauseReason));
                    }
                    if (battle.actionEvents != null &&
                        battle.actionEvents.Count > 0)
                    {
                        builder.AppendLine();
                        builder.AppendLine("**Tactical Actions**");
                        builder.AppendLine();
                        foreach (DeverQuestCombatActionEvent action
                                 in battle.actionEvents)
                        {
                            builder.AppendLine(
                                $"- **Round {action.round}:** " +
                                $"{Escape(action.actor)} used " +
                                $"{Escape(action.actionName)} on " +
                                $"{Escape(action.target)}" +
                                (action.manaSpent > 0
                                    ? $" · {action.manaSpent} mana"
                                    : string.Empty) +
                                (action.effects.Count > 0
                                    ? $" · " +
                                      string.Join(
                                          ", ",
                                          action.effects.Select(Escape))
                                    : string.Empty));
                        }
                    }
                    IReadOnlyList<string> highlights =
                        DeverQuestCombatSummaryService.Highlights(
                            battle,
                            10);
                    if (highlights.Count > 0)
                    {
                        builder.AppendLine();
                        builder.AppendLine("**Combat Highlights**");
                        builder.AppendLine();
                        foreach (string line in highlights)
                        {
                            builder.AppendLine(
                                $"- {Escape(line)}");
                        }
                    }
                    if (battle.combatLog.Count > 0)
                    {
                        builder.AppendLine();
                        builder.AppendLine("<details>");
                        builder.AppendLine(
                            $"<summary>Full combat log " +
                            $"({battle.combatLog.Count} entries)</summary>");
                        builder.AppendLine();
                        foreach (string line in battle.combatLog)
                        {
                            builder.AppendLine(
                                $"- {Escape(line)}");
                        }
                        builder.AppendLine();
                        builder.AppendLine("</details>");
                    }
                    builder.AppendLine();
                }
            }

            if (session.usesQuestContract &&
                !string.IsNullOrWhiteSpace(
                    session.questContractDeliverables))
            {
                builder.AppendLine();
                builder.AppendLine("### Contract Deliverables");
                builder.AppendLine();
                builder.AppendLine(
                    EscapeMultiline(
                        session.questContractDeliverables));
            }

            if (session.usesQuestContract &&
                (!string.IsNullOrWhiteSpace(
                     session.questEncounterProfileId) ||
                 !string.IsNullOrWhiteSpace(
                     session.questEncounterNotes)))
            {
                builder.AppendLine();
                builder.AppendLine("### Reserved Encounter");
                builder.AppendLine();
                if (!string.IsNullOrWhiteSpace(
                        session.questEncounterProfileId))
                {
                    builder.AppendLine(
                        $"- **Encounter Profile:** " +
                        $"{Escape(session.questEncounterProfileId)}");
                }
                if (!string.IsNullOrWhiteSpace(
                        session.questEncounterNotes))
                {
                    builder.AppendLine(
                        EscapeMultiline(
                            session.questEncounterNotes));
                }
            }

            builder.AppendLine();
            builder.AppendLine("### Commit Journal");
            builder.AppendLine();

            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                builder.AppendLine("- No commit entries recorded.");
            }
            else
            {
                foreach (DeverQuestCommitEntry entry in session.commitEntries)
                {
                    string entryType =
                        string.IsNullOrWhiteSpace(entry.entryType)
                            ? "Legacy Entry"
                            : entry.entryType;
                    DateTime created = new DateTime(
                            entry.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();

                    builder.Append(
                        $"- **{created:h:mm tt}** " +
                        $"[{Escape(entryType)}] " +
                        $"`+{FormatDuration(entry.focusedSecondsAtEntry)}` — " +
                        Escape(entry.comment));

                    if (!string.IsNullOrWhiteSpace(entry.branch))
                    {
                        builder.Append(
                            $" | Branch: `{EscapeCode(entry.branch)}`");
                    }

                    if (!string.IsNullOrWhiteSpace(entry.commitHash))
                    {
                        builder.Append(
                            $" | Commit: `{EscapeCode(entry.commitHash)}`");
                    }

                    builder.AppendLine();
                }
            }

            builder.AppendLine();
            builder.AppendLine("### Closing Notes");
            builder.AppendLine();

            builder.AppendLine(
                string.IsNullOrWhiteSpace(session.closingNotes)
                    ? "No closing notes recorded."
                    : EscapeMultiline(session.closingNotes));

            if (session.wellnessEvents != null &&
                session.wellnessEvents.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Wellness Journal");
                builder.AppendLine();

                foreach (DeverQuestWellnessEvent wellnessEvent
                         in session.wellnessEvents)
                {
                    DateTime created = new DateTime(
                            wellnessEvent.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();

                    builder.AppendLine(
                        $"- **{created:h:mm tt}** — " +
                        $"{wellnessEvent.type}: " +
                        $"{Escape(wellnessEvent.action)}");
                }
            }

            if (session.externalActivityEvents != null &&
                session.externalActivityEvents.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### External Activity Journal");
                builder.AppendLine();

                foreach (DeverQuestExternalActivityEvent activity
                         in session.externalActivityEvents)
                {
                    DateTime created = new DateTime(
                            activity.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();
                    string duration = activity.durationSeconds > 0d
                        ? $" · {FormatDuration(activity.durationSeconds)}"
                        : string.Empty;
                    builder.AppendLine(
                        $"- **{created:h:mm tt}** — " +
                        $"{Escape(activity.toolName)}: " +
                        $"{Escape(activity.action)}{duration}");
                }
            }

            if (session.mediaAttachments != null &&
                session.mediaAttachments.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Media Attachments");
                builder.AppendLine();

                foreach (DeverQuestMediaAttachment attachment
                         in session.mediaAttachments)
                {
                    string duration = attachment.durationSeconds > 0d
                        ? $" · {FormatDuration(attachment.durationSeconds)}"
                        : string.Empty;
                    builder.AppendLine(
                        $"- **{Escape(attachment.attachmentType)}:** " +
                        $"[{Escape(attachment.displayName)}]" +
                        $"({EscapeLinkTarget(attachment.filePath)})" +
                        duration);
                }
            }

            if (session.rewardTransactions != null &&
                session.rewardTransactions.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("### Reward Journal");
                builder.AppendLine();

                foreach (DeverQuestRewardTransaction transaction
                         in session.rewardTransactions)
                {
                    if (transaction.copper != 0L ||
                        transaction.experience != 0L)
                    {
                        string coinSign =
                            transaction.copper >= 0L ? "+" : "-";
                        string coinText =
                            DeverQuestAdventurerService.FormatCoins(
                                Math.Abs(transaction.copper));
                        string levelText =
                            transaction.endingLevel >
                            transaction.startingLevel
                                ? $" · Level {transaction.startingLevel} → " +
                                  $"{transaction.endingLevel}"
                                : string.Empty;
                        builder.AppendLine(
                            $"- **{transaction.transactionType}:** " +
                            $"{coinSign}{coinText} · " +
                            $"+{transaction.experience} XP" +
                            $"{levelText} — {Escape(transaction.note)}");
                    }
                    else
                    {
                        string sign =
                            transaction.minutes >= 0d ? "+" : string.Empty;
                        builder.AppendLine(
                            $"- **{transaction.categoryName}:** " +
                            $"{sign}{transaction.minutes:0.#} minutes " +
                            $"({transaction.transactionType}) — " +
                            Escape(transaction.note));
                    }
                }
            }

            builder.AppendLine();
            builder.AppendLine("---");
        }

        private static string FormatDuration(double totalSeconds)
        {
            TimeSpan duration =
                TimeSpan.FromSeconds(Math.Max(0d, totalSeconds));

            return $"{(int)duration.TotalHours}h " +
                   $"{duration.Minutes}m " +
                   $"{duration.Seconds}s";
        }

        private static string Escape(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r", " ")
                .Replace("\n", " ")
                .Replace("|", "\\|");
        }

        private static string EscapeMultiline(string value)
        {
            return (value ?? string.Empty)
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Replace("|", "\\|");
        }

        private static string EscapeCode(string value)
        {
            return (value ?? string.Empty).Replace("`", "'");
        }

        private static string EscapeLinkTarget(string value)
        {
            string path =
                (value ?? string.Empty)
                .Replace('\\', '/')
                .Replace(" ", "%20")
                .Replace("(", "%28")
                .Replace(")", "%29");
            return path.StartsWith(
                    "file:",
                    StringComparison.OrdinalIgnoreCase)
                ? path
                : "file:///" + path.TrimStart('/');
        }
    }
}

//----- DeverQuestTimecardWriter.cs END -----
