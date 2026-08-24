using System;
using System.Collections.Generic;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class TargetCache
    {
        private static readonly HashSet<Creature> _creatures = new HashSet<Creature>();
        private static readonly List<Creature> _staleList = new List<Creature>(32);
        private static float _nextReconcileTime = 0f;
        private const float ReconcileIntervalSeconds = 3.0f;

        public static int Count => _creatures.Count;

        public static void Register(Creature creature)
        {
            if (!creature)
                return;

            lock (_creatures)
            {
                _creatures.Add(creature);
            }
            DebugLogger.Debug(() => $"[TargetCache] Registered creature: {creature.name} (Total: {_creatures.Count})");
        }

        public static void Unregister(Creature creature)
        {
            if (!creature)
                return;

            lock (_creatures)
            {
                _creatures.Remove(creature);
            }
            DebugLogger.Debug(() => $"[TargetCache] Unregistered creature: {creature.name} (Total: {_creatures.Count})");
        }

        public static void Clear()
        {
            lock (_creatures)
            {
                _creatures.Clear();
            }
        }

        /// <summary>
        /// Populates the provided buffer with active cached creatures without generating heap allocations.
        /// Automatically prunes dead/destroyed entries.
        /// </summary>
        public static void GetActiveCreatures(List<Creature> buffer)
        {
            if (buffer == null)
                return;

            buffer.Clear();

            // Periodic fallback reconciliation
            if (Time.time >= _nextReconcileTime)
            {
                Reconcile();
                _nextReconcileTime = Time.time + ReconcileIntervalSeconds;
            }

            lock (_creatures)
            {
                _staleList.Clear();

                foreach (Creature candidate in _creatures)
                {
                    if (!candidate || candidate.IsDead || candidate.IsDestroying)
                    {
                        _staleList.Add(candidate);
                        continue;
                    }

                    if (candidate.isActiveAndEnabled)
                    {
                        buffer.Add(candidate);
                    }
                }

                for (int i = 0; i < _staleList.Count; i++)
                {
                    _creatures.Remove(_staleList[i]);
                }
                _staleList.Clear();
            }
        }

        /// <summary>
        /// Periodic low-frequency reconciliation to ensure newly instantiated scene creatures are not missed.
        /// </summary>
        public static void Reconcile()
        {
            try
            {
#pragma warning disable CS0618
                Creature[] sceneCreatures = UnityEngine.Object.FindObjectsOfType<Creature>();
#pragma warning restore CS0618

                lock (_creatures)
                {
                    if (sceneCreatures != null)
                    {
                        for (int i = 0; i < sceneCreatures.Length; i++)
                        {
                            Creature c = sceneCreatures[i];
                            if (c && !c.IsDead && !c.IsDestroying)
                            {
                                _creatures.Add(c);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                DebugLogger.LogWarning($"[TargetCache] Reconcile exception: {ex.Message}");
            }
        }
    }
}
