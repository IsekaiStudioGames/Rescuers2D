## [0.32.5] - 2026-08-02

### Fixed

- Fully qualified `System.IO.Compression.CompressionLevel.Optimal` in the distribution tarball exporter.
- Removed the compiler ambiguity with `UnityEngine.CompressionLevel` exposed by clean Unity project imports.
- Updated package, Release Readiness, content-health, notices, and distribution version declarations to 0.32.5.

## [0.32.4] - 2026-08-02

### Added

- Beta Packaging and Distribution Center workspace.
- Installed-package SHA-256 manifest and per-file checksum generation.
- Deterministic tarball export with post-export file and checksum verification.
- Internal Beta, External Beta, and Release Candidate audit channels.
- Version, metadata, bundled-media, license, credits, notices, known-limitations, Release Readiness, and content-health gates.
- Exportable release dossier, distribution checklist, JSON audit, content-health snapshot, and `SHA256SUMS.txt`.
- `KNOWN_LIMITATIONS.md`, `RELEASE_NOTES.md`, and explicit license-decision record.

### Changed

- Updated package, readiness, and Beta health-report version declarations to 0.32.4.
- Updated third-party notices to the current package version.

## [0.32.3] - 2026-08-02

### Fixed

- Moved `DeverQuestCompanionProfile` into a correctly named standalone runtime source file with stable package metadata.
- Added automatic and manual migration for Companion Profile assets created with missing script associations.
- Reconnects repaired Original Starter Companion Profiles to the Original Guild Companion Catalog through the idempotent generator.
- Added content-health error `DQ-CONTENT-404` for unresolved Companion Profile script associations.
- Updated package, readiness, and Beta health-report version declarations to 0.32.3.

## [0.32.2] - 2026-08-02

### Fixed

- Moved `DeverQuestMonsterProfile` into a correctly named standalone runtime source file.
- Added a stable package script GUID for Monster Profile assets.
- Added a narrow automatic migration that repairs existing project Monster Profile YAML associations without recreating or discarding authored data.
- Added a manual QA repair command and Library-backed migration backups.
- Updated package, readiness, and Beta health-report version declarations to 0.32.2.

## [0.32.1] - 2026-08-02

### Changed

- Kept completed one-time and satisfied limited Contracts visible to authorized leadership on the Guild Assignment Board until explicitly archived.
- Added **Restore to Offered**, which preserves Completion History and opens one additional completion slot.
- Added archive/restore controls to completed Quest Run history.
- Added wrapped, width-constrained Quest Log, Git commit, Final Quest Log, and Closing Notes text areas.
- Added duplicate-ID group display and **Keep This ID; Regenerate Other Copies** repair.
- Preserved 0.32.0 Meditation Health and Mana recovery behavior.
- Updated package, readiness, and Beta health-report version declarations to 0.32.1.

## [0.32.0] - 2026-08-02

### Changed

- Completed one-time and fully satisfied limited Contracts no longer remain on the live Guild Assignment Board for administrators; their history remains available through Chronicle, Quest Run Management, and asset inspection.
- Manual Meditation now restores capped Health and Mana when the Quest resumes.
- Beta Administration now offers an explicit, confirmed stable-ID regeneration action for selected duplicate Quest Profile and Identity assets.
- Updated package, readiness, and Beta health-report version declarations to 0.32.0.

### Fixed

- Removed completed board listings from the active Guild Assignment Board while preserving archived history.
- Added clear Meditation recovery reporting to the active Quest, Quest HUD, and generated Timecards.
- Added a practical recovery path for duplicate IDs reported by Beta content validation.

# Changelog

## [0.31.9] - 2026-08-01

### Fixed

- Fixed a compilation error in `DeverQuestTimecardWriter` where the Battle Chronicle referenced an `adventurer` variable outside its scope.
- Passed the current Adventurer explicitly into Session rendering so combat summaries use stable report context and safely fall back to the Session developer name.
- Updated package, readiness, and Beta health-report version declarations to 0.31.9.

## [0.31.8] - 2026-08-01

### Added

- Beta Administration workspace.
- Bulk validation for Quest Profiles, Contracts, Identity Catalogs, Companions, Encounters, Shops, audio profiles, and Starter Loadouts.
- Duplicate stable-ID and Quest Run/Completion ID diagnostics.
- Safe repairs for null list entries, missing Catalog defaults, incomplete linked Contracts, and refreshable reward snapshots.
- Deferred, GUI-safe starter generator repair pass.
- Exportable Markdown and JSON Beta health reports.
- Beta content health integration in Release Readiness.

### Changed

- Package version advanced to 0.31.8 Beta 1.

## [0.31.7] - 2026-08-01

### Added

- Wellness Command Center in Audio & Wellness.
- Persistent active, queued, and snoozed reminder state.
- Local searchable Wellness notification history under `Library/DeverQuest`.
- Quiet Hours end time and optional session-reminder suppression.
- Reminder queue management and 5/default/30-minute snooze actions.
- Manual cue tests for every wellness reminder category.
- Quest HUD wellness status and reminder actions.
- Release Readiness check for Wellness History storage and settings.

### Changed

- Multiple simultaneous reminder conditions queue instead of being silently skipped.
- Approved Break outcomes are copied into local notification history while remaining in the Session Wellness Journal.
- Reminder and break timing are visible from the Command Center and Quest HUD.
- Daily reminder handling and snoozes survive assembly reloads and Editor restarts.

### Guardrails

- Local notification history never replaces Timecards or Session Wellness Journal evidence.
- Test reminders do not advance real reminder schedules or award benefits.
- Quiet-hour suppression advances the due session reminder without creating an endless one-second retry loop.
- Clearing the local history does not alter Chronicle or Timecard records.

## [0.31.6] - 2026-08-01

### Added

