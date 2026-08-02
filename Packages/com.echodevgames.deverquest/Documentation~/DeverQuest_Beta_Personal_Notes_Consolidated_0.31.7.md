# DeverQuest Personal Notes
## Consolidated after 0.31.7 Wellness Command Work

**Current patch target:** 0.31.7 Notifications and Wellness Command Center  
**Product lane:** Finish and verify the existing Beta loop before 2.0 systems.

---

# Immediate Importance

## Reminder reliability

Reminders must not disappear because another prompt is already visible.

The operational model is now:

```text
Active Reminder
Queued Ready Reminders
Snoozed Future Reminders
Local Notification History
Session Wellness Journal
```

**0.31.7 status:** implemented, awaiting Unity verification.

---

## Visible break timing

Every reminder should answer:

- How long is the recommended break?
- What is the minimum 80% duration?
- Can an Approved Break begin right now?
- How much permit time remains?
- Was the break completed or ended early?
- Was a benefit awarded?

**0.31.7 status:** Command Center and Quest HUD presentation implemented.

---

## Quiet-hours behavior

Quiet hours now require:

- Start hour
- End hour
- Overnight support
- Optional suppression of session reminders
- One stopping-point reminder
- Suppression history
- No one-second retrigger loop

**0.31.7 status:** implemented.

---

## Notification history

A local operational history is useful for QA and personal review, but it must not replace permanent Quest evidence.

Storage:

```text
Library/DeverQuest/WellnessHistory.json
```

Permanent Quest evidence remains:

- Session Wellness Journal
- Generated Timecard
- Chronicle

**0.31.7 status:** implemented with search, filters, retention limit, and clear action.

---

## Quest HUD wellness state

The dockable HUD should allow a developer to handle a reminder without reopening the full package.

Current HUD actions:

- Take Approved Break
- Acknowledge
- Snooze
- View queue count
- View next reminder
- View quiet-hours state

**0.31.7 status:** implemented.

---

## Cue verification

Wellness cue testing should not require waiting for real time thresholds.

Manual tests now cover:

- Check-In
- Hydration
- Movement
- Exercise
- Lunch
- Dinner
- Quiet Hours

Test prompts must not advance schedules or award benefits.

**0.31.7 status:** implemented.

---

# Medium Importance

## Per-reminder enable and cue controls

Future 1.x controls could separate:

- Enabled state per reminder type
- Visual notification per type
- Audio cue per type
- Auto-open behavior per type
- Default snooze per type
- Break length per type
- Benefit per type

Current intervals of zero already disable session reminder types, but a clearer matrix would be easier to manage.

---

## Wellness presets

Potential profiles:

- Pomodoro
- Deep Work
- Accessibility-Friendly
- Movement-Focused
- Hydration-Focused
- Night Shift
- Quiet Office
- Custom

These should be local presentation/behavior presets, not Guild-enforced health policy by default.

---

## Daily wellness summary

Potential summary:

- Reminders presented
- Acknowledged
- Snoozed
- Breaks started
- Breaks completed
- Breaks ended early
- Total Approved Break time
- Benefits awarded
- Quiet-hours suppressions

The summary should avoid judgmental scoring.

---

## Better reminder queue administration

Possible later actions:

- Present now
- Reschedule to a chosen time
- Reorder queue
- Dismiss all of one type
- Pause reminders for one hour
- Pause until next Quest
- Resume reminder schedule

A global pause should be explicit and visible.

---

## Operating-system notifications

Current reminders are Unity Editor notifications and optional audio cues.

A future platform adapter could support native Windows/macOS notifications, but it must:

- Be optional
- Avoid leaking Quest content
- Respect quiet hours
- Avoid duplicate Unity/native prompts
- Fail without affecting the timer

---

## Wellness reward review

Wellness XP currently rewards completing eligible Approved Breaks.

Before shipment, review:

- Whether XP is appropriate for all studios
- Whether leaders may disable it
- Maximum daily wellness XP
- Avoiding incentive to manufacture break prompts
- Accessibility and medical neutrality

The system should encourage healthy pauses without pretending to provide medical guidance.

---

# Low Importance

## Visual polish

Potential additions:

- Reminder-type icons
- Queue badges
- Quiet-hours moon indicator
- Break progress ring
- History action colors
- Compact HUD mode
- Notification toast styling

---

## Reminder wording library

Future optional wording variations could prevent repetition while remaining factual.

Avoid:

- Shame
- Threats
- Claims of medical benefit
- Punishment for taking longer
- Competitive wellness rankings

---

# Expansion 2.0

## In-world wellness encounters

The future Quest World may represent a break as:

- Camp
- Shrine
- Meal hall
- Healing spring
- Safe room
- Guild wagon stop

These remain presentation layers over real Approved Break data. They must never obscure actual duration or qualification rules.

---

## Food, rest, and survival systems

Future items and Biomes may affect:

- Hunger
- Rest
- Happiness
- Hazard resistance
- Travel endurance
- Companion needs

That system belongs with the 2.0 item, biome, and survival architecture rather than the current reminder command center.

---

# Completed

- Wellness reminders already supported Check-In, Hydration, Movement, Exercise, Lunch, Dinner, and Quiet Hours.
- Approved Break classification exists.
- 80% benefit qualification exists.
- Wellness Journal Timecard output exists.
- Dinner reminder passed previous Beta testing.
- Focus Check-In passed previous Beta testing.
- Supported audio host exists.
- Independent cue volume exists under the supported host.
- 0.31.7 persistent reminder queue implemented.
- 0.31.7 snooze persistence implemented.
- 0.31.7 quiet-hours end and suppression implemented.
- 0.31.7 local notification history implemented.
- 0.31.7 cue tests implemented.
- 0.31.7 Quest HUD wellness controls implemented.
- 0.31.7 Release Readiness wellness check implemented.

---

# Current Decision

After the 0.31.7 smoke test, the strongest next pathway is **0.31.8: Beta Administration and Content Validation**.

That pass should focus on:

- Catalog validation
- Contract validation
- Missing-reference diagnostics
- Bulk content review
- Safer generator reruns
- Exportable Beta health report
- Final pre-shipment issue consolidation

Large 2.0 systems remain deferred.
