//----- DeverQuestWindow.cs START -----

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal sealed class DeverQuestWindow : EditorWindow
    {
        private enum DeverQuestWorkspace
        {
            Quest = 0,
            QuestLog = 1,
            Character = 2,
            Inventory = 3,
            Economy = 4,
            GuildHall = 5,
            RewardsHistory = 6,
            Tactics = 7,
            AudioWellness = 8,
            Settings = 9,
            Chronicle = 10,
            Git = 11,
            Visuals = 12,
            Administration = 13,
            Distribution = 14
        }

        private enum QuestTurnInStep
        {
            Chronicle = 0,
            Rewards = 1
        }

        private const float MinimumWindowWidth = 430f;
        private const float MinimumWindowHeight = 440f;

        private Vector2 scrollPosition;
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle wrappedLabelStyle;
        private GUIStyle wrappedTextAreaStyle;
        private GUIStyle timerStyle;
        private GUIStyle accentLabelStyle;

        private string newProjectName = string.Empty;
        private string newTaskName = string.Empty;
        private string newCategory = "Programming";
        private string newGoal = string.Empty;
        private DeverQuestQuestProfile selectedQuestProfile;
        private string appliedQuestProfileId = string.Empty;
        private DeverQuestQuestContract selectedQuestContract;
        private string appliedQuestContractId = string.Empty;
        private string commitComment = string.Empty;
        private string commitBranch = string.Empty;
        private string commitHash = string.Empty;
        private string closingNotes = string.Empty;
        private bool showFinalization;
        private string rewardMessage = string.Empty;
        private bool historyFoldout;
        private bool hallOfHeroesFoldout = true;
        private bool contractBoardFoldout = true;
        private bool questRunManagementFoldout = true;
        private bool questRunArchiveFoldout = true;
        private bool showArchivedContracts;
        private string questRunArchiveSearch = string.Empty;
        private bool guildAdministrationFoldout = true;
        private bool guildCollapsedForActiveQuest;
        private DeverQuestHistoryRange historyRange =
            DeverQuestHistoryRange.Last7Days;
        private string historyProjectFilter = string.Empty;
        private string historyCategoryFilter = string.Empty;
        private string historyStartDate =
            DateTime.Now.AddDays(-6).ToString("yyyy-MM-dd");
        private string historyEndDate =
            DateTime.Now.ToString("yyyy-MM-dd");
        private string historyMessage = string.Empty;
        private bool chronicleLiveFoldout = true;
        private bool chronicleArchiveFoldout = true;
        private string chronicleSearch = string.Empty;
        private int chronicleFilterIndex;
        private int chronicleResultLimit = 25;
        private readonly HashSet<string> chronicleExpandedSessions =
            new HashSet<string>();
        private string chronicleMessage = string.Empty;
        private string correctionDataPath = string.Empty;
        private string correctionSessionId = string.Empty;
        private string correctionSessionTitle = string.Empty;
        private string correctionReason = string.Empty;
        private string correctionValue = string.Empty;
        private string focusScheduleText = string.Empty;
        private string wellnessHistorySearch = string.Empty;
        private int wellnessHistoryFilter;
        private bool wellnessQueueFoldout = true;
        private bool wellnessSettingsFoldout = true;
        private bool wellnessHistoryFoldout = true;
        private string guildLoginName = string.Empty;
        private string guildPasscode = string.Empty;
        private string newGuildDeveloper = string.Empty;
        private string newGuildCharacter = string.Empty;
        private string newGuildClass = "Warrior";
        private DeverQuestClassDefinition newGuildClassDefinition;
        private string newGuildRank = "Member";
        private string newGuildProjects = string.Empty;
        private string newGuildPasscode = string.Empty;
        private string guildMessage = string.Empty;
        private bool compensationPreviewFoldout = true;
        private bool compensationAdministrationFoldout;
        private int compensationAccountIndex;
        private string compensationLoadedAccountId = string.Empty;
        private bool compensationEnabled;
        private DeverQuestCompensationBasis compensationBasis =
            DeverQuestCompensationBasis.Hourly;
        private string compensationCurrencyCode = "USD";
        private double compensationHourlyRate;
        private double compensationAnnualSalary;
        private double compensationWeeklyHours = 40d;
        private bool compensationIncludeApprovedBreaks;
        private DeverQuestCompensationIntegrityPolicy
            compensationIntegrityPolicy =
                DeverQuestCompensationIntegrityPolicy
                    .VerifiedChroniclesOnly;
        private DeverQuestAbility rulesAbility =
            DeverQuestAbility.Intelligence;
        private int rulesDifficultyClass = 12;
        private bool rulesProficient = true;
        private string rulesSeed = "Ajnaag-Test-1";
        private string rulesResult = string.Empty;
        private DeverQuestEquipment selectedRulesEquipment;
        private DeverQuestSpell selectedRulesSpell;
        private DeverQuestCompanionProfile selectedCompanionProfile;
        private bool companionStableFoldout = true;
        private string companionMessage = string.Empty;
        private bool tacticalReadinessFoldout = true;
        private bool tacticalCompanionFoldout = true;
        private bool tacticalArchiveFoldout = true;
        private string tacticalArchiveSearch = string.Empty;
        private int tacticalArchiveOutcomeIndex;
        private int tacticalCompanionIndex;
        private string tacticalOperationsMessage = string.Empty;
        private string inventorySearch = string.Empty;
        private int inventoryCategoryIndex;
        private bool inventoryShowProvenance = true;
        private bool inventoryShowLore;
        private string inventoryMessage = string.Empty;
        private DeverQuestShopProfile selectedShopProfile;
        private bool economyMerchantFoldout = true;
        private bool economyGrantFoldout = true;
        private bool economyLedgerFoldout = true;
        private int economyAccountIndex;
        private DeverQuestShopItem economyGrantItem;
        private int economyGrantQuantity = 1;
        private long economyGrantCopper = 100;
        private string economyGrantNote = string.Empty;
        private string economySearch = string.Empty;
        private int economyTransactionTypeIndex;
        private string economyMessage = string.Empty;
        private bool guildShopFoldout = true;
        private bool purchaseHistoryFoldout;
        private bool tradeLedgerFoldout;
        private int tradeTargetIndex;
        private string fulfillmentReference = string.Empty;
        private string shopMessage = string.Empty;
        private bool contentScaffoldFoldout = true;
        private string contentScaffoldMessage = string.Empty;
        private DeverQuestContentValidationReport administrationReport;
        private string administrationSearch = string.Empty;
        private int administrationSeverityIndex;
        private string administrationMessage = string.Empty;
        private bool administrationGeneratorQueued;
        private DeverQuestDistributionReport distributionReport;
        private int distributionChannelIndex = 1;
        private string distributionMessage = string.Empty;
        private string distributionExportDirectory = string.Empty;
        private string creationCharacterName = string.Empty;
        private DeverQuestAncestry creationAncestry;
        private DeverQuestClassDefinition creationClassDefinition;
        private DeverQuestDeity creationFaith;
        private DeverQuestAlignment creationAlignment =
            DeverQuestAlignment.TrueNeutral;
        private bool identityCatalogGenerationQueued;
        private DeverQuestGitStatus gitStatus;
        private string gitMessage = string.Empty;
        private string gitCommitMessage = string.Empty;
        private bool gitOperationInProgress;
        private string visualsMessage = string.Empty;
        private string voiceMemoName = "Quest Memo";
        private int selectedMicrophoneIndex;
        private string mediaMessage = string.Empty;
        private QuestTurnInStep turnInStep =
            QuestTurnInStep.Chronicle;
        private DeverQuestWorkspace activeWorkspace =
            DeverQuestWorkspace.Quest;
        private double nextSessionRepaintTime;

        [MenuItem("Tools/DeverQuest/Developer Companion")]
        public static void Open()
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");

            window.minSize =
                new Vector2(MinimumWindowWidth, MinimumWindowHeight);

            window.Show();
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Current Quest")]
        internal static void OpenQuestWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Quest);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Quest Log")]
        internal static void OpenQuestLogWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.QuestLog);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Git")]
        internal static void OpenGitWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Git);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Visuals")]
        internal static void OpenVisualsWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Visuals);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Beta Administration")]
        internal static void OpenAdministrationWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Administration);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Packaging and Distribution")]
        internal static void OpenDistributionWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Distribution);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Character Sheet")]
        private static void OpenCharacterWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Character);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Inventory and Equipment")]
        private static void OpenInventoryWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Inventory);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Guild Economy")]
        private static void OpenEconomyWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Economy);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Guild Hall")]
        private static void OpenGuildWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.GuildHall);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Rewards and History")]
        private static void OpenHistoryWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.RewardsHistory);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Quest Archive and Chronicle")]
        internal static void OpenChronicleWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Chronicle);
        }

        [MenuItem("Tools/DeverQuest/Workspaces/Tactical Operations")]
        private static void OpenTacticsWorkspace()
        {
            OpenWorkspace(DeverQuestWorkspace.Tactics);
        }

        private static void OpenWorkspace(
            DeverQuestWorkspace workspace)
        {
            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");
            window.activeWorkspace = workspace;
            window.minSize =
                new Vector2(
                    MinimumWindowWidth,
                    MinimumWindowHeight);
            window.Show();
            window.Focus();
            window.Repaint();
        }

        internal static void OpenQuestTurnIn()
        {
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                OpenQuestWorkspace();
                return;
            }

            DeverQuestWindow window =
                GetWindow<DeverQuestWindow>("DeverQuest");
            window.activeWorkspace = DeverQuestWorkspace.Quest;
            window.minSize =
                new Vector2(
                    MinimumWindowWidth,
                    MinimumWindowHeight);
            window.Show();
            window.Focus();
            window.BeginFinalization(
                DeverQuestSessionStore.ActiveSession);
            window.Repaint();
        }

        internal static void ShowIdleWarning(int secondsRemaining)
        {
            ShowConfiguredNotification(
                $"Still working? Idle pause in about " +
                $"{secondsRemaining} seconds.",
                4d,
                DeverQuestAudioCue.IdleWarning);
        }

        internal static void ShowIdlePaused()
        {
            ShowConfiguredNotification(
                "Quest entered meditation because no activity was detected.",
                6d,
                DeverQuestAudioCue.IdlePaused);
        }

        internal static void ShowWellnessReminder(
            string title,
            DeverQuestWellnessType type)
        {
            DeverQuestAudioCue cue;
            switch (type)
            {
                case DeverQuestWellnessType.Hydration:
                    cue = DeverQuestAudioCue.Hydration;
                    break;
                case DeverQuestWellnessType.MovementBreak:
                case DeverQuestWellnessType.Exercise:
                    cue = DeverQuestAudioCue.MovementBreak;
                    break;
                case DeverQuestWellnessType.Lunch:
                case DeverQuestWellnessType.Dinner:
                    cue = DeverQuestAudioCue.MealReminder;
                    break;
                default:
                    cue = DeverQuestAudioCue.FocusCheckIn;
                    break;
            }
            ShowConfiguredNotification(
                $"DeverQuest: {title}", 6d, cue);
        }

        private static void ShowConfiguredNotification(
            string message,
            double duration,
            DeverQuestAudioCue cue)
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            DeverQuestWindow[] windows =
                Resources.FindObjectsOfTypeAll<DeverQuestWindow>();
            DeverQuestWindow window =
                windows.Length > 0 ? windows[0] : null;

            if (window == null && profile.autoOpenWindowForReminders)
            {
                window = GetWindow<DeverQuestWindow>("DeverQuest");
            }

            if (profile.showEditorNotifications && window != null)
            {
                window.ShowNotification(
                    new GUIContent(message),
                    duration);
            }

            if (profile.notificationSoundsEnabled)
            {
                if (!DeverQuestAudioDirector.PlayCue(cue))
                {
                    EditorApplication.Beep();
                }
            }

            window?.Repaint();
        }

        private void OnEnable()
        {
            minSize = new Vector2(
                MinimumWindowWidth,
                MinimumWindowHeight);

            EditorApplication.update -= RepaintWhileSessionRuns;
            EditorApplication.update += RepaintWhileSessionRuns;

            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            if (string.IsNullOrWhiteSpace(newProjectName))
            {
                newProjectName = profile.lockProjectName
                    ? profile.lockedProjectName
                    : profile.lastProjectName;
            }
            if (!string.IsNullOrWhiteSpace(profile.lastDepartmentName))
            {
                newCategory = profile.lastDepartmentName;
            }
            selectedShopProfile =
                selectedShopProfile ?? DeverQuestShopService.ActiveProfile;
            focusScheduleText = string.Join(
                ", ",
                profile.focusCheckInScheduleMinutes);
            RefreshGitStatus();
            if (profile.sharedGuildEnabled)
            {
                DeverQuestSharedGuildService.Refresh();
            }
        }

        private void OnDisable()
        {
            EditorApplication.update -= RepaintWhileSessionRuns;
        }

        private void OnGUI()
        {
            BuildStyles();
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            ApplyThemeToStyles(profile);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (!profile.setupComplete || !profile.compactMode)
            {
                DrawHeader();
            }
            EditorGUILayout.Space(12f);

            if (profile.setupComplete)
            {
                if (!DeverQuestGuildAccountService.IsAuthenticated)
                {
                    DrawGuildLogin();
                }
                else if (DeverQuestGuildAccountService
                             .NeedsCharacterCreation)
                {
                    DrawCharacterCreation();
                }
                else if (profile.compactMode)
                {
                    DrawCompactDashboard(profile);
                }
                else
                {
                    DrawSessionDashboard();
                }
            }
            else
            {
                DrawFirstTimeSetup();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawHeader()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            EditorGUILayout.LabelField("DEVERQUEST", titleStyle);
            EditorGUILayout.LabelField(
                "Developer Companion",
                subtitleStyle);

            if (profile.showHeaderTagline)
            {
                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField(
                    "Accept quests, build your legend, and earn your downtime.",
                    wrappedLabelStyle);
            }
        }

        private void DrawGuildLogin()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Enter the Guild Hall",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Choose your administrator-created Guild account and enter " +
                "its local passcode.",
                MessageType.Info);
            string[] names = DeverQuestGuildAccountService.Accounts
                .Where(account => !account.disabled)
                .Select(account => account.developerName)
                .ToArray();
            int index = Mathf.Max(0,
                Array.IndexOf(names, guildLoginName));
            if (names.Length > 0)
            {
                guildLoginName = names[
                    EditorGUILayout.Popup("Developer", index, names)];
            }
            guildPasscode = EditorGUILayout.PasswordField(
                "Passcode", guildPasscode);
            if (GUILayout.Button("Enter Guild Hall"))
            {
                if (DeverQuestGuildAccountService.Login(
                        guildLoginName,
                        guildPasscode,
                        out string error))
                {
                    guildPasscode = string.Empty;
                    guildMessage = "Guild identity verified.";
                }
                else
                {
                    guildMessage = error;
                }
            }
            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage,
                    MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCharacterCreation()
        {
            EnsureCharacterCreationDefaults();
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Create Your Adventurer",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Choose carefully. Your name, Ancestry, Class, Alignment, " +
                "Faith, and home Department lock when you enter the Guild.",
                MessageType.Info);
            creationCharacterName = EditorGUILayout.TextField(
                "Adventurer Name", creationCharacterName);
            creationAncestry =
                (DeverQuestAncestry)EditorGUILayout.ObjectField(
                    "Ancestry",
                    creationAncestry,
                    typeof(DeverQuestAncestry),
                    false);
            creationClassDefinition =
                (DeverQuestClassDefinition)
                EditorGUILayout.ObjectField(
                    "Class",
                    creationClassDefinition,
                    typeof(DeverQuestClassDefinition),
                    false);
            creationAlignment =
                (DeverQuestAlignment)EditorGUILayout.EnumPopup(
                    "Alignment", creationAlignment);
            creationFaith =
                (DeverQuestDeity)EditorGUILayout.ObjectField(
                    "Faith",
                    creationFaith,
                    typeof(DeverQuestDeity),
                    false);
            string creationDepartment =
                creationClassDefinition?.department ?? "Unassigned";
            EditorGUILayout.LabelField(
                "Home Department", creationDepartment);
            EditorGUILayout.HelpBox(
                "Your Class asset determines your starting Department. " +
                "Dungeon Masters can use Contract eligibility rules " +
                "to build Class-, Ancestry-, and Department-specific Quests.",
                MessageType.Info);
            bool eligible =
                DeverQuestIdentityCatalogService.IsEligible(
                    creationAncestry,
                    creationClassDefinition,
                    out string eligibilityReason);
            if (eligible)
            {
                eligible =
                    DeverQuestIdentityCatalogService.IsFaithEligible(
                        creationFaith,
                        creationAncestry,
                        creationClassDefinition,
                        creationAlignment,
                        out eligibilityReason);
            }
            if (!eligible)
            {
                EditorGUILayout.HelpBox(
                    eligibilityReason,
                    MessageType.Warning);
            }
            DeverQuestAdventurer preview =
                new DeverQuestAdventurer();
            DeverQuestIdentityCatalogService.ApplyIdentityFoundation(
                preview,
                creationAncestry,
                creationClassDefinition,
                creationFaith,
                creationAlignment,
                true);
            EditorGUILayout.LabelField(
                "Identity Foundation",
                $"STR {preview.strength} · DEX {preview.dexterity} · " +
                $"CON {preview.constitution} · INT {preview.intelligence} · " +
                $"WIS {preview.wisdom} · CHA {preview.charisma}");
            EditorGUILayout.LabelField(
                "Extended Attributes",
                $"AGI {preview.agility} · STA {preview.stamina} · " +
                $"Luck {preview.luck} · HP {preview.maximumHitPoints} · " +
                $"Mana {preview.maximumMana}");
            if (creationClassDefinition != null &&
                creationClassDefinition.supportsCompanion)
            {
                EditorGUILayout.LabelField(
                    "Companion Tradition",
                    string.IsNullOrWhiteSpace(
                        creationClassDefinition.companionTradition)
                        ? "Supported"
                        : creationClassDefinition.companionTradition);
                if (creationClassDefinition.starterCompanion != null)
                {
                    EditorGUILayout.LabelField(
                        "Starting Companion",
                        creationClassDefinition
                            .starterCompanion.displayName);
                }
            }
            using (new EditorGUI.DisabledScope(
                       !eligible ||
                       string.IsNullOrWhiteSpace(
                           creationCharacterName)))
            {
                if (GUILayout.Button(
                        "Enter the Guild",
                        GUILayout.Height(34f)))
                {
                    if (!DeverQuestGuildAccountService
                            .CompleteCharacterCreation(
                                creationCharacterName,
                                creationAncestry,
                                creationClassDefinition,
                                creationFaith,
                                creationAlignment,
                                out string error))
                    {
                        guildMessage = error;
                    }
                }
            }
            DeverQuestIdentityCatalog activeIdentityCatalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            bool identityCatalogReady =
                activeIdentityCatalog != null &&
                activeIdentityCatalog.defaultAncestry != null &&
                activeIdentityCatalog.defaultClass != null &&
                (activeIdentityCatalog.ancestries?.Count ?? 0) > 0 &&
                (activeIdentityCatalog.classes?.Count ?? 0) > 0;
            if (!identityCatalogReady)
            {
                EditorGUILayout.HelpBox(
                    "No complete playable Identity Catalog is installed. " +
                    "Generate or repair the original starter catalog before " +
                    "finishing character creation.",
                    MessageType.Warning);
                bool canGenerate =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageGuild);
                using (new EditorGUI.DisabledScope(
                           !canGenerate ||
                           identityCatalogGenerationQueued))
                {
                    if (GUILayout.Button(
                            identityCatalogGenerationQueued
                                ? "Generating Starter Identity Catalog…"
                                : "Generate Original Starter Identity Catalog"))
                    {
                        QueueOriginalStarterIdentityCatalogGeneration();
                    }
                }
                if (!canGenerate)
                {
                    EditorGUILayout.HelpBox(
                        "Ask a CEO or Boss to generate or assign the Guild's " +
                        "Identity Catalog.",
                        MessageType.Info);
                }
            }
            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage, MessageType.Warning);
            }
            EditorGUILayout.EndVertical();
        }

        private void QueueOriginalStarterIdentityCatalogGeneration(
            bool reportToContentScaffolding = false)
        {
            if (identityCatalogGenerationQueued)
            {
                return;
            }

            identityCatalogGenerationQueued = true;
            string preparingMessage =
                "Preparing the original starter Identity Catalog…";
            if (reportToContentScaffolding)
            {
                contentScaffoldMessage = preparingMessage;
            }
            else
            {
                guildMessage = preparingMessage;
            }
            Repaint();

            EditorApplication.delayCall += () =>
            {
                try
                {
                    DeverQuestIdentityGenerationReport report =
                        DeverQuestIdentityCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    string resultMessage = report.Summary;
                    if (reportToContentScaffolding)
                    {
                        contentScaffoldMessage = resultMessage;
                    }
                    else
                    {
                        guildMessage = resultMessage;
                    }

                    if (!report.Succeeded || report.Catalog == null)
                    {
                        return;
                    }

                    creationAncestry =
                        report.Catalog.defaultAncestry;
                    creationClassDefinition =
                        report.Catalog.defaultClass;
                    creationFaith =
                        report.Catalog.defaultFaith;
                    Selection.activeObject = report.Catalog;
                    EditorGUIUtility.PingObject(report.Catalog);
                    SelectGeneratedFolder(report.RootPath);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    string failureMessage =
                        "Starter Identity Catalog generation failed: " +
                        exception.Message;
                    if (reportToContentScaffolding)
                    {
                        contentScaffoldMessage = failureMessage;
                    }
                    else
                    {
                        guildMessage = failureMessage;
                    }
                }
                finally
                {
                    identityCatalogGenerationQueued = false;
                    Repaint();
                }
            };
        }

        private void EnsureCharacterCreationDefaults()
        {
            DeverQuestIdentityCatalog catalog =
                DeverQuestIdentityCatalogService.ActiveCatalog;
            if ((creationAncestry == null ||
                 (catalog != null &&
                  !(catalog.ancestries ??
                    new List<DeverQuestAncestry>())
                      .Contains(creationAncestry))) &&
                DeverQuestIdentityCatalogService.Ancestries.Count > 0)
            {
                creationAncestry =
                    catalog?.defaultAncestry ??
                    DeverQuestIdentityCatalogService.Ancestries[0];
            }
            if ((creationClassDefinition == null ||
                 (catalog != null &&
                  !(catalog.classes ??
                    new List<DeverQuestClassDefinition>())
                      .Contains(creationClassDefinition))) &&
                DeverQuestIdentityCatalogService.Classes.Count > 0)
            {
                creationClassDefinition =
                    catalog?.defaultClass ??
                    DeverQuestIdentityCatalogService.Classes
                        .FirstOrDefault(value =>
                            value.displayName == "Warrior") ??
                    DeverQuestIdentityCatalogService.Classes[0];
            }
            if ((creationFaith == null ||
                 (catalog != null &&
                  !(catalog.faiths ??
                    new List<DeverQuestDeity>())
                      .Contains(creationFaith))) &&
                DeverQuestIdentityCatalogService.Faiths.Count > 0)
            {
                creationFaith =
                    catalog?.defaultFaith ??
                    DeverQuestIdentityCatalogService.Faiths
                        .FirstOrDefault(value =>
                            value.displayName == "Agnostic") ??
                    DeverQuestIdentityCatalogService.Faiths[0];
            }
        }

        private void DrawGuildAdministration()
        {
            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.CurrentAccount;
            if (account == null)
            {
                return;
            }
            guildAdministrationFoldout = EditorGUILayout.Foldout(
                guildAdministrationFoldout,
                "Guild Accounts and Authority",
                true);
            if (!guildAdministrationFoldout)
            {
                return;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"{account.developerName} · {account.guildRank}",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Assigned Projects",
                account.assignedProjects.Count == 0
                    ? "All Guild projects (rank permitting)"
                    : string.Join(", ", account.assignedProjects));

            if (DeverQuestGuildAccountService.RequiresPasscodeSetup)
            {
                EditorGUILayout.HelpBox(
                    "Your existing Adventurer was migrated as the founding " +
                    "CEO. Set a local passcode now to lock the identity and " +
                    "permissions.",
                    MessageType.Warning);
                guildPasscode = EditorGUILayout.PasswordField(
                    "New Passcode", guildPasscode);
                if (GUILayout.Button("Secure Founding Account"))
                {
                    if (DeverQuestGuildAccountService
                        .SecureCurrentAccount(
                            guildPasscode, out string error))
                    {
                        guildPasscode = string.Empty;
                        guildMessage =
                            "Founding CEO account secured.";
                    }
                    else
                    {
                        guildMessage = error;
                    }
                }
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Create Adventurer Account",
                    EditorStyles.boldLabel);
                newGuildDeveloper = EditorGUILayout.TextField(
                    "Developer", newGuildDeveloper);
                newGuildCharacter = EditorGUILayout.TextField(
                    "Adventurer", newGuildCharacter);
                if (DeverQuestIdentityCatalogService.Classes.Count > 0)
                {
                    if (newGuildClassDefinition == null)
                    {
                        newGuildClassDefinition =
                            DeverQuestIdentityCatalogService.Classes
                                .FirstOrDefault(value =>
                                    value.displayName ==
                                    newGuildClass) ??
                            DeverQuestIdentityCatalogService.Classes[0];
                    }
                    newGuildClassDefinition =
                        (DeverQuestClassDefinition)
                        EditorGUILayout.ObjectField(
                            "Class",
                            newGuildClassDefinition,
                            typeof(DeverQuestClassDefinition),
                            false);
                    if (newGuildClassDefinition != null)
                    {
                        newGuildClass =
                            newGuildClassDefinition.displayName;
                    }
                }
                else
                {
                    newGuildClass = DrawStringPopup(
                        "Legacy Class", newGuildClass,
                        DeverQuestAdventurerService.Classes);
                }
                newGuildRank = DrawStringPopup(
                    "Guild Rank", newGuildRank,
                    DeverQuestAdventurerService.GuildRanks);
                newGuildProjects = EditorGUILayout.TextField(
                    new GUIContent(
                        "Projects",
                        "Comma-separated Project assignments. Required for " +
                        "Project Leaders."),
                    newGuildProjects);
                newGuildPasscode = EditorGUILayout.PasswordField(
                    "Temporary Passcode", newGuildPasscode);
                if (GUILayout.Button("Create Guild Account"))
                {
                    if (DeverQuestGuildAccountService.CreateAccount(
                            newGuildDeveloper,
                            newGuildCharacter,
                            newGuildClass,
                            newGuildRank,
                            newGuildProjects.Split(','),
                            newGuildPasscode,
                            out string error))
                    {
                        guildMessage =
                            $"Created account for {newGuildDeveloper}.";
                        newGuildDeveloper = string.Empty;
                        newGuildCharacter = string.Empty;
                        newGuildProjects = string.Empty;
                        newGuildPasscode = string.Empty;
                    }
                    else
                    {
                        guildMessage = error;
                    }
                }

                DrawCompensationAdministration();
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Recent Authority Audit",
                EditorStyles.boldLabel);
            foreach (DeverQuestGuildAuditEntry entry
                     in DeverQuestGuildAccountService.AuditEntries.Take(8))
            {
                EditorGUILayout.LabelField(
                    $"{entry.actorName}: {entry.action} → {entry.target}",
                    EditorStyles.wordWrappedLabel);
            }

            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage, MessageType.Info);
            }
            if (GUILayout.Button("Leave Guild Hall"))
            {
                DeverQuestGuildAccountService.Logout();
                guildMessage = string.Empty;
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawCompensationAdministration()
        {
            compensationAdministrationFoldout =
                EditorGUILayout.Foldout(
                    compensationAdministrationFoldout,
                    "Compensation Preview Policies",
                    true);
            if (!compensationAdministrationFoldout)
            {
                return;
            }

            List<DeverQuestGuildAccount> accounts =
                DeverQuestGuildAccountService.Accounts
                    .Where(value => value != null)
                    .ToList();
            if (accounts.Count == 0)
            {
                return;
            }

            compensationAccountIndex = Mathf.Clamp(
                compensationAccountIndex,
                0,
                accounts.Count - 1);
            compensationAccountIndex = EditorGUILayout.Popup(
                "Adventurer Account",
                compensationAccountIndex,
                accounts
                    .Select(value => value.developerName)
                    .ToArray());
            DeverQuestGuildAccount target =
                accounts[compensationAccountIndex];
            LoadCompensationPolicy(target);

            compensationEnabled = EditorGUILayout.Toggle(
                "Enable Preview",
                compensationEnabled);
            compensationBasis =
                (DeverQuestCompensationBasis)
                EditorGUILayout.EnumPopup(
                    "Basis",
                    compensationBasis);
            compensationCurrencyCode = EditorGUILayout.TextField(
                new GUIContent(
                    "Currency Code",
                    "Three-letter display code, such as USD, CAD, EUR, or " +
                    "GBP."),
                compensationCurrencyCode);
            if (compensationBasis ==
                DeverQuestCompensationBasis.Hourly)
            {
                compensationHourlyRate = EditorGUILayout.DoubleField(
                    "Hourly Rate",
                    compensationHourlyRate);
            }
            else
            {
                compensationAnnualSalary = EditorGUILayout.DoubleField(
                    "Annual Salary",
                    compensationAnnualSalary);
                compensationWeeklyHours = EditorGUILayout.DoubleField(
                    "Scheduled Hours/Week",
                    compensationWeeklyHours);
            }
            compensationIncludeApprovedBreaks =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Include Approved Breaks",
                        "When enabled, completed approved-break seconds are " +
                        "included. Meditation and Idle/Unverified time are " +
                        "always excluded."),
                    compensationIncludeApprovedBreaks);
            compensationIntegrityPolicy =
                (DeverQuestCompensationIntegrityPolicy)
                EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Chronicle Eligibility",
                        "Modified or unavailable Chronicles are always " +
                        "excluded."),
                    compensationIntegrityPolicy);

            EditorGUILayout.HelpBox(
                "Compensation settings are a local planning convenience. " +
                "They are not encrypted payroll records and are never " +
                "published to shared Guild snapshots or written into daily " +
                "Chronicles.",
                MessageType.Warning);

            if (GUILayout.Button("Save Preview Policy"))
            {
                if (DeverQuestGuildAccountService
                    .UpdateCompensationPolicy(
                        target.accountId,
                        compensationEnabled,
                        compensationBasis,
                        compensationCurrencyCode,
                        compensationHourlyRate,
                        compensationAnnualSalary,
                        compensationWeeklyHours,
                        compensationIncludeApprovedBreaks,
                        compensationIntegrityPolicy,
                        out string error))
                {
                    compensationLoadedAccountId = string.Empty;
                    guildMessage =
                        $"Saved compensation preview policy for " +
                        $"{target.developerName}.";
                }
                else
                {
                    guildMessage = error;
                }
            }
        }

        private void LoadCompensationPolicy(
            DeverQuestGuildAccount account)
        {
            if (account == null ||
                compensationLoadedAccountId == account.accountId)
            {
                return;
            }
            compensationLoadedAccountId = account.accountId;
            compensationEnabled =
                account.compensationPreviewEnabled;
            compensationBasis = account.compensationBasis;
            compensationCurrencyCode =
                account.compensationCurrencyCode;
            compensationHourlyRate =
                account.compensationHourlyRate;
            compensationAnnualSalary =
                account.compensationAnnualSalary;
            compensationWeeklyHours =
                account.compensationWeeklyHours;
            compensationIncludeApprovedBreaks =
                account.compensationIncludeApprovedBreaks;
            compensationIntegrityPolicy =
                account.compensationIntegrityPolicy;
        }

        private void DrawFirstTimeSetup()
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;

            EditorGUILayout.LabelField(
                "First-Time Setup",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Set up your developer profile and choose where DeverQuest " +
                "will write future timecards. No folder will be created " +
                "until you approve it.",
                MessageType.Info);

            EditorGUILayout.Space(8f);

            profile.developerName = EditorGUILayout.TextField(
                new GUIContent(
                    "Developer Name",
                    "Used on timecards and for the developer subfolder."),
                profile.developerName);

            DrawTimecardRootField(profile);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Session Defaults",
                EditorStyles.boldLabel);

            profile.defaultFocusMinutes = EditorGUILayout.IntField(
                new GUIContent("Focus Minutes"),
                profile.defaultFocusMinutes);

            profile.idleDetectionEnabled = EditorGUILayout.Toggle(
                new GUIContent("Idle Detection"),
                profile.idleDetectionEnabled);

            profile.activityScope =
                (DeverQuestActivityScope)EditorGUILayout.EnumPopup(
                    new GUIContent(
                        "Activity Scope",
                        "Project Focused ignores input made in other apps " +
                        "or Unity projects. System Wide uses all PC input."),
                    profile.activityScope);

            using (new EditorGUI.DisabledScope(
                       !profile.idleDetectionEnabled))
            {
                profile.idleTimeoutMinutes = EditorGUILayout.IntField(
                    new GUIContent("Idle Timeout"),
                    profile.idleTimeoutMinutes);

                profile.idleWarningSeconds = EditorGUILayout.IntField(
                    new GUIContent("Warning Seconds"),
                    profile.idleWarningSeconds);

                EditorGUILayout.LabelField(
                    "Count as Active Work",
                    EditorStyles.boldLabel);

                profile.countPlayModeAsActivity =
                    EditorGUILayout.Toggle(
                        "Play Mode",
                        profile.countPlayModeAsActivity);

                profile.countCompilationAsActivity =
                    EditorGUILayout.Toggle(
                        "Compilation",
                        profile.countCompilationAsActivity);

                profile.countAssetImportAsActivity =
                    EditorGUILayout.Toggle(
                        "Asset Importing",
                        profile.countAssetImportAsActivity);

                profile.countBuildsAsActivity =
                    EditorGUILayout.Toggle(
                        "Player Builds",
                        profile.countBuildsAsActivity);
            }

            profile.defaultFocusMinutes =
                Mathf.Max(1, profile.defaultFocusMinutes);

            profile.idleTimeoutMinutes =
                Mathf.Max(1, profile.idleTimeoutMinutes);

            profile.idleWarningSeconds =
                Mathf.Max(0, profile.idleWarningSeconds);

            DrawWellnessSetup(profile);
            DrawExternalActivitySetup();
            DrawRewardSetup(profile);
            DrawCampaignRulesSetup(profile);
            DrawPlaylistSetup(profile);
            DrawPolishSetup(profile);
            DrawChronicleIntegritySetup(profile);
            DrawProjectDefaultsSetup(profile);

            EditorGUILayout.Space(14f);

            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(profile.developerName) ||
                       string.IsNullOrWhiteSpace(profile.timecardRootPath)))
            {
                if (GUILayout.Button(
                        "Validate Folders and Finish Setup",
                        GUILayout.Height(34f)))
                {
                    TryFinishSetup(profile);
                }
            }
        }

        private void DrawTimecardRootField(DeverQuestProfile profile)
        {
            EditorGUILayout.BeginHorizontal();

            profile.timecardRootPath = EditorGUILayout.TextField(
                new GUIContent(
                    "Timecard Root",
                    "The root folder containing one folder per developer."),
                profile.timecardRootPath);

            if (GUILayout.Button("Browse", GUILayout.Width(70f)))
            {
                string startingPath =
                    Directory.Exists(profile.timecardRootPath)
                        ? profile.timecardRootPath
                        : DeverQuestPathUtility.GetDefaultTimecardRoot();

                string selectedPath = EditorUtility.OpenFolderPanel(
                    "Choose DeverQuest Timecard Root",
                    startingPath,
                    string.Empty);

                if (!string.IsNullOrWhiteSpace(selectedPath))
                {
                    profile.timecardRootPath = selectedPath;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (string.IsNullOrWhiteSpace(profile.timecardRootPath) &&
                GUILayout.Button("Use Recommended Project Folder"))
            {
                profile.timecardRootPath =
                    DeverQuestPathUtility.GetDefaultTimecardRoot();
            }
        }

        private void TryFinishSetup(DeverQuestProfile profile)
        {
            profile.Sanitize();

            if (!EnsureFolderExists(
                    profile.timecardRootPath,
                    "Timecard root folder"))
            {
                return;
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            if (!EnsureFolderExists(
                    developerFolder,
                    "Developer folder"))
            {
                return;
            }

            profile.setupComplete = true;
            DeverQuestSettingsStore.Save();
            DeverQuestGuildAccountService
                .RefreshUnsecuredFounderIdentity(
                    profile.developerName);

            EditorUtility.DisplayDialog(
                "DeverQuest Setup Complete",
                "Your profile is ready.\n\nFuture timecards will be written to:\n" +
                developerFolder,
                "Begin");

            Repaint();
        }

        private bool EnsureFolderExists(
            string path,
            string folderDescription)
        {
            if (Directory.Exists(path))
            {
                return true;
            }

            bool approved = EditorUtility.DisplayDialog(
                $"{folderDescription} Not Found",
                $"{folderDescription} does not exist:\n\n{path}\n\n" +
                "Would you like DeverQuest to create it?",
                "Create Folder",
                "Cancel");

            if (!approved)
            {
                return false;
            }

            if (DeverQuestPathUtility.TryCreateDirectory(
                    path,
                    out string errorMessage))
            {
                return true;
            }

            EditorUtility.DisplayDialog(
                "Folder Creation Failed",
                $"DeverQuest could not create:\n\n{path}\n\n{errorMessage}",
                "Close");

            return false;
        }

        private void DrawSessionDashboard()
        {
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;

            EditorGUILayout.LabelField(
                $"Welcome back, {profile.developerName}. Your quest awaits.",
                EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(
                    "Compact View",
                    GUILayout.Width(110f)))
            {
                profile.compactMode = true;
                DeverQuestSettingsStore.Save();
                Repaint();
                EditorGUILayout.EndHorizontal();
                return;
            }
            if (GUILayout.Button(
                    "Quest HUD",
                    GUILayout.Width(90f)))
            {
                DeverQuestQuestHudWindow.Open();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6f);
            DrawWorkspaceTabs();
            EditorGUILayout.Space(8f);
            if (profile.showWorkspaceHints)
            {
                DrawWorkspaceHint();
                EditorGUILayout.Space(6f);
            }

            if (DeverQuestSessionStore.HasActiveSession &&
                !guildCollapsedForActiveQuest)
            {
                guildAdministrationFoldout = false;
                guildCollapsedForActiveQuest = true;
            }
            else if (!DeverQuestSessionStore.HasActiveSession)
            {
                guildCollapsedForActiveQuest = false;
            }
            switch (activeWorkspace)
            {
                case DeverQuestWorkspace.Quest:
                    DrawGoalsAndStreaks(profile);
                    DrawWellnessReminder();
                    if (DeverQuestSessionStore.HasActiveSession)
                    {
                        DrawActiveSession(false);
                    }
                    else
                    {
                        DrawNewSessionForm(profile);
                        DrawLastCompletedSession();
                    }
                    break;
                case DeverQuestWorkspace.QuestLog:
                    DrawQuestLogWorkspace();
                    break;
                case DeverQuestWorkspace.Git:
                    DrawGitWorkspace();
                    break;
                case DeverQuestWorkspace.Visuals:
                    DrawVisualsWorkspace(profile);
                    break;
                case DeverQuestWorkspace.Administration:
                    DrawAdministrationWorkspace();
                    break;
                case DeverQuestWorkspace.Distribution:
                    DrawDistributionWorkspace();
                    break;
                case DeverQuestWorkspace.Character:
                    DrawAdventurerSheet();
                    DrawCompanionStable();
                    DrawRulesLaboratory(profile);
                    break;
                case DeverQuestWorkspace.Inventory:
                    DrawInventoryWorkspace();
                    break;
                case DeverQuestWorkspace.Economy:
                    DrawEconomyWorkspace();
                    break;
                case DeverQuestWorkspace.GuildHall:
                    DrawContentScaffolding();
                    DrawQuestRunManagement();
                    DrawGuildShop();
                    DrawGuildAdministration();
                    DrawHallOfHeroes(profile);
                    break;
                case DeverQuestWorkspace.RewardsHistory:
                    DrawRewardsPanel(profile);
                    DrawQuestRunArchive();
                    DrawHistoryPanel(profile);
                    break;
                case DeverQuestWorkspace.Chronicle:
                    DrawChronicleWorkspace(profile);
                    break;
                case DeverQuestWorkspace.Tactics:
                    DrawTacticsWorkspace();
                    break;
                case DeverQuestWorkspace.AudioWellness:
                    DrawWellnessCommandCenter(profile);
                    DrawPlaylistPlayer();
                    break;
                case DeverQuestWorkspace.Settings:
                    DrawProfileControls(profile);
                    break;
            }
        }

        private void DrawDistributionWorkspace()
        {
            EditorGUILayout.LabelField(
                "Beta Packaging and Distribution Center",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Audit the installed package, create a SHA-256 manifest, " +
                "verify release documents and licenses, export a clean " +
                "tarball, and preserve a release dossier beside the project.",
                EditorStyles.wordWrappedLabel);

            string[] channels =
            {
                "Internal Beta",
                "External Beta",
                "Release Candidate"
            };
            distributionChannelIndex = EditorGUILayout.Popup(
                "Distribution Channel",
                Mathf.Clamp(distributionChannelIndex, 0, channels.Length - 1),
                channels);
            DeverQuestDistributionChannel channel =
                (DeverQuestDistributionChannel)distributionChannelIndex;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Release Audit", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Run Packaging Audit", GUILayout.Height(28f)))
            {
                distributionReport =
                    DeverQuestDistributionService.Run(channel);
                distributionMessage =
                    distributionReport.Verdict + " · " +
                    distributionReport.Summary;
            }
            if (GUILayout.Button(
                    "Export Dossier + Verified Tarball",
                    GUILayout.Height(28f)))
            {
                try
                {
                    DeverQuestDistributionExportResult result =
                        DeverQuestDistributionService.Export(channel);
                    distributionReport = result.report;
                    distributionExportDirectory = result.outputDirectory;
                    distributionMessage = result.archiveVerified
                        ? "Release dossier exported and tarball verified."
                        : "Release dossier exported, but tarball verification failed.";
                    EditorUtility.RevealInFinder(result.outputDirectory);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                    distributionMessage =
                        "Distribution export failed: " + exception.Message;
                }
            }
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(distributionExportDirectory) ||
                       !Directory.Exists(distributionExportDirectory)))
            {
                if (GUILayout.Button("Reveal Last Release Dossier"))
                {
                    EditorUtility.RevealInFinder(distributionExportDirectory);
                }
            }

            if (!string.IsNullOrWhiteSpace(distributionMessage))
            {
                MessageType type = distributionReport != null &&
                                   distributionReport.BlockerCount > 0
                    ? MessageType.Error
                    : distributionReport != null &&
                      distributionReport.AdvisoryCount > 0
                        ? MessageType.Warning
                        : MessageType.Info;
                EditorGUILayout.HelpBox(distributionMessage, type);
            }
            EditorGUILayout.EndVertical();

            if (distributionReport == null)
            {
                EditorGUILayout.HelpBox(
                    "Run the packaging audit before exporting. External Beta " +
                    "and Release Candidate channels require an explicit " +
                    "software LICENSE; Internal Beta may proceed with a " +
                    "documented advisory.",
                    MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                distributionReport.Verdict,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(distributionReport.Summary);
            if (distributionReport.manifest != null)
            {
                EditorGUILayout.LabelField(
                    "Package Files",
                    distributionReport.manifest.fileCount.ToString());
                EditorGUILayout.LabelField(
                    "Package Bytes",
                    distributionReport.manifest.totalBytes.ToString());
            }
            EditorGUILayout.EndVertical();

            foreach (DeverQuestDistributionFinding finding in
                         distributionReport.findings)
            {
                MessageType type = string.Equals(
                    finding.severity,
                    DeverQuestDistributionSeverity.Blocker.ToString(),
                    StringComparison.Ordinal)
                    ? MessageType.Error
                    : string.Equals(
                        finding.severity,
                        DeverQuestDistributionSeverity.Advisory.ToString(),
                        StringComparison.Ordinal)
                        ? MessageType.Warning
                        : MessageType.Info;
                EditorGUILayout.HelpBox(
                    $"{finding.title}\n{finding.detail}",
                    type);
            }
        }

        private void DrawAdministrationWorkspace()
        {
            EditorGUILayout.LabelField(
                "Beta Administration and Content Validation",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Scan authored content for duplicate IDs, broken references, " +
                "incomplete Contracts, unsafe item rules, empty catalogs, and " +
                "other Beta-shipping risks.",
                EditorStyles.wordWrappedLabel);

            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Content Health", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Run Full Validation", GUILayout.Height(28f)))
            {
                administrationReport =
                    DeverQuestContentValidationService.Run();
                administrationMessage = administrationReport.Summary;
            }
            using (new EditorGUI.DisabledScope(
                       !canManage || administrationReport == null ||
                       administrationReport.RepairableCount == 0))
            {
                if (GUILayout.Button("Repair Safe Issues", GUILayout.Height(28f)))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Repair Safe Content Issues?",
                        "This may remove null list entries, restore missing " +
                        "Catalog defaults, fill blank Contract fields from " +
                        "linked Quest Profiles, and refresh editable reward " +
                        "snapshots. It will not delete valid assets or rewrite " +
                        "locked Contract rewards.",
                        "Repair",
                        "Cancel");
                    if (confirmed)
                    {
                        int repaired =
                            DeverQuestContentValidationService.RepairSafeIssues();
                        administrationReport =
                            DeverQuestContentValidationService.Run();
                        administrationMessage =
                            $"Repaired {repaired} asset(s). " +
                            administrationReport.Summary;
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(administrationReport == null))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Export Markdown Health Report"))
                {
                    string path =
                        DeverQuestContentValidationService.ExportMarkdown(
                            administrationReport);
                    administrationMessage = "Exported: " + path;
                    EditorUtility.RevealInFinder(path);
                }
                if (GUILayout.Button("Export JSON Health Report"))
                {
                    string path =
                        DeverQuestContentValidationService.ExportJson(
                            administrationReport);
                    administrationMessage = "Exported: " + path;
                    EditorUtility.RevealInFinder(path);
                }
                EditorGUILayout.EndHorizontal();
            }

            using (new EditorGUI.DisabledScope(
                       !canManage || administrationGeneratorQueued))
            {
                if (GUILayout.Button(
                        administrationGeneratorQueued
                            ? "Repairing Starter Content…"
                            : "Rerun Safe Starter Generators",
                        GUILayout.Height(26f)))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Rerun Safe Starter Generators?",
                        "This reruns the original Identity Catalog, Companion " +
                        "Stable, Tactical Starter Kit, Combat Codex, starter " +
                        "gear, Quartermaster, and training Encounter. Existing " +
                        "assets are preserved or updated by their generators.",
                        "Rerun Generators",
                        "Cancel");
                    if (confirmed)
                    {
                        administrationGeneratorQueued = true;
                        administrationMessage =
                            "Starter-content repair queued…";
                        EditorApplication.delayCall += () =>
                        {
                            try
                            {
                                administrationMessage =
                                    DeverQuestContentValidationService
                                        .RunSafeStarterRepairs();
                                administrationReport =
                                    DeverQuestContentValidationService.Run();
                            }
                            catch (Exception exception)
                            {
                                Debug.LogException(exception);
                                administrationMessage =
                                    "Starter-content repair failed: " +
                                    exception.Message;
                            }
                            finally
                            {
                                administrationGeneratorQueued = false;
                                Repaint();
                            }
                        };
                    }
                }
            }
            if (!canManage)
            {
                EditorGUILayout.HelpBox(
                    "CEO or Boss permission is required for repairs and generator reruns. Validation and exports remain available.",
                    MessageType.Info);
            }
            if (!string.IsNullOrWhiteSpace(administrationMessage))
            {
                EditorGUILayout.HelpBox(
                    administrationMessage,
                    administrationReport != null &&
                    administrationReport.ErrorCount > 0
                        ? MessageType.Error
                        : MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            if (administrationReport == null)
            {
                EditorGUILayout.HelpBox(
                    "Run Full Validation to build the current Beta health report.",
                    MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                administrationReport.Summary,
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"Safe repairs available: {administrationReport.RepairableCount}");
            administrationSearch = EditorGUILayout.TextField(
                "Search Findings",
                administrationSearch);
            string[] severities = { "All", "Errors", "Warnings", "Notes" };
            administrationSeverityIndex = EditorGUILayout.Popup(
                "Severity",
                administrationSeverityIndex,
                severities);

            IEnumerable<DeverQuestContentFinding> findings =
                administrationReport.findings;
            if (administrationSeverityIndex > 0)
            {
                DeverQuestContentFindingSeverity severity =
                    administrationSeverityIndex == 1
                        ? DeverQuestContentFindingSeverity.Error
                        : administrationSeverityIndex == 2
                            ? DeverQuestContentFindingSeverity.Warning
                            : DeverQuestContentFindingSeverity.Info;
                findings = findings.Where(value => value.severity == severity);
            }
            if (!string.IsNullOrWhiteSpace(administrationSearch))
            {
                string search = administrationSearch.Trim();
                findings = findings.Where(value =>
                    value.code.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    value.title.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    value.detail.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    value.assetPath.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            foreach (DeverQuestContentFinding finding in findings.Take(150))
            {
                MessageType messageType = finding.severity ==
                    DeverQuestContentFindingSeverity.Error
                        ? MessageType.Error
                        : finding.severity ==
                          DeverQuestContentFindingSeverity.Warning
                            ? MessageType.Warning
                            : MessageType.Info;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.HelpBox(
                    $"{finding.code} · {finding.title}\n{finding.detail}",
                    messageType);
                if (!string.IsNullOrWhiteSpace(finding.assetPath))
                {
                    EditorGUILayout.LabelField(
                        "Asset",
                        finding.assetPath,
                        EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.BeginHorizontal();
                using (new EditorGUI.DisabledScope(finding.asset == null))
                {
                    if (GUILayout.Button("Select Asset"))
                    {
                        Selection.activeObject = finding.asset;
                        EditorGUIUtility.PingObject(finding.asset);
                    }
                }
                if (GUILayout.Button("Copy Path"))
                {
                    EditorGUIUtility.systemCopyBuffer = finding.assetPath;
                }
                if (finding.safelyRepairable)
                {
                    GUILayout.Label(
                        "Safe repair available",
                        EditorStyles.miniLabel);
                }
                if (canManage &&
                    DeverQuestContentValidationService
                        .CanRegenerateStableId(finding.asset) &&
                    (finding.code == "DQ-CONTENT-101" ||
                     finding.code == "DQ-CONTENT-301"))
                {
                    List<UnityEngine.Object> duplicateGroup =
                        DeverQuestContentValidationService
                            .FindDuplicateStableIdAssets(
                                finding.asset);
                    if (duplicateGroup.Count > 1)
                    {
                        string duplicatePaths = string.Join(
                            "\n",
                            duplicateGroup
                                .Where(value => value != null)
                                .Select(value =>
                                    AssetDatabase.GetAssetPath(value)));
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.HelpBox(
                            "Duplicate group:\n" +
                            duplicatePaths,
                            MessageType.Warning);
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button(
                                "Keep This ID; Regenerate Other Copies"))
                        {
                            bool keepConfirmed =
                                EditorUtility.DisplayDialog(
                                    "Choose Stable-ID Keeper?",
                                    "The selected asset keeps its current ID. " +
                                    "Every other asset in this duplicate " +
                                    "group receives a new ID. Completion " +
                                    "history is not rewritten.\n\nKeep:\n" +
                                    finding.assetPath +
                                    "\n\nOther copies:\n" +
                                    string.Join(
                                        "\n",
                                        duplicateGroup
                                            .Where(value =>
                                                value != null &&
                                                value != finding.asset)
                                            .Select(value =>
                                                AssetDatabase
                                                    .GetAssetPath(value))),
                                    "Keep Selected Asset",
                                    "Cancel");
                            if (keepConfirmed)
                            {
                                if (DeverQuestContentValidationService
                                    .RegenerateDuplicateIdsKeeping(
                                        finding.asset,
                                        out string groupSummary,
                                        out string groupError))
                                {
                                    administrationReport =
                                        DeverQuestContentValidationService
                                            .Run();
                                    administrationMessage =
                                        groupSummary;
                                }
                                else
                                {
                                    EditorUtility.DisplayDialog(
                                        "Duplicate Group Repair Failed",
                                        groupError,
                                        "Close");
                                }
                            }
                        }
                    }

                    if (GUILayout.Button("Regenerate This Asset ID"))
                    {
                        bool confirmed =
                            EditorUtility.DisplayDialog(
                                "Regenerate Stable ID?",
                                "Use this only on the copied or newer asset " +
                                "inside a duplicate-ID group. The selected " +
                                "asset receives a new stable ID; object " +
                                "references remain, but historical records " +
                                "that stored the old ambiguous ID are not " +
                                "rewritten.\n\nAsset:\n" +
                                finding.assetPath,
                                "Regenerate ID",
                                "Cancel");
                        if (confirmed)
                        {
                            if (DeverQuestContentValidationService
                                .RegenerateStableId(
                                    finding.asset,
                                    out string previousId,
                                    out string replacementId,
                                    out string repairError))
                            {
                                administrationReport =
                                    DeverQuestContentValidationService.Run();
                                administrationMessage =
                                    "Regenerated stable ID on " +
                                    finding.assetPath + "\n" +
                                    previousId + " → " +
                                    replacementId;
                            }
                            else
                            {
                                EditorUtility.DisplayDialog(
                                    "ID Repair Failed",
                                    repairError,
                                    "Close");
                            }
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawContentScaffolding()
        {
            contentScaffoldFoldout = EditorGUILayout.Foldout(
                contentScaffoldFoldout,
                "Campaign Content Scaffolding",
                true);
            if (!contentScaffoldFoldout)
            {
                return;
            }
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Studio Content Generator",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Create an organized production tree with blank template " +
                "assets, or generate an isolated, interconnected tutorial " +
                "campaign. Existing folders and assets are always preserved.",
                EditorStyles.wordWrappedLabel);

            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                DeverQuestIdentityCatalog activeCatalog =
                    (DeverQuestIdentityCatalog)
                    EditorGUILayout.ObjectField(
                        "Active Identity Catalog",
                        DeverQuestIdentityCatalogService.ActiveCatalog,
                        typeof(DeverQuestIdentityCatalog),
                        false);
                if (activeCatalog !=
                    DeverQuestIdentityCatalogService.ActiveCatalog)
                {
                    try
                    {
                        DeverQuestIdentityCatalogService.SetActiveCatalog(
                            activeCatalog);
                        DeverQuestIdentityCatalog selectedCatalog =
                            DeverQuestIdentityCatalogService.ActiveCatalog;
                        contentScaffoldMessage = selectedCatalog == null
                            ? "No Identity Catalog assets are available."
                            : $"Active Guild Identity Catalog: " +
                              $"{selectedCatalog.displayName}.";
                    }
                    catch (Exception exception)
                    {
                        Debug.LogException(exception);
                        contentScaffoldMessage =
                            "The active Identity Catalog could not be saved: " +
                            exception.Message;
                    }
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        "Create Empty Studio Structure"))
                {
                    DeverQuestScaffoldReport report =
                        DeverQuestCampaignScaffolder
                            .CreateProductionStructure();
                    contentScaffoldMessage = report.Summary;
                    SelectGeneratedFolder(
                        report.SelectedFolder);
                }
                if (GUILayout.Button(
                        "Create Tutorial Campaign"))
                {
                    bool create = EditorUtility.DisplayDialog(
                        "Create DeverQuest Tutorial Campaign?",
                        "This creates connected tutorial assets under " +
                        "Assets/DeverQuest/DemoCampaign. Existing assets " +
                        "will not be replaced.",
                        "Create Tutorial",
                        "Cancel");
                    if (create)
                    {
                        DeverQuestScaffoldReport report =
                            DeverQuestCampaignScaffolder
                                .CreateTutorialCampaign();
                        selectedQuestContract =
                            report.TutorialContract;
                        selectedShopProfile =
                            report.TutorialShop;
                        contentScaffoldMessage =
                            report.Summary +
                            " The tutorial Contract and Quartermaster " +
                            "are now selected.";
                        SelectGeneratedFolder(
                            report.SelectedFolder);
                    }
                }
                EditorGUILayout.EndHorizontal();
                using (new EditorGUI.DisabledScope(
                           identityCatalogGenerationQueued))
                {
                    if (GUILayout.Button(
                            identityCatalogGenerationQueued
                                ? "Generating Starter Identity Catalog…"
                                : "Generate Original Starter Identity Catalog"))
                    {
                        QueueOriginalStarterIdentityCatalogGeneration(
                            true);
                    }
                }
                if (GUILayout.Button(
                        "Generate Original Companion Stable"))
                {
                    DeverQuestCompanionGenerationReport report =
                        DeverQuestCompanionCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    contentScaffoldMessage = report.Summary;
                    SelectGeneratedFolder(report.RootPath);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Ancestry Asset…"))
                {
                    CreateCharacterAsset<DeverQuestAncestry>(
                        "NewAncestry");
                }
                if (GUILayout.Button("Create Class Definition…"))
                {
                    CreateCharacterAsset<
                        DeverQuestClassDefinition>(
                        "NewClassDefinition");
                }
                if (GUILayout.Button("Create Faith Asset…"))
                {
                    CreateCharacterAsset<DeverQuestDeity>(
                        "NewFaith");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Companion Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionProfile>(
                        "NewCompanionProfile");
                }
                if (GUILayout.Button("Create Companion Catalog…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionCatalog>(
                        "NewCompanionCatalog");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button("Create Identity Catalog…"))
                {
                    CreateCharacterAsset<
                        DeverQuestIdentityCatalog>(
                        "NewGuildIdentityCatalog");
                }
            }
            if (!canManage)
            {
                EditorGUILayout.HelpBox(
                    "CEO or Boss permission is required to generate " +
                    "studio content.",
                    MessageType.Info);
            }
            if (!string.IsNullOrWhiteSpace(
                    contentScaffoldMessage))
            {
                EditorGUILayout.HelpBox(
                    contentScaffoldMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private static void SelectGeneratedFolder(
            string path)
        {
            UnityEngine.Object folder =
                AssetDatabase.LoadAssetAtPath<
                    UnityEngine.Object>(path);
            if (folder == null)
            {
                return;
            }
            Selection.activeObject = folder;
            EditorGUIUtility.PingObject(folder);
        }

        private void DrawWorkspaceTabs()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            DeverQuestWorkspace[] order =
            {
                DeverQuestWorkspace.Quest,
                DeverQuestWorkspace.QuestLog,
                DeverQuestWorkspace.Chronicle,
                DeverQuestWorkspace.Git,
                DeverQuestWorkspace.Character,
                DeverQuestWorkspace.Inventory,
                DeverQuestWorkspace.Economy,
                DeverQuestWorkspace.Tactics,
                DeverQuestWorkspace.GuildHall,
                DeverQuestWorkspace.RewardsHistory,
                DeverQuestWorkspace.AudioWellness,
                DeverQuestWorkspace.Visuals,
                DeverQuestWorkspace.Administration,
                DeverQuestWorkspace.Distribution,
                DeverQuestWorkspace.Settings
            };
            string[] labels = profile.useCompactWorkspaceLabels
                ? new[]
                {
                    "Quest",
                    "Log",
                    "Chronicle",
                    "Git",
                    "Character",
                    "Inventory",
                    "Economy",
                    "Tactics",
                    "Guild",
                    "History",
                    "Audio",
                    "Visuals",
                    "Admin",
                    "Distribution",
                    "Settings"
                }
                : new[]
                {
                    "Current Quest",
                    "Quest Log",
                    "Chronicle",
                    "Git",
                    "Character",
                    "Inventory",
                    "Economy",
                    "Tactics",
                    "Guild Hall",
                    "Rewards & History",
                    "Audio & Wellness",
                    "Visuals",
                    "Beta Administration",
                    "Packaging & Distribution",
                    "Settings"
                };

            int selectedIndex = Array.IndexOf(order, activeWorkspace);
            if (selectedIndex < 0)
            {
                selectedIndex = 0;
            }

            int nextIndex = GUILayout.SelectionGrid(
                selectedIndex,
                labels,
                profile.workspaceTabColumns,
                EditorStyles.toolbarButton);
            if (nextIndex >= 0 && nextIndex < order.Length)
            {
                activeWorkspace = order[nextIndex];
            }
        }

        private void DrawWorkspaceHint()
        {
            string message;
            switch (activeWorkspace)
            {
                case DeverQuestWorkspace.Quest:
                    message =
                        "Accept, monitor, pause, resume, and turn in the current Quest.";
                    break;
                case DeverQuestWorkspace.QuestLog:
                    message =
                        "Record work notes, link commits, attach evidence, and review the live Quest log.";
                    break;
                case DeverQuestWorkspace.Chronicle:
                    message =
                        "Review the live timeline or search completed Quest records.";
                    break;
                case DeverQuestWorkspace.Git:
                    message =
                        "Inspect the repository, create commits, and publish completed work.";
                    break;
                case DeverQuestWorkspace.Character:
                    message =
                        "Manage Adventurer identity, rules, progression, and Companions.";
                    break;
                case DeverQuestWorkspace.Inventory:
                    message =
                        "Inspect, equip, use, sell, and safely organize carried items.";
                    break;
                case DeverQuestWorkspace.Economy:
                    message =
                        "Manage Quartermaster rules, grants, coin exchange, and transaction history.";
                    break;
                case DeverQuestWorkspace.Tactics:
                    message =
                        "Review combat readiness, Companions, Encounters, and archived Battle Results.";
                    break;
                case DeverQuestWorkspace.GuildHall:
                    message =
                        "Manage Guild content, Contracts, accounts, shops, and shared records.";
                    break;
                case DeverQuestWorkspace.RewardsHistory:
                    message =
                        "Review rewards, Timecards, integrity, corrections, and compensation previews.";
                    break;
                case DeverQuestWorkspace.AudioWellness:
                    message =
                        "Control Music and Ambience, then manage reminders, quiet hours, break timing, cues, and wellness history.";
                    break;
                case DeverQuestWorkspace.Visuals:
                    message =
                        "Adjust DeverQuest colors, layout, text scale, and Quest HUD behavior.";
                    break;
                case DeverQuestWorkspace.Administration:
                    message =
                        "Validate production content, repair safe data issues, rerun starter generators, and export Beta health reports.";
                    break;
                case DeverQuestWorkspace.Distribution:
                    message =
                        "Audit package metadata, licenses, media, versions, manifests, checksums, and export a verified release dossier.";
                    break;
                default:
                    message =
                        "Configure local DeverQuest behavior, storage, rewards, and integrations.";
                    break;
            }

            EditorGUILayout.HelpBox(message, MessageType.None);
        }

        private void DrawQuestLogWorkspace()
        {
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                EditorGUILayout.HelpBox(
                    "No Quest is active. The Quest Log records notes, " +
                    "attachments, commit links, and Encounter evidence for " +
                    "the current Quest.",
                    MessageType.Info);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open Current Quest"))
                {
                    activeWorkspace = DeverQuestWorkspace.Quest;
                }
                if (GUILayout.Button("Open Completed Chronicle"))
                {
                    activeWorkspace = DeverQuestWorkspace.Chronicle;
                }
                if (GUILayout.Button("Open Git"))
                {
                    activeWorkspace = DeverQuestWorkspace.Git;
                }
                EditorGUILayout.EndHorizontal();
                return;
            }
            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;
            EditorGUILayout.LabelField(
                session.taskName,
                EditorStyles.boldLabel);
            DrawReadOnlyValue(
                "Focused",
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()));
            DeverQuestSessionStage activeStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (activeStage != null)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    EditorStyles.boldLabel);
                double stageElapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    activeStage.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    activeStage.stageTitle,
                    $"{stageElapsed / 60d:0.0} / " +
                    $"{activeStage.focusedMinutesRequired} minutes");
                if (activeStage.survivalMode ||
                    DeverQuestEncounterService.IsSurvival(activeStage))
                {
                    DrawSurvivalControls(activeStage);
                }
                else
                {
                    using (new EditorGUI.DisabledScope(
                               !activeStage.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string message);
                            EditorUtility.DisplayDialog(
                                "Encounter Pace",
                                message,
                                "Continue");
                        }
                    }
                }
            }
            DrawBattleResults(session);
            DrawCommitJournal(session);
        }

        private void DrawHallOfHeroes(
            DeverQuestProfile profile)
        {
            hallOfHeroesFoldout = EditorGUILayout.Foldout(
                hallOfHeroesFoldout,
                "Shared Guild Records and Hall of Heroes",
                true);
            if (!hallOfHeroesFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                EditorGUI.BeginChangeCheck();
                profile.sharedGuildEnabled =
                    EditorGUILayout.Toggle(
                        "Enable Shared Guild",
                        profile.sharedGuildEnabled);
                profile.publishCompletedQuests =
                    EditorGUILayout.Toggle(
                        "Publish Completed Quests",
                        profile.publishCompletedQuests);
                profile.healthyDailyFocusMinutes =
                    EditorGUILayout.IntField(
                        new GUIContent(
                            "Ranking Daily Cap",
                            "Ranked focus is capped per day to avoid " +
                            "rewarding unhealthy hours."),
                        profile.healthyDailyFocusMinutes);
                if (EditorGUI.EndChangeCheck())
                {
                    profile.Sanitize();
                    DeverQuestSettingsStore.Save();
                }
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Repository");
            EditorGUILayout.SelectableLabel(
                string.IsNullOrWhiteSpace(
                    profile.sharedGuildRepositoryPath)
                    ? "Not configured"
                    : profile.sharedGuildRepositoryPath,
                EditorStyles.textField,
                GUILayout.Height(
                    EditorGUIUtility.singleLineHeight));
            using (new EditorGUI.DisabledScope(!canManage))
            {
                if (GUILayout.Button(
                        "Choose…",
                        GUILayout.Width(72f)))
                {
                    string selected = EditorUtility.OpenFolderPanel(
                        "Choose Shared DeverQuest Guild Repository",
                        string.IsNullOrWhiteSpace(
                            profile.sharedGuildRepositoryPath)
                            ? DeverQuestPathUtility
                                .GetDefaultTimecardRoot()
                            : profile.sharedGuildRepositoryPath,
                        string.Empty);
                    if (!string.IsNullOrWhiteSpace(selected))
                    {
                        profile.sharedGuildRepositoryPath =
                            selected;
                        profile.sharedGuildEnabled = true;
                        DeverQuestSettingsStore.Save();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.HelpBox(
                "This repository coordinates tamper-evident shared records. " +
                "It becomes authoritative only when the studio controls its " +
                "server, network-share, or cloud-folder permissions. Local " +
                "users who can rewrite every file are not cryptographically " +
                "prevented from replacing history.",
                MessageType.Warning);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Validate Repository"))
            {
                DeverQuestSharedGuildService.ValidateRepository(
                    out string message);
                guildMessage = message;
            }
            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore
                           .LastCompletedSession == null))
            {
                if (GUILayout.Button("Publish Last Quest"))
                {
                    DeverQuestSharedGuildService
                        .PublishLastCompleted(
                            out string message);
                    guildMessage = message;
                }
            }
            if (GUILayout.Button("Refresh Hall"))
            {
                DeverQuestSharedGuildService.Refresh();
                guildMessage =
                    DeverQuestSharedGuildService.LastMessage;
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrWhiteSpace(guildMessage))
            {
                EditorGUILayout.HelpBox(
                    guildMessage,
                    guildMessage.Contains("failed") ||
                    guildMessage.Contains("unavailable")
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            EditorGUILayout.LabelField(
                $"Published records: " +
                $"{DeverQuestSharedGuildService.PublishedRecordCount} · " +
                $"Invalid/quarantined: " +
                $"{DeverQuestSharedGuildService.InvalidRecordCount}",
                EditorStyles.miniLabel);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Hall of Heroes",
                EditorStyles.boldLabel);
            IReadOnlyList<DeverQuestHallEntry> hall =
                DeverQuestSharedGuildService.Hall;
            if (hall.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No shared Adventurer records loaded.",
                    EditorStyles.miniLabel);
            }
            for (int index = 0; index < hall.Count; index++)
            {
                DeverQuestHallEntry entry = hall[index];
                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    $"#{index + 1} {entry.AdventurerName} · " +
                    $"{(string.IsNullOrWhiteSpace(entry.AncestryName) ? string.Empty : entry.AncestryName + " · ")}" +
                    $"{entry.CharacterClass} Level {entry.Level}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    $"{entry.DeveloperName} · {entry.GuildRank} · " +
                    $"{entry.QuestCount} Quest(s) · " +
                    $"{entry.ContractCount} Contract(s)");
                EditorGUILayout.LabelField(
                    $"Ranked {FormatDuration(entry.RankedFocusedSeconds)} · " +
                    $"Raw {FormatDuration(entry.RawFocusedSeconds)} · " +
                    $"{entry.ExperienceEarned} XP · " +
                    $"{DeverQuestAdventurerService.FormatCoins(entry.CopperEarned)}");
                EditorGUILayout.LabelField(
                    $"Current streak: {entry.CurrentStreak} day(s) · " +
                    $"Review flags: {entry.ReviewFlagCount}",
                    entry.ReviewFlagCount > 0
                        ? EditorStyles.boldLabel
                        : EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
            }

            DrawGuildNamedReports(
                "Project Standings",
                DeverQuestSharedGuildService.Projects);
            DrawGuildNamedReports(
                "Department Standings",
                DeverQuestSharedGuildService.Departments);
            EditorGUILayout.EndVertical();
        }

        private static void DrawGuildNamedReports(
            string title,
            IReadOnlyList<DeverQuestGuildNamedReport> reports)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                title,
                EditorStyles.boldLabel);
            if (reports.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No shared data.",
                    EditorStyles.miniLabel);
                return;
            }
            foreach (DeverQuestGuildNamedReport report in reports)
            {
                EditorGUILayout.LabelField(
                    report.Name,
                    $"{FormatDuration(report.FocusedSeconds)} · " +
                    $"{report.QuestCount} Quest(s) · " +
                    $"{report.AdventurerCount} Adventurer(s)");
            }
        }

        private void DrawCompactDashboard(DeverQuestProfile profile)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                $"DEVERQUEST · {profile.developerName}",
                titleStyle);

            if (GUILayout.Button("Quest HUD", GUILayout.Width(82f)))
            {
                DeverQuestQuestHudWindow.Open();
            }
            if (GUILayout.Button("Full View", GUILayout.Width(90f)))
            {
                profile.compactMode = false;
                DeverQuestSettingsStore.Save();
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            DrawWellnessReminder();
            DrawGoalsAndStreaks(profile);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            AudioClip track = DeverQuestPlaylistPlayer.CurrentTrack;
            EditorGUILayout.LabelField(
                "Music",
                track == null ? "No track selected" : track.name);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Previous"))
            {
                DeverQuestPlaylistPlayer.Previous();
            }
            if (GUILayout.Button(
                    DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing
                        ? "Pause"
                        : "Play"))
            {
                if (DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing)
                {
                    DeverQuestPlaylistPlayer.Pause();
                }
                else
                {
                    DeverQuestPlaylistPlayer.Play();
                }
            }
            if (GUILayout.Button("Next"))
            {
                DeverQuestPlaylistPlayer.Next();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(
                "Ambience",
                DeverQuestAudioDirector.CurrentAmbience == null
                    ? "None"
                    : DeverQuestAudioDirector
                        .CurrentAmbience.name);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button(
                    DeverQuestAudioDirector.AmbiencePlaying
                        ? "Stop Ambience"
                        : "Play Ambience"))
            {
                if (DeverQuestAudioDirector.AmbiencePlaying)
                {
                    DeverQuestAudioDirector.StopAmbience();
                }
                else
                {
                    DeverQuestAudioDirector.PlayAmbience();
                }
            }
            if (GUILayout.Button("Next Ambience"))
            {
                DeverQuestAudioDirector.NextAmbience();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();

            if (!DeverQuestSessionStore.HasActiveSession)
            {
                EditorGUILayout.HelpBox(
                    "No active quest. Switch to Full View to accept one.",
                    MessageType.Info);
            }
            else
            {
                DeverQuestSession session =
                    DeverQuestSessionStore.ActiveSession;
                AnnounceCompletedStages();
                EditorGUILayout.LabelField(
                    FormatDuration(
                        DeverQuestSessionStore.GetFocusedSeconds()),
                    timerStyle);
                EditorGUILayout.LabelField(
                    $"{session.projectName} · {session.taskName}",
                    subtitleStyle);
                DrawCompactQuestLog(session);

                if (showFinalization)
                {
                    DrawFinalizationPanel(session);
                }
                else if (!session.idlePauseAcknowledged)
                {
                    EditorGUILayout.HelpBox(
                        $"This quest entered meditation because: " +
                        $"{session.pauseReason}. Acknowledge your return " +
                        "before resuming.",
                        MessageType.Warning);
                    if (GUILayout.Button(
                            "I Have Returned — Acknowledge"))
                    {
                        DeverQuestSessionStore.AcknowledgeIdlePause();
                        Repaint();
                    }
                }
                else
                {
                    if (session.state ==
                        DeverQuestSessionState.Paused)
                    {
                        DrawMeditationRecoveryStatus();
                    }

                    EditorGUILayout.BeginHorizontal();
                    if (session.state ==
                        DeverQuestSessionState.Running)
                    {
                        if (GUILayout.Button("Meditate"))
                        {
                            DeverQuestSessionStore.PauseSession();
                        }
                    }
                    else if (GUILayout.Button("Resume"))
                    {
                        DeverQuestSessionStage currentStage =
                            DeverQuestSessionStore.CurrentQuestStage();
                        if (currentStage?.survivalFightPaused == true)
                        {
                            if (DeverQuestSessionStore
                                .ContinueSurvival(
                                    out string continueMessage))
                            {
                                DeverQuestSessionStore.ResumeSession();
                            }
                            else
                            {
                                EditorUtility.DisplayDialog(
                                    "Survival Expedition",
                                    continueMessage,
                                    "Close");
                            }
                        }
                        else
                        {
                            DeverQuestSessionStore.ResumeSession();
                        }
                    }

                    if (GUILayout.Button("Complete Quest"))
                    {
                        BeginFinalization(session);
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Coin Purse",
                EditorStyles.boldLabel);
            DeverQuestAdventurer compactAdventurer =
                DeverQuestAdventurerService.Adventurer;
            string compactAncestryName =
                string.IsNullOrWhiteSpace(
                    compactAdventurer.ancestryName)
                    ? "Legacy Ancestry"
                    : compactAdventurer.ancestryName;
            EditorGUILayout.LabelField(
                $"{compactAncestryName} · " +
                $"{compactAdventurer.characterClass} · " +
                $"Level {compactAdventurer.level}",
                DeverQuestAdventurerService.FormatCoins(
                    compactAdventurer.copperBalance));
            EditorGUILayout.LabelField(
                $"HP {compactAdventurer.currentHitPoints}/" +
                $"{compactAdventurer.maximumHitPoints} · " +
                $"Mana {compactAdventurer.currentMana}/" +
                $"{compactAdventurer.maximumMana} · " +
                $"AC {DeverQuestRulesService.ArmorClass(compactAdventurer)}");
            DeverQuestCompanionState compactCompanion =
                DeverQuestCompanionService.ActiveCompanion(
                    compactAdventurer);
            if (compactCompanion != null)
            {
                DeverQuestCompanionProfile compactProfile =
                    DeverQuestCompanionService.FindProfile(
                        compactCompanion.profileId);
                EditorGUILayout.LabelField(
                    "Active Companion",
                    $"{DeverQuestCompanionService.DisplayName(compactCompanion)} · " +
                    $"Level {compactCompanion.level} · " +
                    $"HP {compactCompanion.currentHitPoints}/" +
                    $"{DeverQuestCompanionService.MaximumHitPoints(compactCompanion, compactProfile)}");
            }
            EditorGUILayout.LabelField(
                $"STR {compactAdventurer.strength} · " +
                $"DEX {compactAdventurer.dexterity} · " +
                $"AGI {compactAdventurer.agility} · " +
                $"STA {compactAdventurer.stamina} · " +
                $"INT {compactAdventurer.intelligence} · " +
                $"WIS {compactAdventurer.wisdom} · " +
                $"Luck {compactAdventurer.luck}",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                $"Hunger {compactAdventurer.hunger} · " +
                $"Rest {compactAdventurer.rest} · " +
                $"Happiness {compactAdventurer.happiness}");
        }

        private void AnnounceCompletedStages()
        {
            foreach (string title in
                     DeverQuestSessionStore.UpdateQuestStages())
            {
                ShowNotification(
                    new GUIContent(
                        $"Encounter Complete: {title}"),
                    5d);
                if (DeverQuestSettingsStore.Profile
                    .notificationSoundsEnabled)
                {
                    if (!DeverQuestAudioDirector.PlayCue(
                            DeverQuestAudioCue.StageComplete))
                    {
                        EditorApplication.Beep();
                    }
                }
            }
        }

        private static void DrawCompactQuestLog(
            DeverQuestSession session)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Quest Log",
                EditorStyles.boldLabel);
            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.LabelField(
                    session.questStory,
                    EditorStyles.wordWrappedLabel);
            }
            EditorGUILayout.LabelField(
                "Objective",
                string.IsNullOrWhiteSpace(session.goal)
                    ? "No objective recorded."
                    : session.goal,
                EditorStyles.wordWrappedLabel);
            if (session.questIsGroupQuest)
            {
                EditorGUILayout.LabelField(
                    $"Party ({session.questMaximumParticipants} max)",
                    session.questPartyMembers);
            }
            if (session.questStages != null)
            {
                foreach (DeverQuestSessionStage stage
                         in session.questStages)
                {
                    EditorGUILayout.LabelField(
                        stage.completed ? "✓ " + stage.stageTitle
                            : "○ " + stage.stageTitle,
                        $"{stage.focusedMinutesRequired}m" +
                        (stage.completedEarly
                            ? " · Early"
                            : string.Empty) +
                        (stage.survivalMode
                            ? $" · Wave {stage.survivalWave}"
                            : string.Empty) +
                        (string.IsNullOrWhiteSpace(
                             stage.assignedPartyRole)
                            ? string.Empty
                            : " · " + stage.assignedPartyRole));
                }
            }
            DeverQuestSessionStage current =
                DeverQuestSessionStore.CurrentQuestStage();
            if (current != null)
            {
                double elapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    current.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    "Current Stage Pace",
                    $"{elapsed / 60d:0.0} / " +
                    $"{current.focusedMinutesRequired} focused minutes");
                if (!current.survivalMode &&
                    !DeverQuestEncounterService.IsSurvival(current))
                {
                    using (new EditorGUI.DisabledScope(
                               !current.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string paceMessage);
                            EditorUtility.DisplayDialog(
                                "Encounter Pace",
                                paceMessage,
                                "Continue");
                        }
                    }
                }
                else
                {
                    DrawSurvivalControls(current);
                }
            }
            DrawBattleResults(session);
            EditorGUILayout.EndVertical();
        }

        private static void DrawSurvivalControls(
            DeverQuestSessionStage stage)
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            float weight =
                DeverQuestEncumbranceService.CarriedWeight(adventurer);
            float capacity =
                DeverQuestEncumbranceService.CarryCapacity(adventurer);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Survival Expedition",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                DeverQuestEncounterService.EncounterDisplayName(stage),
                $"Completed waves {stage.survivalWave} · Carry " +
                $"{weight:0.0}/{capacity:0.0}");
            EditorGUILayout.LabelField(
                "Expedition Pace",
                DeverQuestEncounterService.DescribeSurvivalProgress(stage),
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                "Exit Status",
                stage.survivalExitOffered
                    ? "Guild Wagon available at this checkpoint."
                    : "Flee or use a prepared return ability; the Guild " +
                      "Wagon has not reached this checkpoint.",
                EditorStyles.wordWrappedLabel);
            if (stage.survivalFightPaused)
            {
                EditorGUILayout.HelpBox(
                    "Fight paused for safety: " +
                    stage.survivalPauseReason,
                    MessageType.Warning);
            }
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Attempt Flee"))
            {
                DeverQuestSessionStore.TryExitSurvival(
                    "Flee", out string message);
                EditorUtility.DisplayDialog(
                    "Survival Escape", message, "Close");
            }
            using (new EditorGUI.DisabledScope(
                       !DeverQuestTacticalCombatService
                           .HasReturnAbility(adventurer)))
            {
                if (GUILayout.Button("Use Homeward Passage"))
                {
                    DeverQuestSessionStore.TryExitSurvival(
                        "Return", out string message);
                    EditorUtility.DisplayDialog(
                        "Homeward Passage", message, "Close");
                }
            }
            using (new EditorGUI.DisabledScope(
                       !stage.survivalExitOffered))
            {
                if (GUILayout.Button("Take Guild Wagon"))
                {
                    DeverQuestSessionStore.TryExitSurvival(
                        "Wagon", out string message);
                    EditorUtility.DisplayDialog(
                        "Guild Wagon", message, "Close");
                }
            }
            EditorGUILayout.EndHorizontal();
            if (DeverQuestEncumbranceService.IsEncumbered(adventurer))
            {
                EditorGUILayout.HelpBox(
                    "Encumbered. Drop something before continuing. " +
                    "Coin may only be exchanged at the Guild Hall.",
                    MessageType.Warning);
                foreach (DeverQuestInventoryEntry item in
                         adventurer.inventory.ToArray())
                {
                    if (GUILayout.Button(
                            $"Drop 1 × {item.displayName} " +
                            $"({item.unitWeight:0.##} wt)"))
                    {
                        DeverQuestEncumbranceService.DropInventory(
                            item.ownershipId, 1, out _);
                        break;
                    }
                }
            }
            if (stage.survivalEndedSafely &&
                !string.IsNullOrWhiteSpace(stage.survivalExitSummary))
            {
                EditorGUILayout.HelpBox(
                    $"Returned safely via " +
                    $"{DeverQuestCombatSummaryService.FriendlyExitMethod(stage.survivalExitMethod)}.\n" +
                    stage.survivalExitSummary,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawBattleResults(
            DeverQuestSession session)
        {
            if (session.battleResults == null ||
                session.battleResults.Count == 0)
            {
                DeverQuestSessionStage stage =
                    DeverQuestSessionStore.CurrentQuestStage();
                if (stage != null &&
                    !string.IsNullOrWhiteSpace(stage.encounterProfileId))
                {
                    EditorGUILayout.Space(5f);
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(
                        "Tactical Encounter Preview",
                        EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(
                        DeverQuestEncounterService.EncounterDisplayName(stage),
                        DeverQuestEncounterService.DescribeEncounter(stage),
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.EndVertical();
                }
                return;
            }

            EditorGUILayout.Space(5f);
            EditorGUILayout.LabelField(
                "Tactical Field Reports",
                EditorStyles.boldLabel);
            DeverQuestBattleResult[] reports =
                session.battleResults
                    .Where(value => value != null)
                    .Reverse()
                    .ToArray();
            for (int index = 0; index < reports.Length; index++)
            {
                DeverQuestBattleResult battle = reports[index];
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                string status =
                    DeverQuestCombatSummaryService.OutcomeTitle(battle);
                string encounterName = string.IsNullOrWhiteSpace(
                    battle.encounterName)
                    ? "Encounter"
                    : battle.encounterName;
                EditorGUILayout.LabelField(
                    $"{status} · {encounterName}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    "Outcome",
                    DeverQuestCombatSummaryService.OutcomeSummary(battle),
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(
                    "Damage Report",
                    DeverQuestCombatSummaryService.DamageSummary(
                        battle,
                        session.developerName,
                        DeverQuestAdventurerService.Adventurer
                            .characterName),
                    EditorStyles.wordWrappedLabel);

                string conditionSummary =
                    DeverQuestCombatSummaryService.ConditionSummary(battle);
                if (!string.IsNullOrWhiteSpace(conditionSummary))
                {
                    EditorGUILayout.LabelField(
                        "Conditions and Reactions",
                        conditionSummary,
                        EditorStyles.wordWrappedLabel);
                }

                string companionSummary =
                    DeverQuestCombatSummaryService
                        .CompanionContributionSummary(battle);
                if (!string.IsNullOrWhiteSpace(companionSummary))
                {
                    EditorGUILayout.LabelField(
                        "Companion Contribution",
                        companionSummary,
                        EditorStyles.wordWrappedLabel);
                }

                if (battle.defeatedMonsters.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Defeated",
                        DeverQuestCombatSummaryService
                            .GroupedDefeatedMonsters(battle));
                }
                if (battle.loot.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Loot",
                        string.Join(", ", battle.loot),
                        EditorStyles.wordWrappedLabel);
                }
                if (!string.IsNullOrWhiteSpace(battle.injury))
                {
                    EditorGUILayout.LabelField(
                        "Consequence",
                        battle.injury,
                        EditorStyles.wordWrappedLabel);
                }
                if (!string.IsNullOrWhiteSpace(
                        battle.safetyPauseReason))
                {
                    EditorGUILayout.HelpBox(
                        battle.safetyPauseReason,
                        MessageType.Warning);
                }

                IReadOnlyList<string> highlights =
                    DeverQuestCombatSummaryService.Highlights(battle, 8);
                if (highlights.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Recent Turns",
                        EditorStyles.boldLabel);
                    foreach (string highlight in highlights)
                    {
                        EditorGUILayout.LabelField(
                            "• " + highlight,
                            EditorStyles.wordWrappedLabel);
                    }
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Full Combat Log"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        DeverQuestCombatSummaryService
                            .BuildFullCombatReport(battle);
                }
                if (GUILayout.Button("Copy Seed"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        battle.seed ?? string.Empty;
                }
                EditorGUILayout.EndHorizontal();

                if (index == 0)
                {
                    EditorGUILayout.LabelField(
                        "Latest resolved battle",
                        EditorStyles.miniLabel);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private static void DrawGoalsAndStreaks(
            DeverQuestProfile profile)
        {
            if (!DeverQuestHistoryService.IsLoaded)
            {
                DeverQuestHistoryService.Refresh(profile);
            }

            DeverQuestGoalStatistics statistics =
                DeverQuestHistoryService.BuildGoalStatistics(
                    profile.dailyWorkGoalMinutes);

            double todaySeconds = statistics.TodayFocusedSeconds;
            if (DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSession active =
                    DeverQuestSessionStore.ActiveSession;
                if (DeverQuestSessionStore
                        .GetLocalStartTime(active).Date ==
                    DateTime.Today)
                {
                    todaySeconds +=
                        DeverQuestSessionStore.GetFocusedSeconds();
                }
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Daily Decree and Questing Streaks",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                $"{profile.campaignDifficulty} campaign · " +
                $"Recommended Level " +
                $"{profile.dailyDecreeRecommendedLevel} · " +
                $"Checks {(profile.dailyDecreeCheckModifier >= 0 ? "+" : string.Empty)}" +
                $"{profile.dailyDecreeCheckModifier}");

            if (profile.dailyWorkGoalMinutes <= 0)
            {
                EditorGUILayout.LabelField(
                    "Daily goal is disabled. Set it in Profile settings.");
            }
            else
            {
                double goalSeconds =
                    profile.dailyWorkGoalMinutes * 60d;
                float progress =
                    Mathf.Clamp01((float)(todaySeconds / goalSeconds));
                Rect rect = GUILayoutUtility.GetRect(
                    18f,
                    18f,
                    GUILayout.ExpandWidth(true));
                EditorGUI.ProgressBar(
                    rect,
                    progress,
                    $"{FormatDuration(todaySeconds)} / " +
                    $"{FormatDuration(goalSeconds)}");
                EditorGUILayout.LabelField(
                    $"Current streak: {statistics.CurrentStreak} day(s) · " +
                    $"Longest: {statistics.LongestStreak} · " +
                    $"Goal days: {statistics.GoalDays}");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawChronicleWorkspace(
            DeverQuestProfile profile)
        {
            EditorGUILayout.LabelField(
                "Quest Archive and Chronicle",
                titleStyle);
            EditorGUILayout.LabelField(
                "Follow the active Quest as it unfolds, then review completed " +
                "work without hunting through separate files and tabs.",
                wrappedLabelStyle);
            EditorGUILayout.Space(8f);

            DrawLiveQuestChronicle();

            chronicleArchiveFoldout = EditorGUILayout.Foldout(
                chronicleArchiveFoldout,
                "Completed Quest Archive",
                true);
            if (!chronicleArchiveFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!DeverQuestHistoryService.IsLoaded)
            {
                DeverQuestHistoryService.Refresh(profile);
            }

            EditorGUILayout.BeginHorizontal();
            chronicleSearch = EditorGUILayout.TextField(
                "Search",
                chronicleSearch);
            if (GUILayout.Button("Refresh", GUILayout.Width(72f)))
            {
                DeverQuestHistoryService.Refresh(profile);
                chronicleMessage = "Quest archive refreshed.";
            }
            EditorGUILayout.EndHorizontal();

            string[] filterLabels =
            {
                "All Completed Quests",
                "Contract Runs",
                "With Rewards",
                "With Commits or Notes",
                "With Media",
                "With Combat"
            };
            chronicleFilterIndex = EditorGUILayout.Popup(
                "Archive Filter",
                Mathf.Clamp(
                    chronicleFilterIndex,
                    0,
                    filterLabels.Length - 1),
                filterLabels);
            chronicleResultLimit = EditorGUILayout.IntSlider(
                "Visible Results",
                chronicleResultLimit,
                5,
                100);

            DeverQuestQuestArchiveFilter filter =
                (DeverQuestQuestArchiveFilter)chronicleFilterIndex;
            List<DeverQuestQuestArchiveRecord> records =
                DeverQuestQuestArchiveService.BuildRecords(
                    DeverQuestHistoryService.AllDays,
                    chronicleSearch,
                    filter);
            DeverQuestQuestArchiveSummary summary =
                DeverQuestQuestArchiveService.BuildSummary(records);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Archive Summary",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                summary.QuestCount + " Quest(s) · " +
                FormatDuration(summary.FocusedSeconds) + " focused · " +
                DeverQuestAdventurerService.FormatCoins(
                    summary.CopperEarned) + " · " +
                summary.ExperienceEarned + " XP",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                summary.CommitCount + " notes/commits · " +
                summary.MediaCount + " media attachment(s) · " +
                summary.BattleCount + " battle report(s)",
                EditorStyles.miniLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Expand Visible"))
            {
                foreach (DeverQuestQuestArchiveRecord record
                         in records.Take(chronicleResultLimit))
                {
                    if (record?.Session != null)
                    {
                        chronicleExpandedSessions.Add(
                            record.Session.sessionId);
                    }
                }
            }
            if (GUILayout.Button("Collapse All"))
            {
                chronicleExpandedSessions.Clear();
            }
            EditorGUILayout.EndHorizontal();

            if (records.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No completed Quests match the current archive filter.",
                    MessageType.Info);
            }
            else
            {
                int visible = Math.Min(
                    chronicleResultLimit,
                    records.Count);
                EditorGUILayout.LabelField(
                    "Showing " + visible + " of " + records.Count +
                    " matching Quest(s).",
                    EditorStyles.miniLabel);
                for (int index = 0; index < visible; index++)
                {
                    DrawQuestArchiveRecord(records[index]);
                }
            }

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestHistoryService.LastError))
            {
                EditorGUILayout.HelpBox(
                    DeverQuestHistoryService.LastError,
                    MessageType.Warning);
            }
            if (!string.IsNullOrWhiteSpace(chronicleMessage))
            {
                EditorGUILayout.HelpBox(
                    chronicleMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawLiveQuestChronicle()
        {
            chronicleLiveFoldout = EditorGUILayout.Foldout(
                chronicleLiveFoldout,
                "Live Quest Chronicle",
                true);
            if (!chronicleLiveFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (!DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSession latest =
                    DeverQuestSessionStore.LastCompletedSession;
                EditorGUILayout.HelpBox(
                    "No Quest is active. The completed archive below remains " +
                    "available for review.",
                    MessageType.Info);
                if (latest != null)
                {
                    EditorGUILayout.LabelField(
                        "Latest Completed Quest",
                        EditorStyles.boldLabel);
                    EditorGUILayout.LabelField(
                        string.IsNullOrWhiteSpace(latest.taskName)
                            ? "Quest"
                            : latest.taskName,
                        EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField(
                        DeverQuestQuestArchiveService
                            .BuildReadableSummary(latest),
                        EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.EndVertical();
                return;
            }

            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;
            EditorGUILayout.LabelField(
                string.IsNullOrWhiteSpace(session.taskName)
                    ? "Active Quest"
                    : session.taskName,
                EditorStyles.boldLabel);
            DrawReadOnlyValue(
                "State",
                DeverQuestQuestArchiveService.StatusLabel(session));
            DrawReadOnlyValue(
                "Focused",
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()));
            DrawReadOnlyValue(
                "Paused",
                FormatDuration(
                    DeverQuestSessionStore.GetPausedSeconds()));
            if (!string.IsNullOrWhiteSpace(session.questContractRunId))
            {
                DrawReadOnlyValue(
                    "Quest Run",
                    session.questContractRunId);
            }
            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.LabelField(
                    "Quest Story",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.questStory,
                    EditorStyles.wordWrappedLabel);
            }
            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.LabelField(
                    "Task Objective",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.goal,
                    EditorStyles.wordWrappedLabel);
            }

            DeverQuestSessionStage stage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (stage != null)
            {
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    string.IsNullOrWhiteSpace(stage.stageTitle)
                        ? "Encounter"
                        : stage.stageTitle,
                    EditorStyles.wordWrappedLabel);
            }

            DrawQuestEventFeed(session, 12, true);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Current Quest"))
            {
                activeWorkspace = DeverQuestWorkspace.Quest;
            }
            if (GUILayout.Button("Open Quest Log"))
            {
                activeWorkspace = DeverQuestWorkspace.QuestLog;
            }
            if (GUILayout.Button("Open Git"))
            {
                activeWorkspace = DeverQuestWorkspace.Git;
            }
            if (GUILayout.Button("Copy Live Summary"))
            {
                EditorGUIUtility.systemCopyBuffer =
                    DeverQuestQuestArchiveService
                        .BuildReadableSummary(session);
                chronicleMessage = "Live Quest summary copied.";
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawCompactQuestEventFeed(
            DeverQuestSession session)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "Recent Quest Events",
                EditorStyles.boldLabel);
            if (GUILayout.Button(
                    "Open Chronicle",
                    GUILayout.Width(110f)))
            {
                activeWorkspace = DeverQuestWorkspace.Chronicle;
            }
            EditorGUILayout.EndHorizontal();
            DrawQuestEventFeed(session, 5, true);
            EditorGUILayout.EndVertical();
        }

        private static void DrawQuestEventFeed(
            DeverQuestSession session,
            int maximum,
            bool newestFirst)
        {
            List<DeverQuestQuestEvent> events =
                DeverQuestQuestArchiveService.BuildTimeline(
                    session,
                    newestFirst);
            if (events.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No Chronicle events have been recorded yet.",
                    EditorStyles.miniLabel);
                return;
            }

            int count = Math.Min(Math.Max(1, maximum), events.Count);
            for (int index = 0; index < count; index++)
            {
                DeverQuestQuestEvent questEvent = events[index];
                DateTime local =
                    DeverQuestQuestArchiveService.LocalEventTime(
                        questEvent.UtcTicks);
                string time = local == DateTime.MinValue
                    ? string.Empty
                    : local.ToString("h:mm tt") + " · ";
                EditorGUILayout.LabelField(
                    time + questEvent.Category + " · " + questEvent.Title,
                    EditorStyles.boldLabel);
                if (!string.IsNullOrWhiteSpace(questEvent.Detail))
                {
                    EditorGUILayout.LabelField(
                        questEvent.Detail,
                        EditorStyles.wordWrappedLabel);
                }
            }
        }

        private void DrawQuestArchiveRecord(
            DeverQuestQuestArchiveRecord record)
        {
            DeverQuestSession session = record?.Session;
            if (session == null)
            {
                return;
            }

            string sessionKey = string.IsNullOrWhiteSpace(session.sessionId)
                ? record.DataPath + ":" + session.taskName
                : session.sessionId;
            bool expanded = chronicleExpandedSessions.Contains(sessionKey);
            string title =
                record.Date.ToString("yyyy-MM-dd") + " · " +
                (string.IsNullOrWhiteSpace(session.taskName)
                    ? "Quest"
                    : session.taskName);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            bool nextExpanded = EditorGUILayout.Foldout(
                expanded,
                title,
                true);
            if (nextExpanded != expanded)
            {
                if (nextExpanded)
                {
                    chronicleExpandedSessions.Add(sessionKey);
                }
                else
                {
                    chronicleExpandedSessions.Remove(sessionKey);
                }
                expanded = nextExpanded;
            }

            EditorGUILayout.LabelField(
                (string.IsNullOrWhiteSpace(session.projectName)
                    ? "Unspecified Project"
                    : session.projectName) + " · " +
                (string.IsNullOrWhiteSpace(session.category)
                    ? "Uncategorized"
                    : session.category),
                EditorStyles.miniLabel);
            EditorGUILayout.LabelField(
                FormatDuration(session.accumulatedFocusedSeconds) +
                " focused · " +
                DeverQuestQuestArchiveService.RewardSummary(session) +
                " · Integrity " + record.IntegrityStatus,
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(
                       !File.Exists(record.MarkdownPath)))
            {
                if (GUILayout.Button("Open Timecard"))
                {
                    DeverQuestIdleMonitor.BeginIntentionalExternalAction();
                    EditorUtility.OpenWithDefaultApp(record.MarkdownPath);
                }
                if (GUILayout.Button("Reveal"))
                {
                    DeverQuestIdleMonitor.BeginIntentionalExternalAction();
                    EditorUtility.RevealInFinder(record.MarkdownPath);
                }
            }
            if (GUILayout.Button("Copy Summary"))
            {
                EditorGUIUtility.systemCopyBuffer =
                    DeverQuestQuestArchiveService
                        .BuildReadableSummary(session);
                chronicleMessage = "Quest summary copied.";
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(
                           session.questContractRunId)))
            {
                if (GUILayout.Button("Copy Run ID"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        session.questContractRunId;
                    chronicleMessage = "Quest Run ID copied.";
                }
            }
            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(
                           session.questContractId)))
            {
                if (GUILayout.Button("Select Contract"))
                {
                    SelectChronicleContract(session.questContractId);
                }
            }
            if (GUILayout.Button("Request Correction"))
            {
                correctionDataPath = record.DataPath;
                correctionSessionId = session.sessionId;
                correctionSessionTitle = session.taskName;
                correctionReason = string.Empty;
                correctionValue = string.Empty;
                activeWorkspace = DeverQuestWorkspace.RewardsHistory;
                historyFoldout = true;
                historyRange = DeverQuestHistoryRange.AllTime;
                historyProjectFilter = string.Empty;
                historyCategoryFilter = string.Empty;
                historyMessage =
                    "The selected Quest is ready for a correction request.";
            }
            EditorGUILayout.EndHorizontal();

            if (expanded)
            {
                DrawQuestArchiveDetails(record);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawQuestArchiveDetails(
            DeverQuestQuestArchiveRecord record)
        {
            DeverQuestSession session = record.Session;
            EditorGUILayout.Space(5f);
            DrawReadOnlyValue(
                "Developer",
                session.developerName);
            DrawReadOnlyValue(
                "Started",
                DeverQuestSessionStore
                    .GetLocalStartTime(session)
                    .ToString("g"));
            if (session.completedUtcTicks > 0L)
            {
                DateTime completed =
                    DeverQuestQuestArchiveService.LocalEventTime(
                        session.completedUtcTicks);
                DrawReadOnlyValue(
                    "Completed",
                    completed == DateTime.MinValue
                        ? "Unknown"
                        : completed.ToString("g"));
            }
            if (!string.IsNullOrWhiteSpace(session.questContractTitle))
            {
                DrawReadOnlyValue(
                    "Contract",
                    session.questContractTitle);
            }
            if (!string.IsNullOrWhiteSpace(session.questContractRunId))
            {
                DrawReadOnlyValue(
                    "Quest Run",
                    session.questContractRunId);
            }
            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.LabelField(
                    "Quest Story",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.questStory,
                    EditorStyles.wordWrappedLabel);
            }
            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.LabelField(
                    "Task Objective",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.goal,
                    EditorStyles.wordWrappedLabel);
            }
            if (!string.IsNullOrWhiteSpace(
                    session.questContractDeliverables))
            {
                EditorGUILayout.LabelField(
                    "Deliverables",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.questContractDeliverables,
                    EditorStyles.wordWrappedLabel);
            }
            if (!string.IsNullOrWhiteSpace(session.closingNotes))
            {
                EditorGUILayout.LabelField(
                    "Closing Notes",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.closingNotes,
                    EditorStyles.wordWrappedLabel);
            }

            if (session.rewardTransactions != null &&
                session.rewardTransactions.Count > 0)
            {
                EditorGUILayout.LabelField(
                    "Reward Journal",
                    EditorStyles.boldLabel);
                foreach (DeverQuestRewardTransaction reward
                         in session.rewardTransactions)
                {
                    if (reward == null)
                    {
                        continue;
                    }
                    EditorGUILayout.LabelField(
                        "• " +
                        (string.IsNullOrWhiteSpace(reward.transactionType)
                            ? reward.categoryName
                            : reward.transactionType) + " · " +
                        DeverQuestAdventurerService.FormatCoins(
                            reward.copper) + " · " +
                        reward.experience + " XP",
                        EditorStyles.wordWrappedLabel);
                }
            }

            if (session.commitEntries != null &&
                session.commitEntries.Count > 0)
            {
                EditorGUILayout.LabelField(
                    "Quest Log and Commits",
                    EditorStyles.boldLabel);
                foreach (DeverQuestCommitEntry entry
                         in session.commitEntries)
                {
                    if (entry == null)
                    {
                        continue;
                    }
                    EditorGUILayout.LabelField(
                        "• " + entry.comment +
                        (string.IsNullOrWhiteSpace(entry.commitHash)
                            ? string.Empty
                            : " · " + entry.commitHash),
                        EditorStyles.wordWrappedLabel);
                }
            }

            DrawArchivedMedia(session);

            if (session.battleResults != null &&
                session.battleResults.Count > 0)
            {
                EditorGUILayout.LabelField(
                    "Tactical Reports",
                    EditorStyles.boldLabel);
                foreach (DeverQuestBattleResult battle
                         in session.battleResults)
                {
                    if (battle == null)
                    {
                        continue;
                    }
                    EditorGUILayout.LabelField(
                        "• " +
                        DeverQuestCombatSummaryService.OutcomeTitle(battle) +
                        " · " +
                        (string.IsNullOrWhiteSpace(battle.encounterName)
                            ? "Encounter"
                            : battle.encounterName),
                        EditorStyles.wordWrappedLabel);
                }
            }

            EditorGUILayout.LabelField(
                "Chronicle Timeline",
                EditorStyles.boldLabel);
            DrawQuestEventFeed(session, 20, false);

            if (record.IntegrityStatus ==
                DeverQuestIntegrityStatus.Modified)
            {
                EditorGUILayout.HelpBox(
                    record.IntegrityMessage,
                    MessageType.Warning);
            }
        }

        private void DrawArchivedMedia(
            DeverQuestSession session)
        {
            if (session.mediaAttachments == null ||
                session.mediaAttachments.Count == 0)
            {
                return;
            }

            EditorGUILayout.LabelField(
                "Media Attachments",
                EditorStyles.boldLabel);
            foreach (DeverQuestMediaAttachment attachment
                     in session.mediaAttachments)
            {
                if (attachment == null)
                {
                    continue;
                }
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    string.IsNullOrWhiteSpace(attachment.displayName)
                        ? "Attachment"
                        : attachment.displayName,
                    EditorStyles.wordWrappedLabel);
                bool exists = File.Exists(attachment.filePath);
                using (new EditorGUI.DisabledScope(!exists))
                {
                    if (GUILayout.Button(
                            "Open",
                            GUILayout.Width(54f)))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.OpenWithDefaultApp(
                            attachment.filePath);
                    }
                    if (GUILayout.Button(
                            "Reveal",
                            GUILayout.Width(58f)))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.RevealInFinder(
                            attachment.filePath);
                    }
                }
                if (GUILayout.Button(
                        "Copy Path",
                        GUILayout.Width(72f)))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        attachment.filePath ?? string.Empty;
                    chronicleMessage = "Attachment path copied.";
                }
                EditorGUILayout.EndHorizontal();
                if (!exists)
                {
                    EditorGUILayout.HelpBox(
                        "The attachment file is no longer present at its " +
                        "recorded path.",
                        MessageType.Warning);
                }
            }
        }

        private void SelectChronicleContract(string contractId)
        {
            DeverQuestQuestContract contract =
                DeverQuestContractService.Find(contractId);
            if (contract == null)
            {
                chronicleMessage =
                    "The source Quest Contract asset could not be found.";
                return;
            }
            selectedQuestContract = contract;
            Selection.activeObject = contract;
            EditorGUIUtility.PingObject(contract);
            chronicleMessage =
                "Selected Contract: " + contract.contractTitle;
        }

        private void DrawHistoryPanel(DeverQuestProfile profile)
        {
            historyFoldout = EditorGUILayout.Foldout(
                historyFoldout,
                "History and Reporting",
                true);

            if (!historyFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (!DeverQuestHistoryService.IsLoaded)
            {
                DeverQuestHistoryService.Refresh(profile);
            }

            EditorGUILayout.BeginHorizontal();

            historyRange =
                (DeverQuestHistoryRange)EditorGUILayout.EnumPopup(
                    "Date Range",
                    historyRange);

            if (GUILayout.Button(
                    "Refresh",
                    GUILayout.Width(72f)))
            {
                DeverQuestHistoryService.Refresh(profile);
                historyMessage = "History refreshed.";
            }

            EditorGUILayout.EndHorizontal();

            if (historyRange == DeverQuestHistoryRange.Custom)
            {
                historyStartDate =
                    EditorGUILayout.TextField(
                        "Start (yyyy-MM-dd)",
                        historyStartDate);

                historyEndDate =
                    EditorGUILayout.TextField(
                        "End (yyyy-MM-dd)",
                        historyEndDate);
            }

            historyProjectFilter =
                EditorGUILayout.TextField(
                    "Project Contains",
                    historyProjectFilter);

            historyCategoryFilter =
                EditorGUILayout.TextField(
                    "Department Contains",
                    historyCategoryFilter);

            if (!TryGetHistoryDateRange(
                    out DateTime? startDate,
                    out DateTime? endDate,
                    out string dateError))
            {
                EditorGUILayout.HelpBox(
                    dateError,
                    MessageType.Error);

                EditorGUILayout.EndVertical();
                return;
            }

            List<DeverQuestHistoryDay> days =
                DeverQuestHistoryService.GetFilteredDays(
                    startDate,
                    endDate,
                    historyProjectFilter,
                    historyCategoryFilter);

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestHistoryService.LastError))
            {
                EditorGUILayout.HelpBox(
                    DeverQuestHistoryService.LastError,
                    MessageType.Error);
            }

            DrawHistorySummary(days);
            DrawCompensationPreview(
                profile,
                days,
                startDate,
                endDate);
            DrawNamedHistorySummary(
                "Weekly Summary",
                DeverQuestHistoryService.BuildWeeklySummaries(days));

            DrawNamedHistorySummary(
                "Project Totals",
                DeverQuestHistoryService.BuildProjectSummaries(days));

            DrawNamedHistorySummary(
                "Department Totals",
                DeverQuestHistoryService.BuildCategorySummaries(days));

            DrawWalletStatistics();
            DrawHistoryDays(days);
            DrawNewChronicleControl(profile);
            DrawHistoryExport(profile, days);

            if (!string.IsNullOrWhiteSpace(historyMessage))
            {
                EditorGUILayout.HelpBox(
                    historyMessage,
                    MessageType.Info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawCompensationPreview(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> filteredDays,
            DateTime? startDate,
            DateTime? endDate)
        {
            compensationPreviewFoldout = EditorGUILayout.Foldout(
                compensationPreviewFoldout,
                "Compensation Preview",
                true);
            if (!compensationPreviewFoldout)
            {
                return;
            }

            DeverQuestGuildAccount account =
                DeverQuestGuildAccountService.CurrentAccount;
            if (account == null)
            {
                return;
            }
            if (!account.compensationPreviewEnabled)
            {
                EditorGUILayout.HelpBox(
                    "No optional compensation preview policy is enabled for " +
                    "this Adventurer. A Boss or CEO can configure one in " +
                    "Guild Hall > Guild Accounts and Authority.",
                    MessageType.Info);
                return;
            }

            DeverQuestCompensationPreview preview =
                DeverQuestCompensationService.BuildPreview(
                    account,
                    profile,
                    filteredDays);
            DateTime today = DateTime.Today;
            int daysSinceMonday =
                ((int)today.DayOfWeek + 6) % 7;
            DateTime weekStart =
                today.AddDays(-daysSinceMonday);
            List<DeverQuestHistoryDay> currentWeekDays =
                DeverQuestHistoryService.GetFilteredDays(
                    weekStart,
                    today,
                    historyProjectFilter,
                    historyCategoryFilter);
            DeverQuestCompensationPreview weekPreview =
                DeverQuestCompensationService.BuildPreview(
                    account,
                    profile,
                    currentWeekDays);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawReadOnlyValue(
                "Policy",
                DeverQuestCompensationService.DescribeBasis(account));
            DrawReadOnlyValue(
                "Current Week Eligible",
                FormatDuration(weekPreview.EligibleSeconds));
            DrawReadOnlyValue(
                "Current Week Estimate",
                DeverQuestCompensationService.FormatMoney(
                    account,
                    weekPreview.EstimatedGross));
            DrawReadOnlyValue(
                "Filtered Eligible",
                FormatDuration(preview.EligibleSeconds));
            DrawReadOnlyValue(
                "Filtered Estimate",
                DeverQuestCompensationService.FormatMoney(
                    account,
                    preview.EstimatedGross));
            DrawReadOnlyValue(
                "Included",
                $"{preview.IncludedChronicles} Chronicle(s) · " +
                $"{preview.IncludedSessions} Quest(s)");
            DrawReadOnlyValue(
                "Eligible Time",
                account.compensationIncludeApprovedBreaks
                    ? "Focused + Approved Breaks"
                    : "Focused only");

            if (preview.ExcludedModifiedSeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.ExcludedModifiedSeconds)} " +
                    "was excluded because its Chronicle was modified or " +
                    "could not be verified.",
                    MessageType.Warning);
            }
            if (preview.ExcludedLegacySeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.ExcludedLegacySeconds)} of " +
                    "legacy/unsealed time was excluded by this policy.",
                    MessageType.Info);
            }
            if (preview.FlaggedSeconds > 0d)
            {
                EditorGUILayout.HelpBox(
                    $"{FormatDuration(preview.FlaggedSeconds)} is included " +
                    "but matches a configured long/frequent Quest flag and " +
                    "should be reviewed manually.",
                    MessageType.Warning);
            }
            EditorGUILayout.HelpBox(
                DeverQuestCompensationService.Disclaimer,
                MessageType.Warning);

            if (filteredDays.Count > 0 &&
                GUILayout.Button("Export Filtered Compensation Preview…"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export Compensation Preview",
                    GetDeveloperFolder(profile),
                    $"DeverQuest_Compensation_Preview_" +
                    $"{DateTime.Now:yyyy-MM-dd}",
                    "csv");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string rangeDescription =
                        startDate.HasValue || endDate.HasValue
                            ? $"{startDate?.ToString("yyyy-MM-dd") ?? "All"} " +
                              $"through " +
                              $"{endDate?.ToString("yyyy-MM-dd") ?? "All"}"
                            : "All filtered Chronicle history";
                    historyMessage =
                        DeverQuestCompensationService.TryExportPreview(
                            path,
                            account,
                            preview,
                            rangeDescription,
                            out string error)
                            ? $"Compensation Preview exported:\n{path}"
                            : $"Compensation Preview export failed: {error}";
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void DrawHistorySummary(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            DeverQuestHistorySummary summary =
                DeverQuestHistoryService.BuildSummary(days);

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Filtered Totals",
                EditorStyles.boldLabel);

            DrawReadOnlyValue(
                "Days",
                summary.DayCount.ToString());

            DrawReadOnlyValue(
                "Sessions",
                summary.SessionCount.ToString());

            DrawReadOnlyValue(
                "Focused",
                FormatDuration(summary.FocusedSeconds));

            DrawReadOnlyValue(
                "Paused",
                FormatDuration(summary.PausedSeconds));

            DrawReadOnlyValue(
                "Commits",
                summary.CommitCount.ToString());

            DrawReadOnlyValue(
                "Breaks",
                summary.BreakCount.ToString());

            DrawReadOnlyValue(
                "Coin Earned",
                DeverQuestAdventurerService.FormatCoins(
                    summary.CopperEarned));

            DrawReadOnlyValue(
                "Coin Spent",
                DeverQuestAdventurerService.FormatCoins(
                    summary.CopperSpent));

            DrawReadOnlyValue(
                "Experience Earned",
                $"{summary.ExperienceEarned} XP");
        }

        private static void DrawNamedHistorySummary(
            string title,
            IReadOnlyList<DeverQuestNamedSummary> summaries)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                title,
                EditorStyles.boldLabel);

            if (summaries.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No matching data.",
                    EditorStyles.miniLabel);
                return;
            }

            foreach (DeverQuestNamedSummary summary in summaries)
            {
                EditorGUILayout.LabelField(
                    summary.Name,
                    $"{FormatDuration(summary.FocusedSeconds)} · " +
                    $"{summary.SessionCount} session(s)");
            }
        }

        private static void DrawWalletStatistics()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Adventurer Treasury",
                EditorStyles.boldLabel);
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Physical Coin",
                $"{adventurer.platinumCoins}p · " +
                $"{adventurer.goldCoins}g · " +
                $"{adventurer.silverCoins}s · " +
                $"{adventurer.copperCoins}c · " +
                $"{DeverQuestAdventurerService.CoinPieceCount(adventurer)} pieces");
            EditorGUILayout.LabelField(
                "Carry Weight",
                $"{DeverQuestEncumbranceService.CarriedWeight(adventurer):0.0} / " +
                $"{DeverQuestEncumbranceService.CarryCapacity(adventurer):0.0}" +
                (DeverQuestEncumbranceService.IsEncumbered(adventurer)
                    ? " · ENCUMBERED"
                    : string.Empty));
            EditorGUILayout.LabelField(
                "Lifetime Earned",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperEarned));
            EditorGUILayout.LabelField(
                "Lifetime Spent",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperSpent));
        }

        private void DrawHistoryDays(
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Daily Timecards",
                EditorStyles.boldLabel);

            if (days.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No timecards match the current filters.",
                    MessageType.Info);
                return;
            }

            foreach (DeverQuestHistoryDay day in days)
            {
                DeverQuestHistorySummary summary =
                    DeverQuestHistoryService.BuildSummary(
                        new List<DeverQuestHistoryDay> { day });

                EditorGUILayout.BeginVertical(
                    EditorStyles.helpBox);

                EditorGUILayout.LabelField(
                    day.Date.ToString("dddd, MMMM d, yyyy"),
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    $"Chronicle {Math.Max(1, day.Record.chronicleIndex)} · " +
                    $"Integrity: {day.IntegrityStatus}");
                EditorGUILayout.HelpBox(
                    day.IntegrityMessage,
                    day.IntegrityStatus ==
                    DeverQuestIntegrityStatus.Modified
                        ? MessageType.Warning
                        : MessageType.None);

                EditorGUILayout.LabelField(
                    $"{summary.SessionCount} session(s) · " +
                    $"{FormatDuration(summary.FocusedSeconds)} focused");
                int dailyLimit = DeverQuestSettingsStore.Profile
                    .suspiciousDailyQuestCount;
                if (day.SuspiciousSessionCount > 0 ||
                    day.SuspiciousFrequency)
                {
                    EditorGUILayout.HelpBox(
                        $"{day.SuspiciousSessionCount} unusually long " +
                        $"Quest(s); daily frequency threshold " +
                        $"{dailyLimit}. Flagged for leadership review, not " +
                        "automatically rejected.",
                        MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();

                using (new EditorGUI.DisabledScope(
                           !File.Exists(day.MarkdownPath)))
                {
                    if (GUILayout.Button("Open Timecard"))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.OpenWithDefaultApp(
                            day.MarkdownPath);
                    }

                    if (GUILayout.Button("Reveal Timecard"))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.RevealInFinder(
                            day.MarkdownPath);
                    }
                }

                EditorGUILayout.EndHorizontal();

                foreach (DeverQuestSession session
                         in day.Record.sessions)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        session.taskName,
                        $"{FormatDuration(session.accumulatedFocusedSeconds)} " +
                        "focused");
                    if (GUILayout.Button(
                            "Request Correction",
                            GUILayout.Width(132f)))
                    {
                        correctionDataPath = day.DataPath;
                        correctionSessionId = session.sessionId;
                        correctionSessionTitle = session.taskName;
                        correctionReason = string.Empty;
                        correctionValue = string.Empty;
                    }
                    EditorGUILayout.EndHorizontal();
                }

                DrawCorrectionEditor(day);
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawCorrectionEditor(DeverQuestHistoryDay day)
        {
            if (correctionDataPath == day.DataPath &&
                !string.IsNullOrWhiteSpace(correctionSessionId))
            {
                EditorGUILayout.Space(5f);
                EditorGUILayout.LabelField(
                    $"Correction: {correctionSessionTitle}",
                    EditorStyles.boldLabel);
                correctionReason = EditorGUILayout.TextField(
                    "Reason", correctionReason);
                correctionValue = EditorGUILayout.TextArea(
                    correctionValue, GUILayout.MinHeight(48f));
                using (new EditorGUI.DisabledScope(
                           string.IsNullOrWhiteSpace(correctionReason) ||
                           string.IsNullOrWhiteSpace(correctionValue)))
                {
                    if (GUILayout.Button(
                            "Append Correction Request"))
                    {
                        DeverQuestChronicleIntegrityService.AddCorrection(
                            day.DataPath,
                            correctionSessionId,
                            correctionSessionTitle,
                            DeverQuestSettingsStore.Profile.developerName,
                            correctionReason,
                            correctionValue);
                        DeverQuestTimecardWriter.TryRegenerate(
                            day.DataPath, out string regenerateError);
                        historyMessage =
                            string.IsNullOrWhiteSpace(regenerateError)
                                ? "Correction request appended."
                                : regenerateError;
                        correctionSessionId = string.Empty;
                        DeverQuestHistoryService.Refresh(
                            DeverQuestSettingsStore.Profile);
                    }
                }
            }

            List<DeverQuestCorrection> corrections =
                DeverQuestChronicleIntegrityService.LoadCorrections(
                    day.DataPath);
            foreach (DeverQuestCorrection correction in corrections)
            {
                DeverQuestSession correctedSession =
                    day.Record.sessions.FirstOrDefault(
                        item => item.sessionId ==
                                correction.sessionId);
                string correctionProject =
                    correctedSession?.projectName ?? string.Empty;
                bool leadership =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ReviewCorrections,
                        correctionProject);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    $"{correction.sessionTitle} · {correction.status}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    correction.reason,
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(
                    correction.correctedValue,
                    EditorStyles.wordWrappedLabel);
                if (leadership && correction.status == "Pending")
                {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Approve"))
                    {
                        ReviewCorrection(
                            day, correction, "Approved");
                    }
                    if (GUILayout.Button("Return"))
                    {
                        ReviewCorrection(
                            day, correction, "Returned");
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void ReviewCorrection(
            DeverQuestHistoryDay day,
            DeverQuestCorrection correction,
            string status)
        {
            DeverQuestSession correctedSession =
                day.Record.sessions.FirstOrDefault(
                    item => item.sessionId == correction.sessionId);
            DeverQuestChronicleIntegrityService.ReviewCorrection(
                day.DataPath,
                correction.correctionId,
                status,
                DeverQuestSettingsStore.Profile.developerName,
                correctedSession?.projectName ?? string.Empty);
            DeverQuestTimecardWriter.TryRegenerate(
                day.DataPath, out string error);
            historyMessage = string.IsNullOrWhiteSpace(error)
                ? $"Correction {status.ToLowerInvariant()}."
                : error;
            DeverQuestHistoryService.Refresh(
                DeverQuestSettingsStore.Profile);
        }

        private void DrawNewChronicleControl(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Chronicle Rollover",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Start a fresh numbered Chronicle for today. Existing " +
                "Chronicles are never overwritten.",
                MessageType.Info);
            if (GUILayout.Button("Start New Chronicle"))
            {
                string developerFolder =
                    DeverQuestPathUtility.GetDeveloperFolder(
                        profile.timecardRootPath,
                        profile.developerName);
                int index =
                    DeverQuestChronicleIntegrityService
                        .StartNewChronicle(
                            developerFolder,
                            DateTime.Now.ToString("yyyy-MM-dd"));
                historyMessage =
                    $"Chronicle {index} will receive the next completed Quest.";
            }
        }

        private void DrawHistoryExport(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Export Filtered Report",
                EditorStyles.boldLabel);

            using (new EditorGUI.DisabledScope(days.Count == 0))
            {
                EditorGUILayout.BeginHorizontal();

                if (GUILayout.Button("Export CSV"))
                {
                    ExportHistoryCsv(profile, days);
                }

                if (GUILayout.Button("Export JSON"))
                {
                    ExportHistoryJson(profile, days);
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        private void ExportHistoryCsv(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            string path = EditorUtility.SaveFilePanel(
                "Export DeverQuest CSV",
                GetDeveloperFolder(profile),
                $"DeverQuest_Report_{DateTime.Now:yyyy-MM-dd}",
                "csv");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            historyMessage =
                DeverQuestHistoryService.TryExportCsv(
                    path,
                    days,
                    out string error)
                    ? $"CSV exported:\n{path}"
                    : $"CSV export failed: {error}";
        }

        private void ExportHistoryJson(
            DeverQuestProfile profile,
            IReadOnlyList<DeverQuestHistoryDay> days)
        {
            string path = EditorUtility.SaveFilePanel(
                "Export DeverQuest JSON",
                GetDeveloperFolder(profile),
                $"DeverQuest_Report_{DateTime.Now:yyyy-MM-dd}",
                "json");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            historyMessage =
                DeverQuestHistoryService.TryExportJson(
                    path,
                    profile.developerName,
                    days,
                    out string error)
                    ? $"JSON exported:\n{path}"
                    : $"JSON export failed: {error}";
        }

        private bool TryGetHistoryDateRange(
            out DateTime? startDate,
            out DateTime? endDate,
            out string errorMessage)
        {
            DateTime today = DateTime.Today;
            startDate = null;
            endDate = null;
            errorMessage = string.Empty;

            switch (historyRange)
            {
                case DeverQuestHistoryRange.Today:
                    startDate = today;
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Last7Days:
                    startDate = today.AddDays(-6);
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Last30Days:
                    startDate = today.AddDays(-29);
                    endDate = today;
                    return true;
                case DeverQuestHistoryRange.Custom:
                    if (!DateTime.TryParseExact(
                            historyStartDate,
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedStart) ||
                        !DateTime.TryParseExact(
                            historyEndDate,
                            "yyyy-MM-dd",
                            System.Globalization.CultureInfo.InvariantCulture,
                            System.Globalization.DateTimeStyles.None,
                            out DateTime parsedEnd))
                    {
                        errorMessage =
                            "Custom dates must use yyyy-MM-dd.";
                        return false;
                    }

                    if (parsedEnd < parsedStart)
                    {
                        errorMessage =
                            "The end date cannot be before the start date.";
                        return false;
                    }

                    startDate = parsedStart;
                    endDate = parsedEnd;
                    return true;
                default:
                    return true;
            }
        }

        private static string GetDeveloperFolder(
            DeverQuestProfile profile)
        {
            return DeverQuestPathUtility.GetDeveloperFolder(
                profile.timecardRootPath,
                profile.developerName);
        }

        private void DrawPlaylistSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Session Music",
                EditorStyles.boldLabel);

            profile.autoPlayMusicOnSessionStart =
                EditorGUILayout.Toggle(
                    "Play on Session Start",
                    profile.autoPlayMusicOnSessionStart);

            profile.pauseMusicWithSession =
                EditorGUILayout.Toggle(
                    "Pause with Session",
                    profile.pauseMusicWithSession);

            profile.resumeMusicWithSession =
                EditorGUILayout.Toggle(
                    "Resume with Session",
                    profile.resumeMusicWithSession);

            profile.stopMusicOnSessionEnd =
                EditorGUILayout.Toggle(
                    "Stop on Session End",
                    profile.stopMusicOnSessionEnd);
        }

        private static void DrawExternalActivitySetup()
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "External Activity",
                EditorStyles.boldLabel);

            DeverQuestExternalActivityProfile profile =
                (DeverQuestExternalActivityProfile)
                EditorGUILayout.ObjectField(
                    new GUIContent(
                        "Activity Profile",
                        "Foreground creative applications that may keep the " +
                        "Quest active while recent input is detected."),
                    DeverQuestExternalActivityMonitor.Profile,
                    typeof(DeverQuestExternalActivityProfile),
                    false);
            if (profile !=
                DeverQuestExternalActivityMonitor.Profile)
            {
                DeverQuestExternalActivityMonitor.SetProfile(
                    profile);
            }

            EditorGUILayout.HelpBox(
                "External activity never counts a permanently idle app as " +
                "work. The configured tool must be foreground and must have " +
                "recent keyboard or pointer input. Windows foreground " +
                "detection is supported in this milestone.",
                MessageType.Info);

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild) &&
                GUILayout.Button(
                    "Create Aseprite Activity Profile…"))
            {
                CreateAsepriteActivityProfile();
            }
        }

        private static void CreateAsepriteActivityProfile()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create Aseprite Activity Profile",
                "AsepriteActivityProfile",
                "asset",
                "Choose where to save the external activity profile.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestExternalActivityProfile profile =
                CreateInstance<DeverQuestExternalActivityProfile>();
            profile.displayName = "Pixel Art Workshop";
            profile.description =
                "Aseprite foreground input is accepted as deliberate " +
                "external Quest activity.";
            profile.providers.Add(
                new DeverQuestExternalActivityProvider
                {
                    displayName = "Aseprite",
                    processName = "aseprite",
                    inputFreshnessSeconds = 30,
                    enabled = true
                });
            AssetDatabase.CreateAsset(profile, path);
            AssetDatabase.SaveAssets();
            DeverQuestExternalActivityMonitor.SetProfile(
                profile);
            Selection.activeObject = profile;
            EditorGUIUtility.PingObject(profile);
        }

        private static void DrawPolishSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Appearance and Notifications",
                EditorStyles.boldLabel);

            profile.theme =
                (DeverQuestTheme)EditorGUILayout.EnumPopup(
                    "Theme",
                    profile.theme);

            profile.showEditorNotifications =
                EditorGUILayout.Toggle(
                    "Editor Notifications",
                    profile.showEditorNotifications);

            profile.notificationSoundsEnabled =
                EditorGUILayout.Toggle(
                    "Notification Sounds",
                    profile.notificationSoundsEnabled);

            profile.autoOpenWindowForReminders =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Open Window for Reminders",
                        "When disabled, reminders never force a closed " +
                        "DeverQuest window to open."),
                    profile.autoOpenWindowForReminders);
        }

        private static void DrawChronicleIntegritySetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Chronicle Integrity and Review",
                EditorStyles.boldLabel);
            profile.chronicleIntegrityEnabled =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Integrity Seals",
                        "Creates a SHA-256 audit chain. This reveals casual " +
                        "edits; it is not an authoritative server signature."),
                    profile.chronicleIntegrityEnabled);
            profile.chronicleMaxSessions =
                EditorGUILayout.IntField(
                    "Quests per Chronicle",
                    profile.chronicleMaxSessions);
            profile.chronicleMaxKilobytes =
                EditorGUILayout.IntField(
                    "Chronicle Size (KB)",
                    profile.chronicleMaxKilobytes);
            profile.suspiciousQuestMinutes =
                EditorGUILayout.IntField(
                    "Flag Quest at (min)",
                    profile.suspiciousQuestMinutes);
            profile.suspiciousDailyQuestCount =
                EditorGUILayout.IntField(
                    "Flag Daily Quest Count",
                    profile.suspiciousDailyQuestCount);
            EditorGUILayout.HelpBox(
                "Integrity hashes reveal changes but cannot stop a person " +
                "with local file access from replacing files. Authoritative " +
                "records require a future shared Guild service.",
                MessageType.Info);
        }

        private static void DrawProjectDefaultsSetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Quest Project Defaults",
                EditorStyles.boldLabel);

            profile.lockProjectName = EditorGUILayout.Toggle(
                new GUIContent(
                    "Lock Project Name",
                    "Useful when this package belongs to one company project."),
                profile.lockProjectName);

            using (new EditorGUI.DisabledScope(!profile.lockProjectName))
            {
                profile.lockedProjectName = EditorGUILayout.TextField(
                    "Locked Project",
                    profile.lockedProjectName);
            }
        }

        private static void ApplyFocusSchedule(
            DeverQuestProfile profile,
            string scheduleText)
        {
            List<int> parsed = new List<int>();
            string[] values =
                (scheduleText ?? string.Empty).Split(',');

            foreach (string value in values)
            {
                if (int.TryParse(value.Trim(), out int minutes) &&
                    minutes > 0 &&
                    !parsed.Contains(minutes))
                {
                    parsed.Add(minutes);
                }
            }

            parsed.Sort();
            profile.focusCheckInScheduleMinutes = parsed;
        }

        private void DrawPlaylistPlayer()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Playlist Player",
                EditorStyles.boldLabel);

            DeverQuestPlaylist selected =
                (DeverQuestPlaylist)EditorGUILayout.ObjectField(
                    "Playlist",
                    DeverQuestPlaylistPlayer.Playlist,
                    typeof(DeverQuestPlaylist),
                    false);

            if (selected != DeverQuestPlaylistPlayer.Playlist)
            {
                DeverQuestPlaylistPlayer.SetPlaylist(selected);
            }

            if (selected == null)
            {
                if (GUILayout.Button("Create Playlist Asset"))
                {
                    CreatePlaylistAsset();
                    GUIUtility.ExitGUI();
                }

                EditorGUILayout.HelpBox(
                    "Create a playlist, select it, then add AudioClips " +
                    "in its Inspector.",
                    MessageType.Info);

                DrawAudioProfiles();
                EditorGUILayout.EndVertical();
                return;
            }

            AudioClip current =
                DeverQuestPlaylistPlayer.CurrentTrack;

            EditorGUILayout.LabelField(
                "Now Playing",
                current == null
                    ? "No track selected"
                    : current.name);

            if (selected.TrackCount > 0)
            {
                string[] trackOptions = selected.Tracks
                    .Select((clip, index) =>
                        clip == null
                            ? $"{index + 1}. Missing AudioClip"
                            : $"{index + 1}. {clip.name}")
                    .ToArray();
                int selectedTrackIndex = EditorGUILayout.Popup(
                    "Select Track",
                    Mathf.Clamp(
                        DeverQuestPlaylistPlayer.TrackIndex,
                        0,
                        trackOptions.Length - 1),
                    trackOptions);
                if (selectedTrackIndex !=
                    DeverQuestPlaylistPlayer.TrackIndex)
                {
                    DeverQuestPlaylistPlayer.SelectTrack(
                        selectedTrackIndex);
                }
            }

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Previous"))
            {
                DeverQuestPlaylistPlayer.Previous();
            }

            string playLabel =
                DeverQuestPlaylistPlayer.State ==
                DeverQuestPlaybackState.Playing
                    ? "Pause"
                    : "Play";

            if (GUILayout.Button(playLabel))
            {
                if (DeverQuestPlaylistPlayer.State ==
                    DeverQuestPlaybackState.Playing)
                {
                    DeverQuestPlaylistPlayer.Pause();
                }
                else
                {
                    DeverQuestPlaylistPlayer.Play();
                }
            }

            if (GUILayout.Button("Next"))
            {
                DeverQuestPlaylistPlayer.Next();
            }

            if (GUILayout.Button("Stop"))
            {
                DeverQuestPlaylistPlayer.Stop();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.BeginChangeCheck();

            selected.Shuffle =
                EditorGUILayout.Toggle(
                    "Shuffle",
                    selected.Shuffle);

            selected.RepeatMode =
                (DeverQuestRepeatMode)EditorGUILayout.EnumPopup(
                    "Repeat",
                    selected.RepeatMode);

            selected.Volume =
                EditorGUILayout.Slider(
                    "Volume",
                    selected.Volume,
                    0f,
                    1f);

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(selected);
                DeverQuestPlaylistPlayer.ApplyVolume();
            }

            if (!DeverQuestAudioTransport.VolumeSupported)
            {
                EditorGUILayout.HelpBox(
                    "The active audio transport does not expose volume " +
                    "control. Playback may still work, but mixer changes " +
                    "cannot be applied reliably.",
                    MessageType.Warning);
            }

            if (!DeverQuestAudioTransport.PlaybackStatusSupported)
            {
                EditorGUILayout.HelpBox(
                    "The active audio transport cannot report playback " +
                    "completion. Use Next manually when a track ends.",
                    MessageType.Warning);
            }

            if (!DeverQuestAudioTransport.IndependentVolumeSupported)
            {
                EditorGUILayout.HelpBox(
                    "DeverQuest is using the legacy preview fallback. " +
                    "Playback remains available, but Unity exposes only " +
                    "global preview gain in this mode, so Music and " +
                    "Ambience cannot be mixed independently.",
                    MessageType.Warning);
            }

            if (!string.IsNullOrWhiteSpace(
                    DeverQuestPlaylistPlayer.LastError))
            {
                EditorGUILayout.HelpBox(
                    DeverQuestPlaylistPlayer.LastError,
                    MessageType.Error);
            }

            if (GUILayout.Button("Select Playlist in Project"))
            {
                Selection.activeObject = selected;
                EditorGUIUtility.PingObject(selected);
            }

            DrawAudioProfiles();
            EditorGUILayout.EndVertical();
        }

        private void DrawAudioProfiles()
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Warnings and Ambience",
                EditorStyles.boldLabel);
            DrawAudioMixerSettings();
            EditorGUILayout.Space(6f);
            EditorGUI.BeginChangeCheck();
            DeverQuestWarningAudioProfile warning =
                (DeverQuestWarningAudioProfile)
                EditorGUILayout.ObjectField(
                    "Warning Profile",
                    DeverQuestAudioDirector.WarningProfile,
                    typeof(DeverQuestWarningAudioProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                DeverQuestAudioDirector.SetWarningProfile(
                    warning);
            }

            EditorGUI.BeginChangeCheck();
            DeverQuestAmbienceProfile ambience =
                (DeverQuestAmbienceProfile)
                EditorGUILayout.ObjectField(
                    "Ambience Profile",
                    DeverQuestAudioDirector.AmbienceProfile,
                    typeof(DeverQuestAmbienceProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                DeverQuestAudioDirector.SetAmbienceProfile(
                    ambience);
                ambience =
                    DeverQuestAudioDirector.AmbienceProfile;
                GUI.FocusControl(null);
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Warning Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestWarningAudioProfile>(
                        "NewWarningAudioProfile");
                }
                if (GUILayout.Button("Create Ambience Profile…"))
                {
                    DeverQuestAmbienceProfile created =
                        CreateCharacterAsset<
                            DeverQuestAmbienceProfile>(
                            "NewAmbienceProfile");
                    if (created != null)
                    {
                        DeverQuestAudioDirector.SetAmbienceProfile(
                            created);
                        ambience = created;
                    }
                }
                EditorGUILayout.EndHorizontal();

                DeverQuestAmbienceProfile selectedAmbience =
                    Selection.activeObject as
                        DeverQuestAmbienceProfile;
                using (new EditorGUI.DisabledScope(
                           selectedAmbience == null ||
                           selectedAmbience == ambience))
                {
                    if (GUILayout.Button(
                            "Use Selected Ambience Profile"))
                    {
                        DeverQuestAudioDirector.SetAmbienceProfile(
                            selectedAmbience);
                        ambience = selectedAmbience;
                        Repaint();
                    }
                }
            }

            if (warning != null)
            {
                EditorGUI.BeginChangeCheck();
                warning.volume = EditorGUILayout.Slider(
                    "Warning Volume",
                    warning.volume,
                    0f,
                    1f);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(warning);
                }
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Test Warning"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.IdleWarning);
                }
                if (GUILayout.Button("Test Victory"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.EncounterVictory);
                }
                if (GUILayout.Button("Test Level Up"))
                {
                    DeverQuestAudioDirector.PlayCue(
                        DeverQuestAudioCue.LevelUp);
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Select Warning Profile in Project"))
                {
                    Selection.activeObject = warning;
                    EditorGUIUtility.PingObject(warning);
                }
            }

            if (ambience != null)
            {
                int ambienceClipCount =
                    ambience.ambienceClips == null
                        ? 0
                        : ambience.ambienceClips.Count(
                            clip => clip != null);
                if (ambienceClipCount == 0)
                {
                    EditorGUILayout.HelpBox(
                        "The selected Ambience Profile is assigned, but it " +
                        "does not contain a playable AudioClip. Select the " +
                        "profile in the Project window and add at least one " +
                        "clip to Ambience Clips.",
                        MessageType.Warning);
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Playable Ambience Clips",
                        ambienceClipCount.ToString());
                }

                EditorGUI.BeginChangeCheck();
                ambience.volume = EditorGUILayout.Slider(
                    "Ambience Volume",
                    ambience.volume,
                    0f,
                    1f);
                ambience.playDuringActiveQuest =
                    EditorGUILayout.Toggle(
                        "Quest-Aware Ambience",
                        ambience.playDuringActiveQuest);
                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(ambience);
                    DeverQuestAudioDirector.ApplyVolumes();
                }
                string[] ambienceOptions =
                    ambience.ambienceClips
                        .Select((clip, index) =>
                            clip == null
                                ? $"{index + 1}. Missing AudioClip"
                                : $"{index + 1}. {clip.name}")
                        .ToArray();
                if (ambienceOptions.Length > 0)
                {
                    int selectedAmbienceIndex =
                        EditorGUILayout.Popup(
                            "Select Ambience",
                            Mathf.Clamp(
                                DeverQuestAudioDirector.AmbienceIndex,
                                0,
                                ambienceOptions.Length - 1),
                            ambienceOptions);
                    if (selectedAmbienceIndex !=
                        DeverQuestAudioDirector.AmbienceIndex)
                    {
                        DeverQuestAudioDirector.SelectAmbience(
                            selectedAmbienceIndex);
                    }
                }
                EditorGUILayout.LabelField(
                    "Now Playing",
                    DeverQuestAudioDirector.CurrentAmbience == null
                        ? "No ambience playing"
                        : DeverQuestAudioDirector
                            .CurrentAmbience.name);
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        DeverQuestAudioDirector.AmbiencePlaying
                            ? "Stop Ambience"
                            : "Play Ambience"))
                {
                    if (DeverQuestAudioDirector.AmbiencePlaying)
                    {
                        DeverQuestAudioDirector.StopAmbience();
                    }
                    else
                    {
                        DeverQuestAudioDirector.PlayAmbience();
                    }
                }
                if (GUILayout.Button("Next Ambience"))
                {
                    DeverQuestAudioDirector.NextAmbience();
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Select Ambience Profile in Project"))
                {
                    Selection.activeObject = ambience;
                    EditorGUIUtility.PingObject(ambience);
                }
            }

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Recover Active Audio"))
            {
                bool recovered =
                    DeverQuestAudioDirector.RecoverAudioTransport();
                ShowNotification(
                    new GUIContent(
                        recovered
                            ? "Audio transport recovered."
                            : "Audio transport could not be recovered."),
                    3d);
            }
            if (GUILayout.Button("Stop and Reset All Audio"))
            {
                if (EditorUtility.DisplayDialog(
                        "Reset DeverQuest Audio?",
                        "This stops the supported audio host and the legacy " +
                        "preview fallback, then clears DeverQuest Music, " +
                        "Ambience, and warning playback state.",
                        "Reset Audio",
                        "Cancel"))
                {
                    DeverQuestAudioDirector.ResetAllAudio();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(
                DeverQuestAudioTransport.InspectorPreviewIsolated
                    ? "The supported AudioSource host is isolated from " +
                      "Unity's Inspector preview controls. Music, Ambience, " +
                      "and cues use separate sources and independent gain."
                    : "The compatibility fallback shares Unity's Inspector " +
                      "preview transport. Use Recover after previewing a " +
                      "clip, or reinitialize the supported host.",
                DeverQuestAudioTransport.InspectorPreviewIsolated
                    ? MessageType.Info
                    : MessageType.Warning);
        }

        private void DrawAudioMixerSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Audio Host and Mixer",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Active Transport",
                DeverQuestAudioTransport.DisplayName);
            EditorGUILayout.HelpBox(
                DeverQuestAudioTransport.StatusMessage,
                DeverQuestAudioTransport.UsingSupportedHost
                    ? MessageType.Info
                    : MessageType.Warning);

            EditorGUI.BeginChangeCheck();
            bool preferHost = EditorGUILayout.Toggle(
                "Use Supported Audio Host",
                DeverQuestAudioMixerSettings.PreferSupportedHost);
            bool masterMute = EditorGUILayout.Toggle(
                "Mute All",
                DeverQuestAudioMixerSettings.MasterMute);
            float masterVolume = EditorGUILayout.Slider(
                "Master Volume",
                DeverQuestAudioMixerSettings.MasterVolume,
                0f,
                1f);
            float musicVolume = EditorGUILayout.Slider(
                "Music Mixer",
                DeverQuestAudioMixerSettings.MusicVolume,
                0f,
                1f);
            bool musicMute = EditorGUILayout.Toggle(
                "Mute Music",
                DeverQuestAudioMixerSettings.MusicMute);
            float ambienceVolume = EditorGUILayout.Slider(
                "Ambience Mixer",
                DeverQuestAudioMixerSettings.AmbienceVolume,
                0f,
                1f);
            bool ambienceMute = EditorGUILayout.Toggle(
                "Mute Ambience",
                DeverQuestAudioMixerSettings.AmbienceMute);
            float cueVolume = EditorGUILayout.Slider(
                "Warning and SFX Mixer",
                DeverQuestAudioMixerSettings.CueVolume,
                0f,
                1f);
            bool cueMute = EditorGUILayout.Toggle(
                "Mute Warnings and SFX",
                DeverQuestAudioMixerSettings.CueMute);
            bool duckEnabled = EditorGUILayout.Toggle(
                "Duck Long Audio During Cues",
                DeverQuestAudioMixerSettings
                    .DuckLongFormDuringCues);
            float duckVolume =
                DeverQuestAudioMixerSettings.DuckVolume;
            using (new EditorGUI.DisabledScope(!duckEnabled))
            {
                duckVolume = EditorGUILayout.Slider(
                    "Ducked Volume",
                    duckVolume,
                    0f,
                    1f);
            }
            bool pauseWhenUnfocused = EditorGUILayout.Toggle(
                "Pause When Unity Loses Focus",
                DeverQuestAudioMixerSettings
                    .PauseWhenEditorUnfocused);

            if (EditorGUI.EndChangeCheck())
            {
                bool hostPreferenceChanged =
                    preferHost !=
                    DeverQuestAudioMixerSettings
                        .PreferSupportedHost;
                DeverQuestAudioMixerSettings.MasterMute =
                    masterMute;
                DeverQuestAudioMixerSettings.MasterVolume =
                    masterVolume;
                DeverQuestAudioMixerSettings.MusicVolume =
                    musicVolume;
                DeverQuestAudioMixerSettings.MusicMute =
                    musicMute;
                DeverQuestAudioMixerSettings.AmbienceVolume =
                    ambienceVolume;
                DeverQuestAudioMixerSettings.AmbienceMute =
                    ambienceMute;
                DeverQuestAudioMixerSettings.CueVolume =
                    cueVolume;
                DeverQuestAudioMixerSettings.CueMute = cueMute;
                DeverQuestAudioMixerSettings
                    .DuckLongFormDuringCues = duckEnabled;
                DeverQuestAudioMixerSettings.DuckVolume =
                    duckVolume;
                DeverQuestAudioMixerSettings
                    .PauseWhenEditorUnfocused =
                    pauseWhenUnfocused;

                if (hostPreferenceChanged)
                {
                    DeverQuestAudioDirector.ResetAllAudio();
                    DeverQuestAudioTransport
                        .SetPreferSupportedHost(preferHost);
                }
                else
                {
                    DeverQuestAudioTransport.ApplyMixerSettings();
                    DeverQuestAudioDirector.ApplyVolumes();
                }
                Repaint();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reinitialize Audio Host"))
            {
                DeverQuestAudioDirector.ResetAllAudio();
                DeverQuestAudioTransport
                    .ReinitializeSupportedHost();
                Repaint();
            }
            if (GUILayout.Button("Reset Mixer Defaults"))
            {
                if (EditorUtility.DisplayDialog(
                        "Reset Audio Mixer?",
                        "Restore DeverQuest's local audio mixer and host " +
                        "preferences to their defaults?",
                        "Reset",
                        "Cancel"))
                {
                    DeverQuestAudioDirector.ResetAllAudio();
                    DeverQuestAudioMixerSettings.ResetDefaults();
                    DeverQuestAudioTransport
                        .ReinitializeSupportedHost();
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private static void CreatePlaylistAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Playlist",
                "DeverQuestPlaylist",
                "asset",
                "Choose where to save the playlist asset.");

            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestPlaylist playlist =
                CreateInstance<DeverQuestPlaylist>();

            AssetDatabase.CreateAsset(playlist, path);
            AssetDatabase.SaveAssets();

            DeverQuestPlaylistPlayer.SetPlaylist(playlist);
            Selection.activeObject = playlist;
            EditorGUIUtility.PingObject(playlist);
        }

        private void DrawRewardSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Reward Economy",
                EditorStyles.boldLabel);

            profile.rewardsEnabled =
                EditorGUILayout.Toggle(
                    "Enable Rewards",
                    profile.rewardsEnabled);

            using (new EditorGUI.DisabledScope(!profile.rewardsEnabled))
            {
                profile.rewardWorkBlockMinutes =
                    EditorGUILayout.IntField(
                        "Work Block (min)",
                        profile.rewardWorkBlockMinutes);

                profile.dailyWorkGoalMinutes =
                    EditorGUILayout.IntField(
                        "Daily Decree (min)",
                        profile.dailyWorkGoalMinutes);

                profile.copperPerWorkBlock =
                    EditorGUILayout.IntField(
                        "Copper per Block",
                        profile.copperPerWorkBlock);

                profile.experiencePerWorkBlock =
                    EditorGUILayout.IntField(
                        "XP per Block",
                        profile.experiencePerWorkBlock);

                profile.dailyCopperBonus =
                    EditorGUILayout.IntField(
                        "Decree Copper",
                        profile.dailyCopperBonus);

                profile.dailyExperienceBonus =
                    EditorGUILayout.IntField(
                        "Decree XP",
                        profile.dailyExperienceBonus);

                profile.baseQuestCopper =
                    EditorGUILayout.IntField(
                        "Base Quest Copper",
                        profile.baseQuestCopper);

                profile.baseQuestExperience =
                    EditorGUILayout.IntField(
                        "Base Quest XP",
                        profile.baseQuestExperience);
            }

            profile.Sanitize();

            EditorGUILayout.Space(8f);
            EditorGUILayout.LabelField(
                "Adventurer Identity",
                EditorStyles.boldLabel);

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Character Name", adventurer.characterName);
            EditorGUILayout.LabelField("Guild", adventurer.guildName);
            EditorGUILayout.LabelField(
                "Class", adventurer.characterClass);
            EditorGUILayout.LabelField(
                "Ancestry",
                string.IsNullOrWhiteSpace(adventurer.ancestryName)
                    ? "Legacy / Not Assigned"
                    : adventurer.ancestryName);
            EditorGUILayout.LabelField(
                "Alignment",
                ObjectNames.NicifyVariableName(
                    adventurer.alignment.ToString()));
            EditorGUILayout.LabelField(
                "Faith",
                string.IsNullOrWhiteSpace(adventurer.deityName)
                    ? "Agnostic"
                    : adventurer.deityName);
            EditorGUILayout.LabelField(
                "Guild Rank", adventurer.guildRank);
            EditorGUILayout.HelpBox(
                "Identity, Ancestry, Class, Alignment, Faith, and Guild " +
                "Rank are locked to the active Guild account.",
                MessageType.Info);
        }

        private void DrawRewardsPanel(DeverQuestProfile profile)
        {
            if (!profile.rewardsEnabled)
            {
                return;
            }

            EditorGUILayout.LabelField(
                "Adventurer Spoils",
                EditorStyles.boldLabel);

            double todayMinutes =
                DeverQuestRewardService.GetTodayFocusedMinutes();

            EditorGUILayout.LabelField(
                $"Daily Decree: {todayMinutes:0.#} / " +
                $"{profile.dailyWorkGoalMinutes} focused minutes");

            double carryMinutes =
                DeverQuestRewardService.Wallet
                    .unrewardedWorkSeconds / 60d;

            EditorGUILayout.LabelField(
                $"Next work block: {carryMinutes:0.#} / " +
                $"{profile.rewardWorkBlockMinutes} minutes");

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance),
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Lifetime Spoils",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.totalCopperEarned));

            if (!string.IsNullOrWhiteSpace(rewardMessage))
            {
                EditorGUILayout.HelpBox(
                    rewardMessage,
                    MessageType.Info);
            }

            EditorGUILayout.HelpBox(
                "Spend earned coin through the Guild Shop. Purchases and " +
                "redemptions remain recorded in the Guild ledger.",
                MessageType.Info);
        }

        private void DrawAdventurerSheet()
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Adventurer Character Sheet",
                EditorStyles.boldLabel);
            string characterName = string.IsNullOrWhiteSpace(
                adventurer.characterName)
                ? "Unnamed Adventurer"
                : adventurer.characterName;
            EditorGUILayout.LabelField(
                characterName,
                $"{adventurer.characterClass} · Level {adventurer.level}");
            EditorGUILayout.LabelField(
                "Ancestry",
                string.IsNullOrWhiteSpace(adventurer.ancestryName)
                    ? "Legacy / Not Assigned"
                    : adventurer.ancestryName);
            EditorGUILayout.LabelField(
                "Alignment",
                ObjectNames.NicifyVariableName(
                    adventurer.alignment.ToString()));
            EditorGUILayout.LabelField(
                "Faith",
                string.IsNullOrWhiteSpace(adventurer.deityName)
                    ? "Agnostic"
                    : adventurer.deityName);
            EditorGUILayout.LabelField(
                adventurer.guildName,
                adventurer.guildRank);
            EditorGUILayout.LabelField(
                "Experience",
                $"{adventurer.currentExperience} / " +
                $"{DeverQuestAdventurerService.ExperienceForNextLevel(adventurer.level)} XP");
            Rect experienceRect =
                EditorGUILayout.GetControlRect(false, 18f);
            float experienceProgress =
                adventurer.currentExperience /
                (float)DeverQuestAdventurerService.ExperienceForNextLevel(
                    adventurer.level);
            EditorGUI.ProgressBar(
                experienceRect,
                Mathf.Clamp01(experienceProgress),
                $"Lifetime XP: {adventurer.lifetimeExperience}");
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Hit Points",
                $"{adventurer.currentHitPoints} / " +
                $"{adventurer.maximumHitPoints} · d{adventurer.hitDie}");
            EditorGUILayout.LabelField(
                "Armor Class",
                DeverQuestRulesService.ArmorClass(adventurer).ToString());
            EditorGUILayout.LabelField(
                "Damage Affinities",
                DeverQuestDamageService
                    .DescribeAdventurerAffinities(adventurer),
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                "Proficiency",
                $"+{DeverQuestRulesService.ProficiencyBonus(adventurer.level)}");
            EditorGUILayout.Space(4f);
            DrawAbility(
                "STR", adventurer,
                DeverQuestAbility.Strength);
            DrawAbility(
                "DEX", adventurer,
                DeverQuestAbility.Dexterity);
            DrawAbility(
                "CON", adventurer,
                DeverQuestAbility.Constitution);
            DrawAbility(
                "INT", adventurer,
                DeverQuestAbility.Intelligence);
            DrawAbility(
                "WIS", adventurer,
                DeverQuestAbility.Wisdom);
            DrawAbility(
                "CHA", adventurer,
                DeverQuestAbility.Charisma);
            EditorGUILayout.LabelField(
                "AGI", adventurer.agility.ToString());
            EditorGUILayout.LabelField(
                "STA", adventurer.stamina.ToString());
            EditorGUILayout.LabelField(
                "Luck", adventurer.luck.ToString());
            EditorGUILayout.LabelField(
                "Mana",
                $"{adventurer.currentMana} / " +
                $"{adventurer.maximumMana}");
            EditorGUILayout.LabelField(
                "Home Department",
                adventurer.homeDepartment);
            EditorGUILayout.LabelField(
                "Wellness",
                $"Hunger {adventurer.hunger} · " +
                $"Rest {adventurer.rest} · " +
                $"Happiness {adventurer.happiness}");
            EditorGUILayout.LabelField(
                "Saving Throw Proficiencies",
                string.Join(", ", adventurer.proficientSaves));
            EditorGUILayout.LabelField(
                "Class Features",
                string.Join(", ",
                    DeverQuestRulesService.ClassFeatures(adventurer)));
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.FindClass(
                    adventurer.classId,
                    adventurer.characterClass);
            DeverQuestAncestry ancestry =
                DeverQuestIdentityCatalogService.FindAncestry(
                    adventurer.ancestryId,
                    adventurer.ancestryName);
            DeverQuestDeity faith =
                DeverQuestIdentityCatalogService.FindFaith(
                    adventurer.deityId,
                    adventurer.deityName);
            if (ancestry != null)
            {
                EditorGUILayout.LabelField(
                    "Ancestry Traits",
                    ancestry.innateTraits.Count == 0
                        ? "None"
                        : string.Join(", ", ancestry.innateTraits));
                EditorGUILayout.LabelField(
                    "Languages",
                    ancestry.languages.Count == 0
                        ? "None"
                        : string.Join(", ", ancestry.languages));
            }
            if (faith != null &&
                !string.IsNullOrWhiteSpace(faith.grantedTrait))
            {
                EditorGUILayout.LabelField(
                    "Faith Boon", faith.grantedTrait);
            }
            if (classDefinition != null &&
                classDefinition.supportsCompanion)
            {
                EditorGUILayout.LabelField(
                    "Companion Tradition",
                    string.IsNullOrWhiteSpace(
                        classDefinition.companionTradition)
                        ? "Supported"
                        : classDefinition.companionTradition);
            }
            if (classDefinition?.abilityProfile != null)
            {
                EditorGUILayout.LabelField(
                    "Tactical Ability Profile",
                    $"{classDefinition.abilityProfile.displayName} · " +
                    $"{classDefinition.abilityProfile.tacticalStyle} · " +
                    $"{classDefinition.abilityProfile.abilities.Count} actions");
            }
            EditorGUILayout.LabelField(
                "Status Effects",
                adventurer.statusEffects.Count == 0
                    ? "None"
                    : string.Join(", ", adventurer.statusEffects));
            if (adventurer.isFallen)
            {
                EditorGUILayout.HelpBox(
                    "This Adventurer has fallen. Resurrection costs 50 " +
                    "copper and restores half Hit Points.",
                    MessageType.Warning);
                if (GUILayout.Button("Resurrect at the Guild Shrine"))
                {
                    DeverQuestEncounterService.Resurrect(
                        out string resurrectionMessage);
                    EditorUtility.DisplayDialog(
                        "Guild Shrine",
                        resurrectionMessage,
                        "Close");
                }
            }
            List<string> equipment =
                DeverQuestRulesService.EquippedNames(adventurer);
            List<string> spells =
                DeverQuestRulesService.KnownSpellNames(adventurer);
            EditorGUILayout.LabelField(
                "Equipment",
                equipment.Count == 0
                    ? "None"
                    : string.Join(", ", equipment));
            EditorGUILayout.LabelField(
                "Known Spells",
                spells.Count == 0
                    ? "None"
                    : string.Join(", ", spells));
            foreach (DeverQuestSpell spell in
                     DeverQuestRulesService.KnownSpellAssets(adventurer))
            {
                string effects =
                    spell.effects == null ||
                    spell.effects.Count == 0
                        ? string.IsNullOrWhiteSpace(spell.damageDice)
                            ? "Legacy utility"
                            : "Legacy direct damage"
                        : string.Join(
                            ", ",
                            spell.effects
                                .Where(effect => effect != null)
                                .Select(effect =>
                                    effect.effectType.ToString()));
                EditorGUILayout.LabelField(
                    "  " + spell.displayName,
                    $"{effects} · {spell.manaCost} mana");
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.Space(6f);
                if (GUILayout.Button(
                        "Customize Current Adventurer Identity…"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Reopen Character Creation?",
                        "This lets you choose a new Adventurer name, " +
                        "Ancestry, Class, Alignment, and Faith. Existing " +
                        "level, XP, coin, inventory, and Chronicle history " +
                        "remain attached to this Guild account.",
                        "Customize Adventurer",
                        "Cancel");
                    if (confirmed &&
                        !DeverQuestGuildAccountService
                            .ReopenCurrentCharacterCreation(
                                out string error))
                    {
                        EditorUtility.DisplayDialog(
                            "Cannot Reopen Character Creation",
                            error,
                            "Close");
                    }
                    GUIUtility.ExitGUI();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTacticsWorkspace()
        {
            EditorGUILayout.LabelField(
                "Tactical Operations",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Review combat readiness, manage the active Companion, " +
                "inspect the current Encounter, and search the local " +
                "Battle Archive. Tactical operations never create " +
                "focused-work time by themselves.",
                MessageType.Info);

            DrawTacticalReadiness();
            DrawTacticalCompanionOperations();

            DeverQuestSession reportSession =
                DeverQuestSessionStore.HasActiveSession
                    ? DeverQuestSessionStore.ActiveSession
                    : DeverQuestSessionStore.LastCompletedSession;
            if (reportSession != null)
            {
                EditorGUILayout.Space(8f);
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    DeverQuestSessionStore.HasActiveSession
                        ? "Active Quest Field Reports"
                        : "Latest Completed Quest Field Reports",
                    EditorStyles.boldLabel);
                DrawBattleResults(reportSession);
                EditorGUILayout.EndVertical();
            }

            DrawTacticalArchive();

            if (!string.IsNullOrWhiteSpace(tacticalOperationsMessage))
            {
                EditorGUILayout.HelpBox(
                    tacticalOperationsMessage,
                    MessageType.Info);
            }
        }

        private void DrawTacticalReadiness()
        {
            tacticalReadinessFoldout = EditorGUILayout.Foldout(
                tacticalReadinessFoldout,
                "Combat Readiness",
                true);
            if (!tacticalReadinessFoldout)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                adventurer.characterName,
                adventurer.characterClass + " · Level " +
                adventurer.level + " · " + adventurer.guildRank);
            EditorGUILayout.LabelField(
                "Vitals",
                "HP " + adventurer.currentHitPoints + "/" +
                adventurer.maximumHitPoints + " · Mana " +
                adventurer.currentMana + "/" +
                adventurer.maximumMana + " · AC " +
                DeverQuestRulesService.ArmorClass(adventurer));
            EditorGUILayout.LabelField(
                "Carry Load",
                DeverQuestEncumbranceService
                    .CarriedWeight(adventurer).ToString("0.0") +
                " / " +
                DeverQuestEncumbranceService
                    .CarryCapacity(adventurer).ToString("0.0"));
            EditorGUILayout.LabelField(
                "Status",
                adventurer.statusEffects.Count == 0
                    ? "Ready"
                    : string.Join(", ", adventurer.statusEffects),
                EditorStyles.wordWrappedLabel);

            List<string> equipment =
                DeverQuestRulesService.EquippedNames(adventurer);
            List<string> spells =
                DeverQuestRulesService.KnownSpellNames(adventurer);
            EditorGUILayout.LabelField(
                "Equipment",
                equipment.Count == 0
                    ? "None equipped"
                    : string.Join(", ", equipment),
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.LabelField(
                "Known Tactical Actions",
                spells.Count == 0
                    ? "Class techniques and basic attacks only"
                    : string.Join(", ", spells),
                EditorStyles.wordWrappedLabel);

            if (adventurer.isFallen)
            {
                EditorGUILayout.HelpBox(
                    "The Adventurer is Fallen and cannot safely enter a " +
                    "new Encounter.",
                    MessageType.Error);
                if (GUILayout.Button("Resurrect at the Guild Shrine"))
                {
                    DeverQuestEncounterService.Resurrect(
                        out tacticalOperationsMessage);
                }
            }
            else if (adventurer.currentHitPoints <=
                     Math.Max(1, adventurer.maximumHitPoints / 4))
            {
                EditorGUILayout.HelpBox(
                    "Hit Points are at or below 25%. A safety pause is " +
                    "likely in a dangerous Encounter.",
                    MessageType.Warning);
            }
            if (DeverQuestEncumbranceService.IsEncumbered(adventurer))
            {
                EditorGUILayout.HelpBox(
                    "The Adventurer is encumbered. Survival combat may " +
                    "pause until carried weight is reduced.",
                    MessageType.Warning);
            }

            DeverQuestSessionStage stage =
                DeverQuestSessionStore.HasActiveSession
                    ? DeverQuestSessionStore.CurrentQuestStage()
                    : null;
            if (stage == null)
            {
                EditorGUILayout.Space(5f);
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    "No active Quest Encounter.");
            }
            else
            {
                DeverQuestEncounterProfile encounter =
                    DeverQuestEncounterService.FindEncounter(
                        stage.encounterProfileId);
                EditorGUILayout.Space(5f);
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    DeverQuestEncounterService
                        .EncounterDisplayName(stage),
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    DeverQuestEncounterService.DescribeEncounter(stage),
                    EditorStyles.wordWrappedLabel);
                if (DeverQuestEncounterService.IsSurvival(stage))
                {
                    EditorGUILayout.LabelField(
                        DeverQuestEncounterService
                            .DescribeSurvivalProgress(stage),
                        EditorStyles.wordWrappedLabel);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Open Current Quest"))
                {
                    activeWorkspace = DeverQuestWorkspace.Quest;
                    scrollPosition = Vector2.zero;
                    GUIUtility.ExitGUI();
                }
                using (new EditorGUI.DisabledScope(encounter == null))
                {
                    if (GUILayout.Button("Select Encounter Profile"))
                    {
                        Selection.activeObject = encounter;
                        EditorGUIUtility.PingObject(encounter);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTacticalCompanionOperations()
        {
            tacticalCompanionFoldout = EditorGUILayout.Foldout(
                tacticalCompanionFoldout,
                "Companion Operations",
                true);
            if (!tacticalCompanionFoldout)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            List<DeverQuestCompanionState> companions =
                (adventurer.companions ??
                 new List<DeverQuestCompanionState>())
                .Where(value => value != null)
                .ToList();

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            if (companions.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No Companion has joined this Adventurer. Recruit one " +
                    "from Character > Companion Stable.",
                    MessageType.Info);
                if (GUILayout.Button("Open Character and Companion Stable"))
                {
                    activeWorkspace = DeverQuestWorkspace.Character;
                    scrollPosition = Vector2.zero;
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndVertical();
                return;
            }

            tacticalCompanionIndex = Mathf.Clamp(
                tacticalCompanionIndex,
                0,
                companions.Count - 1);
            string[] names = companions
                .Select(value =>
                    DeverQuestCompanionService.DisplayName(value) +
                    (value.isActive ? " [ACTIVE]" :
                     value.isFallen ? " [FALLEN]" : string.Empty))
                .ToArray();
            tacticalCompanionIndex = EditorGUILayout.Popup(
                "Roster",
                tacticalCompanionIndex,
                names);
            DeverQuestCompanionState companion =
                companions[tacticalCompanionIndex];
            DeverQuestCompanionProfile profile =
                DeverQuestCompanionService.FindProfile(
                    companion.profileId);
            int maximumHitPoints =
                DeverQuestCompanionService.MaximumHitPoints(
                    companion,
                    profile);
            int recoveryCost =
                DeverQuestCompanionService.RecoveryCost(companion);
            bool needsRecovery =
                profile != null &&
                (companion.isFallen ||
                 companion.currentHitPoints < maximumHitPoints);

            EditorGUILayout.LabelField(
                "Role",
                profile == null
                    ? "Missing Companion Profile"
                    : profile.kind + " · " + profile.role + " · " +
                      profile.creatureType);
            EditorGUILayout.LabelField(
                "Readiness",
                "HP " + companion.currentHitPoints + "/" +
                maximumHitPoints + " · Loyalty " +
                companion.loyalty + "/100 · Level " +
                companion.level);
            EditorGUILayout.LabelField(
                "Record",
                companion.battles + " battles · " +
                companion.victories + " victories · Damage " +
                companion.lifetimeDamageDealt + " · Healing " +
                companion.lifetimeHealingDone,
                EditorStyles.wordWrappedLabel);

            if (companion.isFallen)
            {
                EditorGUILayout.HelpBox(
                    "This Companion is Fallen and cannot be activated.",
                    MessageType.Warning);
            }
            else if (companion.currentHitPoints <=
                     Math.Max(1, maximumHitPoints / 4))
            {
                EditorGUILayout.HelpBox(
                    "This Companion is at or below 25% Hit Points.",
                    MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            using (new EditorGUI.DisabledScope(
                       companion.isActive || companion.isFallen ||
                       profile == null))
            {
                if (GUILayout.Button("Set Active"))
                {
                    DeverQuestCompanionService.Activate(
                        companion,
                        out tacticalOperationsMessage);
                }
            }
            using (new EditorGUI.DisabledScope(!companion.isActive))
            {
                if (GUILayout.Button("Send to Stable"))
                {
                    DeverQuestCompanionService.Dismiss(companion);
                    tacticalOperationsMessage =
                        DeverQuestCompanionService.DisplayName(companion) +
                        " is resting in the Stable.";
                }
            }
            using (new EditorGUI.DisabledScope(!needsRecovery))
            {
                if (GUILayout.Button(
                        !needsRecovery
                            ? "Ready"
                            : recoveryCost <= 0
                                ? "Recover (Free)"
                                : "Recover (" +
                                  DeverQuestAdventurerService.FormatCoins(
                                      recoveryCost) + ")"))
                {
                    DeverQuestCompanionService.Recover(
                        companion,
                        out tacticalOperationsMessage);
                }
            }
            EditorGUILayout.EndHorizontal();

            List<DeverQuestCompanionState> recoveryTargets =
                companions.Where(value =>
                {
                    DeverQuestCompanionProfile valueProfile =
                        DeverQuestCompanionService.FindProfile(
                            value.profileId);
                    return valueProfile != null &&
                           (value.isFallen ||
                            value.currentHitPoints <
                            DeverQuestCompanionService.MaximumHitPoints(
                                value,
                                valueProfile));
                }).ToList();
            int rosterRecoveryCost = recoveryTargets.Sum(
                DeverQuestCompanionService.RecoveryCost);
            using (new EditorGUI.DisabledScope(
                       recoveryTargets.Count == 0))
            {
                if (GUILayout.Button(
                        recoveryTargets.Count == 0
                            ? "All Companions Ready"
                            : rosterRecoveryCost <= 0
                                ? "Recover Entire Roster (Free)"
                                : "Recover Entire Roster (" +
                                  DeverQuestAdventurerService.FormatCoins(
                                      rosterRecoveryCost) + ")"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Recover Entire Companion Roster?",
                        "Restore every injured or Fallen Companion for " +
                        DeverQuestAdventurerService.FormatCoins(
                            rosterRecoveryCost) + ".",
                        "Recover Roster",
                        "Cancel");
                    if (confirmed)
                    {
                        DeverQuestCompanionService.RecoverAll(
                            out tacticalOperationsMessage);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawTacticalArchive()
        {
            tacticalArchiveFoldout = EditorGUILayout.Foldout(
                tacticalArchiveFoldout,
                "Battle Archive",
                true);
            if (!tacticalArchiveFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Local Tactical Archive",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "The newest 100 Battle Results are stored locally in " +
                "Unity EditorPrefs. This archive is for review and Beta " +
                "diagnostics; Timecards remain the permanent Chronicle.",
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import Current and Last Quest Reports"))
            {
                int imported = 0;
                imported += DeverQuestTacticalArchiveService.ImportSession(
                    DeverQuestSessionStore.ActiveSession);
                imported += DeverQuestTacticalArchiveService.ImportSession(
                    DeverQuestSessionStore.LastCompletedSession);
                tacticalOperationsMessage = imported == 0
                    ? "No new Battle Results were found to import."
                    : "Imported " + imported + " Battle Result" +
                      (imported == 1 ? "." : "s.");
            }
            using (new EditorGUI.DisabledScope(
                       DeverQuestTacticalArchiveService.Records.Count == 0))
            {
                if (GUILayout.Button("Clear Local Archive"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Clear Local Tactical Archive?",
                        "This removes the local Battle Archive only. " +
                        "Timecards and Chronicle files are not deleted.",
                        "Clear Archive",
                        "Cancel");
                    if (confirmed)
                    {
                        DeverQuestTacticalArchiveService.Clear();
                        tacticalOperationsMessage =
                            "The local Tactical Archive was cleared.";
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            tacticalArchiveSearch = EditorGUILayout.TextField(
                "Search",
                tacticalArchiveSearch);
            string[] outcomes =
            {
                "All Outcomes",
                "Victory",
                "Early Victory",
                "Safety Pause",
                "Defeat",
                "Survival"
            };
            tacticalArchiveOutcomeIndex = EditorGUILayout.Popup(
                "Outcome",
                tacticalArchiveOutcomeIndex,
                outcomes);

            List<DeverQuestArchivedBattle> records =
                DeverQuestTacticalArchiveService.Records
                    .Where(MatchesTacticalArchiveFilter)
                    .Take(50)
                    .ToList();
            EditorGUILayout.LabelField(
                "Results",
                records.Count + " shown · " +
                DeverQuestTacticalArchiveService.Records.Count +
                " stored");

            if (records.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No archived Battle Result matches the current filter.",
                    MessageType.Info);
            }

            foreach (DeverQuestArchivedBattle record in records)
            {
                DeverQuestBattleResult battle = record.battle;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    DeverQuestCombatSummaryService.OutcomeTitle(battle) +
                    " · " +
                    (string.IsNullOrWhiteSpace(battle.encounterName)
                        ? "Encounter"
                        : battle.encounterName),
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    "Quest",
                    (string.IsNullOrWhiteSpace(record.projectName)
                        ? "Unassigned Project"
                        : record.projectName) + " · " +
                    (string.IsNullOrWhiteSpace(record.taskName)
                        ? "Untitled Task"
                        : record.taskName),
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(
                    "Adventurer",
                    string.IsNullOrWhiteSpace(record.adventurerName)
                        ? "Unknown Adventurer"
                        : record.adventurerName);
                EditorGUILayout.LabelField(
                    "Resolved",
                    TacticalArchiveLocalTime(record).ToString("g") +
                    " · " + battle.rounds + " round" +
                    (battle.rounds == 1 ? string.Empty : "s"));
                EditorGUILayout.LabelField(
                    "Outcome",
                    DeverQuestCombatSummaryService.OutcomeSummary(battle),
                    EditorStyles.wordWrappedLabel);
                string companion =
                    DeverQuestCombatSummaryService
                        .CompanionContributionSummary(battle);
                if (!string.IsNullOrWhiteSpace(companion))
                {
                    EditorGUILayout.LabelField(
                        "Companion",
                        companion,
                        EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.LabelField(
                    "Rewards",
                    DeverQuestAdventurerService.FormatCoins(
                        battle.bonusCopper) + " + " +
                    battle.bonusExperience + " XP");

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Copy Report"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        DeverQuestCombatSummaryService
                            .BuildFullCombatReport(battle);
                }
                if (GUILayout.Button("Copy JSON"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        JsonUtility.ToJson(record, true);
                }
                DeverQuestEncounterProfile encounter =
                    DeverQuestEncounterService.FindEncounter(
                        battle.encounterId);
                using (new EditorGUI.DisabledScope(encounter == null))
                {
                    if (GUILayout.Button("Select Profile"))
                    {
                        Selection.activeObject = encounter;
                        EditorGUIUtility.PingObject(encounter);
                    }
                }
                if (GUILayout.Button("Remove"))
                {
                    DeverQuestTacticalArchiveService.Remove(
                        record.archiveId);
                    tacticalOperationsMessage =
                        "Removed one local Battle Archive record.";
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private bool MatchesTacticalArchiveFilter(
            DeverQuestArchivedBattle record)
        {
            if (record?.battle == null)
            {
                return false;
            }
            DeverQuestBattleResult battle = record.battle;
            bool outcomeMatches;
            switch (tacticalArchiveOutcomeIndex)
            {
                case 1:
                    outcomeMatches = battle.victory;
                    break;
                case 2:
                    outcomeMatches = battle.earlyVictory;
                    break;
                case 3:
                    outcomeMatches = battle.safetyPaused;
                    break;
                case 4:
                    outcomeMatches =
                        !battle.victory && !battle.safetyPaused;
                    break;
                case 5:
                    outcomeMatches = battle.survivalWave > 0;
                    break;
                default:
                    outcomeMatches = true;
                    break;
            }
            if (!outcomeMatches)
            {
                return false;
            }

            string search = tacticalArchiveSearch?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(search))
            {
                return true;
            }
            string haystack = string.Join(
                " ",
                record.projectName,
                record.taskName,
                record.developerName,
                record.adventurerName,
                record.questRunId,
                battle.encounterName,
                battle.companionName,
                battle.seed);
            return haystack.IndexOf(
                       search,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static DateTime TacticalArchiveLocalTime(
            DeverQuestArchivedBattle record)
        {
            long ticks = Math.Max(
                0L,
                record?.archivedUtcTicks ?? 0L);
            if (ticks <= 0L)
            {
                return DateTime.Now;
            }
            try
            {
                return new DateTime(
                    ticks,
                    DateTimeKind.Utc).ToLocalTime();
            }
            catch
            {
                return DateTime.Now;
            }
        }

        private void DrawCompanionStable()
        {
            companionStableFoldout = EditorGUILayout.Foldout(
                companionStableFoldout,
                "Companion Stable",
                true);
            if (!companionStableFoldout)
            {
                return;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Pets, Familiars, Minions, and Companions",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "One active Companion may join deterministic encounters. " +
                "Companions affect RPG battles only—they never create " +
                "focused-work time or productivity rewards.",
                EditorStyles.wordWrappedLabel);

            selectedCompanionProfile =
                (DeverQuestCompanionProfile)
                EditorGUILayout.ObjectField(
                    "Companion Profile",
                    selectedCompanionProfile,
                    typeof(DeverQuestCompanionProfile),
                    false);
            bool canRecruit =
                DeverQuestCompanionService.CanRecruit(
                    adventurer,
                    selectedCompanionProfile,
                    out string recruitReason);
            using (new EditorGUI.DisabledScope(!canRecruit))
            {
                if (GUILayout.Button("Recruit Companion"))
                {
                    DeverQuestCompanionService.Recruit(
                        selectedCompanionProfile,
                        out companionMessage);
                }
            }
            if (!canRecruit &&
                selectedCompanionProfile != null &&
                !string.IsNullOrWhiteSpace(recruitReason))
            {
                EditorGUILayout.HelpBox(
                    recruitReason,
                    MessageType.Info);
            }

            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            using (new EditorGUI.DisabledScope(!canManage))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(
                        "Generate Original Starter Stable"))
                {
                    DeverQuestCompanionGenerationReport report =
                        DeverQuestCompanionCatalogGenerator
                            .GenerateOriginalStarterCatalog();
                    companionMessage = report.Summary;
                }
                if (GUILayout.Button("Create Profile…"))
                {
                    CreateCharacterAsset<
                        DeverQuestCompanionProfile>(
                        "NewCompanionProfile");
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space(5f);
            if ((adventurer.companions?.Count ?? 0) == 0)
            {
                EditorGUILayout.LabelField(
                    "No Companions have joined this Adventurer.");
            }
            else
            {
                foreach (DeverQuestCompanionState companion in
                         adventurer.companions.ToList())
                {
                    if (companion == null)
                    {
                        continue;
                    }
                    DeverQuestCompanionProfile profile =
                        DeverQuestCompanionService.FindProfile(
                            companion.profileId);
                    EditorGUILayout.BeginVertical(
                        EditorStyles.helpBox);
                    EditorGUI.BeginChangeCheck();
                    companion.customName =
                        EditorGUILayout.TextField(
                            "Name",
                            companion.customName);
                    if (EditorGUI.EndChangeCheck())
                    {
                        companion.customName =
                            companion.customName?.Trim() ??
                            string.Empty;
                        DeverQuestAdventurerService.Save();
                    }
                    int maximumHitPoints =
                        DeverQuestCompanionService
                            .MaximumHitPoints(
                                companion,
                                profile);
                    EditorGUILayout.LabelField(
                        profile == null
                            ? "Missing Companion Profile"
                            : $"{profile.kind} · {profile.role} · " +
                              $"{profile.creatureType}",
                        companion.isActive
                            ? "ACTIVE"
                            : companion.isFallen
                                ? "FALLEN"
                                : "Resting");
                    EditorGUILayout.LabelField(
                        "Level and Bond",
                        $"Level {companion.level} · " +
                        $"{companion.currentExperience}/" +
                        $"{DeverQuestCompanionService.ExperienceForNextLevel(companion.level)} XP · " +
                        $"Loyalty {companion.loyalty}/100");
                    int winRate = companion.battles <= 0
                        ? 0
                        : (int)Math.Round(
                            companion.victories * 100d /
                            companion.battles);
                    EditorGUILayout.LabelField(
                        "Vitals",
                        $"HP {companion.currentHitPoints}/" +
                        $"{maximumHitPoints} · " +
                        $"Battles {companion.battles} · " +
                        $"Victories {companion.victories} " +
                        $"({winRate}% win rate)");
                    EditorGUILayout.LabelField(
                        "Lifetime Contribution",
                        $"Damage {companion.lifetimeDamageDealt} · " +
                        $"Healing {companion.lifetimeHealingDone} · " +
                        $"Damage Taken {companion.lifetimeDamageTaken}",
                        EditorStyles.wordWrappedLabel);
                    if (!string.IsNullOrWhiteSpace(
                            companion.lastBattleSummary))
                    {
                        EditorGUILayout.LabelField(
                            "Last Battle",
                            companion.lastBattleSummary,
                            EditorStyles.wordWrappedLabel);
                    }
                    EditorGUILayout.BeginHorizontal();
                    if (companion.isActive)
                    {
                        if (GUILayout.Button("Dismiss"))
                        {
                            DeverQuestCompanionService.Dismiss(
                                companion);
                            companionMessage =
                                $"{DeverQuestCompanionService.DisplayName(companion)} is resting.";
                        }
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(
                                   companion.isFallen ||
                                   profile == null))
                        {
                            if (GUILayout.Button("Set Active"))
                            {
                                DeverQuestCompanionService.Activate(
                                    companion,
                                    out companionMessage);
                            }
                        }
                    }
                    using (new EditorGUI.DisabledScope(
                               profile == null ||
                               (!companion.isFallen &&
                                companion.currentHitPoints >=
                                maximumHitPoints)))
                    {
                        if (GUILayout.Button("Recover"))
                        {
                            DeverQuestCompanionService.Recover(
                                companion,
                                out companionMessage);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                }
            }
            if (!string.IsNullOrWhiteSpace(companionMessage))
            {
                EditorGUILayout.HelpBox(
                    companionMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawInventoryWorkspace()
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestCarrySummary carry =
                DeverQuestEncumbranceService.Summary(adventurer);

            EditorGUILayout.LabelField(
                "Inventory and Equipment",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Review the pack, compare gear, trace where loot came from, " +
                "and perform guarded inventory actions.",
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Carry Load",
                $"{carry.Status} · {carry.TotalWeight:0.0}/" +
                $"{carry.Capacity:0.0} ({carry.LoadPercent:0.#}%)",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Inventory Weight",
                $"{carry.InventoryWeight:0.0}");
            EditorGUILayout.LabelField(
                "Coin Weight",
                $"{carry.CoinWeight:0.0} " +
                $"({DeverQuestAdventurerService.CoinPieceCount(adventurer)} " +
                $"coin pieces × 0.01)");
            EditorGUILayout.LabelField(
                "Remaining Capacity",
                $"{carry.RemainingCapacity:0.0}");
            EditorGUILayout.LabelField(
                "Capacity Formula",
                $"30 + Strength ({adventurer.strength}) × 2 + " +
                $"Level ({adventurer.level}) = {carry.Capacity:0.0}",
                EditorStyles.miniLabel);
            if (carry.IsEncumbered)
            {
                EditorGUILayout.HelpBox(
                    "The Adventurer is encumbered. Unequip, sell, or safely " +
                    "drop items before continuing Survival travel.",
                    MessageType.Warning);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Equipped Gear",
                EditorStyles.boldLabel);
            DeverQuestEquipment[] equipped =
                DeverQuestRulesService.EquippedAssets(adventurer)
                    .Where(value => value != null)
                    .OrderBy(value => value.slot)
                    .ToArray();
            if (equipped.Length == 0)
            {
                EditorGUILayout.LabelField(
                    "No equipment is currently equipped.");
            }
            int missingInventoryRecords = equipped.Count(equipment =>
                !(adventurer.inventory ??
                  new List<DeverQuestInventoryEntry>())
                .Any(entry =>
                    entry != null &&
                    entry.equipmentId == equipment.EquipmentId &&
                    entry.quantity > 0));
            if (missingInventoryRecords > 0)
            {
                EditorGUILayout.HelpBox(
                    $"{missingInventoryRecords} equipped item" +
                    (missingInventoryRecords == 1 ? " is" : "s are") +
                    " missing a pack record from an older loadout.",
                    MessageType.Warning);
                if (GUILayout.Button(
                        "Repair Equipped Inventory Records"))
                {
                    DeverQuestInventoryService
                        .RepairEquippedInventory(
                            out inventoryMessage);
                    GUIUtility.ExitGUI();
                }
            }
            foreach (DeverQuestEquipment equipment in equipped)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    equipment.displayName,
                    DeverQuestInventoryService
                        .DescribeEquipment(equipment));
                if (GUILayout.Button(
                        "Unequip",
                        GUILayout.Width(75f)))
                {
                    DeverQuestInventoryService.TryUnequipEquipment(
                        equipment.EquipmentId,
                        out inventoryMessage);
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Pack",
                EditorStyles.boldLabel);

            inventorySearch = EditorGUILayout.TextField(
                "Search",
                inventorySearch);

            DeverQuestItemCategory[] categories =
                Enum.GetValues(typeof(DeverQuestItemCategory))
                    .Cast<DeverQuestItemCategory>()
                    .Where(value =>
                        value != DeverQuestItemCategory.Unknown)
                    .ToArray();
            string[] categoryLabels =
                new[] { "All Categories" }
                    .Concat(categories.Select(value =>
                        value.ToString()))
                    .ToArray();
            inventoryCategoryIndex = Mathf.Clamp(
                inventoryCategoryIndex,
                0,
                categoryLabels.Length - 1);
            inventoryCategoryIndex = EditorGUILayout.Popup(
                "Category",
                inventoryCategoryIndex,
                categoryLabels);

            EditorGUILayout.BeginHorizontal();
            inventoryShowProvenance = EditorGUILayout.ToggleLeft(
                "Show Provenance",
                inventoryShowProvenance,
                GUILayout.Width(125f));
            inventoryShowLore = EditorGUILayout.ToggleLeft(
                "Show Descriptions",
                inventoryShowLore,
                GUILayout.Width(140f));
            EditorGUILayout.EndHorizontal();

            IEnumerable<DeverQuestInventoryEntry> entries =
                (adventurer.inventory ??
                 new List<DeverQuestInventoryEntry>())
                .Where(value => value != null && value.quantity > 0);

            string search = inventorySearch?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                entries = entries.Where(value =>
                    (value.displayName ?? string.Empty).IndexOf(
                        search,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    value.itemCategory.ToString().IndexOf(
                        search,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (value.subcategory ?? string.Empty).IndexOf(
                        search,
                        StringComparison.OrdinalIgnoreCase) >= 0 ||
                    (value.tags ?? new List<string>()).Any(tag =>
                        (tag ?? string.Empty).IndexOf(
                            search,
                            StringComparison.OrdinalIgnoreCase) >= 0));
            }
            if (inventoryCategoryIndex > 0)
            {
                DeverQuestItemCategory selectedCategory =
                    categories[inventoryCategoryIndex - 1];
                entries = entries.Where(value =>
                    value.itemCategory == selectedCategory);
            }

            DeverQuestInventoryEntry[] visibleEntries =
                entries
                    .OrderBy(value => value.itemCategory)
                    .ThenBy(value => value.displayName)
                    .ToArray();

            if (visibleEntries.Length == 0)
            {
                EditorGUILayout.LabelField(
                    adventurer.inventory.Count == 0
                        ? "The pack is empty."
                        : "No inventory entries match the current filters.");
            }

            foreach (DeverQuestInventoryEntry entry in visibleEntries)
            {
                DrawInventoryEntryCard(adventurer, entry);
            }

            if (!string.IsNullOrWhiteSpace(inventoryMessage))
            {
                EditorGUILayout.HelpBox(
                    inventoryMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Quartermaster",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Purchases, denomination exchange, sales, trading, and " +
                "redemption approvals remain in Guild Hall.",
                EditorStyles.wordWrappedLabel);
            if (GUILayout.Button("Open Guild Hall Quartermaster"))
            {
                activeWorkspace = DeverQuestWorkspace.GuildHall;
                Repaint();
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawInventoryEntryCard(
            DeverQuestAdventurer adventurer,
            DeverQuestInventoryEntry entry)
        {
            string accountId =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty;
            entry.EnsureOwnership(accountId);

            DeverQuestShopItem shopItem =
                DeverQuestInventoryService.FindShopItem(entry);
            if (shopItem != null)
            {
                DeverQuestInventoryService.SynchronizeEntry(
                    entry, shopItem);
            }
            DeverQuestEquipment equipment =
                DeverQuestInventoryService.FindEquipment(entry);
            bool equipped =
                DeverQuestInventoryService.IsEquipped(
                    entry, adventurer);
            float stackWeight =
                Math.Max(0f, entry.unitWeight) * entry.quantity;
            int sellValue =
                DeverQuestInventoryService.SellValue(entry);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"{entry.displayName} ×{entry.quantity}",
                DeverQuestInventoryService
                    .DescribeClassification(entry),
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Pack Impact",
                $"{entry.unitWeight:0.##} each · " +
                $"{stackWeight:0.##} total · " +
                $"{DeverQuestAdventurerService.FormatCoins(sellValue)} " +
                $"resale each",
                EditorStyles.miniLabel);

            List<string> flags = new List<string>();
            if (equipped)
            {
                flags.Add("Equipped");
            }
            if (entry.questProtected)
            {
                flags.Add("Quest Protected");
            }
            if (!entry.tradable)
            {
                flags.Add("Not Tradable");
            }
            if (!entry.droppable)
            {
                flags.Add("Not Droppable");
            }
            if (flags.Count > 0)
            {
                EditorGUILayout.LabelField(
                    "Status",
                    string.Join(" · ", flags));
            }

            if (inventoryShowLore && shopItem != null)
            {
                if (!string.IsNullOrWhiteSpace(shopItem.description))
                {
                    EditorGUILayout.LabelField(
                        shopItem.description,
                        EditorStyles.wordWrappedLabel);
                }
                if (!string.IsNullOrWhiteSpace(shopItem.loreText))
                {
                    EditorGUILayout.HelpBox(
                        shopItem.loreText,
                        MessageType.None);
                }
            }

            if (inventoryShowProvenance)
            {
                EditorGUILayout.LabelField(
                    "Origin",
                    DeverQuestInventoryService
                        .DescribeProvenance(entry),
                    EditorStyles.wordWrappedLabel);
            }

            if (equipment != null)
            {
                EditorGUILayout.LabelField(
                    "Equipment",
                    DeverQuestInventoryService
                        .DescribeEquipment(equipment),
                    EditorStyles.wordWrappedLabel);
                EditorGUILayout.LabelField(
                    "Comparison",
                    DeverQuestInventoryService
                        .DescribeComparison(equipment, adventurer),
                    EditorStyles.wordWrappedLabel);
            }
            else if (entry.itemType ==
                     DeverQuestShopItemType.Equipment)
            {
                EditorGUILayout.HelpBox(
                    "This entry is classified as Equipment, but its " +
                    "equipment asset could not be resolved.",
                    MessageType.Warning);
            }

            EditorGUILayout.BeginHorizontal();
            if (equipment != null)
            {
                if (equipped)
                {
                    if (GUILayout.Button("Unequip"))
                    {
                        DeverQuestInventoryService.TryUnequip(
                            entry.ownershipId,
                            out inventoryMessage);
                        GUIUtility.ExitGUI();
                    }
                }
                else
                {
                    using (new EditorGUI.DisabledScope(
                               adventurer.level <
                               equipment.minimumLevel))
                    {
                        if (GUILayout.Button("Equip"))
                        {
                            DeverQuestInventoryService.TryEquip(
                                entry.ownershipId,
                                out inventoryMessage);
                            GUIUtility.ExitGUI();
                        }
                    }
                }
            }

            bool usable = IsUsableInventoryItem(shopItem);
            using (new EditorGUI.DisabledScope(!usable))
            {
                if (GUILayout.Button("Use"))
                {
                    DeverQuestShopService.UseInventoryEntry(
                        entry.ownershipId,
                        out inventoryMessage);
                    GUIUtility.ExitGUI();
                }
            }

            bool canSell =
                DeverQuestInventoryService.CanSell(
                    entry, out string sellReason);
            using (new EditorGUI.DisabledScope(!canSell))
            {
                if (GUILayout.Button("Sell 1"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Sell Inventory Item?",
                        $"Sell 1 × {entry.displayName} for " +
                        $"{DeverQuestAdventurerService.FormatCoins(sellValue)}?",
                        "Sell",
                        "Cancel");
                    if (confirmed)
                    {
                        DeverQuestInventoryService.TrySell(
                            entry.ownershipId,
                            1,
                            out inventoryMessage);
                        GUIUtility.ExitGUI();
                    }
                }
            }

            bool canDrop =
                DeverQuestInventoryService.CanDrop(
                    entry, out string dropReason);
            using (new EditorGUI.DisabledScope(!canDrop))
            {
                if (GUILayout.Button("Drop 1"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Drop Inventory Item?",
                        $"Permanently drop 1 × {entry.displayName}?",
                        "Drop",
                        "Cancel");
                    if (confirmed)
                    {
                        DeverQuestInventoryService.TryDrop(
                            entry.ownershipId,
                            1,
                            out inventoryMessage);
                        GUIUtility.ExitGUI();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            if (entry.quantity > 1)
            {
                EditorGUILayout.BeginHorizontal();
                using (new EditorGUI.DisabledScope(!canSell))
                {
                    if (GUILayout.Button(
                            $"Sell Stack ×{entry.quantity}"))
                    {
                        long total =
                            (long)sellValue * entry.quantity;
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Sell Entire Stack?",
                            $"Sell {entry.quantity} × " +
                            $"{entry.displayName} for " +
                            $"{DeverQuestAdventurerService.FormatCoins(total)}?",
                            "Sell Stack",
                            "Cancel");
                        if (confirmed)
                        {
                            DeverQuestInventoryService.TrySell(
                                entry.ownershipId,
                                entry.quantity,
                                out inventoryMessage);
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                using (new EditorGUI.DisabledScope(!canDrop))
                {
                    if (GUILayout.Button(
                            $"Drop Stack ×{entry.quantity}"))
                    {
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Drop Entire Stack?",
                            $"Permanently drop {entry.quantity} × " +
                            $"{entry.displayName}?",
                            "Drop Stack",
                            "Cancel");
                        if (confirmed)
                        {
                            DeverQuestInventoryService.TryDrop(
                                entry.ownershipId,
                                entry.quantity,
                                out inventoryMessage);
                            GUIUtility.ExitGUI();
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }

            if (!canSell &&
                !string.IsNullOrWhiteSpace(sellReason))
            {
                EditorGUILayout.LabelField(
                    "Sell",
                    sellReason,
                    EditorStyles.miniLabel);
            }
            if (!canDrop &&
                !string.IsNullOrWhiteSpace(dropReason))
            {
                EditorGUILayout.LabelField(
                    "Drop",
                    dropReason,
                    EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
        }

        private static bool IsUsableInventoryItem(
            DeverQuestShopItem item)
        {
            return item != null &&
                   (item.itemType ==
                    DeverQuestShopItemType.Consumable ||
                    item.itemType ==
                    DeverQuestShopItemType.Food ||
                    item.itemType ==
                    DeverQuestShopItemType.Drink ||
                    item.itemType ==
                    DeverQuestShopItemType.InnRest ||
                    item.itemType ==
                    DeverQuestShopItemType.BreakPermit);
        }

        private void DrawEconomyWorkspace()
        {
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            bool canManage =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);

            EditorGUILayout.LabelField(
                "Guild Economy and Item Operations",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Configure the active Quartermaster, consolidate coin, " +
                "issue audited leadership grants, and review the local " +
                "economy ledger.",
                EditorStyles.wordWrappedLabel);

            EditorGUILayout.Space(6f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("Coin Purse", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Canonical Value",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Physical Pieces",
                $"{adventurer.platinumCoins}p · " +
                $"{adventurer.goldCoins}g · " +
                $"{adventurer.silverCoins}s · " +
                $"{adventurer.copperCoins}c " +
                $"({DeverQuestAdventurerService.CoinPieceCount(adventurer)} " +
                "pieces)");
            EditorGUILayout.HelpBox(
                "Denomination exchange changes the number of physical coin " +
                "pieces, never the purse's canonical copper value.",
                MessageType.None);
            if (GUILayout.Button("Consolidate Coin Denominations"))
            {
                bool reduced =
                    DeverQuestAdventurerService.ExchangeCoinAtGuildHall(
                        out long before, out long after);
                economyMessage = reduced
                    ? $"Consolidated {before} pieces into {after}."
                    : "The purse is already fully consolidated.";
            }
            EditorGUILayout.EndVertical();

            economyMerchantFoldout = EditorGUILayout.Foldout(
                economyMerchantFoldout,
                "Active Quartermaster",
                true);
            if (economyMerchantFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUI.BeginChangeCheck();
                selectedShopProfile =
                    (DeverQuestShopProfile)EditorGUILayout.ObjectField(
                        "Shop Profile",
                        selectedShopProfile,
                        typeof(DeverQuestShopProfile),
                        false);
                if (EditorGUI.EndChangeCheck())
                {
                    DeverQuestShopService.SetActiveProfile(
                        selectedShopProfile);
                }

                if (selectedShopProfile == null)
                {
                    EditorGUILayout.HelpBox(
                        "Select or generate a Shop Profile before buying or " +
                        "selling items.",
                        MessageType.Info);
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Stock",
                        $"{(selectedShopProfile.items?.Count ?? 0)} item(s)");
                    if (canManage)
                    {
                        EditorGUI.BeginChangeCheck();
                        selectedShopProfile.shopOpen =
                            EditorGUILayout.Toggle(
                                "Shop Open",
                                selectedShopProfile.shopOpen);
                        selectedShopProfile.availableToMembers =
                            EditorGUILayout.Toggle(
                                "Available to Members",
                                selectedShopProfile.availableToMembers);
                        selectedShopProfile.allowPurchases =
                            EditorGUILayout.Toggle(
                                "Allow Purchases",
                                selectedShopProfile.allowPurchases);
                        selectedShopProfile.buyItemsFromMembers =
                            EditorGUILayout.Toggle(
                                "Buy Member Items",
                                selectedShopProfile.buyItemsFromMembers);
                        selectedShopProfile
                            .leadershipApprovalThresholdCopper =
                            EditorGUILayout.IntField(
                                "Approval Threshold (c)",
                                selectedShopProfile
                                    .leadershipApprovalThresholdCopper);
                        selectedShopProfile
                            .leadershipApprovalThresholdCopper =
                            Mathf.Max(
                                0,
                                selectedShopProfile
                                    .leadershipApprovalThresholdCopper);
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorUtility.SetDirty(selectedShopProfile);
                            AssetDatabase.SaveAssets();
                        }
                    }
                    else
                    {
                        EditorGUILayout.LabelField(
                            "Status",
                            selectedShopProfile.shopOpen
                                ? "Open"
                                : "Closed");
                    }
                    if (GUILayout.Button("Open Guild Shop"))
                    {
                        activeWorkspace = DeverQuestWorkspace.GuildHall;
                    }
                    if (canManage &&
                        GUILayout.Button("Select Shop Profile Asset"))
                    {
                        Selection.activeObject = selectedShopProfile;
                        EditorGUIUtility.PingObject(selectedShopProfile);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            economyGrantFoldout = EditorGUILayout.Foldout(
                economyGrantFoldout,
                "Leadership Grants",
                true);
            if (economyGrantFoldout)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                if (!canManage)
                {
                    EditorGUILayout.HelpBox(
                        "Guild leadership permission is required to issue " +
                        "item or coin grants.",
                        MessageType.Info);
                }
                else
                {
                    List<DeverQuestGuildAccount> accounts =
                        DeverQuestGuildAccountService.Accounts
                            .Where(account =>
                                account != null && !account.disabled)
                            .ToList();
                    if (accounts.Count == 0)
                    {
                        EditorGUILayout.LabelField(
                            "No enabled Guild accounts are available.");
                    }
                    else
                    {
                        economyAccountIndex = Mathf.Clamp(
                            economyAccountIndex, 0, accounts.Count - 1);
                        economyAccountIndex = EditorGUILayout.Popup(
                            "Recipient",
                            economyAccountIndex,
                            accounts.Select(account =>
                                $"{account.characterName} " +
                                $"({account.developerName})").ToArray());
                        DeverQuestGuildAccount target =
                            accounts[economyAccountIndex];
                        economyGrantNote = EditorGUILayout.TextField(
                            "Grant Note", economyGrantNote);
                        economyGrantItem =
                            (DeverQuestShopItem)EditorGUILayout.ObjectField(
                                "Item",
                                economyGrantItem,
                                typeof(DeverQuestShopItem),
                                false);
                        economyGrantQuantity = Mathf.Max(
                            1,
                            EditorGUILayout.IntField(
                                "Quantity", economyGrantQuantity));
                        using (new EditorGUI.DisabledScope(
                                   economyGrantItem == null))
                        {
                            if (GUILayout.Button("Grant Item…"))
                            {
                                bool confirmed =
                                    EditorUtility.DisplayDialog(
                                        "Confirm Item Grant",
                                        $"Grant {economyGrantQuantity} × " +
                                        $"{economyGrantItem.displayName} " +
                                        $"to {target.characterName}?",
                                        "Grant",
                                        "Cancel");
                                if (confirmed)
                                {
                                    DeverQuestEconomyService.GrantItem(
                                        target,
                                        economyGrantItem,
                                        economyGrantQuantity,
                                        economyGrantNote,
                                        out economyMessage);
                                }
                            }
                        }

                        economyGrantCopper = Math.Max(
                            1L,
                            EditorGUILayout.LongField(
                                "Coin Grant (c)",
                                economyGrantCopper));
                        if (GUILayout.Button("Grant Coin…"))
                        {
                            bool confirmed = EditorUtility.DisplayDialog(
                                "Confirm Coin Grant",
                                $"Grant " +
                                $"{DeverQuestAdventurerService.FormatCoins(economyGrantCopper)} " +
                                $"to {target.characterName}?",
                                "Grant",
                                "Cancel");
                            if (confirmed)
                            {
                                DeverQuestEconomyService.GrantCoin(
                                    target,
                                    economyGrantCopper,
                                    economyGrantNote,
                                    out economyMessage);
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }

            economyLedgerFoldout = EditorGUILayout.Foldout(
                economyLedgerFoldout,
                "Economy Transaction Ledger",
                true);
            if (economyLedgerFoldout)
            {
                DrawEconomyLedger();
            }

            if (!string.IsNullOrWhiteSpace(economyMessage))
            {
                EditorGUILayout.HelpBox(
                    economyMessage, MessageType.Info);
            }
        }

        private void DrawEconomyLedger()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            economySearch = EditorGUILayout.TextField(
                "Search", economySearch);
            string[] typeLabels = new[] { "All Transactions" }
                .Concat(Enum.GetNames(
                    typeof(DeverQuestEconomyTransactionType)))
                .ToArray();
            economyTransactionTypeIndex = Mathf.Clamp(
                economyTransactionTypeIndex, 0, typeLabels.Length - 1);
            economyTransactionTypeIndex = EditorGUILayout.Popup(
                "Type",
                economyTransactionTypeIndex,
                typeLabels);

            IEnumerable<DeverQuestEconomyTransaction> records =
                DeverQuestEconomyService.Records
                    .Where(record => record != null);
            if (economyTransactionTypeIndex > 0)
            {
                DeverQuestEconomyTransactionType selected =
                    (DeverQuestEconomyTransactionType)
                    (economyTransactionTypeIndex - 1);
                records = records.Where(record =>
                    record.transactionType == selected);
            }
            if (!string.IsNullOrWhiteSpace(economySearch))
            {
                string query = economySearch.Trim();
                records = records.Where(record =>
                    ContainsIgnoreCase(record.actorName, query) ||
                    ContainsIgnoreCase(
                        record.targetDeveloperName, query) ||
                    ContainsIgnoreCase(
                        record.targetAdventurerName, query) ||
                    ContainsIgnoreCase(record.itemName, query) ||
                    ContainsIgnoreCase(record.note, query) ||
                    ContainsIgnoreCase(
                        record.relatedRecordId, query));
            }
            DeverQuestEconomyTransaction[] visible =
                records.Take(50).ToArray();
            long income = visible
                .Where(record => record.balanceDeltaCopper > 0)
                .Sum(record => record.balanceDeltaCopper);
            long expense = -visible
                .Where(record => record.balanceDeltaCopper < 0)
                .Sum(record => record.balanceDeltaCopper);
            EditorGUILayout.LabelField(
                "Visible Summary",
                $"{visible.Length} record(s) · +" +
                $"{DeverQuestAdventurerService.FormatCoins(income)} · -" +
                DeverQuestAdventurerService.FormatCoins(expense));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Export Ledger CSV…"))
            {
                string path = EditorUtility.SaveFilePanel(
                    "Export DeverQuest Economy Ledger",
                    string.Empty,
                    "DeverQuest_Economy_Ledger.csv",
                    "csv");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    DeverQuestEconomyService.ExportCsv(
                        path, out economyMessage);
                }
            }
            if (GUILayout.Button("Open Inventory"))
            {
                activeWorkspace = DeverQuestWorkspace.Inventory;
            }
            EditorGUILayout.EndHorizontal();

            if (visible.Length == 0)
            {
                EditorGUILayout.LabelField(
                    "No economy transactions match the current filter.");
            }
            foreach (DeverQuestEconomyTransaction record in visible)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                string subject = !string.IsNullOrWhiteSpace(record.itemName)
                    ? record.itemName
                    : record.transactionType.ToString();
                EditorGUILayout.LabelField(
                    $"[{record.transactionType}] {subject}",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    "Recipient",
                    string.IsNullOrWhiteSpace(
                        record.targetAdventurerName)
                        ? "—"
                        : $"{record.targetAdventurerName} " +
                          $"({record.targetDeveloperName})");
                if (record.quantity > 0)
                {
                    EditorGUILayout.LabelField(
                        "Quantity", record.quantity.ToString());
                }
                if (record.copperAmount > 0 ||
                    record.balanceDeltaCopper != 0)
                {
                    string sign = record.balanceDeltaCopper > 0
                        ? "+"
                        : record.balanceDeltaCopper < 0
                            ? "-"
                            : string.Empty;
                    long displayedCopper =
                        record.balanceDeltaCopper != 0
                            ? Math.Abs(record.balanceDeltaCopper)
                            : record.copperAmount;
                    EditorGUILayout.LabelField(
                        "Coin",
                        sign + DeverQuestAdventurerService.FormatCoins(
                            displayedCopper));
                }
                if (record.coinPiecesBefore > 0 ||
                    record.coinPiecesAfter > 0)
                {
                    EditorGUILayout.LabelField(
                        "Coin Pieces",
                        $"{record.coinPiecesBefore} → " +
                        $"{record.coinPiecesAfter}");
                }
                if (!string.IsNullOrWhiteSpace(record.note))
                {
                    EditorGUILayout.LabelField(
                        record.note,
                        EditorStyles.wordWrappedLabel);
                }
                EditorGUILayout.LabelField(
                    "Recorded",
                    FormatUtcForDisplay(record.createdUtc),
                    EditorStyles.miniLabel);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private static bool ContainsIgnoreCase(
            string value,
            string query)
        {
            return !string.IsNullOrWhiteSpace(value) &&
                   value.IndexOf(
                       query,
                       StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private static string FormatUtcForDisplay(string utc)
        {
            return DateTime.TryParse(utc, out DateTime parsed)
                ? parsed.ToLocalTime().ToString("g")
                : utc;
        }

        private void DrawGuildShop()
        {
            guildShopFoldout = EditorGUILayout.Foldout(
                guildShopFoldout,
                "Guild Shop and Inventory",
                true);
            if (!guildShopFoldout)
            {
                return;
            }

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Guild Quartermaster",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Coin Purse",
                DeverQuestAdventurerService.FormatCoins(
                    adventurer.copperBalance));
            EditorGUILayout.LabelField(
                "Carry Weight",
                $"{DeverQuestEncumbranceService.CarriedWeight(adventurer):0.0} / " +
                $"{DeverQuestEncumbranceService.CarryCapacity(adventurer):0.0}");
            if (GUILayout.Button(
                    "Exchange Coin Denominations at Guild Hall"))
            {
                bool reduced =
                    DeverQuestAdventurerService.ExchangeCoinAtGuildHall(
                        out long piecesBefore,
                        out long piecesAfter);
                shopMessage = reduced
                    ? $"Coin consolidated from {piecesBefore} pieces to " +
                      $"{piecesAfter} pieces without changing its value."
                    : "The purse is already using the fewest available " +
                      "coin pieces.";
            }
            EditorGUI.BeginChangeCheck();
            selectedShopProfile =
                (DeverQuestShopProfile)EditorGUILayout.ObjectField(
                    "Shop Profile",
                    selectedShopProfile,
                    typeof(DeverQuestShopProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                DeverQuestShopService.SetActiveProfile(
                    selectedShopProfile);
            }

            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Shop Profile…"))
                {
                    CreateCharacterAsset<DeverQuestShopProfile>(
                        "NewGuildShop");
                }
                if (GUILayout.Button("Create Shop Item…"))
                {
                    CreateCharacterAsset<DeverQuestShopItem>(
                        "NewGuildShopItem");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Generate Starter Quartermaster"))
                {
                    selectedShopProfile =
                        DeverQuestStarterContentGenerator
                            .GenerateBasicShop();
                    DeverQuestShopService.SetActiveProfile(
                        selectedShopProfile);
                    shopMessage =
                        "Starter provisions and break permits generated " +
                        "under Assets/DeverQuest/GuildShop.";
                }
            }

            if (selectedShopProfile != null &&
                !DeverQuestShopService.CanBrowse(
                    selectedShopProfile,
                    out string merchantAvailability))
            {
                EditorGUILayout.HelpBox(
                    merchantAvailability,
                    MessageType.Warning);
            }
            else if (selectedShopProfile == null)
            {
                EditorGUILayout.HelpBox(
                    "Select a Shop Profile to browse the " +
                    "Quartermaster's stock.",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    selectedShopProfile.welcomeMessage,
                    MessageType.None);
                foreach (DeverQuestShopItem item
                         in selectedShopProfile.items)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    EditorGUILayout.BeginVertical(
                        EditorStyles.helpBox);
                    EditorGUILayout.LabelField(
                        item.displayName,
                        $"{item.rarity} {item.itemCategory} · " +
                        DeverQuestAdventurerService.FormatCoins(
                            item.copperCost));
                    if (!string.IsNullOrWhiteSpace(
                            item.description))
                    {
                        EditorGUILayout.LabelField(
                            item.description,
                            EditorStyles.wordWrappedLabel);
                    }
                    string approval =
                        item.requiresLeadershipApproval
                            ? " · Leadership approval required"
                            : string.Empty;
                    EditorGUILayout.LabelField(
                        $"Level {item.minimumLevel}{approval} · " +
                        $"{item.unitWeight:0.##} wt · Resale " +
                        $"{DeverQuestAdventurerService.FormatCoins(item.EffectiveSellValueCopper)}",
                        EditorStyles.miniLabel);
                    if (item.equipment != null)
                    {
                        EditorGUILayout.LabelField(
                            "Equipment",
                            DeverQuestInventoryService
                                .DescribeEquipment(item.equipment),
                            EditorStyles.wordWrappedLabel);
                        EditorGUILayout.LabelField(
                            "Comparison",
                            DeverQuestInventoryService
                                .DescribeComparison(
                                    item.equipment, adventurer),
                            EditorStyles.wordWrappedLabel);
                    }
                    float projectedWeight =
                        DeverQuestEncumbranceService
                            .CarriedWeight(adventurer) +
                        (item.equipment == null
                            ? item.unitWeight
                            : item.equipment.weight);
                    if (projectedWeight >
                        DeverQuestEncumbranceService
                            .CarryCapacity(adventurer))
                    {
                        EditorGUILayout.HelpBox(
                            "Purchasing this item would exceed the current " +
                            "carry capacity.",
                            MessageType.Warning);
                    }
                    using (new EditorGUI.DisabledScope(
                               adventurer.level <
                               item.minimumLevel ||
                               adventurer.copperBalance <
                               item.copperCost))
                    {
                        if (GUILayout.Button("Purchase"))
                        {
                            DeverQuestShopService.Purchase(
                                selectedShopProfile,
                                item,
                                out shopMessage);
                        }
                    }
                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.Space(5f);
            DeverQuestCarrySummary carry =
                DeverQuestEncumbranceService.Summary(adventurer);
            EditorGUILayout.LabelField(
                "Inventory",
                $"{adventurer.inventory.Count} entries · " +
                $"{carry.TotalWeight:0.0}/{carry.Capacity:0.0} · " +
                $"{carry.Status}",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Use the Inventory workspace for equipment comparison, " +
                "provenance, guarded dropping, and Quartermaster sales.",
                EditorStyles.wordWrappedLabel);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Inventory and Equipment"))
            {
                activeWorkspace = DeverQuestWorkspace.Inventory;
                Repaint();
            }
            if (GUILayout.Button("Open Guild Economy"))
            {
                activeWorkspace = DeverQuestWorkspace.Economy;
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            DrawTradingPost(adventurer);
            DrawPurchaseApprovals();
            if (!string.IsNullOrWhiteSpace(shopMessage))
            {
                EditorGUILayout.HelpBox(
                    shopMessage,
                    MessageType.Info);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawTradingPost(DeverQuestAdventurer adventurer)
        {
            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Trading Post", EditorStyles.boldLabel);
            List<DeverQuestGuildAccount> targets =
                DeverQuestGuildAccountService.Accounts
                    .Where(account =>
                        !account.disabled &&
                        account.accountId !=
                        DeverQuestGuildAccountService
                            .CurrentAccount?.accountId)
                    .ToList();
            if (targets.Count > 0)
            {
                tradeTargetIndex = Mathf.Clamp(
                    tradeTargetIndex, 0, targets.Count - 1);
                tradeTargetIndex = EditorGUILayout.Popup(
                    "Trade With",
                    tradeTargetIndex,
                    targets.Select(account =>
                        $"{account.characterName} " +
                        $"({account.developerName})").ToArray());
                foreach (DeverQuestInventoryEntry entry
                         in adventurer.inventory.ToArray())
                {
                    bool tradable = entry.tradable &&
                        !entry.questProtected &&
                        entry.itemCategory !=
                        DeverQuestItemCategory.QuestItem &&
                        entry.itemType !=
                        DeverQuestShopItemType.Redemption &&
                        entry.binding !=
                        DeverQuestItemBinding.BindOnPickup &&
                        entry.binding !=
                        DeverQuestItemBinding.AccountBound &&
                        string.IsNullOrWhiteSpace(
                            entry.boundAccountId);
                    using (new EditorGUI.DisabledScope(!tradable))
                    {
                        if (GUILayout.Button(
                                $"Offer {entry.displayName}"))
                        {
                            DeverQuestTradeService.Offer(
                                entry,
                                targets[tradeTargetIndex].accountId,
                                out shopMessage);
                        }
                    }
                }
            }
            else
            {
                EditorGUILayout.LabelField(
                    "Create another enabled Guild account to trade.");
            }

            string accountId =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty;
            foreach (DeverQuestTradeRecord trade
                     in DeverQuestTradeService.Records.Where(
                         value =>
                             value.status ==
                             DeverQuestTradeStatus.Offered &&
                             value.toAccountId == accountId).ToArray())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(
                    $"{trade.fromName} offers {trade.itemName}",
                    trade.rarity.ToString());
                if (GUILayout.Button("Accept", GUILayout.Width(65f)))
                {
                    DeverQuestTradeService.Accept(
                        trade, out shopMessage);
                }
                if (GUILayout.Button("Reject", GUILayout.Width(60f)))
                {
                    DeverQuestTradeService.Reject(
                        trade, out shopMessage);
                }
                EditorGUILayout.EndHorizontal();
            }
            foreach (DeverQuestTradeRecord trade
                     in DeverQuestTradeService.Records.Where(
                         value =>
                             value.fromAccountId == accountId &&
                             (value.status ==
                              DeverQuestTradeStatus.Offered ||
                              value.status ==
                              DeverQuestTradeStatus.Rejected)).ToArray())
            {
                if (GUILayout.Button(
                        $"{(trade.status == DeverQuestTradeStatus.Offered ? "Cancel" : "Reclaim")} " +
                        $"{trade.itemName} ({trade.toName})"))
                {
                    DeverQuestTradeService.CancelOrReclaim(
                        trade, out shopMessage);
                }
            }
            tradeLedgerFoldout = EditorGUILayout.Foldout(
                tradeLedgerFoldout, "Permanent Trade Ledger", true);
            if (tradeLedgerFoldout)
            {
                foreach (DeverQuestTradeRecord trade
                         in DeverQuestTradeService.Records.Take(20))
                {
                    EditorGUILayout.LabelField(
                        $"[{trade.status}] {trade.itemName}",
                        $"{trade.fromName} → {trade.toName}");
                }
            }
        }

        private void DrawPurchaseApprovals()
        {
            bool canApprove =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (canApprove)
            {
                List<DeverQuestPurchaseRecord> pending =
                    DeverQuestShopService.Records
                        .Where(record =>
                            record.status ==
                            DeverQuestPurchaseStatus.Requested)
                        .ToList();
                if (pending.Count > 0)
                {
                    EditorGUILayout.Space(6f);
                    EditorGUILayout.LabelField(
                        "Leadership Approval Queue",
                        EditorStyles.boldLabel);
                    foreach (DeverQuestPurchaseRecord record
                             in pending)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(
                            $"{record.adventurerName}: " +
                            $"{record.itemName}",
                            DeverQuestAdventurerService.FormatCoins(
                                record.copperCost));
                        if (GUILayout.Button(
                                "Approve",
                                GUILayout.Width(70f)))
                        {
                            DeverQuestShopService.Resolve(
                                record, true, out shopMessage);
                        }
                        if (GUILayout.Button(
                                "Deny",
                                GUILayout.Width(50f)))
                        {
                            DeverQuestShopService.Resolve(
                                record, false, out shopMessage);
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                List<DeverQuestPurchaseRecord> fulfillment =
                    DeverQuestShopService.Records.Where(record =>
                        record.itemType ==
                        DeverQuestShopItemType.Redemption &&
                        record.status ==
                        DeverQuestPurchaseStatus.Approved).ToList();
                if (fulfillment.Count > 0)
                {
                    EditorGUILayout.LabelField(
                        "Real Reward Fulfillment",
                        EditorStyles.boldLabel);
                    fulfillmentReference =
                        EditorGUILayout.TextField(
                            "Delivery Reference",
                            fulfillmentReference);
                    foreach (DeverQuestPurchaseRecord record
                             in fulfillment)
                    {
                        if (GUILayout.Button(
                                $"Mark Delivered: " +
                                $"{record.itemName} → " +
                                $"{record.adventurerName}"))
                        {
                            DeverQuestShopService.MarkFulfilled(
                                record,
                                fulfillmentReference,
                                out shopMessage);
                        }
                    }
                }
            }

            purchaseHistoryFoldout = EditorGUILayout.Foldout(
                purchaseHistoryFoldout,
                "Purchase and Redemption History",
                true);
            if (!purchaseHistoryFoldout)
            {
                return;
            }
            string accountId =
                DeverQuestGuildAccountService.CurrentAccount
                    ?.accountId ?? string.Empty;
            IEnumerable<DeverQuestPurchaseRecord> visible =
                canApprove
                    ? DeverQuestShopService.Records
                    : DeverQuestShopService.Records.Where(
                        record => record.accountId == accountId);
            foreach (DeverQuestPurchaseRecord record
                     in visible.Take(20))
            {
                EditorGUILayout.LabelField(
                    $"[{record.status}] {record.itemName}",
                    $"{record.adventurerName} · " +
                    DeverQuestAdventurerService.FormatCoins(
                        record.copperCost));
                if (!string.IsNullOrWhiteSpace(
                        record.fulfillmentReference))
                {
                    EditorGUILayout.LabelField(
                        "Delivery",
                        record.fulfillmentReference);
                }
            }
        }

        private static void DrawAbility(
            string label,
            DeverQuestAdventurer adventurer,
            DeverQuestAbility ability)
        {
            int score = DeverQuestRulesService.GetAbilityScore(
                adventurer, ability);
            int modifier =
                DeverQuestRulesService.AbilityModifier(score);
            EditorGUILayout.LabelField(
                label,
                $"{score} ({(modifier >= 0 ? "+" : string.Empty)}" +
                $"{modifier})");
        }

        private void DrawRulesLaboratory(
            DeverQuestProfile profile)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Rules Laboratory",
                EditorStyles.boldLabel);
            rulesAbility =
                (DeverQuestAbility)EditorGUILayout.EnumPopup(
                    "Ability", rulesAbility);
            rulesDifficultyClass = EditorGUILayout.IntField(
                "Difficulty Class", rulesDifficultyClass);
            rulesProficient = EditorGUILayout.Toggle(
                "Proficient", rulesProficient);
            rulesSeed = EditorGUILayout.TextField(
                new GUIContent(
                    "Recorded Seed",
                    "The same character, rules, and seed produce the same " +
                    "roll so future encounters can be audited."),
                rulesSeed);
            if (GUILayout.Button("Resolve Deterministic Check"))
            {
                DeverQuestRuleResult result =
                    DeverQuestRulesService.ResolveCheck(
                        DeverQuestAdventurerService.Adventurer,
                        rulesAbility,
                        rulesProficient,
                        rulesDifficultyClass,
                        rulesSeed,
                        profile.dailyDecreeCheckModifier);
                rulesResult =
                    $"{(result.Success ? "SUCCESS" : "FAILURE")} · " +
                    $"{result.Formula} vs DC {result.DifficultyClass}";
            }
            if (!string.IsNullOrWhiteSpace(rulesResult))
            {
                EditorGUILayout.HelpBox(
                    rulesResult,
                    MessageType.Info);
            }
            if (DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Equipment Asset…"))
                {
                    CreateCharacterAsset<DeverQuestEquipment>(
                        "NewDeverQuestEquipment");
                }
                if (GUILayout.Button("Create Spell Asset…"))
                {
                    CreateCharacterAsset<DeverQuestSpell>(
                        "NewDeverQuestSpell");
                }
                if (GUILayout.Button("Create Attack Technique…"))
                {
                    CreateCharacterAsset<DeverQuestAttackTechnique>(
                        "NewCombatTechnique");
                }
                if (GUILayout.Button("Create Ability Profile…"))
                {
                    CreateCharacterAsset<DeverQuestAbilityProfile>(
                        "NewAbilityProfile");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Monster Profile…"))
                {
                    CreateCharacterAsset<DeverQuestMonsterProfile>(
                        "NewMonsterProfile");
                }
                if (GUILayout.Button("Create Encounter Profile…"))
                {
                    CreateCharacterAsset<DeverQuestEncounterProfile>(
                        "NewEncounterProfile");
                }
                EditorGUILayout.EndHorizontal();
                if (GUILayout.Button(
                        "Generate Guildhall Training Encounter"))
                {
                    DeverQuestEncounterProfile encounter =
                        DeverQuestStarterContentGenerator
                            .GenerateTrainingEncounter();
                    Selection.activeObject = encounter;
                    EditorGUIUtility.PingObject(encounter);
                    rulesResult =
                        "Generated a safe two-wave training Encounter " +
                        "under Assets/DeverQuest/Encounters.";
                }
                if (GUILayout.Button(
                        "Generate Tactical Starter Kit + Quest Templates"))
                {
                    DeverQuestTacticalContentReport report =
                        DeverQuestTacticalContentGenerator
                            .GenerateStarterKit();
                    UnityEngine.Object folder =
                        AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(
                            report.RootPath);
                    Selection.activeObject = folder;
                    EditorGUIUtility.PingObject(folder);
                    rulesResult =
                        $"Tactical kit ready: {report.Created} created, " +
                        $"{report.Updated} updated. Includes original " +
                        "abilities, class profiles, a 15-minute skirmish, " +
                        "and an endless survival Quest.";
                }
                if (GUILayout.Button("Create Starter Loadout…"))
                {
                    CreateCharacterAsset<DeverQuestStarterLoadout>(
                        "NewStarterLoadout");
                }
                if (GUILayout.Button(
                        "Generate Copper–Steel Starter Gear"))
                {
                    int created =
                        DeverQuestStarterContentGenerator
                            .GenerateBasicGear();
                    rulesResult =
                        $"Generated {created} new starter gear asset(s) " +
                        "under Assets/DeverQuest/StarterGear.";
                }
                if (GUILayout.Button(
                        "Generate Guild Combat Codex"))
                {
                    DeverQuestCombatTypeCatalog catalog =
                        DeverQuestStarterContentGenerator
                            .GenerateCombatCodex();
                    Selection.activeObject = catalog;
                    EditorGUIUtility.PingObject(catalog);
                    rulesResult =
                        "Generated the complete creature and damage-type " +
                        "codex under Assets/DeverQuest/Combat.";
                }
                selectedRulesEquipment =
                    (DeverQuestEquipment)EditorGUILayout.ObjectField(
                        "Equipment",
                        selectedRulesEquipment,
                        typeof(DeverQuestEquipment),
                        false);
                using (new EditorGUI.DisabledScope(
                           selectedRulesEquipment == null ||
                           (selectedRulesEquipment != null &&
                            DeverQuestAdventurerService.Adventurer.level <
                            selectedRulesEquipment.minimumLevel)))
                {
                    if (GUILayout.Button("Grant and Equip"))
                    {
                        DeverQuestAdventurer character =
                            DeverQuestAdventurerService.Adventurer;
                        DeverQuestRulesService.Equip(
                            character, selectedRulesEquipment);
                        DeverQuestAdventurerService.Save();
                        DeverQuestGuildAccountService.AddAudit(
                            "Equipment Granted",
                            character.characterName,
                            selectedRulesEquipment.displayName);
                    }
                }
                selectedRulesSpell =
                    (DeverQuestSpell)EditorGUILayout.ObjectField(
                        "Spell",
                        selectedRulesSpell,
                        typeof(DeverQuestSpell),
                        false);
                using (new EditorGUI.DisabledScope(
                           selectedRulesSpell == null ||
                           (selectedRulesSpell != null &&
                            DeverQuestAdventurerService.Adventurer.level <
                            selectedRulesSpell.minimumCharacterLevel)))
                {
                    if (GUILayout.Button("Teach Spell"))
                    {
                        DeverQuestAdventurer character =
                            DeverQuestAdventurerService.Adventurer;
                        if (!character.knownSpellIds.Contains(
                                selectedRulesSpell.SpellId))
                        {
                            character.knownSpellIds.Add(
                                selectedRulesSpell.SpellId);
                            DeverQuestAdventurerService.Save();
                        }
                        DeverQuestGuildAccountService.AddAudit(
                            "Spell Taught",
                            character.characterName,
                            selectedRulesSpell.displayName);
                    }
                }
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private static T CreateCharacterAsset<T>(
            string defaultName)
            where T : ScriptableObject
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Character Asset",
                defaultName,
                "asset",
                "Choose where to save the character rules asset.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            T asset = CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            Selection.activeObject = asset;
            EditorGUIUtility.PingObject(asset);
            return asset;
        }

        private static void DrawCampaignRulesSetup(
            DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Daily Decree Campaign Rules",
                EditorStyles.boldLabel);
            profile.dailyDecreeRecommendedLevel =
                EditorGUILayout.IntField(
                    "Recommended Level",
                    profile.dailyDecreeRecommendedLevel);
            profile.campaignDifficulty =
                (DeverQuestCampaignDifficulty)
                EditorGUILayout.EnumPopup(
                    "Campaign Difficulty",
                    profile.campaignDifficulty);
            profile.dailyDecreeCheckModifier =
                EditorGUILayout.IntSlider(
                    new GUIContent(
                        "Check Modifier",
                        "Applied to deterministic ability and saving-throw " +
                        "checks for the active Daily Decree."),
                    profile.dailyDecreeCheckModifier,
                    -10,
                    10);
        }

        private static string DrawStringPopup(
            string label,
            string current,
            string[] options)
        {
            int index = Array.IndexOf(options, current);
            index = Mathf.Max(0, index);
            return options[EditorGUILayout.Popup(label, index, options)];
        }

        private void DrawWellnessCommandCenter(
            DeverQuestProfile profile)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Wellness Command Center",
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Review the active reminder, queued and snoozed prompts, " +
                "quiet hours, break qualification, and local notification " +
                "history from one workspace.",
                wrappedLabelStyle);

            EditorGUILayout.Space(4f);
            EditorGUILayout.LabelField(
                "Status",
                EditorStyles.miniBoldLabel);
            EditorGUILayout.LabelField(
                "Active Reminder",
                DeverQuestWellnessMonitor.HasActiveReminder
                    ? DeverQuestWellnessMonitor.ActiveTitle
                    : "None");
            EditorGUILayout.LabelField(
                "Queued / Snoozed",
                DeverQuestWellnessMonitor.PendingCount.ToString());
            EditorGUILayout.LabelField(
                "Next Session Reminder",
                DeverQuestWellnessMonitor.NextSessionReminderSummary());
            EditorGUILayout.LabelField(
                "Quiet Hours",
                DeverQuestWellnessMonitor.QuietHoursActive
                    ? "Active until " +
                      DeverQuestWellnessMonitor.QuietHoursEndsAtLocal
                          .ToString("h:mm tt")
                    : "Inactive");

            if (DeverQuestSessionStore.HasActiveSession &&
                DeverQuestSessionStore.ActiveSession
                    .approvedBreakUntilUtcTicks > DateTime.UtcNow.Ticks)
            {
                DeverQuestSession session =
                    DeverQuestSessionStore.ActiveSession;
                double remaining = TimeSpan.FromTicks(
                    session.approvedBreakUntilUtcTicks -
                    DateTime.UtcNow.Ticks).TotalSeconds;
                int minimum = Mathf.CeilToInt(
                    session.approvedBreakPlannedMinutes * 0.8f);
                EditorGUILayout.HelpBox(
                    "Approved Break: " + FormatDuration(remaining) +
                    " remaining · minimum " + minimum +
                    " minute(s) for benefit.",
                    MessageType.Info);
            }

            DrawWellnessReminder();

            wellnessQueueFoldout = EditorGUILayout.Foldout(
                wellnessQueueFoldout,
                $"Reminder Queue ({DeverQuestWellnessMonitor.PendingCount})",
                true);
            if (wellnessQueueFoldout)
            {
                IReadOnlyList<DeverQuestWellnessReminder> pending =
                    DeverQuestWellnessMonitor.PendingReminders;
                if (pending.Count == 0)
                {
                    EditorGUILayout.LabelField(
                        "No queued or snoozed reminders.",
                        EditorStyles.miniLabel);
                }
                else
                {
                    foreach (DeverQuestWellnessReminder reminder in pending)
                    {
                        EditorGUILayout.BeginHorizontal(
                            EditorStyles.helpBox);
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField(
                            reminder.title,
                            EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(
                            DeverQuestWellnessMonitor
                                .PendingDueSummary(reminder),
                            EditorStyles.miniLabel);
                        EditorGUILayout.EndVertical();
                        if (GUILayout.Button(
                                "Dismiss",
                                GUILayout.Width(72f)))
                        {
                            DeverQuestWellnessMonitor.DismissPending(
                                reminder.reminderId);
                            GUIUtility.ExitGUI();
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("Clear Reminder Queue"))
                    {
                        if (EditorUtility.DisplayDialog(
                                "Clear Wellness Reminder Queue?",
                                "This dismisses queued and snoozed reminders. " +
                                "It does not remove Session Wellness Journal " +
                                "entries or local notification history.",
                                "Clear Queue",
                                "Cancel"))
                        {
                            DeverQuestWellnessMonitor.ClearPending();
                        }
                    }
                }
            }

            wellnessSettingsFoldout = EditorGUILayout.Foldout(
                wellnessSettingsFoldout,
                "Reminder Settings and Cue Tests",
                true);
            if (wellnessSettingsFoldout)
            {
                EditorGUI.BeginChangeCheck();
                profile.showEditorNotifications = EditorGUILayout.Toggle(
                    "Editor Notifications",
                    profile.showEditorNotifications);
                profile.notificationSoundsEnabled = EditorGUILayout.Toggle(
                    "Notification Cues",
                    profile.notificationSoundsEnabled);
                profile.autoOpenWindowForReminders = EditorGUILayout.Toggle(
                    "Open DeverQuest for Reminder",
                    profile.autoOpenWindowForReminders);
                profile.showWellnessInQuestHud = EditorGUILayout.Toggle(
                    "Show Wellness in Quest HUD",
                    profile.showWellnessInQuestHud);
                profile.suppressWellnessDuringQuietHours =
                    EditorGUILayout.Toggle(
                        "Suppress Session Reminders in Quiet Hours",
                        profile.suppressWellnessDuringQuietHours);
                profile.wellnessHistoryLimit = EditorGUILayout.IntField(
                    "History Record Limit",
                    profile.wellnessHistoryLimit);
                DrawWellnessSetup(profile);
                if (EditorGUI.EndChangeCheck())
                {
                    profile.Sanitize();
                    DeverQuestSettingsStore.Save();
                }

                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField(
                    "Test Reminder and Cue",
                    EditorStyles.miniBoldLabel);
                DrawWellnessTestButtons();
            }

            DrawWellnessHistory();
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawWellnessTestButtons()
        {
            DeverQuestWellnessType[] types =
            {
                DeverQuestWellnessType.CheckIn,
                DeverQuestWellnessType.Hydration,
                DeverQuestWellnessType.MovementBreak,
                DeverQuestWellnessType.Exercise,
                DeverQuestWellnessType.Lunch,
                DeverQuestWellnessType.Dinner,
                DeverQuestWellnessType.QuietHours
            };
            for (int index = 0; index < types.Length; index += 2)
            {
                EditorGUILayout.BeginHorizontal();
                for (int offset = 0; offset < 2; offset++)
                {
                    int candidate = index + offset;
                    if (candidate >= types.Length)
                    {
                        GUILayout.FlexibleSpace();
                        continue;
                    }
                    DeverQuestWellnessType type = types[candidate];
                    if (GUILayout.Button("Test " + WellnessLabel(type)))
                    {
                        DeverQuestWellnessMonitor.TriggerTest(type);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void DrawWellnessHistory()
        {
            wellnessHistoryFoldout = EditorGUILayout.Foldout(
                wellnessHistoryFoldout,
                "Notification History",
                true);
            if (!wellnessHistoryFoldout)
            {
                return;
            }

            EditorGUILayout.BeginHorizontal();
            wellnessHistorySearch = EditorGUILayout.TextField(
                "Search",
                wellnessHistorySearch);
            if (GUILayout.Button("Clear", GUILayout.Width(58f)))
            {
                wellnessHistorySearch = string.Empty;
            }
            EditorGUILayout.EndHorizontal();

            string[] filters =
            {
                "All",
                "Presented",
                "Breaks",
                "Snoozed",
                "Acknowledged",
                "Suppressed",
                "Tests"
            };
            wellnessHistoryFilter = EditorGUILayout.Popup(
                "Filter",
                Mathf.Clamp(wellnessHistoryFilter, 0, filters.Length - 1),
                filters);

            List<DeverQuestWellnessHistoryRecord> records =
                DeverQuestWellnessHistoryService.Records
                    .Where(WellnessHistoryMatches)
                    .Take(50)
                    .ToList();
            EditorGUILayout.LabelField(
                "Visible Records",
                records.Count.ToString());

            if (records.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No matching wellness notifications.",
                    EditorStyles.miniLabel);
            }
            foreach (DeverQuestWellnessHistoryRecord record in records)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    record.title + " · " + record.action,
                    EditorStyles.boldLabel);
                DateTime created = record.createdUtcTicks > 0L
                    ? new DateTime(
                        record.createdUtcTicks,
                        DateTimeKind.Utc).ToLocalTime()
                    : DateTime.MinValue;
                EditorGUILayout.LabelField(
                    created == DateTime.MinValue
                        ? "Unknown time"
                        : created.ToString("g"),
                    EditorStyles.miniLabel);
                if (!string.IsNullOrWhiteSpace(record.detail))
                {
                    EditorGUILayout.LabelField(
                        record.detail,
                        wrappedLabelStyle);
                }
                if (!string.IsNullOrWhiteSpace(record.sessionId))
                {
                    EditorGUILayout.LabelField(
                        "Session",
                        record.sessionId);
                }
                EditorGUILayout.EndVertical();
            }

            if (GUILayout.Button("Clear Local Notification History"))
            {
                if (EditorUtility.DisplayDialog(
                        "Clear Local Wellness History?",
                        "This removes the local notification command history " +
                        "under Library/DeverQuest. Session Wellness Journal " +
                        "entries and generated Timecards are preserved.",
                        "Clear History",
                        "Cancel"))
                {
                    DeverQuestWellnessHistoryService.Clear();
                }
            }
        }

        private bool WellnessHistoryMatches(
            DeverQuestWellnessHistoryRecord record)
        {
            if (record == null)
            {
                return false;
            }
            string search = wellnessHistorySearch?.Trim() ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(search))
            {
                string haystack = string.Join(
                    " ",
                    record.title,
                    record.action,
                    record.detail,
                    record.sessionId,
                    record.type.ToString());
                if (haystack.IndexOf(
                        search,
                        StringComparison.OrdinalIgnoreCase) < 0)
                {
                    return false;
                }
            }

            switch (wellnessHistoryFilter)
            {
                case 1:
                    return record.action == "Presented";
                case 2:
                    return record.action.IndexOf(
                        "Break",
                        StringComparison.OrdinalIgnoreCase) >= 0;
                case 3:
                    return record.action == "Snoozed";
                case 4:
                    return record.action == "Acknowledged";
                case 5:
                    return record.action.IndexOf(
                        "Suppressed",
                        StringComparison.OrdinalIgnoreCase) >= 0;
                case 6:
                    return record.testRecord;
                default:
                    return true;
            }
        }

        private static string WellnessLabel(
            DeverQuestWellnessType type)
        {
            switch (type)
            {
                case DeverQuestWellnessType.MovementBreak:
                    return "Movement";
                case DeverQuestWellnessType.Hydration:
                    return "Hydration";
                case DeverQuestWellnessType.Exercise:
                    return "Exercise";
                case DeverQuestWellnessType.Lunch:
                    return "Lunch";
                case DeverQuestWellnessType.Dinner:
                    return "Dinner";
                case DeverQuestWellnessType.QuietHours:
                    return "Quiet Hours";
                default:
                    return "Check-In";
            }
        }

        private void DrawWellnessSetup(DeverQuestProfile profile)
        {
            EditorGUILayout.Space(10f);
            EditorGUILayout.LabelField(
                "Wellness Reminder Schedule",
                EditorStyles.boldLabel);

            profile.wellnessEnabled =
                EditorGUILayout.Toggle(
                    "Enable Wellness",
                    profile.wellnessEnabled);

            using (new EditorGUI.DisabledScope(!profile.wellnessEnabled))
            {
                profile.checkInMinutes =
                    EditorGUILayout.IntField(
                        "Fallback Check-In (min)",
                        profile.checkInMinutes);

                focusScheduleText = EditorGUILayout.TextField(
                    new GUIContent(
                        "Check-In Schedule",
                        "Comma-separated focused minutes, such as " +
                        "15, 30, 45, 60. Empty uses the fallback interval."),
                    focusScheduleText);
                ApplyFocusSchedule(profile, focusScheduleText);

                profile.hydrationMinutes =
                    EditorGUILayout.IntField(
                        "Hydration (min)",
                        profile.hydrationMinutes);

                profile.movementBreakMinutes =
                    EditorGUILayout.IntField(
                        "Movement Break (min)",
                        profile.movementBreakMinutes);

                profile.exerciseMinutes =
                    EditorGUILayout.IntField(
                        "Exercise (min)",
                        profile.exerciseMinutes);

                profile.snoozeMinutes =
                    EditorGUILayout.IntField(
                        "Default Snooze (min)",
                        profile.snoozeMinutes);

                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField(
                    "Completed Break Benefits",
                    EditorStyles.miniBoldLabel);
                profile.wellnessShortBreakMinutes =
                    EditorGUILayout.IntField(
                        "Short Break (min)",
                        profile.wellnessShortBreakMinutes);
                profile.wellnessMealBreakMinutes =
                    EditorGUILayout.IntField(
                        "Meal Break (min)",
                        profile.wellnessMealBreakMinutes);
                profile.wellnessQuietBreakMinutes =
                    EditorGUILayout.IntField(
                        "Quiet Break (min)",
                        profile.wellnessQuietBreakMinutes);
                profile.wellnessBreakExperience =
                    EditorGUILayout.IntField(
                        "Completed Break XP",
                        profile.wellnessBreakExperience);

                profile.mealRemindersEnabled =
                    EditorGUILayout.Toggle(
                        "Meal Reminders",
                        profile.mealRemindersEnabled);

                using (new EditorGUI.DisabledScope(
                           !profile.mealRemindersEnabled))
                {
                    profile.lunchHour =
                        EditorGUILayout.IntSlider(
                            "Lunch Hour",
                            profile.lunchHour,
                            0,
                            23);
                    profile.lunchMinute =
                        EditorGUILayout.IntSlider(
                            "Lunch Minute",
                            profile.lunchMinute,
                            0,
                            59);
                    profile.dinnerHour =
                        EditorGUILayout.IntSlider(
                            "Dinner Hour",
                            profile.dinnerHour,
                            0,
                            23);
                    profile.dinnerMinute =
                        EditorGUILayout.IntSlider(
                            "Dinner Minute",
                            profile.dinnerMinute,
                            0,
                            59);
                }

                profile.quietHoursEnabled =
                    EditorGUILayout.Toggle(
                        "Quiet Hours",
                        profile.quietHoursEnabled);

                using (new EditorGUI.DisabledScope(
                           !profile.quietHoursEnabled))
                {
                    profile.quietHoursStartHour =
                        EditorGUILayout.IntSlider(
                            "Quiet Start Hour",
                            profile.quietHoursStartHour,
                            0,
                            23);
                    profile.quietHoursEndHour =
                        EditorGUILayout.IntSlider(
                            "Quiet End Hour",
                            profile.quietHoursEndHour,
                            0,
                            23);
                }
            }

            profile.Sanitize();
        }

        private void DrawWellnessReminder()
        {
            if (!DeverQuestWellnessMonitor.HasActiveReminder)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                DeverQuestWellnessMonitor.ActiveTitle +
                (DeverQuestWellnessMonitor.ActiveIsTest
                    ? " · Test"
                    : string.Empty),
                EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                DeverQuestWellnessMonitor.ActiveMessage,
                wrappedLabelStyle);

            int recommended =
                DeverQuestWellnessMonitor.RecommendedBreakMinutes;
            int required =
                DeverQuestWellnessMonitor.RequiredBreakMinutes;
            EditorGUILayout.HelpBox(
                "Recommended break: " + recommended +
                " minute(s). Complete at least " + required +
                " minute(s) to earn the configured wellness benefit. " +
                "Acknowledge dismisses the reminder without a break benefit.",
                MessageType.Info);

            using (new EditorGUI.DisabledScope(
                       !DeverQuestWellnessMonitor.CanStartApprovedBreak))
            {
                if (GUILayout.Button("Take Approved Break"))
                {
                    DeverQuestWellnessMonitor.Acknowledge(true);
                    Repaint();
                }
            }
            if (!DeverQuestWellnessMonitor.CanStartApprovedBreak)
            {
                EditorGUILayout.LabelField(
                    "Start an active running Quest to use an Approved Break.",
                    EditorStyles.miniLabel);
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Acknowledge"))
            {
                DeverQuestWellnessMonitor.Acknowledge(false);
                Repaint();
            }
            if (GUILayout.Button("Snooze 5m"))
            {
                DeverQuestWellnessMonitor.Snooze(5);
                Repaint();
            }
            if (GUILayout.Button(
                    "Snooze " +
                    DeverQuestSettingsStore.Profile.snoozeMinutes + "m"))
            {
                DeverQuestWellnessMonitor.Snooze();
                Repaint();
            }
            if (GUILayout.Button("Snooze 30m"))
            {
                DeverQuestWellnessMonitor.Snooze(30);
                Repaint();
            }
            EditorGUILayout.EndHorizontal();

            if (DeverQuestWellnessMonitor.PendingCount > 0)
            {
                EditorGUILayout.LabelField(
                    DeverQuestWellnessMonitor.PendingCount +
                    " additional reminder(s) are queued.",
                    EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawNewSessionForm(DeverQuestProfile profile)
        {
            if (profile.lockProjectName)
            {
                newProjectName = profile.lockedProjectName;
            }

            EditorGUILayout.LabelField(
                "Accept a Deliberate Quest",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                "Focused time begins only when you press Accept Quest.",
                MessageType.Info);

            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            bool canCreateCustomQuest =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    newProjectName) ||
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);

            EditorGUI.BeginChangeCheck();
            selectedQuestContract =
                (DeverQuestQuestContract)EditorGUILayout.ObjectField(
                    "Quest Contract",
                    selectedQuestContract,
                    typeof(DeverQuestQuestContract),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                ApplySelectedQuestContract();
            }

            string contractJoinReason = string.Empty;
            bool contractAssignedToAdventurer =
                selectedQuestContract != null &&
                DeverQuestContractService.CanJoin(
                    selectedQuestContract,
                    adventurer,
                    out contractJoinReason);
            bool contractStatusAvailable =
                selectedQuestContract != null &&
                (selectedQuestContract.status ==
                 DeverQuestContractStatus.Offered ||
                 selectedQuestContract.status ==
                 DeverQuestContractStatus.Accepted ||
                 (selectedQuestContract.status ==
                  DeverQuestContractStatus.Active &&
                  selectedQuestContract.ContainsAdventurer(
                      adventurer.characterName)) ||
                 (canCreateCustomQuest &&
                  selectedQuestContract.status ==
                  DeverQuestContractStatus.Draft));
            bool contractUnavailable =
                selectedQuestContract != null &&
                (!contractStatusAvailable ||
                 (!canCreateCustomQuest &&
                  (!contractAssignedToAdventurer ||
                   adventurer.level <
                   selectedQuestContract.minimumAdventurerLevel)));

            if (!canCreateCustomQuest && selectedQuestContract == null)
            {
                EditorGUILayout.HelpBox(
                    "Members must select an assigned or open Quest Contract.",
                    MessageType.Warning);
            }
            else if (contractUnavailable)
            {
                EditorGUILayout.HelpBox(
                    !contractStatusAvailable
                        ? $"This Contract is {selectedQuestContract.status} " +
                          "and cannot currently be accepted."
                        : contractJoinReason,
                    MessageType.Warning);
            }

            using (new EditorGUI.DisabledScope(!canCreateCustomQuest))
            {
            EditorGUI.BeginChangeCheck();
            selectedQuestProfile =
                (DeverQuestQuestProfile)EditorGUILayout.ObjectField(
                    "Quest Profile",
                    selectedQuestProfile,
                    typeof(DeverQuestQuestProfile),
                    false);
            if (EditorGUI.EndChangeCheck())
            {
                ApplySelectedQuestProfile();
            }
            }

            if (canCreateCustomQuest)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Quest Profile…"))
                {
                    CreateQuestProfileAsset();
                }
                if (GUILayout.Button("Create Quest Contract…"))
                {
                    CreateQuestContractAsset(adventurer);
                }
                if (selectedQuestProfile != null &&
                    GUILayout.Button("Inspect Selected Profile"))
                {
                    Selection.activeObject = selectedQuestProfile;
                    EditorGUIUtility.PingObject(selectedQuestProfile);
                }
                EditorGUILayout.EndHorizontal();
                if (selectedQuestContract != null &&
                    GUILayout.Button("Inspect Selected Contract"))
                {
                    Selection.activeObject = selectedQuestContract;
                    EditorGUIUtility.PingObject(selectedQuestContract);
                }
            }

            DrawContractBoard(adventurer, canCreateCustomQuest);

            if (selectedQuestContract != null &&
                !string.IsNullOrWhiteSpace(
                    selectedQuestContract.questStory))
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    "Quest Story",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    selectedQuestContract.questStory,
                    wrappedLabelStyle);
                EditorGUILayout.EndVertical();
            }

            bool profileUnavailable =
                !canCreateCustomQuest &&
                selectedQuestContract == null &&
                selectedQuestProfile != null &&
                (!selectedQuestProfile.availableToMembers ||
                 adventurer.level <
                 selectedQuestProfile.minimumAdventurerLevel);
            if (profileUnavailable)
            {
                EditorGUILayout.HelpBox(
                    !selectedQuestProfile.availableToMembers
                        ? "This Quest Profile is reserved for Guild leadership."
                        : $"Requires Adventurer Level " +
                          $"{selectedQuestProfile.minimumAdventurerLevel}.",
                    MessageType.Warning);
            }

            bool lockQuestFields =
                !canCreateCustomQuest &&
                selectedQuestContract != null;

            using (new EditorGUI.DisabledScope(
                       profile.lockProjectName || lockQuestFields))
            {
                newProjectName = EditorGUILayout.TextField(
                    "Project",
                    profile.lockProjectName
                        ? profile.lockedProjectName
                        : newProjectName);
            }

            using (new EditorGUI.DisabledScope(lockQuestFields))
            {
                newTaskName = EditorGUILayout.TextField(
                    "Task / Milestone",
                    newTaskName);

                newCategory = EditorGUILayout.TextField(
                    "Department",
                    newCategory);

                EditorGUILayout.LabelField("Task Objective");
                newGoal = EditorGUILayout.TextArea(
                    newGoal,
                    GUILayout.MinHeight(54f));
            }

            DrawSelectedQuestSpoils(canCreateCustomQuest);
            if (selectedQuestContract != null)
            {
                EditorGUILayout.LabelField(
                    "Contract",
                    $"{selectedQuestContract.status} · " +
                    $"{selectedQuestContract.priority} · Due " +
                    $"{(string.IsNullOrWhiteSpace(selectedQuestContract.dueDate) ? "Unscheduled" : selectedQuestContract.dueDate)}");
                EditorGUILayout.LabelField(
                    "Assigned To",
                    selectedQuestContract.openToAnyMember
                        ? "Any eligible Member"
                        : selectedQuestContract.assignedAdventurer);
            }

            bool canStart =
                !string.IsNullOrWhiteSpace(newProjectName) &&
                !string.IsNullOrWhiteSpace(newTaskName) &&
                !profileUnavailable &&
                !contractUnavailable &&
                (canCreateCustomQuest ||
                 selectedQuestContract != null);

            if (!canStart)
            {
                string startBlockReason =
                    BuildQuestStartBlockReason(
                        canCreateCustomQuest,
                        profileUnavailable,
                        contractUnavailable,
                        contractStatusAvailable,
                        contractJoinReason);
                if (!string.IsNullOrWhiteSpace(startBlockReason))
                {
                    EditorGUILayout.HelpBox(
                        startBlockReason,
                        MessageType.Warning);
                }
            }

            using (new EditorGUI.DisabledScope(!canStart))
            {
                if (GUILayout.Button(
                        "Accept Quest",
                        GUILayout.Height(36f)))
                {
                    string contractRunId = string.Empty;
                    if (selectedQuestContract != null &&
                        !DeverQuestContractService.Join(
                            selectedQuestContract,
                            adventurer,
                            profile.developerName,
                            out contractRunId,
                            out string joinError))
                    {
                        EditorUtility.DisplayDialog(
                            "Cannot Join Quest",
                            joinError,
                            "Close");
                        return;
                    }
                    if (selectedQuestContract != null &&
                        selectedQuestContract.groupQuest &&
                        !selectedQuestContract.CanPartyStart)
                    {
                        EditorUtility.DisplayDialog(
                            "Party Joined",
                            "Your place is reserved. This Quest can begin " +
                            "after the minimum party has assembled.",
                            "Return to the Guild Hall");
                        Repaint();
                        return;
                    }
                    profile.lastProjectName = newProjectName;
                    profile.lastDepartmentName = newCategory;
                    DeverQuestSettingsStore.Save();
                    DeverQuestSessionStore.StartSession(
                        profile.developerName,
                        newProjectName,
                        newTaskName,
                        newCategory,
                        newGoal,
                        selectedQuestProfile,
                        selectedQuestContract,
                        contractRunId);

                    Repaint();
                }
            }
        }

        private string BuildQuestStartBlockReason(
            bool canCreateCustomQuest,
            bool profileUnavailable,
            bool contractUnavailable,
            bool contractStatusAvailable,
            string contractJoinReason)
        {
            if (string.IsNullOrWhiteSpace(newProjectName))
            {
                return "Enter or select a Project before accepting the Quest.";
            }
            if (string.IsNullOrWhiteSpace(newTaskName))
            {
                return "Enter or select a Task / Milestone before accepting " +
                       "the Quest.";
            }
            if (profileUnavailable)
            {
                return "The selected Quest Profile is unavailable to this " +
                       "Adventurer.";
            }
            if (selectedQuestContract != null &&
                !contractStatusAvailable)
            {
                return $"The selected Contract is " +
                       $"{selectedQuestContract.status} and cannot currently " +
                       "be accepted.";
            }
            if (contractUnavailable &&
                !string.IsNullOrWhiteSpace(contractJoinReason))
            {
                return contractJoinReason;
            }
            if (!canCreateCustomQuest &&
                selectedQuestContract == null)
            {
                return "Select an offered or assigned Quest Contract.";
            }
            return string.Empty;
        }

        private void DrawSelectedQuestSpoils(
            bool canManageQuest)
        {
            if (selectedQuestContract != null)
            {
                EditorGUILayout.LabelField(
                    "Effective Contract Spoils",
                    FormatSpoils(
                        selectedQuestContract.baseCopper,
                        selectedQuestContract.baseExperience,
                        selectedQuestContract.copperPerWorkBlock,
                        selectedQuestContract.experiencePerWorkBlock,
                        selectedQuestContract.workBlockMinutes));

                if (selectedQuestContract.questProfile != null &&
                    !selectedQuestContract.SpoilsMatchLinkedProfile())
                {
                    EditorGUILayout.HelpBox(
                        "This Contract's snapshotted Spoils differ from its " +
                        "linked Quest Profile. The Contract values shown " +
                        "above are the values that will actually be awarded.",
                        MessageType.Warning);
                    EditorGUILayout.LabelField(
                        "Linked Profile Spoils",
                        FormatSpoils(
                            selectedQuestContract.questProfile.baseCopper,
                            selectedQuestContract.questProfile.baseExperience,
                            selectedQuestContract.questProfile
                                .copperPerWorkBlock,
                            selectedQuestContract.questProfile
                                .experiencePerWorkBlock,
                            selectedQuestContract.questProfile
                                .workBlockMinutes));

                    using (new EditorGUI.DisabledScope(
                               !canManageQuest ||
                               !selectedQuestContract
                                   .CanRefreshSpoilsFromProfile()))
                    {
                        if (GUILayout.Button(
                                "Refresh Contract Spoils from Linked Profile"))
                        {
                            selectedQuestContract
                                .RefreshSpoilsFromProfile();
                            EditorUtility.SetDirty(
                                selectedQuestContract);
                            AssetDatabase.SaveAssets();
                            Repaint();
                        }
                    }
                }
                return;
            }

            if (selectedQuestProfile != null)
            {
                EditorGUILayout.LabelField(
                    "Profile Spoils",
                    FormatSpoils(
                        selectedQuestProfile.baseCopper,
                        selectedQuestProfile.baseExperience,
                        selectedQuestProfile.copperPerWorkBlock,
                        selectedQuestProfile.experiencePerWorkBlock,
                        selectedQuestProfile.workBlockMinutes));
            }
        }

        private static string FormatSpoils(
            int baseCopper,
            int baseExperience,
            int copperPerWorkBlock,
            int experiencePerWorkBlock,
            int workBlockMinutes)
        {
            return
                $"{DeverQuestAdventurerService.FormatCoins(baseCopper)} " +
                $"+ {baseExperience} XP base · " +
                $"{DeverQuestAdventurerService.FormatCoins(copperPerWorkBlock)} " +
                $"+ {experiencePerWorkBlock} XP per " +
                $"{Math.Max(1, workBlockMinutes)}m block";
        }

        private void ApplySelectedQuestContract()
        {
            if (selectedQuestContract == null)
            {
                appliedQuestContractId = string.Empty;
                return;
            }

            if (appliedQuestContractId ==
                selectedQuestContract.ContractId)
            {
                return;
            }

            appliedQuestContractId =
                selectedQuestContract.ContractId;

            bool canRefreshContract =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    selectedQuestContract.projectName) ||
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (canRefreshContract &&
                selectedQuestContract.CanRefreshSpoilsFromProfile() &&
                !selectedQuestContract.SpoilsMatchLinkedProfile())
            {
                selectedQuestContract.RefreshSpoilsFromProfile();
                EditorUtility.SetDirty(selectedQuestContract);
                AssetDatabase.SaveAssets();
            }

            selectedQuestProfile =
                selectedQuestContract.questProfile;
            appliedQuestProfileId = selectedQuestProfile == null
                ? string.Empty
                : selectedQuestProfile.ProfileId;
            newProjectName = selectedQuestContract.projectName;
            newTaskName = selectedQuestContract.taskName;
            newCategory = selectedQuestContract.department;
            newGoal = selectedQuestContract.objective;
        }

        private void ApplySelectedQuestProfile()
        {
            if (selectedQuestProfile == null)
            {
                appliedQuestProfileId = string.Empty;
                return;
            }

            if (appliedQuestProfileId ==
                selectedQuestProfile.ProfileId)
            {
                return;
            }

            appliedQuestProfileId = selectedQuestProfile.ProfileId;
            if (!string.IsNullOrWhiteSpace(
                    selectedQuestProfile.projectName))
            {
                newProjectName = selectedQuestProfile.projectName;
            }
            if (!string.IsNullOrWhiteSpace(
                    selectedQuestProfile.taskName))
            {
                newTaskName = selectedQuestProfile.taskName;
            }
            newCategory = selectedQuestProfile.department;
            newGoal = selectedQuestProfile.goalTemplate;
        }

        private void CreateQuestProfileAsset()
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Quest Profile",
                "NewDeverQuestProfile",
                "asset",
                "Choose where this Guild Quest Profile should be saved.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestQuestProfile questProfile =
                CreateInstance<DeverQuestQuestProfile>();
            AssetDatabase.CreateAsset(questProfile, path);
            AssetDatabase.SaveAssets();
            selectedQuestProfile = questProfile;
            appliedQuestProfileId = string.Empty;
            ApplySelectedQuestProfile();
            Selection.activeObject = questProfile;
            EditorGUIUtility.PingObject(questProfile);
        }

        private void CreateQuestContractAsset(
            DeverQuestAdventurer adventurer)
        {
            string path = EditorUtility.SaveFilePanelInProject(
                "Create DeverQuest Quest Contract",
                "NewQuestContract",
                "asset",
                "Choose where this assigned work Contract should be saved.");
            if (string.IsNullOrWhiteSpace(path))
            {
                return;
            }

            DeverQuestQuestContract contract =
                CreateInstance<DeverQuestQuestContract>();
            contract.InitializeFromProfile(
                selectedQuestProfile,
                adventurer.characterName);
            AssetDatabase.CreateAsset(contract, path);
            AssetDatabase.SaveAssets();
            selectedQuestContract = contract;
            appliedQuestContractId = string.Empty;
            ApplySelectedQuestContract();
            Selection.activeObject = contract;
            EditorGUIUtility.PingObject(contract);
        }

        private void DrawContractBoard(
            DeverQuestAdventurer adventurer,
            bool canManage)
        {
            contractBoardFoldout = EditorGUILayout.Foldout(
                contractBoardFoldout,
                "Guild Assignment Board",
                true);
            if (!contractBoardFoldout)
            {
                return;
            }

            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            int visibleCount = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract == null)
                {
                    continue;
                }

                bool canManageContract =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageContracts,
                        contract.projectName);
                bool retiredFromLiveBoard =
                    contract.archived ||
                    (!canManageContract &&
                     contract.status ==
                     DeverQuestContractStatus.Completed &&
                     contract.IsBoardComplete);
                if (retiredFromLiveBoard)
                {
                    continue;
                }


                bool assigned =
                    DeverQuestContractService.CanJoin(
                        contract,
                        adventurer,
                        out string joinReason);
                bool memberVisible =
                    !contract.archived &&
                    assigned &&
                    adventurer.level >=
                    contract.minimumAdventurerLevel &&
                    (contract.status ==
                     DeverQuestContractStatus.Offered ||
                     contract.status ==
                     DeverQuestContractStatus.Accepted ||
                     contract.status ==
                     DeverQuestContractStatus.Active);
                if (!canManageContract && !memberVisible)
                {
                    continue;
                }

                visibleCount++;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    contract.contractTitle,
                    $"{contract.status} · {contract.priority}");
                EditorGUILayout.LabelField(
                    "Assignee",
                    contract.openToAnyMember
                        ? "Any eligible Member"
                        : contract.assignedAdventurer);
                EditorGUILayout.LabelField(
                    "Due",
                    string.IsNullOrWhiteSpace(contract.dueDate)
                        ? "Unscheduled"
                        : contract.dueDate);
                EditorGUILayout.LabelField(
                    "Availability",
                    BuildContractAvailabilityLabel(contract));
                if (contract.archived)
                {
                    EditorGUILayout.LabelField(
                        "Board State",
                        "Archived · leadership history only");
                }
                EditorGUILayout.LabelField(
                    "Completed Runs",
                    contract.CompletedRunCount.ToString());
                if (contract.completionHistory != null &&
                    contract.completionHistory.Count > 0)
                {
                    DeverQuestContractCompletionRecord lastCompletion =
                        contract.completionHistory[
                            contract.completionHistory.Count - 1];
                    EditorGUILayout.LabelField(
                        "Last Completed By",
                        lastCompletion == null ||
                        lastCompletion.adventurerNames == null ||
                        lastCompletion.adventurerNames.Count == 0
                            ? "Unknown Adventurer"
                            : string.Join(
                                ", ",
                                lastCompletion.adventurerNames));
                }
                EditorGUILayout.LabelField(
                    contract.groupQuest ? "Party" : "Capacity",
                    contract.groupQuest
                        ? $"{contract.partyMembers.Count}/" +
                          $"{contract.maximumParticipants} joined"
                        : "1 Adventurer");
                if (contract.groupQuest)
                {
                    EditorGUILayout.LabelField(
                        "Start Rule",
                        contract.requireFullParty
                            ? "Full party required"
                            : $"Minimum {contract.RequiredPartySize}; " +
                              $"maximum {contract.maximumParticipants}");
                }
                EditorGUILayout.LabelField(
                    "Base Reward",
                    $"{DeverQuestAdventurerService.FormatCoins(contract.baseCopper)} " +
                    $"+ {contract.baseExperience} XP");
                EditorGUILayout.LabelField(
                    "Work Block",
                    $"Every {Math.Max(1, contract.workBlockMinutes)}m: " +
                    $"{DeverQuestAdventurerService.FormatCoins(contract.copperPerWorkBlock)} " +
                    $"+ {contract.experiencePerWorkBlock} XP");

                bool currentAdventurerWaiting =
                    contract.groupQuest &&
                    contract.partyMembers.Any(member =>
                        string.Equals(
                            member.adventurerName,
                            adventurer.characterName,
                            StringComparison.OrdinalIgnoreCase)) &&
                    !contract.CanPartyStart &&
                    string.IsNullOrWhiteSpace(
                        contract.ActivePartyRunId);
                if (currentAdventurerWaiting)
                {
                    EditorGUILayout.HelpBox(
                        "You are enlisted and waiting for the minimum " +
                        $"party size ({contract.partyMembers.Count}/" +
                        $"{contract.RequiredPartySize} required; " +
                        $"{contract.maximumParticipants} maximum).",
                        MessageType.Info);
                }
                if (!assigned &&
                    !string.IsNullOrWhiteSpace(joinReason))
                {
                    EditorGUILayout.HelpBox(
                        joinReason, MessageType.Warning);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select"))
                {
                    selectedQuestContract = contract;
                    appliedQuestContractId = string.Empty;
                    ApplySelectedQuestContract();
                }
                if (currentAdventurerWaiting &&
                    GUILayout.Button("Leave Party"))
                {
                    if (!DeverQuestContractService.LeaveParty(
                            contract,
                            adventurer,
                            out string leaveError))
                    {
                        EditorUtility.DisplayDialog(
                            "Cannot Leave Party",
                            leaveError,
                            "Close");
                    }
                    Repaint();
                }
                EditorGUILayout.EndHorizontal();

                if (canManageContract)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (contract.status ==
                            DeverQuestContractStatus.Completed &&
                        contract.IsBoardComplete &&
                        GUILayout.Button("Restore to Offered"))
                    {
                        bool confirmed = EditorUtility.DisplayDialog(
                            "Restore Completed Quest?",
                            "This preserves every Completion History record " +
                            "and opens one additional completion slot for " +
                            "this Contract. It does not remove prior rewards " +
                            "or Chronicle evidence.",
                            "Restore to Offered",
                            "Cancel");
                        if (confirmed &&
                            !DeverQuestContractService
                                .ReopenForAnotherRun(
                                    contract,
                                    out string reopenError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Restore Quest",
                                reopenError,
                                "Close");
                        }
                        Repaint();
                    }

                    if (GUILayout.Button("Archive Listing"))
                    {
                        if (!DeverQuestContractService.SetArchived(
                                contract,
                                true,
                                out string archiveError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Archive Listing",
                                archiveError,
                                "Close");
                        }
                        Repaint();
                    }

                    if ((contract.status ==
                         DeverQuestContractStatus.Draft ||
                         contract.status ==
                         DeverQuestContractStatus.Returned) &&
                        GUILayout.Button("Offer"))
                    {
                        DeverQuestContractService.SetStatus(
                            contract,
                            DeverQuestContractStatus.Offered);
                    }
                    else if (contract.status ==
                             DeverQuestContractStatus.Submitted)
                    {
                        if (GUILayout.Button("Return"))
                        {
                            DeverQuestContractService.SetStatus(
                                contract,
                                DeverQuestContractStatus.Returned);
                        }
                        if (GUILayout.Button("Approve"))
                        {
                            DeverQuestContractService.SetStatus(
                                contract,
                                DeverQuestContractStatus.Approved);
                        }
                    }
                    else if (contract.status ==
                             DeverQuestContractStatus.Approved &&
                             GUILayout.Button("Complete"))
                    {
                        DeverQuestContractService.SetStatus(
                            contract,
                            DeverQuestContractStatus.Completed);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            if (visibleCount == 0)
            {
                EditorGUILayout.LabelField(
                    canManage
                        ? "No Quest Contracts are on the live Guild Board. " +
                          "Completed listings remain visible to leadership " +
                          "until archived. Archived history remains available " +
                          "through Chronicle and Quest Run Management."
                        : "No Contracts are currently assigned to you.",
                    EditorStyles.wordWrappedLabel);
            }
        }

        private void DrawQuestRunManagement()
        {
            questRunManagementFoldout = EditorGUILayout.Foldout(
                questRunManagementFoldout,
                "Quest Run Management",
                true);
            if (!questRunManagementFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Active Runs and Waiting Parties",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Use these controls only for stale reservations. Cancelling " +
                "a reservation does not stop a Quest running in another " +
                "Unity project or Git clone.",
                MessageType.Info);

            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            int activeRunCount = 0;
            int waitingPartyCount = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract == null)
                {
                    continue;
                }

                bool canManage =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageContracts,
                        contract.projectName);
                if (!canManage)
                {
                    continue;
                }

                List<DeverQuestContractRunReservation> activeRuns =
                    (contract.activeRuns ??
                     new List<DeverQuestContractRunReservation>())
                        .ToList();
                bool waitingParty =
                    contract.groupQuest &&
                    string.IsNullOrWhiteSpace(contract.ActivePartyRunId) &&
                    contract.partyMembers != null &&
                    contract.partyMembers.Count > 0;
                if (activeRuns.Count == 0 && !waitingParty)
                {
                    continue;
                }

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    contract.contractTitle,
                    contract.archived
                        ? "Archived"
                        : contract.status.ToString());

                foreach (DeverQuestContractRunReservation run in activeRuns)
                {
                    if (run == null)
                    {
                        continue;
                    }
                    activeRunCount++;
                    string participants = run.adventurerNames == null ||
                                          run.adventurerNames.Count == 0
                        ? "Unknown Adventurer"
                        : string.Join(", ", run.adventurerNames);
                    EditorGUILayout.LabelField(
                        "Run",
                        ShortId(run.runId));
                    EditorGUILayout.LabelField(
                        run.groupRun ? "Party" : "Adventurer",
                        participants);
                    EditorGUILayout.LabelField(
                        "Started",
                        FormatRunTimestamp(run.startedUtc));
                    EditorGUILayout.LabelField(
                        "Age",
                        FormatRunAge(run.startedUtc));
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Select Contract"))
                    {
                        Selection.activeObject = contract;
                        EditorGUIUtility.PingObject(contract);
                    }
                    if (GUILayout.Button("Cancel Stale Run…"))
                    {
                        bool approved = EditorUtility.DisplayDialog(
                            "Cancel Quest Run Reservation?",
                            "This releases the Guild Board reservation for " +
                            participants + ". It does not stop a Session that " +
                            "is still open in another project or clone.",
                            "Cancel Reservation",
                            "Keep Run");
                        if (approved &&
                            !DeverQuestContractService.CancelRunReservation(
                                contract,
                                run.runId,
                                out string cancelError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Cancel Quest Run",
                                cancelError,
                                "Close");
                        }
                        Repaint();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space(4f);
                }

                if (waitingParty)
                {
                    waitingPartyCount++;
                    string waitingNames = string.Join(
                        ", ",
                        contract.partyMembers
                            .Where(member => member != null)
                            .Select(member => member.adventurerName));
                    EditorGUILayout.LabelField(
                        "Waiting Party",
                        waitingNames);
                    EditorGUILayout.LabelField(
                        "Capacity",
                        $"{contract.partyMembers.Count}/" +
                        $"{contract.RequiredPartySize} required · " +
                        $"{contract.maximumParticipants} maximum");
                    if (GUILayout.Button("Clear Waiting Party…"))
                    {
                        bool approved = EditorUtility.DisplayDialog(
                            "Clear Waiting Party?",
                            "This removes the waiting roster without creating " +
                            "a completion record.",
                            "Clear Roster",
                            "Keep Roster");
                        if (approved &&
                            !DeverQuestContractService.ClearWaitingParty(
                                contract,
                                out string clearError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Clear Waiting Party",
                                clearError,
                                "Close");
                        }
                        Repaint();
                    }
                }

                EditorGUILayout.EndVertical();
            }

            if (activeRunCount == 0 && waitingPartyCount == 0)
            {
                EditorGUILayout.LabelField(
                    "No active Quest Runs or waiting Parties were found.",
                    EditorStyles.miniLabel);
            }
            else
            {
                EditorGUILayout.LabelField(
                    "Summary",
                    $"{activeRunCount} active run(s) · " +
                    $"{waitingPartyCount} waiting party roster(s)");
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private void DrawQuestRunArchive()
        {
            questRunArchiveFoldout = EditorGUILayout.Foldout(
                questRunArchiveFoldout,
                "Completed Quest Run Archive",
                true);
            if (!questRunArchiveFoldout)
            {
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Guild Contract Completion Records",
                EditorStyles.boldLabel);
            questRunArchiveSearch = EditorGUILayout.TextField(
                "Search",
                questRunArchiveSearch);
            showArchivedContracts = EditorGUILayout.Toggle(
                "Include Archived Listings",
                showArchivedContracts);

            string search = questRunArchiveSearch?.Trim() ?? string.Empty;
            List<Tuple<DeverQuestQuestContract,
                DeverQuestContractCompletionRecord>> records =
                new List<Tuple<DeverQuestQuestContract,
                    DeverQuestContractCompletionRecord>>();
            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract == null ||
                    (!showArchivedContracts && contract.archived) ||
                    contract.completionHistory == null)
                {
                    continue;
                }

                foreach (DeverQuestContractCompletionRecord record
                         in contract.completionHistory)
                {
                    if (record == null)
                    {
                        continue;
                    }
                    string participantText = record.adventurerNames == null
                        ? string.Empty
                        : string.Join(", ", record.adventurerNames);
                    bool matches = string.IsNullOrWhiteSpace(search) ||
                        contract.contractTitle.IndexOf(
                            search,
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        participantText.IndexOf(
                            search,
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        (record.runId ?? string.Empty).IndexOf(
                            search,
                            StringComparison.OrdinalIgnoreCase) >= 0;
                    if (matches)
                    {
                        records.Add(Tuple.Create(contract, record));
                    }
                }
            }

            records = records
                .OrderByDescending(item =>
                    ParseRunTimestamp(item.Item2.completedUtc))
                .ToList();
            double focusedMinutes = records.Sum(item =>
                Math.Max(0d, item.Item2.focusedMinutes));
            long copper = records.Sum(item =>
                Math.Max(0L, item.Item2.awardedCopper));
            long experience = records.Sum(item =>
                Math.Max(0L, item.Item2.awardedExperience));
            EditorGUILayout.LabelField(
                "Summary",
                $"{records.Count} run(s) · " +
                $"{focusedMinutes / 60d:0.##} focused hour(s) · " +
                $"{DeverQuestAdventurerService.FormatCoins(copper)} · " +
                $"{experience} XP");

            int displayCount = Math.Min(records.Count, 50);
            for (int index = 0; index < displayCount; index++)
            {
                DeverQuestQuestContract contract = records[index].Item1;
                DeverQuestContractCompletionRecord record =
                    records[index].Item2;
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    contract.contractTitle,
                    FormatRunTimestamp(record.completedUtc));
                EditorGUILayout.LabelField(
                    "Completed By",
                    record.adventurerNames == null ||
                    record.adventurerNames.Count == 0
                        ? "Unknown Adventurer"
                        : string.Join(", ", record.adventurerNames));
                EditorGUILayout.LabelField(
                    "Quest Run",
                    ShortId(record.runId));
                EditorGUILayout.LabelField(
                    "Focused",
                    $"{Math.Max(0d, record.focusedMinutes):0.#} minutes");
                EditorGUILayout.LabelField(
                    "Rewards",
                    $"{DeverQuestAdventurerService.FormatCoins(record.awardedCopper)} " +
                    $"+ {record.awardedExperience} XP");
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select Contract"))
                {
                    Selection.activeObject = contract;
                    EditorGUIUtility.PingObject(contract);
                }
                if (GUILayout.Button("Copy Run ID"))
                {
                    EditorGUIUtility.systemCopyBuffer =
                        record.runId ?? string.Empty;
                }
                EditorGUILayout.EndHorizontal();

                bool canManageContract =
                    DeverQuestGuildAccountService.HasPermission(
                        DeverQuestGuildPermission.ManageContracts,
                        contract.projectName);
                if (canManageContract)
                {
                    EditorGUILayout.BeginHorizontal();
                    if (contract.archived &&
                        GUILayout.Button("Restore Listing"))
                    {
                        if (!DeverQuestContractService.SetArchived(
                                contract,
                                false,
                                out string restoreError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Restore Listing",
                                restoreError,
                                "Close");
                        }
                    }
                    if (!contract.archived &&
                        contract.status ==
                            DeverQuestContractStatus.Completed &&
                        contract.IsBoardComplete &&
                        GUILayout.Button("Restore to Offered"))
                    {
                        if (!DeverQuestContractService
                            .ReopenForAnotherRun(
                                contract,
                                out string reopenError))
                        {
                            EditorUtility.DisplayDialog(
                                "Cannot Restore Quest",
                                reopenError,
                                "Close");
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            if (records.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No matching completed Quest Runs were found.",
                    EditorStyles.miniLabel);
            }
            else if (records.Count > displayCount)
            {
                EditorGUILayout.LabelField(
                    $"Showing the newest {displayCount} of " +
                    $"{records.Count} matching runs.",
                    EditorStyles.miniLabel);
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private static string ShortId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "Legacy / unavailable";
            }
            return value.Length <= 12
                ? value
                : value.Substring(0, 12);
        }

        private static DateTime ParseRunTimestamp(string value)
        {
            return DateTime.TryParse(
                value,
                null,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out DateTime parsed)
                    ? parsed.ToUniversalTime()
                    : DateTime.MinValue;
        }

        private static string FormatRunTimestamp(string value)
        {
            DateTime parsed = ParseRunTimestamp(value);
            return parsed == DateTime.MinValue
                ? "Legacy / unavailable"
                : parsed.ToLocalTime().ToString("g");
        }

        private static string FormatRunAge(string value)
        {
            DateTime parsed = ParseRunTimestamp(value);
            if (parsed == DateTime.MinValue)
            {
                return "Unknown";
            }
            TimeSpan age = DateTime.UtcNow - parsed;
            if (age.TotalMinutes < 1d)
            {
                return "Less than one minute";
            }
            if (age.TotalHours < 1d)
            {
                return $"{Math.Floor(age.TotalMinutes)} minute(s)";
            }
            if (age.TotalDays < 1d)
            {
                return $"{age.TotalHours:0.#} hour(s)";
            }
            return $"{age.TotalDays:0.#} day(s)";
        }

        private static void DrawMeditationRecoveryStatus()
        {
            if (!DeverQuestSessionStore.GetMeditationRecoveryPreview(
                    out int completedMinutes,
                    out int hitPoints,
                    out int mana))
            {
                return;
            }

            string recovery = hitPoints <= 0 && mana <= 0
                ? "No additional Health or Mana is currently recoverable."
                : $"Recovery on Resume: +{hitPoints} HP · +{mana} Mana";

            EditorGUILayout.HelpBox(
                $"{recovery}\n" +
                $"{completedMinutes} full meditation minute(s) completed. " +
                $"Rate: {DeverQuestSessionStore.MeditationHitPointsPerMinute} " +
                "HP and " +
                $"{DeverQuestSessionStore.MeditationManaPerMinute} Mana " +
                "per full minute, capped at maximum values.",
                MessageType.Info);
        }

        private static string BuildContractAvailabilityLabel(
            DeverQuestQuestContract contract)
        {
            if (contract == null)
            {
                return "Unavailable";
            }
            if (contract.archived)
            {
                return "Archived";
            }

            switch (contract.availabilityPolicy)
            {
                case DeverQuestContractAvailabilityPolicy.Repeatable:
                    return contract.oneCompletionPerAdventurer
                        ? "Repeatable · once per Adventurer"
                        : "Repeatable · unlimited runs";
                case DeverQuestContractAvailabilityPolicy
                    .LimitedCompletions:
                    return $"{contract.CompletedRunCount}/" +
                           $"{contract.CompletionTarget} " +
                           "completed" +
                           (contract.oneCompletionPerAdventurer
                               ? " · unique Adventurers"
                               : string.Empty);
                default:
                    return contract.CompletedRunCount > 0
                        ? "One-time · completed"
                        : "One-time · available";
            }
        }

        private static void DrawQuestProgressSummary(
            DeverQuestSession session)
        {
            double focusedSeconds =
                DeverQuestSessionStore.GetFocusedSeconds();
            int stagedMinutes = session.questStages == null
                ? 0
                : session.questStages.Sum(stage =>
                    stage == null
                        ? 0
                        : Math.Max(0, stage.focusedMinutesRequired));
            int targetMinutes =
                session.questSuggestedFocusMinutes > 0
                    ? session.questSuggestedFocusMinutes
                    : stagedMinutes;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Quest Progress",
                EditorStyles.boldLabel);

            if (targetMinutes <= 0)
            {
                EditorGUILayout.LabelField(
                    "Time Remaining",
                    "No target duration configured");
                EditorGUILayout.HelpBox(
                    "Focused time is being recorded normally. Assign a " +
                    "suggested duration to the Quest Profile or Contract " +
                    "to enable pacing feedback.",
                    MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            double targetSeconds = targetMinutes * 60d;
            double remainingSeconds =
                targetSeconds - focusedSeconds;
            float progress = Mathf.Clamp01(
                (float)(focusedSeconds / targetSeconds));
            Rect progressRect = GUILayoutUtility.GetRect(
                18f,
                22f,
                GUILayout.ExpandWidth(true));
            EditorGUI.ProgressBar(
                progressRect,
                progress,
                $"{progress * 100f:0}% of {targetMinutes} minutes");

            EditorGUILayout.LabelField(
                remainingSeconds >= 0d
                    ? "Time Remaining"
                    : "Beyond Target",
                FormatDuration(Math.Abs(remainingSeconds)));

            int totalStages = session.questStages == null
                ? 0
                : session.questStages.Count(stage => stage != null);
            if (totalStages > 0)
            {
                int completedStages = session.questStages.Count(stage =>
                    stage != null && stage.completed);
                DeverQuestSessionStage currentStage =
                    DeverQuestSessionStore.CurrentQuestStage();
                string stageLabel = currentStage == null
                    ? $"{completedStages} of {totalStages} complete"
                    : $"{Math.Min(totalStages, completedStages + 1)} of " +
                      $"{totalStages}: {currentStage.stageTitle}";
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    stageLabel);
            }

            EditorGUILayout.LabelField(
                "Progress Report",
                BuildQuestProgressFeedback(
                    session,
                    progress,
                    remainingSeconds),
                EditorStyles.wordWrappedLabel);

            if (session.usesQuestProfile)
            {
                int blockMinutes =
                    Math.Max(1, session.questWorkBlockMinutes);
                int completedBlocks = (int)Math.Floor(
                    focusedSeconds / (blockMinutes * 60d));
                long projectedCopper =
                    session.questBaseCopper +
                    completedBlocks *
                    (long)session.questCopperPerWorkBlock;
                long projectedExperience =
                    session.questBaseExperience +
                    completedBlocks *
                    (long)session.questExperiencePerWorkBlock;
                EditorGUILayout.LabelField(
                    "Current Spoils Estimate",
                    DeverQuestAdventurerService.FormatCoins(
                        projectedCopper) +
                    $" + {projectedExperience} XP");
            }

            EditorGUILayout.EndVertical();
        }

        private static string BuildQuestProgressFeedback(
            DeverQuestSession session,
            float progress,
            double remainingSeconds)
        {
            if (session.state == DeverQuestSessionState.Paused)
            {
                return string.IsNullOrWhiteSpace(session.pauseReason)
                    ? "The Quest is paused. Focused time is not increasing."
                    : "The Quest is paused: " + session.pauseReason + ".";
            }

            DeverQuestSessionStage currentStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (currentStage != null &&
                (currentStage.survivalMode ||
                 DeverQuestEncounterService.IsSurvival(currentStage)))
            {
                return currentStage.survivalFightPaused
                    ? "The Survival Encounter is paused at a safe decision " +
                      "point."
                    : "The party is advancing through Survival wave " +
                      Math.Max(1, currentStage.survivalWave) + ".";
            }

            if (remainingSeconds < 0d)
            {
                return "The planned duration has passed, but the Quest is " +
                       "still recording valid focused work.";
            }
            if (progress >= 0.9f)
            {
                return "The Quest is in its final stretch.";
            }
            if (progress >= 0.5f)
            {
                return "More than half of the planned Quest time is " +
                       "complete.";
            }
            if (progress >= 0.25f)
            {
                return "The Quest is underway and focused progress is being " +
                       "recorded.";
            }
            return "The Quest has begun. Establish the current objective and " +
                   "build momentum.";
        }

        private void DrawActiveSession(bool includeQuestLog = true)
        {
            DeverQuestSession session =
                DeverQuestSessionStore.ActiveSession;
            AnnounceCompletedStages();

            bool isRunning =
                session.state == DeverQuestSessionState.Running;

            EditorGUILayout.LabelField(
                isRunning ? "QUEST IN PROGRESS" : "QUEST PAUSED",
                EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                FormatDuration(
                    DeverQuestSessionStore.GetFocusedSeconds()),
                timerStyle,
                GUILayout.Height(48f));

            EditorGUILayout.LabelField(
                $"Meditation: {FormatDuration(DeverQuestSessionStore.GetPausedSeconds())}",
                subtitleStyle,
                GUILayout.Height(22f));

            if (isRunning &&
                DeverQuestSettingsStore.Profile.idleDetectionEnabled)
            {
                string idleStatus = DeverQuestIdleMonitor.IsSupported
                    ? $"Input idle: {FormatShortDuration(DeverQuestIdleMonitor.CurrentIdleSeconds)}"
                    : "Idle detection is unavailable on this platform.";

                EditorGUILayout.LabelField(
                    idleStatus,
                    subtitleStyle,
                    GUILayout.Height(22f));
            }
            if (DeverQuestExternalActivityMonitor
                .HasRecentConfiguredActivity)
            {
                EditorGUILayout.LabelField(
                    "External craft active: " +
                    DeverQuestExternalActivityMonitor
                        .ActiveProviderName,
                    subtitleStyle,
                    GUILayout.Height(22f));
            }
            EditorGUILayout.EndVertical();

            if (!isRunning &&
                session.pauseReason.StartsWith(
                    "Approved Break:",
                    StringComparison.Ordinal))
            {
                double remainingBreakSeconds = Math.Max(
                    0d,
                    (session.approvedBreakUntilUtcTicks -
                     DateTime.UtcNow.Ticks) /
                    (double)TimeSpan.TicksPerSecond);
                int requiredBreakMinutes = (int)Math.Ceiling(
                    Math.Max(1, session.approvedBreakPlannedMinutes) *
                    0.8d);
                EditorGUILayout.HelpBox(
                    $"{session.pauseReason}\n" +
                    $"Planned: {session.approvedBreakPlannedMinutes}m · " +
                    $"Minimum for benefit: {requiredBreakMinutes}m · " +
                    $"Permit remaining: " +
                    $"{FormatDuration(remainingBreakSeconds)}",
                    MessageType.Info);
            }

            DrawQuestProgressSummary(session);

            EditorGUILayout.Space(10f);

            if (showFinalization)
            {
                DrawFinalizationPanel(session);
                return;
            }

            if (!session.idlePauseAcknowledged)
            {
                EditorGUILayout.HelpBox(
                    $"This quest was automatically paused because: " +
                    $"{session.pauseReason}.\n\nAcknowledge your return " +
                    "before the quest can resume.",
                    MessageType.Warning);
                if (GUILayout.Button(
                        "I Have Returned — Acknowledge Pause",
                        GUILayout.Height(38f)))
                {
                    DeverQuestSessionStore.AcknowledgeIdlePause();
                    Repaint();
                }
                return;
            }

            DrawReadOnlyValue("Project", session.projectName);
            DrawReadOnlyValue("Task", session.taskName);
            DrawReadOnlyValue("Department", session.category);
            if (session.usesQuestProfile)
            {
                DrawReadOnlyValue(
                    "Quest Profile",
                    session.questProfileName);
                DrawReadOnlyValue(
                    "Predicted Task Length",
                    $"{session.questSuggestedFocusMinutes} minutes");
            }
            if (session.usesQuestContract)
            {
                DrawReadOnlyValue(
                    "Quest Contract",
                    session.questContractTitle);
                if (!string.IsNullOrWhiteSpace(
                        session.questContractRunId))
                {
                    DrawReadOnlyValue(
                        "Quest Run",
                        session.questContractRunId);
                }
                DrawReadOnlyValue(
                    "Assignment",
                    $"{session.questContractAssignee} · " +
                    $"{session.questContractPriority} · Due " +
                    $"{(string.IsNullOrWhiteSpace(session.questContractDueDate) ? "Unscheduled" : session.questContractDueDate)}");
                if (!string.IsNullOrWhiteSpace(
                        session.questContractDeliverables))
                {
                    EditorGUILayout.LabelField(
                        "Deliverables",
                        session.questContractDeliverables,
                        wrappedLabelStyle);
                }
            }
            DrawReadOnlyValue(
                "Started",
                DeverQuestSessionStore
                    .GetLocalStartTime(session)
                    .ToString("g"));

            if (!string.IsNullOrWhiteSpace(session.questStory))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Quest Story",
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    session.questStory,
                    wrappedLabelStyle);
            }

            if (!string.IsNullOrWhiteSpace(session.goal))
            {
                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Task Objective",
                    EditorStyles.boldLabel);

                EditorGUILayout.LabelField(
                    session.goal,
                    wrappedLabelStyle);
            }

            DrawCompactQuestEventFeed(session);

            DeverQuestSessionStage activeSessionStage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (activeSessionStage != null)
            {
                EditorGUILayout.Space(8f);
                double stageElapsed = Math.Max(
                    0d,
                    DeverQuestSessionStore.GetFocusedSeconds() -
                    activeSessionStage.startedFocusedSeconds);
                EditorGUILayout.LabelField(
                    "Current Encounter",
                    $"{activeSessionStage.stageTitle} · " +
                    $"{stageElapsed / 60d:0.0}/" +
                    $"{activeSessionStage.focusedMinutesRequired}m");
                if (activeSessionStage.survivalMode ||
                    DeverQuestEncounterService.IsSurvival(
                        activeSessionStage))
                {
                    DrawSurvivalControls(activeSessionStage);
                }
                else
                {
                    using (new EditorGUI.DisabledScope(
                               !activeSessionStage.allowEarlyTurnIn))
                    {
                        if (GUILayout.Button(
                                "Report Development Objective Complete"))
                        {
                            DeverQuestSessionStore
                                .CompleteCurrentStageEarly(
                                    out string message);
                            EditorUtility.DisplayDialog(
                                "Encounter Pace",
                                message,
                                "Continue");
                        }
                    }
                }
            }
            DrawBattleResults(session);
            if (includeQuestLog)
            {
                DrawCommitJournal(session);
            }

            if (!isRunning &&
                !string.IsNullOrWhiteSpace(session.pauseReason))
            {
                EditorGUILayout.Space(4f);
                EditorGUILayout.HelpBox(
                    $"Meditating because: {session.pauseReason}",
                    session.pauseReason == "Idle Detection" ||
                    session.pauseReason == "Unity Project Lost Focus"
                        ? MessageType.Warning
                        : MessageType.Info);
                DrawMeditationRecoveryStatus();
            }

            EditorGUILayout.Space(12f);

            EditorGUILayout.BeginHorizontal();

            if (isRunning)
            {
                if (GUILayout.Button(
                        "Meditate",
                        GUILayout.Height(32f)))
                {
                    DeverQuestSessionStore.PauseSession();
                    Repaint();
                }
            }
            else if (GUILayout.Button(
                         "Resume Quest",
                         GUILayout.Height(32f)))
            {
                DeverQuestSessionStage currentStage =
                    DeverQuestSessionStore.CurrentQuestStage();
                if (currentStage?.survivalFightPaused == true)
                {
                    if (DeverQuestSessionStore.ContinueSurvival(
                            out string continueMessage))
                    {
                        DeverQuestSessionStore.ResumeSession();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog(
                            "Survival Expedition",
                            continueMessage,
                            "Close");
                    }
                }
                else
                {
                    DeverQuestSessionStore.ResumeSession();
                }
                Repaint();
            }

            if (GUILayout.Button(
                    "Complete Quest",
                    GUILayout.Height(32f)))
            {
                BeginFinalization(session);
            }

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Abandon Quest"))
            {
                bool discard = EditorUtility.DisplayDialog(
                    "Abandon Current Quest?",
                    "This removes the current quest without keeping its " +
                    "focused time. This cannot be undone.",
                    "Abandon",
                    "Keep Quest");

                if (discard)
                {
                    if (session.usesQuestContract)
                    {
                        DeverQuestContractService.AbandonRun(
                            session.questContractId,
                            session.questContractRunId,
                            DeverQuestAdventurerService.Adventurer
                                .characterName);
                    }
                    DeverQuestSessionStore.DiscardSession();
                    Repaint();
                }
            }
        }

        private void DrawCommitJournal(DeverQuestSession session)
        {
            EditorGUILayout.Space(14f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "Quest Log",
                EditorStyles.boldLabel);
            if (GUILayout.Button(
                    "Open Git",
                    GUILayout.Width(84f)))
            {
                activeWorkspace = DeverQuestWorkspace.Git;
            }
            EditorGUILayout.EndHorizontal();

            DrawExternalActivityAndMedia(session);

            EditorGUILayout.LabelField("Quest Log Entry");
            commitComment = DrawWrappedTextArea(
                commitComment,
                46f);

            EditorGUILayout.BeginHorizontal();
            if (gitStatus != null && gitStatus.IsRepository)
            {
                if (string.IsNullOrWhiteSpace(commitBranch))
                {
                    commitBranch = gitStatus.Branch;
                }
                if (string.IsNullOrWhiteSpace(commitHash))
                {
                    commitHash = gitStatus.ShortHash;
                }
            }
            commitBranch = EditorGUILayout.TextField(
                new GUIContent(
                    "Branch Context",
                    "Recorded with notes, but does not create a commit."),
                commitBranch);
            commitHash = EditorGUILayout.TextField(
                new GUIContent(
                    "Current HEAD",
                    "Attached only by Link Note to Current Commit."),
                commitHash);
            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(
                       string.IsNullOrWhiteSpace(commitComment)))
            {
                if (GUILayout.Button(
                        "Add Quest Log Note (No Git Commit)"))
                {
                    DeverQuestSessionStore.AddCommitEntry(
                        commitComment,
                        commitBranch,
                        string.Empty,
                        "Quest Log Note");

                    commitComment = string.Empty;
                    commitHash = string.Empty;
                    Repaint();
                }

                using (new EditorGUI.DisabledScope(
                           gitStatus == null ||
                           !gitStatus.IsRepository ||
                           string.IsNullOrWhiteSpace(
                               gitStatus.ShortHash)))
                {
                    if (GUILayout.Button(
                            "Link Note to Current Commit"))
                    {
                        DeverQuestSessionStore.AddCommitEntry(
                            commitComment,
                            gitStatus.Branch,
                            gitStatus.ShortHash,
                            "Linked Commit Note");

                        commitComment = string.Empty;
                        commitBranch = gitStatus.Branch;
                        commitHash = gitStatus.ShortHash;
                        Repaint();
                    }
                }
            }

            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No commit entries yet.",
                    EditorStyles.miniLabel);
                return;
            }

            for (int index = 0;
                 index < session.commitEntries.Count;
                 index++)
            {
                DeverQuestCommitEntry entry =
                    session.commitEntries[index];

                EditorGUILayout.BeginHorizontal(
                    EditorStyles.helpBox);

                string entryLabel =
                    $"{index + 1}. [{entry.entryType}] " +
                    $"{entry.comment} " +
                    $"(+{FormatDuration(entry.focusedSecondsAtEntry)})";

                EditorGUILayout.LabelField(
                    entryLabel,
                    wrappedLabelStyle);

                bool removeEntry = GUILayout.Button(
                        "Remove",
                        GUILayout.Width(64f));

                EditorGUILayout.EndHorizontal();

                if (removeEntry)
                {
                    DeverQuestSessionStore.RemoveCommitEntry(
                        entry.entryId);

                    GUIUtility.ExitGUI();
                }
            }
        }

        private void DrawExternalActivityAndMedia(
            DeverQuestSession session)
        {
            EditorGUILayout.Space(8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "External Craft and Voice Chronicle",
                EditorStyles.boldLabel);

            DeverQuestExternalActivityProfile profile =
                (DeverQuestExternalActivityProfile)
                EditorGUILayout.ObjectField(
                    "Activity Profile",
                    DeverQuestExternalActivityMonitor.Profile,
                    typeof(DeverQuestExternalActivityProfile),
                    false);
            if (profile !=
                DeverQuestExternalActivityMonitor.Profile)
            {
                DeverQuestExternalActivityMonitor.SetProfile(
                    profile);
            }

            string activityStatus =
                DeverQuestExternalActivityMonitor
                    .HasRecentConfiguredActivity
                    ? "Active: " +
                      DeverQuestExternalActivityMonitor
                          .ActiveProviderName
                    : DeverQuestExternalActivityMonitor.IsSupported
                        ? "No configured external tool is currently active."
                        : "Foreground external-tool detection is currently " +
                          "available on Windows only.";
            EditorGUILayout.HelpBox(
                activityStatus,
                DeverQuestExternalActivityMonitor
                    .HasRecentConfiguredActivity
                    ? MessageType.Info
                    : MessageType.None);

            EditorGUILayout.Space(4f);
            voiceMemoName = EditorGUILayout.TextField(
                "Memo Name",
                voiceMemoName);
            string[] devices =
                DeverQuestVoiceMemoService.Devices;
            if (devices.Length > 0)
            {
                selectedMicrophoneIndex = Mathf.Clamp(
                    selectedMicrophoneIndex,
                    0,
                    devices.Length - 1);
                selectedMicrophoneIndex =
                    EditorGUILayout.Popup(
                        "Microphone",
                        selectedMicrophoneIndex,
                        devices);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "No microphone detected. Existing media files can still " +
                    "be attached.",
                    MessageType.Info);
            }

            EditorGUILayout.BeginHorizontal();
            if (!DeverQuestVoiceMemoService.IsRecording)
            {
                using (new EditorGUI.DisabledScope(
                           devices.Length == 0))
                {
                    if (GUILayout.Button("Record Voice Memo"))
                    {
                        DeverQuestVoiceMemoService.Start(
                            devices[selectedMicrophoneIndex],
                            out mediaMessage);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(
                        $"Stop and Attach " +
                        $"({DeverQuestVoiceMemoService.RecordingSeconds:0}s)"))
                {
                    DeverQuestVoiceMemoService.StopAndAttach(
                        voiceMemoName,
                        out mediaMessage);
                }
                if (GUILayout.Button("Cancel Recording"))
                {
                    DeverQuestVoiceMemoService.CancelRecording();
                    mediaMessage =
                        "Voice memo recording cancelled.";
                }
            }

            if (GUILayout.Button("Attach Existing File…"))
            {
                string sourcePath = EditorUtility.OpenFilePanel(
                    "Attach Media to Current Quest",
                    string.Empty,
                    string.Empty);
                if (!string.IsNullOrWhiteSpace(sourcePath))
                {
                    DeverQuestVoiceMemoService.AttachExistingFile(
                        sourcePath,
                        out mediaMessage);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrWhiteSpace(mediaMessage))
            {
                EditorGUILayout.HelpBox(
                    mediaMessage,
                    mediaMessage.Contains("failed") ||
                    mediaMessage.Contains("could not") ||
                    mediaMessage.Contains("No microphone")
                        ? MessageType.Warning
                        : MessageType.Info);
            }

            if (session.mediaAttachments != null)
            {
                foreach (DeverQuestMediaAttachment attachment
                         in session.mediaAttachments.ToList())
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(
                        $"[{attachment.attachmentType}] " +
                        attachment.displayName,
                        wrappedLabelStyle);
                    if (GUILayout.Button(
                            "Reveal",
                            GUILayout.Width(58f)))
                    {
                        DeverQuestIdleMonitor
                            .BeginIntentionalExternalAction();
                        EditorUtility.RevealInFinder(
                            attachment.filePath);
                    }
                    if (GUILayout.Button(
                            "Unlink",
                            GUILayout.Width(58f)))
                    {
                        DeverQuestSessionStore
                            .RemoveMediaAttachment(
                                attachment.attachmentId);
                        GUIUtility.ExitGUI();
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawGitWorkspace()
        {
            EditorGUILayout.LabelField(
                "Git",
                EditorStyles.boldLabel);

            if (DeverQuestSessionStore.HasActiveSession)
            {
                DeverQuestSession activeSession =
                    DeverQuestSessionStore.ActiveSession;
                EditorGUILayout.HelpBox(
                    "Commits and pushes created here are recorded in the " +
                    "active Quest Log for " + activeSession.taskName + ".",
                    MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Git remains available without an active Quest. " +
                    "Repository operations performed now are not attached " +
                    "to focused-work time.",
                    MessageType.None);
            }

            DrawGitPanel();
            DrawRecentGitQuestEntries();
        }

        private void DrawRecentGitQuestEntries()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.HasActiveSession
                    ? DeverQuestSessionStore.ActiveSession
                    : DeverQuestSessionStore.LastCompletedSession;
            if (session?.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No recent Quest Log entries are available.",
                    MessageType.None);
                return;
            }

            List<DeverQuestCommitEntry> entries =
                session.commitEntries
                    .Where(entry =>
                        entry != null &&
                        (!string.IsNullOrWhiteSpace(entry.commitHash) ||
                         (entry.entryType ?? string.Empty)
                             .IndexOf(
                                 "Git",
                                 StringComparison.OrdinalIgnoreCase) >= 0))
                    .OrderByDescending(entry => entry.createdUtcTicks)
                    .Take(10)
                    .ToList();

            EditorGUILayout.LabelField(
                DeverQuestSessionStore.HasActiveSession
                    ? "Current Quest Git Activity"
                    : "Last Quest Git Activity",
                EditorStyles.boldLabel);
            if (entries.Count == 0)
            {
                EditorGUILayout.LabelField(
                    "No Git-linked entries were recorded.",
                    EditorStyles.miniLabel);
                return;
            }

            foreach (DeverQuestCommitEntry entry in entries)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(
                    string.IsNullOrWhiteSpace(entry.entryType)
                        ? "Git Activity"
                        : entry.entryType,
                    EditorStyles.boldLabel);
                EditorGUILayout.LabelField(
                    entry.comment,
                    wrappedLabelStyle);
                if (!string.IsNullOrWhiteSpace(entry.commitHash))
                {
                    EditorGUILayout.SelectableLabel(
                        entry.commitHash,
                        EditorStyles.textField,
                        GUILayout.Height(
                            EditorGUIUtility.singleLineHeight));
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawGitPanel()
        {
            if (DeverQuestGitMonitor.LatestStatus != null)
            {
                gitStatus = DeverQuestGitMonitor.LatestStatus;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(
                "Git Repository",
                EditorStyles.boldLabel);
            if (GUILayout.Button("Refresh", GUILayout.Width(72f)))
            {
                RefreshGitStatus();
            }
            EditorGUILayout.EndHorizontal();

            if (gitStatus == null || !gitStatus.GitAvailable)
            {
                EditorGUILayout.HelpBox(
                    "Git was not found. Install Git and ensure the git " +
                    "command is available to Unity.",
                    MessageType.Warning);
                EditorGUILayout.EndVertical();
                return;
            }

            if (!gitStatus.IsRepository)
            {
                EditorGUILayout.HelpBox(
                    "This Unity project is not inside a Git repository. " +
                    "Create or clone a repository before using Git actions.",
                    MessageType.Info);
                DrawReadOnlyValue(
                    "Unity Project",
                    gitStatus.UnityProjectRoot);
                if (GUILayout.Button("Choose Repository Folder…"))
                {
                    ChooseGitRepositoryFolder();
                }
                EditorGUILayout.EndVertical();
                return;
            }

            DrawReadOnlyValue(
                "Branch",
                string.IsNullOrWhiteSpace(gitStatus.Branch)
                    ? "Detached HEAD"
                    : gitStatus.Branch);
            DrawReadOnlyValue(
                "Current Commit",
                string.IsNullOrWhiteSpace(gitStatus.ShortHash)
                    ? "No commits yet"
                    : gitStatus.ShortHash);
            DrawReadOnlyValue(
                "Repository",
                gitStatus.RepositoryRoot);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Choose Different Repository…"))
            {
                ChooseGitRepositoryFolder();
            }
            if (!string.IsNullOrWhiteSpace(
                    DeverQuestSettingsStore.Profile
                        .gitRepositoryOverridePath) &&
                GUILayout.Button("Use Unity Project"))
            {
                DeverQuestSettingsStore.Profile
                    .gitRepositoryOverridePath = string.Empty;
                DeverQuestSettingsStore.Save();
                RefreshGitStatus();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField(
                $"Staged: {gitStatus.StagedCount} · " +
                $"Modified: {gitStatus.UnstagedCount} · " +
                $"Untracked: {gitStatus.UntrackedCount}");
            EditorGUILayout.LabelField(
                "Upstream",
                string.IsNullOrWhiteSpace(gitStatus.UpstreamBranch)
                    ? "Not configured"
                    : gitStatus.UpstreamBranch);
            if (!string.IsNullOrWhiteSpace(
                    gitStatus.UpstreamBranch))
            {
                EditorGUILayout.LabelField(
                    $"Ahead: {gitStatus.AheadCount} · " +
                    $"Behind: {gitStatus.BehindCount}");
            }

            EditorGUILayout.HelpBox(
                "Branch = your current development path. Staged files = " +
                "the changes selected for the next Git commit. Hash = the " +
                "unique ID Git assigns after committing. Staging is not " +
                "stashing: a stash temporarily shelves changes.",
                MessageType.Info);

            EditorGUILayout.LabelField(
                "Commit Message",
                EditorStyles.boldLabel);
            gitCommitMessage = DrawWrappedTextArea(
                gitCommitMessage,
                58f);

            using (new EditorGUI.DisabledScope(
                       gitOperationInProgress ||
                       !gitStatus.HasStagedChanges ||
                       string.IsNullOrWhiteSpace(gitCommitMessage)))
            {
                if (GUILayout.Button("Commit Staged Changes"))
                {
                    CommitWithGit(false);
                }
            }

            using (new EditorGUI.DisabledScope(
                       gitOperationInProgress ||
                       gitStatus.IsClean ||
                       string.IsNullOrWhiteSpace(gitCommitMessage)))
            {
                if (GUILayout.Button("Stage All and Commit…"))
                {
                    CommitWithGit(true);
                }
            }

            if (!string.IsNullOrWhiteSpace(
                    gitStatus.UpstreamBranch))
            {
                using (new EditorGUI.DisabledScope(
                           gitOperationInProgress ||
                           !gitStatus.IsClean ||
                           gitStatus.AheadCount <= 0 ||
                           gitStatus.BehindCount > 0))
                {
                    if (GUILayout.Button(
                            $"Push {gitStatus.AheadCount} Commit(s)…"))
                    {
                        PushWithGit(false);
                    }
                }

                if (gitStatus.BehindCount > 0)
                {
                    EditorGUILayout.HelpBox(
                        "The remote branch is ahead. DeverQuest will not " +
                        "push until you pull and resolve the repository " +
                        "through your normal Git client.",
                        MessageType.Warning);
                }
            }
            else
            {
                using (new EditorGUI.DisabledScope(
                           gitOperationInProgress ||
                           !gitStatus.IsClean ||
                           !gitStatus.HasOriginRemote ||
                           string.IsNullOrWhiteSpace(gitStatus.Branch)))
                {
                    if (GUILayout.Button(
                            "Publish Branch to origin…"))
                    {
                        PushWithGit(true);
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(gitMessage))
            {
                EditorGUILayout.HelpBox(
                    gitMessage,
                    gitMessage.StartsWith("Git commit created") ||
                    gitMessage.StartsWith("Git push completed") ||
                    gitMessage.StartsWith("Git branch published")
                        ? MessageType.Info
                        : MessageType.Warning);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(8f);
        }

        private async void CommitWithGit(bool stageAll)
        {
            if (gitStatus == null || !gitStatus.IsRepository)
            {
                gitMessage = "No Git repository is available.";
                return;
            }

            if (stageAll)
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Stage Every Project Change?",
                    "This stages every modified, deleted, and untracked " +
                    "file in the repository for one commit.\n\nReview your " +
                    "changes first if you are unsure.",
                    "Stage All and Commit",
                    "Cancel");

                if (!confirmed)
                {
                    return;
                }

            }

            string repositoryRoot = gitStatus.RepositoryRoot;
            string committedMessage = gitCommitMessage.Trim();
            gitOperationInProgress = true;
            gitMessage = stageAll
                ? "Staging project changes…"
                : "Creating Git commit…";
            Repaint();

            DeverQuestGitResult stageResult = null;
            DeverQuestGitResult commitResult = await Task.Run(() =>
            {
                if (stageAll)
                {
                    stageResult =
                        DeverQuestGitService.StageAll(repositoryRoot);
                    if (!stageResult.Succeeded)
                    {
                        return null;
                    }
                }

                return DeverQuestGitService.CommitStaged(
                    repositoryRoot,
                    committedMessage);
            });

            gitOperationInProgress = false;
            if (this == null)
            {
                return;
            }

            if (stageResult != null && !stageResult.Succeeded)
            {
                gitMessage =
                    $"Git could not stage the changes: " +
                    $"{stageResult.Error}";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            if (commitResult == null)
            {
                gitMessage = "Git commit was not started.";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            if (!commitResult.Succeeded)
            {
                gitMessage =
                    $"Git commit failed: {commitResult.Error}";
                RefreshGitStatus(false);
                Repaint();
                return;
            }

            RefreshGitStatus(false);
            DeverQuestGitMonitor.MarkObserved(gitStatus);
            DeverQuestSessionStore.AddCommitEntry(
                committedMessage,
                gitStatus.Branch,
                gitStatus.ShortHash,
                "Git Commit");
            commitBranch = gitStatus.Branch;
            commitHash = gitStatus.ShortHash;
            gitCommitMessage = string.Empty;
            gitMessage =
                $"Git commit created: {gitStatus.ShortHash}";
            Repaint();
        }

        private async void PushWithGit(bool publishBranch)
        {
            if (gitStatus == null || !gitStatus.IsRepository)
            {
                gitMessage = "No Git repository is available.";
                return;
            }

            string destination = publishBranch
                ? $"origin/{gitStatus.Branch}"
                : gitStatus.UpstreamBranch;
            bool confirmed = EditorUtility.DisplayDialog(
                publishBranch
                    ? "Publish Git Branch?"
                    : "Push Git Commits?",
                $"Send committed work to:\n\n{destination}\n\n" +
                "This does not include uncommitted files and will never " +
                "force-push.",
                publishBranch ? "Publish Branch" : "Push Commits",
                "Cancel");
            if (!confirmed)
            {
                return;
            }

            string repositoryRoot = gitStatus.RepositoryRoot;
            string branch = gitStatus.Branch;
            string hash = gitStatus.ShortHash;
            gitOperationInProgress = true;
            gitMessage = publishBranch
                ? $"Publishing {destination}…"
                : $"Pushing commits to {destination}…";
            Repaint();

            DeverQuestGitResult result = await Task.Run(() =>
                publishBranch
                    ? DeverQuestGitService.PublishBranch(
                        repositoryRoot,
                        branch)
                    : DeverQuestGitService.Push(repositoryRoot));

            gitOperationInProgress = false;
            if (this == null)
            {
                return;
            }

            if (!result.Succeeded)
            {
                gitMessage = $"Git push failed: {result.Error}";
                RefreshGitStatus(false);
                return;
            }

            RefreshGitStatus(false);
            DeverQuestGitMonitor.MarkObserved(gitStatus);
            DeverQuestSessionStore.AddCommitEntry(
                publishBranch
                    ? $"Published {branch} to origin"
                    : $"Pushed commits to {destination}",
                branch,
                hash,
                "Git Push");
            gitMessage = publishBranch
                ? $"Git branch published: {destination}"
                : $"Git push completed: {destination}";
            Repaint();
        }

        private void RefreshGitStatus(bool clearMessage = true)
        {
            gitStatus = DeverQuestGitService.Refresh();
            DeverQuestGitMonitor.SetLatestStatus(gitStatus);
            if (clearMessage)
            {
                gitMessage = string.Empty;
            }
        }

        private void ChooseGitRepositoryFolder()
        {
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            string startingPath =
                !string.IsNullOrWhiteSpace(
                    profile.gitRepositoryOverridePath)
                    ? profile.gitRepositoryOverridePath
                    : gitStatus?.UnityProjectRoot ??
                      Path.GetFullPath(
                          Path.Combine(Application.dataPath, ".."));

            string selected = EditorUtility.OpenFolderPanel(
                "Choose the Git Repository Containing This Project",
                startingPath,
                string.Empty);
            if (string.IsNullOrWhiteSpace(selected))
            {
                return;
            }

            profile.gitRepositoryOverridePath = selected;
            DeverQuestSettingsStore.Save();
            RefreshGitStatus();
        }

        private void BeginFinalization(
            DeverQuestSession session)
        {
            DeverQuestSessionStage stage =
                DeverQuestSessionStore.CurrentQuestStage();
            if (stage != null &&
                (stage.survivalMode ||
                 DeverQuestEncounterService.IsSurvival(stage)))
            {
                string exitMethod =
                    stage.survivalExitOffered
                        ? "Wagon"
                        : DeverQuestTacticalCombatService
                            .HasReturnAbility(
                                DeverQuestAdventurerService
                                    .Adventurer)
                        ? "Return"
                        : "Flee";
                if (!DeverQuestSessionStore.TryExitSurvival(
                        exitMethod,
                        out string exitMessage))
                {
                    EditorUtility.DisplayDialog(
                        "Cannot Turn In Survival Quest Yet",
                        exitMessage,
                        "Return to Expedition");
                    return;
                }
                ShowNotification(
                    new GUIContent(exitMessage), 5d);
            }
            if (!session.idlePauseAcknowledged)
            {
                DeverQuestSessionStore.AcknowledgeIdlePause();
            }

            showFinalization = true;
            turnInStep = QuestTurnInStep.Chronicle;
            closingNotes = string.Empty;
            Repaint();
        }

        private void DrawFinalizationPanel(
            DeverQuestSession session)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                $"Quest Turn-In · Step {(int)turnInStep + 1} of 2",
                EditorStyles.boldLabel);

            EditorGUILayout.HelpBox(
                session.state == DeverQuestSessionState.Running
                    ? "Focused time continues through Quest Log, Git, and " +
                      "closing notes. It stops only when you claim spoils."
                    : "This Quest was already meditating when turn-in began. " +
                      "Return to the Quest to resume focused time.",
                MessageType.Info);

            if (turnInStep == QuestTurnInStep.Chronicle)
            {
                EditorGUILayout.LabelField(
                    "Review Chronicle",
                    EditorStyles.boldLabel);
                DrawReadOnlyValue("Quest", session.taskName);
                if (session.usesQuestContract)
                {
                    DrawReadOnlyValue(
                        "Contract",
                        session.questContractTitle);
                    DrawReadOnlyValue(
                        "Deliverables",
                        session.questContractDeliverables);
                }
                DrawReadOnlyValue(
                    "Focused",
                    FormatDuration(
                        DeverQuestSessionStore.GetFocusedSeconds()));

                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField("Final Quest Log Entry");
                commitComment = DrawWrappedTextArea(
                    commitComment,
                    54f);
                DrawGitPanel();
                DrawQuestLogReview(session);

                EditorGUILayout.Space(6f);
                EditorGUILayout.LabelField(
                    "Closing Notes",
                    EditorStyles.boldLabel);
                closingNotes = DrawWrappedTextArea(
                    closingNotes,
                    72f);

                if (GUILayout.Button(
                        "Review Spoils",
                        GUILayout.Height(32f)))
                {
                    turnInStep = QuestTurnInStep.Rewards;
                }
            }
            else
            {
                DrawRewardPreview(session);
                EditorGUILayout.HelpBox(
                    "Claiming these spoils completes the Quest and writes " +
                    "today's Chronicle.",
                    MessageType.Info);

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Back to Chronicle"))
                {
                    turnInStep = QuestTurnInStep.Chronicle;
                }
                if (GUILayout.Button(
                        "Claim Spoils and Complete Quest",
                        GUILayout.Height(34f)))
                {
                    FinalizeSession();
                    GUIUtility.ExitGUI();
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button("Return to Quest"))
            {
                CancelTurnIn();
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawQuestLogReview(
            DeverQuestSession session)
        {
            EditorGUILayout.LabelField(
                "Review Quest Log",
                EditorStyles.boldLabel);
            if (session.commitEntries == null ||
                session.commitEntries.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No Quest Log entries were recorded.",
                    MessageType.Info);
                return;
            }

            foreach (DeverQuestCommitEntry entry
                     in session.commitEntries)
            {
                string gitLabel =
                    entry.entryType == "Git Commit"
                        ? $"Git Commit {entry.commitHash}"
                        : entry.entryType == "Git Push"
                            ? $"Git Push {entry.commitHash}"
                        : entry.entryType == "Linked Commit Note"
                            ? $"Linked to {entry.commitHash}"
                    : string.IsNullOrWhiteSpace(entry.entryType)
                        ? "Legacy Entry"
                        : entry.entryType;
                EditorGUILayout.LabelField(
                    gitLabel,
                    $"{entry.comment} · {entry.branch}",
                    wrappedLabelStyle);
            }
        }

        private static void DrawRewardPreview(
            DeverQuestSession session)
        {
            EditorGUILayout.LabelField(
                "Projected Rewards",
                EditorStyles.boldLabel);
            DeverQuestProfile profile =
                DeverQuestSettingsStore.Profile;
            if (!profile.rewardsEnabled)
            {
                EditorGUILayout.LabelField("Rewards are disabled.");
                return;
            }

            double totalWorkSeconds =
                session.usesQuestProfile
                    ? DeverQuestSessionStore.GetFocusedSeconds()
                    : DeverQuestRewardService.Wallet
                          .unrewardedWorkSeconds +
                      DeverQuestSessionStore.GetFocusedSeconds();
            int workBlockMinutes = session.usesQuestProfile
                ? Math.Max(1, session.questWorkBlockMinutes)
                : profile.rewardWorkBlockMinutes;
            int copperPerBlock = session.usesQuestProfile
                ? session.questCopperPerWorkBlock
                : profile.copperPerWorkBlock;
            int experiencePerBlock = session.usesQuestProfile
                ? session.questExperiencePerWorkBlock
                : profile.experiencePerWorkBlock;
            int baseCopper = session.usesQuestProfile
                ? session.questBaseCopper
                : profile.baseQuestCopper;
            int baseExperience = session.usesQuestProfile
                ? session.questBaseExperience
                : profile.baseQuestExperience;
            double blockSeconds =
                Math.Max(60d, workBlockMinutes * 60d);
            int blocks =
                (int)Math.Floor(totalWorkSeconds / blockSeconds);

            EditorGUILayout.LabelField(
                "Completed Work Blocks",
                blocks.ToString());
            long copper =
                baseCopper +
                blocks * (long)copperPerBlock;
            long experience =
                baseExperience +
                blocks * (long)experiencePerBlock;
            if (session.usesQuestProfile)
            {
                EditorGUILayout.LabelField(
                    "Quest Profile",
                    session.questProfileName);
            }
            EditorGUILayout.LabelField(
                "Projected Coin",
                $"+{DeverQuestAdventurerService.FormatCoins(copper)}");
            EditorGUILayout.LabelField(
                "Projected Experience",
                $"+{experience} XP");
            EditorGUILayout.HelpBox(
                "Daily Decree bonus: " +
                $"{DeverQuestAdventurerService.FormatCoins(profile.dailyCopperBonus)} " +
                $"+ {profile.dailyExperienceBonus} XP when the focused-work " +
                "decree is fulfilled.",
                MessageType.Info);
        }

        private void CancelTurnIn()
        {
            showFinalization = false;
            turnInStep = QuestTurnInStep.Chronicle;

            Repaint();
        }

        private void FinalizeSession()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.CompleteSession(
                    closingNotes);

            if (session == null)
            {
                return;
            }

            DeverQuestRewardService.ProcessCompletedSession(
                DeverQuestSettingsStore.Profile,
                session);

            if (session.usesQuestContract)
            {
                DeverQuestContractService.RecordSessionCompletion(
                    session,
                    DeverQuestAdventurerService.Adventurer
                        .characterName,
                    DeverQuestSettingsStore.Profile.developerName);
            }

            WriteTimecard(session);
            if (DeverQuestSettingsStore.Profile
                .notificationSoundsEnabled)
            {
                DeverQuestAudioDirector.PlayCue(
                    DeverQuestAudioCue.QuestComplete);
            }

            newProjectName = session.projectName;
            newCategory = session.category;
            newTaskName = string.Empty;
            newGoal = string.Empty;
            appliedQuestProfileId = string.Empty;
            selectedQuestContract = null;
            appliedQuestContractId = string.Empty;
            ApplySelectedQuestProfile();
            commitComment = string.Empty;
            commitHash = string.Empty;
            closingNotes = string.Empty;
            showFinalization = false;
            Repaint();
        }

        private static void WriteTimecard(
            DeverQuestSession session)
        {
            bool succeeded =
                DeverQuestTimecardWriter.TryWriteSession(
                    DeverQuestSettingsStore.Profile,
                    session,
                    out string timecardPath,
                    out string errorMessage);

            session.timecardWriteSucceeded = succeeded;
            session.timecardWriteAttempted = true;
            session.timecardPath = timecardPath;
            session.timecardWriteError = errorMessage;

            DeverQuestSessionStore.SaveCompletedSession(session);
            DeverQuestHistoryService.Refresh(
                DeverQuestSettingsStore.Profile);
        }

        private void DrawLastCompletedSession()
        {
            DeverQuestSession session =
                DeverQuestSessionStore.LastCompletedSession;

            if (session == null ||
                session.state != DeverQuestSessionState.Completed)
            {
                return;
            }

            EditorGUILayout.Space(18f);
            EditorGUILayout.LabelField(
                "Last Completed Session",
                EditorStyles.boldLabel);

            DrawReadOnlyValue("Project", session.projectName);
            DrawReadOnlyValue("Task", session.taskName);
            DrawReadOnlyValue(
                "Focused",
                FormatDuration(
                    session.accumulatedFocusedSeconds));

            DrawReadOnlyValue(
                "Completed",
                DeverQuestSessionStore
                    .GetLocalCompletionTime(session)
                    .ToString("g"));

            DrawReadOnlyValue(
                "Commits",
                (session.commitEntries?.Count ?? 0).ToString());

            if (!session.timecardWriteAttempted)
            {
                EditorGUILayout.HelpBox(
                    "This session was completed before timecard writing " +
                    "was installed.",
                    MessageType.Info);

                if (GUILayout.Button("Write Session to Timecard"))
                {
                    WriteTimecard(session);
                    Repaint();
                }
            }
            else if (session.timecardWriteSucceeded)
            {
                EditorGUILayout.HelpBox(
                    $"Timecard written:\n{session.timecardPath}",
                    MessageType.Info);

                if (GUILayout.Button("Reveal Timecard"))
                {
                    DeverQuestIdleMonitor
                        .BeginIntentionalExternalAction();
                    EditorUtility.RevealInFinder(
                        session.timecardPath);
                }
            }
            else
            {
                EditorGUILayout.HelpBox(
                    "Timecard write failed:\n" +
                    session.timecardWriteError,
                    MessageType.Error);

                if (GUILayout.Button("Retry Timecard Write"))
                {
                    WriteTimecard(session);
                    Repaint();
                }
            }
        }

        private void DrawVisualsWorkspace(
            DeverQuestProfile profile)
        {
            EditorGUILayout.LabelField(
                "Visuals",
                EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "These settings are local to this Unity Editor profile. " +
                "They change DeverQuest presentation without changing " +
                "Quest, Chronicle, reward, or Guild data.",
                MessageType.Info);

            EditorGUI.BeginChangeCheck();

            profile.theme =
                (DeverQuestTheme)EditorGUILayout.EnumPopup(
                    "Theme Preset",
                    profile.theme);

            if (profile.theme == DeverQuestTheme.Custom)
            {
                EditorGUILayout.Space(4f);
                EditorGUILayout.LabelField(
                    "Custom Colors",
                    EditorStyles.boldLabel);
                profile.customTitleColor =
                    EditorGUILayout.ColorField(
                        "Title",
                        profile.customTitleColor);
                profile.customTimerColor =
                    EditorGUILayout.ColorField(
                        "Timer",
                        profile.customTimerColor);
                profile.customAccentColor =
                    EditorGUILayout.ColorField(
                        "Accent",
                        profile.customAccentColor);
            }

            profile.interfaceScale =
                EditorGUILayout.Slider(
                    new GUIContent(
                        "DeverQuest Text Scale",
                        "Scales DeverQuest titles, timer text, and " +
                        "prominent labels without changing Unity's global " +
                        "Editor scale."),
                    profile.interfaceScale,
                    0.85f,
                    1.35f);
            profile.workspaceTabColumns =
                EditorGUILayout.IntSlider(
                    new GUIContent(
                        "Workspace Columns",
                        "Controls how many workspace buttons appear in " +
                        "each row."),
                    profile.workspaceTabColumns,
                    2,
                    6);
            profile.useCompactWorkspaceLabels =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Compact Workspace Labels",
                        "Uses shorter tab names for narrow dock layouts."),
                    profile.useCompactWorkspaceLabels);
            profile.showWorkspaceHints =
                EditorGUILayout.Toggle(
                    "Workspace Guidance",
                    profile.showWorkspaceHints);
            profile.showHeaderTagline =
                EditorGUILayout.Toggle(
                    "Header Tagline",
                    profile.showHeaderTagline);

            EditorGUILayout.Space(6f);
            EditorGUILayout.LabelField(
                "Quest HUD",
                EditorStyles.boldLabel);
            profile.autoOpenQuestHudOnSessionStart =
                EditorGUILayout.Toggle(
                    new GUIContent(
                        "Open HUD When Quest Starts",
                        "Opens the dockable Quest HUD when a new local " +
                        "Quest Session begins."),
                    profile.autoOpenQuestHudOnSessionStart);
            profile.questHudShowStory =
                EditorGUILayout.Toggle(
                    "Show Story in HUD",
                    profile.questHudShowStory);

            if (EditorGUI.EndChangeCheck())
            {
                profile.Sanitize();
                DeverQuestSettingsStore.Save();
                visualsMessage =
                    "Visual settings saved for this Unity Editor profile.";
                Repaint();
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(
                "Preview",
                titleStyle);
            EditorGUILayout.LabelField(
                "Developer Companion",
                subtitleStyle);
            EditorGUILayout.LabelField(
                "01:23:45",
                timerStyle);
            EditorGUILayout.LabelField(
                "Current Quest · Encounter 2 of 4",
                accentLabelStyle);
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Open Dockable Quest HUD"))
            {
                DeverQuestQuestHudWindow.Open();
            }
            if (GUILayout.Button("Reset Visual Settings"))
            {
                bool confirmed = EditorUtility.DisplayDialog(
                    "Reset Visual Settings?",
                    "Restore the Echo Neon theme, normal text scale, " +
                    "four workspace columns, and default HUD behavior?",
                    "Reset Visuals",
                    "Cancel");
                if (confirmed)
                {
                    ResetVisualSettings(profile);
                    visualsMessage =
                        "Visual settings restored to DeverQuest defaults.";
                    Repaint();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrWhiteSpace(visualsMessage))
            {
                EditorGUILayout.HelpBox(
                    visualsMessage,
                    MessageType.Info);
            }

            EditorGUILayout.Space(8f);
            EditorGUILayout.HelpBox(
                "Saved named Visual Profile assets, portrait frames, " +
                "high-contrast presets, and full per-panel color controls " +
                "remain later polish work. This build establishes the " +
                "persistent local presentation foundation.",
                MessageType.None);
        }

        private static void ResetVisualSettings(
            DeverQuestProfile profile)
        {
            if (profile == null)
            {
                return;
            }

            profile.theme = DeverQuestTheme.EchoNeon;
            profile.interfaceScale = 1f;
            profile.workspaceTabColumns = 4;
            profile.useCompactWorkspaceLabels = false;
            profile.showWorkspaceHints = true;
            profile.showHeaderTagline = true;
            profile.autoOpenQuestHudOnSessionStart = false;
            profile.questHudShowStory = true;
            profile.customTitleColor =
                new Color(0.20f, 0.94f, 0.86f, 1f);
            profile.customTimerColor =
                new Color(1f, 0.30f, 0.70f, 1f);
            profile.customAccentColor =
                new Color(0.55f, 0.82f, 1f, 1f);
            DeverQuestSettingsStore.Save();
        }

        private void DrawProfileControls(DeverQuestProfile profile)
        {
            EditorGUILayout.LabelField(
                "Profile",
                EditorStyles.boldLabel);

            bool canManageGuild =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageGuild);
            if (!canManageGuild)
            {
                EditorGUILayout.HelpBox(
                    "Guild settings are read-only for this account.",
                    MessageType.Info);
            }
            using (new EditorGUI.DisabledScope(!canManageGuild))
            {
            EditorGUI.BeginChangeCheck();
            DrawPolishSetup(profile);
            DrawChronicleIntegritySetup(profile);
            DrawExternalActivitySetup();
            if (EditorGUI.EndChangeCheck())
            {
                profile.Sanitize();
                DeverQuestSettingsStore.Save();
                DeverQuestGuildAccountService.AddAudit(
                    "Guild Settings Updated",
                    profile.developerName,
                    "Appearance and Chronicle settings changed.");
                Repaint();
            }

            string developerFolder =
                DeverQuestPathUtility.GetDeveloperFolder(
                    profile.timecardRootPath,
                    profile.developerName);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Reveal Timecard Folder"))
            {
                if (Directory.Exists(developerFolder))
                {
                    DeverQuestIdleMonitor
                        .BeginIntentionalExternalAction();
                    EditorUtility.RevealInFinder(developerFolder);
                }
                else
                {
                    EditorUtility.DisplayDialog(
                        "Folder Missing",
                        "The developer folder no longer exists. " +
                        "Reconfigure the profile to validate it again.",
                        "Close");
                }
            }

            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore.HasActiveSession))
            {
                if (GUILayout.Button("Reconfigure Profile"))
                {
                    DeverQuestGuildAccountService.AddAudit(
                        "Profile Reconfiguration",
                        profile.developerName,
                        "First-time setup was reopened.");
                    profile.setupComplete = false;
                    DeverQuestSettingsStore.Save();
                    Repaint();
                }
            }

            EditorGUILayout.EndHorizontal();

            using (new EditorGUI.DisabledScope(
                       DeverQuestSessionStore.HasActiveSession))
            {
                if (GUILayout.Button("Reset DeverQuest Profile"))
                {
                    bool confirmed = EditorUtility.DisplayDialog(
                        "Reset DeverQuest Profile?",
                        "This clears DeverQuest's saved profile settings. " +
                        "It will not delete folders or timecards.",
                        "Reset Profile",
                        "Cancel");

                    if (confirmed)
                    {
                        DeverQuestGuildAccountService.AddAudit(
                            "Profile Reset",
                            profile.developerName,
                            "Local DeverQuest settings were reset.");
                        DeverQuestSettingsStore.ResetProfile();
                        Repaint();
                    }
                }
            }
            }
        }

        private string DrawWrappedTextArea(
            string value,
            float minimumHeight)
        {
            if (wrappedTextAreaStyle == null)
            {
                wrappedTextAreaStyle =
                    new GUIStyle(EditorStyles.textArea)
                    {
                        wordWrap = true,
                        stretchWidth = true,
                        clipping = TextClipping.Clip
                    };
            }

            float availableWidth = Mathf.Max(
                180f,
                position.width - 44f);
            return EditorGUILayout.TextArea(
                value ?? string.Empty,
                wrappedTextAreaStyle,
                GUILayout.MinHeight(minimumHeight),
                GUILayout.MaxWidth(availableWidth),
                GUILayout.ExpandWidth(true));
        }

        private static void DrawReadOnlyValue(
            string label,
            string value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(label);
            EditorGUILayout.SelectableLabel(
                value,
                EditorStyles.textField,
                GUILayout.Height(
                    EditorGUIUtility.singleLineHeight));
            EditorGUILayout.EndHorizontal();
        }

        private void BuildStyles()
        {
            if (titleStyle == null)
            {
                titleStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 24,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (subtitleStyle == null)
            {
                subtitleStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 14,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (wrappedLabelStyle == null)
            {
                wrappedLabelStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    alignment = TextAnchor.MiddleCenter
                };
            }

            if (timerStyle == null)
            {
                timerStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    fontSize = 32,
                    alignment = TextAnchor.MiddleCenter,
                    fixedHeight = 44f
                };
            }

            if (accentLabelStyle == null)
            {
                accentLabelStyle = new GUIStyle(EditorStyles.boldLabel)
                {
                    alignment = TextAnchor.MiddleCenter,
                    wordWrap = true
                };
            }
        }

        private void ApplyThemeToStyles(DeverQuestProfile profile)
        {
            Color titleColor = EditorStyles.label.normal.textColor;
            Color timerColor = EditorStyles.label.normal.textColor;
            Color accentColor = EditorStyles.label.normal.textColor;

            switch (profile.theme)
            {
                case DeverQuestTheme.Dark:
                    titleColor = new Color(0.78f, 0.86f, 0.94f);
                    timerColor = new Color(0.55f, 0.82f, 1f);
                    accentColor = new Color(0.55f, 0.82f, 1f);
                    break;
                case DeverQuestTheme.Light:
                    titleColor = new Color(0.12f, 0.20f, 0.28f);
                    timerColor = new Color(0.05f, 0.42f, 0.56f);
                    accentColor = new Color(0.10f, 0.52f, 0.60f);
                    break;
                case DeverQuestTheme.EchoNeon:
                    titleColor = new Color(0.20f, 0.94f, 0.86f);
                    timerColor = new Color(1f, 0.30f, 0.70f);
                    accentColor = new Color(0.55f, 0.82f, 1f);
                    break;
                case DeverQuestTheme.Custom:
                    titleColor = profile.customTitleColor;
                    timerColor = profile.customTimerColor;
                    accentColor = profile.customAccentColor;
                    break;
            }

            float scale = Mathf.Clamp(
                profile.interfaceScale,
                0.85f,
                1.35f);
            titleStyle.fontSize =
                Mathf.RoundToInt(24f * scale);
            subtitleStyle.fontSize =
                Mathf.RoundToInt(14f * scale);
            timerStyle.fontSize =
                Mathf.RoundToInt(32f * scale);
            timerStyle.fixedHeight = 44f * scale;

            titleStyle.normal.textColor = titleColor;
            subtitleStyle.normal.textColor = titleColor;
            timerStyle.normal.textColor = timerColor;
            accentLabelStyle.normal.textColor = accentColor;
        }

        private void RepaintWhileSessionRuns()
        {
            if (!DeverQuestSessionStore.HasActiveSession ||
                (activeWorkspace != DeverQuestWorkspace.Quest &&
                 activeWorkspace !=
                 DeverQuestWorkspace.QuestLog &&
                 activeWorkspace != DeverQuestWorkspace.Tactics &&
                 activeWorkspace != DeverQuestWorkspace.Chronicle) ||
                EditorApplication.timeSinceStartup <
                nextSessionRepaintTime)
            {
                return;
            }
            nextSessionRepaintTime =
                EditorApplication.timeSinceStartup + 0.25d;
            Repaint();
        }

        private static string FormatDuration(double totalSeconds)
        {
            System.TimeSpan duration =
                System.TimeSpan.FromSeconds(
                    System.Math.Max(0d, totalSeconds));

            if (duration.TotalHours >= 100d)
            {
                return $"{(int)duration.TotalHours:000}:" +
                       $"{duration.Minutes:00}:" +
                       $"{duration.Seconds:00}";
            }

            return $"{(int)duration.TotalHours:00}:" +
                   $"{duration.Minutes:00}:" +
                   $"{duration.Seconds:00}";
        }

        private static string FormatShortDuration(double totalSeconds)
        {
            System.TimeSpan duration =
                System.TimeSpan.FromSeconds(
                    System.Math.Max(0d, totalSeconds));

            if (duration.TotalMinutes >= 1d)
            {
                return $"{(int)duration.TotalMinutes}m " +
                       $"{duration.Seconds}s";
            }

            return $"{duration.Seconds}s";
        }
    }
}

//----- DeverQuestWindow.cs END -----
