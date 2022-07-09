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
    [SerializeField]
    private PlayerMovement pm;

    [SerializeField]
    private float baseHealth;

    [SerializeField]
    private int maxHealth;

    public int currentHealth;

    [SerializeField]
    private HealthBar healthBar;

    public GameObject emote;

    public bool dead = false;

    public FunctionPeriodic p; 

    [SerializeField]
    public Transform playerEmotePosition;

    private void Start()
    {        
        healthBar.SetMaxHealth(maxHealth);
        p = FunctionPeriodic.Create(() =>
        {
            if (DamageCircle.IsOutsideCircle_Static(transform.position))
            {
                TakeDamage(10, null);
            }
            else
            {
                GainHealth(1);
            }
        }, 2f);
        emote = EmoteFactory.instance.GetEmoteById(PlayerPrefs.GetInt("emote", 0));
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
        pm.SetStun();
    }

    public void SmokePlayer()
    {
        pm.SetSmoke();
    }

    public void TakeDamage(int takeDamage, NetworkObject attacker)
    {
        currentHealth -= takeDamage;
        healthBar.SetHealth(currentHealth);
        if (currentHealth <= 0)
        {
            OnDeath(attacker);
        }
    }

    private void OnDestroy()
    {
        p.DestroySelf();
    }
    private void OnDeath(NetworkObject killer)
    {
        dead = true;
        //If there is an owning client then destroy the object and respawn.
        HandleDeath(GetComponent<NetworkObject>(), gameObject.scene, killer);
    }


    [ServerRpc]
    private void HandleDeath(NetworkObject owner, UnityEngine.SceneManagement.Scene scene, NetworkObject killer)
    {
        GameplayManager.instance.HandleDeath(owner, scene, killer);
    }
}
