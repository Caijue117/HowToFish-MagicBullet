using System.Collections.Generic;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class TargetManager
    {
        private static readonly List<Creature> _candidateBuffer = new List<Creature>(64);
        public static Creature CurrentTarget { get; private set; }

        public static void ClearTarget()
        {
            CurrentTarget = null;
        }

        public static bool IsAllowedTarget(Creature candidate)
        {
            if (!candidate || !candidate.isActiveAndEnabled || candidate.IsDead || candidate.IsDestroying)
                return false;

            // Strict target category filtering
            if (candidate is Albatross)
            {
                return ModConfig.TargetAlbatross != null && ModConfig.TargetAlbatross.Value;
            }

            if (candidate is Bird)
            {
                return ModConfig.TargetBirds != null && ModConfig.TargetBirds.Value;
            }

            if (candidate is Fish)
            {
                // Note: Fish includes BowheadWhale, AttackingFish, Pufferfish, RunningFish, Piranha
                return ModConfig.TargetFish != null && ModConfig.TargetFish.Value;
            }

            // All other types (Player, NPC, etc.) are strictly excluded
            return false;
        }

        public static bool CheckLineOfSight(Vector3 origin, Vector3 targetPoint, Creature target)
        {
            Vector3 direction = targetPoint - origin;
            float distance = direction.magnitude;

            if (distance <= 0.01f)
                return true;

            // Check against Level and Boat layers
            int layerMask = GameInfo.LevelLayer | GameInfo.BoatLayer;
            if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, distance, layerMask, QueryTriggerInteraction.Ignore))
            {
                // If it hits something other than the target or a child of the target, LOS is blocked
                if (hit.collider != null)
                {
                    Item hitItem = ItemManager.Get(hit.collider);
                    if (hitItem != null && hitItem == target)
                    {
                        return true;
                    }

                    if (hit.collider.transform.IsChildOf(target.transform))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return true;
        }

        public static void UpdateLiveTargeting()
        {
            if (!ModConfig.Enabled.Value || !Player.LocalPlayer)
            {
                CurrentTarget = null;
                return;
            }

            Camera cam = GetAimCamera();
            if (!cam)
            {
                CurrentTarget = null;
                return;
            }

            CurrentTarget = FindBestTarget(cam);
        }

        public static Camera GetAimCamera()
        {
            Player local = Player.LocalPlayer;
            if (local && local.CurCam)
                return local.CurCam;

            return Camera.main;
        }

        public static Creature FindBestTarget(Camera cam)
        {
            if (!cam)
                return null;

            Vector3 camPos = cam.transform.position;
            Vector3 camForward = cam.transform.forward;

            float maxDist = ModConfig.MaxTargetDistance != null ? ModConfig.MaxTargetDistance.Value : 500f;
            float maxDistSqr = maxDist * maxDist;
            float baseFov = ModConfig.FovDegrees != null ? ModConfig.FovDegrees.Value : 35f;
            bool reqLos = ModConfig.RequireLineOfSight != null && ModConfig.RequireLineOfSight.Value;
            TargetSelectionMode mode = ModConfig.TargetMode != null ? ModConfig.TargetMode.Value : TargetSelectionMode.Crosshair;

            // 1. Sticky Target Evaluation
            if (ModConfig.StickyTarget != null && ModConfig.StickyTarget.Value && CurrentTarget != null)
            {
                if (IsAllowedTarget(CurrentTarget))
                {
                    Vector3 stickyAimPoint = HeadResolver.ResolveAimPoint(CurrentTarget);
                    Vector3 toSticky = stickyAimPoint - camPos;
                    float stickyDistSqr = toSticky.sqrMagnitude;

                    if (stickyDistSqr <= maxDistSqr)
                    {
                        float stickyAngle = Vector3.Angle(camForward, toSticky);
                        float stickyFovLimit = baseFov * (ModConfig.StickyFovMultiplier != null ? ModConfig.StickyFovMultiplier.Value : 1.25f);

                        bool inFov = mode == TargetSelectionMode.Nearest || stickyAngle <= stickyFovLimit;
                        bool hasLos = !reqLos || CheckLineOfSight(camPos, stickyAimPoint, CurrentTarget);

                        if (inFov && hasLos)
                        {
                            // Retain existing sticky target
                            return CurrentTarget;
                        }
                    }
                }
            }

            // 2. Fetch cached active creatures
            TargetCache.GetActiveCreatures(_candidateBuffer);

            Creature bestTarget = null;
            float bestScore = float.MaxValue;
            float bestDistanceSqr = float.MaxValue;
            float bestAngle = float.MaxValue;

            for (int i = 0; i < _candidateBuffer.Count; i++)
            {
                Creature candidate = _candidateBuffer[i];
                if (!IsAllowedTarget(candidate))
                    continue;

                Vector3 targetHeadPos = HeadResolver.ResolveAimPoint(candidate);
                Vector3 toTarget = targetHeadPos - camPos;
                float distSqr = toTarget.sqrMagnitude;

                if (distSqr > maxDistSqr || distSqr <= 0.0001f)
                    continue;

                float angle = Vector3.Angle(camForward, toTarget);

                // For modes other than Nearest, candidate must be inside acquisition FOV
                if (mode != TargetSelectionMode.Nearest && angle > baseFov)
                    continue;

                // Line of Sight check if enabled
                if (reqLos && !CheckLineOfSight(camPos, targetHeadPos, candidate))
                    continue;

                // Score candidate based on TargetMode
                switch (mode)
                {
                    case TargetSelectionMode.Crosshair:
                        // Primary: angle, Tie-breaker: distance
                        if (angle < bestAngle - 0.01f ||
                            (Mathf.Abs(angle - bestAngle) <= 0.01f && distSqr < bestDistanceSqr))
                        {
                            bestTarget = candidate;
                            bestAngle = angle;
                            bestDistanceSqr = distSqr;
                        }
                        break;

                    case TargetSelectionMode.Nearest:
                        // Primary: distance, Tie-breaker: angle
                        if (distSqr < bestDistanceSqr - 0.01f ||
                            (Mathf.Abs(distSqr - bestDistanceSqr) <= 0.01f && angle < bestAngle))
                        {
                            bestTarget = candidate;
                            bestDistanceSqr = distSqr;
                            bestAngle = angle;
                        }
                        break;

                    case TargetSelectionMode.LowestHealth:
                        // Primary: current HP, Tie-breaker: angle, then distance
                        float hp = candidate.Hp;
                        if (hp < bestScore - 0.01f ||
                            (Mathf.Abs(hp - bestScore) <= 0.01f && angle < bestAngle))
                        {
                            bestTarget = candidate;
                            bestScore = hp;
                            bestAngle = angle;
                            bestDistanceSqr = distSqr;
                        }
                        break;

                    case TargetSelectionMode.BossPriority:
                        // Primary: is boss/Albatross, Secondary: angle, then distance
                        bool isBoss = candidate is Albatross || candidate.BossType != BossType.None;
                        float bossScore = isBoss ? 0f : 1000f;
                        float combinedScore = bossScore + angle;

                        if (combinedScore < bestScore - 0.01f ||
                            (Mathf.Abs(combinedScore - bestScore) <= 0.01f && distSqr < bestDistanceSqr))
                        {
                            bestTarget = candidate;
                            bestScore = combinedScore;
                            bestAngle = angle;
                            bestDistanceSqr = distSqr;
                        }
                        break;
                }
            }

            _candidateBuffer.Clear();
            return bestTarget;
        }
    }
}
