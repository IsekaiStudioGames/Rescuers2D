# Changelog

## [0.12.1] - 2026-07-30

### Fixed

- Git stage, commit, push, and publish operations no longer block the Unity
  Editor UI thread.
- Git command output is consumed asynchronously so full output buffers cannot
  deadlock DeverQuest.
- The 30-second Git timeout now runs before output collection can block.
- Hidden terminal and credential prompts are disabled for DeverQuest Git
  commands; authentication failures return to the panel as errors.

### Changed

- Git action buttons remain disabled while an operation is running.
- The Git panel displays staging, committing, publishing, and pushing progress.
- A timed-out command recommends completing the operation in GitHub Desktop
  and refreshing DeverQuest.

## [0.12.0] - 2026-07-30

### Added

- Persistent Adventurer Character Sheet with character name, guild, Guild Rank,
  class, level, current XP, lifetime XP, and Coin Purse.
- Warrior, Paladin, Ranger, Rogue, Cleric, Druid, Wizard, Sorcerer,
  Necromancer, Bard, Monk, and Barbarian class identities.
- Member, Project Leader, Boss, and CEO Guild Rank identities.
- Copper, silver, gold, and platinum display using the configured 100:1
  denomination ladder.
- Configurable coin and XP awards for completed work blocks and Daily Decrees.
- Automatic character level-ups with increasing XP requirements.
- Projected coin and XP in the guided Quest Turn-In.
- Coin, XP, character identity, and level-up events in generated ledgers.
- Coin and XP statistics in History and Reporting.
- Manual approved-coin spending as a bridge to the future Guild Shop.

### Migration

- Remaining legacy reward-minute balances convert once at one minute to one
  copper.
- Existing reward transactions and old ledgers remain readable and unchanged.
- New progression data is stored separately from the existing developer
  settings and session records.

## [0.11.4] - 2026-07-30

### Added

- Upstream branch plus ahead/behind commit counts.
- Confirmed Push Commits action for clean repositories with local commits.
- Confirmed Publish Branch to origin action when no upstream exists.
- Git Push provenance entries in the Quest Log and Ledger.

### Safety

- Push is never automatic and never uses force.
- Push is disabled while the working tree has pending changes.
- Push is disabled when the known upstream is ahead.
- DeverQuest does not pull or attempt to resolve remote conflicts.
- Every push confirmation names the exact destination branch.

## [0.11.3] - 2026-07-30

### Added

- Explicit Quest Log Note, Git Commit, Linked Commit Note, and Legacy Entry
  provenance.
- Link Note to Current Commit action for intentionally associating a note with
  the current HEAD commit.
- Entry-type labels in the active Quest Log, Turn-In review, and generated
  Quest Ledger.

### Fixed

- Manual Quest Log notes no longer inherit the current HEAD hash or appear to
  have created a Git commit.
- Real DeverQuest and externally detected commits are consistently labeled as
  Git Commit entries.

### Compatibility

- Existing journal entries are retained and labeled Legacy Entry because their
  original intent cannot be determined safely.

## [0.11.2] - 2026-07-30

### Added

- Six-step guided Quest Turn-In:
  Review Quest, Review Git, Review Quest Log, Closing Notes, Rewards Preview,
  and Confirm Turn-In.
- Back and Next navigation without closing the active quest.
- Pending Git-change warning with the option to commit or continue.
- Final confirmation before rewards are processed and the ledger is written.
- Saved manual Git repository-folder override.
- Repository chooser available when automatic project detection fails.

### Changed

- Complete Quest now opens the Turn-In wizard instead of immediately presenting
  the final ledger-write action.
- Return to Quest safely cancels turn-in and resumes work when appropriate.

## [0.11.1] - 2026-07-30

### Fixed

- The Git panel now updates after a repository is initialized while DeverQuest
  is already open.
- Manual Refresh is no longer overwritten by an older monitored status.
- The first commit created while initializing a repository during an active
  quest is recorded instead of being silently treated as an old baseline.
- Compact View now provides the required idle-return acknowledgment and can no
  longer become stuck in meditation.

### Added

- The resolved repository root is displayed for easy verification.
- Git guidance now explicitly distinguishes staging from stashing.

## [0.11.0] - 2026-07-30

### Added

- Git installation and Unity-project repository detection.
- Current repository branch, HEAD hash, and staged, modified, and untracked
  file counts.
- Automatic branch and hash values for Quest Log notes.
- Active-quest monitoring for commits created in external Git tools.
- Commit Staged Changes action using the Quest Log message.
- Separately confirmed Stage All and Commit action.
- Plain-language explanations of branches, staging, commits, and hashes.
- Git errors displayed without losing the developer's pending message.

### Changed

- Make Camp is now Meditate.
- Camped Time is now Meditation Time.
- Manual notes are explicitly labeled as not creating Git commits.

### Safety

