---
title: Audio H - Audio Laboratory UI
project: Rescuers2D
milestone: Audio H
status: Planned
tags: [audio, debug-ui, tooling, unity, checkpoint]
---

# Audio H — Audio Laboratory UI

## Outcome

Build a development UI that browses both libraries and controls the runtime services without owning them.

## Music panel

Library dropdown, selected/current track, play, stop, pause/resume, restart, previous/next library item, fade out, volume preview, speed/pitch, reverse, loop, and playback position.

## SFX panel

Library dropdown, selected cue, play 2D, play at test position, play attached, sample another variation, stop cue/all, variation details, and source diagnostics.

## Shared mixer panel

Master, Music, SFX, and Ambience sliders routed through the existing settings service.

## New files

- `AudioLabController.cs`
- `MusicJukeboxPanelUI.cs`
- `SfxCabinetPanelUI.cs`
- Optional small row/presenter components

## Ownership rule

Closing or destroying the panel must not stop or destroy the persistent audio services.

## Goal line

Dropping the bootstrap and Audio Lab into a project allows immediate asset browsing, music playback, SFX preview, mixer testing, and pool diagnosis.

## Test checklist

- [ ] Both libraries populate without hardcoded options.
- [ ] Empty libraries show a useful state.
- [ ] Track controls call the jukebox.
- [ ] Cue controls call the SFX player.
- [ ] Runtime overrides do not silently rewrite ScriptableObject assets.
- [ ] Mixer sliders use saved settings.
- [ ] Active/available/pool voice counts update.
- [ ] Panel destruction does not affect playback ownership.

## Commit / devlog

- Suggested commit: `feat(audio): add Audio Lab music and SFX testing interface`
- Devlog focus: developer tooling and fast content iteration.

## Portfolio value

Very high. This is the clearest visual demonstration of the framework and deserves polished screenshots or a short video.

