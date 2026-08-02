# DeverQuest 0.30.8 Deferred Verification Checklist

**Status:** IMPLEMENTED / SMOKE-TESTED BY OBSERVATION / FULL REGRESSION DEFERRED  
**Reason:** Multi-run and multi-account testing is being set aside until a dedicated Guild regression session.  
**Do not interpret unchecked entries as failures.** Preserve this checklist and resume it before Release Candidate.

---

# DeverQuest 0.30.8 Beta Test Checklist
## Quest 3 — The Everlasting Notice Board

**Build:** 0.30.8 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

---

# A. Baseline and Upgrade

- [x] 0.30.7 Release Readiness reported 15 passes, 0 advisories, 0 blockers.
- [ ] Install `com.echodevgames.deverquest-0.30.8.tgz`.
- [ ] Confirm Package Manager reports 0.30.8.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm no new blocker.
- [ ] Inspect all existing Quest Contracts.
- [ ] Confirm existing Contracts default to Single Completion.
- [ ] Confirm existing reward snapshots remain unchanged.
- [ ] Confirm existing Contract IDs remain unchanged.
- [ ] Confirm an existing Completed Contract remains Completed.
- [ ] Confirm the existing completed Contract has a legacy Completion History entry.

---

# B. One-Time Contract

Create:

- Availability Policy: Single Completion
- Open to Any Member: enabled
- Group Quest: disabled

Tests:

- [ ] Offer the Contract.
- [ ] Confirm Assignment Board says `One-time · available`.
- [ ] Accept it.
- [ ] Confirm a Quest Run reservation is created.
- [ ] Confirm a Run ID appears in the active Session.
- [ ] Complete the Quest.
- [ ] Confirm the Contract becomes Completed automatically.
- [ ] Confirm it is removed from the normal Member board.
- [ ] Confirm leadership can still inspect it.
- [ ] Confirm Completed Runs equals 1.
- [ ] Confirm Last Completed By names the Adventurer.
- [ ] Inspect Completion History.
- [ ] Confirm Adventurer, developer, Session ID, Run ID, focused time, coin, XP, and timestamp.
- [ ] Open the Timecard.
- [ ] Confirm it includes the Quest Run ID.
- [ ] Attempt to accept the Contract again.
- [ ] Confirm acceptance is denied because the target is complete.

---

# C. Limited Contract, Five Unique Adventurers

Create:

- Availability Policy: Limited Completions
- Required Completions: 5
- One Completion Per Adventurer: enabled
- Open to Any Member: enabled
- Group Quest: disabled

Tests:

- [ ] Confirm board shows `0/5 completed`.
- [ ] Complete Run 1 with Adventurer A.
- [ ] Confirm board shows `1/5 completed`.
- [ ] Attempt Run 2 with Adventurer A.
- [ ] Confirm it is denied with an already-completed message.
- [ ] Complete Run 2 with Adventurer B.
- [ ] Complete Run 3 with Adventurer C.
- [ ] Complete Run 4 with Adventurer D.
- [ ] Confirm the board remains Offered at 4/5.
- [ ] Complete Run 5 with Adventurer E.
- [ ] Confirm the Contract becomes Completed.
- [ ] Confirm it leaves the Member board.
- [ ] Confirm Completion History contains five records.
- [ ] Confirm all five Adventurer names are different.
- [ ] Confirm no reward was awarded twice for one run.

---

# D. Limited Contract, Repeaters Allowed

Create:

- Availability Policy: Limited Completions
- Required Completions: 3
- One Completion Per Adventurer: disabled

Tests:

- [ ] Complete all three runs with the same Adventurer.
- [ ] Confirm each run has a different Run ID.
- [ ] Confirm the Contract remains Offered after runs 1 and 2.
- [ ] Confirm it closes after run 3.
- [ ] Confirm Completion History contains three records.
- [ ] Confirm all three records name the same Adventurer.
- [ ] Confirm each Session has its own reward transaction.

---

# E. Unlimited Repeatable Contract

Create a five-minute exercise Contract:

- Availability Policy: Repeatable
- One Completion Per Adventurer: disabled
- Group Quest: disabled

Tests:

- [ ] Confirm board says `Repeatable · unlimited runs`.
- [ ] Complete it once.
- [ ] Confirm it returns to Offered.
- [ ] Complete it a second time.
- [ ] Confirm it remains Offered.
- [ ] Complete it a third time.
- [ ] Confirm Completed Runs equals 3.
- [ ] Confirm three different Run IDs.
- [ ] Confirm rewards were granted once per completed run.
- [ ] Abandon a fourth run.
- [ ] Confirm no completion record is added.
- [ ] Confirm the Contract remains available.
- [ ] Restart Unity.
- [ ] Confirm completion count and history persist.

---

# F. Active Reservation Limits

Create:

- Availability Policy: Limited Completions
- Required Completions: 2
- One Completion Per Adventurer: disabled

Tests:

