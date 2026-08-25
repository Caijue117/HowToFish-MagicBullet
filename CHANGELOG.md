# Changelog

## 1.2.2 - 2026-08-25

### Fixed
- Corrected files accidentally included in the previous release.
- Updated the release package with the intended documentation and assets.

## 1.2.1 - 2026-08-25

### Changed
- Re-verified compatibility and refreshed local references against the latest *How to Fish* game update.
- Updated and refined documentation with aligned bilingual instructions and controls.

## 1.2.0 - 2026-08-24

### Added
- Added movement prediction and projectile gravity compensation.
- Added Sticky Target.
- Added Crosshair, Nearest, LowestHealth, and BossPriority target modes.
- Added independent Fish, Bird, and Albatross target filters.
- Added optional Line-of-Sight checks and Wall Penetration.
- Added FOV circle and target tracking line options.
- Added configurable target distance and debug logging.

### Improved
- Improved head/weak-point targeting.
- Improved target caching and performance.
- Improved projectile tracking and cleanup.
- Improved rapid-fire and multi-projectile weapon support.
- Improved Harmony startup resilience.

### Fixed
- Fixed Albatross and other NPC/Boss projectiles being incorrectly redirected by Magic Bullet.
- Fixed local-player projectile ownership detection.
- Fixed target lifecycle issues after creature death or despawn.
- Fixed startup failures caused by invalid lifecycle patches.

## 1.1.0 - 2026-08-24

- Initial public release.
- Added Silent Aim projectile redirection.
- Added forced headshots.
- Added FOV circle and target tracking line.
- Added Bowhead Whale targeting support.
