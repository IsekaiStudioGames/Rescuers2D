# DeverQuest 0.24.0 Regression Checklist

## Installation and Migration

- [ ] Install the 0.24.0 tarball in Unity 2022.3 or newer.
- [ ] Open **Tools > DeverQuest > Developer Companion**.
- [ ] Confirm existing accounts, coin, equipment, and inventory remain.
- [ ] Confirm legacy inventory receives ownership metadata without changing
  quantity.

## Rare Loot and Binding

- [ ] Create Shop Items for Common, Rare, and Legendary rarity.
- [ ] Confirm rare-or-better acquisitions receive separate ownership IDs.
- [ ] Confirm Bind on Pickup and Account Bound items cannot be offered.
- [ ] Confirm real-reward Redemption items cannot be traded or “used.”

## Trading Post

- [ ] Create two enabled Guild accounts.
- [ ] Offer an unbound item and confirm it leaves the sender's pack.
- [ ] Log in as the recipient, accept it, and confirm ownership is preserved.
- [ ] Reject a second offer; log in as sender and reclaim it.
- [ ] Cancel an open offer and confirm the item returns exactly once.
- [ ] Confirm all outcomes remain visible in Permanent Trade Ledger.

## Real Rewards

- [ ] Create a Redemption Shop Item and choose its real-reward type.
- [ ] Confirm purchase creates a leadership approval request.
- [ ] Approve it and confirm it remains pending fulfillment.
- [ ] Enter a delivery reference and choose **Mark Delivered**.
- [ ] Confirm the history shows Redeemed and the delivery reference.
- [ ] Confirm a Member cannot approve or mark fulfillment.

## Existing Systems

- [ ] Complete a Quest, receive loot, write the Chronicle, and publish it.
- [ ] Confirm Git commit/push, playlists, wellness, rewards, and timecards
  still operate.
- [ ] Confirm the Console contains no compilation exceptions.
