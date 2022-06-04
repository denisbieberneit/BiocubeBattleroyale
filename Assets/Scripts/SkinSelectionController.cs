using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SkinSelectionController : MonoBehaviour
{
    [SerializeField]
    private Animator anim;

    private string characterKey = "Character";
    private int selectedCharacter;

    [SerializeField]
    private SpriteRenderer sr;

    [SerializeField]
    private Image img;

    // Start is called before the first frame update
    void Start()
    {
        selectedCharacter = PlayerPrefs.GetInt(characterKey, 1);
        ShowSkinBasedOnSelection();
        //SelectHero();
    }

    private void OnEnable()
    {
        selectedCharacter = PlayerPrefs.GetInt(characterKey, 1);
        ShowSkinBasedOnSelection();
    }

    private void FixedUpdate()
    {
        img.sprite = sr.sprite;
    }

    public void NextHero(){
        selectedCharacter = selectedCharacter + 1;
        if (selectedCharacter == 4)
        {
            selectedCharacter = 1;
        }
        ShowSkinBasedOnSelection();
    }
    public void LastHero()
    {
        selectedCharacter = selectedCharacter - 1;
        if (selectedCharacter == 0)
        {
            selectedCharacter = 3;
        }
        ShowSkinBasedOnSelection();
    }

    private void ShowSkinBasedOnSelection(){
        anim.SetBool("isMage", false);
        anim.SetBool("isEvilMage", false);
        anim.SetBool("isHuntress", false);

      
        if (selectedCharacter == 1)
        {
            anim.SetBool("isMage", true);
            img.transform.localScale = new Vector3(4f, 4f, 0f);
        }
        if (selectedCharacter == 2)
        {
            anim.SetBool("isEvilMage", true);
            img.transform.localScale = new Vector3(6f, 6f, 0f);
        }
        if (selectedCharacter == 3)
        {
            
            anim.SetBool("isHuntress", true);
            img.transform.localScale = new Vector3(6f, 6f, 0f);
        }
    }

    public void SelectSkin(){
        PlayerPrefs.SetInt(characterKey, selectedCharacter);
    }
}
