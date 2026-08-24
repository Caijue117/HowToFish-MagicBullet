# Magic Bullet (Silent Aim & Auto-Headshot)

> **Notice / 声明**:  
> **[EN]** This mod was fully researched, designed, coded, and packaged with AI assistance.  
> **[中文]** 本 Mod 的逆向分析、代码编写、算法实现与打包流程均由 AI 辅助生成。

---

## 📖 English Description

**Magic Bullet** is a high-performance BepInEx 5 mod for **How to Fish** that implements true **Silent Aim / Projectile Velocity Redirection**, **forced headshot targeting**, **iterative movement prediction**, **ballistic gravity compensation**, **sticky targeting**, **configurable target-selection strategies**, **independent category filtering**, and **optional selective wall penetration**.

Source code and issue tracking are hosted on GitHub:  
👉 **[https://github.com/Caijue117/HowToFish-MagicBullet](https://github.com/Caijue117/HowToFish-MagicBullet)**

### ✨ Features

- **Silent Aim (Magic Bullet)**: **Never** snaps, shakes, or forces player camera movement. View and crosshair control remain 100% free. Projectile velocities are physically redirected in real time towards the target upon firing.
- **Forced Headshots / Weak-Point Targeting**: Automatically resolves the creature's authoritative head position (`HeadPos`). While Magic Bullet is enabled, ranged hits attributed to the local player on allowed creature targets are promoted into the game's headshot region, consistently triggering headshot damage multipliers and headshot killscore bonuses.
- **Strict Target Whitelist (Zero Player Targeting)**:
  - **Fish** (Normal Fish, Attacking Fish/Sharks, Pufferfish, Running Fish, Piranha, Bowhead Whale, and Volcano Mutated Whale Boss)
  - **Birds** (Seagulls)
  - **Albatross** (Albatross Boss)
  - **Players and teammates are strictly excluded and never targeted.**
- **Iterative Interception Prediction**: 3-step iterative lead calculation factoring in target point velocity and weapon projectile gravity drop.
- **Sticky Target System**: Retains the currently locked target across rapid-fire bursts using configurable hysteresis (`StickyFOVMultiplier`), preventing target jumping.
- **Multiple Target-Selection Modes**:
  - `Crosshair` (Default): Selects the target closest to the center crosshair within FOV.
  - `Nearest`: Selects the target with the shortest 3D world distance (ignores FOV limit).
  - `LowestHealth`: Prioritizes targets with the lowest current HP.
  - `BossPriority`: Prioritizes Albatross and Whale Bosses over regular creatures.
- **Independent Category Filters**: Independently enable or disable targeting for `TargetFish`, `TargetBirds`, and `TargetAlbatross`.
- **On-Screen FOV Circle & Target Line**:
  - Smooth 128-segment white circular outline centered on screen displaying the effective targeting radius.
  - 2px white tracking line from the crosshair to the exact resolved head aim point (clipped to visible on-screen bounds).
- **Projectile Ownership Isolation**: Protected by `MagicBulletShotContext` to ensure NPC attacks, boss projectiles (e.g. Albatross poop bombs, whale lava), and remote player shots are never redirected or tracked.

### 🧱 Line-of-Sight vs. Wall Penetration

The mod provides two independent settings for handling obstacles:

- **`RequireLineOfSight` (Default: `false`)**: Controls **target acquisition**.
  - When `false`, Magic Bullet can acquire and lock onto creatures hidden behind terrain, walls, or boat hulls.
  - When `true`, targets behind obstacles cannot be locked.
- **`WallPenetration` (Default: `false`)**: Controls **projectile physics simulation**.
  - When `false`, projectiles follow standard physics and will stop when colliding with environmental geometry (terrain, rocks, buildings, boat hulls).
  - When `true`, authorized Magic Bullet projectiles fired by the local player will selectively pass through environmental obstacles until reaching the locked creature target.
  - **Note**: `WallPenetration` does **not** globally disable collisions; NPC projectiles and ordinary player bullets continue colliding with the environment normally.

| Setting Combination | Behavior |
| :--- | :--- |
| `RequireLineOfSight = false`<br>`WallPenetration = false` *(Default)* | Targets behind walls **can** be acquired and tracked, but projectiles are stopped by walls normally. |
| `RequireLineOfSight = false`<br>`WallPenetration = true` | Targets behind walls **can** be acquired and authorized projectiles **penetrate** environmental obstacles to hit the target. |
| `RequireLineOfSight = true`<br>`WallPenetration = false` | Targets hidden behind obstacles **cannot** be acquired. |

### 🎮 Controls

| Hotkey | Action |
| :--- | :--- |
| **`F8`** | Toggle Magic Bullet **ON / OFF** |
| **`[`** | Decrease Targeting FOV radius by 5° |
| **`]`** | Increase Targeting FOV radius by 5° |

### 📦 Installation

#### Method 1: Thunderstore Mod Manager / r2modman (Recommended)
1. Install [r2modman](https://thunderstore.io/c/how-to-fish/).
2. Select **How to Fish** and your profile.
3. Click **Install with Mod Manager** on the mod page.
4. Launch the game using **Start modded**.

#### Method 2: Manual Installation
1. Install [BepInExPack](https://thunderstore.io/c/how-to-fish/p/BepInEx/BepInExPack/).
2. Extract `HowToFishMagicBullet.dll` into your `BepInEx\plugins\HowToFishMagicBullet\` folder.
3. Launch the game.

### ⚙️ Configuration

Configuration is automatically generated in `BepInEx/config/com.openai.howtofish.magicbullet.cfg` after first launch:

| Section | Key | Default | Description |
| :--- | :--- | :--- | :--- |
| `[MagicBullet]` | `Enabled` | `true` | Master switch for Magic Bullet / Silent Aim. |
| `[Visuals]` | `ShowFOVCircle` | `true` | Render the centered white FOV targeting circle on screen. |
| `[Visuals]` | `ShowTargetLine` | `true` | Render the white tracking line from crosshair to locked target head point. |
| `[Targeting]` | `TargetMode` | `Crosshair` | Strategy: `Crosshair`, `Nearest`, `LowestHealth`, `BossPriority`. |
| `[Targeting]` | `FOV` | `35.0` | Maximum angular targeting radius from crosshair center (degrees). |
| `[Targeting]` | `FOVStep` | `5.0` | FOV change per hotkey press (degrees). |
| `[Targeting]` | `MaxTargetDistance`| `500.0` | Maximum distance in meters to acquire targets. |
| `[Targeting]` | `MaxPredictionTime`| `2.0` | Maximum ballistic lead prediction time in seconds. |
| `[Targeting]` | `StickyTarget` | `true` | Retain current target across shots while valid and within sticky FOV. |
| `[Targeting]` | `StickyFOVMultiplier`| `1.25` | FOV multiplier for retaining existing sticky target (hysteresis). |
| `[Targeting]` | `RequireLineOfSight`| `false` | If `true`, targets blocked by environment cannot be acquired. |
| `[Targeting]` | `WallPenetration` | `false` | If `true`, authorized local player projectiles penetrate terrain to hit target. |
| `[Filtering]` | `TargetFish` | `true` | Allow targeting Fish and Whales. |
| `[Filtering]` | `TargetBirds` | `true` | Allow targeting Birds (Seagulls). |
| `[Filtering]` | `TargetAlbatross` | `true` | Allow targeting Albatross Boss. |
| `[Hotkeys]` | `Toggle` | `F8` | Hotkey to toggle Magic Bullet ON/OFF. |
| `[Hotkeys]` | `FOVDecrease` | `LeftBracket` | Hotkey to decrease FOV. |
| `[Hotkeys]` | `FOVIncrease` | `RightBracket` | Hotkey to increase FOV. |
| `[Diagnostics]` | `DebugLogging` | `false` | Detailed console diagnostic logging. |

---

## 📖 中文说明

**Magic Bullet** 是专为 **How to Fish** 开发的高性能 BepInEx 5 模组，实现了真正的 **静默自瞄 (Silent Aim / 子弹物理重定向)**、**强制头部弱点打击**、**多步迭代提前量预判**、**重力弹道补偿**、**粘性目标锁定**、**多种自瞄选择模式**、**独立物种过滤** 与 **可选物理穿墙**。

项目开源仓库与反馈地址：  
👉 **[https://github.com/Caijue117/HowToFish-MagicBullet](https://github.com/Caijue117/HowToFish-MagicBullet)**

### ✨ 主要特性

- **静默自瞄 (Silent Aim)**：**绝不**强行吸附或转动玩家镜头，视角完全自由。在开火瞬间由物理引擎实时重定向子弹飞行速度矢量。
- **强制爆头 / 弱点打击**：自动解算生物的权威头部弱点坐标（`HeadPos`）。开启 Magic Bullet 期间，本地玩家对合法生物造成的远程命中将必定落在游戏原版爆头判定区，稳定触发爆头伤害倍率与击杀加分。
- **严格目标白名单（绝不锁定玩家）**：
  - **鱼类**（普通鱼、攻击性鱼类/鲨鱼、河豚、长腿陆行鱼、水虎鱼、弓头鲸及火山熔岩变异弓头鲸 Boss）
  - **飞鸟**（海鸥）
  - **信天翁**（Albatross Boss）
  - **完全排除所有玩家与联机队友，绝对不会锁定玩家。**
- **多步迭代弹道预判**：采用 3 步迭代逐步逼近算法，综合计算目标移动速度与武器重力下坠补偿。
- **粘性锁定 (Sticky Target)**：连发开火时自动维持当前目标（滞后余量 `StickyFOVMultiplier`），彻底消除多目标间准心跳动。
- **多种目标选择模式 (`TargetMode`)**：
  - `Crosshair`（默认）：优先锁定距离准星中心角度最近的目标。
  - `Nearest`：忽略 FOV 限制，优先锁定全图三维世界距离最近的目标。
  - `LowestHealth`：优先锁定当前生命值最低的残血目标。
  - `BossPriority`：优先锁定信天翁与弓头鲸 Boss。
- **独立生物类别过滤**：支持独立开关 `TargetFish`、`TargetBirds`、`TargetAlbatross`。
- **屏幕准星圆环与追踪射线**：
  - 128 段平滑抗锯齿白色圆环，居中直观展示有效自瞄角度范围。
  - 准心指向目标头部弱点的 2px 白色锁定连线（严格限制在屏幕可视范围内渲染，不乱拉线）。
- **发射上下文安全隔离 (`MagicBulletShotContext`)**：严格限制仅本地玩家武器射击生效，绝不重定向信天翁投弹、鲸鱼喷火等 NPC/Boss 攻击。

### 🧱 视线检测 (Line-of-Sight) 与 穿墙模式 (Wall Penetration) 区别

本模组提供两项独立的障碍物控制配置：

- **`RequireLineOfSight`（默认 `false`）**：控制**目标锁定发现**。
  - 为 `false` 时，允许锁定被小岛地形、石头或船体遮挡的水下/障碍物后生物；
  - 为 `true` 时，视线被遮挡的目标无法被锁定。
- **`WallPenetration`（默认 `false`）**：控制**子弹物理穿透**。
  - 为 `false` 时，子弹遵守原版碰撞，撞击地形和障碍物会正常被阻挡；
  - 为 `true` 时，仅限本地玩家受 Magic Bullet 锁定的子弹能选择性穿透地形/障碍物直达目标；
  - **注意**：穿墙功能**不会**全局关闭碰撞，NPC 子弹与其他未锁定射击仍会正常被墙壁阻挡。

### 🎮 快捷键操作

| 按键 | 功能说明 |
| :--- | :--- |
| **`F8`** | 开关 Magic Bullet 功能（开启 / 关闭） |
| **`[`** | 减小自瞄 FOV 角度（每次 -5°） |
| **`]`** | 增大自瞄 FOV 角度（每次 +5°） |

### 📦 安装方式

#### 方式一：r2modman / Thunderstore Mod Manager（推荐）
1. 安装并打开 [r2modman](https://thunderstore.io/c/how-to-fish/)。
2. 选择 **How to Fish** 及对应 Profile。
3. 在 Mod 页面点击 **Install with Mod Manager**。
4. 点击 **Start modded** 启动游戏。

#### 方式二：手动安装
1. 安装游戏对应的 [BepInExPack](https://thunderstore.io/c/how-to-fish/p/BepInEx/BepInExPack/)。
2. 将 `HowToFishMagicBullet.dll` 放入 `BepInEx\plugins\HowToFishMagicBullet\` 目录下。
3. 启动游戏。
