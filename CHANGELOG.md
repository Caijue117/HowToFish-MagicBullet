# Changelog / 更新日志

All notable changes to the **How to Fish - MagicBullet** mod are documented here.  
本项目的所有重要版本更新与功能变动均记录于此。

---

## [1.2.0] - 2026-08-24

### ✨ Major Features & Targeting / 新特性与自瞄强化
- **Iterative Lead Prediction (`TargetPrediction`) / 多步迭代弹道预判**:
  - Implemented 3-step dynamic flight time refinement and movement prediction for fast-moving birds and swimming fish.
  - Added real-time ballistic gravity drop compensation.
- **Dedicated Head Resolver (`HeadResolver`) / 权威头部弱点解算**:
  - Multi-tier weakpoint resolution (`HeadTransform` -> `HeadPos` -> `HeadCollider` -> `CenterOfMass` -> `Transform`).
  - Ensures 100% reliable critical headshots on Bowhead Whales, Lava Whales, and Albatross Bosses.
- **Sticky Target System (`StickyTarget`) / 粘性锁定**:
  - Retains lock on the currently engaged target across rapid burst firing with customizable hysteresis margin (`StickyFOVMultiplier`), preventing crosshair flickering between dense targets.
- **Configurable Target Modes (`TargetMode`) / 多种自瞄模式**:
  - `Crosshair` (Default): Targets creature closest to crosshair center within FOV.
  - `Nearest`: Targets creature with closest world distance.
  - `LowestHealth`: Prioritizes lowest HP targets for fast execution.
  - `BossPriority`: Prioritizes Albatross Boss and Bowhead Whales over regular fish.
- **Target Category Filters / 目标分类开关**:
  - Added independent configuration switches for `TargetFish`, `TargetBirds`, and `TargetAlbatross`.
- **Line-of-Sight & Wall Penetration / 视线检测与物理穿墙**:
  - `RequireLineOfSight` (Default: `false`): Raycast check preventing acquisition behind terrain.
  - `WallPenetration` (Default: `false`): Selective terrain pass-through for local player's Magic Bullet projectiles.

### ⚡ Performance & Safety / 性能与稳定性优化
- **Event-Driven Target Cache (`TargetCache`) / 零内存开销缓存**:
  - Replaced expensive per-shot `FindObjectsOfType<Creature>()` scene scans with an event-driven `HashSet<Creature>` cache and low-frequency background reconciliation, eliminating GC allocations and frame drops during intense combat.
- **Shot Context Isolation (`MagicBulletShotContext`) / 发射上下文严格隔离**:
  - Strictly limits redirection to the local player's weapon fire. Boss abilities (Albatross bombs, Whale lava bursts) and remote player projectiles are completely untouched.
- **Defensive Compatibility Engine (`Compatibility`) / 防御式版本兼容**:
  - Runtime signature validation with isolated Harmony patching to prevent game crashes during game updates.
- **Modular Codebase Architecture**:
  - Refactored monolithic codebase into specialized, decoupled modules.

### 🎨 Visuals & Controls / 视觉与快捷键
- **Configurable Visuals**: Added toggle switches `ShowFOVCircle` (FOV circle) and `ShowTargetLine` (target lock ray).
- **Smooth Render Engine**: 128-segment anti-aliased FOV circle with strict on-screen viewport boundary clipping for the tracking ray.
- **Hotkey Controls**:
  - `F8`: Toggle Magic Bullet ON / OFF.
  - `[`: Decrease FOV by 5°.
  - `]`: Increase FOV by 5°.

---

## [1.1.0] - 2026-08-24
- **Silent Aim Physics Redirection**: Initial release redirecting projectile velocities without locking or moving player camera.
- **Forced Headshot Support**: Weakpoint alignment triggering headshot damage bonuses.
- **Visual FOV Indicator**: Centered 128-segment white FOV circle.
- **Target Tracking Ray**: Real-time white line from crosshair to locked target.
- **Bowhead Whale Support**: Full support for large whale boss targeting.

---

## [1.0.0] - 2026-08-24
- Initial project prototype and BepInEx 5 patch infrastructure.
