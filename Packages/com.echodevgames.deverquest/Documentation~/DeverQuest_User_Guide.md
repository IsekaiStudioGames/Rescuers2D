# DeverQuest User Guide

## Release Candidate and Audio Ownership

Version 0.30.0 begins the release-candidate phase. Major new systems are frozen
while the existing timer, persistence, Chronicle, Guild, RPG, and audio systems
complete regression.

Run **Tools > DeverQuest > Run Release Readiness Check** before a release pass.
The report validates the package and Unity versions, developer profile,
timecard storage, Chronicle policy, shared Guild path, audio transport, and
active Quest state.

Playlist music and ambience now share one explicit long-form editor-audio
channel. Starting one stops and releases the other. Warning cues temporarily
interrupt the active long-form clip and restore it near its captured sample
position. See `DeverQuest_0.30.0_Release_Candidate.md` and
`DeverQuest_0.30.0_Regression_Checklist.md`.

## Tactical Abilities and Survival Quests

Version 0.29.0 adds tactical Ability Profiles, structured combat effects,
independent development/battle pace bonuses, Survival waves, safe exits,
low-HP warnings, and weighted packs plus coin.

See `DeverQuest_0.29.0_Tactical_Survival.md` for starter-kit generation,
authoring rules, survival exits, encumbrance, and compatibility.

## Compensation Preview

Version 0.28.0 added an optional planning estimate based on finalized Chronicle
time. A Boss or CEO configures the current Adventurer's local policy under
**Guild Hall > Guild Accounts and Authority > Compensation Preview Policies**.
The Adventurer reviews current-week and filtered estimates under **Rewards &
History > Compensation Preview**.

Only finalized Focused Work qualifies by default. A policy may also include
completed Approved Break time. Meditation and Idle/Unverified time never
qualify. Modified or unavailable Chronicles are always excluded, and
legacy/unsealed Chronicles require an explicit policy choice.

This is not payroll, a wage statement, a promise of payment, tax advice, or
authorization to pay. Rates stay in local editor preferences rather than
timecards or shared Guild snapshots, and that local storage is not encrypted
payroll storage.

See `DeverQuest_0.28.0_Compensation_Preview.md` for the full setup, calculation,
export, privacy, and validation notes.

## Pets, Familiars, Minions, and Companions

Version 0.27.0 gives every Adventurer a persistent Companion roster. Open the
**Character** workspace and expand **Companion Stable**. Leadership can
generate the original starter Stable, create Companion Profile assets, and
author additional Companion Catalogs.

A Companion Profile controls:

- its Kind, Role, Creature Type, and lore;
- allowed Classes and minimum Adventurer level;
- HP, AC, attack modifier, damage dice, and Damage Type;
- resistance, vulnerability, immunity, or absorption affinities;
- starting loyalty; and
- recruitment and recovery costs.

Only one Companion is active at a time. Recruit an eligible profile, rename
the resulting Companion if desired, and choose **Set Active**. A fallen
Companion stops participating until **Recover** is purchased at the Stable.

Companion combat roles have concrete deterministic behavior:

- **Striker:** additional level-scaled damage.
- **Guardian:** a greater chance to intercept an enemy attack.
- **Support:** a chance to restore Adventurer HP instead of attacking.
- **Controller:** a successful hit can reduce the next enemy attack.

Companions gain half of an encounter's XP after surviving a victory. Their
level, current HP, loyalty, battle count, and victory count persist with the
authenticated Guild account. New Adventurers receive the starter Companion
referenced by their Class Definition when one is configured.

Companion activity is RPG evidence only. It never adds focused seconds,
completes work objectives, or grants productivity rewards.

See `DeverQuest_0.27.0_Companions.md` for authoring and validation.

## Creature Types, Damage Types, and Resistances

Version 0.26.0 makes encounter damage data-driven. A Monster Profile now has a
Creature Type, attack Damage Type, and affinity list. Equipment can define a
weapon Damage Type plus defensive affinities. Spells define their Damage Type,
and Ancestries may provide innate affinities.

Each affinity pairs one Damage Type with one response:

