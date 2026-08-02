# DeverQuest 0.30.0 - Release Candidate and Scope Lock

Milestone 30 closes the feature-expansion phase and begins the release-candidate
phase. The goal is not to add another major RPG subsystem. The goal is to make
the existing productivity loop dependable enough to package, demonstrate, and
maintain.

## Scope lock

The 0.30 release candidate treats the following as the finished product core:

- authenticated Guild accounts and persistent Adventurers;
- deliberate Quests, pause/resume recovery, idle detection, and wellness;
- Focus Stages, Contracts, Git notes, voice memos, and external activity;
- Chronicle history, timecards, integrity checks, and shared Guild records;
- rewards, shops, trading, equipment, spells, companions, and tactical combat;
- playlists, warning cues, and ambience;
- optional compensation planning estimates.

New crafting, banking, housing, biome, and broad tradeskill simulations are
post-release expansion candidates. They are intentionally excluded from the
release candidate so the existing package can be completed without turning the
finish line into a wandering monster.

## Audio transport correction

Unity's internal editor preview transport behaves as one shared channel. Older
DeverQuest builds attempted to treat that transport as independently stoppable
layered playback. Rapid Next, Previous, Stop, Play, ambience, and warning-cue
combinations could therefore leave the transport in an inconsistent state.

Version 0.30.0 introduces explicit preview ownership:

- either the playlist or ambience owns the long-form preview channel;
- starting one releases the other's UI and playback state;
- Next, Previous, Play, and Stop perform deterministic channel replacement;
- warning cues temporarily interrupt the long-form clip;
- the long-form clip resumes from its captured sample position after the cue;
- assembly reload and editor shutdown stop and clear preview state;
- playlist completion detection uses the actual transport state rather than an
  estimated wall-clock ending.

This favors correctness over unsupported pseudo-mixing. Playlist and ambience
are mutually exclusive, while short warning cues can still interrupt and
restore whichever long-form source is active.

## Release Readiness Check

Run:

**Tools > DeverQuest > Run Release Readiness Check**

The report checks:

- package and Unity version expectations;
- completed developer-profile setup;
- writable timecard storage;
- Chronicle integrity policy;
- shared Guild repository availability when enabled;
- editor audio transport and playlist completion support;
- active Quest state before migration or clean-install regression.

Blockers should be corrected before release regression. Advisories describe
optional or environment-dependent capabilities that do not prevent the timer
core from operating.

## Release-candidate rule

After 0.30.0, changes should be limited to:

1. compilation fixes;
2. data-loss and migration fixes;
3. broken timer, pause, finalization, or timecard behavior;
4. audio transport regressions;
5. documentation and usability corrections;
6. measured performance fixes.

Any new major system should begin in a later minor-version roadmap rather than
entering the release candidate.
