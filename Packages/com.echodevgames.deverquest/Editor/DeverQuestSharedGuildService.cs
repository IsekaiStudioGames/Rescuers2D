using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestSharedQuestRecord
    {
        public int schemaVersion = 2;
        public string sessionId = string.Empty;
        public string accountId = string.Empty;
        public string developerName = string.Empty;
        public string adventurerName = string.Empty;
        public string characterClass = string.Empty;
        public string classId = string.Empty;
        public string ancestryName = string.Empty;
        public string ancestryId = string.Empty;
        public string deityName = string.Empty;
        public string deityId = string.Empty;
        public string alignment = string.Empty;
        public string guildRank = string.Empty;
        public int level = 1;
        public string projectName = string.Empty;
        public string department = string.Empty;
        public string taskName = string.Empty;
        public string contractTitle = string.Empty;
        public string localDate = string.Empty;
        public string startedUtc = string.Empty;
        public string completedUtc = string.Empty;
        public double focusedSeconds;
        public double pausedSeconds;
        public double approvedBreakSeconds;
        public double idleUnverifiedSeconds;
        public int commitCount;
        public int breakCount;
        public int mediaAttachmentCount;
        public long copperEarned;
        public long experienceEarned;
        public bool suspiciousDuration;
        public bool suspiciousIdleRatio;
        public string publishedUtc = string.Empty;
        public string integrityHash = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestSharedAdventurerSnapshot
    {
        public int schemaVersion = 3;
        public string accountId = string.Empty;
        public string developerName = string.Empty;
        public string adventurerName = string.Empty;
        public string characterClass = string.Empty;
        public string classId = string.Empty;
        public string ancestryName = string.Empty;
        public string ancestryId = string.Empty;
        public string deityName = string.Empty;
        public string deityId = string.Empty;
        public string alignment = string.Empty;
        public string guildRank = string.Empty;
        public string homeDepartment = string.Empty;
        public int level = 1;
        public long lifetimeExperience;
        public long copperBalance;
        public string activeCompanionName = string.Empty;
        public int activeCompanionLevel;
        public int companionRosterCount;
        public string updatedUtc = string.Empty;
    }

    internal sealed class DeverQuestHallEntry
    {
        public string AccountId = string.Empty;
        public string DeveloperName = string.Empty;
        public string AdventurerName = string.Empty;
        public string CharacterClass = string.Empty;
        public string AncestryName = string.Empty;
        public string GuildRank = string.Empty;
        public int Level;
        public double RawFocusedSeconds;
        public double RankedFocusedSeconds;
        public long ExperienceEarned;
        public long CopperEarned;
        public int QuestCount;
        public int ContractCount;
        public int CurrentStreak;
        public int ReviewFlagCount;
    }

    internal sealed class DeverQuestGuildNamedReport
    {
        public string Name = string.Empty;
        public double FocusedSeconds;
        public int QuestCount;
        public int AdventurerCount;
    }

    [InitializeOnLoad]
    internal static class DeverQuestSharedGuildService
    {
        private const string RecordsFolder = "Records";
        private const string AdventurersFolder = "Adventurers";
        private static readonly List<DeverQuestHallEntry> HallEntries =
            new List<DeverQuestHallEntry>();
        private static readonly List<DeverQuestGuildNamedReport>
            ProjectReports = new List<DeverQuestGuildNamedReport>();
        private static readonly List<DeverQuestGuildNamedReport>
            DepartmentReports = new List<DeverQuestGuildNamedReport>();

        static DeverQuestSharedGuildService()
        {
            DeverQuestSessionStore.SessionFinalized -=
                OnSessionFinalized;
            DeverQuestSessionStore.SessionFinalized +=
                OnSessionFinalized;
        }

        public static IReadOnlyList<DeverQuestHallEntry> Hall =>
            HallEntries;
        public static IReadOnlyList<DeverQuestGuildNamedReport> Projects =>
            ProjectReports;
        public static IReadOnlyList<DeverQuestGuildNamedReport> Departments =>
            DepartmentReports;
        public static string LastMessage { get; private set; } =
            string.Empty;
        public static int InvalidRecordCount { get; private set; }
        public static int PublishedRecordCount { get; private set; }

        public static bool ValidateRepository(out string message)
        {
            message = string.Empty;
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.sharedGuildEnabled)
            {
                message = "Shared Guild records are disabled.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath))
            {
                message = "Choose a shared Guild repository folder.";
                return false;
            }
            try
            {
                Directory.CreateDirectory(
                    Path.Combine(
                        profile.sharedGuildRepositoryPath,
                        RecordsFolder));
                Directory.CreateDirectory(
                    Path.Combine(
                        profile.sharedGuildRepositoryPath,
                        AdventurersFolder));
                string probe = Path.Combine(
                    profile.sharedGuildRepositoryPath,
                    ".deverquest-write-test");
                File.WriteAllText(probe, DateTime.UtcNow.ToString("O"));
                File.Delete(probe);
                message =
                    "Shared Guild repository is available.";
                return true;
            }
            catch (Exception exception)
            {
                message =
                    "Shared Guild repository is unavailable: " +
                    exception.Message;
                return false;
            }
        }

        public static bool PublishLastCompleted(out string message)
        {
            return Publish(
                DeverQuestSessionStore.LastCompletedSession,
                out message);
        }

        public static bool Publish(
            DeverQuestSession session,
            out string message)
        {
            message = string.Empty;
            if (session == null ||
                session.state != DeverQuestSessionState.Completed)
            {
                message = "No completed Quest is available to publish.";
                return false;
            }
            if (!ValidateRepository(out message))
            {
                return false;
            }

            try
            {
                DeverQuestGuildAccount account =
                    DeverQuestGuildAccountService.CurrentAccount;
                DeverQuestAdventurer adventurer =
                    DeverQuestAdventurerService.Adventurer;
                string accountId =
                    account?.accountId;
                if (string.IsNullOrWhiteSpace(accountId))
                {
                    accountId =
                        DeverQuestPathUtility.MakeSafeFolderName(
                            session.developerName);
                }

                DeverQuestSharedQuestRecord record =
                    BuildRecord(
                        session,
                        accountId,
                        adventurer);
                record.integrityHash = ComputeHash(record);

                string accountFolder = Path.Combine(
                    DeverQuestSettingsStore.Profile
                        .sharedGuildRepositoryPath,
                    RecordsFolder,
                    DeverQuestPathUtility.MakeSafeFolderName(
                        accountId),
                    record.localDate);
                Directory.CreateDirectory(accountFolder);
                string recordPath = Path.Combine(
                    accountFolder,
                    DeverQuestPathUtility.MakeSafeFolderName(
                        record.sessionId) +
                    ".guildquest.json");
                if (!File.Exists(recordPath))
                {
                    WriteAtomic(
                        recordPath,
                        JsonUtility.ToJson(record, true));
                }

                WriteAdventurerSnapshot(
                    accountId,
                    session,
                    adventurer);
                DeverQuestGuildAccountService.AddAudit(
                    "Shared Quest Published",
                    session.taskName,
                    record.sessionId);
                message =
                    $"Published {session.taskName} to the Guild repository.";
                LastMessage = message;
                Refresh();
                return true;
            }
            catch (Exception exception)
            {
                message =
                    "Shared Quest publishing failed: " +
                    exception.Message;
                LastMessage = message;
                return false;
            }
        }

        public static void Refresh()
        {
            HallEntries.Clear();
            ProjectReports.Clear();
            DepartmentReports.Clear();
            InvalidRecordCount = 0;
            PublishedRecordCount = 0;

            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.sharedGuildEnabled ||
                string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath))
            {
                LastMessage =
                    "Configure a shared Guild repository to load rankings.";
                return;
            }
            string recordsRoot = Path.Combine(
                profile.sharedGuildRepositoryPath,
                RecordsFolder);
            if (!Directory.Exists(recordsRoot))
            {
                LastMessage =
                    "Configure a shared Guild repository to load rankings.";
                return;
            }

            try
            {
                List<DeverQuestSharedQuestRecord> records =
                    new List<DeverQuestSharedQuestRecord>();
                foreach (string path in Directory.GetFiles(
                             recordsRoot,
                             "*.guildquest.json",
                             SearchOption.AllDirectories))
                {
                    try
                    {
                        DeverQuestSharedQuestRecord record =
                            JsonUtility.FromJson<
                                DeverQuestSharedQuestRecord>(
                                    File.ReadAllText(path));
                        if (record == null ||
                            !string.Equals(
                                record.integrityHash,
                                ComputeHash(record),
                                StringComparison.OrdinalIgnoreCase))
                        {
                            InvalidRecordCount++;
                            continue;
                        }
                        records.Add(record);
                    }
                    catch
                    {
                        InvalidRecordCount++;
                    }
                }

                PublishedRecordCount = records.Count;
                BuildHall(records, profile);
                BuildNamedReports(
                    records,
                    value => value.projectName,
                    "Unspecified Project",
                    ProjectReports);
                BuildNamedReports(
                    records,
                    value => value.department,
                    "Unspecified Department",
                    DepartmentReports);
                LastMessage =
                    $"Loaded {PublishedRecordCount} shared Quest record(s).";
            }
            catch (Exception exception)
            {
                LastMessage =
                    "Shared Guild refresh failed: " +
                    exception.Message;
            }
        }

        private static void OnSessionFinalized()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.sharedGuildEnabled ||
                !profile.publishCompletedQuests)
            {
                return;
            }
            PublishLastCompleted(out _);
        }

        private static DeverQuestSharedQuestRecord BuildRecord(
            DeverQuestSession session,
            string accountId,
            DeverQuestAdventurer adventurer)
        {
            long copper = session.rewardTransactions?.Sum(
                value => Math.Max(0L, value?.copper ?? 0L)) ?? 0L;
            long experience = session.rewardTransactions?.Sum(
                value => Math.Max(0L, value?.experience ?? 0L)) ?? 0L;
            double paused =
                Math.Max(0d, session.accumulatedPausedSeconds);
            double idleRatio = paused <= 0d
                ? 0d
                : session.idleUnverifiedSeconds / paused;
            DateTime startedUtc =
                new DateTime(
                    session.startedUtcTicks,
                    DateTimeKind.Utc);
            DateTime completedUtc =
                new DateTime(
                    session.completedUtcTicks,
                    DateTimeKind.Utc);

            return new DeverQuestSharedQuestRecord
            {
                sessionId = session.sessionId,
                accountId = accountId,
                developerName = session.developerName,
                adventurerName = adventurer.characterName,
                characterClass = adventurer.characterClass,
                classId = adventurer.classId,
                ancestryName = adventurer.ancestryName,
                ancestryId = adventurer.ancestryId,
                deityName = adventurer.deityName,
                deityId = adventurer.deityId,
                alignment = adventurer.alignment.ToString(),
                guildRank = adventurer.guildRank,
                level = adventurer.level,
                projectName = session.projectName,
                department = session.category,
                taskName = session.taskName,
                contractTitle = session.questContractTitle,
                localDate = startedUtc.ToLocalTime()
                    .ToString("yyyy-MM-dd"),
                startedUtc = startedUtc.ToString("O"),
                completedUtc = completedUtc.ToString("O"),
                focusedSeconds =
                    Math.Max(0d, session.accumulatedFocusedSeconds),
                pausedSeconds = paused,
                approvedBreakSeconds =
                    Math.Max(0d, session.approvedBreakSeconds),
                idleUnverifiedSeconds =
                    Math.Max(0d, session.idleUnverifiedSeconds),
                commitCount = session.commitEntries?.Count ?? 0,
                breakCount = session.wellnessEvents?.Count(
                    value => value.action == "Break Started") ?? 0,
                mediaAttachmentCount =
                    session.mediaAttachments?.Count ?? 0,
                copperEarned = copper,
                experienceEarned = experience,
                suspiciousDuration =
                    DeverQuestSettingsStore.Profile
                        .suspiciousQuestMinutes > 0 &&
                    session.accumulatedFocusedSeconds >=
                    DeverQuestSettingsStore.Profile
                        .suspiciousQuestMinutes * 60d,
                suspiciousIdleRatio =
                    paused >= 15d * 60d && idleRatio >= 0.5d,
                publishedUtc = DateTime.UtcNow.ToString("O")
            };
        }

        private static void WriteAdventurerSnapshot(
            string accountId,
            DeverQuestSession session,
            DeverQuestAdventurer adventurer)
        {
            DeverQuestCompanionState activeCompanion =
                DeverQuestCompanionService.ActiveCompanion(
                    adventurer);
            DeverQuestSharedAdventurerSnapshot snapshot =
                new DeverQuestSharedAdventurerSnapshot
                {
                    accountId = accountId,
                    developerName = session.developerName,
                    adventurerName = adventurer.characterName,
                    characterClass = adventurer.characterClass,
                    classId = adventurer.classId,
                    ancestryName = adventurer.ancestryName,
                    ancestryId = adventurer.ancestryId,
                    deityName = adventurer.deityName,
                    deityId = adventurer.deityId,
                    alignment = adventurer.alignment.ToString(),
                    guildRank = adventurer.guildRank,
                    homeDepartment = adventurer.homeDepartment,
                    level = adventurer.level,
                    lifetimeExperience =
                        adventurer.lifetimeExperience,
                    copperBalance = adventurer.copperBalance,
                    activeCompanionName =
                        activeCompanion == null
                            ? string.Empty
                            : DeverQuestCompanionService
                                .DisplayName(activeCompanion),
                    activeCompanionLevel =
                        activeCompanion?.level ?? 0,
                    companionRosterCount =
                        adventurer.companions?.Count ?? 0,
                    updatedUtc = DateTime.UtcNow.ToString("O")
                };
            string path = Path.Combine(
                DeverQuestSettingsStore.Profile
                    .sharedGuildRepositoryPath,
                AdventurersFolder,
                DeverQuestPathUtility.MakeSafeFolderName(
                    accountId) +
                ".adventurer.json");
            WriteAtomic(
                path,
                JsonUtility.ToJson(snapshot, true));
        }

        private static void BuildHall(
            List<DeverQuestSharedQuestRecord> records,
            DeverQuestProfile profile)
        {
            foreach (IGrouping<string, DeverQuestSharedQuestRecord> group
                     in records.GroupBy(
                         value => value.accountId ??
                                  value.developerName))
            {
                DeverQuestSharedQuestRecord latest =
                    group.OrderByDescending(
                            value => value.completedUtc)
                        .First();
                DeverQuestHallEntry entry =
                    new DeverQuestHallEntry
                    {
                        AccountId = group.Key,
                        DeveloperName = latest.developerName,
                        AdventurerName = latest.adventurerName,
                        CharacterClass = latest.characterClass,
                        AncestryName = latest.ancestryName,
                        GuildRank = latest.guildRank,
                        Level = group.Max(value => value.level),
                        RawFocusedSeconds =
                            group.Sum(value => value.focusedSeconds),
                        ExperienceEarned =
                            group.Sum(value => value.experienceEarned),
                        CopperEarned =
                            group.Sum(value => value.copperEarned),
                        QuestCount = group.Count(),
                        ContractCount = group.Count(value =>
                            !string.IsNullOrWhiteSpace(
                                value.contractTitle)),
                        ReviewFlagCount = group.Count(value =>
                            value.suspiciousDuration ||
                            value.suspiciousIdleRatio)
                    };

                List<IGrouping<string, DeverQuestSharedQuestRecord>>
                    days = group.GroupBy(value => value.localDate)
                        .OrderBy(value => value.Key)
                        .ToList();
                foreach (IGrouping<string, DeverQuestSharedQuestRecord> day
                         in days)
                {
                    int count = day.Count();
                    double eligible = day.Where(value =>
                            !value.suspiciousDuration &&
                            !value.suspiciousIdleRatio)
                        .Sum(value => value.focusedSeconds);
                    if (profile.suspiciousDailyQuestCount > 0 &&
                        count >
                        profile.suspiciousDailyQuestCount)
                    {
                        entry.ReviewFlagCount++;
                    }
                    entry.RankedFocusedSeconds += Math.Min(
                        eligible,
                        profile.healthyDailyFocusMinutes * 60d);
                }
                entry.CurrentStreak =
                    CalculateCurrentStreak(
                        days,
                        profile.dailyWorkGoalMinutes * 60d);
                HallEntries.Add(entry);
            }

            HallEntries.Sort((left, right) =>
            {
                int focus = right.RankedFocusedSeconds.CompareTo(
                    left.RankedFocusedSeconds);
                return focus != 0
                    ? focus
                    : right.ExperienceEarned.CompareTo(
                        left.ExperienceEarned);
            });
        }

        private static int CalculateCurrentStreak(
            List<IGrouping<string, DeverQuestSharedQuestRecord>> days,
            double goalSeconds)
        {
            HashSet<DateTime> completed = new HashSet<DateTime>();
            foreach (IGrouping<string, DeverQuestSharedQuestRecord> day
                     in days)
            {
                if (DateTime.TryParseExact(
                        day.Key,
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.None,
                        out DateTime date) &&
                    day.Sum(value => value.focusedSeconds) >=
                    Math.Max(1d, goalSeconds))
                {
                    completed.Add(date.Date);
                }
            }

            int streak = 0;
            DateTime cursor = DateTime.Today;
            if (!completed.Contains(cursor))
            {
                cursor = cursor.AddDays(-1);
            }
            while (completed.Contains(cursor))
            {
                streak++;
                cursor = cursor.AddDays(-1);
            }
            return streak;
        }

        private static void BuildNamedReports(
            List<DeverQuestSharedQuestRecord> records,
            Func<DeverQuestSharedQuestRecord, string> selector,
            string fallback,
            List<DeverQuestGuildNamedReport> destination)
        {
            foreach (IGrouping<string, DeverQuestSharedQuestRecord> group
                     in records.GroupBy(value =>
                     {
                         string name = selector(value);
                         return string.IsNullOrWhiteSpace(name)
                             ? fallback
                             : name;
                     }))
            {
                destination.Add(
                    new DeverQuestGuildNamedReport
                    {
                        Name = group.Key,
                        FocusedSeconds =
                            group.Sum(value => value.focusedSeconds),
                        QuestCount = group.Count(),
                        AdventurerCount =
                            group.Select(value => value.accountId)
                                .Distinct()
                                .Count()
                    });
            }
            destination.Sort(
                (left, right) =>
                    right.FocusedSeconds.CompareTo(
                        left.FocusedSeconds));
        }

        private static string ComputeHash(
            DeverQuestSharedQuestRecord record)
        {
            List<string> fields = new List<string>
            {
                record.schemaVersion.ToString(
                    CultureInfo.InvariantCulture),
                record.sessionId ?? string.Empty,
                record.accountId ?? string.Empty,
                record.developerName ?? string.Empty,
                record.adventurerName ?? string.Empty,
                record.characterClass ?? string.Empty
            };
            if (record.schemaVersion >= 2)
            {
                fields.Add(record.classId ?? string.Empty);
                fields.Add(record.ancestryName ?? string.Empty);
                fields.Add(record.ancestryId ?? string.Empty);
                fields.Add(record.deityName ?? string.Empty);
                fields.Add(record.deityId ?? string.Empty);
                fields.Add(record.alignment ?? string.Empty);
            }
            fields.AddRange(new[]
            {
                record.guildRank ?? string.Empty,
                record.level.ToString(CultureInfo.InvariantCulture),
                record.projectName ?? string.Empty,
                record.department ?? string.Empty,
                record.taskName ?? string.Empty,
                record.contractTitle ?? string.Empty,
                record.localDate ?? string.Empty,
                record.startedUtc ?? string.Empty,
                record.completedUtc ?? string.Empty,
                record.focusedSeconds.ToString(
                    "R", CultureInfo.InvariantCulture),
                record.pausedSeconds.ToString(
                    "R", CultureInfo.InvariantCulture),
                record.approvedBreakSeconds.ToString(
                    "R", CultureInfo.InvariantCulture),
                record.idleUnverifiedSeconds.ToString(
                    "R", CultureInfo.InvariantCulture),
                record.commitCount.ToString(
                    CultureInfo.InvariantCulture),
                record.breakCount.ToString(
                    CultureInfo.InvariantCulture),
                record.mediaAttachmentCount.ToString(
                    CultureInfo.InvariantCulture),
                record.copperEarned.ToString(
                    CultureInfo.InvariantCulture),
                record.experienceEarned.ToString(
                    CultureInfo.InvariantCulture),
                record.suspiciousDuration ? "1" : "0",
                record.suspiciousIdleRatio ? "1" : "0",
                record.publishedUtc ?? string.Empty
            });
            string canonical = string.Join("\n", fields);
            using (SHA256 sha = SHA256.Create())
            {
                return BitConverter.ToString(
                        sha.ComputeHash(
                            Encoding.UTF8.GetBytes(canonical)))
                    .Replace("-", string.Empty)
                    .ToLowerInvariant();
            }
        }

        private static void WriteAtomic(
            string path,
            string content)
        {
            string directory =
                Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }
            string temporary =
                path + "." + Guid.NewGuid().ToString("N") + ".tmp";
            File.WriteAllText(
                temporary,
                content,
                new UTF8Encoding(false));
            if (File.Exists(path))
            {
                File.Replace(temporary, path, null);
            }
            else
            {
                File.Move(temporary, path);
            }
        }
    }
}
