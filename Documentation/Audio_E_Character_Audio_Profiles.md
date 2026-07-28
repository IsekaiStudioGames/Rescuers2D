---
title: Audio E - Character Audio Profiles
project: Rescuers2D
milestone: Audio E
status: Planned
tags: [audio, characters, unity, checkpoint]
---

# Audio E — Character Audio Profiles

## Outcome

Give rescuers, survivors, and later animals reusable sound profiles without adding sound fields to every controller.

## New files

- `CharacterAudioProfileData.cs`
- `CharacterAudioEmitter.cs`

## Profile slots

Jump, land, hurt, death, footstep, swim stroke, swim loop, climb step, interact, pickup, drop, primary action, secondary action, and special action.

## Emitter rule

The emitter translates an existing gameplay or animation event into audio. It never decides when a jump, hurt, or death occurs.

## Initial assets

- `CharacterAudio_Firefighter`
- `CharacterAudio_Rescuer`
- `CharacterAudio_Survivor`
- `CharacterAudio_Dog` later

## Goal line

Characters can share the emitter code while swapping complete audio identities through profile assets.

## Test checklist

- [ ] Jump and land fire once.
- [ ] Alternating footsteps use cue variations.
- [ ] Hurt fires from damage, not every animation frame.
- [ ] Death fires once.
- [ ] Climb and swim events match their movement cadence.
- [ ] Profiles share common cues where desired.
- [ ] Character-specific overrides require no code change.

## Commit / devlog

- Suggested commit: `feat(audio): add reusable character audio profiles and emitter`
- Devlog focus: composition and event-driven character feedback.

## Portfolio value

Good example of data-driven character variation within the broader Audio System page.

