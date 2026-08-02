# DeverQuest 0.30.7 Beta Test Checklist
## Quest 2 — The Full Loop Expedition

**Build:** 0.30.7 Beta 1
**Unity target:** 2022.3 minimum; current test environment 6000.3.8f1
**Status legend:** `[x] PASS` · `[?] CONDITIONAL/BLOCKED` · `[-] FAIL` · `[ ] NOT TESTED`

This checklist preserves the completed 0.30.6 evidence and expands the next testing campaign across the entire existing package.

---

# A. Installation and Release Readiness

- [ ] Install `com.echodevgames.deverquest-0.30.7.tgz`.
- [ ] Confirm Package Manager reports 0.30.7.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Clear the Console.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm no release blockers.
- [ ] Confirm Legacy repository naming passes.
- [ ] Confirm repository documentation passes.
- [ ] Confirm Guild authority passes.
- [ ] Confirm Starter Identity Catalog passes.
- [ ] Confirm no refreshable Contract Spoils mismatch remains.
- [ ] Confirm Timecard Git hygiene passes or produces an understood advisory.
- [ ] Confirm no third-party media exists inside the package tarball.
- [ ] Confirm `.gitignore` excludes `/DeverQuestTimecards/` when the timecard root is inside the repository.
- [ ] Save the readiness report with the Beta evidence.

---

# B. Sole Founder, Authentication, and Character Onboarding

- [ ] Confirm the only active account is CEO.
- [ ] Confirm Character Sheet also reports CEO.
- [ ] Complete a Quest and confirm the rank remains CEO.
- [ ] Restart Unity and confirm the rank remains CEO.
- [ ] Confirm CEO/Boss content controls are enabled.
- [ ] Select **Customize Current Adventurer Identity…**.
- [ ] Confirm customization is blocked while a Quest is active.
- [ ] Reopen customization with no active Quest.
- [ ] Choose a custom Adventurer name containing spaces.
- [ ] Choose an Ancestry.
- [ ] Choose a Class.
- [ ] Choose an Alignment.
- [ ] Choose a Faith.
- [ ] Complete creation.
- [ ] Confirm starting coin is at least five silver.
- [ ] Confirm existing XP and level remain intact.
- [ ] Confirm Chronicle history remains intact.
- [ ] Restart Unity.
- [ ] Confirm the customized identity persists.
- [ ] Confirm Identity Catalog assets still have valid scripts.

---

# C. Music, Ambience, and Warning Audio

## Basic transport

- [ ] Assign a playlist with at least three tracks.
- [ ] Assign an Ambience Profile with at least three clips.
- [ ] Select a specific Music track from the new dropdown.
- [ ] Select a specific Ambience track from the new dropdown.
- [ ] Play Music.
- [ ] Play Ambience.
- [ ] Confirm both are audible.
- [ ] Stop Music and confirm Ambience continues.
- [ ] Restart Music.
- [ ] Stop Ambience and confirm Music continues.
- [ ] Pause and resume Music.
- [ ] Change only Music.
- [ ] Confirm Ambience remains selected and playing.
- [ ] Change only Ambience.
- [ ] Confirm Music remains selected and playing.

## Inspector-interruption recovery

- [ ] Play both channels.
- [ ] Preview an unrelated AudioClip in the Inspector.
- [ ] Stop the Inspector preview.
- [ ] Confirm DeverQuest recovers automatically.
- [ ] Preview a looping clip in the Inspector.
- [ ] Use **Recover Audio Transport**.
- [ ] Confirm Music and Ambience return.
- [ ] Use **Stop and Reset All Audio**.
- [ ] Confirm all sound stops.
- [ ] Confirm Music and Ambience controls become usable again.
- [ ] Trigger Warning.
- [ ] Trigger Victory.
- [ ] Trigger Level Up.
- [ ] Confirm warning cues do not permanently disable either long-form channel.
- [ ] Rapidly alternate Previous, Next, Play, Pause, Stop, and both selectors for two minutes.
- [ ] Confirm no third or abandoned track remains audible.

## Focus and lifecycle

