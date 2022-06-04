using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitySpawner : MonoBehaviour
{
    [SerializeField]
    private List<ItemSpawnPoint> spawnPoints;

    [SerializeField]
    private List<ItemObject> spawnItems;

    [SerializeField]
    private int itemSpawnSeconds;


    int i = 0;

   void FixedUpdate()
    {
        if (spawnItems != null)
        {
            if (Time.time > i)
            {
                foreach (ItemSpawnPoint spawn in spawnPoints)
                {
                    if (spawn.item == null)
                    {
                   
                        ItemObject randItem = GetRandomItem();
                        randItem.setOwner(null);
                        spawn.item = randItem.gameObject;
                        SpawnAbility(randItem, spawn);
                    }
                }
                i += itemSpawnSeconds;
            }
        }
    }

    private ItemObject GetRandomItem()
    {
        return spawnItems[Random.Range(0, spawnItems.Count)];
    }

    public void SpawnAbility(ItemObject respawnAbility, ItemSpawnPoint spawn)
    {
        spawn.ServerSetAnim(respawnAbility.referenceItem.animationName);
    }
}
