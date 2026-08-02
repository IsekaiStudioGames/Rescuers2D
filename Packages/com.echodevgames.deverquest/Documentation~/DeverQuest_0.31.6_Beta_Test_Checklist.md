# DeverQuest 0.31.6 Beta Test Checklist
## Quest 8 — The Three-Channel Tavern

**Build:** 0.31.6 Beta 1  
**Status legend:** `[x] PASS` · `[?] CONDITIONAL` · `[-] FAIL` · `[ ] NOT TESTED`

This is a focused audio-regression checklist. Deferred Contract, Party, Tactical, Inventory, Economy, Chronicle, and workspace checklists remain separate.

---

# A. Installation and Readiness

- [ ] Install `com.echodevgames.deverquest-0.31.6.tgz`.
- [ ] Confirm Package Manager reports 0.31.6.
- [ ] Confirm Unity compiles with zero errors.
- [ ] Restart Unity.
- [ ] Run Release Readiness.
- [ ] Confirm Editor audio transport passes.
- [ ] Confirm Independent audio mixer passes.
- [ ] Confirm Playlist completion detection passes.
- [ ] Record the Active Transport shown in Audio & Wellness.
- [ ] Confirm no hidden audio object appears in normal Hierarchy views.
- [ ] Save and reopen the current scene.
- [ ] Confirm no DeverQuest audio object was added to the scene file.

---

# B. Supported Host Status

- [ ] Open Audio & Wellness.
- [ ] Confirm Active Transport says `Supported AudioSource Host`.
- [ ] Confirm status says Music, Ambience, and cues use separate sources.
- [ ] Disable Use Supported Audio Host.
- [ ] Confirm playback stops safely.
- [ ] Confirm Active Transport changes to Legacy Preview Fallback.
- [ ] Confirm a compatibility warning appears.
- [ ] Re-enable Use Supported Audio Host.
- [ ] Confirm the host reinitializes.
- [ ] Confirm no Console exception appears.

---

# C. Music Transport

- [ ] Assign a playlist containing at least three tracks.
- [ ] Select Track 1 directly.
- [ ] Play Music.
- [ ] Pause Music.
- [ ] Resume Music.
- [ ] Select Track 2 while playing.
- [ ] Press Previous.
- [ ] Press Next repeatedly.
- [ ] Enable Shuffle.
- [ ] Test Repeat Off.
- [ ] Test Repeat All.
- [ ] Test Repeat One.
- [ ] Let a track end naturally.
- [ ] Confirm automatic advancement.
- [ ] Stop Music.
- [ ] Confirm only Music stops.

---

# D. Ambience Transport

- [ ] Assign an Ambience Profile containing at least three clips.
- [ ] Select Ambience 1 directly.
- [ ] Play Ambience.
- [ ] Select Ambience 2 while playing.
- [ ] Press Next Ambience repeatedly.
- [ ] Confirm Ambience loops.
- [ ] Stop Ambience.
- [ ] Confirm only Ambience stops.
- [ ] Start a Quest with Quest-Aware Ambience enabled.
- [ ] Confirm Ambience starts when expected.
- [ ] Complete or abandon the Quest.
- [ ] Confirm Quest-aware Ambience stops when expected.

---

# E. Simultaneous Playback

- [ ] Start Music.
- [ ] Start Ambience.
- [ ] Confirm both are audible.
- [ ] Pause Music.
- [ ] Confirm Ambience continues.
- [ ] Resume Music.
- [ ] Stop Ambience.
- [ ] Confirm Music continues.
- [ ] Restart Ambience.
- [ ] Stop Music.
- [ ] Confirm Ambience continues.
- [ ] Rapidly alternate both transports for two minutes.
- [ ] Confirm no third long-form track appears.
- [ ] Confirm no abandoned loop remains.

---

# F. Independent Mixer

Use clearly different source material for Music and Ambience.

- [ ] Set Master Volume to 100%.
- [ ] Set Music Mixer to 100%.
- [ ] Set Ambience Mixer to 20%.
- [ ] Confirm Music is louder than Ambience.
- [ ] Set Music Mixer to 10%.
- [ ] Set Ambience Mixer to 100%.
- [ ] Confirm Ambience is louder than Music.
- [ ] Change the Playlist asset Volume.
- [ ] Confirm only Music changes.
- [ ] Change the Ambience Profile Volume.
- [ ] Confirm only Ambience changes.
- [ ] Change Warning Profile Volume.
- [ ] Confirm only future cues change.
- [ ] Mute Music.
- [ ] Confirm Music continues logically but is inaudible.
- [ ] Unmute Music.
- [ ] Mute Ambience.
- [ ] Unmute Ambience.
- [ ] Mute Warnings and SFX.
- [ ] Unmute Warnings and SFX.
- [ ] Mute All.
- [ ] Confirm all channels become inaudible.
- [ ] Unmute All.
- [ ] Confirm playback returns without restarting tracks.

