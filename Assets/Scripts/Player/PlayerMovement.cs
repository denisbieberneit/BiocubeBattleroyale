using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;
using FishNet.Object.Prediction;


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


    #region Types.
    public struct MoveData
    {
        public float Horizontal;
        public float Vertical;
    }
    public struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public ReconcileData(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
    #endregion

    private void Awake()
    {
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
    }

    private void Start()
    {
        gameObjecTouchSkill = GameObject.Find("MobileSkill");
        gameObjecTouchAttack = GameObject.Find("MobileAttack");
        inventory = GetComponent<InventorySystem>();
        slopeCheck = GetComponent<SlopeCheck>();

        rb = GetComponent<Rigidbody2D>();
        ac = GetComponentInChildren<AnimationController>();
<<<<<<< HEAD

        Debug.Log("Started client: " + base.IsClient + " and started server " + base.IsServer);
=======
        touchController = GetComponent<TouchController>();
>>>>>>> parent of 3a2e3e5 (auslagerung movement)
    }


    [Replicate]
    private void updateMethod(MoveData md, bool asServer, bool replaying = false)
    {
<<<<<<< HEAD
        if (!base.IsOwner)
        {
            Debug.Log("NOT THE OWNER OF THIS ");
            return;
        }
        controller.Move(md.Horizontal * runSpeed * (float)base.TimeManager.TickDelta, runSpeed);
    }

    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }
=======
>>>>>>> parent of 3a2e3e5 (auslagerung movement)
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
            if (!isStunned)
            {
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

        horizontalMove = md.Horizontal;

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

<<<<<<< HEAD

        checkHit();

    }


    private void HandleFalling()
    {
        if (rb.velocity.y < 0 && !fullGround)
        {
            ac.SetFalling();
            isJumping = false;
        }
    }

    private void HandleAttack()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ac.SetIsAttacking(true);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            OnAbility();
        }
    }

    private void HandleJump()
    {

        if (!isJumping && fullGround)
=======
        if (!isJumping)
>>>>>>> parent of 3a2e3e5 (auslagerung movement)
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

        //fixedupdate
        if (leftGround)
        {
            ac.SetJumping();
            isJumping = true;
            leftGround = false;
        }

        if (holdingJump && forceStacks < maxForceStacks)
        {
            rb.AddForce(new Vector2(0f, 31f));
            forceStacks = forceStacks + 1;
        }

         controller.Move(horizontalMove * runSpeed * (float)base.TimeManager.TickDelta, false, jump);
         jump = false;
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

    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
    }

    private void TimeManager_OnTick()
    {
        if (base.IsOwner)
        {
            Reconciliation(default, false);
            CheckInput(out MoveData md);
            updateMethod(md, false);
        }
        if (base.IsServer)
        {
            updateMethod(default, true);
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation);
            Reconciliation(rd, true);
        }
    }

    private void CheckInput(out MoveData md)
    {
        md = default;

        float horizontal = Input.GetAxisRaw("Horizontal");
<<<<<<< HEAD
        horizontalMove = horizontal;

        md = new MoveData()
        {
            Horizontal = horizontal
=======
        float vertical = Input.GetAxisRaw("Vertical");

        if (horizontal == 0f && vertical == 0f)
            return;

        md = new MoveData()
        {
            Horizontal = horizontal,
            Vertical = vertical
>>>>>>> parent of 3a2e3e5 (auslagerung movement)
        };
    }
    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
        }
    }
}
