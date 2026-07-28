---
title: Audio Test Catalog
project: Rescuers2D
type: qa
tags: [audio, testing, qa]
---

# Audio Test Catalog

## Platforms

- Unity Editor
- Windows development build
- WebGL development build

## Persistent regression tests

- [ ] Saved Master volume affects all categories.
- [ ] Saved Music volume affects jukebox sources.
- [ ] Saved SFX volume affects pooled sources.
- [ ] Scene transition does not duplicate the bootstrap.
- [ ] Missing assets log clear warnings without stopping gameplay.
- [ ] Pausing gameplay follows each cue’s pause rule.
- [ ] Rapid scene loading cannot start competing music transitions.
- [ ] No audio source remains permanently occupied after playback.
- [ ] Repeated events obey cooldown and voice limits.
- [ ] Destroyed attached targets release their voices.

## Listening tests

- [ ] No obvious clipping at expected maximum action density.
- [ ] Variations feel related but not mechanically repetitive.
- [ ] Important feedback remains audible under music.
- [ ] Spatial rolloff matches gameplay distance.
- [ ] Character sounds do not fire twice from overlapping event sources.

## Evidence to capture

- Inspector screenshots of representative track, cue, and profile assets
- Audio Lab playing a selected track and cue
- Source-pool diagnostics under stress
- Level transition with persistent/crossfading music
- Architecture diagram for the portfolio