- Hidden Editor-only AudioSource host in an Editor preview scene.
- Separate Music, Ambience, and warning/SFX sources.
- Local Master, Music, Ambience, and cue mixer controls.
- Per-channel and Master mute controls.
- Optional long-form ducking during warning and SFX cues.
- Audio focus, Play Mode, and output-device recovery.
- Active transport status, host reinitialization, and mixer reset controls.
- Release Readiness check for independent audio mixing.

### Changed

- Playlist, Ambience, and cue playback now route through the supported host when available.
- Unity's internal Inspector preview bridge remains as a compatibility fallback rather than the preferred transport.
- Audio workspace warnings now identify the active transport and fallback limitations.
- Audio preferences remain local and do not alter shared Guild or Quest data.

### Guardrails

- The host is hidden, unsaved, and explicitly destroyed on assembly reload or Editor shutdown.
- Switching transport stops current audio rather than attempting unsafe live migration.
- Host failure clears stale playback state before entering fallback mode.
- No audio media is included in the package.

## [0.31.5] - 2026-08-01

### Added

- Dedicated Git workspace and menu command, separate from the live Quest Log.
- Dockable Quest HUD sharing the active DeverQuest Session, timer, pause, break, turn-in, Encounter, and Chronicle services.
- Optional automatic Quest HUD opening when a local Quest starts.
- Visuals workspace with persistent local theme, custom colors, text scale, workspace-column, compact-label, header, guidance, and HUD controls.
- User-facing workspace guidance and stronger no-active-Quest navigation.
- Release Readiness reporting for the local Editor workspace configuration.

### Changed

- Workspace tabs are arranged by workflow rather than enum order and can use two to six columns.
- Quest Log now focuses on notes, commit links, external activity, media, Encounters, and live evidence.
- Git commit messages no longer reuse the Quest Log note field.
- Chronicle navigation now offers separate Quest Log and Git actions.
- Compact View includes a direct Quest HUD button.
- Focus Stage wording in the Quest Log is presented as Encounter wording.

### Guardrails

- The Quest HUD uses the existing active Session and does not create a second timer, reward path, Contract run, or Chronicle.
- Visual settings are local presentation data and do not modify Guild, Quest, reward, or Timecard records.
- Git operations remain explicit and never force-push.
- Saved named Visual Profile assets, portraits, and complete per-panel styling remain deferred polish.

## [0.31.4] - 2026-08-01

### Added

- Dedicated Chronicle workspace and menu command for active Quest events and completed Quest navigation.
- Live Quest Chronicle combining story, objective, current Encounter, timer state, rewards, notes, media, wellness, external activity, and tactical events.
- Searchable completed Quest archive with filters for Contract runs, rewards, commits, media, and combat.
- Expandable completed-Quest cards containing rewards, Git/Quest notes, attachments, tactical results, closing notes, and a chronological event timeline.
- Direct Timecard open/reveal, Contract selection, Run ID copy, summary copy, and correction-request navigation.
- Release Readiness validation for duplicate Session IDs, missing generated Timecards, and missing attachment paths.

### Changed

- The main Quest workspace now shows a compact recent-event feed with a direct Chronicle shortcut.
- The empty Quest Log workspace now routes users toward the completed Chronicle instead of ending at a dead panel.
- Active Chronicle repainting follows running Quests.

### Guardrails

- The Chronicle workspace reads existing Session and Timecard data; it does not create rewards, focused time, or new Quest runs.
- Missing media files are reported without deleting the recorded attachment metadata.
- Correction approval and compensation reporting remain in Rewards & History.

## [0.31.3] - 2026-08-01

### Added

- Dedicated Guild Economy workspace and menu command.
- Persistent local economy transaction ledger for purchases, sales, grants, denomination exchange, approvals, denials, and fulfillment.
- Audited leadership item and coin grants with recipient, quantity, note, and balance history.
- Merchant availability controls for opening, member access, purchases, member sales, and automatic leadership-approval thresholds.
- Searchable transaction filters and CSV export.
- Release Readiness validation for the active Quartermaster and economy-ledger IDs.

### Changed

- Guild Shop purchases now enforce the active Shop Profile and its availability rules.
- Inventory sales now require an active Quartermaster that buys member items.
- Coin denomination exchange reports physical pieces before and after while preserving canonical copper value.
- Purchase approvals, denials, sales, grants, and fulfillment now create dedicated economy records in addition to Guild audit entries.

### Guardrails

- Real-world redemptions cannot be bypassed through leadership item grants.
- Leadership grants require confirmation and never silently auto-equip equipment.
- Selling remains blocked during active Quests and for protected, equipped, or zero-value items.
- Banking, loans, crafting markets, player auctions, and housing storage remain deferred.

## [0.31.2] - 2026-08-01

### Added

- Dedicated Inventory and Equipment workspace.
- Durable item categories, subcategories, tags, lore, provenance, and resale metadata.
- Equipment-family metadata and side-by-side equipped-item comparison.
- Guarded Equip, Unequip, exact-entry Use, Sell, and Drop operations.
- Carry-load breakdown for inventory weight, coin weight, remaining capacity, and load status.
- Inventory integrity checks in Release Readiness.
- Quest Run, Encounter, monster, Guild Shop, trade, starter-loadout, and migration provenance.
- Repair action for legacy equipped gear that lacks an inventory ownership record.

### Changed

- Guild Shop cards now show category, resale value, equipment comparison, and projected encumbrance.
- Encounter loot and trades preserve item identity and origin details.
- Starter equipment is represented in the pack before it is equipped.
- Timecards summarize inventory categories, origins, and carry-load composition.
- Adventurer persistence schema advanced to data version 9.

### Guardrails

- Quest-protected items cannot be dropped or traded.
- Equipped items must be unequipped before dropping or selling.
- Guild Hall sales are blocked during an active Quest.
- Crafting, banking, housing, and broad skill progression remain deferred.

## [0.31.1] - 2026-08-01

### Added

