using UnityEngine;
namespace XiaoCao
{
    public static class BezierUtil
    {
        /// <summary>
        /// 二阶曲线
        /// </summary>
        /// <param name="t">[0,1]</param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <returns></returns>
        public static Vector3 Curve(float t, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            Vector3 point = Vector3.zero;
            float t1 = (1 - t) * (1 - t);
            float t2 = (1 - t) * t;
            float t3 = t * t;
            point = t1 * p1 + 2 * t2 * p2 + t3 * p3;
            return point;
        }

        /// <summary>
        /// 三阶曲线
        /// </summary>
        /// <param name="t">[0,1]</param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        /// <param name="p4"></param>
        /// <returns></returns>
        public static Vector3 Curve(float t, Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
        {
            Vector3 point = Vector3.zero;
            float t1 = (1 - t) * (1 - t) * (1 - t);
            float t2 = (1 - t) * (1 - t) * t;
            float t3 = t * t * (1 - t);
            float t4 = t * t * t;
            point = p1 * t1 + 3 * t2 * p2 + 3 * t3 * p3 + p4 * t4;
            return point;
        }
    }
}