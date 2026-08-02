# DeverQuest Developer Companion

Current package version: **0.32.5 Beta 1**



## 0.32.5 Distribution Compilation Hotfix

Version 0.32.5 fully qualifies `System.IO.Compression.CompressionLevel` in the Packaging and Distribution tarball exporter. This removes a Unity 6 compiler ambiguity with `UnityEngine.CompressionLevel` discovered during a clean import into another project. No Quest, Chronicle, reward, account, audio, or content behavior changed.


## 0.32.4 Beta Packaging and Distribution Center

Version 0.32.4 adds a Packaging and Distribution workspace for package manifests, SHA-256 checksums, tarball creation and verification, media and license audits, version consistency, known limitations, and exportable release dossiers. Internal Beta, External Beta, and Release Candidate channels apply progressively stricter release gates.


## 0.32.3 Companion Profile Asset Association Hotfix

Version 0.32.3 moves `DeverQuestCompanionProfile` into its own matching runtime source file with a stable package GUID. Existing Companion Profile assets whose script association is missing are repaired automatically after installation, with backups written under `Library/DeverQuest/Migrations/0.32.3`. Repaired Original Starter profiles are reconnected to the Original Guild Companion Catalog. A manual repair command is available at **Tools > DeverQuest > QA > Repair Companion Profile Asset Scripts**.

## 0.32.2 Monster Profile Asset Association Hotfix

Version 0.32.2 moves `DeverQuestMonsterProfile` into its own matching runtime source file with a stable package GUID. Existing generated Monster Profile assets whose script association is missing are repaired automatically after installation, with backups written under `Library/DeverQuest/Migrations/0.32.2`. A manual repair command remains available at **Tools > DeverQuest > QA > Repair Monster Profile Asset Scripts**.


## 0.32.1 Board Administration and Layout Stabilization

Version 0.32.1 keeps completed Contracts visible to authorized Guild leadership until they are explicitly archived. Leadership may restore a completed one-time or limited Contract to Offered without deleting its prior Completion History; DeverQuest opens one additional completion slot and preserves every earlier reward and Chronicle record. Long Quest Log, Git commit, Final Quest Log, and Closing Notes fields now wrap within the Editor window instead of expanding the entire workspace horizontally. Beta Administration also shows every asset in a duplicate-ID group and can preserve one selected ID while regenerating the other copies.

## 0.31.8 Beta Administration and Content Validation

Version 0.31.8 adds a dedicated **Beta Administration** workspace for bulk production-content validation, safe data repair, starter-generator reruns, and exportable Markdown or JSON health reports. The validator scans Quest Profiles, Contracts, Identity assets and Catalogs, Companions, Encounters, Monsters, Shops, items, audio profiles, and Starter Loadouts for duplicate stable IDs, incomplete records, broken references, unsafe Quest-item rules, empty catalogs, invalid run history, and refreshable reward mismatches. Release Readiness now summarizes the current content-health result.

## 0.31.7 Notifications and Wellness Command Center

Version 0.31.7 consolidates active reminders, snoozes, queued prompts, quiet hours, break qualification, cue testing, and local notification history inside Audio & Wellness. Reminders persist through Editor restarts, multiple prompts queue instead of silently disappearing, session reminders may be suppressed during configured quiet hours, and the dockable Quest HUD can display and act on wellness state. Completed and early-ended Approved Breaks enter both the Session Wellness Journal and the local command history. Release Readiness verifies that the local history store is writable.


## 0.31.6 Supported Audio Host and Mixer Reliability

Version 0.31.6 replaces the preferred DeverQuest playback path with a hidden Editor-only AudioSource host containing separate Music, Ambience, and warning/SFX sources. This provides independent channel gain, mute controls, cue ducking, Inspector-preview isolation, focus and Play Mode recovery, and audio-device-change recovery. The previous internal preview bridge remains as a clearly identified compatibility fallback when supported Edit Mode AudioSource playback is unavailable. Mixer and transport preferences are local Editor settings and do not enter Guild, Quest, Chronicle, or shared data.

## 0.31.5 Editor UX and Workspace Organization

Version 0.31.5 separates **Quest Log** from **Git**, adds a normal dockable **Quest HUD** window, reorganizes workspace navigation, improves no-data guidance, and introduces persistent local Visual settings for theme, custom colors, DeverQuest text scale, workspace columns, compact tab labels, header guidance, and HUD behavior. The Quest HUD uses the same active Session and timer services as the main DeverQuest window; it does not create a second Quest or duplicate focused time. Git commit messages are now independent from Quest Log notes, while commits and pushes made during an active Quest continue to enter that Quest's evidence log.

