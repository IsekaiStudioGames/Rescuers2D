---
title: Audio Asset Naming and Folders
project: Rescuers2D
type: convention
tags: [audio, naming, folders]
---

# Audio Asset Naming and Folders

## Suggested reusable code folders

```text
Assets/Rescuers2D/Scripts/Audio/
├── Data/
├── Runtime/
├── Emitters/
├── Integration/
└── Debug/
```

## Suggested content folders

```text
Assets/Rescuers2D/Audio/
├── Mixer/
├── Music/
│   ├── Clips/
│   ├── Tracks/
│   ├── Libraries/
│   └── Playlists/
├── SFX/
│   ├── Clips/
│   ├── Cues/
│   └── Libraries/
└── Profiles/
```

## Naming conventions

| Asset | Pattern | Example |
|---|---|---|
| Music track | `MusicTrack_[Name]` | `MusicTrack_BombShelter` |
| Music library | `MusicLibrary_[Project]` | `MusicLibrary_Rescuers2D` |
| Playlist | `Playlist_[Name]` | `Playlist_BombShelter` |
| SFX cue | `SFX_[Action]` | `SFX_AxeImpact_Rock` |
| SFX library | `SfxLibrary_[Project]` | `SfxLibrary_Rescuers2D` |
| Character profile | `CharacterAudio_[Character]` | `CharacterAudio_Firefighter` |

## Stable ID conventions

- Lowercase snake case: `bomb_shelter`, `axe_impact_rock`.
- IDs are save/search keys and should not change because display text changes.
- Libraries warn about empty and duplicate IDs.
- Gameplay normally uses direct asset references rather than string lookup.

