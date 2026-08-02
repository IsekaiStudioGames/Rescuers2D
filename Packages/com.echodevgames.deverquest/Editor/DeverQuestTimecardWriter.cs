//----- DeverQuestTimecardWriter.cs START -----

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [Serializable]
    internal sealed class DeverQuestDailyRecord
    {
        public string developerName = string.Empty;
        public string localDate = string.Empty;
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

                string baseFileName =
                    $"{dateKey}_{safeDeveloperName}_Timecard";

                markdownPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".md");

                string dataPath = Path.Combine(
                    developerFolder,
                    baseFileName + ".deverquest.json");

                DeverQuestDailyRecord record =
                    LoadRecord(
                        dataPath,
                        profile.developerName,
                        dateKey);

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
                    completedLocal.Date);

                File.WriteAllText(
                    markdownPath,
                    markdown,
                    new UTF8Encoding(false));

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
                sessions = new List<DeverQuestSession>()
            };
        }

        private static string BuildMarkdown(
            DeverQuestDailyRecord record,
            DateTime localDate)
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
            builder.AppendLine($"**Date:** {localDate:MMMM d, yyyy}  ");
            builder.AppendLine($"**Sessions:** {record.sessions.Count}");
            builder.AppendLine();
            builder.AppendLine("## Daily Totals");
            builder.AppendLine();
            builder.AppendLine($"- **Focused Work:** {FormatDuration(totalFocused)}");
            builder.AppendLine($"- **Paused Time:** {FormatDuration(totalPaused)}");
            builder.AppendLine($"- **Commit Entries:** {totalCommits}");
            builder.AppendLine($"- **Breaks Taken:** {totalBreaks}");
            builder.AppendLine(
                $"- **Coin Earned:** {DeverQuestAdventurerService.FormatCoins(totalCopper)}");
            builder.AppendLine(
                $"- **Experience Earned:** {totalExperience} XP");
            builder.AppendLine(
                $"- **Ending Coin Purse:** " +
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
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
                AppendSession(builder, record.sessions[index], index + 1);
            }

            builder.AppendLine();
            builder.AppendLine(
                "_Generated by DeverQuest Developer Companion._");

            return builder.ToString();
        }

        private static void AppendSession(
            StringBuilder builder,
            DeverQuestSession session,
            int sessionNumber)
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
            builder.AppendLine($"- **Started:** {start:h:mm tt}");
            builder.AppendLine($"- **Ended:** {completed:h:mm tt}");
            builder.AppendLine(
                $"- **Focused Work:** {FormatDuration(session.accumulatedFocusedSeconds)}");
            builder.AppendLine(
                $"- **Paused Time:** {FormatDuration(session.accumulatedPausedSeconds)}");

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                builder.AppendLine();
                builder.AppendLine("### Goal");
                builder.AppendLine();
                builder.AppendLine(EscapeMultiline(session.goal));
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
                    DateTime created = new DateTime(
                            entry.createdUtcTicks,
                            DateTimeKind.Utc)
                        .ToLocalTime();

                    builder.Append(
                        $"- **{created:h:mm tt}** " +
                        $"[{Escape(entry.entryType)}] " +
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
    }
}

//----- DeverQuestTimecardWriter.cs END -----
