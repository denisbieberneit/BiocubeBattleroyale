using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;


public class ExplosionItemBehaviour : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private float lastMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        lastMovement = FindObjectOfType<PlayerMovement>().lastMovement;
    }

    void FixedUpdate()
    {
        anim.SetBool("CircleExplosion", true);
        rb.AddForce(new Vector2(7f * lastMovement, 0));
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
            collision.gameObject.GetComponent<Player>().TakeDamage(3f);
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
