# DeverQuest Personal Notes
## Consolidated after 0.31.8 Beta Administration Work

**Current patch target:** 0.31.8 Beta Administration and Content Validation  
**Current product lane:** Stabilize and inspect the existing Beta product before opening new 2.0 systems.

---

# Immediate Importance

## Bulk content health

The project now needs a single answer to:

- Which assets are broken?
- Which IDs collide?
- Which Contracts are incomplete?
- Which references are missing?
- Which problems are safe to repair automatically?
- Which problems require an author to decide?

**0.31.8 status:** Beta Administration validation implemented.

## Safe repair boundary

Automatic repair should only handle unambiguous cleanup:

- Null list entries
- Missing defaults when a valid first entry exists
- Blank Contract fields from a linked Profile
- Editable reward-snapshot refresh
- Empty Encounter titles

It must never silently:

- Replace duplicate IDs
- Rewrite locked rewards
- Delete valid assets
- Alter completed Quest history
- Invent missing authored content

**0.31.8 status:** guarded repair pass implemented.

## Generator rerun safety

Starter generators should be rerunnable after partial creation or damage without duplicate output or IMGUI layout failures.

**0.31.8 status:** centralized deferred rerun action implemented using existing idempotent generators.

## Exportable Beta health evidence

Each serious Beta pass should be able to retain a machine-readable and human-readable content snapshot.

**0.31.8 status:** Markdown and JSON exports implemented under `DeverQuestBetaReports/`.

---

# Medium Importance

## Validation rules as data

Future improvement:

- Rule assets or rule registry
- Enable/disable individual checks
- Project-specific severity overrides
- Suppression with reason and expiration
- Custom studio checks
- Package-defined versus project-defined rules

Do not build this before real false-positive patterns emerge.

## Validation baselines

Potential workflow:

- Save accepted warning baseline
- Compare current scan to previous scan
- Show new, resolved, and unchanged findings
- Fail CI only on newly introduced blockers

## Command-line and CI validation

Future batch-mode entry point:

```text
-executeMethod EchoDevGames.DeverQuest.DeberQuestContentValidationService...
```

The exact public API should be designed carefully and write a deterministic exit code and report path.

## Custom inspectors

Several confusing fields would benefit from purpose-built inspectors:

- Contract reward snapshots
- Completion history
- Run reservations
- Quest-protected item permissions
- Identity Catalog defaults
- Encounter waves

Validation catches bad state; custom inspectors should make bad state harder to create.

## Generator preview

Before rerunning a generator, show:

- Assets to create
- Assets to update
- Assets to preserve
- References to repair
- No-op results

This requires a dry-run model in each generator.

---

# Low Importance

## Validation presentation polish

- Category icons
- Group findings by asset type
- Group by folder
- Copy finding details
- Open containing folder
- Multi-select findings
- Per-finding repair button
- Progress bar for very large projects

## Report naming policy

Possible configurable report naming:

- Version
- Git branch
- Commit hash
- Tester
- Build label
- Readiness summary

---

# Expansion 2.0

## Quest World validation

Future Room, Biome, hazard, NPC, merchant, chest, altar, and crafting-station data will need validation for:

- Graph reachability
- Time-estimate consistency
- Encounter composition
- Reward balance
- Hazard counterplay
- Narrative fragments
- Loot tables
- Environment references

## Crafting validation

Future checks:

- Recipe cycles
- Missing ingredients
- Unreachable materials
- Station requirements
- Skill requirements
- Salvage loops
- Economy exploits

## Housing and banking validation

Future checks:

- Storage ownership
- Capacity
- Duplicate item identity
- Transfer safety
- Shared versus character-bound storage

---

# Completed

- Release repository preparation
- Founder authority
- Identity Catalog generation and registry repair
- Reward snapshot consistency
- Repeatable Contract architecture
- Quest Run management
- Tactical visibility and operations
- Inventory and equipment clarity
- Guild Economy ledger
- Chronicle navigation
- Editor workspace organization and Quest HUD
- Supported audio host and mixer
- Wellness Command Center
- Beta Administration workspace
- Bulk content validation
- Duplicate-ID diagnostics
- Safe repair pass
- Safe starter-generator rerun
- Markdown and JSON health exports
- Release Readiness content-health integration

---

# Current Decision

After 0.31.8 receives a smoke test, the strongest next pathway is **0.31.9: Beta Packaging and Distribution Center**:

- Build/package manifest
- Distribution preflight
- License and media audit
- Version and changelog verification
- Tarball integrity
- Exportable release dossier
- Final known-limitations register

This moves DeverQuest toward shipment without opening crafting, banking, housing, or broad Quest World simulation.
