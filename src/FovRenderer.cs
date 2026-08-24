using UnityEngine;

namespace HowToFishMagicBullet
{
    public static class FovRenderer
    {
        private static Texture2D _whiteTexture;
        private static Texture2D WhiteTexture
        {
            get
            {
                if (_whiteTexture == null)
                {
                    _whiteTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false);
                    _whiteTexture.SetPixel(0, 0, Color.white);
                    _whiteTexture.Apply();
                }
                return _whiteTexture;
            }
        }

        public static void Render()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            if (ModConfig.Enabled == null || !ModConfig.Enabled.Value || !Player.LocalPlayer)
                return;

            Camera cam = TargetManager.GetAimCamera();
            if (!cam)
                return;

            Vector2 screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

            // 1. Draw FOV circle
            if (ModConfig.ShowFOVCircle == null || ModConfig.ShowFOVCircle.Value)
            {
                float visualFov = Mathf.Clamp(ModConfig.FovDegrees.Value, 1f, 89f);
                float halfCamFov = cam.fieldOfView * 0.5f;
                float radius = (Mathf.Tan(visualFov * Mathf.Deg2Rad) / Mathf.Tan(halfCamFov * Mathf.Deg2Rad)) * (Screen.height * 0.5f);
                DrawCircle(screenCenter, radius, Color.white, 2f, 128);
            }

            // 2. Draw Locked-Target Ray (if valid target exists, is in front of camera, and is inside screen bounds)
            if (ModConfig.ShowTargetLine == null || ModConfig.ShowTargetLine.Value)
            {
                Creature target = TargetManager.CurrentTarget;
                if (TargetManager.IsAllowedTarget(target))
                {
                    Vector3 worldAimPoint = HeadResolver.ResolveAimPoint(target);
                    Vector3 screenPoint = cam.WorldToScreenPoint(worldAimPoint);

                    // Point must be in front of the camera and within visible screen bounds
                    if (screenPoint.z > 0f &&
                        screenPoint.x >= 0f && screenPoint.x <= Screen.width &&
                        screenPoint.y >= 0f && screenPoint.y <= Screen.height)
                    {
                        Vector2 targetScreenPos = new Vector2(screenPoint.x, Screen.height - screenPoint.y);
                        DrawLine(screenCenter, targetScreenPos, Color.white, 2f);
                    }
                }
            }
        }

        public static void DrawLine(Vector2 pointA, Vector2 pointB, Color color, float width)
        {
            Vector2 delta = pointB - pointA;
            float length = delta.magnitude;
            if (length < 0.001f)
                return;

            float angle = Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg;

            Color prevColor = GUI.color;
            Matrix4x4 prevMatrix = GUI.matrix;

            GUI.color = color;
            GUIUtility.RotateAroundPivot(angle, pointA);
            GUI.DrawTexture(new Rect(pointA.x, pointA.y - width * 0.5f, length, width), WhiteTexture);

            GUI.matrix = prevMatrix;
            GUI.color = prevColor;
        }

        public static void DrawCircle(Vector2 center, float radius, Color color, float width, int segments = 128)
        {
            if (radius <= 0.001f || segments < 3)
                return;

            float angleStep = 360f / segments;
            Vector2 prevPoint = center + new Vector2(radius, 0f);

            for (int i = 1; i <= segments; i++)
            {
                float rad = i * angleStep * Mathf.Deg2Rad;
                Vector2 nextPoint = center + new Vector2(Mathf.Cos(rad) * radius, Mathf.Sin(rad) * radius);
                DrawLine(prevPoint, nextPoint, color, width);
                prevPoint = nextPoint;
            }
        }
    }
}
