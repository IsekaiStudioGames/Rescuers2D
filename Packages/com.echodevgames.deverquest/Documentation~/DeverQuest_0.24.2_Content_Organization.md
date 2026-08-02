# DeverQuest 0.24.2 Content Organization

## Production Layout

```text
Assets/DeverQuest/
├── ActivityProfiles/
├── Audio/
│   ├── Ambience/
│   ├── Music/
│   └── WarningProfiles/
├── Characters/
│   ├── Classes/
│   ├── Equipment/
│   ├── Spells/
│   └── StarterLoadouts/
├── Guild/
│   ├── Rewards/
│   ├── ShopItems/
│   └── Shops/
├── Playlists/
├── Quests/
│   ├── Contracts/
│   ├── Encounters/
│   ├── Monsters/
│   └── Profiles/
├── Templates/
└── DemoCampaign/
```

The Classes folder is available for class-specific art, notes, and future
class-definition assets. Current class defaults are represented by Starter
Loadout assets.

## Tutorial Walkthrough

1. Open **Guild Hall > Campaign Content Scaffolding**.
2. Choose **Create Tutorial Campaign**.
3. Inspect `Assets/DeverQuest/DemoCampaign`.
4. Return to the Quest workspace.
5. Select the generated **Trouble in the Tutorial Crypt** Contract if it is
   not already selected.
6. Accept the offered Contract.
7. Complete **Prepare the Expedition** and record a Quest Log note.
8. Complete **Confront the Regression** and make or link a Git commit.
9. Resolve the Tutorial Skeleton encounter.
10. Confirm the Rare **Ring of Focused Embers** enters inventory.
11. Complete the guided turn-in and inspect the Chronicle.
12. Use Guild Hall to test equipping, binding, Shop purchase, trading, and the
    deliberately non-delivering redemption example.

## Safe Regeneration

- Existing assets are loaded and preserved.
- Missing folders and assets are created.
- Existing asset values and references are not reset.
- No production asset is deleted.
- The report states how many folders/assets were created or preserved.

## Validation Checklist

- [ ] Install 0.24.2 and open the Guild Hall workspace.
- [ ] Generate the Empty Studio Structure twice.
- [ ] Confirm the second run does not overwrite edited templates.
- [ ] Generate the Tutorial Campaign twice.
- [ ] Confirm the Contract, Shop, encounter, monster, loot, and loadout links.
- [ ] Confirm the Rare ring has a 100% tutorial drop chance.
- [ ] Confirm the redemption voucher cannot deliver anything automatically.
- [ ] Add AudioClips manually and test playlist, ambience, and warning audio.
- [ ] Complete the full tutorial Quest and verify its Chronicle.
- [ ] Confirm Unity reports no compilation errors or exceptions.
