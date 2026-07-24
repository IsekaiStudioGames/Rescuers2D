# Rules

## Purpose

This document defines the project overview, gameplay loop, core design pillars, and the core gameplay rules governing character switching, inventory, health, and mission win/failure conditions.

## Overview

Rescuers2D missions are governed by a strict, no-one-left-behind ruleset. All three rescuers and all civilians must reach safety for a mission to succeed.

## Documentation

### Project Overview

| Field | Detail |
|---|---|
| Engine | Unity 6000.3.8f1 |
| Genre | 2D Side-Scrolling Puzzle Platformer |
| Inspiration | The Lost Vikings (Blizzard Entertainment) |
| Platform | Windows PC |
| Players | Single-player, three-character team control |

Rescuers2D is a single-player puzzle platformer in which the player controls a three-person emergency response team rescuing civilians trapped after a devastating earthquake. The game emphasizes cooperation, environmental traversal, inventory management, and rescue operations over combat.

The project is being developed for the Juntos Global Game Jam in support of disaster relief efforts following the Venezuela earthquakes.

> [!note]
> Rescuers2D prioritizes rescue mechanics and puzzle-solving. Combat is a secondary, supporting system used only to reinforce the dangers of a disaster zone.

### Core Gameplay Loop

```
Receive Distress Call
        ↓
Deploy Rescue Team
        ↓
Navigate Disaster Zone
        ↓
Solve Environmental Puzzles
        ↓
Reach Trapped Civilian
        ↓
Escort Civilian
        ↓
Return to Rescue Camp
        ↓
Mission Complete
```

### Core Design Pillars

| Pillar | Description |
|---|---|
| Rescue Before Combat | Rescue objectives always take priority over combat encounters. |
| Teamwork Through Character Switching | Missions are designed around switching between three rescuers with distinct abilities. |
| Environmental Puzzle Solving | Hazards and level geometry form the primary challenge, not enemies. |
| Respectful Disaster Relief Representation | Content must depict disaster response and affected communities with dignity and accuracy. |
| Every Rescuer Matters | Each character's abilities are required, not optional, to mission design. |
| No One Left Behind | Mission success requires all rescuers and all civilians to reach safety. |

> [!tip]
> Every rule and system below should trace back to one of these pillars. See [[12 - Disaster Relief]] for the humanitarian context behind "Respectful Disaster Relief Representation."

### Character Switching

- The player controls one active rescuer at a time; the other two remain in the level at their last position.
- Switching is instant and has no cooldown.
- Inactive rescuers hold their position and do not take automatic actions.
- Some puzzles require positioning multiple rescuers before switching to trigger a combined interaction (see [[07 - Puzzles]]).

> [!note]
> Character switching is the primary interaction method inherited from The Lost Vikings and is central to puzzle design.

### Inventory

- Each rescuer has **4 inventory slots**.
- Abilities (ladder, shield, jump, swim, crawl, axe) do **not** consume inventory slots.
- Items may be freely transferred between rescuers regardless of distance between them.
- Full inventory blocks pickup of new items until a slot is freed.

See [[06 - Items]] for the full item catalog.

### Health

- Each rescuer has **3 Hearts**.
- Bandages restore 1 Heart.
- Medkits occupy 1 inventory slot and auto-activate when a rescuer would reach 0 HP, restoring the rescuer and consuming themselves.

See [[10 - Death]] for full damage and revival rules.

### Mission Flow

1. Receive Distress Call
2. Deploy Rescue Team
3. Navigate Disaster Zone
4. Solve Environmental Puzzles
5. Reach Trapped Civilian
6. Escort Civilian
7. Return to Rescue Camp
8. Mission Complete

### Win Conditions

> [!tip] Mission Success
> - Rescue every civilian.
> - Escort every civilian safely to Rescue Camp.
> - Every rescuer reaches extraction.

### Failure Conditions

> [!warning] Mission Failure
> - Any rescuer becomes incapacitated without revival.
> - Any rescuer fails to reach safety.
> - Any civilian becomes incapacitated.
> - Any civilian fails to reach Rescue Camp.

### Rule Summary Table

| System | Rule |
|---|---|
| Rescuers | 3, always controllable via switching |
| Hearts per rescuer | 3 |
| Inventory slots per rescuer | 4 |
| Item transfer | Free, unlimited distance |
| Abilities | Do not consume inventory |
| Mission failure trigger | Any rescuer or civilian lost |

## Notes

> [!warning]
> "No one left behind" is a hard failure condition, not a soft penalty. Design mission checkpoints accordingly (see [[10 - Death]]).

### TODO

- [ ] Define whether difficulty settings will relax failure conditions.
- [ ] Define UI feedback rules for near-failure states (e.g., rescuer at 1 Heart).
- [ ] Add logline / elevator pitch once finalized by narrative lead.
- [ ] Add target jam submission build date.
