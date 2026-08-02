# DeverQuest 0.32.5 Beta Issue Log
## Distribution Compilation Hotfix

**Source build:** 0.32.4 Beta 1  
**Patch build:** 0.32.5 Beta 1  
**Severity:** P0 clean-install compilation blocker  
**Status:** Patched; awaiting clean-project verification

---

## DQ-0324-031 — CompressionLevel type is ambiguous in a clean project

### Report

A clean import into Rescuers2D failed with:

```text
DeverQuestDistributionService.cs(920,24): error CS0104:
'CompressionLevel' is an ambiguous reference between
'System.IO.Compression.CompressionLevel' and
'UnityEngine.CompressionLevel'
```

### Cause

`DeverQuestDistributionService.cs` imports both:

- `System.IO.Compression`
- `UnityEngine`

The tarball exporter used the unqualified expression:

```csharp
CompressionLevel.Optimal
```

Unity 6 exposes `UnityEngine.CompressionLevel`, so a full clean compile cannot determine which enum is intended.

### Correction

The exporter now uses:

```csharp
System.IO.Compression.CompressionLevel.Optimal
```

### Impact

- No saved data changes.
- No Quest, reward, Chronicle, Guild, character, audio, or content behavior changes.
- The 0.32.4 tarball should be withdrawn from tester distribution.
- Testers should install 0.32.5 instead.
- The existing 0.32.4 tester handbook and checklists remain valid aside from the package-version label.

### Retest

- [ ] Remove or replace 0.32.4.
- [ ] Install 0.32.5 in a clean Unity project.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Run Release Readiness.
- [ ] Open Packaging & Distribution.
- [ ] Export a verified tarball.
- [ ] Confirm tarball verification passes.
