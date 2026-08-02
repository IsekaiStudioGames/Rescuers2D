# DeverQuest 0.31.7 Beta Test Checklist
## Quest 8 — The Keeper of Healthy Hours

**Build:** 0.31.7 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

This checklist focuses on notifications and wellness. Earlier deferred Contract, Party, Tactical, Inventory, Economy, Chronicle, UX, and Audio matrices remain separate.

---

# A. Installation and Readiness

- [ ] Install `com.echodevgames.deverquest-0.31.7.tgz`.
- [ ] Confirm Package Manager reports 0.31.7.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm Wellness command center passes or produces an understood advisory.
- [ ] Record local history count.
- [ ] Record queued/snoozed reminder count.
- [ ] Confirm the readiness run does not create a reminder or Session.

---

# B. Command Center Layout

Open **Audio & Wellness**.

- [ ] Confirm Wellness Command Center renders before the audio player.
- [ ] Confirm Active Reminder is visible.
- [ ] Confirm queue count is visible.
- [ ] Confirm Next Session Reminder is visible.
- [ ] Confirm Quiet Hours status is visible.
- [ ] Confirm Reminder Queue foldout works.
- [ ] Confirm Reminder Settings and Cue Tests foldout works.
- [ ] Confirm Notification History foldout works.
- [ ] Confirm the panel remains readable in narrow and wide docks.

---

# C. Test Reminders and Cues

With a valid Warning Profile assigned:

- [ ] Test Focus Check-In.
- [ ] Confirm Focus Check-In cue.
- [ ] Test Hydration.
- [ ] Confirm Hydration cue.
- [ ] Test Movement.
- [ ] Confirm Movement cue.
- [ ] Test Exercise.
- [ ] Confirm Movement/Exercise cue behavior.
- [ ] Test Lunch.
- [ ] Confirm Meal cue.
- [ ] Test Dinner.
- [ ] Confirm Meal cue.
- [ ] Test Quiet Hours.
- [ ] Confirm its configured fallback cue.
- [ ] Disable Notification Cues.
- [ ] Confirm test reminders remain visible without sound.
- [ ] Re-enable Notification Cues.
- [ ] Confirm test reminders are identified as tests.
- [ ] Confirm tests do not change the real reminder schedule.
- [ ] Confirm tests do not award XP or change Adventurer needs.

---

# D. Reminder Queue

- [ ] Trigger one test reminder.
- [ ] Trigger three additional types before handling the first.
- [ ] Confirm the first remains active.
- [ ] Confirm the others enter the queue.
- [ ] Confirm no reminder disappears.
- [ ] Acknowledge the active reminder.
- [ ] Confirm the next ready reminder is promoted.
- [ ] Dismiss one queued reminder.
- [ ] Confirm it leaves the queue.
- [ ] Clear the queue.
- [ ] Confirm the active reminder remains.
- [ ] Confirm queue actions enter local history.

---

# E. Snooze Persistence

- [ ] Trigger a reminder.
- [ ] Snooze for 5 minutes.
- [ ] Confirm it appears in the queue with a due countdown.
- [ ] Trigger another reminder while the first is snoozed.
- [ ] Confirm the new reminder may become active.
- [ ] Restart Unity before the snooze expires.
- [ ] Confirm the snoozed reminder persists.
- [ ] Wait for its due time.
- [ ] Confirm it promotes when no other reminder is active.
- [ ] Test the configured default snooze.
- [ ] Test the 30-minute snooze.
- [ ] Confirm each snooze action records its duration.

---

# F. Real Session Reminder Schedule

Create a QA profile with very short intervals.

- [ ] Start a tiny Quest.
- [ ] Confirm Next Session Reminder shows the nearest type.
- [ ] Reach a Focus Check-In.
- [ ] Confirm it appears once.
- [ ] Leave it active until Hydration is due.
- [ ] Confirm Hydration queues.
- [ ] Leave both until Movement is due.
- [ ] Confirm Movement queues.
- [ ] Handle each reminder.
- [ ] Confirm each schedule advances.
- [ ] Confirm a handled reminder does not immediately retrigger.
- [ ] Confirm the Session Wellness Journal records real actions.

---

# G. Approved Break Qualification

- [ ] Trigger a real reminder during a running Quest.
- [ ] Confirm planned break minutes.
- [ ] Confirm minimum 80% minutes.
- [ ] Select Take Approved Break.
- [ ] Confirm the Quest pauses.
- [ ] Confirm Command Center shows permit time remaining.
- [ ] Confirm Quest HUD shows the break state.
- [ ] Resume below 80%.
- [ ] Confirm Break Ended Early.
- [ ] Confirm no wellness XP is awarded.
- [ ] Trigger another reminder.
- [ ] Complete at least 80% of the break.
- [ ] Confirm Break Completed.
- [ ] Confirm configured wellness XP is awarded once.
- [ ] Confirm Adventurer needs update according to reminder type.
- [ ] Confirm local history and Session Wellness Journal agree.
- [ ] Confirm the Timecard contains the outcome.

