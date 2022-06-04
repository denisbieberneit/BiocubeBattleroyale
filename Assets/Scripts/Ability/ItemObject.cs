using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet.Connection;

public class ItemObject : NetworkBehaviour
{
    public InventoryItemData referenceItem;

    private NetworkConnection owner;

    public NetworkConnection getOwner()
    {
        return owner;
    }

    public void setOwner(NetworkConnection owner)
    {
        this.owner = owner;
    }
}
