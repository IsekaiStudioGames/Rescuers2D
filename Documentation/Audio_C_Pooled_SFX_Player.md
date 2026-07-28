---
title: Audio C - Pooled SFX Player
project: Rescuers2D
milestone: Audio C
status: Planned
tags: [audio, sfx, pooling, unity, checkpoint]
---

# Audio C — Pooled SFX Player

## Outcome

Build a persistent pooled service for 2D, positional, and attached `SfxCueData` playback.

## New files

- `SfxPlayer.cs`
- `SfxVoice.cs` or equivalent internal runtime record
- `SfxPlaybackHandle.cs` if controlled looping/stop support requires it

## Public API

`Play`, `PlayAtPosition`, `PlayAttached`, `Stop`, and `StopAll`.

## Responsibilities

- Reusable AudioSource pool
- Random, no-immediate-repeat, sequential, and shuffle-bag selection
- Weighted variations
- Cue and variation volume/pitch calculation
- Spatial settings and mixer routing
- Cooldowns and simultaneous voice limits
- Priority-aware pool exhaustion
- Attachment following and destroyed-target cleanup

## Goal line

Gameplay can request heavy overlapping sound activity without allocating a new AudioSource per event or leaving sources stuck active.

## Test checklist

- [ ] Play a cue as 2D.
- [ ] Play at a world position.
- [ ] Follow a moving target.
- [ ] Release a destroyed attachment.
- [ ] Stress rapid footsteps.
- [ ] Stress simultaneous explosions.
- [ ] Confirm all selection modes.
- [ ] Enforce cooldown.
- [ ] Enforce per-cue voice limit.
- [ ] Recover cleanly when the pool is exhausted.
- [ ] Stop one cue and stop all.

## Commit / devlog

- Suggested commit: `feat(audio): add pooled SFX playback with cue variations`
- Devlog focus: pooling, variation selection, and voice management.

## Portfolio value

High. Pool diagnostics and event-to-source flow should appear on the final Audio System page.