- Added a dedicated Tactics workspace and menu command.
- Added combat-readiness reporting for Adventurer vitals, armor, load, status effects, equipment, actions, and the active Encounter.
- Added quick Companion activation, stable dismissal, individual recovery, and confirmed full-roster recovery.
- Added a persistent local Battle Archive capped at the newest 100 Battle Results.
- Added Battle Archive search and outcome filters for victory, early victory, safety pause, defeat, and Survival.
- Added report, JSON, Encounter Profile, and record-removal actions to archived battles.
- Added import of Battle Results from the active and last-completed Sessions without duplicate records.
- Added a Release Readiness write/delete probe for the Tactical Archive.

### Changed

- New resolved Encounters automatically write a Battle Archive record in addition to the Session and Timecard.
- Active Quest repainting now includes the Tactics workspace.
- Updated package metadata and Beta guidance for Tactical Operations.

### Compatibility

- Existing Timecards and Sessions remain unchanged.
- Existing 0.31.0 Battle Results may be imported from the active or last-completed Session.
- The local Battle Archive uses a project-local Library file and is not a replacement for Timecards or shared Guild records.

## [0.31.0] - 2026-08-01

### Added

- Tactical Field Reports with compact outcome, damage, condition, Companion, loot, and recent-turn summaries.
- Full combat-log copy controls while keeping the visible Quest interface compact.
- Tactical Encounter preview before a staged battle resolves.
- Companion lifetime damage, healing, damage-taken, win-rate, and last-battle reporting.
- Survival expedition milestone visibility for next wave, difficulty tier, wagon timing, and exit state.
- Persistent Survival exit method and summary in Sessions, Timecards, and Guild audit.

### Changed

- Timecards now lead with compact combat highlights and place the full turn-by-turn log in a collapsible details block.
- Battle reward wording is presented as Rewards rather than Spoils in the new tactical reports.
- Companion Stable cards now explain contribution rather than only level and Hit Points.

### Compatibility

- Existing Battle Results remain readable; summaries are derived from their stored damage, action, and combat-log events.
- Existing Companions begin with zero lifetime contribution metrics and accumulate them from battles resolved under 0.31.0 or later.

## [0.30.9] - 2026-08-01

### Added

- Quest Run Management in Guild Hall.
- Searchable Completed Quest Run Archive in Rewards & History.
- Leadership cancellation of stale run reservations and waiting Party rosters.
- Contract archive/restore controls.
- Release Readiness check for stale or invalid Quest Run reservations.

### Changed

- Archived Contracts are hidden from the Member Assignment Board while preserving completion history.
- The 0.30.8 repeatable-Contract checklist is retained as deferred regression rather than marked complete.

## [0.30.8] - 2026-08-01

### Added

- Added one-time, limited-completion, and repeatable Contract availability policies.
- Added independent Quest Run reservations so reusable solo Contracts can remain on the Assignment Board while different Adventurers complete separate runs.
- Added durable per-Contract completion history recording run IDs, Adventurers, developers, sessions, focused time, coin, XP, and completion timestamps.
- Added optional one-completion-per-Adventurer enforcement for limited team assignments.
- Added Party minimum size and full-party-required controls.
- Added automatic board lifecycle handling: one-time Contracts close after one run, limited Contracts close at their target, and repeatable Contracts reopen after each run.
- Added Assignment Board availability, completion counts, last completer, and Party start-rule feedback.
- Added Quest Run IDs to generated Timecards.

### Changed

- Quest completion now records a run directly instead of leaving every completed assignment waiting in the Contract-level Submitted/Approved pipeline.
- Abandoning a Quest releases its run reservation and reopens the Contract when appropriate.
- Partial Party Quests may begin once their configured minimum is met; full-party Contracts still require maximum capacity.

### Compatibility

- Existing Contracts migrate as one-time Contracts with one required completion.
- Existing status, reward snapshots, party fields, and Contract IDs are preserved.

## [0.30.7] - 2026-08-01

### Fixed

- Reclaimed Music, Ambience, and warning playback after Unity Inspector preview
  actions or editor-focus changes, with manual Recover and full Reset controls.
- Added direct Music and Ambience track selectors without discarding the other
  logical channel.
- Repaired the only active Guild account to CEO and stopped character-sheet
  synchronization from demoting account authority.
- Added a controlled current-character customization path and a five-silver
  starting purse for newly completed Adventurers.
- Moved automatic Git status observation off Unity's main update thread so a
  slow repository or locked Git process cannot freeze the Editor monitor loop.
- Added clear Quest-acceptance blocking reasons, Party Quest waiting feedback,
  and pre-start party withdrawal.
- Displayed Quest Story during selection and active work, presented Focus Stages
  as Encounters, and supplied fallback Encounter names in sessions and timecards.
- Displayed recommended and minimum qualifying Approved Break durations.
- Added compact reward values directly to Guild Assignment Board entries.

### Readiness

- Added sole-founder Guild-authority validation.
- Added a timecard Git-hygiene advisory when Chronicles and voice memos are stored
  inside the project repository without an explicit `.gitignore` rule.

## [0.30.6] - 2026-07-31

### Fixed

- Repaired starter Identity Catalog activation when an invalid or Missing Script
  `GuildIdentityRegistry.asset` occupies the canonical registry path.
- Reload and verify the newly created registry before calling `SetDirty`.
- Deferred Guild Hall starter-catalog generation outside the active IMGUI draw
  event, preventing the secondary invalid GUILayout-state error.
- Added the exact Guild Hall navigation path to the readiness advisory.

## [0.30.5] - 2026-07-31

### Fixed

- Added stable Unity `.meta` files for the ScriptableObject source files newly
  separated in 0.30.4, including Ambience and starter Identity assets. Existing
  package script GUIDs were not broadly replaced during the Beta patch.
- Reopened DQ-0302-002 after Beta evidence showed the Ambience Profile could
  return to Missing Script; the screenshot also confirmed 0.30.3 was still the
  active package during that test.
- Replaced mutually exclusive long-form preview ownership with independent
  Music and Ambience logical channels. Stopping, pausing, changing, or replacing
  one channel now preserves the other channel.
- Warning cues can play over Music and Ambience while repeated controls rebuild
  the native preview transport to prevent ghost clips.

