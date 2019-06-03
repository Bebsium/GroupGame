﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Global;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Controller : MonoBehaviourPunCallbacks
{
    //----------------[Public Area]--------------------
    public string playerName;
    [HideInInspector]
    public bool hasDoll = false;
    public Vector3 AnimateTarget { set { _animateTarget = value; } }


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
        _pv = GetComponent<PhotonView>();
        hasDoll = false;
    }

    /// <summary>
    /// 人物移动
    /// </summary>
    protected virtual void Move()
    {
        transform.SampleMove(Parameter.SoulSPD);
    }

    /// <summary>
    /// 人物跳跃
    /// </summary>
    protected virtual void Jump()
    {
        Rigi.SampleJump();
    }

    /// <summary>
    /// 相当于Update
    /// </summary>
    protected abstract void Loop();

    #region Private

    private Rigidbody _rigi;
    private Collider _coll;
    private Renderer _render;
    [SerializeField]
    private PhotonView _pv;
    private Vector3 _animateTarget;
    private bool _animateEnd = false;

    private void Update()
    {
        if (!_pv.IsMine)
            return;
        if (PlayerAction != null)
        {
            SoulState temp;
            switch (temp = PlayerAction(this))
            {
                case SoulState.Enter:
                    SoulAnimate();
                    break;
                case SoulState.Stay:
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
    /// 灵魂进入人偶运动动画
    /// </summary>
    /// <returns>true 动画结束 灵魂状态改变； false 动画进行中</returns>
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
        _coll.enabled = false;
        _render.enabled = false;
        return true;
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

    public ActionDelegate PlayerAction;
    #endregion
}
