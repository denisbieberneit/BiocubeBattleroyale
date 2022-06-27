using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet.Object;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;


public class GameHandler : NetworkBehaviour
{
    [SerializeField]
    private GameObject zonePref;

    private GameObject currentZone;

    private void Start()
    {
        if (!base.IsServer)
        {
            return;
        }
        startMatch();
    }

    void startMatch()
    {
        currentZone = Instantiate(zonePref);
        ServerManager.Spawn(currentZone, null);
        UnitySceneManager.MoveGameObjectToScene(currentZone, gameObject.scene);

    }

    void stopMatch()
    {
        Destroy(currentZone);
    }
}
