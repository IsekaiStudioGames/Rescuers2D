# DeverQuest 0.21.1 Regression Checklist

Run this checklist in a disposable Unity 2022.3 LTS or newer test Project
before promoting the package.

## Installation and Migration

- [ ] Install the `.tgz` through Package Manager.
- [ ] Confirm **Tools > DeverQuest > Developer Companion** appears.
- [ ] Confirm the Console has no compilation errors.
- [ ] Confirm an existing 0.21.0 profile migrates without losing its account,
      character, coin, XP, inventory, Contracts, or Chronicles.
- [ ] Confirm the package reports version 0.21.1.

## Quest Lifecycle

- [ ] Accept a Profile-only Quest.
- [ ] Accept an assigned Contract.
- [ ] Confirm Project and Department defaults.
- [ ] Meditate and resume.
- [ ] Trigger idle pause, acknowledge the return, and resume.
- [ ] Complete the guided finalization flow with closing notes.
- [ ] Confirm finalization never resumes or adds focused time.

## Wellness

- [ ] Trigger a reminder and choose **Acknowledge Only**.
- [ ] Confirm the Chronicle records **Acknowledged**, not **Break Started**.
- [ ] Trigger another reminder and choose **Take Approved Break**.
- [ ] Resume before 80%; confirm **Break Ended Early**, no XP, and no stat
      benefit.
- [ ] Complete at least 80% of a break; confirm **Break Completed**, configured
      XP, and the appropriate hunger/rest/happiness benefit.
- [ ] Stay paused beyond the permitted duration; confirm the excess becomes
      Idle/Unverified.
- [ ] Use a purchased break permit and confirm it remains an Approved Break
      without receiving reminder-only wellness XP.

## Audio and Chronicle Review

- [ ] Start playlist music and ambience.
- [ ] Open the current timecard.
- [ ] Reveal the timecard and its folder.
- [ ] Confirm none of those actions advances, restarts, or replaces the track.
- [ ] Leave Unity inactive beyond the ten-minute review grace; confirm normal
      Unity-focus idle behavior returns.
- [ ] Test Play, Pause, Previous, Next, Shuffle, Repeat, music volume, ambience,
      and warning previews.

## Git and Chronicles

- [ ] Add a Quest Log Note.
- [ ] Link a note to current HEAD.
- [ ] Stage and commit a harmless test change.
- [ ] Push through DeverQuest.
- [ ] Confirm the Chronicle distinguishes each entry type and records hashes
      only where appropriate.
- [ ] Start a new Chronicle and confirm older files remain readable.
- [ ] Confirm integrity verification passes before and after regeneration.

## Parties, Encounters, and Rewards

- [ ] Confirm Contract capacity prevents an extra local participant.
- [ ] Complete a Focus Stage and confirm its guaranteed coin/XP.
- [ ] Resolve an Encounter and confirm the deterministic Battle Chronicle.
- [ ] Confirm defeat never removes focused time or guaranteed work rewards.
- [ ] Purchase and use a provision.
- [ ] Verify Coin Purse, XP, inventory, and character state after relaunch.

## Release Record

- [ ] Preserve the tested `.tgz`.
- [ ] Record its SHA-256 checksum.
- [ ] Tag the matching source revision `v0.21.1`.
- [ ] Record the tested Unity versions and operating systems.
