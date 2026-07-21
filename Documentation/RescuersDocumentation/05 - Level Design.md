# Level Design

## Purpose

This document defines mission structure, pacing, puzzle progression, escort sequences, checkpoint philosophy, and difficulty progression for Rescuers2D levels.

## Overview

Levels are built around the core gameplay loop: deploy, navigate, solve, rescue, escort, return. Each mission is a self-contained disaster zone requiring all three rescuer abilities.

## Documentation

### Mission Structure

| Phase | Description |
|---|---|
| Deployment | Team enters the disaster zone from Rescue Camp. |
| Traversal | Team navigates hazards and terrain using character abilities. |
| Puzzle Gate | A puzzle blocks progress until solved using multi-character interaction. |
| Civilian Discovery | Team locates one or more trapped civilians. |
| Escort | Team guards and guides civilians back through the zone. |
| Extraction | Team and civilians reach Rescue Camp; mission ends. |

### Visual Layer Structure

Every level is built from four depth layers, ordered back to front. This structure governs both visual composition and how much information/geometry is exposed to the player at a glance.

| Layer | Depth Order | Content | Function |
|---|---|---|---|
| Parallax Background | Farthest | Sky, distant skyline, smoke, ambient disaster-zone scenery | Establishes atmosphere and scale; scrolls slower than the camera for depth cueing. Non-interactive. |
| Backdrop | Mid-back | Buildings, ground surfaces, structural scenery | Establishes the immediate environment context behind gameplay space. Non-interactive; visually grounds the Barrier layer. |
| Barrier | Gameplay plane | Floors, walls, collision geometry | The interactive layer. All rescuer movement, hazard placement, and puzzle collision live here (see [[09 - Hazards]], [[07 - Puzzles]]). |
| Foreground | Nearest | Rubble, overhangs, foliage, structural debris | Adds depth by rendering in front of characters; also used to conceal hidden paths, items, or civilians until the player approaches or interacts. |

> [!note]
> Parallax Background and Backdrop are non-interactive dressing layers. Only the Barrier layer carries collision; the Foreground layer is visual-only and never blocks player input, even though it renders in front of the player.

> [!tip]
> Use the Foreground layer deliberately to hide optional civilians, items, or alternate routes — this supports exploration-driven puzzle design without adding new mechanics (see [[07 - Puzzles]]).

- **Parallax scroll rates** should decrease with distance from the Barrier layer (Background slowest, Foreground fastest) to reinforce depth.
- **Backdrop** art should visually communicate the mission's zone type (Urban, Roads, Flood, etc. — see [[11 - Level Codes]]) even before the player reaches the Barrier layer's hazards.
- **Barrier** geometry should remain visually distinct from Backdrop dressing so players can immediately identify traversable surfaces.
- **Foreground** elements concealing content should have a subtle visual tell (e.g., slight motion, gap, or lighting change) to keep hidden-location discovery fair rather than arbitrary.

### Tile Workflow

Level geometry is built from modular, tileable assets rather than hand-painted per-tile art, to support rapid iteration during production.

| Tool | Usage |
|---|---|
| Unity Rule Tiles | Auto-tiling for Barrier and Backdrop geometry, similar to the workflow used in *Hollow Knight*. Automatically selects correct edge/corner/fill sprites based on neighboring tiles. |
| SuperTile2Unity | Imports modular, pre-authored tile pieces (multi-tile prefab chunks) for larger structural elements, reducing manual placement of repeated geometry. |

> [!note]
> Purpose: allow designers to block out and iterate on levels quickly without hand-painting every tile, while keeping visual consistency across missions.

**Application by layer**

- **Backdrop** — Rule Tiles handle building facades and ground surfaces; SuperTile2Unity chunks handle larger repeated structures (e.g., collapsed building sections).
- **Barrier** — Rule Tiles handle floor/wall collision geometry, ensuring collision shapes stay consistent with auto-selected sprites.
- **Parallax Background / Foreground** — Typically use hand-placed or modular prefab dressing rather than Rule Tiles, since these layers are non-interactive and lower-frequency.

> [!tip]
> Keep Rule Tile rule sets and SuperTile2Unity chunk libraries organized per zone type (Urban, Roads, Flood, etc. — see [[11 - Level Codes]]) so level designers can reuse consistent tile sets across missions in the same zone.

### Level Pacing

- Open with a low-hazard introduction segment to establish character controls.
- Escalate hazard density and puzzle complexity toward the civilian location.
- Escort sequences should reduce puzzle complexity but increase hazard avoidance tension, since civilians are unprotected without the Riot Officer's shield.

> [!tip]
> Pace escort segments as the tension peak of a mission — the puzzle-solving is done, and hazard avoidance becomes the primary challenge.

### Puzzle Progression

- Early missions introduce single-ability puzzles (e.g., ladder-only, shield-only).
- Mid missions require two-character combinations (e.g., Firefighter clears debris to expose a switch the Rescue Specialist activates).
- Late missions require full three-character coordination within a single puzzle chain.

See [[07 - Puzzles]] for puzzle archetypes.

### Escort Sequence Design

- Civilians move at a fixed pace and cannot use rescuer abilities.
- Civilians must be shielded from directional hazards by the Riot Officer where possible.
- Escort routes should avoid backtracking through unresolved hazards already cleared during traversal.

### Checkpoint Philosophy

> [!note]
> Because mission failure is strict (see [[03 - Rules]]), checkpoints must be frequent enough to avoid punishing long backtracking, especially during escort sequences.

- Place checkpoints immediately before and after major puzzle gates.
- Place a checkpoint at the start of every escort sequence.
- Checkpoints restore all rescuers' health and inventory state at time of save.

### Difficulty Progression

| Mission Tier | Hazard Density | Puzzle Complexity | Enemy Presence |
|---|---|---|---|
| Introductory | Low | Single-ability | None |
| Standard | Medium | Two-character | Occasional |
| Advanced | High | Three-character | Regular |

## Notes

> [!warning]
> Avoid difficulty spikes driven by enemy density. Difficulty should scale primarily through hazard and puzzle complexity, consistent with the "Rescue Before Combat" pillar.

### TODO

- [ ] Define exact checkpoint save data schema.
- [ ] Build a reference mission map/flowchart template.
- [ ] Determine total mission count for jam scope.
- [ ] Define Unity Sorting Layer names/order matching Parallax Background, Backdrop, Barrier, and Foreground.
- [ ] Define parallax scroll multipliers per layer.
- [ ] Confirm whether Foreground-hidden content requires a dedicated discovery/reveal mechanic.
- [ ] Build initial Rule Tile sets per zone type.
- [ ] Confirm SuperTile2Unity import pipeline and folder structure.
- [ ] Define naming convention for tile chunk prefabs.
