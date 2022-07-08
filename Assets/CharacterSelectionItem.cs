using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionItem : MonoBehaviour
{
    [SerializeField]
    private GameObject player;

    [SerializeField]
    private bool isUnlocked;

    [SerializeField]
    private string animationKey;

    [SerializeField]
    private Transform spawnLocation;

    private SpriteRenderer sr;

    [SerializeField]
    private Image img;

    private void Start()
    {
        isUnlocked = true;
        GameObject go = Instantiate(player, spawnLocation.position, Quaternion.identity);
        go.transform.parent = spawnLocation;
        sr = player.GetComponent<SpriteRenderer>();
        player.GetComponent<Animator>().SetBool(animationKey, true);
    }


    public void OnSelectCharacter()
    {
        if(isUnlocked){
            PlayerPrefs.SetString("Character", animationKey);
        }
        else
        {
            //redirect to shop
        }
    }

    private void FixedUpdate()
    {
        img.sprite = sr.sprite;
    }
}
