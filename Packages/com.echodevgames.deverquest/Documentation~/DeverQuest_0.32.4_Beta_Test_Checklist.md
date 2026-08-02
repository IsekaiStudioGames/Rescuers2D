# DeverQuest 0.32.4 Beta Test Checklist
## Quest 8 — Seal the Release Crate

- [ ] Install 0.32.4 and compile with zero errors.
- [ ] Run Release Readiness.
- [ ] Open Packaging & Distribution.
- [ ] Run Internal Beta audit.
- [ ] Confirm installed package identity passes.
- [ ] Confirm required package files pass.
- [ ] Confirm version consistency passes.
- [ ] Confirm Unity metadata integrity passes.
- [ ] Confirm bundled media audit passes.
- [ ] Confirm repository release files pass.
- [ ] Confirm known limitations pass.
- [ ] Confirm Release Readiness baseline passes.
- [ ] Confirm Production Content Health passes.
- [ ] Record software-license status.
- [ ] Export Dossier + Verified Tarball.
- [ ] Confirm `.tgz` exists.
- [ ] Confirm archive verification passes.
- [ ] Confirm manifest JSON exists.
- [ ] Confirm per-file SHA-256 list exists.
- [ ] Confirm `SHA256SUMS.txt` exists.
- [ ] Confirm release dossier exists.
- [ ] Confirm content-health snapshot exists.
- [ ] Confirm known-limitations copy exists.
- [ ] Confirm distribution checklist exists.
- [ ] Extract the tarball outside Unity.
- [ ] Compare its file count with the manifest.
- [ ] Install the exported tarball in a clean Unity project.
- [ ] Run Release Readiness in the clean project.
- [ ] Upgrade an existing project from 0.32.3 to the exported tarball.
- [ ] Confirm character, Contracts, inventory, audio, and Chronicles survive.
- [ ] Run External Beta audit.
- [ ] Confirm missing LICENSE blocks public distribution.
- [ ] Choose and add an approved LICENSE.
- [ ] Rerun External Beta audit.
- [ ] Preserve the dossier with the tagged commit.

## Verdict

- [ ] PASS
- [ ] CONDITIONAL PASS
- [ ] FAIL
