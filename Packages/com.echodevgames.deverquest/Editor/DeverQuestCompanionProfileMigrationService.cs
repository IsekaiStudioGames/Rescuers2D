//----- DeverQuestCompanionProfileMigrationService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    [InitializeOnLoad]
    internal static class DeverQuestCompanionProfileMigrationService
    {
        private const string PackageScriptPath =
            "Packages/com.echodevgames.deverquest/Runtime/" +
            "DeverQuestCompanionProfile.cs";
        private const string StableScriptGuid =
            "5ed871fb5feb42cb8086c26ad8c33791";
        private const string MigrationVersion = "0.32.3";
        private const string OriginalStarterProfilesRoot =
            "Assets/DeverQuest/Companions/OriginalStarter/Profiles/";

        private static readonly Regex ScriptReferencePattern =
            new Regex(
                @"m_Script:\s*\{fileID:\s*11500000,\s*guid:\s*" +
                @"[0-9a-fA-F]{32},\s*type:\s*3\}",
                RegexOptions.Compiled);

        static DeverQuestCompanionProfileMigrationService()
        {
            EditorApplication.delayCall += RunAutomaticMigration;
        }

        [MenuItem(
            "Tools/DeverQuest/QA/Repair Companion Profile Asset Scripts")]
        private static void RepairFromMenu()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Repair Companion Profile Assets?",
                "DeverQuest will inspect project-owned .asset files for " +
                "Companion Profile data whose script association is missing. " +
                "Only unmistakable Companion Profile YAML records are " +
                "changed. Backups are written under " +
                "Library/DeverQuest/Migrations.",
                "Repair",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            DeverQuestCompanionProfileMigrationReport report =
                RepairBrokenCompanionProfiles();

            if (report.repairedStarterProfiles)
            {
                TryRepopulateOriginalStarterCatalog(report);
            }

            EditorUtility.DisplayDialog(
                "Companion Profile Repair",
                report.BuildSummary(),
                "Close");
        }

        public static IReadOnlyList<string>
            FindBrokenCompanionProfilePaths()
        {
            List<string> paths = new List<string>();

            foreach (string assetPath in EnumerateProjectAssetPaths())
            {
                if (!LooksLikeCompanionProfile(assetPath))
                {
                    continue;
                }

                DeverQuestCompanionProfile loaded =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestCompanionProfile>(assetPath);
                if (loaded == null)
                {
                    paths.Add(assetPath);
                }
            }

            return paths;
        }

        public static DeverQuestCompanionProfileMigrationReport
            RepairBrokenCompanionProfiles()
        {
            DeverQuestCompanionProfileMigrationReport report =
                new DeverQuestCompanionProfileMigrationReport();

            string scriptGuid = ResolveScriptGuid();
            if (string.IsNullOrWhiteSpace(scriptGuid))
            {
                report.failures.Add(
                    "The DeverQuestCompanionProfile script GUID could not " +
                    "be resolved from the installed package.");
                return report;
            }

            foreach (string assetPath in EnumerateProjectAssetPaths())
            {
                if (!LooksLikeCompanionProfile(assetPath))
                {
                    continue;
                }

                report.inspected++;
                if (AssetDatabase.LoadAssetAtPath<DeverQuestCompanionProfile>(
                        assetPath) != null)
                {
                    report.alreadyValid++;
                    continue;
                }

                if (TryRepairAsset(assetPath, scriptGuid, out string error))
                {
                    report.repairedPaths.Add(assetPath);
                    if (assetPath.StartsWith(
                            OriginalStarterProfilesRoot,
                            StringComparison.Ordinal))
                    {
                        report.repairedStarterProfiles = true;
                    }
                }
                else
                {
                    report.failures.Add(assetPath + ": " + error);
                }
            }

            AssetDatabase.SaveAssets();
            return report;
        }

        private static void RunAutomaticMigration()
        {
            if (EditorApplication.isCompiling ||
                EditorApplication.isUpdating)
            {
                EditorApplication.delayCall += RunAutomaticMigration;
                return;
            }

            string projectKey = Hash128.Compute(
                Application.dataPath ?? string.Empty).ToString();
            string preferenceKey =
                "EchoDevGames.DeverQuest.CompanionProfileMigration." +
                MigrationVersion + "." + projectKey;

            if (EditorPrefs.GetBool(preferenceKey, false))
            {
                return;
            }

            DeverQuestCompanionProfileMigrationReport report =
                RepairBrokenCompanionProfiles();

            if (report.repairedStarterProfiles)
            {
                TryRepopulateOriginalStarterCatalog(report);
            }

            if (report.failures.Count == 0)
            {
                EditorPrefs.SetBool(preferenceKey, true);
            }

            if (report.repairedPaths.Count > 0)
            {
                Debug.LogWarning(
                    "[DeverQuest " + MigrationVersion + "] Repaired " +
                    report.repairedPaths.Count +
                    " Companion Profile script association(s):\n" +
                    string.Join("\n", report.repairedPaths));
            }

            if (!string.IsNullOrWhiteSpace(report.catalogMessage))
            {
                Debug.Log(
                    "[DeverQuest " + MigrationVersion + "] " +
                    report.catalogMessage);
            }

            if (report.failures.Count > 0)
            {
                Debug.LogError(
                    "[DeverQuest " + MigrationVersion + "] Companion " +
                    "Profile migration encountered failures:\n" +
                    string.Join("\n", report.failures));
            }
        }

        private static void TryRepopulateOriginalStarterCatalog(
            DeverQuestCompanionProfileMigrationReport report)
        {
            try
            {
                DeverQuestCompanionGenerationReport generation =
                    DeverQuestCompanionCatalogGenerator
                        .GenerateOriginalStarterCatalog();
                report.catalogMessage = generation.Summary;
            }
            catch (Exception exception)
            {
                report.failures.Add(
                    "Original Companion Stable could not be repopulated: " +
                    exception.Message);
            }
        }

        private static bool TryRepairAsset(
            string assetPath,
            string scriptGuid,
            out string error)
        {
            error = string.Empty;
            string absolutePath = ToAbsolutePath(assetPath);
            if (!File.Exists(absolutePath))
            {
                error = "Asset file does not exist on disk.";
                return false;
            }

            string originalText;
            try
            {
                originalText = File.ReadAllText(absolutePath);
            }
            catch (Exception exception)
            {
                error = "Could not read asset: " + exception.Message;
                return false;
            }

            if (!HasCompanionFingerprint(originalText))
            {
                error =
                    "Asset no longer matches the Companion Profile schema.";
                return false;
            }

            Match match = ScriptReferencePattern.Match(originalText);
            if (!match.Success)
            {
                error = "The serialized m_Script reference was not found.";
                return false;
            }

            string replacement =
                "m_Script: {fileID: 11500000, guid: " + scriptGuid +
                ", type: 3}";
            string repairedText =
                ScriptReferencePattern.Replace(
                    originalText,
                    replacement,
                    1);

            string backupPath = BuildBackupPath(assetPath);
            try
            {
                Directory.CreateDirectory(
                    Path.GetDirectoryName(backupPath) ?? string.Empty);
                File.WriteAllText(backupPath, originalText);
                File.WriteAllText(absolutePath, repairedText);
                AssetDatabase.ImportAsset(
                    assetPath,
                    ImportAssetOptions.ForceUpdate);
            }
            catch (Exception exception)
            {
                TryRestore(absolutePath, originalText, assetPath);
                error = "Could not rewrite asset: " + exception.Message;
                return false;
            }

            if (AssetDatabase.LoadAssetAtPath<DeverQuestCompanionProfile>(
                    assetPath) != null)
            {
                return true;
            }

            TryRestore(absolutePath, originalText, assetPath);
            error =
                "Unity still could not load the repaired Companion Profile.";
            return false;
        }

        private static void TryRestore(
            string absolutePath,
            string originalText,
            string assetPath)
        {
            try
            {
                File.WriteAllText(absolutePath, originalText);
                AssetDatabase.ImportAsset(
                    assetPath,
                    ImportAssetOptions.ForceUpdate);
            }
            catch
            {
                // The original repair failure is reported by the caller.
            }
        }

        private static IEnumerable<string> EnumerateProjectAssetPaths()
        {
            return AssetDatabase.GetAllAssetPaths()
                .Where(path =>
                    path.StartsWith("Assets/", StringComparison.Ordinal) &&
                    path.EndsWith(
                        ".asset",
                        StringComparison.OrdinalIgnoreCase));
        }

        private static bool LooksLikeCompanionProfile(string assetPath)
        {
            string absolutePath = ToAbsolutePath(assetPath);
            if (!File.Exists(absolutePath))
            {
                return false;
            }

            try
            {
                return HasCompanionFingerprint(
                    File.ReadAllText(absolutePath));
            }
            catch
            {
                return false;
            }
        }

        private static bool HasCompanionFingerprint(string text)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   text.Contains("companionId:") &&
                   text.Contains("requiresCompanionClass:") &&
                   text.Contains("startingLoyalty:") &&
                   text.Contains("recruitCopperCost:") &&
                   text.Contains("recoveryCopperCost:");
        }

        private static string ResolveScriptGuid()
        {
            string guid = AssetDatabase.AssetPathToGUID(PackageScriptPath);
            return string.IsNullOrWhiteSpace(guid)
                ? StableScriptGuid
                : guid;
        }

        private static string ToAbsolutePath(string assetPath)
        {
            string projectRoot = Directory.GetParent(
                Application.dataPath)?.FullName ?? string.Empty;
            return Path.Combine(
                projectRoot,
                assetPath.Replace('/', Path.DirectorySeparatorChar));
        }

        private static string BuildBackupPath(string assetPath)
        {
            string projectRoot = Directory.GetParent(
                Application.dataPath)?.FullName ?? string.Empty;
            string relative = assetPath.Replace(
                '/',
                Path.DirectorySeparatorChar);
            return Path.Combine(
                projectRoot,
                "Library",
                "DeverQuest",
                "Migrations",
                MigrationVersion,
                relative + ".bak");
        }
    }

    internal sealed class DeverQuestCompanionProfileMigrationReport
    {
        public int inspected;
        public int alreadyValid;
        public bool repairedStarterProfiles;
        public string catalogMessage = string.Empty;
        public readonly List<string> repairedPaths =
            new List<string>();
        public readonly List<string> failures =
            new List<string>();

        public string BuildSummary()
        {
            return "Inspected: " + inspected + "\n" +
                   "Already valid: " + alreadyValid + "\n" +
                   "Repaired: " + repairedPaths.Count + "\n" +
                   "Failures: " + failures.Count +
                   (string.IsNullOrWhiteSpace(catalogMessage)
                       ? string.Empty
                       : "\n\n" + catalogMessage) +
                   (failures.Count == 0
                       ? string.Empty
                       : "\n\n" + string.Join("\n", failures));
        }
    }
}

//----- DeverQuestCompanionProfileMigrationService.cs END -----
