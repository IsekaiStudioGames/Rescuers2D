# DeverQuest 0.31.2 Beta Issue Log
## Pathway 6 — Inventory and Equipment Clarity

**Source build:** 0.31.1 Beta 1  
**Patch build:** 0.31.2 Beta 1  
**Status:** Prepared, awaiting Unity verification

---

## DQ-0311-032 — Inventory entries lack durable classification

**Type:** Item data / inventory clarity  
**Severity:** P1  
**Status:** Patched in 0.31.2

### Previous behavior

Inventory entries primarily stored a Shop Item type, rarity, binding, quantity,
weight, and a free-form acquisition source. This was not enough to distinguish
equipment, provisions, tradeskill supplies, crafting components, lore books,
merchant trash, Quest items, tools, containers, keys, trophies, Companion
supplies, environmental protection, services, and future housing items.

### 0.31.2 correction

- Added a secondary Item Category independent from use behavior.
- Added subcategory and tags.
- Added lore text to Shop Items.
- Added stack-size, resale, drop, Quest-protection, and auto-equip rules.
- Added equipment family, two-handed flag, required-skill identifier, and tags.
- Existing items infer a safe category from their current Shop Item type.
- The full crafting system remains deferred.

---

## DQ-0311-033 — Equipment cannot be compared or safely managed from inventory

**Type:** Equipment UX  
**Severity:** P1  
**Status:** Patched in 0.31.2

### Previous behavior

Equipment could be granted or auto-equipped, but the user lacked a dedicated
inventory view showing the current item in the same slot, rule differences, and
safe Equip or Unequip operations.

### 0.31.2 correction

- Added an Inventory workspace.
- Added an Equipped Gear section.
- Added candidate-versus-current comparison.
- Added slot, family, AC, ability, damage, handedness, and weight summaries.
- Added Equip and Unequip actions.
- Bind-on-equip items become account-bound when equipped.
- Unequipping older loadout equipment repairs its missing inventory record.
- Added a bulk repair action for legacy equipped items that were never added to
  the pack.

---

## DQ-0311-034 — Loot provenance is too vague

**Type:** Chronicle / item history  
**Severity:** P1  
**Status:** Patched in 0.31.2

### Previous behavior

Loot generally recorded a free-form source such as `Encounter Loot`. It did not
retain the Contract, Quest Run, Encounter, Monster, or original acquisition
timestamp through later trades.

### 0.31.2 correction

Inventory entries now retain:

- Origin type
- Original source
- Original acquisition time
- Contract ID
- Quest Run ID
- Encounter ID
- Monster ID
- Monster name
- Equipment ID
- Snapshot resale value

Trade escrow preserves the original provenance while also recording the current
transfer source.

Generated Timecards now include category and origin in their inventory summary.

---

## DQ-0311-035 — Carry load does not explain its components

**Type:** Encumbrance UX  
**Severity:** P1  
**Status:** Patched in 0.31.2

### Previous behavior

The UI showed only total weight and capacity.

### 0.31.2 correction

The Inventory workspace now shows:

- Inventory weight
- Coin weight
- Total weight
- Capacity
- Remaining capacity
- Load percentage
- Load state
- Capacity formula

Load states are:

- Light
- Comfortable
- Heavy
- Near Limit
- Encumbered

Quartermaster cards warn before a purchase would exceed capacity.

---

## DQ-0311-036 — Inventory actions can target the wrong item or destroy gear

**Type:** Inventory safety  
**Severity:** P0/P1  
**Status:** Patched in 0.31.2

### Previous behavior

- Use selected the first matching Shop Item stack rather than a specific
  ownership entry.
- Drop occurred immediately.
- Equipped items could be removed from inventory without clearing or protecting
  their equipped state.
- Quest-protected item policy was not represented.
- Merchant-trash selling did not exist.

### 0.31.2 correction

- Use now targets the selected ownership ID.
- Drop 1 and Drop Stack require confirmation.
- Equipped items must be unequipped before dropping or selling.
- Quest-protected entries cannot be dropped, sold, or traded.
- Non-droppable rules are enforced in the service layer.
- Guild Hall sales award the snapshotted resale value.
- Selling is blocked during an active Quest.
- Drop and sale operations create Guild audit records.
- Equipment, redemption items, rare items, and one-stack items retain unique
  ownership behavior.
- Merchant trash may stack even when its old behavior type was Equipment.

---

## DQ-0311-037 — Starter loadout equipment may exist outside inventory

**Type:** Migration / persistence  
**Severity:** P1  
**Status:** Patched in 0.31.2

### Previous behavior

Starter loadouts equipped assets directly without adding corresponding inventory
entries. Unequipping could therefore make the item appear to vanish.

### 0.31.2 correction

- New starter loadouts add equipment to inventory before equipping it.
- Existing equipped assets without inventory records are detected.
- The Inventory workspace can repair those records without changing slots.
- Unequip also repairs a missing record before returning the item to the pack.
- Release Readiness reports inventory integrity issues.

---

## Compatibility

- Existing Shop Item IDs, Equipment IDs, inventory ownership IDs, balances,
  bindings, and quantities are preserved.
- Existing inventory entries infer classification and provenance defaults.
- Existing starter equipment may produce one advisory until repaired.
- No crafting recipes, banks, houses, biome effects, or skill progression were
  added.
- Shop Item remains the current item-definition asset for Beta. A neutral shared
  runtime item-definition package remains a later architecture decision.

---

## Required retest

- [ ] Install 0.31.2 with zero compilation errors.
- [ ] Run Release Readiness.
- [ ] Open Inventory and Equipment.
- [ ] Repair any legacy equipped inventory records.
- [ ] Compare two items for the same slot.
- [ ] Equip and unequip both.
- [ ] Confirm Bind On Equip becomes account-bound.
- [ ] Attempt to drop equipped gear.
- [ ] Attempt to drop a Quest-protected item.
- [ ] Drop one item with confirmation.
- [ ] Drop a stack with confirmation.
- [ ] Sell one merchant-trash item.
- [ ] Sell a stack.
- [ ] Confirm sales are blocked during an active Quest.
- [ ] Acquire loot from a tactical Encounter.
- [ ] Confirm Contract, Run, Encounter, and Monster provenance.
- [ ] Trade that loot and confirm origin survives.
- [ ] Confirm Timecard inventory summary contains category and origin.
- [ ] Restart Unity and confirm all changes persist.

---

## Current verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
