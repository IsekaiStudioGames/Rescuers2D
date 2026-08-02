# DeverQuest Personal Notes
## Consolidated after 0.31.4 Quest Archive Work

**Current patch target:** 0.31.4 Quest Archive and Chronicle Navigation  
**Current product lane:** Complete and polish the existing Beta loop before 2.0 systems.

---

# Immediate Importance

## Unified Chronicle workflow

The completed Quest loop needs one readable home.

The Chronicle workspace now combines:

- Active Quest events
- Completed Quest archive
- Quest Story
- Task Objective
- Encounter events
- Rewards
- Quest Log notes
- Commit hashes
- Media
- Wellness
- External activity
- Tactical outcomes
- Closing Notes
- Run and Contract navigation
- Timecard navigation

**0.31.4 status:** implemented, awaiting Unity verification.

---

## Active Quest event feed

The current Quest should explain what has happened without making the user infer state from several disconnected panels.

The event feed derives from existing records:

- Quest Started
- Encounter Completed
- Combat Result
- Quest Log Note
- Linked Commit
- Media Attached
- Wellness Event
- External Craft Activity
- Reward Updated
- Quest Completed

**0.31.4 status:** full feed in Chronicle and compact feed in Current Quest.

---

## Completed Quest cards

Completed Quest review should expose:

- Outcome
- Focused and paused duration
- Project and Department
- Contract and Run ID
- Story and Objective
- Deliverables
- Closing Notes
- Reward Journal
- Commit Journal
- Media
- Tactical results
- Integrity status
- Full timeline

**0.31.4 status:** implemented.

---

## Chronicle navigation safety

Viewing history must never:

- Award coin
- Award XP
- Add focused time
- Create a Quest Run
- Complete a Contract
- Resolve an Encounter
- Delete media metadata
- Rewrite a Timecard

**0.31.4 status:** read-only design; requires regression verification.

---

## Missing-file visibility

A recorded attachment or generated Timecard may later be moved, renamed, or deleted.

Expected behavior:

- Preserve the Chronicle metadata.
- Disable file-opening controls.
- Display a readable warning.
- Surface the condition in Release Readiness.
- Allow restoration without editing the Session.

**0.31.4 status:** implemented.

---

# Medium Importance

## Separate Quest Log and Git

The Chronicle now solves completed-Quest review, but the active **Quest Log & Git** workspace still combines two distinct jobs.

Future split:

- Quest Log
  - Live event feed
  - Notes
  - Media
  - Encounter updates
  - Completion evidence

- Git
  - Repository state
  - Branch
  - Commit
  - Push
  - Linked commit notes
  - Monitor status

This should happen as a UI architecture pass rather than a serialized-data migration.

---

## Dedicated Chronicle identifiers and links

Potential future features:

- Copy Chronicle URI
- Deep-link to Session ID
- Deep-link to Run ID
- Open DeverQuest directly to a completed Quest
- Bookmark a Quest
- Pin important Chronicles
- Share a read-only local report
- Export one selected Quest

---

## Archive sorting and grouping

Possible options:

- Newest
- Oldest
- Longest
- Highest XP
- Highest coin
- Project
- Department
- Contract
- Adventurer
- Chronicle number

Potential grouping:

- Day
- Week
- Project
- Campaign
- Contract
- Chronicle volume

---

## Dedicated completed-Quest export

Export one Quest as:

- Markdown
- JSON
- CSV row
- Human-readable text
- Evidence bundle containing selected attachments

Any evidence bundle must avoid copying files without explicit confirmation.

---

## Better narrative event text

Current timeline events are factual summaries derived from existing data.

Future 1.x improvement:

- More natural transitions
- Better event category icons
- Party-member attribution
- Companion attribution
- Item acquisition events
- Early/late pace statements
- Daily Decree event
- Rank and level-up event

Full Room/Biome procedural prose remains 2.0.

---

## Chronicle correction experience

Current correction workflow routes to Rewards & History.

Potential improvements:

- Inline correction-request drawer
- Side-by-side original and requested value
- Direct leadership review queue
- Correction status badge on completed card
- Link correction to exact field
- Regenerate and reopen the Timecard after approval

Authority and integrity rules must remain unchanged.

---

## Archive performance

The initial Chronicle renders at most 100 selected cards.

Future options when history grows:

- Pagination
- Virtualized UI Toolkit list
- Lazy detail loading
- Cached index
- Background indexing
- Incremental file watching

Do not optimize before real Chronicle sizes justify it.

---

# Low Importance

## Visual polish

Potential additions:

- Category icons
- Integrity badges
- Reward chips
- Media badges
- Combat badges
- Quest Story parchment styling
- Timeline connector lines
- Compact and expanded card modes
- Favorite/pinned Quests

---

## Terminology

The current words now have clearer roles:

- Chronicle: permanent history and narrative
- Quest Archive: searchable completed Sessions
- Timecard: generated daily Markdown report
- Quest Run Archive: Contract completion records
- Battle Archive: local tactical field reports

Later wording should reduce overlap without renaming serialized classes during Beta.

---

# Expansion 2.0

## Procedural Chronicle narrative

The future Chronicle may synthesize prose from:

- Biome
- Structure
- Room
- Encounter
- Enemies
- Hazards
- Companion contributions
- Loot
- Crafting stations
- Travel
- Quest timing
- Party actions

The permanent record should remain compact and aggregate repeated combat actions.

---

## World-map Chronicle

Possible 2.0 presentation:

- Campaign map
- Visited regions
- Quest routes
- Room sequence
- Encounter markers
- Loot markers
- Companion milestones
- Character history
- Guild history

This is presentation over structured Quest World data, not a replacement for Session evidence.

---

# Completed

- Repository and release preparation passed.
- Founder authority passed.
- Identity Catalog passed.
- Reward snapshots passed.
- Timecard Git hygiene passed.
- Current Quest progress panel exists.
- Quest Story appears during active work.
- Encounters replace blank Focus Stage labels.
- Repeatable Contract model exists.
- Quest Run completion history exists.
- Quest Run Management exists.
- Tactical visibility exists.
- Tactical Operations and Battle Archive exist.
- Inventory and Equipment workspace exists.
- Item provenance exists.
- Guild Economy and transaction ledger exist.
- Chronicle workspace implemented in 0.31.4.
- Live event feed implemented.
- Completed Quest cards implemented.
- Timecard, Run ID, Contract, media, and correction navigation implemented.
- Chronicle Readiness check implemented.

---

# Current Decision

After 0.31.4 receives a smoke test, the next pathway should address **Editor UX and Workspace Organization**:

- Separate Git from Quest Log
- Add a dockable compact Quest HUD
- Improve workspace navigation
- Remove internal/developer-facing explanatory text
- Add clearer empty states
- Prepare the Visual Profiles foundation

The deferred gameplay and multi-account test matrices remain preserved for dedicated QA days.
