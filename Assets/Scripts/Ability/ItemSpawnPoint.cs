using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using FishNet.Object;
using FishNet;

public class ItemSpawnPoint : NetworkBehaviour
{
    public GameObject item;
    private SpriteRenderer sprite;
    private Animator anim;
    public string animName;

    private void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        anim.SetBool(animName,true);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player" && item != null && collider.gameObject.GetComponent<InventorySystem>().inventory == null) 
        {
            ItemObject copy = item.GetComponent<ItemObject>();
            copy.setOwner(collider.gameObject.GetComponent<NetworkObject>().LocalConnection);
            Debug.Log("ClientId of owner: "+  collider.gameObject.GetComponent<NetworkObject>().LocalConnection.ClientId);
            copy.referenceItem.inInventory = true;
            collider.gameObject.GetComponent<InventorySystem>().Add(copy);
            InstanceFinder.ServerManager.Despawn(gameObject);
        }
    }
}
