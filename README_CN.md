# How To Fish — Magic Bullet

针对你提供的 `Assembly-CSharp.dll` 编写的 BepInEx 5 / Harmony Mod 源码。

## 当前功能

- **Magic Bullet / Silent Aim**：镜头不会自动转向目标，只修改游戏本身传给 `ProjectileManager` 的弹丸速度向量。
- **严格目标白名单**：
  - `Fish`
  - `Bird`（海鸥）
- **不会把玩家/队友作为锁定目标**。
- **可调 FOV**：这里的 FOV 是“准星中心到目标的最大夹角（半径）”。
- 默认：
  - `F8`：开关 Magic Bullet
  - `[`：FOV -5°
  - `]`：FOV +5°
- 多弹丸武器会逐颗重定向。
- 会根据目标 Rigidbody 速度做移动预测。
- 会根据游戏 `ProjectileGravity` 做弹道下坠补偿。
- HUD 左上角显示状态、FOV 和当前目标类型。

## 已根据上传 DLL 确认的实际游戏接口

`Weapon.Shoot()` 最后调用：

```csharp
ProjectileManager.AddProjectile(
    Player owner,
    WeaponInfo weaponInfo,
    bool isLocal,
    Vector3 pos,
    Vector3 velocity,
    uint catchingUpToDo,
    uint id,
    bool fromNpc
);

ProjectileManager.AddProjectiles(
    Player owner,
    WeaponInfo weaponInfo,
    bool isLocal,
    Vector3 pos,
    Vector3[] velocities,
    uint catchingUpToDo,
    uint id,
    bool canHitOwner
);
```

目标类型继承关系也已确认：

```text
Item
└─ Creature
   ├─ Fish
   └─ Bird
```

因此源码不是用 Tag 猜目标，而是直接以 `Fish || Bird` 作为目标白名单。

## 编译

前提：

1. 游戏里已装 **BepInEx 5**。
2. Windows 已安装 .NET SDK。
3. 打开 PowerShell，进入本源码目录。

Steam 默认路径可直接：

```powershell
.\build.ps1
```

如果游戏不在默认 Steam 目录：

```powershell
.\build.ps1 -GameDir "D:\SteamLibrary\steamapps\common\How to Fish"
```

脚本编译成功后会自动复制到：

```text
How to Fish\BepInEx\plugins\HowToFishMagicBullet\HowToFishMagicBullet.dll
```

## 配置

第一次运行后会生成：

```text
BepInEx\config\com.openai.howtofish.magicbullet.cfg
```

主要参数：

```ini
[MagicBullet]
Enabled = true

[Targeting]
FOV = 35
FOVStep = 5
MaxPredictionTime = 2

[Hotkeys]
Toggle = F8
FOVDecrease = LeftBracket
FOVIncrease = RightBracket

[UI]
ShowHUD = true
```

### FOV 的含义

例如：

- `FOV = 5`：基本必须贴近准星才会锁。
- `FOV = 30`：准星周围 30° 内的鱼/海鸥。
- `FOV = 90`：整个正面半球附近都很容易锁。
- `FOV = 180`：方向基本不再构成限制。

候选目标不止一个时，优先选择**最靠近准星中心**的目标，再用距离打破并列。

## 版本说明

本源码是按用户上传的 `Assembly-CSharp.dll` 静态检查后生成。

上传 DLL SHA-256：

```text
89ab30e5ffe478c286aa10cf466a2bac612b21b5f966a9ceec2c0dd3b0a482c5
```

如果游戏更新后 `Weapon.Shoot` 或 `ProjectileManager.AddProjectile(s)` 的签名发生变化，需要重新适配。
