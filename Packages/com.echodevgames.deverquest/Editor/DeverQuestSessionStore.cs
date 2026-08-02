//----- DeverQuestSessionStore.cs START -----

using System;
using System.Collections.Generic;
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
        public static event Action SessionDiscarded;

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
            string goal)
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
                state = DeverQuestSessionState.Running,
                startedUtcTicks = nowTicks,
                lastStateChangeUtcTicks = nowTicks
            };

            activeSession.Sanitize();
            SaveActiveSession();
            SessionStarted?.Invoke();
        }

        public static void PauseSession(string reason = "Manual")
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
            ActiveSession.accumulatedPausedSeconds +=
                GetSecondsBetween(
                    ActiveSession.lastStateChangeUtcTicks,
                    nowTicks);

            ActiveSession.lastStateChangeUtcTicks = nowTicks;
            ActiveSession.state = DeverQuestSessionState.Running;
            ActiveSession.pausedByEditorShutdown = false;
            ActiveSession.pauseReason = string.Empty;
            SaveActiveSession();
            SessionResumed?.Invoke();
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

        public static DeverQuestSession CompleteSession(
            string closingNotes)
        {
            if (!HasActiveSession)
            {
                return null;
            }

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
        }

        public static void DiscardSession()
        {
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
