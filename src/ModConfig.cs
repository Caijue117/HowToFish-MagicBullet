using BepInEx.Configuration;
using UnityEngine;

namespace HowToFishMagicBullet
{
    public enum TargetSelectionMode
    {
        Crosshair,
        Nearest,
        LowestHealth,
        BossPriority
    }

    public static class ModConfig
    {
        // General
        public static ConfigEntry<bool> Enabled { get; private set; }

        // Targeting
        public static ConfigEntry<TargetSelectionMode> TargetMode { get; private set; }
        public static ConfigEntry<float> FovDegrees { get; private set; }
        public static ConfigEntry<float> FovStep { get; private set; }
        public static ConfigEntry<float> MaxTargetDistance { get; private set; }
        public static ConfigEntry<float> MaxPredictionTime { get; private set; }
        public static ConfigEntry<bool> StickyTarget { get; private set; }
        public static ConfigEntry<float> StickyFovMultiplier { get; private set; }
        public static ConfigEntry<bool> RequireLineOfSight { get; private set; }
        public static ConfigEntry<bool> WallPenetration { get; private set; }

        // Category Filtering
        public static ConfigEntry<bool> TargetFish { get; private set; }
        public static ConfigEntry<bool> TargetBirds { get; private set; }
        public static ConfigEntry<bool> TargetAlbatross { get; private set; }

        // Hotkeys
        public static ConfigEntry<KeyCode> ToggleKey { get; private set; }
        public static ConfigEntry<KeyCode> FovDecreaseKey { get; private set; }
        public static ConfigEntry<KeyCode> FovIncreaseKey { get; private set; }

        // Visuals
        public static ConfigEntry<bool> ShowFOVCircle { get; private set; }
        public static ConfigEntry<bool> ShowTargetLine { get; private set; }

        // Diagnostics
        public static ConfigEntry<bool> DebugLogging { get; private set; }

        public static void Initialize(ConfigFile config)
        {
            // MagicBullet Section (backward compatible with v1.1.0)
            Enabled = config.Bind(
                "MagicBullet",
                "Enabled",
                true,
                "Enable or disable magic bullet / silent aim.");

            // Visuals Section
            ShowFOVCircle = config.Bind(
                "Visuals",
                "ShowFOVCircle",
                true,
                "Render the centered white FOV targeting circle on screen.");

            ShowTargetLine = config.Bind(
                "Visuals",
                "ShowTargetLine",
                true,
                "Render the white line connecting crosshair center to the locked target's head weak point.");

            // Targeting Section (preserves existing FOV, FOVStep, MaxPredictionTime)
            TargetMode = config.Bind(
                "Targeting",
                "TargetMode",
                TargetSelectionMode.Crosshair,
                "Target selection strategy: Crosshair (closest to center), Nearest (world distance), LowestHealth (lowest HP), BossPriority (prefer Albatross / bosses).");

            FovDegrees = config.Bind(
                "Targeting",
                "FOV",
                35f,
                new ConfigDescription(
                    "Maximum angular distance from crosshair center in degrees (FOV radius).",
                    new AcceptableValueRange<float>(1f, 180f)));

            FovStep = config.Bind(
                "Targeting",
                "FOVStep",
                5f,
                new ConfigDescription(
                    "Degrees changed per FOV hotkey press.",
                    new AcceptableValueRange<float>(1f, 30f)));

            MaxTargetDistance = config.Bind(
                "Targeting",
                "MaxTargetDistance",
                500f,
                new ConfigDescription(
                    "Maximum distance in meters to acquire targets.",
                    new AcceptableValueRange<float>(10f, 2000f)));

            MaxPredictionTime = config.Bind(
                "Targeting",
                "MaxPredictionTime",
                2.0f,
                new ConfigDescription(
                    "Maximum movement/gravity prediction time in seconds.",
                    new AcceptableValueRange<float>(0f, 5f)));

            StickyTarget = config.Bind(
                "Targeting",
                "StickyTarget",
                true,
                "If enabled, retains the current target across shots while it remains valid and within the sticky FOV.");

            StickyFovMultiplier = config.Bind(
                "Targeting",
                "StickyFOVMultiplier",
                1.25f,
                new ConfigDescription(
                    "Multiplier applied to FOV for retaining an existing sticky target (hysteresis).",
                    new AcceptableValueRange<float>(1.0f, 3.0f)));

            RequireLineOfSight = config.Bind(
                "Targeting",
                "RequireLineOfSight",
                false,
                "If true, targets blocked by walls / environmental geometry cannot be acquired.");

            WallPenetration = config.Bind(
                "Targeting",
                "WallPenetration",
                false,
                "If true, Magic Bullet projectiles fired by the local player will pass through terrain and environmental geometry to hit the locked target.");

            // Category Filtering Section
            TargetFish = config.Bind(
                "Filtering",
                "TargetFish",
                true,
                "Allow targeting Fish (including Bowhead Whale, Mutated Whale, AttackingFish, Pufferfish, RunningFish, Piranha).");

            TargetBirds = config.Bind(
                "Filtering",
                "TargetBirds",
                true,
                "Allow targeting Birds (Seagulls).");

            TargetAlbatross = config.Bind(
                "Filtering",
                "TargetAlbatross",
                true,
                "Allow targeting Albatross.");

            // Hotkeys Section (backward compatible with v1.1.0)
            ToggleKey = config.Bind(
                "Hotkeys",
                "Toggle",
                KeyCode.F8,
                "Hotkey to toggle Magic Bullet ON/OFF.");

            FovDecreaseKey = config.Bind(
                "Hotkeys",
                "FOVDecrease",
                KeyCode.LeftBracket,
                "Hotkey to decrease FOV.");

            FovIncreaseKey = config.Bind(
                "Hotkeys",
                "FOVIncrease",
                KeyCode.RightBracket,
                "Hotkey to increase FOV.");

            // Diagnostics Section
            DebugLogging = config.Bind(
                "Diagnostics",
                "DebugLogging",
                false,
                "Enable detailed debug logging for targeting, ballistics, and penetration events.");
        }
    }
}
