# DeverQuest 0.31.6 Beta Issue Log
## Pathway 8 — Supported Audio Host and Mixer Reliability

**Source build:** 0.31.5 Beta 1  
**Patch build:** 0.31.6 Beta 1  
**Unity target:** 2022.3 minimum  
**Primary test environment:** Unity 6000.3.8f1  
**Patch status:** Prepared, awaiting Unity verification

---

# DQ-0315-031 — Long-form audio depends on Inspector preview transport

**Type:** Audio architecture  
**Severity:** P0 reliability risk  
**Status:** Replaced by supported host with compatibility fallback

## Previous behavior

Music, Ambience, and warning cues used Unity's internal preview-audio API. The same native preview transport is also used by Inspector AudioClip previews.

This produced several failure modes during Beta:

- Inspector playback could replace DeverQuest audio.
- Stop and Pause controls could lose ownership.
- Ambience could become a permanent loop.
- Warning cues could stop responding.
- Recovering one channel required reconstructing every logical channel.
- Some Unity versions exposed only one global preview-volume control.

## 0.31.6 correction

DeverQuest now prefers a hidden Editor-only AudioSource host containing separate sources for:

- Music
- Ambience
- Warnings and SFX

The host is created in an Editor preview scene and is not saved into the user's project scenes.

The previous preview bridge remains available as an automatic compatibility fallback.

---

# DQ-0315-032 — Music and Ambience cannot be mixed independently

**Type:** Mixer behavior  
**Severity:** P1  
**Status:** Patched in 0.31.6; awaiting verification

## Added local mixer controls

- Master volume
- Music mixer volume
- Ambience mixer volume
- Warning/SFX mixer volume
- Master mute
- Music mute
- Ambience mute
- Warning/SFX mute
- Optional long-form ducking during cues
- Configurable ducked volume

Playlist, Ambience Profile, and Warning Profile volumes remain intact. The effective output combines the profile volume with the local mixer volume.

These settings are stored in local Editor preferences and do not enter Guild, Quest, Chronicle, or shared repository data.

---

# DQ-0315-033 — Inspector preview can seize DeverQuest controls

**Type:** Audio isolation  
**Severity:** P0 reliability risk  
**Status:** Patched by supported host; fallback limitation documented

When the supported host is active, Inspector AudioClip previews no longer use the same playback sources as DeverQuest.

When the host is unavailable and the legacy fallback is active, the Audio workspace displays a warning that Inspector previews may still interrupt playback.

The Audio workspace now reports the active transport:

- Supported AudioSource Host
- Legacy Preview Fallback
- Unavailable

---

# DQ-0315-034 — Editor focus and audio-device changes can strand playback

**Type:** Lifecycle recovery  
**Severity:** P1  
**Status:** Patched in 0.31.6; awaiting verification

The supported host now responds to:

- Unity focus changes
- Entering Play Mode
- Returning to Edit Mode
- Audio-device/configuration changes
- Unexpected source stoppage
- Assembly reload
- Editor shutdown

Local preference:

`Pause When Unity Loses Focus`

When disabled, DeverQuest attempts to continue playback and repairs unexpectedly stopped sources after focus returns.

When enabled, DeverQuest pauses and resumes the supported host intentionally.

---

# DQ-0315-035 — Audio transport health is unclear

**Type:** Diagnostics  
**Severity:** P1  
**Status:** Patched in 0.31.6; awaiting verification

## Audio workspace additions

- Active transport label
- Transport status explanation
- Use Supported Audio Host toggle
- Reinitialize Audio Host
- Recover Active Audio
- Stop and Reset All Audio
- Reset Mixer Defaults

## Release Readiness additions

Readiness now separately checks:

- Editor audio transport
- Independent audio mixer
- Playlist completion detection

A supported host produces passes for transport and independent mixing.

A functioning legacy fallback produces an advisory rather than a blocker.

No available transport produces an advisory because the productivity timer remains usable without audio.

---

# Guardrails

- The host object is hidden and not saved into user scenes.
- The host is explicitly destroyed during assembly reload and Unity shutdown.
- Audio preferences remain local.
- Switching transport stops current playback rather than attempting a risky live migration.
- A host failure clears stale playback state and activates the compatibility fallback.
- Cue ducking changes volume only; it does not pause Quest timing.
- No audio operation awards XP, coin, items, or focused time.
- No audio file is bundled in the package.

---

# Required Retest

- [ ] Install 0.31.6 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Confirm Supported AudioSource Host is active.
- [ ] Confirm Independent audio mixer passes.
- [ ] Play Music and Ambience together.
- [ ] Change Music volume without changing Ambience.
- [ ] Change Ambience volume without changing Music.
- [ ] Change Warning/SFX volume.
- [ ] Test all mute controls.
- [ ] Trigger a warning and verify ducking.
- [ ] Preview a separate AudioClip in Inspector.
- [ ] Confirm DeverQuest channels remain controllable.
- [ ] Switch among DeverQuest workspaces.
- [ ] Switch to another application and return.
- [ ] Enter and exit Play Mode.
- [ ] Change the system audio output device when practical.
- [ ] Test Reinitialize Audio Host.
- [ ] Test Recover Active Audio.
- [ ] Test Stop and Reset All Audio.
- [ ] Force Legacy Preview Fallback and confirm the warning appears.
- [ ] Return to Supported Audio Host.
- [ ] Restart Unity and confirm mixer preferences persist.

---

# Current Verdict

**PATCH PREPARED — UNITY VERIFICATION REQUIRED**
