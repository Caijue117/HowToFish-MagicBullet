# How to Fish Magic Bullet Mod

## Project

This is a BepInEx 5 + Harmony C# mod for How to Fish.

Target runtime:
- Unity Mono
- BepInEx 5
- .NET Standard 2.1
- Compile using .NET 8 SDK

## Main feature

Implement Magic Bullet / Silent Aim.

Requirements:

- Do NOT rotate the player's camera.
- Do NOT modify player aim visually.
- Redirect actual projectile velocity.
- Target Fish and Bird only.
- Never target Player objects.
- Never target teammates.
- FOV must be configurable.
- Prefer target nearest to crosshair center.
- Preserve original projectile speed and damage.
- Support weapons with multiple projectiles.
- Account for projectile gravity.
- Prefer movement prediction for moving Fish/Bird targets.

## Important game classes already identified

- Weapon
- WeaponInfo
- Projectile
- ProjectileManager
- Creature
- Fish
- Bird
- Player
- PlayerAimAssist

Weapon.Shoot() ultimately calls:

ProjectileManager.AddProjectile(...)
ProjectileManager.AddProjectiles(...)

Magic Bullet should currently be implemented by patching projectile
velocity rather than moving the camera.

Fish and Bird inherit Creature.

Players must never be included in targeting.

## Source

Main source:

src/MagicBulletPlugin.cs

## References

Game assemblies:

references/Game/

BepInEx assemblies:

references/BepInEx/

Never edit DLL files in references/.

They are read-only compile/reference material.

## Build

Use .NET 8.

Do not upgrade the project to .NET 10.

The user's Windows environment previously produced a CET/Roslyn error
with .NET 10.

Build:

dotnet build -c Release

Expected output:

bin/Release/netstandard2.1/HowToFishMagicBullet.dll

## Deployment

r2modman profile:

C:\Users\Administrator\AppData\Roaming\r2modmanPlus-local\HowToFish\profiles\Default

Runtime plugin destination:

BepInEx\plugins\HowToFishMagicBullet\HowToFishMagicBullet.dll

Do not modify other installed mods.

Do not replace or edit the game's original Assembly-CSharp.dll.

## Current controls

F8 = enable/disable Magic Bullet

[ = reduce FOV

] = increase FOV

## Development rules

Before changing Harmony patches:

1. Inspect the supplied Assembly-CSharp.dll.
2. Verify actual method signatures.
3. Do not guess method names.
4. Build after changes.
5. Fix all compiler errors before deployment.
6. Keep changes scoped to this mod.