# DeverQuest 0.31.5 Beta Issue Log
## Pathway 8 — Editor UX and Workspace Organization

**Source build:** 0.31.4 Beta 1  
**Patch build:** 0.31.5 Beta 1  
**Unity target:** 2022.3 minimum  
**Primary test environment:** Unity 6000.3.8f1  
**Patch status:** Prepared, awaiting Unity verification

---

# Baseline

DeverQuest 0.31.4 contains the core Quest, Chronicle, Guild, Inventory, Economy, Tactical, audio, wellness, Git, and reporting systems. The next usability problem was no longer missing data. It was the amount of unrelated work sharing the same Editor surface.

This build reorganizes the existing tools without changing Quest persistence, reward calculations, Contract rules, Chronicle records, or Guild authority.

---

# DQ-0314-031 — Quest Log and Git are one crowded workspace

**Type:** Editor UX / information architecture  
**Severity:** P1  
**Status:** Patched in 0.31.5; awaiting verification

## Previous behavior

The same workspace contained:

- Encounter progress
- Tactical results
- External-activity state
- Voice memos and attachments
- Quest Log notes
- Commit links
- Repository status
- Git commit
- Git push

This made the live evidence log feel like a repository control panel and made Git harder to use when no Quest was active.

## 0.31.5 correction

Two workspaces now exist:

### Quest Log

Focused on:

- Active Encounter information
- Tactical results
- Quest Log notes
- Commit links
- External activity
- Voice memos
- File attachments
- Existing evidence entries

### Git

Focused on:

- Repository
- Branch and HEAD
- Staged, modified, and untracked counts
- Upstream state
- Commit message
- Commit staged changes
- Stage all and commit
- Push or publish
- Recent Git-linked Quest activity

Git remains usable when no Quest is active.

---

# DQ-0314-032 — No independently dockable Quest timer

**Type:** Editor workflow  
**Severity:** P1  
**Status:** Patched in 0.31.5; awaiting verification

## 0.31.5 correction

A normal dockable EditorWindow is available at:

`Tools > DeverQuest > Quest HUD`

The HUD shows:

- Task and Project
- Focused timer
- Predicted Task Length
- Progress and remaining time or overtime
- Current state
- Approved Break status
- Current Encounter
- Quest Story, when enabled
- Task Objective
- Latest Quest event
- Pause/Resume
- Approved Break
- Turn-in navigation
- Current Quest, Quest Log, and Chronicle navigation

The HUD uses `DeverQuestSessionStore`. It does not create another timer or another Session.

Optional auto-open behavior is controlled in Visuals.

---

# DQ-0314-033 — Workspace navigation is crowded and poorly adaptable

**Type:** Layout  
**Severity:** P1  
**Status:** Patched in 0.31.5; awaiting verification

## 0.31.5 correction

Workspace order now follows the user workflow:

1. Current Quest
2. Quest Log
3. Chronicle
4. Git
5. Character
6. Inventory
7. Economy
8. Tactics
9. Guild Hall
10. Rewards & History
11. Audio & Wellness
12. Visuals
13. Settings

Users may choose:

- Two to six workspace columns
- Full or compact workspace labels
- Workspace guidance on or off
- Header tagline on or off

The previous internal/performance explanation is not part of the user-facing navigation.

---

# DQ-0314-034 — Visual settings lack a dedicated home

**Type:** Presentation settings  
**Severity:** P2  
**Status:** Foundation added in 0.31.5; awaiting verification

## Added settings

- Theme preset
- Custom title color
- Custom timer color
- Custom accent color
- DeverQuest text scale
- Workspace columns
- Compact workspace labels
- Workspace guidance
- Header tagline
- Open HUD when Quest starts
- Show Quest Story in HUD
- Reset Visual Settings

These are local Editor-profile settings. They do not alter Guild or Quest data.

## Boundary

Named Visual Profile assets, portrait frames, complete per-panel color controls, and accessibility presets are not included yet.

---

# DQ-0314-035 — Git commit text reuses the Quest Log note field

**Type:** Data-entry UX  
**Severity:** P1  
**Status:** Patched in 0.31.5; awaiting verification

Quest Log notes and Git commit messages now use separate text fields.

Expected behavior:

- Writing a Quest Log note does not populate the Git commit message.
- Writing a Git commit message does not overwrite the Quest Log note.
- A successful Git commit may still create one Git Commit entry in the active Quest Log.
- A linked note still uses the current repository branch and HEAD.

---

# Compatibility

- Existing profile data migrates to presentation data version 15.
- Existing themes remain valid.
- Existing Compact View remains.
- Existing Quest Log entries remain.
- Existing Git monitor and background command behavior remain.
- Existing Chronicle data is unchanged.
- Existing Session, Contract, Adventurer, Inventory, Economy, and tactical schemas are unchanged.

---

# Required Retest

- [ ] Install 0.31.5 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Confirm Editor workspace configuration passes.
- [ ] Open every workspace.
- [ ] Confirm Quest Log and Git are separate.
- [ ] Add a Quest Log note.
- [ ] Create a Git commit with different text.
- [ ] Confirm neither text field overwrites the other.
- [ ] Open the Quest HUD with no active Quest.
- [ ] Dock the HUD.
- [ ] Start a Quest and confirm the same timer appears in both windows.
- [ ] Pause from the HUD.
- [ ] Resume from the main window.
- [ ] Start an Approved Break from the HUD.
- [ ] Open turn-in from the HUD.
- [ ] Confirm only one Session, Run ID, and reward path exist.
- [ ] Change theme and custom colors.
- [ ] Change text scale and workspace columns.
- [ ] Restart Unity and confirm persistence.
- [ ] Reset visuals and confirm defaults return.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
