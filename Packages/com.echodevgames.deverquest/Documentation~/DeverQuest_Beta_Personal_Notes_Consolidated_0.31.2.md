# DeverQuest Personal Notes
## Consolidated for 0.31.2 Inventory and Equipment Clarity

**Current build:** 0.31.2 Beta 1  
**Scope rule:** Strengthen the current inventory loop without opening crafting,
banking, housing, or broad skill progression.

---

# Immediate Importance

## Durable item classification

The existing Shop Item type describes behavior but not the full identity of an
item. 0.31.2 adds a separate category model for:

- Equipment
- Consumables
- Provisions
- Tradeskill supplies
- Crafting components
- Lore books
- Merchant trash
- Quest items
- Tools
- Containers
- Keys
- Trophies
- Companion supplies
- Housing items
- Environmental protection
- Currency
- Services
- Spells
- Other

Subcategories and tags provide expansion room without adding a new enum for
every future noun.

**0.31.2:** implemented, awaiting Unity verification.

---

## Equipment comparison

The inventory needs to answer:

- What slot does this use?
- What is currently equipped there?
- What changes if I equip it?
- Is it one-handed or two-handed?
- What family and future skill does it belong to?
- How heavy is it?
- What damage or defense does it provide?

**0.31.2:** comparison and safe Equip/Unequip implemented.

---

## Loot provenance

Items should remember where they came from even after a trade.

Current provenance fields:

- Original source type
- Original source text
- Original acquisition time
- Contract ID
- Quest Run ID
- Encounter ID
- Monster ID and name
- Equipment ID

**0.31.2:** implemented for Shop, Encounter loot, direct Equipment drops, and
Trade escrow.

---

## Safe inventory actions

Release-safe rules:

- Use targets one ownership entry.
- Drop requires confirmation.
- Drop Stack requires confirmation.
- Equipped gear must be unequipped.
- Quest-protected items cannot leave the pack.
- Non-droppable assets are enforced in the service layer.
- Sales happen at Guild Hall.
- Sales are blocked during an active Quest.
- Audit records are written for successful operations.

**0.31.2:** implemented.

---

## Carry-load explanation

Carry load now separates:

- Inventory weight
- Coin weight
- Total
- Capacity
- Remaining capacity
- Percentage
- State

**0.31.2:** implemented.

---

# Medium Importance

## Neutral item-definition architecture

The current Beta uses `DeverQuestShopItem` as the inventory definition because it
already owns purchasing, effects, equipment references, rarity, binding, and
identity.

A future shared runtime-safe foundation should separate:

```text
Item Definition
├── Identity and classification
├── Inventory rules
├── Economy
├── Equipment reference
├── Consumable effects
├── Lore
└── Tags
```

from:

```text
Shop Listing
├── Item Definition
├── Buy price
├── Stock
├── Approval requirement
└── Merchant restrictions
```

This aligns with the planned shared `EchoRPG.Foundation` direction and prevents
Hackulos from depending on DeverQuest editor services.

---

## Equipment requirements

Current `requiredSkillId` is a forward-compatible identifier only.

Future requirements may include:

- Skill level
- Class
- Ancestry
- Alignment
- Faith
- Character level
- Two-handed conflicts
- Shield compatibility
- Ammunition
- Durability

Do not implement all of these until the skill/catalog architecture is approved.

---

## Inventory stack operations

Future UI may add:

- Split stack
- Merge compatible stacks
- Move quantity to trade escrow
- Favorite
- Lock against sale/drop
- Sort presets
- Saved filters
- Compare two selected items
- Batch merchant sale

---

## Equipment sets and loadouts

Potential later feature:

- Named equipment loadouts
- Switch loadout outside active combat
- Set bonuses
- Missing-item warnings
- Class starter loadout templates
- Companion equipment

---

## Merchant systems

Future Quartermaster rules:

- Merchant-specific buy categories
- Different resale percentages
- Limited stock
- Restock cadence
- Reputation
- Buyback
- Traveling merchants
- Biome merchants
- Currency restrictions

---

# Low Importance

- Rarity colors after Visual Profiles exist.
- Item icons and character portraits.
- Compact inventory cards.
- Tooltips for every tag.
- Recently acquired filter.
- Last sold summary.
- Confirmation preference for Drop 1 while retaining confirmation for Drop Stack.
- Optional alphabetical versus category sorting.

---

# Expansion 2.0

## Crafting and tradeskills

The new categories are foundations only. 2.0 may add:

- Gathering sources
- Material families
- Refining
- Recipes
- Required stations
- Tool requirements
- Skill XP
- Quality
- Durability
- Salvage
- Failure outcomes
- Special Encounter stations

Tradeskills remain grouped by gathering, refining, and production families.

---

## Banking and housing

Future storage should distinguish:

- Character pack
- Account bank
- Guild bank
- Housing storage
- Crafting material storage
- Quest-protected storage
- Companion storage

Do not overload the current inventory list with every future storage container.

---

## Biome and environmental equipment

Environmental Protection items may later:

- Reduce poison accumulation
- Resist heat or cold
- Negate swamp movement penalties
- Improve gathering
- Unlock paths
- Change Encounter timing
- Protect Companions

The category exists now; the biome simulation does not.

---

# Completed

- 0.30.7 readiness achieved a fully green report.
- Repeatable Contract architecture added in 0.30.8.
- Quest Run management added in 0.30.9.
- Tactical Visibility added in 0.31.0.
- Tactical Operations and local Battle Archive added in 0.31.1.
- Item categories added in 0.31.2.
- Equipment family and comparison added in 0.31.2.
- Loot provenance added in 0.31.2.
- Guarded Drop, Sell, Use, Equip, and Unequip added in 0.31.2.
- Legacy equipped-item inventory repair added in 0.31.2.
- Carry breakdown and Inventory integrity readiness check added in 0.31.2.

---

# Current Decision

After 0.31.2 opens cleanly, the next pathway should be **0.31.3: Guild Economy
and Item Operations**, focused on purchase history, merchant sale history,
redemption clarity, trade filtering, and currency rules.

The full 2.0 crafting and storage expansion remains deferred.
