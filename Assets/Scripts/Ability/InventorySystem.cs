using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{    
    public ItemObject inventory;

    [SerializeField]
    private InventoryUIHandler uiHandler;


    private void Awake()
    {
        inventory = null;
    }

    public void Add(ItemObject referenceData)
    {
        inventory = referenceData;
        uiHandler.UpdateView(referenceData);
    }
    public void Remove()
    {
        inventory = null;
        uiHandler.Remove();
    }
}