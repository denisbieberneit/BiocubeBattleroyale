using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using FishNet.Object.Prediction;

public class StunItemBehaviour : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private float lastMovement;
    private SlopeCheck slopeCheck;
    private bool exploded = false;

    public struct MoveData
    {
        public float Horizontal;
        public MoveData(float horizontal)
        {
            Horizontal = horizontal;
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
    private void Awake()
    {
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        InstanceFinder.TimeManager.OnPostTick += TimeManager_OnPostTick;
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        slopeCheck = GetComponent<SlopeCheck>();
        if (rb.velocity.x < 0)
        {
            lastMovement = -1;
        }
        else
        {
            lastMovement = 1;
        }
    }
    [Replicate]
    private void updateMethod(MoveData md, bool asServer, bool replaying = false)
    {

        if (slopeCheck.atWall)
        {
            StartCoroutine(Explode());
        }

        anim.SetBool("CircleExplosion", true);
        rb.AddForce(new Vector2(7f * lastMovement, 0));
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
        if (!exploded)
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
    }
    private void TimeManager_OnPostTick()
    {
        if (!exploded)
        {
            if (base.IsServer)
            {
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation, rb.velocity, rb.angularVelocity);
                Reconciliation(rd, true);
            }
        }
    }
    void CheckInput(out MoveData md)
    {
        md = default;

        md = new MoveData(7f * lastMovement);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Collided with " + collision.gameObject.tag);
            int ownerId = GetComponent<NetworkObject>().OwnerId;
            int objectId = collision.gameObject.GetComponent<NetworkObject>().OwnerId;
            if (ownerId == objectId)
            {
                Debug.Log("Selfhit");
                //selfhit
                return;
            }
            StartCoroutine(Explode());
            collision.gameObject.GetComponent<Player>().StunPlayer();
        }
    }

    IEnumerator Explode()
    {
        exploded = true;
        rb.simulated = false;
        anim.SetBool("CircleExplosion", true);
        anim.SetBool("CircleExplosionExplosion", true);
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(.5f);
        base.Despawn();
    }
}
