using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using FishNet;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class AbilitySpawner : NetworkBehaviour
{
    [SerializeField]
    private List<Transform> spawnPoints;

    [SerializeField]
    private List<GameObject> spawnableItems;

    [SerializeField]
    private int itemSpawnSeconds;

   void Start()
    { 
        foreach (Transform spawn in spawnPoints)
        {
            GameObject item = GetRandomItem();  
            GameObject instItem = Instantiate(item, new Vector3(spawn.position.x, spawn.position.y), Quaternion.identity); 
            InstanceFinder.ServerManager.Spawn(instItem, null);
            UnitySceneManager.MoveGameObjectToScene(instItem, gameObject.scene);
        }
    }

    private GameObject GetRandomItem()
    {
        return spawnableItems[Random.Range(0, spawnableItems.Count)];
    }
}
