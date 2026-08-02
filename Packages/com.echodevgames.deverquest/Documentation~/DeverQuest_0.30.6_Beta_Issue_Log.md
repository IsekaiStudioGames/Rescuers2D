# DeverQuest 0.30.6 Beta Issue Log
## Starter Identity Catalog Activation Fix

**Date:** 2026-07-31  
**Observed build:** 0.30.5 Beta 1  
**Patch build:** 0.30.6 Beta 1  
**Unity:** 6000.3.8f1  
**Tester profile:** EchoDev  
**Overall status:** Patch prepared, Unity retest required

---

## DQ-0302-004 — Original Starter Identity Catalog generation throws an error

**Type:** Asset generation / onboarding  
**Severity:** Beta blocker for new-character onboarding  
**Status:** Reopened from Unity test; patched in 0.30.6  

### Test result under 0.30.5

The starter content assets were generated, but activation did not complete.
Release Readiness therefore continued to report:

> No complete active Identity Catalog was found.

### Primary exception

```text
ArgumentNullException: Value cannot be null.
Parameter name: target
UnityEditor.EditorUtility.SetDirty(UnityEngine.Object target)
DeverQuestIdentityCatalogService.EnsureRegistry()
DeverQuestIdentityCatalogService.SetActiveCatalog(...)
DeverQuestIdentityCatalogGenerator.GenerateOriginalStarterCatalog()
```

### Secondary GUI error

```text
GUI Error: Invalid GUILayout state in DeverQuestWindow view.
Verify that all layout Begin/End calls match.
```

The GUILayout message was a cascading error. The generator threw while Unity was
inside the DeverQuest IMGUI draw event, interrupting the normal completion of the
layout pass.

### Root cause

An invalid or Missing Script `GuildIdentityRegistry.asset` could already occupy:

```text
Assets/DeverQuest/IdentityCatalogs/GuildIdentityRegistry.asset
```

The service loaded no valid `DeverQuestIdentityCatalogRegistry` from that path,
created a temporary registry object, then attempted to create a new asset at the
occupied path. Unity did not produce a usable registry object, and the following
`EditorUtility.SetDirty(registry)` call received null.

### 0.30.6 correction

- Detect an incompatible asset occupying the canonical registry path.
- Delete only that invalid registry asset.
- Create a fresh `DeverQuestIdentityCatalogRegistry`.
- Save, synchronously import, and reload the asset before using it.
- Throw a descriptive error if Unity still cannot load the new registry.
- Defer Guild Hall starter-catalog generation through `EditorApplication.delayCall`.
- Catch active-catalog assignment errors inside the window so an asset failure
  cannot unbalance Unity's IMGUI layout.
- Update the readiness advisory with the exact Guild Hall navigation path.

### Existing data handling

Do **not** delete the generated Original Starter folder before the first retest.
The generator is intended to preserve and reuse the Ancestry, Class, Faith, and
Catalog assets created by the successful portion of the 0.30.5 attempt.

Only the invalid registry at the canonical registry path is automatically
replaced by the patch.

### Focused retest

1. Install `com.echodevgames.deverquest-0.30.6.tgz`.
2. Confirm Package Manager reports **0.30.6**.
3. Clear the Unity Console.
4. Open **Guild Hall > Campaign Content Scaffolding**.
5. Click **Generate Original Starter Identity Catalog** once.
6. Wait for the queued generation operation to finish.
7. Confirm no `ArgumentNullException` occurs.
8. Confirm no `Invalid GUILayout state` message occurs.
9. Confirm the Active Identity Catalog field contains
   `Original Guild Identity Catalog`.
10. Inspect the generated Catalog, Ancestry, Class, and Faith assets.
11. Confirm none shows Missing Script.
12. Restart Unity.
13. Confirm the active Catalog remains assigned.
14. Run Release Readiness again.

### Expected readiness result

If the Contract Spoils advisory remains unresolved, the expected report is:

```text
12 passed, 1 advisory, 0 blockers
```

After the Contract Spoils mismatch is also resolved, the target is:

```text
13 passed, 0 advisories, 0 blockers
```

### Acceptance criteria

- Starter generation completes without exceptions.
- No GUILayout-state error is produced.
- The canonical registry is a valid ScriptableObject asset.
- The Original Guild Identity Catalog becomes active.
- The active assignment survives Unity restart.
- New-character onboarding can read the Catalog.
- Release Readiness clears the Starter Identity Catalog advisory.

---

## DQ-0305-008 — Generator discoverability

**Type:** UI / onboarding  
**Severity:** Minor  
**Status:** Partially addressed in 0.30.6

### Correction

The Release Readiness advisory now states the exact path:

```text
Guild Hall > Campaign Content Scaffolding
```

A Character-workspace shortcut remains a later usability improvement and is not
required to verify the current stabilization patch.

---

## Current verdict

**0.30.5:** Starter Identity Catalog test failed during activation.  
**0.30.6:** Fix prepared; awaiting Unity verification.
