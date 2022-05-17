using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SimulateParabolaUtils
{
    private const float k_GRAVITY = -9.8f;
    
    /// <summary>
    /// 计算抛物线初速度
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="maxHeight"></param>
    /// <returns></returns>
    public static Vector3 CalculateParabolaInitialVelocity(Vector3 start, Vector3 end, float maxHeight)
    {
        // 重力加速度
        float g = Math.Abs(k_GRAVITY);
        
        // 起点到最高点的高度差
        float height1 = maxHeight - start.y;
        // 起点到最高点所需时间
        float t1 = (float)Math.Sqrt(2 * height1 / g);
        
        // 最高点到终点的高度差
        float height2 = maxHeight - end.y;
        // 最高点到终点所需时间
        float t2 = (float)Math.Sqrt(2 * height2 / g);

        // 水平方向偏移
        Vector3 offset_horizontal = new Vector3(end.x - start.x, 0, end.z - start.z);
        // 水平方向初速度
        Vector3 velocity_horizontal = offset_horizontal / (t1 + t2);
        // 垂直方向初速度
        Vector3 velocity_vertical = new Vector3(0, g * t1, 0);

        // 初速度
        return velocity_horizontal + velocity_vertical;
    }

    /// <summary>
    /// 抛物线采样
    /// </summary>
    /// <param name="start">注意发生碰撞时，start位置要改成碰撞点</param>
    /// <param name="initialVelocity"></param>
    /// <param name="time">注意发生碰撞时，time要重置</param>
    /// <returns></returns>
    public static Vector3 SampleParabolaPoint(Vector3 start, Vector3 initialVelocity, float time)
    {
        // 重力加速度
        float g = k_GRAVITY;
        // 水平方向参数方程: v0*t
        // 垂直方向参数方程: v0*t + 1/2*g*t^2
        Vector3 offset = initialVelocity * time;
        offset.y += 0.5f * g * time * time;

        return start + offset;
    }

    /// <summary>
    /// 计算反弹之后的速度
    /// </summary>
    /// <param name="initialVelocity"></param>
    /// <param name="time"></param>
    /// <param name="normal">反弹处法线</param>
    /// <returns></returns>
    public static Vector3 CalculateBounceVelocity(Vector3 initialVelocity, float time, Vector3 normal)
    {
        // 重力加速度
        float g = k_GRAVITY;

        // 计算碰撞前的速度(入射速度)
        Vector3 velocityIn = initialVelocity;
        // 垂直方向速度
        velocityIn.y += g * time;
        
        // 计算碰撞后的速度
        return Vector3.Reflect(velocityIn, normal);
    }
    
}