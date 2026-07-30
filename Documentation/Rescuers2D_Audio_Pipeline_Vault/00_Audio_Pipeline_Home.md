---
title: Rescuers2D Audio Pipeline
project: Rescuers2D
system: Reusable Audio Framework
status: Planned
tags: [rescuers2d, audio, unity, milestone-dashboard]
---

# Rescuers2D Audio Pipeline

> [!abstract] Goal
> Build a reusable Unity audio framework containing a persistent music jukebox, pooled SFX player, browsable libraries, character audio profiles, level music handoff, an Audio Lab UI, and playlists.

## Pipeline status

| Milestone | Purpose | Status |
|---|---|---|
| [[Audio_A_Data_Foundation]] | Define reusable music and SFX assets | Planned |
| [[Audio_B_Music_Jukebox]] | Persistent two-source music playback | Planned |
| [[Audio_C_Pooled_SFX_Player]] | Pooled 2D, positional, and attached SFX | Planned |
| [[Audio_D_Bootstrap_Level_Integration]] | Connect audio to bootstrap and levels | Planned |
| [[Audio_E_Character_Audio_Profiles]] | Reusable character sound profiles | Planned |
| [[Audio_F_World_Interaction_Integration]] | Connect gameplay events to SFX cues | Planned |
| [[Audio_G_Splash_UI_Migration]] | Migrate splash and optional UI audio | Planned |
| [[Audio_H_Audio_Laboratory_UI]] | Build music and SFX testing tools | Planned |
| [[Audio_I_Playlists]] | Add playlist sequencing and shuffle | Planned |

## Core references

- [[01_Checkpoint_Build_Plan_Format]]
- [[02_Audio_System_Architecture]]
- [[03_Audio_Asset_Naming_and_Folders]]
- [[04_Audio_Test_Catalog]]
- [[90_Devlog_Entry_Template]]
- [[91_Portfolio_System_Page_Checklist]]
- [[99_Audio_Decision_Log]]

## Current goal line

Complete **Audio A**, test its assets and validation, then move to **Audio B** so music can begin playing as early as possible.

## Three-conversation milestone loop

1. **PLAN / BUILD** — plan, complete scripts, Inspector and scene setup, test checklist, and milestone note.
2. **TEST / COMMIT / PUSH / DEVLOG** — test in Unity, repair failures, commit, push, and record the devlog.
3. **SYSTEM PAGE** — create or update the portfolio page after the milestone is stable and worth presenting.

## Definition of pipeline complete

- Each level can declare its music through `LevelConfigurationData`.
- Music survives scene changes and crossfades through `MusicJukebox`.
- Gameplay can play direct, positional, or attached `SfxCueData`.
- Repeated sounds use variations without uncontrolled duplication.
- Character sounds are assigned through reusable profiles.
- Splash and chosen UI sounds use the shared framework.
- Audio Lab can browse, tune, preview, and diagnose both libraries.
- Playlists support sequencing, shuffle, navigation, and looping.
- Saved mixer settings continue controlling Master, Music, SFX, and Ambience.

