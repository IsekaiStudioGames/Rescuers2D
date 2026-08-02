# DeverQuest 0.31.1 Beta Issue Log
## Pathway 5 — Tactical Operations

**Source build:** 0.31.0 Beta 1  
**Patch build:** 0.31.1 Beta 1  
**Unity test environment:** 6000.3.8f1  
**Source status:** Tactical visibility implemented; detailed Combat, Companion, and Survival regression remains deferred  
**Patch status:** Prepared, awaiting Unity verification

---

# Scope

0.31.0 made tactical results readable. This patch makes those results easier to operate, retain, search, and troubleshoot without creating a separate game runtime or altering focused-work time.

---

## DQ-0310-031 — Tactical information is scattered across Quest and Character workspaces

**Type:** Tactical UX  
**Severity:** P1  
**Status:** Patched in 0.31.1; awaiting verification

### Previous behavior

Combat readiness, the active Encounter, Companion state, Survival progress, and latest Battle Results lived in different portions of the Quest and Character workspaces.

### 0.31.1 correction

Added a dedicated **Tactics** workspace and menu command:

`Tools > DeverQuest > Workspaces > Tactical Operations`

The workspace shows:

- Adventurer Hit Points, Mana, Armor Class, carry load, and status effects
- Equipped items and known tactical actions
- Fallen, low-health, and encumbrance warnings
- Current Encounter and Survival progress
- Direct navigation to the current Quest and Encounter Profile
- Active or latest-completed Tactical Field Reports
- Companion operations
- Searchable Battle Archive

---

## DQ-0310-032 — Battle Results disappear from the UI after later Sessions replace the last-completed Session

**Type:** Tactical history  
**Severity:** P1  
**Status:** Patched in 0.31.1; awaiting verification

### Previous behavior

Detailed Battle Results lived inside the active Session, latest completed Session, and generated Timecard. Once another Session replaced the last-completed record, older results were no longer searchable inside DeverQuest.

### 0.31.1 correction

Added a local Battle Archive:

- Automatically records newly resolved battles
- Stores the newest 100 records in the project Library folder
- Imports the active and last-completed Session without duplicates
- Searches Contract, Project, Task, Adventurer, Companion, Run ID, Encounter, and seed
- Filters by Victory, Early Victory, Safety Pause, Defeat, and Survival
- Copies readable reports
- Copies JSON evidence
- Selects the related Encounter Profile
- Removes individual local records
- Clears the local archive without deleting Timecards

### Boundary

The archive is local diagnostic convenience. Timecards and Chronicle files remain the permanent evidence. Shared multi-clone tactical history remains a later shared-Guild feature.

---

## DQ-0310-033 — Companion readiness requires navigating and repairing one Companion at a time

**Type:** Companion operations  
**Severity:** P1  
**Status:** Patched in 0.31.1; awaiting verification

### 0.31.1 correction

The Tactics workspace now provides:

- Roster selector
- Active, Fallen, and resting indicators
- Role, creature type, level, loyalty, Hit Points, and lifetime contribution
- Set Active
- Send to Stable
- Individual recovery with visible cost
- Confirmed full-roster recovery with total cost

Full-roster recovery validates the total purse before changing any Companion, preventing a partially repaired roster when funds are insufficient.

---

## DQ-0310-034 — Tactical local storage is not covered by Release Readiness

**Type:** Beta diagnostics  
**Severity:** Advisory  
**Status:** Patched in 0.31.1; awaiting verification

### 0.31.1 correction

Release Readiness now performs a write/delete probe against the local Tactical Archive and reports the current number of stored Battle Results.

A fully configured project should report one additional pass compared with 0.31.0.

---

# Compatibility

- Existing Contracts, Sessions, Timecards, Companions, and Battle Results remain valid.
- Existing 0.31.0 Battle Results are not silently duplicated.
- Current and last-completed reports can be imported manually.
- New battles archive automatically.
- The archive is capped at 100 records.
- Clearing the archive does not delete Chronicle files.

---

# Required smoke test

- [ ] Install 0.31.1 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Confirm Tactical Archive passes.
- [ ] Open the Tactics workspace.
- [ ] Confirm Adventurer readiness is visible.
- [ ] Confirm the current Encounter is visible during a Quest.
- [ ] Recruit two disposable test Companions.
- [ ] Switch the active Companion from Tactics.
- [ ] Damage or mark a Companion Fallen using disposable test data.
- [ ] Recover one Companion.
- [ ] Recover the entire roster.
- [ ] Resolve one Encounter.
- [ ] Confirm it appears automatically in the Battle Archive.
- [ ] Search by Adventurer or Encounter name.
- [ ] Filter by outcome.
- [ ] Copy the report and JSON.
- [ ] Restart Unity and confirm the archive persists.
- [ ] Clear the local archive and confirm Timecards remain.

---

# Current verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
