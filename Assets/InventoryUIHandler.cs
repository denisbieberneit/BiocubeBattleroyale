using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class InventoryUIHandler : NetworkBehaviour
{
    private Animator anim;

    private InventoryItemData current;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateView(ItemObject data)
    {
        current = data.referenceItem;
        anim.SetBool(data.referenceItem.animationName, true);
    }

    public void Remove()
    {
        anim.SetBool(current.animationName, false);
        current = null;
    }
}
