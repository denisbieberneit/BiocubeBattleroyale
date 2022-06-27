using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class GameHandler : NetworkBehaviour
{
    [SerializeField]
    private GameObject zonePref;

    private GameObject currentZone;

    public override void OnStartServer()
    {
        base.OnStartServer();
        startMatch();
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        stopMatch();
    }


    void startMatch()
    {
        currentZone = Instantiate(zonePref);
        ServerManager.Spawn(currentZone, null);
    }

    void stopMatch()
    {
        Destroy(currentZone);
    }
}
