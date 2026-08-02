# DeverQuest 0.32.3 Beta Issue Log
## Companion Profile Asset Association Hotfix

**Source build:** 0.32.2 Beta 1  
**Patch build:** 0.32.3 Beta 1  
**Unity:** 6000.3.8f1  
**Readiness baseline:** 23 passed, 2 advisories, 0 blockers  
**Status:** Patched, awaiting Unity verification

---

## DQ-0322-031 — Companion Profiles are created with Missing Script

**Type:** ScriptableObject asset association  
**Severity:** P0 tactical-content blocker  
**Status:** Patched in 0.32.3

### Evidence

- The Original Guild Companion Catalog remained empty after generating five starter Companion Profiles.
- A manually created `NewCompanionProfile.asset` displayed `None (Mono Script)`.
- Unity reported:

```text
Type cannot be found:
EchoDevGames.DeverQuest.DeverQuestCompanionProfile.
Containing file and class name must match.
```

### Root cause

`DeverQuestCompanionProfile` was declared inside:

```text
Runtime/DeverQuestCompanionAssets.cs
```

Unity compiled the C# type, but standalone Companion Profile assets could not retain a valid `MonoScript` association because the source filename did not match the ScriptableObject class name.

### 0.32.3 correction

- Moved `DeverQuestCompanionProfile` into:

```text
Runtime/DeverQuestCompanionProfile.cs
```

- Added stable package metadata for the new source file.
- Added automatic migration for unmistakable broken Companion Profile YAML assets.
- Added manual repair command:

```text
Tools > DeverQuest > QA > Repair Companion Profile Asset Scripts
```

- Writes backups under:

```text
Library/DeverQuest/Migrations/0.32.3/
```

- Reconnects repaired Original Starter profiles to the Original Guild Companion Catalog through the idempotent starter generator.
- Added `DQ-CONTENT-404` when a broken Companion Profile remains unresolved.

### Preserved data

The migration preserves serialized Companion data, including:

- Companion ID
- Display name and lore
- Kind and role
- Creature type
- Class restrictions
- Hit Points and Armor Class
- Attack values and damage type
- Damage affinities
- Loyalty
- Recruitment and recovery costs

### Retest

- [ ] Install 0.32.3.
- [ ] Allow Unity to finish compiling and importing.
- [ ] Review the automatic migration summary in Console.
- [ ] Select `NewCompanionProfile.asset`.
- [ ] Confirm normal Companion fields appear.
- [ ] Select all five profiles under `Assets/DeverQuest/Companions/OriginalStarter/Profiles/`.
- [ ] Confirm none shows Missing Script.
- [ ] Select `Original_Guild_Companion_Catalog.asset`.
- [ ] Confirm it contains five Companion references.
- [ ] Open Character > Companion Stable.
- [ ] Confirm profiles can be selected and recruited when eligible.
- [ ] Restart Unity and inspect the assets again.
- [ ] Run Full Validation.
- [ ] Run Release Readiness.
- [ ] Confirm Tactical test content clears when the other required tactical assets exist.
- [ ] Confirm Beta content health has no `DQ-CONTENT-404` error.

### Manual fallback

If automatic migration does not run:

1. Run **Tools > DeverQuest > QA > Repair Companion Profile Asset Scripts**.
2. Open **Guild Hall > Campaign Content Scaffolding**.
3. Run **Generate Original Companion Stable** once.
4. Rerun Full Validation and Release Readiness.

---

## Current verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
