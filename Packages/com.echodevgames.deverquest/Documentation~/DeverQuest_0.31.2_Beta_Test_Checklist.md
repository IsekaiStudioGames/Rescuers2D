# DeverQuest 0.31.2 Beta Test Checklist
## Quest 6 — The Quartermaster's Ledger

**Build:** 0.31.2 Beta 1  
**Legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

The larger repeatable-Contract, Party, Tactical Visibility, and Tactical
Operations matrices remain deferred. This checklist focuses on inventory and
equipment.

---

# A. Installation and migration

- [ ] Install `com.echodevgames.deverquest-0.31.2.tgz`.
- [ ] Confirm Package Manager reports 0.31.2.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm the Inventory integrity check appears.
- [ ] Confirm existing quantities and coin remain unchanged.
- [ ] Confirm existing equipped slots remain unchanged.
- [ ] Confirm existing inventory ownership IDs remain populated.
- [ ] Record any unresolved equipment or duplicate ownership advisory.

---

# B. Inventory workspace

- [ ] Open the Inventory tab.
- [ ] Open `Tools > DeverQuest > Workspaces > Inventory and Equipment`.
- [ ] Confirm both routes open the same workspace.
- [ ] Confirm carry status is visible.
- [ ] Confirm inventory weight is visible.
- [ ] Confirm coin weight is visible.
- [ ] Confirm remaining capacity is visible.
- [ ] Confirm the capacity formula uses Strength and Level.
- [ ] Search by item name.
- [ ] Search by category.
- [ ] Search by subcategory.
- [ ] Search by tag.
- [ ] Filter by one category.
- [ ] Clear the filter.
- [ ] Toggle provenance.
- [ ] Toggle descriptions and lore.
- [ ] Confirm a narrow dock remains readable.

---

# C. Legacy equipped-item repair

- [ ] Confirm whether any equipped item lacks an inventory record.
- [ ] If warned, select **Repair Equipped Inventory Records**.
- [ ] Confirm slots remain equipped.
- [ ] Confirm one inventory entry appears per repaired item.
- [ ] Confirm repaired items use `Equipped Gear Migration` provenance.
- [ ] Restart Unity.
- [ ] Run Release Readiness again.
- [ ] Confirm the warning clears.

---

# D. Classification

Create or select examples for:

- [ ] Equipment
- [ ] Consumable
- [ ] Provision
- [ ] Tradeskill Supply
- [ ] Crafting Component
- [ ] Lore Book
- [ ] Merchant Trash
- [ ] Quest Item
- [ ] Tool
- [ ] Container
- [ ] Key
- [ ] Trophy
- [ ] Companion Supply
- [ ] Environmental Protection
- [ ] Service
- [ ] Spell
- [ ] Other

For at least three entries:

- [ ] Set a subcategory.
- [ ] Add multiple tags.
- [ ] Add lore text.
- [ ] Save and deselect.
- [ ] Confirm internal spaces remain.
- [ ] Confirm the Inventory workspace displays the classification.
- [ ] Restart Unity and confirm persistence.

---

# E. Equipment comparison

Create two items for the same slot with visibly different values.

- [ ] Confirm candidate slot and family display.
- [ ] Confirm current item name displays.
- [ ] Confirm AC delta displays.
- [ ] Confirm ability delta displays.
- [ ] Confirm weight delta displays.
- [ ] Confirm damage dice and damage type display.
- [ ] Confirm a two-handed item is labeled.
- [ ] Confirm required-skill identifier survives serialization.
- [ ] Confirm empty-slot comparison is understandable.
- [ ] Confirm currently equipped comparison says so.
- [ ] Equip the candidate.
- [ ] Confirm the old item remains in inventory.
- [ ] Unequip the candidate.
- [ ] Confirm the item remains in inventory.

---

# F. Binding and ownership

- [ ] Equip an Unbound item.
- [ ] Confirm it remains tradable.
- [ ] Equip a Bind On Equip item.
- [ ] Confirm it becomes Account Bound.
- [ ] Confirm its bound account ID is populated.
- [ ] Confirm it can no longer be traded.
- [ ] Confirm unique equipment entries retain unique ownership IDs.
- [ ] Split one stack through trade escrow.
- [ ] Confirm the remaining stack receives a different ownership ID.
- [ ] Accept the trade.
- [ ] Confirm original provenance survives.

---

# G. Guarded drop

