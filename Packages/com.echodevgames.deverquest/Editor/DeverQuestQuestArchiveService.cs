//----- DeverQuestQuestArchiveService.cs START -----

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestQuestArchiveFilter
    {
        All = 0,
        ContractRuns = 1,
        WithRewards = 2,
        WithCommits = 3,
        WithMedia = 4,
        WithCombat = 5
    }

    internal sealed class DeverQuestQuestArchiveRecord
    {
        public DateTime Date;
        public string DataPath = string.Empty;
        public string MarkdownPath = string.Empty;
        public DeverQuestIntegrityStatus IntegrityStatus;
        public string IntegrityMessage = string.Empty;
        public DeverQuestSession Session;
    }

    internal sealed class DeverQuestQuestArchiveSummary
    {
        public int QuestCount;
        public double FocusedSeconds;
        public double PausedSeconds;
        public long CopperEarned;
        public long ExperienceEarned;
        public int CommitCount;
        public int MediaCount;
        public int BattleCount;
    }

    internal sealed class DeverQuestQuestEvent
    {
        public long UtcTicks;
        public string Category = string.Empty;
        public string Title = string.Empty;
        public string Detail = string.Empty;
    }

    internal static class DeverQuestQuestArchiveService
    {
        public static List<DeverQuestQuestArchiveRecord> BuildRecords(
            IReadOnlyList<DeverQuestHistoryDay> days,
            string search,
            DeverQuestQuestArchiveFilter filter)
        {
            string query = search?.Trim() ?? string.Empty;
            List<DeverQuestQuestArchiveRecord> records =
                new List<DeverQuestQuestArchiveRecord>();

            if (days == null)
            {
                return records;
            }

            foreach (DeverQuestHistoryDay day in days)
            {
                if (day?.Record?.sessions == null)
                {
                    continue;
                }

                foreach (DeverQuestSession session in day.Record.sessions)
                {
                    if (session == null || !MatchesFilter(session, filter) ||
                        !MatchesSearch(session, query))
                    {
                        continue;
                    }

                    records.Add(
                        new DeverQuestQuestArchiveRecord
                        {
                            Date = day.Date,
                            DataPath = day.DataPath,
                            MarkdownPath = day.MarkdownPath,
                            IntegrityStatus = day.IntegrityStatus,
                            IntegrityMessage = day.IntegrityMessage,
                            Session = session
                        });
                }
            }

            records.Sort(
                (left, right) =>
                    SortTicks(right.Session)
                        .CompareTo(SortTicks(left.Session)));
            return records;
        }

        public static DeverQuestQuestArchiveSummary BuildSummary(
            IReadOnlyList<DeverQuestQuestArchiveRecord> records)
        {
            DeverQuestQuestArchiveSummary summary =
                new DeverQuestQuestArchiveSummary();
            if (records == null)
            {
                return summary;
            }

            foreach (DeverQuestQuestArchiveRecord record in records)
            {
                DeverQuestSession session = record?.Session;
                if (session == null)
                {
                    continue;
                }

                summary.QuestCount++;
                summary.FocusedSeconds +=
                    Math.Max(0d, session.accumulatedFocusedSeconds);
                summary.PausedSeconds +=
                    Math.Max(0d, session.accumulatedPausedSeconds);
                summary.CommitCount += session.commitEntries?.Count ?? 0;
                summary.MediaCount += session.mediaAttachments?.Count ?? 0;
                summary.BattleCount += session.battleResults?.Count ?? 0;

                if (session.rewardTransactions == null)
                {
                    continue;
                }

                foreach (DeverQuestRewardTransaction transaction
                         in session.rewardTransactions)
                {
                    if (transaction == null)
                    {
                        continue;
                    }
                    summary.CopperEarned +=
                        Math.Max(0L, transaction.copper);
                    summary.ExperienceEarned +=
                        Math.Max(0L, transaction.experience);
                }
            }

            return summary;
        }

        public static List<DeverQuestQuestEvent> BuildTimeline(
            DeverQuestSession session,
            bool newestFirst = true)
        {
            List<DeverQuestQuestEvent> events =
                new List<DeverQuestQuestEvent>();
            if (session == null)
            {
                return events;
            }

            Add(
                events,
                session.startedUtcTicks,
                "Quest",
                "Quest Started",
                string.IsNullOrWhiteSpace(session.taskName)
                    ? "The Quest began."
                    : session.taskName);

            if (session.questStages != null)
            {
                int stageIndex = 0;
                foreach (DeverQuestSessionStage stage in session.questStages)
                {
                    stageIndex++;
                    if (stage == null || !stage.completed)
                    {
                        continue;
                    }
                    string stageTitle = string.IsNullOrWhiteSpace(
                        stage.stageTitle)
                            ? "Encounter " + stageIndex
                            : stage.stageTitle;
                    Add(
                        events,
                        stage.completedUtcTicks,
                        "Encounter",
                        stageTitle + " Completed",
                        stage.completedEarly
                            ? "Completed ahead of the predicted Encounter pace."
                            : "Encounter objective completed.");
                }
            }

            if (session.battleResults != null)
            {
                foreach (DeverQuestBattleResult battle
                         in session.battleResults)
                {
                    if (battle == null)
                    {
                        continue;
                    }
                    Add(
                        events,
                        battle.resolvedUtcTicks,
                        "Combat",
                        DeverQuestCombatSummaryService.OutcomeTitle(battle),
                        string.IsNullOrWhiteSpace(battle.encounterName)
                            ? DeverQuestCombatSummaryService
                                .OutcomeSummary(battle)
                            : battle.encounterName + ": " +
                              DeverQuestCombatSummaryService
                                  .OutcomeSummary(battle));
                }
            }

            if (session.commitEntries != null)
            {
                foreach (DeverQuestCommitEntry entry
                         in session.commitEntries)
                {
                    if (entry == null)
                    {
                        continue;
                    }
                    string detail = entry.comment ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(entry.commitHash))
                    {
                        detail += " · " + entry.commitHash;
                    }
                    Add(
                        events,
                        entry.createdUtcTicks,
                        "Quest Log",
                        string.IsNullOrWhiteSpace(entry.entryType)
                            ? "Quest Log Entry"
                            : entry.entryType,
                        detail);
                }
            }

            if (session.mediaAttachments != null)
            {
                foreach (DeverQuestMediaAttachment attachment
                         in session.mediaAttachments)
                {
                    if (attachment == null)
                    {
                        continue;
                    }
                    Add(
                        events,
                        attachment.createdUtcTicks,
                        "Media",
                        string.IsNullOrWhiteSpace(attachment.attachmentType)
                            ? "Media Attached"
                            : attachment.attachmentType + " Attached",
                        attachment.displayName);
                }
            }

            if (session.wellnessEvents != null)
            {
                foreach (DeverQuestWellnessEvent wellness
                         in session.wellnessEvents)
                {
                    if (wellness == null)
                    {
                        continue;
                    }
                    Add(
                        events,
                        wellness.createdUtcTicks,
                        "Wellness",
                        wellness.type.ToString(),
                        wellness.action);
                }
            }

            if (session.externalActivityEvents != null)
            {
                foreach (DeverQuestExternalActivityEvent activity
                         in session.externalActivityEvents)
                {
                    if (activity == null)
                    {
                        continue;
                    }
                    Add(
                        events,
                        activity.createdUtcTicks,
                        "External Craft",
                        string.IsNullOrWhiteSpace(activity.toolName)
                            ? "External Activity"
                            : activity.toolName,
                        activity.action);
                }
            }

            if (session.rewardTransactions != null)
            {
                foreach (DeverQuestRewardTransaction reward
                         in session.rewardTransactions)
                {
                    if (reward == null)
                    {
                        continue;
                    }
                    string detail = RewardLine(reward);
                    Add(
                        events,
                        reward.createdUtcTicks,
                        "Rewards",
                        string.IsNullOrWhiteSpace(reward.transactionType)
                            ? "Reward Updated"
                            : reward.transactionType,
                        detail);
                }
            }

            Add(
                events,
                session.completedUtcTicks,
                "Quest",
                "Quest Completed",
                BuildOutcomeLine(session));

            events = events
                .Where(value => value.UtcTicks > 0L)
                .OrderBy(value => value.UtcTicks)
                .ThenBy(value => value.Category)
                .ToList();
            if (newestFirst)
            {
                events.Reverse();
            }
            return events;
        }

        public static string BuildReadableSummary(
            DeverQuestSession session)
        {
            if (session == null)
            {
                return "No Quest Session was available.";
            }

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                string.IsNullOrWhiteSpace(session.taskName)
                    ? "Quest"
                    : session.taskName);
            builder.AppendLine(
                "Project: " +
                (string.IsNullOrWhiteSpace(session.projectName)
                    ? "Unspecified"
                    : session.projectName));
            builder.AppendLine("Status: " + StatusLabel(session));
            builder.AppendLine(
                "Focused: " + FormatDuration(
                    session.accumulatedFocusedSeconds));
            builder.AppendLine(
                "Paused: " + FormatDuration(
                    session.accumulatedPausedSeconds));
            if (!string.IsNullOrWhiteSpace(session.questContractTitle))
            {
                builder.AppendLine(
                    "Contract: " + session.questContractTitle);
            }
            if (!string.IsNullOrWhiteSpace(session.questContractRunId))
            {
                builder.AppendLine(
                    "Quest Run: " + session.questContractRunId);
            }
            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                builder.AppendLine("Objective: " + session.goal);
            }
            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                builder.AppendLine("Story: " + session.questStory);
            }
            builder.AppendLine("Rewards: " + RewardSummary(session));
            builder.AppendLine(
                "Commits/Notes: " +
                (session.commitEntries?.Count ?? 0));
            builder.AppendLine(
                "Media: " +
                (session.mediaAttachments?.Count ?? 0));
            builder.AppendLine(
                "Battles: " +
                (session.battleResults?.Count ?? 0));
            if (!string.IsNullOrWhiteSpace(session.closingNotes))
            {
                builder.AppendLine("Closing Notes: " + session.closingNotes);
            }
            return builder.ToString().TrimEnd();
        }

        public static string RewardSummary(DeverQuestSession session)
        {
            if (session?.rewardTransactions == null ||
                session.rewardTransactions.Count == 0)
            {
                return "No recorded rewards";
            }

            long copper = 0L;
            long experience = 0L;
            double minutes = 0d;
            foreach (DeverQuestRewardTransaction reward
                     in session.rewardTransactions)
            {
                if (reward == null)
                {
                    continue;
                }
                copper += reward.copper;
                experience += reward.experience;
                minutes += reward.minutes;
            }

            List<string> parts = new List<string>();
            if (copper != 0L)
            {
                parts.Add(FormatSignedCoins(copper));
            }
            if (experience != 0L)
            {
                parts.Add(experience + " XP");
            }
            if (Math.Abs(minutes) > 0.001d)
            {
                parts.Add(minutes.ToString("0.#") + " reward minutes");
            }
            return parts.Count == 0
                ? "No numeric reward"
                : string.Join(" · ", parts);
        }

        public static string StatusLabel(DeverQuestSession session)
        {
            if (session == null)
            {
                return "Unavailable";
            }
            switch (session.state)
            {
                case DeverQuestSessionState.Running:
                    return "In Progress";
                case DeverQuestSessionState.Paused:
                    return "Paused";
                case DeverQuestSessionState.Completed:
                    return "Completed";
                default:
                    return "Recorded";
            }
        }

        public static DateTime LocalEventTime(long utcTicks)
        {
            if (utcTicks <= 0L)
            {
                return DateTime.MinValue;
            }
            try
            {
                return new DateTime(
                        utcTicks,
                        DateTimeKind.Utc)
                    .ToLocalTime();
            }
            catch
            {
                return DateTime.MinValue;
            }
        }

        private static bool MatchesFilter(
            DeverQuestSession session,
            DeverQuestQuestArchiveFilter filter)
        {
            switch (filter)
            {
                case DeverQuestQuestArchiveFilter.ContractRuns:
                    return session.usesQuestContract;
                case DeverQuestQuestArchiveFilter.WithRewards:
                    return session.rewardTransactions != null &&
                           session.rewardTransactions.Count > 0;
                case DeverQuestQuestArchiveFilter.WithCommits:
                    return session.commitEntries != null &&
                           session.commitEntries.Count > 0;
                case DeverQuestQuestArchiveFilter.WithMedia:
                    return session.mediaAttachments != null &&
                           session.mediaAttachments.Count > 0;
                case DeverQuestQuestArchiveFilter.WithCombat:
                    return session.battleResults != null &&
                           session.battleResults.Count > 0;
                default:
                    return true;
            }
        }

        private static bool MatchesSearch(
            DeverQuestSession session,
            string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return true;
            }

            IEnumerable<string> values = new[]
            {
                session.sessionId,
                session.developerName,
                session.projectName,
                session.taskName,
                session.category,
                session.goal,
                session.questProfileName,
                session.questContractTitle,
                session.questContractRunId,
                session.questStory,
                session.closingNotes
            };
            if (values.Any(value => Contains(value, query)))
            {
                return true;
            }

            return (session.commitEntries ??
                    new List<DeverQuestCommitEntry>())
                       .Any(entry => entry != null &&
                                     (Contains(entry.comment, query) ||
                                      Contains(entry.commitHash, query))) ||
                   (session.mediaAttachments ??
                    new List<DeverQuestMediaAttachment>())
                       .Any(attachment => attachment != null &&
                           Contains(attachment.displayName, query)) ||
                   (session.battleResults ??
                    new List<DeverQuestBattleResult>())
                       .Any(battle => battle != null &&
                           (Contains(battle.encounterName, query) ||
                            Contains(battle.seed, query)));
        }

        private static bool Contains(string value, string query)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.IndexOf(
                       query,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static long SortTicks(DeverQuestSession session)
        {
            if (session == null)
            {
                return 0L;
            }
            return session.completedUtcTicks > 0L
                ? session.completedUtcTicks
                : session.startedUtcTicks;
        }

        private static void Add(
            ICollection<DeverQuestQuestEvent> events,
            long utcTicks,
            string category,
            string title,
            string detail)
        {
            if (utcTicks <= 0L)
            {
                return;
            }
            events.Add(
                new DeverQuestQuestEvent
                {
                    UtcTicks = utcTicks,
                    Category = category ?? string.Empty,
                    Title = title ?? string.Empty,
                    Detail = detail ?? string.Empty
                });
        }

        private static string RewardLine(
            DeverQuestRewardTransaction reward)
        {
            List<string> parts = new List<string>();
            if (reward.copper != 0L)
            {
                parts.Add(
                    (reward.copper > 0L ? "+" : string.Empty) +
                    FormatSignedCoins(reward.copper));
            }
            if (reward.experience != 0L)
            {
                parts.Add(
                    (reward.experience > 0L ? "+" : string.Empty) +
                    reward.experience + " XP");
            }
            if (Math.Abs(reward.minutes) > 0.001d)
            {
                parts.Add(
                    (reward.minutes > 0d ? "+" : string.Empty) +
                    reward.minutes.ToString("0.#") + " minutes");
            }
            if (!string.IsNullOrWhiteSpace(reward.note))
            {
                parts.Add(reward.note);
            }
            return parts.Count == 0
                ? "Reward state updated."
                : string.Join(" · ", parts);
        }

        private static string FormatSignedCoins(long copper)
        {
            if (copper < 0L)
            {
                return "-" +
                       DeverQuestAdventurerService.FormatCoins(-copper);
            }
            return DeverQuestAdventurerService.FormatCoins(copper);
        }

        private static string BuildOutcomeLine(
            DeverQuestSession session)
        {
            string rewards = RewardSummary(session);
            if (string.Equals(
                    rewards,
                    "No recorded rewards",
                    StringComparison.Ordinal))
            {
                return "The Quest was recorded without a reward transaction.";
            }
            return rewards;
        }

        private static string FormatDuration(double totalSeconds)
        {
            TimeSpan duration = TimeSpan.FromSeconds(
                Math.Max(0d, totalSeconds));
            return ((int)duration.TotalHours).ToString("00") + ":" +
                   duration.Minutes.ToString("00") + ":" +
                   duration.Seconds.ToString("00");
        }
    }
}

//----- DeverQuestQuestArchiveService.cs END -----
