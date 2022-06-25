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
        /// Region players may spawn.
        /// </summary>
        [Tooltip("Region players may spawn.")]
        [SerializeField]
        private Vector3 _spawnRegion = Vector3.one;
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


    private void Awake()
    {
        instance = this;
        InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
    }
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

        #region Initialization and Deinitialization.
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
        /// <param name="roomDetails"></param>
        public void FirstInitialize(RoomDetails roomDetails, LobbyNetwork lobbynetwork)
        {
            _roomDetails = roomDetails;
            _lobbyNetwork = lobbynetwork;
            _lobbyNetwork.OnClientStarted += LobbyNetwork_OnClientStarted;
            _lobbyNetwork.OnClientLeftRoom += LobbyNetwork_OnClientLeftRoom;
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
                return;
            //NetIdent is null or not a player.
            if (client == null || client.Owner == null)
                return;

            /* POSSIBLY USEFUL INFORMATION!!!!!
             * POSSIBLY USEFUL INFORMATION!!!!!
             * If you want to wait until all players are in the scene
             * before spaning then check if roomDetails.StartedMembers.Count
             * is the same as roomDetails.MemberIds.Count. A member is considered
             * started AFTER they have loaded all of the scenes. */
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
            UnitySceneManager.MoveGameObjectToScene(go, gameObject.scene);
            Destroy(go, 1f);
        }
        #endregion

        #region Winning.



        /// <summary>
        /// Ends the game announcing winner and sending clients back to lobby.
        /// </summary>
        /// <returns></returns>
    private void __PlayerWon()
    {
       
    }
    private void TimeManager_OnTick()
    {
        if (!gameOver)
            __PlayerWon();
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
        SceneLookupData sld = SceneLookupData.CreateData(gameObject.scene.handle);
        UnitySceneManager.MoveGameObjectToScene(netIdent.gameObject, sld.GetScene(out _));
        _spawnedPlayerObjects.Add(netIdent);
        //Subscriber to kingtimer so we know when a player reaches 0.
        base.Spawn(netIdent.gameObject, conn);
        //NetworkObject netIdent = conn.identity
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

        /// <summary>
        /// Draw spawn region.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(transform.position, _spawnRegion);
        }
    #endregion

    private Vector3 GetSpawn()
    {
        return spawns[Random.Range(0, spawns.Count)].gameObject.transform.position;
    }

    [Server]
    public void SpawnAbility(NetworkConnection playerConnection, GameObject item, Vector3 v, UnityEngine.SceneManagement.Scene scene)
    {
        GameObject obj = Instantiate(item, v, Quaternion.identity);
        ServerManager.Spawn(obj, playerConnection);
        UnitySceneManager.MoveGameObjectToScene(obj, scene);
    }

    [Server]
    public void HandleDeath(NetworkObject netIdent, UnityEngine.SceneManagement.Scene scene, NetworkObject killer)
    {
        //Send Rpc to spawn death dummy then destroy original.
        RpcSpawnDeathDummy(netIdent.transform.position);
        NetworkConnection deathConn = netIdent.Owner;
        InstanceFinder.ServerManager.Despawn(netIdent.gameObject);
        NetworkObject netDeathCam = Instantiate<NetworkObject>(deathCam.GetComponent<NetworkObject>(), new Vector3(killer.gameObject.transform.position.x, killer.gameObject.transform.position.y, -1), Quaternion.identity);
        SceneLookupData sld = SceneLookupData.CreateData(gameObject.scene.handle);
        UnitySceneManager.MoveGameObjectToScene(netDeathCam.gameObject, sld.GetScene(out _));
        base.Spawn(netDeathCam.gameObject, deathConn);

        //NetworkObject netIdent = conn.identity;            
        netDeathCam.transform.position = new Vector3(killer.gameObject.transform.position.x, killer.gameObject.transform.position.y, -1);
        netDeathCam.transform.parent = killer.gameObject.transform;
        RpcTeleport(netDeathCam, new Vector3(killer.gameObject.transform.position.x, killer.gameObject.transform.position.y, -1));
    }
}
