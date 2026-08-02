# DeverQuest Personal Notes
## Consolidated for 0.31.3 Guild Economy and Item Operations

**Current build:** 0.31.3 Beta 1  
**Scope rule:** Strengthen merchant and administrative item operations without opening banking, crafting markets, loans, auctions, or housing storage.

---

# Immediate Importance

## One active Quartermaster

The Guild needs one clearly selected Shop Profile that controls current buying
and selling behavior. The profile should survive workspace changes, script
reloads, and Unity restarts.

**0.31.3:** active Shop Profile GUID persistence implemented.

---

## Enforced merchant policy

A Shop Profile now answers:

- Is the shop open?
- May ordinary Members use it?
- Are purchases allowed?
- Is the merchant buying items from Members?
- At what copper value does leadership approval become mandatory?

These checks belong in services, not only disabled UI buttons.

**0.31.3:** implemented.

---

## Unified transaction history

Economy activity needs a common trail across purchases, sales, grants,
exchanges, approvals, denials, and fulfillment.

Records should explain:

- Who acted
- Who received the result
- Which item was involved
- How many
- How much coin changed
- The resulting balance
- Which purchase or ownership record it relates to
- Why it happened

**0.31.3:** local 1,000-record Economy Ledger implemented with search, type filters, and CSV export.

---

## Safe leadership grants

Beta testing and studio rewards need a supported way to grant items or coin
without manually editing JSON or EditorPrefs.

Rules:

- Leadership permission required
- Explicit recipient
- Confirmation required
- Maximum-owned limit respected
- Redemption workflow cannot be bypassed
- Equipment is not silently equipped
- Provenance and audit entries are mandatory

**0.31.3:** implemented.

---

## Denomination clarity

The purse has two separate ideas:

1. Canonical value measured in copper
2. Physical platinum, gold, silver, and copper pieces contributing to weight

Exchange may reduce physical pieces, but it may never change canonical value.

**0.31.3:** before/after piece reporting and transaction records implemented.

---

# Medium Importance

## Merchant stock limits and restocking

Future Shop Profiles may need:

- Limited stock
- Restock interval
- Per-account purchase limits
- Daily or weekly limits
- Item availability windows
- Required Guild rank
- Required Project or Department
- Required Quest completion
- Rotating stock

Do not add these until the basic purchase and grant ledger passes.

---

## Per-merchant pricing

Future pricing may include:

- Buy multiplier
- Sell multiplier
- Reputation modifier
- Difficulty modifier
- Limited-time discount
- Merchant specialization

The Shop Item should retain a base value while the merchant calculates the final
offer visibly. Never hide a modifier from the purchase preview or transaction
record.

---

## Item and coin corrections

Leadership may eventually need correction actions rather than grants:

- Remove an item issued by mistake
- Reverse a duplicate transaction
- Correct a balance with a signed adjustment
- Restore an accidentally sold item

Corrections need an immutable reversal record, reason, actor, and link to the
original transaction. Do not implement destructive editing of old ledger rows.

---

## Shared Guild economy ledger

The 0.31.3 Economy Ledger is local EditorPrefs data. A multi-clone Guild needs
append-only shared records similar to planned Quest Run records:

```text
SharedGuild/
└── Economy/
    ├── Transaction-A.json
    ├── Transaction-B.json
    └── Transaction-C.json
```

This later system should support merge-safe transactions, signatures, imported
account snapshots, and duplicate detection.

---

## Merchant interface separation

The Guild Shop is still rendered under Guild Hall while configuration and the
ledger live in Economy. Later UI cleanup may separate:

- Member Shop
- Quartermaster Administration
- Purchase Approval Queue
- Redemption Fulfillment
- Trade Post
- Economy Ledger

Keep serialized data stable while reorganizing visible workspaces.

---

## Currency conversion policy

The earlier design rule remains valuable:

- Field rewards retain their original physical denominations
- Conversion occurs only at appropriate Guild Hall, banker, money changer, or merchant locations
- No silent conversion during combat or drops

0.31.3 already lets rewards accumulate as loose copper and consolidates them at
Guild Hall. A fuller physical-currency model should wait until economy tests are
complete.

---

# Low Importance

## Quartermaster flavor

Possible polish:

- Merchant portrait
- Greeting variants
- Closing-hour flavor text
- Purchase and sale sound profiles
- Category icons
- Favorite or pinned items
- Recent purchases
- Compact receipt copy button

---

## Economy charts

Possible later views:

- Coin earned versus spent
- Top purchased items
- Merchant-trash sales
- Grants by leader
- Economy by Project
- Daily and weekly transaction totals

Charts are reporting polish, not a prerequisite for correct balances.

---

# Expansion 2.0

## Banking

- Account storage
- Character storage
- Guild vault
- Deposit and withdrawal records
- Shared permissions
- Storage capacity
- Protected Quest items
- Bank-specific denomination exchange

## Player auctions and markets

- Listings
- Bids
- Buyout
- Expiration
- Escrow
- Fees
- Cross-account delivery
- Fraud and duplicate protection

## Crafting economy

- Material values
- Recipe costs
- Station fees
- Quality tiers
- Salvage
- Vendor specialization
- Biome supply and demand

## Housing economy

- Property purchase
- Rent or upkeep
- Furniture inventory
- Storage permissions
- Crafting stations
- Guild housing

These systems remain outside the current Beta gate.

---

# Completed

- Durable item classification introduced in 0.31.2.
- Equipment comparison and guarded operations introduced in 0.31.2.
- Loot provenance introduced in 0.31.2.
- Active Quartermaster selection added in 0.31.3.
- Merchant availability rules added in 0.31.3.
- Economy Transaction Ledger added in 0.31.3.
- Leadership item and coin grants added in 0.31.3.
- Denomination exchange clarity added in 0.31.3.
- Economy Release Readiness validation added in 0.31.3.

---

# Current Decision

Install and smoke-test 0.31.3 before adding further economy depth.

The next strongest pathway after it opens cleanly is **0.31.4: Quest Archive and Chronicle Navigation**, consolidating active Quest events, completed Quest reports, reward summaries, attachments, and history navigation without changing core persistence.
