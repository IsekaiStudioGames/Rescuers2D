# DeverQuest Personal Notes
## Consolidated for 0.31.1 Tactical Operations

**Previous build:** 0.31.0 Beta 1  
**Current build:** 0.31.1 Beta 1  
**Deferred regression:** Repeatable Contracts, multi-account Quest Runs, full Combat/Survival effect matrix, and large tactical archive stress

---

# Immediate Importance

## Dedicated Tactics workspace

The tactical systems now have a central operational home containing:

- Adventurer readiness
- Current Encounter
- Survival milestones
- Companion selection and recovery
- Active/latest Battle Reports
- Local Battle Archive

**0.31.1:** implemented, awaiting Unity verification.

## Persistent Battle Archive

New Battle Results should remain searchable after a later Quest replaces the last-completed Session.

Requirements:

- Automatic recording
- No duplicate imports
- Search and outcome filters
- Report and JSON evidence
- Encounter Profile navigation
- Safe local deletion
- Hard cap to prevent unbounded local archive growth

**0.31.1:** newest 100 local records implemented.

## Companion operations

Companion state should be manageable without moving between multiple distant panels.

Requirements:

- Quick active selection
- Send to Stable
- Individual recovery
- Combined roster recovery
- Up-front total cost validation
- No partial recovery on insufficient funds

**0.31.1:** implemented.

## Tactical storage readiness

Release Readiness should verify the local archive can write and delete its data.

**0.31.1:** implemented.

---

# Medium Importance

## Structured combat event model

Current summaries still derive some metrics from Battle Result lists and human-readable combat lines.

A future typed event model should represent:

- Attack attempted
- Hit
- Miss
- Critical result
- Raw damage
- Final damage
- Shield absorption
- Resistance reaction
- Condition applied
- Condition resisted
- Condition removed
- Heal
- Companion protection
- Killing blow
- Escape attempt
- Loot award

This should eventually drive statistics, tactical history, and procedural Chronicle text.

## Shared Guild Battle Archive

0.31.1 stores the archive locally under `Library/DeverQuest/`.

Future shared structure:

```text
SharedGuild/
└── TacticalReports/
    └── ContractId/
        └── RunId/
            ├── Battle-001.json
            └── Battle-002.json
```

Requirements:

- Append-only records
- No Unity asset merge conflicts
- Integrity signature
- Permission-aware corrections
- Guild-wide filters
- Retention policy

## Encounter administration

Potential leadership tools:

- Archive or restore Encounter Profiles
- Validate enemy-wave composition
- Estimate expected rounds
- Compare actual rounds to par
- Inspect reward budget
- Run non-rewarding diagnostic simulations
- Copy a deterministic reproduction package

A diagnostic simulator must never mutate the real Adventurer, Companion, inventory, rewards, or Quest Session.

## Companion loadouts

Future progression:

- Multiple saved Companion loadouts
- Quest-specific allowed slots
- Role requirements
- Companion equipment
- Companion abilities
- Rest timers
- Injuries
- Portraits
- Stable sorting and filtering

Do not add multiple active Companions without first defining Encounter balance and save migration.

## Combat history analytics

Possible archive statistics:

- Win rate by Encounter
- Average rounds
- Average damage taken
- Fastest victory
- Most-used Companion
- Damage type performance
- Safety-pause frequency
- Survival best wave
- Reward yield per Encounter

These must remain gameplay analytics, not productivity scoring.

---

# Low Importance

- Outcome colors and icons
- Copy Highlights only
- Export selected archive search as JSON or CSV
- Pin important Battle Results
- Add archive tags and notes
- Show the deterministic seed in monospace
- Display average damage per round
- Add a compact tactical HUD window
- Add Companion portraits
- Add a small Stable-status icon to the Quest panel

---

# Expansion 2.0

## Quest World tactical context

Battle Archive records can later include:

- Room or Area
- Biome
- Structure type
- Hazard
- Altar
- Chest
- Merchant
- Crafting station
- Boss phase
- Extraction method

## Procedural Chronicle narrative

Structured Battle events and contextual world data can produce concise prose without printing every attack.

Example:

> Fenwick guarded the western archive while EchoDev broke the skeleton line in four rounds. Fire resistance prevented six damage, and the party recovered a charred registry key before the chamber collapsed.

## Skills, items, crafting, banking, and housing

The larger 2.0 foundation remains unchanged:

- Detailed item taxonomy
- Weapon and personal skills
- Tradeskills
- Biome resources
- Crafting stations
- Banking
- Housing
- Environmental protection

These remain outside the current Beta completion loop.

---

# Completed

- Repository and documentation preparation
- Clean founder authority and zero-blocker readiness baseline
- Identity Catalog generation
- Quest timer and progress reporting
- Reward consistency
- Audio recovery foundation
- Background Git monitoring
- Reusable Contract and Quest Run architecture
- Quest Run management and completed-run archive
- Tactical Encounter previews
- Tactical Field Reports
- Companion lifetime contribution reporting
- Survival milestone and exit reporting
- Compact Timecard combat summaries
- Dedicated Tactics workspace implemented in 0.31.1
- Local Battle Archive implemented in 0.31.1
- Companion roster recovery implemented in 0.31.1

---

# Current Product Decision

0.31.1 remains a Beta operations build.

After it opens and passes a smoke test, the strongest next pathway is **0.31.2 Inventory and Equipment Clarity**, covering detailed item classification, equipment comparison, carry-load explanation, loot provenance, and safer inventory operations without beginning the full 2.0 crafting system.