- **Vulnerable** doubles damage.
- **Resistant** halves damage, rounded up.
- **Immune** prevents damage.
- **Absorbs** prevents damage and restores that amount up to maximum HP.

Resistance and vulnerability cancel instead of stacking. Duplicate entries do
not multiply their effect. Absorption takes precedence, followed by immunity.

Open **Character > Rules Laboratory** and select **Generate Guild Combat
Codex** to create a reference asset containing every shipped Creature and
Damage Type. Use the ordinary ScriptableObject Inspector to author affinities
on Monsters, Ancestries, and Equipment.

During an encounter, DeverQuest uses the equipped Main Hand weapon when it has
damage dice. Otherwise it uses the first known damaging Spell, followed by a
safe Guild Strike fallback. The Character Sheet displays the Adventurer's
effective affinities. Battle records preserve typed totals and every round's
raw damage, final damage, response, and absorption.

See `DeverQuest_0.26.0_Combat_Typing.md` for the validation checklist.

## Ancestries, Classes, Faiths, and Catalogs

Version 0.25.0 makes Adventurer identity project-authored content. A CEO or
Boss can open **Guild Hall > Campaign Content Scaffolding** and select
**Generate Original Starter Identity Catalog**. The generator creates:

- nine playable original Ancestry assets;
- fifteen Class Definition assets;
- five original Faith assets; and
- one Identity Catalog that connects them and declares creation defaults.

The generator is idempotent. It preserves any existing asset at a generated
path, while adding missing content. The same panel can create individual
blank Ancestry, Class Definition, Faith, and Identity Catalog assets.

During first-login character creation, select project assets for Ancestry,
Class, and Faith, then choose an Alignment. The screen previews attributes,
HP, Mana, Department, and future companion support. Invalid Ancestry/Class or
Faith/Alignment combinations cannot enter the Guild.

Legacy Adventurers are migrated by stable identity IDs when matching assets
exist. Generating the starter catalog assigns safe default identity references
where a legacy record had no Ancestry or Faith, but it does not reroll or
replace that Adventurer's existing statistics or progression.

The included names and lore are original starter content. A private Guild may
create any custom catalogs it has permission to use. Do not distribute
third-party game names, artwork, audio, rules text, or lore without the
appropriate rights.

See `DeverQuest_0.25.0_Identity_Catalogs.md` for asset fields, testing, and
milestone boundaries.

## Starter Campaign and Organized Content

Version 0.24.2 adds **Campaign Content Scaffolding** to the Guild Hall
workspace. CEO and Boss accounts may choose:

### Create Empty Studio Structure

This creates organized folders beneath `Assets/DeverQuest` for audio,
characters, Guild content, Quests, activity profiles, playlists, and reusable
templates. The Templates folder contains a blank example of every currently
supported ScriptableObject type.

### Create Tutorial Campaign

This also creates **Trouble in the Tutorial Crypt**, an isolated demonstration
under `Assets/DeverQuest/DemoCampaign`. Its Contract and Quartermaster are
selected automatically. The tutorial connects a Quest Profile, offered
Contract, two focus stages, encounter, monster, guaranteed Rare equipment
drop, Necromancer equipment and spell, starter loadout, Shop, consumable,
manual real-reward example, empty audio profiles, and Aseprite activity
provider.

The generated audio assets intentionally contain no AudioClips. Drag compatible
Unity AudioClips into the Playlist, Ambience, and Warning Profile before
testing playback.

Generation is idempotent: any asset already found at a generated path is
preserved. Delete or rename tutorial content manually only when you explicitly
want the generator to create a fresh copy.

## Performance Workspaces

Version 0.24.1 replaces the always-rendered full dashboard with seven lazy
workspaces:

- **Quest** — timer, objective, controls, wellness notice, and new Quest form.
- **Quest Log & Git** — ledger notes, commits, push, and media attachments.
- **Character** — character sheet, equipment, spells, and rules tools.
- **Guild Hall** — Shop, Trading Post, accounts, shared records, and rankings.
- **Rewards & History** — wallet, reports, corrections, and Chronicle history.
- **Audio & Wellness** — playlist, ambience, warning audio, and reminders.
- **Settings** — project and user configuration.