## 0.31.4 Quest Archive and Chronicle Navigation

Version 0.31.4 adds a dedicated **Chronicle** workspace that combines the active Quest event feed with a searchable completed Quest archive. Live work now presents Quest Story, Task Objective, current Encounter, timer state, recent notes, rewards, wellness events, media, external activity, and tactical outcomes in one timeline. Completed Quest cards expose rewards, closing notes, Git/Quest entries, attachments, combat reports, source Contract navigation, Run IDs, generated Timecards, and correction-request routing. Release Readiness audits duplicate Session IDs, missing Timecards, and missing media paths. The workspace is a read/navigation layer over existing Chronicle data and cannot award rewards or create focused time.

## 0.31.3 Guild Economy and Item Operations

Version 0.31.3 adds a dedicated **Economy** workspace for active-Quartermaster configuration, coin denomination clarity, audited leadership item and coin grants, and searchable transaction history with CSV export. Purchases and Inventory sales now respect the active Shop Profile's open, member-access, purchase, sale, and approval-threshold rules. Economy events retain recipient, item, quantity, copper delta, resulting balance, actor, note, and related purchase IDs. This strengthens the current merchant loop without opening deferred banking, loans, auction-house, crafting-market, or housing-storage systems.

## 0.31.2 Inventory and Equipment Clarity

Version 0.31.2 adds a dedicated **Inventory** workspace with durable item categories, tags, lore, equipment comparison, exact ownership records, loot provenance, carry-load breakdowns, and guarded Equip, Unequip, Use, Sell, and Drop actions. Encounter drops, Guild Shop purchases, trades, starter gear, and legacy migrations now retain clearer origin data. Release Readiness audits duplicate ownership IDs, unresolved equipment, unsafe Quest items, and equipped gear missing pack records. This release strengthens the existing item loop without opening the deferred crafting, banking, housing, or broad skill systems.

## 0.31.1 Tactical Operations

Version 0.31.1 adds a dedicated **Tactics** workspace for combat readiness, Companion operations, current Encounter inspection, and a searchable local Battle Archive. New Battle Results are archived automatically, while current and last-session reports can be imported without duplication. The archive stores the newest 100 records locally, supports outcome and text filters, copies readable reports or JSON, and keeps Timecards as the permanent Chronicle. Companion operations now include quick activation, stable dismissal, individual recovery, and confirmed full-roster recovery. Release Readiness verifies that the Tactical Archive can safely write local diagnostic data.

## Previous Beta work

## 0.31.0 Tactical Visibility

Version 0.31.0 makes the existing deterministic Companion, Combat, and Survival systems readable without changing their underlying rules. Active Quests now show Tactical Encounter previews and compact Tactical Field Reports containing outcome, typed damage, conditions, Companion contribution, defeated foes, loot, recent turns, and copyable full logs. Survival stages show the next wave, difficulty tier, Guild Wagon timing, carry state, and safe-exit result. Companion Stable cards now retain lifetime contribution metrics and a last-battle summary. Generated Timecards keep combat readable by showing highlights first and placing the full turn transcript inside a collapsible details block.





## 0.30.9 Quest Board and Run Management

Version 0.30.9 adds a Guild Hall management panel for active Quest Run reservations and waiting Parties, leadership cancellation controls for stale reservations, explicit Contract archiving, a searchable completed Quest Run archive in Rewards & History, and a Release Readiness advisory for invalid or older-than-24-hour reservations. The full 0.30.8 behavior checklist remains deferred for later regression.


## 0.30.8 Beta Loop Stabilization

Version 0.30.8 responds to the first sustained one-hour Beta expedition. It
adds explicit Music and Ambience track selectors, automatic and manual recovery
when Unity's Inspector preview player steals the shared editor-audio transport,
and an emergency full-audio reset. True independent channel gain remains limited
by Unity's internal preview API when only global preview volume is available.

The only active Guild account is now repaired and preserved as CEO, Guild rank
is no longer overwritten by stale character-sheet data, existing founders can
reopen character creation, and newly completed characters begin with five
silver. Quest acceptance now explains why it is blocked, Party Quests show a
waiting state and allow withdrawal before launch, Quest Story appears during
selection and active work, Focus Stages are presented as Encounters, and break
reminders show both recommended and minimum qualifying durations.

