# Magic Bullet

**Silent Aim / Auto-Headshot mod for _How to Fish_**

Magic Bullet redirects real projectile velocity toward valid creature targets without moving your camera or crosshair.

> This mod was researched, developed, and packaged with AI assistance.  
> 本 Mod 的逆向分析、代码实现与打包流程由 AI 辅助完成。

## Features

- **Silent Aim** — redirects real projectiles without camera snapping.
- **Forced Headshots** — while Magic Bullet is enabled, eligible ranged hits from the local player are promoted into the game's headshot region.
- **Target Prediction** — movement prediction with projectile gravity compensation.
- **Sticky Target** — helps keep the same target during rapid fire.
- **Target Modes** — `Crosshair`, `Nearest`, `LowestHealth`, `BossPriority`.
- **Target Filters** — Fish, Birds, and Albatross can be enabled independently.
- **No Player Targeting** — players and teammates are never selected as targets.
- **FOV Circle & Target Line** — optional on-screen targeting visuals.
- **Optional Wall Penetration** — only authorized local Magic Bullet projectiles are affected.
- **Projectile Ownership Isolation** — NPC/Boss projectiles such as Albatross attacks are not redirected.

## Controls

| Key | Action |
| --- | --- |
| `F8` | Toggle Magic Bullet |
| `[` | Decrease FOV |
| `]` | Increase FOV |

## Target Modes

| Mode | Behavior |
| --- | --- |
| `Crosshair` | Target closest to the crosshair within FOV |
| `Nearest` | Nearest valid target |
| `LowestHealth` | Valid target with the lowest HP |
| `BossPriority` | Prioritize boss targets |

## Line of Sight vs Wall Penetration

These settings are independent:

- `RequireLineOfSight = false` — targets behind obstacles **can be acquired**.
- `WallPenetration = true` — authorized Magic Bullet projectiles **can pass through environmental obstacles** to reach the selected target.

Example:

```ini
RequireLineOfSight = false
WallPenetration = false
```

Targets behind walls can be locked, but the wall still blocks the projectile.

```ini
RequireLineOfSight = false
WallPenetration = true
```

Targets behind walls can be locked and authorized Magic Bullet projectiles can penetrate the obstacle.

Wall penetration does **not** globally disable collisions and does not apply to NPC/Boss or remote-player projectiles.

## Installation

### r2modman / Thunderstore Mod Manager

1. Install **Magic Bullet** and its **BepInExPack** dependency.
2. Launch the game using **Start modded**.
3. Press `F8` to toggle Magic Bullet.

### Manual

1. Install BepInExPack.
2. Place `HowToFishMagicBullet.dll` in:

```text
BepInEx/plugins/HowToFishMagicBullet/
```

3. Launch the game.

## Configuration

The config file is generated after first launch:

```text
BepInEx/config/com.openai.howtofish.magicbullet.cfg
```

Important options include:

```ini
Enabled = true
TargetMode = Crosshair
FOV = 35
MaxTargetDistance = 500
StickyTarget = true
RequireLineOfSight = false
WallPenetration = false

TargetFish = true
TargetBirds = true
TargetAlbatross = true

ShowFOVCircle = true
ShowTargetLine = true
DebugLogging = false
```

---

## 中文说明

**Magic Bullet** 是一个用于 _How to Fish_ 的静默自瞄 Mod。它不会拉动镜头，而是在开火时直接调整真实子弹的飞行速度，使其飞向有效目标。

主要功能：

- 静默自瞄，不改变玩家视角。
- 本地玩家远程攻击可强制进入爆头判定区域。
- 移动目标提前量与重力补偿。
- 粘性锁定。
- `Crosshair` / `Nearest` / `LowestHealth` / `BossPriority` 四种目标模式。
- 鱼类、飞鸟、信天翁独立过滤。
- **不会锁定玩家或联机队友。**
- 可显示 FOV 圆环与目标连线。
- 可选穿墙，仅作用于经过 Magic Bullet 授权的本地玩家子弹。
- 信天翁、鲸鱼等 NPC/Boss 攻击不会被 Magic Bullet 重定向。

### 视线与穿墙

- `RequireLineOfSight = false`：允许锁定墙后目标。
- `WallPenetration = true`：允许已授权的 Magic Bullet 子弹穿过环境障碍物。

两者互相独立。关闭视线检测并不等于子弹自动穿墙。

## Source / Issues

GitHub:  
https://github.com/Caijue117/HowToFish-MagicBullet
