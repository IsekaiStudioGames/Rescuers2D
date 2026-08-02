# DeverQuest 0.31.1 Beta Test Checklist
## Quest 5 — The Tactician's Ledger

**Build:** 0.31.1 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

The larger 0.30.8 repeatable-Contract, 0.30.9 run-management, and 0.31.0 full tactical matrices remain available for later dedicated testing.

---

# A. Installation and readiness

- [ ] Install `com.echodevgames.deverquest-0.31.1.tgz`.
- [ ] Confirm Package Manager reports 0.31.1.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm the existing readiness checks remain green or understood.
- [ ] Confirm **Tactical archive** passes.
- [ ] Record the number of archived Battle Results reported.
- [ ] Save the readiness output.

---

# B. Tactics workspace

- [ ] Open the new **Tactics** tab.
- [ ] Open it from **Tools > DeverQuest > Workspaces > Tactical Operations**.
- [ ] Confirm both navigation methods open the same workspace.
- [ ] Confirm Adventurer name, Class, level, and Guild rank are visible.
- [ ] Confirm Hit Points, Mana, Armor Class, and carry load are correct.
- [ ] Confirm status effects are shown.
- [ ] Confirm equipped items are shown.
- [ ] Confirm known tactical actions are shown.
- [ ] Test the workspace docked wide.
- [ ] Test it near minimum width.
- [ ] Confirm long names wrap without forcing an extreme panel width.

---

# C. Combat readiness warnings

Using disposable data where needed:

- [ ] Confirm a healthy Adventurer shows a ready state.
- [ ] Lower Hit Points to 25% or less.
- [ ] Confirm a low-health warning appears.
- [ ] Mark the Adventurer Fallen.
- [ ] Confirm the Fallen warning appears.
- [ ] Use **Resurrect at the Guild Shrine**.
- [ ] Confirm cost and half-Hit-Point restoration match the existing rule.
- [ ] Become encumbered.
- [ ] Confirm the encumbrance warning appears.
- [ ] Clear encumbrance.
- [ ] Confirm the warning disappears.

---

# D. Current Encounter operations

- [ ] Start a Quest with an Encounter Profile.
- [ ] Open Tactics.
- [ ] Confirm the current Encounter name is visible.
- [ ] Confirm Fixed or Survival details are visible.
- [ ] For Survival, confirm next-wave and milestone text is visible.
- [ ] Click **Select Encounter Profile**.
- [ ] Confirm the correct asset is selected and pinged.
- [ ] Click **Open Current Quest**.
- [ ] Confirm the Quest workspace opens.
- [ ] Return to Tactics.
- [ ] Complete or abandon the Quest.
- [ ] Confirm Tactics returns to the no-active-Encounter state safely.

---

# E. Companion quick operations

Prepare at least two disposable Companions.

- [ ] Confirm the roster dropdown lists both.
- [ ] Confirm the active Companion is marked `[ACTIVE]`.
- [ ] Select a resting Companion.
- [ ] Click **Set Active**.
- [ ] Confirm the old active Companion becomes resting.
- [ ] Confirm the newly selected Companion becomes active.
- [ ] Click **Send to Stable**.
- [ ] Confirm no Companion remains active.
- [ ] Set one Companion active again.
- [ ] Restart Unity.
- [ ] Confirm the active selection persists.
- [ ] Confirm role, creature type, level, loyalty, and Hit Points are correct.
- [ ] Confirm lifetime damage and healing match the Companion Stable.

---

# F. Individual Companion recovery

- [ ] Damage a Companion without making it Fallen.
- [ ] Confirm the recovery button shows its configured cost.
- [ ] Recover it.
- [ ] Confirm Hit Points reach maximum.
- [ ] Confirm the purse decreases once.
- [ ] Confirm the Guild audit records recovery.
- [ ] Mark a Companion Fallen.
- [ ] Confirm it cannot be activated.
- [ ] Recover it.
- [ ] Confirm Fallen clears.
- [ ] Confirm loyalty follows the existing recovery rule.
- [ ] Confirm a healthy Companion shows `Ready` rather than a payable action.

---

# G. Full-roster recovery

- [ ] Injure at least two Companions.
- [ ] Record each recovery cost.
- [ ] Confirm the roster button displays the combined total.
- [ ] Cancel the confirmation once.
- [ ] Confirm nothing changes.
- [ ] Confirm recovery with sufficient coin.
- [ ] Confirm all eligible Companions recover.
- [ ] Confirm the purse decreases by the exact combined total.
- [ ] Confirm one roster-recovery audit entry appears.
- [ ] Injure multiple Companions again.
- [ ] Reduce the purse below the combined cost.
- [ ] Attempt roster recovery.
- [ ] Confirm no Companion is partially recovered.
- [ ] Confirm the insufficient-funds message is clear.