- [ ] Switch among every DeverQuest workspace while Music plays.
- [ ] Confirm Music does not stop merely because another workspace is selected.
- [ ] Switch focus to another application.
- [ ] Return to Unity.
- [ ] Confirm expected audio resumes without a manual Pause/Play toggle.
- [ ] Start a Quest with auto-play enabled.
- [ ] Pause the Quest and verify configured Music behavior.
- [ ] Resume the Quest and verify configured Music behavior.
- [ ] Complete the Quest and verify configured Music behavior.
- [ ] Assign an empty playlist and confirm safe guidance.
- [ ] Assign an empty Ambience Profile and confirm safe guidance.

## Known limitation

- [?] Confirm whether Unity exposes only global preview gain.
- [?] Record whether Music and Ambience volume sliders can be mixed independently.
- [?] Do not fail 0.30.7 solely because independent gain is unavailable when playback and transport controls remain reliable.

---

# D. Quest Profiles, Contracts, and Assignment Board

## Profile data

- [ ] Create a Quest Profile whose Display Name contains spaces.
- [ ] Create Project Name and Task Name values containing spaces.
- [ ] Save, deselect, reload, and confirm internal spaces persist.
- [ ] Confirm the Quest Board displays readable names.
- [ ] Confirm **Predicted Task Length** appears instead of Suggested Focus.
- [ ] Enter a Task Objective.
- [ ] Configure unmistakable base and work-block rewards.
- [ ] Save and reload the profile.

## Contract snapshot

- [ ] Create a Contract from the profile.
- [ ] Confirm the Contract copies Project, Task, Department, Objective, and reward snapshot.
- [ ] Confirm the Inspector labels the snapshot as copied from the Quest Profile.
- [ ] Change the profile while the Contract is Draft.
- [ ] Select the Contract in DeverQuest.
- [ ] Confirm the refreshable snapshot updates.
- [ ] Confirm Assignment Board base reward matches.
- [ ] Confirm Assignment Board work-block reward matches.
- [ ] Accept or activate the Contract.
- [ ] Change the profile again.
- [ ] Confirm the locked Contract warns instead of silently changing.
- [ ] Confirm the active Quest uses the Contract snapshot.
- [ ] Complete one work block and confirm the estimate increases exactly once.
- [ ] Complete the Quest.
- [ ] Compare Profile, Contract, active estimate, final award, wallet, XP, Chronicle, and Timecard.

## Acceptance diagnostics

- [ ] Select a Contract with no Project and record the blocking reason.
- [ ] Select a Contract with no Task and record the blocking reason.
- [ ] Select a Draft Contract as a Member and record the blocking reason.
- [ ] Select a level-restricted Contract with an under-level character.
- [ ] Select a class-restricted Contract with the wrong Class.
- [ ] Select an ancestry-restricted Contract with the wrong Ancestry.
- [ ] Select a department-restricted Contract with the wrong Department.
- [ ] Select a Contract assigned to another Adventurer.
- [ ] Confirm every disabled Accept button explains why.
- [ ] Fix each requirement and confirm acceptance becomes available.

---

# E. Party Quests

- [ ] Create a Party Quest with two required participants.
- [ ] Join as the first participant.
- [ ] Confirm the board reports enlisted/waiting.
- [ ] Confirm capacity displays 1/2.
- [ ] Confirm no solo timer begins.
- [ ] Select **Leave Party**.
- [ ] Confirm the roster returns to 0/2.
- [ ] Rejoin.
- [ ] Join with a second test account.
- [ ] Confirm the Contract changes from Offered to Accepted when full.
- [ ] Start the Party Quest.
- [ ] Confirm leaving is no longer allowed after activation.
- [ ] Submit one participant early.
- [ ] Confirm the Quest remains active until required submissions are complete.
- [ ] Confirm group bonuses are awarded once.
- [ ] Confirm each participant receives the intended reward.
- [ ] Confirm Chronicle and shared records name the party correctly.

---

# F. Quest Story, Encounters, and Progress

