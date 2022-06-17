using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAnimBehaviour : StateMachineBehaviour
{
    [SerializeField]
    private PlayerAttackController attack;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
       
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