### Verification

- Existing assets already showing Missing Script must be deleted and recreated
  once after installing 0.30.5.
- DQ-0302-002 and DQ-0304-005 remain pending Unity verification.

## [0.30.4] - 2026-07-31

### Beta Asset Association Hotfix

- Fixed DQ-0302-002 after Unity verification showed that newly created
  Ambience Profiles had a missing ScriptableObject script.
- Moved `DeverQuestAmbienceProfile` into its own matching runtime source file
  so Unity can create, inspect, serialize, and assign the asset correctly.
- Corrected the same secondary ScriptableObject layout for Ability Profiles,
  Spells, Companion Catalogs, Encounter Profiles, and Shop Profiles.
- Moved starter Identity asset types into dedicated matching files, addressing
  the asset-association root cause behind DQ-0302-004.
- Added explicit cleanup and recreation instructions for assets that were
  created with Missing Script under 0.30.3.
- Updated package and Release Readiness metadata to 0.30.4.

## [0.30.3] - 2026-07-31

### Beta 1 Stabilization

- Fixed Ambience Profile creation and assignment so newly created profiles are
  immediately installed, manually selected profiles can be adopted directly,
  and empty profiles explain why no ambience can play.
- Fixed Quest Spoils previews by showing the effective Contract snapshot,
  detecting linked-profile mismatches, and refreshing editable Contract
  snapshots for authorized Guild managers.
- Hardened original starter Identity Catalog generation by deferring it outside
  the Editor GUI event, accepting safe retries after partial creation, repairing
  missing collections, and avoiding an unnecessary AssetDatabase refresh.
- Added Main Quest progress feedback with target duration, time remaining or
  overtime, progress percentage, current Encounter, pacing messages, and the
  current Spoils estimate.
- Added repository-hygiene, Identity Catalog, and Spoils snapshot checks to the
  Release Readiness report.
- Added credits, third-party notices, Beta notes, and repository-root setup
  guidance.

## [0.30.2] - 2026-07-31

### Fixed

- Fully qualified `UnityEditor.PackageManager.PackageInfo` in the release-readiness service to resolve its collision with `UnityEditor.PackageInfo` under Unity 2022.3.
- Updated package and readiness-report version metadata to 0.30.2.

## [0.30.1] - 2026-07-31

### Fixed

- Restored Unity 2022.3 / C# 9 compatibility by moving multiline conditional
  expressions outside non-verbatim interpolated strings.
- Added explicit generic type arguments to all Tactical Starter Kit `Upsert`
  calls so C# can resolve each ScriptableObject asset type.
- Removed a misplaced combat-safety block from Survival escape handling that
  referenced a nonexistent `battle` variable.
- Qualified `UnityEngine.Object` in the Rules Laboratory to avoid ambiguity
  with `System.Object`.
- Updated release-readiness version checks for the 0.30.1 hotfix package.

## [0.30.0] - 2026-07-31

### Added

- A release-readiness report under **Tools > DeverQuest** for package, Unity,
  profile, storage, Chronicle, shared-Guild, audio, and active-Quest checks.
- Release-candidate scope-lock and regression documentation.

### Fixed

- Replaced unsupported pseudo-layered editor preview audio with explicit
  single-channel ownership.
- Playlist and ambience now release one another deterministically instead of
  leaving stale playback state.
- Warning cues temporarily interrupt and restore the active long-form clip at
  its captured sample position.
- Rapid Next, Previous, Stop, Play, cue, and ambience combinations can no
  longer accumulate independently tracked preview clips.
- Playlist completion now follows the actual preview transport state rather
  than a wall-clock estimate that could drift after pause or cue interruption.
- Preview audio is stopped and cleared during assembly reload and editor exit.

### Release Candidate

- Major feature expansion is frozen after this milestone. Crafting, banking,
  housing, biome simulation, and broad tradeskill systems are deferred to a
  post-release roadmap.

## [0.29.0] - 2026-07-31

### Added

- Class-linked Ability Profiles, Attack Techniques, mana costs, cooldowns,
  tactical priorities, and structured combat effects.
- Direct damage, healing, ongoing damage/healing, life drain, root, snare,
  stun, silence, shields, buffs/debuffs, cleanse, dispel, and original
  homeward-return actions.
- Independent development early-completion and battle-under-par bonuses.
- Cascading per-Stage Focus clocks and manual objective completion reporting.
- Repeating Survival waves with configurable difficulty/reward growth, flee
  checks, return abilities, and periodic Guild wagon exits.
- Item and coin weight, encumbrance pauses, Guild Hall denomination exchange,
  and in-session item dropping.
- Low-HP fight safety pauses using the configurable Encounter Danger cue.
- Tactical Battle Chronicle action, pace, wave, carry, and safety records.
- Re-runnable original Tactical Starter Kit with 15-minute and Survival Quest
  templates.

### Changed

- Encounter lookup is cached and invalidated only when the Unity project
  changes, keeping the live Stage update path free of repeated asset scans.
- Existing copper balances migrate to physical denominations without changing
  their canonical value.

## [0.28.0] - 2026-07-31

### Added

- Optional per-Adventurer Compensation Preview policies managed by Bosses and
  CEOs.
- Hourly-rate and annual-salary tracking-equivalent modes with configurable
  three-letter currency display and scheduled weekly hours.
- Current-workweek and filtered-history estimates in Rewards & History.
- Focused-only or Focused-plus-Approved-Break eligibility policies.
- Verified-only or Verified-plus-Legacy Chronicle eligibility policies.
- Explicit exclusion totals for modified, unavailable, and policy-ineligible
  legacy Chronicles.
- Manual-review warnings for time matching configured long/frequent Quest
  flags.
- A dedicated CSV export labeled as a planning estimate.

### Boundaries, Privacy, and Performance

- Compensation Preview is disabled by default and never performs payment.
- Estimates are not payroll, wage statements, promises, tax advice, or payment
  authorization.