- [ ] Start Run A without completing it.
- [ ] Start Run B with another Adventurer/account.
- [ ] Confirm both runs have different Run IDs.
- [ ] Attempt a third run.
- [ ] Confirm it is denied because all remaining slots are claimed.
- [ ] Abandon Run A.
- [ ] Confirm one slot becomes available.
- [ ] Start Run C.
- [ ] Complete Runs B and C.
- [ ] Confirm the Contract closes at two completed runs.
- [ ] Confirm abandoned Run A is not in Completion History.

---

# G. Flexible Party Quest

Create:

- Group Quest: enabled
- Minimum Participants: 1
- Maximum Participants: 4
- Require Full Party: disabled
- Availability Policy: Repeatable

Tests:

- [ ] Confirm board shows minimum 1 and maximum 4.
- [ ] Join with one Adventurer.
- [ ] Confirm the Quest may begin immediately.
- [ ] Confirm the active Party Run contains one participant.
- [ ] Attempt to join the active run with another Adventurer.
- [ ] Confirm the roster is locked.
- [ ] Complete the Quest.
- [ ] Confirm the completion record names the one participant.
- [ ] Confirm the roster clears.
- [ ] Confirm the Contract returns to Offered.

---

# H. Minimum Party Below Maximum

Create:

- Group Quest: enabled
- Minimum Participants: 2
- Maximum Participants: 4
- Require Full Party: disabled
- Availability Policy: Repeatable

Tests:

- [ ] Join Participant A.
- [ ] Confirm the Quest waits at 1/2 required.
- [ ] Confirm Participant A may Leave Party.
- [ ] Rejoin Participant A.
- [ ] Join Participant B.
- [ ] Confirm the Quest may begin at 2/4.
- [ ] Confirm it does not wait for four.
- [ ] Start the run for both participants.
- [ ] Submit Participant A.
- [ ] Confirm the run remains Active.
- [ ] Confirm Participant A cannot start the same active run again.
- [ ] Submit Participant B.
- [ ] Confirm one group completion record is created.
- [ ] Confirm both names appear in the record.
- [ ] Confirm both Session IDs appear.
- [ ] Confirm the Contract returns to Offered.

---

# I. Full Party Required

Create:

- Group Quest: enabled
- Maximum Participants: 3
- Require Full Party: enabled
- Availability Policy: Single Completion

Tests:

- [ ] Join Participant A.
- [ ] Confirm it waits at 1/3.
- [ ] Join Participant B.
- [ ] Confirm it waits at 2/3.
- [ ] Join Participant C.
- [ ] Confirm the Party Run begins at 3/3.
- [ ] Confirm no fourth participant may join.
- [ ] Complete all three participant Sessions.
- [ ] Confirm one group completion record is created.
- [ ] Confirm the Contract becomes Completed.
- [ ] Confirm group bonuses follow the existing full-party rule.

---

# J. Board and History Presentation

- [ ] Confirm every Contract displays Availability.
- [ ] Confirm every Contract displays Completed Runs.
- [ ] Confirm completed history displays Last Completed By.
- [ ] Confirm Limited Completions displays current/target count.
- [ ] Confirm Unique Adventurer wording appears when enabled.
- [ ] Confirm Party cards show joined count.
- [ ] Confirm Party cards show start rule.
- [ ] Confirm waiting messages use minimum required count.
- [ ] Confirm long participant names wrap safely.
- [ ] Confirm narrow dock layout remains readable.
- [ ] Confirm leadership can inspect completion records in the Contract Inspector.

---

# K. Reward and Chronicle Integrity

For one one-time, one limited, one repeatable, and one Party run:

- [ ] Compare projected coin with awarded coin.
- [ ] Compare projected XP with awarded XP.
- [ ] Confirm one reward journal entry per reward source.
- [ ] Confirm no run is rewarded twice.
- [ ] Confirm the Timecard names the Contract.
- [ ] Confirm the Timecard includes the Run ID.
- [ ] Confirm the Chronicle names the correct Adventurer.
- [ ] Confirm group completion does not erase individual Timecards.
- [ ] Confirm completion history totals equal the submitted Session rewards.
- [ ] Restart Unity and confirm all records persist.

---

# L. Multi-Clone Shared Guild Boundary

- [ ] Complete a repeatable Contract in Clone A.
- [ ] Commit and push the Contract asset.
- [ ] Pull into Clone B.
- [ ] Confirm the Completion History appears.
- [ ] Complete another run in Clone B.
- [ ] Push and pull normally.
- [?] Attempt truly concurrent edits in both clones.
- [?] Record whether Unity YAML creates a merge conflict.
- [?] Treat merge-safe append-only shared Run records as a future shared-Guild improvement, not a silent promise of 0.30.8.

---

# Verdict

- [ ] **PASS** — all one-time, limited, repeatable, reservation, and Party rules pass.
- [ ] **CONDITIONAL PASS** — local behavior passes; documented cross-clone merge limitation remains.
- [ ] **FAIL** — completion counts, availability, rewards, participant history, or Party rules are inconsistent.
