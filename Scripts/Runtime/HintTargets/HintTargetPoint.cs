using UnityEngine;

namespace PostEnot.DxHints
{
    public sealed class HintTargetPoint : HintTarget
    {
        #region Inspector
        [SerializeField] private Transform point;
        #endregion

        public override Vector3 GetPivot() => point != null ? point.position : transform.position;

        public override int GetVolumePoints(IBuffer<Vector3> pointsBuffer) => 0;
    }
}
