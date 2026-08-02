# DeverQuest Personal Notes
## Consolidated for 0.31.0 Tactical Visibility

**Previous build:** 0.30.9 Beta 1  
**Current build:** 0.31.0 Beta 1  
**Deferred regression:** 0.30.8 repeatable/Party policy matrix and 0.30.9 run-management multi-account tests

---

# Immediate Importance

## Readable combat outcomes

The existing deterministic combat system needs to explain:

- What happened
- Who acted
- What damage mattered
- Which resistance or vulnerability triggered
- Which conditions were applied
- What the Companion contributed
- What was defeated
- What was collected
- Whether the party won, lost, escaped, or paused safely

**0.31.0:** Tactical Field Reports and Encounter previews implemented.

## Companion contribution

Companions should feel like participants rather than extra Hit Point counters.

Required reporting:

- Damage dealt
- Healing
- Damage taken
- Hits and misses
- XP and level changes
- Fall/recovery state
- Battles, victories, and win rate
- Last battle summary

**0.31.0:** battle and lifetime reporting implemented.

## Survival navigation

A Survival run needs visible milestones:

- Next wave
- Difficulty tier
- Next tier increase
- Guild Wagon timing
- Wave interval
- Encumbrance state
- Exit options
- Successful exit method

**0.31.0:** milestone and exit reporting implemented.

## Chronicle compression

The Chronicle should summarize combat rather than lead with every repeated attack.

**0.31.0:** compact summaries and highlights are shown first; the full transcript remains in a collapsible block.

## Tactical test setup

Release Readiness should tell a tester when Encounter, Companion, and ability assets are missing.

**0.31.0:** Tactical test content readiness check implemented.

---

# Medium Importance

## Dedicated Tactics workspace

After the current reports are verified, consider a separate Tactics workspace containing:

- Active Encounter
- Party and Companion status
- Current or latest round
- Combatants
- Conditions
- Damage reactions
- Wave progress
- Full transcript
- Debug seed

Do not add this workspace until the compact reports prove which information is genuinely useful.

## Better event model

Some metrics are currently derived from stored text lines, especially Companion healing and misses.

A later event schema should explicitly record:

- Attack attempted
- Hit
- Miss
- Heal
- Shield absorbed
- Condition applied
- Condition resisted
- Condition removed
- Killing blow
- Companion protection
- Escape attempt

This will make statistics and procedural Chronicles more reliable.

## Combat history archive

Add filters for:

- Encounter
- Monster
- Companion
- Damage type
- Victory/defeat
- Survival wave
- Date
- Project
- Quest Run

This can live beside the Completed Quest Run Archive after the active reports are stable.

## Companion roster expansion

Future Companion improvements:

- Multiple active slots where a Quest permits them
- Party role requirements
- Equipment
- Abilities
- Loyalty events
- Injuries
- Rest and recovery time
- Stable sorting and filters
- Portraits

## Survival administration

Future controls:

- Configurable extraction checkpoints
- Named extraction methods
- Wave modifiers
- Boss waves
- Rest stops
- Merchant or Wagon inventory
- Run-level leaderboards
- Best wave record

## Reward language

The new tactical interface uses **Rewards** rather than **Spoils** in some player-facing areas. Continue deliberating a consistent term for:

- Quest base rewards
- Encounter drops
- Completion claim
- Character rewards
- Member/account rewards

Do not rename serialized fields during Beta.

---

# Low Importance

- Colored outcome badges.
- Small damage-type icons.
- Companion portrait in battle reports.
- Copy only Highlights button.
- Export one battle as JSON.
- Highlight killing blows.
- Show average damage per round.
- Show fastest victory per Encounter.
- Add restrained victory and defeat animations.
- Add monospace formatting for deterministic seeds.

---

# Expansion 2.0

## Rooms, Biomes, and hazards

The future Quest World may compose each run from:

- Room or outdoor Area
- Biome
- Structure type
- Enemy waves
- Boss
- Hazards
- Chests
- Traps
- Altars
- Merchants
- Crafting opportunities
- Extraction points

Combat and Survival summaries created in 0.31.0 become the reporting foundation for this expansion.

## Procedural Chronicle narrative

Use structured combat events to generate compact prose:

> Ajnaag entered a smoke-filled archive beneath the old server vault. The party defeated three skeletons before the eastern hall collapsed. Fenwick absorbed two attacks, restored 4 Hit Points, and helped secure the final room. The Guild Wagon recovered the party after wave six with 22 copper and a charred registry key.

Do not generate every attack as narrative. Aggregate meaningful contributions.

## Expanded skills and items

Preserve the broader roadmap for:

- Weapon skills
- Personal and movement skills
- Tradeskills
- Detailed item categories
- Environmental protection
- Crafting
- Banking
- Housing
- Biome resources

These remain outside the Beta release loop.

---

# Completed

- Repository and documentation preparation.
- Zero-blocker 0.30.7 readiness run.
- Founder CEO authority.
- Identity Catalog generation.
- Quest progress panel.
- Reward consistency.
- Audio recovery foundation.
- Background Git monitoring.
- Repeatable, limited, and one-time Contract architecture.
- Quest Run IDs and completion history.
- Flexible and full Party rules implemented.
- Quest Run Management.
- Completed Quest Run Archive.
- Contract archive/restore.
- Stale reservation readiness check.
- Tactical Encounter preview implemented in 0.31.0.
- Tactical Field Reports implemented in 0.31.0.
- Companion lifetime contribution implemented in 0.31.0.
- Survival milestone and exit reporting implemented in 0.31.0.
- Compact Timecard combat highlights implemented in 0.31.0.

---

# Current Decision

Keep 0.31.0 focused on visibility and verification.

After this pathway opens and compiles cleanly, the next improvement path should be selected from:

1. Guild accounts and multi-member workflow validation
2. Inventory, equipment, trading, and shop visibility
3. Dedicated Quest Board / Git / Completed Log workspace separation

Do not begin crafting, banking, housing, or the full Quest World expansion until the existing Beta loop is verified.
