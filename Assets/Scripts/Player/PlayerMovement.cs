using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;

public class PlayerMovement : NetworkBehaviour
{
    public bool isPlayer;
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public float horizontalMove = 0f;
    public bool jump = false;
    public bool holdingJump = false;

    public float lastMovement;

    private float maxForceStacks = 30f;
    public float forceStacks = 0;

    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D friction;
    private SlopeCheck slopeCheck;

    public Rigidbody2D rb;

    public AnimationController ac;

    public bool fullGround = false;

    public TouchController touchController;

    private float width;
    private float height;

    public bool forceStackSetZero = false;

    public bool isStunned = false;

    public bool isGased = false;

    private bool isJumping = false;
    private bool leftGround = false;

    public GameObject gameObjecTouchSkill;
    public GameObject gameObjecTouchAttack;

    private InventorySystem inventory;

    [SerializeField]
    private PlayerAttackController attackController;

    private void Start()
    {
        gameObjecTouchSkill = GameObject.Find("MobileSkill");
        gameObjecTouchAttack = GameObject.Find("MobileAttack");
        inventory = GetComponent<InventorySystem>();
        slopeCheck = GetComponent<SlopeCheck>();

        rb = GetComponent<Rigidbody2D>();
        ac = GetComponentInChildren<AnimationController>();
        touchController = GetComponent<TouchController>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!base.IsOwner)
        {
            //rb.gravityScale = 0; //TODO: ruckelt mit gravity
            return;
        }

        if (slopeCheck.onGround || slopeCheck.onSlope)
        {
            fullGround = true;
            ac.SetFullGround(fullGround);
            isJumping = false;
            if (leftGround == true)
            {
                leftGround = false;
            }
        }
        else
        {
            fullGround = false;
            ac.SetFullGround(fullGround);
            leftGround = true; //weil player beim slopes runter laufen direkt auf fall übergeht, dachte ich diese variable könnte das blockieren
        }


        if (horizontalMove == 0f)
        {
            if (slopeCheck.onSlope)
            {
                Invoke("SetFriction", .2f);
            }
        }
        else
        {
            if (!isStunned) {
                rb.sharedMaterial = noFriction;
            }
          
        }

        //if (!isJumping) { ac.SetFullGround(fullGround);}


        if (rb.velocity.y < 0 && !fullGround)
        {
            ac.SetFalling();
            isJumping = false;
            forceStacks = maxForceStacks;
        }


        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ac.SetIsAttacking(true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnAbility();
        }

        horizontalMove = Input.GetAxisRaw("Horizontal");

        if (touchController.moveLeft || touchController.moveRight)
        {
            ac.HorizontalMovement(true);
        }

        if (Mathf.Abs(horizontalMove) > 0f)
        {
            lastMovement = horizontalMove;
            ac.HorizontalMovement(true);

        }
        else
        {
            ac.HorizontalMovement(false);
        }

        if (!isJumping)
        {
            if (Input.GetButtonDown("Jump") && ac.isAttacking == false)
            {
                OnJumpDown();
            }
        }
        if (Input.GetButtonUp("Jump"))
        {
            OnJumpUp();
        }

        checkHit();
    }

    private void checkHit()
    {
        if (ac.isAttacking)
        {
            attackController.OnStartHit();
        }
        else
        {
            attackController.OnEndHit();
        }
    }

    void FixedUpdate()
    {
        if (leftGround)
        {
            ac.SetJumping();
            isJumping = true;
            leftGround = false;
        }

        if (!base.IsOwner)
        {
            return;
        }

        if (holdingJump && forceStacks < maxForceStacks)
        {
            rb.AddForce(new Vector2(0f, 31f));
            forceStacks = forceStacks + 1;
        }
        if (isPlayer)
        {
            controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, false, jump);
            jump = false;
        }
    }

    public void OnJumpUp()
    {
        if (!base.IsOwner)
        {
            return;
        }
        holdingJump = false;
        forceStacks = maxForceStacks;
        forceStackSetZero = true;
    }

    public void OnJumpDown()
    {
        if (!base.IsOwner)
        {
            return;
        }
        jump = true;
        holdingJump = true;
        forceStacks = 0;
        forceStackSetZero = false;
    }

    public void OnAbility()
    {
        if (!base.IsOwner)
        {
            return;
        }
        if (inventory.inventory != null)
        {
            ac.SetIsAttacking(true);
            //inventory attack,
            Vector3 v = new Vector3(transform.position.x + (.6f * lastMovement), transform.position.y, transform.position.z);
            GameObject ability = inventory.inventory.referenceItem.prefab;
            GameObject obj = Instantiate(ability, v, Quaternion.identity);
            InstanceFinder.ServerManager.Spawn(obj, inventory.inventory.getOwner());

            inventory.Remove();
        }
    }


    void SetFriction()
    {
        rb.sharedMaterial = friction;
    }

    public void SetStun()
    {
        //mach stun
        SetFriction();
        Invoke("RemoveStun", .2f);

    }

    public void RemoveStun()
    {
        rb.sharedMaterial = noFriction;
    }


    [ServerRpc]
    public void ServerHitback(GameObject target, float direction)
    {
        ObserversHitback(target, direction);
    }

    [ObserversRpc]
    private void ObserversHitback(GameObject target, float direction)
    {
        target.GetComponent<Rigidbody2D>().AddForce(new Vector2(600f * direction, 600f));
        target.GetComponent<Player>().TakeDamage(1f);
    }
}
