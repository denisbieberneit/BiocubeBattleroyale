using FishNet.Connection;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackController : NetworkBehaviour
{
    private Collider2D col;

    [SerializeField]
    private PlayerMovement playerMovement;
    
    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public void OnStartHit()
    {
        col.enabled = true;
    }

    public void OnEndHit()
    {
        col.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && col.enabled == true)
        {
            NetworkConnection owner = playerMovement.GetComponent<NetworkObject>().Owner;
            NetworkConnection targetOwner = collision.gameObject.GetComponent<NetworkObject>().Owner;
            Debug.Log(owner.ClientId + ":" + targetOwner.ClientId);
            if (owner.ClientId != targetOwner.ClientId)
            {
                playerMovement.ServerHitback(collision.gameObject, playerMovement.GetComponent<PlayerMovement>().lastMovement);
                playerMovement.ac.isAttacking = false;
            }
        }
        
    }
}