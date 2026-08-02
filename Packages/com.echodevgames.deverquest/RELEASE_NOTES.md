# DeverQuest 0.32.5 Beta Packaging and Distribution Hotfix

Version 0.32.5 preserves the packaging and distribution workspace and fixes a clean-import compiler ambiguity in its tarball exporter. The workspace continues to audit the installed Unity package, release documents, version declarations, source metadata, bundled media, content health, Release Readiness, license state, and known limitations.

The center can export a release dossier containing a deterministic package manifest, per-file SHA-256 hashes, a verified `.tgz` tarball, content-health report, known-limitations copy, distribution checklist, JSON audit report, and `SHA256SUMS.txt`.

## Distribution verdicts

- **Internal Beta:** may proceed with documented license or third-party-ledger advisories when no code, content, or archive blocker exists.
- **External Beta:** requires an explicit software license and no distribution blockers.
- **Release Candidate:** applies the same hard gates and should only be selected after deferred Beta regressions are resolved or formally accepted.

## Current license state

DeverQuest does not select a software license automatically. The Distribution Center reports a blocker for External Beta or Release Candidate when no `LICENSE`, `LICENSE.md`, or `LICENSE.txt` exists at the package or repository root.
