# DeverQuest 0.32.0 Beta Issue Log
## Board Cleanup, Meditation Recovery, and Content-ID Repair

**Source build:** 0.31.9 Beta 1  
**Patch build:** 0.32.0 Beta 1  
**Unity:** 6000.3.8f1  
**Readiness baseline:** 21 passed, 3 advisories, 1 blocker  
**Patch status:** Prepared, awaiting Unity verification

---

## DQ-0319-031 — Completed Contract remains on the live Guild Board

**Type:** Quest Board lifecycle  
**Severity:** P1  
**Status:** Patched in 0.32.0

### Observed

After a one-time Quest completed, the Contract remained visible on the Guild Assignment Board as `Completed`. Administrators could still select it from the live Board.

### Expected

A completed one-time Contract, or a limited Contract that reached its completion target, should leave the live Guild Board for every account.

Its history must remain available through:

- Chronicle
- Completed Quest Run Archive
- Quest Run Management
- Direct Contract asset inspection
- Beta Administration

### 0.32.0 behavior

The live Guild Assignment Board now hides:

- Archived Contracts
- Completed one-time Contracts
- Completed limited Contracts whose target has been reached

Repeatable Contracts continue returning to `Offered`.

---

## DQ-0319-032 — Meditation does not recover Health or Mana

**Type:** Quest state / RPG progression  
**Severity:** P1  
**Status:** Patched in 0.32.0

### 0.32.0 rule

Manual Meditation restores, when the Quest resumes:

- 1 Hit Point per completed minute
- 2 Mana per completed minute

Recovery:

- Uses full completed minutes only
- Is capped at maximum Health and Mana
- Does not revive a Fallen Adventurer
- Does not apply to Approved Break, idle detection, focus loss, or combat-safety pauses
- Is shown before resuming in Current Quest and Quest HUD
- Is summarized in the generated Timecard

---

## DQ-0319-033 — Duplicate stable IDs block Beta content health

**Type:** Content integrity  
**Severity:** P0 release blocker  
**Status:** Explicit repair workflow added in 0.32.0

### Reported duplicate Quest Profile ID

The following assets share one Profile ID:

- `Assets/_Data/Quests/QuestTasks/01_FiveMinuteChallengeTask.asset`
- `Assets/_Data/Quests/QuestTasks/02_OneHourChallengeTask.asset`

Recommended repair:

- Preserve the ID on `01_FiveMinuteChallengeTask.asset`.
- Regenerate the ID on `02_OneHourChallengeTask.asset`, assuming it was copied from the first asset.

### Reported duplicate Identity ID

The following Faith assets share one Identity ID:

- `Assets/DeverQuest/IdentityCatalogs/OriginalStarter/Faiths/Agnostic.asset`
- `Assets/DeverQuest/IdentityCatalogs/OriginalStarter/Faiths/Agnostic 1.asset`

Recommended repair:

- Preserve the ID on `Agnostic.asset`.
- Regenerate the ID on `Agnostic 1.asset`, or delete the duplicate asset when it is not referenced.

### 0.32.0 repair workflow

Open:

`Beta Administration > Run Full Validation`

For the copied/newer asset in each duplicate group, click:

`Regenerate This Asset ID`

The action:

- Requires CEO or Boss permission
- Requires confirmation
- Changes only the selected asset
- Keeps Unity object references intact
- Does not rewrite ambiguous historical IDs
- Reruns validation after repair

Do not regenerate every asset in one duplicate group.

---

## Remaining readiness advisories

### Tactical test content

Generate or assign:

- Encounter Profile
- Companion Profile
- Spell or Attack Technique

### Inventory integrity

Open Inventory and Equipment, then:

- Repair equipped inventory records
- Resolve the unresolved equipment entry
- Confirm both equipped items have ownership records

### Empty playlists

The following playlists are empty:

- `DeverQuestPlaylist.asset`
- `Garfield - Caught In The Act.asset`

Add clips, archive/delete unused assets, or leave the warnings documented for internal testing.

### Active Quest state

Complete or abandon the active Quest before clean migration or install regression.

### Hand-authored tactical Contracts

`FifteenMinuteSkirmishQuest.asset` and `WayfarerSurvivalQuest.asset` have no linked Quest Profile. This is informational and valid when intentionally hand-authored.

---

## Required verification

- [ ] Install 0.32.0 with zero compilation errors.
- [ ] Complete a one-time Quest.
- [ ] Confirm it disappears from the live Guild Board for CEO.
- [ ] Confirm it remains in Chronicle and Completion History.
- [ ] Damage the Adventurer and spend Mana.
- [ ] Meditate for at least two full minutes.
- [ ] Confirm preview reports +2 HP and +4 Mana, capped as necessary.
- [ ] Resume and confirm the values are applied once.
- [ ] Confirm Timecard reports Meditation Recovery.
- [ ] Regenerate the ID on `02_OneHourChallengeTask.asset`.
- [ ] Regenerate or remove `Agnostic 1.asset`.
- [ ] Rerun Full Validation.
- [ ] Confirm the four duplicate-ID errors clear.
- [ ] Run Release Readiness again.

---

## Current verdict

**PATCH PREPARED — BETA CONTENT BLOCKER REQUIRES PROJECT-ASSET REPAIR**
