using System;
using System.Collections.Generic;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public enum HeadPointSource
    {
        HeadTransform,
        HeadPos,
        HeadCollider,
        CenterOfMass,
        TransformFallback
    }

    public static class HeadResolver
    {
        // Cache resolved child head transforms to avoid repeated hierarchy searches
        private static readonly Dictionary<int, Transform> _headTransformCache = new Dictionary<int, Transform>();

        public static bool TryGetAimPoint(Creature target, out Vector3 aimPoint, out HeadPointSource source)
        {
            aimPoint = Vector3.zero;
            source = HeadPointSource.TransformFallback;

            if (!target)
                return false;

#pragma warning disable CS0618
            int instanceId = target.GetInstanceID();
#pragma warning restore CS0618

            // 1. Check for dedicated Head Transform
            if (_headTransformCache.TryGetValue(instanceId, out Transform cachedHead) && cachedHead)
            {
                aimPoint = cachedHead.position;
                source = HeadPointSource.HeadTransform;
                return true;
            }

            Transform foundHead = FindHeadTransform(target.transform);
            if (foundHead)
            {
                _headTransformCache[instanceId] = foundHead;
                aimPoint = foundHead.position;
                source = HeadPointSource.HeadTransform;
                return true;
            }

            // 2. Use game's authoritative HeadPos property (local Z boundary for headshots)
            // Headshots are evaluated by game as: localPoint.z > target.HeadPos.
            // We offset slightly forward (+0.15m) along local Z to target the center of the head volume.
            if (Mathf.Abs(target.HeadPos) > 0.001f || target is Fish || target is Bird || target is Albatross)
            {
                Vector3 localOffset = Vector3.zero;
                if (target.Rig)
                {
                    localOffset = target.transform.InverseTransformPoint(target.Rig.worldCenterOfMass);
                }

                Vector3 localHeadPoint = new Vector3(localOffset.x, localOffset.y, target.HeadPos + 0.15f);
                aimPoint = target.transform.TransformPoint(localHeadPoint);
                source = HeadPointSource.HeadPos;
                return true;
            }

            // 3. Check for forward-most head collider bounds
            Collider[] colliders = target.GetComponentsInChildren<Collider>();
            if (colliders != null && colliders.Length > 0)
            {
                Collider bestCol = null;
                float highestLocalZ = float.MinValue;

                for (int i = 0; i < colliders.Length; i++)
                {
                    Collider col = colliders[i];
                    if (!col || !col.enabled)
                        continue;

                    Vector3 localCenter = target.transform.InverseTransformPoint(col.bounds.center);
                    if (localCenter.z > highestLocalZ)
                    {
                        highestLocalZ = localCenter.z;
                        bestCol = col;
                    }
                }

                if (bestCol != null)
                {
                    aimPoint = bestCol.bounds.center;
                    source = HeadPointSource.HeadCollider;
                    return true;
                }
            }

            // 4. Rigidbody Center of Mass
            if (target.Rig)
            {
                aimPoint = target.Rig.worldCenterOfMass;
                source = HeadPointSource.CenterOfMass;
                return true;
            }

            // 5. Creature transform position fallback
            aimPoint = target.transform.position;
            source = HeadPointSource.TransformFallback;
            return true;
        }

        public static Vector3 ResolveAimPoint(Creature target)
        {
            TryGetAimPoint(target, out Vector3 point, out _);
            return point;
        }

        private static Transform FindHeadTransform(Transform root)
        {
            if (!root)
                return null;

            for (int i = 0; i < root.childCount; i++)
            {
                Transform child = root.GetChild(i);
                string name = child.name;
                if (name.IndexOf("head", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("mouth", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    name.IndexOf("beak", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return child;
                }

                Transform subChild = FindHeadTransform(child);
                if (subChild)
                    return subChild;
            }

            return null;
        }

        public static void ClearCache()
        {
            _headTransformCache.Clear();
        }
    }
}