- DeverQuest never stages files through the staged-commit action.
- Stage All always requires a confirmation describing its full scope.
- Git commands run only against the repository containing the Unity project.
- Successful Git commits are recorded with their real branch and hash.

## [0.10.1] - 2026-07-30

### Fixed

- Complete Quest now opens the closing-notes panel even when the quest was
  previously paused by idle or project-focus detection.
- Finalization takes priority over the forced return-acknowledgment gate.
- Beginning finalization safely clears a pending idle acknowledgment without
  resuming focused time.

## [0.10.0] - 2026-07-30

### Added

- Forced return acknowledgment after idle and project-focus pauses.
- Unity-project-focused and system-wide input activity modes.
- Last-used Project and Department defaults.
- Optional locked project name for project-contained installations.
- Comma-separated focus check-in schedules such as 15, 30, 45, 60.
- Initial medieval quest terminology throughout the active workflow.

### Fixed

- Focus, paused, and idle timers now have dedicated non-overlapping rows.
- External input no longer keeps a quest active in project-focused mode.
- Opening a ledger or leaving Unity no longer falsely advances a playlist.
- Track completion requires elapsed clip duration and a confirmed stop.

### Changed

- Newest quests appear first in generated daily ledgers.
- Session actions are presented as Accept Quest, Make Camp, Resume Quest,
  Complete Quest, and Abandon Quest.

## [0.9.0] - 2026-07-30

### Added

- Daily-goal progress including the current active session.
- Current streak, longest streak, and total goal-day statistics.
- Compact session dashboard with timer, controls, music, wellness, and wallet.
- System, Dark, Light, and Echo Neon visual accents.
- Preferences for editor notifications, sounds, and reminder window auto-open.
- Complete user guide and Milestone 9 validation checklist.

### Changed

- User-facing Category terminology is now Department.
- Existing serialized `category` fields remain intact for compatibility.
- New CSV exports and generated Markdown timecards use Department labels.
- Reminder delivery respects the user's notification preferences.

## [0.8.0] - 2026-07-30

### Added

- Daily history browser backed by timecard sidecars.
- All-time, today, 7-day, 30-day, and custom date ranges.
- Project and category text filters.
- Overall focused, paused, session, commit, break, and reward totals.
- Weekly, project, and category summaries.
- Reward-wallet balance, earned, and spent statistics.
- CSV and JSON exports for the current filtered view.
- Open and reveal controls for individual timecards.

## [0.7.0] - 2026-07-30

### Added

- Reusable DeverQuest playlist assets.
- AudioClip track lists.
- Play, pause, resume, stop, previous, and next controls.
- Shuffle and Off/All/One repeat modes.
- Automatic track advancement.
- Preview-volume control when supported by the installed Unity editor.
- Selected-playlist persistence.
- Optional session start, pause, resume, end, and discard integration.

## [0.6.0] - 2026-07-30

### Added

- Persistent reward wallet.
- Configurable focused-work reward blocks.
- Default Game Time and Other Fun Time rewards.
- Custom reward categories.
- Reward spending with balance validation.
- Unfinished work carries toward the next reward block.
- Configurable daily focused-work goal and category bonuses.
- Idempotent session processing to prevent duplicate awards.
- Reward transactions in session timecards.

## [0.5.0] - 2026-07-30

### Added

- Recurring focus check-ins, movement breaks, hydration, and exercise prompts.
- Lunch and dinner reminders.
- Quiet-hours warning.
- Configurable reminder intervals and meal times.
- Snooze, dismiss, and pause-for-break actions.
- Persistent reminder scheduling through script recompilation.
- Wellness-event records in session timecards.

## [0.4.0] - 2026-07-30

### Added

- Timestamped in-session commit journal.
- Optional branch and commit-hash fields.
- Closing notes during session finalization.
- Automatic daily Markdown timecards.
- Multiple completed sessions per daily timecard.
- Recalculated focused, paused, session, and commit totals.
- JSON sidecar data used to regenerate reliable daily reports.
- Timecard write-status feedback and manual retry.

## [0.3.0] - 2026-07-30

### Added

- Windows keyboard-and-mouse idle detection.
- Configurable idle threshold and warning countdown.
- Automatic session pause when the threshold is reached.
- Visible idle warning and automatic-pause notifications.
- Pause-reason tracking.
- Configurable exceptions for Play Mode, compilation, asset importing, and
  player builds.

## [0.2.0] - 2026-07-30

### Added

- Deliberate focus-session creation.
- Project, task, category, and goal fields.
- Start, pause, resume, end, and discard controls.
- Live focused-time and paused-time display.
- Active-session persistence through editor recompilation.
- Automatic pause when Unity closes normally.
- Last-session summary retained until the next session begins.

## [0.1.0] - 2026-07-30

### Added

- Initial editor-only package.
- Dockable DeverQuest window.
- First-time profile setup.
- Timecard root and developer-folder validation.
- Per-user persistent settings.
