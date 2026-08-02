# DeverQuest 0.26.0 — Combat Typing

Milestone 26 introduces reusable creature families, typed damage, and
defensive affinities while keeping every combat roll deterministic and
reviewable.

## Included combat vocabulary

Creature Types are generic fantasy families such as Humanoid, Beast, Undead,
Construct, Dragonkin, Elemental, Spirit, and Insectoid. Damage Types cover
physical attacks and original generic magical categories: Bludgeoning,
Piercing, Slashing, Fire, Frost, Lightning, Acid, Poison, Arcane, Radiant,
Shadow, Psychic, Sonic, and Force.

The package intentionally does not ship game-specific creature names, lore,
art, rules text, or branded settings. A Guild may create private content it
has permission to use.

## Authoring

1. Open **Tools > DeverQuest > Developer Companion**.
2. Select the **Character** workspace.
3. In **Rules Laboratory**, select **Generate Guild Combat Codex**.
4. Create or select Equipment, Spell, Monster Profile, and Ancestry assets.
5. Assign attack Damage Types and add affinity entries in the Inspector.
6. Put the Monster into an Encounter and assign that Encounter to a completed
   Focus Stage.

The organized studio generator also creates:

- `Assets/DeverQuest/Combat/Codices`
- `Assets/DeverQuest/Templates/CombatTypeCatalog_Template.asset`

## Resolution order

For a matching incoming Damage Type:

1. **Absorbs** prevents damage and heals the target by the raw amount, capped
   at maximum HP.
2. **Immune** prevents the damage.
3. **Resistant** halves damage, rounded up.
4. **Vulnerable** doubles damage.
5. Resistance and vulnerability together cancel to normal damage.

Repeated entries do not stack. This lets an Ancestry and equipped item name
the same resistance without creating exponential defense.

The Adventurer attacks with an equipped Main Hand item that has damage dice.
If none exists, DeverQuest chooses the first known damaging Spell, then falls
back to the original Guild Strike. Existing assets therefore continue to
resolve without reauthoring.

## Chronicle evidence

Every hit records:

- combat round;
- source and target;
- Damage Type;
- raw damage;
- applied response;
- final damage; and
- absorbed healing.

The live Battle Chronicle shows typed totals. The generated timecard contains
the typed summary and human-readable round log. The encounter seed remains
unchanged, so equivalent old data continues to be deterministic.

## Unity validation checklist

- [ ] Install `com.echodevgames.deverquest-0.26.0.tgz`.
- [ ] Confirm **Tools > DeverQuest > Developer Companion** appears.
- [ ] Generate the Guild Combat Codex.
- [ ] Confirm all Creature Types and Damage Types appear in its Inspector.
- [ ] Generate the tutorial campaign in a clean test project.
- [ ] Equip the tutorial Grave Wand and resistant ring.
- [ ] Confirm the Character Sheet lists the Fire resistance.
- [ ] Resolve the Tutorial Crypt encounter.
- [ ] Confirm the Skeleton is identified as Undead.
- [ ] Confirm Shadow, Bludgeoning, or other configured typed damage appears in
  the Battle Chronicle.
- [ ] Test one Vulnerable, Resistant, Immune, and Absorbs affinity.
- [ ] Confirm raw and final values remain in the finalized timecard.
- [ ] Open an older Chronicle and confirm it remains readable.
- [ ] Switch between workspaces during a running Quest and confirm Unity
  responsiveness remains consistent with 0.24.1 or better.

## Milestone boundary

This release adds the data and resolution layer only. Persistent summoned
pets, animal companions, companion equipment, and companion turns remain
Milestone 27. Compensation Preview remains Milestone 28.
