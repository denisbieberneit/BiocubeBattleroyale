using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Connection;
using FishNet.Object.Prediction;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



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

    public bool isSmoked = false;

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

    private bool hit;
    private float hitDirection;
    [SerializeField]
    private bool canAttack;
    [SerializeField]
    private int hitKnockBackStrength;


    [SerializeField]
    private GameObject gasPiece;
    private float nextActionTime = 0.0f;
    public float period = 0.1f;


    #region Types.
    public struct MoveData
    {
        public bool Jump;
        public bool CanJump;
        public bool Hit;
        public float HitDirection;
        public bool Stunned;
        public float Horizontal;
        public float Vertical;
        public MoveData(bool jump,bool canJump, bool hit, float hitDirection, bool stunned, float horizontal, float vertical)
        {
            Jump = jump;
            CanJump = canJump;
            Hit = hit;
            HitDirection = hitDirection;
            Stunned = stunned;
            Horizontal = horizontal;
            Vertical = vertical;
        }
    }
    public struct ReconcileData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;
        public float AngularVelocity;
        public ReconcileData(Vector3 position, Quaternion rotation, Vector3 velocity, float angularVelocity)
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
        rb.isKinematic = (!base.IsOwner || base.IsServerOnly);

        if (!IsOwner)
        {
            return;
        }
        inventory = GetComponent<InventorySystem>();
        slopeCheck = GetComponent<SlopeCheck>();
        controller = GetComponent<CharacterController2D>();

        ac = GetComponentInChildren<AnimationController>();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }


    [Replicate]
    private void updateMethod(MoveData md, bool asServer, bool replaying = false)
    {
       
        if (md.Jump && md.CanJump)
        {
            rb.velocity = new Vector2(0f, 0f);
            rb.AddForce(new Vector2(0f, jumpForce));
        }
        //synch hit
        if (md.Hit)
        {
            rb.AddForce(new Vector2(md.HitDirection * hitKnockBackStrength, 350f));
        }
        else
        {
            controller.Move(md.Horizontal * runSpeed * (float) base.TimeManager.TickDelta, rb.velocity.y);
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
        if (canAttack)
        {
            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                ac.SetIsAttacking(true);
                canAttack = false;
                Invoke("SetCanAttack", 1f);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                OnAbility();
                canAttack = false;
                Invoke("SetCanAttack", 1f);
            }
        }

    }

    private void HandleJump()
    {
        if (jumps >= maxJumps)
        {
            canJump = false;
            holdingJump = false;
        }
        else
        {
            canJump = true;
        }
        if (Input.GetKeyDown(KeyCode.Space))
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
        if (holdingJump && !fullGround && rb.velocity.y <= 0 && canJump)
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
            Vector3 v = new Vector3(transform.position.x +( 0.6f * lastMovement), transform.position.y, transform.position.z);
            float move = lastMovement;
            SpawnItemAttack(inventory.inventory.referenceItem.prefab, base.Owner, v, move);
            inventory.Remove();
        }
    }

    [ServerRpc]
    private void SpawnItemAttack(GameObject _item, NetworkConnection owner, Vector3 v, float move)
    {
        GameplayManager.instance.SpawnAbility(owner, _item, move, gameObject.scene, v);
    }

    [ServerRpc]
    private void SpawnEmote()
    {
        GameplayManager.instance.SpawnEmote(GetComponent<Player>().emote, GetComponent<Player>().playerEmotePosition, gameObject.scene, base.Owner);
    }


    [ServerRpc]
    private void SpawnGas()
    {
        GameplayManager.instance.SpawnGasPiece(gasPiece, GetComponent<Player>().playerEmotePosition, gameObject.scene, base.Owner);
    }


    void SetFriction()
    {
        rb.sharedMaterial = friction;
    }

    public void SetStun()
    {
        //mach stun
        isStunned = true;
        Invoke("RemoveStun", 2f);

    }

    private void RemoveStun()
    {
        isStunned = false;
    }

    public void SetSmoke()
    {
        //mach stun
        isSmoked = true;
        runSpeed = runSpeed / 2;
        Invoke("RemoveSmoke", 4f);

    }

    private void SetCanAttack()
    {
        canAttack = true;
    }

    private void RemoveSmoke()
    {
        isSmoked = false;
        runSpeed = runSpeed * 2;
    }

    [ServerRpc]
    public void ServerHitback(GameObject target, float direction)
    {
        ObserversHitback(target, direction);
    }

    [ObserversRpc]
    private void ObserversHitback(GameObject target, float direction)
    {
        target.GetComponent<PlayerMovement>().hitDirection = direction;
        target.GetComponent<PlayerMovement>().hit = true;
        target.GetComponent<Player>().TakeDamage(30, gameObject.GetComponent<NetworkObject>());
    }


    [Reconcile]
    private void Reconciliation(ReconcileData rd, bool asServer)
    {
        transform.position = rd.Position;
        transform.rotation = rd.Rotation;
        rb.velocity = rd.Velocity;
        rb.angularVelocity = rd.AngularVelocity;
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
            ReconcileData rd = new ReconcileData(transform.position, transform.rotation, rb.velocity, rb.angularVelocity);
            Reconciliation(rd, true);
        }
    }
    private void CheckInput(out MoveData md)
    {
        md = default;
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        horizontalMove = horizontal;
        if (horizontal != 0)
        {
            lastMovement = horizontal;
        }

        if (horizontal == 0f && vertical == 0f && !_jump && !hit)
            return;

        md = new MoveData(_jump, canJump, hit, hitDirection, isStunned, horizontal, vertical);
        

        _jump = false;
        hit = false;

    }
    private void Update()
    {
        if (!base.IsOwner)
            return;

        if (!isStunned)
        {
            HandleJump();
            HandleSlope();
            HandleMove();
            HandleFalling();
            HandleAttack();
            checkHit();
            if (Input.GetKeyDown(KeyCode.F))
            {
                SpawnEmote();
            }
            if (isSmoked && (Time.time > nextActionTime))
            {
                nextActionTime += period;
                SpawnGas();
            }
        }
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
            leftGround = true; //weil player beim slopes runter laufen direkt auf fall ?bergeht, dachte ich diese variable k?nnte das blockieren
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
        if (slopeCheck.atWall)
        {
            rb.sharedMaterial = noFriction;
        }
    }
}
