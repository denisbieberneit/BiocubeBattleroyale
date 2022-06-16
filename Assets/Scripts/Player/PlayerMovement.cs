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

    public float lastMovement;

    public PhysicsMaterial2D noFriction;
    public PhysicsMaterial2D friction;
    private SlopeCheck slopeCheck;

    public Rigidbody2D rb;

    public AnimationController ac;

    public bool fullGround = false;

    public bool isStunned = false;

    public bool isGased = false;

    public bool isJumping = false;
    private bool leftGround = false;

    [SerializeField]
    private float jumpForce;

    private InventorySystem inventory;

    [SerializeField]
    private PlayerAttackController attackController;


    #region Types.
    public struct MoveData
    {
        public float Horizontal;
        public bool Jump;
    }
    public struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public Vector3 AngularVelocity;
        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, Vector3 angularVelocity)
        {
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
            AngularVelocity = angularVelocity;
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
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;

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

        // TODO HANDLE PROPER, CHECK Y VALUE OF HEIGHT AND ADJUST BASED ON SERVER
        //moving
        lastMovement = md.Horizontal;
        controller.Move(md.Horizontal * runSpeed * (float) base.TimeManager.TickDelta, md.Jump, jumpForce);
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


        if (leftGround)
        {
            ac.SetJumping();
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


    public void OnJumpDown()
    {
        isJumping = true;
    }

    public void OnAbility()
    {
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
        rb.velocity = rd.Velocity;
        rb.angularVelocity = 1f;
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
        }
    }
    private void TimeManager_OnPostTick()
    {
        if (base.IsServer)
        {
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, rb.velocity, Vector3.one);
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
            Jump = isJumping
        };
    }
    private void FixedUpdate()
    {
        HandleJump();

        if (slopeCheck.onGround || slopeCheck.onSlope)
        {
            fullGround = true;
            if (leftGround)
            {
                leftGround = false;
            }
            ac.SetFullGround(fullGround);

        }
        else
        {
            fullGround = false;
            ac.SetFullGround(fullGround);
            leftGround = true; //weil player beim slopes runter laufen direkt auf fall �bergeht, dachte ich diese variable k�nnte das blockieren
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

        HandleFalling();
        HandleAttack();

        if (Mathf.Abs(horizontalMove) != 0f)
        {
            ac.HorizontalMovement(true);

        }
        checkHit();
    }
    private void OnDestroy()
    {
        if (InstanceFinder.TimeManager != null)
        {
            InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            InstanceFinder.TimeManager.OnPostTick -= TimeManager_OnPostTick;

        }
    }
}
