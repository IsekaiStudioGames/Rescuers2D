# Enemies

## Purpose

This document defines the enemy design philosophy, behaviors, and combat goals for Rescuers2D.

## Overview

Combat is intentionally limited in Rescuers2D. Enemies exist to create obstacles during rescue operations, not to serve as the primary gameplay focus. Environmental hazards remain the main antagonist (see [[09 - Hazards]]).

## Documentation

### Enemy Philosophy

> [!note]
> Enemies should never overshadow rescue objectives. Combat exists to reinforce the danger of a disaster zone, not to provide an action-focused challenge loop.

- Enemy encounters should be brief and avoidable where possible.
- Enemies should reinforce urgency (e.g., pressuring the player to complete a rescue quickly) rather than requiring sustained combat.
- Enemy presence should scale with mission difficulty tier (see [[05 - Level Design]]).

### Enemy Types

| Enemy | Behavior Concept | Design Intent |
|---|---|---|
| Looters | Opportunistic, may threaten civilians or steal items. | Reinforces stakes of leaving civilians unprotected. |
| Rabid Dogs | Aggressive, territorial, attack on sight. | Represents unpredictable post-disaster wildlife danger. |
| Aggressive Wildlife | Displaced animals reacting defensively to the disaster zone. | Reinforces environmental chaos without moral complexity. |

### Combat Goals

- Combat should be resolvable using the Riot Officer's shield (block/protect) or Firefighter's axe (melee) without requiring precision combat skill.
- Combat should never block puzzle progression as the only solution — an avoidance or protection path should typically exist.
- Enemy damage should be balanced against the 3-Heart health pool (see [[10 - Death]]) so that combat is dangerous but rarely lethal in a single encounter.

### Enemy Behavior Guidelines

| Behavior Trait | Guideline |
|---|---|
| Aggro range | Short, to keep encounters localized and avoidable. |
| Damage output | Low-to-moderate; should not out-pace Bandage/Medkit recovery. |
| Civilian threat | Looters may specifically target civilians during escort, raising escort tension. |
| Persistence | Enemies should not chase across the full level; localized encounter zones preferred. |

## Notes

> [!warning]
> Do not expand enemy variety at the expense of rescue and puzzle content. Combat remains a secondary system per the "Rescue Before Combat" pillar.

### TODO

- [ ] Define enemy stats (HP, damage, aggro range) per type.
- [ ] Determine whether Looters can steal items from rescuer inventory, and how items are recovered.
- [ ] Consider a "non-lethal deterrent" option for players who wish to avoid combat entirely.
