# DeverQuest 0.31.0 Beta Test Checklist
## Quest 4 — Lanterns on the Battlefield

**Build:** 0.31.0 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

The 0.30.8 repeatable-Contract and 0.30.9 run-management checklists remain deferred for a later multi-account testing session.

---

# A. Installation and readiness

- [ ] Install `com.echodevgames.deverquest-0.31.0.tgz`.
- [ ] Confirm Package Manager reports 0.31.0.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm package version passes.
- [ ] Confirm Guild authority passes.
- [ ] Confirm Identity Catalog passes.
- [ ] Confirm Contract rewards pass.
- [ ] Confirm Quest Run reservations pass or have an understood advisory.
- [ ] Review the new Tactical test content result.
- [ ] Save the full readiness report.

---

# B. Tactical starter content

- [ ] Open **Guild Hall > Campaign Content Scaffolding**.
- [ ] Generate the Tactical Starter Kit and Quest Templates.
- [ ] Generate the Original Companion Stable.
- [ ] Confirm Encounter Profiles have valid scripts.
- [ ] Confirm Monster Profiles have valid scripts.
- [ ] Confirm Attack Techniques have valid scripts.
- [ ] Confirm Spells have valid scripts.
- [ ] Confirm Companion Profiles have valid scripts.
- [ ] Rerun generators.
- [ ] Confirm existing assets are preserved rather than duplicated.
- [ ] Run Release Readiness again.
- [ ] Confirm Tactical test content passes.

---

# C. Companion recruitment and persistence

- [ ] Open the Character workspace.
- [ ] Select a Companion Profile.
- [ ] Record any recruitment restriction.
- [ ] Recruit an eligible Companion.
- [ ] Rename the Companion with a name containing spaces.
- [ ] Set it active.
- [ ] Confirm only one Companion is active.
- [ ] Confirm its role, creature type, level, loyalty, and Hit Points display.
- [ ] Confirm lifetime damage, healing, and damage taken begin at zero for a new Companion.
- [ ] Restart Unity.
- [ ] Confirm the Companion and custom name persist.
- [ ] Confirm active state persists.

---

# D. Encounter preview

Create or select a Quest Contract containing an Encounter Profile.

- [ ] Accept the Quest.
- [ ] Open the Quest workspace.
- [ ] Confirm **Tactical Encounter Preview** appears before resolution.
- [ ] Confirm Encounter name is readable.
- [ ] Confirm Fixed or Survival mode is shown.
- [ ] Confirm configured foe count is shown.
- [ ] Confirm par rounds are shown.
- [ ] Confirm victory coin and XP are shown.
- [ ] Confirm a missing Encounter Profile fails safely with guidance.

---

# E. Fixed Encounter field report

- [ ] Resolve a fixed Encounter without a Companion.
- [ ] Confirm the report says Victory, Early Victory, Defeat, or Safety Pause.
- [ ] Confirm rounds and par are correct.
- [ ] Confirm starting and ending Hit Points are correct.
- [ ] Confirm Adventurer damage dealt is plausible.
- [ ] Confirm Adventurer damage taken is plausible.
- [ ] Confirm defeated enemies are grouped.
- [ ] Confirm battle rewards match the reward journal.
- [ ] Confirm loot matches inventory changes.
- [ ] Confirm recent turns are readable.
- [ ] Select **Copy Full Combat Log**.
- [ ] Paste it into a text editor and confirm the full transcript is present.
- [ ] Select **Copy Seed**.
- [ ] Confirm the deterministic seed copies correctly.

---

# F. Typed damage and conditions

Use starter or custom tactical content to exercise as many as practical:

- [ ] Normal damage.
- [ ] Resistant damage.
- [ ] Vulnerable damage.
- [ ] Immune damage.
- [ ] Absorbed damage.
- [ ] Damage over time.
- [ ] Healing over time.
- [ ] Life drain.
- [ ] Shield.
- [ ] Root.
- [ ] Snare.
- [ ] Stun.
- [ ] Silence.
- [ ] Attack buff.
- [ ] Attack debuff.
- [ ] Armor buff.
- [ ] Armor debuff.
- [ ] Cleanse.
- [ ] Dispel.
- [ ] Saving throw negates an effect.
- [ ] Confirm Conditions and Reactions lists meaningful effects without printing every basic hit.

---

# G. Companion contribution

Resolve at least one battle with an active Companion.

- [ ] Confirm the Companion joins the Encounter log.
- [ ] Confirm Companion damage appears separately.
- [ ] Confirm Companion damage taken appears separately.
- [ ] Confirm Companion hits and misses are counted plausibly.
- [ ] Use a Support Companion and confirm healing is reported when triggered.
- [ ] Confirm XP earned is shown.
- [ ] Confirm level change is shown when applicable.
- [ ] Confirm a fallen Companion is clearly reported.
- [ ] Return to the Companion Stable.
- [ ] Confirm Battles increments once.
- [ ] Confirm Victories increments only after victory.
- [ ] Confirm win rate updates.
- [ ] Confirm lifetime damage updates.
- [ ] Confirm lifetime healing updates when healing occurred.
- [ ] Confirm lifetime damage taken updates.
- [ ] Confirm Last Battle summary appears.
- [ ] Restart Unity and verify all totals persist.

---

# H. Safety pause, defeat, and recovery