---

# H. Quiet Hours

Configure an immediately active window.

- [ ] Set Quiet Start Hour.
- [ ] Set Quiet End Hour.
- [ ] Confirm an overnight window such as 22 → 7 is accepted.
- [ ] Confirm Quiet Hours status reports its end time.
- [ ] Enable Suppress Session Reminders in Quiet Hours.
- [ ] Start a Quest with a due Hydration reminder.
- [ ] Confirm Hydration is suppressed rather than displayed.
- [ ] Confirm it does not retry every second.
- [ ] Confirm suppression enters local history.
- [ ] Confirm suppression enters the Session Wellness Journal.
- [ ] Confirm the Quiet Hours stopping-point reminder may appear once.
- [ ] Disable suppression.
- [ ] Confirm due session reminders display normally.
- [ ] Disable Quiet Hours.
- [ ] Confirm status becomes inactive.

---

# I. Meal Reminders

- [ ] Configure Lunch a few minutes ahead.
- [ ] Confirm Lunch appears at the configured time.
- [ ] Snooze Lunch.
- [ ] Confirm it returns after snooze.
- [ ] Acknowledge Lunch.
- [ ] Confirm it does not repeatedly trigger that day.
- [ ] Configure Dinner a few minutes ahead.
- [ ] Confirm Dinner appears.
- [ ] Start a Meal Approved Break during a Quest.
- [ ] Confirm the meal break uses the configured meal duration.
- [ ] Confirm a completed meal break updates needs and history.

---

# J. Quest HUD

- [ ] Enable Show Wellness in Quest HUD.
- [ ] Open and dock Quest HUD.
- [ ] Confirm Next reminder appears during a Quest.
- [ ] Trigger a reminder.
- [ ] Confirm its title and message appear.
- [ ] Confirm planned and minimum times appear.
- [ ] Acknowledge from HUD.
- [ ] Snooze from HUD.
- [ ] Take Approved Break from HUD.
- [ ] Confirm the main window updates immediately.
- [ ] Confirm no duplicate break or reward is created.
- [ ] Disable Show Wellness in Quest HUD.
- [ ] Confirm wellness controls disappear from HUD only.

---

# K. Notification History

- [ ] Generate Presented records.
- [ ] Generate Queued records.
- [ ] Generate Snoozed records.
- [ ] Generate Acknowledged records.
- [ ] Generate Break Started records.
- [ ] Generate Break Completed records.
- [ ] Generate Break Ended Early records.
- [ ] Generate Suppressed records.
- [ ] Search by title.
- [ ] Search by action.
- [ ] Search by Session ID.
- [ ] Test every history filter.
- [ ] Restart Unity.
- [ ] Confirm history persists.
- [ ] Clear local notification history.
- [ ] Confirm it becomes empty.
- [ ] Confirm Session Wellness Journal entries remain.
- [ ] Confirm generated Timecards remain unchanged.

---

# L. Disabled and Failure States

- [ ] Disable Wellness while real reminders are queued.
- [ ] Confirm non-test queued reminders clear.
- [ ] Confirm a test reminder can still be used for QA.
- [ ] Re-enable Wellness.
- [ ] Make `Library/DeverQuest` temporarily unwritable where practical.
- [ ] Run Release Readiness.
- [ ] Confirm a readable advisory.
- [ ] Confirm reminders still do not interrupt timer operation.
- [ ] Restore write access.
- [ ] Confirm readiness clears.
- [ ] Remove the Warning Profile.
- [ ] Trigger a reminder.
- [ ] Confirm safe visual fallback and optional Editor beep.

---

# M. Safety Regression

- [ ] Browsing history does not add focused time.
- [ ] Browsing history does not award XP.
- [ ] Cue tests do not award XP.
- [ ] Cue tests do not alter Adventurer needs.
- [ ] Queue clearing does not alter Timecards.
- [ ] History clearing does not alter Timecards.
- [ ] Snoozing does not pause the Quest.
- [ ] Acknowledging does not pause the Quest.
- [ ] Approved Break pauses exactly once.
- [ ] A completed break awards its benefit exactly once.
- [ ] Restart recovery does not duplicate a reminder history action.

---

# Verdict

- [ ] **PASS** — queue, snooze, quiet hours, break qualification, history, cues, and HUD controls are reliable.
- [ ] **CONDITIONAL PASS** — core behavior passes; documented notification-platform limitations remain.
- [ ] **FAIL** — reminders disappear, schedules loop, breaks duplicate rewards, or history damages Quest evidence.
