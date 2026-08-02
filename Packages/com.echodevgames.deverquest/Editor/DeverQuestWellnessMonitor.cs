//----- DeverQuestWellnessMonitor.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestWellnessReminder
    {
        public string reminderId = string.Empty;
        public DeverQuestWellnessType type;
        public string title = string.Empty;
        public string message = string.Empty;
        public long createdUtcTicks;
        public long dueUtcTicks;
        public bool testReminder;

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(reminderId))
            {
                reminderId = Guid.NewGuid().ToString("N");
            }
            title = title?.Trim() ?? string.Empty;
            message = message?.Trim() ?? string.Empty;
            createdUtcTicks = Math.Max(0L, createdUtcTicks);
            dueUtcTicks = Math.Max(0L, dueUtcTicks);
        }
    }

    [Serializable]
    internal sealed class DeverQuestWellnessCommandState
    {
        public DeverQuestWellnessReminder activeReminder;
        public List<DeverQuestWellnessReminder> pendingReminders =
            new List<DeverQuestWellnessReminder>();

        public void Sanitize()
        {
            activeReminder?.Sanitize();
            pendingReminders = pendingReminders ??
                               new List<DeverQuestWellnessReminder>();
            pendingReminders.RemoveAll(value => value == null);
            foreach (DeverQuestWellnessReminder reminder in pendingReminders)
            {
                reminder.Sanitize();
            }
            pendingReminders = pendingReminders
                .GroupBy(value => value.reminderId)
                .Select(group => group.First())
                .OrderBy(value => value.dueUtcTicks)
                .Take(24)
                .ToList();
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestWellnessMonitor
    {
        private const string StateKey =
            "EchoDevGames.DeverQuest.Wellness.CommandState.v1";
        private const string LegacySnoozeUntilKey =
            "EchoDevGames.DeverQuest.Wellness.SnoozeUntil";
        private const string LunchDateKey =
            "EchoDevGames.DeverQuest.Wellness.LunchDate";
        private const string DinnerDateKey =
            "EchoDevGames.DeverQuest.Wellness.DinnerDate";
        private const string QuietDateKey =
            "EchoDevGames.DeverQuest.Wellness.QuietDate";

        private static double nextCheckTime;
        private static DeverQuestWellnessCommandState state;

        public static event Action StateChanged;

        public static bool HasActiveReminder =>
            state?.activeReminder != null;

        public static DeverQuestWellnessType ActiveType =>
            state?.activeReminder?.type ??
            DeverQuestWellnessType.CheckIn;

        public static string ActiveTitle =>
            state?.activeReminder?.title ?? string.Empty;

        public static string ActiveMessage =>
            state?.activeReminder?.message ?? string.Empty;

        public static bool ActiveIsTest =>
            state?.activeReminder?.testReminder ?? false;

        public static int PendingCount =>
            state?.pendingReminders?.Count ?? 0;

        public static IReadOnlyList<DeverQuestWellnessReminder>
            PendingReminders
        {
            get
            {
                EnsureState();
                return state.pendingReminders
                    .OrderBy(value => value.dueUtcTicks)
                    .ToList();
            }
        }

        public static int RecommendedBreakMinutes =>
            HasActiveReminder
                ? GetBreakMinutes(
                    DeverQuestSettingsStore.Profile,
                    ActiveType)
                : 0;

        public static int RequiredBreakMinutes =>
            RecommendedBreakMinutes <= 0
                ? 0
                : (int)Math.Ceiling(
                    RecommendedBreakMinutes * 0.8d);

        public static bool CanStartApprovedBreak =>
            HasActiveReminder &&
            DeverQuestSessionStore.HasActiveSession &&
            DeverQuestSessionStore.ActiveSession.state ==
            DeverQuestSessionState.Running;

        public static bool QuietHoursActive =>
            IsQuietHoursActive(
                DateTime.Now,
                DeverQuestSettingsStore.Profile);

        public static DateTime QuietHoursEndsAtLocal =>
            ResolveQuietHoursEnd(
                DateTime.Now,
                DeverQuestSettingsStore.Profile);

        static DeverQuestWellnessMonitor()
        {
            LoadState();
            EditorPrefs.DeleteKey(LegacySnoozeUntilKey);
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
            if (HasActiveReminder)
            {
                EditorApplication.delayCall += () =>
                {
                    if (HasActiveReminder)
                    {
                        DeverQuestWindow.ShowWellnessReminder(
                            ActiveTitle,
                            ActiveType);
                        NotifyChanged();
                    }
                };
            }
        }

        public static void Acknowledge(bool startBreak)
        {
            EnsureState();
            DeverQuestWellnessReminder reminder =
                state.activeReminder;
            if (reminder == null)
            {
                return;
            }

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            int recommended = GetBreakMinutes(profile, reminder.type);
            int required = (int)Math.Ceiling(recommended * 0.8d);
            bool breakStarted = false;

            if (!reminder.testReminder)
            {
                if (startBreak && CanStartApprovedBreak)
                {
                    breakStarted =
                        DeverQuestSessionStore.PauseForApprovedBreak(
                            recommended,
                            reminder.title,
                            reminder.type,
                            true);
                }

                if (IsSessionReminder(reminder.type) &&
                    DeverQuestSessionStore.HasActiveSession)
                {
                    DeverQuestSessionStore.RecordWellnessAction(
                        reminder.type,
                        breakStarted
                            ? "Break Started"
                            : "Acknowledged",
                        GetIntervalMinutes(profile, reminder.type));
                }

                if (!IsSessionReminder(reminder.type))
                {
                    MarkDailyReminderHandled(reminder.type);
                }
            }

            DeverQuestWellnessHistoryService.Record(
                reminder.type,
                reminder.title,
                breakStarted ? "Break Started" : "Acknowledged",
                reminder.testReminder
                    ? "Test reminder handled"
                    : breakStarted
                        ? "Approved Break began"
                        : "Reminder dismissed without starting a break",
                recommended,
                required,
                0,
                reminder.testReminder);

            state.activeReminder = null;
            SaveState();
            PromoteNextReminder();
            NotifyChanged();
        }

        public static void Snooze()
        {
            Snooze(DeverQuestSettingsStore.Profile.snoozeMinutes);
        }

        public static void Snooze(int minutes)
        {
            EnsureState();
            DeverQuestWellnessReminder reminder =
                state.activeReminder;
            if (reminder == null)
            {
                return;
            }

            minutes = Math.Max(1, minutes);
            reminder.dueUtcTicks =
                DateTime.UtcNow.AddMinutes(minutes).Ticks;
            state.pendingReminders.Add(reminder);
            state.activeReminder = null;

            int recommended = GetBreakMinutes(
                DeverQuestSettingsStore.Profile,
                reminder.type);
            DeverQuestWellnessHistoryService.Record(
                reminder.type,
                reminder.title,
                "Snoozed",
                $"Reminder will return in {minutes} minute(s)",
                recommended,
                (int)Math.Ceiling(recommended * 0.8d),
                minutes,
                reminder.testReminder);

            SaveState();
            PromoteNextReminder();
            NotifyChanged();
        }

        public static bool DismissPending(string reminderId)
        {
            EnsureState();
            DeverQuestWellnessReminder reminder =
                state.pendingReminders.FirstOrDefault(value =>
                    value != null &&
                    string.Equals(
                        value.reminderId,
                        reminderId,
                        StringComparison.Ordinal));
            if (reminder == null)
            {
                return false;
            }

            state.pendingReminders.Remove(reminder);
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!reminder.testReminder &&
                IsSessionReminder(reminder.type) &&
                DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSessionStore.RecordWellnessAction(
                    reminder.type,
                    "Queued Reminder Dismissed",
                    GetIntervalMinutes(profile, reminder.type));
            }
            int recommended = GetBreakMinutes(
                profile,
                reminder.type);
            DeverQuestWellnessHistoryService.Record(
                reminder.type,
                reminder.title,
                "Queued Reminder Dismissed",
                string.Empty,
                recommended,
                (int)Math.Ceiling(recommended * 0.8d),
                0,
                reminder.testReminder);
            SaveState();
            NotifyChanged();
            return true;
        }

        public static void ClearPending()
        {
            EnsureState();
            state.pendingReminders.Clear();
            SaveState();
            NotifyChanged();
        }

        public static void TriggerTest(
            DeverQuestWellnessType type)
        {
            CreateOrQueueReminder(
                type,
                TitleFor(type),
                MessageFor(type),
                true,
                true);
        }

        public static string NextSessionReminderSummary()
        {
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                return "No active Quest schedule";
            }

            DeverQuestSettingsStore.Profile.Sanitize();
            DeverQuestSessionStore.EnsureWellnessSchedule(
                DeverQuestSettingsStore.Profile);
            double focused = DeverQuestSessionStore.GetFocusedSeconds();
            var candidates = new[]
            {
                new
                {
                    Type = DeverQuestWellnessType.CheckIn,
                    Label = "Focus Check-In"
                },
                new
                {
                    Type = DeverQuestWellnessType.Hydration,
                    Label = "Hydration"
                },
                new
                {
                    Type = DeverQuestWellnessType.MovementBreak,
                    Label = "Movement"
                },
                new
                {
                    Type = DeverQuestWellnessType.Exercise,
                    Label = "Exercise"
                }
            }
            .Select(value => new
            {
                value.Label,
                Seconds = DeverQuestSessionStore
                    .GetNextWellnessSeconds(value.Type) - focused
            })
            .Where(value => value.Seconds < double.MaxValue / 2d)
            .OrderBy(value => value.Seconds)
            .FirstOrDefault();

            if (candidates == null)
            {
                return "No session reminder scheduled";
            }

            return candidates.Label + " in " +
                   FormatDuration(Math.Max(0d, candidates.Seconds));
        }

        public static string PendingDueSummary(
            DeverQuestWellnessReminder reminder)
        {
            if (reminder == null)
            {
                return string.Empty;
            }
            long remainingTicks = reminder.dueUtcTicks -
                                  DateTime.UtcNow.Ticks;
            return remainingTicks <= 0L
                ? "Ready"
                : "Due in " + FormatDuration(
                    TimeSpan.FromTicks(remainingTicks).TotalSeconds);
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup < nextCheckTime)
            {
                return;
            }
            nextCheckTime = EditorApplication.timeSinceStartup + 1d;

            EnsureState();
            PromoteNextReminder();

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.setupComplete)
            {
                return;
            }
            if (!profile.wellnessEnabled)
            {
                bool changed = false;
                if (state.activeReminder != null &&
                    !state.activeReminder.testReminder)
                {
                    state.activeReminder = null;
                    changed = true;
                }
                int removed = state.pendingReminders.RemoveAll(
                    value => value != null && !value.testReminder);
                if (changed || removed > 0)
                {
                    SaveState();
                    PromoteNextReminder();
                    NotifyChanged();
                }
                return;
            }

            TryTriggerDailyReminders(profile);

            if (!DeverQuestSessionStore.HasActiveSession ||
                DeverQuestSessionStore.ActiveSession.state !=
                DeverQuestSessionState.Running)
            {
                return;
            }

            DeverQuestSessionStore.EnsureWellnessSchedule(profile);
            double focused = DeverQuestSessionStore.GetFocusedSeconds();

            TryTriggerFocused(
                DeverQuestWellnessType.Exercise,
                focused,
                profile.exerciseMinutes);
            TryTriggerFocused(
                DeverQuestWellnessType.MovementBreak,
                focused,
                profile.movementBreakMinutes);
            TryTriggerFocused(
                DeverQuestWellnessType.Hydration,
                focused,
                profile.hydrationMinutes);
            TryTriggerFocused(
                DeverQuestWellnessType.CheckIn,
                focused,
                profile.checkInMinutes);
        }

        private static void TryTriggerDailyReminders(
            DeverQuestProfile profile)
        {
            DateTime now = DateTime.Now;
            string dateKey = now.ToString("yyyy-MM-dd");
            DateTime dinnerTime = now.Date
                .AddHours(profile.dinnerHour)
                .AddMinutes(profile.dinnerMinute);
            DateTime lunchTime = now.Date
                .AddHours(profile.lunchHour)
                .AddMinutes(profile.lunchMinute);

            if (profile.mealRemindersEnabled &&
                now >= dinnerTime &&
                EditorPrefs.GetString(DinnerDateKey, string.Empty) != dateKey)
            {
                if (CreateOrQueueReminder(
                        DeverQuestWellnessType.Dinner,
                        "Dinner Reminder",
                        "Pause long enough to eat a real meal.",
                        false,
                        false))
                {
                    MarkDailyReminderHandled(
                        DeverQuestWellnessType.Dinner);
                }
            }
            else if (profile.mealRemindersEnabled &&
                     now >= lunchTime &&
                     now < dinnerTime &&
                     EditorPrefs.GetString(LunchDateKey, string.Empty) !=
                     dateKey)
            {
                if (CreateOrQueueReminder(
                        DeverQuestWellnessType.Lunch,
                        "Lunch Reminder",
                        "Time to stop and eat something before continuing.",
                        false,
                        false))
                {
                    MarkDailyReminderHandled(
                        DeverQuestWellnessType.Lunch);
                }
            }

            if (profile.quietHoursEnabled &&
                IsQuietHoursActive(now, profile) &&
                EditorPrefs.GetString(QuietDateKey, string.Empty) != dateKey)
            {
                if (CreateOrQueueReminder(
                        DeverQuestWellnessType.QuietHours,
                        "Quiet Hours",
                        "It is getting late. Consider finalizing your " +
                        "session and giving yourself a real stopping point.",
                        false,
                        false))
                {
                    MarkDailyReminderHandled(
                        DeverQuestWellnessType.QuietHours);
                }
            }
        }

        private static void TryTriggerFocused(
            DeverQuestWellnessType type,
            double focusedSeconds,
            int intervalMinutes)
        {
            if (intervalMinutes <= 0 ||
                focusedSeconds <
                DeverQuestSessionStore.GetNextWellnessSeconds(type) ||
                ContainsType(type))
            {
                return;
            }

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (profile.suppressWellnessDuringQuietHours &&
                IsQuietHoursActive(DateTime.Now, profile))
            {
                DeverQuestSessionStore.RecordWellnessAction(
                    type,
                    "Suppressed by Quiet Hours",
                    intervalMinutes);
                int recommended = GetBreakMinutes(profile, type);
                DeverQuestWellnessHistoryService.Record(
                    type,
                    TitleFor(type),
                    "Suppressed by Quiet Hours",
                    "The session reminder was advanced without displaying.",
                    recommended,
                    (int)Math.Ceiling(recommended * 0.8d));
                return;
            }

            CreateOrQueueReminder(
                type,
                TitleFor(type),
                MessageFor(type),
                false,
                false);
        }

        private static bool CreateOrQueueReminder(
            DeverQuestWellnessType type,
            string title,
            string message,
            bool testReminder,
            bool allowDuplicate)
        {
            EnsureState();
            if (!allowDuplicate && ContainsType(type))
            {
                return false;
            }

            DeverQuestWellnessReminder reminder =
                new DeverQuestWellnessReminder
                {
                    reminderId = Guid.NewGuid().ToString("N"),
                    type = type,
                    title = title,
                    message = message,
                    createdUtcTicks = DateTime.UtcNow.Ticks,
                    dueUtcTicks = DateTime.UtcNow.Ticks,
                    testReminder = testReminder
                };

            if (state.activeReminder == null)
            {
                Activate(reminder);
            }
            else
            {
                state.pendingReminders.Add(reminder);
                int recommended = GetBreakMinutes(
                    DeverQuestSettingsStore.Profile,
                    type);
                DeverQuestWellnessHistoryService.Record(
                    type,
                    title,
                    "Queued",
                    "Another reminder is already active.",
                    recommended,
                    (int)Math.Ceiling(recommended * 0.8d),
                    0,
                    testReminder);
                SaveState();
                NotifyChanged();
            }
            return true;
        }

        private static void Activate(
            DeverQuestWellnessReminder reminder)
        {
            state.activeReminder = reminder;
            int recommended = GetBreakMinutes(
                DeverQuestSettingsStore.Profile,
                reminder.type);
            DeverQuestWellnessHistoryService.Record(
                reminder.type,
                reminder.title,
                "Presented",
                reminder.message,
                recommended,
                (int)Math.Ceiling(recommended * 0.8d),
                0,
                reminder.testReminder);
            SaveState();
            DeverQuestWindow.ShowWellnessReminder(
                reminder.title,
                reminder.type);
            NotifyChanged();
        }

        private static void PromoteNextReminder()
        {
            EnsureState();
            if (state.activeReminder != null)
            {
                return;
            }

            long now = DateTime.UtcNow.Ticks;
            DeverQuestWellnessReminder next =
                state.pendingReminders
                    .Where(value => value != null &&
                                    value.dueUtcTicks <= now)
                    .OrderBy(value => value.dueUtcTicks)
                    .ThenBy(value => value.createdUtcTicks)
                    .FirstOrDefault();
            if (next == null)
            {
                return;
            }

            state.pendingReminders.Remove(next);
            Activate(next);
        }

        private static bool ContainsType(
            DeverQuestWellnessType type)
        {
            EnsureState();
            return state.activeReminder?.type == type ||
                   state.pendingReminders.Any(value =>
                       value != null && value.type == type);
        }

        private static void EnsureState()
        {
            if (state == null)
            {
                LoadState();
            }
        }

        private static void LoadState()
        {
            state = new DeverQuestWellnessCommandState();
            string json = EditorPrefs.GetString(StateKey, string.Empty);
            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    state = JsonUtility.FromJson<
                        DeverQuestWellnessCommandState>(json) ??
                            new DeverQuestWellnessCommandState();
                }
                catch (Exception exception)
                {
                    Debug.LogWarning(
                        "[DeverQuest Wellness] Reminder state could not be " +
                        "read. A fresh queue was loaded: " +
                        exception.Message);
                    state = new DeverQuestWellnessCommandState();
                }
            }
            state.Sanitize();
        }

        private static void SaveState()
        {
            EnsureState();
            state.Sanitize();
            EditorPrefs.SetString(
                StateKey,
                JsonUtility.ToJson(state));
        }

        private static void NotifyChanged()
        {
            StateChanged?.Invoke();
        }

        private static bool IsSessionReminder(
            DeverQuestWellnessType type)
        {
            return type == DeverQuestWellnessType.CheckIn ||
                   type == DeverQuestWellnessType.MovementBreak ||
                   type == DeverQuestWellnessType.Hydration ||
                   type == DeverQuestWellnessType.Exercise;
        }

        private static int GetBreakMinutes(
            DeverQuestProfile profile,
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.Lunch:
                case DeverQuestWellnessType.Dinner:
                    return profile.wellnessMealBreakMinutes;
                case DeverQuestWellnessType.QuietHours:
                    return profile.wellnessQuietBreakMinutes;
                default:
                    return profile.wellnessShortBreakMinutes;
            }
        }

        private static int GetIntervalMinutes(
            DeverQuestProfile profile,
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.CheckIn:
                    return profile.checkInMinutes;
                case DeverQuestWellnessType.MovementBreak:
                    return profile.movementBreakMinutes;
                case DeverQuestWellnessType.Hydration:
                    return profile.hydrationMinutes;
                case DeverQuestWellnessType.Exercise:
                    return profile.exerciseMinutes;
                default:
                    return 0;
            }
        }

        private static void MarkDailyReminderHandled(
            DeverQuestWellnessType type)
        {
            string dateKey = DateTime.Now.ToString("yyyy-MM-dd");
            switch (type)
            {
                case DeverQuestWellnessType.Lunch:
                    EditorPrefs.SetString(LunchDateKey, dateKey);
                    break;
                case DeverQuestWellnessType.Dinner:
                    EditorPrefs.SetString(DinnerDateKey, dateKey);
                    break;
                case DeverQuestWellnessType.QuietHours:
                    EditorPrefs.SetString(QuietDateKey, dateKey);
                    break;
            }
        }

        private static string TitleFor(
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.MovementBreak:
                    return "Movement Break";
                case DeverQuestWellnessType.Hydration:
                    return "Hydration Check";
                case DeverQuestWellnessType.Exercise:
                    return "Exercise Break";
                case DeverQuestWellnessType.Lunch:
                    return "Lunch Reminder";
                case DeverQuestWellnessType.Dinner:
                    return "Dinner Reminder";
                case DeverQuestWellnessType.QuietHours:
                    return "Quiet Hours";
                default:
                    return "Focus Check-In";
            }
        }

        private static string MessageFor(
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.MovementBreak:
                    return "Time to step away from the screen, stretch, " +
                           "and move.";
                case DeverQuestWellnessType.Hydration:
                    return "Grab some water and give your eyes a moment " +
                           "away from the screen.";
                case DeverQuestWellnessType.Exercise:
                    return "You have put in serious focus time. Stand up, " +
                           "move, and take a short exercise break.";
                case DeverQuestWellnessType.Lunch:
                    return "Time to stop and eat something before continuing.";
                case DeverQuestWellnessType.Dinner:
                    return "Pause long enough to eat a real meal.";
                case DeverQuestWellnessType.QuietHours:
                    return "It is getting late. Consider finalizing your " +
                           "session and giving yourself a real stopping point.";
                default:
                    return "Are you still working on the task you started " +
                           "this session for?";
            }
        }

        private static bool IsQuietHoursActive(
            DateTime now,
            DeverQuestProfile profile)
        {
            if (profile == null || !profile.quietHoursEnabled)
            {
                return false;
            }
            int start = profile.quietHoursStartHour;
            int end = profile.quietHoursEndHour;
            if (start == end)
            {
                return false;
            }
            return start < end
                ? now.Hour >= start && now.Hour < end
                : now.Hour >= start || now.Hour < end;
        }

        private static DateTime ResolveQuietHoursEnd(
            DateTime now,
            DeverQuestProfile profile)
        {
            DateTime end = now.Date.AddHours(profile.quietHoursEndHour);
            if (profile.quietHoursStartHour > profile.quietHoursEndHour &&
                now.Hour >= profile.quietHoursStartHour)
            {
                end = end.AddDays(1d);
            }
            else if (end <= now)
            {
                end = end.AddDays(1d);
            }
            return end;
        }

        private static string FormatDuration(double seconds)
        {
            TimeSpan span = TimeSpan.FromSeconds(Math.Max(0d, seconds));
            if (span.TotalHours >= 1d)
            {
                return $"{(int)span.TotalHours}h {span.Minutes}m";
            }
            if (span.TotalMinutes >= 1d)
            {
                return $"{(int)span.TotalMinutes}m {span.Seconds}s";
            }
            return $"{Math.Max(0, span.Seconds)}s";
        }
    }
}

//----- DeverQuestWellnessMonitor.cs END -----
