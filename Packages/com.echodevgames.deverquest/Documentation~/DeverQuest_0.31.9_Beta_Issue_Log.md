# DeverQuest 0.31.9 Beta Issue Log

## DQ-0318-031 — Timecard writer fails compilation

**Source build:** 0.31.8 Beta 1  
**Hotfix build:** 0.31.9 Beta 1  
**Severity:** P0 compilation blocker  
**Status:** Patched, awaiting Unity verification

### Reported compiler error

```text
Packages\com.echodevgames.deverquest\Editor\DeverQuestTimecardWriter.cs(669,37): error CS0103: The name 'adventurer' does not exist in the current context
```

### Root cause

`BuildMarkdown()` loaded the current Adventurer for the daily header, but `AppendSession()` did not receive that object. The Battle Chronicle section inside `AppendSession()` attempted to use the out-of-scope local variable while formatting the damage summary.

### Correction

- `BuildMarkdown()` now passes the current `DeverQuestAdventurer` into `AppendSession()`.
- Battle Chronicle rendering uses that explicit report context.
- When the Adventurer or character name is unavailable, the Session developer name is used as a safe fallback.
- No Session schema, rewards, combat results, or Chronicle data are changed.

### Retest

- [ ] Install 0.31.9.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Run Release Readiness.
- [ ] Complete or reopen a Quest containing a Battle Result.
- [ ] Confirm the generated Timecard includes the Battle Chronicle damage report.
- [ ] Confirm no null-reference or missing-name error occurs.