Automatic Git observation now runs outside Unity's main update thread. Release
Readiness also checks sole-founder authority and warns when timecards or voice
memos live inside the repository without a matching `.gitignore` entry.

See `Documentation~/DeverQuest_0.30.8_Beta_Issue_Log.md`.

## 0.30.6 Identity Catalog Registry Repair

Version 0.30.6 repairs an invalid or Missing Script Guild Identity Registry
at its canonical project path, then activates the generated starter catalog.
Guild Hall generation is deferred outside Unity's IMGUI draw event to prevent
secondary layout-state errors when asset creation reports a failure.

## 0.30.5 Beta Asset Persistence and Audio Channels

Version 0.30.5 adds stable Unity `.meta` files to the newly separated
ScriptableObject source files introduced during the 0.30.4 asset correction.
This stabilizes new Ambience and starter-catalog assets without forcing new
GUIDs onto every established DeverQuest script during the Beta test. Assets
already showing **Missing Script** must be recreated once after 0.30.5 is
installed.

Music and Ambience now use separate logical channels. Their Play, Pause, Stop,
Next, and replacement actions preserve the other channel while still rebuilding
Unity's global preview transport to prevent abandoned clips from accumulating.
Warning cues may play over both channels.

## 0.30.4 Beta Asset Association Hotfix

Version 0.30.4 fixes the missing-script failure confirmed during Ambience
Profile testing. Unity requires independently creatable ScriptableObject types
to have importable script assets. Ambience and starter Identity types now live
in dedicated source files, and the same secondary-asset pattern was corrected
for Ability, Spell, Companion Catalog, Encounter Profile, and Shop Profile
types before it could surface later in Beta.

Assets created while 0.30.3 showed **Missing Script** cannot be repaired by
assigning them in the picker. Delete those broken assets and create them again
after installing 0.30.4. For the starter Identity Catalog, delete the partial
`Assets/DeverQuest/IdentityCatalogs/OriginalStarter` folder and rerun the
generator.

See `Documentation~/DeverQuest_0.30.4_Asset_Association_Hotfix.md`.


## 0.30.3 Beta 1 Stabilization

Version 0.30.3 begins the first formal Beta test campaign. It fixes Ambience
Profile assignment, makes Quest and Contract Spoils previews agree with the
values that will be awarded, hardens original starter Identity Catalog
generation, and adds a live progress panel to the main Quest workspace.

The progress panel shows elapsed progress, time remaining or overtime, the
current Encounter, pacing feedback, and a current Spoils estimate. Release
Readiness now also checks repository hygiene, starter Identity data, and
refreshable Contract Spoils snapshots.

See `Documentation~/DeverQuest_0.30.3_Beta_Stabilization.md` and
`Documentation~/DeverQuest_0.30.3_Beta_Test_Checklist.md`, and
`Documentation~/DeverQuest_0.30.3_Beta_Issue_Log.md`.


## 0.30.2 Namespace Compatibility Hotfix

Version 0.30.2 resolves the `PackageInfo` namespace collision reported by
Unity 2022.3 in the release-readiness service. The service now explicitly uses
`UnityEditor.PackageManager.PackageInfo`. No gameplay or productivity behavior
was changed.

See `Documentation~/DeverQuest_0.30.2_Namespace_Hotfix.md`.

## 0.30.1 Compilation Hotfix

Version 0.30.1 restores Unity 2022.3 / C# 9 compilation for the 0.30 release
candidate. It removes unsupported multiline interpolation expressions, supplies
explicit generic types to Tactical Starter Kit asset generation, removes an
invalid combat variable reference, and qualifies Unity's `Object` type where
`System.Object` was also in scope.

See `Documentation~/DeverQuest_0.30.1_Compilation_Hotfix.md`.

## Release Candidate and Scope Lock

Version 0.30.0 began the release-candidate phase by replacing unsupported
preview layering with deterministic ownership, safe warning-cue restoration,
accurate pause/completion behavior, and cleanup during assembly reload or editor
exit. Its mutually exclusive Music/Ambience rule was superseded by the
independent logical channels introduced in 0.30.5.

Run **Tools > DeverQuest > Run Release Readiness Check** before regression to
validate the package version, Unity version, profile, timecard storage,
Chronicle policy, shared Guild path, audio transport, and active Quest state.
Major new systems are frozen until after the release candidate.

