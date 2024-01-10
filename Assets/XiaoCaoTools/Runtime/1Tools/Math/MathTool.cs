
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UIElements;

//矩形排列, TODO: 圆形排列
public static class MathLayoutTool
{
    #region   矩形排列
    public enum Alignment
    {
        Left,
        Center,
        Right
    }
    private const int objectWidth = 0;
    private const int objectHeight = 0;
    //矩形排列
    public static Vector2Int GetRectPos(int xIndex, int yIndex, int xCount, Alignment alignment, int spacingX = 10, int spacingY = 10)
    {
        int startX = 0;
        int startY = 0;

        startY = yIndex * (objectHeight + spacingY);

        int x = startX + xIndex * (objectWidth + spacingX);
        switch (alignment)
        {
            case Alignment.Center:
                x += (objectWidth - spacingX * (xCount - 1)) / 2;
                break;
            case Alignment.Right:
                x += (objectWidth - spacingX) * (xCount - 1);
                break;
        }
        //Left不需要任何处理
        return new Vector2Int(x, startY);
    }

    //空心矩形排列
    public class RectangularArrangement
    {
        public static List<(int, int)> ArrangeObjects(int rectangleWidth, int rectangleHeight, int objectSize, int margin)
        {
            List<(int, int)> coordinates = new List<(int, int)>();

            // Calculate the number of objects that can fit on each side
            int objectsOnTop = (rectangleWidth - 2 * margin) / objectSize;
            int objectsOnSide = (rectangleHeight - 2 * margin) / objectSize;

            // Calculate the coordinates for each side
            for (int i = 0; i < objectsOnTop; i++)
            {
                int x = margin + i * objectSize;
                int y = margin;
                coordinates.Add((x, y));
            }

            for (int i = 0; i < objectsOnSide; i++)
            {
                int x = rectangleWidth - margin - objectSize;
                int y = margin + i * objectSize;
                coordinates.Add((x, y));
            }

            for (int i = objectsOnTop - 1; i >= 0; i--)
            {
                int x = margin + i * objectSize;
                int y = rectangleHeight - margin - objectSize;
                coordinates.Add((x, y));
            }

            for (int i = objectsOnSide - 1; i > 0; i--)
            {
                int x = margin;
                int y = margin + i * objectSize;
                coordinates.Add((x, y));
            }

            return coordinates;
        }
    }


    #endregion

    #region 圆形排列 TODO

    #endregion
}

public static class MathTool
{
    #region Value
    //先慢后快 t -> [0,1]
    public static float RLerp(float start, float end, float t)
    {
        return (end - start) * t;
    }

    public static bool InRange(float value, float closedLeft, float openRight)
    {
        return value >= closedLeft && value < openRight;
    }
    /// <summary>
    /// 值映射, 比如原本0~1的0.4, 映射到0~100,就是40
    /// </summary>
    /// <returns></returns>
    public static float ValueMapping(float value,float from,float to,float newFrom,float newTo) 
    {
        float p = (value-from) / (to - from);
        float newValue = p * (newTo - newFrom) + newFrom;
        return newValue;
    }

    //判断浮点数是否相等
    public static bool IsFEqual(this float value, float value2)
    {
        return Math.Abs(value - value2) < 0.00001f;
    }

    #endregion
    #region Vector & Rotate
    public static bool IsNaN(this Vector2 v)
    {
        return v == Vector2.zero;
    }    
    public static bool IsNaN(this Vector3 v)
    {
        return v == Vector3.zero;
    }


    /// <summary>
    /// 旋转向量,忽略y轴 
    /// </summary>
    public static Vector3 RotateY(Vector3 dir, float angle)
    {
        //angle旋转角度 axis围绕旋转轴 position自身坐标 自身坐标 center旋转中心
        //完整公式:  Quaternion.AngleAxis(angle, axis) * (position - center) + center;
        //Vec3 = R * Vec3
        return Quaternion.AngleAxis(angle, Vector3.up) * (dir);
    }
    /// <summary>
    /// 二维向量旋转
    /// </summary>
    public static Vector2 Rotate(Vector2 vector, float angleInDeg)
    {
        float angleInRad = Mathf.Deg2Rad * angleInDeg;
        float cosAngle = Mathf.Cos(angleInRad);
        float sinAngle = Mathf.Sin(angleInRad);

        float x = vector.x * cosAngle - vector.y * sinAngle;
        float y = vector.x * sinAngle + vector.y * cosAngle;

        return new Vector2(x, y);
    }

    /// <summary>
    /// 向量绕某点旋转
    /// 思路:将向量移动到原点, 旋转后再移回
    /// </summary>
    public static Vector2 RotateAround(this Vector2 vector, float angleInDeg, Vector2 axisPosition)
    {
        return Rotate((vector - axisPosition), (angleInDeg)) + axisPosition;
    }


    //角度转二维向量
    //从正右方开始计算,逆时针,90度为正上方
    public static Vector2 AngleToVector(float angleInDegrees)
    {
        double angleInRadians = angleInDegrees * (Math.PI / 180.0f);
        double xComponent = Math.Cos(angleInRadians);
        double yComponent = Math.Sin(angleInRadians);

        return new Vector2((float)xComponent, (float)yComponent);
    }

