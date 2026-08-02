# DeverQuest 0.22.0 Regression Checklist

## Installation and Migration

- [ ] Install the 0.22.0 `.tgz` through Unity Package Manager.
- [ ] Confirm **Tools > DeverQuest > Developer Companion** appears.
- [ ] Confirm the Console contains no compilation errors.
- [ ] Confirm the existing account, character, Chronicles, Contracts, shop,
      audio profiles, and 0.21.1 wellness settings remain intact.

## Aseprite and External Activity

- [ ] Create an Aseprite Activity Profile and select it.
- [ ] Start a Quest using **Unity Project Focused** activity scope.
- [ ] Bring Aseprite to the foreground and draw.
- [ ] Confirm the window reports **External craft active: Aseprite**.
- [ ] Remain in Aseprite beyond the normal Unity-focus timeout while continuing
      to work; confirm the Quest does not pause.
- [ ] Stop providing input beyond the configured freshness and idle timeout;
      confirm the Quest pauses normally.
- [ ] Confirm opening an unrelated, unconfigured application does not count.
- [ ] Add a second provider and test process-name and window-title matching.

## Voice Memos

- [ ] Confirm Unity lists the intended microphone.
- [ ] Record at least five seconds and choose **Stop and Attach**.
- [ ] Reveal the WAV file and play it externally.
- [ ] Confirm its channel count, duration, and playback are reasonable.
- [ ] Start another recording and cancel it; confirm no attachment is created.
- [ ] Start recording and trigger script compilation; confirm recording safely
      cancels without a stuck microphone.

## Existing Media

- [ ] Attach an image or audio file from outside the timecard folder.
- [ ] Confirm DeverQuest copies it into the dated Media directory.
- [ ] Rename or remove the original and confirm the copied attachment remains.
- [ ] Unlink it and confirm the session reference disappears while the copied
      file remains recoverable.

## Chronicle

- [ ] Complete the Quest with an external activity interval and voice memo.
- [ ] Confirm Daily Totals show external craft time and attachment count.
- [ ] Confirm **External Activity Journal** records start/end and duration.
- [ ] Confirm **Media Attachments** includes a working local link.
- [ ] Verify Chronicle integrity and regeneration.

## Regression

- [ ] Run the packaged 0.21.1 checklist.
- [ ] Confirm opening/revealing Chronicles still does not change music.
- [ ] Confirm acknowledgments still do not count as completed breaks.
- [ ] Confirm Git commit, push, finalization, rewards, encounters, and Chronicle
      rollover remain functional.
