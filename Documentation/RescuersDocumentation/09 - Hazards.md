# Hazards

## Purpose

This document catalogs every environmental hazard in Rescuers2D, including behavior, damage, and player interaction.

## Overview

Environmental hazards are the primary antagonist of Rescuers2D. Each hazard is designed to interact with specific rescuer abilities, forming the backbone of the puzzle system (see [[07 - Puzzles]]).

## Documentation

### Hazard Catalog

| Hazard | Behavior | Damage | Player Interaction | Design Notes |
|---|---|---|---|---|
| Collapsed Buildings | Static obstruction blocking paths. | None (blocks movement) | Cleared by Firefighter (debris) or bypassed via alternate route. | Primary traversal-puzzle driver. |
| Falling Debris | Periodic or triggered drops from above. | Moderate, on contact | Avoided by timing, or blocked by Riot Officer's shield. | Strong candidate for timing puzzles. |
| Fire | Spreading or static damage zone. | High, continuous while in contact | Avoided; may require alternate route or extinguishing mechanic. | Consider spread behavior for tension. |
| Flood Water | Rising or standing water blocking ground paths. | None directly; risk of drowning without swim ability | Crossed via Rescue Specialist's swim ability. | Non-swimmers must find alternate routes. |
| Electricity | Charged hazard zone or exposed wiring. | High, on contact | Avoided, or de-powered via switch/battery removal. | Good fit for inventory puzzles (battery). |
| Gas Leaks | Invisible or lightly telegraphed hazard; combustible. | High, potentially delayed (ignition) | Avoided; may require valve shutoff before proceeding. | Strong candidate for timing puzzles. |
| Aftershocks | Timed, level-wide event causing temporary hazards. | Variable, based on triggered effects | Requires reaching safety before the event resolves. | Use sparingly as a tension spike. |
| Weak Floors | Terrain that collapses under weight after delay or repeated contact. | Fall damage on collapse | Crossed quickly, or avoided via ladder/alternate path. | Should telegraph clearly before collapse. |
| Destroyed Roads | Terrain gaps in traversal paths. | None (blocks movement) | Crossed via ladder, jump, or alternate route. | Basic traversal-puzzle driver. |

### Hazard Interaction Summary

> [!tip]
> Each hazard should map to at least one clear ability-based solution to keep puzzle logic consistent (see [[07 - Puzzles]]).

| Hazard | Firefighter | Riot Officer | Rescue Specialist |
|---|---|---|---|
| Collapsed Buildings | Clears debris | — | Crawls through gaps |
| Falling Debris | — | Blocks with shield | — |
| Fire | — | — | — (avoidance only) |
| Flood Water | — | — | Swims across |
| Electricity | — | — | Activates de-power switch |
| Gas Leaks | — | — | Reaches valve via confined space |
| Aftershocks | — | Shields during event | — |
| Weak Floors | Ladder bypass | — | High jump bypass |
| Destroyed Roads | Ladder bridge | — | High jump crossing |

## Notes

> [!warning]
> Hazards should be visually and audibly telegraphed before triggering damage, particularly Weak Floors and Gas Leaks, to keep failure feeling fair rather than arbitrary.

### TODO

- [ ] Define exact damage values per hazard once health balancing pass begins.
- [ ] Define VFX/SFX telegraph requirements per hazard.
- [ ] Confirm whether Aftershocks are scripted events or randomized.
