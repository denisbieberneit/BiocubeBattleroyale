using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.HealthSystemCM;
using CodeMonkey.Utils;
using FishNet.Object;
using FishNet.Connection;

public class Player : NetworkBehaviour,IGetHealthSystem
{

    private HealthSystem healthSystem;

    private PlayerMovement pm;

    [SerializeField]
    private float baseHealth;

    private void Start()
    {
        if (!base.IsClient)
        {
            return;
        }
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
        },2f);
    }

    public void GainHealth(float health)
    {
        if (!base.IsClient)
        {
            return;
        }
        if (healthSystem == null)
        {
            healthSystem = new HealthSystem(baseHealth);
            healthSystem.OnDead += HealthSystem_OnDead;
        }
        healthSystem.Heal(health);
    }


    public void StunPlayer()
    {
        if (pm == null)
        {
            GetPlayerMovement();
        }
        pm.SetStun();
    }

    private void GetPlayerMovement()
    {
        pm = GetComponent<PlayerMovement>();
    }


    public void TakeDamage(float takeDamage)
    {
        if (!base.IsClient)
        {
            return;
        }
        if (healthSystem == null)
        {
            healthSystem = new HealthSystem(baseHealth);
            healthSystem.OnDead += HealthSystem_OnDead;
        }
        healthSystem.Damage(takeDamage);
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        if (!base.IsClient)
        {
            return;
        }
 
        Destroy(gameObject);
    }

    public HealthSystem GetHealthSystem()
    {
        if (!base.IsClient)
        {
            return null;
        }
        if (healthSystem == null)
        {
            healthSystem = new HealthSystem(baseHealth);
            healthSystem.OnDead += HealthSystem_OnDead;
        }
        return healthSystem;
    }
}
