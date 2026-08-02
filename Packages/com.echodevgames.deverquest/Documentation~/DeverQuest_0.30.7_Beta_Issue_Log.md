# DeverQuest 0.30.7 Beta Issue Log
## Beta Expedition 01 Consolidation

**Source build tested:** 0.30.6 Beta 1
**Patch build:** 0.30.7 Beta 1
**Unity:** 6000.3.8f1
**Tester:** EchoDev
**Overall source-build verdict:** CONDITIONAL PASS
**Patch status:** Prepared, awaiting Unity verification

---

# Evidence Summary

The one-hour Quest completed and produced a valid Chronicle with:

- 1h 0m 15s focused work
- 36m 38s paused time
- Four Commit Journal entries
- Four Wellness Journal breaks
- One voice memo
- 2 silver earned
- 200 XP earned
- Level 1 to Level 2 progression

The reward calculation was internally consistent:

- Base completion: 1 silver + 100 XP
- Four 15-minute work blocks: 25 copper + 25 XP each
- Work-block total: 1 silver + 100 XP
- Final Quest reward: 2 silver + 200 XP
- Existing 10-copper purse produced the reported 2 silver 10 copper ending balance

The Chronicle also exposed a blank Focus Stage title and a zero-value Stage Completion entry, which is addressed in the Encounter fallback work below.

---

# Patched Issues

## DQ-0306-009 — Editor audio transport becomes uncontrollable

**Type:** Audio / Editor integration
**Severity:** P0 Beta blocker
**Status:** Patched in 0.30.7; awaiting stress test

### Observed behavior

After previewing an AudioClip from Unity's Inspector:

- Music could no longer be paused or stopped.
- Ambience could continue looping without responding to controls.
- Warning cues stopped playing.
- Controls sometimes returned only after the current music track completed.
- Later testing degraded into an ambience loop that could not be changed or stopped.
- Music sometimes failed to resume after returning focus to Unity.

### Root behavior

Unity's Inspector preview and DeverQuest use the same internal native editor-audio transport. DeverQuest tracks Music and Ambience as separate logical channels, but Unity may stop, replace, or retain native preview clips outside DeverQuest's control.

### 0.30.7 correction

- Detect editor focus loss and restore logical playback after focus returns.
- Detect missing native playback when DeverQuest still expects an active channel.
- Rebuild Music and Ambience after Inspector-preview interruption.
- Add **Recover Audio Transport**.
- Add **Stop and Reset All Audio** as an emergency brake.
- Add direct Music track selection.
- Add direct Ambience track selection.
- Retry warning cues once after reclaiming the preview transport.
- Preserve the other logical channel when one channel is changed or stopped.

### Remaining limitation

When Unity exposes only global preview gain, Music and Ambience cannot have truly independent volume levels. A proper mixer requires a later runtime/scene audio-host architecture.

### Acceptance test

- [ ] Start Music and Ambience together.
- [ ] Preview a different AudioClip in the Inspector.
- [ ] Return to DeverQuest and verify automatic recovery.
- [ ] Stop only Music.
- [ ] Confirm Ambience continues.
- [ ] Start Music again.
- [ ] Stop only Ambience.
- [ ] Confirm Music continues.
- [ ] Trigger Warning, Victory, and Level Up cues.
- [ ] Use both track selectors while both channels are active.
- [ ] Use Recover Audio Transport.
- [ ] Use Stop and Reset All Audio.
- [ ] Restart Unity and repeat.
- [ ] Confirm no third or abandoned loop remains audible.

---

## DQ-0306-010 — Sole Guild founder becomes Member

**Type:** Authority / persistence
**Severity:** P0 Beta blocker
**Status:** Patched in 0.30.7; awaiting restart verification

### Observed behavior

EchoDev was the only created account, but the Character Sheet and Timecard reported:

`Guild Rank: Member`

This prevented CEO/Boss-only actions such as regenerating studio content.

### Root cause

Guild authority was stored on the account, but character synchronization could copy a stale `Member` rank from the RPG Adventurer back into the authoritative Guild account.

### 0.30.7 correction

- When exactly one active Guild account exists, it is repaired as CEO.
- The only active account is selected as the founder.
- Character-sheet synchronization no longer overwrites Guild authority.
- The Character Sheet receives the account's current Guild rank.
- Release Readiness checks sole-founder authority.

### Acceptance test

