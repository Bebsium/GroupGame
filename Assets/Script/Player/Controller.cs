using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(NetworkTransform))]
[RequireComponent(typeof(Rigidbody))]
public class Controller : NetworkBehaviour
{
    //----------------[Public Area]--------------------
    [SyncVar]
    public string playerUniqueIdentity;
    [HideInInspector]
    public bool hasDoll = false;

    public void LeaveDoll()
    {
        _isVisible = true;
    }

    public override void OnStartLocalPlayer()
    {
        GetNetIdentity();
        SetIdentity();
    }

    //----------------[Protected Area]-----------------

    protected Rigidbody Rigi { get { return _rigi; } }

    /// <summary>
    /// 初始化
    /// </summary>
    protected virtual void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");
        //playerUniqueIdentity = GetComponent<NetworkIdentity>().netId;
        //playerId = playerControllerId.ToString();
        _rigi = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        //_render = GetComponent<Renderer>();
        _particleRenderer = transform.Find("Renderer").gameObject;
        hasDoll = false;

        if(isLocalPlayer)
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

    private Rigidbody _rigi;
    private Collider _coll;
    //private Renderer _render;
    private Vector3 _animateTarget;
    private bool _animateEnd = false;
    private bool _isVisible { set {
            _rigi.isKinematic = !value;
            _coll.enabled = value;
            //_render.enabled = value;
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
        if (!isLocalPlayer)
        {
            transform.name = playerUniqueIdentity;
        }
        else
        {
            transform.name = MakeUniqueIdentity();
        }
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
        playerUniqueIdentity = name;
    }

    private void Enter()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Input.GetMouseButtonDown(1))
            if(Physics.Raycast(ray,out RaycastHit hit,10f,LayerMask.GetMask(new string[] { "Doll" })))
            {
                if(_enterCor == null && hit.transform.GetComponent<Doll>().CanEnter)
                    _enterCor = StartCoroutine(Entering(hit.transform));
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
        obj.GetComponent<Doll>().CmdEnterCheck(gameObject);
        CmdSetAuth(obj.gameObject);
        _enterCor = null;
    }

    public NetworkIdentity networkIdentity;

    [Command]
    private void CmdSetAuth(GameObject obj)
    {
        networkIdentity = obj.GetComponent<NetworkIdentity>();

        NetworkConnection otherOwner = networkIdentity.clientAuthorityOwner;
        print("Owner: " + connectionToClient.connectionId);
        if (otherOwner != null)
            networkIdentity.RemoveClientAuthority(otherOwner);
        networkIdentity.localPlayerAuthority = true;
        networkIdentity.AssignClientAuthority(connectionToClient);
    }
    

    public ActionDelegate PlayerAction;
    #endregion
}
