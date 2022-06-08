using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    private GameObject characterSelection;
    [SerializeField]
    private GameObject menu;


    public void OpenCharacterSelection()
    {
        menu.SetActive(false);
        characterSelection.SetActive(true);
    }

    public void OpenMenu()
    {
        characterSelection.SetActive(false);
        menu.SetActive(true);
    }

}