Only the selected workspace performs its GUI and data lookups. Direct shortcuts
are available beneath **Tools > DeverQuest > Workspaces**.

During an active Quest, the Quest and Quest Log workspaces repaint four times
per second. The other workspaces remain event-driven. Git commit observation
uses a lightweight fifteen-second HEAD check; the visible Git panel's Refresh
button still performs an immediate complete status refresh.

## Trading, Rare Loot, and Real Rewards

Version 0.24.0 adds the **Trading Post** beneath Guild Shop and Inventory.
Select another enabled Guild account and offer an eligible item. The item
moves into escrow immediately. The recipient can accept or reject it; the
sender can cancel an open offer or reclaim a rejected one. Bound items and
real-reward redemptions cannot be traded.

Shop Items now configure rarity, binding, and trade eligibility. Equipment,
redemptions, and rare-or-better items receive unique ownership records.
Older inventory remains valid and receives ownership metadata when loaded.

Real-world Shop Items must use the Redemption type. A redemption always
requires leadership approval. Approval reserves and charges the reward;
delivery remains pending until a CEO or Boss uses **Mark Delivered** and may
record a confirmation, order, or ticket reference. This is an administrative
record—not an automatic Discord, merchandise, gift-card, or payroll system.

## Shared Guild Records and Hall of Heroes

Version 0.23.0 adds a folder-backed shared Guild repository. In full view,
expand **Shared Guild Records and Hall of Heroes**. A CEO or Boss should:

1. Enable Shared Guild records.
2. Choose a studio-controlled repository folder.
3. Set the daily ranking cap.
4. Validate the repository.
5. Enable automatic completed-Quest publishing.

Each finalized Quest is written beneath:

`<Guild Repository>/Records/<Account>/<date>/`

The latest public character summary is written beneath:

`<Guild Repository>/Adventurers/`

Publishing occurs after rewards and the local Chronicle have finalized.
**Publish Last Quest** safely retries the most recent session; an existing
session record is not duplicated.

The Hall ranks capped, eligible focus rather than raw hours. Suspiciously long
sessions, high Idle/Unverified ratios, excessive daily Quest frequency, and
records that fail their integrity hash are excluded or flagged. Raw time
remains visible so leadership can review it rather than silently discarding
the underlying work record.

The ranking cap is a safety feature, not a payroll rule. For example, a
ten-hour cap means eleven recorded hours may remain visible while no more than
ten count toward the competitive ranking.

### Authority Boundary

The repository's SHA-256 hash detects accidental edits and simple corruption.
It is not a secret signature. Anyone who can replace every JSON file can also
recompute an unkeyed hash. Treat the repository as authoritative only when the
studio controls its folder permissions and backups. Members should receive
write access only through an approved publishing path or restricted share.

For stronger internet-scale authority, place the same record contract behind a
server/API with authenticated accounts, server-side validation, immutable
storage, and administrator-controlled corrections.

## External Activity and Voice Memos

Version 0.22.0 introduces **External Activity Profiles**. In first-time setup
or Profile settings, choose **Create Aseprite Activity Profile** for the
default pixel-art provider. Select the resulting asset as the Activity Profile.
Additional providers can be added in its Inspector using:

- a display name;
- the executable process name without `.exe`;
- optional text that must appear in the foreground window title;
- the number of seconds for which recent input remains fresh.

On Windows, DeverQuest checks the foreground process. The configured tool must
be foreground and keyboard or pointer input must remain recent. Leaving a tool
open in the background does not qualify. Activity intervals appear in the
External Activity Journal, but they do not add focused time or rewards.

During a Quest, open **External Craft and Voice Chronicle**:

1. Choose a detected microphone.
2. Enter a memo name.
3. Select **Record Voice Memo**.
4. Select **Stop and Attach** when finished.

The WAV file is written beneath:

`DeverQuestTimecards/<Developer>/Media/<date>/`

