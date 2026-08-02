# DeverQuest 0.30.2 - Namespace Compatibility Hotfix

Version 0.30.2 fixes a Unity 2022.3 compilation error in
`DeverQuestReleaseReadinessService`.

## Fixed

- Replaced the ambiguous unqualified `PackageInfo` reference with
  `UnityEditor.PackageManager.PackageInfo`.
- Updated the release-readiness expected package version to 0.30.2.

No runtime, timer, progression, inventory, tactical, or audio behavior changed.
