using UnityEngine;

namespace PostEnot.DxHints
{
    [AddComponentMenu(ComponentMenuPathes.DefaultPath + "Hint Target Bounds")]
    public class HintTargetBounds : HintTarget
    {
        #region Inspector
        [SerializeField] private Collider[] colliders;

        protected virtual void Reset() => colliders = GetComponentsInChildren<Collider>();

        protected virtual void OnDrawGizmos()
        {
            if ((colliders == null) || (colliders.Length == 0))
            {
                return;
            }
            Bounds worldBounds = CalculateColliderBounds();
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.color = GizmosFillColor;
            Gizmos.DrawCube(worldBounds.center, worldBounds.size);
            Gizmos.color = GizmosWireColor;
            Gizmos.DrawWireCube(worldBounds.center, worldBounds.size);
        }
        #endregion

        public override Vector3 GetPivot()
        {
            Bounds bounds = CalculateColliderBounds();
            return bounds.center;
        }

        public override int GetVolumePoints(IBuffer<Vector3> pointsBuffer)
        {
            pointsBuffer.EnsureCapacity(8);
            Bounds worldBounds = CalculateColliderBounds();
            CalculateBoundsCorners(worldBounds, pointsBuffer);
            return 8;
        }

        private Bounds CalculateColliderBounds()
        {
            if (!TryGetFirstNotNullCollider(out int index, out Collider collider))
            {
                return new Bounds(transform.position, Vector3.zero);
            }
            Bounds worldBounds = collider.bounds;
            for (; index < colliders.Length; index += 1)
            {
                if (colliders[index] != null)
                {
                    Bounds bounds = colliders[index].bounds;
                    worldBounds.Encapsulate(bounds);
                }
            }
            return worldBounds;
        }

        private bool TryGetFirstNotNullCollider(out int index, out Collider result)
        {
            for (index = 0; index < colliders.Length; index += 1)
            {
                if (colliders[index] != null)
                {
                    result = colliders[index];
                    return true;
                }
            }
            index = -1;
            result = null;
            return false;
        }

        public static void CalculateBoundsCorners(Bounds worldBounds, IBuffer<Vector3> buffer)
        {
            buffer[0] = worldBounds.min;
            buffer[1] = worldBounds.max;
            buffer[2] = new Vector3(worldBounds.min.x, worldBounds.min.y, worldBounds.max.z);
            buffer[3] = new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.min.z);
            buffer[4] = new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z);
            buffer[5] = new Vector3(worldBounds.min.x, worldBounds.max.y, worldBounds.max.z);
            buffer[6] = new Vector3(worldBounds.max.x, worldBounds.min.y, worldBounds.max.z);
            buffer[7] = new Vector3(worldBounds.max.x, worldBounds.max.y, worldBounds.min.z);
        }
    }
}
