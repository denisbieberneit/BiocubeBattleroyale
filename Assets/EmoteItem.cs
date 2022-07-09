using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class EmoteItem : NetworkBehaviour
{
    [SerializeField]
    public int id;

    private void Start()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        Destroy(gameObject, 2f);
    }
}
