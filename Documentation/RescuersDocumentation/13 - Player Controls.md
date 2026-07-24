# Player Controls

## Purpose

This document defines the keyboard and controller control schemes for Rescuers2D, covering movement, character switching, item usage, and pause-menu/inventory controls.

## Overview

Rescuers2D supports both keyboard and controller input. Several keys are context-sensitive — the same input produces different actions depending on the rescuer's current state (e.g., grounded vs. on a ladder) or whether the game is paused. Controller mapping mirrors keyboard behavior 1:1, including context sensitivity.

## Documentation

### Gameplay Controls — Keyboard

| Key | Action | Context |
|---|---|---|
| W | Climb Up | Climbs a ladder upward. No effect if not on a ladder. |
| A | Move Left | Standard horizontal movement. |
| S | Climb Down / Crawl | Climbs a ladder downward. If the active rescuer is the Rescue Specialist and a confined space is entered, crawls instead. No crawl effect for Firefighter or Riot Officer — they simply have no action off a ladder. |
| D | Move Right | Standard horizontal movement. |
| F | Use Selected Item | Uses the currently selected inventory item. |
| J | Ability 1 | Character-specific primary ability — see mapping table below. |
| K | Ability 2 | Character-specific secondary ability — see mapping table below. |
| Q | Cycle Character Left | Switches control to the previous rescuer in the roster order. |
| E | Cycle Character Right | Switches control to the next rescuer in the roster order. |
| Spacebar | Activate | Context-sensitive interact (e.g., switch activation, environment/object interaction). |
| Tab | Pause | Opens the pause menu. |
| Enter | Open Quit Confirmation Window | Available only while paused. |

> [!note]
> W and S are now dedicated exclusively to ladder climbing and have no effect off a ladder — jumping and abilities have moved to J/K to remove the previous dual-purpose overload on the movement keys. The one exception is Crawl, which stays on S for the Rescue Specialist only, triggering when they enter a confined space.

> [!tip]
> The Firefighter and Riot Officer have **no jump and no crawl** by design. This is intentional — it reinforces the "Teamwork Through Character Switching" pillar (see [[03 - Rules]]) by ensuring some terrain can only be crossed by switching to the Rescue Specialist, rather than being solvable by any single character.

### Ability 1 / Ability 2 Mapping by Character

Ability 1 (J) and Ability 2 (K) are character-specific slots — their effect changes depending on which rescuer is currently active, similar to how Activate already behaves.

| Character | Ability 1 (J) | Ability 2 (K) |
|---|---|---|
| Firefighter | Place / Retrieve Ladder | Axe Attack |
| Riot Officer | Toggle Shield (Up/Down) | Stun Baton — stuns adjacent targets |
| Rescue Specialist | Jump | Jet Swim — dart forward while swimming |

> [!note]
> Rescue Specialist's Jump (J) also functions while swimming: pressing J while in water propels the Specialist upward, allowing them to exit the water surface.

> [!warning]
> The Riot Officer has **no standalone jump or crawl** — J is fully dedicated to the shield toggle for this character, consistent with the Firefighter/Riot Officer restriction above. Level design should treat this as a deliberate puzzle constraint: any Riot Officer-only path must route around gaps and confined spaces, not require jumping over or crawling through them (see [[05 - Level Design]], [[07 - Puzzles]]).

### Gameplay Controls — Controller

| Action | Xbox | PlayStation | Input Type |
|---|---|---|---|
| Move Left / Right | Left Stick / D-Pad ← → | Left Stick / D-Pad ← → | Directional |
| Climb Up | Left Stick / D-Pad ↑ | Left Stick / D-Pad ↑ | Directional (ladder only) |
| Climb Down / Crawl | Left Stick / D-Pad ↓ | Left Stick / D-Pad ↓ | Directional (ladder for all; crawl for Rescue Specialist in confined spaces) |
| Use Selected Item | X | Square | Face Button (West) |
| Ability 1 | B | Circle | Face Button (East) |
| Ability 2 | LT | L2 | Left Trigger |
| Activate | A | Cross | Face Button (South) |
| Cycle Character Left | LB | L1 | Bumper |
| Cycle Character Right | RB | R1 | Bumper |
| Pause / Unpause | Start | Options | System Button — toggles pause state |
| Open Quit Confirmation Window | Select (View) | Select (Share) | System Button, pause menu only |

