using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class InventoryItem
{ 
    public InventoryItemData data { get; private set; }

    public InventoryItem(InventoryItemData source)
    {
        data = source;
    }

}
