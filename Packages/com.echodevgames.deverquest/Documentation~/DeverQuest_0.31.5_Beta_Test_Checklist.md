# DeverQuest 0.31.5 Beta Test Checklist
## Quest 8 — The Cartographer’s Desk

**Build:** 0.31.5 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

This focused checklist tests Editor organization, the dockable Quest HUD, local visual settings, and the separation of Quest Log from Git. Deferred gameplay checklists remain separate.

---

# A. Installation and Readiness

- [ ] Install `com.echodevgames.deverquest-0.31.5.tgz`.
- [ ] Confirm Package Manager reports 0.31.5.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm Editor workspace configuration passes.
- [ ] Confirm the report names the selected theme.
- [ ] Confirm the report names the workspace-column count.
- [ ] Confirm the report names the HUD auto-open state.
- [ ] Confirm no Quest is created by the readiness run.

---

# B. Workspace Navigation

- [ ] Confirm Current Quest exists.
- [ ] Confirm Quest Log exists.
- [ ] Confirm Chronicle exists.
- [ ] Confirm Git exists separately.
- [ ] Confirm Character exists.
- [ ] Confirm Inventory exists.
- [ ] Confirm Economy exists.
- [ ] Confirm Tactics exists.
- [ ] Confirm Guild Hall exists.
- [ ] Confirm Rewards & History exists.
- [ ] Confirm Audio & Wellness exists.
- [ ] Confirm Visuals exists.
- [ ] Confirm Settings exists.
- [ ] Open every workspace once.
- [ ] Confirm no Console error.
- [ ] Confirm no workspace displays internal repaint-path or AssetDatabase implementation text.

---

# C. Workspace Layout

In Visuals:

- [ ] Set Workspace Columns to 2.
- [ ] Confirm the workspace grid uses two columns.
- [ ] Set Workspace Columns to 6.
- [ ] Confirm the workspace grid uses six columns.
- [ ] Enable Compact Workspace Labels.
- [ ] Confirm shorter labels appear.
- [ ] Disable Compact Workspace Labels.
- [ ] Confirm full labels return.
- [ ] Disable Workspace Guidance.
- [ ] Confirm workspace help text disappears.
- [ ] Enable Workspace Guidance.
- [ ] Confirm useful user-facing guidance returns.
- [ ] Test a narrow dock.
- [ ] Test a wide dock.
- [ ] Confirm no tab becomes inaccessible.

---

# D. Quest Log Without an Active Quest

- [ ] Open Quest Log with no active Quest.
- [ ] Confirm a clear empty-state message.
- [ ] Select Open Current Quest.
- [ ] Return to Quest Log.
- [ ] Select Open Completed Chronicle.
- [ ] Return to Quest Log.
- [ ] Select Open Git.
- [ ] Confirm no Session is created by navigation.

---

# E. Git Without an Active Quest

- [ ] Open Git with no active Quest.
- [ ] Confirm repository status appears.
- [ ] Confirm the message explains that work is not attached to focused time.
- [ ] Refresh repository status.
- [ ] Confirm Branch.
- [ ] Confirm Current Commit.
- [ ] Confirm staged, modified, and untracked counts.
- [ ] Confirm upstream state.
- [ ] Enter a Git commit message.
- [ ] Navigate away and back during the same Editor session.
- [ ] Confirm the Git message remains.
- [ ] Confirm no Quest Log entry is created merely by typing.

---

# F. Quest Log and Git Text Separation

Start a small Quest.

- [ ] Open Quest Log.
- [ ] Enter `Quest note alpha` in Quest Log Entry.
- [ ] Open Git.
- [ ] Enter `Git commit beta` in Commit Message.
- [ ] Return to Quest Log.
- [ ] Confirm `Quest note alpha` remains.
- [ ] Confirm the Git message did not replace it.
- [ ] Add the Quest Log note.
- [ ] Return to Git.
- [ ] Confirm `Git commit beta` remains.
- [ ] Commit a small staged change.
- [ ] Confirm one Git Commit entry enters the active Quest Log.
- [ ] Confirm its comment is `Git commit beta`.
- [ ] Confirm the ordinary Quest Log note remains separate.
- [ ] Link another note to the current commit.
- [ ] Confirm the linked note uses the current branch and hash.

---

# G. Quest HUD Empty State

Open:

`Tools > DeverQuest > Quest HUD`

- [ ] Confirm the window opens as a normal dockable EditorWindow.
- [ ] Dock it beside Inspector, Scene, or Console.
- [ ] Confirm no active Quest message.
- [ ] Confirm Last Completed appears when available.
- [ ] Confirm Open Quest Board works.
- [ ] Confirm Open Chronicle works.
- [ ] Close and reopen the HUD.
- [ ] Confirm no Session or focused time is created.

---

# H. Quest HUD Active State

Start a five-minute Quest.

