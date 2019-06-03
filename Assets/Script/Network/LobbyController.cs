﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class LobbyController : MonoBehaviourPunCallbacks
{
    public override void OnConnectedToMaster()
    {
        startBt.SetActive(true);
        nickNameInput.SetActive(true);
    }

    void CreateRoom()
    {
        RoomOptions ros = new RoomOptions();
        ros.IsOpen = true;
        ros.IsVisible = true;
        ros.MaxPlayers = 4;
        PhotonNetwork.CreateRoom(Random.Range(1, 1000).ToString(), ros);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        CreateRoom();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Default");
    }

    private GameObject startBt;
    private GameObject nickNameInput;

    private void JoinLobby()
    {
        //PhotonNetwork.JoinLobby();
        PhotonNetwork.JoinRandomRoom();
    }

    private void Start()
    {
        startBt = GameObject.Find("Start");
        startBt.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(JoinLobby);
        startBt.SetActive(false);

        nickNameInput = GameObject.Find("NickName");
        nickNameInput.SetActive(false);
        string tempName = "Player" + Random.Range(1, 100);
        nickNameInput.GetComponent<UnityEngine.UI.InputField>().text = tempName;

        PhotonNetwork.NickName = tempName;
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();
    }
}