---

# H. Automatic Battle Archive recording

- [ ] Record the starting archive count.
- [ ] Resolve a fixed Encounter.
- [ ] Confirm the archive count increases by one.
- [ ] Confirm the newest record is the battle just resolved.
- [ ] Confirm Project and Task context are correct.
- [ ] Confirm Adventurer name is correct.
- [ ] Confirm Encounter name and outcome are correct.
- [ ] Confirm rounds and rewards are correct.
- [ ] Resolve one Survival wave.
- [ ] Confirm a separate Survival record is added.
- [ ] Resolve another wave.
- [ ] Confirm it receives a distinct record rather than replacing the first.
- [ ] Restart Unity.
- [ ] Confirm records persist.

---

# I. Import and duplicate protection

- [ ] Click **Import Current and Last Quest Reports**.
- [ ] Record how many reports are imported.
- [ ] Click it again without resolving another battle.
- [ ] Confirm zero new records are imported.
- [ ] Complete another battle.
- [ ] Import again.
- [ ] Confirm only the new result is added if automatic recording was unavailable.
- [ ] Confirm no duplicate seed/time combination appears.

---

# J. Archive search and filters

- [ ] Search by Project.
- [ ] Search by Task.
- [ ] Search by Adventurer.
- [ ] Search by Companion.
- [ ] Search by Encounter.
- [ ] Search by Quest Run ID.
- [ ] Search by deterministic seed.
- [ ] Confirm unmatched searches show a safe empty state.
- [ ] Filter All Outcomes.
- [ ] Filter Victory.
- [ ] Filter Early Victory.
- [ ] Filter Safety Pause.
- [ ] Filter Defeat.
- [ ] Filter Survival.
- [ ] Confirm each filter excludes unrelated records.
- [ ] Confirm the UI shows stored count and displayed count.

---

# K. Archive evidence actions

For one archived battle:

- [ ] Click **Copy Report**.
- [ ] Paste it into a text editor.
- [ ] Confirm the full readable combat report is present.
- [ ] Click **Copy JSON**.
- [ ] Confirm the JSON contains Session context and Battle Result data.
- [ ] Click **Select Profile**.
- [ ] Confirm the correct Encounter Profile is selected.
- [ ] Delete or temporarily remove the Encounter Profile in disposable data.
- [ ] Confirm Select Profile becomes safely unavailable.
- [ ] Click **Remove** on one archive record.
- [ ] Confirm only that local record disappears.
- [ ] Confirm its Timecard still exists.

---

# L. Archive cap and clearing

This section may be deferred or tested with generated disposable records.

- [?] Create or import more than 100 unique Battle Results.
- [?] Confirm only the newest 100 remain.
- [ ] Click **Clear Local Archive**.
- [ ] Cancel once and confirm nothing changes.
- [ ] Confirm clearing.
- [ ] Confirm the archive becomes empty.
- [ ] Confirm Timecards remain on disk.
- [ ] Confirm Chronicle integrity is unaffected.
- [ ] Resolve another battle.
- [ ] Confirm archiving starts again normally.

---

# M. Legacy compatibility

- [ ] Upgrade a project containing 0.31.0 Battle Results.
- [ ] Confirm Unity loads without migration errors.
- [ ] Import the last 0.31.0 Session.
- [ ] Confirm its result appears in the archive.
- [ ] Open a pre-0.31.0 Timecard.
- [ ] Confirm it remains readable.
- [ ] Confirm existing Companion state is unchanged.
- [ ] Confirm existing rewards and Chronicle records are unchanged.

---

# N. Performance and safety

- [ ] Open Tactics with no active Quest.
- [ ] Open Tactics during an active timer.
- [ ] Confirm the timer continues to repaint.
- [ ] Populate at least 50 archive records where practical.
- [ ] Search rapidly.
- [ ] Switch filters repeatedly.
- [ ] Confirm Unity remains responsive.
- [ ] Confirm the Tactics workspace does not resolve an Encounter merely by opening it.
- [ ] Confirm archive actions do not award coin or XP.
- [ ] Confirm Companion selection does not create focused-work time.

---

# Verdict

- [ ] **PASS** — Tactics, Companion operations, and Battle Archive are stable.
- [ ] **CONDITIONAL PASS** — core operations pass; archive-cap or large-data stress remains deferred.
- [ ] **FAIL** — data loss, duplicate rewards, broken Companion state, archive corruption, or compilation failure occurs.