See `Documentation~/DeverQuest_0.30.0_Release_Candidate.md` and
`Documentation~/DeverQuest_0.30.0_Regression_Checklist.md`.

## Tactical Abilities and Survival Quests

Version 0.29.0 adds class-linked tactical Ability Profiles, structured
Spells and Attack Techniques, cascading Focus-stage pace bonuses, encounter
par rewards, Survival expeditions, low-health safety pauses, weighted loot,
coin encumbrance, and Guild Hall denomination exchange.

Generate the original starter abilities plus the 15-minute and Survival Quest
templates from the Rules Laboratory. See
`Documentation~/DeverQuest_0.29.0_Tactical_Survival.md`.

## Compensation Preview

Version 0.28.0 added an optional, local Compensation Preview for an
authenticated Adventurer. A Boss or CEO can configure an hourly rate or an
annual-salary tracking equivalent, currency code, scheduled weekly hours,
approved-break treatment, and Chronicle-integrity policy from **Guild Hall >
Guild Accounts and Authority > Compensation Preview Policies**.

The Adventurer can then open **Rewards & History > Compensation Preview** to
see current-workweek and filtered-history eligible time and estimated gross
equivalents. Modified or unavailable Chronicles are excluded. Legacy
Chronicles are included only when the policy explicitly permits them.
Configured long/frequent Quest flags remain visible for manual review.

This is a planning estimate, not payroll, a wage statement, a promise of
payment, tax advice, or authorization to pay. It never transfers money.
Meditation and Idle/Unverified time never qualify, and active Quests are
excluded until finalized. Rates remain in local editor preferences and are not
written to daily timecards or shared Guild snapshots; that local storage is
not encrypted payroll storage.

See `Documentation~/DeverQuest_0.28.0_Compensation_Preview.md`.

## Pets, Familiars, Minions, and Companions

Version 0.27.0 adds persistent Companion rosters to each authenticated
Adventurer. Companion Profile assets define original pet, familiar, minion,
spirit, construct, or mercenary identities with Class eligibility, combat
role, creature and damage types, affinities, recruitment cost, and recovery
rules.

One active Companion joins deterministic encounters. Strikers gain damage,
Guardians intercept more attacks, Support Companions may restore HP, and
Controllers can hinder an enemy attack. Companion HP, loyalty, battles,
victories, XP, and levels persist per Guild account and appear in the
Character Sheet, compact Quest view, Battle Chronicle, shared Adventurer
snapshot, and daily timecard.

Generate the five commercially clean original starters from **Guild Hall >
Campaign Content Scaffolding > Generate Original Companion Stable** or from
the Character workspace's **Companion Stable**. New characters receive their
Class's configured starter Companion; existing characters may recruit from
the Stable. Companions affect RPG outcomes only and never create focused-work
time or productivity rewards.

See `Documentation~/DeverQuest_0.27.0_Companions.md`.

## Creature Types, Damage Types, and Resistances

Version 0.26.0 adds a commercially safe Guild Combat Codex with seventeen
creature families and fourteen damage types. Monster attacks, weapons, and
spells now carry a damage type; Monsters, Ancestries, and Equipment can grant
vulnerability, resistance, immunity, or absorption.

The deterministic encounter resolver applies those rules without stacking
duplicate defenses. Resistance halves damage, vulnerability doubles it,
immunity prevents it, absorption converts it to healing, and a vulnerability
paired with resistance cancels to normal damage. Typed damage events and raw
versus final values are preserved in the Battle Chronicle and daily timecard.

Generate the full reference asset from **Character > Rules Laboratory >
Generate Guild Combat Codex**. The tutorial campaign demonstrates an Undead
opponent, typed spell and weapon damage, a resistant ring, and multiple enemy
affinities.

See `Documentation~/DeverQuest_0.26.0_Combat_Typing.md`.

## Ancestries, Classes, Faiths, and Identity Catalogs

Version 0.25.0 replaces hard-coded character-creation choices with reusable
Ancestry, Class Definition, Faith, and Identity Catalog ScriptableObjects.
Character creation now validates playable/sapient Ancestries, Class
eligibility, Alignment, Faith restrictions, starting attributes, Department,
HP, Mana, traits, languages, and the future companion tradition hook.

Authenticated CEOs and Bosses can open **Guild Hall > Campaign Content
Scaffolding** to generate a commercially clean original starter catalog, make
blank identity assets, or add identity templates to the organized studio
structure. Existing Adventurers migrate to stable catalog IDs without losing
their names, progression, stats, coin, inventory, or Chronicle history.