- Meditation and Idle/Unverified time never qualify.
- Active Quests do not appear until finalized into Chronicle history.
- Rates remain in the local Guild-account preference store. They are not
  written into daily timecards, shared Guild snapshots, or audit details.
- Local compensation settings are a convenience, not encrypted payroll
  storage.
- Calculations use the already cached History view and run only while Rewards
  & History is visible; no AssetDatabase scan or background payroll loop was
  added.

## [0.27.0] - 2026-07-31

### Added

- Persistent per-account Companion rosters with one active Companion.
- Companion Profile and Companion Catalog ScriptableObjects.
- Bonded Beast, Familiar, Bound Minion, Spirit, Construct, and Mercenary
  categories.
- Striker, Guardian, Support, and Controller combat roles.
- Class and level eligibility, optional recruitment cost, starter Companion
  references, and commercially clean original starter generation.
- Persistent Companion HP, loyalty, XP, levels, battles, victories, active
  state, fallen state, names, and recruitment provenance.
- Companion combat turns using Milestone 26 creature types, typed damage,
  resistance, immunity, vulnerability, and absorption.
- Stable recruitment, activation, dismissal, renaming, and paid recovery.
- Companion results in compact mode, the Character Sheet, Battle Chronicle,
  shared Adventurer snapshots, and generated timecards.
- Blank Companion Profile and Catalog templates plus organized production
  folders.

### Combat Roles

- Strikers receive a small level-scaled damage bonus.
- Guardians are more likely to intercept attacks aimed at the Adventurer.
- Support Companions can spend their turn restoring Adventurer HP.
- Controllers can reduce the next enemy attack after landing a hit.

### Migration, Safety, and Performance

- Existing Adventurers and Guild accounts migrate to an empty Companion roster
  without changing identity, progression, coin, equipment, Quests, or
  Chronicles.
- Companion Profiles use the same project-change-invalidated asset cache as
  other character content; no per-repaint AssetDatabase scan was added.
- Companion simulation never grants focused-work seconds or productivity
  rewards.
- Shipped Companions use original DeverQuest names and lore. Private Guilds
  remain responsible for content they author.
- Compensation Preview remains Milestone 28.

## [0.26.0] - 2026-07-31

### Added

- Seventeen original, generic creature families and fourteen damage types.
- Vulnerable, Resistant, Immune, and Absorbs affinity responses.
- Typed attacks on Monster Profiles, Equipment, and Spells.
- Defensive affinities on Monsters, Ancestries, and Equipment.
- A complete Guild Combat Codex generator and blank catalog template.
- Typed damage totals on the Character workspace, Battle Chronicle, and
  generated daily timecard.
- Deterministic damage-event records containing source, target, round, raw
  damage, final damage, response, and absorbed healing.
- Typed tutorial weapon, spell, resistant ring, and Undead opponent.

### Rules

- Resistance halves incoming damage, rounded up.
- Vulnerability doubles incoming damage.
- Immunity prevents damage.
- Absorption prevents damage and restores that amount, up to maximum HP.
- Resistance and vulnerability cancel each other; duplicate affinities do not
  stack. Absorption and immunity take defensive precedence.
- Guaranteed work rewards remain independent from battle outcomes.

### Performance and Compatibility

- Equipped gear and known spells continue using project-change-invalidated
  caches; the combat pass adds no per-repaint AssetDatabase scans.
- Existing equipment, spells, Monsters, Ancestries, sessions, and Chronicles
  deserialize with safe default damage types and empty affinity lists.
- Companion simulation remains Milestone 27. Compensation Preview remains
  Milestone 28.

## [0.25.0] - 2026-07-31

### Added

- Reusable Ancestry, Class Definition, Faith, and Identity Catalog
  ScriptableObjects with durable stable IDs.
- Guided Ancestry, Class, Alignment, and Faith selection during first-login
  Adventurer creation.
- Eligibility validation for playable/sapient Ancestries, Class restrictions,
  and Faith Alignment restrictions.
- Quest Contract Class and Ancestry eligibility using durable asset
  references, while preserving legacy Class-name lists.
- Catalog-driven ability foundations, Department, hit die, HP, Mana, saving
  throws, Ancestry adjustments, traits, languages, and Faith identity.
- Companion-tradition metadata as a non-simulated Milestone 27 hook.
- One-click commercially clean original starter catalog with nine Ancestries,
  fifteen Classes, five Faiths, and an aggregate Guild catalog.
- Project-backed Guild Identity Registry that records the active catalog for
  the whole Unity project rather than as a per-user editor preference.
- Blank identity templates and organized identity-catalog production folders.
- Identity details on the Character workspace, compact view, Settings
  identity summary, generated timecards, shared Guild snapshots, and the Hall
  of Heroes.

### Migration

- Existing Adventurer and Guild-account data receives stable Class, Ancestry,
  and Faith references when matching assets exist.
- Legacy characters keep their names, ranks, levels, XP, coin, attributes,
  equipment, spells, inventory, and Chronicle history.
- Missing legacy Ancestry/Faith values adopt the generated catalog defaults
  without recalculating or replacing existing character statistics.

### Commercial Safety

- Shipped starter Ancestries and Faiths use original DeverQuest names and
  lore rather than third-party game-specific names.
- Custom private Guild catalogs are supported; their creators remain
  responsible for permissions and licenses for content they add.
- Creature types, elemental resistances, and combat damage categories remain
  reserved for Milestone 26. Companion simulation remains Milestone 27.

## [0.24.3] - 2026-07-30

### Fixed

- Corrected the Trading Post account filter to use the existing
  `DeverQuestGuildAccount.disabled` field.
- Restored compilation after 0.24.2 referenced a nonexistent `enabled`
  property.
- Enabled trade targeting for every non-disabled Guild account except the
  currently signed-in account.

## [0.24.2] - 2026-07-30

### Added

