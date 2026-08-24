using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class TargetPrediction
    {
        public static Vector3 GetTargetVelocity(Creature target, Vector3 aimPoint)
        {
            if (!target || !target.Rig)
                return Vector3.zero;

            return target.Rig.GetPointVelocity(aimPoint);
        }

        /// <summary>
        /// Solves projectile interception iteratively (2-3 iterations) with gravity compensation.
        /// </summary>
        public static Vector3 PredictInterceptPoint(
            Vector3 origin,
            Creature target,
            float projectileSpeed,
            float gravityForce,
            out float flightTime)
        {
            flightTime = 0f;

            if (!target)
                return origin;

            Vector3 initialAimPoint = HeadResolver.ResolveAimPoint(target);
            Vector3 targetVel = GetTargetVelocity(target, initialAimPoint);
            float maxTime = ModConfig.MaxPredictionTime != null ? ModConfig.MaxPredictionTime.Value : 2.0f;

            if (projectileSpeed <= 0.001f)
            {
                return initialAimPoint;
            }

            // Step 1: Initial estimate
            float dist = (initialAimPoint - origin).magnitude;
            float t = dist / projectileSpeed;

            // Step 2: Iterative refinement (3 iterations)
            for (int i = 0; i < 3; i++)
            {
                Vector3 estimatedPos = initialAimPoint + targetVel * t;
                float currentDist = (estimatedPos - origin).magnitude;
                float nextT = currentDist / projectileSpeed;

                if (float.IsNaN(nextT) || float.IsInfinity(nextT) || nextT < 0f)
                {
                    // Fallback to simple direct calculation
                    t = dist / projectileSpeed;
                    break;
                }

                // Check convergence
                if (Mathf.Abs(nextT - t) < 0.001f)
                {
                    t = nextT;
                    break;
                }

                t = nextT;
            }

            t = Mathf.Clamp(t, 0f, maxTime);
            flightTime = t;

            // Step 3: Compute final predicted position
            Vector3 predictedPoint = initialAimPoint + targetVel * t;

            // Step 4: Apply vertical gravity compensation
            // ProjectileManager integrates: velocity += Vector3.down * (GravityForce * dt)
            // Displacement after time t is: y(t) = y0 + v0y*t - 0.5*gravity*t^2
            // To hit target at y(t), initial aim must be elevated by: + 0.5*gravity*t^2
            if (gravityForce > 0f)
            {
                predictedPoint += Vector3.up * (0.5f * gravityForce * t * t);
            }

            return predictedPoint;
        }

        public static Vector3 CalculateRedirectedVelocity(
            Vector3 origin,
            Vector3 originalVelocity,
            Creature target,
            WeaponInfo weaponInfo)
        {
            if (!target)
                return originalVelocity;

            float speed = originalVelocity.magnitude;
            if (speed <= 0.001f)
                return originalVelocity;

            float gravity = weaponInfo != null ? weaponInfo.ProjectileGravity : 0f;
            Vector3 aimPoint = PredictInterceptPoint(origin, target, speed, gravity, out float travelTime);

            Vector3 direction = aimPoint - origin;
            if (direction.sqrMagnitude <= 0.0001f)
                return originalVelocity;

            DebugLogger.Debug(() => $"[TargetPrediction] Redirected velocity toward {target.name} | Speed: {speed:0.#} | Gravity: {gravity:0.#} | Time: {travelTime:0.###}s");

            return direction.normalized * speed;
        }
    }
}
