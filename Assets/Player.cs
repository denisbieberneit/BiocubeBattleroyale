using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.HealthSystemCM;
using CodeMonkey.Utils;
using FishNet.Object;
using FishNet.Connection;

public class Player : NetworkBehaviour, IGetHealthSystem
{

    private HealthSystem healthSystem;

    private PlayerMovement pm;

    [SerializeField]
    private float baseHealth;


    private void Start()
    {
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

    public override void OnStartClient()
    {
        base.OnStartClient();
        healthSystem = new HealthSystem(baseHealth);

    }

    public void GainHealth(float health)
    {
        if (!base.IsOwner)
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


    public void TakeDamage(float takeDamage)
    {
        if (!base.IsOwner)
        {
            Debug.Log("Not owner");
            return;
        }
        if (healthSystem == null)
        {
            healthSystem = new HealthSystem(baseHealth);
            healthSystem.OnDead += HealthSystem_OnDead;
        }
        healthSystem.Damage(takeDamage);
        Debug.Log("Remaining after hit HP:" + healthSystem.GetHealth());
    }

    private void HealthSystem_OnDead(object sender, System.EventArgs e)
    {
        if (!base.IsOwner)
        {
            return;
        }
        Destroy(gameObject);
    }

    public HealthSystem GetHealthSystem()
    {
       
        return healthSystem;
    }
}
