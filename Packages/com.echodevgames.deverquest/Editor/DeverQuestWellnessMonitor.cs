//----- DeverQuestWellnessMonitor.cs START -----

using System;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestWellnessMonitor
    {
        private const string SnoozeUntilKey =
            "EchoDevGames.DeverQuest.Wellness.SnoozeUntil";

        private const string LunchDateKey =
            "EchoDevGames.DeverQuest.Wellness.LunchDate";

        private const string DinnerDateKey =
            "EchoDevGames.DeverQuest.Wellness.DinnerDate";

        private const string QuietDateKey =
            "EchoDevGames.DeverQuest.Wellness.QuietDate";

        private static double nextCheckTime;

        public static bool HasActiveReminder { get; private set; }
        public static DeverQuestWellnessType ActiveType { get; private set; }
        public static string ActiveTitle { get; private set; } = string.Empty;
        public static string ActiveMessage { get; private set; } = string.Empty;

        static DeverQuestWellnessMonitor()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        public static void Acknowledge(bool startBreak)
        {
            if (!HasActiveReminder)
            {
                return;
            }

            DeverQuestWellnessType type = ActiveType;
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;

            if (DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSessionStore.RecordWellnessAction(
                    type,
                    startBreak ? "Break Started" : "Acknowledged",
                    GetIntervalMinutes(profile, type));
            }

            if (!IsSessionReminder(type))
            {
                MarkDailyReminderHandled(type);
            }

            if (startBreak &&
                DeverQuestSessionStore.HasActiveSession &&
                DeverQuestSessionStore.ActiveSession.state ==
                DeverQuestSessionState.Running)
            {
                DeverQuestSessionStore.PauseSession(
                    GetPauseReason(type));
            }

            ClearReminder();
        }

        public static void Snooze()
        {
            if (!HasActiveReminder)
            {
                return;
            }

            int minutes = Math.Max(
                1,
                DeverQuestSettingsStore.Profile.snoozeMinutes);

            long snoozeUntil =
                DateTime.UtcNow.AddMinutes(minutes).Ticks;

            EditorPrefs.SetString(
                SnoozeUntilKey,
                snoozeUntil.ToString());

            ClearReminder();
        }

        private static void Update()
        {
            if (EditorApplication.timeSinceStartup < nextCheckTime)
            {
                return;
            }

            nextCheckTime =
                EditorApplication.timeSinceStartup + 1d;

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;

            if (!profile.setupComplete || !profile.wellnessEnabled)
            {
                ClearReminder();
                return;
            }

            if (HasActiveReminder || IsSnoozed())
            {
                return;
            }

            if (TryTriggerDailyReminder(profile))
            {
                return;
            }

            if (!DeverQuestSessionStore.HasActiveSession ||
                DeverQuestSessionStore.ActiveSession.state !=
                DeverQuestSessionState.Running)
            {
                return;
            }

            DeverQuestSessionStore.EnsureWellnessSchedule(profile);

            double focused =
                DeverQuestSessionStore.GetFocusedSeconds();

            if (TryTriggerFocused(
                    DeverQuestWellnessType.Exercise,
                    focused,
                    profile.exerciseMinutes,
                    "Exercise Break",
                    "You have put in serious focus time. Stand up, move, " +
                    "and do a short exercise break."))
            {
                return;
            }

            if (TryTriggerFocused(
                    DeverQuestWellnessType.MovementBreak,
                    focused,
                    profile.movementBreakMinutes,
                    "Movement Break",
                    "Time to step away from the screen, stretch, and move."))
            {
                return;
            }

            if (TryTriggerFocused(
                    DeverQuestWellnessType.Hydration,
                    focused,
                    profile.hydrationMinutes,
                    "Hydration Check",
                    "Grab some water and give your eyes a moment away " +
                    "from the screen."))
            {
                return;
            }

            TryTriggerFocused(
                DeverQuestWellnessType.CheckIn,
                focused,
                profile.checkInMinutes,
                "Focus Check-In",
                "Are you still working on the task you started this " +
                "session for?");
        }

        private static bool TryTriggerDailyReminder(
            DeverQuestProfile profile)
        {
            DateTime now = DateTime.Now;
            string dateKey = now.ToString("yyyy-MM-dd");

            DateTime dinnerTime = now.Date
                .AddHours(profile.dinnerHour)
                .AddMinutes(profile.dinnerMinute);

            if (profile.mealRemindersEnabled &&
                now >= dinnerTime &&
                EditorPrefs.GetString(DinnerDateKey, string.Empty) != dateKey)
            {
                Trigger(
                    DeverQuestWellnessType.Dinner,
                    "Dinner Reminder",
                    "Pause long enough to eat a real meal.");
                return true;
            }

            DateTime lunchTime = now.Date
                .AddHours(profile.lunchHour)
                .AddMinutes(profile.lunchMinute);

            if (profile.mealRemindersEnabled &&
                now >= lunchTime &&
                now < dinnerTime &&
                EditorPrefs.GetString(LunchDateKey, string.Empty) != dateKey)
            {
                Trigger(
                    DeverQuestWellnessType.Lunch,
                    "Lunch Reminder",
                    "Time to stop and eat something before continuing.");
                return true;
            }

            if (profile.quietHoursEnabled &&
                now.Hour >= profile.quietHoursStartHour &&
                EditorPrefs.GetString(QuietDateKey, string.Empty) != dateKey)
            {
                Trigger(
                    DeverQuestWellnessType.QuietHours,
                    "Quiet Hours",
                    "It is getting late. Consider finalizing your session " +
                    "and giving yourself a real stopping point.");
                return true;
            }

            return false;
        }

        private static bool TryTriggerFocused(
            DeverQuestWellnessType type,
            double focusedSeconds,
            int intervalMinutes,
            string title,
            string message)
        {
            if (intervalMinutes <= 0 ||
                focusedSeconds <
                DeverQuestSessionStore.GetNextWellnessSeconds(type))
            {
                return false;
            }

            Trigger(type, title, message);
            return true;
        }

        private static void Trigger(
            DeverQuestWellnessType type,
            string title,
            string message)
        {
            ActiveType = type;
            ActiveTitle = title;
            ActiveMessage = message;
            HasActiveReminder = true;

            DeverQuestWindow.ShowWellnessReminder(title);
        }

        private static void ClearReminder()
        {
            HasActiveReminder = false;
            ActiveTitle = string.Empty;
            ActiveMessage = string.Empty;
        }

        private static bool IsSnoozed()
        {
            string value =
                EditorPrefs.GetString(SnoozeUntilKey, string.Empty);

            if (!long.TryParse(value, out long ticks))
            {
                return false;
            }

            if (DateTime.UtcNow.Ticks < ticks)
            {
                return true;
            }

            EditorPrefs.DeleteKey(SnoozeUntilKey);
            return false;
        }

        private static bool IsSessionReminder(
            DeverQuestWellnessType type)
        {
            return type == DeverQuestWellnessType.CheckIn ||
                   type == DeverQuestWellnessType.MovementBreak ||
                   type == DeverQuestWellnessType.Hydration ||
                   type == DeverQuestWellnessType.Exercise;
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

        private static string GetPauseReason(
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.Lunch:
                    return "Lunch Break";
                case DeverQuestWellnessType.Dinner:
                    return "Dinner Break";
                case DeverQuestWellnessType.Exercise:
                    return "Exercise Break";
                default:
                    return "Wellness Break";
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
    }
}

//----- DeverQuestWellnessMonitor.cs END -----
