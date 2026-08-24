using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class MagicBulletShotContext
    {
        private static bool _isActive;
        private static Weapon _activeWeapon;
        private static Creature _activeTarget;

        public static bool IsActive => _isActive;
        public static Weapon ActiveWeapon => _activeWeapon;
        public static Creature ActiveTarget => _activeTarget;

        public static void Begin(Weapon weapon, Creature target)
        {
            _isActive = true;
            _activeWeapon = weapon;
            _activeTarget = target;
        }

        public static void End()
        {
            _isActive = false;
            _activeWeapon = null;
            _activeTarget = null;
        }
    }
}
