using UnityEngine;

namespace GG.Extensions
{
    public static class VectorExtensions
    {

        /// <summary>
        /// 投影 到 baseVector
        /// </summary>
        public static Vector2 Proj(this Vector2 vector, Vector2 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }


        /// <summary>
        /// 投影 到 baseVector
        /// </summary>
        public static Vector3 Proj(this Vector3 vector, Vector3 baseVector)
        {
            var direction = baseVector.normalized;
            var magnitude = Vector2.Dot(vector, direction);

            return direction * magnitude;
        }

    }

    /// <summary>
    /// 判断点是否在平面的上方
    /// 判断 点到平面的点 与 面的法线向量是否同向就可以
    /// </summary>
    public static class Geometry
    {
        public static bool IsInHalfPlane(Vector3 point, Vector3 halfPlanePoint, Vector3 halfPlaneDirection)
        {
            return Vector3.Dot(point - halfPlanePoint, halfPlaneDirection) >= 0;
        }
    }
}