- [ ] Create a Contract with a Quest Story.
- [ ] Confirm the story appears while selecting the Contract.
- [ ] Accept the Quest.
- [ ] Confirm the story appears in the active Quest details.
- [ ] Add three Focus Stage records.
- [ ] Give one stage a custom title.
- [ ] Leave one stage title blank.
- [ ] Confirm the UI presents them as Encounters.
- [ ] Confirm the blank stage becomes `Encounter 2` or the correct sequence number.
- [ ] Confirm current Encounter and Encounter count are visible.
- [ ] Complete an Encounter.
- [ ] Confirm the completion notification uses Encounter wording.
- [ ] Confirm the progress estimate updates.
- [ ] Confirm base reward and work-block reward update correctly.
- [ ] Complete the Quest.
- [ ] Confirm the Timecard section is named **Encounters**.
- [ ] Confirm no blank Encounter title appears.
- [ ] Confirm Quest Story, Task Objective, and Contract Deliverables appear in the correct sections.
- [?] Record generic pacing messages that should later be replaced by procedural narrative.
- [?] Confirm full biome/room mad-lib storytelling is deferred to 2.0 rather than treated as a 0.30.7 failure.

---

# G. Timer, Pause, Idle, and Recovery

- [ ] Start a five-minute Quest.
- [ ] Confirm focused time increments.
- [ ] Pause manually.
- [ ] Confirm focused time stops.
- [ ] Resume.
- [ ] Confirm focused time continues.
- [ ] Trigger idle warning.
- [ ] Confirm the warning cue plays.
- [ ] Allow idle auto-pause.
- [ ] Confirm idle time is classified as Idle/Unverified.
- [ ] Resume after activity.
- [ ] Enter Meditation.
- [ ] Confirm Meditation is classified separately.
- [ ] Start an Approved Break.
- [ ] Confirm focused time stops.
- [ ] Close Unity during an active Quest.
- [ ] Reopen Unity.
- [ ] Recover the Quest.
- [ ] Confirm focused, paused, and classified durations remain plausible.
- [ ] Close Unity during a paused Quest.
- [ ] Recover again.
- [ ] Complete the Quest.
- [ ] Compare the live totals with the Timecard.

---

# H. Wellness

- [x] Dinner reminder appeared at the configured time.
- [x] Focus Check-In appeared.
- [x] Approved Break workflow began.
- [ ] Trigger Hydration.
- [ ] Trigger Movement Break.
- [ ] Trigger Exercise.
- [ ] Trigger Lunch.
- [ ] Trigger Dinner.
- [ ] Trigger Quiet Hours.
- [ ] Confirm every reminder displays recommended duration.
- [ ] Confirm every reminder displays minimum qualifying duration.
- [ ] Complete a break below 80%.
- [ ] Confirm it is recorded without the benefit.
- [ ] Complete a break at or above 80%.
- [ ] Confirm the benefit is recorded.
- [ ] End a break early.
- [ ] Confirm the Wellness Journal records the result.
- [ ] Snooze a reminder.
- [ ] Confirm it does not immediately recur.
- [ ] Acknowledge without taking a break.
- [ ] Confirm no break benefit is awarded.

---

# I. Chronicle, History, and Reporting

- [x] One-hour Timecard generated.
- [x] Commit Journal entries generated.
- [x] Wellness Journal entries generated.
- [x] Voice memo attachment generated.
- [x] Reward Journal generated.
- [ ] Confirm completed Quest appears in Rewards & History.
- [ ] Open the Timecard from the UI.
- [ ] Reveal the Timecard in the file browser.
- [ ] Filter history by date.
- [ ] Filter by Project.
- [ ] Filter by Department.
- [ ] Export CSV.
- [ ] Export JSON.
- [ ] Verify Chronicle integrity.
- [ ] Modify a Chronicle manually.
- [ ] Confirm integrity reports Modified.
- [ ] Submit a correction request.
- [ ] Approve or reject it with an authorized account.
- [ ] Start a new Chronicle.
- [ ] Confirm the next completed Quest writes to the new Chronicle.
- [ ] Confirm individual Quest rewards are readable in history.
- [?] Record the need for a dedicated collapsible Completed Quest Log as a Medium-priority UI feature.

---

# J. Git and Media