- Idempotent **Campaign Content Scaffolding** panel in Guild Hall.
- One-click organized production folders under `Assets/DeverQuest`.
- Blank ScriptableObject templates for Quest Profiles, Contracts, Encounters,
  Monsters, Equipment, Spells, Starter Loadouts, Shop Items, Shop Profiles,
  Playlists, Warning Audio, Ambience, and External Activity Profiles.
- Interconnected **Trouble in the Tutorial Crypt** demonstration campaign.
- Tutorial Necromancer loadout, equipment, spell, Shop, provisions,
  real-reward workflow example, monster, encounter, focus stages, guaranteed
  rare loot, audio placeholders, and Aseprite activity profile.
- Automatic selection of the generated folder, tutorial Contract, and
  tutorial Quartermaster.
- Generation report listing created and preserved content.
- Milestone 0.24.2 organization map and tutorial walkthrough.

### Safety

- Generation never deletes or overwrites an existing folder or asset.
- Production templates and tutorial content are isolated from one another.
- The tutorial real-reward voucher is explicitly non-delivering test content.
- Content generation requires authenticated CEO or Boss permission.

## [0.24.1] - 2026-07-30

### Performance

- Replaced the monolithic dashboard with seven lazy workspace tabs: Quest,
  Quest Log & Git, Character, Guild Hall, Rewards & History, Audio &
  Wellness, and Settings.
- Added direct workspace entries beneath **Tools > DeverQuest > Workspaces**.
- Reduced active-timer repainting from every Unity editor update to four
  updates per second.
- Timer repainting now stops entirely while a non-live workspace is selected.
- Reduced background Git polling from a full status suite every five seconds
  to a two-command HEAD snapshot every fifteen seconds.
- Full Git inspection now runs in the monitor only when the repository or HEAD
  changes; manual Refresh remains immediate.
- Cached Shop Item, equipment, and spell AssetDatabase lookups until Unity
  reports that project assets changed.
- History, shared Guild records, Contract boards, inventory, and character
  calculations render only in their selected workspace.
- Removed a duplicated Campaign Rules setup draw.

### Compatibility

- Quest timing, idle detection, wellness, playlist playback, Git observation,
  rewards, trading, Chronicles, and shared publishing retain their existing
  data formats.
- This is the performance checkpoint before Milestone 25. It contains all
  Milestone 24 functionality and does not contain Compensation Preview.

## [0.24.0] - 2026-07-30

### Added

- Item rarity, binding, acquisition source, and persistent ownership IDs.
- Unique ownership records for equipment, redemptions, and rare-or-better
  loot; legacy inventory is migrated without discarding quantities.
- Guild Trading Post with explicit recipient selection and escrow.
- Accept, reject, cancel, and reclaim actions with a permanent trade ledger.
- Bound-item and real-reward restrictions that prevent invalid transfers.
- Configurable real-reward types for Nitro, merchandise, gift cards,
  monetary bonuses, and custom fulfillment.
- Leadership approval followed by a separate delivered/fulfilled action and
  optional delivery reference.
- Milestone 24 regression documentation.

### Integrity

- An offered item leaves the sender's usable inventory until accepted,
  cancelled, rejected and reclaimed.
- Trade history is append-preserving and records both Adventurers, item
  identity, rarity, status, and resolution.
- Real rewards are never automatically represented as externally delivered;
  Guild leadership must explicitly mark fulfillment.

## [0.23.0] - 2026-07-30

### Added

- Configurable shared Guild repository folder with availability and
  write-access validation.
- Automatic publishing after rewards and Chronicle writing have finalized.
- Manual last-Quest publishing for recovery and migration checks.
- One shared Quest record per Adventurer/session plus current Adventurer
  snapshots.
- Atomic JSON writes and record-level SHA-256 integrity hashes.
- Hall of Heroes rankings for healthy ranked focus, raw focus, XP, coin,
  Quest count, Contract count, level, and current streak.
- Project and Department standings across published Adventurers.
- Invalid or modified shared-record quarantine counts.
- Shared repository activity in the local Guild administration audit.
- Milestone 23 setup and regression documentation.

### Fairness and Review

- Suspiciously long Quests and sessions with a high Idle/Unverified ratio
  receive review flags and do not contribute to ranked focus.
- Excessive daily Quest frequency adds a review flag.
- Ranked focus is capped per Adventurer/day using the configurable healthy
  daily limit. Raw totals remain visible for review.
- Fantasy progression never changes recorded work time.

### Security Boundary

- Shared JSON records are tamper-evident, not self-authorizing. A shared
  folder becomes an authoritative source only when the studio controls its
  server, network-share, or cloud permissions. A local user with permission
  to replace every file can also recompute an unkeyed integrity hash.

## [0.22.0] - 2026-07-30

### Added

- External Activity Profile assets containing reusable foreground-tool
  providers.
- One-click Aseprite Activity Profile generation.
- Windows foreground-process and window-title matching with recent-input
  freshness checks.
- External Activity Journal entries with start/end timestamps and observed
  duration.
- Voice memo recording through Unity's selected microphone.
- WAV storage under each Adventurer's dated Chronicle Media folder.
- Existing-file attachment with a protected copied media file.
- Chronicle Media Attachments with local links and voice-memo durations.
- External craft time and media-attachment counts in Daily Totals.
- Milestone 22 regression checklist.

### Integrity

- A configured external application must be foreground and show recent system
  input; merely leaving Aseprite or another provider open does not keep a
  Quest active indefinitely.
- External activity changes idle classification only. It does not invent
  focused seconds, Focus Stage completion, coin, or XP.
- Unlinking an attachment removes it from the active Quest without deleting
  the copied media file.
- Active external intervals are closed and recordings are safely cancelled
  before Quest completion, abandonment, or script reload.

### Compatibility

- Foreground external-tool detection is implemented for Windows in 0.22.0.
  Other editor platforms retain normal idle behavior and may still use media
  attachment and microphone features where Unity exposes them.

## [0.21.1] - 2026-07-30

### Fixed

- Opening or revealing a Chronicle now grants an intentional external-action
  grace period so reviewing a timecard does not pause the active Quest or
  disturb session-aware playlist playback.
