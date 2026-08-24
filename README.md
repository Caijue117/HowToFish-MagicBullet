# Magic Bullet (Silent Aim & Auto-Headshot)

> **Notice / 声明**:  
> **[EN]** This mod was fully researched, designed, coded, and packaged with AI assistance.  
> **[中文]** 本 Mod 的逆向分析、代码编写、算法实现与打包流程均由 AI 辅助生成。

---

## 📖 English Description

A mod for **How to Fish** that implements **Magic Bullet / Silent Aim**, **forced headshot/weakpoint targeting**, **projectile gravity compensation**, **dynamic movement prediction**, **on-screen FOV circle rendering**, and **real-time target lock-ray tracking**.

### ✨ Features
- **Silent Aim**: **Never** snaps or moves the player's camera, leaving your view completely free. Projectile velocities are redirected in real time towards the target upon firing.
- **Forced Headshots**: Automatically resolves the weak-point / head position (`HeadPos`), ensuring hits directly strike the head and trigger headshot multipliers and bonuses.
- **On-Screen FOV Circle**: Centered display indicating the current active targeting FOV radius.
- **Target Lock Ray**: When a valid target is locked, draws a 2px white line from the crosshair directly to the target's head weak point (only within visible screen bounds).
- **Target Filtering & Modes**: Configurable strategies (`Crosshair`, `Nearest`, `LowestHealth`, `BossPriority`) with independent category filters for Fish, Birds, and Albatross.
- **Sticky Targeting**: Smooth target hysteresis retention across shots.
- **Optional Wall Penetration & LOS**: Optional settings for line-of-sight checks and selective terrain/environmental penetration.

### 🎮 Controls
| Hotkey | Action |
| :--- | :--- |
| **`F8`** | Toggle Magic Bullet **ON / OFF** |
| **`[`** | Decrease Targeting FOV by 5° |
| **`]`** | Increase Targeting FOV by 5° |

### 📦 Installation
#### Method 1: r2modman / Thunderstore Mod Manager (Recommended)
1. Install and open [r2modman](https://thunderstore.io/c/how-to-fish/).
2. Select **How to Fish** and your profile.
3. Click **Install with Mod Manager** on the mod page.
4. Click **Start modded** to launch the game.

#### Method 2: Manual Installation
1. Install [BepInExPack](https://thunderstore.io/c/how-to-fish/p/BepInEx/BepInExPack/).
2. Place `HowToFishMagicBullet.dll` into your `BepInEx\plugins\HowToFishMagicBullet\` directory.
3. Launch the game.

### ⚙️ Configuration
Generated in `BepInEx/config/com.openai.howtofish.magicbullet.cfg` after first launch:
- `FOV` (Default: `35.0°`)
- `FOVStep` (Default: `5.0°`)
- `ShowFOVCircle` (Default: `true`)
- `ShowTargetLine` (Default: `true`)
- `TargetMode` (Default: `Crosshair`)
- `StickyTarget` (Default: `true`)
- `RequireLineOfSight` (Default: `false`)
- `WallPenetration` (Default: `false`)
- `MaxPredictionTime` (Default: `2.0s`)
- Customizable hotkeys

---

## 📖 中文说明

专为 **How to Fish** 开发的模组，实现了真正的 **Magic Bullet（静默自瞄 / 子弹重定向）**、**强制头部弱点锁定**、**重力弹道补偿**、**动态速度预判**、**屏幕 FOV 准星圆环渲染** 与 **实时目标锁定射线追踪**。

### ✨ 主要特性
- **静默自瞄 (Silent Aim)**：**绝不**强行转动或吸附玩家镜头，视角完全自由。在发射瞬间由物理引擎实时重定向子弹飞行速度矢量。
- **强制爆头 / 弱点打击**：自动解析生物局部坐标系的头部弱点位置（`HeadPos`），弹道直击头部，必定触发游戏内的爆头伤害倍率与 Headshot 击杀加成。
- **屏幕 FOV 范围圆环**：居中显示当前有效自瞄角度范围（可通过配置开关）。
- **目标锁定射线**：当有目标被锁定时，自准心向目标头部弱点绘制白色追踪线（仅在屏幕可视范围内渲染）。
- **多模式与分类过滤**：支持十字准星最近、世界距离最近、最低血量优先、Boss优先；支持鱼类/飞鸟/信天翁独立开关。
- **粘性锁定 (Sticky Target)**：连发开火时自动维持当前锁定目标，防止多目标间准心跳动。
- **可选视线检测与穿墙**：支持可选的 LOS 视线遮挡检测与物理弹道穿墙模式。

### 🎮 快捷键操作
| 按键 | 功能说明 |
| :--- | :--- |
| **`F8`** | 开关 Magic Bullet 功能（开启 / 关闭） |
| **`[`** | 减小自瞄 FOV 角度（每次 -5°） |
| **`]`** | 增大自瞄 FOV 角度（每次 +5°） |

### 📦 安装方式
#### 方式一：r2modman / Thunderstore Mod Manager（推荐）
1. 安装并打开 [r2modman](https://thunderstore.io/c/how-to-fish/).
2. 选择 **How to Fish** 及对应 Profile。
3. 在 Mod 页面点击 **Install with Mod Manager**。
4. 点击 **Start modded** 启动游戏。

#### 方式二：手动安装
1. 安装游戏对应的 [BepInExPack](https://thunderstore.io/c/how-to-fish/p/BepInEx/BepInExPack/)。
2. 将 `HowToFishMagicBullet.dll` 放入 `BepInEx\plugins\HowToFishMagicBullet\` 目录下。
3. 启动游戏。

### ⚙️ 配置文件
运行一次后可在 `BepInEx/config/com.openai.howtofish.magicbullet.cfg` 中修改配置：
- `FOV`（默认 `35.0°`）
- `FOVStep`（默认 `5.0°`）
- `ShowFOVCircle`（默认 `true`，是否显示准星圆环）
- `ShowTargetLine`（默认 `true`，是否显示目标锁定连线）
- `TargetMode`（默认 `Crosshair`）
- `StickyTarget`（默认 `true`）
- `RequireLineOfSight`（默认 `false`）
- `WallPenetration`（默认 `false`）
- `MaxPredictionTime`（预判时长上限，默认 `2.0s`）
- 各项自定义快捷键
