# DeverQuest 0.31.4 Beta Issue Log
## Pathway 7 — Quest Archive and Chronicle Navigation

**Source build:** 0.31.3 Beta 1  
**Patch build:** 0.31.4 Beta 1  
**Unity target:** 2022.3 minimum  
**Primary test environment:** Unity 6000.3.8f1  
**Patch status:** Prepared, awaiting Unity verification

---

# Baseline

The current package already stores completed Quest evidence across:

- Session data
- Generated daily Timecards
- Chronicle integrity records
- Quest Run completion history
- Reward Journal
- Commit Journal
- Media attachments
- Tactical Battle Results
- Wellness Journal
- Shared Guild publication

The problem was navigation rather than missing data. Users had to move among Current Quest, Quest Log & Git, Rewards & History, Contract assets, generated Markdown files, and the Project window to reconstruct one Quest.

---

# DQ-0313-026 — Completed Quest evidence is fragmented

**Type:** Workflow / information architecture  
**Severity:** P1  
**Status:** Patched in 0.31.4; awaiting verification

## Previous behavior

A completed Quest could require separate inspection of:

- Rewards & History for the daily record
- The generated Timecard file for the full report
- Quest Run Archive for Contract completion details
- Quest Contract asset for the source assignment
- Tactics for combat details
- The file browser for attachments
- Quest Log & Git only while a Quest remained active

This made the completed loop technically recorded but awkward to review.

## 0.31.4 correction

A dedicated **Chronicle** workspace now combines:

- Live Quest Chronicle
- Completed Quest Archive
- Search and archive filters
- Story and Task Objective
- Current Encounter
- Reward Journal
- Quest Log notes and commit hashes
- Media attachments
- Tactical outcomes
- Closing notes
- Run ID and source Contract navigation
- Generated Timecard open/reveal
- Correction-request routing
- Chronological event timeline

---

# DQ-0313-027 — Active Quest events are not presented as one timeline

**Type:** Active Quest UX  
**Severity:** P1  
**Status:** Patched in 0.31.4; awaiting verification

## Previous behavior

The active Quest panel displayed the timer, objective, Encounter, combat result, and Quest Log controls, but meaningful events were separated by feature section.

## 0.31.4 correction

The Chronicle builds a chronological event stream from existing Session data:

- Quest start
- Encounter completion
- Tactical result
- Quest Log note
- Linked commit
- Media attachment
- Wellness event
- External activity
- Reward transaction
- Quest completion

The main Quest workspace also displays a compact recent-event feed and a direct **Open Chronicle** action.

The event feed derives from existing evidence. It does not create new reward or timing records.

---

# DQ-0313-028 — Completed Quest records lack direct navigation

**Type:** Navigation  
**Severity:** P1  
**Status:** Patched in 0.31.4; awaiting verification

Each completed Quest card now supports:

- Open Timecard
- Reveal Timecard
- Copy readable Quest summary
- Copy Quest Run ID
- Select and ping source Contract
- Route the selected Session to Rewards & History correction controls

Media records support:

- Open
- Reveal
- Copy path

Missing files produce warnings without deleting their attachment records.

---

# DQ-0313-029 — Archive searching is too coarse

**Type:** Reporting UX  
**Severity:** P2  
**Status:** Patched in 0.31.4; awaiting verification

The completed archive may now search:

- Session ID
- Developer
- Project
- Task
- Department
- Objective
- Quest Profile
- Contract
- Quest Run ID
- Quest Story
- Closing notes
- Commit comments
- Commit hashes
- Attachment names
- Encounter names
- Tactical seeds

Archive filters include:

- All completed Quests
- Contract Runs
- With Rewards
- With Commits or Notes
- With Media
- With Combat

The visible result count may be limited from 5 to 100.

---

# DQ-0313-030 — Chronicle navigation health is not checked before regression

**Type:** Release Readiness  
**Severity:** P2  
**Status:** Patched in 0.31.4; awaiting verification

Release Readiness now checks:

- Duplicate Session ID groups
- Missing generated Markdown Timecards
- Missing recorded attachment paths
- History-loading errors

Duplicate Session IDs and missing Timecards produce an advisory.

Missing attachments produce an advisory while preserving the underlying Chronicle entry.

---

# Compatibility and Guardrails

- No Session schema migration is required.
- Existing Timecards remain the permanent Markdown Chronicle.
- Existing JSON daily records remain authoritative for archive loading.
- Existing Rewards & History features remain.
- Existing correction approval remains under Rewards & History.
- Existing Compensation Preview remains under Rewards & History.
- The Chronicle does not award coin, XP, items, or focused time.
- The Chronicle does not resolve Encounters.
- The Chronicle does not create or complete Quest Runs.
- Missing attachments are never silently removed.
- Search and expansion state are local Editor UI state only.

---

# Required Retest

- [ ] Install 0.31.4 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Open Chronicle with no active Quest.
- [ ] Confirm the most recent completed Quest appears.
- [ ] Open and reveal its generated Timecard.
- [ ] Search by Task.
- [ ] Search by Project.
- [ ] Search by Run ID.
- [ ] Search by commit hash.
- [ ] Filter With Media.
- [ ] Filter With Combat.
- [ ] Expand a completed Quest.
- [ ] Confirm Story, Objective, rewards, notes, media, closing notes, and timeline.
- [ ] Copy its summary.
- [ ] Copy its Run ID.
- [ ] Select its source Contract.
- [ ] Route it to Request Correction.
- [ ] Start a Quest.
- [ ] Confirm the compact event feed appears in Current Quest.
- [ ] Open Chronicle.
- [ ] Add a Quest Log note and confirm it appears.
- [ ] Attach media and confirm it appears.
- [ ] Trigger a Wellness event and confirm it appears.
- [ ] Complete an Encounter and confirm the event appears.
- [ ] Complete the Quest.
- [ ] Refresh the archive and confirm the Quest moves into completed history.
- [ ] Confirm no duplicate reward or completion is created by opening Chronicle.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
