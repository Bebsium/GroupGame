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

    /// <summary>
    /// 设置阵营
    /// </summary>
    /// <param name="faction">新阵营</param>
    public void SetFaction(Faction faction)
    {
        _faction = faction;
    }

    public bool SoulAnimate(Vector3 pos)
    {
        _rigi.isKinematic = true;
        if (Vector3.Distance(transform.position,pos) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, pos, 0.1f);
            return false;
        }
        return true;
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

    private void Update()
    {
        if (PlayerAction != null)
        {
            switch (PlayerAction(this))
            {
                case SoulState.Enter:
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
        _rigi.isKinematic = true;
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
    }

    public ActionDelegate PlayerAction;
    #endregion
}
