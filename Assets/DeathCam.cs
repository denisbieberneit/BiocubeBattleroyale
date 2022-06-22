using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class DeathCam : NetworkBehaviour
{
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsServer)
        {
            gameObject.SetActive(false);
        }
        if (base.IsOwner)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}
