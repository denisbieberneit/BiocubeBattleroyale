using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EmoteSelectionItem : MonoBehaviour
{
    [SerializeField]
    private bool isUnlocked;

    [SerializeField]
    private int emoteId;

    [SerializeField]
    private Image img;

    [SerializeField]
    private Text nameText;

    [SerializeField]
    private string nameString;

    [SerializeField]
    private GameObject mainMenu;

    [SerializeField]
    private GameObject emoteMenu;

    private void Start()
    {
        isUnlocked = true;
        nameText.text = nameString;
    }


    public void OnSelectEmote()
    {
        if (isUnlocked)
        {
            PlayerPrefs.SetInt("emote", emoteId);
            Debug.Log("Selected Emote " + nameString);
            emoteMenu.SetActive(false);
            mainMenu.SetActive(true);
            //highlight card
        }
        else
        {
            //redirect to shop
        }
    }
}
