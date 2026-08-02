# DeverQuest 0.31.7 Beta Issue Log
## Pathway 8 — Notifications and Wellness Command Center

**Source build:** 0.31.6 Beta 1  
**Patch build:** 0.31.7 Beta 1  
**Unity target:** 2022.3 minimum  
**Primary test environment:** Unity 6000.3.8f1  
**Patch status:** Prepared, awaiting Unity verification

---

# Baseline

DeverQuest already supported focused-time reminders, meal reminders, quiet-hours warnings, Approved Breaks, Wellness Journal entries, optional XP benefits, and notification sounds.

The remaining problem was operational clarity:

- Only one reminder could exist at a time.
- A later reminder could be skipped while another was visible.
- Snooze state was global and difficult to inspect.
- Quiet hours had a start but no explicit end or suppression policy.
- Notification decisions outside an active Quest had no durable local history.
- The Quest HUD did not expose reminder actions.
- Cue testing required waiting for the real trigger.

---

# DQ-0316-031 — Concurrent reminders can disappear

**Type:** Wellness scheduling  
**Severity:** P1  
**Status:** Patched in 0.31.7; awaiting verification

## Previous behavior

`HasActiveReminder` prevented the monitor from evaluating other due reminders. A meal, movement, hydration, exercise, or check-in prompt could therefore remain unseen while another reminder was active.

## 0.31.7 correction

The monitor now stores:

- One active reminder
- Queued reminders
- Snoozed reminders with due timestamps

Multiple due conditions are retained rather than discarded. The next ready reminder is promoted after the active reminder is acknowledged, snoozed, or converted into an Approved Break.

The queue persists through assembly reload and Unity restart.

---

# DQ-0316-032 — Snooze and break timing are difficult to inspect

**Type:** Wellness UX  
**Severity:** P1  
**Status:** Patched in 0.31.7; awaiting verification

## 0.31.7 correction

The active reminder now shows:

- Recommended break duration
- Minimum 80% duration required for benefit
- Whether an Approved Break can currently begin
- Additional queued reminder count

Available snoozes:

- 5 minutes
- Configured default
- 30 minutes

The queue shows when each snoozed reminder becomes ready.

During an active Approved Break, the Command Center reports remaining permit time and the minimum qualifying duration.

---

# DQ-0316-033 — Quiet hours do not define a complete notification policy

**Type:** Notification policy  
**Severity:** P1  
**Status:** Patched in 0.31.7; awaiting verification

## Added settings

- Quiet Start Hour
- Quiet End Hour
- Suppress Session Reminders in Quiet Hours

Overnight windows such as `22:00 → 07:00` are supported.

When suppression is enabled:

- Focus Check-In, Hydration, Movement, and Exercise prompts are advanced without displaying repeatedly.
- The suppression is recorded in local notification history.
- The Session Wellness Journal records the suppressed action when a Quest is active.
- The Quiet Hours stopping-point reminder may still appear once per quiet period.

---

# DQ-0316-034 — Notification decisions lack a local operational history

**Type:** Diagnostics / reporting  
**Severity:** P1  
**Status:** Patched in 0.31.7; awaiting verification

A local searchable history is now stored at:

```text
Library/DeverQuest/WellnessHistory.json
```

It records:

- Presented
- Queued
- Snoozed
- Acknowledged
- Break Started
- Break Completed
- Break Ended Early
- Queued Reminder Dismissed
- Suppressed by Quiet Hours
- Test reminder activity

Records may contain:

- Reminder type
- Title
- Session ID
- Focused time at action
- Recommended and required break minutes
- Snooze duration
- Detail text
- Timestamp

The history is local operational data. Timecards and Session Wellness Journal entries remain the permanent Quest evidence.

---

# DQ-0316-035 — Wellness controls are absent from the Quest HUD

**Type:** Docked workflow  
**Severity:** P1  
**Status:** Patched in 0.31.7; awaiting verification

The Quest HUD may now display:

- Active reminder
- Reminder message
- Planned and minimum break duration
- Take Approved Break
- Acknowledge
- Snooze
- Queue count
- Next scheduled session reminder
- Quiet-hours status

The HUD uses the same monitor and Session services as the main window. It does not create a second reminder schedule or break record.

---

# DQ-0316-036 — Reminder cues are difficult to regression-test

**Type:** Audio / QA  
**Severity:** P2  
**Status:** Patched in 0.31.7; awaiting verification

The Command Center provides test controls for:

- Focus Check-In
- Hydration
- Movement
- Exercise
- Lunch
- Dinner
- Quiet Hours

Test reminders:

- Use the configured editor notification and cue path.
- May be queued or snoozed.
- Enter local history as test records.
- Do not advance real reminder schedules.
- Do not award wellness benefits.

---

# DQ-0316-037 — Wellness health is not included in Release Readiness

**Type:** Release diagnostics  
**Severity:** P2  
**Status:** Patched in 0.31.7; awaiting verification

Release Readiness now verifies:

- Local Wellness History storage is writable.
- Quiet-hour values are valid.
- History-limit configuration is valid.
- Current local record count.
- Current queued/snoozed reminder count.

A storage or configuration failure produces an advisory rather than blocking timer use.

---

# Compatibility and Guardrails

- Existing Session Wellness Journal entries are unchanged.
- Existing Approved Break reward logic is unchanged.
- Existing reminder intervals and meal times migrate unchanged.
- New profiles default to quiet hours ending at 07:00.
- The previous single global snooze key is retired during migration.
- Clearing local notification history does not alter Timecards.
- Clearing the reminder queue does not alter existing Session evidence.
- Test reminders do not alter real schedules or rewards.
- Disabling Wellness clears non-test pending reminders.
- Local history write failures cannot stop a Quest or break workflow.

---

# Required Retest

- [ ] Install 0.31.7 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Open Audio & Wellness.
- [ ] Trigger every test reminder.
- [ ] Confirm each configured cue is audible.
- [ ] Queue at least three reminders.
- [ ] Acknowledge one and confirm the next appears.
- [ ] Snooze one for 5 minutes.
- [ ] Restart Unity and confirm it remains queued.
- [ ] Take an Approved Break.
- [ ] End one below 80% and inspect history.
- [ ] Complete one at or above 80% and inspect history.
- [ ] Confirm the Session Wellness Journal agrees.
- [ ] Test an overnight quiet-hours window.
- [ ] Confirm suppressed session reminders do not retry every second.
- [ ] Open the Quest HUD and use its wellness controls.
- [ ] Clear local history and confirm Timecards remain unchanged.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
