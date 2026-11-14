using System;
using UnityEngine;

namespace PostEnot.DxHints
{
    public static class HintUtility
    {
        public static Rect CalculateCenter(HintTarget target, Camera camera)
        {
            Vector3 worldCenter = target.GetPivot();
            Vector3 center = camera.WorldToScreenPoint(worldCenter);
            return new Rect(center, Vector2.zero);
        }

        public static Rect CalculateRect(Buffer<Vector3> pointsBuffer, HintTarget hint, Camera camera, int screenEdgeMargin)
        {
            int length = hint.GetVolumePoints(pointsBuffer);
            Span<Vector3> slice = pointsBuffer[..length];
            Rect rect = PointsToRect(slice, camera);
            return ClampRect(rect, screenEdgeMargin, Screen.width, Screen.height);
        }

        private static Rect ClampRect(
            Rect rect,
            int screenEdgeMargin,
            int screenWidth,
            int screenHeight)
        {
            if (rect.xMin < screenEdgeMargin)
            {
                rect.xMin = screenEdgeMargin;
            }
            if (rect.yMin < screenEdgeMargin)
            {
                rect.yMin = screenEdgeMargin;
            }
            float lim = screenWidth - screenEdgeMargin;
            if (rect.xMax > lim)
            {
                rect.xMax = lim;
            }
            lim = screenHeight - screenEdgeMargin;
            if (rect.yMax > lim)
            {
                rect.yMax = lim;
            }
            return rect;
        }

        private static Rect PointsToRect(ReadOnlySpan<Vector3> corners, Camera camera)
        {
            if (corners.IsEmpty)
            {
                return new Rect();
            }
            Vector3 corner = corners[0];
            Vector2 point = camera.WorldToScreenPoint(corner);
            Rect rect = new(point, Vector2.zero);
            for (int i = 1; i < corners.Length; i += 1)
            {
                corner = corners[i];
                point = camera.WorldToScreenPoint(corner);
                Encapsulate(ref rect, point);
            }
            return rect;
        }

        private static void Encapsulate(ref Rect rect, Vector2 point)
        {
            float minX = Mathf.Min(rect.xMin, point.x);
            float minY = Mathf.Min(rect.yMin, point.y);
            float maxX = Mathf.Max(rect.xMax, point.x);
            float maxY = Mathf.Max(rect.yMax, point.y);
            rect.x = minX;
            rect.y = minY;
            rect.width = maxX - minX;
            rect.height = maxY - minY;
        }
    }
}