- [ ] Attempt to drop equipped gear.
- [ ] Confirm the service refuses and explains why.
- [ ] Unequip it.
- [ ] Select Drop 1.
- [ ] Cancel the confirmation.
- [ ] Confirm quantity is unchanged.
- [ ] Confirm Drop 1.
- [ ] Confirm quantity decreases by one.
- [ ] Select Drop Stack.
- [ ] Cancel the confirmation.
- [ ] Confirm quantity is unchanged.
- [ ] Confirm Drop Stack.
- [ ] Confirm the entry is removed.
- [ ] Attempt to drop a non-droppable item.
- [ ] Attempt to drop a Quest-protected item.
- [ ] Confirm both are refused.
- [ ] Confirm Guild audit records the successful drop.

---

# H. Quartermaster sales

- [ ] Create Merchant Trash with a clear resale value.
- [ ] Add a stack of three.
- [ ] Confirm resale each is displayed.
- [ ] Sell one.
- [ ] Confirm quantity decreases by one.
- [ ] Confirm coin increases by exactly one resale value.
- [ ] Sell the remaining stack.
- [ ] Confirm the entry is removed.
- [ ] Confirm denominations normalize at Guild Hall.
- [ ] Attempt to sell equipped gear.
- [ ] Attempt to sell a Quest Item.
- [ ] Attempt to sell a Service or Redemption.
- [ ] Confirm each is refused with guidance.
- [ ] Start a Quest.
- [ ] Attempt to sell an ordinary item.
- [ ] Confirm selling is blocked during the active Quest.
- [ ] Confirm Guild audit records successful sales.

---

# I. Exact-entry use

Create two unique entries backed by the same usable Shop Item.

- [ ] Give each entry different provenance.
- [ ] Use the second entry.
- [ ] Confirm the second entry is consumed.
- [ ] Confirm the first entry remains.
- [ ] Test Food.
- [ ] Test Drink.
- [ ] Test Consumable.
- [ ] Test Inn Rest.
- [ ] Test Break Permit.
- [ ] Confirm a missing source Shop Item fails safely.
- [ ] Confirm reusable items are not consumed.

---

# J. Encounter loot provenance

- [ ] Run an Encounter with a Shop Item drop.
- [ ] Confirm origin type is Encounter Loot.
- [ ] Confirm Monster name is recorded.
- [ ] Confirm Monster ID is recorded.
- [ ] Confirm Encounter ID is recorded.
- [ ] Confirm Quest Contract ID is recorded.
- [ ] Confirm Quest Run ID is recorded.
- [ ] Confirm original acquisition time is recorded.
- [ ] Run an Encounter with a direct Equipment drop.
- [ ] Confirm it creates an inventory entry.
- [ ] Confirm it equips according to the existing drop behavior.
- [ ] Unequip it and confirm it remains in the pack.

---

# K. Trade provenance

- [ ] Offer Encounter loot to another account.
- [ ] Confirm trade escrow records category and tags.
- [ ] Accept the trade.
- [ ] Confirm current source says Trade.
- [ ] Confirm original Encounter origin remains.
- [ ] Confirm Monster and Run provenance remain.
- [ ] Reject a second trade.
- [ ] Reclaim it.
- [ ] Confirm original provenance remains.
- [ ] Attempt to trade a Quest-protected item.
- [ ] Confirm it is refused.

---

# L. Carry-load states

Adjust weight to produce:

- [ ] Light
- [ ] Comfortable
- [ ] Heavy
- [ ] Near Limit
- [ ] Encumbered

For each state:

- [ ] Confirm percentage is correct.
- [ ] Confirm remaining capacity is correct.
- [ ] Confirm inventory plus coin equals total.
- [ ] Confirm a Shop purchase warns when it would exceed capacity.
- [ ] Confirm Survival still blocks or warns when encumbered.
- [ ] Confirm a successful drop updates the live total.
- [ ] Confirm a successful sale updates the live total.

---

# M. Timecard and persistence

- [ ] Complete one Quest after acquiring loot.
- [ ] Open the Timecard.
- [ ] Confirm carry status and weight breakdown appear.
- [ ] Confirm inventory entries include category.
- [ ] Confirm inventory entries include origin.
- [ ] Restart Unity.
- [ ] Confirm equipped items persist.
- [ ] Confirm inventory quantities persist.
- [ ] Confirm categories and provenance persist.
- [ ] Confirm balances persist.
- [ ] Run Release Readiness one final time.

---

# Verdict

- [ ] **PASS** — classification, comparison, provenance, carry math, and guarded operations pass.
- [ ] **CONDITIONAL PASS** — no data loss; limited UI or migration advisories remain.
- [ ] **FAIL** — any item loss, duplicate ownership, ghost equipment, incorrect sale, broken binding, or lost provenance occurs.
