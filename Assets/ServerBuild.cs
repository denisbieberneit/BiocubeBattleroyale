using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
public class ServerBuild : NetworkBehaviour
{

    [SerializeField]
    private bool isEnabled;
    // Start is called before the first frame update
    private void Awake()
    {
        if (isEnabled) 
            InstanceFinder.NetworkManager.ServerManager.StartConnection();
    }
}
