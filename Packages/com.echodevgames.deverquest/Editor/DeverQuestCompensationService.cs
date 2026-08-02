using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestCompensationBasis
    {
        Hourly = 0,
        AnnualSalary = 1
    }

    internal enum DeverQuestCompensationIntegrityPolicy
    {
        VerifiedChroniclesOnly = 0,
        VerifiedAndLegacyChronicles = 1
    }

    internal sealed class DeverQuestCompensationPreview
    {
        public int IncludedChronicles;
        public int IncludedSessions;
        public int ExcludedChronicles;
        public double FocusedSeconds;
        public double ApprovedBreakSeconds;
        public double EligibleSeconds;
        public double ExcludedModifiedSeconds;
        public double ExcludedLegacySeconds;
        public double FlaggedSeconds;
        public double EffectiveHourlyRate;
        public double EstimatedGross;
    }

    internal static class DeverQuestCompensationService
    {
        public const string Disclaimer =
            "Planning estimate only. This is not payroll, a wage statement, " +
            "a promise of payment, tax advice, or authorization to pay. " +
            "A Guild administrator must review approved time and apply the " +
            "actual employment agreement and applicable law.";

        public static DeverQuestCompensationPreview BuildPreview(
            DeverQuestGuildAccount account,
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            DeverQuestCompensationPreview preview =
                new DeverQuestCompensationPreview();
            if (account == null || days == null)
            {
                return preview;
            }

            foreach (DeverQuestHistoryDay day in days)
            {
                if (day?.Record?.sessions == null)
                {
                    continue;
                }

                double dayFocused = day.Record.sessions.Sum(
                    session =>
                        Math.Max(
                            0d,
                            session?.accumulatedFocusedSeconds ?? 0d));
                double dayApprovedBreaks = day.Record.sessions.Sum(
                    session =>
                        Math.Max(
                            0d,
                            session?.approvedBreakSeconds ?? 0d));

                if (day.IntegrityStatus ==
                    DeverQuestIntegrityStatus.Modified ||
                    day.IntegrityStatus ==
                    DeverQuestIntegrityStatus.Unavailable)
                {
                    preview.ExcludedChronicles++;
                    preview.ExcludedModifiedSeconds +=
                        dayFocused +
                        (account.compensationIncludeApprovedBreaks
                            ? dayApprovedBreaks
                            : 0d);
                    continue;
                }

                if (day.IntegrityStatus ==
                    DeverQuestIntegrityStatus.Legacy &&
                    account.compensationIntegrityPolicy ==
                    DeverQuestCompensationIntegrityPolicy
                        .VerifiedChroniclesOnly)
                {
                    preview.ExcludedChronicles++;
                    preview.ExcludedLegacySeconds +=
                        dayFocused +
                        (account.compensationIncludeApprovedBreaks
                            ? dayApprovedBreaks
                            : 0d);
                    continue;
                }

                preview.IncludedChronicles++;
                preview.IncludedSessions += day.Record.sessions.Count;
                preview.FocusedSeconds += dayFocused;
                if (account.compensationIncludeApprovedBreaks)
                {
                    preview.ApprovedBreakSeconds += dayApprovedBreaks;
                }

                if (day.SuspiciousFrequency)
                {
                    preview.FlaggedSeconds += dayFocused;
                }
                else if (profile != null &&
                         profile.suspiciousQuestMinutes > 0)
                {
                    double threshold =
                        profile.suspiciousQuestMinutes * 60d;
                    preview.FlaggedSeconds +=
                        day.Record.sessions
                            .Where(
                                session =>
                                    session != null &&
                                    session.accumulatedFocusedSeconds >=
                                    threshold)
                            .Sum(
                                session =>
                                    session.accumulatedFocusedSeconds);
                }
            }

            preview.EligibleSeconds =
                preview.FocusedSeconds +
                preview.ApprovedBreakSeconds;
            preview.EffectiveHourlyRate =
                EffectiveHourlyRate(account);
            preview.EstimatedGross =
                preview.EligibleSeconds / 3600d *
                preview.EffectiveHourlyRate;
            return preview;
        }

        public static double EffectiveHourlyRate(
            DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return 0d;
            }
            if (account.compensationBasis ==
                DeverQuestCompensationBasis.Hourly)
            {
                return Math.Max(0d, account.compensationHourlyRate);
            }

            double weeklyHours =
                Math.Max(0.01d, account.compensationWeeklyHours);
            return Math.Max(0d, account.compensationAnnualSalary) /
                   (52d * weeklyHours);
        }

        public static string FormatMoney(
            DeverQuestGuildAccount account,
            double amount)
        {
            string currency =
                NormalizeCurrencyCode(account?.compensationCurrencyCode);
            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1:N2}",
                currency,
                Math.Max(0d, amount));
        }

        public static string DescribeBasis(
            DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return "Not configured";
            }
            if (account.compensationBasis ==
                DeverQuestCompensationBasis.Hourly)
            {
                return $"{FormatMoney(account, account.compensationHourlyRate)} " +
                       "per eligible hour";
            }
            return $"{FormatMoney(account, account.compensationAnnualSalary)} " +
                   $"annual tracking equivalent at " +
                   $"{Math.Max(0.01d, account.compensationWeeklyHours):0.##} " +
                   "scheduled hours/week";
        }

        public static string NormalizeCurrencyCode(string value)
        {
            value = value?.Trim().ToUpperInvariant() ?? string.Empty;
            if (value.Length != 3 ||
                value.Any(character => !char.IsLetter(character)))
            {
                return "USD";
            }
            return value;
        }

        public static bool TryExportPreview(
            string path,
            DeverQuestGuildAccount account,
            DeverQuestCompensationPreview preview,
            string rangeDescription,
            out string error)
        {
            error = string.Empty;
            if (account == null || preview == null)
            {
                error = "Compensation Preview data was unavailable.";
                return false;
            }
            try
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine(
                    "DeverQuest Compensation Preview (Planning Estimate)");
                AppendCsv(builder, "Developer", account.developerName);
                AppendCsv(builder, "Adventurer", account.characterName);
                AppendCsv(
                    builder,
                    "Range",
                    rangeDescription ?? "Filtered history");
                AppendCsv(
                    builder,
                    "Eligible Focused Hours",
                    (preview.FocusedSeconds / 3600d)
                        .ToString("0.0000", CultureInfo.InvariantCulture));
                AppendCsv(
                    builder,
                    "Eligible Approved Break Hours",
                    (preview.ApprovedBreakSeconds / 3600d)
                        .ToString("0.0000", CultureInfo.InvariantCulture));
                AppendCsv(
                    builder,
                    "Total Eligible Hours",
                    (preview.EligibleSeconds / 3600d)
                        .ToString("0.0000", CultureInfo.InvariantCulture));
                AppendCsv(
                    builder,
                    "Compensation Basis",
                    DescribeBasis(account));
                AppendCsv(
                    builder,
                    "Effective Hourly Equivalent",
                    FormatMoney(account, preview.EffectiveHourlyRate));
                AppendCsv(
                    builder,
                    "Estimated Gross Equivalent",
                    FormatMoney(account, preview.EstimatedGross));
                AppendCsv(
                    builder,
                    "Included Chronicles",
                    preview.IncludedChronicles.ToString(
                        CultureInfo.InvariantCulture));
                AppendCsv(
                    builder,
                    "Excluded Chronicles",
                    preview.ExcludedChronicles.ToString(
                        CultureInfo.InvariantCulture));
                AppendCsv(
                    builder,
                    "Flagged Hours Requiring Review",
                    (preview.FlaggedSeconds / 3600d)
                        .ToString("0.0000", CultureInfo.InvariantCulture));
                AppendCsv(builder, "Disclaimer", Disclaimer);

                string directory = Path.GetDirectoryName(path);
                if (!string.IsNullOrWhiteSpace(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                File.WriteAllText(path, builder.ToString());
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }

        private static void AppendCsv(
            StringBuilder builder,
            string name,
            string value)
        {
            builder.Append(EscapeCsv(name));
            builder.Append(',');
            builder.AppendLine(EscapeCsv(value));
        }

        private static string EscapeCsv(string value)
        {
            value = value ?? string.Empty;
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }
    }
}
