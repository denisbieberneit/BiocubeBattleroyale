using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class ExplosionItemBehaviour : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private float lastMovement;
    private SlopeCheck slopeCheck;

    private void Start()
    {
        Debug.Log("Spawned");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        slopeCheck = GetComponent<SlopeCheck>();
        lastMovement = FindObjectOfType<PlayerMovement>().lastMovement;
    }

    void FixedUpdate()
    {
        anim.SetBool("CircleExplosion", true);
        rb.AddForce(new Vector2(7f * lastMovement, 0));
        if (slopeCheck.atWall)
        {
            Explode();
        }
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
            Explode();
            collision.gameObject.GetComponent<Player>().TakeDamage(30, gameObject.GetComponent<NetworkObject>());
        }
    }

    private void Explode()
    {
        rb.simulated = false;
        anim.SetBool("CircleExplosion", true);
        anim.SetBool("CircleExplosionExplosion", true);
        rb.bodyType = RigidbodyType2D.Static;
    }
}
