# DeverQuest Developer Companion

DeverQuest is an editor-only Unity productivity utility for deliberate focus
sessions, developer timecards, break reminders, rewards, and music playlists.

Version 0.12 adds a persistent Adventurer Character Sheet, experience,
leveling, and a copper/silver/gold/platinum Coin Purse. Real focused work now
advances a tabletop-inspired character instead of awarding reward minutes.

This package currently contains:

- **Milestone 1 — Package and Profile Foundation**
- **Milestone 2 — Deliberate Focus Sessions**
- **Milestone 3 — Idle Detection**
- **Milestone 4 — Commit Journal and Timecards**
- **Milestone 5 — Break and Wellness System**
- **Milestone 6 — Reward Economy**
- **Milestone 7 — Playlist Player**
- **Milestone 8 — History and Reporting**
- **Milestone 9 — Goals, Streaks, and Polish**
- **Milestone 10 — Stability and Quality of Life**
- **Milestone 11 — Git Integration**

## Requirements

- Unity 2022.3 LTS or newer

## Installation

1. Copy `com.echodevgames.deverquest` into the project's `Packages` folder.
2. Return to Unity and allow the package to compile.
3. Open **Tools > DeverQuest > Developer Companion**.
4. Complete the first-time setup.

You can also install it through Unity's Package Manager by choosing
**Add package from disk...** and selecting this package's `package.json`.

## Current features

- Reusable editor-only Unity package
- Dockable DeverQuest window
- First-time developer profile
- Project-aware default timecard folder
- Folder existence validation
- Confirmation before creating folders
- Per-user timecard subfolder
- Persistent editor settings
- Reset and reconfigure controls
- Named deliberate focus sessions
- Project, task, department, and goal details
- Start, pause, resume, end, and discard controls
- Live focused and paused timers
- Session recovery through Unity script compilation
- Automatic pause during a normal Unity shutdown
- Last-completed-session summary
- Configurable Windows idle detection
- Warning before automatic idle pause
- Play Mode, compilation, importing, and build exceptions
- Recorded pause reasons
- Timestamped commit journal with optional branch and hash
- Closing notes at session finalization
- Automatic daily Markdown timecards
- Multiple sessions and recalculated daily totals
- Timecard write retry
- Focus check-ins, movement breaks, hydration, and exercise prompts
- Lunch, dinner, and quiet-hours reminders
- Snooze, dismiss, and pause-for-break actions
- Wellness events recorded in timecards
- Persistent Game Time, Other Fun, and custom reward balances
- Configurable work blocks and category reward rates
- Spending controls and daily-goal bonuses
- Persistent character class, Guild Rank, XP, level, and lifetime progression
- Coin rewards displayed as copper, silver, gold, and platinum at 100:1
- Configurable work-block and Daily Decree coin/XP awards
- Duplicate-award protection
- Reusable AudioClip playlist assets
- Playback, navigation, shuffle, repeat, and volume controls
- Session-aware music behavior
- Filterable daily history and weekly summaries
- Project, department, and wallet statistics
- CSV and JSON reporting exports
- Live daily-goal progress from completed and active work
- Current and longest focused-work streaks
- Compact active-session dashboard
- System, dark, light, and Echo Neon accents
- Editor notification, sound, and auto-open preferences
- Non-destructive Category-to-Department display migration
- Forced acknowledgment after automatic idle pauses
- Unity-project-focused or system-wide activity scope
- Last-used Project and Department defaults
- Optional locked project name
- Custom focus check-in schedules
- Newest-first daily ledger entries
- Focus-safe playlist end detection
- Quest-themed session terminology
- Git executable and repository detection
- Current branch, HEAD hash, and working-tree totals
- Automatic branch and hash fields in the Quest Log
- External Git commit observation during active quests
- Guarded staged-change commits
- Separately confirmed Stage All and Commit action
- Beginner-focused Git vocabulary guidance
- Meditate and Meditation Time terminology
- Six-step guided Quest Turn-In wizard
- Pending-change review before ledger writing
- Manual repository-folder selection
- Explicit note/commit provenance
- Optional notes linked to the current Git commit
- Guarded push and branch publishing
- Upstream ahead/behind status

Settings are stored in Unity's per-user `EditorPrefs`, so they are not committed
to a project's repository. Timecards default to a `DeverQuestTimecards` folder
beside the project's `Assets` folder.

## Planned milestones

1. Package and Profile Foundation
2. Deliberate Focus Sessions — complete
3. Idle Detection — complete
4. Commit Journal and Timecards — complete
5. Break and Wellness System — complete
6. Reward Economy — complete
7. Playlist Player — complete
8. History and Reporting — complete
9. Goals, Streaks, and Polish — complete
10. Stability and Quality of Life — complete
11. Git Integration — complete

See `Documentation~/DeverQuest_User_Guide.md` for the complete user guide,
data locations, compatibility notes, and troubleshooting.
