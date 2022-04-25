using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyUtils
{
    /// <summary>
    /// 计算抛物线所需施加的力 (ForceMode.Force)
    /// </summary>
    /// <param name="mass">质量</param>
    /// <param name="startPosition">起点</param>
    /// <param name="finalPosition">终点</param>
    /// <param name="maxHeightOffset">高度</param>
    /// <param name="rangeOffset"></param>
    /// <returns></returns>
    public static Vector3 CalculateForce(float mass, Vector3 startPosition, Vector3 finalPosition, float maxHeightOffset = 0.0f,
        float rangeOffset = 0.11f)
    {
        Vector3 initVelocity = FindInitialVelocity(startPosition, finalPosition, maxHeightOffset, rangeOffset);
        // ForceMode.Impulse
        Vector3 force = initVelocity * mass;
        // ForceMode.Force
        force = force / Time.fixedDeltaTime;
        return force;
    }
    
    /**findInitialVelocity
     * Finds the initial velocity of a projectile given the initial positions and some offsets
     * @param Vector3 startPosition - the starting position of the projectile
     * @param Vector3 finalPosition - the position that we want to hit
     * @param float maxHeightOffset (default=0.6f) - the amount we want to add to the height for short range shots. We need enough clearance so the
     * ball will be able to get over the rim before dropping into the target position
     * @param float rangeOffset (default=0.11f) - the amount to add to the range to increase the chances that the ball will go through the rim
     * @return Vector3 - the initial velocity of the ball to make it hit the target under the current gravity force.
     * 
     * Vector3 tt = findInitialVelocity (gameObject.transform.position, target.transform.position);
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody> ();
            Debug.Log (tt);
            rigidbody.AddForce(tt*rigidbody.mass,ForceMode.Impulse);

     */
    public static Vector3 FindInitialVelocity(Vector3 startPosition, Vector3 finalPosition, float maxHeightOffset = 0.0f, float rangeOffset = 0.11f)
    {
        // get our return value ready. Default to (0f, 0f, 0f)
        Vector3 newVel = new Vector3();
        // Find the direction vector without the y-component
        /// /找到未经y分量的方向矢量//
        Vector3 direction = new Vector3(finalPosition.x, 0f, finalPosition.z) - new Vector3(startPosition.x, 0f, startPosition.z);
        // Find the distance between the two points (without the y-component)
        //发现这两个点之间的距离（不y分量）//
        float range = direction.magnitude;
        // Add a little bit to the range so that the ball is aiming at hitting the back of the rim.
        // Back of the rim shots have a better chance of going in.
        // This accounts for any rounding errors that might make a shot miss (when we don't want it to).
        range += rangeOffset;
        // Find unit direction of motion without the y component
        Vector3 unitDirection = direction.normalized;
        // Find the max height
        // Start at a reasonable height above the hoop, so short range shots will have enough clearance to go in the basket
        // without hitting the front of the rim on the way up or down.
        float maxYPos = finalPosition.y + maxHeightOffset;
        // check if the range is far enough away where the shot may have flattened out enough to hit the front of the rim
        // if it has, switch the height to match a 45 degree launch angle
        //if (range / 2f > maxYPos)
        //  maxYPos = range / 2f;
        if(maxYPos < startPosition.y)
            maxYPos =  startPosition.y;

        // find the initial velocity in y direction
        /// /发现在y方向上的初始速度//
        float ft;


        ft = -2.0f * Physics.gravity.y * (maxYPos - startPosition.y);
        if(ft<0)
            ft = 0f;

        newVel.y = Mathf.Sqrt(ft);
        // find the total time by adding up the parts of the trajectory
        // time to reach the max
        //发现的总时间加起来的轨迹的各部分//
        //时间达到最大//

        ft = -2.0f * (maxYPos - startPosition.y) / Physics.gravity.y;
        if(ft<0)
            ft = 0f;

        float timeToMax = Mathf.Sqrt(ft);
        // time to return to y-target
        //时间返回到y轴的目标//

        ft = -2.0f * (maxYPos - finalPosition.y) / Physics.gravity.y;
        if(ft<0)
            ft = 0f;

        float timeToTargetY = Mathf.Sqrt(ft);
        // add them up to find the total flight time
        //把它们加起来找到的总飞行时间//
        float totalFlightTime;

        totalFlightTime = timeToMax + timeToTargetY;

        // find the magnitude of the initial velocity in the xz direction
        /// /查找的初始速度的大小在xz方向//
        float horizontalVelocityMagnitude = range / totalFlightTime;
        // use the unit direction to find the x and z components of initial velocity
        //使用该单元的方向寻找初始速度的x和z分量//
        newVel.x = horizontalVelocityMagnitude * unitDirection.x;
        newVel.z = horizontalVelocityMagnitude * unitDirection.z;
        return newVel;
    }
    
    /// <summary>
    /// 预测Rigidbody运动轨迹 (Rigidbody扩展)
    /// </summary>
    /// <param name="that"></param>
    /// <param name="stepCount">运动轨迹点位数量，数量越多，得到的轨迹越远，计算量也就越多</param>
    /// <param name="calculateCountPerStep">每相邻点位之间，模拟的次数，次数越多，相邻点位距离越远（轨迹也会随之变远），但精度会降低</param>
    /// <param name="addedSpeed">要增加的速度</param>
    /// <param name="addedForce">要施加的力</param>
    /// <returns>运动轨迹点位位置及该点的速度</returns>
    public static List<Vector3[]> CalculateMovements(Rigidbody that, int stepCount, int calculateCountPerStep, Vector3 addedForce)
    {
        List<Vector3[]> movePath = new List<Vector3[]>();
        //计算刚体现有速度
        Vector3 vel = !that.isKinematic ? that.velocity : Vector3.zero;
        //计算刚体重力
        Vector3 gra = (that.useGravity && !that.isKinematic) ? Physics.gravity : Vector3.zero;
        //根据所需数据计算轨迹
        CalculateMovements(movePath, that.transform.position, vel, gra, stepCount, calculateCountPerStep, addedForce, that.mass, that.drag);

        return movePath;
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
    public static void CalculateMovements(List<Vector3[]> movePath, Vector3 position, Vector3 velocity, Vector3 gravity, int stepCount, int calculateCountPerStep, Vector3 addedForce, float mass, float drag)
    {
        //将受力转化为速度（动量公式 F*t = M*V）
        Vector3 addedVel = addedForce * Time.fixedDeltaTime / mass;
        //速度和
        Vector3 vel = velocity + addedVel;
        //点位的位置和该位置下的速度
        Vector3[] calc = new Vector3[] { position, vel };
        //依次计算所有点位
        for (var i = 0; i < stepCount; ++i)
        {
            //使用上一点位的位置和速度信息，计算下一点位
            calc = PredictOneStep(calc[0], calc[1], gravity, drag, calculateCountPerStep);
            //记录点位位置速度信息
            movePath.Add(new Vector3[2] { calc[0], calc[1] });
        }
    }

    /// <summary>
    /// 向前预测一帧
    /// </summary>
    /// <param name="pos">起始位置</param>
    /// <param name="vel">起始速度</param>
    /// <param name="gra">重力</param>
    /// <param name="drag">阻力</param>
    /// <param name="count">预测数量</param>
    /// <param name="oneStepFrames">预测一次的步长(几次FixedUpdate)</param>
    /// <returns></returns>
    static Vector3[] PredictOneStep(Vector3 pos, Vector3 vel, Vector3 gravity, float drag, int count)
    {
        // 数据转化
        // 物理刷新时间间隔
        var dt = Time.fixedDeltaTime;
        // 所受重力转化为速度
        var graVel = gravity * dt;
        // 抵消掉阻力之后的受力系数（drag * dt：所受阻力）（不懂这个计算方式）
        var dragDt = 1 - drag * dt;
        // 确保系数不为负
        dragDt = dragDt < 0 ? 0 : dragDt;
        // 模拟速度位置
        for (int i = 0; i < count; i++)
        {
            // 新速度 = (原速度 + 重力产生的速度) * 系数
            vel = (vel + graVel) * dragDt;
            // 新位置 = 原位置 + 速度产生的位移 + 重力产生的位移（不懂0.5的作用）
            pos = pos + vel * dt + 0.5f * graVel * dt;
        }
        return new Vector3[] { pos, vel };
    }
}
