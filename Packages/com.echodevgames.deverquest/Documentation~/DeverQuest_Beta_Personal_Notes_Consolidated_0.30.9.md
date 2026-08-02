# DeverQuest Personal Notes
## Consolidated for 0.30.9 Quest Board and Run Management

**0.30.8 status:** Implemented, full policy regression deferred  
**Current build:** 0.30.9 Beta 1

---

# Immediate Importance

## Operational control for reusable Quests

Reusable and limited Contracts need management once real users begin claiming them. Leadership must be able to see active reservations, waiting Parties, and old claims without inspecting raw ScriptableObject arrays.

**0.30.9:** Quest Run Management implemented.

## Completed Quest visibility

A completed-Quest archive is part of the finished productivity loop. Users should be able to review who completed a run, how long it took, and what was awarded without opening YAML or individual Contract assets.

**0.30.9:** searchable Completed Quest Run Archive implemented in Rewards & History.

## Non-destructive retirement

Repeatable Quests need an off switch that preserves history. Deleting or changing them to Draft is too ambiguous.

**0.30.9:** Archive/Restore listing implemented.

## Stale reservation safety

Leadership needs a way to release abandoned board claims, but must not accidentally cancel the active local Quest.

**0.30.9:** stale cancellation refuses to touch the active local Session and requires confirmation.

---

# Medium Importance

## Dedicated Quest Board workspace

The current Quest creation form and Assignment Board still share the Quest workspace. A later UI pass should separate:

- Quest Board
- Active Quest
- Quest Run Management
- Completed Quest Archive
- Git

Do this after the underlying loop is stable.

## Run-level approval and evidence

A future run record should support:

- Under Review
- Approved
- Returned
- Rejected
- Evidence links
- Reviewer
- Review notes

This should apply to one Run, not change the reusable Contract's global status.

## Append-only shared Run ledger

Contract-asset history remains vulnerable to ordinary Git conflicts when two clones edit the same Contract simultaneously. Move shared reservations and completions into one file per Run in a later network/Guild milestone.

## Archive filters

Future filters:

- Date range
- Project
- Department
- Adventurer
- Contract policy
- Party/solo
- Archived/active
- Reward range

## Stale threshold settings

The current Readiness threshold is 24 hours. Later allow Guild administrators to configure thresholds by Contract type or predicted task length.

---

# Low Importance

- Colored badges for Repeatable, Limited, One-Time, Waiting, Active, Completed, and Archived.
- Progress pips for limited completion targets.
- Last five completers on the board card.
- Copy Completion ID in addition to Run ID.
- Export selected Contract history.
- Celebration animation when a limited Guild target is completed.

---

# Expansion 2.0

- Shared server-backed reservations.
- Procedurally generated Run variants.
- Biome and Encounter history per Run.
- Run-specific loot and item drops.
- Run-level combat summaries.
- Seasonal repeatable Contract rotations.
- Guild-wide world-event completion targets.

---

# Completed

- 0.30.7 Readiness reached 15/0/0.
- Repeatable Contract architecture implemented.
- Limited completion policy implemented.
- Unique-Adventurer rule implemented.
- Flexible/full Party rules implemented.
- Quest Run IDs implemented.
- Contract Completion History implemented.
- 0.30.8 full policy regression preserved for later.
- Quest Run Management implemented in 0.30.9.
- Stale reservation cleanup implemented.
- Waiting Party cleanup implemented.
- Contract archive/restore implemented.
- Completed Quest Run Archive implemented.
- Readiness stale-run check implemented.

---

# Current Decision

Do a short 0.30.9 smoke test when convenient. The next active development pathway should then move into **Companions, Combat, and Survival visibility**, because those systems exist but have not yet received the same readable player-facing feedback as Quests and rewards.
