//----- DeverQuestContentValidationService.cs START -----

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestContentFindingSeverity
    {
        Info = 0,
        Warning = 1,
        Error = 2
    }

    internal sealed class DeverQuestContentFinding
    {
        public DeverQuestContentFindingSeverity severity;
        public string code = string.Empty;
        public string title = string.Empty;
        public string detail = string.Empty;
        public string assetPath = string.Empty;
        public UnityEngine.Object asset;
        public bool safelyRepairable;
    }

    internal sealed class DeverQuestContentValidationReport
    {
        public string generatedUtc = string.Empty;
        public int scannedAssets;
        public readonly List<DeverQuestContentFinding> findings =
            new List<DeverQuestContentFinding>();

        public int ErrorCount => findings.Count(value =>
            value.severity == DeverQuestContentFindingSeverity.Error);
        public int WarningCount => findings.Count(value =>
            value.severity == DeverQuestContentFindingSeverity.Warning);
        public int InfoCount => findings.Count(value =>
            value.severity == DeverQuestContentFindingSeverity.Info);
        public int RepairableCount => findings.Count(value =>
            value.safelyRepairable);
        public bool HasBlockingFindings => ErrorCount > 0;
        public string Summary =>
            $"{scannedAssets} assets scanned · {ErrorCount} errors · " +
            $"{WarningCount} warnings · {InfoCount} notes";
    }

    [Serializable]
    internal sealed class DeverQuestContentHealthJson
    {
        public string packageVersion = "0.32.5";
        public string generatedUtc = string.Empty;
        public int scannedAssets;
        public int errors;
        public int warnings;
        public int notes;
        public List<DeverQuestContentHealthJsonFinding> findings =
            new List<DeverQuestContentHealthJsonFinding>();
    }

    [Serializable]
    internal sealed class DeverQuestContentHealthJsonFinding
    {
        public string severity = string.Empty;
        public string code = string.Empty;
        public string title = string.Empty;
        public string detail = string.Empty;
        public string assetPath = string.Empty;
        public bool safelyRepairable;
    }

    internal static class DeverQuestContentValidationService
    {
        private const string ReportFolder = "DeverQuestBetaReports";

        [MenuItem("Tools/DeverQuest/Administration/Run Content Validation")]
        private static void RunFromMenu()
        {
            DeverQuestContentValidationReport report = Run();
            string text = BuildMarkdown(report);
            if (report.ErrorCount > 0)
            {
                Debug.LogError(text);
            }
            else if (report.WarningCount > 0)
            {
                Debug.LogWarning(text);
            }
            else
            {
                Debug.Log(text);
            }
            EditorUtility.DisplayDialog(
                "DeverQuest Content Validation",
                report.Summary + "\n\nOpen Beta Administration for details.",
                "Close");
        }

        public static DeverQuestContentValidationReport Run()
        {
            DeverQuestContentValidationReport report =
                new DeverQuestContentValidationReport
                {
                    generatedUtc = DateTime.UtcNow.ToString("o")
                };

            List<DeverQuestQuestProfile> profiles =
                LoadAssets<DeverQuestQuestProfile>();
            List<DeverQuestQuestContract> contracts =
                LoadAssets<DeverQuestQuestContract>();
            List<DeverQuestIdentityCatalog> identityCatalogs =
                LoadAssets<DeverQuestIdentityCatalog>();
            List<DeverQuestAncestry> ancestries =
                LoadAssets<DeverQuestAncestry>();
            List<DeverQuestClassDefinition> classes =
                LoadAssets<DeverQuestClassDefinition>();
            List<DeverQuestDeity> faiths =
                LoadAssets<DeverQuestDeity>();
            List<DeverQuestCompanionCatalog> companionCatalogs =
                LoadAssets<DeverQuestCompanionCatalog>();
            List<DeverQuestCompanionProfile> companions =
                LoadAssets<DeverQuestCompanionProfile>();
            List<DeverQuestEncounterProfile> encounters =
                LoadAssets<DeverQuestEncounterProfile>();
            List<DeverQuestMonsterProfile> monsters =
                LoadAssets<DeverQuestMonsterProfile>();
            List<DeverQuestShopProfile> shops =
                LoadAssets<DeverQuestShopProfile>();
            List<DeverQuestShopItem> items =
                LoadAssets<DeverQuestShopItem>();
            List<DeverQuestPlaylist> playlists =
                LoadAssets<DeverQuestPlaylist>();
            List<DeverQuestAmbienceProfile> ambienceProfiles =
                LoadAssets<DeverQuestAmbienceProfile>();
            List<DeverQuestWarningAudioProfile> warningProfiles =
                LoadAssets<DeverQuestWarningAudioProfile>();
            List<DeverQuestStarterLoadout> loadouts =
                LoadAssets<DeverQuestStarterLoadout>();

            report.scannedAssets = profiles.Count + contracts.Count +
                identityCatalogs.Count + ancestries.Count + classes.Count +
                faiths.Count + companionCatalogs.Count + companions.Count +
                encounters.Count + monsters.Count + shops.Count + items.Count +
                playlists.Count + ambienceProfiles.Count +
                warningProfiles.Count + loadouts.Count;

            ValidateProfiles(report, profiles);
            ValidateContracts(report, contracts);
            ValidateIdentity(report, identityCatalogs, ancestries, classes, faiths);
            ValidateCompanions(report, companionCatalogs, companions);
            ValidateEncounters(report, encounters, monsters);
            ValidateEconomy(report, shops, items);
            ValidateAudio(report, playlists, ambienceProfiles, warningProfiles);
            ValidateLoadouts(report, loadouts);

            if (report.findings.Count == 0)
            {
                Add(report, DeverQuestContentFindingSeverity.Info,
                    "DQ-CONTENT-000", "Content health",
                    "No broken references or incomplete production assets were found.",
                    null, false);
            }

            return report;
        }

        public static int RepairSafeIssues()
        {
            int changed = 0;
            foreach (DeverQuestQuestContract contract in
                     LoadAssets<DeverQuestQuestContract>())
            {
                bool dirty = false;
                if (string.IsNullOrWhiteSpace(contract.contractTitle) &&
                    contract.questProfile != null)
                {
                    contract.contractTitle = contract.questProfile.displayName;
                    dirty = true;
                }
                if (string.IsNullOrWhiteSpace(contract.projectName) &&
                    contract.questProfile != null)
                {
                    contract.projectName = contract.questProfile.projectName;
                    dirty = true;
                }
                if (string.IsNullOrWhiteSpace(contract.taskName) &&
                    contract.questProfile != null)
                {
                    contract.taskName = contract.questProfile.taskName;
                    dirty = true;
                }
                if (string.IsNullOrWhiteSpace(contract.objective) &&
                    contract.questProfile != null)
                {
                    contract.objective = contract.questProfile.goalTemplate;
                    dirty = true;
                }
                if (!contract.SpoilsMatchLinkedProfile() &&
                    contract.CanRefreshSpoilsFromProfile())
                {
                    contract.RefreshSpoilsFromProfile();
                    dirty = true;
                }
                contract.focusStages = contract.focusStages ??
                    new List<DeverQuestFocusStage>();
                contract.focusStages.RemoveAll(value => value == null);
                for (int index = 0; index < contract.focusStages.Count; index++)
                {
                    DeverQuestFocusStage stage = contract.focusStages[index];
                    string before = stage.stageTitle;
                    stage.Sanitize();
                    if (before != stage.stageTitle)
                    {
                        dirty = true;
                    }
                }
                if (dirty)
                {
                    EditorUtility.SetDirty(contract);
                    changed++;
                }
            }

            foreach (DeverQuestIdentityCatalog catalog in
                     LoadAssets<DeverQuestIdentityCatalog>())
            {
                bool dirty = false;
                catalog.ancestries = catalog.ancestries ??
                    new List<DeverQuestAncestry>();
                catalog.classes = catalog.classes ??
                    new List<DeverQuestClassDefinition>();
                catalog.faiths = catalog.faiths ??
                    new List<DeverQuestDeity>();
                int before = catalog.ancestries.Count + catalog.classes.Count +
                    catalog.faiths.Count;
                catalog.ancestries.RemoveAll(value => value == null);
                catalog.classes.RemoveAll(value => value == null);
                catalog.faiths.RemoveAll(value => value == null);
                dirty |= before != catalog.ancestries.Count + catalog.classes.Count +
                    catalog.faiths.Count;
                if (catalog.defaultAncestry == null && catalog.ancestries.Count > 0)
                {
                    catalog.defaultAncestry = catalog.ancestries[0];
                    dirty = true;
                }
                if (catalog.defaultClass == null && catalog.classes.Count > 0)
                {
                    catalog.defaultClass = catalog.classes[0];
                    dirty = true;
                }
                if (catalog.defaultFaith == null && catalog.faiths.Count > 0)
                {
                    catalog.defaultFaith = catalog.faiths[0];
                    dirty = true;
                }
                if (dirty)
                {
                    EditorUtility.SetDirty(catalog);
                    changed++;
                }
            }

            foreach (DeverQuestCompanionCatalog catalog in
                     LoadAssets<DeverQuestCompanionCatalog>())
            {
                int before = catalog.companions == null ? 0 : catalog.companions.Count;
                catalog.companions = catalog.companions ??
                    new List<DeverQuestCompanionProfile>();
                catalog.companions.RemoveAll(value => value == null);
                if (before != catalog.companions.Count)
                {
                    EditorUtility.SetDirty(catalog);
                    changed++;
                }
            }

            foreach (DeverQuestShopProfile shop in LoadAssets<DeverQuestShopProfile>())
            {
                int before = shop.items == null ? 0 : shop.items.Count;
                shop.items = shop.items ?? new List<DeverQuestShopItem>();
                shop.items.RemoveAll(value => value == null);
                if (before != shop.items.Count)
                {
                    EditorUtility.SetDirty(shop);
                    changed++;
                }
            }

            AssetDatabase.SaveAssets();
            return changed;
        }

        public static bool CanRegenerateStableId(
            UnityEngine.Object asset)
        {
            return asset is DeverQuestQuestProfile ||
                   asset is DeverQuestIdentityAsset;
        }

        public static bool RegenerateStableId(
            UnityEngine.Object asset,
            out string previousId,
            out string replacementId,
            out string error)
        {
            previousId = string.Empty;
            replacementId = string.Empty;
            error = string.Empty;

            if (!CanRegenerateStableId(asset))
            {
                error =
                    "This asset type does not expose an approved explicit " +
                    "stable-ID repair.";
                return false;
            }

            string propertyName =
                asset is DeverQuestQuestProfile
                    ? "profileId"
                    : "identityId";
            SerializedObject serialized =
                new SerializedObject(asset);
            serialized.Update();
            SerializedProperty idProperty =
                serialized.FindProperty(propertyName);
            if (idProperty == null)
            {
                error =
                    $"The serialized ID field '{propertyName}' could not " +
                    "be found.";
                return false;
            }

            previousId = idProperty.stringValue?.Trim() ??
                         string.Empty;
            replacementId = Guid.NewGuid().ToString("N");

            Undo.RecordObject(
                asset,
                "Regenerate DeverQuest Stable ID");
            idProperty.stringValue = replacementId;
            serialized.ApplyModifiedProperties();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(asset),
                ImportAssetOptions.ForceUpdate);
            return true;
        }

        public static List<UnityEngine.Object>
            FindDuplicateStableIdAssets(UnityEngine.Object asset)
        {
            List<UnityEngine.Object> results =
                new List<UnityEngine.Object>();
            if (!CanRegenerateStableId(asset))
            {
                return results;
            }

            string propertyName =
                asset is DeverQuestQuestProfile
                    ? "profileId"
                    : "identityId";
            string id = ReadSerializedId(asset, propertyName);
            if (string.IsNullOrWhiteSpace(id))
            {
                return results;
            }

            if (asset is DeverQuestQuestProfile)
            {
                results.AddRange(
                    LoadAssets<DeverQuestQuestProfile>()
                        .Where(value =>
                            string.Equals(
                                ReadSerializedId(
                                    value,
                                    propertyName),
                                id,
                                StringComparison.OrdinalIgnoreCase))
                        .Cast<UnityEngine.Object>());
                return results;
            }

            List<DeverQuestIdentityAsset> identities =
                new List<DeverQuestIdentityAsset>();
            identities.AddRange(LoadAssets<DeverQuestAncestry>());
            identities.AddRange(
                LoadAssets<DeverQuestClassDefinition>());
            identities.AddRange(LoadAssets<DeverQuestDeity>());
            results.AddRange(
                identities
                    .Where(value =>
                        string.Equals(
                            ReadSerializedId(
                                value,
                                propertyName),
                            id,
                            StringComparison.OrdinalIgnoreCase))
                    .Cast<UnityEngine.Object>());
            return results;
        }

        public static bool RegenerateDuplicateIdsKeeping(
            UnityEngine.Object keeper,
            out string summary,
            out string error)
        {
            summary = string.Empty;
            error = string.Empty;
            if (!CanRegenerateStableId(keeper))
            {
                error =
                    "The selected asset does not support stable-ID repair.";
                return false;
            }

            List<UnityEngine.Object> duplicates =
                FindDuplicateStableIdAssets(keeper);
            if (duplicates.Count <= 1)
            {
                error =
                    "No duplicate-ID group was found for the selected asset.";
                return false;
            }

            List<string> changes = new List<string>();
            foreach (UnityEngine.Object duplicate in duplicates)
            {
                if (duplicate == null || duplicate == keeper)
                {
                    continue;
                }

                if (!RegenerateStableId(
                        duplicate,
                        out string previousId,
                        out string replacementId,
                        out string repairError))
                {
                    error =
                        "Could not repair " +
                        AssetDatabase.GetAssetPath(duplicate) +
                        ": " +
                        repairError;
                    return false;
                }

                changes.Add(
                    AssetDatabase.GetAssetPath(duplicate) +
                    " · " +
                    previousId +
                    " → " +
                    replacementId);
            }

            summary =
                "Kept the stable ID on " +
                AssetDatabase.GetAssetPath(keeper) +
                " and regenerated " +
                changes.Count +
                " duplicate copy/copies." +
                (changes.Count == 0
                    ? string.Empty
                    : "\n" + string.Join("\n", changes));
            return true;
        }

        public static string RunSafeStarterRepairs()
        {
            DeverQuestIdentityGenerationReport identity =
                DeverQuestIdentityCatalogGenerator.GenerateOriginalStarterCatalog();
            DeverQuestCompanionGenerationReport companion =
                DeverQuestCompanionCatalogGenerator.GenerateOriginalStarterCatalog();
            DeverQuestTacticalContentReport tactical =
                DeverQuestTacticalContentGenerator.GenerateStarterKit();
            DeverQuestStarterContentGenerator.GenerateCombatCodex();
            int gear = DeverQuestStarterContentGenerator.GenerateBasicGear();
            DeverQuestShopProfile shop =
                DeverQuestStarterContentGenerator.GenerateBasicShop();
            DeverQuestStarterContentGenerator.GenerateTrainingEncounter();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return identity.Summary + " " + companion.Summary + " " +
                   $"Tactical content: {tactical.Created} created, " +
                   $"{tactical.Updated} updated. Basic gear created: {gear}. " +
                   $"Quartermaster: {(shop == null ? "unavailable" : shop.displayName)}.";
        }

        public static string ExportMarkdown(DeverQuestContentValidationReport report)
        {
            return WriteReport("md", BuildMarkdown(report));
        }

        public static string ExportJson(DeverQuestContentValidationReport report)
        {
            DeverQuestContentHealthJson data = BuildJson(report);
            return WriteReport("json", JsonUtility.ToJson(data, true));
        }

        public static string BuildMarkdown(DeverQuestContentValidationReport report)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("# DeverQuest 0.32.5 Beta Content Health Report");
            builder.AppendLine();
            builder.AppendLine($"**Generated UTC:** {report.generatedUtc}");
            builder.AppendLine($"**Summary:** {report.Summary}");
            builder.AppendLine();
            foreach (DeverQuestContentFinding finding in report.findings
                         .OrderByDescending(value => value.severity)
                         .ThenBy(value => value.assetPath))
            {
                builder.AppendLine($"## [{finding.severity.ToString().ToUpperInvariant()}] {finding.code} — {finding.title}");
                builder.AppendLine();
                builder.AppendLine(finding.detail);
                if (!string.IsNullOrWhiteSpace(finding.assetPath))
                {
                    builder.AppendLine();
                    builder.AppendLine($"- Asset: `{finding.assetPath}`");
                }
                if (finding.safelyRepairable)
                {
                    builder.AppendLine("- Safe repair available: Yes");
                }
                builder.AppendLine();
            }
            return builder.ToString().TrimEnd();
        }

        private static DeverQuestContentHealthJson BuildJson(
            DeverQuestContentValidationReport report)
        {
            DeverQuestContentHealthJson data = new DeverQuestContentHealthJson
            {
                generatedUtc = report.generatedUtc,
                scannedAssets = report.scannedAssets,
                errors = report.ErrorCount,
                warnings = report.WarningCount,
                notes = report.InfoCount
            };
            foreach (DeverQuestContentFinding finding in report.findings)
            {
                data.findings.Add(new DeverQuestContentHealthJsonFinding
                {
                    severity = finding.severity.ToString(),
                    code = finding.code,
                    title = finding.title,
                    detail = finding.detail,
                    assetPath = finding.assetPath,
                    safelyRepairable = finding.safelyRepairable
                });
            }
            return data;
        }

        private static string WriteReport(string extension, string content)
        {
            string projectRoot = Directory.GetParent(Application.dataPath)?.FullName ??
                                 Directory.GetCurrentDirectory();
            string folder = Path.Combine(projectRoot, ReportFolder);
            Directory.CreateDirectory(folder);
            string stamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string path = Path.Combine(folder,
                $"DeverQuest_Beta_Health_{stamp}.{extension}");
            File.WriteAllText(path, content, Encoding.UTF8);
            return path;
        }

        private static void ValidateProfiles(
            DeverQuestContentValidationReport report,
            List<DeverQuestQuestProfile> profiles)
        {
            ValidateDuplicateIds(report, profiles, value => ReadSerializedId(value, "profileId"),
                "DQ-CONTENT-101", "Duplicate Quest Profile ID");
            foreach (DeverQuestQuestProfile profile in profiles)
            {
                if (string.IsNullOrWhiteSpace(profile.displayName))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-102", "Quest Profile has no display name",
                        "Add a readable display name before offering Contracts.",
                        profile, false);
                }
                if (string.IsNullOrWhiteSpace(profile.projectName) ||
                    string.IsNullOrWhiteSpace(profile.taskName))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-103", "Quest Profile is incomplete",
                        "Project Name and Task Name should both be populated.",
                        profile, false);
                }
            }
        }

        private static void ValidateContracts(
            DeverQuestContentValidationReport report,
            List<DeverQuestQuestContract> contracts)
        {
            ValidateDuplicateIds(report, contracts, value => ReadSerializedId(value, "contractId"),
                "DQ-CONTENT-201", "Duplicate Quest Contract ID");
            HashSet<string> runIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            HashSet<string> completionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DeverQuestQuestContract contract in contracts)
            {
                if (string.IsNullOrWhiteSpace(contract.contractTitle) ||
                    string.IsNullOrWhiteSpace(contract.projectName) ||
                    string.IsNullOrWhiteSpace(contract.taskName))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-202", "Quest Contract is incomplete",
                        "Contract Title, Project, and Task should be populated before offering it.",
                        contract, contract.questProfile != null);
                }
                if (contract.questProfile == null)
                {
                    Add(report, DeverQuestContentFindingSeverity.Info,
                        "DQ-CONTENT-203", "Contract has no linked Quest Profile",
                        "This is valid for a fully hand-authored Contract, but it cannot refresh template rewards.",
                        contract, false);
                }
                else if (!contract.SpoilsMatchLinkedProfile())
                {
                    Add(report,
                        contract.CanRefreshSpoilsFromProfile()
                            ? DeverQuestContentFindingSeverity.Warning
                            : DeverQuestContentFindingSeverity.Info,
                        "DQ-CONTENT-204", "Contract reward snapshot differs",
                        contract.CanRefreshSpoilsFromProfile()
                            ? "The editable Contract can safely refresh its reward snapshot from the linked profile."
                            : "The Contract is locked; the difference is preserved intentionally.",
                        contract, contract.CanRefreshSpoilsFromProfile());
                }
                if (contract.archived &&
                    ((contract.activeRuns?.Count ?? 0) > 0 ||
                     (contract.partyMembers?.Count ?? 0) > 0))
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        "DQ-CONTENT-205", "Archived Contract has active reservations",
                        "Restore the listing or clear the active run/party reservation before distribution.",
                        contract, false);
                }
                if (contract.focusStages != null)
                {
                    foreach (DeverQuestFocusStage stage in contract.focusStages)
                    {
                        if (stage == null)
                        {
                            Add(report, DeverQuestContentFindingSeverity.Warning,
                                "DQ-CONTENT-206", "Contract contains a null Encounter",
                                "Remove the empty Encounter entry.", contract, true);
                        }
                        else if (stage.encounterProfile == null &&
                                 !string.IsNullOrWhiteSpace(stage.encounterProfileId))
                        {
                            Add(report, DeverQuestContentFindingSeverity.Warning,
                                "DQ-CONTENT-207", "Encounter reference cannot be resolved",
                                $"Encounter '{stage.stageTitle}' stores ID {stage.encounterProfileId} but no asset reference.",
                                contract, false);
                        }
                    }
                }
                foreach (DeverQuestContractRunReservation run in
                         contract.activeRuns ?? new List<DeverQuestContractRunReservation>())
                {
                    if (run == null || string.IsNullOrWhiteSpace(run.runId))
                    {
                        Add(report, DeverQuestContentFindingSeverity.Error,
                            "DQ-CONTENT-208", "Active Quest Run has no Run ID",
                            "Cancel the invalid reservation before using the Contract.",
                            contract, false);
                    }
                    else if (!runIds.Add(run.runId))
                    {
                        Add(report, DeverQuestContentFindingSeverity.Error,
                            "DQ-CONTENT-209", "Duplicate Quest Run ID",
                            $"Run ID {run.runId} appears more than once across Contracts.",
                            contract, false);
                    }
                }
                foreach (DeverQuestContractCompletionRecord completion in
                         contract.completionHistory ?? new List<DeverQuestContractCompletionRecord>())
                {
                    if (completion == null || string.IsNullOrWhiteSpace(completion.completionId))
                    {
                        Add(report, DeverQuestContentFindingSeverity.Error,
                            "DQ-CONTENT-210", "Completion record has no ID",
                            "The Contract contains an invalid completion-history record.",
                            contract, false);
                    }
                    else if (!completionIds.Add(completion.completionId))
                    {
                        Add(report, DeverQuestContentFindingSeverity.Error,
                            "DQ-CONTENT-211", "Duplicate completion record ID",
                            $"Completion ID {completion.completionId} appears more than once.",
                            contract, false);
                    }
                }
            }
        }

        private static void ValidateIdentity(
            DeverQuestContentValidationReport report,
            List<DeverQuestIdentityCatalog> catalogs,
            List<DeverQuestAncestry> ancestries,
            List<DeverQuestClassDefinition> classes,
            List<DeverQuestDeity> faiths)
        {
            List<DeverQuestIdentityAsset> all = new List<DeverQuestIdentityAsset>();
            all.AddRange(ancestries);
            all.AddRange(classes);
            all.AddRange(faiths);
            ValidateDuplicateIds(report, all, value => ReadSerializedId(value, "identityId"),
                "DQ-CONTENT-301", "Duplicate Identity ID");
            foreach (DeverQuestIdentityAsset asset in all)
            {
                if (string.IsNullOrWhiteSpace(asset.displayName))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-302", "Identity asset has no display name",
                        "Add a readable name before character onboarding.", asset, false);
                }
            }
            foreach (DeverQuestIdentityCatalog catalog in catalogs)
            {
                bool missing = (catalog.ancestries?.Count ?? 0) == 0 ||
                               (catalog.classes?.Count ?? 0) == 0;
                if (missing)
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        "DQ-CONTENT-303", "Identity Catalog is not playable",
                        "At least one Ancestry and one Class are required.", catalog, false);
                }
                if (catalog.defaultAncestry == null || catalog.defaultClass == null)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-304", "Identity Catalog is missing defaults",
                        "Safe repair can select the first valid Ancestry and Class.",
                        catalog, true);
                }
                if ((catalog.ancestries?.Any(value => value == null) ?? false) ||
                    (catalog.classes?.Any(value => value == null) ?? false) ||
                    (catalog.faiths?.Any(value => value == null) ?? false))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-305", "Identity Catalog contains missing references",
                        "Safe repair can remove null entries without deleting valid assets.",
                        catalog, true);
                }
            }
        }

        private static void ValidateCompanions(
            DeverQuestContentValidationReport report,
            List<DeverQuestCompanionCatalog> catalogs,
            List<DeverQuestCompanionProfile> companions)
        {
            foreach (string brokenPath in
                     DeverQuestCompanionProfileMigrationService
                         .FindBrokenCompanionProfilePaths())
            {
                report.findings.Add(new DeverQuestContentFinding
                {
                    severity = DeverQuestContentFindingSeverity.Error,
                    code = "DQ-CONTENT-404",
                    title = "Companion Profile script is missing",
                    detail =
                        "Run Tools > DeverQuest > QA > Repair Companion " +
                        "Profile Asset Scripts, then rerun the Original " +
                        "Companion Stable generator.",
                    asset = null,
                    assetPath = brokenPath,
                    safelyRepairable = false
                });
            }

            ValidateDuplicateIds(report, companions, value => ReadSerializedId(value, "companionId"),
                "DQ-CONTENT-401", "Duplicate Companion ID");
            foreach (DeverQuestCompanionCatalog catalog in catalogs)
            {
                if ((catalog.companions?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-402", "Companion Catalog is empty",
                        "Populate the Stable or remove the unused Catalog.", catalog, false);
                }
                if (catalog.companions?.Any(value => value == null) ?? false)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-403", "Companion Catalog contains missing references",
                        "Safe repair can remove null entries.", catalog, true);
                }
            }
        }

        private static void ValidateEncounters(
            DeverQuestContentValidationReport report,
            List<DeverQuestEncounterProfile> encounters,
            List<DeverQuestMonsterProfile> monsters)
        {
            ValidateDuplicateIds(report, encounters, value => ReadSerializedId(value, "encounterId"),
                "DQ-CONTENT-501", "Duplicate Encounter ID");
            ValidateDuplicateIds(report, monsters, value => ReadSerializedId(value, "monsterId"),
                "DQ-CONTENT-502", "Duplicate Monster ID");
            foreach (DeverQuestEncounterProfile encounter in encounters)
            {
                if ((encounter.waves?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-503", "Encounter has no waves",
                        "Add at least one wave before attaching it to a Quest.",
                        encounter, false);
                    continue;
                }
                if (encounter.waves.Any(value => value == null || value.monster == null))
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        "DQ-CONTENT-504", "Encounter wave has no Monster",
                        "Every authored wave needs a Monster Profile.", encounter, false);
                }
            }
        }

        private static void ValidateEconomy(
            DeverQuestContentValidationReport report,
            List<DeverQuestShopProfile> shops,
            List<DeverQuestShopItem> items)
        {
            ValidateDuplicateIds(report, items, value => ReadSerializedId(value, "shopItemId"),
                "DQ-CONTENT-601", "Duplicate Shop Item ID");
            foreach (DeverQuestShopProfile shop in shops)
            {
                if ((shop.items?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-602", "Quartermaster has no stock",
                        "Add items or disable the unused Shop Profile.", shop, false);
                }
                if (shop.items?.Any(value => value == null) ?? false)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-603", "Quartermaster contains missing item references",
                        "Safe repair can remove null stock entries.", shop, true);
                }
            }
            foreach (DeverQuestShopItem item in items)
            {
                if (string.IsNullOrWhiteSpace(item.displayName))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-604", "Shop Item has no display name",
                        "Add a readable item name.", item, false);
                }
                if (item.questProtected && (item.tradable || item.droppable))
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        "DQ-CONTENT-605", "Quest-protected item has unsafe permissions",
                        "Quest-protected items must not be tradable or droppable.",
                        item, false);
                }
                if (item.itemType == DeverQuestShopItemType.Equipment &&
                    item.equipment == null)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-606", "Equipment Shop Item has no Equipment asset",
                        "Assign an Equipment asset before stocking this item.", item, false);
                }
            }
        }

        private static void ValidateAudio(
            DeverQuestContentValidationReport report,
            List<DeverQuestPlaylist> playlists,
            List<DeverQuestAmbienceProfile> ambience,
            List<DeverQuestWarningAudioProfile> warnings)
        {
            foreach (DeverQuestPlaylist playlist in playlists)
            {
                if ((playlist.Tracks?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-701", "Playlist is empty",
                        "Add one or more AudioClips before selecting this Playlist.",
                        playlist, false);
                }
                else if (playlist.Tracks.Any(value => value == null))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-702", "Playlist contains missing clips",
                        "Remove or replace missing AudioClip references.", playlist, false);
                }
            }
            foreach (DeverQuestAmbienceProfile profile in ambience)
            {
                if ((profile.ambienceClips?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-703", "Ambience Profile is empty",
                        "Add at least one ambience clip.", profile, false);
                }
                else if (profile.ambienceClips.Any(value => value == null))
                {
                    Add(report, DeverQuestContentFindingSeverity.Warning,
                        "DQ-CONTENT-704", "Ambience Profile contains missing clips",
                        "Remove or replace missing AudioClip references.", profile, false);
                }
            }
            foreach (DeverQuestWarningAudioProfile profile in warnings)
            {
                if (profile == null)
                {
                    continue;
                }
                // A partially populated warning profile is valid, so only retain it in the scan count.
            }
        }

        private static void ValidateLoadouts(
            DeverQuestContentValidationReport report,
            List<DeverQuestStarterLoadout> loadouts)
        {
            ValidateDuplicateIds(report, loadouts, value => ReadSerializedId(value, "loadoutId"),
                "DQ-CONTENT-801", "Duplicate Starter Loadout ID");
            foreach (DeverQuestStarterLoadout loadout in loadouts)
            {
                if ((loadout.equipment?.Count ?? 0) == 0 &&
                    (loadout.spells?.Count ?? 0) == 0)
                {
                    Add(report, DeverQuestContentFindingSeverity.Info,
                        "DQ-CONTENT-802", "Starter Loadout is empty",
                        "This is valid when the loadout is intentionally a blank template.",
                        loadout, false);
                }
            }
        }

        private static string ReadSerializedId(
            UnityEngine.Object asset,
            string propertyName)
        {
            if (asset == null)
            {
                return string.Empty;
            }
            SerializedObject serialized = new SerializedObject(asset);
            SerializedProperty property =
                serialized.FindProperty(propertyName);
            return property == null
                ? string.Empty
                : property.stringValue?.Trim() ?? string.Empty;
        }

        private static void ValidateDuplicateIds<T>(
            DeverQuestContentValidationReport report,
            IEnumerable<T> assets,
            Func<T, string> idSelector,
            string code,
            string title) where T : UnityEngine.Object
        {
            List<T> materialized = assets
                .Where(value => value != null)
                .ToList();
            foreach (T asset in materialized)
            {
                if (string.IsNullOrWhiteSpace(idSelector(asset)))
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        code, title,
                        "The asset has no stable ID. Reopen and save it, or recreate it in a disposable repair workflow.",
                        asset, false);
                }
            }
            foreach (IGrouping<string, T> group in materialized
                         .GroupBy(idSelector, StringComparer.OrdinalIgnoreCase)
                         .Where(value => !string.IsNullOrWhiteSpace(value.Key) &&
                                         value.Count() > 1))
            {
                foreach (T asset in group)
                {
                    Add(report, DeverQuestContentFindingSeverity.Error,
                        code, title,
                        $"ID {group.Key} is shared by {group.Count()} assets. " +
                        "Choose the copied or newer asset and use the explicit " +
                        "Regenerate This Asset ID action. Do not regenerate " +
                        "every asset in the duplicate group.",
                        asset, false);
                }
            }
        }

        private static List<T> LoadAssets<T>() where T : UnityEngine.Object
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            List<T> assets = new List<T>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null && !assets.Contains(asset))
                {
                    assets.Add(asset);
                }
            }
            return assets;
        }

        private static void Add(
            DeverQuestContentValidationReport report,
            DeverQuestContentFindingSeverity severity,
            string code,
            string title,
            string detail,
            UnityEngine.Object asset,
            bool safelyRepairable)
        {
            report.findings.Add(new DeverQuestContentFinding
            {
                severity = severity,
                code = code,
                title = title,
                detail = detail,
                asset = asset,
                assetPath = asset == null
                    ? string.Empty
                    : AssetDatabase.GetAssetPath(asset),
                safelyRepairable = safelyRepairable
            });
        }
    }
}

//----- DeverQuestContentValidationService.cs END -----
