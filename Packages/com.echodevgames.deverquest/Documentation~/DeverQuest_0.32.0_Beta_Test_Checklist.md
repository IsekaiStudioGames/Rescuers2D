# DeverQuest 0.32.0 Focused Beta Checklist
## Quest 8 — The Quiet Shrine and the Retired Notice

**Status:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

---

# A. Installation

- [ ] Install `com.echodevgames.deverquest-0.32.0.tgz`.
- [ ] Confirm Package Manager reports 0.32.0.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Record the new pass/advisory/blocker totals.

---

# B. Completed Board Listing

Create or use a Single Completion Contract.

- [ ] Confirm it appears while Offered.
- [ ] Accept it.
- [ ] Complete it.
- [ ] Confirm its status becomes Completed.
- [ ] Open Guild Hall as CEO.
- [ ] Confirm it no longer appears on the live Guild Assignment Board.
- [ ] Open Chronicle.
- [ ] Confirm the completed Quest remains available.
- [ ] Open Completed Quest Run Archive.
- [ ] Confirm its completion record remains available.
- [ ] Select its source Contract from archive navigation.
- [ ] Confirm no history was deleted.

Create or use a Limited Completions Contract at its target.

- [ ] Confirm it leaves the Board when the target is reached.
- [ ] Confirm it remains in history.

Create or use a Repeatable Contract.

- [ ] Complete it once.
- [ ] Confirm it returns to Offered.
- [ ] Confirm it remains on the Board.

---

# C. Meditation Recovery

Prepare an Adventurer below maximum Health and Mana.

- [ ] Record starting HP.
- [ ] Record starting Mana.
- [ ] Start a Quest.
- [ ] Select Meditate.
- [ ] Confirm Current Quest shows the recovery rate.
- [ ] Confirm Quest HUD shows the recovery rate.
- [ ] Wait less than one full minute.
- [ ] Confirm preview remains +0 HP and +0 Mana.
- [ ] Continue to two full minutes.
- [ ] Confirm preview is up to +2 HP and +4 Mana.
- [ ] Resume.
- [ ] Confirm recovery applies exactly once.
- [ ] Meditate again at full Health and Mana.
- [ ] Confirm no extra values exceed maximums.
- [ ] Trigger an Approved Break.
- [ ] Confirm it does not grant Meditation recovery.
- [ ] Trigger idle auto-pause.
- [ ] Confirm it does not grant Meditation recovery.
- [ ] Confirm a Fallen Adventurer is not revived.
- [ ] Complete the Quest.
- [ ] Confirm Timecard includes Meditation Recovery totals.

---

# D. Duplicate Quest Profile ID

Open Beta Administration and run validation.

- [ ] Find both DQ-CONTENT-101 entries.
- [ ] Preserve `01_FiveMinuteChallengeTask.asset`.
- [ ] On `02_OneHourChallengeTask.asset`, click Regenerate This Asset ID.
- [ ] Confirm the warning dialog names the selected asset.
- [ ] Confirm only that asset receives a new ID.
- [ ] Rerun validation.
- [ ] Confirm both DQ-CONTENT-101 errors disappear.
- [ ] Confirm Contracts linked to either profile still resolve.
- [ ] Confirm existing historical records remain readable.

---

# E. Duplicate Faith ID

- [ ] Find both DQ-CONTENT-301 entries.
- [ ] Preserve `Agnostic.asset`.
- [ ] Determine whether `Agnostic 1.asset` is referenced.
- [ ] Regenerate its ID, or delete it when it is truly unused.
- [ ] Rerun validation.
- [ ] Confirm both DQ-CONTENT-301 errors disappear.
- [ ] Confirm the active Identity Catalog remains playable.
- [ ] Confirm the current character's Faith remains valid.
- [ ] Restart Unity and verify persistence.

---

# F. Remaining Advisory Cleanup

- [ ] Generate or assign Tactical test content.
- [ ] Open Inventory and Equipment.
- [ ] Run Repair Equipped Inventory Records.
- [ ] Resolve the remaining equipment entry.
- [ ] Confirm equipped items are represented in inventory.
- [ ] Add clips to the two empty playlists or remove unused playlist assets.
- [ ] Complete or abandon the active Quest.
- [ ] Rerun Full Validation.
- [ ] Rerun Release Readiness.

---

# Verdict

- [ ] **PASS** — Board behavior, Meditation, and duplicate-ID repair pass; no content blocker remains.
- [ ] **CONDITIONAL PASS** — code behavior passes; documented non-blocking content advisories remain.
- [ ] **FAIL** — completed listings remain live, recovery duplicates or exceeds caps, or ID repair breaks references.
