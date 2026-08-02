//----- DeverQuestDistributionService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestDistributionChannel
    {
        InternalBeta = 0,
        ExternalBeta = 1,
        ReleaseCandidate = 2
    }

    internal enum DeverQuestDistributionSeverity
    {
        Pass = 0,
        Advisory = 1,
        Blocker = 2
    }

    [Serializable]
    internal sealed class DeverQuestDistributionFinding
    {
        public string severity = string.Empty;
        public string title = string.Empty;
        public string detail = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestPackageManifestEntry
    {
        public string path = string.Empty;
        public long size;
        public string sha256 = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestPackageManifest
    {
        public string packageName = string.Empty;
        public string packageVersion = string.Empty;
        public string generatedUtc = string.Empty;
        public int fileCount;
        public long totalBytes;
        public List<DeverQuestPackageManifestEntry> files =
            new List<DeverQuestPackageManifestEntry>();
    }

    [Serializable]
    internal sealed class DeverQuestDistributionJson
    {
        public string packageVersion = string.Empty;
        public string channel = string.Empty;
        public string generatedUtc = string.Empty;
        public string verdict = string.Empty;
        public int passes;
        public int advisories;
        public int blockers;
        public int packageFiles;
        public long packageBytes;
        public List<DeverQuestDistributionFinding> findings =
            new List<DeverQuestDistributionFinding>();
    }

    internal sealed class DeverQuestDistributionReport
    {
        public DeverQuestDistributionChannel channel;
        public string generatedUtc = string.Empty;
        public string packageRoot = string.Empty;
        public string repositoryRoot = string.Empty;
        public string packageVersion = string.Empty;
        public DeverQuestPackageManifest manifest;
        public readonly List<DeverQuestDistributionFinding> findings =
            new List<DeverQuestDistributionFinding>();

        public int PassCount => findings.Count(value =>
            string.Equals(
                value.severity,
                DeverQuestDistributionSeverity.Pass.ToString(),
                StringComparison.Ordinal));

        public int AdvisoryCount => findings.Count(value =>
            string.Equals(
                value.severity,
                DeverQuestDistributionSeverity.Advisory.ToString(),
                StringComparison.Ordinal));

        public int BlockerCount => findings.Count(value =>
            string.Equals(
                value.severity,
                DeverQuestDistributionSeverity.Blocker.ToString(),
                StringComparison.Ordinal));

        public string Summary =>
            $"{PassCount} passed · {AdvisoryCount} advisories · " +
            $"{BlockerCount} blockers";

        public string Verdict
        {
            get
            {
                if (BlockerCount > 0)
                {
                    return channel == DeverQuestDistributionChannel.InternalBeta
                        ? "Internal Beta package needs attention"
                        : "External distribution blocked";
                }

                if (AdvisoryCount > 0)
                {
                    return channel == DeverQuestDistributionChannel.InternalBeta
                        ? "Internal Beta dossier ready with advisories"
                        : "Conditional distribution readiness";
                }

                return channel == DeverQuestDistributionChannel.ReleaseCandidate
                    ? "Release Candidate dossier ready"
                    : "Beta distribution dossier ready";
            }
        }
    }

    internal sealed class DeverQuestDistributionExportResult
    {
        public DeverQuestDistributionReport report;
        public string outputDirectory = string.Empty;
        public string archivePath = string.Empty;
        public string dossierPath = string.Empty;
        public string manifestPath = string.Empty;
        public string checksumPath = string.Empty;
        public bool archiveVerified;
        public string archiveVerification = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestPackageJsonData
    {
        public string name = string.Empty;
        public string version = string.Empty;
        public string displayName = string.Empty;
        public string unity = string.Empty;
    }

    internal static class DeverQuestDistributionService
    {
        public const string ExpectedVersion = "0.32.5";
        private const string ExpectedPackageName =
            "com.echodevgames.deverquest";
        private const string ReleaseFolderName = "DeverQuestReleases";

        private static readonly string[] ForbiddenMediaExtensions =
        {
            ".wav", ".mp3", ".ogg", ".aiff", ".aif", ".flac", ".m4a",
            ".png", ".jpg", ".jpeg", ".gif", ".bmp", ".tga", ".psd",
            ".ttf", ".otf", ".woff", ".woff2", ".mp4", ".mov", ".webm"
        };

        public static string LastExportDirectory { get; private set; } =
            string.Empty;

        [MenuItem(
            "Tools/DeverQuest/Distribution/Run Packaging Audit",
            false,
            60)]
        private static void RunFromMenu()
        {
            DeverQuestDistributionReport report = Run(
                DeverQuestDistributionChannel.ExternalBeta);
            string text = BuildDossierMarkdown(report, null);

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
                "DeverQuest Packaging Audit",
                report.Verdict + "\n\n" + report.Summary +
                "\n\nOpen the Distribution workspace for details.",
                "Close");
        }

        public static DeverQuestDistributionReport Run(
            DeverQuestDistributionChannel channel)
        {
            DeverQuestDistributionReport report =
                new DeverQuestDistributionReport
                {
                    channel = channel,
                    generatedUtc = DateTime.UtcNow.ToString("o"),
                    repositoryRoot = GetProjectRoot()
                };

            UnityEditor.PackageManager.PackageInfo package =
                UnityEditor.PackageManager.PackageInfo.FindForAssembly(
                    typeof(DeverQuestDistributionService).Assembly);

            if (package == null ||
                string.IsNullOrWhiteSpace(package.resolvedPath))
            {
                Add(
                    report,
                    DeverQuestDistributionSeverity.Blocker,
                    "Installed package location",
                    "Unity could not resolve the installed DeverQuest package " +
                    "directory.");
                return report;
            }

            report.packageRoot = package.resolvedPath;
            report.packageVersion = package.version ?? string.Empty;
            report.manifest = BuildManifest(report.packageRoot);

            CheckInstalledPackage(report, package);
            CheckRequiredPackageFiles(report);
            CheckVersionConsistency(report);
            CheckScriptMetadata(report);
            CheckMedia(report);
            CheckRepositoryReleaseFiles(report);
            CheckLicense(report);
            CheckCreditsAndNotices(report);
            CheckKnownLimitations(report);
            CheckReadiness(report);
            CheckContentHealth(report);

            return report;
        }

        public static DeverQuestDistributionExportResult Export(
            DeverQuestDistributionChannel channel)
        {
            DeverQuestDistributionReport report = Run(channel);
            string projectRoot = report.repositoryRoot;
            if (string.IsNullOrWhiteSpace(projectRoot))
            {
                throw new InvalidOperationException(
                    "The Unity project root could not be resolved.");
            }
            if (string.IsNullOrWhiteSpace(report.packageRoot) ||
                !Directory.Exists(report.packageRoot))
            {
                throw new InvalidOperationException(
                    "The installed package root could not be resolved.");
            }

            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string channelName = ChannelSlug(channel);
            string outputDirectory = Path.Combine(
                projectRoot,
                ReleaseFolderName,
                $"DeverQuest_{ExpectedVersion}_{channelName}_{stamp}");
            Directory.CreateDirectory(outputDirectory);

            string manifestPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Package_Manifest.json");
            WriteUtf8(
                manifestPath,
                JsonUtility.ToJson(report.manifest, true));

            string fileHashesPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Package_Files.sha256");
            WriteUtf8(
                fileHashesPath,
                BuildPackageHashList(report.manifest));

            string knownLimitationsPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Known_Limitations.md");
            WriteUtf8(
                knownLimitationsPath,
                ReadKnownLimitations(report.packageRoot));

            string checklistPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Distribution_Checklist.md");
            WriteUtf8(
                checklistPath,
                BuildDistributionChecklist(channel));

            string healthPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Content_Health.md");
            DeverQuestContentValidationReport content =
                DeverQuestContentValidationService.Run();
            WriteUtf8(
                healthPath,
                DeverQuestContentValidationService.BuildMarkdown(content));

            string archivePath = Path.Combine(
                outputDirectory,
                $"com.echodevgames.deverquest-{ExpectedVersion}.tgz");
            CreateTarGz(
                report.packageRoot,
                report.manifest,
                archivePath);

            bool archiveVerified = VerifyTarGz(
                archivePath,
                report.manifest,
                out string archiveVerification);

            Add(
                report,
                archiveVerified
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Exported tarball integrity",
                archiveVerification);

            string dossierPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Release_Dossier.md");
            DeverQuestDistributionExportResult result =
                new DeverQuestDistributionExportResult
                {
                    report = report,
                    outputDirectory = outputDirectory,
                    archivePath = archivePath,
                    dossierPath = dossierPath,
                    manifestPath = manifestPath,
                    archiveVerified = archiveVerified,
                    archiveVerification = archiveVerification
                };
            WriteUtf8(
                dossierPath,
                BuildDossierMarkdown(report, result));

            string reportJsonPath = Path.Combine(
                outputDirectory,
                $"DeverQuest_{ExpectedVersion}_Distribution_Report.json");
            WriteUtf8(
                reportJsonPath,
                JsonUtility.ToJson(BuildJson(report), true));

            string checksumsPath = Path.Combine(
                outputDirectory,
                "SHA256SUMS.txt");
            WriteUtf8(
                checksumsPath,
                BuildOutputChecksums(outputDirectory, checksumsPath));
            result.checksumPath = checksumsPath;

            LastExportDirectory = outputDirectory;
            AssetDatabase.Refresh();
            return result;
        }

        public static string BuildDossierMarkdown(
            DeverQuestDistributionReport report,
            DeverQuestDistributionExportResult export)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                $"# DeverQuest {ExpectedVersion} Beta Release Dossier");
            builder.AppendLine();
            builder.AppendLine($"**Generated UTC:** {report.generatedUtc}");
            builder.AppendLine($"**Channel:** {ChannelDisplay(report.channel)}");
            builder.AppendLine($"**Verdict:** {report.Verdict}");
            builder.AppendLine($"**Audit:** {report.Summary}");
            builder.AppendLine();

            if (report.manifest != null)
            {
                builder.AppendLine("## Installed package inventory");
                builder.AppendLine();
                builder.AppendLine($"- Files: {report.manifest.fileCount}");
                builder.AppendLine($"- Bytes: {report.manifest.totalBytes}");
                builder.AppendLine(
                    $"- Package: `{report.manifest.packageName}`");
                builder.AppendLine(
                    $"- Version: `{report.manifest.packageVersion}`");
                builder.AppendLine();
            }

            if (export != null)
            {
                builder.AppendLine("## Exported release artifacts");
                builder.AppendLine();
                builder.AppendLine(
                    $"- Package tarball: `{Path.GetFileName(export.archivePath)}`");
                builder.AppendLine(
                    $"- Tarball verification: " +
                    (export.archiveVerified ? "PASS" : "FAIL"));
                builder.AppendLine(
                    $"- Package manifest: `{Path.GetFileName(export.manifestPath)}`");
                builder.AppendLine(
                    "- Package-file checksums: " +
                    $"`DeverQuest_{ExpectedVersion}_Package_Files.sha256`");
                builder.AppendLine("- Dossier checksums: `SHA256SUMS.txt`");
                builder.AppendLine();
            }

            builder.AppendLine("## Findings");
            builder.AppendLine();
            foreach (DeverQuestDistributionFinding finding in report.findings)
            {
                builder.AppendLine(
                    $"### [{finding.severity.ToUpperInvariant()}] " +
                    finding.title);
                builder.AppendLine();
                builder.AppendLine(finding.detail);
                builder.AppendLine();
            }

            builder.AppendLine("## Distribution rule");
            builder.AppendLine();
            if (report.BlockerCount > 0)
            {
                builder.AppendLine(
                    "Do not publish this release outside the intended test " +
                    "group until every blocker is resolved and a new dossier " +
                    "is generated.");
            }
            else if (report.AdvisoryCount > 0)
            {
                builder.AppendLine(
                    "Distribution is conditional. Every advisory must be " +
                    "accepted explicitly in release notes or resolved before " +
                    "the next release gate.");
            }
            else
            {
                builder.AppendLine(
                    "The automated packaging gates are clear. Preserve this " +
                    "dossier, checksum set, health report, and readiness report " +
                    "with the tagged source revision.");
            }

            return builder.ToString().TrimEnd();
        }

        private static void CheckInstalledPackage(
            DeverQuestDistributionReport report,
            UnityEditor.PackageManager.PackageInfo package)
        {
            bool nameCorrect = string.Equals(
                package.name,
                ExpectedPackageName,
                StringComparison.Ordinal);
            bool versionCorrect = string.Equals(
                package.version,
                ExpectedVersion,
                StringComparison.Ordinal);

            Add(
                report,
                nameCorrect && versionCorrect
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Installed package identity",
                nameCorrect && versionCorrect
                    ? $"Package Manager reports {ExpectedPackageName} " +
                      $"{ExpectedVersion}."
                    : $"Package Manager reports {package.name} " +
                      $"{package.version}; expected {ExpectedPackageName} " +
                      $"{ExpectedVersion}.");
        }

        private static void CheckRequiredPackageFiles(
            DeverQuestDistributionReport report)
        {
            string[] required =
            {
                "package.json",
                "README.md",
                "CHANGELOG.md",
                "CREDITS.md",
                "THIRD_PARTY_NOTICES.md",
                "KNOWN_LIMITATIONS.md",
                "RELEASE_NOTES.md",
                "Runtime/EchoDevGames.DeverQuest.Runtime.asmdef",
                "Editor/EchoDevGames.DeverQuest.Editor.asmdef"
            };
            List<string> missing = required
                .Where(value =>
                    !File.Exists(Path.Combine(report.packageRoot, value)))
                .ToList();

            Add(
                report,
                missing.Count == 0
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Required package files",
                missing.Count == 0
                    ? "Package metadata, release notes, limitations, credits, " +
                      "notices, and assembly definitions are present."
                    : "Missing package files: " + string.Join(", ", missing));
        }

        private static void CheckVersionConsistency(
            DeverQuestDistributionReport report)
        {
            List<string> mismatches = new List<string>();
            string packageJsonPath = Path.Combine(
                report.packageRoot,
                "package.json");
            if (File.Exists(packageJsonPath))
            {
                DeverQuestPackageJsonData data = JsonUtility.FromJson<
                    DeverQuestPackageJsonData>(File.ReadAllText(packageJsonPath));
                if (data == null ||
                    !string.Equals(
                        data.version,
                        ExpectedVersion,
                        StringComparison.Ordinal))
                {
                    mismatches.Add("package.json");
                }
            }

            RequireText(
                report.packageRoot,
                "README.md",
                $"Current package version: **{ExpectedVersion} Beta 1**",
                mismatches);
            RequireText(
                report.packageRoot,
                "CHANGELOG.md",
                $"## [{ExpectedVersion}]",
                mismatches);
            RequireText(
                report.packageRoot,
                "THIRD_PARTY_NOTICES.md",
                $"**Package version:** {ExpectedVersion} Beta 1",
                mismatches);
            RequireText(
                report.packageRoot,
                "RELEASE_NOTES.md",
                $"# DeverQuest {ExpectedVersion}",
                mismatches);
            RequireText(
                report.packageRoot,
                "Editor/DeverQuestReleaseReadinessService.cs",
                $"ExpectedPackageVersion = \"{ExpectedVersion}\"",
                mismatches);
            RequireText(
                report.packageRoot,
                "Editor/DeverQuestContentValidationService.cs",
                $"packageVersion = \"{ExpectedVersion}\"",
                mismatches);

            Add(
                report,
                mismatches.Count == 0
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Version consistency",
                mismatches.Count == 0
                    ? $"Package and release declarations agree on " +
                      $"{ExpectedVersion}."
                    : "Version declarations need attention: " +
                      string.Join(", ", mismatches.Distinct()));
        }

        private static void CheckScriptMetadata(
            DeverQuestDistributionReport report)
        {
            string[] scripts = Directory.GetFiles(
                report.packageRoot,
                "*.cs",
                SearchOption.AllDirectories);
            List<string> missingMeta = scripts
                .Where(path => !File.Exists(path + ".meta"))
                .Select(path => Relative(report.packageRoot, path))
                .ToList();

            Dictionary<string, List<string>> guidPaths =
                new Dictionary<string, List<string>>(
                    StringComparer.OrdinalIgnoreCase);
            foreach (string metaPath in Directory.GetFiles(
                         report.packageRoot,
                         "*.meta",
                         SearchOption.AllDirectories))
            {
                string guid = ReadMetaGuid(metaPath);
                if (string.IsNullOrWhiteSpace(guid))
                {
                    continue;
                }
                if (!guidPaths.TryGetValue(guid, out List<string> paths))
                {
                    paths = new List<string>();
                    guidPaths.Add(guid, paths);
                }
                paths.Add(Relative(report.packageRoot, metaPath));
            }
            List<string> duplicates = guidPaths
                .Where(pair => pair.Value.Count > 1)
                .Select(pair => pair.Key + " => " +
                    string.Join(" | ", pair.Value))
                .ToList();

            DeverQuestDistributionSeverity severity = duplicates.Count > 0
                ? DeverQuestDistributionSeverity.Blocker
                : missingMeta.Count > 0
                    ? DeverQuestDistributionSeverity.Advisory
                    : DeverQuestDistributionSeverity.Pass;
            string detail;
            if (duplicates.Count > 0)
            {
                detail = "Duplicate package GUID groups: " +
                         string.Join("; ", duplicates);
            }
            else if (missingMeta.Count > 0)
            {
                detail = $"All explicit package GUIDs are unique. " +
                         $"{missingMeta.Count} established source file(s) do " +
                         "not ship explicit metadata. Do not assign new GUIDs " +
                         "during Beta without a reference-migration plan. " +
                         "New standalone asset types must continue to include " +
                         "stable metadata.";
            }
            else
            {
                detail = $"{scripts.Length} C# source files have metadata and " +
                         "all package GUIDs are unique.";
            }

            Add(
                report,
                severity,
                "Unity metadata integrity",
                detail);
        }

        private static void CheckMedia(
            DeverQuestDistributionReport report)
        {
            List<string> media = Directory.GetFiles(
                    report.packageRoot,
                    "*",
                    SearchOption.AllDirectories)
                .Where(path => ForbiddenMediaExtensions.Contains(
                    Path.GetExtension(path),
                    StringComparer.OrdinalIgnoreCase))
                .Select(path => Relative(report.packageRoot, path))
                .ToList();

            Add(
                report,
                media.Count == 0
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Bundled media audit",
                media.Count == 0
                    ? "No music, ambience, SFX, image, font, or video files " +
                      "are bundled in the package."
                    : "Bundled media requires an explicit license review: " +
                      string.Join(", ", media));
        }

        private static void CheckRepositoryReleaseFiles(
            DeverQuestDistributionReport report)
        {
            string[] required =
            {
                "README.md",
                "CREDITS.md",
                "THIRD_PARTY_NOTICES.md"
            };
            List<string> missing = required
                .Where(value => !File.Exists(Path.Combine(
                    report.repositoryRoot,
                    value)))
                .ToList();
            if (!Directory.Exists(Path.Combine(
                    report.repositoryRoot,
                    "Documentation")))
            {
                missing.Add("Documentation/");
            }

            Add(
                report,
                missing.Count == 0
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Repository release files",
                missing.Count == 0
                    ? "Repository README, credits, notices, and Documentation " +
                      "folder are present."
                    : "Missing repository release files: " +
                      string.Join(", ", missing));
        }

        private static void CheckLicense(
            DeverQuestDistributionReport report)
        {
            bool packageLicense = FindLicense(report.packageRoot);
            bool repositoryLicense = FindLicense(report.repositoryRoot);
            bool available = packageLicense || repositoryLicense;
            DeverQuestDistributionSeverity severity;
            if (available)
            {
                severity = DeverQuestDistributionSeverity.Pass;
            }
            else
            {
                severity = report.channel ==
                    DeverQuestDistributionChannel.InternalBeta
                        ? DeverQuestDistributionSeverity.Advisory
                        : DeverQuestDistributionSeverity.Blocker;
            }

            Add(
                report,
                severity,
                "Software license",
                available
                    ? "A LICENSE file is present for distribution review."
                    : "No LICENSE, LICENSE.md, or LICENSE.txt file was found " +
                      "at the repository or package root. Internal testing may " +
                      "continue, but public distribution requires an explicit " +
                      "license decision.");
        }

        private static void CheckCreditsAndNotices(
            DeverQuestDistributionReport report)
        {
            string credits = ReadText(
                Path.Combine(report.packageRoot, "CREDITS.md"));
            string notices = ReadText(
                Path.Combine(report.packageRoot, "THIRD_PARTY_NOTICES.md"));
            bool hasCredits = credits.IndexOf(
                "Jesse",
                StringComparison.OrdinalIgnoreCase) >= 0 &&
                credits.IndexOf(
                    "ChatGPT",
                    StringComparison.OrdinalIgnoreCase) >= 0;
            bool noBundledMedia = notices.IndexOf(
                "No third-party music",
                StringComparison.OrdinalIgnoreCase) >= 0;
            bool hasOpenReviewLanguage =
                credits.IndexOf(
                    "must be recorded before",
                    StringComparison.OrdinalIgnoreCase) >= 0 ||
                notices.IndexOf(
                    "license review",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            Add(
                report,
                hasCredits && noBundledMedia
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Credits and third-party notices",
                hasCredits && noBundledMedia
                    ? "Credits identify project ownership and development " +
                      "assistance; notices state that no third-party media is " +
                      "bundled."
                    : "Credits or third-party notice declarations are incomplete.");

            if (hasOpenReviewLanguage)
            {
                Add(
                    report,
                    report.channel == DeverQuestDistributionChannel.InternalBeta
                        ? DeverQuestDistributionSeverity.Advisory
                        : DeverQuestDistributionSeverity.Advisory,
                    "Third-party asset ledger",
                    "RPG Sounds, Loudlib, and other candidate local media still " +
                    "require exact asset-pack, creator, receipt, attribution, " +
                    "and redistribution records before any of those files may " +
                    "be shipped. The current package contains none of them.");
            }
        }

        private static void CheckKnownLimitations(
            DeverQuestDistributionReport report)
        {
            string path = Path.Combine(
                report.packageRoot,
                "KNOWN_LIMITATIONS.md");
            string text = ReadText(path);
            bool healthy = File.Exists(path) &&
                text.IndexOf(
                    "Deferred verification",
                    StringComparison.OrdinalIgnoreCase) >= 0 &&
                text.IndexOf(
                    "EditorPrefs",
                    StringComparison.OrdinalIgnoreCase) >= 0;

            Add(
                report,
                healthy
                    ? DeverQuestDistributionSeverity.Pass
                    : DeverQuestDistributionSeverity.Blocker,
                "Known limitations",
                healthy
                    ? "Known limitations and deferred verification boundaries " +
                      "are documented."
                    : "KNOWN_LIMITATIONS.md is missing required release and " +
                      "persistence boundaries.");
        }

        private static void CheckReadiness(
            DeverQuestDistributionReport report)
        {
            DeverQuestReadinessReport readiness =
                DeverQuestReleaseReadinessService.Run();
            DeverQuestDistributionSeverity severity =
                readiness.BlockerCount > 0
                    ? DeverQuestDistributionSeverity.Blocker
                    : readiness.AdvisoryCount > 0
                        ? DeverQuestDistributionSeverity.Advisory
                        : DeverQuestDistributionSeverity.Pass;
            Add(
                report,
                severity,
                "Release Readiness baseline",
                readiness.Summary +
                (readiness.BlockerCount == 0 && readiness.AdvisoryCount == 0
                    ? " The installed project is clean for regression."
                    : " Review the full Release Readiness report before export."));
        }

        private static void CheckContentHealth(
            DeverQuestDistributionReport report)
        {
            DeverQuestContentValidationReport content =
                DeverQuestContentValidationService.Run();
            DeverQuestDistributionSeverity severity =
                content.ErrorCount > 0
                    ? DeverQuestDistributionSeverity.Blocker
                    : content.WarningCount > 0
                        ? DeverQuestDistributionSeverity.Advisory
                        : DeverQuestDistributionSeverity.Pass;
            Add(
                report,
                severity,
                "Production content health",
                content.Summary +
                (content.ErrorCount == 0 && content.WarningCount == 0
                    ? ". Production content is healthy."
                    : ". Open Beta Administration for the exact findings."));
        }

        private static DeverQuestPackageManifest BuildManifest(
            string packageRoot)
        {
            DeverQuestPackageManifest manifest =
                new DeverQuestPackageManifest
                {
                    packageName = ExpectedPackageName,
                    packageVersion = ExpectedVersion,
                    generatedUtc = DateTime.UtcNow.ToString("o")
                };

            foreach (string path in Directory.GetFiles(
                         packageRoot,
                         "*",
                         SearchOption.AllDirectories)
                     .Where(IncludePackageFile)
                     .OrderBy(value => value, StringComparer.Ordinal))
            {
                FileInfo info = new FileInfo(path);
                manifest.files.Add(new DeverQuestPackageManifestEntry
                {
                    path = Relative(packageRoot, path),
                    size = info.Length,
                    sha256 = HashFile(path)
                });
                manifest.totalBytes += info.Length;
            }
            manifest.fileCount = manifest.files.Count;
            return manifest;
        }

        private static bool IncludePackageFile(string path)
        {
            string name = Path.GetFileName(path);
            if (string.Equals(name, ".DS_Store", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(name, "Thumbs.db", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
            string normalized = path.Replace('\\', '/');
            return normalized.IndexOf("/.git/", StringComparison.OrdinalIgnoreCase) < 0 &&
                   normalized.IndexOf("/bin/", StringComparison.OrdinalIgnoreCase) < 0 &&
                   normalized.IndexOf("/obj/", StringComparison.OrdinalIgnoreCase) < 0;
        }

        private static void CreateTarGz(
            string packageRoot,
            DeverQuestPackageManifest manifest,
            string destinationPath)
        {
            if (File.Exists(destinationPath))
            {
                File.Delete(destinationPath);
            }

            using (FileStream file = new FileStream(
                       destinationPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None))
            using (GZipStream gzip = new GZipStream(
                       file,
                       System.IO.Compression.CompressionLevel.Optimal,
                       false))
            {
                foreach (DeverQuestPackageManifestEntry entry in manifest.files)
                {
                    string sourcePath = Path.Combine(
                        packageRoot,
                        entry.path.Replace('/', Path.DirectorySeparatorChar));
                    WriteTarFile(gzip, sourcePath, "package/" + entry.path);
                }

                byte[] zeroBlocks = new byte[1024];
                gzip.Write(zeroBlocks, 0, zeroBlocks.Length);
            }
        }

        private static void WriteTarFile(
            Stream destination,
            string sourcePath,
            string archivePath)
        {
            byte[] header = new byte[512];
            WriteAscii(header, 0, 100, archivePath);
            WriteOctal(header, 100, 8, 420L);
            WriteOctal(header, 108, 8, 0L);
            WriteOctal(header, 116, 8, 0L);
            long length = new FileInfo(sourcePath).Length;
            WriteOctal(header, 124, 12, length);
            WriteOctal(header, 136, 12, 0L);
            for (int index = 148; index < 156; index++)
            {
                header[index] = 32;
            }
            header[156] = (byte)'0';
            WriteAscii(header, 257, 6, "ustar");
            WriteAscii(header, 263, 2, "00");
            WriteAscii(header, 265, 32, "EchoDevGames");
            WriteAscii(header, 297, 32, "EchoDevGames");

            int checksum = header.Sum(value => (int)value);
            string checksumText = Convert.ToString(checksum, 8)
                .PadLeft(6, '0');
            WriteAscii(header, 148, 6, checksumText);
            header[154] = 0;
            header[155] = 32;
            destination.Write(header, 0, header.Length);

            using (FileStream source = File.OpenRead(sourcePath))
            {
                source.CopyTo(destination);
            }

            int remainder = (int)(length % 512L);
            if (remainder != 0)
            {
                byte[] padding = new byte[512 - remainder];
                destination.Write(padding, 0, padding.Length);
            }
        }

        private static bool VerifyTarGz(
            string archivePath,
            DeverQuestPackageManifest manifest,
            out string detail)
        {
            try
            {
                Dictionary<string, DeverQuestPackageManifestEntry> expected =
                    manifest.files.ToDictionary(
                        value => "package/" + value.path,
                        value => value,
                        StringComparer.Ordinal);
                HashSet<string> seen = new HashSet<string>(
                    StringComparer.Ordinal);

                using (FileStream file = File.OpenRead(archivePath))
                using (GZipStream gzip = new GZipStream(
                           file,
                           CompressionMode.Decompress,
                           false))
                {
                    byte[] header = new byte[512];
                    while (ReadBlock(gzip, header))
                    {
                        if (header.All(value => value == 0))
                        {
                            break;
                        }

                        string name = ReadAscii(header, 0, 100);
                        long size = ReadOctal(header, 124, 12);
                        char type = (char)header[156];
                        if (type != '0' && type != '\0')
                        {
                            Skip(gzip, size);
                            SkipPadding(gzip, size);
                            continue;
                        }

                        if (!expected.TryGetValue(
                                name,
                                out DeverQuestPackageManifestEntry entry))
                        {
                            detail = "Tarball contains an unexpected file: " + name;
                            return false;
                        }
                        if (entry.size != size)
                        {
                            detail = $"Tarball size mismatch for {name}: " +
                                     $"{size} versus {entry.size}.";
                            return false;
                        }

                        string hash = HashLimited(gzip, size);
                        SkipPadding(gzip, size);
                        if (!string.Equals(
                                hash,
                                entry.sha256,
                                StringComparison.OrdinalIgnoreCase))
                        {
                            detail = "Tarball checksum mismatch for " + name;
                            return false;
                        }
                        seen.Add(name);
                    }
                }

                List<string> missing = expected.Keys
                    .Where(value => !seen.Contains(value))
                    .ToList();
                if (missing.Count > 0)
                {
                    detail = "Tarball is missing: " + string.Join(", ", missing);
                    return false;
                }

                detail = $"Verified {seen.Count} files against the generated " +
                         $"SHA-256 manifest. Archive SHA-256: " +
                         HashFile(archivePath) + ".";
                return true;
            }
            catch (Exception exception)
            {
                detail = "Tarball verification failed: " + exception.Message;
                return false;
            }
        }

        private static string BuildPackageHashList(
            DeverQuestPackageManifest manifest)
        {
            StringBuilder builder = new StringBuilder();
            foreach (DeverQuestPackageManifestEntry entry in manifest.files)
            {
                builder.Append(entry.sha256);
                builder.Append("  package/");
                builder.AppendLine(entry.path);
            }
            return builder.ToString().TrimEnd();
        }

        private static string BuildOutputChecksums(
            string outputDirectory,
            string checksumPath)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string path in Directory.GetFiles(
                         outputDirectory,
                         "*",
                         SearchOption.TopDirectoryOnly)
                     .Where(value => !string.Equals(
                         value,
                         checksumPath,
                         StringComparison.OrdinalIgnoreCase))
                     .OrderBy(value => value, StringComparer.Ordinal))
            {
                builder.Append(HashFile(path));
                builder.Append("  ");
                builder.AppendLine(Path.GetFileName(path));
            }
            return builder.ToString().TrimEnd();
        }

        private static string BuildDistributionChecklist(
            DeverQuestDistributionChannel channel)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(
                $"# DeverQuest {ExpectedVersion} Distribution Checklist");
            builder.AppendLine();
            builder.AppendLine($"**Channel:** {ChannelDisplay(channel)}");
            builder.AppendLine();
            builder.AppendLine("- [ ] Source revision is committed and pushed.");
            builder.AppendLine("- [ ] Source revision is tagged.");
            builder.AppendLine("- [ ] Unity compiles with zero errors.");
            builder.AppendLine("- [ ] Release Readiness has no blockers.");
            builder.AppendLine("- [ ] Beta Content Health has no errors.");
            builder.AppendLine("- [ ] Deferred test matrices are acknowledged.");
            builder.AppendLine("- [ ] README and CHANGELOG match the package version.");
            builder.AppendLine("- [ ] Credits and third-party notices were reviewed.");
            builder.AppendLine("- [ ] No unlicensed media is bundled.");
            builder.AppendLine("- [ ] Software LICENSE was selected for public distribution.");
            builder.AppendLine("- [ ] Known limitations were reviewed.");
            builder.AppendLine("- [ ] Package manifest was generated.");
            builder.AppendLine("- [ ] Tarball verification passed.");
            builder.AppendLine("- [ ] SHA256SUMS.txt was preserved.");
            builder.AppendLine("- [ ] Release dossier was archived with the tag.");
            builder.AppendLine("- [ ] Clean-project installation was completed.");
            builder.AppendLine("- [ ] Upgrade installation was completed.");
            builder.AppendLine("- [ ] Final release verdict was recorded.");
            return builder.ToString().TrimEnd();
        }

        private static string ReadKnownLimitations(string packageRoot)
        {
            string path = Path.Combine(packageRoot, "KNOWN_LIMITATIONS.md");
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            return "# Known Limitations\n\nNo limitations file was found.";
        }

        private static DeverQuestDistributionJson BuildJson(
            DeverQuestDistributionReport report)
        {
            return new DeverQuestDistributionJson
            {
                packageVersion = ExpectedVersion,
                channel = report.channel.ToString(),
                generatedUtc = report.generatedUtc,
                verdict = report.Verdict,
                passes = report.PassCount,
                advisories = report.AdvisoryCount,
                blockers = report.BlockerCount,
                packageFiles = report.manifest == null
                    ? 0
                    : report.manifest.fileCount,
                packageBytes = report.manifest == null
                    ? 0L
                    : report.manifest.totalBytes,
                findings = report.findings.ToList()
            };
        }

        private static void RequireText(
            string root,
            string relativePath,
            string expectedText,
            List<string> mismatches)
        {
            string path = Path.Combine(
                root,
                relativePath.Replace('/', Path.DirectorySeparatorChar));
            string text = ReadText(path);
            if (text.IndexOf(expectedText, StringComparison.Ordinal) < 0)
            {
                mismatches.Add(relativePath);
            }
        }

        private static bool FindLicense(string root)
        {
            if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            {
                return false;
            }
            return new[] { "LICENSE", "LICENSE.md", "LICENSE.txt" }
                .Any(value => File.Exists(Path.Combine(root, value)));
        }

        private static string ReadMetaGuid(string path)
        {
            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("guid: ", StringComparison.Ordinal))
                {
                    return line.Substring(6).Trim();
                }
            }
            return string.Empty;
        }

        private static string ReadText(string path)
        {
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        private static string GetProjectRoot()
        {
            DirectoryInfo assets = Directory.GetParent(Application.dataPath);
            return assets == null
                ? Directory.GetCurrentDirectory()
                : assets.FullName;
        }

        private static string Relative(string root, string path)
        {
            string normalizedRoot = Path.GetFullPath(root)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) +
                Path.DirectorySeparatorChar;
            string normalizedPath = Path.GetFullPath(path);
            if (!normalizedPath.StartsWith(
                    normalizedRoot,
                    StringComparison.OrdinalIgnoreCase))
            {
                return normalizedPath.Replace('\\', '/');
            }
            return normalizedPath.Substring(normalizedRoot.Length)
                .Replace('\\', '/');
        }

        private static string HashFile(string path)
        {
            using (SHA256 sha = SHA256.Create())
            using (FileStream stream = File.OpenRead(path))
            {
                return ToHex(sha.ComputeHash(stream));
            }
        }

        private static string HashLimited(Stream stream, long size)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] buffer = new byte[81920];
                long remaining = size;
                while (remaining > 0)
                {
                    int count = (int)Math.Min(buffer.Length, remaining);
                    int read = stream.Read(buffer, 0, count);
                    if (read <= 0)
                    {
                        throw new EndOfStreamException(
                            "The tarball ended before the declared file size.");
                    }
                    sha.TransformBlock(buffer, 0, read, buffer, 0);
                    remaining -= read;
                }
                sha.TransformFinalBlock(new byte[0], 0, 0);
                return ToHex(sha.Hash);
            }
        }

        private static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes)
                .Replace("-", string.Empty)
                .ToLowerInvariant();
        }

        private static void WriteUtf8(string path, string text)
        {
            File.WriteAllText(path, text, new UTF8Encoding(false));
        }

        private static void WriteAscii(
            byte[] destination,
            int offset,
            int length,
            string value)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(value ?? string.Empty);
            int count = Math.Min(length, bytes.Length);
            Buffer.BlockCopy(bytes, 0, destination, offset, count);
        }

        private static void WriteOctal(
            byte[] destination,
            int offset,
            int length,
            long value)
        {
            string text = ToOctal(value);
            if (text.Length > length - 1)
            {
                throw new InvalidOperationException(
                    "A tar header value exceeded its field length.");
            }
            text = text.PadLeft(length - 1, '0');
            WriteAscii(destination, offset, length - 1, text);
            destination[offset + length - 1] = 0;
        }


        private static string ToOctal(long value)
        {
            if (value <= 0L)
            {
                return "0";
            }

            char[] buffer = new char[24];
            int index = buffer.Length;
            long remaining = value;
            while (remaining > 0L)
            {
                buffer[--index] = (char)('0' + (remaining & 7L));
                remaining >>= 3;
            }
            return new string(buffer, index, buffer.Length - index);
        }

        private static bool ReadBlock(Stream stream, byte[] buffer)
        {
            int offset = 0;
            while (offset < buffer.Length)
            {
                int read = stream.Read(
                    buffer,
                    offset,
                    buffer.Length - offset);
                if (read <= 0)
                {
                    return offset != 0;
                }
                offset += read;
            }
            return true;
        }

        private static string ReadAscii(
            byte[] source,
            int offset,
            int length)
        {
            int end = offset;
            int maximum = offset + length;
            while (end < maximum && source[end] != 0)
            {
                end++;
            }
            return Encoding.ASCII.GetString(source, offset, end - offset)
                .Trim();
        }

        private static long ReadOctal(
            byte[] source,
            int offset,
            int length)
        {
            string text = ReadAscii(source, offset, length).Trim();
            if (string.IsNullOrWhiteSpace(text))
            {
                return 0L;
            }
            return Convert.ToInt64(text, 8);
        }

        private static void Skip(Stream stream, long count)
        {
            byte[] buffer = new byte[81920];
            long remaining = count;
            while (remaining > 0)
            {
                int read = stream.Read(
                    buffer,
                    0,
                    (int)Math.Min(buffer.Length, remaining));
                if (read <= 0)
                {
                    throw new EndOfStreamException();
                }
                remaining -= read;
            }
        }

        private static void SkipPadding(Stream stream, long size)
        {
            long remainder = size % 512L;
            if (remainder != 0)
            {
                Skip(stream, 512L - remainder);
            }
        }

        private static string ChannelDisplay(
            DeverQuestDistributionChannel channel)
        {
            switch (channel)
            {
                case DeverQuestDistributionChannel.InternalBeta:
                    return "Internal Beta";
                case DeverQuestDistributionChannel.ReleaseCandidate:
                    return "Release Candidate";
                default:
                    return "External Beta";
            }
        }

        private static string ChannelSlug(
            DeverQuestDistributionChannel channel)
        {
            return ChannelDisplay(channel)
                .Replace(" ", "-")
                .ToLowerInvariant();
        }

        private static void Add(
            DeverQuestDistributionReport report,
            DeverQuestDistributionSeverity severity,
            string title,
            string detail)
        {
            report.findings.Add(new DeverQuestDistributionFinding
            {
                severity = severity.ToString(),
                title = title ?? string.Empty,
                detail = detail ?? string.Empty
            });
        }
    }
}

//----- DeverQuestDistributionService.cs END -----
