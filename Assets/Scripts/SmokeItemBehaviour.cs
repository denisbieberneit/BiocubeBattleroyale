using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class SmokeItemBehaviour : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private float lastMovement;
    private SlopeCheck slopeCheck;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        slopeCheck = FindObjectOfType<SlopeCheck>();
        if (rb.velocity.x <= 0)
        {
            lastMovement = -1;
        }
        else
        {
            lastMovement = 1;
        }
    }

    void FixedUpdate()
    {
        anim.SetBool("CircleGas",true);
        rb.AddForce(new Vector2(7f*lastMovement, 0));
        if (slopeCheck.atWall)
        {
            Explode();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            int ownerId = GetComponent<NetworkObject>().OwnerId;
            int objectId = collision.gameObject.GetComponent<NetworkObject>().OwnerId;
            if (ownerId == objectId)
            {
                Debug.Log("Selfhit");
                return;
            }
            Debug.Log("Collided with " + collision.gameObject.tag);
            Explode();
        }
    }


    private void Explode()
    {
        rb.simulated = false;
        anim.SetBool("CircleGas", true);
        anim.SetBool("CircleGasExplosion", true);
        rb.bodyType = RigidbodyType2D.Static;
    }
}
