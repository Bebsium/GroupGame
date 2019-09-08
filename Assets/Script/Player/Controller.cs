using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Global;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
[RequireComponent(typeof(Rigidbody))]
public abstract class Controller : MonoBehaviourPun,IPunObservable
{
    //----------------[Public Area]--------------------
    public string playerName;
    [HideInInspector]
    public bool hasDoll = false;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_isVisible);
        }
        else
        {
            this._isVisible = (bool)stream.ReceiveNext();
        }
    }

    public void LeaveDoll()
    {
        _isVisible = true;
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
        _pv = GetComponent<PhotonView>();
        _particleRenderer = transform.Find("Renderer").gameObject;
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
    private bool _isVisible { set {
            _rigi.isKinematic = !value;
            _coll.enabled = value;
            _render.enabled = value;
            _particleRenderer.SetActive(value);
        } get { return _render.enabled; } }
    private bool _entering = false;
    private Coroutine _enterCor = null;
    private GameObject _particleRenderer;

    private void Update()
    {
        if (!_pv.IsMine)
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
        obj.GetComponent<Doll>().EnterCheck(this);
        _enterCor = null;
    }

    public ActionDelegate PlayerAction;
    #endregion
}
