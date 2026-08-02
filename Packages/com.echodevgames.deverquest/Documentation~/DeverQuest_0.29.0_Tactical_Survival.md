# DeverQuest 0.29.0 — Tactical Abilities and Survival Quests

Milestone 29 turns Focus Stages into auditable development objectives and
tabletop-style tactical encounters. All shipped names and lore are original;
the package does not claim compatibility with or affiliation to any
third-party tabletop or video-game property.

## Generate the starter kit

Open **Tools > DeverQuest**, sign in with a Guild account that can manage the
Guild, then open **Settings > Rules Laboratory** and choose:

**Generate Tactical Starter Kit + Quest Templates**

The re-runnable generator creates:

- original spells covering direct damage, healing, damage over time, root,
  snare, life drain, and a homeward return;
- martial attack techniques and class-linked Ability Profiles;
- a monster Ability Profile with an ongoing poison condition;
- **Fifteen-Minute Skirmish**, a one-stage pace template;
- **Wayfarer Survival Expedition**, an endless-wave template;
- weighted salvage for testing encumbrance.

Assets are placed under `Assets/DeverQuest/Tactical`. Existing assets are
updated in place rather than duplicated.

## Class Ability Profiles

A Class Definition can reference a `DeverQuestAbilityProfile`. Its ordered
slots select known Spells or Attack Techniques according to priority, Hit
Point threshold, mana, cooldown, and whether an effect is already being
maintained.

Supported effect building blocks include:

- direct damage and damage over time;
- healing, healing over time, life drain, and mana restoration;
- root, snare, stun, silence, shields, attack/armor modifiers;
- cleanse, dispel, and return-to-Guild effects.

Legacy Spells still work. A legacy damage die becomes direct damage, and its
old status text is adapted as a short snare until the asset is upgraded.

## Cascading Focus pace

Each Focus Stage owns its own clock. When one Stage finishes, the next Stage
starts at that exact Focus time.

- Reaching the configured Focus target records normal Stage rewards.
- **Report Development Objective Complete** turns in the current Stage early
  and grants its separately configured early-completion coin and XP.
- The following Stage begins immediately, so pace bonuses cascade through the
  whole Quest without stealing time from later encounters.
- Encounter `parRounds` is independent of development pace. Beating par grants
  the Encounter's early-victory coin and XP.

This intentionally rewards both efficient development and a strong character
build without treating them as the same achievement.

## Survival expeditions

A Survival Encounter resolves one wave after each configured Focus interval.
Difficulty and wave rewards grow at configurable rates. The Chronicle records
the wave number, deterministic action log, typed damage, par result, loot,
and safety outcome.

An Adventurer can leave by:

- succeeding on an Agility-based **Attempt Flee** check;
- using a prepared `ReturnToGuild` ability such as Homeward Sigil;
- taking the **Guild Wagon** at its configured checkpoint.

Completing the Unity work session first attempts the safest available exit.
The session cannot silently finalize a stranded Survival Stage after a failed
flee check.

## Health and encumbrance safety

The encounter profile defines a low-HP safety threshold. At or below it,
DeverQuest plays the configured Encounter Danger cue and pauses before another
enemy turn. Character death is never silently advanced while the developer is
working.

Equipment, shop inventory, loot, and coin pieces contribute to carried weight.
When over capacity, Survival combat pauses. Drop inventory from the Survival
panel or Guild Shop before continuing.

Coin exchange only occurs at the Guild Hall:

- 100 copper = 1 silver
- 100 silver = 1 gold
- 100 gold = 1 platinum

Rewards arrive as loose copper pieces. **Exchange Coin Denominations at Guild
Hall** reduces the physical piece count without changing purse value.

## Meditation rule

Meditation remains freely available between encounters. A safety-paused
Survival fight must first meet its recovery and carry requirements before it
can resume. Leaving the expedition itself requires a flee check, a prepared
homeward ability, or the Guild wagon.

## Upgrade and compatibility

The package migrates existing local Adventurer and Guild account coin balances
into denomination fields without changing value. Old sessions, Spells,
Encounters, Contracts, inventory, and Battle Chronicle entries remain
readable. New fields use safe defaults.