Use **Attach Existing File** for artwork, reference audio, screenshots, or
other evidence. DeverQuest copies the selected file into the same protected
media folder so the Chronicle does not depend on the original location.
**Unlink** removes the active Quest reference but deliberately does not delete
the copied file.

Microphone access is controlled by the operating system. If Unity reports no
devices, enable microphone permission for Unity Editor and restart the editor.
Recordings are cancelled safely if scripts reload, the Quest is abandoned, or
the editor shuts down.

## Wellness Acknowledgments and Completed Breaks

Version 0.21.1 makes the reminder choices explicit:

- **Acknowledge Only** records that the reminder was seen, advances its
  schedule, and does not claim a break or grant a character benefit.
- **Snooze** delays the reminder without recording a completed action.
- **Take Approved Break** pauses the active Quest and classifies the configured
  duration as Approved Break rather than focused work or Meditation.

Short, meal, and quiet-hours durations plus Completed Break XP are configured
under Wellness settings. Resume after at least 80% of the planned break to
receive its XP and hunger, rest, or happiness benefit. Resuming earlier records
**Break Ended Early** and grants no benefit. Time beyond the approved duration
remains Idle/Unverified.

Opening or revealing a Chronicle is treated as an intentional external action
for ten minutes. This prevents a normal timecard review from triggering the
Unity-focus idle pause or session-aware music controls.

## Audio, Warning Profiles, and Ambience

Version 0.21.0 replaces generic-only beeps with configurable fantasy audio.
Create a **Warning Profile** from the Playlist Player panel, select it, and
assign AudioClips in its Inspector. Separate fields cover idle warnings,
wellness reminders, Focus Stages, attacks, low-HP danger, victories, defeats,
Quest completion, purchases, and level-ups. Empty fields safely fall back to
the existing system beep where appropriate.

Create an **Ambience Profile** for fireplaces, rain, forests, taverns, caves,
or other environmental loops. Add clips in the Inspector, select the Profile,
and use Play/Stop/Next controls in full or compact view. Quest-aware ambience
can begin with a Quest and stop when the Quest ends.

Playlist assets now expose a weight beside each track. Weighted shuffle makes
higher values more likely while still avoiding repeats until the shuffle pool
resets.

Music, ambience, and warnings have separate volume values. DeverQuest uses
Unity's available per-clip Editor preview methods. If a Unity version exposes
only global preview pause/stop controls, the UI displays the existing
compatibility warning and uses the safest available fallback.

## Encounter Profiles and Quest Battles

Version 0.20.0 adds deterministic encounters to completed Focus Stages.
Dungeon Masters create **Monster Profile** assets, build them into
multi-wave **Encounter Profile** assets, and assign an Encounter to a Focus
Stage on its Quest Contract. The fight resolves only after the Stage's real
focused-work requirement is complete.

Monsters define HP, AC, attack modifier, damage dice, victory coin and XP, and
a drop table. Drops may contain extra coin/XP, Equipment, Spells, or Shop
Items. Encounters define waves, counts, bosses, victory bonuses, injury rules,
and whether a defeat may leave the character Fallen.

Combat uses deterministic seeds derived from the Quest session, Focus Stage,
and Adventurer. The active window and generated Chronicle record the result,
round count, HP change, enemies, loot, consequences, seed, and combat log.

Use **Rules Laboratory → Generate Guildhall Training Encounter** to create a
safe two-wave sample. Assign the generated Encounter Profile to a Contract's
Focus Stage, start the Quest, and complete that Stage's focused minutes.

A nonlethal defeat leaves the Adventurer at 1 HP and can add an injury status.
Death-enabled Encounters may leave the character Fallen. The Character Sheet
then offers Guild Shrine resurrection for 50 copper, restoring half HP.

Guaranteed Quest and Focus Stage rewards are never rolled, removed, or reduced
by combat. Encounters award bonus spoils only.

## Guild Shop, Inventory, and Wellness Economy

Version 0.19.0 opens the Guild Quartermaster. Dungeon Masters can create
**Shop Profile** and **Shop Item** assets or use **Generate Starter
Quartermaster** for a ready-to-test stock of food, water, an inn stay, HP and
mana consumables, and sanctioned smoke/privy break permits.

