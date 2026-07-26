# Items

## Purpose

This document catalogs all inventory items, their usage, and stack limits within Rescuers2D.

## Overview

Each rescuer has 4 inventory slots. Items are shared freely across the team regardless of rescuer distance. Character abilities (ladder, shield, jump, swim, crawl, axe) never occupy inventory — only consumable and mission items do.

## Documentation

### Item Catalog

| Item         | Category     | Slot Cost | Usage                                                                                                 |
| ------------ | ------------ | --------- | ----------------------------------------------------------------------------------------------------- |
| Bandage      | Consumable   | 1         | Restores 1 Heart on use.                                                                              |
| Medkit       | Consumable   | 1         | Auto-activates at 0 HP threshold; fully restores and consumes rescuer's health, then consumes itself. |
| Key          | Mission Item | 1         | Unlocks a specific door, gate, or container.                                                          |
| C4           | Consumable   | 1         | Destroys reinforced obstacles beyond the Firefighter's axe capability.                                |
| Battery      | Mission Item | 1         | Powers electrical devices or switches.                                                                |
| Fuel         | Mission Item | 1         | Powers vehicles or generators required for progression.                                               |
| Mission Item | Mission Item | 1         | Generic placeholder for narrative/objective-specific items.                                           |
| Supplies     | Mission Item | 1         | Represents aid materials delivered to civilians or camp.                                              |

### Stack Limits

> [!note]
> Items do not stack by default; each unit occupies one full slot. Stacking is a candidate future feature (see TODO).

### Item Transfer

- Items can be moved between any two rescuers' inventories at any time, regardless of distance in the level.
- Transfer has no cooldown or restriction beyond destination slot availability.

### Item Usage Rules

| Rule | Detail |
|---|---|
| Full inventory | Blocks new pickups until a slot is freed. |
| Medkit activation | Automatic only; cannot be manually triggered early. |
| Bandage activation | Manual, player-triggered. |
| Mission items | Typically consumed on use at their designated interaction point. |

## Notes

> [!tip]
> Keep mission-critical items (Keys, Batteries, Fuel) visually distinct from consumables (Bandages, Medkits, C4) in UI iconography to reduce player confusion under inventory pressure.

### TODO

- [ ] Define stacking rules if implemented (e.g., Bandages stack to 2–3).
- [ ] Add placeholder icons per item for UI implementation.
- [ ] Confirm whether C4 requires a detonator item or is instant-use.
- [ ] Expand Supplies item into specific sub-types if needed (food, water, medical aid).
