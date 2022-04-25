using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RigidbodyUtils
{
    /// <summary>
    /// 预测Rigidbody运动轨迹 (Rigidbody扩展)
    /// </summary>
    /// <param name="that"></param>
    /// <param name="stepCount">运动轨迹点位数量，数量越多，得到的轨迹越远，计算量也就越多</param>
    /// <param name="calculateCountPerStep">每相邻点位之间，模拟的次数，次数越多，相邻点位距离越远（轨迹也会随之变远），但精度会降低</param>
    /// <param name="addedSpeed">要增加的速度</param>
    /// <param name="addedForce">要施加的力</param>
    /// <returns>运动轨迹点位位置及该点的速度</returns>
    public static List<Vector3[]> CalculateMovements(this Rigidbody that, int stepCount, int calculateCountPerStep, Vector3 addedSpeed, Vector3 addedForce)
    {
        //计算刚体现有速度
        Vector3 vel = !that.isKinematic ? that.velocity : Vector3.zero;
        //计算刚体重力
        Vector3 gra = (that.useGravity && !that.isKinematic) ? Physics.gravity : Vector3.zero;
        //根据所需数据计算轨迹
        return CalculateMovements(that.transform.position, vel, gra, stepCount, calculateCountPerStep, addedSpeed, addedForce, that.mass, that.drag);
    }
    
    /// <summary>
    /// 预测Rigidbody运动轨迹
    /// </summary>
    /// <param name="position">刚体起始位置</param>
    /// <param name="velocity">刚体原有速度</param>
    /// <param name="gravity">刚体重力</param>
    /// <param name="stepCount">点位数量</param>
    /// <param name="calculateCountPerStep">模拟精度</param>
    /// <param name="addedSpeed">增加速度</param>
    /// <param name="addedForce">增加受力</param>
    /// <param name="mass">刚体质量</param>
    /// <param name="drag">所受阻力</param>
    /// <returns>运动轨迹点位的位置与速度</returns>
    public static List<Vector3[]> CalculateMovements(Vector3 position, Vector3 velocity, Vector3 gravity, int stepCount, int calculateCountPerStep, Vector3 addedSpeed, Vector3 addedForce, float mass, float drag)
    {
        List<Vector3[]> movePath = new List<Vector3[]>();
        //将受力转化为速度（动量公式 F*t = M*V）
        Vector3 addedVel = addedForce / mass * Time.fixedDeltaTime;
        //速度和
        Vector3 vel = velocity + addedSpeed + addedVel;
        //点位的位置和该位置下的速度
        Vector3[] calc = new Vector3[] { position, vel };
        //依次计算所有点位
        for (var i = 0; i < stepCount; ++i)
        {
            //使用上一点位的位置和速度信息，计算下一点位
            calc = PredictOneFrame(calc[0], calc[1], gravity, drag, calculateCountPerStep);
            //记录点位位置速度信息
            movePath.Add(new Vector3[2] { calc[0], calc[1] });
        }
        //返回值
        return movePath;
    }

    /// <summary>
    /// 向前预测一帧
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="vel"></param>
    /// <param name="gra"></param>
    /// <param name="drag"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    static Vector3[] PredictOneFrame(Vector3 pos, Vector3 vel, Vector3 gra, float drag, int count)
    {
        //数据转化
        //物理刷新时间间隔
        var dt = Time.fixedDeltaTime;
        //所受重力转化为速度
        var graVel = gra * dt;
        //抵消掉阻力之后的受力系数（drag * dt：所受阻力）（不懂这个计算方式）
        var dragDt = 1 - drag * dt;
        //确保系数不为负
        dragDt = dragDt < 0 ? 0 : dragDt;
        //模拟速度位置
        for (int i = 0; i < count; i++)
        {
            //新速度 = (原速度 + 重力产生的速度) * 系数
            vel = (vel + graVel) * dragDt;
            //新位置 = 原位置 + 速度产生的位移 + 重力产生的位移（不懂0.5的作用）
            pos = pos + vel * dt + 0.5f * graVel * dt;
        }
        return new Vector3[] { pos, vel };
    }
}
