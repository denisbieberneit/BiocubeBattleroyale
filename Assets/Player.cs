using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using FishNet.Object;
using FishNet.Connection;
using FishNet;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class Player : NetworkBehaviour
{
    private PlayerMovement pm;

    [SerializeField]
    private float baseHealth;

    [SerializeField]
    private GameObject deathDummy;

    [SerializeField]
    private int maxHealth;

    public int currentHealth;

    [SerializeField]
    private HealthBar healthBar;

    private GameplayManager gameplayManager;

    public override void OnStartClient()
    {
        base.OnStartClient();
        
        healthBar.SetMaxHealth(maxHealth);

        if (!IsOwner)
        {
            return;
        }
        gameplayManager = GameObject.Find("GameplayManager").GetComponent<GameplayManager>();

        FunctionPeriodic.Create(() =>
        {
            /*if (DamageCircle.IsOutsideCircle_Static(transform.position)) //TODO: add back
            {
                TakeDamage(1f);
            }
            else
            {
                GainHealth(1f);
            }*/
        }, 2f);

    }

    public void GainHealth(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }


    public void StunPlayer()
    {
        if (!base.IsOwner)
        {
            return;
        }
        if (pm == null)
        {
            GetPlayerMovement();
        }
        pm.SetStun();
    }

    private void GetPlayerMovement()
    {
        if (!base.IsOwner)
        {
            return;
        }
        pm = GetComponent<PlayerMovement>();
    }


    public void TakeDamage(int takeDamage)
    {
        currentHealth -= takeDamage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }


    private void OnDeath()
    {
    
        //If there is an owning client then destroy the object and respawn.
        __DelayRespawn(this.gameObject);
    }

    private void __DelayRespawn(NetworkObject netIdent)
    {
        //Send Rpc to spawn death dummy then destroy original.
        RpcSpawnDeathDummy(netIdent.transform.position,netIdent);
    }

    public void RpcSpawnDeathDummy(Vector3 position, NetworkObject netIdent)
    {
        GameObject go = Instantiate(deathDummy, position, Quaternion.identity);
        base.Spawn(go, base.Owner);
        StartCoroutine(GameplayManager.instance.__DelayRespawn(netIdent));

    }
}
