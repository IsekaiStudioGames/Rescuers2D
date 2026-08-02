# DeverQuest 0.30.5 Beta Issue Log

**Conversation date:** 2026-07-31  
**Evidence build visible in screenshot:** 0.30.3 Custom Package  
**Unity version visible in screenshot:** Unity 6.3 LTS (6000.3.8f1)  
**Patch prepared:** 0.30.5 Beta 1

## Current issue table

| Issue | Summary | Severity | Patch status | Unity verification |
|---|---|---:|---|---|
| DQ-0302-002 | Ambience Profile becomes Missing Script after creation/selection changes | P0 | Reopened; package identity fix prepared in 0.30.5 | Pending |
| DQ-0302-003 | Selected Quest displayed default Spoils instead of effective values | P0 | Patched in 0.30.3 | Pending |
| DQ-0302-004 | Original Starter Identity Catalog generation threw an error | P0 | Generator and asset layout patched in 0.30.3–0.30.4; stable package metadata added in 0.30.5 | Pending |
| DQ-0304-005 | Music and Ambience are mutually exclusive and share global Stop behavior | P1 | Independent logical channels prepared in 0.30.5 | Pending |

---

# DQ-0302-002 — Ambience Profile returns to Missing Script

## Report

An Ambience Profile can initially be created and assigned, but after modifying or selecting other content and returning to the profile, its Inspector displays:

> The associated script can not be loaded.

The Ambience Profile field then reports `Missing (Dever Quest Ambience Profile)`.

## Environment

- **Reported build:** Package Manager visibly reports 0.30.3, not 0.30.4.
- **Unity:** 6.3 LTS, 6000.3.8f1.
- **Frequency:** Reproduced by the tester during the current Beta pass.
- **Severity:** P0 release blocker.
- **Data risk:** The affected ScriptableObject can no longer be edited or loaded as `DeverQuestAmbienceProfile`.

## Reproduction steps

1. Open DeverQuest.
2. Create an Ambience Profile.
3. Assign ambience clips and select the profile in the player.
4. Select or modify another asset.
5. Return to the Ambience Profile.
6. Inspect the asset and the DeverQuest Ambience Profile field.

## Expected result

- The asset remains associated with `DeverQuestAmbienceProfile`.
- Its clips and settings remain editable.
- The assigned profile survives selection changes, script reloads, editor restarts, and package updates.

## Actual result

- The asset displays `None (Mono Script)` and a missing-script warning.
- DeverQuest displays `Missing (Dever Quest Ambience Profile)`.
- A `NullReferenceException` is also visible in the screenshot’s Console status bar.

## Evidence

- Screenshot supplied 2026-07-31.
- Package Manager card in the screenshot reports version 0.30.3.
- Inspector identifies the missing class as `EchoDevGames.DeverQuest.Runtime:EchoDevGames.DeverQuest:DeverQuestAmbienceProfile`.

## Root cause

Two package-state problems were identified:

1. The screenshot confirms that 0.30.3 was still the active package during the test, so the standalone-class correction from 0.30.4 was not active.
2. The newly separated ScriptableObject source files did not yet ship with deliberate Unity `.meta` identities. That made their package asset identity dependent on import/cache state instead of a version-controlled GUID.

## 0.30.5 correction

- `DeverQuestAmbienceProfile` remains in its own matching source file.
- Stable deterministic `.meta` files are now included for the ScriptableObject files newly separated during the 0.30.4 correction, including Ambience, Identity, Ability, Spell, Companion Catalog, Encounter Profile, and Shop Profile types.
- Established DeverQuest scripts were not assigned replacement GUIDs during this Beta patch, avoiding an uncontrolled migration of existing assets.
- Future package versions must preserve the new `.meta` files and their GUID values. A complete repository-wide metadata audit should be performed only after the current embedded-package metadata is captured and backed up.
- Package version and Release Readiness expectations are updated to 0.30.5.

## Required one-time cleanup

An asset already showing Missing Script cannot be trusted or automatically repaired without its former script GUID.