- [ ] Confirm `/DeverQuestTimecards/` is ignored or moved outside the repository.
- [ ] Attach a five-second voice memo.
- [ ] Confirm the memo appears in the active Quest.
- [ ] Reveal the memo.
- [ ] Unlink the memo.
- [ ] Attach an existing file.
- [ ] Add a Quest Log note without a commit.
- [ ] Add a linked commit note.
- [ ] Stage a small source change.
- [ ] Commit from DeverQuest.
- [ ] Confirm the Commit Journal links the new commit once.
- [ ] Commit from GitHub Desktop.
- [ ] Confirm DeverQuest detects it without freezing.
- [ ] Run another Git process for more than 15 seconds.
- [ ] Confirm the automatic monitor does not block Unity.
- [ ] Push a small commit.
- [ ] Confirm success.
- [ ] Simulate a 30-second Git timeout.
- [ ] Confirm a readable message appears.
- [ ] Confirm no Unity busy dialog remains attached to `DeverQuestGitMonitor.Update`.
- [ ] Confirm media and generated reports are not accidentally staged.

---

# K. Rewards, Coin, Shop, and Inventory

- [ ] Confirm the new character begins with five silver.
- [ ] Complete a Quest with base coin only.
- [ ] Complete a Quest with work-block coin.
- [ ] Complete a Quest with XP only.
- [ ] Confirm denomination display is correct.
- [ ] Exchange denominations at Guild Hall.
- [ ] Confirm total canonical copper value does not change.
- [ ] Create or select a Shop Profile.
- [ ] Buy an affordable item.
- [ ] Attempt an unaffordable purchase.
- [ ] Confirm the ledger records the purchase.
- [ ] Equip an item.
- [ ] Unequip it.
- [ ] Add stackable inventory.
- [ ] Add non-stackable inventory.
- [ ] Test carry weight.
- [ ] Cross the encumbrance threshold.
- [ ] Confirm the Quest safety behavior.
- [ ] Drop an allowed item during a Quest.
- [ ] Attempt to drop a protected item.
- [ ] Confirm inventory persists after restart.
- [?] Record the town-only denomination-conversion rule as a future economy change rather than changing 0.30.7 data silently.

---

# L. Trading and Redemptions

- [ ] Create two test accounts.
- [ ] Offer a tradable item.
- [ ] Confirm escrow removes it from usable inventory.
- [ ] Accept the trade.
- [ ] Confirm ownership transfers.
- [ ] Reject a second trade.
- [ ] Cancel a third trade.
- [ ] Reclaim an abandoned escrow item.
- [ ] Attempt to trade a bound item.
- [ ] Attempt to trade a protected Quest item.
- [ ] Confirm every action appears in the Trade Ledger.
- [ ] Request a real-world redemption.
- [ ] Approve it as leadership.
- [ ] Mark it fulfilled.
- [ ] Confirm DeverQuest never claims external delivery occurred automatically.

---

# M. Guild Accounts, Permissions, and Shared Records

- [ ] Create Boss, Project Leader, and Member test accounts.
- [ ] Confirm CEO permissions.
- [ ] Confirm Boss restrictions.
- [ ] Assign a Project Leader to one Project.
- [ ] Confirm Project Leader management is limited to that Project.
- [ ] Confirm Member cannot generate studio content.
- [ ] Confirm Member can accept eligible Contracts.
- [ ] Disable an account.
- [ ] Confirm login is denied.
- [ ] Change an account passcode.
- [ ] Log out and log back in.
- [ ] Enable Shared Guild publishing with a clean repository.
- [ ] Publish a completed record.
- [ ] Publish an Adventurer snapshot.
- [ ] Confirm Hall of Heroes updates.
- [ ] Confirm duplicate publication is handled safely.
- [ ] Disable Shared Guild publishing.
- [ ] Confirm local work continues.

---

# N. Companions

