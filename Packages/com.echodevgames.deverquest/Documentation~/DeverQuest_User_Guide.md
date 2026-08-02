# DeverQuest User Guide

DeverQuest is an editor-only Unity companion for deliberate work sessions,
timecards, wellness, earned rewards, music, goals, and reporting.

## Install or update

Use Unity 2022.3 LTS or newer. Copy `com.echodevgames.deverquest` into the
project's `Packages` folder, or use Package Manager > Add package from disk and
select `package.json`. To update, replace the package folder while Unity is
closed or use your normal package-source workflow. DeverQuest profile settings
and timecards live outside the package, so updating does not erase them.

Open Tools > DeverQuest > Developer Companion.

## First-time setup

Enter a developer name and choose a timecard root. DeverQuest asks before
creating the root or developer subfolder. Configure session defaults, idle
detection, wellness reminders, reward rules, session-aware music, theme, and
notification preferences, then validate the folders.

## Sessions and idle detection

Enter a project, task or milestone, department, and optional objective, then
accept a quest. Make camp and resume manually, or let the optional idle
detector pause after the configured timeout. An automatic idle pause requires
an explicit return acknowledgment before the quest can resume.

Unity Project Focused activity scope pauses after the current Unity process
loses focus, so work in another application or project does not keep the quest
active. System Wide Input preserves the original honor-system behavior and
counts keyboard or mouse input anywhere on the computer. Windows provides
native keyboard and mouse idle time. Other platforms continue working normally
but may report that native idle detection is unavailable.

Play Mode, compilation, asset importing, and player builds can count as active
work so unattended Unity operations do not pause a valid session.

## Commits and timecards

During a session, add journal entries with a comment and optional branch and
hash. Ending a session pauses the clock while you add closing notes. Finalizing
writes or updates that day's Markdown timecard and JSON sidecar in:

`<Timecard Root>/<Developer Name>/`

The sidecar is DeverQuest's reporting data. Keep it beside the Markdown file.
Multiple sessions on the same day are combined into one daily card.
The newest quest is printed first.

## Git integration

When Git is installed and the Unity project is inside a repository, the Quest
Log displays the current branch, HEAD commit hash, and counts for staged,
modified, and untracked files. It also displays the resolved repository root.
If a repository is created while DeverQuest is open, wait about five seconds
or press Refresh.

- Add Quest Log Note records a productivity note only. It does not change Git.
- Link Note to Current Commit records a note associated with the current HEAD
  hash, but does not create another Git commit.
- Commit Staged Changes commits only files already staged through another Git
  tool. The Quest Log comment becomes the Git commit message.
- Stage All and Commit stages every modified, deleted, and untracked file,
  then commits. It always requires confirmation.
- Push Commits sends committed work to the configured upstream only when the
  working tree is clean and the known remote branch is not ahead.
- Publish Branch to origin creates an upstream for a clean local branch after
  a separate confirmation.

After a successful commit, DeverQuest records the real branch and resulting
hash in the active quest. It also checks for commits made through an external
Git client during an active quest and records newly detected HEAD commits.

Ledger entries are labeled Quest Log Note, Git Commit, Linked Commit Note, or
Git Push. Legacy Entry means the entry predates provenance tracking; DeverQuest
preserves it without guessing whether it represented a real commit.

DeverQuest never pushes automatically and never force-pushes. Ahead/behind
counts use the remote-tracking information already known locally. If the remote
has changed since the last fetch, Git can still reject a push safely. Use
GitHub Desktop to fetch, pull, review conflicts, and synchronize before trying
again. A successful push is recorded as Git Push using the HEAD hash that was
sent.

A branch is the current development path. Staging selects changes for the next
commit. A commit is a saved repository snapshot. Its hash uniquely identifies
that snapshot. Staging is different from stashing: a stash temporarily shelves
uncommitted work for later. DeverQuest does not push, pull, merge, switch
branches, discard
changes, or rewrite history.

If automatic detection reports the wrong repository, choose Repository Folder
and select the folder shown as the repository in GitHub Desktop. DeverQuest
saves the override per user. Use Unity Project to clear the override.

## Quest Turn-In

