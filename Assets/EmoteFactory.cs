using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmoteFactory : MonoBehaviour
{
    [SerializeField]
    private List<GameObject> emotes;

    public static EmoteFactory instance;

    public GameObject GetEmoteById(int id)
    {
        foreach (GameObject emote in emotes)
        {
            if (emote.GetComponentInChildren<EmoteItem>().id == id)
            {
                return emote;
            }
        }
        Debug.LogError("Emote ID not found");
        return null;
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
}
