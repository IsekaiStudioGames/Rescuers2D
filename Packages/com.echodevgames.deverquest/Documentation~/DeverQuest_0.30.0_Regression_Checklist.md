# DeverQuest 0.30.0 Release-Candidate Regression Checklist

## Package and migration

1. Install the `0.30.0` tarball through Unity Package Manager.
2. Confirm **Tools > DeverQuest** opens with no Console compilation errors.
3. Confirm the existing profile, Guild accounts, Adventurers, inventory,
   Companions, Contracts, active settings, and Chronicle history remain intact.
4. Run **Tools > DeverQuest > Run Release Readiness Check**.
5. Resolve every blocker before continuing.

## Core timer loop

1. Start a normal Quest and confirm Focus time advances.
2. Pause manually, wait, and resume.
3. Trigger an idle pause and acknowledge the return.
4. Restart the Unity editor during an active Quest and confirm recovery.
5. Add a Quest Log note and link a note to the current Git commit.
6. Complete the Quest and verify Markdown and JSON timecards are written.
7. Confirm the Chronicle entry reloads after another editor restart.

## Playlist ownership stress test

Use a playlist containing at least three clips with Repeat All enabled.

1. Press Play, Next, Previous, Stop, and Play at a normal pace.
2. Rapidly alternate Next and Previous at least ten times.
3. Press Stop during a warning cue.
4. Pause and resume during the middle of a track.
5. Let a track finish and confirm the next track starts automatically.
6. Confirm only one long-form clip is audible at every step.
7. Confirm Stop produces silence and does not leave a short hidden loop.
8. Repeat with Shuffle enabled.
9. Repeat with Repeat Off and confirm the playlist stops after its final track.
10. Repeat with Repeat One and confirm the selected track loops alone.

## Warning-cue interruption

1. Start playlist music.
2. Test Idle Warning, Victory, and Level Up cues.
3. Confirm each cue interrupts the music once.
4. Confirm the music resumes near its previous position after each cue.
5. Trigger several cues in quick succession and confirm the newest cue replaces
   the previous cue without creating layered duplicates.
6. Pause the playlist during a cue and confirm both cue and music become silent.
7. Resume and confirm one music track continues.

## Playlist and ambience exclusivity

1. Start playlist music, then start ambience.
2. Confirm playlist state changes to Stopped and only ambience remains.
3. Start playlist music again.
4. Confirm ambience state clears and only the playlist remains.
5. Use Next Ambience repeatedly and confirm no hidden playlist resumes.

## Existing RPG systems smoke test

1. Open every workspace once: Quest, Quest Log, Character, Guild Hall,
   Rewards & History, Audio & Wellness, and Settings.
2. Accept and complete a Focus-Stage Quest.
3. Resolve one normal tactical encounter.
4. Run one shortened Survival wave and exit safely.
5. Recruit or activate a Companion.
6. Buy, equip, drop, and trade a test item where permissions allow.
7. Verify physical coin value and denomination exchange preserve total value.
8. Open Compensation Preview and confirm its planning-only disclaimer remains.

## Clean finish

1. Confirm no Quest is active.
2. Run Release Readiness Check again.
3. Confirm no blockers remain.
4. Export or copy the final timecard and local profile data used for regression.
5. Tag the tested package commit as the release candidate.
