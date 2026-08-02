# DeverQuest 0.23.0 Regression Checklist

## Installation and Migration

- [ ] Install the 0.23.0 tarball.
- [ ] Confirm the DeverQuest toolbar item appears without Console errors.
- [ ] Confirm 0.22.0 account, character, Quest, media, audio, and Chronicle
      data remain available.
- [ ] Confirm new shared-Guild settings default to disabled.

## Repository Setup

- [ ] Create a disposable shared-folder test location.
- [ ] Enable Shared Guild and select that folder.
- [ ] Validate the repository.
- [ ] Confirm `Records` and `Adventurers` directories are created.
- [ ] Test a read-only or invalid path and confirm a clear warning.

## Publishing

- [ ] Complete a Quest with rewards and a successful Chronicle.
- [ ] Confirm automatic publishing creates one `.guildquest.json` file.
- [ ] Confirm the Adventurer snapshot reflects current level and coin.
- [ ] Select **Publish Last Quest** and confirm it does not duplicate the
      session.
- [ ] Confirm the local Guild audit records the publication.

## Multiple Adventurers

- [ ] Publish records from at least two separate Guild accounts or Unity
      installations using the same repository.
- [ ] Refresh the Hall and confirm both Adventurers appear.
- [ ] Confirm level, XP, coin, Quest, Contract, and streak values.
- [ ] Confirm Project and Department standings aggregate both users.

## Integrity and Fairness

- [ ] Copy the repository before testing destructive cases.
- [ ] Modify a published JSON value manually.
- [ ] Refresh and confirm the record is quarantined as invalid.
- [ ] Create a test session over the suspicious duration threshold and confirm
      a review flag.
- [ ] Create enough same-day Quests to exceed the frequency threshold and
      confirm a review flag.
- [ ] Exceed the configured daily ranking cap and confirm raw time increases
      while ranked time stops at the cap.
- [ ] Confirm flagged sessions do not increase ranked focus.

## Concurrency and Recovery

- [ ] Publish different sessions from two computers or processes into the same
      repository.
- [ ] Confirm both unique records remain readable.
- [ ] Interrupt access to the folder during a publish and confirm local
      Chronicle completion is not lost.
- [ ] Restore access and use **Publish Last Quest** successfully.

## Regression

- [ ] Run the 0.21.1 and 0.22.0 packaged checklists.
- [ ] Confirm Aseprite activity, voice memos, Git, wellness, audio, encounters,
      rewards, finalization, and Chronicle rollover still work.
