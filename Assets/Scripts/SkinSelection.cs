using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinSelection : MonoBehaviour
{
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        int selectedPlayer = 0;//PlayerPrefs.GetInt("Character"); //TODO: add back again
        if (selectedPlayer == 1)
        {
            anim.SetBool("isMage", true);
        }
        if (selectedPlayer == 2)
        {
            anim.SetBool("isEvilMage", true);
        }
        if (selectedPlayer == 3)
        {
            anim.SetBool("isHuntress", true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
