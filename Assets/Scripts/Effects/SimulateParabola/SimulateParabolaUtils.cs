using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class SimulateParabolaUtils
{
    /// <summary>
    /// 获取反弹的对称点数组
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="hit">碰撞点</param>
    /// <param name="normal">碰撞点法线</param>
    /// <param name="points">理想位置数组(假设碰撞未发生)</param>
    /// <param name="startIdx">数组处理起始idx</param>
    /// <param name="endIdx">数组处理结束idx</param>
    public static void GetReflectSymmetryPoints(Vector3 start, Vector3 hit, Vector3 normal, List<Vector3> points, int startIdx, int endIdx)
    {
        // normal.y = 0;
        // normal = normal.normalized;
        
        float height;
        Vector3 point;
        Vector3 vec_SP, vec_PH, vec_2NPH, vec_SR, result;
        for (int i = startIdx; i < endIdx; ++i)
        {
            point = points[i];
            
            // height
            height = point.y;
            // start->point
            vec_SP = point - start;
            // point->hit
            vec_PH = hit - point;
            // 2 * (dot(normal, point->hit)) * normal
            vec_2NPH = 2 * Vector3.Dot(normal, vec_PH) * normal;
            // start->result = start->point + 2 * (dot(normal, point->hit)) * normal
            vec_SR = vec_SP + vec_2NPH;
        
            // result = start->result + start
            result = vec_SR + start;
            result.y = height;

            points[i] = result;
        }
    }
    
    /// <summary>
    /// 获取反弹的对称点
    /// </summary>
    /// <param name="start">起始点</param>
    /// <param name="hit">碰撞点</param>
    /// <param name="normal">碰撞点法线</param>
    /// <param name="point">理想位置(假设碰撞未发生)</param>
    /// <returns></returns>
    public static Vector3 GetReflectSymmetryPoint(Vector3 start, Vector3 hit, Vector3 normal, Vector3 point)
    {
        normal.y = 0;
        normal = normal.normalized;
        // height
        float height = point.y;
        
        // start->point
        Vector3 vec_SP = point - start;
        // point->hit
        Vector3 vec_PH = hit - point;
        // 2 * (dot(normal, point->hit)) * normal
        Vector3 vec_2NPH = 2 * Vector3.Dot(normal, vec_PH) * normal;
        // start->result = start->point + 2 * (dot(normal, point->hit)) * normal
        Vector3 vec_SR = vec_SP + vec_2NPH;
        
        // result = start->result + start
        Vector3 result = vec_SR + start;
        result.y = height;
        return result;
    }
    
    /// <summary>
    /// 获取存储贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起始点</param>
    /// <param name="controlPoint">控制点</param>
    /// <param name="endPoint">目标点</param>
    /// <param name="segmentNum">采样点的数量</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetLineBeizerList(Vector3 startPoint,  Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = calculateLineBezierPoint(t, startPoint, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;
 
    }
 
    /// <summary>
    /// 获取存储的二次贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起始点</param>
    /// <param name="controlPoint">控制点</param>
    /// <param name="endPoint">目标点</param>
    /// <param name="segmentNum">采样点的数量</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetCubicBeizerList(Vector3 startPoint, Vector3 controlPoint, Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = calculateCubicBezierPoint(t, startPoint, controlPoint, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;
 
    }
 
    /// <summary>
    /// 获取存储的三次贝塞尔曲线点的数组
    /// </summary>
    /// <param name="startPoint">起始点</param>
    /// <param name="controlPoint1">控制点1</param>
    /// <param name="controlPoint2">控制点2</param>
    /// <param name="endPoint">目标点</param>
    /// <param name="segmentNum">采样点的数量</param>
    /// <returns>存储贝塞尔曲线点的数组</returns>
    public static Vector3[] GetThreePowerBeizerList(Vector3 startPoint, Vector3 controlPoint1, Vector3 controlPoint2 , Vector3 endPoint, int segmentNum)
    {
        Vector3[] path = new Vector3[segmentNum];
        for (int i = 1; i <= segmentNum; i++)
        {
            float t = i / (float)segmentNum;
            Vector3 pixel = calculateThreePowerBezierPoint(t, startPoint, controlPoint1, controlPoint2, endPoint);
            path[i - 1] = pixel;
            Debug.Log(path[i - 1]);
        }
        return path;
 
    }
    
    // 线性贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    private static Vector3 calculateLineBezierPoint(float t, Vector3 p0, Vector3 p1)
    {
        float u = 1 - t;
         
        Vector3 p = u * p0;
        p +=  t * p1;
    
 
        return p;
    }
 
    // 二次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    private static Vector3 calculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
 
        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;
 
        return p;
    }
 
    // 三次贝塞尔曲线，根据T值，计算贝塞尔曲线上面相对应的点
    private static Vector3 calculateThreePowerBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float ttt = tt * t;
        float uuu = uu * u;
 
        Vector3 p = uuu * p0;
        p += 3 * t * uu * p1;
        p += 3 * tt * u * p2;
        p += ttt * p3;
 
        return p;
    }
    
}