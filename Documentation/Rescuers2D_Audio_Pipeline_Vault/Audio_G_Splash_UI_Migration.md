---
title: Audio G - Splash and UI Migration
project: Rescuers2D
milestone: Audio G
status: Planned
tags: [audio, splash, ui, unity, checkpoint]
---

# Audio G — Splash and UI Migration

## Outcome

Move splash audio into reusable SFX cues and decide whether the existing UI audio profile should remain or migrate.

## Existing files affected

- `SplashEntry` / splash sequence data source
- `SplashSequenceController.cs`
- Optionally `UIAudioProfileData.cs`
- Optionally `UIAudioService.cs`

## Safe migration order

1. Keep current splash AudioSource until `SfxPlayer` passes integration tests.
2. Add cue references beside legacy fields temporarily if needed.
3. Route splash playback through the persistent SFX service.
4. Remove legacy clip/volume fields after asset migration.
5. Evaluate UI migration separately to avoid breaking menu feedback.

## Goal line

Splash sounds use the same cue tuning and saved SFX mixer level, with no regression to the scene handoff.

## Test checklist

- [ ] Every splash entry plays once.
- [ ] Fade sequence and audio timing remain aligned.
- [ ] Scene handoff stops or completes audio intentionally.
- [ ] Splash playback works with timescale zero.
- [ ] Saved SFX volume applies.
- [ ] UI clicks still work while paused.
- [ ] No legacy AudioSource is removed before migration succeeds.

## Commit / devlog

- Suggested commit: `refactor(audio): migrate splash playback to shared SFX cues`
- Devlog focus: staged migration without breaking existing UI.

## Portfolio value

Supporting evidence for framework adoption across both gameplay and presentation.

