# DeverQuest 0.32.1 Focused Beta Checklist
## Quest 9 — The Archivist and the Restored Notice

**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

---

# A. Installation

- [ ] Install `com.echodevgames.deverquest-0.32.1.tgz`.
- [ ] Confirm Package Manager reports 0.32.1.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Record the Beta content-health error count.

---

# B. Completed Board Visibility

Use a completed Single Completion Contract.

- [ ] Sign in as CEO or Boss.
- [ ] Open Guild Hall.
- [ ] Confirm the completed Contract remains visible.
- [ ] Confirm its status reads Completed.
- [ ] Confirm Completed Runs and Last Completed By are visible.
- [ ] Confirm Restore to Offered is visible.
- [ ] Confirm Archive Listing is visible.
- [ ] Sign in as a Member.
- [ ] Confirm the completed Contract is hidden from the Member Board.
- [ ] Confirm the Member cannot accept it.

---

# C. Archive and Restore

- [ ] As leadership, click Archive Listing.
- [ ] Confirm the listing leaves the live Board.
- [ ] Open Completed Quest Run Archive.
- [ ] Enable Include Archived Listings.
- [ ] Find the Contract.
- [ ] Click Restore Listing.
- [ ] Confirm it returns as a completed leadership-visible listing.
- [ ] Click Restore to Offered.
- [ ] Confirm its status becomes Offered.
- [ ] Confirm its previous Completion History remains.
- [ ] Confirm the availability target increased by one.
- [ ] Accept and complete it again.
- [ ] Confirm a new Run ID is created.
- [ ] Confirm one new completion record is added.
- [ ] Confirm prior rewards and Timecards remain unchanged.
- [ ] Confirm the Contract becomes Completed again.

---

# D. Long Text Layout

Test in a narrow dock.

- [ ] Paste a 500-character Quest Log Entry.
- [ ] Confirm it wraps vertically.
- [ ] Confirm the window does not widen.
- [ ] Confirm Add Quest Log Note remains visible.
- [ ] Paste a 500-character Git Commit Message.
- [ ] Confirm Commit and Push controls remain visible.
- [ ] Enter Quest Turn-In.
- [ ] Paste a 500-character Final Quest Log Entry.
- [ ] Paste 500-character Closing Notes.
- [ ] Confirm Review Spoils remains visible.
- [ ] Resize the window wider and narrower.
- [ ] Confirm text reflows.
- [ ] Confirm no `Invalid GUILayout state` warning.

---

# E. Duplicate-ID Group Repair

- [ ] Open Beta Administration.
- [ ] Run Full Validation.
- [ ] Locate one duplicate-ID error.
- [ ] Confirm the full duplicate group is listed.
- [ ] Select the asset whose identity should remain authoritative.
- [ ] Click Keep This ID; Regenerate Other Copies.
- [ ] Review the confirmation list.
- [ ] Confirm the repair.
- [ ] Confirm the result lists each changed path and new ID.
- [ ] Rerun Full Validation.
- [ ] Repeat for each remaining duplicate group.
- [ ] Confirm zero content-health errors.
- [ ] Export Markdown and JSON health reports.
- [ ] Run Release Readiness.
- [ ] Confirm Beta content health passes or contains warnings only.

---

# F. Meditation Recovery

- [ ] Start a Quest with damaged HP and missing Mana.
- [ ] Enter manual Meditation.
- [ ] Resume before one full minute.
- [ ] Confirm no recovery.
- [ ] Meditate for two full minutes.
- [ ] Resume.
- [ ] Confirm +2 HP and +4 Mana, capped at maximum.
- [ ] Confirm the HUD prediction matched.
- [ ] Complete the Quest.
- [ ] Confirm Meditation recovery appears in the Timecard.
- [ ] Confirm Approved Break does not trigger the same recovery.
- [ ] Confirm idle/focus-loss pause does not trigger recovery.

---

# G. Warning Cue Clarification

- [ ] Confirm Quiet Hours are inactive.
- [ ] Open Audio & Wellness.
- [ ] Trigger a Focus Check-In test cue.
- [ ] Trigger Hydration.
- [ ] Trigger Movement.
- [ ] Confirm cues are audible.
- [ ] Enable Quiet Hours suppression.
- [ ] Trigger a scheduled reminder inside the quiet window.
- [ ] Confirm it is recorded as suppressed and no cue plays.
- [ ] Confirm this is treated as expected behavior.

---

# Verdict

- [ ] **PASS** — Board controls, wrapping, ID repair, Meditation, and cue behavior pass.
- [ ] **CONDITIONAL PASS** — no P0 defect; tactical content remains deferred.
- [ ] **FAIL** — history is lost, Member visibility is wrong, layout still widens, or content errors remain.
