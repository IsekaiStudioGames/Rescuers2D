# Level Codes

## Purpose

This document defines mission numbering, naming conventions, templates, and internal identifiers for Rescuers2D levels.

## Overview

A consistent level coding system allows the team to reference missions unambiguously across design, art, and engineering documentation.

## Documentation

### Naming Convention

```
R2D-[Zone]-[MissionNumber]_[ShortName]
```

| Component | Description | Example |
|---|---|---|
| R2D | Project prefix (Rescuers2D) | R2D |
| Zone | Two-letter disaster zone code | UR (Urban), RD (Roads), FL (Flood) |
| MissionNumber | Two-digit sequential number within zone | 01, 02, 03 |
| ShortName | Lowercase snake_case descriptor | collapsed_block |

### Internal Identifiers

- Unity scene files should match the mission code exactly (e.g., `R2D-UR-01_collapsed_block.unity`).
- Save/checkpoint data keys should reference the mission code as a namespace prefix.

### Mission Numbering Template

| Zone Code | Zone Name |
|---|---|
| UR | Urban / Collapsed Structures |
| RD | Roads / Infrastructure |
| FL | Flood Zones |
| FR | Fire Zones |
| AS | Aftershock Events |

> [!tip]
> Reserve zone codes in advance even if not all zones ship in the jam build, to keep numbering stable for future expansion.

### Example Mission List

| Code | Name | Zone | Tier |
|---|---|---|---|
| R2D-UR-01_collapsed_block | Collapsed Block | Urban | Introductory |
| R2D-UR-02_trapped_family | Trapped Family | Urban | Standard |
| R2D-RD-01_broken_overpass | Broken Overpass | Roads | Standard |
| R2D-FL-01_flooded_basement | Flooded Basement | Flood | Advanced |

> [!note]
> Example list is illustrative only; final mission list to be confirmed by level design lead.

## Notes

> [!warning]
> Do not reuse a mission code once assigned, even if a mission is cut, to avoid ambiguity in version control history.

### TODO

- [ ] Confirm final zone list for jam scope.
- [ ] Finalize full mission list with codes prior to production start.
- [ ] Document save file schema referencing mission codes.
