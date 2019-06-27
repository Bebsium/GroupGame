using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class GameNetController : MonoBehaviour
{
    public static GameNetController instance;
    [HideInInspector]
    public GameObject player;
    
    private void Start()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        if (!PhotonNetwork.IsConnected)
        {
            //UnityEngine.SceneManagement.SceneManager.LoadScene("NetLobby");
            PhotonNetwork.OfflineMode = true;
        }
        player = PhotonNetwork.Instantiate(Path.Combine("Prefab", "Player", "Player"), Vector3.zero + Vector3.up * 4, Quaternion.identity);
        CameraFollow.instance.CameraFollowObj = player;
    }
}
