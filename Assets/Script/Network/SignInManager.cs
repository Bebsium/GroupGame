using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class SignInManager : MonoBehaviourPunCallbacks
{
    public static SignInManager instance;

    public void StartGame()
    {
        PlayerPrefs.SetString("NickName", _nickName.text);
        PhotonNetwork.LocalPlayer.NickName = PlayerPrefs.GetString("NickName");
        PhotonNetwork.JoinLobby();
    }

    public void CreateNewRoom()
    {
        Show("NewRoom");
    }

    public void OnCreateNewRoomBtClick()
    {
        string name = _newRoom.GetComponentInChildren<InputField>().text;
        Dropdown temp = _newRoom.GetComponentInChildren<Dropdown>();
        string type = temp.options[temp.value].text;
        int max = (int)_newRoom.GetComponentInChildren<Slider>().value;
        bool visible = _newRoom.GetComponentInChildren<Toggle>().isOn;
        CreateRoom(name, type, max, visible);
    }

    public void JoinRoom(string name)
    {
        if (name == "")
            Message("Please input the room name.");
        else
        {
            PhotonNetwork.JoinRoom(name);
        }
    }

    public void JoinRoom()
    {
        string name = _lobby.GetComponentInChildren<InputField>().text;
        Message("Name: " + name);
        JoinRoom(name);
    }

    public void Back(string s)
    {
        switch (s)
        {
            case "Lobby":
                Show("Lobby");
                break;
            case "Start":
                PhotonNetwork.LeaveLobby();
                Show("Login");
                break;
        }
    }

    public void SliderChange(float n)
    {
        _newRoom.transform.Find("MaxPlayer").GetComponent<Text>().text = ((int)n).ToString();
    }

    public override void OnJoinedLobby()
    {
        Show("Lobby");
    }

    public override void OnJoinedRoom()
    {
        //PhotonNetwork.LoadLevel("Default");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Room");
    }

    public override void OnConnectedToMaster()
    {
        print("On Connect to Master, Lobby "+ PhotonNetwork.InLobby);
    }

    public override void OnConnected()
    {
        print("On Connected, Lobby " + PhotonNetwork.InLobby);
        if (PhotonNetwork.InLobby)
        {
            Show("Lobby");
        }
        else
        {
            Show("Login");
            _startBt.SetActive(true);
            _roomList = new List<RoomInfo>();
        }
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Message("Has not this room.");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        int tempIndex;
        foreach(RoomInfo room in roomList)
        {
            if (_roomList != null)
                tempIndex = _roomList.FindIndex(n => n.Name == room.Name);
            else
                tempIndex = -1;

            if(tempIndex != -1)
            {
                _roomList.RemoveAt(tempIndex);
                Destroy(_roomListScroll.GetChild(tempIndex).gameObject);
            }
            if(room.PlayerCount > 0)
            {
                if (room.IsVisible)
                {
                    _roomList.Add(room);
                    GameObject temp = Instantiate(_roomItemPrefab, _roomListScroll);
                    temp.GetComponent<RoomItem>().Init(room.Name, room.CustomProperties["Type"].ToString(), room.PlayerCount, room.MaxPlayers, room.IsOpen);
                    //temp.GetComponent<RoomItem>().Init(room.Name, "Game Type", room.PlayerCount, room.MaxPlayers);
                }
            }
        }
    }

    [SerializeField]
    private GameObject _login;
    [SerializeField]
    private GameObject _lobby;
    [SerializeField]
    private GameObject _newRoom;
    [SerializeField]
    private GameObject _message;
    [SerializeField]
    private GameObject _startBt;
    [SerializeField]
    private InputField _nickName;
    [SerializeField]
    private Transform _roomListScroll;
    [SerializeField]
    private GameObject _roomItemPrefab;

    private Coroutine _messageCor;
    [SerializeField]
    private List<RoomInfo> _roomList = new List<RoomInfo>();

    private void CreateRoom(string name,string type,int maxPlayer,bool visible)
    {
        //Message("Name: " + name + "/n Type: " + type + "/n Max: " + maxPlayer +
        //    "/n Visible" + visible);
        //return;
        RoomOptions ros = new RoomOptions
        {
            IsOpen = true,
            IsVisible = visible,
            MaxPlayers = (byte)maxPlayer,
        };
        ros.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "Type", type }
        };
        ros.CustomRoomPropertiesForLobby = new string[] { "Type"};
        //PhotonNetwork.CreateRoom(name, ros, TypedLobby.Default);
        PhotonNetwork.CreateRoom(name, ros);
    }

    private void Start()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        Init();

        print("Start +" + PhotonNetwork.InLobby);

        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.ConnectUsingSettings();
        }

        
        //_startBt.SetActive(false);
        
        //PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToMaster("172.0.0.1", 5055,
        //    PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime);

        //PhotonNetwork.PhotonServerSettings.AppSettings.UseNameServer = false;
        //PhotonNetwork.PhotonServerSettings.AppSettings.Server = "127.0.0.1";
        //PhotonNetwork.PhotonServerSettings.AppSettings.Port = 5055;
        //PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = null;

    }

    private void Init()
    {
        _login.SetActive(false);
        _lobby.SetActive(false);
        _newRoom.SetActive(false);
    }

    private void Show(string s)
    {
        Init();
        switch (s)
        {
            case "Login":
                _login.SetActive(true);
                if (!PlayerPrefs.HasKey("NickName"))
                {
                    PlayerPrefs.SetString("NickName", "Player" + Random.Range(1, 100));
                }
                _nickName.text = PlayerPrefs.GetString("NickName");
                break;
            case "Lobby":
                _lobby.SetActive(true);
                break;
            case "NewRoom":
                _newRoom.SetActive(true);
                NewRoomInit();
                break;
        }
    }

    private void NewRoomInit()
    {
        string name = "Room " + Random.Range(0, 100);
        _newRoom.GetComponentInChildren<InputField>().text = name;
    }

    private void Message(string m)
    {
        if(_messageCor != null)
            StopCoroutine(_messageCor);

        _message.GetComponentInChildren<Text>().text = m;
        _messageCor = StartCoroutine(ShowMessage());
    }

    private IEnumerator ShowMessage()
    {
        _message.SetActive(true);
        yield return new WaitForSeconds(2f);
        _message.SetActive(false);
    }
}
