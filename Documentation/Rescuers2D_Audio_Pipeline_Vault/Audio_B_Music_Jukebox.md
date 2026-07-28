---
title: Audio B - Music Jukebox
project: Rescuers2D
milestone: Audio B
status: Planned
tags: [audio, music, unity, checkpoint]
---

# Audio B — Music Jukebox

## Outcome

Build a persistent, two-source jukebox capable of playing and crossfading `MusicTrackData`.

## New files

- `MusicJukebox.cs`

## Public API

`Play`, `Stop`, `Pause`, `Resume`, `Restart`, `FadeOut`, `SetPlaybackSpeed`, `SetReverse`, and `SetLoop`.

## Runtime behavior

- Two reusable AudioSources
- Fade-in, fade-out, and crossfade
- Track start delay
- Same-track restart protection
- Explicit restart support
- Looping and playback-position tracking
- Speed/pitch override
- Experimental reverse playback
- Unscaled-time transitions
- Clean cancellation of prior routines

## Implementation order

1. Establish source A/B state.
2. Implement immediate play and stop.
3. Add pause, resume, and restart.
4. Add fades and crossfading.
5. Add delay, looping, and same-track rules.
6. Add runtime speed and reverse overrides.
7. Expose diagnostic properties and events.

## Goal line

One persistent jukebox can move between tracks cleanly without duplicate playback or scene ownership.

## Test checklist

- [ ] Normal single-track playback works.
- [ ] Re-requesting the same track does not restart it.
- [ ] Explicit restart returns to the correct beginning/end.
- [ ] Different tracks crossfade.
- [ ] Pause/resume preserves playback position.
- [ ] Stop and fade-out release both sources.
- [ ] Start delay uses unscaled time.
- [ ] Speed/pitch works through the supported range.
- [ ] Reverse fails safely where unsupported.
- [ ] Timescale zero does not freeze transitions.

## Commit / devlog

- Suggested commit: `feat(audio): implement persistent crossfading music jukebox`
- Devlog focus: two-source state management and reusable track data.

## Portfolio value

Capture for the final system page: track Inspector, crossfade state diagram, and runtime controls.

