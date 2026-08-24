using System;
using BepInEx.Logging;

namespace HowToFishMagicBullet
{
    public static class DebugLogger
    {
        private static ManualLogSource _logger;

        public static void Initialize(ManualLogSource logger)
        {
            _logger = logger;
        }

        public static void LogInfo(string message)
        {
            _logger?.LogInfo(message);
        }

        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }

        public static void LogError(string message)
        {
            _logger?.LogError(message);
        }

        public static void Debug(string message)
        {
            if (ModConfig.DebugLogging != null && ModConfig.DebugLogging.Value)
            {
                _logger?.LogInfo($"[DEBUG] {message}");
            }
        }

        public static void Debug(Func<string> messageFactory)
        {
            if (ModConfig.DebugLogging != null && ModConfig.DebugLogging.Value && messageFactory != null)
            {
                _logger?.LogInfo($"[DEBUG] {messageFactory()}");
            }
        }
    }
}
