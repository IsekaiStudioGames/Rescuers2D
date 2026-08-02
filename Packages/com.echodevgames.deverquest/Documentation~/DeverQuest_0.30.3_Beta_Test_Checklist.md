# DeverQuest 0.30.3 Beta Stabilization Checklist

Record PASS, FAIL, or BLOCKED beside each test. Capture Console errors, asset
paths, screenshots, and exact reproduction steps for failures.

## Installation and repository

- [ ] Install 0.30.3 from the tarball.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm the report identifies package version 0.30.3.
- [ ] Confirm the project path contains no prohibited legacy name.
- [ ] Place `README.md`, `Documentation/`, `CREDITS.md`, and
      `THIRD_PARTY_NOTICES.md` at the Git root.
- [ ] Confirm no third-party audio file exists inside the package.

## DQ-0302-002 — Ambience

- [ ] Create an Ambience Profile from the Audio workspace.
- [ ] Confirm the new profile is immediately assigned.
- [ ] Add one AudioClip and confirm the playable count becomes 1.
- [ ] Clear the field and reassign by drag-and-drop.
- [ ] Select the profile in Project and use **Use Selected Ambience Profile**.
- [ ] Restart Unity and confirm the assignment persists.
- [ ] Play, stop, and advance ambience.
- [ ] Switch repeatedly between playlist and ambience.
- [ ] Trigger a warning cue and confirm no overlapping long-form audio remains.
- [ ] Assign an empty profile and confirm it fails safely with guidance.

## DQ-0302-003 — Spoils

- [ ] Create a Quest Profile with clearly non-default coin and XP values.
- [ ] Create a Contract from that profile.
- [ ] Confirm Effective Contract Spoils match the profile.
- [ ] Change the profile while the Contract is Draft.
- [ ] Select the Contract and confirm its refreshable snapshot updates.
- [ ] Start the Quest and confirm the active Spoils estimate uses those values.
- [ ] Complete one reward block and confirm the estimate increases correctly.
- [ ] Complete the Quest and compare preview, award, Chronicle, and wallet.
- [ ] Confirm an Active or otherwise locked Contract warns instead of silently
      changing its snapshot.
- [ ] Run Release Readiness and confirm no refreshable mismatch remains.

## DQ-0302-004 — Identity Catalog

- [ ] Use a clean project with no Identity Catalog.
- [ ] Click Generate Original Starter Identity Catalog once.
- [ ] Confirm the button prevents duplicate queued generation.
- [ ] Confirm Ancestry, Class, Faith, Catalog, and Registry assets are created.
- [ ] Confirm defaults populate the character-creation fields.
- [ ] Create a test character successfully.
- [ ] Run the generator again and confirm assets are preserved, not duplicated.
- [ ] Delete or clear one generated list, rerun, and confirm no exception occurs.
- [ ] Interrupt a test copy after partial creation, rerun, and confirm recovery.
- [ ] Confirm any failure appears as a readable UI message and Console exception.

## Main Quest progress

- [ ] Start a Quest with a target duration.
- [ ] Confirm percentage, target, and remaining time are visible.
- [ ] Confirm the panel refreshes while the Quest runs.
- [ ] Pause and confirm feedback reports that focused time is not increasing.
- [ ] Resume and confirm progress continues.
- [ ] Use a staged Contract and confirm the current Encounter is shown.
- [ ] Pass 50% and 90% thresholds and review feedback.
- [ ] Exceed the target and confirm overtime is shown without ending the Quest.
- [ ] Start a Quest without a duration and confirm the safe fallback message.

## Verdict

- [ ] PASS — all P0 tests pass and no data inconsistency is found.
- [ ] CONDITIONAL PASS — no data loss; limited documented P1 issues remain.
- [ ] FAIL — compilation, assignment, reward, identity generation, persistence,
      or data-integrity failure remains.
