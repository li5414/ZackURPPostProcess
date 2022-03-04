using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Rouge.Physic
{
    [RequireComponent(typeof(CharacterController))]
public class ZRigidbody : MonoBehaviour
{
    // 重力
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float Gravity = -9.8f;
    // 最大下落速度
    [SerializeField]
    private float _TerminalVelocity = 53.0f;
    // 跌落忽略时长
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float FallTimeout = 0.15f;
    
    // 地面检测相关
    // 是否在地面上
    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    public bool Grounded = true;
    [SerializeField]
    [Tooltip("Useful for rough ground")]
    private float GroundedOffset = -0.14f;
    [SerializeField]
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    private float GroundedRadius = 0.28f;
    [SerializeField]
    [Tooltip("What layers the character uses as ground")]
    private LayerMask GroundLayers;
    // 碰撞球位置
    private Vector3 _SpherePosition = Vector3.zero;
    
    // timeout deltatime
    private float _FallTimeoutDelta;

    public Vector3 velocity
    {
        get { return this._Velocity; }
        set { this._Velocity = value; }
    }
    // 当前速度(只用到了xz分量，y分量由垂直方向速度控制)
    private Vector3 _Velocity = Vector3.zero;
    // 垂直方向速度
    [SerializeField]
    private Vector3 _VerticalVelocity = Vector3.zero;
    // 角色控制器
    private CharacterController _Controller;
    

    // Start is called before the first frame update
    void Awake()
    {
        this._Controller = GetComponent<CharacterController>();
        
        // reset our timeouts on start
        this._FallTimeoutDelta = FallTimeout;
    }
    
    private void GroundedCheck()
    {
        // set sphere position, with offset
        this._SpherePosition.x = this.transform.position.x;
        this._SpherePosition.y = this.transform.position.y - GroundedOffset;
        this._SpherePosition.z = this.transform.position.z;
        
        this.Grounded = Physics.CheckSphere(this._SpherePosition, this.GroundedRadius, this.GroundLayers, QueryTriggerInteraction.Ignore);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Grounded)
        {
            // reset the fall timeout timer
            _FallTimeoutDelta = FallTimeout;
            
            // stop our velocity dropping infinitely when grounded
            if (this._VerticalVelocity.y < 0.0f)
            {
                this._VerticalVelocity.y = -2f;
            }
        }
        else
        {
            // fall timeout
            if (_FallTimeoutDelta >= 0.0f)
            {
                _FallTimeoutDelta -= Time.deltaTime;
            }
        }
        
        // 检测是否在地面上
        GroundedCheck();
        
        // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
        if (this._VerticalVelocity.y < this._TerminalVelocity)
        {
            this._VerticalVelocity.y += this.Gravity * Time.fixedDeltaTime;
        }
        
        _Controller.Move((this._Velocity * Time.fixedDeltaTime) + this._VerticalVelocity * Time.fixedDeltaTime);

        
        Debug.Log("============FixedUpdate()=============");
    }
    
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        {
            Debug.Log("touch gameObject： " + hit.collider.gameObject.name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("trigger gameObject: " + other.gameObject.name);
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("trigger gameObject: " + other.gameObject.name);

    }


    private void OnDrawGizmosSelected()
    {
        Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
        Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

        if (Grounded) Gizmos.color = transparentGreen;
        else Gizmos.color = transparentRed;
			
        this._SpherePosition.x = this.transform.position.x;
        this._SpherePosition.y = this.transform.position.y - GroundedOffset;
        this._SpherePosition.z = this.transform.position.z;
        // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
        Gizmos.DrawSphere(this._SpherePosition, this.GroundedRadius);
    }
}

}

