---
title: Audio Portfolio System Page Checklist
project: Rescuers2D
type: portfolio
tags: [portfolio, audio, checklist]
---

# Audio Portfolio System Page Checklist

## Recommended page

One main **Reusable Audio Framework** system page, updated as milestones become stable. Avoid nine thin pages.

## Story structure

- Problem: audio calls and tuning can become scattered and scene-bound.
- Goal: portable, data-driven playback with strong iteration tools.
- Architecture: persistent services, asset data, scene bridges, and emitters.
- Engineering highlights: crossfading, pooling, selection modes, limits, profiles, and playlists.
- Integration: levels, characters, interactions, splash, and UI.
- Tooling: Audio Lab.
- Results: reusable setup and faster content iteration.

## Evidence checklist

- [ ] Architecture diagram
- [ ] `MusicTrackData` Inspector
- [ ] `SfxCueData` variations Inspector
- [ ] Audio Lab music panel
- [ ] Audio Lab SFX diagnostics
- [ ] Level configuration music field
- [ ] Character profile asset
- [ ] Short crossfade demonstration
- [ ] Stress test showing source-pool reuse

## Website checklist

- [ ] Title, description, canonical, and OpenGraph metadata
- [ ] Link from `systems.html`
- [ ] Clear Unity/C# role statement
- [ ] Responsive screenshots
- [ ] Working back/navigation links
- [ ] Alt text
- [ ] GitHub link only if the reusable code is public and cleaned
- [ ] Final mobile and desktop check

