using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace EchoDevGames.DeverQuest
{
    internal enum DeverQuestGuildPermission
    {
        WorkInput = 0,
        ManageContracts = 1,
        ReviewCorrections = 2,
        ManageProject = 3,
        ManageGuild = 4,
        DeleteRecords = 5,
        DeleteProgram = 6
    }

    [Serializable]
    internal sealed class DeverQuestGuildAccount
    {
        public string accountId = string.Empty;
        public string developerName = string.Empty;
        public string characterName = string.Empty;
        public string guildName = string.Empty;
        public string characterClass = "Warrior";
        public string classId = string.Empty;
        public string ancestryName = string.Empty;
        public string ancestryId = string.Empty;
        public string deityName = "Agnostic";
        public string deityId = string.Empty;
        public DeverQuestAlignment alignment =
            DeverQuestAlignment.TrueNeutral;
        public string guildRank = "Member";
        public List<string> assignedProjects = new List<string>();
        public string passwordSalt = string.Empty;
        public string passwordHash = string.Empty;
        public int passwordIterations = 100000;
        public int level = 1;
        public long currentExperience;
        public long lifetimeExperience;
        public long copperBalance;
        public long platinumCoins;
        public long goldCoins;
        public long silverCoins;
        public long copperCoins;
        public long totalCopperEarned;
        public long totalCopperSpent;
        public int strength = 10;
        public int dexterity = 10;
        public int constitution = 10;
        public int intelligence = 10;
        public int wisdom = 10;
        public int charisma = 10;
        public int agility = 10;
        public int stamina = 10;
        public int luck = 10;
        public int hitDie = 8;
        public int maximumHitPoints = 8;
        public int currentHitPoints = 8;
        public int maximumMana;
        public int currentMana;
        public int hunger = 100;
        public int rest = 100;
        public int happiness = 100;
        public bool isFallen;
        public int defeats;
        public string homeDepartment = "Programming";
        public bool characterCreationComplete;
        public List<string> proficientSaves = new List<string>();
        public List<string> statusEffects = new List<string>();
        public List<string> equippedEquipmentIds = new List<string>();
        public List<string> knownSpellIds = new List<string>();
        public string activeCompanionInstanceId = string.Empty;
        public List<DeverQuestCompanionState> companions =
            new List<DeverQuestCompanionState>();
        public List<DeverQuestInventoryEntry> inventory =
            new List<DeverQuestInventoryEntry>();
        public bool compensationPreviewEnabled;
        public DeverQuestCompensationBasis compensationBasis =
            DeverQuestCompensationBasis.Hourly;
        public string compensationCurrencyCode = "USD";
        public double compensationHourlyRate;
        public double compensationAnnualSalary;
        public double compensationWeeklyHours = 40d;
        public bool compensationIncludeApprovedBreaks;
        public DeverQuestCompensationIntegrityPolicy
            compensationIntegrityPolicy =
                DeverQuestCompensationIntegrityPolicy
                    .VerifiedChroniclesOnly;
        public bool disabled;
    }

    [Serializable]
    internal sealed class DeverQuestGuildAccountCollection
    {
        public int dataVersion = 10;
        public List<DeverQuestGuildAccount> accounts =
            new List<DeverQuestGuildAccount>();
    }

    [Serializable]
    internal sealed class DeverQuestGuildAuditEntry
    {
        public string createdUtc = string.Empty;
        public string actorAccountId = string.Empty;
        public string actorName = string.Empty;
        public string action = string.Empty;
        public string target = string.Empty;
        public string detail = string.Empty;
    }

    [Serializable]
    internal sealed class DeverQuestGuildAuditLog
    {
        public List<DeverQuestGuildAuditEntry> entries =
            new List<DeverQuestGuildAuditEntry>();
    }

    [InitializeOnLoad]
    internal static class DeverQuestGuildAccountService
    {
        private const string AccountsKey =
            "EchoDevGames.DeverQuest.GuildAccounts.v1";
        private const string CurrentAccountKey =
            "EchoDevGames.DeverQuest.CurrentGuildAccount.v1";
        private const string AuditKey =
            "EchoDevGames.DeverQuest.GuildAudit.v1";
        private const string AuthenticatedKey =
            "EchoDevGames.DeverQuest.GuildAuthenticated.v1";

        private static DeverQuestGuildAccountCollection collection;
        private static DeverQuestGuildAuditLog audit;

        static DeverQuestGuildAccountService()
        {
            Load();
            EnsureLegacyFounder();
            RepairSoleAccountAuthorityAndOnboarding();
        }

        public static IReadOnlyList<DeverQuestGuildAccount> Accounts =>
            collection.accounts;

        public static IReadOnlyList<DeverQuestGuildAuditEntry> AuditEntries =>
            audit.entries;

        public static DeverQuestGuildAccount CurrentAccount
        {
            get
            {
                string id = EditorPrefs.GetString(
                    CurrentAccountKey, string.Empty);
                return collection.accounts.FirstOrDefault(
                    item => item.accountId == id);
            }
        }

        public static bool IsAuthenticated =>
            CurrentAccount != null &&
            !CurrentAccount.disabled &&
            (SessionState.GetBool(AuthenticatedKey, false) ||
             string.IsNullOrWhiteSpace(CurrentAccount.passwordHash));

        public static bool RequiresPasscodeSetup =>
            CurrentAccount != null &&
            string.IsNullOrWhiteSpace(CurrentAccount.passwordHash);

        public static bool NeedsCharacterCreation =>
            CurrentAccount != null &&
            (!CurrentAccount.characterCreationComplete ||
             string.IsNullOrWhiteSpace(
                 CurrentAccount.characterName));

        public static bool HasPermission(
            DeverQuestGuildPermission permission,
            string projectName = "")
        {
            if (!IsAuthenticated)
            {
                return false;
            }
            DeverQuestGuildAccount account = CurrentAccount;
            if (permission == DeverQuestGuildPermission.WorkInput)
            {
                return true;
            }
            if (account.guildRank == "CEO")
            {
                return true;
            }
            if (account.guildRank == "Boss")
            {
                return permission !=
                       DeverQuestGuildPermission.DeleteRecords &&
                       permission !=
                       DeverQuestGuildPermission.DeleteProgram;
            }
            if (account.guildRank != "Project Leader")
            {
                return false;
            }
            bool projectAssigned =
                account.assignedProjects.Any(
                    item => string.Equals(
                        item, projectName,
                        StringComparison.OrdinalIgnoreCase));
            return projectAssigned &&
                   (permission ==
                    DeverQuestGuildPermission.ManageContracts ||
                    permission ==
                    DeverQuestGuildPermission.ReviewCorrections ||
                    permission ==
                    DeverQuestGuildPermission.ManageProject);
        }

        public static bool Login(
            string developerName,
            string passcode,
            out string error)
        {
            error = string.Empty;
            DeverQuestGuildAccount account =
                collection.accounts.FirstOrDefault(
                    item => string.Equals(
                        item.developerName,
                        developerName?.Trim(),
                        StringComparison.OrdinalIgnoreCase));
            if (account == null || account.disabled)
            {
                error = "Guild account was not found or is disabled.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(account.passwordHash))
            {
                error = "This founding account must first be secured locally.";
                return false;
            }
            if (!VerifyPasscode(account, passcode))
            {
                error = "Passcode was incorrect.";
                AddAudit("Login Failed", account.developerName,
                    "Incorrect local passcode.");
                return false;
            }
            SelectAccount(account);
            SessionState.SetBool(AuthenticatedKey, true);
            AddAudit("Login", account.developerName,
                "Local Guild authentication succeeded.");
            return true;
        }

        public static void Logout()
        {
            SyncFromAdventurer();
            AddAudit("Logout",
                CurrentAccount?.developerName ?? string.Empty,
                "Local Guild session ended.");
            SessionState.SetBool(AuthenticatedKey, false);
        }

        public static bool SecureCurrentAccount(
            string passcode,
            out string error)
        {
            error = ValidatePasscode(passcode);
            if (!string.IsNullOrEmpty(error) || CurrentAccount == null)
            {
                return false;
            }
            SetPasscode(CurrentAccount, passcode);
            Save();
            SessionState.SetBool(AuthenticatedKey, true);
            AddAudit("Founder Secured", CurrentAccount.developerName,
                "The migrated founding account received a local passcode.");
            return true;
        }

        public static void RefreshUnsecuredFounderIdentity(
            string developerName)
        {
            DeverQuestGuildAccount account = CurrentAccount;
            developerName = developerName?.Trim() ?? string.Empty;
            if (account == null ||
                !string.IsNullOrWhiteSpace(account.passwordHash) ||
                string.IsNullOrWhiteSpace(developerName))
            {
                return;
            }
            account.developerName = developerName;
            Save();
        }

        public static bool CreateAccount(
            string developerName,
            string characterName,
            string characterClass,
            string rank,
            IEnumerable<string> projects,
            string temporaryPasscode,
            out string error)
        {
            error = string.Empty;
            if (!HasPermission(DeverQuestGuildPermission.ManageGuild))
            {
                error = "Only a Boss or CEO can create Guild accounts.";
                return false;
            }
            developerName = developerName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(developerName))
            {
                error = "Developer name is required.";
                return false;
            }
            if (collection.accounts.Any(item => string.Equals(
                    item.developerName, developerName,
                    StringComparison.OrdinalIgnoreCase)))
            {
                error = "That developer name already has an account.";
                return false;
            }
            error = ValidatePasscode(temporaryPasscode);
            if (!string.IsNullOrEmpty(error))
            {
                return false;
            }
            DeverQuestGuildAccount account =
                new DeverQuestGuildAccount
                {
                    accountId = Guid.NewGuid().ToString("N"),
                    developerName = developerName,
                    characterName = characterName?.Trim() ?? string.Empty,
                    guildName = DeverQuestAdventurerService.Adventurer.guildName,
                    characterClass = characterClass,
                    guildRank = rank,
                    characterCreationComplete =
                        !string.IsNullOrWhiteSpace(characterName),
                    assignedProjects = projects?
                        .Select(item => item.Trim())
                        .Where(item => !string.IsNullOrWhiteSpace(item))
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList() ?? new List<string>()
                };
            DeverQuestAdventurer foundation =
                new DeverQuestAdventurer
                {
                    characterClass = characterClass
                };
            DeverQuestClassDefinition classDefinition =
                DeverQuestIdentityCatalogService.FindClass(
                    string.Empty, characterClass);
            if (classDefinition != null)
            {
                DeverQuestIdentityCatalog catalog =
                    DeverQuestIdentityCatalogService.ActiveCatalog;
                DeverQuestIdentityCatalogService
                    .ApplyIdentityFoundation(
                        foundation,
                        catalog?.defaultAncestry,
                        classDefinition,
                        catalog?.defaultFaith,
                        DeverQuestAlignment.TrueNeutral,
                        true);
            }
            else
            {
                DeverQuestAdventurerService.ApplyClassFoundation(
                    foundation, characterClass, true);
            }
            account.classId = foundation.classId;
            account.ancestryName = foundation.ancestryName;
            account.ancestryId = foundation.ancestryId;
            account.deityName = foundation.deityName;
            account.deityId = foundation.deityId;
            account.alignment = foundation.alignment;
            CopyRules(foundation, account);
            SetPasscode(account, temporaryPasscode);
            collection.accounts.Add(account);
            Save();
            AddAudit("Account Created", account.developerName,
                $"{account.guildRank}; projects: " +
                string.Join(", ", account.assignedProjects));
            return true;
        }

        public static bool UpdateCompensationPolicy(
            string accountId,
            bool enabled,
            DeverQuestCompensationBasis basis,
            string currencyCode,
            double hourlyRate,
            double annualSalary,
            double weeklyHours,
            bool includeApprovedBreaks,
            DeverQuestCompensationIntegrityPolicy integrityPolicy,
            out string error)
        {
            error = string.Empty;
            if (!HasPermission(DeverQuestGuildPermission.ManageGuild))
            {
                error =
                    "Only a Boss or CEO can configure compensation previews.";
                return false;
            }
            DeverQuestGuildAccount target = FindAccount(accountId);
            if (target == null)
            {
                error = "The selected Guild account was not found.";
                return false;
            }
            if (hourlyRate < 0d || annualSalary < 0d)
            {
                error = "Compensation values cannot be negative.";
                return false;
            }
            if (weeklyHours <= 0d || weeklyHours > 168d)
            {
                error =
                    "Scheduled weekly hours must be greater than zero and " +
                    "no more than 168.";
                return false;
            }

            target.compensationPreviewEnabled = enabled;
            target.compensationBasis = basis;
            target.compensationCurrencyCode =
                DeverQuestCompensationService.NormalizeCurrencyCode(
                    currencyCode);
            target.compensationHourlyRate = hourlyRate;
            target.compensationAnnualSalary = annualSalary;
            target.compensationWeeklyHours = weeklyHours;
            target.compensationIncludeApprovedBreaks =
                includeApprovedBreaks;
            target.compensationIntegrityPolicy = integrityPolicy;
            Save();
            AddAudit(
                "Compensation Preview Policy",
                target.developerName,
                "Optional local planning policy updated. Rate omitted from " +
                "the audit log.");
            return true;
        }

        public static bool CompleteCharacterCreation(
            string characterName,
            DeverQuestAncestry ancestry,
            DeverQuestClassDefinition classDefinition,
            DeverQuestDeity faith,
            DeverQuestAlignment alignment,
            out string error)
        {
            error = string.Empty;
            DeverQuestGuildAccount account = CurrentAccount;
            if (!IsAuthenticated || account == null)
            {
                error = "Authenticate before creating an Adventurer.";
                return false;
            }
            if (account.characterCreationComplete &&
                !string.IsNullOrWhiteSpace(account.characterName))
            {
                error = "This Adventurer has already been created.";
                return false;
            }
            characterName = characterName?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(characterName))
            {
                error = "Adventurer name is required.";
                return false;
            }
            if (!DeverQuestIdentityCatalogService.IsEligible(
                    ancestry, classDefinition, out error))
            {
                return false;
            }
            if (!DeverQuestIdentityCatalogService.IsFaithEligible(
                    faith,
                    ancestry,
                    classDefinition,
                    alignment,
                    out error))
            {
                return false;
            }
            account.characterName = characterName;
            DeverQuestAdventurer foundation =
                new DeverQuestAdventurer
                {
                    characterName = characterName,
                    guildName = account.guildName,
                    guildRank = account.guildRank,
                    level = Math.Max(1, account.level)
                };
            DeverQuestIdentityCatalogService.ApplyIdentityFoundation(
                foundation,
                ancestry,
                classDefinition,
                faith,
                alignment,
                true);
            ApplyStarterLoadout(
                foundation, classDefinition.displayName);
            DeverQuestCompanionService.GrantStarter(
                foundation,
                classDefinition.starterCompanion);
            account.characterClass = foundation.characterClass;
            account.classId = foundation.classId;
            account.ancestryName = foundation.ancestryName;
            account.ancestryId = foundation.ancestryId;
            account.deityName = foundation.deityName;
            account.deityId = foundation.deityId;
            account.alignment = foundation.alignment;
            account.homeDepartment = foundation.homeDepartment;
            CopyRules(foundation, account);

            // A new Adventurer begins with five silver. Existing Beta
            // progression is preserved and only topped up when below the
            // starting purse.
            account.copperBalance = Math.Max(500L, account.copperBalance);
            NormalizeAccountCoinPurse(account);
            account.characterCreationComplete = true;
            Save();
            SelectAccount(account);
            AddAudit("Adventurer Created", characterName,
                $"{account.ancestryName} · {account.characterClass} · " +
                $"{account.alignment} · {account.deityName} · " +
                $"{account.homeDepartment}");
            return true;
        }

        public static bool ReopenCurrentCharacterCreation(
            out string error)
        {
            error = string.Empty;
            DeverQuestGuildAccount account = CurrentAccount;
            if (account == null || !IsAuthenticated)
            {
                error = "Authenticate before editing an Adventurer.";
                return false;
            }
            if (!HasPermission(DeverQuestGuildPermission.ManageGuild))
            {
                error =
                    "Only a Boss or CEO can reopen character creation.";
                return false;
            }
            if (DeverQuestSessionStore.HasActiveSession)
            {
                error =
                    "Complete or abandon the active Quest before changing " +
                    "the Adventurer identity.";
                return false;
            }

            account.characterCreationComplete = false;
            Save();
            AddAudit(
                "Character Creation Reopened",
                account.developerName,
                "Identity may be rebuilt; level, XP, coin, inventory, and " +
                "ledger history remain attached to the Guild account.");
            return true;
        }

        private static void ApplyStarterLoadout(
            DeverQuestAdventurer target,
            string characterClass)
        {
            foreach (string guid in AssetDatabase.FindAssets(
                         "t:DeverQuestStarterLoadout"))
            {
                DeverQuestStarterLoadout loadout =
                    AssetDatabase.LoadAssetAtPath<DeverQuestStarterLoadout>(
                        AssetDatabase.GUIDToAssetPath(guid));
                if (loadout == null ||
                    !string.Equals(
                        loadout.characterClass,
                        characterClass,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                foreach (DeverQuestEquipment item in loadout.equipment)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    bool alreadyCarried =
                        (target.inventory ??
                         new List<DeverQuestInventoryEntry>())
                        .Any(value =>
                            value != null &&
                            value.equipmentId == item.EquipmentId &&
                            value.quantity > 0);
                    if (!alreadyCarried)
                    {
                        DeverQuestInventoryService.AddEquipmentAsset(
                            target.inventory,
                            item,
                            CurrentAccount?.accountId ?? string.Empty,
                            DeverQuestItemOriginKind.StarterLoadout,
                            "Starter Loadout");
                    }
                    DeverQuestRulesService.Equip(target, item);
                }
                foreach (DeverQuestSpell spell in loadout.spells)
                {
                    if (spell != null &&
                        !target.knownSpellIds.Contains(spell.SpellId))
                    {
                        target.knownSpellIds.Add(spell.SpellId);
                    }
                }
                break;
            }
        }

        public static void SyncFromAdventurer()
        {
            DeverQuestGuildAccount account = CurrentAccount;
            if (account == null)
            {
                return;
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            account.characterName = adventurer.characterName;
            account.guildName = adventurer.guildName;
            account.characterClass = adventurer.characterClass;
            account.classId = adventurer.classId;
            account.ancestryName = adventurer.ancestryName;
            account.ancestryId = adventurer.ancestryId;
            account.deityName = adventurer.deityName;
            account.deityId = adventurer.deityId;
            account.alignment = adventurer.alignment;
            // Guild authority belongs to the account, not the RPG
            // character sheet. Never allow a legacy Member value on the
            // Adventurer to demote the only CEO during progression sync.
            adventurer.guildRank = account.guildRank;
            account.level = adventurer.level;
            account.currentExperience = adventurer.currentExperience;
            account.lifetimeExperience = adventurer.lifetimeExperience;
            account.copperBalance = adventurer.copperBalance;
            account.platinumCoins = adventurer.platinumCoins;
            account.goldCoins = adventurer.goldCoins;
            account.silverCoins = adventurer.silverCoins;
            account.copperCoins = adventurer.copperCoins;
            account.totalCopperEarned = adventurer.totalCopperEarned;
            account.totalCopperSpent = adventurer.totalCopperSpent;
            CopyRules(adventurer, account);
            Save();
        }

        public static void MigrateIdentityCatalogs()
        {
            foreach (DeverQuestGuildAccount account
                     in collection.accounts)
            {
                DeverQuestIdentityCatalogService.Migrate(account);
            }
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestIdentityCatalogService.Migrate(adventurer);
            Save();
            DeverQuestAdventurerService.Save();
            if (CurrentAccount != null)
            {
                SelectAccount(CurrentAccount);
            }
        }

        public static void AddAudit(
            string action,
            string target,
            string detail)
        {
            DeverQuestGuildAccount actor = CurrentAccount;
            audit.entries.Insert(0, new DeverQuestGuildAuditEntry
            {
                createdUtc = DateTime.UtcNow.ToString("O"),
                actorAccountId = actor?.accountId ?? string.Empty,
                actorName = actor?.developerName ?? "System",
                action = action ?? string.Empty,
                target = target ?? string.Empty,
                detail = detail ?? string.Empty
            });
            if (audit.entries.Count > 500)
            {
                audit.entries.RemoveRange(500, audit.entries.Count - 500);
            }
            EditorPrefs.SetString(AuditKey, JsonUtility.ToJson(audit));
        }

        private static void EnsureLegacyFounder()
        {
            if (collection.accounts.Count > 0)
            {
                return;
            }
            DeverQuestProfile profile = DeverQuestSettingsStore.Profile;
            DeverQuestAdventurer old =
                DeverQuestAdventurerService.Adventurer;
            DeverQuestGuildAccount founder =
                new DeverQuestGuildAccount
                {
                    accountId = Guid.NewGuid().ToString("N"),
                    developerName =
                        string.IsNullOrWhiteSpace(profile.developerName)
                            ? "Founder"
                            : profile.developerName,
                    characterName = old.characterName,
                    guildName = old.guildName,
                    characterClass = old.characterClass,
                    classId = old.classId,
                    ancestryName = old.ancestryName,
                    ancestryId = old.ancestryId,
                    deityName = old.deityName,
                    deityId = old.deityId,
                    alignment = old.alignment,
                    guildRank = "CEO",
                    characterCreationComplete =
                        !string.IsNullOrWhiteSpace(old.characterName),
                    level = old.level,
                    currentExperience = old.currentExperience,
                    lifetimeExperience = old.lifetimeExperience,
                    copperBalance = old.copperBalance,
                    platinumCoins = old.platinumCoins,
                    goldCoins = old.goldCoins,
                    silverCoins = old.silverCoins,
                    copperCoins = old.copperCoins,
                    totalCopperEarned = old.totalCopperEarned,
                    totalCopperSpent = old.totalCopperSpent
                };
            CopyRules(old, founder);
            collection.accounts.Add(founder);
            EditorPrefs.SetString(CurrentAccountKey, founder.accountId);
            Save();
            SelectAccount(founder);
            AddAudit("Legacy Migration", founder.developerName,
                "Existing Adventurer migrated as the founding CEO.");
        }

        private static void RepairSoleAccountAuthorityAndOnboarding()
        {
            List<DeverQuestGuildAccount> activeAccounts =
                collection.accounts
                    .Where(account => account != null && !account.disabled)
                    .ToList();
            if (activeAccounts.Count != 1)
            {
                return;
            }

            DeverQuestGuildAccount founder = activeAccounts[0];
            bool authorityRepaired =
                !string.Equals(
                    founder.guildRank,
                    "CEO",
                    StringComparison.OrdinalIgnoreCase);
            bool onboardingRepaired =
                string.IsNullOrWhiteSpace(founder.characterName) &&
                founder.characterCreationComplete;

            if (authorityRepaired)
            {
                founder.guildRank = "CEO";
            }
            if (onboardingRepaired)
            {
                founder.characterCreationComplete = false;
            }

            string currentId = EditorPrefs.GetString(
                CurrentAccountKey, string.Empty);
            bool selectionRepaired = currentId != founder.accountId;
            if (selectionRepaired)
            {
                EditorPrefs.SetString(
                    CurrentAccountKey, founder.accountId);
            }

            if (!authorityRepaired &&
                !onboardingRepaired &&
                !selectionRepaired)
            {
                return;
            }

            Save();
            SelectAccount(founder);
            AddAudit(
                "Sole Founder Repaired",
                founder.developerName,
                "The only active Guild account was restored as CEO and " +
                "prepared for character onboarding when necessary.");
        }

        private static void NormalizeAccountCoinPurse(
            DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return;
            }

            long total = Math.Max(0L, account.copperBalance);
            account.platinumCoins = total / 1000000L;
            total %= 1000000L;
            account.goldCoins = total / 10000L;
            total %= 10000L;
            account.silverCoins = total / 100L;
            account.copperCoins = total % 100L;
        }

        private static void SelectAccount(DeverQuestGuildAccount account)
        {
            DeverQuestIdentityCatalogService.Migrate(account);
            Save();
            EditorPrefs.SetString(CurrentAccountKey, account.accountId);
            DeverQuestSettingsStore.Profile.developerName =
                account.developerName;
            DeverQuestSettingsStore.Save();
            DeverQuestAdventurer adventurer =
                DeverQuestAdventurerService.Adventurer;
            adventurer.characterName = account.characterName;
            adventurer.guildName = account.guildName;
            adventurer.characterClass = account.characterClass;
            adventurer.classId = account.classId;
            adventurer.ancestryName = account.ancestryName;
            adventurer.ancestryId = account.ancestryId;
            adventurer.deityName = account.deityName;
            adventurer.deityId = account.deityId;
            adventurer.alignment = account.alignment;
            adventurer.guildRank = account.guildRank;
            adventurer.level = account.level;
            adventurer.currentExperience = account.currentExperience;
            adventurer.lifetimeExperience = account.lifetimeExperience;
            adventurer.copperBalance = account.copperBalance;
            adventurer.platinumCoins = account.platinumCoins;
            adventurer.goldCoins = account.goldCoins;
            adventurer.silverCoins = account.silverCoins;
            adventurer.copperCoins = account.copperCoins;
            adventurer.totalCopperEarned = account.totalCopperEarned;
            adventurer.totalCopperSpent = account.totalCopperSpent;
            CopyRules(account, adventurer);
            DeverQuestAdventurerService.Save();
        }

        private static void CopyRules(
            DeverQuestAdventurer source,
            DeverQuestGuildAccount target)
        {
            target.strength = source.strength;
            target.dexterity = source.dexterity;
            target.constitution = source.constitution;
            target.intelligence = source.intelligence;
            target.wisdom = source.wisdom;
            target.charisma = source.charisma;
            target.agility = source.agility;
            target.stamina = source.stamina;
            target.luck = source.luck;
            target.hitDie = source.hitDie;
            target.maximumHitPoints = source.maximumHitPoints;
            target.currentHitPoints = source.currentHitPoints;
            target.maximumMana = source.maximumMana;
            target.currentMana = source.currentMana;
            target.hunger = source.hunger;
            target.rest = source.rest;
            target.happiness = source.happiness;
            target.isFallen = source.isFallen;
            target.defeats = source.defeats;
            target.homeDepartment = source.homeDepartment;
            target.proficientSaves =
                new List<string>(source.proficientSaves);
            target.statusEffects =
                new List<string>(source.statusEffects);
            target.equippedEquipmentIds =
                new List<string>(source.equippedEquipmentIds);
            target.knownSpellIds =
                new List<string>(source.knownSpellIds);
            target.activeCompanionInstanceId =
                source.activeCompanionInstanceId;
            target.companions =
                (source.companions ??
                 new List<DeverQuestCompanionState>())
                .Select(CloneCompanionState)
                .ToList();
            target.inventory = source.inventory
                .Select(CloneInventoryEntry).ToList();
            foreach (DeverQuestInventoryEntry entry
                     in target.inventory)
            {
                entry.EnsureOwnership(target.accountId);
            }
        }

        private static void CopyRules(
            DeverQuestGuildAccount source,
            DeverQuestAdventurer target)
        {
            target.strength = source.strength;
            target.dexterity = source.dexterity;
            target.constitution = source.constitution;
            target.intelligence = source.intelligence;
            target.wisdom = source.wisdom;
            target.charisma = source.charisma;
            target.agility = source.agility;
            target.stamina = source.stamina;
            target.luck = source.luck;
            target.hitDie = source.hitDie;
            target.maximumHitPoints = source.maximumHitPoints;
            target.currentHitPoints = source.currentHitPoints;
            target.maximumMana = source.maximumMana;
            target.currentMana = source.currentMana;
            target.hunger = source.hunger;
            target.rest = source.rest;
            target.happiness = source.happiness;
            target.isFallen = source.isFallen;
            target.defeats = source.defeats;
            target.homeDepartment = source.homeDepartment;
            target.proficientSaves =
                new List<string>(source.proficientSaves ??
                                 new List<string>());
            target.statusEffects =
                new List<string>(source.statusEffects ??
                                 new List<string>());
            target.equippedEquipmentIds =
                new List<string>(source.equippedEquipmentIds ??
                                 new List<string>());
            target.knownSpellIds =
                new List<string>(source.knownSpellIds ??
                                 new List<string>());
            target.activeCompanionInstanceId =
                source.activeCompanionInstanceId ?? string.Empty;
            target.companions =
                (source.companions ??
                 new List<DeverQuestCompanionState>())
                .Select(CloneCompanionState)
                .ToList();
            target.inventory =
                (source.inventory ??
                 new List<DeverQuestInventoryEntry>())
                .Select(CloneInventoryEntry).ToList();
            foreach (DeverQuestInventoryEntry entry
                     in target.inventory)
            {
                entry.EnsureOwnership(source.accountId);
            }
        }

        private static DeverQuestInventoryEntry CloneInventoryEntry(
            DeverQuestInventoryEntry source)
        {
            return new DeverQuestInventoryEntry
            {
                shopItemId = source?.shopItemId ?? string.Empty,
                displayName = source?.displayName ?? string.Empty,
                itemType = source == null
                    ? DeverQuestShopItemType.Consumable
                    : source.itemType,
                itemCategory = source == null
                    ? DeverQuestItemCategory.Unknown
                    : source.itemCategory,
                subcategory =
                    source?.subcategory ?? string.Empty,
                tags = source == null
                    ? new List<string>()
                    : new List<string>(
                        source.tags ?? new List<string>()),
                quantity = Math.Max(0, source?.quantity ?? 0),
                ownershipId =
                    source?.ownershipId ?? string.Empty,
                rarity = source == null
                    ? DeverQuestItemRarity.Common
                    : source.rarity,
                binding = source == null
                    ? DeverQuestItemBinding.Unbound
                    : source.binding,
                boundAccountId =
                    source?.boundAccountId ?? string.Empty,
                tradable = source == null || source.tradable,
                droppable = source == null || source.droppable,
                questProtected =
                    source != null && source.questProtected,
                acquiredUtc =
                    source?.acquiredUtc ?? string.Empty,
                acquisitionSource =
                    source?.acquisitionSource ?? string.Empty,
                originKind = source == null
                    ? DeverQuestItemOriginKind.Unknown
                    : source.originKind,
                originSource =
                    source?.originSource ?? string.Empty,
                originAcquiredUtc =
                    source?.originAcquiredUtc ?? string.Empty,
                sourceContractId =
                    source?.sourceContractId ?? string.Empty,
                sourceRunId =
                    source?.sourceRunId ?? string.Empty,
                sourceEncounterId =
                    source?.sourceEncounterId ?? string.Empty,
                sourceMonsterId =
                    source?.sourceMonsterId ?? string.Empty,
                sourceMonsterName =
                    source?.sourceMonsterName ?? string.Empty,
                equipmentId =
                    source?.equipmentId ?? string.Empty,
                unitValueCopper =
                    Math.Max(0, source?.unitValueCopper ?? 0),
                unitWeight = Math.Max(
                    0f, source?.unitWeight ?? 0.25f)
            };
        }

        private static DeverQuestCompanionState CloneCompanionState(
            DeverQuestCompanionState source)
        {
            DeverQuestCompanionState clone =
                new DeverQuestCompanionState
                {
                    instanceId =
                        source?.instanceId ?? string.Empty,
                    profileId =
                        source?.profileId ?? string.Empty,
                    customName =
                        source?.customName ?? string.Empty,
                    level = Math.Max(1, source?.level ?? 1),
                    currentExperience =
                        Math.Max(
                            0L,
                            source?.currentExperience ?? 0L),
                    lifetimeExperience =
                        Math.Max(
                            0L,
                            source?.lifetimeExperience ?? 0L),
                    currentHitPoints =
                        Math.Max(
                            0,
                            source?.currentHitPoints ?? 0),
                    loyalty = Math.Min(
                        100,
                        Math.Max(0, source?.loyalty ?? 50)),
                    isActive = source?.isActive ?? false,
                    isFallen = source?.isFallen ?? false,
                    battles = Math.Max(
                        0,
                        source?.battles ?? 0),
                    victories = Math.Max(
                        0,
                        source?.victories ?? 0),
                    lifetimeDamageDealt = Math.Max(
                        0L,
                        source?.lifetimeDamageDealt ?? 0L),
                    lifetimeDamageTaken = Math.Max(
                        0L,
                        source?.lifetimeDamageTaken ?? 0L),
                    lifetimeHealingDone = Math.Max(
                        0L,
                        source?.lifetimeHealingDone ?? 0L),
                    lastBattleSummary =
                        source?.lastBattleSummary ?? string.Empty,
                    lastBattleUtc =
                        source?.lastBattleUtc ?? string.Empty,
                    recruitedUtc =
                        source?.recruitedUtc ?? string.Empty
                };
            clone.Sanitize();
            return clone;
        }

        private static void SetPasscode(
            DeverQuestGuildAccount account,
            string passcode)
        {
            byte[] salt = new byte[16];
            using (RandomNumberGenerator generator =
                   RandomNumberGenerator.Create())
            {
                generator.GetBytes(salt);
            }
            account.passwordSalt = Convert.ToBase64String(salt);
            account.passwordHash = Convert.ToBase64String(
                Derive(passcode, salt, account.passwordIterations));
        }

        private static bool VerifyPasscode(
            DeverQuestGuildAccount account,
            string passcode)
        {
            byte[] expected = Convert.FromBase64String(
                account.passwordHash);
            byte[] actual = Derive(
                passcode,
                Convert.FromBase64String(account.passwordSalt),
                account.passwordIterations);
            int difference = expected.Length ^ actual.Length;
            for (int index = 0;
                 index < Math.Min(expected.Length, actual.Length);
                 index++)
            {
                difference |= expected[index] ^ actual[index];
            }
            return difference == 0;
        }

        private static byte[] Derive(
            string passcode,
            byte[] salt,
            int iterations)
        {
            using (Rfc2898DeriveBytes derive =
                   new Rfc2898DeriveBytes(
                       passcode ?? string.Empty,
                       salt,
                       Math.Max(10000, iterations)))
            {
                return derive.GetBytes(32);
            }
        }

        private static string ValidatePasscode(string passcode)
        {
            return string.IsNullOrWhiteSpace(passcode) ||
                   passcode.Length < 6
                ? "Passcode must contain at least six characters."
                : string.Empty;
        }

        private static void Load()
        {
            try
            {
                collection =
                    JsonUtility.FromJson<DeverQuestGuildAccountCollection>(
                        EditorPrefs.GetString(AccountsKey, string.Empty)) ??
                    new DeverQuestGuildAccountCollection();
                collection.accounts =
                    collection.accounts ??
                    new List<DeverQuestGuildAccount>();
                audit = JsonUtility.FromJson<DeverQuestGuildAuditLog>(
                            EditorPrefs.GetString(AuditKey, string.Empty)) ??
                        new DeverQuestGuildAuditLog();
                if (collection.dataVersion < 3)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        DeverQuestAdventurer foundation =
                            new DeverQuestAdventurer
                            {
                                characterClass =
                                    account.characterClass,
                                level = account.level
                            };
                        DeverQuestAdventurerService
                            .ApplyClassFoundation(
                                foundation,
                                account.characterClass,
                                true);
                        CopyRules(foundation, account);
                        account.characterCreationComplete =
                            !string.IsNullOrWhiteSpace(
                                account.characterName);
                    }
                    collection.dataVersion = 3;
                    Save();
                }
                if (collection.dataVersion < 4)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        account.inventory =
                            account.inventory ??
                            new List<DeverQuestInventoryEntry>();
                    }
                    collection.dataVersion = 4;
                    Save();
                }
                if (collection.dataVersion < 5)
                {
                    collection.dataVersion = 5;
                    Save();
                }
                if (collection.dataVersion < 6)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        account.alignment =
                            DeverQuestAlignment.TrueNeutral;
                        DeverQuestIdentityCatalogService.Migrate(
                            account);
                        account.assignedProjects =
                            account.assignedProjects ??
                            new List<string>();
                        account.proficientSaves =
                            account.proficientSaves ??
                            new List<string>();
                        account.statusEffects =
                            account.statusEffects ??
                            new List<string>();
                        account.equippedEquipmentIds =
                            account.equippedEquipmentIds ??
                            new List<string>();
                        account.knownSpellIds =
                            account.knownSpellIds ??
                            new List<string>();
                        account.inventory =
                            account.inventory ??
                            new List<DeverQuestInventoryEntry>();
                    }
                    collection.dataVersion = 6;
                    Save();
                }
                if (collection.dataVersion < 7)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        account.activeCompanionInstanceId =
                            account.activeCompanionInstanceId ??
                            string.Empty;
                        account.companions =
                            account.companions ??
                            new List<DeverQuestCompanionState>();
                        foreach (DeverQuestCompanionState companion
                                 in account.companions)
                        {
                            companion?.Sanitize();
                        }
                    }
                    collection.dataVersion = 7;
                    Save();
                }
                if (collection.dataVersion < 8)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        account.compensationPreviewEnabled = false;
                        account.compensationBasis =
                            DeverQuestCompensationBasis.Hourly;
                        account.compensationCurrencyCode = "USD";
                        account.compensationHourlyRate = 0d;
                        account.compensationAnnualSalary = 0d;
                        account.compensationWeeklyHours = 40d;
                        account.compensationIncludeApprovedBreaks = false;
                        account.compensationIntegrityPolicy =
                            DeverQuestCompensationIntegrityPolicy
                                .VerifiedChroniclesOnly;
                    }
                    collection.dataVersion = 8;
                    Save();
                }
                if (collection.dataVersion < 9)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        NormalizeAccountCoinPurse(account);
                    }
                    collection.dataVersion = 9;
                    Save();
                }
                if (collection.dataVersion < 10)
                {
                    foreach (DeverQuestGuildAccount account
                             in collection.accounts)
                    {
                        if (account != null &&
                            string.IsNullOrWhiteSpace(
                                account.characterName))
                        {
                            account.characterCreationComplete = false;
                        }
                    }
                    collection.dataVersion = 10;
                    Save();
                }
            }
            catch
            {
                collection = new DeverQuestGuildAccountCollection();
                audit = new DeverQuestGuildAuditLog();
            }
        }

        private static void Save()
        {
            EditorPrefs.SetString(
                AccountsKey, JsonUtility.ToJson(collection));
        }

        internal static DeverQuestGuildAccount FindAccount(
            string accountId)
        {
            return collection.accounts.FirstOrDefault(
                item => item.accountId == accountId);
        }

        internal static void CommitAccountChanges(
            DeverQuestGuildAccount account)
        {
            if (account == null)
            {
                return;
            }
            Save();
            if (CurrentAccount != null &&
                CurrentAccount.accountId == account.accountId)
            {
                SelectAccount(account);
            }
        }
    }
}
