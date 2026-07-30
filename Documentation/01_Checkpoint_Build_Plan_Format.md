---
title: Checkpoint Build Plan Format
project: Rescuers2D
type: workflow
tags: [workflow, checkpoint, unity]
---

# Checkpoint Build Plan Format

Use this note as the source format for future Rescuers2D implementation checkpoints.

## Purpose

Each checkpoint must answer:

- What are we building?
- Why does it matter to the overall system?
- What scripts, assets, prefabs, and scene objects are needed?
- In what order should the work happen?
- What is the goal line?
- How will it be tested?
- What should be committed when it works?
- What belongs in the devlog?
- Does it deserve a portfolio system page?
- How does it strengthen Jesse’s gameplay-systems portfolio?

## Conversation 1 — Plan / Build

Every implementation response should contain:

1. Checkpoint summary and scope boundaries
2. Architecture and data flow
3. Required files and affected existing files
4. Complete scripts in implementation order
5. ScriptableObject asset creation
6. Prefab, Inspector, scene, and bootstrap setup
7. Migration and compatibility notes
8. Testing checklist
9. Goal line / definition of done
10. Obsidian-ready milestone note update

Do not automatically include the commit/devlog or portfolio work unless requested.

## Conversation 2 — Test / Commit / Push / Devlog

After Jesse implements and tests:

1. Review results and Console errors
2. Diagnose and fix defects
3. Run the completion checklist
4. Summarize changed files
5. Propose a focused commit message
6. Provide branch, commit, and push guidance
7. Write the completed devlog entry
8. Update milestone status and decision log

## Conversation 3 — System Page

After the checkpoint is stable and pushed:

1. Decide whether it merits its own page or an update to the main Audio System page
2. Write the portfolio case-study content
3. Add screenshots and a compact architecture diagram
4. Add metadata and SEO
5. Link it from `systems.html`
6. Test navigation, layout, and responsive behavior
7. Record the portfolio URL in the milestone note

## Standard milestone status

`Planned → In Progress → Testing → Complete → Documented → Portfolio Published`

## Scope rule

Build the smallest stable vertical slice for the current milestone. Record later ideas under **Future Expansion** instead of quietly expanding implementation.

