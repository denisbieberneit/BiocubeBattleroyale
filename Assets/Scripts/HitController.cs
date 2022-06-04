using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitController : MonoBehaviour
{
    private float timeBetweenGasCount = 30f;
    private float gasLenghtCount = 5f; 
    public bool spawnGas = false;
    public GameObject gasToSpawn;

    public bool playerStunned = false;
    private float stunTime = 80f;

    public PlayerMovement playerMovement;


    // Update is called once per frame
    void FixedUpdate()
    {
        if (spawnGas)
        {
            timeBetweenGasCount = timeBetweenGasCount - 1f;

            playerMovement.isGased = true;
        }
        if (timeBetweenGasCount < 0f && spawnGas)
        {
            Instantiate(gasToSpawn, transform.position, Quaternion.identity);
            timeBetweenGasCount = 30f;
            gasLenghtCount = gasLenghtCount - 1f;
        }
        //Debug.Log(gasCount + " " +spawnGas.ToString());
        if (gasLenghtCount < 0f)
        {
            gasLenghtCount = 5f;
            playerMovement.runSpeed = 20f;
            spawnGas = false;
            playerMovement.isGased = false;
        }

        if (playerStunned)
        {
            stunTime = stunTime - 1f;
        }

        if (stunTime < 0f)
        {
            playerMovement.rb.sharedMaterial = playerMovement.noFriction;
            playerMovement.isStunned = false;
            stunTime = 80f;
            playerStunned = false;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DarkZone")
        {
            var DIst = Vector2.Distance(transform.position, collision.transform.position);
            Debug.Log(DIst);
        }
    }
}
