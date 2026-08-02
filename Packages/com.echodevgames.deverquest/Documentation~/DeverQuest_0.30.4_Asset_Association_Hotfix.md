# DeverQuest 0.30.4 Beta Asset Association Hotfix

## Confirmed failure

DQ-0302-002 failed Unity verification in 0.30.3. The Ambience Profile field
was correctly filtered, but the created asset displayed **Missing Script** and
the object picker contained no valid `DeverQuestAmbienceProfile` assets.

The runtime type was the second ScriptableObject declared inside
`DeverQuestAudioProfiles.cs`. Unity compiled the C# type, but did not expose a
standalone script asset that could back the created profile.

## Correction

`DeverQuestAmbienceProfile` now lives in
`Runtime/DeverQuestAmbienceProfile.cs`. The same source-layout audit corrected
secondary independently creatable asset types for Ability Profiles, Spells,
Companion Catalogs, Encounter Profiles, Shop Profiles, and the starter Identity
Catalog family.

## Required cleanup after 0.30.3

A ScriptableObject asset already saved with **Missing Script** will not become
valid merely because the class now has a proper source file.

1. Install 0.30.4 and allow Unity to compile.
2. Delete the broken Ambience Profile asset created under 0.30.3.
3. Create a new Ambience Profile.
4. Assign ambience clips and save the project.
5. For a partial starter Identity generation, delete
   `Assets/DeverQuest/IdentityCatalogs/OriginalStarter`.
6. Run **Create Original Starter Identity Catalog** again.

Do not delete working Warning Profiles. Their original source file remains in
place to preserve that established asset path.

## Verification

- A new Ambience Profile Inspector shows its serialized fields instead of
  `None (Mono Script)`.
- The DeverQuest Ambience Profile picker lists the new asset.
- Assignment survives deselection, script reload, and Unity restart.
- The starter Identity Catalog creates Ancestries, Classes, Faiths, and the
  Catalog without Missing Script assets.
