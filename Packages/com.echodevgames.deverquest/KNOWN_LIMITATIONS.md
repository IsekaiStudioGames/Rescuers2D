# DeverQuest 0.32.5 Known Limitations

## Deferred verification

The following feature matrices are implemented but not yet fully verified across multiple accounts, Git clones, and long-duration production use:

- Repeatable and limited-completion Contract contention
- Full multi-account Party Quest regression
- Concurrent shared-Guild Contract edits across separate clones
- Complete Companion, Combat, and Survival matrices
- Full Inventory, trading, redemption, and Economy regression
- Long-history Chronicle and archive performance
- Clean-install and upgrade testing across every historical package version

## Persistence boundaries

- Some local settings and account state are stored in Unity `EditorPrefs`. Reinstalling the package does not automatically erase those values.
- Timecards and media live outside the package at the configured project path.
- Battle, Wellness, and other local diagnostic archives under `Library/DeverQuest` are not permanent Chronicle records.
- Shared Guild publishing uses files and normal source-control workflows; it is not an authoritative hosted service.

## Collaboration boundaries

- Simultaneous edits to the same Unity Contract asset in separate Git clones may produce ordinary YAML merge conflicts.
- Cross-clone run reservations are not transactional. Teams must pull before claiming shared work and resolve conflicts deliberately.

## Audio boundaries

- The supported hidden `AudioSource` host is preferred. The legacy preview fallback may not support independent channel volume and may be vulnerable to Inspector-preview interruption.
- No audio media is bundled. Administrators are responsible for the rights to imported media.

## Product boundaries

- DeverQuest is a productivity and recordkeeping aid, not payroll, medical, legal, or financial software.
- Compensation Preview does not issue payment or replace employer records.
- Real-world redemption records do not prove external delivery.
- Chronicle integrity detects changes but cannot prevent a person with local file access from replacing files.

## Deferred 2.0 systems

Crafting, banking, housing, broad tradeskills, Room/Biome simulation, procedural world narratives, and account-level reward currency remain outside the current Beta release.

## Character boundary

One Guild account currently owns one active Adventurer identity. Multiple character slots are deferred.

## Licensing boundary

Public distribution requires a deliberate software-license decision. Candidate local audio sources must not be shipped until exact creator, source, license, attribution, receipt, and redistribution records are complete.
