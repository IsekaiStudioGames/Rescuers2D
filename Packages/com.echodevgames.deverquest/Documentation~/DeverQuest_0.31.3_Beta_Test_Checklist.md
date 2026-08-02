# DeverQuest 0.31.3 Beta Test Checklist
## Quest 7 — The Quartermaster's Ledger

**Build:** 0.31.3 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

---

# A. Upgrade and readiness

- [ ] Install `com.echodevgames.deverquest-0.31.3.tgz`.
- [ ] Confirm Package Manager reports 0.31.3.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm Guild Economy reports an active Shop Profile or a clear advisory.
- [ ] Confirm no duplicate Economy transaction IDs are reported.
- [ ] Confirm existing balances and inventory remain unchanged.
- [ ] Confirm existing Purchase and Trade ledgers remain readable.

---

# B. Active Quartermaster

- [ ] Open the Economy workspace.
- [ ] Select a Shop Profile.
- [ ] Switch to another workspace and return.
- [ ] Confirm the selected profile remains active.
- [ ] Restart Unity.
- [ ] Confirm the same profile remains active.
- [ ] Confirm its stock count is correct.
- [ ] Select the profile asset from the Economy workspace.
- [ ] Confirm the Inspector displays all merchant controls.

---

# C. Merchant availability

Using a Member account:

- [ ] Set Shop Open to disabled.
- [ ] Confirm the Member cannot browse or purchase.
- [ ] Confirm the configured closed message appears.
- [ ] Confirm leadership may still inspect the shop.
- [ ] Reopen the shop.
- [ ] Disable Available to Members.
- [ ] Confirm the Member is denied.
- [ ] Re-enable Member access.
- [ ] Disable Allow Purchases.
- [ ] Confirm purchase attempts are rejected at the service layer.
- [ ] Re-enable purchases.
- [ ] Disable Buy Member Items.
- [ ] Open Inventory.
- [ ] Confirm selling is blocked with a clear explanation.
- [ ] Re-enable member sales.
- [ ] Confirm selling becomes available again.

---

# D. Direct purchases

- [ ] Purchase a stocked item below the approval threshold.
- [ ] Confirm the correct copper amount is deducted.
- [ ] Confirm the item enters inventory once.
- [ ] Confirm provenance says Guild Shop.
- [ ] Confirm a Purchase record appears in the Economy Ledger.
- [ ] Confirm the record includes item, recipient, cost, negative delta, and resulting balance.
- [ ] Attempt to purchase an item not stocked by the active profile.
- [ ] Confirm the service rejects it.
- [ ] Attempt to purchase above maximum owned.
- [ ] Confirm rejection.
- [ ] Attempt to purchase without enough coin.
- [ ] Confirm rejection without an Economy transaction.

---

# E. Approval threshold

Configure a positive Leadership Approval Threshold.

- [ ] Use a Member account to request an item at or above the threshold.
- [ ] Confirm no coin is deducted at request time.
- [ ] Confirm a PurchaseRequested Economy record is created.
- [ ] Approve the request as leadership.
- [ ] Confirm the item is delivered once.
- [ ] Confirm coin is deducted once.
- [ ] Confirm denominations reflect the new balance.
- [ ] Confirm a PurchaseApproved record is created.
- [ ] Create a second request.
- [ ] Deny it.
- [ ] Confirm no coin or item changes.
- [ ] Confirm a PurchaseDenied record is created.
- [ ] Confirm the Purchase Ledger and Economy Ledger agree on status.

---

# F. Inventory sales

- [ ] Sell one merchant-trash item.
- [ ] Confirm inventory quantity decreases by one.
- [ ] Confirm coin increases by the expected resale value.
- [ ] Confirm a Sale Economy record appears.
- [ ] Confirm the sale records quantity, positive delta, and resulting balance.
- [ ] Sell an entire stack.
- [ ] Confirm the total value is quantity × unit resale value.
- [ ] Attempt to sell equipped gear.
- [ ] Attempt to sell a Quest-protected item.
- [ ] Attempt to sell a zero-value item.
- [ ] Attempt to sell during an active Quest.
- [ ] Confirm every prohibited action is rejected without changing the ledger.

