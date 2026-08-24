using System;
using System.Collections.Generic;
using System.Reflection;
using FishNet.Connection;
using HarmonyLib;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class Compatibility
    {
        public static bool IsInitialized { get; private set; }
        public static bool WallPenetrationSupported { get; private set; }

        // Cached reflection delegates for private ProjectileManager members
        private static MethodInfo _hitMethod;
        private static MethodInfo _hitWaterMethod;
        private static MethodInfo _addToRemoveQueueMethod;
        private static FieldInfo _playerProjectilesField;
        private static FieldInfo _catchUpSpeedField;
        private static FieldInfo _sqrMaxProjRangeField;

        public static void Initialize()
        {
            try
            {
                Type pmType = typeof(ProjectileManager);

                _hitMethod = AccessTools.Method(pmType, "Hit", new Type[] { typeof(Projectile), typeof(ProjectileType), typeof(RaycastHit) });
                _hitWaterMethod = AccessTools.Method(pmType, "HitWater", new Type[] { typeof(Projectile) });
                _addToRemoveQueueMethod = AccessTools.Method(pmType, "AddToRemoveQueue", new Type[] { typeof(Projectile) });
                _playerProjectilesField = AccessTools.Field(pmType, "_playerProjectiles");
                _catchUpSpeedField = AccessTools.Field(pmType, "_catchUpSpeed");
                _sqrMaxProjRangeField = AccessTools.Field(pmType, "_sqrMaxProjRange");

                if (_hitMethod != null && _playerProjectilesField != null)
                {
                    WallPenetrationSupported = true;
                    DebugLogger.LogInfo("[Compatibility] ProjectileManager methods verified successfully. Wall Penetration is supported.");
                }
                else
                {
                    WallPenetrationSupported = false;
                    DebugLogger.LogWarning("[Compatibility] ProjectileManager expected members were not found. Wall Penetration disabled for safety.");
                }

                IsInitialized = true;
            }
            catch (Exception ex)
            {
                WallPenetrationSupported = false;
                DebugLogger.LogError($"[Compatibility] Validation failed with exception: {ex}");
            }
        }

        public static void DisableWallPenetration()
        {
            WallPenetrationSupported = false;
        }

        public static void CallHit(ProjectileManager pm, Projectile projectile, ProjectileType type, RaycastHit hit)
        {
            if (_hitMethod != null && pm != null)
            {
                _hitMethod.Invoke(pm, new object[] { projectile, type, hit });
            }
        }

        public static void CallHitWater(ProjectileManager pm, Projectile projectile)
        {
            if (_hitWaterMethod != null && pm != null)
            {
                _hitWaterMethod.Invoke(pm, new object[] { projectile });
            }
        }

        public static void CallAddToRemoveQueue(ProjectileManager pm, Projectile projectile)
        {
            if (_addToRemoveQueueMethod != null && pm != null)
            {
                _addToRemoveQueueMethod.Invoke(pm, new object[] { projectile });
            }
        }

        public static float GetCatchUpSpeed(ProjectileManager pm)
        {
            if (_catchUpSpeedField != null && pm != null)
            {
                return (float)_catchUpSpeedField.GetValue(pm);
            }
            return 0.1f;
        }

        public static float GetSqrMaxProjRange(ProjectileManager pm)
        {
            if (_sqrMaxProjRangeField != null && pm != null)
            {
                return (float)_sqrMaxProjRangeField.GetValue(pm);
            }
            return 250000f; // 500m^2 fallback
        }

        public static Projectile FindSpawnedProjectile(ProjectileManager pm, Player owner, uint id)
        {
            if (pm == null || owner == null || _playerProjectilesField == null)
                return null;

            try
            {
                var dict = _playerProjectilesField.GetValue(pm) as Dictionary<NetworkConnection, Dictionary<uint, Projectile>>;
                if (dict != null && owner.Owner != null && dict.TryGetValue(owner.Owner, out var playerDict))
                {
                    if (playerDict.TryGetValue(id, out var proj))
                    {
                        return proj;
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.Debug(() => $"[Compatibility] FindSpawnedProjectile exception: {ex.Message}");
            }

            return null;
        }
    }
}