Each Shop Item defines its coin price, minimum level, owned limit, optional
Equipment or Spell, wellness effects, and whether leadership approval is
required. Immediate purchases deduct coin and deliver the item. Restricted
purchases enter the leadership queue and do not deduct coin until approved.
Denied requests preserve their history without charging the Adventurer.

Consumables remain in the active account's inventory until used. Food, drink,
inns, and potions can restore HP, mana, hunger, rest, or happiness. Equipment
is equipped on delivery and Spells are learned. Redemption items can represent
administrator-controlled rewards and receive a final Redeemed state when
claimed.

Break permits require a running Quest. Using one pauses the Quest and
classifies only the configured duration as **Approved Break**. Remaining
paused time beyond the permit becomes **Idle/Unverified**. It never becomes
focused work.

## Parties, Quest Stages, and Character Creation

Version 0.18.0 adds a guided first-login creation screen for Member accounts
whose Adventurer name was intentionally left blank by Guild leadership. Pick
an Adventurer name and class; the class establishes the starting Department,
attributes, HP, mana, class features, and optional matching Starter Loadout.
Existing accounts and progression migrate without being reset.

Quest Contracts may now be solo or Party Quests. Dungeon Masters configure
capacity, an optional reserved Adventurer roster, class and Department
eligibility, authored story, group rewards, and Focus Stages. Joining reserves
a party place. Work begins once the required party is assembled; each
participant turns in independently, while the Contract reaches Submitted only
after the full party has turned in.

Focus Stages are cumulative, role-aware real-work gates. Each Stage may define
an objective, focused minutes, party role, coin, XP, and a reserved future
Encounter identifier. Completed Stages are recorded on both the active Quest
and Contract and never replace or invalidate legitimate focused time.

In **Rules Laboratory**, Dungeon Masters can generate Copper, Bronze, Iron,
and Steel starter Equipment covering Helm, Face, Neck, ears, Shoulders, Back,
Chest, Shirt, Hands, wrists, rings, Belt, Legs, Boots, weapon hands, and
Trinket slots. Create Starter Loadout assets to assign class-specific starting
gear and spells.

Compact view now serves as an active Quest log and character HUD. The Guild
Hall collapses while a Quest is active. Completing a Quest opens the turn-in
wizard without stopping the work clock; tracking ends only when the final
claim is submitted.

## Character Sheet and Rules Engine

Version 0.17.0 automatically gives existing Adventurers a class foundation.
This does not reset level, XP, coin, rank, or identity. The Character Sheet
shows ability scores and modifiers, HP, Armor Class, proficiency, saving
throws, class features, statuses, equipped items, and known spells.

The Rules Laboratory resolves checks from a recorded seed. Use the same
character, seed, ability, proficiency setting, Daily Decree modifier, and DC
to reproduce the same result.

Guild leadership can create Equipment and Spell assets from the Rules
Laboratory. Equipment can add AC or an ability bonus and occupies a defined
slot. Spells record casting ability, level, dice, status effect, and minimum
character level. The Guild Shop will replace direct administrative grants with
the real purchase and inventory loop in Milestone 19.

Daily Decree Campaign Rules define a recommended level, Story/Standard/Heroic/
Mythic difficulty label, and a modifier applied to deterministic checks.

## Guild Accounts and Authority

Version 0.16.0 migrates the existing Adventurer to the founding CEO account.
Open **Guild Accounts and Authority** and secure that account with a local
passcode of at least six characters. Level, XP, coin, class, and character
identity are preserved.

Bosses and CEOs can create accounts. Set a temporary passcode and, for Project
Leaders, provide comma-separated Project names matching Contract Project
values. Members can perform normal Quest input and turn-in. Project Leaders
can manage Contracts and corrections only for their assigned Projects.

Local passcodes are salted and derived rather than stored as plaintext. They
protect DeverQuest UI actions, but do not replace operating-system security or
a future shared Guild identity service.

## Chronicle Integrity and Review

