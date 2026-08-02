# DeverQuest 0.30.9 Beta Test Checklist
## Quest 4 — The Quartermaster's Ledger

**Build:** 0.30.9 Beta 1  
**Purpose:** Verify management and history surfaces without repeating the entire 0.30.8 multi-account campaign.

Status: `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

---

# A. Install and Readiness

- [ ] Install `com.echodevgames.deverquest-0.30.9.tgz`.
- [ ] Confirm Package Manager reports 0.30.9.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm Quest Run reservations reports PASS when no suspicious run exists.
- [ ] Confirm previous Readiness checks remain green.

---

# B. Completed Quest Run Archive

Use existing completion records when available. One completed Quest is enough for the smoke test.

- [ ] Open **Rewards & History**.
- [ ] Expand **Completed Quest Run Archive**.
- [ ] Confirm a completed run appears.
- [ ] Confirm Contract title.
- [ ] Confirm Adventurer name.
- [ ] Confirm completion time.
- [ ] Confirm focused minutes.
- [ ] Confirm coin and XP.
- [ ] Confirm Run ID.
- [ ] Search by Contract title.
- [ ] Search by Adventurer.
- [ ] Search by Run ID fragment.
- [ ] Select the Contract from the archive.
- [ ] Copy the Run ID and paste it into a text field.
- [ ] Confirm the summary totals match the visible records.
- [ ] Confirm no layout error appears in a narrow dock.

---

# C. Contract Archive and Restore

Use a Contract with no active Session or waiting Party.

- [ ] Open the Assignment Board as CEO/Boss.
- [ ] Select **Archive Listing**.
- [ ] Confirm the Contract reports Archived.
- [ ] Confirm its completion history remains.
- [ ] Log in as Member or use a Member test account.
- [ ] Confirm the archived Contract is hidden from the Member board.
- [ ] Log back in as leadership.
- [ ] Select **Restore Listing**.
- [ ] Confirm the Contract returns to its correct lifecycle state.
- [ ] Confirm it may be accepted when its policy still permits another run.

---

# D. Active Run Management

A single temporary Quest Run is enough.

- [ ] Start a Quest Run.
- [ ] Open **Guild Hall > Quest Run Management**.
- [ ] Confirm Contract title.
- [ ] Confirm participants.
- [ ] Confirm Run ID.
- [ ] Confirm start time.
- [ ] Confirm reservation age.
- [ ] Attempt **Cancel Stale Run** for the active local Session.
- [ ] Confirm DeverQuest refuses and directs you to the Quest workspace.
- [ ] Abandon the Quest normally.
- [ ] Confirm the reservation disappears.

Optional stale-run simulation:

- [ ] Create a reservation from another account/clone or temporarily leave one without a local Session.
- [ ] Cancel the stale reservation from Quest Run Management.
- [ ] Confirm the Contract reopens correctly.
- [ ] Confirm no Completion History record is created.
- [ ] Confirm a Guild audit entry is written.

---

# E. Waiting Party Management

Optional when a second account is available.

- [ ] Join a Party Quest below its start threshold.
- [ ] Confirm the waiting roster appears in Quest Run Management.
- [ ] Confirm current/required/maximum capacity is displayed.
- [ ] Select **Clear Waiting Party**.
- [ ] Confirm the roster clears.
- [ ] Confirm no completion is added.
- [ ] Confirm the Contract returns to Offered.
- [ ] Confirm a Guild audit entry is written.

---

# F. Stale Reservation Advisory

This may be deferred until a naturally stale test record exists.

- [ ] Create or preserve a reservation older than 24 hours.
- [ ] Run Release Readiness.
- [ ] Confirm a Quest Run reservations advisory.
- [ ] Clear the reservation.
- [ ] Run Readiness again.
- [ ] Confirm the check returns to PASS.

---

# G. Regression Smoke Test

- [ ] Accept one normal Contract.
- [ ] Complete it.
- [ ] Confirm reward is granted once.
- [ ] Confirm Timecard is generated.
- [ ] Confirm Completion History adds one record.
- [ ] Confirm the new record appears in the archive.
- [ ] Confirm an archived Contract cannot be accepted.
- [ ] Confirm no `Invalid GUILayout state` warning.
- [ ] Confirm no null exception.
- [ ] Restart Unity and confirm archive state persists.

---

# Deferred regression reference

The full 0.30.8 policy and Party matrix remains in:

`DeverQuest_0.30.8_Deferred_Verification_Checklist.md`

Do not mark those deferred tests as passed without running them.

---

# Verdict

- [ ] **PASS** — management, archive, cancellation safety, and history display work.
- [ ] **CONDITIONAL PASS** — local smoke tests pass; multi-account cleanup remains deferred.
- [ ] **FAIL** — history is lost, an active Session can be orphaned, rewards duplicate, or board state is corrupted.
