# DeverQuest 0.25.0 — Identity Catalogs

## Outcome

Milestone 25 makes the Adventurer identity layer data-driven. Ancestries,
Classes, Faiths, Alignment, and Guild catalog membership are reusable Unity
assets instead of a closed list embedded in the editor window.

## Generate the Starter Catalog

1. Sign in as an authenticated CEO or Boss.
2. Open **Tools > DeverQuest > Developer Companion**.
3. Select **Guild Hall**.
4. Expand **Campaign Content Scaffolding**.
5. Choose **Generate Original Starter Identity Catalog**.

Assets are created beneath:

`Assets/DeverQuest/IdentityCatalogs/OriginalStarter`

The operation is safe to repeat. Existing assets at the generated paths are
preserved. The generated catalog becomes the active Guild Identity Catalog.
Leadership can replace it using the **Active Identity Catalog** field in the
same panel; Members can only create characters from identities included in
that active catalog. The selection is stored in the project asset
`Assets/DeverQuest/IdentityCatalogs/GuildIdentityRegistry.asset`, so it can be
reviewed and committed with the rest of the Guild's Unity content.

## Asset Types

### Ancestry

An Ancestry defines whether it is playable and sapient, its size and movement,
ability adjustments, natural bonuses, languages, innate traits, and optional
eligible or restricted Class IDs.

### Class Definition

A Class Definition provides its Department, primary ability, hit die, Mana
use, starting attributes, saving-throw proficiencies, features, and optional
companion-tradition metadata. Companion mechanics are intentionally not
simulated until Milestone 27.

### Faith

A Faith defines original lore, its Alignment, allowed follower Alignments,
domains, favored Classes, restricted Ancestries, and an optional granted
trait.

### Identity Catalog

An Identity Catalog groups the Ancestries, Classes, and Faiths a Guild wants
to use and establishes default creation selections.

Quest Contracts can use the resulting Class Definition and Ancestry assets as
eligibility restrictions. The previous string-based Class list remains
readable for backward compatibility.

## Stable IDs and Migration

Each identity asset owns a durable generated ID. Guild accounts and
Adventurer records store both that ID and a display-name snapshot. Display
names can therefore be edited without disconnecting an established
character.

When 0.25.0 loads older data:

- matching legacy Class names gain their stable Class ID;
- missing Ancestry and Faith references can adopt generated catalog defaults;
- existing stats and progression are not rerolled;
- coin, inventory, equipment, spells, permissions, and Chronicles remain
  unchanged.

## Original Starter Content

The generator ships original DeverQuest identities such as Freefolk,
Stonekin, Mirekin, Ashscale, Moonclaw, High Scholar, and Northlander. It does
not ship third-party game-specific races, brands, art, audio, or copied lore.

Private Guilds can author custom catalog assets. The studio distributing those
assets is responsible for confirming that it has the required permissions and
licenses.

## Verification Pass

1. Generate the original starter catalog twice and confirm the second run
   reports preserved assets.
2. Create a fresh Member account without an Adventurer name.
3. Sign in and confirm the guided identity creator appears.
4. Select an Ancestry, Class, Alignment, and Faith.
5. Confirm invalid eligibility combinations disable Guild entry.
6. Create the Adventurer and confirm the Character workspace shows Ancestry,
   Class, Alignment, Faith, Department, traits, languages, attributes, HP,
   Mana, and companion-tradition metadata where applicable.
7. Finalize a short Quest and confirm the Markdown timecard includes Ancestry,
   Alignment, and Faith.
8. Reopen an existing legacy account and confirm its progression, coin,
   inventory, equipment, spells, and Chronicles remain intact.

## Deliberate Boundaries

- Milestone 26: creature types, damage types, and elemental resistances.
- Milestone 27: pets, minions, familiars, and companion simulation.
- Milestone 28: optional compensation preview, kept separate from payroll.
