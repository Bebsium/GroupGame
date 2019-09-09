using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameDiscovery : NetworkDiscovery
{
    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        NetworkManager manager = NetworkManager.singleton;

        if (manager.isNetworkActive)
            return;

        manager.networkAddress = fromAddress;
        manager.StartClient();
        StartCoroutine(StopBroadcastCoroutine());
    }

    private IEnumerator StopBroadcastCoroutine()
    {
        yield return new WaitForEndOfFrame();
        StopBroadcast();
    }
}
