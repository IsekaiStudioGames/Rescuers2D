# Characters

## Purpose

This document details the three playable rescuers, their abilities, and their design role within the team.

## Overview

Rescuers2D uses a fixed three-character roster, each filling a distinct mechanical role: environmental manipulation, protection, and mobility. Missions are designed to require all three.

| Character | Role |
|---|---|
| Firefighter | Environmental Manipulation |
| Riot Officer | Protection |
| Rescue Specialist | Mobility |

## Documentation

### Firefighter

**Role:** Environmental Manipulation

**Abilities**

- Place ladder
- Retrieve ladder
- Climb ladder
- Axe attack
- Break wooden debris
- Destroy blocked passages

**Strengths**

- Creates vertical traversal routes for the whole team.
- Clears wooden obstacles blocking paths or civilians.
- Ladder and axe are always available at no inventory cost.

**Weaknesses**

- Only one ladder can be deployed at a time.
- No ranged capability; axe is melee-only.
- Cannot reach areas requiring high jump, swimming, or crawling.

**Gameplay Purpose**

Unlocks vertical traversal and clears debris-based obstacles, acting as the team's primary path-opener.

> [!note]
> The ladder is not an inventory item — the Firefighter always owns it. Attempting to place a second ladder while one is already deployed triggers a confused animation instead of a functional action.

**Future Expansion Notes**

> [!note] TODO
> - [ ] Consider a fire-suppression ability for fire hazards.
> - [ ] Consider ladder durability or reusable retrieval limits for balancing.

---

### Riot Officer

**Role:** Protection

**Abilities**

- Raise shield
- Lower shield
- Block hazards
- Protect teammates
- Protect civilians

**Strengths**

- Can block projectiles, falling debris, and other directional hazards.
- Shield is always equipped; no setup cost.
- Enables safe passage for teammates and civilians through hazard zones.

**Weaknesses**

- No traversal-enhancing abilities.
- Shield only blocks hazards from the direction it faces.
- Cannot manipulate the environment or reach specialist-only areas.

**Gameplay Purpose**

Provides the team's primary defensive tool, enabling safe escort through active hazard or combat zones.

> [!note]
> The shield never occupies inventory. It is a toggled state (raised/lowered), not a consumable or equippable item.

**Future Expansion Notes**

> [!note] TODO
> - [ ] Consider shield stamina/durability system.
> - [ ] Consider a "brace" ability to protect civilians during escort specifically.

---

### Rescue Specialist

**Role:** Mobility

**Abilities**

- High Jump
- Swim
- Crawl through confined spaces

**Strengths**

- Reaches locations inaccessible to the other two rescuers.
- Required for switch activation and alternate path discovery.
- Enables underwater and confined-space traversal.

**Weaknesses**

- No offensive or defensive ability.
- No environmental manipulation (cannot clear debris or block hazards).
- Relies on teammates for protection in combat-adjacent situations.

**Gameplay Purpose**

Acts as the team's access key for optional areas, switches, and confined rescue paths that gate puzzle progression.

**Future Expansion Notes**

> [!note] TODO
> - [ ] Consider a rope/grapple ability for extended vertical mobility.
> - [ ] Define swim depth/hazard interactions (e.g., flood water currents).

## Notes

> [!warning]
> Every mission must be solvable using all three characters' unique abilities. Avoid designing sequences where one character becomes redundant for an extended stretch.

### TODO

- [ ] Finalize animation sets per character per ability.
- [ ] Confirm whether additional rescuers are planned post-jam (see [[12 - Disaster Relief]] for scope constraints).
