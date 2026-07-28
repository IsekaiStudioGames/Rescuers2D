---
title: Audio F - World Interaction Integration
project: Rescuers2D
milestone: Audio F
status: Planned
tags: [audio, gameplay, interactions, unity, checkpoint]
---

# Audio F — World Interaction Integration

## Outcome

Connect world actions and successful gameplay results to SFX cues.

## Integration catalog

- Axe swing from animation event
- Axe impact from hit result
- Rock impact and destruction
- Wood impact and destruction
- Door locked/unlocked
- Key pickup
- C4 drop and explosion
- Switch activation
- Ladder pickup/drop
- Survivor rescued

## Implementation rule

Use a direct serialized `SfxCueData` reference on the responsible component. Use small reusable emitters only where multiple related calls would otherwise repeat.

## Correct event timing

- Swing animation → whoosh
- Confirmed collision → material impact
- Destruction state → break sound
- State change succeeds → switch/door confirmation
- Explosion executes → explosion at world position

## Goal line

Important world interactions produce correctly timed, positioned, replaceable feedback without coupling gameplay logic to the audio framework.

## Test checklist

- [ ] Missed axe swing produces no impact.
- [ ] Rock and wood select different impact cues.
- [ ] C4 explosion remains audible if the C4 object is destroyed immediately.
- [ ] Locked and unlocked door feedback are distinct.
- [ ] Repeated switch requests do not duplicate activation audio.
- [ ] Pickup/drop sounds occur only after successful state changes.
- [ ] Voice limits handle destruction chains.

## Commit / devlog

- Suggested commit: `feat(audio): integrate SFX cues with world gameplay events`
- Devlog focus: separating attempt, success, impact, and state-transition sounds.

## Portfolio value

Capture short gameplay examples showing one reusable player serving unrelated systems.

