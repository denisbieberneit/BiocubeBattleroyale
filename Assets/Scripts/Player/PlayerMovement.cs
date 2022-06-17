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

    private bool leftGround = false;

    [SerializeField]
    private float jumpForce;

    private InventorySystem inventory;

    [SerializeField]
    private PlayerAttackController attackController;

    private bool _jump;
    private bool canJump;
    private int jumps = 1;
    [SerializeField]
    private int maxJumps;

    private float fallDistance;
    private bool setFallDistance;

    private bool holdingJump;


    #region Types.
    public struct MoveData
    {
        public bool Jump;
        public bool CanJump;
        public float Horizontal;
        public float Vertical;
        public MoveData(bool jump,bool canJump, float horizontal, float vertical)
        {
            Jump = jump;
            CanJump = canJump;
            Horizontal = horizontal;
            Vertical = vertical;
        }
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
        rb.isKinematic = (!base.IsOwner || base.IsServerOnly);

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
        // synch move
        controller.Move(md.Horizontal * runSpeed * (float) base.TimeManager.TickDelta);

        // synch jump
        if (md.Jump && md.CanJump)
        {
            rb.velocity = new Vector2(0f, 0f);
            rb.AddForce(new Vector2(0f, jumpForce));
        }

    }


    private void HandleFalling()
    {
        if (rb.velocity.y < 0 && !fullGround && !holdingJump)
        {
            if (!setFallDistance)
            {
                fallDistance = transform.position.y;
                setFallDistance = true;
            }
            ac.SetFalling();
        }
       
        if (((Mathf.Abs(fallDistance) - Mathf.Abs(transform.position.y))) > 2 && setFallDistance)
        {
            canJump = false;
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
        if (jumps == maxJumps)
        {
            canJump = false;
            holdingJump = false;
        }
        else
        {
            canJump = true;
        }
        if (Input.GetKeyDown(KeyCode.Space) & canJump)
        {
            _jump = true;
            leftGround = true;
            jumps = jumps + 1;
            holdingJump = true;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            holdingJump = false;
        }

        if (leftGround)
        {
            ac.SetJumping();
        }
        if (holdingJump && !fullGround && rb.velocity.y <= 0 & canJump)
        {
            _jump = true;
            jumps = jumps + 1;
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
        float vertical = Input.GetAxisRaw("Vertical");
        lastMovement = horizontal;
        horizontalMove = horizontal;
        if (horizontal == 0f && vertical == 0f && !_jump)
            return;

        md = new MoveData(_jump, canJump, horizontal, vertical);
        _jump = false;

    }
    private void Update()
    {
        if (!base.IsOwner)
        {
            return;
        }
        HandleJump();
        HandleSlope();
        HandleMove();
        HandleFalling();
        HandleAttack();
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

    private void HandleMove()
    {
        if (horizontalMove != 0f)
        {
            ac.HorizontalMovement(true);

        }
    }

    private void HandleSlope()
    {
        if (slopeCheck.onGround || slopeCheck.onSlope)
        {
            setFallDistance = false;
            jumps = 1;
            fullGround = true;
            leftGround = false;
            ac.SetFullGround(fullGround);
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
    }
}
