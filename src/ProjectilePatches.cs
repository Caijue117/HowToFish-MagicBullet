using System;
using HarmonyLib;
using UnityEngine;

namespace HowToFishMagicBullet
{
    // ================= Creature Lifecycle Hooks =================
    [HarmonyPatch(typeof(Creature), "Awake")]
    internal static class CreatureAwakePatch
    {
        private static void Postfix(Creature __instance)
        {
            TargetCache.Register(__instance);
        }
    }

    [HarmonyPatch(typeof(Creature), "OnStopClient")]
    internal static class CreatureOnStopClientPatch
    {
        private static void Prefix(Creature __instance)
        {
            TargetCache.Unregister(__instance);
        }
    }

    [HarmonyPatch(typeof(Item), "DestroyItem")]
    internal static class ItemDestroyItemPatch
    {
        private static void Prefix(Item __instance)
        {
            if (__instance is Creature creature)
            {
                TargetCache.Unregister(creature);
            }
        }
    }

    // ================= Weapon & Projectile Patches =================
    [HarmonyPatch(typeof(Weapon), "Shoot")]
    internal static class WeaponShootPatch
    {
        private static void Prefix(Weapon __instance)
        {
            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value)
                return;

            // Strict ownership: verify this Weapon is currently held by Player.LocalPlayer
            if (__instance == null || !__instance.Holder || !Player.LocalPlayer || __instance.Holder != Player.LocalPlayer)
                return;

            if (__instance.Holder.Owner == null || !__instance.Holder.Owner.IsLocalClient)
                return;

            // Acquire and refresh target for this authorized shot
            TargetManager.UpdateLiveTargeting();
            Creature target = TargetManager.CurrentTarget;

            MagicBulletShotContext.Begin(__instance, target);
            DebugLogger.Debug(() => $"[ProjectileGate] Authorized Weapon.Shoot context started for {__instance.name} (Target: {target?.name ?? "None"})");
        }

