# DeverQuest 0.32.2 Beta Issue Log
## Monster Profile Asset Association

**Source build:** 0.32.1 Beta 1  
**Patch build:** 0.32.2 Beta 1  
**Unity:** 6000.3.8f1  
**Status:** Patched, awaiting Unity verification

---

## DQ-0321-031 — Generated Monster Profile displays Missing Script

**Type:** ScriptableObject asset association  
**Severity:** P0 tactical-content blocker  
**Status:** Patched in 0.32.2

### Evidence

Selecting a generated tactical Monster Profile displayed:

```text
Script: None (Mono Script)
The associated script can not be loaded:
EchoDevGames.DeverQuest.Runtime:
EchoDevGames.DeverQuest:
DeverQuestMonsterProfile
```

### Root cause

`DeverQuestMonsterProfile` was declared inside:

```text
Runtime/DeverQuestEncounterAssets.cs
```

rather than a source file whose name matched the ScriptableObject class. Unity could compile the type, but generated `.asset` files could lose or fail to resolve their `MonoScript` association.

### 0.32.2 correction

- Moved the class to:

```text
Runtime/DeverQuestMonsterProfile.cs
```

- Added a stable package `.meta` GUID for the standalone script.
- Preserved `DeverQuestDropEntry`, `DeverQuestEncounterWave`, and encounter enums in the original shared data file.
- Added a targeted migration that scans project-owned `.asset` files for the Monster Profile serialization fingerprint.
- Rewrites only assets that match the Monster schema and cannot currently load as `DeverQuestMonsterProfile`.
- Writes the original YAML to:

```text
Library/DeverQuest/Migrations/0.32.2/
```

before changing the script reference.
- Reimports and verifies every repaired asset.
- Restores the original YAML when verification fails.
- Added the manual command:

```text
Tools > DeverQuest > QA > Repair Monster Profile Asset Scripts
```

### Data-safety boundary

The migration does not regenerate Monster IDs, statistics, drops, abilities, names, descriptions, or Encounter references. It only replaces the broken serialized `m_Script` reference.

### Retest

1. Install 0.32.2.
2. Let Unity finish compiling and importing.
3. Check the Console for the migration summary.
4. Select `Training_Rat`, `Goblin_Foreman`, and other generated Monster Profile assets.
5. Confirm normal Monster fields appear instead of `None (Mono Script)`.
6. Confirm existing Monster IDs and authored values remain unchanged.
7. Open attached Encounter Profiles and confirm their wave Monster references resolve.
8. Restart Unity and verify the assets remain valid.
9. Run Beta Administration validation.
10. Run Release Readiness.

### Expected result

All generated Monster Profile assets retain their data and load through the standalone `DeverQuestMonsterProfile` script. Tactical content may then be validated normally.

---

## Current verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
