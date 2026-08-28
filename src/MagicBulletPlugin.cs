using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace HowToFishMagicBullet
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class MagicBulletPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "com.openai.howtofish.magicbullet";
        public const string PluginName = "How To Fish - Magic Bullet";
        public const string PluginVersion = "1.2.3";

        internal static MagicBulletPlugin Instance;
        private Harmony _harmony;

        private void Awake()
        {
            Instance = this;

            // 1. Initialize Logger
            DebugLogger.Initialize(Logger);

            // 2. Initialize Configuration
            ModConfig.Initialize(Config);

            // 3. Initialize Compatibility Layer
            Compatibility.Initialize();

            // 4. Apply Harmony Patches Defensively by Category
            _harmony = new Harmony(PluginGuid);
            InstallPatches();

            // 5. Initial Cache Sweep
            TargetCache.Reconcile();

            DebugLogger.LogInfo($"{PluginName} v{PluginVersion} loaded.");
            DebugLogger.LogInfo($"Target mode: {ModConfig.TargetMode.Value} | FOV: {ModConfig.FovDegrees.Value:0.#}° | Sticky: {ModConfig.StickyTarget.Value} | WallPen: {ModConfig.WallPenetration.Value}");
        }

        private void InstallPatches()
        {
            // Category 1: Critical Ballistics & Redirection (Core Magic Bullet functionality)
            PatchCategory("Core Ballistics", true,
                typeof(WeaponShootPatch),
                typeof(AddProjectilePatch),
                typeof(AddProjectilesPatch),
                typeof(CreatureLocalHitPatch));

            // Category 2: Optional Target Lifecycle (Optimized cache registration)
            PatchCategory("Target Lifecycle", false,
                typeof(CreatureAwakePatch),
                typeof(CreatureOnStopClientPatch),
                typeof(ItemDestroyItemPatch));

            // Category 3: Optional Wall Penetration (Through-wall targeting & ballistics)
            PatchCategory("Wall Penetration", false,
                typeof(UpdateProjectileScanPatch),
                typeof(HitScanPatch),
                typeof(AddToRemoveQueuePatch));
        }

        private bool PatchCategory(string categoryName, bool isCritical, params System.Type[] patchTypes)
        {
            bool allSucceeded = true;
            foreach (System.Type patchType in patchTypes)
            {
                try
                {
                    var processor = _harmony.CreateClassProcessor(patchType);
                    var patches = processor.Patch();
                    if (patches == null || patches.Count == 0)
                    {
                        DebugLogger.LogWarning($"[Harmony] Patch class {patchType.Name} in '{categoryName}' returned 0 hooks.");
                    }
                    else
                    {
                        DebugLogger.Debug(() => $"[Harmony] Applied patch: {patchType.Name} ({patches.Count} hooks)");
                    }
                }
                catch (System.Exception ex)
                {
                    allSucceeded = false;
                    if (isCritical)
                    {
                        DebugLogger.LogError($"[Harmony] CRITICAL patch failure in '{categoryName}' on {patchType.Name}: {ex.Message}");
                    }
                    else
                    {
                        DebugLogger.LogWarning($"[Harmony] Optional patch failure in '{categoryName}' on {patchType.Name}: {ex.Message}");
                    }
                }
            }

            if (!allSucceeded)
            {
                if (isCritical)
                {
                    DebugLogger.LogError($"[MagicBullet] Critical patches failed. Disabling Magic Bullet.");
                    ModConfig.Enabled.Value = false;
                }
                else if (categoryName == "Wall Penetration")
                {
                    DebugLogger.LogWarning($"[MagicBullet] Wall Penetration patch failed. Wall Penetration feature disabled.");
                    Compatibility.DisableWallPenetration();
                }
            }

            return allSucceeded;
        }

        private void OnDestroy()
        {
            TargetManager.ClearTarget();
            TargetCache.Clear();
            ProjectileTracker.Clear();
            HeadResolver.ClearCache();
            _harmony?.UnpatchSelf();
        }

        private void Update()
        {
            // Hotkey: Toggle Magic Bullet
            if (Input.GetKeyDown(ModConfig.ToggleKey.Value))
            {
                ModConfig.Enabled.Value = !ModConfig.Enabled.Value;
                Config.Save();
                DebugLogger.LogInfo($"Magic Bullet: {(ModConfig.Enabled.Value ? "ON" : "OFF")}");
            }

            // Hotkey: Decrease FOV
            if (Input.GetKeyDown(ModConfig.FovDecreaseKey.Value))
            {
                ModConfig.FovDegrees.Value = Mathf.Clamp(ModConfig.FovDegrees.Value - ModConfig.FovStep.Value, 1f, 180f);
                Config.Save();
                DebugLogger.LogInfo($"Magic Bullet FOV: {ModConfig.FovDegrees.Value:0.#}°");
            }

            // Hotkey: Increase FOV
            if (Input.GetKeyDown(ModConfig.FovIncreaseKey.Value))
            {
                ModConfig.FovDegrees.Value = Mathf.Clamp(ModConfig.FovDegrees.Value + ModConfig.FovStep.Value, 1f, 180f);
                Config.Save();
                DebugLogger.LogInfo($"Magic Bullet FOV: {ModConfig.FovDegrees.Value:0.#}°");
            }

            // Live target tracking
            TargetManager.UpdateLiveTargeting();
        }

        private void OnGUI()
        {
            FovRenderer.Render();
        }
    }
}
