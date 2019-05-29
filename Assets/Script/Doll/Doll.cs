﻿using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Doll : MonoBehaviour
{
    //----------------[Public Area]--------------------
    //人偶受到伤害
    public float Hurt { set { _hp -= value; } }

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="buff">Buff种类</param>
    /// <param name="time">Buff持续时间</param>
    public void AddBuff(BuffSort buff, float time)
    {
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
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Doll");
        if (GetComponent<Rigidbody>())
            _rigi = GetComponent<Rigidbody>();
        _dollAreaPrefab = Resources.Load<GameObject>("Prefab/Doll/DollArea");
        _dollArea = Instantiate(_dollAreaPrefab, transform);
        _state = SoulState.Leave;
        _damagedNumber = 0;
        _owner = null;
        _buff = new List<Buff>();
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
        transform.SampleMove(SPD, Owner.Faction);
    }

    /// <summary>
    /// 人偶基础跳跃
    /// </summary>
    protected virtual void Jump()
    {
        Rigi.SampleJump(Owner.Faction);
    }

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
    private int _damagedNumber;
    private SoulState _state;
    private float _mCd;
    private float _cd;
    private GameObject _dollAreaPrefab;
    private GameObject _dollArea;

    private List<Buff> _buff;

    /// <summary>
    /// 操纵人偶
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns></returns>
    private SoulState Action(Controller player)
    {
        if (!Owner)
        {
            //if (!player.SoulAnimate(transform.position))
            //    return SoulState.Wait;
            transform.tag = "Doll";
            ChangeOwner(player);
            return _state;
        }
        if (!Damaged && _state == SoulState.Stay)
        {
            BuffCountDown();
            Loop();
            Move();
            Jump();
            LeaveDoll();
        }
        return _state;
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
            _state = SoulState.Enter;
            Owner.transform.SetParent(transform);
            StartCoroutine(WaitForAnimate());
        }
        else
        {
            _state = SoulState.Leave;
            Owner.transform.SetParent(null);
            _owner = owner;
        }
    }

    private IEnumerator WaitForAnimate()
    {
        yield return new WaitForSeconds(1f);
        _state = SoulState.Stay;
        _owner.transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// 离开人偶
    /// </summary>
    private void LeaveDoll()
    {
        if ((Owner.Faction == Faction.Player1 && Input.GetKeyDown(Key.Leave))
            || (Owner.Faction == Faction.Player2 && Input.GetKeyDown(Key.Leave2)))
        {
            LeaveDollOwnerFunction();
        }
    }

    private void LeaveDollOwnerFunction()
    {
        transform.tag = "Untagged";
        Owner.PlayerAction -= Action;
        Owner.hasDoll = false;
        ChangeOwner(null);
        ReInit();
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
        GuiAction?.Invoke(new DollComm(DollCDType.HPBar, _hp / _mHp));
        _dollArea.SetActive(false);
    }

    /// <summary>
    /// 判断玩家进入附身范围
    /// </summary>
    /// <param name="other">进入范围对象</param>
    private void OnTriggerStay(Collider other)
    {
        if (!Owner && other.tag == "Player")
        {
            Controller temp = other.GetComponent<Controller>();
            if (temp.hasDoll || _cd > 0f)
                return;
            if ((temp.Faction == Faction.Player1 && Input.GetKeyDown(Key.Enter))
                || (temp.Faction == Faction.Player2 && Input.GetKeyDown(Key.Enter2)))
            {
                temp.AnimateTarget = transform.position;
                temp.PlayerAction = Action;
                temp.hasDoll = true;
            }
        }
    }

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

    //人偶界面代理
    public GUIDelegate GuiAction;
    #endregion
}
