//----- DeverQuestContractService.cs START -----

using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoDevGames.DeverQuest
{
    internal static class DeverQuestContractService
    {
        public static void SetStatus(
            DeverQuestQuestContract contract,
            DeverQuestContractStatus status)
        {
            if (contract == null)
            {
                return;
            }

            bool leadershipAction =
                status == DeverQuestContractStatus.Offered ||
                status == DeverQuestContractStatus.Returned ||
                status == DeverQuestContractStatus.Approved ||
                status == DeverQuestContractStatus.Completed;
            if (leadershipAction &&
                !DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                Debug.LogWarning(
                    "[DeverQuest] Current Guild account cannot manage " +
                    $"Contracts for {contract.projectName}.");
                return;
            }

            contract.status = status;
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
            DeverQuestGuildAccountService.AddAudit(
                "Contract " + status,
                contract.contractTitle,
                contract.projectName);
        }

        public static bool SetStatus(
            string contractId,
            DeverQuestContractStatus status)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null)
            {
                return false;
            }

            SetStatus(contract, status);
            return true;
        }

        public static DeverQuestQuestContract Find(string contractId)
        {
            if (string.IsNullOrWhiteSpace(contractId))
            {
                return null;
            }

            string[] guids =
                AssetDatabase.FindAssets("t:DeverQuestQuestContract");
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                DeverQuestQuestContract contract =
                    AssetDatabase.LoadAssetAtPath<DeverQuestQuestContract>(
                        path);
                if (contract != null &&
                    contract.ContractId == contractId)
                {
                    return contract;
                }
            }

            return null;
        }

        public static bool CanJoin(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            out string reason)
        {
            reason = string.Empty;
            if (contract == null || adventurer == null)
            {
                reason = "Contract or Adventurer was unavailable.";
                return false;
            }

            if (contract.archived)
            {
                reason = "This Quest listing is archived by Guild leadership.";
                return false;
            }

            DeverQuestContractRunReservation existingRun =
                contract.FindActiveRunFor(adventurer.characterName);
            if (existingRun != null)
            {
                if (contract.groupQuest)
                {
                    DeverQuestPartyMember submittedMember =
                        (contract.partyMembers ??
                         new List<DeverQuestPartyMember>())
                            .FirstOrDefault(member =>
                                string.Equals(
                                    member.adventurerName,
                                    adventurer.characterName,
                                    StringComparison.OrdinalIgnoreCase));
                    if (submittedMember != null &&
                        submittedMember.submitted)
                    {
                        reason =
                            "This Adventurer has already submitted the " +
                            "current Party run.";
                        return false;
                    }
                }
                return true;
            }

            if (contract.groupQuest &&
                (contract.partyMembers ??
                 new List<DeverQuestPartyMember>()).Any(member =>
                    string.Equals(
                        member.adventurerName,
                        adventurer.characterName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            if (contract.IsBoardComplete)
            {
                reason = "This Quest has reached its completion target.";
                return false;
            }

            if (contract.oneCompletionPerAdventurer &&
                contract.HasCompletedBy(adventurer.characterName))
            {
                reason =
                    "This Adventurer has already completed this Quest.";
                return false;
            }

            if (contract.availabilityPolicy !=
                DeverQuestContractAvailabilityPolicy.Repeatable &&
                contract.ActiveRunCount >= contract.RemainingCompletions)
            {
                reason =
                    "All remaining completion slots are currently claimed.";
                return false;
            }

            bool canManageContract =
                DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName);
            if ((contract.status == DeverQuestContractStatus.Draft ||
                 contract.status == DeverQuestContractStatus.Returned) &&
                !canManageContract)
            {
                reason =
                    $"This Contract is {contract.status} and is not on the " +
                    "Assignment Board.";
                return false;
            }

            if (contract.status == DeverQuestContractStatus.Completed &&
                contract.IsBoardComplete)
            {
                reason = "This Contract is complete.";
                return false;
            }

            if (contract.groupQuest &&
                !string.IsNullOrWhiteSpace(contract.ActivePartyRunId))
            {
                reason =
                    "A Party is already running this Quest. Wait for that " +
                    "run to finish before assembling another Party.";
                return false;
            }

            if (contract.groupQuest && !contract.HasOpenPartySlot)
            {
                reason = "This Quest party is full.";
                return false;
            }

            if (adventurer.level < contract.minimumAdventurerLevel)
            {
                reason =
                    $"Requires Level {contract.minimumAdventurerLevel}.";
                return false;
            }

            if (contract.restrictToClasses &&
                !(contract.eligibleClassDefinitions ??
                  new List<DeverQuestClassDefinition>())
                    .Any(item =>
                        item != null &&
                        (item.IdentityId == adventurer.classId ||
                         string.Equals(
                             item.displayName,
                             adventurer.characterClass,
                             StringComparison.OrdinalIgnoreCase))) &&
                !(contract.eligibleClasses ?? new List<string>()).Any(
                    item => string.Equals(
                        item,
                        adventurer.characterClass,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    $"{adventurer.characterClass} is not eligible.";
                return false;
            }

            if (contract.restrictToAncestries &&
                !(contract.eligibleAncestries ??
                  new List<DeverQuestAncestry>())
                    .Any(item =>
                        item != null &&
                        (item.IdentityId == adventurer.ancestryId ||
                         string.Equals(
                             item.displayName,
                             adventurer.ancestryName,
                             StringComparison.OrdinalIgnoreCase))))
            {
                string ancestryDisplayName =
                    string.IsNullOrWhiteSpace(adventurer.ancestryName)
                        ? "This Ancestry"
                        : adventurer.ancestryName;
                reason = $"{ancestryDisplayName} is not eligible.";
                return false;
            }

            if (contract.restrictToDepartments &&
                !contract.eligibleDepartments.Any(
                    item => string.Equals(
                        item,
                        adventurer.homeDepartment,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    $"{adventurer.homeDepartment} is not eligible.";
                return false;
            }

            if (contract.groupQuest &&
                !contract.openToAnyMember &&
                contract.assignedAdventurers != null &&
                contract.assignedAdventurers.Count > 0 &&
                !contract.assignedAdventurers.Any(
                    item => string.Equals(
                        item,
                        adventurer.characterName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                reason =
                    "This Party Quest has a reserved Adventurer roster.";
                return false;
            }

            bool individuallyAssigned =
                string.IsNullOrWhiteSpace(contract.assignedAdventurer) ||
                string.Equals(
                    contract.assignedAdventurer,
                    adventurer.characterName,
                    StringComparison.OrdinalIgnoreCase);
            if (!contract.openToAnyMember &&
                !contract.groupQuest &&
                !individuallyAssigned)
            {
                reason =
                    "This Quest is assigned to another Adventurer.";
                return false;
            }

            return true;
        }

        public static bool Join(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            string developerName,
            out string runId,
            out string error)
        {
            runId = string.Empty;
            if (!CanJoin(contract, adventurer, out error))
            {
                return false;
            }

            DeverQuestContractRunReservation existingRun =
                contract.FindActiveRunFor(adventurer.characterName);
            if (existingRun != null)
            {
                runId = existingRun.runId;
                return true;
            }

            if (!contract.groupQuest)
            {
                DeverQuestContractRunReservation soloRun =
                    CreateRun(
                        false,
                        new[] { adventurer.characterName },
                        new[] { developerName });
                contract.activeRuns.Add(soloRun);
                runId = soloRun.runId;
                if (contract.availabilityPolicy ==
                    DeverQuestContractAvailabilityPolicy.SingleCompletion)
                {
                    contract.status = DeverQuestContractStatus.Active;
                }
                else
                {
                    contract.status = DeverQuestContractStatus.Offered;
                }

                SaveContract(contract);
                DeverQuestGuildAccountService.AddAudit(
                    "Quest Run Started",
                    contract.contractTitle,
                    adventurer.characterName + " · " + runId);
                return true;
            }

            DeverQuestPartyMember existingMember =
                contract.partyMembers.FirstOrDefault(member =>
                    string.Equals(
                        member.adventurerName,
                        adventurer.characterName,
                        StringComparison.OrdinalIgnoreCase));
            if (existingMember == null)
            {
                contract.partyMembers.Add(
                    new DeverQuestPartyMember
                    {
                        adventurerName = adventurer.characterName,
                        developerName =
                            developerName?.Trim() ?? string.Empty,
                        partyRole = adventurer.homeDepartment,
                        joinedUtc = DateTime.UtcNow.ToString("O")
                    });
                DeverQuestGuildAccountService.AddAudit(
                    "Party Joined",
                    contract.contractTitle,
                    $"{adventurer.characterName} · " +
                    $"{contract.partyMembers.Count}/" +
                    $"{contract.maximumParticipants}");
            }

            if (!contract.CanPartyStart)
            {
                contract.status = DeverQuestContractStatus.Offered;
                SaveContract(contract);
                return true;
            }

            DeverQuestContractRunReservation partyRun =
                contract.FindActiveRun(contract.ActivePartyRunId);
            if (partyRun == null)
            {
                partyRun = CreateRun(
                    true,
                    contract.partyMembers.Select(
                        member => member.adventurerName),
                    contract.partyMembers.Select(
                        member => member.developerName));
                contract.activeRuns.Add(partyRun);
                contract.SetActivePartyRunId(partyRun.runId);
            }

            runId = partyRun.runId;
            contract.status = DeverQuestContractStatus.Active;
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Party Quest Started",
                contract.contractTitle,
                $"{partyRun.adventurerNames.Count} Adventurer(s) · " +
                runId);
            return true;
        }

        public static bool Join(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            string developerName,
            out string error)
        {
            return Join(
                contract,
                adventurer,
                developerName,
                out _,
                out error);
        }

        public static bool LeaveParty(
            DeverQuestQuestContract contract,
            DeverQuestAdventurer adventurer,
            out string error)
        {
            error = string.Empty;
            if (contract == null || adventurer == null)
            {
                error = "Contract or Adventurer was unavailable.";
                return false;
            }
            if (!contract.groupQuest)
            {
                error = "Only Party Quests have a waiting roster.";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(contract.ActivePartyRunId))
            {
                error =
                    "This Party Quest has already started and can no longer " +
                    "be left from the board.";
                return false;
            }

            int removed = contract.partyMembers.RemoveAll(member =>
                string.Equals(
                    member.adventurerName,
                    adventurer.characterName,
                    StringComparison.OrdinalIgnoreCase));
            if (removed == 0)
            {
                error = "This Adventurer is not on the Party roster.";
                return false;
            }

            contract.status = DeverQuestContractStatus.Offered;
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Party Left",
                contract.contractTitle,
                adventurer.characterName + " withdrew before the Quest " +
                "began.");
            return true;
        }

        public static void RecordSessionCompletion(
            DeverQuestSession session,
            string adventurerName,
            string developerName)
        {
            if (session == null ||
                string.IsNullOrWhiteSpace(session.questContractId))
            {
                return;
            }

            DeverQuestQuestContract contract =
                Find(session.questContractId);
            if (contract == null)
            {
                return;
            }

            DeverQuestContractRunReservation run =
                contract.FindActiveRun(session.questContractRunId) ??
                contract.FindActiveRunFor(adventurerName);
            if (run == null)
            {
                run = CreateRun(
                    session.questIsGroupQuest,
                    new[] { adventurerName },
                    new[] { developerName });
                run.runId = string.IsNullOrWhiteSpace(
                    session.questContractRunId)
                        ? run.runId
                        : session.questContractRunId;
            }

            long awardedCopper = session.rewardTransactions == null
                ? 0L
                : session.rewardTransactions.Sum(
                    transaction => Math.Max(0L, transaction.copper));
            long awardedExperience = session.rewardTransactions == null
                ? 0L
                : session.rewardTransactions.Sum(
                    transaction => Math.Max(0L, transaction.experience));
            double focusedMinutes = Math.Max(
                0d,
                session.accumulatedFocusedSeconds / 60d);

            if (contract.groupQuest)
            {
                DeverQuestPartyMember member =
                    contract.partyMembers.FirstOrDefault(item =>
                        string.Equals(
                            item.adventurerName,
                            adventurerName,
                            StringComparison.OrdinalIgnoreCase));
                if (member != null)
                {
                    member.submitted = true;
                    member.submittedUtc =
                        DateTime.UtcNow.ToString("O");
                    member.sessionId = session.sessionId;
                    member.focusedMinutes = focusedMinutes;
                    member.awardedCopper = awardedCopper;
                    member.awardedExperience = awardedExperience;
                }

                bool allSubmitted =
                    run.adventurerNames.All(name =>
                        contract.partyMembers.Any(memberItem =>
                            string.Equals(
                                memberItem.adventurerName,
                                name,
                                StringComparison.OrdinalIgnoreCase) &&
                            memberItem.submitted));
                if (!allSubmitted)
                {
                    contract.status = DeverQuestContractStatus.Active;
                    SaveContract(contract);
                    DeverQuestGuildAccountService.AddAudit(
                        "Party Turn-In",
                        contract.contractTitle,
                        $"{adventurerName} submitted; " +
                        $"{contract.partyMembers.Count(item => item.submitted)}/" +
                        $"{run.adventurerNames.Count}");
                    return;
                }

                List<DeverQuestPartyMember> completedMembers =
                    contract.partyMembers
                        .Where(item =>
                            run.adventurerNames.Any(name =>
                                string.Equals(
                                    name,
                                    item.adventurerName,
                                    StringComparison.OrdinalIgnoreCase)))
                        .ToList();
                awardedCopper = completedMembers.Sum(
                    item => Math.Max(0L, item.awardedCopper));
                awardedExperience = completedMembers.Sum(
                    item => Math.Max(0L, item.awardedExperience));
                focusedMinutes = completedMembers.Sum(
                    item => Math.Max(0d, item.focusedMinutes));
            }

            List<string> completionSessionIds = contract.groupQuest
                ? contract.partyMembers
                    .Where(item =>
                        run.adventurerNames.Any(name =>
                            string.Equals(
                                name,
                                item.adventurerName,
                                StringComparison.OrdinalIgnoreCase)))
                    .Select(item => item.sessionId)
                    .Where(id => !string.IsNullOrWhiteSpace(id))
                    .ToList()
                : new List<string> { session.sessionId };

            DeverQuestContractCompletionRecord completion =
                new DeverQuestContractCompletionRecord
                {
                    completionId = Guid.NewGuid().ToString("N"),
                    runId = run.runId,
                    sessionId = session.sessionId,
                    sessionIds = completionSessionIds,
                    completedUtc = DateTime.UtcNow.ToString("O"),
                    adventurerNames =
                        new List<string>(run.adventurerNames),
                    developerNames =
                        new List<string>(run.developerNames),
                    focusedMinutes = focusedMinutes,
                    awardedCopper = awardedCopper,
                    awardedExperience = awardedExperience
                };
            contract.completionHistory.Add(completion);
            contract.activeRuns.RemoveAll(item =>
                item != null && item.runId == run.runId);

            if (contract.groupQuest)
            {
                contract.partyMembers.Clear();
                contract.stageProgress.Clear();
                contract.SetActivePartyRunId(string.Empty);
            }

            UpdateBoardLifecycle(contract);
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Quest Run Completed",
                contract.contractTitle,
                $"Run {contract.CompletedRunCount} · " +
                string.Join(", ", completion.adventurerNames));
        }

        public static void AbandonRun(
            string contractId,
            string runId,
            string adventurerName)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null)
            {
                return;
            }

            DeverQuestContractRunReservation run =
                contract.FindActiveRun(runId) ??
                contract.FindActiveRunFor(adventurerName);
            if (run != null)
            {
                contract.activeRuns.Remove(run);
            }

            if (contract.groupQuest)
            {
                contract.partyMembers.Clear();
                contract.stageProgress.Clear();
                contract.SetActivePartyRunId(string.Empty);
            }

            UpdateBoardLifecycle(contract);
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Quest Run Abandoned",
                contract.contractTitle,
                adventurerName);
        }

        public static bool ReopenForAnotherRun(
            DeverQuestQuestContract contract,
            out string error)
        {
            error = string.Empty;
            if (contract == null)
            {
                error = "Contract was unavailable.";
                return false;
            }
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                error =
                    "Current Guild account cannot restore this Contract.";
                return false;
            }
            if (contract.archived)
            {
                error =
                    "Restore the archived listing before reopening it.";
                return false;
            }
            if (contract.ActiveRunCount > 0 ||
                (contract.partyMembers != null &&
                 contract.partyMembers.Count > 0))
            {
                error =
                    "Finish or cancel active Quest Runs and waiting Party " +
                    "reservations before reopening this listing.";
                return false;
            }
            if (contract.availabilityPolicy ==
                DeverQuestContractAvailabilityPolicy.Repeatable)
            {
                error =
                    "Repeatable Contracts already remain available.";
                return false;
            }
            if (!contract.IsBoardComplete)
            {
                error =
                    "This Contract already has an available completion slot.";
                return false;
            }

            contract.AddReopenedCompletionSlot();
            contract.status = DeverQuestContractStatus.Offered;
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Completed Contract Reopened",
                contract.contractTitle,
                $"Completion target is now {contract.CompletionTarget}.");
            return true;
        }

        public static bool SetArchived(
            DeverQuestQuestContract contract,
            bool archived,
            out string error)
        {
            error = string.Empty;
            if (contract == null)
            {
                error = "Contract was unavailable.";
                return false;
            }
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                error = "Current Guild account cannot archive this Contract.";
                return false;
            }
            if (archived &&
                (contract.ActiveRunCount > 0 ||
                 (contract.partyMembers != null &&
                  contract.partyMembers.Count > 0)))
            {
                error =
                    "Finish or cancel active Quest Runs and waiting Party " +
                    "reservations before archiving this listing.";
                return false;
            }

            contract.archived = archived;
            if (!archived)
            {
                UpdateBoardLifecycle(contract);
            }
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                archived ? "Contract Archived" : "Contract Restored",
                contract.contractTitle,
                contract.projectName);
            return true;
        }

        public static bool CancelRunReservation(
            DeverQuestQuestContract contract,
            string runId,
            out string error)
        {
            error = string.Empty;
            if (contract == null)
            {
                error = "Contract was unavailable.";
                return false;
            }
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                error = "Current Guild account cannot cancel Quest Runs.";
                return false;
            }

            DeverQuestContractRunReservation run =
                contract.FindActiveRun(runId);
            if (run == null)
            {
                error = "The selected Quest Run is no longer active.";
                return false;
            }

            DeverQuestSession localSession =
                DeverQuestSessionStore.ActiveSession;
            if (localSession != null &&
                localSession.IsActive &&
                string.Equals(
                    localSession.questContractRunId,
                    run.runId,
                    StringComparison.Ordinal))
            {
                error =
                    "This Quest Run belongs to the active local Session. " +
                    "Complete or abandon that Quest from the Quest workspace.";
                return false;
            }

            contract.activeRuns.Remove(run);
            if (run.groupRun ||
                string.Equals(
                    contract.ActivePartyRunId,
                    run.runId,
                    StringComparison.Ordinal))
            {
                contract.partyMembers.Clear();
                contract.stageProgress.Clear();
                contract.SetActivePartyRunId(string.Empty);
            }

            UpdateBoardLifecycle(contract);
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Quest Run Reservation Cancelled",
                contract.contractTitle,
                run.runId + " · " +
                string.Join(", ", run.adventurerNames));
            return true;
        }

        public static bool ClearWaitingParty(
            DeverQuestQuestContract contract,
            out string error)
        {
            error = string.Empty;
            if (contract == null)
            {
                error = "Contract was unavailable.";
                return false;
            }
            if (!DeverQuestGuildAccountService.HasPermission(
                    DeverQuestGuildPermission.ManageContracts,
                    contract.projectName))
            {
                error = "Current Guild account cannot clear Party reservations.";
                return false;
            }
            if (!contract.groupQuest ||
                contract.partyMembers == null ||
                contract.partyMembers.Count == 0)
            {
                error = "This Contract has no waiting Party reservation.";
                return false;
            }
            if (!string.IsNullOrWhiteSpace(contract.ActivePartyRunId))
            {
                error =
                    "The Party Quest has already started. Cancel its Quest " +
                    "Run reservation instead.";
                return false;
            }

            string names = string.Join(", ", contract.partyMembers
                .Where(member => member != null)
                .Select(member => member.adventurerName));
            contract.partyMembers.Clear();
            contract.stageProgress.Clear();
            UpdateBoardLifecycle(contract);
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Waiting Party Cleared",
                contract.contractTitle,
                names);
            return true;
        }

        public static void RecordStageCompletion(
            string contractId,
            string stageId,
            string stageTitle,
            string adventurerName)
        {
            DeverQuestQuestContract contract = Find(contractId);
            if (contract == null ||
                string.IsNullOrWhiteSpace(stageId) ||
                contract.stageProgress.Any(item =>
                    item.stageId == stageId &&
                    string.Equals(
                        item.adventurerName,
                        adventurerName,
                        StringComparison.OrdinalIgnoreCase)))
            {
                return;
            }
            contract.stageProgress.Add(
                new DeverQuestPartyStageProgress
                {
                    stageId = stageId,
                    stageTitle = stageTitle,
                    adventurerName = adventurerName,
                    completedUtc = DateTime.UtcNow.ToString("O")
                });
            SaveContract(contract);
            DeverQuestGuildAccountService.AddAudit(
                "Encounter Completed",
                contract.contractTitle,
                $"{adventurerName} · {stageTitle}");
        }

        private static DeverQuestContractRunReservation CreateRun(
            bool groupRun,
            IEnumerable<string> adventurerNames,
            IEnumerable<string> developerNames)
        {
            return new DeverQuestContractRunReservation
            {
                runId = Guid.NewGuid().ToString("N"),
                startedUtc = DateTime.UtcNow.ToString("O"),
                groupRun = groupRun,
                adventurerNames = adventurerNames
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList(),
                developerNames = developerNames
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Select(name => name.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList()
            };
        }

        private static void UpdateBoardLifecycle(
            DeverQuestQuestContract contract)
        {
            if (contract.IsBoardComplete)
            {
                contract.status = DeverQuestContractStatus.Completed;
                return;
            }

            if (contract.availabilityPolicy ==
                DeverQuestContractAvailabilityPolicy.SingleCompletion &&
                contract.ActiveRunCount > 0)
            {
                contract.status = DeverQuestContractStatus.Active;
                return;
            }

            contract.status = DeverQuestContractStatus.Offered;
        }

        private static void SaveContract(
            DeverQuestQuestContract contract)
        {
            EditorUtility.SetDirty(contract);
            AssetDatabase.SaveAssets();
        }
    }
}

//----- DeverQuestContractService.cs END -----
