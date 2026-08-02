//----- DeverQuestWellnessHistoryService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestWellnessHistoryRecord
    {
        public string recordId = string.Empty;
        public DeverQuestWellnessType type;
        public string title = string.Empty;
        public string action = string.Empty;
        public string detail = string.Empty;
        public string sessionId = string.Empty;
        public long createdUtcTicks;
        public double focusedSeconds;
        public int recommendedMinutes;
        public int requiredMinutes;
        public int snoozeMinutes;
        public bool testRecord;

        public void Sanitize()
        {
            if (string.IsNullOrWhiteSpace(recordId))
            {
                recordId = Guid.NewGuid().ToString("N");
            }
            title = title?.Trim() ?? string.Empty;
            action = action?.Trim() ?? string.Empty;
            detail = detail?.Trim() ?? string.Empty;
            sessionId = sessionId?.Trim() ?? string.Empty;
            createdUtcTicks = Math.Max(0L, createdUtcTicks);
            focusedSeconds = Math.Max(0d, focusedSeconds);
            recommendedMinutes = Math.Max(0, recommendedMinutes);
            requiredMinutes = Math.Max(0, requiredMinutes);
            snoozeMinutes = Math.Max(0, snoozeMinutes);
        }
    }

    [Serializable]
    internal sealed class DeverQuestWellnessHistoryData
    {
        public List<DeverQuestWellnessHistoryRecord> records =
            new List<DeverQuestWellnessHistoryRecord>();

        public void Sanitize()
        {
            records = records ??
                      new List<DeverQuestWellnessHistoryRecord>();
            records.RemoveAll(value => value == null);
            foreach (DeverQuestWellnessHistoryRecord record in records)
            {
                record.Sanitize();
            }
        }
    }

    [InitializeOnLoad]
    internal static class DeverQuestWellnessHistoryService
    {
        private static string HistoryPath => Path.GetFullPath(
            Path.Combine(
                Application.dataPath,
                "..",
                "Library",
                "DeverQuest",
                "WellnessHistory.json"));

        private static DeverQuestWellnessHistoryData data;

        static DeverQuestWellnessHistoryService()
        {
            Load();
        }

        public static IReadOnlyList<DeverQuestWellnessHistoryRecord> Records
        {
            get
            {
                EnsureLoaded();
                return data.records
                    .OrderByDescending(value => value.createdUtcTicks)
                    .ToList();
            }
        }

        public static void Record(
            DeverQuestWellnessType type,
            string title,
            string action,
            string detail,
            int recommendedMinutes,
            int requiredMinutes,
            int snoozeMinutes = 0,
            bool testRecord = false)
        {
            try
            {
                EnsureLoaded();
                DeverQuestSession session =
                    DeverQuestSessionStore.HasActiveSession
                        ? DeverQuestSessionStore.ActiveSession
                        : null;
                data.records.Add(
                    new DeverQuestWellnessHistoryRecord
                    {
                        recordId = Guid.NewGuid().ToString("N"),
                        type = type,
                        title = title ?? string.Empty,
                        action = action ?? string.Empty,
                        detail = detail ?? string.Empty,
                        sessionId = session?.sessionId ?? string.Empty,
                        createdUtcTicks = DateTime.UtcNow.Ticks,
                        focusedSeconds = session == null
                            ? 0d
                            : DeverQuestSessionStore.GetFocusedSeconds(),
                        recommendedMinutes =
                            Math.Max(0, recommendedMinutes),
                        requiredMinutes = Math.Max(0, requiredMinutes),
                        snoozeMinutes = Math.Max(0, snoozeMinutes),
                        testRecord = testRecord
                    });
                TrimAndSave();
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest Wellness History] Recording failed " +
                    "without interrupting the reminder: " +
                    exception.Message);
            }
        }

        public static void RecordBreakOutcome(
            DeverQuestWellnessType type,
            bool completed,
            int plannedMinutes,
            double actualSeconds)
        {
            int requiredMinutes = (int)Math.Ceiling(
                Math.Max(1, plannedMinutes) * 0.8d);
            Record(
                type,
                WellnessTitle(type),
                completed ? "Break Completed" : "Break Ended Early",
                $"{actualSeconds / 60d:0.0} of " +
                $"{Math.Max(1, plannedMinutes)} planned minutes",
                Math.Max(1, plannedMinutes),
                requiredMinutes);
        }

        public static void Clear()
        {
            data = new DeverQuestWellnessHistoryData();
            try
            {
                if (File.Exists(HistoryPath))
                {
                    File.Delete(HistoryPath);
                }
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest Wellness History] The local history " +
                    "could not be cleared: " + exception.Message);
            }
        }

        public static bool CanWrite(out string reason)
        {
            reason = string.Empty;
            try
            {
                string directory = Path.GetDirectoryName(HistoryPath);
                if (string.IsNullOrWhiteSpace(directory))
                {
                    reason = "The Wellness History directory could not be " +
                             "resolved.";
                    return false;
                }
                Directory.CreateDirectory(directory);
                string probe = Path.Combine(
                    directory,
                    "WellnessHistory.probe");
                File.WriteAllText(probe, "ok");
                bool success = string.Equals(
                    File.ReadAllText(probe),
                    "ok",
                    StringComparison.Ordinal);
                File.Delete(probe);
                if (!success)
                {
                    reason = "The Wellness History write probe did not " +
                             "round-trip through the Library folder.";
                }
                return success;
            }
            catch (Exception exception)
            {
                reason = exception.Message;
                return false;
            }
        }

        private static void EnsureLoaded()
        {
            if (data == null)
            {
                Load();
            }
        }

        private static void Load()
        {
            data = new DeverQuestWellnessHistoryData();
            try
            {
                if (File.Exists(HistoryPath))
                {
                    string json = File.ReadAllText(HistoryPath);
                    data = JsonUtility.FromJson<
                        DeverQuestWellnessHistoryData>(json) ??
                           new DeverQuestWellnessHistoryData();
                }
                data.Sanitize();
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    "[DeverQuest Wellness History] The local history " +
                    "could not be read. A fresh history was loaded: " +
                    exception.Message);
                data = new DeverQuestWellnessHistoryData();
            }
        }

        private static void TrimAndSave()
        {
            int maximum = Math.Max(
                25,
                DeverQuestSettingsStore.Profile.wellnessHistoryLimit);
            data.records = data.records
                .Where(value => value != null)
                .OrderByDescending(value => value.createdUtcTicks)
                .Take(maximum)
                .ToList();
            Save();
        }

        private static void Save()
        {
            string directory = Path.GetDirectoryName(HistoryPath);
            if (string.IsNullOrWhiteSpace(directory))
            {
                return;
            }
            Directory.CreateDirectory(directory);
            File.WriteAllText(
                HistoryPath,
                JsonUtility.ToJson(data, true));
        }

        private static string WellnessTitle(
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
    }
}

//----- DeverQuestWellnessHistoryService.cs END -----
