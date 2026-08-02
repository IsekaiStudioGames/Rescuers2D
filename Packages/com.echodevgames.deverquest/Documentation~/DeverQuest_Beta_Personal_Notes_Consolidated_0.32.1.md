# DeverQuest Personal Notes
## Consolidated after 0.32.1 Board Administration

---

# Immediate Importance

## Completed Contract disposition

Completed Contracts should remain visible to Guild leadership until a decision is made.

Leadership options:

- Archive Listing
- Restore to Offered

Members should not see completed listings.

Restoring a Contract must preserve every earlier completion and open one additional completion slot rather than deleting history.

**0.32.1 status:** implemented.

---

## Long-form Editor text

The following fields must wrap:

- Quest Log Entry
- Git Commit Message
- Final Quest Log Entry
- Closing Notes

Long text must never push controls beyond the window edge.

**0.32.1 status:** implemented.

---

## Clear remaining content-health blocker

Readiness improved to 23 passes. Beta content health is the only blocker.

The duplicate repair workflow now shows the whole group and lets leadership preserve one chosen ID while regenerating every other copy.

**0.32.1 status:** implemented, requires new validation export.

---

## Meditation verification

Meditation recovery exists, but the submitted Timecard recorded zero Meditation minutes.

Required direct test:

- Damage HP
- Spend Mana
- Meditate for two full minutes
- Resume
- Confirm +2 HP and +4 Mana
- Confirm Timecard entry

---

## Warning-audio interpretation

The submitted overnight Timecard recorded multiple reminder events as suppressed by Quiet Hours. Those reminders should not play cues.

Test cue buttons outside Quiet Hours before treating missing warnings as an audio regression.

---

# Medium Importance

## Contract archive administration

Future improvements:

- Dedicated archived Contract list
- Bulk archive
- Archive reason
- Archive timestamp
- Restored-by account
- Reopen count
- Retired versus temporarily archived distinction
- Board filter for Completed and Archived

---

## Completion cycles

The current reopen allowance preserves history and adds one slot.

A later model may expose explicit cycles:

```text
Contract
├── Cycle 1: Completed
├── Cycle 2: Completed
└── Cycle 3: Offered
```

This would improve seasonal or recurring one-time work without turning it into an unlimited Repeatable Contract.

---

## Text editing quality

Possible later additions:

- Character counter
- Expandable editor popup
- Markdown preview
- Spell-check integration
- Saved draft recovery
- Clear button
- Template snippets

---

# Low Importance

- Add archive and restore icons.
- Show previous completion count in the restore confirmation.
- Add a brief “why this warning was silent” hint for Quiet Hours.
- Add a one-click route from readiness to the exact remaining validation finding.

---

# Expansion 2.0

No changes. Crafting, banking, housing, broad skills, Rooms, Biomes, and procedural Chronicle narrative remain deferred.

---

# Completed

- Readiness reached 23 passes.
- Inventory integrity passes.
- Economy passes.
- Supported audio host and independent mixer pass.
- Chronicle archive contains seven records.
- Quiet Hours suppression is recorded.
- Completed Contracts are leadership-visible in 0.32.1.
- Archive and restore-to-Offered controls implemented.
- Long text wrapping implemented.
- Group duplicate-ID repair implemented.
