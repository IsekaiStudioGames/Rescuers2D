# Death

## Purpose

This document defines health, damage, revival, mission failure, civilian death, respawning, and checkpoint mechanics.

## Overview

Rescuers2D uses a strict failure model: the loss of any rescuer or civilian ends the mission. Recovery tools (Bandages, Medkits) and checkpoints are the primary safety nets.

## Documentation

### Health

- Each rescuer has **3 Hearts**.
- Damage from hazards or enemies reduces Hearts (see [[09 - Hazards]] and [[08 - Enemies]] for sources).
- Reaching 0 Hearts incapacitates the rescuer unless a Medkit is available (see below).

### Damage Sources

| Source | Typical Severity |
|---|---|
| Environmental hazards | Low to high, hazard-dependent (see [[09 - Hazards]]) |
| Enemy contact | Low to moderate (see [[08 - Enemies]]) |
| Falls / collapses | Moderate |

### Bandages

- Restore 1 Heart on manual use.
- Consumed on use.
- Primary tool for incremental health management between encounters.

### Medkits

- Occupy 1 inventory slot.
- Automatically activate when a rescuer would otherwise reach 0 HP.
- Fully restore the rescuer and consume themselves upon activation.

> [!tip]
> Medkits function as a "last chance" safety net. Encourage players to keep at least one Medkit per rescuer before entering high-hazard segments.

### Rescuer Incapacitation

- A rescuer reaching 0 HP without an available Medkit becomes incapacitated.
- Incapacitation without revival is an immediate mission failure condition (see [[03 - Rules]]).

> [!warning]
> There is no partial-mission recovery from rescuer incapacitation. Design checkpoints generously to offset this strict failure state.

### Civilian Death

- Civilians have no combat or defensive ability and rely on rescuer protection (primarily the Riot Officer's shield).
- Civilian incapacitation is an immediate mission failure condition.
- Civilians should be clearly distinguishable from rescuers in UI and animation to avoid player confusion during escort.

### Mission Failure

Mission failure is triggered by any of the following (see [[03 - Rules]] for full list):

- Any rescuer incapacitated without revival.
- Any rescuer fails to reach safety.
- Any civilian incapacitated.
- Any civilian fails to reach Rescue Camp.

### Respawning and Checkpoints

- On mission failure, the player restarts from the most recent checkpoint.
- Checkpoints restore all rescuers to full inventory and health state as recorded at save time.
- Checkpoints are placed before/after major puzzle gates and at the start of escort sequences (see [[05 - Level Design]]).

| Checkpoint Rule | Detail |
|---|---|
| Placement | Before/after puzzle gates, start of escort sequences |
| Restores | Health, inventory, rescuer positions |
| Failure behavior | Restart from last checkpoint, no permanent penalty |

## Notes

> [!note]
> Because "no one left behind" is an absolute rule, generous checkpointing is essential to keeping failure feel fair rather than punishing.

### TODO

- [ ] Define exact incapacitation animation/feedback per character.
- [ ] Confirm whether a manual retry option exists outside of checkpoint restart.
- [ ] Define UI warning state for civilians at risk during escort.
