# DeverQuest Personal Notes
## Consolidated after 0.31.5 Editor UX Work

**Current patch target:** 0.31.5 Editor UX and Workspace Organization  
**Current product lane:** Finish and clarify the existing Beta loop before opening 2.0 systems.

---

# Immediate Importance

## Separate Quest Log and Git

Quest Log is now the live work-evidence surface:

- Notes
- Commit links
- Media
- Voice memos
- External activity
- Encounter and tactical evidence

Git is now the repository surface:

- Status
- Branch
- HEAD
- Commit
- Push
- Publish
- Recent Git-linked Quest evidence

**0.31.5 status:** implemented, awaiting Unity verification.

---

## Dockable Quest HUD

The HUD should be useful beside normal Unity workspaces without becoming a second timer.

Required behavior:

- Same active Session
- Same focused duration
- Same pause state
- Same Encounter
- Same Run ID
- Same turn-in
- No duplicate rewards
- No separate persistence

**0.31.5 status:** implemented as a normal dockable EditorWindow.

---

## Visual settings home

The Visuals workspace now owns local presentation settings:

- Theme
- Custom title/timer/accent colors
- DeverQuest text scale
- Workspace columns
- Compact labels
- Workspace guidance
- Header tagline
- HUD auto-open
- HUD story visibility

**0.31.5 status:** persistent local foundation implemented.

---

## Better workspace guidance

Empty and specialized workspaces should explain the next useful action rather than exposing implementation commentary.

**0.31.5 status:** workspace hints and stronger Quest Log empty-state navigation implemented.

---

## Git/Quest text separation

Quest Log notes and Git commit messages must never overwrite one another.

**0.31.5 status:** separate local fields implemented.

---

# Medium Importance

## Named Visual Profile assets

Future Visual Profile assets could store:

- Profile name
- Theme
- Custom colors
- Text scale
- Workspace columns
- Compact labels
- Header and guidance settings
- HUD settings
- Portrait-frame preferences
- Accessibility flags

Potential workflow:

- Create
- Duplicate
- Select
- Export
- Import
- Reset
- Studio default
- User override

Do not mix local user preferences into shared Guild authority records.

---

## Character and Companion portraits

Allow user-imported `Sprite` or `Texture2D` references for:

- Guild account
- Adventurer
- Companion
- Hall of Heroes

Recommended project path:

`Assets/DeverQuest/UserContent/Portraits/`

Requirements:

- Fallback initials
- Missing asset handling
- Non-square image handling
- Recommended dimensions
- Package upgrades preserve references
- No portrait file stored inside the package

---

## HUD refinements

Potential additions:

- Current reward estimate
- Current Companion
- HP/Mana/AC
- Music/Ambience status
- Wellness reminder
- Next work-block progress
- Configurable HUD sections
- Minimal timer-only mode
- More compact horizontal layout
- Remembered dock-specific layout

Avoid turning the HUD into a duplicate of the full DeverQuest window.

---

## Accessibility profiles

Future presets:

- High Contrast
- Reduced Color
- Reduced Motion
- Large Text
- Minimal Workbench
- Colorblind-friendly status colors

These should be tested against Unity light and dark Editor skins.

---

## Workspace favorites

Possible later behavior:

- Pin favorite workspaces
- Hide unused workspaces
- Custom workspace order
- Keyboard shortcuts
- Quick command palette
- Return to last workspace after restart

---

## Settings authority cleanup

Some existing appearance controls remain inside Guild-managed Settings even though presentation is local.

Future cleanup:

- Local Visuals and notifications remain user-owned.
- Guild policy and integrity settings remain leadership-owned.
- First-time setup clearly separates local preferences from Guild policy.

---

# Low Importance

## Additional visual polish

- Workspace icons
- Portrait frames
- Accent bars
- Status chips
- Better narrow-window wrapping
- Theme-aware progress bars
- Optional fantasy texture styling
- Smaller toolbar density

---

## Compact View relationship

Compact View and Quest HUD overlap but serve different roles:

- Compact View replaces the full DeverQuest body.
- Quest HUD is independently dockable beside normal Unity work.

Later testing may decide whether Compact View should remain, become a preset, or delegate entirely to the HUD.

---

# Expansion 2.0

## World-aware HUD

After Rooms, Biomes, hazards, and generated Quest Runs exist, the HUD may show:

- Current Area
- Biome
- Hazard timer
- Party
- Monsters
- Companion
- Loot
- Travel state
- Generated narrative

This remains downstream of the Quest World architecture.

---

## Visual world themes

Future 2.0 Campaigns may supply presentation themes:

- Swamp
- Dungeon
- Burning structure
- Arcane vault
- Guild office
- Devroth regions

Campaign presentation must remain optional and must never obscure productivity data or accessibility needs.

---

# Completed

- Repository preparation passed.
- Founder authority passed.
- Starter Identity Catalog passed.
- Reward snapshot consistency passed.
- Main Quest progress exists.
- Quest Story and Encounter wording exist.
- Repeatable Contract and Quest Run architecture exists.
- Quest Run Management exists.
- Tactical visibility and Tactical Operations exist.
- Inventory and Equipment workspace exists.
- Guild Economy workspace exists.
- Chronicle workspace exists.
- Quest Log and Git separated in 0.31.5.
- Dockable Quest HUD implemented.
- Visuals workspace implemented.
- Custom local colors and text scale implemented.
- Workspace columns and compact labels implemented.
- Header and guidance controls implemented.
- Git commit text separated from Quest Log note text.
- Release Readiness workspace configuration check implemented.

---

# Current Decision

After the 0.31.5 smoke test, the strongest next pathway is **0.31.6: Supported Audio Host and Mixer Reliability**.

That pathway should investigate replacing or isolating Unity's fragile shared preview transport so DeverQuest can provide:

- Reliable Music
- Reliable Ambience
- Reliable warning/SFX cues
- Independent volume controls
- Inspector-preview isolation
- Focus-loss recovery
- Clear fallback behavior

It should begin as a controlled prototype and must preserve the current audio profiles and playlists.
