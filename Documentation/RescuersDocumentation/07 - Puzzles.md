# Puzzles

## Purpose

This document defines the puzzle design philosophy and archetypes used throughout Rescuers2D.

## Overview

Puzzles are the primary challenge layer of Rescuers2D, built on character switching and combined rescuer abilities, following The Lost Vikings' cooperative-puzzle model.

## Documentation

### Puzzle Philosophy

> [!note]
> Every puzzle should have a clear ability-based solution rather than relying on trial and error. Puzzles test coordination of the three rescuers, not reflex skill alone.

- Puzzles should be solvable by understanding character abilities, not by hidden mechanics.
- Multi-character puzzles should require deliberate positioning and switching, reinforcing the "Teamwork Through Character Switching" pillar.
- Difficulty should come from combination complexity, not obscurity.

### Puzzle Archetypes

| Archetype | Description | Primary Character(s) |
|---|---|---|
| Traversal | Requires reaching a location via ladder, high jump, swim, or crawl. | Firefighter, Rescue Specialist |
| Inventory | Requires transporting or combining items (keys, batteries, fuel) to unlock progress. | Any |
| Escort | Requires guiding a civilian safely past hazards, often with shield protection. | Riot Officer |
| Hazard | Requires neutralizing or bypassing an environmental hazard using ability combinations. | Varies by hazard (see [[09 - Hazards]]) |
| Timing | Requires synchronized actions under a time constraint (e.g., closing gas valve before ignition). | Any |
| Multi-Character Interaction | Requires two or more rescuers acting in sequence or simultaneously to progress. | All three |

### Traversal Puzzles

- Ladder placement to cross vertical gaps.
- High jump to reach elevated switches or ledges.
- Crawling through confined debris gaps.
- Swimming through flood water sections.

### Inventory Puzzles

- Transporting a key or battery from one rescuer to another across a hazard the carrier cannot cross.
- Requires switching to a rescuer capable of traversal, then transferring the item.

### Escort Puzzles

- Positioning the Riot Officer's shield to block a hazard while a civilian passes.
- Timing civilian movement between hazard cycles (e.g., falling debris intervals).

### Hazard Puzzles

- Using the Firefighter to clear debris blocking a hazard's source.
- Using the Riot Officer to shield teammates while crossing an active hazard.

See [[09 - Hazards]] for hazard-specific interactions.

### Timing Puzzles

- Sequential switch activation within a time window.
- Coordinated escape before an aftershock or collapse.

### Multi-Character Interaction Puzzles

- Firefighter clears a path → Rescue Specialist activates a switch → Riot Officer shields the resulting hazard release.
- Designed as the capstone puzzle type for advanced missions (see [[05 - Level Design]]).

## Notes

> [!warning]
> Avoid puzzles that can be solved by only one character across an entire mission. All three rescuers must be functionally necessary.

### TODO

- [ ] Build a puzzle template document per mission for designers.
- [ ] Define a hint system for stuck players.
- [ ] Catalog puzzle examples per mission once level list is finalized (see [[11 - Level Codes]]).
