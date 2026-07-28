---
title: Audio Decision Log
project: Rescuers2D
type: decisions
tags: [audio, decisions, architecture]
---

# Audio Decision Log

## Accepted decisions

| Decision | Reason |
|---|---|
| Persistent players live with `ApplicationBootstrap` | Playback survives scene changes and has one owner |
| UI is a client, not the audio owner | Audio continues when debug UI closes |
| `SceneLoadService` remains scene-only | Level audio belongs in a small scene bridge |
| Levels directly reference `MusicTrackData` | Safe, discoverable, and no string lookup |
| Gameplay directly references `SfxCueData` | Keeps the core service reusable |
| Per-clip tuning lives in `SfxVariation` | Avoids hundreds of tiny assets |
| Two music AudioSources | Enables crossfades |
| SFX uses an AudioSource pool | Supports overlap without per-event components |
| Character sound slots live in profiles | Controllers remain focused on gameplay |
| Playlists wait until the jukebox is stable | Prevents orchestration from masking playback defects |
| Swing sound and hit sound use different events | A missed swing must not create impact feedback |
| Speed control uses `AudioSource.pitch` | Simple and supported; speed and pitch change together |
| Reverse is experimental | Platform/import behavior requires validation |

## Open decisions

- Initial SFX pool size and maximum expansion
- Priority/voice-stealing policy when the pool is exhausted
- Whether attached looping playback returns a handle
- Whether UI audio migrates fully or remains a focused service
- Whether silent levels explicitly request `Stop` or inherit current music
- Which platform limitations cause reverse to be hidden

## Change record

Add dated entries whenever an accepted decision changes:

```text
YYYY-MM-DD — Changed:
Reason:
Affected milestones/assets:
Migration required:
```

