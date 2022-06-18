using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;

public class AbilitySpawner : NetworkBehaviour
{
    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<GameObject> spawnableItems;

    [SerializeField]
    private int itemSpawnSeconds;


    int i = 0;

   void FixedUpdate()
    {
        if (Time.time > i)
        {
            foreach (Transform spawn in spawnPoints)
            {
                if (spawn.childCount != 0)
                {
                    continue;
                }
                GameObject item = GetRandomItem();
                GameObject instItem = Instantiate(item, new Vector3(spawn.position.x, spawn.position.y), Quaternion.identity);
                instItem.transform.parent =spawn.transform;
                InstanceFinder.ServerManager.Spawn(instItem, null);

            }
            i += itemSpawnSeconds;
        }        
    }

    private GameObject GetRandomItem()
    {
        return spawnableItems[Random.Range(0, spawnableItems.Count)];
    }
}
