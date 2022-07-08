using FirstGearGames.LobbyAndWorld.Clients;
using FirstGearGames.LobbyAndWorld.Global;
using FirstGearGames.LobbyAndWorld.Global.Canvases;
using FirstGearGames.LobbyAndWorld.Lobbies;
using FirstGearGames.LobbyAndWorld.Lobbies.JoinCreateRoomCanvases;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;



public class GameplayManager : NetworkBehaviour
{
    #region Serialized
    [Header("Spawning")]
    /// <summary>
    /// Prefab to spawn.
    /// </summary>
    [Tooltip("Prefab to spawn.")]
    [SerializeField]
    private NetworkObject _playerPrefab = null;
    /// <summary>
    /// DeathDummy to spawn.
    /// </summary>
    [Tooltip("DeathDummy to spawn.")]
    [SerializeField]
    private GameObject _deathDummy = null;
    #endregion

    /// <summary>
    /// RoomDetails for this game. Only available on the server.
    /// </summary>
    private RoomDetails _roomDetails = null;
    /// <summary>
    /// LobbyNetwork.
    /// </summary>
    private LobbyNetwork _lobbyNetwork = null;
    /// <summary>
    /// Becomes true once someone has won.
    /// </summary>
    private bool _winner = false;
    /// <summary>
    /// Currently spawned player objects. Only exist on the server.
    /// </summary>
    private List<NetworkObject> _spawnedPlayerObjects = new List<NetworkObject>();

    public static GameplayManager instance;


    [SerializeField]
    private List<GameObject> spawns = new List<GameObject>();
    private bool gameOver = false;

    /// <summary>
    /// DeathDummy to spawn.
    /// </summary>
    [Tooltip("DeathCam to spawn.")]
    [SerializeField]
    private GameObject deathCam = null;

    #region Initialization and Deinitialization.

    private void Awake()
    {
        instance = this;
    }

    private void OnDestroy()
    {
        if (_lobbyNetwork != null)
        {
            _lobbyNetwork.OnClientJoinedRoom -= LobbyNetwork_OnClientStarted;
            _lobbyNetwork.OnClientLeftRoom -= LobbyNetwork_OnClientLeftRoom;
        }
    }

    /// <summary>
    /// Initializes this script for use.
    /// </summary>
    public void FirstInitialize(RoomDetails roomDetails, LobbyNetwork lobbyNetwork)
    {
        _roomDetails = roomDetails;
        _lobbyNetwork = lobbyNetwork;
        _lobbyNetwork.OnClientStarted += LobbyNetwork_OnClientStarted;
        _lobbyNetwork.OnClientLeftRoom += LobbyNetwork_OnClientLeftRoom;
        Debug.Log("Initialized with " + roomDetails + ", " + lobbyNetwork);
    }

    /// <summary>
    /// Called when a client leaves the room.
    /// </summary>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void LobbyNetwork_OnClientLeftRoom(RoomDetails arg1, NetworkObject arg2)
    {
        //Destroy all of clients objects, except their client instance.
        for (int i = 0; i < _spawnedPlayerObjects.Count; i++)
        {
            NetworkObject entry = _spawnedPlayerObjects[i];
            //Entry is null. Remove and iterate next.
            if (entry == null)
            {
                _spawnedPlayerObjects.RemoveAt(i);
                i--;
                continue;
            }

            //If same connection to client (owner) as client instance of leaving player.
            if (_spawnedPlayerObjects[i].Owner == arg2.Owner)
            {
                //Destroy entry then remove from collection.
                //InstanceFinder.ServerManager.Despawn(entry.gameObject);
                _spawnedPlayerObjects.RemoveAt(i);
                i--;
            }

        }
    }

    /// <summary>
    /// Called when a client starts a game.
    /// </summary>
    /// <param name="roomDetails"></param>
    /// <param name="client"></param>
    private void LobbyNetwork_OnClientStarted(RoomDetails roomDetails, NetworkObject client)
    {
        //Not for this room.
        if (roomDetails != _roomDetails)
        {
            Debug.Log("roomDetails!=_roomDetails");
            return;
        }

        //NetIdent is null or not a player.
        if (client == null)
        {
            Debug.Log("client==null");
            return;
        }
        if (client.Owner == null)
        {
            Debug.Log("client.Owner==null");
            return;
        }
        Debug.Log("Now Spawning");
        SpawnPlayer(client.Owner);
    }
    #endregion