Version 0.15.0 divides long days into numbered Chronicles. Configure automatic
rollover by Quest count or file size in Profile, or use **History and
Reporting → Start New Chronicle** before the next Quest is completed.

History reports each structured Chronicle as:

- **Verified** — the current JSON and correction journal match the latest
  chained seal.
- **Modified** — a sealed record or its hash chain changed.
- **Legacy** — the record predates integrity seals.
- **Unavailable** — verification could not read the required files.

Use **Request Correction** instead of editing a completed Quest. Corrections
append the author, timestamp, reason, and proposed corrected record. Guild
leadership can approve or return pending requests. The original Quest remains
unchanged in every case.

The `.deverquest.json` file is the sealed source record. Markdown is a
regenerated reading copy. Local hashes provide tamper evidence, not independent
authority; shared Guild records are required for that later security boundary.

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

Every completed Quest can also award configurable base coin and XP, ensuring
short intentional Quests still advance the Adventurer. Coin remains earn-only
until the Guild Shop milestone introduces meaningful purchases.

## Adventurer Character Sheet

Configure an Adventurer name, Guild, class, and Guild Rank in setup. Classes
are role-playing identities; Guild Ranks describe workplace authority and are
kept separate. Completing Quests earns XP, automatically advances levels, and
records level-up events in the daily ledger.

Remaining legacy reward balances migrate once at one reward minute per copper.
Historical reward transactions remain visible as legacy minutes.

## Quest Profiles

Project Leaders, Bosses, and CEOs can create unlimited Quest Profile assets
from the Accept Quest panel or through `Assets → Create → DeverQuest → Quest
Profile`. Each profile can define its project, task, department, objective,
suggested focus duration, minimum level, Member availability, base coin and XP,
and work-block payout.

Members must select an available profile for which they meet the minimum level.
Leadership can also accept custom Quests. DeverQuest copies the selected
profile's values into the session when the Quest is accepted, so later asset
edits never rewrite historical payouts or Chronicles.

## Quest Contracts and Assignment Board

A Quest Profile is a reusable template. A Quest Contract is the actual studio
assignment created from that template. Leadership can create Contract assets
from the Accept Quest panel or `Assets → Create → DeverQuest → Quest Contract`.

Contracts include creator, assignee, open-Member availability, minimum level,
priority, due date, deliverables, real work details, snapshotted spoils, and
reserved Encounter fields. The Guild Assignment Board supports Draft, Offered,
Accepted, Active, Submitted, Approved, Returned, and Completed states.

Members select Contracts assigned to their Adventurer or open to any eligible
Member. Starting work makes the Contract Active; Turn-In submits it for
leadership review. Abandonment returns it. Leadership can offer, return,
approve, and complete Contracts from the board.

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

## Compensation Preview Reference

Compensation Preview is optional and disabled by default. A Boss or CEO opens
**Guild Hall > Guild Accounts and Authority > Compensation Preview Policies**,
selects an Adventurer account, and configures:

- hourly rate or annual-salary tracking equivalent;
- three-letter currency display code;
- scheduled weekly hours for the salary equivalent;
- whether completed Approved Break time is eligible; and
- whether only sealed/verified Chronicles qualify, or legacy/unsealed
  Chronicles may also be included.

The Adventurer opens **Rewards & History > Compensation Preview** to review
the current workweek and the active History filter. Meditation and
Idle/Unverified time never qualify. Modified and unavailable Chronicles are
always excluded. Active Quests do not appear until they have been finalized
into Chronicle history. Time matching long/frequent Quest flags remains in the
estimate but is called out for manual review.

Use **Export Filtered Compensation Preview** to write a dedicated CSV planning
statement. The export carries the same non-payroll disclaimer as the window.
Compensation rates are not written into daily timecards, shared Guild
snapshots, or authority-audit details.

This feature is a local planning calculator. It does not perform payroll,
promise payment, calculate deductions or overtime, apply employment law,
provide tax advice, or authorize a payment. The local account preference store
is not encrypted payroll storage. A qualified administrator must review actual
agreements and applicable law outside DeverQuest.

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
