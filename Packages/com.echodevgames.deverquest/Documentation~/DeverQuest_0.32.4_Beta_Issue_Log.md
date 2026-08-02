# DeverQuest 0.32.4 Beta Issue Log
## Beta Packaging and Distribution Center

**Source build:** 0.32.3 Beta 1  
**Patch build:** 0.32.4 Beta 1  
**Baseline:** 25 passes, 0 advisories, 0 blockers; 77 assets healthy  
**Status:** Implemented, awaiting Unity verification

## DQ-0323-030 — Release packaging is manual and unaudited

### Previous behavior

Package tarballs, checksums, release notes, known limitations, license state, content health, and Release Readiness evidence were assembled across separate manual steps. A clean runtime/content report did not prove that the shipped archive matched the installed package.

### 0.32.4 correction

- Added Packaging and Distribution workspace.
- Added Internal Beta, External Beta, and Release Candidate channels.
- Audits package identity, required files, version declarations, source metadata, GUID uniqueness, media, repository documents, software license, credits, notices, known limitations, Release Readiness, and content health.
- Generates a per-file SHA-256 package manifest.
- Exports and verifies a `.tgz` against that manifest.
- Generates a dossier, JSON audit, content-health snapshot, known-limitations copy, distribution checklist, package-file hash list, and `SHA256SUMS.txt`.

## License boundary

The package intentionally does not choose a software license for the owner. Internal Beta may continue with an advisory. External Beta and Release Candidate are blocked until an approved LICENSE file exists.

## Required verification

- [ ] Install 0.32.4.
- [ ] Run Packaging Audit as Internal Beta.
- [ ] Export a dossier and verify the tarball.
- [ ] Confirm manifest and checksum files exist.
- [ ] Confirm no bundled media is reported.
- [ ] Confirm version consistency passes.
- [ ] Confirm External Beta reports the expected license blocker until a license is chosen.
- [ ] Add the approved LICENSE.
- [ ] Rerun External Beta audit.
- [ ] Install the exported tarball in a clean Unity project.