        private static System.Exception Finalizer(System.Exception __exception)
        {
            if (MagicBulletShotContext.IsActive)
            {
                MagicBulletShotContext.End();
                DebugLogger.Debug(() => "[ProjectileGate] Authorized Weapon.Shoot context ended.");
            }
            return __exception;
        }
    }

    [HarmonyPatch(typeof(ProjectileManager), nameof(ProjectileManager.AddProjectile), new Type[] {
        typeof(Player),
        typeof(WeaponInfo),
        typeof(bool),
        typeof(Vector3),
        typeof(Vector3),
        typeof(uint),
        typeof(uint),
        typeof(bool)
    })]
    internal static class AddProjectilePatch
    {
        private static void Prefix(
            Player owner,
            WeaponInfo weaponInfo,
            bool isLocal,
            Vector3 pos,
            ref Vector3 velocity,
            bool fromNpc)
        {
            // GATE 1: Must be executing inside an active, authorized local-player Weapon.Shoot context
            if (!MagicBulletShotContext.IsActive)
                return;

            // GATE 2: Must NOT be an NPC/boss attack and must belong to LocalPlayer
            if (fromNpc || !isLocal || !owner || !Player.LocalPlayer || owner != Player.LocalPlayer)
                return;

            if (!ModConfig.Enabled.Value)
                return;

            Creature target = MagicBulletShotContext.ActiveTarget;
            if (TargetManager.IsAllowedTarget(target))
            {
                velocity = TargetPrediction.CalculateRedirectedVelocity(pos, velocity, target, weaponInfo);
                DebugLogger.Debug(() => $"[ProjectileGate] Accepted player Weapon.Shoot projectile toward {target.name}");
            }
        }

        private static void Postfix(
            ProjectileManager __instance,
            Player owner,
            bool isLocal,
            uint id,
            bool fromNpc)
        {
            if (!MagicBulletShotContext.IsActive)
                return;

            if (fromNpc || !isLocal || !owner || !Player.LocalPlayer || owner != Player.LocalPlayer)
                return;

            if (!ModConfig.Enabled.Value)
                return;

            if (ModConfig.WallPenetration != null && ModConfig.WallPenetration.Value)
            {
                Creature target = MagicBulletShotContext.ActiveTarget;
                if (TargetManager.IsAllowedTarget(target))
                {
                    Projectile proj = Compatibility.FindSpawnedProjectile(__instance, owner, id);
                    if (proj != null)
                    {
                        ProjectileTracker.Register(proj, target);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(ProjectileManager), nameof(ProjectileManager.AddProjectiles), new Type[] {
        typeof(Player),
        typeof(WeaponInfo),
        typeof(bool),
        typeof(Vector3),
        typeof(Vector3[]),
        typeof(uint),
        typeof(uint),
        typeof(bool)
    })]
    internal static class AddProjectilesPatch
    {
        private static void Prefix(
            Player owner,
            WeaponInfo weaponInfo,
            bool isLocal,
            Vector3 pos,
            Vector3[] velocities,
            bool canHitOwner)
        {
            // GATE 1: Must be executing inside an active, authorized local-player Weapon.Shoot context
            if (!MagicBulletShotContext.IsActive)
                return;

            // GATE 2: Must belong to LocalPlayer
            if (!isLocal || !owner || !Player.LocalPlayer || owner != Player.LocalPlayer || velocities == null)
                return;

            if (!ModConfig.Enabled.Value)
                return;

            Creature target = MagicBulletShotContext.ActiveTarget;
            if (TargetManager.IsAllowedTarget(target))
            {
                for (int i = 0; i < velocities.Length; i++)
                {
                    velocities[i] = TargetPrediction.CalculateRedirectedVelocity(pos, velocities[i], target, weaponInfo);
                }
                DebugLogger.Debug(() => $"[ProjectileGate] Accepted player Weapon.Shoot multi-projectile batch ({velocities.Length}) toward {target.name}");
            }
        }

        private static void Postfix(
            ProjectileManager __instance,
            Player owner,
            bool isLocal,
            Vector3[] velocities,
            uint id,
            bool canHitOwner)
        {
            if (!MagicBulletShotContext.IsActive)
                return;

            if (!isLocal || !owner || !Player.LocalPlayer || owner != Player.LocalPlayer || velocities == null)
                return;

            if (!ModConfig.Enabled.Value)
                return;

            if (ModConfig.WallPenetration != null && ModConfig.WallPenetration.Value)
            {
                Creature target = MagicBulletShotContext.ActiveTarget;
                if (TargetManager.IsAllowedTarget(target))
                {
                    for (int i = 0; i < velocities.Length; i++)
                    {
                        uint projId = id + (uint)i;
                        Projectile proj = Compatibility.FindSpawnedProjectile(__instance, owner, projId);
                        if (proj != null)
                        {
                            ProjectileTracker.Register(proj, target);
                        }
                    }
                }
            }
        }
    }

    // ================= Wall Penetration Patches =================
    [HarmonyPatch(typeof(ProjectileManager), "UpdateProjectileScan", new Type[] { typeof(Projectile), typeof(ProjectileType) })]
    internal static class UpdateProjectileScanPatch
    {
        private static bool Prefix(
            ProjectileManager __instance,
            Projectile projectile,
            ProjectileType type)
        {
            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value)
                return true;

            if (ModConfig.WallPenetration == null || !ModConfig.WallPenetration.Value || !Compatibility.WallPenetrationSupported)
                return true;

            if (projectile == null || !projectile.IsLocal || projectile.Owner == null || projectile.Owner != Player.LocalPlayer)
                return true;

            if (!ProjectileTracker.TryGetTarget(projectile, out Creature target))
                return true; // Not a tracked penetration projectile, run normal scan

            // Custom penetration spherecast simulation
            float catchUp = Compatibility.GetCatchUpSpeed(__instance);
            float sqrMaxRange = Compatibility.GetSqrMaxProjRange(__instance);
            float num = 1f + (float)projectile.CatchingUpToDo * catchUp;

            if (!projectile.Owner || (projectile.Position - projectile.Owner.Transform.position).sqrMagnitude > sqrMaxRange)
            {
                Compatibility.CallAddToRemoveQueue(__instance, projectile);
                return false;
            }

            if (projectile.Position.y < WaterManager.WaterHeight - 0.5f)
            {
                Compatibility.CallAddToRemoveQueue(__instance, projectile);
                Compatibility.CallHitWater(__instance, projectile);
                return false;
            }

            float stepDist = projectile.Velocity.magnitude * num * Time.fixedDeltaTime;
            LayerMask layerMask = GameInfo.ProjectileHitLayer;

            RaycastHit[] hits = Physics.SphereCastAll(projectile.Position, type.WidthRadius, projectile.Velocity, stepDist, layerMask);
            if (hits != null && hits.Length > 0)
            {
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (!hit.collider)
                        continue;

                    Item hitItem = ItemManager.Get(hit.collider);
                    bool isTargetHit = (hitItem != null && hitItem == target) || hit.collider.transform.IsChildOf(target.transform);
                    if (isTargetHit)
                    {
                        DebugLogger.Debug(() => $"[WallPenetration] Projectile {projectile.Id} reached target {target.name} through wall!");
                        ProjectileTracker.Unregister(projectile);
                        Compatibility.CallHit(__instance, projectile, type, hit);
                        return false;
                    }
                }

                // Environment or non-target obstacles were hit: Ignore them and keep projectile alive!
                DebugLogger.Debug(() => $"[WallPenetration] Projectile {projectile.Id} penetrated environmental obstruction towards {target.name}.");
                return false;
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(ProjectileManager), "HitScan", new Type[] { typeof(Projectile), typeof(ProjectileType) })]
    internal static class HitScanPatch
    {
        private static bool Prefix(
            ProjectileManager __instance,
            Projectile projectile,
            ProjectileType type,
            ref float __result)
        {
            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value)
                return true;

            if (ModConfig.WallPenetration == null || !ModConfig.WallPenetration.Value || !Compatibility.WallPenetrationSupported)
                return true;

            if (projectile == null || !projectile.IsLocal || projectile.Owner == null || projectile.Owner != Player.LocalPlayer)
                return true;

            if (!ProjectileTracker.TryGetTarget(projectile, out Creature target))
                return true;

            RaycastHit[] hits = Physics.SphereCastAll(projectile.Position, type.WidthRadius, projectile.Velocity, float.PositiveInfinity, GameInfo.ProjectileHitLayer);
            if (hits != null && hits.Length > 0 && !type.ProjectilesToRemove.Contains(projectile))
            {
                Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

                for (int i = 0; i < hits.Length; i++)
                {
                    RaycastHit hit = hits[i];
                    if (!hit.collider)
                        continue;

                    Item hitItem = ItemManager.Get(hit.collider);
                    bool isTargetHit = (hitItem != null && hitItem == target) || hit.collider.transform.IsChildOf(target.transform);
                    if (isTargetHit)
                    {
                        DebugLogger.Debug(() => $"[WallPenetration] Hitscan projectile reached target {target.name} through wall!");
                        ProjectileTracker.Unregister(projectile);
                        Compatibility.CallHit(__instance, projectile, type, hit);
                        __result = 0f;
                        return false;
                    }
                }
            }

            return true;
        }
    }

    [HarmonyPatch(typeof(ProjectileManager), "AddToRemoveQueue", new Type[] { typeof(Projectile) })]
    internal static class AddToRemoveQueuePatch
    {
        private static void Postfix(Projectile projectile)
        {
            ProjectileTracker.Unregister(projectile);
        }
    }

    // ================= Headshot Multiplier Patch =================
    [HarmonyPatch(typeof(Creature), "LocalHit")]
    internal static class CreatureLocalHitPatch
    {
        private static void Prefix(
            Creature __instance,
            ref Vector3 point,
            Player player,
            ref int damage,
            bool rangedHit)
        {
            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value)
                return;

            if (!player || !Player.LocalPlayer || player != Player.LocalPlayer)
                return;

            if (!rangedHit)
                return;

            if (!TargetManager.IsAllowedTarget(__instance))
                return;

            // Ensure hit point is within head region (local Z > HeadPos)
            float localZ = __instance.transform.InverseTransformPoint(point).z;
            if (localZ <= __instance.HeadPos)
            {
                Vector3 localPoint = __instance.transform.InverseTransformPoint(point);
                localPoint.z = __instance.HeadPos + 0.15f;
                point = __instance.transform.TransformPoint(localPoint);
            }
        }
    }
}