The shipped starter content uses original DeverQuest names. Private Guilds can
author their own catalog assets, but they remain responsible for the rights to
any third-party names, art, audio, or lore they add.

See `Documentation~/DeverQuest_0.25.0_Identity_Catalogs.md`.

## Starter Campaign and Content Organization

Open **Guild Hall > Campaign Content Scaffolding** and choose:

- **Create Empty Studio Structure** for organized production folders and
  blank ScriptableObject templates.
- **Create Tutorial Campaign** for a complete connected walkthrough named
  **Trouble in the Tutorial Crypt**.

Both generators are safe to rerun. Existing assets are preserved, production
templates live under `Assets/DeverQuest/Templates`, and tutorial content stays
under `Assets/DeverQuest/DemoCampaign`.

See `Documentation~/DeverQuest_0.24.2_Content_Organization.md`.

## Performance and Workspace Hotfix

Version 0.24.1 splits DeverQuest's full interface into lazy workspaces. Only
the selected tab is rendered, preventing inactive Guild, Character,
AssetDatabase, history, Git, and shared-record panels from participating in
every timer repaint.

Use the in-window workspace bar or open a section directly from:

`Tools > DeverQuest > Workspaces`

The live Quest and Quest Log tabs update four times per second. Other tabs do
not request continuous repainting. Background Git commit observation uses a
lightweight HEAD check every fifteen seconds and expands to full status only
when a commit actually changes.

See `Documentation~/DeverQuest_0.24.1_Performance_Checklist.md`.

## Trading, Rare Loot, and Real Rewards

Milestone 24 gives inventory items durable ownership identities, rarity,
binding, acquisition provenance, and trade eligibility. The Trading Post
supports escrowed offers, acceptance, rejection, cancellation, reclamation,
and a permanent local ledger.

Real-world rewards such as Nitro, merchandise, gift cards, bonuses, or
custom rewards use an explicit workflow: request, leadership approval, and
manual fulfillment confirmation. DeverQuest records the result but never
pretends an external benefit was delivered automatically.

See `Documentation~/DeverQuest_0.24.0_Regression_Checklist.md`.

## Shared Guild Records and Hall of Heroes

Milestone 23 publishes finalized Quests into a configured shared Guild
repository. The Hall of Heroes compares Adventurers by healthy ranked focus,
raw focus, XP, coin, levels, streaks, Quests, and Contracts, with additional
Project and Department standings.

Long sessions, high idle ratios, excessive daily Quest counts, modified
records, and unhealthy daily totals are flagged or capped instead of being
rewarded. The source JSON remains available for leadership review.

The shared folder is tamper-evident, but its authority depends on external
permissions. Use a studio-controlled server, network share, or cloud folder
where ordinary Members cannot rewrite Guild records.

See
`Documentation~/DeverQuest_0.23.0_Regression_Checklist.md`.

## External Activity and Voice Memos

Milestone 22 adds reusable External Activity Profiles. On Windows, a configured
foreground creative tool can prevent Unity-project-focus idle pausing while
recent keyboard or pointer input continues. Generate the included Aseprite
preset or author providers for other applications by process name and optional
window-title text. External activity is Chronicle evidence; it never creates
focused seconds or rewards by itself.

The active Quest can record microphone voice memos or copy an existing media
file into the Adventurer's dated `Media` folder. Each attachment is preserved
in session data and linked from the generated Chronicle.

See
`Documentation~/DeverQuest_0.22.0_Regression_Checklist.md`
before promoting the package.

## 0.21.1 Stabilization

This checkpoint clarifies wellness behavior and protects intentional Chronicle
review. **Acknowledge Only** dismisses a reminder without claiming a break.
**Take Approved Break** pauses and classifies the configured duration, then
grants the configured XP and character-state benefit only after at least 80%
is completed. Opening or revealing a timecard grants a temporary
external-action grace period so Chronicle review does not pause the Quest or
change session-aware music.

The package includes
`Documentation~/DeverQuest_0.21.1_Regression_Checklist.md` for the recommended
Unity verification pass.

## Audio, Warning Profiles, and Ambience

DeverQuest supports custom Warning Audio Profiles for idle, wellness,
Focus Stage, combat, danger, victory, defeat, Quest completion, purchase, and
level-up cues. Ambience Profiles provide looping environmental audio alongside
playlist music. Because Unity exposes global editor-preview transport controls,
DeverQuest maintains Music and Ambience as independent logical channels and
rebuilds their expected native clips after transport changes. Warning cues play
over the active long-form channels
position. Playlist shuffle honors per-track weights.

