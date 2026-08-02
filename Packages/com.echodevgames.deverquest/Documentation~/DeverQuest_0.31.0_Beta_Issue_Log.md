# DeverQuest 0.31.0 Beta Issue Log
## Pathway 4 — Tactical Visibility

**Source build:** 0.30.9 Beta 1  
**Patch build:** 0.31.0 Beta 1  
**Unity test environment:** 6000.3.8f1  
**Source status:** Quest Board and Run Management appeared operational; full 0.30.8/0.30.9 multi-account regression remains deferred  
**Patch status:** Prepared, awaiting Unity verification

---

# Scope

This build does not replace deterministic combat or add a new tactical game mode. It makes the existing Companion, Encounter, Combat, damage-typing, and Survival systems understandable enough to test.

---

## DQ-0309-026 — Battle outcomes are too opaque in the active Quest UI

**Type:** Combat visibility  
**Severity:** P1  
**Status:** Patched in 0.31.0; awaiting verification

### Previous behavior

The active Quest displayed a compact Battle Chronicle containing:

- Victory, defeat, or safety pause
- Encounter name
- Round count
- Hit Point change
- Typed-damage sentence
- Last tactical action

This did not clearly explain total damage, Companion contribution, important conditions, defeated groups, rewards, or the meaningful final turns.

### 0.31.0 correction

The active Quest now renders **Tactical Field Reports** containing:

- Outcome summary
- Round count and par
- Adventurer Hit Point change
- Damage dealt and taken
- Resistance, vulnerability, immunity, and absorption reactions
- Conditions and tactical effects
- Companion contribution
- Grouped defeated enemies
- Loot
- Injury or safety-pause consequence
- Recent combat turns
- Copy Full Combat Log
- Copy deterministic seed

Before the Encounter resolves, the Quest shows a **Tactical Encounter Preview** with mode, configured foe count, par rounds, and victory rewards.

---

## DQ-0309-027 — Companion contribution is not retained or explained

**Type:** Companion progression / reporting  
**Severity:** P1  
**Status:** Patched in 0.31.0; awaiting verification

### Previous behavior

The Companion Stable showed:

- Level
- XP
- Loyalty
- Hit Points
- Battles
- Victories

A resolved battle stored Companion Hit Point and XP changes, but the Stable did not explain what the Companion actually contributed.

### 0.31.0 correction

Companions now retain:

- Lifetime damage dealt
- Lifetime healing performed
- Lifetime damage taken
- Last battle summary
- Last battle timestamp

Stable cards now show:

- Win rate
- Lifetime contribution
- Last battle contribution

Battle reports and Timecards show Companion damage, healing, damage taken, hits, misses, XP, level change, and whether the Companion fell.

### Compatibility

Existing Companions start with zero lifetime contribution totals. Their historical Battle Results remain readable, but lifetime totals begin accumulating from battles resolved under 0.31.0.

---

## DQ-0309-028 — Survival progress and exit milestones are unclear

**Type:** Survival UX  
**Severity:** P1  
**Status:** Patched in 0.31.0; awaiting verification

### Previous behavior

The Survival panel displayed the wave number, carry weight, safety state, and exit buttons. It did not explain:

- Which wave comes next
- Current difficulty tier
- When difficulty increases
- When the Guild Wagon arrives
- Focused minutes per wave
- Which exit method successfully ended the expedition

### 0.31.0 correction

The Survival panel now displays:

- Completed waves
- Next wave number
- Current difficulty tier
- Waves until the next tier
- Waves until the Guild Wagon
- Focused minutes per wave
- Current carry load
- Current exit availability

Successful exits retain:

- Exit method
- Exit summary
- Exit timestamp

The result is written to the Session, Timecard, and Guild audit.

---

## DQ-0309-029 — Full combat transcripts overwhelm Timecards

**Type:** Chronicle readability  
**Severity:** P1  
**Status:** Patched in 0.31.0; awaiting verification

### Previous behavior

Every battle printed the complete combat log directly into the Timecard. Larger encounters could bury the Quest objective, rewards, and closing notes beneath many repetitive turn entries.

### 0.31.0 correction

Timecards now lead with:

- Outcome
- Damage Report
- Conditions and reactions
- Companion contribution
- Rewards
- Defeated enemies
- Loot
- Tactical actions
- Up to ten combat highlights

The complete transcript remains available inside a collapsible HTML `<details>` block.

This preserves debugging evidence without turning the main Chronicle into an attack-by-attack thicket.

---

## DQ-0309-030 — Tactical test content is not included in readiness guidance

**Type:** Beta setup  
**Severity:** Advisory  
**Status:** Patched in 0.31.0; awaiting verification

Release Readiness now checks for:

- At least one Encounter Profile
- At least one Companion Profile
- At least one Spell or Attack Technique

When missing, it directs the tester to:

`Guild Hall > Campaign Content Scaffolding`

and recommends generating the Tactical Starter Kit and Original Companion Stable.

---

# Known Boundaries

- Combat remains deterministic and resolves as a report rather than an interactive turn-by-turn game window.
- Companion healing totals are derived from stored Companion healing log entries.
- Old battles remain visible, but older records may not contain every newer event type.
- True independent editor-audio volume remains outside this patch.
- Full Biome, Room, hazard, boss, and procedural narrative systems remain Expansion 2.0 work.

---

# Required Retest

- [ ] Install 0.31.0 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Generate Tactical Starter Kit if advised.
- [ ] Generate Original Companion Stable if advised.
- [ ] Recruit and activate a Companion.
- [ ] Resolve a fixed Encounter.
- [ ] Verify Tactical Field Report.
- [ ] Verify damage reactions.
- [ ] Verify Companion contribution.
- [ ] Resolve a safety pause.
- [ ] Resolve a defeat where safely testable.
- [ ] Run multiple Survival waves.
- [ ] Verify difficulty and Wagon milestones.
- [ ] Exit by Flee, Homeward Passage, or Guild Wagon.
- [ ] Verify Timecard highlights and collapsible full log.
- [ ] Restart Unity and verify Companion lifetime totals persist.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
