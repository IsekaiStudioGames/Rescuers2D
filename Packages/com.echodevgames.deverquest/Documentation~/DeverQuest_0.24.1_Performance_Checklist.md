# DeverQuest 0.24.1 Performance Checklist

## Installation

- [ ] Install the 0.24.1 tarball in Unity 2022.3 or newer.
- [ ] Open **Tools > DeverQuest > Developer Companion**.
- [ ] Confirm the seven workspace tabs appear.
- [ ] Confirm existing profile, Quest, inventory, trade, and Chronicle data.

## Workspace Isolation

- [ ] Start a Quest and leave the **Quest** workspace visible.
- [ ] Confirm its timer updates about four times per second.
- [ ] Select **Character** and confirm the window no longer repaints
  continuously.
- [ ] Open each shortcut beneath **Tools > DeverQuest > Workspaces**.
- [ ] Confirm Guild Hall, History, and Character content loads only when its
  workspace is selected.
- [ ] Confirm Quest Log & Git can add notes and perform Git actions.

## Background Work

- [ ] Use Unity normally for at least five minutes during an active Quest.
- [ ] Confirm Scene view, Inspector, animation, and script editing remain
  responsive.
- [ ] Create an external Git commit and allow up to fifteen seconds for it to
  appear in the Quest Log.
- [ ] Confirm manual Git **Refresh** remains immediate.
- [ ] Modify or create an equipment, spell, or Shop Item asset and confirm the
  related workspace reflects it after Unity's asset change event.

## Regression

- [ ] Test meditate/resume, idle pause acknowledgment, and approved breaks.
- [ ] Complete a Quest through the guided turn-in.
- [ ] Test commit, push, reward, trade, Chronicle, and shared publishing.
- [ ] Confirm music and ambience continue while changing workspaces.
- [ ] Confirm the Unity Console contains no compilation errors or exceptions.

## Suggested Profiler Comparison

Capture an Editor Profiler sample during a running Quest before and after this
update. Compare **EditorLoop**, **GUI.Repaint**, process creation, AssetDatabase
search calls, and managed allocations over the same 60-second interval.