- [ ] Install 0.30.7 and restart Unity.
- [ ] Confirm EchoDev displays as CEO.
- [ ] Run Release Readiness.
- [ ] Confirm Guild authority passes.
- [ ] Complete a Quest.
- [ ] Restart Unity.
- [ ] Confirm the account remains CEO.
- [ ] Confirm CEO/Boss content tools are enabled.

---

## DQ-0306-011 — Automatic Git monitor freezes Unity

**Type:** Git integration / performance
**Severity:** P0 Beta blocker
**Status:** Patched in 0.30.7; awaiting repository stress test

### Observed behavior

Unity displayed a long-running busy dialog for:

`EditorApplication.update: EchoDevGames.DeverQuest.DeverQuestGitMonitor.Update`

The problem occurred while Git operations were already slow or blocked.

### 0.30.7 correction

- Automatic status observation now runs on a background task.
- The Unity main update loop only polls for a completed result.
- Git still retains its 30-second command timeout.
- Commit detection is processed on Unity's main thread only after the background result is ready.
- Release Readiness warns when the timecard folder is inside the repository but is not explicitly ignored.

### Related repository risk

The timecard root contains Chronicles and voice memos. When it lives inside the Git repository and is not ignored, commits and pushes may become unexpectedly large.

Recommended root `.gitignore` entry:

```gitignore
/DeverQuestTimecards/
```

### Acceptance test

- [ ] Start a Quest.
- [ ] Lock the repository with another Git operation.
- [ ] Wait through at least two automatic monitor intervals.
- [ ] Confirm Unity remains responsive.
- [ ] Commit through GitHub Desktop.
- [ ] Confirm DeverQuest detects the new commit afterward.
- [ ] Confirm the Commit Journal receives one entry, not duplicates.
- [ ] Confirm timecards and voice memos are ignored or stored outside the repository.

---

## DQ-0306-012 — Quest acceptance and Party waiting are unclear

**Type:** Quest UX
**Severity:** P1
**Status:** Patched in 0.30.7; awaiting workflow test

### Observed behavior

- A final Beta Quest could not be accepted, but the reason was not clear.
- Joining a group Quest sent the user back to Guild Hall without a persistent waiting indicator.
- No way to withdraw before the party assembled was visible.

### 0.30.7 correction

- A disabled **Accept Quest** button now displays the exact blocking reason.
- Party Quest cards show enlisted/waiting status and current capacity.
- A waiting Adventurer may select **Leave Party** before the Quest starts.
- Selected Contracts show compact base and work-block rewards directly on the board.
- Quest Story appears beneath the selected Contract.

### Acceptance test

- [ ] Select a Draft Contract as a Member and read the blocking reason.
- [ ] Select a Contract missing Project or Task data and read the reason.
- [ ] Select an ineligible Contract and read the reason.
- [ ] Join an incomplete Party Quest.
- [ ] Confirm the waiting notice remains visible.
- [ ] Leave the party.
- [ ] Confirm the roster and status update.
- [ ] Rejoin and fill the party.
- [ ] Confirm the Quest can begin.

---

## DQ-0306-013 — Existing founder cannot customize the first character

**Type:** Character onboarding
**Severity:** P1
**Status:** Patched in 0.30.7; awaiting test

### Observed behavior

The starter Identity Catalog generated successfully, but the existing founder already had a base character and could not find a path to create or customize the intended Adventurer.

### 0.30.7 correction

- CEO/Boss users receive **Customize Current Adventurer Identity…** on the Character Sheet.
- Reopening character creation preserves:
  - Level
  - XP
  - Coin
  - Inventory
  - Equipment
  - Companions
  - Ledgers
  - Chronicle history
- Newly completed characters receive a minimum starting purse of five silver.
- A fresh account with no chosen character is routed through character creation.

### Boundary

Multiple character slots per Guild account are not included in 0.30.7. They remain a medium-priority post-Beta architecture item.

### Acceptance test

- [ ] Confirm EchoDev is CEO.
- [ ] Select Customize Current Adventurer Identity.
- [ ] Choose a custom name, Ancestry, Class, Alignment, and Faith.
- [ ] Complete onboarding.
- [ ] Confirm the purse is at least five silver.
- [ ] Confirm prior XP and Chronicle history remain.
- [ ] Restart Unity and confirm persistence.

---

## DQ-0306-014 — Approved Break duration is not visible

**Type:** Wellness UX
**Severity:** P1
**Status:** Patched in 0.30.7; awaiting reminder test

### Observed behavior

The UI warned that less than 80% of the break would not qualify, but did not clearly show the recommended duration or minimum qualifying duration.

### 0.30.7 correction

