using System.Runtime.CompilerServices;
using UnityEngine;

namespace PostEnot.DxHints
{
    [AddComponentMenu(ComponentMenuPathes.DefaultPath + "Hint Target Sphere")]
    public class HintTargetSphere : HintTarget
    {
        #region Inspector
        [SerializeField] private Collider collider;
        [SerializeField, Min(1)] private int pointsPerQuarter;
        [SerializeField, Min(0.0f)] private float offset;

        protected virtual void OnDrawGizmos()
        {
            int vertexCount = pointsPerQuarter * 4;
            float step = Mathf.PI / vertexCount;
            float radius = CalculateRadius();
            float polygonRadius = radius / Mathf.Cos(step);
            Gizmos.color = GizmosWireColor;
            DrawCircle(pointsPerQuarter, transform.position, polygonRadius, Quaternion.Euler(0.0f, 0.0f, 0.0f));
            DrawCircle(pointsPerQuarter, transform.position, polygonRadius, Quaternion.Euler(90.0f, 0.0f, 0.0f));
            DrawCircle(pointsPerQuarter, transform.position, polygonRadius, Quaternion.Euler(0.0f, 90.0f, 0.0f));
        }

        protected virtual void Reset() => collider = GetComponent<Collider>();
        #endregion

        public override int GetVolumePoints(IBuffer<Vector3> pointsBuffer)
        {
            float radius = CalculateRadius();
            CalculateOrthogonalCirclesPoints(pointsBuffer, transform.position, radius, pointsPerQuarter);
            return pointsPerQuarter * 4 * 3;
        }

        public override Vector3 GetPivot() => transform.position;

        private float CalculateRadius()
        {
            Vector3 halfSize = collider.bounds.size * 0.5f;
            float maxXY = Mathf.Max(halfSize.x, halfSize.y);
            float max = Mathf.Max(maxXY, halfSize.z);
            return max + offset;
        }

        private static void DrawCircle(int pointsPerQuarter, Vector3 center, float radius, Quaternion rotation)
        {
            if (pointsPerQuarter <= 0)
            {
                return;
            }
            int vertexCount = pointsPerQuarter * 4;
            float step = Mathf.PI * 2 / vertexCount;
            Vector3 previousPosition = CalculateNextPosition(step, 0, radius, center, rotation);
            for (int i = 1; i <= vertexCount; i += 1)
            {
                Vector3 nextPosition = CalculateNextPosition(step, i, radius, center, rotation);
                Gizmos.DrawLine(previousPosition, nextPosition);
                previousPosition = nextPosition;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 CalculateNextPosition(float step, int index, float radius, Vector3 center, Quaternion rotation)
        {
            float theta = (step * index) + (Mathf.PI * 0.5f);
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            return center + (rotation * new Vector3(x, y));
        }

        public static void CalculateOrthogonalCirclesPoints(
            IBuffer<Vector3> buffer,
            Vector3 center,
            float radius,
            int pointsPerQuarter)
        {
            int pointsPerCircleCount = 4 * pointsPerQuarter;

            float angleStep = Mathf.PI * 2 / pointsPerCircleCount;

            for (int plane = 0; plane < 3; plane += 1)
            {
                int offset = plane * pointsPerCircleCount;
                for (int i = 0; i < pointsPerCircleCount; i += 1)
                {
                    float angle = i * angleStep;
                    float cos = Mathf.Cos(angle);
                    float sin = Mathf.Sin(angle);
                    int idx = offset + i;
                    if (plane == 0)
                    {
                        buffer[idx] = new Vector3(
                            center.x + radius * cos,
                            center.y + radius * sin,
                            center.z
                        );
                    }
                    else if (plane == 1)
                    {
                        buffer[idx] = new Vector3(
                            center.x + radius * cos,
                            center.y,
                            center.z + radius * sin
                        );
                    }
                    else
                    {
                        buffer[idx] = new Vector3(
                            center.x,
                            center.y + radius * cos,
                            center.z + radius * sin
                        );
                    }
                }
            }
        }
    }
}
