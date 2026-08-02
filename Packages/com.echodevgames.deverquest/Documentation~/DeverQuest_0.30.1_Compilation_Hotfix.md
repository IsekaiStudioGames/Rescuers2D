# DeverQuest 0.30.1 - Compilation Hotfix

Version 0.30.1 is a source-compatibility hotfix for the 0.30 release candidate.
It does not add gameplay or productivity features.

## Corrected compiler failures

- C# 9 multiline interpolation failures in Contract eligibility, the compact
  Adventurer display, Battle Chronicle UI, and markdown timecards.
- Generic inference failures in every Tactical Starter Kit `Upsert` call.
- A misplaced Survival escape block that referenced an unavailable `battle`
  variable.
- Ambiguous `Object` references in the Rules Laboratory content generator UI.

## Verification pass

1. Install the 0.30.1 tarball through Unity Package Manager.
2. Allow Unity to finish its script reload.
3. Confirm that the Console contains no red compiler errors.
4. Run **Tools > DeverQuest > Run Release Readiness Check**.
5. Generate the Tactical Starter Kit from the Rules Laboratory.
6. Run the 0.30.0 release-candidate regression checklist, especially the music
   transport stress test and Survival escape flow.
