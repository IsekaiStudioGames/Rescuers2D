---
title: Audio D - Bootstrap and Level Integration
project: Rescuers2D
milestone: Audio D
status: Planned
tags: [audio, bootstrap, levels, unity, checkpoint]
---

# Audio D — Bootstrap and Level Integration

## Outcome

Make the jukebox and SFX player persistent services and allow each level configuration to choose its music.

## New files

- `LevelConfigurationProvider.cs`
- `LevelAudioCoordinator.cs`

## Existing files changed

- `ApplicationBootstrap.cs`
- `LevelConfigurationData.cs`

`SceneLoadService.cs` remains focused on scene loading.

## Data flow

`Scene load → LevelConfigurationProvider → LevelAudioCoordinator → MusicJukebox.Play(track)`

## Implementation order

1. Add serialized jukebox and SFX player references to bootstrap.
2. Validate and expose both services.
3. Add `MusicTrackData` to `LevelConfigurationData`.
4. Create a scene-level configuration provider.
5. Create the audio coordinator.
6. Configure menu and Bomb Shelter scenes.
7. Verify same-track behavior across reloads.

## Goal line

Every configured level starts the intended music through the one persistent runtime jukebox.

## Test checklist

- [ ] Bootstrap initializes one jukebox and SFX player.
- [ ] Duplicate bootstrap is destroyed safely.
- [ ] Main menu music starts.
- [ ] Bomb Shelter music starts.
- [ ] Different levels crossfade.
- [ ] Reloading the same scene does not restart unnecessarily.
- [ ] Missing track assignment is safe.
- [ ] A deliberately silent level remains silent by design.
- [ ] Mixer settings still apply after scene changes.

## Commit / devlog

- Suggested commit: `feat(audio): integrate runtime audio services with level configuration`
- Devlog focus: service lifetime, scene boundaries, and single-responsibility loading.

## Portfolio value

High architectural value; diagram the persistent service and scene-level bridge.

