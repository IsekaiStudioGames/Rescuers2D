# DeverQuest 0.31.8 Beta Test Checklist
## Quest 8 — The Auditor's Lantern

**Build:** 0.31.8 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

This checklist focuses on Beta Administration and content validation. Earlier gameplay and multi-account matrices remain deferred separately.

---

# A. Installation and Readiness

- [ ] Install `com.echodevgames.deverquest-0.31.8.tgz`.
- [ ] Confirm Package Manager reports 0.31.8.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm **Beta content health** appears.
- [ ] Record errors, warnings, notes, and scanned-asset count.
- [ ] Confirm an advisory does not block normal Quest use.
- [ ] Confirm an error produces a blocker.

---

# B. Administration Workspace

Open:

```text
Tools > DeverQuest > Workspaces > Beta Administration
```

- [ ] Confirm the workspace opens without Console errors.
- [ ] Confirm validation remains available to a Member.
- [ ] Confirm repair and generator controls require CEO or Boss permission.
- [ ] Confirm the workspace remains readable in narrow and wide docks.
- [ ] Confirm it appears in compact and full workspace labels.

---

# C. Full Validation

- [ ] Click **Run Full Validation**.
- [ ] Confirm scanned-asset total is greater than zero.
- [ ] Confirm the summary matches visible findings.
- [ ] Confirm errors sort above warnings and notes in exports.
- [ ] Search by finding code.
- [ ] Search by asset name.
- [ ] Search by asset path.
- [ ] Filter Errors.
- [ ] Filter Warnings.
- [ ] Filter Notes.
- [ ] Clear filters and confirm all findings return.
- [ ] Click Select Asset on several finding types.
- [ ] Confirm the correct Project asset is selected and pinged.
- [ ] Copy an asset path and verify it.

---

# D. Quest Profile and Contract Validation

In a disposable QA folder:

- [ ] Create a Quest Profile with no Project Name.
- [ ] Confirm an incomplete-profile warning.
- [ ] Create a Contract linked to that profile.
- [ ] Clear Contract title, Project, Task, and Objective.
- [ ] Confirm repairable Contract warnings.
- [ ] Change the profile reward while the Contract is Draft.
- [ ] Confirm a refreshable reward-snapshot warning.
- [ ] Lock a Contract and change its profile reward.
- [ ] Confirm the difference is informational, not auto-repairable.
- [ ] Add a null Encounter list element.
- [ ] Confirm a repairable warning.
- [ ] Store an Encounter ID without an asset reference.
- [ ] Confirm an unresolved-reference warning.
- [ ] Archive a Contract with an active reservation in a disposable test.
- [ ] Confirm an error.

---

# E. Stable-ID Integrity

Use disposable copies only.

- [ ] Duplicate a Quest Profile asset while preserving serialized ID.
- [ ] Confirm duplicate Profile ID errors identify both assets.
- [ ] Duplicate a Contract asset.
- [ ] Confirm duplicate Contract ID errors.
- [ ] Duplicate an Identity asset.
- [ ] Confirm duplicate Identity ID errors.
- [ ] Duplicate a Shop Item.
- [ ] Confirm duplicate Shop Item ID errors.
- [ ] Remove the test copies.
- [ ] Rerun validation and confirm every duplicate-ID error clears.
- [ ] Confirm safe repair never generates new IDs to conceal a collision.

---

# F. Identity and Companion Catalogs

- [ ] Validate the active Original Identity Catalog.
- [ ] Confirm no playable-catalog error.
- [ ] Clear a default in a disposable Catalog.
- [ ] Confirm a safe-repair warning.
- [ ] Add a null Ancestry, Class, or Faith entry.
- [ ] Confirm a safe-repair warning.
- [ ] Create an empty Identity Catalog.
- [ ] Confirm the non-playable Catalog error.
- [ ] Add a null Companion reference to a disposable Companion Catalog.
- [ ] Confirm a safe-repair warning.
- [ ] Create an empty Companion Catalog.
- [ ] Confirm an empty-Catalog warning.

---

