# DeverQuest 0.30.9 Beta Issue Log
## Pathway 4 — Quest Board and Run Management

**Source build:** 0.30.8 Beta 1  
**Patch build:** 0.30.9 Beta 1  
**0.30.8 verification state:** Deferred  
**Patch state:** Prepared, awaiting Unity compilation and smoke test

---

# DQ-0308-026 — Reusable Contracts lack operational management

**Type:** Guild administration / Quest Run lifecycle  
**Severity:** P1 usability and recovery gap  
**Status:** Patched in 0.30.9; awaiting verification

## Problem

0.30.8 introduced independent Quest Runs, repeatable Contracts, limited completions, and flexible Party starts. The underlying records existed, but DeverQuest lacked a practical management surface for a busy Guild Board.

Leadership could not easily:

- See all active Quest Run reservations
- See how long a reservation had existed
- Clear an abandoned Party roster
- Release a stale run without editing a Contract asset
- Retire a repeatable listing without deleting its history
- Browse completion records from inside DeverQuest

## 0.30.9 correction

Added **Guild Hall > Quest Run Management** with:

- Active run count
- Waiting Party count
- Contract title
- Run ID
- Participants
- Start time
- Reservation age
- Select Contract
- Cancel Stale Run
- Clear Waiting Party

The active local Session cannot be cancelled from this management panel. It must be completed or abandoned through the normal Quest workspace.

---

# DQ-0308-027 — Completed Quest Runs are hidden inside Contract assets

**Type:** History / reporting  
**Severity:** P1 loop-completion gap  
**Status:** Patched in 0.30.9; awaiting verification

## 0.30.9 correction

Added **Rewards & History > Completed Quest Run Archive**.

The archive provides:

- Search by Contract, Adventurer, or Run ID
- Optional inclusion of archived listings
- Total matching runs
- Total focused hours
- Total coin
- Total XP
- Completion time
- Participants
- Run ID
- Focused minutes
- Reward totals
- Select Contract
- Copy Run ID

The newest 50 matching records are displayed to protect Editor layout performance.

---

# DQ-0308-028 — No non-destructive way to retire a listing

**Type:** Guild Board administration  
**Severity:** P1  
**Status:** Patched in 0.30.9; awaiting verification

## 0.30.9 correction

Quest Contracts now support an `Archived` board state.

- Archived listings are hidden from Members.
- Leadership can still inspect them.
- Completion history remains intact.
- Archived Contracts cannot be accepted.
- A Contract with active runs or a waiting Party cannot be archived.
- Leadership may restore a listing later.

---

# DQ-0308-029 — Stale reservations are not reported by Readiness

**Type:** Release diagnostics  
**Severity:** P1 advisory  
**Status:** Patched in 0.30.9; awaiting verification

Release Readiness now checks Contract assets for:

- Empty or invalid Run IDs
- Active reservations older than 24 hours

A clean state produces a pass. Suspicious reservations produce an advisory directing leadership to Guild Hall > Quest Run Management.

---

# Safety boundaries

Cancelling a reservation in one clone cannot stop Unity or DeverQuest running in another clone. The panel therefore labels the action as reservation cleanup and requires confirmation.

0.30.9 still stores run state in Contract assets. Truly concurrent multi-clone reservations remain a future append-only shared-ledger problem.

---

# Deferred 0.30.8 verification

The following remain unverified rather than failed:

- Three repeated completions on one Contract
- Limited-completion target closure
- One-completion-per-Adventurer across multiple accounts
- Reservation-slot contention
- Flexible two-to-four member Party launch
- Full-party-required launch
- Concurrent clone merge behavior

Resume `DeverQuest_0.30.8_Deferred_Verification_Checklist.md` before Release Candidate.

---

# Current verdict

**0.30.8:** CONDITIONAL / DEFERRED VERIFICATION  
**0.30.9:** PATCH PREPARED / UNITY VERIFICATION REQUIRED
