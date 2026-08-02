# DeverQuest 0.31.8 Beta Issue Log
## Pathway 8 — Beta Administration and Content Validation

**Source build:** 0.31.7 Beta 1  
**Patch build:** 0.31.8 Beta 1  
**Unity target:** 2022.3 minimum  
**Primary test environment:** Unity 6000.3.8f1  
**Patch status:** Prepared, awaiting Unity verification

---

## DQ-0317-031 — Production content has no unified validation pass

**Type:** Administration / release safety  
**Severity:** P1  
**Status:** Patched in 0.31.8; awaiting verification

Before 0.31.8, Release Readiness verified runtime configuration but could not provide one detailed report covering authored ScriptableObject content. Broken references, duplicate IDs, incomplete Contracts, empty Catalogs, and unsafe item settings had to be discovered manually.

### 0.31.8 correction

A new **Beta Administration** workspace scans:

- Quest Profiles
- Quest Contracts and run history
- Identity Catalogs, Ancestries, Classes, and Faiths
- Companion Catalogs and Profiles
- Encounter and Monster Profiles
- Shop Profiles and Shop Items
- Playlists, Ambience Profiles, and Warning Profiles
- Starter Loadouts

It reports errors, warnings, notes, asset paths, and safe-repair availability.

---

## DQ-0317-032 — Stable-ID collisions are difficult to diagnose

**Type:** Data integrity  
**Severity:** P0 when present  
**Status:** Detection added in 0.31.8

The validator detects duplicate:

- Quest Profile IDs
- Contract IDs
- Identity IDs
- Companion IDs
- Encounter IDs
- Monster IDs
- Shop Item IDs
- Starter Loadout IDs
- Active Quest Run IDs
- Contract completion-record IDs

Duplicate stable IDs are Release Readiness blockers because they can make references or history ambiguous.

---

## DQ-0317-033 — Safe repairs require manual asset editing

**Type:** Administration  
**Severity:** P1  
**Status:** Patched in 0.31.8; awaiting verification

The new safe-repair pass may:

- Remove null list entries
- Restore missing Identity Catalog defaults from valid entries
- Fill blank Contract title, Project, Task, or Objective from a linked Quest Profile
- Refresh editable Contract reward snapshots
- Normalize blank Encounter titles

It does not:

- Delete valid assets
- Rewrite locked Contract rewards
- Change completed history
- Regenerate stable IDs
- Resolve ambiguous duplicate IDs automatically

---

## DQ-0317-034 — Generator reruns are scattered and easy to misuse

**Type:** Content authoring  
**Severity:** P1  
**Status:** Patched in 0.31.8; awaiting verification

Beta Administration now offers a confirmed, deferred **Rerun Safe Starter Generators** action. It runs outside the active IMGUI draw event and invokes the existing idempotent generators for:

- Original Identity Catalog
- Original Companion Stable
- Tactical Starter Kit
- Combat Codex
- Starter Gear
- Guild Quartermaster
- Training Encounter

Existing assets are preserved or updated according to each generator's existing rules.

---

## DQ-0317-035 — Beta content health cannot be archived with a test run

**Type:** QA evidence  
**Severity:** P2  
**Status:** Patched in 0.31.8; awaiting verification

The current validation result can be exported to:

- Markdown
- JSON

Reports are written under:

```text
DeverQuestBetaReports/
```

at the Unity project root. They include generation time, counts, finding codes, severity, detail, asset path, and repairability.

---

## DQ-0317-036 — Release Readiness does not summarize authored-content health

**Type:** Release Readiness  
**Severity:** P1  
**Status:** Patched in 0.31.8; awaiting verification

Release Readiness now adds **Beta content health**:

- Errors produce a blocker.
- Warnings produce an advisory.
- A clean scan produces a pass.
- A validator exception produces an advisory rather than disabling the Quest timer.

---

# Required Retest

- [ ] Install 0.31.8 and compile with zero errors.
- [ ] Open Beta Administration.
- [ ] Run Full Validation.
- [ ] Confirm every finding selects the correct asset.
- [ ] Search by code, title, detail, and path.
- [ ] Filter errors, warnings, and notes.
- [ ] Export Markdown and JSON.
- [ ] Confirm both reports describe the same findings.
- [ ] Create a safe repair case and run Repair Safe Issues.
- [ ] Confirm valid data is preserved.
- [ ] Create a duplicate-ID test copy in a disposable project.
- [ ] Confirm Release Readiness blocks it.
- [ ] Remove the duplicate and confirm the blocker clears.
- [ ] Rerun Safe Starter Generators.
- [ ] Confirm no duplicate starter assets are created.
- [ ] Restart Unity and repeat validation.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