## Encounter Profiles and Quest Battles

Focus Stages can now resolve deterministic tabletop battles against reusable
Monster and Encounter Profile assets. Encounters support waves, bosses, dice,
attacks, damage, bonus coin/XP, drop tables, injuries, defeat, optional
character death, and Guild Shrine resurrection. Every round and seed is
preserved in the Battle Chronicle. Work and Focus rewards are guaranteed and
remain separate from combat outcomes.

## Guild Shop, Inventory, and Wellness Economy

Adventurers can now spend earned coin through Shop Profile and Shop Item
assets. The Quartermaster supports equipment, spells, provisions, potions,
inn rest, approved-break permits, and controlled redemption rewards.
Purchases, approval decisions, deliveries, and redemptions remain in the Guild
ledger, while inventory and character effects persist per account.

## Parties, Quest Stages, and Character Creation

New Member accounts now complete guided Adventurer creation on first login.
Classes establish the starting Department and expanded character foundation.
Quest Contracts support solo or capacity-limited parties, reserved rosters,
eligibility rules, party roles, authored story, staged focus objectives, and
coordinated submission. Compact mode shows the active Quest, party, Stages,
core combat statistics, and wellness needs.

Dungeon Masters can generate Copper, Bronze, Iron, and Steel starter gear for
the full equipment-slot model, then create additional Equipment and Starter
Loadout assets in the Unity Editor.

## Character Sheet and Rules Engine

Adventurers now have class-based ability scores, HP, AC, saving throws,
proficiency, class features, statuses, equipment, and spells. Seeded checks
and dice expressions are deterministic so later Quest encounters can record
and reproduce their results.

## Guild Accounts and Authority

Guild leadership can create locally protected Adventurer accounts with locked
identity, class, Guild Rank, progression, and Project assignments. CEO, Boss,
Project Leader, and Member permissions are enforced at Contract, correction,
settings, and work-input boundaries. Existing Adventurer data migrates into
the founding CEO account.

## Chronicle Integrity and Review

DeverQuest can divide a workday into numbered Chronicles, seal their
structured records with a chained SHA-256 audit journal, append correction
requests without replacing original Quest records, and flag unusual activity
for human review. Existing records remain compatible and appear as Legacy.

Integrity seals are local tamper evidence, not authoritative security. A
future shared Guild service is required for independently controlled records.

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
- **Milestone 12 — Adventurer Progression**
- **Milestone 13 — Roadmap Checkpoint**
- **Milestone 14 — Quest Contracts and Assignment Board**
- **Milestone 15 — Chronicle Integrity and Review**
- **Milestone 16 — Guild Accounts and Authority**
- **Milestone 17 — Character Sheet and Rules Engine**
- **Milestone 18 — Parties, Quest Stages, and Character Creation**
- **Milestone 19 — Guild Shop, Inventory, and Wellness Economy**
- **Milestone 20 — Encounter Profiles and Quest Battles**
- **Milestone 21 — Audio, Warning Profiles, and Ambience**
- **Milestone 22 — External Activity and Voice Memos**
- **Milestone 23 — Shared Guild Records and Hall of Heroes**
- **Milestone 24 — Trading, Rare Loot, and Real Rewards**
- **Milestone 25 — Ancestries, Classes, Faiths, and Catalogs**
- **Milestone 26 — Creature Types, Damage Types, and Resistances**
- **Milestone 27 — Pets, Familiars, Minions, and Companions**

## Requirements

- Unity 2022.3 LTS or newer

## 0.30.8 reusable Quest Contracts

Quest Contracts now act as reusable Guild Board definitions, while each acceptance creates an independent Quest Run. Contracts may be configured as one-time, limited to a completion target, or repeatable. Limited Contracts may require unique Adventurers, and Party Quests may require a full roster or begin at a configured minimum size. Each completed run is retained in the Contract's Completion History and receives a unique Run ID in the generated Timecard.


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
- Administrator-authored Quest Profile assets with reusable objectives,
  eligibility, duration, and payout rules
- Member profile selection plus leadership custom-Quest permissions
- Actual Quest Contract assets with assignments, deliverables, priorities, due
  dates, lifecycle states, and a Guild Assignment Board
- Reserved Encounter hooks for future dice-driven Quest battles
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
