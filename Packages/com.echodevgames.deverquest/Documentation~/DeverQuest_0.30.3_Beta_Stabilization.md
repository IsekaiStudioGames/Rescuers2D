# DeverQuest 0.30.3 Beta 1 Stabilization

## Scope

This milestone deliberately contains only release stabilization work:

1. Ambience Profile assignment
2. Quest and Contract Spoils consistency
3. Original starter Identity Catalog generation
4. Main Quest progress feedback
5. Repository and credits preparation

No crafting, banking, housing, biome, skill, room-generation, or broad visual
profile system is introduced in this build.

## Fixed issues

### DQ-0302-002 — Ambience Profile assignment

- Creating an Ambience Profile from the DeverQuest window now assigns it
  immediately.
- A selected Ambience Profile can be installed with **Use Selected Ambience
  Profile** when drag-and-drop is inconvenient.
- The field uses an explicit change check and persists the selected asset.
- Empty profiles display a clear warning and playable-clip count.

### DQ-0302-003 — Incorrect Spoils preview

- The Quest form now displays effective Contract Spoils when a Contract is
  selected.
- Editable Contract snapshots refresh from their linked Quest Profile for an
  authorized manager.
- Locked or intentionally divergent snapshots show both values and explain that
  the Contract values are the values that will be awarded.
- The active Quest panel calculates its Spoils estimate from the snapshotted
  session values.

### DQ-0302-004 — Starter Identity Catalog generation

- Generation is deferred until after the current Editor GUI event.
- Repeated clicks are blocked while generation is queued.
- Existing partial assets have missing collections repaired.
- The generator is safe to run again and preserves existing assets.
- The unnecessary `AssetDatabase.Refresh()` call was removed.
- Errors are caught, logged, and reported in the character screen instead of
  leaving the UI in an unknown state.

## Main Quest progress panel

The main Quest workspace now includes:

- Target duration
- Percentage progress
- Time remaining or time beyond target
- Current Encounter and stage count
- Factual pacing feedback
- Current projected coin and XP

The timer continues recording valid focused work after the target duration.
Overtime is feedback, not a failure state.

## Repository preparation

The package includes `CREDITS.md` and `THIRD_PARTY_NOTICES.md`. The Release
Readiness service now checks the project-root path for prohibited legacy naming
and advises when root README, Documentation, credits, or notices are absent.

The package itself contains no third-party media files.
