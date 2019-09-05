using System.Collections;
using System.Collections.Generic;
using Global;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Doll : MonoBehaviourPun,IPunObservable,IPunOwnershipCallbacks
{
    //----------------[Public Area]--------------------
    //item
    public GameObject item;


    public AttackState attackState;
    public ThrowState throwState;

    public bool itemSetting;
    public bool isAttack;
    //KO值回傳-------------
    public float Knockout { set { _ko -= value; } }
    private float _ko;
    //--------------------


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

    public bool CanEnter { get { if (_cd > 0f) return false; else return true; } }

    public bool EnterCheck(Controller player)
    {
        if (_cd > 0f)
            return false;
        if (Owner == "" || Owner == null)
        {
            _controller = player;
            photonView.RPC("EnterRPC", RpcTarget.All, player.photonView.Owner.UserId);
            player.PlayerAction = Action;
            photonView.TransferOwnership(player.photonView.Owner);
            return true;
        }
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (Owner == null || Owner == "")
            return;
        if (stream.IsWriting)
        {
            stream.SendNext(_hp);
            stream.SendNext(_atk);
            stream.SendNext(_spd);
            stream.SendNext(_defense);
        }
        else if (stream.IsReading)
        {
            this._hp = (float)stream.ReceiveNext();
            this._atk = (float)stream.ReceiveNext();
            this._spd = (float)stream.ReceiveNext();
            this._defense = (float)stream.ReceiveNext();
        }
    }

    //拾取物件，子类必须实现
    public abstract bool PickItem(string name);

    //----------------[Protected Area]-----------------
    protected int DamagedNumber { get { return _damagedNumber; } }
    protected string Owner { get { return _owner; } }
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
    bool used;
    float time;
    Vector3 shootpoint;
    public GameObject cursor;
    public LayerMask layer;
    private Camera cam;
    public GameObject shootRange;
    public Vector3[] itmeMove = new Vector3[50];
    int numPoints = 50;
    LineRenderer lineRenderer;
    public Material line;
    public float line_hight = 10;
    protected bool isShoot;
    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.tag = "Doll";
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
        //初始KO值
        _ko = 100;
        // throw
        cam = Camera.main;
        shootRange = Instantiate(cursor, transform);
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = line;
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
    [SerializeField]
    private string _owner;
    private Controller _controller;
    private Rigidbody _rigi;
    private float _mHp;
    private float _mAtk;
    private float _mSpd;
    [SerializeField]
    private float _hp;
    private float _atk;
    private float _spd;
    private float _defense;
    private int _damagedNumber;
    private float _mCd;
    [SerializeField]
    private float _cd;
    private GameObject _dollAreaPrefab;
    private GameObject _dollArea;

    private float _tempAtk;
    private float _tempSpd;
    private float _tempDefense;
    private bool _attrPromoted = false;

    private List<Buff> _buff = new List<Buff>();

    /// <summary>
    /// 操纵人偶
    /// </summary>
    /// <param name="player">玩家</param>
    /// <returns></returns>
    private Vector3 Action()
    {
        if(Owner != PhotonNetwork.LocalPlayer.UserId)
        {
            return transform.position;
        }
            
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
        _controller.PlayerAction -= Action;
        _controller.hasDoll = false;
        photonView.TransferOwnership(0);
        _controller.LeaveDoll();
        photonView.RPC("LeaveRPC", RpcTarget.All, _damagedNumber);
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
    //[PunRPC]
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
        if (Owner == "" || Owner == null)
        {
            transform.SendMessage("NickName", "");
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
        else if (Owner != PhotonNetwork.LocalPlayer.UserId)
        {
            GuiAction?.Invoke(new DollComm(DollCDType.HPBar, _hp / _mHp));
            transform.SendMessage("NickName", photonView.Owner.NickName);
        }
        _dollArea.SetActive(false);
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

    [PunRPC]
    public void EnterRPC(string con)
    {
        _owner = con;
    }

    [PunRPC]
    public void LeaveRPC(int n)
    {
        _owner = null;
        _damagedNumber = n;
        ReInit();
        _cd = _mCd;
    }

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        
    }

    public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
    {
        //if (targetView.IsSceneView)
        //{
        //    SendMessage("NickName", "");
        //}
        //else if (targetView.IsOwnerActive) 
        //{
        //    if (!photonView.IsMine)
        //    {
        //        SendMessage("NickName", targetView.Owner.NickName);
        //    }
        //}
    }

    //Item
    protected virtual void ItemType(string name)
    {
        if (name == "Stick")
        {
            attackState = AttackState.StickAttack;
            throwState = ThrowState.lightThrow;
        }
        else if (name == "Nightstand")
        {
            attackState = AttackState.HeavyAttack;
            throwState = ThrowState.heavyThrow;
        }
        else if (name == "Pillow" || name == "Board")
        {
            attackState = AttackState.CampstoolAttack;

            if (name == "Pillow")
            {
                throwState = ThrowState.heavyThrow;
            }
            else
            {
                throwState = ThrowState.stickThrow;
            }
        }
    }

    protected virtual void ItemUse(AttackState attackState, ThrowState throwState)
    {
        switch (attackState)
        {
            case AttackState.StickAttack:
                StickAttack();
                break;
            case AttackState.HeavyAttack:
                HeavyAttack();
                break;
            case AttackState.CampstoolAttack:
                CampstoolAttack();
                break;
        }
        switch (throwState)
        {
            case ThrowState.lightThrow:
                LightThrow();
                break;
            case ThrowState.heavyThrow:
                HeavyThrow();
                break;
            case ThrowState.stickThrow:
                StickThrow();
                break;
        }
    }
    //Attack
    void StickAttack()
    {

    }

    void HeavyAttack()
    {

    }

    void CampstoolAttack()
    {

    }

    //Throw
    void LightThrow()
    {

    }

    void HeavyThrow()
    {

    }

    void StickThrow()
    {

    }
    //attack-------------------
    [SerializeField]
    private GameObject temp;
    protected virtual void Attack()
    {
        //右クリックすると、shootRange出てくる
        if (!Input.GetMouseButton(1))
        {
            shootRange.SetActive(false);
        }

        if (item != null)
        {
            //GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            temp = Instantiate(item, gameObject.transform);
            temp.transform.position = transform.position + transform.forward * 0.5f;
            temp.name = "Putted Item";
            StartCoroutine(NotFall(temp));
            itemSetting = true;

            temp.GetComponent<Item>().picked = true;
            temp.GetComponent<Item>().item = true;
            item = null;
            used = true;
        }
        else
        {
            if (temp == null)
            {
                itemSetting = false;
                //attack
                if (Input.GetMouseButtonDown(0) && !isAttack)
                {
                    isAttack = true;
                    //ものをあってた場合は1秒過ぎるとhurtなし
                    StartCoroutine(Wait());
                }

            }
            //throw
            if (Input.GetMouseButton(1) && used)
            {
                //射擊方向
                ToMouse();
            }

            //丟道具
            if (Input.GetMouseButtonUp(1) && used)
            {
                used = false;
                isShoot = false;
                temp.transform.SetParent(null);
                temp = null;
                lineRenderer.positionCount = 0;

            }
        }

    }

    protected virtual IEnumerator Wait()
    {
        yield return new WaitForSeconds(1f);
        if (item != null)
        {
            temp.GetComponent<Item>().isAttack = false;
        }
        else
        {
            isAttack = false;
        }

    }
    //Item attack--------------------------
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Item" && other.gameObject != temp)
        {
            target_Item = other.gameObject.GetComponent<Item>();
            if (target_Item.isAttack)
            {
                //print(other.gameObject);
                //碰到道具 -HP -KO
                Hurt = target_Item.Attack();
                Knockout = target_Item.Knockout();
                target_Item.isAttack = false;
            }

        }
        if (other.gameObject.tag == "DollHand")
        {
            Hurt = 5;
            isAttack = false;
        }

    }


    //-------------------------------------------

    protected Item_hat target_Hat;
    protected Item_Rose target_Rose;
    protected Item target_Item;

    //-------------------------------------------
    protected virtual void OnCollisionEnter(Collision collision)
    {
        //Doll_hot 以外的其他玩偶共通-----
        if (collision.gameObject.tag == "Hat")
        {
            target_Hat = collision.gameObject.GetComponent<Item_hat>();

            if (target_Hat.isWear)
            {
                //碰到對方時，產生效果
                AddBuff(Global.BuffSort.Prisoner, 2f);
                //print("AddBuff");
            }
        }
        //---------------------------------
        //Doll_Rose 以外的其他玩偶共通-----
        if (collision.gameObject.tag == "Rose")
        {
            target_Rose = collision.gameObject.GetComponent<Item_Rose>();

            if (target_Rose.isAttack)
            {
                //碰到對方時，產生效果
                Hurt = target_Rose.SkillAttack();
                AddBuff(Global.BuffSort.Stun, 5f);
                //print("AddBuff");
            }
        }
        //------------------------------
    }
    protected virtual void SkillHurt()
    {
        if (target_Hat != null)
        {
            if (target_Hat.isAttack)
            {
                AddBuff(Global.BuffSort.Stun, 5f);
                Hurt = target_Hat.SkillAttack();
                target_Hat.isAttack = false;
            }
        }
    }
    //射擊---------------------------------
    protected virtual void ToMouse()
    {
        shootpoint = transform.position + transform.forward;
        lineRenderer.positionCount = numPoints;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        layer = 1 << 0;
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {
            shootRange.SetActive(true);
            shootRange.transform.position = hit.point + Vector3.up * 0.1f;

            //射擊的位置

            Vector3 hight = (hit.point + transform.position) / 2;
            hight += new Vector3(0, line_hight, 0);

            DrawLine(shootpoint, hight, shootRange.transform);
            transform.LookAt(shootRange.transform);
        }

        isShoot = true;
    }

    protected virtual void DrawLine(Vector3 origin, Vector3 hight, Transform target)
    {
        for (int i = 1; i < numPoints + 1; i++)
        {
            float sd = i / (float)numPoints;
            itmeMove[i - 1] = CalculatQuadraticPoint(origin, hight, target.position, sd);
        }
        lineRenderer.SetPositions(itmeMove);
    }

    protected virtual  Vector3 CalculatQuadraticPoint(Vector3 origin, Vector3 hight, Vector3 target, float speed)
    {
        //return =  (1-t)2P0 + 2(1-t)tP1 + t2P2 
        float x = 1 - speed;
        float sd = speed * speed;
        float xx = x * x;
        Vector3 pos = xx * origin;
        pos += 2 * x * speed * hight;
        pos += sd * target;

        return pos;
    }
    //---------------------------------------


    protected virtual  IEnumerator NotFall(GameObject temp)
    {
        //防止落下
        yield return new WaitForSeconds(1f);
        temp.GetComponent<Rigidbody>().isKinematic = true;
    }
    //人偶界面代理
    public GUIDelegate GuiAction;
    #endregion
}