    #region Death.
    /// <summary>
    /// Called when object exits trigger. Used to respawn players.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (!base.IsServer)
            return;

        NetworkObject netIdent = other.gameObject.GetComponent<NetworkObject>();
        //If doesn't have a netIdent or no owning client exit.
        if (netIdent == null || netIdent.Owner == null)
            return;

        //If there is an owning client then destroy the object and respawn.
        StartCoroutine((__DelayRespawn(netIdent)));
    }

    /// <summary>
    /// Destroys netIdent and respawns player after delay.
    /// </summary>
    /// <param name="netIdent"></param>
    /// <returns></returns>
    private IEnumerator __DelayRespawn(NetworkObject netIdent)
    {
        //Send Rpc to spawn death dummy then destroy original.
        RpcSpawnDeathDummy(netIdent.transform.position);
        NetworkConnection conn = netIdent.Owner;
        InstanceFinder.ServerManager.Despawn(netIdent.gameObject);

        //Wait a little to respawn player.
        yield return new WaitForSeconds(1f);
        //Don't respawn if someone won.
        if (_winner)
            yield break;
        /* Check for rage quit conditions (left room). */
        if (conn == null)
            yield break;
        ClientInstance ci = ClientInstance.ReturnClientInstance(conn);
        if (ci == null || !_roomDetails.StartedMembers.Contains(ci.NetworkObject))
            yield break;

        SpawnPlayer(conn);
    }

    /// <summary>
    /// Spawns a dummy player to show death.
    /// </summary>
    /// <param name="player"></param>
    [ObserversRpc]
    private void RpcSpawnDeathDummy(Vector3 position)
    {
        GameObject go = Instantiate(_deathDummy, position, Quaternion.identity);
        ServerManager.Spawn(go, null);

        UnitySceneManager.MoveGameObjectToScene(go, gameObject.scene);
    }
    #endregion

    #region Winning.

    private bool CheckGameEnd()
    {
        int alivePlayer = 0;
        foreach (NetworkObject item in _roomDetails.StartedMembers)
        {
            bool isDead = item.gameObject.GetComponent<Player>().dead;

            //If not winner.
            if (!isDead)
            {
                alivePlayer = alivePlayer + 1;
            }
        }
        if (alivePlayer == 1)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Ends the game announcing winner and sending clients back to lobby.
    /// </summary>
    /// <returns></returns>
    private IEnumerator __PlayerWon()
    {
        List<NetworkObject> deadPlayers = new List<NetworkObject>();
        NetworkObject winnerObject = null;

        //Find all players in room and destroy their objects. Don't destroy client instance!
        foreach (NetworkObject item in _roomDetails.StartedMembers)
        {
            bool isDead = item.gameObject.GetComponent<Player>().dead;

            //If not winner.
            if (isDead)
            {
                deadPlayers.Add(item);
            }
            else
            {
                winnerObject = item;
            }
        }

        //Send out winner text.
        ClientInstance ci = ClientInstance.ReturnClientInstance(winnerObject.Owner);
        string playerName = ci.PlayerSettings.GetUsername();
        foreach (NetworkObject item in _roomDetails.StartedMembers)
        {
            if (item != null && item.Owner != null)
                TargetShowWinner(item.Owner, playerName, (item.Owner == winnerObject.Owner));
        }

        //Wait a moment then kick the players out. Not required.
        yield return new WaitForSeconds(4f);
        List<NetworkObject> collectedIdents = new List<NetworkObject>();
        foreach (NetworkObject item in _roomDetails.StartedMembers)
        {
            ClientInstance cli = ClientInstance.ReturnClientInstance(item.Owner);
            if (ci != null)
                collectedIdents.Add(cli.NetworkObject);
        }
        foreach (NetworkObject item in collectedIdents)
            _lobbyNetwork.TryLeaveRoom(item);
    }

    /// <summary>
    /// Displayers who won.
    /// </summary>
    /// <param name="winner"></param>
    [TargetRpc]
    private void TargetShowWinner(NetworkConnection conn, string winnerName, bool won)
    {
        Color c = (won) ? MessagesCanvas.LIGHT_BLUE : Color.red;
        string text = (won) ? "Congrats, you won!" :
            $"{winnerName} has won, better luck next time!";
        GlobalManager.CanvasesManager.MessagesCanvas.InfoMessages.ShowTimedMessage(text, c, 4f);
    }
    #endregion

    #region Spawning.
    /// <summary>
    /// Spawns a player at a random position for a connection.
    /// </summary>
    /// <param name="conn"></param>
    private void SpawnPlayer(NetworkConnection conn)
    {
        Vector3 next = GetSpawn();

        //Make object and move it to proper scene.
        NetworkObject netIdent = Instantiate<NetworkObject>(_playerPrefab, next, Quaternion.identity);
        UnitySceneManager.MoveGameObjectToScene(netIdent.gameObject, gameObject.scene);

        _spawnedPlayerObjects.Add(netIdent);
        base.Spawn(netIdent.gameObject, conn);

        //NetworkObject netIdent = conn.identity;            
        netIdent.transform.position = next;
        RpcTeleport(netIdent, next);
    }
    /// <summary>
    /// teleports a NetworkObject to a position.
    /// </summary>
    /// <param name="ident"></param>
    /// <param name="position"></param>
    [ObserversRpc]
    private void RpcTeleport(NetworkObject ident, Vector3 position)
    {
        ident.transform.position = position;
    }
    #endregion

    [Server]
    public void HandleDeath(NetworkObject netIdent, UnityEngine.SceneManagement.Scene scene, NetworkObject killer)
    {
        Transform killerTransform;
         if (killer != null)
         {
             killerTransform = killer.gameObject.transform;
         }
         else
         {
             killerTransform = netIdent.gameObject.transform;
         }
        Vector3 next = killerTransform.position;

         //Make object and move it to proper scene.
        NetworkObject netIdentNew = Instantiate<NetworkObject>(_deathDummy.GetComponent<NetworkObject>(), next, Quaternion.identity);
        UnitySceneManager.MoveGameObjectToScene(netIdentNew.gameObject, gameObject.scene);

        _spawnedPlayerObjects.Add(netIdentNew);// not
        base.Spawn(netIdentNew.gameObject, netIdent.Owner);

        //NetworkObject netIdent = conn.identity;            
        netIdentNew.transform.position = next;
        RpcTeleport(netIdentNew, next);
        //Send Rpc to spawn death dummy then destroy original.
         NetworkConnection deathConn = netIdentNew.Owner;
         NetworkObject netDeathCam = Instantiate<NetworkObject>(deathCam.GetComponent<NetworkObject>(), new Vector3(killerTransform.position.x, killerTransform.position.y, -1), Quaternion.identity);
         SceneLookupData sld = SceneLookupData.CreateData(gameObject.scene.handle);
         UnitySceneManager.MoveGameObjectToScene(netDeathCam.gameObject, sld.GetScene(out _));
         base.Spawn(netDeathCam.gameObject, deathConn);

         //NetworkObject netIdent = conn.identity;            
         netDeathCam.transform.position = new Vector3(killerTransform.position.x, killerTransform.position.y, -1);
         RpcTeleport(netDeathCam, new Vector3(killerTransform.position.x, killerTransform.position.y, -1));
    }

    private Vector3 GetSpawn()
    {
        return spawns[Random.Range(0, spawns.Count)].gameObject.transform.position;
    }

    [Server]
    public void SpawnAbility(NetworkConnection playerConnection, GameObject item, float move , UnityEngine.SceneManagement.Scene scene, Vector3 v)
    {
        GameObject obj = Instantiate(item, v, Quaternion.identity);
        obj.GetComponent<Rigidbody2D>().AddForce(new Vector2(20f * move, 0), ForceMode2D.Force);
        ServerManager.Spawn(obj, playerConnection);
        UnitySceneManager.MoveGameObjectToScene(obj, scene);
    }
}
