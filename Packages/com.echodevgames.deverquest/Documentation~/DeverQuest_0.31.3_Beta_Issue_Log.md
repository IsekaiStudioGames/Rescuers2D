# DeverQuest 0.31.3 Beta Issue Log
## Pathway 7 — Guild Economy and Item Operations

**Source build:** 0.31.2 Beta 1  
**Patch build:** 0.31.3 Beta 1  
**Status:** Prepared, awaiting Unity verification

---

## DQ-0312-038 — Merchant rules are not enforced consistently

**Type:** Guild Shop / economy policy  
**Severity:** P1  
**Status:** Patched in 0.31.3

### Previous behavior

Shop Profiles contained a member-availability flag, but purchasing and selling
were primarily controlled by the visible UI. There was no durable active
Quartermaster, no explicit open/closed state, and no merchant-level control for
purchases, member sales, or automatic leadership approval.

### 0.31.3 correction

Shop Profiles now support:

- Shop Open
- Available to Members
- Allow Purchases
- Buy Member Items
- Leadership Approval Threshold in copper
- A closed-message field

The active Shop Profile is persisted by asset GUID. Purchases validate that the
item is stocked by that active merchant. Inventory sales require an active,
available Quartermaster that is currently buying from members.

---

## DQ-0312-039 — Economy activity is split across unrelated logs

**Type:** Audit / reporting  
**Severity:** P1  
**Status:** Patched in 0.31.3

### Previous behavior

Purchases had a Purchase Ledger, trades had a Trade Ledger, and some coin
changes appeared in reward transactions or the Guild audit. There was no single
searchable record showing the complete local item-and-coin economy.

### 0.31.3 correction

Added a persistent local Economy Transaction Ledger covering:

- Purchase requested
- Purchase completed
- Purchase approved
- Purchase denied
- Inventory sale
- Leadership item grant
- Leadership coin grant
- Denomination exchange
- Redemption fulfillment

Each record may retain:

- Transaction ID
- Actor
- Recipient account, developer, and Adventurer
- Item and quantity
- Copper amount
- Balance delta
- Resulting balance
- Coin-piece count before and after exchange
- Related purchase or ownership ID
- Administrative note
- UTC timestamp

The newest 1,000 records are retained locally in EditorPrefs. The Economy
workspace supports text and transaction-type filters plus CSV export.

---

## DQ-0312-040 — Leadership cannot safely grant test or reward items

**Type:** Guild administration / item operations  
**Severity:** P1  
**Status:** Patched in 0.31.3

### Previous behavior

Testing a Guild economy or awarding an exceptional item required direct asset,
EditorPrefs, or account-data editing. Those changes were difficult to audit and
could accidentally bypass ownership rules.

### 0.31.3 correction

Guild leadership may now issue confirmed:

- Item grants
- Coin grants

Item grants:

- Require an enabled target account
- Respect maximum-owned quantity
- Preserve binding and item metadata
- Use `LeadershipGrant` provenance
- Do not silently auto-equip equipment
- Add granted Spell knowledge when appropriate
- Reject real-world Redemption items

Coin grants:

- Require a positive amount
- Preserve the canonical copper balance
- Add loose copper pieces until exchanged at Guild Hall
- Update total earned coin

Both operations create Economy Ledger and Guild audit records.

---

## DQ-0312-041 — Coin denomination exchange is unclear

**Type:** Currency UX  
**Severity:** P1  
**Status:** Patched in 0.31.3

### Previous behavior

The exchange button reported conversion ratios but did not explain the
difference between canonical copper value and the number of physical coin
pieces being carried.

### 0.31.3 correction

The Economy workspace displays:

- Canonical purse value
- Platinum, gold, silver, and copper pieces
- Total physical piece count

Exchange now reports the piece count before and after consolidation. It never
changes the canonical copper value. A transaction is recorded only when the
physical number of pieces changes.

---

## DQ-0312-042 — Purchase history lacks a unified balance trail

**Type:** Transaction integrity  
**Severity:** P1  
**Status:** Patched in 0.31.3

### Previous behavior

Purchase records retained cost and status, but sales, grants, denomination
changes, and final balances could not be reviewed together.

### 0.31.3 correction

Economy records now store signed balance deltas and the recipient's resulting
balance. Purchase requests record no balance change. Completed purchases and
approvals record negative deltas. Sales and coin grants record positive deltas.
Denomination exchanges record a zero-value change with piece-count movement.

---

## Compatibility

- Existing Shop Profiles retain member availability and receive safe defaults
  for new merchant controls.
- Existing Shop Item IDs, inventory entries, balances, purchase records, and
  Trade Ledger entries are preserved.
- The new Economy Ledger begins with transactions performed under 0.31.3; old
  Purchase and Trade records remain available in their existing ledgers.
- The active Shop Profile resolves from its persisted GUID or falls back to the
  first available Shop Profile.
- No banking, lending, auction house, crafting market, taxes, or housing storage
  were added.

---

## Required retest

- [ ] Install 0.31.3 with zero compilation errors.
- [ ] Run Release Readiness.
- [ ] Open the Economy workspace.
- [ ] Select a Shop Profile and restart Unity.
- [ ] Confirm the active Shop Profile persists.
- [ ] Close the shop and test Member access.
- [ ] Disable purchases and confirm purchase rejection.
- [ ] Disable member sales and confirm Inventory sale rejection.
- [ ] Configure an approval threshold and test a Member purchase above it.
- [ ] Approve and deny separate requests.
- [ ] Purchase an inexpensive item directly.
- [ ] Sell one item.
- [ ] Grant one item to the current account.
- [ ] Grant one item to another account.
- [ ] Grant coin to both accounts.
- [ ] Attempt to grant a Redemption item.
- [ ] Confirm the grant is rejected.
- [ ] Earn loose copper and consolidate denominations.
- [ ] Confirm purse value stays unchanged.
- [ ] Search and filter the Economy Ledger.
- [ ] Export the ledger to CSV.
- [ ] Restart Unity and confirm transaction persistence.

---

## Current verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