- Wellness reminder breaks now use Approved Break classification instead of
  being recorded as ordinary Meditation.
- A reminder dismissed with **Acknowledge Only** no longer counts as a break.
- Starting a break while the Quest is already paused no longer creates a
  misleading **Break Started** entry.

### Added

- Configurable short, meal, and quiet-hours wellness break durations.
- Configurable XP for a substantially completed wellness break.
- Completed wellness breaks update appropriate hunger, rest, or happiness
  values and appear separately from acknowledgments in the Chronicle.
- A packaged 0.21.1 regression checklist.

### Changed

- Wellness reminders now explain the difference between acknowledgment,
  snoozing, and taking an Approved Break.
- A wellness break must reach at least 80% of its configured duration before
  granting XP or character-state benefits. Ending early remains recorded
  without granting the benefit.

## [0.21.0] - 2026-07-30

### Added

- Warning Audio Profile assets with custom clips for idle warning/pause,
  focus, hydration, movement, meals, Stage completion, combat attack/danger,
  victory, defeat, Quest completion, purchase, level-up, and errors.
- Ambience Profile assets with multiple clips, shuffle, volume, looping, and
  active-Quest behavior.
- Independent ambience controls in full and compact views.
- Warning, victory, and level-up preview buttons.
- Persistent Warning and Ambience Profile selection.
- Per-track playlist weights used by weighted shuffle.

### Changed

- Music, ambience, and one-shot cues use layered Editor preview playback.
- Music stop/pause/resume attempts per-clip control before falling back to
  Unity's global preview controls.
- Wellness, idle, Stage, Encounter, Quest, shop, and level-up events now route
  through configured fantasy audio before using system beeps.

### Compatibility

- Unity Editor preview APIs remain internal and vary by version. DeverQuest
  detects available volume, playback, and per-clip controls and retains safe
  fallbacks where a specific control is unavailable.

## [0.20.0] - 2026-07-30

### Added

- Monster Profile assets with level, HP, AC, attacks, damage dice, victory
  spoils, and configurable drop tables.
- Encounter Profile assets with authored introduction, multiple waves, enemy
  counts, boss waves, victory bonuses, injuries, and optional death.
- Deterministic staged battle resolution when real-work Focus Stages complete.
- Class-aware attack abilities, armor checks, attacks, misses, damage, defeat,
  and capped combat logs.
- Equipment, Spell, Shop Item, coin, and XP drops.
- Persistent injuries, defeat count, fallen state, and paid Guild Shrine
  resurrection.
- Active and compact Battle Chronicle summaries.
- Full battle details, deterministic seed, combat log, enemies, loot,
  consequences, and bonus spoils in generated Chronicles.
- Safe Guildhall Training Encounter generator.

### Integrity

- Quest completion and Focus Stage work rewards never depend on combat rolls.
- Battle outcomes award separate bonus spoils and cannot erase focused time.
- Encounter seeds and round logs are retained for review.

## [0.19.1] - 2026-07-30

### Fixed

- Restored the missing UnityEngine import required by the Shop ledger's
  JsonUtility persistence.
- Removed an obsolete finalization-state field that produced a compiler
  warning.
- Restored successful Editor assembly compilation and the
  **Tools > DeverQuest > Developer Companion** menu.

## [0.19.0] - 2026-07-30

### Added

- Guild Shop Profile and Shop Item ScriptableObjects.
- Purchasable Equipment, Spells, consumables, food, drink, inn rest, approved
  break permits, and leadership-controlled redemption items.
- Persistent per-account inventory with quantities.
- Purchase requests, leadership approval/denial, delivery, and redemption
  history.
- Wellness effects for HP, mana, hunger, rest, and happiness.
- Sanctioned break permits with explicit approved-break duration.
- Starter Quartermaster generator with provisions, potions, inn rest, and
  smoke/privy break permits.
- Inventory summaries in generated Chronicles.

### Integrity

- Coin is charged when an immediate purchase succeeds or when leadership
  approves a restricted request.
- Approved break duration is kept separate from meditation and focused work.
- Time beyond a permit's duration is classified as idle/unverified.

## [0.18.0] - 2026-07-30

### Added

- Guided first-login Adventurer creation for administrator-created accounts.
- Expanded Agility, Stamina, Luck, mana, hunger, rest, happiness, and home
  Department character data with compatible account migration.
- EQ-inspired equipment slots and a generator for Copper, Bronze, Iron, and
  Steel starter Equipment.
- Class-specific Starter Loadout assets for starting gear and spells.
- Solo and Party Quest capacity, reserved rosters, participant roles, class
  and Department restrictions, and party submission state.
- Dungeon Master-authored Quest story and role-aware Focus Stages.
- Per-Stage coin and XP awards, shared Contract progress, and future Encounter
  hooks.
- Full and compact active-Quest summaries for story, party, Stages, combat
  statistics, and wellness needs.
- Group Quest completion bonuses once the required party is assembled.

### Changed

- The Guild Hall automatically collapses during an active Quest.
- The completion wizard keeps counting focused work through notes and Git
  operations; the timer stops only on the final claim.
- The Guild Shop and later roadmap milestones move forward by one number.

## [0.17.0] - 2026-07-30

### Added

- Versioned character rules data with Strength, Dexterity, Constitution,
  Intelligence, Wisdom, and Charisma.
- Character HP, maximum HP, class hit die, Armor Class, proficiency bonus,
  saving-throw proficiencies, class features, and status effects.
- Class-specific migration foundations for existing and newly created
  Adventurers, including Necromancer rules for Ajnaag.
- Level-up HP progression alongside the existing XP progression.
- Equipment ScriptableObjects with slots, AC bonuses, ability bonuses, and
  minimum levels.
- Spell ScriptableObjects with spell level, casting ability, damage dice,
  status effects, and minimum character levels.
- Equipped item and known spell persistence per Guild account.
- Administrator actions to grant/equip items and teach spells.
- Deterministic seeded d20 checks with ability, proficiency, Daily Decree,
  total, DC, and success/failure reporting.
