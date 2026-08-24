# Changelog

## [1.2.0] - 2026-08-24

### Added
- **Target Cache System (`TargetCache`)**: Replaced per-shot full scene scanning with an event-driven `HashSet<Creature>` cache and low-frequency fallback reconciliation for zero-allocation targeting.
- **Dedicated Head Resolver (`HeadResolver`)**: Multi-tier weak-point resolution hierarchy prioritizing Head Transform -> `HeadPos` -> Head Collider -> Center of Mass -> Transform.
- **Iterative Interception Prediction (`TargetPrediction`)**: Multi-step flight-time refinement algorithm with target velocity prediction and ballistic gravity drop compensation.
- **Sticky Target System**: Added `StickyTarget` and `StickyFOVMultiplier` to prevent target flickering during rapid fire while providing smooth hysteresis retention.
- **Line-of-Sight & Wall Penetration**:
  - `RequireLineOfSight` (optional): Raycast-based environmental visibility checking.
  - `WallPenetration` (optional): Per-projectile tracking allowing local player Magic Bullet shots to pass through terrain and environmental geometry to hit the locked target.
- **Target Selection Modes (`TargetMode`)**:
  - `Crosshair` (default): Closest to crosshair within FOV.
  - `Nearest`: Closest world distance.
  - `LowestHealth`: Target with lowest current HP.
  - `BossPriority`: Prioritizes Albatross and bosses over standard creatures.
- **Target Category Filters**: Independent toggles for `TargetFish`, `TargetBirds`, and `TargetAlbatross`.
- **Debug Logging (`DebugLogging`)**: Zero-overhead conditional diagnostic logging.
- **Defensive Compatibility Layer (`Compatibility`)**: Validates game method signatures at startup to prevent game crashes upon game updates.
- **Codebase Modular Refactor**: Separated monolithic architecture into dedicated, testable modules.

---

## [1.1.0] - 2026-08-24
- Initial release with Magic Bullet / Silent Aim, forced headshots, 128-segment on-screen FOV circle, locked target tracking ray, and Bowhead Whale support.
