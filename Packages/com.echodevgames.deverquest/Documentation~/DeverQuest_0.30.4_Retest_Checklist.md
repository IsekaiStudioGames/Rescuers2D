# DeverQuest 0.30.4 Focused Retest

## Installation

- [ ] Replace 0.30.3 with the 0.30.4 tarball.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Run the Release Readiness Check and confirm version 0.30.4.

## DQ-0302-002 — Ambience

- [ ] Delete the 0.30.3 Ambience Profile that displays Missing Script.
- [ ] Create a new Ambience Profile.
- [ ] Confirm its Inspector displays name, description, clips, volume, shuffle,
      and active-Quest options.
- [ ] Add at least one valid AudioClip.
- [ ] Assign the profile in DeverQuest.
- [ ] Confirm the picker lists it.
- [ ] Play, pause, stop, and switch ambience.
- [ ] Restart Unity and confirm the assignment persists.

## DQ-0302-004 — Starter Identity Catalog

- [ ] Delete any partial
      `Assets/DeverQuest/IdentityCatalogs/OriginalStarter` folder from 0.30.3.
- [ ] Run Create Original Starter Identity Catalog.
- [ ] Confirm Ancestries, Classes, Faiths, and Catalog assets have valid scripts.
- [ ] Rerun the generator and confirm it repairs/preserves without duplicates.
- [ ] Restart Unity and confirm the active Catalog still loads.

## Secondary asset audit

- [ ] Create an Ability Profile.
- [ ] Create a Spell.
- [ ] Create a Companion Catalog.
- [ ] Create an Encounter Profile.
- [ ] Create a Shop Profile.
- [ ] Confirm none display Missing Script.
