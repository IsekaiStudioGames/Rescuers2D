# DeverQuest 0.32.1 Beta Issue Log
## Board Administration and Layout Stabilization

**Source build:** 0.32.0 Beta 1  
**Patch build:** 0.32.1 Beta 1  
**Unity test environment:** 6000.3.8f1  
**Readiness baseline:** 23 passed, 1 advisory, 1 blocker  
**Patch status:** Prepared, awaiting Unity verification

---

# Baseline Findings

The 0.32.0 readiness run confirmed:

- Package, repository, profile, authority, storage, Git hygiene, Chronicle, shared Guild, audio host, independent mixer, wellness, Identity Catalog, Tactical Archive, Contract rewards, run reservations, Inventory, Economy, and active Quest state all passed.
- Tactical starter content remains advisory-only.
- Beta content health remains the sole blocker with three unresolved errors.
- Inventory integrity improved from unresolved entries to a clean eight-entry validation.

The submitted Timecard also confirms that Quiet Hours suppressed several reminders. The absence of those warning sounds during the overnight Session is therefore consistent with configured suppression rather than proof of audio-host failure.

---

# DQ-0320-031 — Completed Contracts need leadership disposition controls

**Type:** Guild Board lifecycle  
**Severity:** P1  
**Status:** Patched in 0.32.1

## Requested behavior

A completed one-time or fully satisfied limited Contract may remain visible to administrators. Leadership should decide whether to:

- Archive the listing, or
- Restore it to Offered so it can be performed again.

Members should not see or accept the completed listing.

## 0.32.1 behavior

- Completed listings remain visible to accounts with Manage Contracts permission.
- Completed listings remain hidden from ordinary Members.
- **Archive Listing** removes the Contract from the live Board while preserving all history.
- **Restore to Offered** preserves prior Completion History and opens one additional completion slot.
- Restoring does not delete rewards, Timecards, Sessions, Run IDs, or Chronicle records.
- Completed Quest Run history also provides archive/restore navigation.

## Data model

Restoring a non-repeatable Contract does not erase its completed-run count. A private additional-completion allowance increases the effective target by one.

Example:

```text
Original one-time target: 1
Completed runs: 1
Restore to Offered: effective target becomes 2
After next completion: 2/2 and Completed again
```

---

# DQ-0320-032 — Long text expands the Editor window horizontally

**Type:** Editor layout  
**Severity:** P1  
**Status:** Patched in 0.32.1

## Observed fields

- Quest Log Entry
- Git Commit Message
- Final Quest Log Entry
- Closing Notes

Long text could make the DeverQuest layout demand an excessive horizontal width, pushing Commit and Push controls off-screen.

## 0.32.1 behavior

These fields now use:

- Word wrapping
- A width constrained to the current Editor window
- Expandable vertical height
- Clipping rather than horizontal layout inflation

The Board's primary and leadership controls are also split across separate rows to reduce narrow-dock pressure.

---

# DQ-0320-033 — Duplicate-ID repair requires too much guesswork

**Type:** Beta content administration  
**Severity:** P0 release blocker support  
**Status:** Improved in 0.32.1

The previous health report identified duplicate Quest Profile and Identity IDs. After partial repair, readiness still reports three content errors.

## 0.32.1 behavior

Each duplicate-ID finding now displays every asset in its duplicate group.

Leadership may choose:

```text
Keep This ID; Regenerate Other Copies
```

The selected asset retains its ID. Every other asset in that specific duplicate group receives a new stable ID.

The action:

- Requires CEO or Boss permission
- Requires confirmation
- Lists the keeper and every changed copy
- Preserves object references
- Does not rewrite ambiguous historical ID strings
- Reruns validation afterward

The original one-asset regeneration button remains available for surgical repair.

---

# DQ-0320-034 — Meditation recovery still requires behavioral verification

**Type:** Quest recovery  
**Severity:** Test pending  
**Status:** Implemented in 0.32.0, not verified by submitted Timecard

The submitted Timecard recorded zero Meditation time in both Sessions. Therefore, it does not yet verify the 1 HP and 2 Mana per full minute recovery rule.

---

# DQ-0320-035 — Warning audio not heard during overnight Session

**Type:** Observation  
**Severity:** Informational pending retest  
**Status:** Not confirmed as a bug

The Timecard recorded several reminder events as:

```text
Suppressed by Quiet Hours
```

Suppressed reminders are not expected to play their warning cues. Encounter-start and reward audio were heard earlier. Test warning cues directly from Audio & Wellness outside Quiet Hours before reopening an audio defect.

---

# Retest

- [ ] Install 0.32.1 with zero compilation errors.
- [ ] Confirm completed Contract appears for CEO/Boss.
- [ ] Confirm completed Contract remains hidden from a Member.
- [ ] Archive the completed Contract.
- [ ] Confirm it leaves the live Board.
- [ ] Restore the archived listing from completed-run history.
- [ ] Restore it to Offered.
- [ ] Confirm prior Completion History remains.
- [ ] Complete the restored Contract once.
- [ ] Confirm a new Run ID and completion record are added.
- [ ] Paste a very long Quest Log note.
- [ ] Paste a very long Git commit message.
- [ ] Paste very long final and closing notes.
- [ ] Confirm the window does not widen.
- [ ] Run Beta Administration.
- [ ] Use the grouped duplicate-ID repair.
- [ ] Rerun Full Validation.
- [ ] Confirm Beta content errors reach zero.
- [ ] Test at least two full minutes of manual Meditation.
- [ ] Confirm HP and Mana recover on Resume.
- [ ] Test a warning cue outside Quiet Hours.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