- [ ] Generate the Original Companion Stable.
- [ ] Confirm Companion Profiles have valid scripts.
- [ ] Recruit a Companion.
- [ ] Attempt to recruit an ineligible Companion.
- [ ] Activate one Companion.
- [ ] Switch active Companion.
- [ ] Complete a Quest with a Companion.
- [ ] Run a deterministic encounter.
- [ ] Confirm role behavior is visible.
- [ ] Confirm Companion HP persists.
- [ ] Confirm loyalty, battles, victories, and XP persist.
- [ ] Test a fallen Companion.
- [ ] Recover it through the configured rule.
- [ ] Confirm Timecard and Chronicle record meaningful Companion contributions.
- [ ] Restart Unity and confirm the stable persists.

---

# O. Combat, Abilities, and Survival

- [ ] Generate the Tactical Starter Kit.
- [ ] Confirm Ability Profiles, Spells, Encounter Profiles, and templates have valid scripts.
- [ ] Equip a weapon.
- [ ] Teach a spell.
- [ ] Run a standard deterministic encounter.
- [ ] Confirm attack, damage, healing, mitigation, and result are visible.
- [ ] Test resistance.
- [ ] Test vulnerability.
- [ ] Test immunity.
- [ ] Test absorption.
- [ ] Test shield or blocking.
- [ ] Test stun, root, snare, silence, cleanse, and dispel.
- [ ] Confirm cooldown and mana behavior.
- [ ] Test low-HP safety pause.
- [ ] Test Encounter Danger cue.
- [ ] Run a Survival Quest.
- [ ] Complete multiple waves.
- [ ] Use an escape or return action.
- [ ] Test defeat.
- [ ] Confirm rewards do not double.
- [ ] Confirm Combat Chronicle remains compact enough to read.
- [?] Record where richer feedback is needed before changing combat architecture.

---

# P. Backup, Migration, and Failure Recovery

- [ ] Back up EditorPrefs-based local data.
- [ ] Back up timecards.
- [ ] Back up generated project assets.
- [ ] Upgrade from 0.30.6 with an active character.
- [ ] Confirm CEO rank survives.
- [ ] Confirm coin, XP, inventory, Contracts, and Catalog references survive.
- [ ] Remove the package and reinstall 0.30.7.
- [ ] Confirm project-owned ScriptableObject assets survive.
- [ ] Move the timecard root.
- [ ] Confirm history reads from the new location.
- [ ] Make the timecard folder read-only.
- [ ] Run readiness and confirm a blocker.
- [ ] Restore write access.
- [ ] Break the shared Guild path.
- [ ] Confirm a readable blocker.
- [ ] Delete a selected audio asset.
- [ ] Confirm safe fallback.
- [ ] Delete one generated Catalog entry.
- [ ] Rerun the generator.
- [ ] Confirm repair without duplication.
- [ ] Restart after every repair and verify persistence.

---

# Q. UI and Accessibility Review

- [ ] Test DeverQuest docked wide.
- [ ] Test DeverQuest docked narrow.
- [ ] Test Compact View.
- [ ] Confirm no Contract expands the panel to an unreadable width.
- [ ] Confirm long Project, Task, Story, and Deliverable text wraps.
- [ ] Confirm developer-only repaint-path explanation is no longer visible.
- [ ] Confirm all disabled buttons provide understandable guidance.
- [ ] Confirm terminology is internally consistent enough for Beta.
- [?] Record candidates for separate Quest Board, Git, Completed Log, Visuals, and Mod Tools tabs.
- [?] Record character portrait and dockable HUD requests as Medium priority.
- [?] Record color profiles and accessibility presets as Medium priority.

---

# Final Verdict

## P0 release gates

- [ ] No compilation errors.
- [ ] No data loss.
- [ ] No authority demotion.
- [ ] No unrecoverable audio loop.
- [ ] No main-thread Git freeze.
- [ ] No incorrect reward award.
- [ ] No Chronicle corruption.
- [ ] No broken package asset association.

## Verdict

- [ ] **PASS** — all P0 gates pass and the full loop is stable.
- [ ] **CONDITIONAL PASS** — no P0 failure; documented P1 limitations remain.
- [ ] **FAIL** — any P0 gate fails.

### Evidence folder

Record:

- Readiness report
- Console logs
- Screenshots
- Generated Timecards
- Exported reports
- Git commit hashes
- Exact asset paths
- Issue IDs
