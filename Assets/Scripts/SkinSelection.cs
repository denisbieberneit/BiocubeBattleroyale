using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SkinSelection : NetworkBehaviour
{
    [SerializeField]
    private Animator anim;

    // Start is called before the first frame update
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!base.IsOwner)
        {
            return;
        }
        int selectedPlayer = PlayerPrefs.GetInt("Character", 0);
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

}
