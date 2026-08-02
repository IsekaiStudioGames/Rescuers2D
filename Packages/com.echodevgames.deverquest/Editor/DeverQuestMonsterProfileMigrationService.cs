//----- DeverQuestMonsterProfileMigrationService.cs START -----

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
    internal static class DeverQuestMonsterProfileMigrationService
    {
        private const string PackageScriptPath =
            "Packages/com.echodevgames.deverquest/Runtime/" +
            "DeverQuestMonsterProfile.cs";
        private const string StableScriptGuid =
            "0e56bfa6a5c3488c846b2a1cd9ce2365";
        private const string MigrationVersion = "0.32.2";

        private static readonly Regex ScriptReferencePattern =
            new Regex(
                @"m_Script:\s*\{fileID:\s*11500000,\s*guid:\s*" +
                @"[0-9a-fA-F]{32},\s*type:\s*3\}",
                RegexOptions.Compiled);

        static DeverQuestMonsterProfileMigrationService()
        {
            EditorApplication.delayCall += RunAutomaticMigration;
        }

        [MenuItem(
            "Tools/DeverQuest/QA/Repair Monster Profile Asset Scripts")]
        private static void RepairFromMenu()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Repair Monster Profile Assets?",
                "DeverQuest will inspect project-owned .asset files for " +
                "Monster Profile data whose script association is missing. " +
                "Only unmistakable Monster Profile YAML records are changed. " +
                "Backups are written under Library/DeverQuest/Migrations.",
                "Repair",
                "Cancel");

            if (!confirmed)
            {
                return;
            }

            DeverQuestMonsterProfileMigrationReport report =
                RepairBrokenMonsterProfiles();

            EditorUtility.DisplayDialog(
                "Monster Profile Repair",
                report.BuildSummary(),
                "Close");
        }

        public static IReadOnlyList<string> FindBrokenMonsterProfilePaths()
        {
            List<string> paths = new List<string>();

            foreach (string assetPath in EnumerateProjectAssetPaths())
            {
                if (!LooksLikeMonsterProfile(assetPath))
                {
                    continue;
                }

                DeverQuestMonsterProfile loaded =
                    AssetDatabase.LoadAssetAtPath<
                        DeverQuestMonsterProfile>(assetPath);
                if (loaded == null)
                {
                    paths.Add(assetPath);
                }
            }

            return paths;
        }

        public static DeverQuestMonsterProfileMigrationReport
            RepairBrokenMonsterProfiles()
        {
            DeverQuestMonsterProfileMigrationReport report =
                new DeverQuestMonsterProfileMigrationReport();

            string scriptGuid = ResolveScriptGuid();
            if (string.IsNullOrWhiteSpace(scriptGuid))
            {
                report.failures.Add(
                    "The DeverQuestMonsterProfile script GUID could not be " +
                    "resolved from the installed package.");
                return report;
            }

            foreach (string assetPath in EnumerateProjectAssetPaths())
            {
                if (!LooksLikeMonsterProfile(assetPath))
                {
                    continue;
                }

                report.inspected++;
                if (AssetDatabase.LoadAssetAtPath<DeverQuestMonsterProfile>(
                        assetPath) != null)
                {
                    report.alreadyValid++;
                    continue;
                }

                if (TryRepairAsset(assetPath, scriptGuid, out string error))
                {
                    report.repairedPaths.Add(assetPath);
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
                "EchoDevGames.DeverQuest.MonsterProfileMigration." +
                MigrationVersion + "." + projectKey;

            if (EditorPrefs.GetBool(preferenceKey, false))
            {
                return;
            }

            DeverQuestMonsterProfileMigrationReport report =
                RepairBrokenMonsterProfiles();

            if (report.failures.Count == 0)
            {
                EditorPrefs.SetBool(preferenceKey, true);
            }

            if (report.repairedPaths.Count > 0)
            {
                Debug.LogWarning(
                    "[DeverQuest " + MigrationVersion + "] Repaired " +
                    report.repairedPaths.Count +
                    " Monster Profile script association(s):\n" +
                    string.Join("\n", report.repairedPaths));
            }

            if (report.failures.Count > 0)
            {
                Debug.LogError(
                    "[DeverQuest " + MigrationVersion + "] Monster Profile " +
                    "migration encountered failures:\n" +
                    string.Join("\n", report.failures));
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

            if (!HasMonsterFingerprint(originalText))
            {
                error = "Asset no longer matches the Monster Profile schema.";
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

            if (AssetDatabase.LoadAssetAtPath<DeverQuestMonsterProfile>(
                    assetPath) != null)
            {
                return true;
            }

            TryRestore(absolutePath, originalText, assetPath);
            error = "Unity still could not load the repaired Monster Profile.";
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
                    path.EndsWith(".asset", StringComparison.OrdinalIgnoreCase));
        }

        private static bool LooksLikeMonsterProfile(string assetPath)
        {
            string absolutePath = ToAbsolutePath(assetPath);
            if (!File.Exists(absolutePath))
            {
                return false;
            }

            try
            {
                return HasMonsterFingerprint(
                    File.ReadAllText(absolutePath));
            }
            catch
            {
                return false;
            }
        }

        private static bool HasMonsterFingerprint(string text)
        {
            return !string.IsNullOrWhiteSpace(text) &&
                   text.Contains("monsterId:") &&
                   text.Contains("maximumHitPoints:") &&
                   text.Contains("attackModifier:") &&
                   text.Contains("victoryCopper:") &&
                   text.Contains("dropTable:");
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

    internal sealed class DeverQuestMonsterProfileMigrationReport
    {
        public int inspected;
        public int alreadyValid;
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
                   (failures.Count == 0
                       ? string.Empty
                       : "\n\n" + string.Join("\n", failures));
        }
    }
}

//----- DeverQuestMonsterProfileMigrationService.cs END -----
