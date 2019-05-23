using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    //----------------[Public Area]--------------------
    //玩家阵营 Player1 Player2 Computer
    public Faction Faction { get { return _faction; } }
    [HideInInspector]
    public bool hasDoll = false;

    public Vector3 AnimateTarget { set { _animateTarget = value; } }


    /// <summary>
    /// 设置阵营
    /// </summary>
    /// <param name="faction">新阵营</param>
    public void SetFaction(Faction faction)
    {
        _faction = faction;
    }

    //----------------[Protected Area]-----------------

    protected Rigidbody Rigi { get { return _rigi; } }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        _rigi = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _render = GetComponent<Renderer>();
        hasDoll = false;
    }

    /// <summary>
    /// 人物移动
    /// </summary>
    protected virtual void Move()
    {
        transform.SampleMove(Parameter.SoulSPD, Faction);
    }

    /// <summary>
    /// 人物跳跃
    /// </summary>
    protected virtual void Jump()
    {
        Rigi.SampleJump(Faction);
    }

    /// <summary>
    /// 相当于Update
    /// </summary>
    protected abstract void Loop();

    #region Private

    private Rigidbody _rigi;
    private Collider _coll;
    private Renderer _render;
    private Faction _faction;
    [SerializeField]
    private Vector3 _animateTarget;
    private bool _animateEnd = false;

    private void Update()
    {
        if (PlayerAction != null)
        {
            switch (PlayerAction(this))
            {
                case SoulState.Enter:
                    if (SoulAnimate()) 
                        EnterDoll();
                    break;
                case SoulState.Leave:
                    LeaveDoll();
                    break;
            }
        }
        else
        {
            Move();
            Jump();
            Loop();
        }
    }

    /// <summary>
    /// 进入人偶的灵魂状态改变
    /// </summary>
    private void EnterDoll()
    {
        //_rigi.isKinematic = true;
        _coll.enabled = false;
        _render.enabled = false;
    }

    /// <summary>
    /// 离开人偶的灵魂状态改变
    /// </summary>
    private void LeaveDoll()
    {
        _rigi.isKinematic = false;
        _coll.enabled = true;
        _render.enabled = true;
        _animateEnd = false;
    }

    /// <summary>
    /// 灵魂进入人偶运动动画
    /// </summary>
    /// <returns>true 动画结束； false 动画进行中</returns>
    private bool SoulAnimate()
    {
        if (_animateEnd)
            return true;
        
        _rigi.isKinematic = true;
        if (Vector3.Distance(transform.position, _animateTarget) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, _animateTarget, 0.1f);
            return false;
        }
        _animateEnd = true;
        return true;
    }

    public ActionDelegate PlayerAction;
    #endregion
}