# G. Encounter and Economy Validation

- [ ] Create an Encounter with no waves.
- [ ] Confirm a warning.
- [ ] Add a wave with no Monster.
- [ ] Confirm an error.
- [ ] Create an empty Shop Profile.
- [ ] Confirm a warning.
- [ ] Add a missing/null Shop Item reference.
- [ ] Confirm a safe-repair warning.
- [ ] Create an Equipment Shop Item without Equipment.
- [ ] Confirm a warning.
- [ ] Create a Quest-protected item and force unsafe trade/drop settings through serialized editing in a disposable test.
- [ ] Confirm an error.

---

# H. Audio and Loadout Validation

- [ ] Create an empty Playlist.
- [ ] Confirm a warning.
- [ ] Add a missing AudioClip reference.
- [ ] Confirm a warning.
- [ ] Create an empty Ambience Profile.
- [ ] Confirm a warning.
- [ ] Add a missing Ambience clip reference.
- [ ] Confirm a warning.
- [ ] Create an empty Starter Loadout.
- [ ] Confirm it is a note rather than an error.

---

# I. Safe Repair

- [ ] Prepare only repairable findings.
- [ ] Record asset contents before repair.
- [ ] Click **Repair Safe Issues**.
- [ ] Confirm the confirmation dialog explains the boundaries.
- [ ] Confirm null entries are removed.
- [ ] Confirm Catalog defaults are restored.
- [ ] Confirm blank linked Contract fields are copied from the profile.
- [ ] Confirm editable reward snapshots refresh.
- [ ] Confirm locked snapshots do not change.
- [ ] Confirm valid assets are not deleted.
- [ ] Confirm completion history does not change.
- [ ] Confirm duplicate IDs remain visible for manual resolution.
- [ ] Restart Unity and confirm repairs persist.

---

# J. Starter Generator Repair

- [ ] Back up or commit the project first.
- [ ] Click **Rerun Safe Starter Generators**.
- [ ] Cancel once and confirm nothing changes.
- [ ] Run it again and confirm it executes after the GUI event.
- [ ] Confirm Identity assets are preserved or repaired.
- [ ] Confirm Companion assets are preserved or repaired.
- [ ] Confirm Tactical assets are preserved or updated.
- [ ] Confirm Combat Codex remains valid.
- [ ] Confirm Starter Gear does not duplicate.
- [ ] Confirm Quartermaster stock does not duplicate.
- [ ] Confirm Training Encounter remains valid.
- [ ] Run the generator a second time.
- [ ] Confirm the second run remains idempotent.
- [ ] Run Full Validation again.

---

# K. Report Export

- [ ] Export Markdown.
- [ ] Confirm it appears under `DeverQuestBetaReports/`.
- [ ] Export JSON.
- [ ] Confirm it appears under the same folder.
- [ ] Confirm filenames include timestamps.
- [ ] Compare error, warning, and note counts.
- [ ] Confirm finding codes match.
- [ ] Confirm asset paths match.
- [ ] Confirm repairability flags match.
- [ ] Commit one report as QA evidence or keep it outside Git according to project policy.

---

# L. Safety and Regression

- [ ] Open Administration repeatedly.
- [ ] Run validation repeatedly.
- [ ] Confirm no Quest starts.
- [ ] Confirm no focused time is added.
- [ ] Confirm no rewards are granted.
- [ ] Confirm no Contract completes.
- [ ] Confirm no Encounter resolves.
- [ ] Confirm validation does not modify assets.
- [ ] Confirm export does not modify assets.
- [ ] Confirm only explicit Repair or Generator actions modify content.
- [ ] Confirm Current Quest, Chronicle, Git, Inventory, Economy, Tactics, Audio, and Wellness still open.

---

# Verdict

- [ ] **PASS** — validation, safe repair, generators, exports, and readiness integration work correctly.
- [ ] **CONDITIONAL PASS** — tooling works; documented content warnings remain intentionally open.
- [ ] **FAIL** — validation misses broken references, modifies data while scanning, or repairs destructive/ambiguous findings automatically.
