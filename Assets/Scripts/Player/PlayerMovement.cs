using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;
using FishNet.Object.Prediction;


public class PlayerMovement : NetworkBehaviour
{
    public CharacterController2D controller;
    public float runSpeed = 40f;
    public float horizontalMove = 0f;
    public bool holdingJump = false;

    public float lastMovement;

    [SerializeField]
    private float maxForceStacks = 30f;
    public float forceStacks = 0;

    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D friction;
    private SlopeCheck slopeCheck;

    public Rigidbody2D rb;

    public AnimationController ac;

    public bool fullGround = false;

    public bool forceStackSetZero = false;

    public bool isStunned = false;

    public bool isGased = false;

    private bool isJumping = false;
    private bool leftGround = false;

    private InventorySystem inventory;

    [SerializeField]
    private PlayerAttackController attackController;


    #region Types.
    public struct MoveData
    {
        public float Horizontal;
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        controller.enabled = (base.IsServer || base.IsOwner);
    }

    private void Awake()
    {
        controller = GetComponent<CharacterController2D>();
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
    }

    private void Start()
    {

        inventory = GetComponent<InventorySystem>();
        slopeCheck = GetComponent<SlopeCheck>();

        rb = GetComponent<Rigidbody2D>();
        ac = GetComponentInChildren<AnimationController>();
    }


    [Replicate]
    private void updateMethod(MoveData md, bool asServer, bool replaying = false)
    {
        controller.Move(md.Horizontal * runSpeed * (float)base.TimeManager.TickDelta);
    }

    private void Update()
    {
        if (!base.IsOwner)
        {
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

        HandleJump();

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


        HandleFalling();


        HandleAttack();


        if (Mathf.Abs(horizontalMove) > 0f)
        {
            lastMovement = horizontalMove;
            ac.HorizontalMovement(true);

        }
        else
        {
            ac.HorizontalMovement(false);
        }


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

        if (forceStackSetZero && !holdingJump)
        {
            forceStacks = 0;
        }

        if (leftGround)
        {
            ac.SetJumping();
            isJumping = true;
            leftGround = false;
        }

        if (forceStacks > maxForceStacks)
        {
            forceStacks = maxForceStacks;
        }
        if (holdingJump && forceStacks < maxForceStacks)
        {
            rb.AddForce(new Vector2(0f, forceStacks));
            forceStacks = forceStacks + 1;
        }
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
        holdingJump = false;
        forceStackSetZero = true;
    }

    public void OnJumpDown()
    {
        isJumping = true;
        holdingJump = true;
        forceStacks = 10;
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
        horizontalMove = horizontal;
        md = new MoveData()
        {
            Horizontal = horizontal,
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