- [ ] Configure a low-Hit-Point safety threshold.
- [ ] Trigger a safety pause.
- [ ] Confirm the report shows Safety Pause rather than Defeat.
- [ ] Confirm the safety reason is visible.
- [ ] Confirm no further enemy turn occurs after the safety trigger.
- [ ] Recover above the continuation threshold.
- [ ] Continue safely where supported.
- [ ] Trigger a defeat in disposable test data.
- [ ] Confirm defeat count increments.
- [ ] Confirm injury or Fallen state appears.
- [ ] Test resurrection when Fallen.
- [ ] Confirm coin cost and Hit Point recovery are correct.
- [ ] Test Companion recovery from the Stable.
- [ ] Confirm recovery cost and loyalty behavior.

---

# I. Survival visibility

Create or select a Survival Encounter.

- [ ] Confirm the panel displays completed waves.
- [ ] Confirm the next wave number is visible.
- [ ] Confirm current difficulty tier is visible.
- [ ] Confirm waves until difficulty increase are visible.
- [ ] Confirm waves until Guild Wagon are visible.
- [ ] Confirm focused minutes per wave are visible.
- [ ] Confirm carry weight and capacity are visible.
- [ ] Confirm exit status explains available methods.
- [ ] Complete one wave.
- [ ] Confirm the wave counter increments.
- [ ] Confirm rewards scale according to the profile.
- [ ] Reach a difficulty increase.
- [ ] Confirm the displayed tier changes.
- [ ] Reach a Guild Wagon checkpoint.
- [ ] Confirm the Wagon button becomes available.

---

# J. Survival exits

Use separate disposable runs where needed.

## Flee

- [ ] Attempt a Flee that fails.
- [ ] Confirm the expedition remains safely paused.
- [ ] Confirm the failure formula is readable.
- [ ] Attempt a successful Flee.
- [ ] Confirm the stage ends safely.
- [ ] Confirm Session records the Flee method.

## Homeward Passage

- [ ] Test without a prepared return ability.
- [ ] Confirm the button is disabled or the reason is clear.
- [ ] Test with a valid return ability.
- [ ] Confirm the stage ends safely.
- [ ] Confirm Session records Homeward Passage.

## Guild Wagon

- [ ] Attempt before the checkpoint.
- [ ] Confirm it is unavailable.
- [ ] Reach the checkpoint.
- [ ] Take the Guild Wagon.
- [ ] Confirm the stage ends safely.
- [ ] Confirm Session records Guild Wagon.

---

# K. Encumbrance during Survival

- [ ] Enter Survival below capacity.
- [ ] Acquire enough loot to become encumbered.
- [ ] Confirm combat safely pauses.
- [ ] Confirm the reason names carried loot and coin.
- [ ] Confirm item-drop controls appear.
- [ ] Drop one permitted item.
- [ ] Confirm carry weight changes.
- [ ] Attempt to continue while still encumbered.
- [ ] Confirm continuation is denied.
- [ ] Clear encumbrance.
- [ ] Confirm continuation becomes available.

---

# L. Timecard and Chronicle

Complete one fixed Encounter and one Survival expedition.

- [ ] Open the generated Timecard.
- [ ] Confirm Battle Chronicle contains Outcome.
- [ ] Confirm Damage Report appears.
- [ ] Confirm Conditions and Reactions appear when relevant.
- [ ] Confirm Companion Contribution appears when relevant.
- [ ] Confirm Bonus Rewards are correct.
- [ ] Confirm defeated enemies and loot are correct.
- [ ] Confirm Tactical Actions remain available.
- [ ] Confirm Combat Highlights show no more than ten entries.
- [ ] Confirm the full combat log is inside a collapsible `<details>` block.
- [ ] Expand the details block in a compatible Markdown viewer.
- [ ] Confirm the complete transcript remains available.
- [ ] Confirm Survival exit method appears in the Encounter line.
- [ ] Confirm Guild audit records the Survival exit.
- [ ] Verify Chronicle integrity.

---

# M. Legacy compatibility

- [ ] Open a Timecard created before 0.31.0.
- [ ] Confirm it still reads normally.
- [ ] Load a Session containing an older Battle Result.
- [ ] Confirm the new UI does not throw an exception.
- [ ] Confirm missing typed-damage data receives a safe fallback.
- [ ] Confirm existing Companion data loads with zero contribution totals.
- [ ] Resolve one new battle.
- [ ] Confirm only the new battle adds lifetime metrics.

---

# N. Layout and performance

- [ ] Test the Quest window docked wide.
- [ ] Test it near minimum width.
- [ ] Confirm long Encounter names wrap.
- [ ] Confirm long condition summaries wrap.
- [ ] Confirm long Companion names wrap.
- [ ] Confirm fifty or more combat-log entries do not flood the visible Quest panel.
- [ ] Confirm switching workspaces remains responsive.
- [ ] Confirm Copy buttons do not change battle data.
- [ ] Confirm no GUILayout errors occur.

---

# Verdict

- [ ] **PASS** — visibility, persistence, Survival exits, and Timecard summaries are correct.
- [ ] **CONDITIONAL PASS** — core reporting works; rare tactical effects remain untested.
- [ ] **FAIL** — combat results, Companion totals, Survival state, rewards, or persistence are inconsistent.
