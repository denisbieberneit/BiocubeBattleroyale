using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSelection;
    [SerializeField]
    private GameObject menu;
    [SerializeField]
    private GameObject emotes;


    public void OpenCharacterSelection()
    {
        OpenPage("chars");
    }

    public void OpenMenu()
    {
        OpenPage("menu");
    }

    public void OpenEmoteSelection()
    {
        OpenPage("emotes");
    }

    private void OpenPage(string page)
    {
        menu.SetActive(false);
        characterSelection.SetActive(false);
        emotes.SetActive(false);

        if (page == "chars")
        {
            characterSelection.SetActive(true);
        }
        if (page == "emotes")
        {
            emotes.SetActive(true);
        }
        if (page == "menu")
        {
            menu.SetActive(true);
        }
    }
}
