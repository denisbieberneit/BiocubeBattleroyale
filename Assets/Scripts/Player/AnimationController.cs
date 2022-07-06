using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;
using FishNet.Object;

public class AnimationController : NetworkBehaviour
{
    private Animator anim;
    public bool isAttacking = false;

    private SpriteRenderer spriteRenderer;


    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!IsOwner)
        {
            return;
        }
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }
        string animationName = spriteRenderer.sprite.name;
        
        if (animationName == "GroundAttackLast" || animationName == "AirAttackLast")
        {
            SetIsAttacking(false);
        }
    }

    public void HorizontalMovement(bool isRunning)
    {
            anim.SetBool("isRunning", isRunning);
    }

    public void SetIsAttacking(bool Attacking)
    {
        anim.SetBool("isAttacking", Attacking);
        isAttacking = Attacking;
    }

    public void SetFullGround(bool fullGround)
    {
        if (!fullGround)
        {
            anim.SetBool("isGround", false);
            return;
        }
        anim.SetBool("isGround", true);
        anim.SetBool("isJumping", false);
        anim.SetBool("isFalling", false);
        anim.SetBool("isRunning",false);
    }

    public void SetJumping()
    {
        anim.SetBool("isGround", false);
        anim.SetBool("isFalling",false);
        anim.SetBool("isRunning", false);
        anim.SetBool("isJumping", true);
    }

    public void SetFalling()
    {
        anim.SetBool("isJumping", false);
        anim.SetBool("isFalling", true);
        anim.SetBool("isRunning", false);
        anim.SetBool("isGround", false);
    }
}