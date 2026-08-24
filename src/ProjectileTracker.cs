using System.Collections.Generic;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class ProjectileTracker
    {
        private static readonly Dictionary<Projectile, Creature> _projectileToTarget = new Dictionary<Projectile, Creature>();
        private static readonly Dictionary<uint, Creature> _idToTarget = new Dictionary<uint, Creature>();

        public static void Register(Projectile projectile, Creature target)
        {
            if (projectile == null || target == null)
                return;

            lock (_projectileToTarget)
            {
                _projectileToTarget[projectile] = target;
                _idToTarget[projectile.Id] = target;
            }

            DebugLogger.Debug(() => $"[ProjectileTracker] Tracked projectile ID {projectile.Id} -> Target: {target.name}");
        }

        public static bool TryGetTarget(Projectile projectile, out Creature target)
        {
            target = null;
            if (projectile == null)
                return false;

            lock (_projectileToTarget)
            {
                if (_projectileToTarget.TryGetValue(projectile, out target))
                {
                    if (target != null && !target.IsDead && !target.IsDestroying)
                        return true;
                }

                if (_idToTarget.TryGetValue(projectile.Id, out target))
                {
                    if (target != null && !target.IsDead && !target.IsDestroying)
                        return true;
                }
            }

            return false;
        }

        public static void Unregister(Projectile projectile)
        {
            if (projectile == null)
                return;

            lock (_projectileToTarget)
            {
                _projectileToTarget.Remove(projectile);
                _idToTarget.Remove(projectile.Id);
            }

            DebugLogger.Debug(() => $"[ProjectileTracker] Untracked projectile ID {projectile.Id}");
        }

        public static void Clear()
        {
            lock (_projectileToTarget)
            {
                _projectileToTarget.Clear();
                _idToTarget.Clear();
            }
        }
    }
}
