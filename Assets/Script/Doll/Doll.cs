﻿using System.Collections;
using System.Collections.Generic;
using Global;
using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Doll : MonoBehaviourPun,IPunObservable
{
    //----------------[Public Area]--------------------
    //人偶受到伤害
    public virtual float Hurt { set { _hp -= value * (1 - _defense); } }

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="buff">Buff种类</param>
    /// <param name="time">Buff持续时间</param>
    public void AddBuff(BuffSort buff, float time)
    {
        if (_buff.FindIndex(n => n.Sort == BuffSort.Invulnerable) >= 0)
            return;
        int index = _buff.FindIndex(n => n.Sort == buff);
        if (index >= 0)
        {
            var temp = _buff[index];
            temp.Time += time;
            _buff[index] = temp;
        }
        else
            _buff.Add(new Global.Buff(time, buff));
    }

    /// <summary>
    /// 移除全部Buff
    /// </summary>
    public void RemoveBuff()
    {
        _buff.Clear();
    }

    /// <summary>
    /// 移除指定buff
    /// </summary>
    /// <param name="buff">被移除buff</param>
    public void RemoveBuff(BuffSort buff)
    {
        int index = _buff.FindIndex(n => n.Sort == buff);
        if (index >= 0)
            _buff.RemoveAt(index);
    }

    public bool EnterCheck(Controller player)
    {
        if (!Owner)
        {
            _owner = player;
            player.PlayerAction = Action;
            photonView.TransferOwnership(player.photonView.Owner);
            return true;
        }
        return false;
    }

    //private float _mHp;
    //private float _mAtk;
    //private float _mSpd;
    //private float _hp;
    //private float _atk;
    //private float _spd;
    //private float _defense;
    //private int _damagedNumber;
    //private float _mCd;
    //private float _cd;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Owner);
            stream.SendNext(_damagedNumber);
        }
        else
        {
            this._owner = (Controller)stream.ReceiveNext();
            this._damagedNumber = (int)stream.ReceiveNext();
        }
    }

    //拾取物件，子类必须实现
    public abstract bool PickItem(string name);

    //----------------[Protected Area]-----------------
    protected int DamagedNumber { get { return _damagedNumber; } }
    protected Controller Owner { get { return _owner; } }
    protected Rigidbody Rigi { get { return _rigi; } }
    protected float ATK { get { return _atk; } }
    protected float HP { get { return _hp; } }
    protected float SPD { get { return _spd; } }
    protected List<Buff> Buff { get { return _buff; } }

    /// <summary>
    /// 暂时提升属性
    /// </summary>
    /// <param name="atk">提升攻击力值</param>
    /// <param name="spd">提升速度值</param>
    /// <param name="def">提升防御力值</param>
    /// <param name="time">提升属性时间</param>
    /// <returns></returns>
    protected void AttributePromotion(float atk,float spd,float def,float time)
    {
        if (!_attrPromoted)
        {
            _tempAtk = _atk;
            _tempSpd = _spd;
            _tempDefense = _defense;
            _attrPromoted = true;
            StartCoroutine(AttrProTimeCalc(atk, spd, def, time));
        }
        else
        {
            StopCoroutine("AttrProTimeCalc");
            StartCoroutine(AttrProTimeCalc(atk, spd, def, time));
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Doll");
        if (GetComponent<Rigidbody>())
            _rigi = GetComponent<Rigidbody>();
        _dollAreaPrefab = Resources.Load<GameObject>("Prefab/Doll/DollArea");
        _dollArea = Instantiate(_dollAreaPrefab, transform);
        _damagedNumber = 0;
        _defense = 0;
        _owner = null;
        _buff = new List<Buff>();
        //photonView.RPC("ReInit", RpcTarget.All);
        ReInit();
    }

    /// <summary>
    /// 相当于Update函数
    /// </summary>
    protected abstract void Loop();

    /// <summary>
    /// 人偶基础移动
    /// </summary>
    protected virtual void Move()
    {
        transform.SampleMove(SPD);
    }

    /// <summary>
    /// 人偶基础跳跃
    /// </summary>
    protected virtual void Jump()
    {
        Rigi.SampleJump();
    }

    /// <summary>
    /// 判断是否有特定buff
    /// </summary>
    /// <param name="buff">查询buff</param>
    /// <returns>true 有</returns>
    protected bool HasBuff(BuffSort buff)
    {
        if(_buff.FindIndex(n => n.Sort == buff) >= 0)
            return true;
        return false;
    }

    #region Private
    private Controller _owner;
    private Rigidbody _rigi;
    private float _mHp;
    private float _mAtk;
    private float _mSpd;
    private float _hp;
    private float _atk;
    private float _spd;
    private float _defense;
    private int _damagedNumber;
    private float _mCd;
    private float _cd;
    private GameObject _dollAreaPrefab;
    private GameObject _dollArea;

    private float _tempAtk;
    private float _tempSpd;
    private float _tempDefense;
    private bool _attrPromoted = false;

    private List<Buff> _buff;

    /// <summary>
    /// 操纵人偶
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns></returns>
    private Vector3 Action()
    {
        if (!Damaged)
        {
            BuffCountDown();
            Loop();
            Move();
            Jump();
            LeaveDoll();
        }
        else
        {
            LeaveDollOwnerFunction();
        }
        return transform.position;
    }

    /// <summary>
    /// 改变人偶拥有者
    /// </summary>
    /// <param name="owner">新拥有者</param>
    private void ChangeOwner(Controller owner)
    {
        if (owner)
        {   
            _owner = owner;
            Owner.transform.SetParent(transform);
            StartCoroutine(WaitForAnimate());
        }
        else
        {
            Owner.transform.SetParent(null);
            _owner = owner;
        }
    }

    private IEnumerator WaitForAnimate()
    {
        yield return new WaitForSeconds(1f);
        _owner.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 离开人偶
    /// </summary>
    private void LeaveDoll()
    {
        if (Input.GetKeyDown(Key.Leave))
        {
            LeaveDollOwnerFunction();
        }
    }

    private void LeaveDollOwnerFunction()
    {
        transform.tag = "Untagged";
        Owner.PlayerAction -= Action;
        Owner.hasDoll = false;
        //ChangeOwner(null);
        photonView.RPC("ReInit", RpcTarget.All);
        photonView.TransferOwnership(0);
        Owner.LeaveDoll();
        _owner = null;
        _cd = _mCd;
    }

    /// <summary>
    /// 判断人偶是否被损坏
    /// </summary>
    private bool Damaged
    {
        get
        {
            if (HP < 0)
            {
                _damagedNumber++;
                LeaveDollOwnerFunction();
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// 重置人偶状态
    /// </summary>
    [PunRPC]
    private void ReInit()
    {
        if (_damagedNumber == 0)
        {
            _mHp = Parameter.MaxHP;
            _mAtk = Parameter.MaxATK;
            _mSpd = Parameter.MaxSPD;
            _mCd = Parameter.CoolDown;
        }
        else
        {
            _mHp *= Parameter.DeathDecay;
            _mAtk *= Parameter.DeathDecay;
            _mSpd *= Parameter.DeathDecay;
            _mCd += 10f;
        }
        _hp = _mHp;
        _atk = _mAtk;
        _spd = _mSpd;
    }

    /// <summary>
    /// 人偶可进入冷却时间计算
    /// </summary>
    private void Update()
    {
        if(_damagedNumber > Parameter.CanDestroyNum)
        {
            _dollArea.SetActive(false);
            return;
        }
        if (!Owner)
        {
            if (_cd > 0f)
            {
                _cd -= Time.deltaTime;
                GuiAction?.Invoke(new DollComm(DollCDType.PossessCd, _cd / _mCd));
                _dollArea.SetActive(false);
                return;
            }
            _dollArea.SetActive(true);
            return;
        }
        if(!photonView.IsMine)
            GuiAction?.Invoke(new DollComm(DollCDType.HPBar, _hp / _mHp));
        _dollArea.SetActive(false);
    }

    ///// <summary>
    ///// 判断玩家进入附身范围
    ///// </summary>
    ///// <param name="other">进入范围对象</param>
    //private void OnTriggerStay(Collider other)
    //{
    //    if (!Owner && other.tag == "Player")
    //    {
    //        Controller temp = other.GetComponent<Controller>();
    //        if (temp.hasDoll || _cd > 0f)
    //            return;
    //        if (Input.GetKeyDown(Key.Enter))
    //        {
    //            temp.AnimateTarget = transform.position;
    //            temp.PlayerAction = Action;
    //            temp.hasDoll = true;
    //        }
    //    }
    //}

    /// <summary>
    /// Buff倒计时
    /// </summary>
    private void BuffCountDown()
    {
        for (int i = 0; i < _buff.Count; i++)
        {
            var temp = _buff[i];
            temp.Time -= Time.deltaTime;
            if(temp.Time < 0)
                _buff.RemoveAt(i);
            else
                _buff[i] = temp;
        }
    }

    /// <summary>
    /// 协程，暂时提升属性
    /// </summary>
    /// <param name="atk">提升攻击力值</param>
    /// <param name="spd">提升速度值</param>
    /// <param name="def">提升防御力值</param>
    /// <param name="time">提升属性时间</param>
    /// <returns></returns>
    private IEnumerator AttrProTimeCalc(float atk, float spd, float def, float time)
    {
        _atk += atk;
        _spd += spd;
        _defense += def;
        yield return new WaitForSeconds(time);
        _atk = _tempAtk;
        _spd = _tempSpd;
        _defense = _tempDefense;
        _attrPromoted = false;
    }

    //人偶界面代理
    public GUIDelegate GuiAction;
    #endregion
}
