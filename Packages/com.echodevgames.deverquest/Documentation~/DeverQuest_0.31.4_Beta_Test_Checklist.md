# DeverQuest 0.31.4 Beta Test Checklist
## Quest 7 — The Living Chronicle

**Build:** 0.31.4 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

The larger deferred checklists for repeatable Contracts, Party behavior, tactical systems, Inventory, and Economy remain separate. This checklist focuses only on Chronicle navigation and its safety boundary.

---

# A. Installation and Readiness

- [ ] Install `com.echodevgames.deverquest-0.31.4.tgz`.
- [ ] Confirm Package Manager reports 0.31.4.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run **Tools > DeverQuest > Run Release Readiness Check**.
- [ ] Confirm Package version passes.
- [ ] Confirm Quest Chronicle archive passes or produces an understood advisory.
- [ ] Record duplicate Session ID count.
- [ ] Record missing Timecard count.
- [ ] Record missing attachment count.
- [ ] Confirm no active Quest is modified by the readiness run.

---

# B. Chronicle Workspace Entry

Open:

```text
Tools > DeverQuest > Workspaces > Quest Archive and Chronicle
```

or select:

```text
Chronicle
```

Tests:

- [ ] Confirm the workspace opens without a Console error.
- [ ] Confirm its title and explanation render.
- [ ] Confirm Live Quest Chronicle renders.
- [ ] Confirm Completed Quest Archive renders.
- [ ] Confirm the panel remains readable in a narrow dock.
- [ ] Confirm the panel remains readable in a wide dock.
- [ ] Confirm switching away and back preserves the current search during the same Editor session.

---

# C. No Active Quest

- [ ] Open Chronicle with no active Quest.
- [ ] Confirm a clear no-active-Quest message appears.
- [ ] Confirm the latest completed Quest is summarized when one exists.
- [ ] Confirm no timer starts.
- [ ] Confirm no Session is created.
- [ ] Confirm no reward is granted.
- [ ] Confirm no Contract status changes.

---

# D. Completed Archive Loading

- [ ] Click Refresh.
- [ ] Confirm all readable daily Chronicle files load.
- [ ] Confirm the archive summary shows Quest count.
- [ ] Confirm aggregate focused time is plausible.
- [ ] Confirm aggregate coin and XP are plausible.
- [ ] Confirm notes/commit count is plausible.
- [ ] Confirm media count is plausible.
- [ ] Confirm battle count is plausible.
- [ ] Confirm newest completed Quest appears first.
- [ ] Confirm Visible Results limits the rendered card count.
- [ ] Confirm Expand Visible expands only visible results.
- [ ] Confirm Collapse All closes all cards.

---

# E. Archive Search

Use a known completed Quest.

- [ ] Search by Task Name.
- [ ] Search by Project Name.
- [ ] Search by Department.
- [ ] Search by Objective text.
- [ ] Search by Quest Profile name.
- [ ] Search by Contract title.
- [ ] Search by Quest Run ID.
- [ ] Search by Developer name.
- [ ] Search by Closing Notes.
- [ ] Search by Quest Log note text.
- [ ] Search by commit hash.
- [ ] Search by attachment display name.
- [ ] Search by Encounter name.
- [ ] Search by tactical seed.
- [ ] Confirm a nonsense search returns no cards with clear guidance.
- [ ] Clear the search and confirm all records return.

---

# F. Archive Filters

- [ ] Select All Completed Quests.
- [ ] Select Contract Runs.
- [ ] Confirm non-Contract Sessions are excluded.
- [ ] Select With Rewards.
- [ ] Confirm Sessions without rewards are excluded.
- [ ] Select With Commits or Notes.
- [ ] Confirm Sessions without notes are excluded.
- [ ] Select With Media.
- [ ] Confirm Sessions without attachments are excluded.
- [ ] Select With Combat.
- [ ] Confirm Sessions without Battle Results are excluded.
- [ ] Confirm changing filters never edits the underlying Session.

---

# G. Completed Quest Card

Expand a completed Quest.

- [ ] Confirm date and Task title.
- [ ] Confirm Project and Department.
- [ ] Confirm focused duration.
- [ ] Confirm reward summary.
- [ ] Confirm Chronicle integrity status.
- [ ] Confirm Developer.
- [ ] Confirm Started time.
- [ ] Confirm Completed time.
- [ ] Confirm source Contract title when applicable.
- [ ] Confirm Quest Run ID when applicable.
- [ ] Confirm Quest Story.
- [ ] Confirm Task Objective.
- [ ] Confirm Deliverables.
- [ ] Confirm Closing Notes.
- [ ] Confirm Reward Journal.
- [ ] Confirm Quest Log notes and commit hashes.
- [ ] Confirm Tactical Reports.
- [ ] Confirm Chronicle Timeline.

---

# H. Timecard Navigation

- [ ] Click Open Timecard.
- [ ] Confirm the correct generated Markdown file opens.
- [ ] Return to Unity.
- [ ] Click Reveal.
- [ ] Confirm the correct file is selected in the operating-system file browser.
- [ ] Temporarily move or rename a Timecard.
- [ ] Refresh the archive.
- [ ] Confirm Release Readiness reports the missing file.
- [ ] Restore the file.
- [ ] Confirm the advisory clears.

---

# I. Contract and Run Navigation

For a completed Contract run:

