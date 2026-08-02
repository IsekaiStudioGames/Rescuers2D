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
    internal sealed class DeverQuestSession
    {
        public string sessionId = string.Empty;
        public string developerName = string.Empty;
        public string projectName = string.Empty;
        public string taskName = string.Empty;
        public string category = string.Empty;
        public string goal = string.Empty;

        public DeverQuestSessionState state =
            DeverQuestSessionState.None;

        public long startedUtcTicks;
        public long lastStateChangeUtcTicks;
        public long completedUtcTicks;

        public double accumulatedFocusedSeconds;
        public double accumulatedPausedSeconds;
        public bool pausedByEditorShutdown;
        public string pauseReason = string.Empty;
        public bool idlePauseAcknowledged = true;
        public string closingNotes = string.Empty;
        public List<DeverQuestCommitEntry> commitEntries =
            new List<DeverQuestCommitEntry>();
        public List<DeverQuestWellnessEvent> wellnessEvents =
            new List<DeverQuestWellnessEvent>();
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

            if (rewardTransactions == null)
            {
                rewardTransactions =
                    new List<DeverQuestRewardTransaction>();
            }

            accumulatedFocusedSeconds =
                Math.Max(0d, accumulatedFocusedSeconds);

            accumulatedPausedSeconds =
                Math.Max(0d, accumulatedPausedSeconds);
        }
    }
}

//----- DeverQuestSession.cs END -----
