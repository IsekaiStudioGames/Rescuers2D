//----- DeverQuestHistoryService.cs START -----

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestHistoryRange
    {
        AllTime = 0,
        Today = 1,
        Last7Days = 2,
        Last30Days = 3,
        Custom = 4
    }

    internal sealed class DeverQuestHistoryDay
    {
        public DateTime Date;
        public string DataPath = string.Empty;
        public string MarkdownPath = string.Empty;
        public DeverQuestDailyRecord Record;
        public DeverQuestIntegrityStatus IntegrityStatus;
        public string IntegrityMessage = string.Empty;
        public int SuspiciousSessionCount;
        public bool SuspiciousFrequency;
    }

    internal sealed class DeverQuestHistorySummary
    {
        public int DayCount;
        public int SessionCount;
        public int CommitCount;
        public int BreakCount;
        public double FocusedSeconds;
        public double PausedSeconds;
        public double RewardMinutesEarned;
        public double RewardMinutesSpent;
        public long CopperEarned;
        public long CopperSpent;
        public long ExperienceEarned;
    }

    internal sealed class DeverQuestNamedSummary
    {
        public string Name = string.Empty;
        public int SessionCount;
        public double FocusedSeconds;
    }

    internal sealed class DeverQuestGoalStatistics
    {
        public double TodayFocusedSeconds;
        public int CurrentStreak;
        public int LongestStreak;
        public int GoalDays;
        public bool TodayGoalComplete;
    }

    [Serializable]
    internal sealed class DeverQuestHistoryExport
    {
        public string exportedUtc = string.Empty;
        public string developerName = string.Empty;
        public List<DeverQuestDailyRecord> days =
            new List<DeverQuestDailyRecord>();
    }

    internal static class DeverQuestHistoryService
    {
        private static readonly List<DeverQuestHistoryDay> Days =
            new List<DeverQuestHistoryDay>();

        public static IReadOnlyList<DeverQuestHistoryDay> AllDays => Days;
        public static bool IsLoaded { get; private set; }
        public static string LastError { get; private set; } = string.Empty;

        public static void Refresh(DeverQuestProfile profile)
        {
            Days.Clear();
            LastError = string.Empty;
            IsLoaded = true;

            if (profile == null)
            {
                LastError = "Developer profile was unavailable.";
                return;
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            if (!Directory.Exists(developerFolder))
            {
                LastError =
                    "The configured developer timecard folder does not exist.";
                return;
            }

            try
            {
                string[] files = Directory.GetFiles(
                    developerFolder,
                    "*.deverquest.json",
                    SearchOption.AllDirectories);

                foreach (string dataPath in files)
                {
                    TryLoadDay(dataPath);
                }

                int frequencyLimit =
                    profile.suspiciousDailyQuestCount;
                if (frequencyLimit > 0)
                {
                    foreach (IGrouping<DateTime, DeverQuestHistoryDay> group
                             in Days.GroupBy(day => day.Date.Date))
                    {
                        bool flagged = group.Sum(
                            day => day.Record.sessions.Count) >=
                            frequencyLimit;
                        foreach (DeverQuestHistoryDay day in group)
                        {
                            day.SuspiciousFrequency = flagged;
                        }
                    }
                }

                Days.Sort(
                    (left, right) =>
                        right.Date.CompareTo(left.Date));
            }
            catch (Exception exception)
            {
                LastError = exception.Message;
            }
        }

        public static List<DeverQuestHistoryDay> GetFilteredDays(
            DateTime? startDate,
            DateTime? endDate,
            string projectFilter,
            string categoryFilter)
        {
            string project =
                projectFilter?.Trim() ?? string.Empty;

            string category =
                categoryFilter?.Trim() ?? string.Empty;

            List<DeverQuestHistoryDay> filtered =
                new List<DeverQuestHistoryDay>();

            foreach (DeverQuestHistoryDay day in Days)
            {
                if (startDate.HasValue &&
                    day.Date.Date < startDate.Value.Date)
                {
                    continue;
                }

                if (endDate.HasValue &&
                    day.Date.Date > endDate.Value.Date)
                {
                    continue;
                }

                List<DeverQuestSession> matchingSessions =
                    day.Record.sessions
                        .Where(
                            session =>
                                Matches(
                                    session.projectName,
                                    project) &&
                                Matches(
                                    session.category,
                                    category))
                        .ToList();

                if (matchingSessions.Count == 0)
                {
                    continue;
                }

                filtered.Add(
                    new DeverQuestHistoryDay
                    {
                        Date = day.Date,
                        DataPath = day.DataPath,
                        MarkdownPath = day.MarkdownPath,
                        Record = new DeverQuestDailyRecord
                        {
                            developerName =
                                day.Record.developerName,
                            localDate = day.Record.localDate,
                            chronicleIndex =
                                day.Record.chronicleIndex,
                            sessions = matchingSessions
                        },
                        IntegrityStatus = day.IntegrityStatus,
                        IntegrityMessage = day.IntegrityMessage,
                        SuspiciousSessionCount =
                            matchingSessions.Count(
                                IsSuspiciousSession),
                        SuspiciousFrequency =
                            day.SuspiciousFrequency
                    });
            }

            return filtered;
        }

        public static DeverQuestHistorySummary BuildSummary(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            DeverQuestHistorySummary summary =
                new DeverQuestHistorySummary
                {
                    DayCount = days.Count
                };

            foreach (DeverQuestHistoryDay day in days)
            {
                foreach (DeverQuestSession session
                         in day.Record.sessions)
                {
                    summary.SessionCount++;
                    summary.FocusedSeconds +=
                        session.accumulatedFocusedSeconds;
                    summary.PausedSeconds +=
                        session.accumulatedPausedSeconds;
                    summary.CommitCount +=
                        session.commitEntries?.Count ?? 0;
                    summary.BreakCount +=
                        session.wellnessEvents?.Count(
                            wellnessEvent =>
                                wellnessEvent.action ==
                                "Break Started") ?? 0;

                    if (session.rewardTransactions == null)
                    {
                        continue;
                    }

                    foreach (DeverQuestRewardTransaction transaction
                             in session.rewardTransactions)
                    {
                        if (transaction.copper >= 0L)
                        {
                            summary.CopperEarned += transaction.copper;
                        }
                        else
                        {
                            summary.CopperSpent += -transaction.copper;
                        }
                        summary.ExperienceEarned +=
                            Math.Max(0L, transaction.experience);

                        if (transaction.minutes >= 0d)
                        {
                            summary.RewardMinutesEarned +=
                                transaction.minutes;
                        }
                        else
                        {
                            summary.RewardMinutesSpent +=
                                -transaction.minutes;
                        }
                    }
                }
            }

            return summary;
        }

        public static List<DeverQuestNamedSummary> BuildProjectSummaries(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            return BuildNamedSummaries(
                days,
                session => session.projectName,
                "Unspecified Project");
        }

        public static List<DeverQuestNamedSummary> BuildCategorySummaries(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            return BuildNamedSummaries(
                days,
                session => session.category,
                "Uncategorized");
        }

        public static List<DeverQuestNamedSummary> BuildWeeklySummaries(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            Dictionary<DateTime, DeverQuestNamedSummary> summaries =
                new Dictionary<DateTime, DeverQuestNamedSummary>();

            foreach (DeverQuestHistoryDay day in days)
            {
                DateTime weekStart = GetWeekStart(day.Date);

                if (!summaries.TryGetValue(
                        weekStart,
                        out DeverQuestNamedSummary summary))
                {
                    summary = new DeverQuestNamedSummary
                    {
                        Name =
                            $"{weekStart:MMM d} – " +
                            $"{weekStart.AddDays(6):MMM d, yyyy}"
                    };

                    summaries.Add(weekStart, summary);
                }

                summary.SessionCount += day.Record.sessions.Count;
                summary.FocusedSeconds +=
                    day.Record.sessions.Sum(
                        session =>
                            session.accumulatedFocusedSeconds);
            }

            return summaries
                .OrderByDescending(pair => pair.Key)
                .Select(pair => pair.Value)
                .ToList();
        }

        public static DeverQuestGoalStatistics BuildGoalStatistics(
            int dailyGoalMinutes)
        {
            DeverQuestGoalStatistics statistics =
                new DeverQuestGoalStatistics();

            double goalSeconds = Math.Max(0, dailyGoalMinutes) * 60d;
            if (goalSeconds <= 0d)
            {
                return statistics;
            }

            Dictionary<DateTime, double> dailyTotals = Days
                .GroupBy(day => day.Date.Date)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(
                        day => day.Record.sessions.Sum(
                            session =>
                                session.accumulatedFocusedSeconds)));

            dailyTotals.TryGetValue(
                DateTime.Today,
                out statistics.TodayFocusedSeconds);

            HashSet<DateTime> achieved = new HashSet<DateTime>(
                dailyTotals
                    .Where(pair => pair.Value >= goalSeconds)
                    .Select(pair => pair.Key));

            statistics.GoalDays = achieved.Count;
            statistics.TodayGoalComplete =
                achieved.Contains(DateTime.Today);

            DateTime cursor = statistics.TodayGoalComplete
                ? DateTime.Today
                : DateTime.Today.AddDays(-1);

            while (achieved.Contains(cursor))
            {
                statistics.CurrentStreak++;
                cursor = cursor.AddDays(-1);
            }

            DateTime? previous = null;
            int run = 0;
            foreach (DateTime date in achieved.OrderBy(date => date))
            {
                run = previous.HasValue &&
                      date == previous.Value.AddDays(1)
                    ? run + 1
                    : 1;
                statistics.LongestStreak =
                    Math.Max(statistics.LongestStreak, run);
                previous = date;
            }

            return statistics;
        }

        public static bool TryExportCsv(
            string path,
            IReadOnlyList<DeverQuestHistoryDay> days,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(
                    "Date,Developer,Project,Department,Task,Started,Ended," +
                    "FocusedSeconds,PausedSeconds,Commits,Breaks," +
                    "RewardMinutesEarned,RewardMinutesSpent");

                foreach (DeverQuestHistoryDay day in days)
                {
                    foreach (DeverQuestSession session
                             in day.Record.sessions)
                    {
                        double earned =
                            session.rewardTransactions?
                                .Where(item => item.minutes > 0d)
                                .Sum(item => item.minutes) ?? 0d;

                        double spent =
                            session.rewardTransactions?
                                .Where(item => item.minutes < 0d)
                                .Sum(item => -item.minutes) ?? 0d;

                        int breaks =
                            session.wellnessEvents?.Count(
                                item =>
                                    item.action ==
                                    "Break Started") ?? 0;

                        builder.AppendLine(
                            string.Join(
                                ",",
                                Csv(day.Date.ToString("yyyy-MM-dd")),
                                Csv(day.Record.developerName),
                                Csv(session.projectName),
                                Csv(session.category),
                                Csv(session.taskName),
                                Csv(DeverQuestSessionStore
                                    .GetLocalStartTime(session)
                                    .ToString("s")),
                                Csv(DeverQuestSessionStore
                                    .GetLocalCompletionTime(session)
                                    .ToString("s")),
                                session.accumulatedFocusedSeconds
                                    .ToString(
                                        "0.###",
                                        CultureInfo.InvariantCulture),
                                session.accumulatedPausedSeconds
                                    .ToString(
                                        "0.###",
                                        CultureInfo.InvariantCulture),
                                (session.commitEntries?.Count ?? 0)
                                    .ToString(),
                                breaks.ToString(),
                                earned.ToString(
                                    "0.###",
                                    CultureInfo.InvariantCulture),
                                spent.ToString(
                                    "0.###",
                                    CultureInfo.InvariantCulture)));
                    }
                }

                File.WriteAllText(
                    path,
                    builder.ToString(),
                    new UTF8Encoding(false));

                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        public static bool TryExportJson(
            string path,
            string developerName,
            IReadOnlyList<DeverQuestHistoryDay> days,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            try
            {
                DeverQuestHistoryExport export =
                    new DeverQuestHistoryExport
                    {
                        exportedUtc =
                            DateTime.UtcNow.ToString("O"),
                        developerName = developerName,
                        days = days
                            .Select(day => day.Record)
                            .ToList()
                    };

                File.WriteAllText(
                    path,
                    JsonUtility.ToJson(export, true),
                    new UTF8Encoding(false));

                return true;
            }
            catch (Exception exception)
            {
                errorMessage = exception.Message;
                return false;
            }
        }

        private static void TryLoadDay(string dataPath)
        {
            try
            {
                DeverQuestDailyRecord record =
                    JsonUtility.FromJson<DeverQuestDailyRecord>(
                        File.ReadAllText(dataPath));

                if (record == null ||
                    record.sessions == null ||
                    !DateTime.TryParseExact(
                        record.localDate,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date))
                {
                    return;
                }

                foreach (DeverQuestSession session in record.sessions)
                {
                    session?.Sanitize();
                }

                const string dataSuffix = ".deverquest.json";

                string markdownPath =
                    dataPath.EndsWith(
                        dataSuffix,
                        StringComparison.OrdinalIgnoreCase)
                        ? dataPath.Substring(
                              0,
                              dataPath.Length - dataSuffix.Length) +
                          ".md"
                        : Path.ChangeExtension(dataPath, ".md");

                DeverQuestIntegrityStatus integrityStatus =
                    DeverQuestChronicleIntegrityService.Verify(
                        dataPath,
                        out string integrityMessage);

                Days.Add(
                    new DeverQuestHistoryDay
                    {
                        Date = date,
                        DataPath = dataPath,
                        MarkdownPath = markdownPath,
                        Record = record,
                        IntegrityStatus = integrityStatus,
                        IntegrityMessage = integrityMessage,
                        SuspiciousSessionCount =
                            record.sessions.Count(IsSuspiciousSession)
                    });
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[DeverQuest] Could not read history file " +
                    $"{dataPath}: {exception.Message}");
            }
        }

        private static List<DeverQuestNamedSummary> BuildNamedSummaries(
            IReadOnlyList<DeverQuestHistoryDay> days,
            Func<DeverQuestSession, string> selector,
            string fallbackName)
        {
            return days
                .SelectMany(day => day.Record.sessions)
                .GroupBy(
                    session =>
                        string.IsNullOrWhiteSpace(selector(session))
                            ? fallbackName
                            : selector(session))
                .Select(
                    group =>
                        new DeverQuestNamedSummary
                        {
                            Name = group.Key,
                            SessionCount = group.Count(),
                            FocusedSeconds = group.Sum(
                                session =>
                                    session.accumulatedFocusedSeconds)
                        })
                .OrderByDescending(summary => summary.FocusedSeconds)
                .ToList();
        }

        private static bool Matches(
            string value,
            string filter)
        {
            return string.IsNullOrWhiteSpace(filter) ||
                   (value ?? string.Empty).IndexOf(
                       filter,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static bool IsSuspiciousSession(
            DeverQuestSession session)
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            return profile.suspiciousQuestMinutes > 0 &&
                   session.accumulatedFocusedSeconds >=
                   profile.suspiciousQuestMinutes * 60d;
        }

        private static DateTime GetWeekStart(DateTime date)
        {
            int difference =
                (7 + (int)date.DayOfWeek -
                 (int)DayOfWeek.Monday) % 7;

            return date.Date.AddDays(-difference);
        }

        private static string Csv(string value)
        {
            string escaped =
                (value ?? string.Empty).Replace("\"", "\"\"");

            return $"\"{escaped}\"";
        }
    }
}

//----- DeverQuestHistoryService.cs END -----
