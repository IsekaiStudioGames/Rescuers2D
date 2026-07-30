---
title: Audio I - Playlists
project: Rescuers2D
milestone: Audio I
status: Planned
tags: [audio, playlist, music, unity, checkpoint]
---

# Audio I — Playlists

## Outcome

Add data-driven playlist control only after single-track playback and the Audio Lab are stable.

## New files

- `MusicPlaylistData.cs`
- `MusicPlaylistPlayer.cs` or playlist responsibility added carefully to `MusicJukebox`
- Playlist controls in Audio Lab

## Playlist data

Playlist ID, display name, track collection, playback order, loop playlist, start index, and optional crossfade override.

## Playback modes

- Sequential
- Shuffle
- Shuffle without immediate repeat
- Random

## Runtime controls

Play playlist, next, previous, restart current, loop playlist, stop playlist, and inspect current index/history.

## Goal line

A playlist can drive the existing jukebox without duplicating track playback, fade, reverse, or source-management logic.

## Test checklist

- [ ] Sequential order reaches every track.
- [ ] Previous restores expected history.
- [ ] Shuffle avoids immediate repetition.
- [ ] Playlist loop returns to the correct track.
- [ ] Non-looping playlist stops cleanly.
- [ ] Missing tracks are skipped safely.
- [ ] Manual track play exits or suspends playlist mode intentionally.
- [ ] Crossfade overrides do not mutate track assets.

## Commit / devlog

- Suggested commit: `feat(audio): add data-driven music playlists`
- Devlog focus: composition over duplication—the playlist orchestrates the existing jukebox.

## Portfolio value

Use as the final polish section of the main Audio System case study.

