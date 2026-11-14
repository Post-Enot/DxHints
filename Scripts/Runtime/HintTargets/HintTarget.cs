using UnityEngine;

namespace PostEnot.DxHints
{
    [DisallowMultipleComponent]
    public abstract class HintTarget : MonoBehaviour
    {
        protected static Color GizmosFillColor { get; set; } = new(0.92f, 0.5f, 0.03f, 0.15f);
        protected static Color GizmosWireColor { get; set; }  = new(0.92f, 0.5f, 0.03f, 1.0f);

        public abstract int GetVolumePoints(IBuffer<Vector3> pointsBuffer);

        public abstract Vector3 GetPivot();
    }
}
