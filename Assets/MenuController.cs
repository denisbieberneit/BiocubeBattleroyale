using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public void OnStartGame()
    {
        //TODO replace with proper solution, e.g. scenemanager with enum of scenes
        SceneManager.LoadScene(1);
    }

}
