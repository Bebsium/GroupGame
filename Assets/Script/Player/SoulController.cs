using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody))]
public class SoulController : NetworkBehaviour
{
    //----------------[Public Area]--------------------
    [SyncVar]
    public string playerName;
    //[HideInInspector]
    public bool hasDoll = false;

    //BGM
    public List<AudioClip> BGM;

    public void LeaveDoll()
    {
        _isVisible = true;
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();
    //    GetNetIdentity();
    //    SetIdentity();
    //}

    //public override void OnStartLocalPlayer()
    //{
    //    GetNetIdentity();
    //    SetIdentity();
    //}

    //----------------[Protected Area]-----------------

    protected Rigidbody Rigi { get { return _rigi; } }
    public AudioSource Rource{get {return _source;}}

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        _rigi = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _source=GetComponent<AudioSource>();
        _particleRenderer = transform.Find("Renderer").gameObject;
        hasDoll = false;
        _source.volume=PlayerPrefs.GetFloat("BGMVolume");
        if (isLocalPlayer)
            CameraFollow.instance.CameraFollowObj = gameObject;
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
    protected virtual void Loop()
    {

    }

    #region Private
    private AudioSource _source;
    

    private Rigidbody _rigi;
    private Collider _coll;
    private Vector3 _animateTarget;
    private bool _animateEnd = false;
    private bool _isVisible { set {
            _rigi.isKinematic = !value;
            _coll.enabled = value;
            _particleRenderer.SetActive(value);
        } get { return _coll.enabled; } }
    private bool _entering = false;
    private Coroutine _enterCor = null;
    private GameObject _particleRenderer;
    private NetworkInstanceId playerNetID;

    private void Update()
    {
        if (!isLocalPlayer)
            return;
        if (PlayerAction != null)
        {
            transform.position = PlayerAction();
        }
        else
        {
            Enter();
            if (!_rigi.isKinematic)
            {
                Move();
                Jump();
                Loop();
            }
        }
    }

    [Client]
    private void GetNetIdentity()
    {
        playerNetID = GetComponent<NetworkIdentity>().netId;
        CmdTellServerMyIdentity(MakeUniqueIdentity());
    }

    private void SetIdentity()
    {
        print(transform.name);
        transform.name = playerName;
        //if (!isLocalPlayer)
        //{
        //    transform.name = playerName;
        //}
        //else
        //{
        //    transform.name = MakeUniqueIdentity();
        //}
    }

    private string MakeUniqueIdentity()
    {
        string uniqueName = "Player" + playerNetID.ToString();
        PlayerPrefs.SetString("LocalUnique", uniqueName);
        return uniqueName;
    }

    [Command]
    private void CmdTellServerMyIdentity(string name)
    {
        playerName = name;
    }

    private void Enter()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(1))
            if(Physics.Raycast(ray,out RaycastHit hit,10f,LayerMask.GetMask(new string[] { "Doll" })))
            {
                if(_enterCor == null && hit.transform.GetComponent<Doll>().CanEnter){
                    _enterCor = StartCoroutine(Entering(hit.transform));
                    //進入後tag = "Doll"
                    hit.transform.gameObject.tag = "Doll";
                    hasDoll = true;
                }
            }
    }

    private IEnumerator Entering(Transform obj)
    {
        _rigi.isKinematic = true;
        while (Vector3.Distance(obj.position, transform.position) > 0.1f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, obj.position, 0.1f);
        }
        _isVisible = false;
        CmdSetAuth(obj.gameObject);
        //obj.GetComponent<Doll>().Enter(gameObject);
        DollEnter(obj.gameObject);
        
        //print(obj.gameObject.GetComponent<PhotonView>().ViewID);
        _source.clip=BGM[obj.gameObject.GetComponent<Doll>().bgmId];
        _source.Play();
        _enterCor = null;
    }

    [Client]
    void DollEnter(GameObject g)
    {
        g.GetComponent<Doll>().Enter(gameObject);
    }

    [Command]
    private void CmdSetAuth(GameObject obj)
    {
        print("CmdSetAuth: " + obj.name);
        NetworkIdentity networkIdentity = obj.GetComponent<NetworkIdentity>();

        NetworkConnection otherOwner = networkIdentity.clientAuthorityOwner;
        if (otherOwner != null)
            networkIdentity.RemoveClientAuthority(otherOwner);
        networkIdentity.localPlayerAuthority = true;
        networkIdentity.AssignClientAuthority(connectionToClient);
    }

    public ActionDelegate PlayerAction;
    #endregion
}