---

# G. Cue Ducking

- [ ] Enable Duck Long Audio During Cues.
- [ ] Set Ducked Volume to 25%.
- [ ] Play Music and Ambience.
- [ ] Trigger Test Warning.
- [ ] Confirm both long-form channels reduce temporarily.
- [ ] Confirm both restore after the cue.
- [ ] Trigger Test Victory.
- [ ] Trigger Test Level Up.
- [ ] Trigger several cues quickly.
- [ ] Confirm only the intended cue source changes.
- [ ] Disable ducking.
- [ ] Trigger another cue.
- [ ] Confirm long-form volume remains unchanged.

---

# H. Inspector Preview Isolation

- [ ] Play Music and Ambience.
- [ ] Select an unrelated AudioClip in Project.
- [ ] Preview it in Inspector.
- [ ] Stop the Inspector preview.
- [ ] Confirm DeverQuest Music remains controllable.
- [ ] Confirm DeverQuest Ambience remains controllable.
- [ ] Confirm warning cues still work.
- [ ] Preview a looping Inspector clip.
- [ ] Change Music track.
- [ ] Change Ambience track.
- [ ] Stop the Inspector preview.
- [ ] Confirm no DeverQuest channel is stranded.
- [ ] Confirm no Recover operation was required.

---

# I. Focus and Play Mode

## Continue while unfocused

- [ ] Disable Pause When Unity Loses Focus.
- [ ] Play Music and Ambience.
- [ ] Switch to another application for 30 seconds.
- [ ] Return to Unity.
- [ ] Confirm both channels are playing or recover automatically.
- [ ] Confirm controls remain responsive.

## Pause while unfocused

- [ ] Enable Pause When Unity Loses Focus.
- [ ] Switch to another application.
- [ ] Return to Unity.
- [ ] Confirm playback resumes near its previous position.

## Play Mode

- [ ] Play Music and Ambience in Edit Mode.
- [ ] Enter Play Mode.
- [ ] Confirm DeverQuest audio does not create duplicate scene objects.
- [ ] Exit Play Mode.
- [ ] Confirm expected Editor audio resumes.
- [ ] Repeat with Domain Reload disabled when available.

---

# J. Recovery and Failure Controls

- [ ] Click Recover Active Audio during normal playback.
- [ ] Confirm playback remains usable.
- [ ] Click Stop and Reset All Audio.
- [ ] Confirm every channel stops.
- [ ] Confirm the UI reports stopped state.
- [ ] Restart all channels.
- [ ] Click Reinitialize Audio Host.
- [ ] Confirm playback state clears safely.
- [ ] Confirm Supported AudioSource Host returns.
- [ ] Click Reset Mixer Defaults.
- [ ] Confirm default gains and toggles return.
- [ ] Restart Unity.
- [ ] Confirm saved non-default mixer values persist when not reset.

---

# K. Audio Device Change

When the test machine permits:

- [ ] Play Music and Ambience.
- [ ] Change Windows audio output device.
- [ ] Return to Unity.
- [ ] Confirm the host recovers.
- [ ] Confirm both sources remain controllable.
- [ ] Confirm warning cues remain audible.
- [?] Record any platform-specific limitation.

---

# L. Legacy Fallback

- [ ] Disable Use Supported Audio Host.
- [ ] Confirm Legacy Preview Fallback is active.
- [ ] Confirm Music can play.
- [ ] Confirm Ambience can play.
- [ ] Confirm the independent-mixer advisory appears.
- [ ] Preview an Inspector clip.
- [ ] Confirm the fallback limitation is accurately described.
- [ ] Use Recover Active Audio.
- [ ] Re-enable the supported host.
- [ ] Confirm normal independent mixing returns.

---

# M. Safety Regression

- [ ] Audio browsing creates no Quest Session.
- [ ] Audio playback adds no focused time.
- [ ] Audio playback awards no coin.
- [ ] Audio playback awards no XP.
- [ ] Changing transport changes no Guild authority.
- [ ] Mixer settings do not enter shared Guild records.
- [ ] Mixer settings do not dirty Quest assets.
- [ ] Package contains no AudioClip files.
- [ ] Closing Unity stops all host sources cleanly.
- [ ] Assembly recompilation produces no stuck audio.

---

# Verdict

- [ ] **PASS** — supported host, independent mixer, isolation, and lifecycle recovery pass.
- [ ] **CONDITIONAL PASS** — supported host works with a documented device or Play Mode limitation.
- [ ] **FAIL** — channels become stranded, mixer controls are coupled, Inspector previews seize ownership, or hidden host objects enter user scenes.
