using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;

public class StunItemBehaviour : NetworkBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private float lastMovement;
    private SlopeCheck slopeCheck;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastMovement = FindObjectOfType<PlayerMovement>().lastMovement;
        slopeCheck = FindObjectOfType<SlopeCheck>();
    }

    void FixedUpdate()
    {
        anim.SetBool("CircleStun", true);
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
            int ownerId = GetComponent<NetworkObject>().OwnerId;
            int objectId = collision.gameObject.GetComponent<NetworkObject>().OwnerId;
            if (ownerId == objectId)
            {
                //siehst du was ich schreibe?
                Debug.Log("Selfhit");
                //selfhit
                return;
            }
            Explode();
            collision.gameObject.GetComponent<Player>().StunPlayer();
        }
    }


    private void Explode()
    {
        rb.simulated = false;
        anim.SetBool("CircleStun", true);
        anim.SetBool("CircleStunExplosion", true);
        rb.bodyType = RigidbodyType2D.Static;
    }
}
