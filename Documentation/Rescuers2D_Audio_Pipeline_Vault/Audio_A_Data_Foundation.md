---
title: Audio A - Data Foundation
project: Rescuers2D
milestone: Audio A
status: Planned
tags: [audio, unity, scriptableobject, checkpoint]
---

# Audio A — Data Foundation

## Outcome

Create the reusable assets that describe music tracks, music libraries, SFX cues, per-clip variations, and SFX libraries.

## Why it matters

This separates content tuning from runtime playback and prevents the future jukebox or SFX player from becoming Rescuers2D-specific.

## Build scope

### New files

- `MusicTrackData.cs`
- `MusicLibraryData.cs`
- `SfxSelectionMode.cs`
- `SfxVariation.cs`
- `SfxCueData.cs`
- `SfxLibraryData.cs`

### `MusicTrackData`

Track ID, display name, clip, volume, playback speed/pitch, reverse, loop, fade-in, fade-out, start delay, and mixer group.

### `SfxVariation`

Clip, volume multiplier, pitch offset, and selection weight.

### `SfxCueData`

Cue ID, display name, variations, selection mode, cue volume, randomized volume/pitch, spatial blend, min/max distance, cooldown, voice limit, priority, pause behavior, attachment behavior, and mixer group.

## Implementation order

1. Create enums and serializable variation type.
2. Create track and cue assets with validation.
3. Create both library assets.
4. Add library duplicate-ID and missing-reference validation.
5. Create representative Bomb Shelter track and initial SFX cues.

## Goal line

Unity can create, inspect, and validate every asset type without runtime playback code.

## Test checklist

- [ ] All assets appear in the intended Create menu.
- [ ] Ranges clamp safely in `OnValidate`.
- [ ] Empty IDs and missing clips warn clearly.
- [ ] Duplicate track IDs are detected.
- [ ] Duplicate cue IDs are detected.
- [ ] Variation weights and ranges cannot become invalid.
- [ ] Libraries expose read-only collections.

## Commit / devlog

- Suggested commit: `feat(audio): add reusable music and SFX data foundation`
- Devlog focus: content-driven tuning, direct references, and validation.

## Portfolio value

Strong supporting material for the final Audio System page, but probably not a standalone page.

## Future expansion

Custom inspectors, waveform previews, addressable libraries, and import automation.