Complete Quest pauses focused time and opens a six-step review:

1. Review Quest details and accumulated time.
2. Review Git status and optionally commit pending changes.
3. Review Quest Log notes and real Git-backed entries.
4. Write closing notes.
5. Preview projected work-block rewards.
6. Confirm Turn-In and write the Quest Ledger.

Back and Next do not close the quest. Return to Quest cancels the turn-in and
resumes focused time when the quest was running beforehand. Pending Git changes
produce a warning but do not force a commit. Only Turn In Quest and Write
Ledger completes the session.

Older data uses the serialized field name `category`. Milestone 9 displays it
as Department without rewriting or risking existing records.

## Wellness and notifications

Focus, hydration, movement, exercise, meal, and quiet-hours prompts can be
snoozed, acknowledged, or used to pause for a break. Preferences control
whether DeverQuest shows editor notifications, plays sounds, or opens a closed
window for reminders. Disabling auto-open never disables the underlying timer.
Focus check-ins accept a comma-separated schedule such as `15, 30, 45, 60`.
Leave the schedule empty to use the repeating fallback interval.

## Rewards

Focused work blocks earn universal coin and experience. Coin is stored as
copper and displayed using 100 copper per silver, 100 silver per gold, and 100
gold per platinum. Daily Decrees can award additional coin and XP. Session
processing is protected against duplicate awards.

## Adventurer Character Sheet

Configure an Adventurer name, Guild, class, and Guild Rank in setup. Classes
are role-playing identities; Guild Ranks describe workplace authority and are
kept separate. Completing Quests earns XP, automatically advances levels, and
records level-up events in the daily ledger.

Remaining legacy reward balances migrate once at one reward minute per copper.
Historical reward transactions remain visible as legacy minutes.

## Playlists

Create a DeverQuest playlist asset from the player, add AudioClips in its
Inspector, and select it in the window. Controls include play/pause, previous,
next, stop, shuffle, repeat, and volume where the Unity editor exposes preview
audio volume. Session-aware options can play, pause, resume, or stop music with
the work session.

## Goals, streaks, and compact mode

The daily progress bar combines completed timecards with today's active
session. A goal day is completed when its finalized focused time reaches the
configured daily target. The current streak tolerates an unfinished current
day by counting backward from yesterday; completing today extends it.

Compact View keeps the live goal, reminders, music, timer, meditate/resume/end
controls, and reward balances visible in a smaller workspace. Use Full View to
start a new session or access journals, reports, and settings.

## History and exports

History reads JSON sidecars from the developer folder. Filter by date, project,
or department; review daily, weekly, project, department, and reward summaries;
then export the current view as CSV or JSON.

## Themes

System follows Unity's normal label colors. Dark and Light provide explicit
readable accents. Echo Neon uses DeverQuest teal and pink accents. Themes tint
DeverQuest headings and timers; they do not replace the Unity editor skin.

## Backup and privacy

Back up the entire developer timecard folder to preserve Markdown and JSON
together. Profile and wallet preferences use per-user Unity EditorPrefs and are
not committed to the project by default. DeverQuest runs locally and does not
upload timecards, activity, playlists, or personal data.

## Troubleshooting

- No timecard: finalize the session, then inspect the status shown under the
  last completed session. Use Retry Timecard Write if needed.
- Empty history: verify the configured developer folder contains
  `.deverquest.json` files and press Refresh.
- Idle detector unavailable: use manual pause or disable idle detection; the
  rest of DeverQuest remains supported.
- Audio does not play: ensure the playlist contains AudioClips. Unity preview
  audio APIs vary by editor release; DeverQuest shows a warning when playback,
  status, or volume control is unavailable.
- Settings look wrong after an update: choose Reconfigure Profile, review the
  values, and finish setup. This does not delete timecards.
- Work in another application paused the quest: choose System Wide Input if
  that external activity should count. Unity Project Focused is intentionally
  strict.

## Compatibility

The package targets Unity 2022.3 LTS and is editor-only. Timecard data remains
backward-compatible across Milestones 4–9. Runtime builds do not include the
editor window or productivity monitors; the playlist asset type is kept in the
runtime assembly so assets remain valid.
