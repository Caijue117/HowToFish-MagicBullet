# Changelog

## [1.2.0] - 2026-08-24

### Added
- **Target Cache System (`TargetCache`)**: Replaced per-shot scene scanning with an event-driven `HashSet<Creature>` cache and low-frequency fallback reconciliation for zero-allocation targeting.
- **Dedicated Head Resolver (`HeadResolver`)**: Multi-tier weak-point resolution hierarchy prioritizing Head Transform -> `HeadPos` -> Head Collider -> Center of Mass -> Transform.
- **Iterative Interception Prediction (`TargetPrediction`)**: Multi-step flight-time refinement algorithm with target velocity prediction and ballistic gravity drop compensation.
- **Sticky Target System**: Added `StickyTarget` and `StickyFOVMultiplier` to prevent target flickering during rapid fire while providing smooth hysteresis retention.
- **Target Selection Modes (`TargetMode`)**: Added configurable strategies (`Crosshair`, `Nearest`, `LowestHealth`, `BossPriority`).
- **Target Category Filters**: Independent toggles for `TargetFish`, `TargetBirds`, and `TargetAlbatross`.
- **Line-of-Sight & Wall Penetration**:
  - `RequireLineOfSight` (optional): Raycast-based environmental visibility checking.
  - `WallPenetration` (optional): Per-projectile tracking allowing local player Magic Bullet shots to pass through terrain and environmental geometry to hit the locked target.
- **Visual Options**: Added `ShowFOVCircle` and `ShowTargetLine` config options with strict on-screen bounds clipping.
- **Projectile Ownership & Shot Context (`MagicBulletShotContext`)**: Implemented strict local-player weapon shooting context gating to prevent Albatross poop attacks, whale lava bursts, NPC projectiles, and remote players from being accidentally redirected or tracked.
- **Defensive Compatibility Layer (`Compatibility`)**: Validates game method signatures at startup with isolated patch error handling to prevent game crashes upon game updates.
- **Debug Logging (`DebugLogging`)**: Zero-overhead conditional diagnostic logging.
- **Codebase Modular Refactor**: Separated monolithic architecture into dedicated, testable modules.

---

## [1.1.0] - 2026-08-24
- Initial release with Magic Bullet / Silent Aim, forced headshots, 128-segment on-screen FOV circle, locked target tracking ray, and Bowhead Whale support.