> [!tip]
> Directional inputs (movement, climb up/down) accept both Left Stick and D-Pad simultaneously — do not require the player to choose one input method exclusively.

> [!note]
> The "Select" button is labeled differently across controller generations (Xbox: View; PlayStation: Select on older pads, Share on PS4, Create on PS5). Bind to the OS-reported "Select/Back" input rather than a hardcoded label. Y (Xbox) / Triangle (PS) is now unassigned and free for future use.

### Pause Menu & Inventory Controls

The pause menu reuses several gameplay inputs contextually.

**Keyboard**

| Input | Behavior |
|---|---|
| F | Selects an item in the inventory. |
| WASD (item selected) | Moves the selected item to an empty slot, or swaps it with the item currently in the target slot. |
| Q / E (item selected) | Transfers the selected item to the previous/next character, **only if that character has an empty slot**. No empty slot = no transfer. |
| WASD (no item selected) | Pans the camera to view the map. |
| Enter | Opens the Quit Confirmation window. |

**Controller**

| Input | Behavior |
|---|---|
| X (Xbox) / Square (PS) | Selects an item in the inventory. |
| Left Stick / D-Pad (item selected) | Moves the selected item to an empty slot, or swaps it with the item currently in the target slot. |
| LB/RB (Xbox) / L1/R1 (PS) (item selected) | Transfers the selected item to the previous/next character, only if that character has an empty slot. |
| Left Stick / D-Pad (no item selected) | Pans the camera to view the map. |
| Select (Xbox: View, PS: Select/Share) | Opens the Quit Confirmation window. |

### Camera Behavior While Paused

- If no item is selected, directional input pans the camera across the map instead of moving a character.
- On unpause, the camera snaps back to whichever rescuer is currently controlled.
- If the camera was panned away and the player switches the controlled character (Q/E or bumpers) while still paused, the camera pans to the newly controlled character instead of the previously controlled one.

> [!warning]
> Item transfer via Q/E (or bumpers) must silently fail — not error or crash — when the target character's inventory is full. See [[06 - Items]] for inventory slot rules.

### Control Summary Table

| System | Keyboard | Controller |
|---|---|---|
| Movement | WASD | Left Stick / D-Pad |
| Climb Up / Down / Crawl | W / S | Left Stick / D-Pad ↑↓ |
| Use Item | F | X / Square |
| Ability 1 (Ladder / Shield Toggle / Jump) | J | B / Circle |
| Ability 2 (Axe / Stun Baton / Jet Swim) | K | LT / L2 |
| Activate | Spacebar | A / Cross |
| Cycle Character | Q / E | LB+RB / L1+R1 |
| Pause / Unpause | Tab | Start |
| Quit Confirmation | Enter (paused) | Select (paused) |

## Notes

> [!note]
> Controller support should be treated as a first-class input method, not a fallback — all context-sensitive keyboard behavior (ladder climbing, crawling, pause-menu item handling) must have a matching controller equivalent, as documented above.

### TODO

- [ ] Confirm final controller button glyphs/icons for on-screen prompts.
- [ ] Audit level design to ensure Firefighter/Riot Officer sections never require jumping or crawling, and that gaps requiring those actions gate on a Rescue Specialist swap (see [[05 - Level Design]]).
- [ ] Define rebinding support (keyboard and controller) if in scope for the jam build.
- [ ] Define analog stick deadzone values.
- [ ] Confirm behavior when both keyboard and controller input are active simultaneously.
- [ ] Add Nintendo Switch Pro Controller mapping if platform support expands beyond Windows PC.
