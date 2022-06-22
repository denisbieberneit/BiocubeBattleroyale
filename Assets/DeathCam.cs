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


    private void Start()
    {
        target = GetComponentInParent<Player>().gameObject;
    }
    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }
        if (target)
        {
            Vector3 posNoZ = transform.position;
            posNoZ.z = target.transform.position.z;

            Vector3 targetDirection = (target.transform.position - posNoZ);

            interpVelocity = targetDirection.magnitude * 5f;

            targetPos = transform.position + (targetDirection.normalized * interpVelocity * Time.deltaTime);

            transform.position = Vector3.Lerp(transform.position, targetPos + offset, 0.25f);

        }
    }
}
