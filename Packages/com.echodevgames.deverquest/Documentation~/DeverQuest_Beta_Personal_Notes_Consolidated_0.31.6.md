# DeverQuest Personal Notes
## Consolidated after 0.31.6 Audio Host Work

**Current patch target:** 0.31.6 Supported Audio Host and Mixer Reliability  
**Product lane:** Finish and stabilize the current Beta loop before 2.0 expansion systems.

---

# Immediate Importance

## Supported audio host

DeverQuest now prefers a hidden Editor-only AudioSource host rather than Unity's shared Inspector preview transport.

Channels:

- Music
- Ambience
- Warnings/SFX

**0.31.6 status:** implemented with automatic legacy fallback; awaiting Unity verification.

---

## Independent volume

Local mixer controls now include:

- Master
- Music
- Ambience
- Warning/SFX
- Per-channel mute
- Master mute
- Cue ducking

Asset-level Playlist, Ambience Profile, and Warning Profile volumes remain available.

**0.31.6 status:** implemented through separate AudioSources when the supported host is active.

---

## Inspector preview isolation

The supported host must remain independent from Inspector AudioClip preview controls.

Expected result:

- Inspector preview does not stop DeverQuest Music.
- Inspector preview does not trap Ambience.
- Warning cues remain available.
- No manual Pause/Play ritual is required after returning to DeverQuest.

**0.31.6 status:** architecture implemented; needs torture testing.

---

## Audio lifecycle recovery

Audio must survive or recover from:

- Workspace changes
- Unity focus loss
- Play Mode transitions
- Audio-device changes
- Assembly reload
- Editor shutdown

Local users may choose whether Unity focus loss pauses audio.

**0.31.6 status:** recovery hooks implemented.

---

## Clear fallback behavior

When supported Edit Mode AudioSource playback is unavailable:

- Switch to the legacy preview bridge.
- Show an advisory.
- Explain that independent gain and Inspector isolation are unavailable.
- Preserve the timer and all non-audio systems.

**0.31.6 status:** implemented.

---

# Medium Importance

## Named Audio Profile assets

A future Audio Experience Profile could combine:

- Playlist
- Ambience Profile
- Warning Profile
- Master/channel mixer values
- Ducking preference
- Focus behavior
- Quest-start behavior
- Workspace or Campaign association

Local user preference should override a Guild default without changing shared Guild data.

---

## Crossfades

Potential 1.x improvements:

- Music track crossfade
- Ambience crossfade
- Quest-start fade-in
- Quest-completion fade-out
- Warning duck attack/release times
- Smooth mute/unmute

Crossfades should use the supported host and should not be attempted through the legacy preview fallback.

---

## Audio device diagnostics

Possible future diagnostics:

- Current output sample rate
- Speaker mode
- DSP buffer size
- Last configuration-change time
- Host source state
- Current sample position
- Automatic recovery count
- Last fallback reason

Keep this behind an advanced diagnostics foldout.

---

## Playlist persistence

Potential enhancements:

- Remember playback position across domain reload
- Resume the previous track after Unity restart
- Per-project versus global playlist choice
- Recent playlists
- Favorites
- Playlist import/export

Do not write commercial soundtrack files into the package or public repository.

---

## Audio and Quest HUD

The dockable HUD may later show:

- Music title
- Ambience title
- Music mute
- Ambience mute
- Master volume
- Next track

Keep it compact and avoid duplicating the complete Audio workspace.

---

# Low Importance

## Visual meters

Potential polish:

- Music activity indicator
- Ambience activity indicator
- Cue flash
- Simple level meters
- Ducking indicator
- Transport fallback badge

Avoid expensive per-frame waveform rendering.

---

## Audio preset names

Possible presets:

- Quiet Workbench
- Guild Hall
- Deep Focus
- Tavern Noise
- Silent Chronicle
- Custom

These would be local convenience presets, not serialized Guild law.

---

# Expansion 2.0

## Biome and Room audio

After Quest World data exists, a Quest Run may select audio from:

- Biome
- Structure
- Room
- Hazard
- Weather
- Combat state
- Merchant area
- Rest area
- Boss encounter

The system should layer or transition between data-driven audio profiles while preserving accessibility and mute controls.

---

## Character and Companion audio identity

Potential 2.0 features:

- Character theme
- Companion cues
- Class-specific stingers
- Faith or deity motifs
- Item discovery sounds
- Skill-up sounds
- Regional music

All external assets require explicit licensing and attribution records.

---

# Completed

- Repository preparation passed.
- Founder authority passed.
- Identity Catalog passed.
- Reward snapshots passed.
- Repeatable Contract and Quest Run architecture exists.
- Quest Run Management exists.
- Tactical visibility and Tactical Operations exist.
- Inventory and Equipment clarity exists.
- Guild Economy exists.
- Chronicle workspace exists.
- Quest Log and Git are separated.
- Dockable Quest HUD exists.
- Visual settings foundation exists.
- Supported hidden AudioSource host implemented in 0.31.6.
- Music, Ambience, and cue sources separated.
- Independent mixer controls implemented.
- Cue ducking implemented.
- Focus, Play Mode, and device-change recovery implemented.
- Legacy preview fallback retained.
- Audio Readiness checks expanded.

---

# Current Decision

After the 0.31.6 audio smoke test, the strongest next pathway is **0.31.7: Notifications and Wellness Command Center**.

That pathway should unify:

- Focus check-ins
- Hydration
- Movement
- Meals
- Approved Break timing
- Snooze state
- Quiet hours
- Notification history
- Configurable cues
- HUD wellness visibility

It should improve the systems already present rather than adding passive XP for simply leaving Unity open.
