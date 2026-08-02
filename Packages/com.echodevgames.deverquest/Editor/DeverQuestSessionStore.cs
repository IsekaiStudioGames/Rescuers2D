//----- DeverQuestSessionStore.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestSessionStore
    {
        public static event Action SessionStarted;
        public static event Action SessionPaused;
        public static event Action SessionResumed;
        public static event Action SessionCompleted;
        public static event Action SessionFinalized;
        public static event Action SessionDiscarded;

        public const int MeditationHitPointsPerMinute = 1;
        public const int MeditationManaPerMinute = 2;

        private const string ActiveSessionKey =
            "EchoDevGames.DeverQuest.ActiveSession.v1";

        private const string LastCompletedSessionKey =
            "EchoDevGames.DeverQuest.LastCompletedSession.v1";

        private static DeverQuestSession activeSession;
        private static DeverQuestSession lastCompletedSession;

        static DeverQuestSessionStore()
        {
            Load();
            EditorApplication.quitting -= PauseBeforeEditorCloses;
            EditorApplication.quitting += PauseBeforeEditorCloses;
        }

        public static DeverQuestSession ActiveSession
        {
            get
            {
                if (activeSession == null)
                {
                    LoadActiveSession();
                }

                return activeSession;
            }
        }

        public static DeverQuestSession LastCompletedSession
        {
            get
            {
                if (lastCompletedSession == null)
                {
                    LoadLastCompletedSession();
                }

                return lastCompletedSession;
            }
        }

        public static bool HasActiveSession =>
            ActiveSession != null &&
            ActiveSession.IsActive;

        public static void StartSession(
            string developerName,
            string projectName,
            string taskName,
            string category,
            string goal,
            DeverQuestQuestProfile questProfile = null,
            DeverQuestQuestContract questContract = null,
            string questContractRunId = "")
        {
            if (HasActiveSession)
            {
                Debug.LogWarning(
                    "[DeverQuest] A session is already active.");
                return;
            }

            long nowTicks = DateTime.UtcNow.Ticks;

            activeSession = new DeverQuestSession
            {
                sessionId = Guid.NewGuid().ToString("N"),
                developerName = developerName,
                projectName = projectName,
                taskName = taskName,
                category = category,
                goal = goal,
                usesQuestProfile =
                    questProfile != null || questContract != null,
                questProfileId =
                    questContract != null
                        ? questContract.questProfile == null
                            ? string.Empty
                            : questContract.questProfile.ProfileId
                        : questProfile == null
                        ? string.Empty
                        : questProfile.ProfileId,
                questProfileName =
                    questContract != null
                        ? questContract.questProfile == null
                            ? string.Empty
                            : questContract.questProfile.displayName
                        : questProfile == null
                        ? string.Empty
                        : questProfile.displayName,
                questSuggestedFocusMinutes =
                    questContract != null
                        ? questContract.suggestedFocusMinutes
                        : questProfile == null
                        ? 0
                        : questProfile.suggestedFocusMinutes,
                questBaseCopper =
                    questContract != null
                        ? questContract.baseCopper
                        : questProfile == null ? 0 : questProfile.baseCopper,
                questBaseExperience =
                    questContract != null
                        ? questContract.baseExperience
                        : questProfile == null
                        ? 0
                        : questProfile.baseExperience,
                questWorkBlockMinutes =
                    questContract != null
                        ? questContract.workBlockMinutes
                        : questProfile == null
                        ? 0
                        : questProfile.workBlockMinutes,
                questCopperPerWorkBlock =
                    questContract != null
                        ? questContract.copperPerWorkBlock
                        : questProfile == null
                        ? 0
                        : questProfile.copperPerWorkBlock,
                questExperiencePerWorkBlock =
                    questContract != null
                        ? questContract.experiencePerWorkBlock
                        : questProfile == null
                        ? 0
                        : questProfile.experiencePerWorkBlock,
                usesQuestContract = questContract != null,
                questContractId =
                    questContract == null
                        ? string.Empty
                        : questContract.ContractId,
                questContractRunId =
                    questContract == null
                        ? string.Empty
                        : questContractRunId?.Trim() ?? string.Empty,
                questContractTitle =
                    questContract == null
                        ? string.Empty
                        : questContract.contractTitle,
                questContractCreator =
                    questContract == null
                        ? string.Empty
                        : questContract.createdBy,
                questContractAssignee =
                    questContract == null
                        ? string.Empty
                        : questContract.assignedAdventurer,
                questContractPriority =
                    questContract == null
                        ? string.Empty
                        : questContract.priority.ToString(),
                questContractDueDate =
                    questContract == null
                        ? string.Empty
                        : questContract.dueDate,
                questContractDeliverables =
                    questContract == null
                        ? string.Empty
                        : questContract.deliverables,
                questEncounterProfileId =
                    questContract == null
                        ? string.Empty
                        : questContract.encounterProfileId,
                questEncounterNotes =
                    questContract == null
                        ? string.Empty
                        : questContract.encounterNotes,
                questIsGroupQuest =
                    questContract != null &&
                    questContract.groupQuest,
                questMaximumParticipants =
                    questContract == null
                        ? 1
                        : questContract.maximumParticipants,
                questPartyMembers =
                    questContract == null ||
                    questContract.partyMembers == null
                        ? string.Empty
                        : string.Join(
                            ", ",
                            questContract.partyMembers.ConvertAll(
                                member =>
                                    $"{member.adventurerName} " +
                                    $"({member.partyRole})")),
                questStory =
                    questContract == null
                        ? string.Empty
                        : questContract.questStory,
                questGroupBonusCopper =
                    questContract == null
                        ? 0
                        : questContract.groupBonusCopper,
                questGroupBonusExperience =
                    questContract == null
                        ? 0
                        : questContract.groupBonusExperience,
                questStages =
                    questContract == null ||
                    questContract.focusStages == null
                        ? new List<DeverQuestSessionStage>()
                        : questContract.focusStages
                            .Select((stage, index) =>
                                new DeverQuestSessionStage
                                {
                                    stageId = stage.stageId,
                                    stageTitle =
                                        string.IsNullOrWhiteSpace(
                                            stage.stageTitle)
                                            ? $"Encounter {index + 1}"
                                            : stage.stageTitle,
                                    workObjective =
                                        stage.workObjective,
                                    focusedMinutesRequired =
                                        stage.focusedMinutesRequired,
                                    assignedPartyRole =
                                        stage.assignedPartyRole,
                                    copperReward =
                                        stage.copperReward,
                                    experienceReward =
                                        stage.experienceReward,
                                    allowEarlyTurnIn =
                                        stage.allowEarlyTurnIn,
                                    earlyCompletionCopperBonus =
                                        stage.earlyCompletionCopperBonus,
                                    earlyCompletionExperienceBonus =
                                        stage.earlyCompletionExperienceBonus,
                                    encounterProfileId =
                                        stage.encounterProfileId
                                })
                            .ToList(),
                state = DeverQuestSessionState.Running,
                startedUtcTicks = nowTicks,
                lastStateChangeUtcTicks = nowTicks
            };

            activeSession.Sanitize();
            SaveActiveSession();
            SessionStarted?.Invoke();
        }

        public static void PauseSession(string reason = "Meditation")
        {
            if (!HasActiveSession ||
                ActiveSession.state != DeverQuestSessionState.Running)
            {
                return;
            }

            long nowTicks = DateTime.UtcNow.Ticks;
            ActiveSession.accumulatedFocusedSeconds +=
                GetSecondsBetween(
                    ActiveSession.lastStateChangeUtcTicks,
                    nowTicks);

            ActiveSession.lastStateChangeUtcTicks = nowTicks;
            ActiveSession.state = DeverQuestSessionState.Paused;
            ActiveSession.pauseReason = reason;
            ActiveSession.idlePauseAcknowledged =
                reason != "Idle Detection" &&
                reason != "Unity Project Lost Focus";
            SaveActiveSession();
            SessionPaused?.Invoke();
        }

        public static bool PauseForApprovedBreak(
            int minutes,
            string permitName,
            DeverQuestWellnessType wellnessType =
                DeverQuestWellnessType.CheckIn,
            bool isWellnessReminder = false)
        {
            if (!HasActiveSession ||
                ActiveSession.state !=
                DeverQuestSessionState.Running)
            {
                return false;
            }
            minutes = Math.Max(1, minutes);
            PauseSession(
                "Approved Break: " +
                (permitName?.Trim() ?? "Guild Permit"));
            ActiveSession.approvedBreakUntilUtcTicks =
                DateTime.UtcNow.AddMinutes(minutes).Ticks;
            ActiveSession.approvedBreakIsWellness =
                isWellnessReminder;
            ActiveSession.approvedBreakWellnessType =
                wellnessType;
            ActiveSession.approvedBreakPlannedMinutes =
                minutes;
            SaveActiveSession();
            return true;
        }

        public static void ResumeSession()
        {
            if (!HasActiveSession ||
                ActiveSession.state != DeverQuestSessionState.Paused)
            {
                return;
            }

            if (!ActiveSession.idlePauseAcknowledged)
            {
                return;
            }

            long nowTicks = DateTime.UtcNow.Ticks;
            double pausedSeconds = GetSecondsBetween(
                    ActiveSession.lastStateChangeUtcTicks,
                    nowTicks);
            ActiveSession.accumulatedPausedSeconds += pausedSeconds;
            if (ActiveSession.pauseReason.StartsWith(
                    "Approved Break:",
                    StringComparison.Ordinal))
            {
                double approvedSeconds = Math.Min(
                    pausedSeconds,
                    Math.Max(
                        0d,
                        GetSecondsBetween(
                            ActiveSession.lastStateChangeUtcTicks,
                            ActiveSession.approvedBreakUntilUtcTicks)));
                ActiveSession.approvedBreakSeconds +=
                    approvedSeconds;
                ActiveSession.idleUnverifiedSeconds +=
                    Math.Max(0d, pausedSeconds - approvedSeconds);
                CompleteWellnessBreakIfEligible(
                    approvedSeconds);
            }
            else if (ActiveSession.pauseReason == "Idle Detection" ||
                ActiveSession.pauseReason == "Unity Project Lost Focus")
            {
                ActiveSession.idleUnverifiedSeconds += pausedSeconds;
            }
            else
            {
                ActiveSession.meditationSeconds += pausedSeconds;
                ApplyMeditationRecovery(
                    pausedSeconds,
                    ActiveSession.pauseReason);
            }

            ActiveSession.lastStateChangeUtcTicks = nowTicks;
            ActiveSession.state = DeverQuestSessionState.Running;
            ActiveSession.pausedByEditorShutdown = false;
            ActiveSession.pauseReason = string.Empty;
            ActiveSession.approvedBreakUntilUtcTicks = 0L;
            ActiveSession.approvedBreakIsWellness = false;
            ActiveSession.approvedBreakPlannedMinutes = 0;
            SaveActiveSession();
            SessionResumed?.Invoke();
        }

        public static bool GetMeditationRecoveryPreview(
            out int completedMinutes,
            out int hitPoints,
            out int mana)
        {
            completedMinutes = 0;
            hitPoints = 0;
            mana = 0;

            if (!HasActiveSession ||
                ActiveSession.state != DeverQuestSessionState.Paused ||
                !IsMeditationPauseReason(ActiveSession.pauseReason))
            {
                return false;
            }

            double pausedSeconds = GetSecondsBetween(
                ActiveSession.lastStateChangeUtcTicks,
                DateTime.UtcNow.Ticks);
            completedMinutes = Math.Max(
                0,
                (int)Math.Floor(pausedSeconds / 60d));

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (adventurer == null ||
                adventurer.isFallen ||
                adventurer.currentHitPoints <= 0)
            {
                return true;
            }

            hitPoints = Math.Min(
                Math.Max(
                    0,
                    adventurer.maximumHitPoints -
                    adventurer.currentHitPoints),
                completedMinutes * MeditationHitPointsPerMinute);
            mana = Math.Min(
                Math.Max(
                    0,
                    adventurer.maximumMana -
                    adventurer.currentMana),
                completedMinutes * MeditationManaPerMinute);
            return true;
        }

        private static void ApplyMeditationRecovery(
            double pausedSeconds,
            string reason)
        {
            if (!IsMeditationPauseReason(reason))
            {
                return;
            }

            int completedMinutes = Math.Max(
                0,
                (int)Math.Floor(pausedSeconds / 60d));
            if (completedMinutes <= 0)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (adventurer == null ||
                adventurer.isFallen ||
                adventurer.currentHitPoints <= 0)
            {
                return;
            }

            int priorHitPoints = adventurer.currentHitPoints;
            int priorMana = adventurer.currentMana;

            adventurer.currentHitPoints = Math.Min(
                adventurer.maximumHitPoints,
                adventurer.currentHitPoints +
                completedMinutes * MeditationHitPointsPerMinute);
            adventurer.currentMana = Math.Min(
                adventurer.maximumMana,
                adventurer.currentMana +
                completedMinutes * MeditationManaPerMinute);

            int restoredHitPoints =
                adventurer.currentHitPoints - priorHitPoints;
            int restoredMana =
                adventurer.currentMana - priorMana;

            if (restoredHitPoints <= 0 && restoredMana <= 0)
            {
                return;
            }

            ActiveSession.meditationHitPointsRestored +=
                restoredHitPoints;
            ActiveSession.meditationManaRestored += restoredMana;
            DeverQuestAdventurerService.Save();
        }

        private static bool IsMeditationPauseReason(string reason)
        {
            return string.Equals(
                       reason,
                       "Meditation",
                       StringComparison.Ordinal) ||
                   string.Equals(
                       reason,
                       "Manual",
                       StringComparison.Ordinal);
        }

        private static void CompleteWellnessBreakIfEligible(
            double approvedSeconds)
        {
            if (!ActiveSession.approvedBreakIsWellness)
            {
                return;
            }

            int plannedMinutes = Math.Max(
                1,
                ActiveSession.approvedBreakPlannedMinutes);
            double requiredSeconds =
                plannedMinutes * 60d * 0.8d;
            bool completed =
                approvedSeconds >= requiredSeconds;

            ActiveSession.wellnessEvents.Add(
                new DeverQuestWellnessEvent
                {
                    type =
                        ActiveSession.approvedBreakWellnessType,
                    action = completed
                        ? "Break Completed"
                        : "Break Ended Early",
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    focusedSecondsAtEvent = GetFocusedSeconds()
                });

            DeverQuestWellnessHistoryService.RecordBreakOutcome(
                ActiveSession.approvedBreakWellnessType,
                completed,
                plannedMinutes,
                approvedSeconds);

            if (!completed)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            switch (ActiveSession.approvedBreakWellnessType)
            {
                case DeverQuestWellnessType.Hydration:
                    adventurer.hunger =
                        Math.Min(100, adventurer.hunger + 5);
                    adventurer.happiness =
                        Math.Min(100, adventurer.happiness + 1);
                    break;
                case DeverQuestWellnessType.Lunch:
                case DeverQuestWellnessType.Dinner:
                    adventurer.hunger =
                        Math.Min(100, adventurer.hunger + 20);
                    adventurer.happiness =
                        Math.Min(100, adventurer.happiness + 3);
                    break;
                case DeverQuestWellnessType.QuietHours:
                    adventurer.rest =
                        Math.Min(100, adventurer.rest + 15);
                    break;
                case DeverQuestWellnessType.MovementBreak:
                case DeverQuestWellnessType.Exercise:
                    adventurer.rest =
                        Math.Min(100, adventurer.rest + 5);
                    adventurer.happiness =
                        Math.Min(100, adventurer.happiness + 3);
                    break;
                default:
                    adventurer.happiness =
                        Math.Min(100, adventurer.happiness + 2);
                    break;
            }

            long experience = Math.Max(
                0,
                DeverQuestSettingsStore.Profile
                    .wellnessBreakExperience);
            DeverQuestProgressionResult result =
                DeverQuestAdventurerService.Award(
                    0L,
                    experience);
            ActiveSession.rewardTransactions.Add(
                new DeverQuestRewardTransaction
                {
                    categoryName = "Adventurer Wellness",
                    transactionType = "Completed Wellness Break",
                    experience = experience,
                    startingLevel = result.StartingLevel,
                    endingLevel = result.EndingLevel,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    note =
                        $"{ActiveSession.approvedBreakWellnessType} " +
                        $"break completed"
                });
        }

        public static void AcknowledgeIdlePause()
        {
            if (!HasActiveSession ||
                ActiveSession.state != DeverQuestSessionState.Paused)
            {
                return;
            }

            ActiveSession.idlePauseAcknowledged = true;
            SaveActiveSession();
        }

        public static void AddCommitEntry(
            string comment,
            string branch,
            string commitHash,
            string entryType = "Quest Log Note")
        {
            if (!HasActiveSession ||
                string.IsNullOrWhiteSpace(comment))
            {
                return;
            }

            DeverQuestCommitEntry entry =
                new DeverQuestCommitEntry
                {
                    entryId = Guid.NewGuid().ToString("N"),
                    comment = comment,
                    branch = branch,
                    commitHash = commitHash,
                    entryType = entryType,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    focusedSecondsAtEntry = GetFocusedSeconds()
                };

            entry.Sanitize();
            ActiveSession.commitEntries.Add(entry);
            SaveActiveSession();
        }

        public static void RemoveCommitEntry(string entryId)
        {
            if (!HasActiveSession ||
                string.IsNullOrWhiteSpace(entryId))
            {
                return;
            }

            ActiveSession.commitEntries.RemoveAll(
                entry => entry != null &&
                         entry.entryId == entryId);

            SaveActiveSession();
        }

        public static List<string> UpdateQuestStages()
        {
            List<string> completedTitles = new List<string>();
            if (!HasActiveSession ||
                ActiveSession.questStages == null ||
                ActiveSession.questStages.Count == 0)
            {
                return completedTitles;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            double focused = GetFocusedSeconds();
            bool changed = false;
            foreach (DeverQuestSessionStage stage
                     in ActiveSession.questStages)
            {
                bool assigned =
                    string.IsNullOrWhiteSpace(
                        stage.assignedPartyRole) ||
                    string.Equals(
                        stage.assignedPartyRole,
                        adventurer.homeDepartment,
                        StringComparison.OrdinalIgnoreCase);
                if (!assigned)
                {
                    continue;
                }
                if (stage.completed)
                {
                    continue;
                }
                bool survival =
                    DeverQuestEncounterService.IsSurvival(stage);
                stage.survivalMode = survival;
                if (survival)
                {
                    int interval =
                        DeverQuestEncounterService
                            .SurvivalIntervalMinutes(stage);
                    if (stage.nextSurvivalWaveFocusedSeconds <= 0d)
                    {
                        stage.nextSurvivalWaveFocusedSeconds =
                            stage.startedFocusedSeconds +
                            interval * 60d;
                        changed = true;
                    }
                    if (!stage.survivalFightPaused &&
                        focused >=
                        stage.nextSurvivalWaveFocusedSeconds)
                    {
                        DeverQuestBattleResult survivalBattle =
                            DeverQuestEncounterService.Resolve(
                                ActiveSession,
                                stage,
                                DeverQuestSettingsStore.Profile
                                    .dailyDecreeCheckModifier);
                        stage.nextSurvivalWaveFocusedSeconds +=
                            interval * 60d;
                        completedTitles.Add(
                            survivalBattle == null
                                ? $"{stage.stageTitle} · wave unavailable"
                                : $"{stage.stageTitle} · wave " +
                                  $"{stage.survivalWave} · " +
                                  BattleOutcome(survivalBattle));
                        if (survivalBattle?.safetyPaused == true &&
                            ActiveSession.state ==
                            DeverQuestSessionState.Running)
                        {
                            PauseSession(
                                "Combat Safety: " +
                                survivalBattle.safetyPauseReason);
                        }
                        changed = true;
                    }
                    break;
                }
                double elapsed =
                    Math.Max(0d, focused - stage.startedFocusedSeconds);
                if (elapsed <
                    Math.Max(1, stage.focusedMinutesRequired) * 60d)
                {
                    break;
                }
                DeverQuestBattleResult battle =
                    CompleteStage(stage, false, focused);
                completedTitles.Add(
                    battle == null
                        ? stage.stageTitle
                        : $"{stage.stageTitle} · " +
                          BattleOutcome(battle));
                changed = true;
            }
            if (changed)
            {
                SaveActiveSession();
            }
            return completedTitles;
        }

        public static bool CompleteCurrentStageEarly(
            out string message)
        {
            message = string.Empty;
            DeverQuestSessionStage stage = CurrentQuestStage();
            if (stage == null)
            {
                message = "There is no active Quest stage to report.";
                return false;
            }
            if (stage.survivalMode ||
                DeverQuestEncounterService.IsSurvival(stage))
            {
                message =
                    "Survival stages end through Flee, Return, or the " +
                    "Guild wagon—not early objective turn-in.";
                return false;
            }
            if (!stage.allowEarlyTurnIn)
            {
                message =
                    "This stage does not permit early objective turn-in.";
                return false;
            }
            double focused = GetFocusedSeconds();
            double elapsed =
                Math.Max(0d, focused - stage.startedFocusedSeconds);
            double target =
                Math.Max(1, stage.focusedMinutesRequired) * 60d;
            if (elapsed >= target)
            {
                message =
                    "The Focus target has already been reached; normal " +
                    "completion will be recorded.";
                UpdateQuestStages();
                return true;
            }
            DeverQuestBattleResult battle =
                CompleteStage(stage, true, focused);
            SaveActiveSession();
            message =
                $"{stage.stageTitle} reported early at " +
                $"{elapsed / 60d:0.0}/{target / 60d:0.#} minutes" +
                (battle == null
                    ? "."
                    : $" · {BattleOutcome(battle)}.");
            return true;
        }

        public static bool TryExitSurvival(
            string method,
            out string message)
        {
            message = string.Empty;
            DeverQuestSessionStage stage = CurrentQuestStage();
            if (stage == null ||
                (!stage.survivalMode &&
                 !DeverQuestEncounterService.IsSurvival(stage)))
            {
                message = "No survival expedition is active.";
                return false;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            string normalized = method?.Trim() ?? string.Empty;
            bool success;
            if (normalized == "Return")
            {
                success =
                    DeverQuestTacticalCombatService.HasReturnAbility(
                        adventurer);
                message = success
                    ? "A prepared homeward passage returns the party."
                    : "This Adventurer has no prepared return ability.";
            }
            else if (normalized == "Wagon")
            {
                success = stage.survivalExitOffered;
                message = success
                    ? "The Guild wagon carries the party and spoils home."
                    : "The Guild wagon has not reached this checkpoint.";
            }
            else
            {
                stage.survivalFleeAttempts++;
                int difficultyClass =
                    10 + Math.Max(0, stage.survivalWave / 2);
                DeverQuestRuleResult flee =
                    DeverQuestRulesService.ResolveCheck(
                        adventurer,
                        DeverQuestAbility.Agility,
                        true,
                        difficultyClass,
                        $"{ActiveSession.sessionId}:{stage.stageId}:" +
                        $"flee:{stage.survivalWave}:" +
                        $"{stage.survivalFleeAttempts}",
                        DeverQuestSettingsStore.Profile
                            .dailyDecreeCheckModifier);
                success = flee.Success;
                message = success
                    ? $"Safe escape succeeded ({flee.Formula})."
                    : $"Escape failed ({flee.Formula}); the fight " +
                      "remains paused before another enemy turn.";
            }
            if (!success)
            {
                stage.survivalFightPaused = true;
                stage.survivalPauseReason = message;
                if (ActiveSession.state ==
                    DeverQuestSessionState.Running)
                {
                    PauseSession("Combat Safety: failed escape");
                }
                SaveActiveSession();
                return false;
            }
            stage.survivalFightPaused = false;
            stage.survivalEndedSafely = true;
            stage.survivalExitMethod = normalized;
            stage.survivalExitSummary = message;
            stage.survivalExitUtcTicks = DateTime.UtcNow.Ticks;
            CompleteStage(stage, false, GetFocusedSeconds());
            DeverQuestGuildAccountService.AddAudit(
                "Survival Expedition Exit",
                string.IsNullOrWhiteSpace(stage.stageTitle)
                    ? "Survival Encounter"
                    : stage.stageTitle,
                $"{adventurer.characterName} · {normalized} · " +
                $"wave {stage.survivalWave}");
            SaveActiveSession();
            return true;
        }

        public static bool ContinueSurvival(out string message)
        {
            DeverQuestSessionStage stage = CurrentQuestStage();
            if (stage == null || !stage.survivalMode)
            {
                message = "No survival expedition is active.";
                return false;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            if (DeverQuestEncumbranceService.IsEncumbered(adventurer))
            {
                message =
                    "Drop carried loot or exchange coin at the Guild " +
                    "Hall before continuing.";
                return false;
            }
            int threshold = Math.Max(
                1, adventurer.maximumHitPoints / 4);
            if (adventurer.currentHitPoints <= threshold)
            {
                message =
                    "Recover above one-quarter Hit Points before " +
                    "continuing.";
                return false;
            }
            stage.survivalFightPaused = false;
            stage.survivalPauseReason = string.Empty;
            stage.survivalExitOffered = false;
            SaveActiveSession();
            message = "The survival expedition continues.";
            return true;
        }

        public static DeverQuestSessionStage CurrentQuestStage()
        {
            if (!HasActiveSession ||
                ActiveSession.questStages == null)
            {
                return null;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            return ActiveSession.questStages.FirstOrDefault(stage =>
                stage != null &&
                !stage.completed &&
                (string.IsNullOrWhiteSpace(
                     stage.assignedPartyRole) ||
                 string.Equals(
                     stage.assignedPartyRole,
                     adventurer.homeDepartment,
                     StringComparison.OrdinalIgnoreCase)));
        }

        private static DeverQuestBattleResult CompleteStage(
            DeverQuestSessionStage stage,
            bool early,
            double focused)
        {
            stage.completed = true;
            stage.completedEarly = early;
            stage.completedUtcTicks = DateTime.UtcNow.Ticks;
            stage.focusedSecondsAtCompletion = focused;
            stage.elapsedFocusedSeconds =
                Math.Max(0d, focused - stage.startedFocusedSeconds);
            long copper =
                stage.copperReward +
                (early ? stage.earlyCompletionCopperBonus : 0);
            long experience =
                stage.experienceReward +
                (early ? stage.earlyCompletionExperienceBonus : 0);
            DeverQuestProgressionResult progression =
                DeverQuestAdventurerService.Award(
                    copper, experience);
            ActiveSession.rewardTransactions.Add(
                new DeverQuestRewardTransaction
                {
                    categoryName = "Focus Stage",
                    transactionType = early
                        ? "Early Stage Completion"
                        : stage.survivalMode
                        ? "Survival Expedition Exit"
                        : "Stage Completion",
                    copper = copper,
                    experience = experience,
                    startingLevel = progression.StartingLevel,
                    endingLevel = progression.EndingLevel,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    note =
                        $"{stage.stageTitle} completed in " +
                        $"{stage.elapsedFocusedSeconds / 60d:0.0} minutes"
                });
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestContractService.RecordStageCompletion(
                ActiveSession.questContractId,
                stage.stageId,
                stage.stageTitle,
                adventurer.characterName);
            DeverQuestBattleResult battle =
                stage.survivalMode
                    ? null
                    : DeverQuestEncounterService.Resolve(
                        ActiveSession,
                        stage,
                        DeverQuestSettingsStore.Profile
                            .dailyDecreeCheckModifier);
            DeverQuestSessionStage next = CurrentQuestStage();
            if (next != null && next.startedFocusedSeconds <= 0d)
            {
                next.startedFocusedSeconds = focused;
            }
            return battle;
        }

        private static string BattleOutcome(
            DeverQuestBattleResult battle)
        {
            return battle.safetyPaused
                ? "Safety Pause"
                : battle.victory
                ? battle.earlyVictory
                    ? "Early Victory"
                    : "Victory"
                : "Defeat";
        }

        public static void EnsureWellnessSchedule(
            DeverQuestProfile profile)
        {
            if (!HasActiveSession || profile == null)
            {
                return;
            }

            double focused = GetFocusedSeconds();
            bool changed = false;
            DeverQuestSession session = ActiveSession;

            if (profile.focusCheckInScheduleMinutes != null &&
                profile.focusCheckInScheduleMinutes.Count > 0)
            {
                int scheduleIndex = Math.Max(
                    0,
                    session.nextFocusCheckInScheduleIndex);
                double next = scheduleIndex <
                              profile.focusCheckInScheduleMinutes.Count
                    ? profile.focusCheckInScheduleMinutes[scheduleIndex] *
                      60d
                    : double.MaxValue;
                if (session.nextCheckInFocusedSeconds <= 0d ||
                    session.nextFocusCheckInScheduleIndex != scheduleIndex)
                {
                    session.nextCheckInFocusedSeconds = next;
                    session.nextFocusCheckInScheduleIndex = scheduleIndex;
                    changed = true;
                }
            }
            else
            {
                changed |= EnsureNext(
                    ref session.nextCheckInFocusedSeconds,
                    focused,
                    profile.checkInMinutes);
            }

            changed |= EnsureNext(
                ref session.nextMovementBreakFocusedSeconds,
                focused,
                profile.movementBreakMinutes);

            changed |= EnsureNext(
                ref session.nextHydrationFocusedSeconds,
                focused,
                profile.hydrationMinutes);

            changed |= EnsureNext(
                ref session.nextExerciseFocusedSeconds,
                focused,
                profile.exerciseMinutes);

            if (changed)
            {
                SaveActiveSession();
            }
        }

        public static double GetNextWellnessSeconds(
            DeverQuestWellnessType type)
        {
            if (!HasActiveSession)
            {
                return double.MaxValue;
            }

            switch (type)
            {
                case DeverQuestWellnessType.CheckIn:
                    return ActiveSession.nextCheckInFocusedSeconds;
                case DeverQuestWellnessType.MovementBreak:
                    return ActiveSession.nextMovementBreakFocusedSeconds;
                case DeverQuestWellnessType.Hydration:
                    return ActiveSession.nextHydrationFocusedSeconds;
                case DeverQuestWellnessType.Exercise:
                    return ActiveSession.nextExerciseFocusedSeconds;
                default:
                    return double.MaxValue;
            }
        }

        public static void RecordWellnessAction(
            DeverQuestWellnessType type,
            string action,
            int nextIntervalMinutes)
        {
            if (!HasActiveSession)
            {
                return;
            }

            double focused = GetFocusedSeconds();

            ActiveSession.wellnessEvents.Add(
                new DeverQuestWellnessEvent
                {
                    type = type,
                    action = action,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    focusedSecondsAtEvent = focused
                });

            double nextSeconds = nextIntervalMinutes > 0
                ? focused + nextIntervalMinutes * 60d
                : double.MaxValue;

            switch (type)
            {
                case DeverQuestWellnessType.CheckIn:
                    if (DeverQuestSettingsStore.Profile
                            .focusCheckInScheduleMinutes != null &&
                        DeverQuestSettingsStore.Profile
                            .focusCheckInScheduleMinutes.Count > 0)
                    {
                        ActiveSession.nextFocusCheckInScheduleIndex++;
                        int index =
                            ActiveSession.nextFocusCheckInScheduleIndex;
                        List<int> schedule =
                            DeverQuestSettingsStore.Profile
                                .focusCheckInScheduleMinutes;
                        ActiveSession.nextCheckInFocusedSeconds =
                            index < schedule.Count
                                ? schedule[index] * 60d
                                : double.MaxValue;
                    }
                    else
                    {
                        ActiveSession.nextCheckInFocusedSeconds =
                            nextSeconds;
                    }
                    break;
                case DeverQuestWellnessType.MovementBreak:
                    ActiveSession.nextMovementBreakFocusedSeconds = nextSeconds;
                    break;
                case DeverQuestWellnessType.Hydration:
                    ActiveSession.nextHydrationFocusedSeconds = nextSeconds;
                    break;
                case DeverQuestWellnessType.Exercise:
                    ActiveSession.nextExerciseFocusedSeconds = nextSeconds;
                    break;
            }

            SaveActiveSession();
        }

        public static void AddRewardTransaction(
            DeverQuestRewardTransaction transaction)
        {
            if (!HasActiveSession || transaction == null)
            {
                return;
            }

            ActiveSession.rewardTransactions.Add(transaction);
            SaveActiveSession();
        }

        public static void RecordExternalActivity(
            string toolName,
            bool started,
            long createdUtcTicks,
            double durationSeconds)
        {
            if (!HasActiveSession ||
                string.IsNullOrWhiteSpace(toolName))
            {
                return;
            }

            ActiveSession.externalActivityEvents.Add(
                new DeverQuestExternalActivityEvent
                {
                    toolName = toolName.Trim(),
                    action = started ? "Activity Started" : "Activity Ended",
                    createdUtcTicks = createdUtcTicks,
                    durationSeconds = Math.Max(0d, durationSeconds)
                });
            SaveActiveSession();
        }

        public static void AddMediaAttachment(
            DeverQuestMediaAttachment attachment)
        {
            if (!HasActiveSession || attachment == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(attachment.attachmentId))
            {
                attachment.attachmentId =
                    Guid.NewGuid().ToString("N");
            }
            ActiveSession.mediaAttachments.Add(attachment);
            SaveActiveSession();
        }

        public static void RemoveMediaAttachment(
            string attachmentId)
        {
            if (!HasActiveSession ||
                string.IsNullOrWhiteSpace(attachmentId))
            {
                return;
            }

            ActiveSession.mediaAttachments.RemoveAll(
                value => value != null &&
                         value.attachmentId == attachmentId);
            SaveActiveSession();
        }

        public static DeverQuestSession CompleteSession(
            string closingNotes)
        {
            if (!HasActiveSession)
            {
                return null;
            }

            DeverQuestExternalActivityMonitor
                .EndSessionActivity();
            DeverQuestVoiceMemoService.CancelRecording();

            long nowTicks = DateTime.UtcNow.Ticks;
            AccumulateCurrentState(nowTicks);

            ActiveSession.completedUtcTicks = nowTicks;
            ActiveSession.lastStateChangeUtcTicks = nowTicks;
            ActiveSession.state = DeverQuestSessionState.Completed;
            ActiveSession.closingNotes = closingNotes;
            ActiveSession.Sanitize();

            lastCompletedSession = ActiveSession;
            SaveLastCompletedSession();

            activeSession = null;
            EditorPrefs.DeleteKey(ActiveSessionKey);
            SessionCompleted?.Invoke();

            return lastCompletedSession;
        }

        public static void SaveCompletedSession(
            DeverQuestSession completedSession)
        {
            if (completedSession == null ||
                completedSession.state !=
                DeverQuestSessionState.Completed)
            {
                return;
            }

            completedSession.Sanitize();
            lastCompletedSession = completedSession;
            SaveLastCompletedSession();
            SessionFinalized?.Invoke();
        }

        public static void DiscardSession()
        {
            DeverQuestExternalActivityMonitor
                .EndSessionActivity();
            DeverQuestVoiceMemoService.CancelRecording();
            activeSession = null;
            EditorPrefs.DeleteKey(ActiveSessionKey);
            SessionDiscarded?.Invoke();
        }

        public static double GetFocusedSeconds()
        {
            if (!HasActiveSession)
            {
                return 0d;
            }

            double total = ActiveSession.accumulatedFocusedSeconds;

            if (ActiveSession.state == DeverQuestSessionState.Running)
            {
                total += GetSecondsBetween(
                    ActiveSession.lastStateChangeUtcTicks,
                    DateTime.UtcNow.Ticks);
            }

            return Math.Max(0d, total);
        }

        public static double GetPausedSeconds()
        {
            if (!HasActiveSession)
            {
                return 0d;
            }

            double total = ActiveSession.accumulatedPausedSeconds;

            if (ActiveSession.state == DeverQuestSessionState.Paused)
            {
                total += GetSecondsBetween(
                    ActiveSession.lastStateChangeUtcTicks,
                    DateTime.UtcNow.Ticks);
            }

            return Math.Max(0d, total);
        }

        public static DateTime GetLocalStartTime(
            DeverQuestSession session)
        {
            if (session == null || session.startedUtcTicks <= 0)
            {
                return DateTime.Now;
            }

            return new DateTime(
                    session.startedUtcTicks,
                    DateTimeKind.Utc)
                .ToLocalTime();
        }

        public static DateTime GetLocalCompletionTime(
            DeverQuestSession session)
        {
            if (session == null || session.completedUtcTicks <= 0)
            {
                return DateTime.Now;
            }

            return new DateTime(
                    session.completedUtcTicks,
                    DateTimeKind.Utc)
                .ToLocalTime();
        }

        private static void PauseBeforeEditorCloses()
        {
            if (!HasActiveSession)
            {
                return;
            }

            if (ActiveSession.state == DeverQuestSessionState.Running)
            {
                PauseSession("Unity Closed");
            }
            else
            {
                long nowTicks = DateTime.UtcNow.Ticks;
                AccumulateCurrentState(nowTicks);
                ActiveSession.lastStateChangeUtcTicks = nowTicks;
            }

            ActiveSession.pausedByEditorShutdown = true;
            SaveActiveSession();
        }

        private static void AccumulateCurrentState(long nowTicks)
        {
            double elapsedSeconds = GetSecondsBetween(
                ActiveSession.lastStateChangeUtcTicks,
                nowTicks);

            if (ActiveSession.state == DeverQuestSessionState.Running)
            {
                ActiveSession.accumulatedFocusedSeconds += elapsedSeconds;
            }
            else if (
                ActiveSession.state == DeverQuestSessionState.Paused)
            {
                ActiveSession.accumulatedPausedSeconds += elapsedSeconds;
                if (ActiveSession.pauseReason.StartsWith(
                        "Approved Break:",
                        StringComparison.Ordinal))
                {
                    double approvedSeconds = Math.Min(
                        elapsedSeconds,
                        Math.Max(
                            0d,
                            GetSecondsBetween(
                                ActiveSession.lastStateChangeUtcTicks,
                                ActiveSession
                                    .approvedBreakUntilUtcTicks)));
                    ActiveSession.approvedBreakSeconds +=
                        approvedSeconds;
                    ActiveSession.idleUnverifiedSeconds +=
                        Math.Max(
                            0d,
                            elapsedSeconds - approvedSeconds);
                }
                else if (ActiveSession.pauseReason == "Idle Detection" ||
                    ActiveSession.pauseReason ==
                    "Unity Project Lost Focus")
                {
                    ActiveSession.idleUnverifiedSeconds += elapsedSeconds;
                }
                else
                {
                    ActiveSession.meditationSeconds += elapsedSeconds;
                }
            }
        }

        private static double GetSecondsBetween(
            long earlierTicks,
            long laterTicks)
        {
            if (earlierTicks <= 0 || laterTicks <= earlierTicks)
            {
                return 0d;
            }

            return TimeSpan
                .FromTicks(laterTicks - earlierTicks)
                .TotalSeconds;
        }

        private static bool EnsureNext(
            ref double nextSeconds,
            double currentFocusedSeconds,
            int intervalMinutes)
        {
            if (nextSeconds > 0d)
            {
                return false;
            }

            nextSeconds = intervalMinutes > 0
                ? currentFocusedSeconds + intervalMinutes * 60d
                : double.MaxValue;

            return true;
        }

        private static void SaveActiveSession()
        {
            if (activeSession == null)
            {
                EditorPrefs.DeleteKey(ActiveSessionKey);
                return;
            }

            activeSession.Sanitize();
            EditorPrefs.SetString(
                ActiveSessionKey,
                JsonUtility.ToJson(activeSession));
        }

        private static void SaveLastCompletedSession()
        {
            if (lastCompletedSession == null)
            {
                EditorPrefs.DeleteKey(LastCompletedSessionKey);
                return;
            }

            lastCompletedSession.Sanitize();
            EditorPrefs.SetString(
                LastCompletedSessionKey,
                JsonUtility.ToJson(lastCompletedSession));
        }

        private static void Load()
        {
            LoadActiveSession();
            LoadLastCompletedSession();
        }

        private static void LoadActiveSession()
        {
            activeSession = LoadSession(ActiveSessionKey);

            if (activeSession != null && !activeSession.IsActive)
            {
                activeSession = null;
                EditorPrefs.DeleteKey(ActiveSessionKey);
            }
            else if (
                activeSession != null &&
                activeSession.state == DeverQuestSessionState.Paused &&
                activeSession.pausedByEditorShutdown)
            {
                activeSession.lastStateChangeUtcTicks =
                    DateTime.UtcNow.Ticks;

                activeSession.pausedByEditorShutdown = false;
                SaveActiveSession();
            }
        }

        private static void LoadLastCompletedSession()
        {
            lastCompletedSession =
                LoadSession(LastCompletedSessionKey);
        }

        private static DeverQuestSession LoadSession(string key)
        {
            string json = EditorPrefs.GetString(key, string.Empty);

            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                DeverQuestSession session =
                    JsonUtility.FromJson<DeverQuestSession>(json);

                session?.Sanitize();
                return session;
            }
            catch
            {
                Debug.LogWarning(
                    $"[DeverQuest] Could not load saved session: {key}");

                EditorPrefs.DeleteKey(key);
                return null;
            }
        }
    }
}

//----- DeverQuestSessionStore.cs END -----
