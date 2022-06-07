using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Component.Animating;

public class AnimationController : MonoBehaviour
{
    private Animator anim;
    public bool isAttacking = false;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
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