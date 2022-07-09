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

    private Animator animator;

    [SerializeField]
    private int scale;

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private string nameString;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject characterMenu;

    private void Start()
    {
        isUnlocked = true;
        GameObject go = Instantiate(player, spawnLocation.position, Quaternion.identity);
        go.transform.parent = spawnLocation;
        sr = go.GetComponent<SpriteRenderer>();
        animator = go.GetComponent<Animator>();
        animator.SetBool(animationKey, true);
        img.transform.localScale = new Vector3(scale, scale, scale);
        nameText.text = nameString;
    }


    public void OnSelectCharacter()
    {
        if(isUnlocked){
            PlayerPrefs.SetString("Character", animationKey);
            Debug.Log("Selected character " + nameString);
            characterMenu.SetActive(false);
            mainMenu.SetActive(true);
            //highlight card
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
