using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(GameDiscovery))]
public class NetConnectController : NetworkManager
{
    public static NetConnectController instance;

    GameObject loadingPrefab;
    Loading loading;

    public override void OnStopServer()
    {
        discovery.StopBroadcast();
    }

    private GameDiscovery discovery;
    
    private bool start = false; 

    private void Start()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;

        loadingPrefab = Resources.Load<GameObject>("Prefab/UI/Loading");
        loading = Instantiate(loadingPrefab, GameObject.Find("Canvas").transform).GetComponent<Loading>();

        discovery = GetComponent<GameDiscovery>();
        StartCoroutine(StartAsClient());
        //startpo

        //if (!PhotonNetwork.IsConnected)
        //{
        //    UnityEngine.SceneManagement.SceneManager.LoadScene("SignIn");
        //    return;
        //    //PhotonNetwork.OfflineMode = true;
        //}
        //player = PhotonNetwork.Instantiate(Path.Combine("Prefab", "Player", "Player"), Vector3.zero + Vector3.up * 4, Quaternion.identity);
        //CameraFollow.instance.CameraFollowObj = player;
    }

    private IEnumerator StartAsClient()
    {
        discovery.Initialize();
        discovery.StartAsClient();
        
        yield return new WaitForSeconds(0.5f);
        StartAsServer();
        StartCoroutine(Check());
    }

    private void StartAsServer()
    {
        if (!IsClientConnected())
        {
            discovery.StopBroadcast();
            StartHost();
            discovery.StartAsServer();
        }
    }

    private IEnumerator Check()
    {
        while (!start)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            if (discovery.isServer)
            {
                if (numPlayers > 1)
                {
                    discovery.StopBroadcast();
                    loading.run = false;
                    start = true;
                }
            }

            else if (IsClientConnected())
            {
                loading.run = false;
                start = true;
            }
        }
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        
    }
}