- Deterministic standard dice-expression resolution for future combat.
- Daily Decree recommended level, campaign difficulty, and check modifier.
- Character rules summary in generated Chronicles.

### Migration

- Existing XP, level, coin, class, rank, and identity remain intact.
- Guild accounts created in 0.16.0 receive their class foundation when first
  loaded by 0.17.0.

### Encounter Boundary

- This milestone establishes character mechanics and reproducible rules.
  Enemies, initiative, damage exchanges, and battle rewards remain reserved
  for the Encounter milestone.

## [0.16.0] - 2026-07-30

### Added

- Local Guild accounts created by authenticated Guild leadership.
- PBKDF2-derived local passcode protection with per-account random salts.
- Automatic migration of the existing Adventurer into the founding CEO
  account without resetting XP, level, coin, or lifetime totals.
- Per-account Adventurer progression, identity, class, Guild Rank, and Project
  assignments.
- Guild login/logout flow and locked identity fields.
- CEO, Boss, Project Leader, and Member permission enforcement.
- Project-scoped Contract and correction authority for Project Leaders.
- Local authority audit entries for login, account creation, Contract state,
  correction review, settings changes, migration, and profile reset actions.

### Permission Matrix

- CEO: all local Guild actions.
- Boss: all local Guild actions except destructive record or program deletion.
- Project Leader: Contract, correction, and Project management only for
  explicitly assigned Projects.
- Member: Quest input, check-in, check-out, notes, and turn-in.

### Security Boundary

- Passcodes protect actions through the DeverQuest interface and are never
  stored as plaintext.
- Local administrators and users with unrestricted access to the machine can
  still alter Editor preferences. Shared authoritative identity remains
  reserved for the Guild service milestone.

## [0.15.0] - 2026-07-30

### Added

- Numbered same-day Chronicles with automatic rollover by Quest count or
  JSON size and a manual **Start New Chronicle** action.
- SHA-256 integrity seals stored in an append-only, chained audit journal.
- Verified, Modified, Legacy, and Unavailable integrity states in History.
- Append-only correction requests that preserve the original Quest record.
- Leadership approval and return actions for pending correction requests.
- Correction author, reason, proposed record, timestamps, reviewer, and
  disposition in JSON sidecars and generated Markdown.
- Configurable flags for unusually long Quests and unusually frequent daily
  Quest activity. Flags request review and never automatically reject time.
- Focused, Meditation, Approved Break, Idle/Unverified, and Legacy
  Unclassified time reporting.

### Compatibility

- Existing daily records remain readable and are labeled Legacy until a new
  integrity-aware write creates a seal.
- Chronicle 1 retains the original filename. Later Chronicles use numbered
  filenames and therefore never overwrite Chronicle 1.

### Security Boundary

- Local integrity seals expose accidental or casual record edits; they are not
  server authority and cannot prevent a user with full local access from
  replacing both a record and its audit journal.
- The structured `.deverquest.json` file is the sealed source record.
  Markdown remains a human-readable generated report.

## [0.14.0] - 2026-07-30

### Added

- Quest Contract ScriptableObject assets for actual assigned studio work.
- Contract creator, assignee, open assignment, minimum level, priority, due
  date, deliverables, project, department, objective, and snapshotted spoils.
- Guild Assignment Board visible from the Accept Quest panel.
- Draft, Offered, Accepted, Active, Submitted, Approved, Returned, and
  Completed Contract states.
- Leadership actions to offer, return, approve, and complete Contracts.
- Member selection of assigned or open, level-appropriate Contracts.
- Reserved Encounter Profile identifier and encounter notes for the later
  battle milestone.
- Contract identity, assignment, deliverables, and encounter reservations in
  active Quests and generated Chronicles.

### Lifecycle

- Selecting an Offered Contract as a Member accepts it.
- Starting Contract work makes it Active.
- Turning in Contract work makes it Submitted.
- Abandoning Contract work returns it for leadership review.
- Leadership can approve or return a submission and complete an approved
  Contract.

### Integrity

- Contract and Quest Profile terms are copied into the session when work
  begins, preventing later asset edits from changing historical records.
- Profile-specific work-block carry no longer crosses between different Quest
  Profiles or Contracts.

## [0.13.0] - 2026-07-30

### Added

- Reusable DeverQuest Quest Profile ScriptableObject assets.
- Unlimited administrator-authored profiles containing identity, project,
  task, department, objective, suggested duration, eligibility, coin, XP, and
  work-block payout rules.
- Create Quest Profile and Inspect Selected Profile actions for Guild
  leadership.
- Member eligibility rules for approved profiles and minimum Adventurer level.
- Quest Profile identity, suggested focus time, and projected profile spoils
  in the active workflow.
- Immutable Quest Profile snapshots in session data and generated Chronicles.

### Permissions

- Members must select an available, level-appropriate Quest Profile.
- Project Leaders, Bosses, and CEOs can create profiles and accept custom
  Quests.
- Guild Rank remains locally managed in this foundation release; authoritative
  accounts and administrative locking are reserved for the Guild
  Administration milestone.

### Compatibility

- Existing custom Quests continue using the global reward configuration.
- Editing a Quest Profile never changes an already accepted or completed
  Quest because payout values are copied into the session snapshot.

## [0.12.2] - 2026-07-30

### Changed

- Quest Turn-In is now two focused steps instead of six:
  Chronicle review followed by Spoils and completion.
- Git commit/push controls, Quest Log review, and Closing Notes share the
  Chronicle step.
- The second step previews rewards and completes the Quest with one explicit
  Claim Spoils action.
- The temporary manual coin-spending control has been removed. Coin remains
  earn-only until the Guild Shop provides meaningful purchases.

### Added

- Configurable base coin and XP for every completed Quest.
- Work-block and Daily Decree rewards stack on the base Quest payout.

### Fixed

- Older commit records with an empty provenance value now print as
  `[Legacy Entry]` instead of `[]`.

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
