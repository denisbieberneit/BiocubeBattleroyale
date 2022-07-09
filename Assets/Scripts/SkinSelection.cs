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
        anim.SetBool(PlayerPrefs.GetString("Character", "isEvilWarrior"), true);
    }

}
