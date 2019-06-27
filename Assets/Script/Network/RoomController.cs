using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomController : MonoBehaviourPunCallbacks
{
    public static RoomController instance;

    public void OnStartBtClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(PhotonNetwork.CurrentRoom.CustomProperties["Type"].ToString());
        }
        else
        {
            //photonView.RPC("SetReady", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, true);
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["Ready"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
            _startBt.interactable = false;
        }
    }

    public void OnCancelBtClick()
    {
        //photonView.RPC("SetReady", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId, false);
        _cancelBt.interactable = false;
        PhotonNetwork.LeaveRoom();
    }

    public void SetCameraTarget(int point)
    {
        if (_cor != null)
            StopCoroutine(_cor);
        _cor = StartCoroutine(CameraMove(_point[point].position + new Vector3(0, 3, -10)));
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("UpdateSI", RpcTarget.All);
            photonView.RPC("PlayerCountUpdate", RpcTarget.All, _playerCount + 1);
            _startBt.GetComponentInChildren<Text>().text = "Start";
        }
        else
        {
            _startBt.GetComponentInChildren<Text>().text = "Ready";
            _startBt.interactable = true;
        }
    }

    public override void OnPlayerPropertiesUpdate(Player target, ExitGames.Client.Photon.Hashtable changedProps)
    {
        print(target.NickName + " is ready.");
        ReadyUpdate();

        if (PhotonNetwork.IsMasterClient)
        {
            bool allReady = true;
            foreach (Player temp in PhotonNetwork.PlayerList)
            {
                if (temp.CustomProperties.TryGetValue("Ready", out object result))
                {
                    allReady = (bool)result;
                }
                else
                {
                    allReady = false;
                }
                if (!allReady)
                    break;
            }
            if (PhotonNetwork.CurrentRoom.MaxPlayers == 1 || (allReady && _playerCount > 1))
            {
                _startBt.interactable = true;
            }
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveSlot(otherPlayer.UserId);
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("PlayerCountUpdate", RpcTarget.All, _playerCount - 1);
            if (_isReady)
            {
                //photonView.RPC("SetReady", RpcTarget.All);
                _isReady = false;
            }

            _startBt.GetComponentInChildren<Text>().text = "Start";
            if (_playerCount == _ready)
                _startBt.interactable = true;
            else
                _startBt.interactable = false;
        }
    }

    public override void OnLeftRoom()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
    }

    //public override void OnEnable()
    //{
    //    base.OnEnable();
    //    PhotonNetwork.AddCallbackTarget(this);
    //}

    //public override void OnDisable()
    //{
    //    base.OnDisable();
    //    PhotonNetwork.RemoveCallbackTarget(this);
    //}

    [SerializeField]
    private Button _startBt;
    [SerializeField]
    private Button _cancelBt;
    [SerializeField]
    private List<PlayerSlot> _slot;
    [SerializeField]
    private List<PlayerInstance> _instance;
    [SerializeField]
    private List<Transform> _point;
    [SerializeField]
    private Transform _slotScroll;
    [SerializeField]
    private GameObject _slotItemPrefab;
    [SerializeField]
    private GameObject _playerMesh;
    private Transform _camera;
    [SerializeField]
    private int _playerCount = 1;
    private int _ready = 1;
    private bool _isReady = false;
    private Coroutine _cor;

    [System.Serializable]
    private struct PlayerSlot
    {
        public string userId;
        public string nickName;
        public int craftType;

        public PlayerSlot(string userId, string nickName, int craftType)
        {
            this.userId = userId;
            this.nickName = nickName;
            this.craftType = craftType;
        }
    }

    [System.Serializable]
    private struct PlayerInstance
    {
        public string userId;
        public int point;
        public Transform slot;

        public PlayerInstance(string userId, int point, Transform slot)
        {
            this.userId = userId;
            this.point = point;
            this.slot = slot;
        }
    }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
        }

        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        _camera = Camera.main.transform;
        _slot = new List<PlayerSlot>();
        _instance = new List<PlayerInstance>();

        Player player = PhotonNetwork.LocalPlayer;
        AddSlot(player.UserId, player.NickName, 0);
        AddInstance(player.UserId);

        if (PhotonNetwork.IsMasterClient)
        {
            _isReady = true;
            _startBt.GetComponentInChildren<Text>().text = "Start";
            if(PhotonNetwork.CurrentRoom.MaxPlayers > 1)
                _startBt.interactable = false;
            var hashtable = new ExitGames.Client.Photon.Hashtable();
            hashtable["Ready"] = true;
            PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
        }
    }

    IEnumerator CameraMove(Vector3 pos)
    {
        while(Vector3.Distance(pos, _camera.position) > 0.01f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            _camera.position = Vector3.Lerp(_camera.position, pos, Time.deltaTime * 5f);
        }
    }

    private void AddSlot(string id,string nickName,int type)
    {
        _slot.Add(new PlayerSlot(id, nickName, type));
    }

    private void AddInstance(string id)
    {
        int point = 0;
        foreach (Transform temp in _point)
        {
            if (temp.childCount == 0)
            {
                Instantiate(_playerMesh, _point[point]);
                break;
            }
            point++;
        }
        Transform tempSlot = Instantiate(_slotItemPrefab, _slotScroll).transform;
        tempSlot.GetComponent<RoomSlotItem>().Init(_slot.Find(n => n.userId == id).nickName, point);
        _instance.Add(new PlayerInstance(id, point, tempSlot));
    }

    private void ReadyUpdate()
    {
        foreach (Player temp in PhotonNetwork.PlayerList)
        {
            if (temp.CustomProperties.TryGetValue("Ready", out object result))
            {
                if ((bool)result)
                {
                    _instance.Find(n => n.userId == temp.UserId).slot.GetComponent<RoomSlotItem>().Ready();
                }
            }
        }
    }

    [PunRPC]
    private void UpdateSI()
    {
        foreach(Player temp in PhotonNetwork.PlayerList)
        {
            int index = _slot.FindIndex(n => n.userId == temp.UserId);
            if(index == -1)
            {
                AddSlot(temp.UserId, temp.NickName, 0);
                AddInstance(temp.UserId);
            }
        }
        ReadyUpdate();
    }

    private void RemoveSlot(string userId)
    {
        int slotIndex = _slot.FindIndex(n => n.userId == userId);
        if(slotIndex != -1)
            _slot.RemoveAt(slotIndex);

        int instanceIndex = _instance.FindIndex(n => n.userId == userId);
        if (instanceIndex == -1)
            return;
        PlayerInstance tI = _instance[instanceIndex];
        Destroy(_point[tI.point].GetChild(0).gameObject);
        Destroy(tI.slot.gameObject);
        _instance.RemoveAt(instanceIndex);
    }

    [PunRPC]
    void PlayerCountUpdate(int c)
    {
        _playerCount = c;
    }

    //[PunRPC]
    //void SetReady(string id,bool b)
    //{
    //    if (b)
    //    {
    //        _ready++;
    //        if (PhotonNetwork.IsMasterClient)
    //        {
    //            if(_ready == _playerCount)
    //                _startBt.interactable = true;
    //        }
    //        else
    //        {
    //            _instance.Find(n => n.userId == id).slot.GetComponent<RoomSlotItem>().Ready();
    //        }
    //    }
    //    else
    //    {
    //        if(_isReady)
    //            _ready--;
    //    }
    //}
}