    //二维向量转角度,正右方为0角
    public static float VectorToAngle(this Vector2 vector)
    {
        // 计算向量相对于 x 轴的角度（弧度）
        double angleRadians = Math.Atan2(vector.y, vector.x);
        // 将弧度转换为角度
        double angleDegrees = angleRadians * 180.0 / Math.PI;
        // 确保角度在 0 到 360 范围内
        if (angleDegrees < 0)
        {
            angleDegrees += 360;
        }
        return (float)angleDegrees;
    }

    public static Quaternion ForwardToRotation(Vector3 forward)
    {
        //对于角色的Y轴一般向上的
        return Quaternion.LookRotation(forward, Vector3.up);
    }

    ///矩阵相关
    /// <summary>
    /// 局部和世界空间转换 示例
    /// </summary>
    public static void SpaceExample(Transform tf, Vector3 worldPos, Vector3 localPos)
    {
        //世界转局部 坐标
        localPos = tf.TransformPoint(worldPos);
        //局部转世界
        worldPos = tf.InverseTransformPoint(localPos);
        //向量同理...
        tf.TransformDirection(worldPos);

        //获取旋转矩阵
        Quaternion rotation = tf.rotation;
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);
        //或者
        Matrix4x4 localToWorldMatrix = tf.localToWorldMatrix;
        //在没有Transfrom的情况需要使用下面的方法计算
        localPos = WorldToLocalPos(rotationMatrix, worldPos);
    }

    public static Vector3 WorldToLocalPos(Matrix4x4 rotationMatrix, Vector3 worldPos)
    {
        return rotationMatrix.inverse.MultiplyPoint3x4(worldPos);
        //当然等价于 WorldToLocalDir(rotation,worldPos-centerPos)
    }

    public static Vector3 WorldToLocalDir(Matrix4x4 rotationMatrix, Vector3 worldDir)
    {
        return rotationMatrix.inverse.MultiplyVector(worldDir);
    }

    //局部空间向量 转 世界空间
    public static Vector3 LocalToWorldDir(Matrix4x4 localToWorldMatrix, Vector3 localDir)
    {
        return localToWorldMatrix.MultiplyVector(localDir);
    }


    //旋转向目标方向, 忽略y轴
    public static void RotaToPos(this Transform transform, Vector3 wordlPos, float lerp = 1)
    {
        wordlPos.y = transform.position.y; //保持同一高度
        Quaternion rotation = Quaternion.LookRotation(wordlPos - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, lerp);
    }
    #endregion

    #region 曲线
    //二阶贝塞尔
    public static Vector3 GetBezierPoint2(Vector3 begin, Vector3 end, Vector3 handle, float t)
    {
        float pow = Mathf.Pow(1 - t, 2);
        float x = pow * begin.x + 2 * t * (1 - t) * handle.x + t * t * end.x;
        float y = pow * begin.y + 2 * t * (1 - t) * handle.y + t * t * end.y;
        float z = pow * begin.z + 2 * t * (1 - t) * handle.z + t * t * end.z;
        return new Vector3(x, y, z);
    }
    //求导
    public static Vector3 GetBezierPoint2_Speed(Vector3 begin, Vector3 end, Vector3 handle, float t)
    {
        float pow_s = 2 * t - 2;
        float x = pow_s * begin.x + (2 - 4 * t) * handle.x + 2 * t * end.x;
        float y = pow_s * begin.y + (2 - 4 * t) * handle.y + 2 * t * end.y;
        float z = pow_s * begin.z + (2 - 4 * t) * handle.z + 2 * t * end.z;
        return new Vector3(x, y, z);
    }
    //获得尽量平滑的Handle点
    public static Vector3 GetAutoHandle(Vector3 A, Vector3 B, Vector3 C,float rate = 0.8f)
    {
        Vector3 AB = B - A;
        Vector3 BC = C - B;

        float angle = Vector3.Angle(AB, BC);


        //获得垂直向量
        Vector3 normalVector = Vector3.Cross(AB, BC);

        Vector3 panleVector = Vector3.Cross(AB, normalVector).normalized;


        Debug.Log($" dot {Vector3.Dot(panleVector, BC)} {angle} {Mathf.Sin(angle)}");

        if (Vector3.Dot(panleVector, BC) > 0)
        {
            panleVector =-panleVector;
        }


        //根据AB 与 BC的夹角, 越大handle点离AB中点M 越远
        float distance = Mathf.Sin(angle * Mathf.Deg2Rad) * AB.magnitude * rate;

        return panleVector * distance + (B + A) / 2;
    }

    //计算平面内向量 左右关系, 和视角有关系, unity中默认用y
    private static bool CheckCrossProduct(Vector3 a, Vector3 b)
    {
        // 计算叉积
        Vector3 crossProduct = Vector3.Cross(a, b);
        return crossProduct.y > 0;
    }


    //三阶段贝塞尔
    public static Vector3 GetBezierPoint3(float time, Vector3 startPosition, Vector3 endPosition, Vector3 startTangent, Vector3 endTangent)
    {
        float t = time;
        float u = 1f - t;
        float t2 = t * t;
        float u2 = u * u;
        float u3 = u2 * u;
        float t3 = t2 * t;

        Vector3 result =
            (u3) * startPosition +
            (3f * u2 * t) * startTangent +
            (3f * u * t2) * endTangent +
            (t3) * endPosition;

        return result;
    }


    public static Vector3 LinearVec3(Vector3 start, Vector3 end, float t)
    {
        end -= start;
        return end * t + start;
    }
    #endregion
}
