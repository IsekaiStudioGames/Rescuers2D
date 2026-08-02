# DeverQuest 0.27.0 — Pets and Companions

Milestone 27 turns the Companion metadata introduced in the identity catalog
into a persistent, deterministic RPG system.

## Generate the original Stable

1. Install `com.echodevgames.deverquest-0.27.0.tgz`.
2. Generate the original Identity Catalog if it is not already present.
3. Open **Guild Hall > Campaign Content Scaffolding**.
4. Select **Generate Original Companion Stable**.

The generator creates five original profiles:

- **Gravebound Wisp** — Necromancer Controller;
- **Trailclaw Lynx** — Ranger/Wildwarden Striker;
- **Verdant Mote** — Druid Support;
- **Ancestor Echo** — Shaman Support; and
- **Brasswing Sentry** — a purchasable Guardian available beyond
  Companion-specialist Classes.

Existing assets are preserved. Rerunning the generator reconnects starter
references and adds missing profiles to the Catalog without deleting custom
entries.

## Authoring a Companion

Create **DeverQuest > Companions > Companion Profile** from Unity's asset
menu. Configure:

- allowed Class IDs and/or legacy Class names;
- whether the profile requires a Companion-enabled Class;
- minimum level and optional recruitment price;
- Kind, Role, Creature Type, HP, AC, and attack modifier;
- damage dice and typed damage;
- defensive affinities; and
- loyalty and recovery cost.

Assign a profile to a Class Definition's **Starter Companion** field to grant
it during first-login character creation. Existing Adventurers recruit through
the Character workspace.

## Persistence

Companion state is stored with the Adventurer and copied into the authenticated
Guild account:

- durable instance and profile identity;
- custom name;
- active/fallen state;
- HP, XP, level, and loyalty;
- battles and victories; and
- recruitment timestamp.

Version 0.27.0 migrates older account collections to schema 7. Missing lists
become empty; no existing identity, character, economy, inventory, Contract,
timecard, or Chronicle data is recalculated.

## Encounter behavior

The active Companion acts after the Adventurer and before the enemy. Its
attack uses the Companion Profile's dice and Milestone 26 Damage Type.
Monster affinities apply normally.

Enemy target selection is deterministic. Guardians intercept more often than
other Roles. Companion affinities apply when they are hit. A Companion at zero
HP falls, becomes inactive, and must recover at the Stable. Companion defeat
does not erase the profile or permanently kill the Companion.

Surviving a victorious encounter grants half the encounter XP to the
Companion. Companion leveling uses a separate fifty-XP-per-level curve.
Companion XP is not Adventurer XP and never represents work performed.

## Unity validation checklist

- [ ] Confirm **Tools > DeverQuest > Developer Companion** appears.
- [ ] Generate the original Identity Catalog and Companion Stable.
- [ ] Confirm five original Companion Profiles and one Catalog were created.
- [ ] Sign in as a Companion-enabled Class and recruit its eligible profile.
- [ ] Rename it, dismiss it, and set it active again.
- [ ] Confirm ineligible Class/Profile combinations explain why recruitment
  is unavailable.
- [ ] Start an encounter and confirm the active Companion receives a turn.
- [ ] Confirm Striker damage, Guardian interception, Support healing, or
  Controller attack reduction appears in the Battle Chronicle.
- [ ] Confirm Milestone 26 typed damage and affinities apply to Companion
  attacks and incoming damage.
- [ ] Let a Companion fall, then recover it from the Stable.
- [ ] Confirm HP, loyalty, XP, level, battles, and victories survive an Editor
  restart and account switch.
- [ ] Finalize the Quest and confirm Companion results appear in the timecard.
- [ ] Confirm focused-work totals and work rewards are unchanged by Companion
  turns.
- [ ] Switch among workspaces during a live Quest and confirm there is no
  continuous AssetDatabase scanning or new slowdown.

## Commercial-content boundary

The shipped profile names and lore are original DeverQuest content. Companion
categories and generic fantasy vocabulary are not tied to a third-party game
setting. Guilds may author private content they have permission to use.

## Next milestone

Milestone 28 is Compensation Preview: optional salary/hourly estimates based
on verified time, clearly separated from payroll, promises, and automatic
payment.