---

# G. Leadership item grants

Create or use two enabled accounts.

- [ ] Select Account A as recipient.
- [ ] Select a normal Shop Item.
- [ ] Enter a quantity and grant note.
- [ ] Confirm the operation asks for confirmation.
- [ ] Confirm Account A receives the exact quantity.
- [ ] Confirm provenance says Leadership Grant.
- [ ] Confirm the note appears in origin data or the Economy Ledger.
- [ ] Confirm granted equipment is not silently auto-equipped.
- [ ] Grant a Spell item.
- [ ] Confirm the target learns the Spell when appropriate.
- [ ] Grant an item to Account B.
- [ ] Log in as Account B and confirm persistence.
- [ ] Attempt to exceed maximum owned.
- [ ] Confirm the entire grant is rejected.
- [ ] Attempt to grant a real-world Redemption item.
- [ ] Confirm it is rejected.
- [ ] Confirm each successful grant creates one ItemGrant record and one Guild audit entry.

---

# H. Leadership coin grants

- [ ] Grant 500 copper to Account A.
- [ ] Confirm Account A gains exactly five silver of canonical value.
- [ ] Confirm the physical purse initially receives loose copper pieces.
- [ ] Confirm total earned coin increases.
- [ ] Confirm a CoinGrant transaction appears.
- [ ] Grant coin to Account B.
- [ ] Confirm Account B receives it without switching data between accounts.
- [ ] Attempt zero and negative grants.
- [ ] Confirm both are rejected.
- [ ] Confirm Members cannot access grant controls.

---

# I. Denomination exchange

- [ ] Record canonical balance and physical piece count.
- [ ] Consolidate coin denominations.
- [ ] Confirm canonical balance is unchanged.
- [ ] Confirm physical piece count decreases when conversion is possible.
- [ ] Confirm platinum, gold, silver, and copper totals are correct.
- [ ] Confirm a DenominationExchange transaction records before and after pieces.
- [ ] Exchange again without earning new loose coin.
- [ ] Confirm the UI reports that the purse is already consolidated.
- [ ] Confirm no redundant exchange record is added.

---

# J. Economy Ledger

- [ ] Search by Adventurer name.
- [ ] Search by developer name.
- [ ] Search by item name.
- [ ] Search by grant note.
- [ ] Search by purchase or ownership ID.
- [ ] Filter to Purchase.
- [ ] Filter to Sale.
- [ ] Filter to ItemGrant.
- [ ] Filter to CoinGrant.
- [ ] Filter to DenominationExchange.
- [ ] Confirm visible income and expense totals respect filters.
- [ ] Export CSV.
- [ ] Open the CSV and confirm columns and escaping are valid.
- [ ] Restart Unity.
- [ ] Confirm the ledger persists.
- [ ] Confirm it retains no more than the newest 1,000 records during a synthetic stress test.

---

# K. Redemption workflow

- [ ] Request a Redemption item normally.
- [ ] Approve it through leadership.
- [ ] Confirm the Economy Ledger records request and approval.
- [ ] Mark the reward fulfilled with a delivery reference.
- [ ] Confirm a RedemptionFulfilled transaction appears.
- [ ] Confirm the fulfillment reference remains in Purchase History.
- [ ] Confirm leadership Item Grant cannot bypass this workflow.

---

# L. Regression

- [ ] Open Inventory and confirm item controls still work.
- [ ] Open Tactics and confirm no economy operation resolves combat.
- [ ] Complete a Quest and confirm reward coin still records correctly.
- [ ] Buy, sell, and exchange after Quest completion.
- [ ] Confirm Trade Ledger behavior is unchanged.
- [ ] Confirm shared Guild publishing still works.
- [ ] Confirm Timecards remain writable.
- [ ] Confirm no data loss after restart.

---

# Verdict

- [ ] **PASS** — merchant controls, purchases, sales, grants, exchange, and ledger pass.
- [ ] **CONDITIONAL PASS** — core operations pass with documented P1 limitations.
- [ ] **FAIL** — any balance, item, permission, duplication, or persistence error remains.