- Reminder panel shows recommended break minutes.
- Reminder panel shows minimum qualifying minutes.
- Active Approved Break panel shows:
  - Planned duration
  - Minimum duration for benefit
  - Permit time remaining

### Acceptance test

- [ ] Trigger each configured wellness reminder.
- [ ] Confirm planned and minimum times are visible.
- [ ] End one break below 80%.
- [ ] Confirm no benefit is awarded.
- [ ] Complete one break at or above 80%.
- [ ] Confirm the configured benefit is awarded.

---

## DQ-0306-015 — Quest Story and Encounter progress are underreported

**Type:** Quest narrative / reporting
**Severity:** P1
**Status:** Patched in 0.30.7; awaiting staged-Quest test

### Observed behavior

- Quest Story was stored on the Contract but not visible during the active Quest.
- “Staged Contract” terminology was unclear.
- The Timecard contained a blank Focus Stage title.
- Current progress feedback was generic pacing text rather than Quest-specific narrative.

### Clarification

A staged Contract is a Contract with one or more Focus Stage records. In 0.30.7 those stages are presented to the user as **Encounters**.

### 0.30.7 correction

- Show Quest Story while selecting a Contract.
- Show Quest Story during the active Quest.
- Present the current stage as **Current Encounter**.
- Use `Encounter 1`, `Encounter 2`, and so forth when titles are blank.
- Write the section as **Encounters** in the Timecard.
- Rename **Suggested Focus** to **Predicted Task Length** in user-facing areas.

### Boundary

Generated pacing lines remain factual timer feedback. Full mad-lib narrative built from Biomes, Rooms, enemies, hazards, and combat events is reserved for the 2.0 Quest World expansion.

---

# Open and Deferred Issues

## DQ-0306-016 — True independent audio volume

**Priority:** Medium
**Status:** Deferred

Unity 6000.3 may expose only global editor-preview gain. Independent playback controls are possible through the logical bridge, but independent channel volumes require a runtime audio host, hidden scene service, or another supported mixer path.

---

## DQ-0306-017 — Multiple characters per account

**Priority:** Medium
**Status:** Deferred

0.30.7 supports rebuilding the current character identity. Multiple saved character slots and Guild Hall switching require a new account-to-character roster model and save migration.

---

## DQ-0306-018 — Display names appear without spaces

**Priority:** Medium investigation
**Status:** Not reproduced in source review

Quest Profile and Contract string fields currently trim leading and trailing whitespace but do not remove internal spaces. Asset filenames and stable IDs may use condensed names independently from user-facing display text.

A future report should identify the exact field, typed value, saved value, and displayed Quest Board value.

---

## DQ-0306-019 — Contract reward snapshot fields are confusing

**Priority:** Medium UX
**Status:** Open

The fields are intentionally frozen copies of the linked Quest Profile reward values. They protect accepted or active Contracts from silent reward changes. The Inspector now labels the section as:

`Contract Reward Snapshot (Copied from Quest Profile)`

A future custom inspector should hide or lock these fields based on Contract status and offer a clearer refresh workflow.

---

## DQ-0306-020 — Completed Quest history is not a dedicated Quest Log

**Priority:** Medium
**Status:** Open

Rewards & History contains Chronicle records, but the Quest Log workspace does not yet provide a collapsible, narrative completed-Quest archive.

---

## DQ-0306-021 — Separate Quest, Git, and administration workspaces

**Priority:** Medium
**Status:** Open

The current combined Quest Log & Git workspace remains dense. A later UI pass should separate:

- Active Quest Log
- Completed Quest Archive
- Git
- Content/Mod tools
- Visual Profiles

---

# Closed or Verified in the Source Test

- Repository legacy naming was removed.
- Root README and documentation placement passed.
- Credits and third-party notices passed.
- Starter Identity Catalog and Registry generated with valid assets.
- Ambience Profile creation, assignment, and persistence passed.
- Main Quest progress, percentage, target, and remaining time passed.
- Pause and Resume passed.
- Five-minute and one-hour Quest rewards were awarded and recorded consistently.
- Active Contract reward snapshots correctly warned against silent changes.
- Chronicle generation, Commit Journal, Wellness Journal, and voice memo attachment completed.
- No data loss was reported.

---

# 0.30.7 Retest Verdict

- [ ] PASS
- [ ] CONDITIONAL PASS
- [ ] FAIL

**Required before PASS:** audio stress test, founder authority persistence, Git responsiveness, character customization, Party withdrawal, break timing, and Encounter reporting.
