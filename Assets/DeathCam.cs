using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class DeathCam : NetworkBehaviour
{

    public float interpVelocity;
    public float minDistance;
    public float followDistance;
    public GameObject target;
    public Vector3 offset;
    Vector3 targetPos;

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