- [ ] Confirm Task.
- [ ] Confirm Project.
- [ ] Confirm focused timer.
- [ ] Confirm Predicted Task Length.
- [ ] Confirm progress bar.
- [ ] Confirm remaining time.
- [ ] Confirm state is Working.
- [ ] Confirm Current Encounter when configured.
- [ ] Confirm Quest Story when enabled.
- [ ] Confirm Task Objective.
- [ ] Confirm Latest Quest Event.
- [ ] Compare the HUD timer to Current Quest.
- [ ] Confirm both represent the same focused duration.
- [ ] Confirm only one active Session ID.
- [ ] Confirm only one Quest Run ID.

---

# I. Quest HUD Controls

- [ ] Select Meditate in the HUD.
- [ ] Confirm Current Quest also shows Paused.
- [ ] Resume from Current Quest.
- [ ] Confirm the HUD returns to Working.
- [ ] Start the configured short break from the HUD.
- [ ] Confirm Approved Break remaining time.
- [ ] Confirm minimum qualifying duration.
- [ ] Resume after the break.
- [ ] Trigger idle pause.
- [ ] Confirm the HUD requires return acknowledgement.
- [ ] Acknowledge from the HUD.
- [ ] Resume.
- [ ] Select Open Quest Turn-In.
- [ ] Confirm the main DeverQuest window opens the normal turn-in flow.
- [ ] Cancel turn-in and return to the Quest.
- [ ] Confirm no reward was granted.

---

# J. HUD Auto-Open

In Visuals:

- [ ] Enable Open HUD When Quest Starts.
- [ ] Close the HUD.
- [ ] Complete or abandon the active QA Quest.
- [ ] Start another Quest.
- [ ] Confirm the HUD opens.
- [ ] Disable auto-open.
- [ ] Close the HUD.
- [ ] Start another Quest.
- [ ] Confirm the HUD does not force itself open.

---

# K. HUD Story Option

- [ ] Enable Show Story in HUD.
- [ ] Start a Contract with Quest Story.
- [ ] Confirm the story appears.
- [ ] Disable Show Story in HUD.
- [ ] Confirm the story disappears after repaint.
- [ ] Confirm the story remains in Current Quest and Chronicle.
- [ ] Confirm no Contract or Session data changed.

---

# L. Visual Themes

- [ ] Select System.
- [ ] Select Dark.
- [ ] Select Light.
- [ ] Select Echo Neon.
- [ ] Select Custom.
- [ ] Change Title color.
- [ ] Change Timer color.
- [ ] Change Accent color.
- [ ] Confirm the preview updates.
- [ ] Confirm the main header updates.
- [ ] Confirm the Quest HUD updates.
- [ ] Restart Unity.
- [ ] Confirm the selected theme and colors persist.

---

# M. Text Scale and Header

- [ ] Set DeverQuest Text Scale to minimum.
- [ ] Confirm titles and timer shrink without clipping.
- [ ] Set it to maximum.
- [ ] Confirm titles and timer enlarge without breaking layout.
- [ ] Restore 1.0.
- [ ] Disable Header Tagline.
- [ ] Confirm the tagline disappears.
- [ ] Enable Header Tagline.
- [ ] Confirm it returns.
- [ ] Confirm Unity's global Editor scale is unchanged.

---

# N. Reset Visual Settings

- [ ] Change theme, colors, scale, columns, labels, guidance, tagline, and HUD settings.
- [ ] Select Reset Visual Settings.
- [ ] Cancel once.
- [ ] Confirm nothing changes.
- [ ] Select reset again and confirm.
- [ ] Confirm Echo Neon.
- [ ] Confirm scale 1.0.
- [ ] Confirm four columns.
- [ ] Confirm full labels.
- [ ] Confirm guidance enabled.
- [ ] Confirm tagline enabled.
- [ ] Confirm HUD auto-open disabled.
- [ ] Confirm HUD story enabled.

---

# O. Turn-In and Chronicle Regression

- [ ] Complete one Quest after using the HUD.
- [ ] Confirm one completion.
- [ ] Confirm one reward award.
- [ ] Confirm one Timecard Session.
- [ ] Confirm one Contract Completion History record.
- [ ] Confirm Quest Log note appears.
- [ ] Confirm Git commit entry appears.
- [ ] Confirm Chronicle timeline remains correct.
- [ ] Confirm HUD switches to no-active-Quest state.
- [ ] Confirm Last Completed is displayed.

---

# P. Safety

- [ ] Open multiple views of the same Quest.
- [ ] Confirm focused time does not multiply.
- [ ] Confirm pause affects all views.
- [ ] Confirm resume affects all views.
- [ ] Confirm Encounter completion is recorded once.
- [ ] Confirm stage reward is recorded once.
- [ ] Confirm opening Visuals does not change Quest state.
- [ ] Confirm changing colors does not rewrite Timecards.
- [ ] Confirm Git remains explicit and never force-pushes.
- [ ] Confirm closing the HUD does not abandon the Quest.

---

# Verdict

- [ ] **PASS** — workspace organization, HUD, and local visuals are stable.
- [ ] **CONDITIONAL PASS** — core behavior passes; minor narrow-layout polish remains.
- [ ] **FAIL** — duplicate timing, duplicate rewards, lost Quest Log text, broken Git actions, or persistent layout errors occur.
