# DeverQuest 0.30.8 Beta Issue Log
## Pathway 3 — Reusable Quest Contracts and Independent Runs

**Source build:** 0.30.7 Beta 1  
**Patch build:** 0.30.8 Beta 1  
**Unity test environment:** 6000.3.8f1  
**Readiness baseline:** 15 passed, 0 advisories, 0 blockers  
**Patch status:** Prepared, awaiting Unity verification

---

# Baseline Result

The 0.30.7 Release Readiness run passed every check:

- Package version
- Unity version
- Repository naming
- Repository documentation
- Developer profile
- Guild authority
- Timecard storage
- Timecard Git hygiene
- Chronicle integrity
- Shared Guild repository
- Editor audio transport
- Playlist completion detection
- Starter Identity Catalog
- Contract reward snapshots
- Active Quest state

This closes the Founder and First Adventurer pathway.

---

# DQ-0307-022 — Contract completion is global rather than per run

**Type:** Quest architecture / Guild Board lifecycle  
**Severity:** P1 loop limitation  
**Status:** Patched in 0.30.8; awaiting verification

## Previous behavior

A Quest Contract used one shared lifecycle:

`Offered → Accepted → Active → Submitted → Approved → Completed`

The Contract asset stored the current party roster and submission state. A completed Quest was recorded in the local Session, Timecard, Chronicle, reward journal, and Guild audit, but the Contract itself did not maintain a durable history of independent executions.

Consequences:

- One Adventurer could consume the listing for everyone.
- A daily workout could not remain permanently available.
- A Contract could not naturally request five independent completions.
- A completed run could remain waiting for leadership to move the entire Contract through Submitted, Approved, and Completed.
- Independent solo runs were not represented separately.

## 0.30.8 correction

A Contract is now the reusable board definition. Each acceptance creates a **Quest Run** with a unique run ID.

The Contract stores:

- Active run reservations
- Run participants
- Completion history
- Session IDs
- Adventurer names
- Developer names
- Completion timestamps
- Focused minutes
- Awarded coin
- Awarded XP

Generated Timecards also record the Quest Run ID.

---

# DQ-0307-023 — No repeatable or limited-completion policy

**Type:** Assignment configuration  
**Severity:** P1  
**Status:** Patched in 0.30.8; awaiting verification

## Added policies

### Single Completion

- One successful Quest Run completes the Contract.
- The listing closes after the first completion.
- Existing Contracts migrate to this policy by default.

### Limited Completions

- The Contract remains available until its configured completion target is reached.
- Example: five completed runs.
- Active reservations count against the remaining available slots.
- The Contract closes automatically when the target is met.

### Repeatable

- The Contract returns to Offered after every completed run.
- The same Adventurer may repeat it unless the one-completion-per-Adventurer option is enabled.
- Suitable for exercise, recurring maintenance, daily rituals, and reusable practice tasks.

## Unique Adventurer rule

`One Completion Per Adventurer` is independent from the availability policy.

Examples:

- Five runs by five different people:
  - Policy: Limited Completions
  - Required Completions: 5
  - One Completion Per Adventurer: enabled

- Five runs by any mixture of people:
  - Policy: Limited Completions
  - Required Completions: 5
  - One Completion Per Adventurer: disabled

- Unlimited five-minute workout:
  - Policy: Repeatable
  - One Completion Per Adventurer: disabled

---

# DQ-0307-024 — Party Quests always wait for maximum capacity

**Type:** Party workflow  
**Severity:** P1  
**Status:** Patched in 0.30.8; awaiting verification

## Added Party rules

- Minimum Participants
- Maximum Participants
- Require Full Party

### Full Party required

The Quest waits until the roster reaches Maximum Participants.

### Flexible Party

The Quest may begin when Minimum Participants is reached, even when open slots remain.

Examples:

- Solo-capable group adventure:
  - Minimum: 1
  - Maximum: 4
  - Require Full Party: disabled

- At least two people, up to four:
  - Minimum: 2
  - Maximum: 4
  - Require Full Party: disabled

- Exactly four required:
  - Maximum: 4
  - Require Full Party: enabled

Once a Party Run begins, its roster is locked. A new Adventurer cannot join that active run.

---

# DQ-0307-025 — Completed Contracts wait for manual board closure

**Type:** Completion workflow  
**Severity:** P1  
**Status:** Patched in 0.30.8; awaiting verification

Quest completion now updates the board lifecycle automatically:

- Single Completion → Completed
- Limited Completions below target → Offered
- Limited Completions at target → Completed
- Repeatable → Offered

Abandoning a Quest releases its run reservation and reopens the Contract when appropriate.

The existing status controls remain available for leadership and legacy workflows, but normal successful runs no longer require the whole Contract to sit at Submitted until someone manually closes it.

---

# Compatibility Notes

- Existing Contracts migrate as Single Completion with one required run.
- Existing Contract IDs and reward snapshots are preserved.
- Existing completed Contracts receive a legacy completion-history entry so they do not reopen accidentally.
- Existing party, stage, assignment, restriction, and reward fields remain.
- Changing a completed Contract to Repeatable or Limited Completions reopens it when more runs are allowed.

---

# Known Boundary

Completion history is serialized into the Quest Contract asset.

This supports multiple independent runs in one project and normal source-controlled Guild workflows. Truly simultaneous completion updates from separate Git clones may still create ordinary asset merge conflicts. A future shared Guild run ledger should move cross-clone run reservations and completions into append-only shared records.

---

# Required Retest

- [ ] Existing one-time Contract remains one-time after upgrade.
- [ ] Existing completed Contract does not reopen unexpectedly.
- [ ] One-time Quest closes after one completion.
- [ ] Repeatable Quest remains on the board after three completions by the same Adventurer.
- [ ] Limited Quest closes after its configured target.
- [ ] Unique-Adventurer limited Quest rejects a second run by the same Adventurer.
- [ ] Abandonment releases the reserved completion slot.
- [ ] Completion History records the correct Adventurer and developer.
- [ ] Completion History records Session and Run IDs.
- [ ] Timecard displays Quest Run ID.
- [ ] Partial Party starts at its configured minimum.
- [ ] Full-party Quest waits for maximum capacity.
- [ ] Active Party roster is locked.
- [ ] Repeatable Party Quest clears the old roster and returns to Offered.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
