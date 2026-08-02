//----- DeverQuestReleaseReadinessService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestReadinessSeverity
    {
        Pass = 0,
        Advisory = 1,
        Blocker = 2
    }

    internal sealed class DeverQuestReadinessFinding
    {
        public DeverQuestReadinessSeverity severity;
        public string title = string.Empty;
        public string detail = string.Empty;
    }

    internal sealed class DeverQuestReadinessReport
    {
        public readonly List<DeverQuestReadinessFinding> findings =
            new List<DeverQuestReadinessFinding>();

        public int PassCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Pass);

        public int AdvisoryCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Advisory);

        public int BlockerCount => findings.Count(
            finding =>
                finding.severity == DeverQuestReadinessSeverity.Blocker);

        public bool ReadyForRegression => BlockerCount == 0;

        public string Summary =>
            $"{PassCount} passed, {AdvisoryCount} advisories, " +
            $"{BlockerCount} blockers.";

        public string ToConsoleText()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                "[DeverQuest 0.32.5 Beta 1] Release Readiness Report");
            builder.AppendLine(Summary);

            foreach (DeverQuestReadinessFinding finding in findings)
            {
                builder.Append('[');
                builder.Append(finding.severity.ToString().ToUpperInvariant());
                builder.Append("] ");
                builder.AppendLine(finding.title);
                builder.AppendLine("  " + finding.detail);
            }

            return builder.ToString().TrimEnd();
        }
    }

    internal static class DeverQuestReleaseReadinessService
    {
        private const string ExpectedPackageVersion = "0.32.5";

        [MenuItem(
            "Tools/DeverQuest/Run Release Readiness Check",
            false,
            40)]
        private static void RunFromMenu()
        {
            DeverQuestReadinessReport report = Run();
            string text = report.ToConsoleText();

            if (report.BlockerCount > 0)
            {
                Debug.LogError(text);
            }
            else if (report.AdvisoryCount > 0)
            {
                Debug.LogWarning(text);
            }
            else
            {
                Debug.Log(text);
            }

            EditorUtility.DisplayDialog(
                report.ReadyForRegression
                    ? "DeverQuest Ready for Regression"
                    : "DeverQuest Release Blocked",
                report.Summary +
                "\n\nThe full report was written to the Console.",
                "Close");
        }

        public static DeverQuestReadinessReport Run()
        {
            DeverQuestReadinessReport report =
                new DeverQuestReadinessReport();

            CheckPackageVersion(report);
            CheckUnityVersion(report);
            CheckRepositoryHygiene(report);
            CheckProfile(report);
            CheckEditorWorkspaceConfiguration(report);
            CheckGuildAuthority(report);
            CheckTimecardStorage(report);
            CheckTimecardGitHygiene(report);
            CheckChroniclePolicy(report);
            CheckQuestChronicleArchive(report);
            CheckSharedGuild(report);
            CheckAudioTransport(report);
            CheckWellnessCommandCenter(report);
            CheckIdentityCatalog(report);
            CheckTacticalTestContent(report);
            CheckTacticalArchive(report);
            CheckSpoilsSnapshots(report);
            CheckQuestRunReservations(report);
            CheckInventoryHealth(report);
            CheckGuildEconomy(report);
            CheckContentHealth(report);
            CheckSessionState(report);

            return report;
        }

        private static void CheckPackageVersion(
            DeverQuestReadinessReport report)
        {
            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                    typeof(DeverQuestReleaseReadinessService).Assembly);

            if (package == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Package version",
                    "Unity could not resolve Package Manager metadata for " +
                    "the loaded DeverQuest assembly.");
                return;
            }

            bool correctVersion = string.Equals(
                package.version,
                ExpectedPackageVersion,
                StringComparison.Ordinal);

            Add(
                report,
                correctVersion
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Package version",
                correctVersion
                    ? $"Package Manager reports {ExpectedPackageVersion}."
                    : $"Package Manager reports {package.version}; expected " +
                      $"{ExpectedPackageVersion}.");
        }

        private static void CheckUnityVersion(
            DeverQuestReadinessReport report)
        {
            bool supported = TryReadUnityVersion(
                Application.unityVersion,
                out int major,
                out int minor) &&
                (major > 2022 ||
                 (major == 2022 && minor >= 3));

            Add(
                report,
                supported
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Unity editor version",
                supported
                    ? $"Unity {Application.unityVersion} meets the 2022.3 " +
                      "minimum."
                    : $"Unity {Application.unityVersion} is below or could " +
                      "not be validated against the 2022.3 minimum.");
        }

        private static void CheckRepositoryHygiene(
            DeverQuestReadinessReport report)
        {
            DirectoryInfo projectDirectory =
                Directory.GetParent(Application.dataPath);
            if (projectDirectory == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Repository hygiene",
                    "Unity could not resolve the project root directory.");
                return;
            }

            string rootPath = projectDirectory.FullName;
            string normalizedPath = rootPath
                .Replace("_", " ")
                .Replace("-", " ")
                .ToLowerInvariant();
            string prohibitedLegacyName = new string(
                new[]
                {
                    'c', 'o', 'd', 'e', ' ',
                    'n', 'a', 'z', 'i'
                });
            bool legacyNameFound =
                normalizedPath.Contains(prohibitedLegacyName) ||
                normalizedPath.Contains(
                    prohibitedLegacyName.Replace(" ", string.Empty));
            Add(
                report,
                legacyNameFound
                    ? DeverQuestReadinessSeverity.Blocker
                    : DeverQuestReadinessSeverity.Pass,
                "Legacy repository naming",
                legacyNameFound
                    ? "The Unity project path still contains a prohibited " +
                      "legacy name. Rename the folder before Beta distribution."
                    : "No prohibited legacy name was found in the Unity " +
                      "project path.");

            string[] expectedFiles =
            {
                "README.md",
                "CREDITS.md",
                "THIRD_PARTY_NOTICES.md"
            };
            List<string> missingFiles = expectedFiles
                .Where(file => !File.Exists(Path.Combine(rootPath, file)))
                .ToList();
            bool documentationExists =
                Directory.Exists(Path.Combine(rootPath, "Documentation"));
            if (!documentationExists)
            {
                missingFiles.Add("Documentation/");
            }

            Add(
                report,
                missingFiles.Count == 0
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Repository documentation",
                missingFiles.Count == 0
                    ? "README, credits, third-party notices, and the " +
                      "Documentation folder are present at the project root."
                    : "Add these release files to the project root: " +
                      string.Join(", ", missingFiles) + ".");
        }

        private static void CheckProfile(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            bool configured =
                profile != null &&
                profile.setupComplete &&
                !string.IsNullOrWhiteSpace(profile.developerName);

            Add(
                report,
                configured
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Developer profile",
                configured
                    ? $"Profile is configured for {profile.developerName}."
                    : "Complete DeverQuest setup and provide a developer " +
                      "name before release regression.");
        }

        private static void CheckEditorWorkspaceConfiguration(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (profile == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Editor workspace configuration",
                    "The local DeverQuest presentation profile could not " +
                    "be loaded.");
                return;
            }

            bool valid =
                profile.interfaceScale >= 0.85f &&
                profile.interfaceScale <= 1.35f &&
                profile.workspaceTabColumns >= 2 &&
                profile.workspaceTabColumns <= 6;

            Add(
                report,
                valid
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Editor workspace configuration",
                valid
                    ? $"{profile.theme} theme · " +
                      $"{profile.workspaceTabColumns} workspace columns · " +
                      $"Quest HUD auto-open " +
                      $"{(profile.autoOpenQuestHudOnSessionStart ? "enabled" : "disabled")}."
                    : "Open the Visuals workspace and restore valid text " +
                      "scale and workspace-column settings.");
        }

        private static void CheckGuildAuthority(
            DeverQuestReadinessReport report)
        {
            List<DeverQuestGuildAccount> activeAccounts =
                DeverQuestGuildAccountService.Accounts
                    .Where(account => account != null && !account.disabled)
                    .ToList();
            DeverQuestGuildAccount current =
                DeverQuestGuildAccountService.CurrentAccount;

            if (current == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Guild authority",
                    "No active Guild account is selected.");
                return;
            }

            bool soleFounderInvalid =
                activeAccounts.Count == 1 &&
                !string.Equals(
                    current.guildRank,
                    "CEO",
                    StringComparison.OrdinalIgnoreCase);

            Add(
                report,
                soleFounderInvalid
                    ? DeverQuestReadinessSeverity.Blocker
                    : DeverQuestReadinessSeverity.Pass,
                "Guild authority",
                soleFounderInvalid
                    ? "The only active Guild account is not CEO. Reopen " +
                      "Unity so the sole-founder repair can run."
                    : $"{current.developerName} is authenticated as " +
                      $"{current.guildRank}.");
        }

        private static void CheckTimecardStorage(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (profile == null ||
                string.IsNullOrWhiteSpace(profile.timecardRootPath) ||
                string.IsNullOrWhiteSpace(profile.developerName))
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The timecard root or developer name is not configured.");
                return;
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            if (!DeverQuestPathUtility.TryCreateDirectory(
                    developerFolder,
                    out string createError))
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The developer folder could not be created: " +
                    createError);
                return;
            }

            string probePath = Path.Combine(
                developerFolder,
                ".deverquest_write_probe");

            try
            {
                File.WriteAllText(
                    probePath,
                    DateTime.UtcNow.ToString("O"));
                File.Delete(probePath);

                Add(
                    report,
                    DeverQuestReadinessSeverity.Pass,
                    "Timecard storage",
                    "The developer folder exists and accepted a write/delete " +
                    "probe.");
            }
            catch (Exception exception)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Timecard storage",
                    "The developer folder is not writable: " +
                    exception.Message);
            }
        }

        private static void CheckTimecardGitHygiene(
            DeverQuestReadinessReport report)
        {
            try
            {
                DeverQuestProfile profile =
                    DeverQuestSettingsStore.Profile;
                if (profile == null ||
                    string.IsNullOrWhiteSpace(profile.timecardRootPath))
                {
                    Add(
                        report,
                        DeverQuestReadinessSeverity.Advisory,
                        "Timecard Git hygiene",
                        "The timecard root is not configured, so repository " +
                        "exclusion could not be verified.");
                    return;
                }

                string projectRoot = Path.GetFullPath(
                    Path.Combine(Application.dataPath, ".."));
                string timecardRoot = Path.GetFullPath(
                    profile.timecardRootPath);
                string projectPrefix = projectRoot.TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar) +
                    Path.DirectorySeparatorChar;

                bool insideRepository = timecardRoot.StartsWith(
                    projectPrefix,
                    StringComparison.OrdinalIgnoreCase);
                if (!insideRepository)
                {
                    Add(
                        report,
                        DeverQuestReadinessSeverity.Pass,
                        "Timecard Git hygiene",
                        "The timecard root is outside the Unity project and " +
                        "will not be included by the project repository by " +
                        "default.");
                    return;
                }

                string relative = timecardRoot
                    .Substring(projectPrefix.Length)
                    .Replace('\\', '/');
                string gitIgnorePath = Path.Combine(
                    projectRoot, ".gitignore");
                bool ignored = false;
                if (File.Exists(gitIgnorePath))
                {
                    string[] candidates =
                    {
                        relative,
                        relative + "/",
                        "/" + relative,
                        "/" + relative + "/"
                    };
                    ignored = File.ReadAllLines(gitIgnorePath)
                        .Select(line => line.Trim())
                        .Where(line =>
                            !string.IsNullOrWhiteSpace(line) &&
                            !line.StartsWith(
                                "#",
                                StringComparison.Ordinal))
                        .Any(line => candidates.Any(candidate =>
                            string.Equals(
                                line,
                                candidate,
                                StringComparison.OrdinalIgnoreCase)));
                }

                Add(
                    report,
                    ignored
                        ? DeverQuestReadinessSeverity.Pass
                        : DeverQuestReadinessSeverity.Advisory,
                    "Timecard Git hygiene",
                    ignored
                        ? $"{relative}/ is excluded by the project " +
                          ".gitignore."
                        : $"The timecard folder {relative}/ is inside the " +
                          "Git project and is not explicitly ignored. Voice " +
                          "memos and Chronicles can make commits and pushes " +
                          "unexpectedly large. Add /" + relative +
                          "/ to .gitignore or move the timecard root outside " +
                          "the repository.");
            }
            catch (Exception exception)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Timecard Git hygiene",
                    "Repository exclusion could not be verified: " +
                    exception.Message);
            }
        }

        private static void CheckChroniclePolicy(
            DeverQuestReadinessReport report)
        {
            bool enabled =
                DeverQuestSettingsStore.Profile.chronicleIntegrityEnabled;

            Add(
                report,
                enabled
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Chronicle integrity",
                enabled
                    ? "Chronicle integrity verification is enabled."
                    : "Chronicle integrity verification is disabled. Release " +
                      "testing is safer with it enabled.");
        }

        private static void CheckQuestChronicleArchive(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            DeverQuestHistoryService.Refresh(profile);

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestHistoryService.LastError))
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Quest Chronicle archive",
                    "Chronicle history could not be fully loaded: " +
                    DeverQuestHistoryService.LastError);
                return;
            }

            List<DeverQuestSession> sessions =
                DeverQuestHistoryService.AllDays
                    .Where(day => day?.Record?.sessions != null)
                    .SelectMany(day => day.Record.sessions)
                    .Where(session => session != null)
                    .ToList();
            int duplicateSessionIds = sessions
                .Where(session =>
                    !string.IsNullOrWhiteSpace(session.sessionId))
                .GroupBy(session => session.sessionId)
                .Count(group => group.Count() > 1);
            int missingTimecards =
                DeverQuestHistoryService.AllDays.Count(day =>
                    !string.IsNullOrWhiteSpace(day.MarkdownPath) &&
                    !File.Exists(day.MarkdownPath));
            int missingAttachments = sessions.Sum(session =>
                (session.mediaAttachments ??
                 new List<DeverQuestMediaAttachment>())
                    .Count(attachment =>
                        attachment != null &&
                        !string.IsNullOrWhiteSpace(attachment.filePath) &&
                        !File.Exists(attachment.filePath)));

            bool clean = duplicateSessionIds == 0 &&
                         missingTimecards == 0;
            string detail = clean
                ? sessions.Count + " completed Quest record" +
                  (sessions.Count == 1 ? " is" : "s are") +
                  " available for Chronicle navigation" +
                  (missingAttachments > 0
                      ? "; " + missingAttachments +
                        " media attachment path(s) are no longer present."
                      : ".")
                : "Found " + duplicateSessionIds +
                  " duplicate Session ID group(s) and " +
                  missingTimecards +
                  " missing generated Timecard file(s). Refresh or repair " +
                  "the Chronicle before archive regression.";

            Add(
                report,
                clean
                    ? missingAttachments == 0
                        ? DeverQuestReadinessSeverity.Pass
                        : DeverQuestReadinessSeverity.Advisory
                    : DeverQuestReadinessSeverity.Advisory,
                "Quest Chronicle archive",
                detail);
        }

        private static void CheckSharedGuild(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (!profile.sharedGuildEnabled)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Pass,
                    "Shared Guild repository",
                    "Shared Guild publishing is disabled for this local " +
                    "profile.");
                return;
            }

            bool pathConfigured =
                !string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath) &&
                Directory.Exists(profile.sharedGuildRepositoryPath);

            Add(
                report,
                pathConfigured
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Blocker,
                "Shared Guild repository",
                pathConfigured
                    ? "The configured shared repository exists."
                    : "Shared Guild publishing is enabled, but its repository " +
                      "folder is missing or unavailable.");
        }

        private static void CheckAudioTransport(
            DeverQuestReadinessReport report)
        {
            Add(
                report,
                DeverQuestAudioTransport.IsAvailable
                    ? DeverQuestAudioTransport.UsingSupportedHost
                        ? DeverQuestReadinessSeverity.Pass
                        : DeverQuestReadinessSeverity.Advisory
                    : DeverQuestReadinessSeverity.Advisory,
                "Editor audio transport",
                DeverQuestAudioTransport.IsAvailable
                    ? DeverQuestAudioTransport.UsingSupportedHost
                        ? "The supported hidden AudioSource host is active and " +
                          "isolated from Inspector preview playback."
                        : "Audio is using the legacy Inspector-preview " +
                          "fallback. Open Audio & Wellness to reinitialize the " +
                          "supported host. " +
                          DeverQuestAudioTransport.StatusMessage
                    : "No Editor audio transport is available. Timers still " +
                      "work, but Music, Ambience, and cues will be silent.");

            Add(
                report,
                DeverQuestAudioTransport.IndependentVolumeSupported
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Independent audio mixer",
                DeverQuestAudioTransport.IndependentVolumeSupported
                    ? "Music, Ambience, and warning/SFX gain can be adjusted " +
                      "independently."
                    : "The current fallback exposes only shared preview gain. " +
                      "Independent mixer controls require the supported host.");

            Add(
                report,
                DeverQuestAudioTransport.PlaybackStatusSupported
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Playlist completion detection",
                DeverQuestAudioTransport.PlaybackStatusSupported
                    ? "Track completion status is available."
                    : "Automatic playlist advancement is unavailable in this " +
                      "Unity editor version.");
        }

        private static void CheckWellnessCommandCenter(
            DeverQuestReadinessReport report)
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            bool writable =
                DeverQuestWellnessHistoryService.CanWrite(
                    out string reason);
            int historyCount =
                DeverQuestWellnessHistoryService.Records.Count;
            int pendingCount =
                DeverQuestWellnessMonitor.PendingCount;
            bool settingsValid =
                profile.wellnessHistoryLimit >= 25 &&
                profile.wellnessHistoryLimit <= 1000 &&
                profile.quietHoursStartHour >= 0 &&
                profile.quietHoursStartHour <= 23 &&
                profile.quietHoursEndHour >= 0 &&
                profile.quietHoursEndHour <= 23;

            Add(
                report,
                writable && settingsValid
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Wellness command center",
                writable && settingsValid
                    ? $"Wellness history is writable with {historyCount} " +
                      $"local record(s) and {pendingCount} queued or " +
                      "snoozed reminder(s)."
                    : !writable
                        ? "The local Wellness History cannot be written: " +
                          reason
                        : "Wellness settings are outside their supported " +
                          "range. Open Audio & Wellness and review Quiet " +
                          "Hours and the History Record Limit.");
        }

        private static void CheckIdentityCatalog(
            DeverQuestReadinessReport report)
        {
            DeverQuestIdentityCatalog catalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            bool ready = catalog != null &&
                         catalog.defaultAncestry != null &&
                         catalog.defaultClass != null &&
                         (catalog.ancestries?.Count ?? 0) > 0 &&
                         (catalog.classes?.Count ?? 0) > 0;
            Add(
                report,
                ready
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Starter Identity Catalog",
                ready
                    ? $"{catalog.displayName} is active with valid defaults."
                    : "No complete active Identity Catalog was found. " +
                      "Open Guild Hall > Campaign Content Scaffolding, then " +
                      "generate the original starter catalog before testing " +
                      "new-character onboarding.");
        }

        private static void CheckTacticalTestContent(
            DeverQuestReadinessReport report)
        {
            int encounterCount = AssetDatabase.FindAssets(
                "t:DeverQuestEncounterProfile").Length;
            int companionCount = AssetDatabase.FindAssets(
                "t:DeverQuestCompanionProfile").Length;
            int abilityCount = AssetDatabase.FindAssets(
                "t:DeverQuestAttackTechnique").Length +
                AssetDatabase.FindAssets("t:DeverQuestSpell").Length;
            bool ready = encounterCount > 0 &&
                         companionCount > 0 &&
                         abilityCount > 0;
            Add(
                report,
                ready
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Tactical test content",
                ready
                    ? $"{encounterCount} Encounter Profile(s), " +
                      $"{companionCount} Companion Profile(s), and " +
                      $"{abilityCount} tactical ability asset(s) are " +
                      "available for Combat and Survival regression."
                    : "Combat visibility requires an Encounter Profile, a " +
                      "Companion Profile, and at least one Spell or Attack " +
                      "Technique. Open Guild Hall > Campaign Content " +
                      "Scaffolding and generate the Tactical Starter Kit and " +
                      "Original Companion Stable before Quest 4 testing.");
        }

        private static void CheckTacticalArchive(
            DeverQuestReadinessReport report)
        {
            bool writable =
                DeverQuestTacticalArchiveService.CanWrite(
                    out string reason);
            int records =
                DeverQuestTacticalArchiveService.Records.Count;
            Add(
                report,
                writable
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Tactical archive",
                writable
                    ? "The local Battle Archive accepted a write/delete " +
                      "probe and currently stores " + records +
                      " record" + (records == 1 ? "." : "s.")
                    : "The local Battle Archive could not be verified: " +
                      reason);
        }

        private static void CheckSpoilsSnapshots(
            DeverQuestReadinessReport report)
        {
            int mismatchCount = 0;
            foreach (string guid in AssetDatabase.FindAssets(
                         "t:DeverQuestQuestContract"))
            {
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestQuestContract>(
                        AssetDatabase.GUIDToAssetPath(guid));
                if (contract != null &&
                    contract.questProfile != null &&
                    !contract.SpoilsMatchLinkedProfile() &&
                    contract.CanRefreshSpoilsFromProfile())
                {
                    mismatchCount++;
                }
            }

            Add(
                report,
                mismatchCount == 0
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Quest Contract Spoils",
                mismatchCount == 0
                    ? "All refreshable Contract Spoils snapshots match their " +
                      "linked Quest Profiles."
                    : $"{mismatchCount} refreshable Contract(s) have Spoils " +
                      "that differ from their linked profiles. Select each " +
                      "Contract in DeverQuest to refresh it before testing.");
        }

        private static void CheckQuestRunReservations(
            DeverQuestReadinessReport report)
        {
            int staleRunCount = 0;
            int invalidRunCount = 0;
            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract == null || contract.activeRuns == null)
                {
                    continue;
                }
                foreach (DeverQuestContractRunReservation run
                         in contract.activeRuns)
                {
                    if (run == null ||
                        string.IsNullOrWhiteSpace(run.runId))
                    {
                        invalidRunCount++;
                        continue;
                    }
                    if (DateTime.TryParse(
                            run.startedUtc,
                            null,
                            System.Globalization.DateTimeStyles.RoundtripKind,
                            out DateTime started) &&
                        DateTime.UtcNow - started.ToUniversalTime() >
                        TimeSpan.FromHours(24d))
                    {
                        staleRunCount++;
                    }
                }
            }

            bool clean = staleRunCount == 0 && invalidRunCount == 0;
            Add(
                report,
                clean
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Quest Run reservations",
                clean
                    ? "No invalid or older-than-24-hour Quest Run " +
                      "reservations were found."
                    : $"Found {staleRunCount} older-than-24-hour and " +
                      $"{invalidRunCount} invalid reservation(s). Review " +
                      "Guild Hall > Quest Run Management.");
        }

        private static void CheckInventoryHealth(
            DeverQuestReadinessReport report)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            List<DeverQuestInventoryEntry> entries =
                (adventurer.inventory ??
                 new List<DeverQuestInventoryEntry>())
                .Where(value => value != null && value.quantity > 0)
                .ToList();

            int duplicateOwnershipIds = entries
                .Where(value =>
                    !string.IsNullOrWhiteSpace(value.ownershipId))
                .GroupBy(value => value.ownershipId)
                .Count(group => group.Count() > 1);
            int unresolvedEquipment = entries.Count(value =>
                value.itemType == DeverQuestShopItemType.Equipment &&
                DeverQuestInventoryService.FindEquipment(value) == null);
            int unsafeProtected = entries.Count(value =>
                (value.questProtected ||
                 value.itemCategory ==
                 DeverQuestItemCategory.QuestItem) &&
                (value.tradable || value.droppable));
            int orphanEquipped =
                (adventurer.equippedEquipmentIds ??
                 new List<string>())
                .Count(value =>
                    DeverQuestRulesService.FindEquipment(value) == null);
            HashSet<string> carriedEquipmentIds =
                new HashSet<string>(
                    entries
                        .Where(value =>
                            !string.IsNullOrWhiteSpace(
                                value.equipmentId))
                        .Select(value => value.equipmentId));
            int equippedWithoutInventory =
                (adventurer.equippedEquipmentIds ??
                 new List<string>())
                .Count(value =>
                    DeverQuestRulesService.FindEquipment(value) != null &&
                    !carriedEquipmentIds.Contains(value));
            DeverQuestCarrySummary carry =
                DeverQuestEncumbranceService.Summary(adventurer);

            bool healthy = duplicateOwnershipIds == 0 &&
                           unresolvedEquipment == 0 &&
                           unsafeProtected == 0 &&
                           orphanEquipped == 0 &&
                           equippedWithoutInventory == 0;
            string detail = healthy
                ? $"{entries.Count} inventory entr" +
                  (entries.Count == 1 ? "y" : "ies") +
                  $" validated. Carry load is " +
                  $"{carry.TotalWeight:0.0}/{carry.Capacity:0.0} " +
                  $"({carry.Status})."
                : $"Inventory validation found {duplicateOwnershipIds} " +
                  $"duplicate ownership ID group(s), " +
                  $"{unresolvedEquipment} unresolved equipment entr" +
                  (unresolvedEquipment == 1 ? "y" : "ies") +
                  $", {unsafeProtected} unsafe protected entr" +
                  (unsafeProtected == 1 ? "y" : "ies") +
                  $", {orphanEquipped} missing equipped asset " +
                  (orphanEquipped == 1 ? "reference" : "references") +
                  $", and {equippedWithoutInventory} equipped item" +
                  (equippedWithoutInventory == 1 ? " is" : "s are") +
                  " missing from inventory. Open Inventory and Equipment " +
                  "before continuing " +
                  "item regression.";

            Add(
                report,
                healthy
                    ? DeverQuestReadinessSeverity.Pass
                    : DeverQuestReadinessSeverity.Advisory,
                "Inventory integrity",
                detail);
        }

        private static void CheckGuildEconomy(
            DeverQuestReadinessReport report)
        {
            DeverQuestShopProfile profile =
                DeverQuestShopService.ActiveProfile;
            int duplicates =
                DeverQuestEconomyService.DuplicateIdCount();
            if (duplicates > 0)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Blocker,
                    "Guild economy",
                    $"The local economy ledger contains {duplicates} " +
                    "empty or duplicate transaction ID group(s).");
                return;
            }
            if (profile == null)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Guild economy",
                    "No active Shop Profile is selected. Open the Economy " +
                    "workspace and select or generate a Quartermaster " +
                    "before testing purchases, sales, and grants.");
                return;
            }

            int stock = profile.items?.Count(item => item != null) ?? 0;
            Add(
                report,
                DeverQuestReadinessSeverity.Pass,
                "Guild economy",
                $"{profile.displayName} is the active Quartermaster with " +
                $"{stock} stocked item(s), and the local ledger contains " +
                $"{DeverQuestEconomyService.Records.Count} transaction(s).");
        }

        private static void CheckContentHealth(
            DeverQuestReadinessReport report)
        {
            try
            {
                DeverQuestContentValidationReport content =
                    DeverQuestContentValidationService.Run();
                DeverQuestReadinessSeverity severity =
                    content.ErrorCount > 0
                        ? DeverQuestReadinessSeverity.Blocker
                        : content.WarningCount > 0
                            ? DeverQuestReadinessSeverity.Advisory
                            : DeverQuestReadinessSeverity.Pass;
                string detail = content.ErrorCount > 0
                    ? content.Summary +
                      ". Open Beta Administration and resolve all errors."
                    : content.WarningCount > 0
                        ? content.Summary +
                          ". Review warnings under Beta Administration."
                        : content.Summary + ". Production content is healthy.";
                Add(report, severity, "Beta content health", detail);
            }
            catch (Exception exception)
            {
                Add(
                    report,
                    DeverQuestReadinessSeverity.Advisory,
                    "Beta content health",
                    "Content validation could not complete: " +
                    exception.Message);
            }
        }

        private static void CheckSessionState(
            DeverQuestReadinessReport report)
        {
            Add(
                report,
                DeverQuestSessionStore.HasActiveSession
                    ? DeverQuestReadinessSeverity.Advisory
                    : DeverQuestReadinessSeverity.Pass,
                "Active Quest state",
                DeverQuestSessionStore.HasActiveSession
                    ? "A Quest is active. Complete or abandon it before " +
                      "migration and clean-install regression."
                    : "No active Quest will interfere with clean regression.");
        }

        private static bool TryReadUnityVersion(
            string version,
            out int major,
            out int minor)
        {
            major = 0;
            minor = 0;
            if (string.IsNullOrWhiteSpace(version))
            {
                return false;
            }

            string[] parts = version.Split('.');
            return parts.Length >= 2 &&
                   int.TryParse(parts[0], out major) &&
                   int.TryParse(parts[1], out minor);
        }

        private static void Add(
            DeverQuestReadinessReport report,
            DeverQuestReadinessSeverity severity,
            string title,
            string detail)
        {
            report.findings.Add(
                new DeverQuestReadinessFinding
                {
                    severity = severity,
                    title = title,
                    detail = detail
                });
        }
    }
}

//----- DeverQuestReleaseReadinessService.cs END -----