- [ ] Click Copy Run ID.
- [ ] Paste it into a text editor.
- [ ] Confirm it matches the Timecard.
- [ ] Click Select Contract.
- [ ] Confirm the correct Quest Contract asset is selected.
- [ ] Confirm the Contract is pinged in Project.
- [ ] Confirm selecting the Contract does not change its status.
- [ ] Confirm selecting the Contract does not add a Completion History record.

---

# J. Summary Copy

- [ ] Click Copy Summary.
- [ ] Paste the result into a text editor.
- [ ] Confirm Task.
- [ ] Confirm Project.
- [ ] Confirm status.
- [ ] Confirm focused and paused time.
- [ ] Confirm Contract and Run ID when present.
- [ ] Confirm Objective and Story.
- [ ] Confirm reward summary.
- [ ] Confirm notes, media, and battle counts.
- [ ] Confirm Closing Notes when present.

---

# K. Media Navigation

Expand a Quest with media.

- [ ] Confirm each attachment name appears.
- [ ] Click Open.
- [ ] Confirm the correct file opens.
- [ ] Click Reveal.
- [ ] Confirm the correct file is revealed.
- [ ] Click Copy Path.
- [ ] Confirm the recorded path is copied.
- [ ] Temporarily move one attachment.
- [ ] Confirm the card shows a missing-file warning.
- [ ] Confirm the attachment metadata remains in the Chronicle.
- [ ] Run Release Readiness and confirm the missing attachment advisory.
- [ ] Restore the file and rerun readiness.

---

# L. Correction Navigation

- [ ] Expand a completed Quest.
- [ ] Click Request Correction.
- [ ] Confirm DeverQuest switches to Rewards & History.
- [ ] Confirm the correct Session is selected for correction.
- [ ] Enter a test reason and corrected value.
- [ ] Cancel without submitting, or submit only in a dedicated QA Chronicle.
- [ ] Confirm Chronicle correction authority rules remain unchanged.

---

# M. Live Quest Chronicle

Start a tiny Quest.

- [ ] Confirm Current Quest shows Recent Quest Events.
- [ ] Confirm Open Chronicle is visible.
- [ ] Click Open Chronicle.
- [ ] Confirm the same active Quest appears.
- [ ] Confirm state.
- [ ] Confirm focused and paused time.
- [ ] Confirm Run ID when present.
- [ ] Confirm Story.
- [ ] Confirm Task Objective.
- [ ] Confirm Current Encounter.
- [ ] Confirm Open Current Quest returns to Current Quest.
- [ ] Confirm Open Quest Log & Git routes correctly.
- [ ] Confirm Copy Live Summary works.

---

# N. Live Event Timeline

During the tiny Quest:

- [ ] Confirm Quest Started appears.
- [ ] Add a Quest Log note.
- [ ] Confirm it appears.
- [ ] Link a note to a commit.
- [ ] Confirm the commit hash appears.
- [ ] Attach an existing media file.
- [ ] Confirm the attachment event appears.
- [ ] Trigger a Wellness acknowledgement or break.
- [ ] Confirm the Wellness event appears.
- [ ] Trigger configured external activity when supported.
- [ ] Confirm the External Craft event appears.
- [ ] Complete one Encounter.
- [ ] Confirm Encounter Completed appears.
- [ ] Resolve a Tactical Encounter.
- [ ] Confirm Combat outcome appears.
- [ ] Trigger a reward transaction.
- [ ] Confirm the Reward event appears.
- [ ] Confirm newest events appear first in the live feed.
- [ ] Confirm the Current Quest compact feed shows only recent events.

---

# O. Quest Completion Transition

- [ ] Complete the tiny Quest.
- [ ] Confirm Quest Completed enters the Session timeline.
- [ ] Confirm the active Session closes normally.
- [ ] Open Chronicle.
- [ ] Click Refresh.
- [ ] Confirm the Quest appears in Completed Quest Archive.
- [ ] Confirm all live evidence moved into the completed card.
- [ ] Confirm reward amount matches the Timecard.
- [ ] Confirm no duplicate reward was created.
- [ ] Confirm no duplicate Contract completion was created.
- [ ] Confirm no duplicate Session ID was created.

---

# P. Integrity and Safety

- [ ] Open Chronicle repeatedly.
- [ ] Search repeatedly.
- [ ] Expand and collapse repeatedly.
- [ ] Open and reveal files repeatedly.
- [ ] Confirm no focused time is added by Chronicle browsing.
- [ ] Confirm no rewards are added.
- [ ] Confirm no Quest Run is created.
- [ ] Confirm no Encounter resolves.
- [ ] Confirm no Contract status changes.
- [ ] Confirm no media metadata is deleted.
- [ ] Confirm no Timecard is rewritten merely by viewing it.
- [ ] Confirm missing files fail safely.

---

# Q. Regression Links

- [ ] Rewards & History still opens.
- [ ] Compensation Preview still opens.
- [ ] Daily Timecards still open.
- [ ] History CSV export still works.
- [ ] History JSON export still works.
- [ ] Quest Run Archive still opens.
- [ ] Battle Archive still opens.
- [ ] Economy Ledger still opens.
- [ ] Current Quest still repaints while running.
- [ ] Chronicle also repaints while a Quest runs.

---

# Verdict

- [ ] **PASS** — Chronicle navigation is correct and read-only.
- [ ] **CONDITIONAL PASS** — navigation works; known missing legacy files remain documented.
- [ ] **FAIL** — browsing changes Quest state, duplicates rewards, loses evidence, or opens the wrong records.
