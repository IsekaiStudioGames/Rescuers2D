//----- DeverQuestSession.cs START -----

using System;
using System.Collections.Generic;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestSessionState
    {
        None = 0,
        Running = 1,
        Paused = 2,
        Completed = 3
    }

    [Serializable]
    internal enum DeverQuestWellnessType
    {
        CheckIn = 0,
        MovementBreak = 1,
        Hydration = 2,
        Exercise = 3,
        Lunch = 4,
        Dinner = 5,
        QuietHours = 6
    }

    [Serializable]
    internal sealed class DeverQuestWellnessEvent
    {
        public DeverQuestWellnessType type;
        public string action = string.Empty;
        public long createdUtcTicks;
        public double focusedSecondsAtEvent;
    }

    [Serializable]
    internal sealed class DeverQuestExternalActivityEvent
    {
        public string toolName = string.Empty;
        public string action = string.Empty;
        public long createdUtcTicks;
        public double durationSeconds;
    }

    [Serializable]
    internal sealed class DeverQuestMediaAttachment
    {
        public string attachmentId = string.Empty;
        public string attachmentType = "File";
        public string displayName = string.Empty;
        public string filePath = string.Empty;
        public long createdUtcTicks;
        public double durationSeconds;
    }

    [Serializable]
    internal sealed class DeverQuestRewardTransaction
    {
        public string categoryName = string.Empty;
        public string transactionType = string.Empty;
        public double minutes;
        public long copper;
        public long experience;
        public int startingLevel;
        public int endingLevel;
        public long createdUtcTicks;
        public string note = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestCommitEntry
    {
        public string entryId = string.Empty;
        public string comment = string.Empty;
        public string branch = string.Empty;
        public string commitHash = string.Empty;
        public string entryType = string.Empty;
        public long createdUtcTicks;
        public double focusedSecondsAtEntry;

        public void Sanitize()
        {
            entryId = entryId?.Trim() ?? string.Empty;
            comment = comment?.Trim() ?? string.Empty;
            branch = branch?.Trim() ?? string.Empty;
            commitHash = commitHash?.Trim() ?? string.Empty;
            entryType = entryType?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(entryType))
            {
                entryType = "Legacy Entry";
            }
            focusedSecondsAtEntry =
                Math.Max(0d, focusedSecondsAtEntry);
        }
    }

    [Serializable]
    internal sealed class DeverQuestSessionStage
    {
        public string stageId = string.Empty;
        public string stageTitle = string.Empty;
        public string workObjective = string.Empty;
        public int focusedMinutesRequired;
        public string assignedPartyRole = string.Empty;
        public int copperReward;
        public int experienceReward;
        public bool allowEarlyTurnIn;
        public int earlyCompletionCopperBonus;
        public int earlyCompletionExperienceBonus;
        public string encounterProfileId = string.Empty;
        public double startedFocusedSeconds;
        public bool completed;
        public long completedUtcTicks;
        public double focusedSecondsAtCompletion;
        public double elapsedFocusedSeconds;
        public bool completedEarly;
        public bool survivalMode;
        public int survivalWave;
        public double nextSurvivalWaveFocusedSeconds;
        public bool survivalFightPaused;
        public bool survivalExitOffered;
        public int survivalFleeAttempts;
        public string survivalPauseReason = string.Empty;
        public bool survivalEndedSafely;
        public string survivalExitMethod = string.Empty;
        public string survivalExitSummary = string.Empty;
        public long survivalExitUtcTicks;
        public bool encounterResolved;
    }

    [Serializable]
    internal sealed class DeverQuestCombatActionEvent
    {
        public int round;
        public string actor = string.Empty;
        public string actionName = string.Empty;
        public string target = string.Empty;
        public int manaSpent;
        public List<string> effects = new List<string>();
    }

    [Serializable]
    internal sealed class DeverQuestDamageEvent
    {
        public int round;
        public string source = string.Empty;
        public string target = string.Empty;
        public DeverQuestDamageType damageType =
            DeverQuestDamageType.Bludgeoning;
        public DeverQuestDamageResponse response =
            DeverQuestDamageResponse.Normal;
        public int rawDamage;
        public int finalDamage;
        public int absorbedHealing;
    }

    [Serializable]
    internal sealed class DeverQuestBattleResult
    {
        public string stageId = string.Empty;
        public string stageTitle = string.Empty;
        public string encounterId = string.Empty;
        public string encounterName = string.Empty;
        public string seed = string.Empty;
        public bool victory;
        public bool characterFell;
        public int startingHitPoints;
        public int endingHitPoints;
        public string companionName = string.Empty;
        public int companionStartingHitPoints;
        public int companionEndingHitPoints;
        public bool companionFell;
        public int companionLevelBefore;
        public int companionLevelAfter;
        public long companionExperienceEarned;
        public int rounds;
        public int parRounds;
        public bool earlyVictory;
        public int survivalWave;
        public bool safetyPaused;
        public string safetyPauseReason = string.Empty;
        public float carriedWeight;
        public float carryCapacity;
        public long bonusCopper;
        public long bonusExperience;
        public string injury = string.Empty;
        public List<string> defeatedMonsters =
            new List<string>();
        public List<string> loot =
            new List<string>();
        public List<string> combatLog =
            new List<string>();
        public List<DeverQuestDamageEvent> damageEvents =
            new List<DeverQuestDamageEvent>();
        public List<DeverQuestCombatActionEvent> actionEvents =
            new List<DeverQuestCombatActionEvent>();
        public string typedDamageSummary = string.Empty;
        public long resolvedUtcTicks;
    }

    [Serializable]
    internal sealed class DeverQuestSession
    {
        public string sessionId = string.Empty;
        public string developerName = string.Empty;
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string category = string.Empty;
        public string goal = string.Empty;
        public bool usesQuestProfile;
        public string questProfileId = string.Empty;
        public string questProfileName = string.Empty;
        public int questSuggestedFocusMinutes;
        public int questBaseCopper;
        public int questBaseExperience;
        public int questWorkBlockMinutes;
        public int questCopperPerWorkBlock;
        public int questExperiencePerWorkBlock;
        public bool usesQuestContract;
        public string questContractId = string.Empty;
        public string questContractRunId = string.Empty;
        public string questContractTitle = string.Empty;
        public string questContractCreator = string.Empty;
        public string questContractAssignee = string.Empty;
        public string questContractPriority = string.Empty;
        public string questContractDueDate = string.Empty;
        public string questContractDeliverables = string.Empty;
        public string questEncounterProfileId = string.Empty;
        public string questEncounterNotes = string.Empty;
        public bool questIsGroupQuest;
        public int questMaximumParticipants = 1;
        public string questPartyMembers = string.Empty;
        public string questStory = string.Empty;
        public int questGroupBonusCopper;
        public int questGroupBonusExperience;
        public List<DeverQuestSessionStage> questStages =
            new List<DeverQuestSessionStage>();
        public List<DeverQuestBattleResult> battleResults =
            new List<DeverQuestBattleResult>();

        public DeverQuestSessionState state =
            DeverQuestSessionState.None;

        public long startedUtcTicks;
        public long lastStateChangeUtcTicks;
        public long completedUtcTicks;

        public double accumulatedFocusedSeconds;
        public double accumulatedPausedSeconds;
        public double meditationSeconds;
        public int meditationHitPointsRestored;
        public int meditationManaRestored;
        public double idleUnverifiedSeconds;
        public double approvedBreakSeconds;
        public long approvedBreakUntilUtcTicks;
        public bool approvedBreakIsWellness;
        public DeverQuestWellnessType approvedBreakWellnessType;
        public int approvedBreakPlannedMinutes;
        public bool pausedByEditorShutdown;
        public string pauseReason = string.Empty;
        public bool idlePauseAcknowledged = true;
        public string closingNotes = string.Empty;
        public List<DeverQuestCommitEntry> commitEntries =
            new List<DeverQuestCommitEntry>();
        public List<DeverQuestWellnessEvent> wellnessEvents =
            new List<DeverQuestWellnessEvent>();
        public List<DeverQuestExternalActivityEvent>
            externalActivityEvents =
                new List<DeverQuestExternalActivityEvent>();
        public List<DeverQuestMediaAttachment> mediaAttachments =
            new List<DeverQuestMediaAttachment>();
        public List<DeverQuestRewardTransaction> rewardTransactions =
            new List<DeverQuestRewardTransaction>();
        public bool rewardsProcessed;

        public double nextCheckInFocusedSeconds;
        public int nextFocusCheckInScheduleIndex;
        public double nextMovementBreakFocusedSeconds;
        public double nextHydrationFocusedSeconds;
        public double nextExerciseFocusedSeconds;

        public bool timecardWriteSucceeded;
        public bool timecardWriteAttempted;
        public string timecardPath = string.Empty;
        public string timecardWriteError = string.Empty;

        public bool IsActive =>
            state == DeverQuestSessionState.Running ||
            state == DeverQuestSessionState.Paused;

        public void Sanitize()
        {
            sessionId = sessionId?.Trim() ?? string.Empty;
            developerName = developerName?.Trim() ?? string.Empty;
            projectName = projectName?.Trim() ?? string.Empty;
            taskName = taskName?.Trim() ?? string.Empty;
            category = category?.Trim() ?? string.Empty;
            goal = goal?.Trim() ?? string.Empty;
            questProfileId = questProfileId?.Trim() ?? string.Empty;
            questProfileName = questProfileName?.Trim() ?? string.Empty;
            questSuggestedFocusMinutes =
                Math.Max(0, questSuggestedFocusMinutes);
            questBaseCopper = Math.Max(0, questBaseCopper);
            questBaseExperience = Math.Max(0, questBaseExperience);
            questWorkBlockMinutes =
                Math.Max(0, questWorkBlockMinutes);
            questCopperPerWorkBlock =
                Math.Max(0, questCopperPerWorkBlock);
            questExperiencePerWorkBlock =
                Math.Max(0, questExperiencePerWorkBlock);
            questContractId = questContractId?.Trim() ?? string.Empty;
            questContractRunId =
                questContractRunId?.Trim() ?? string.Empty;
            questContractTitle =
                questContractTitle?.Trim() ?? string.Empty;
            questContractCreator =
                questContractCreator?.Trim() ?? string.Empty;
            questContractAssignee =
                questContractAssignee?.Trim() ?? string.Empty;
            questContractPriority =
                questContractPriority?.Trim() ?? string.Empty;
            questContractDueDate =
                questContractDueDate?.Trim() ?? string.Empty;
            questContractDeliverables =
                questContractDeliverables?.Trim() ?? string.Empty;
            questEncounterProfileId =
                questEncounterProfileId?.Trim() ?? string.Empty;
            questEncounterNotes =
                questEncounterNotes?.Trim() ?? string.Empty;
            questPartyMembers =
                questPartyMembers?.Trim() ?? string.Empty;
            questStory = questStory?.Trim() ?? string.Empty;
            questMaximumParticipants =
                Math.Max(1, questMaximumParticipants);
            questGroupBonusCopper =
                Math.Max(0, questGroupBonusCopper);
            questGroupBonusExperience =
                Math.Max(0, questGroupBonusExperience);
            questStages = questStages ??
                          new List<DeverQuestSessionStage>();
            foreach (DeverQuestSessionStage stage in questStages)
            {
                if (stage == null)
                {
                    continue;
                }
                stage.stageId = stage.stageId?.Trim() ?? string.Empty;
                stage.stageTitle = stage.stageTitle?.Trim() ?? string.Empty;
                stage.workObjective =
                    stage.workObjective?.Trim() ?? string.Empty;
                stage.assignedPartyRole =
                    stage.assignedPartyRole?.Trim() ?? string.Empty;
                stage.encounterProfileId =
                    stage.encounterProfileId?.Trim() ?? string.Empty;
                stage.survivalPauseReason =
                    stage.survivalPauseReason?.Trim() ?? string.Empty;
                stage.survivalExitMethod =
                    stage.survivalExitMethod?.Trim() ?? string.Empty;
                stage.survivalExitSummary =
                    stage.survivalExitSummary?.Trim() ?? string.Empty;
                stage.survivalWave = Math.Max(0, stage.survivalWave);
                stage.survivalFleeAttempts =
                    Math.Max(0, stage.survivalFleeAttempts);
                stage.survivalExitUtcTicks =
                    Math.Max(0L, stage.survivalExitUtcTicks);
            }
            battleResults = battleResults ??
                            new List<DeverQuestBattleResult>();
            foreach (DeverQuestBattleResult battle in battleResults)
            {
                if (battle == null)
                {
                    continue;
                }
                battle.defeatedMonsters =
                    battle.defeatedMonsters ??
                    new List<string>();
                battle.loot = battle.loot ?? new List<string>();
                battle.combatLog =
                    battle.combatLog ?? new List<string>();
                battle.damageEvents =
                    battle.damageEvents ??
                    new List<DeverQuestDamageEvent>();
                battle.actionEvents =
                    battle.actionEvents ??
                    new List<DeverQuestCombatActionEvent>();
                battle.typedDamageSummary =
                    battle.typedDamageSummary?.Trim() ??
                    string.Empty;
                battle.companionName =
                    battle.companionName?.Trim() ??
                    string.Empty;
            }
            pauseReason = pauseReason?.Trim() ?? string.Empty;
            if (state == DeverQuestSessionState.Running ||
                (pauseReason != "Idle Detection" &&
                 pauseReason != "Unity Project Lost Focus"))
            {
                idlePauseAcknowledged = true;
            }
            closingNotes = closingNotes?.Trim() ?? string.Empty;
            timecardPath = timecardPath?.Trim() ?? string.Empty;
            timecardWriteError = timecardWriteError?.Trim() ?? string.Empty;

            if (commitEntries == null)
            {
                commitEntries =
                    new List<DeverQuestCommitEntry>();
            }

            foreach (DeverQuestCommitEntry entry in commitEntries)
            {
                entry?.Sanitize();
            }

            if (wellnessEvents == null)
            {
                wellnessEvents =
                    new List<DeverQuestWellnessEvent>();
            }

            if (externalActivityEvents == null)
            {
                externalActivityEvents =
                    new List<DeverQuestExternalActivityEvent>();
            }
            externalActivityEvents.RemoveAll(value => value == null);
            foreach (DeverQuestExternalActivityEvent activity
                     in externalActivityEvents)
            {
                activity.toolName =
                    activity.toolName?.Trim() ?? string.Empty;
                activity.action =
                    activity.action?.Trim() ?? string.Empty;
                activity.durationSeconds =
                    Math.Max(0d, activity.durationSeconds);
            }

            if (mediaAttachments == null)
            {
                mediaAttachments =
                    new List<DeverQuestMediaAttachment>();
            }
            mediaAttachments.RemoveAll(value => value == null);
            foreach (DeverQuestMediaAttachment attachment
                     in mediaAttachments)
            {
                attachment.attachmentId =
                    attachment.attachmentId?.Trim() ?? string.Empty;
                attachment.attachmentType =
                    attachment.attachmentType?.Trim() ?? string.Empty;
                attachment.displayName =
                    attachment.displayName?.Trim() ?? string.Empty;
                attachment.filePath =
                    attachment.filePath?.Trim() ?? string.Empty;
                attachment.durationSeconds =
                    Math.Max(0d, attachment.durationSeconds);
            }

            if (rewardTransactions == null)
            {
                rewardTransactions =
                    new List<DeverQuestRewardTransaction>();
            }

            accumulatedFocusedSeconds =
                Math.Max(0d, accumulatedFocusedSeconds);

            accumulatedPausedSeconds =
                Math.Max(0d, accumulatedPausedSeconds);
            meditationSeconds = Math.Max(0d, meditationSeconds);
            meditationHitPointsRestored =
                Math.Max(0, meditationHitPointsRestored);
            meditationManaRestored =
                Math.Max(0, meditationManaRestored);
            idleUnverifiedSeconds =
                Math.Max(0d, idleUnverifiedSeconds);
            approvedBreakSeconds =
                Math.Max(0d, approvedBreakSeconds);
            approvedBreakUntilUtcTicks =
                Math.Max(0L, approvedBreakUntilUtcTicks);
            approvedBreakPlannedMinutes =
                Math.Max(0, approvedBreakPlannedMinutes);
        }
    }
}

//----- DeverQuestSession.cs END -----