1. Confirm Package Manager visibly reports 0.30.5.
2. Delete the broken Ambience Profile asset.
3. Create a fresh Ambience Profile under 0.30.5.
4. Reassign its clips and select it in DeverQuest.
5. Save the project.

## Verification checklist

- [ ] Package Manager reports 0.30.5.
- [ ] A new Ambience Profile shows normal Inspector fields.
- [ ] Add at least two ambience clips.
- [ ] Assign the profile in DeverQuest.
- [ ] Select and edit another asset.
- [ ] Return to the profile; no Missing Script warning appears.
- [ ] Trigger a script recompile; the asset remains valid.
- [ ] Restart Unity; the assignment and clips remain valid.
- [ ] Reimport the package; the asset remains valid.
- [ ] Run Release Readiness Check; no package-identity warning appears.

## Status

**Patched in source, pending Unity verification.**

---

# DQ-0304-005 — Music and Ambience are not independent

## Report

Music and Ambience do not play together. Starting one replaces the other, and their Stop behavior appears shared.

## Environment

- **Reported build:** 0.30.3 visible in Package Manager.
- **Unity:** 6.3 LTS, 6000.3.8f1.
- **Frequency:** Consistent with the 0.30.x single-owner implementation.
- **Severity:** P1 Beta usability defect.

## Reproduction steps

1. Select a playlist and play music.
2. Select a valid Ambience Profile and choose Play Ambience.
3. Observe the music state and audible output.
4. Start music again.
5. Use either Stop control.

## Expected result

- Music and Ambience can play simultaneously.
- Music Play, Pause, Next, Previous, and Stop affect only Music.
- Ambience Play, Next, and Stop affect only Ambience.
- Warning cues may play over both.
- Rapid controls do not leave abandoned clips playing.

## Actual result

- The 0.30.x bridge design allowed only one long-form primary clip.
- Starting Music released Ambience.
- Starting Ambience released Music.
- Unity’s global native preview Stop operation made both controls appear connected.

## Root cause

This behavior was intentional in 0.30.0 as an emergency correction for the earlier ghost-track bug. Unity’s internal preview transport exposes global stop behavior, so DeverQuest treated Music or Ambience as one primary owner. That stopped unwanted stacking but also removed the intended two-layer soundscape.

## 0.30.5 correction

The editor audio bridge now owns two logical long-form channels:

- `Music`
- `Ambience`

For each control action, the bridge:

1. Estimates and stores the current position of both channels.
2. Stops Unity’s global native preview transport.
3. Replays only the logical channels that should remain active.
4. Applies the requested Play, Pause, Stop, Next, or replacement operation to only its target channel.

Warning cues are played over the two long-form channels. Repeated cue or transport actions rebuild the expected channel set, preventing hidden clips from accumulating.

## Known Unity limitation

Some Unity editor versions expose only global preview-volume control. In that case, Music and Ambience can still play and stop independently, but their two volume sliders might not behave as a true independent mixer. DeverQuest now reports this distinction in the Audio UI.

## Verification checklist

- [ ] Start Music, then start Ambience; both remain audible.
- [ ] Stop Music; Ambience continues.
- [ ] Restart Music; Ambience continues.
- [ ] Pause Music; Ambience continues.
- [ ] Resume Music; Ambience continues.
- [ ] Choose Next Music repeatedly; only Music changes.
- [ ] Choose Next Ambience repeatedly; only Ambience changes.
- [ ] Stop Ambience; Music continues.
- [ ] Trigger Warning, Victory, and Level Up cues over both channels.
- [ ] Rapidly alternate Music Next/Previous/Stop/Play while Ambience runs.
- [ ] Rapidly alternate Ambience Next/Stop/Play while Music runs.
- [ ] Confirm no third or abandoned long-form clip remains audible.
- [ ] End and restart a Quest; session audio rules affect only their configured channels.

## Status

**Patched in source, pending Unity verification.**

---

# Conversation verdict

0.30.5 should not close either issue automatically. DQ-0302-002 and DQ-0304-005 close only after their complete verification lists pass in Unity and remain correct after an Editor restart.
