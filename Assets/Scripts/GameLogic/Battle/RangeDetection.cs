using UnityEngine;
using System.Collections;

/// <summary>
/// 范围检测类
/// </summary>
public class RangeDetection
{

    /// <summary>
    /// 检测目标点是否在扇形区域内
    /// </summary>
    /// <param name="origin">圆心</param>
    /// <param name="u">过圆心的单位向量</param>
    /// <param name="dis">半径</param>
    /// <param name="theta">与u的角度</param>
    /// <param name="target">目标点</param>
    /// <returns></returns>
    public static bool IsPointInCircularSector(Vector3 origin, Vector3 u, float dis, float theta, Vector3 target)
    {
        Vector3 direction = target - origin;

        float length = Vector3.Distance(target, origin);
        if (length > dis)
        {
            return false;
        }

        direction = direction.normalized;
        //direction和u都是单位向量，点乘等于cos<direction，u>，cos值越大，角度越小
        return Vector3.Dot(direction, u) > Mathf.Cos(theta);
    }

    public static bool IsPointInRectangle()
    {
        return true;
    }


    public static bool IsPointInCircular(Vector3 origin,float dis,Vector3 target)
    {
        return Vector3.Distance(target, origin) < dis;
    }


}
