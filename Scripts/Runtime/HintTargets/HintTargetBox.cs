using System;
using UnityEngine;

namespace PostEnot.DxHints
{
    public class HintTargetBox : HintTarget
    {
        #region Inspector
        [SerializeField] private BoxCollider collider;

        protected virtual void Reset() => collider = GetComponentInChildren<BoxCollider>();

        protected virtual void OnDrawGizmos()
        {
            if (collider == null)
            {
                return;
            }
            Gizmos.matrix = collider.transform.localToWorldMatrix;
            Gizmos.color = GizmosFillColor;
            Gizmos.DrawCube(collider.center, collider.size);
            Gizmos.color = GizmosWireColor;
            Gizmos.DrawWireCube(collider.center, collider.size);
        }
        #endregion

        public override int GetVolumePoints(IBuffer<Vector3> pointsBuffer)
        {
            pointsBuffer.EnsureCapacity(8);
            CalculateBoxColliderCorners(collider, pointsBuffer);
            return 8;
        }

        public override Vector3 GetPivot() => collider.transform.TransformPoint(collider.center);

        private static void CalculateBoxColliderCorners(BoxCollider collider, IBuffer<Vector3> buffer)
        {
            Vector3 center = collider.center;
            Vector3 halfSize = collider.size * 0.5f;
            buffer[0] = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            buffer[1] = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            buffer[2] = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            buffer[3] = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);
            buffer[4] = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            buffer[5] = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            buffer[6] = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            buffer[7] = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            Span<Vector3> span = buffer.AsSpan();
            collider.transform.TransformPoints(span);
        }
    }
}
