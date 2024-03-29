using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks, IOnEventCallback, IInRoomCallbacks
{
    #region singleton 

    private static NetworkManager _instance;

    public static NetworkManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<NetworkManager>();
            }
            return _instance;
        }
    }

    #endregion   
    const byte EVENT_ADD_TILE_CODE = 3;
    const byte EVENT_REMOVE_ALL_TILES_CODE = 4;
    const byte EVENT_GAME_OVER_CODE = 5;
    const byte EVENT_REMOVE_TILE_CODE = 6;

    const byte EVENT_PLAYER_WON_AUCTION_CODE = 7;

    private TypedLobby mainLobby = new TypedLobby("ElfenGameLobby", LobbyType.Default);

    public Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();
    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = GameConstants.username;
            _ = PhotonNetwork.ConnectUsingSettings();
            PhotonPeer.RegisterType(typeof(CardEnum), 255, CardEnumExtension.Serialize, CardEnumExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(MovementTile), 252, MovementTileExtension.Serialize, MovementTileExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(PlayerColor), 254, PlayerColorExtension.Serialize, PlayerColorExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(GamePhase), 253, GamePhaseExtension.Serialize, GamePhaseExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(GameMode), 251, GameModeExtension.Serialize, GameModeExtension.Deserialize);
            networkPlayers = new Dictionary<string, Photon.Realtime.Player>();
            ResetPlayerProperties();
        }
    }

    private Dictionary<string, Photon.Realtime.Player> networkPlayers;
    public Photon.Realtime.Player GetPlayer(string playerId)
    {
        if (!networkPlayers.ContainsKey(playerId))
        {
            foreach (Photon.Realtime.Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                networkPlayers[p.UserId] = p;
            }
        }

        if (networkPlayers.ContainsKey(playerId)) return networkPlayers[playerId];
        else return null;
    }

    public void ResetPlayerProperties()
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(Player.pCOINS, null);
        hashtable.Add(Player.pCOLOR, null);
        hashtable.Add(Player.pNAME, null);
        hashtable.Add(Player.pPOINTS, null);
        hashtable.Add(Player.pHIDDEN_TILES, null);
        hashtable.Add(Player.pVISIBLE_TILES, null);
        hashtable.Add(Player.pCARDS, null);
        hashtable.Add(Player.pTOWN, null);
        Photon.Realtime.Player p = PhotonNetwork.LocalPlayer;
        if (p != null) p.SetCustomProperties(hashtable);

    }


    public bool isConnected()
    {
        return PhotonNetwork.IsConnected;
    }

    public void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }

    public void JoinOrCreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
        {
            Debug.Log("Joining or creating room: " + roomName);
            PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 6, IsVisible = true, PublishUserId = true }, null);
        }
    }


    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
        {
            Debug.Log("Joining room: " + roomName);
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public List<Photon.Realtime.Player> GetPlayers()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>();

            foreach (Photon.Realtime.Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                players.Add(p);
            }
            return players;
        }
        return null;
    }

    public void verifyAllPlayersExist()
    {
        foreach (Photon.Realtime.Player p in GetPlayers())
        {
            _ = Player.GetOrCreatePlayer(p.UserId);
        }
    }

    public string getNetworkState()
    {
        return PhotonNetwork.NetworkClientState.ToString();
    }

    public bool inGameMaster()
    {
        return PhotonNetwork.InRoom && PhotonNetwork.IsMasterClient;
    }

    public bool inGame()
    {
        return PhotonNetwork.InRoom;
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //base.OnPlayerLeftRoom(otherPlayer);
        if (otherPlayer != PhotonNetwork.LocalPlayer && (MainUIManager.manager ||
        (MainMenuUIManager.manager && MainMenuUIManager.manager.GetLoadedOwner() == otherPlayer.UserId)))
        {
            LeaveRoom();
        }

    }

    // public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    // {
    //     if (!PhotonNetwork.InRoom) return;
    //     Debug.Log($"{newMasterClient.UserId} is now the master client");
    //     if (newMasterClient.UserId == PhotonNetwork.LocalPlayer.UserId)
    //     {
    //         string roomName = PhotonNetwork.CurrentRoom.Name;
    //         Debug.Log("I am the Captain Now!");
    //         TaskRunner.runOnMainThread(() =>
    //         {
    //             MainMenuUIManager.manager.OnRoomCreated(roomName);
    //         });
    //     };
    // }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        Debug.Log("Room properties updated");
        if (Game.currentGame != null)
        {
            Game.currentGame.UpdateGameProperties(propertiesThatChanged);
        }
    }

    public void SetGameProperty(object key, object value)
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(key, value);
        _ = PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    public void SetGameProperties(ExitGames.Client.Photon.Hashtable properties)
    {
        if (PhotonNetwork.InRoom)
        {
            _ = PhotonNetwork.CurrentRoom.SetCustomProperties(properties);
        }
        else
        {
            Debug.LogError("Cannot set properties when not in room");
        }
    }

    public void SetGamePropertiesWithCheck(ExitGames.Client.Photon.Hashtable properties, ExitGames.Client.Photon.Hashtable expectedProperties)
    {
        _ = PhotonNetwork.CurrentRoom.SetCustomProperties(properties, expectedProperties);
    }

    public void SetPlayerStatsByPlayerName(string playerName, ExitGames.Client.Photon.Hashtable hashtable)
    {
        Photon.Realtime.Player p = GetPlayer(playerName);
        if (p != null)
        {
            p.SetCustomProperties(hashtable);
        }
    }

    public void SetLocalPlayerStats(ExitGames.Client.Photon.Hashtable hashtable)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(hashtable);
    }

    public void SetPlayerPropertyByPlayerName(string playerName, object key, object value)
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(key, value);
        Photon.Realtime.Player p = GetPlayer(playerName);
        if (p != null) p.SetCustomProperties(hashtable);
    }

    public void SetPlayerProperty(object key, object value)
    {
        SetPlayerPropertyByPlayerName(GameConstants.username, key, value);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        //Debug.Log($"{targetPlayer.UserId} properties updated");
        Player pm = Player.GetOrCreatePlayer(targetPlayer.UserId);
        if (pm != null)
        {
            pm.UpdatePlayerStats(changedProps);
        }
    }

    public void AddTileToRoad(string roadName, MovementTile movementTile)
    {
        object[] data = new object[] { roadName, movementTile };
        //Debug.Log("Sending Event");
        RaiseEvent(EVENT_ADD_TILE_CODE, data);
    }


    public void RemoveTileFromRoad(MovementTile m1, string path)
    {
        object[] data = new object[] { m1, path };
        RaiseEvent(EVENT_REMOVE_TILE_CODE, data);
    }

    public void SignalPlayerWonAuction(string playerName, int BidAmount, MovementTile auctionTile)
    {
        object[] data = new object[] { playerName, BidAmount, auctionTile };
        RaiseEvent(EVENT_PLAYER_WON_AUCTION_CODE, data);

    }

    public void ClearAllTiles()
    {
        object[] data = new object[] { };

        RaiseEvent(EVENT_REMOVE_ALL_TILES_CODE, data);
    }

    public void GameOver()
    {
        object[] data = new object[] { };

        RaiseEvent(EVENT_GAME_OVER_CODE, data);
    }

    private void RaiseEvent(byte eventCode, object[] data)
    {
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
        {
            Receivers = ReceiverGroup.Others,
            CachingOption = EventCaching.AddToRoomCache
        };

        SendOptions sendOptions = new SendOptions
        {
            Reliability = true
        };

        PhotonNetwork.RaiseEvent(eventCode, data, raiseEventOptions, sendOptions);

    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == EVENT_ADD_TILE_CODE && MainUIManager.manager)
        {
            object[] data = (object[])photonEvent.CustomData;
            string roadName = (string)data[0];
            MovementTile movementTile = (MovementTile)data[1];
            //Debug.LogError($"Add Tile Event triggered for road {roadName} and Tile {Enum.GetName(typeof(MovementTile), movementTile)}");

            MainUIManager.manager.AddTile(roadName, movementTile);
        }
        else if (photonEvent.Code == EVENT_REMOVE_TILE_CODE && MainUIManager.manager)
        {
            object[] data = (object[])photonEvent.CustomData;
            MovementTile m1 = (MovementTile)data[0];
            string path = (string)data[1];
            MainUIManager.manager.RemoveTile(m1, path);
        }
        else if (photonEvent.Code == EVENT_REMOVE_ALL_TILES_CODE && MainUIManager.manager)
        {
            _ = MainUIManager.manager.ClearAllTiles();
        }
        else if (photonEvent.Code == EVENT_GAME_OVER_CODE)
        {
            Game.currentGame.GameOver();
        }
        else if (photonEvent.Code == EVENT_PLAYER_WON_AUCTION_CODE)
        {
            object[] data = (object[])photonEvent.CustomData;
            string playerName = (string)data[0];
            int bidAmount = (int)data[1];
            MovementTile auctionTile = (MovementTile)data[2];
            Player local = Player.GetLocalPlayer();
            if (local.userName == playerName)
            {
                local.nCoins -= bidAmount;
                local.AddVisibleTile(auctionTile);
            }
        }
    }

    public void LoadArena()
    {
        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("PhotonNetwork: Trying to load game but not in room");
            return;
        }
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork: Trying to load a level but we are not the master client");
        }
        Debug.Log($"PhotonNetwork : Loading Map with {PhotonNetwork.CurrentRoom.PlayerCount} players");
        PhotonNetwork.LoadLevel("Main");
    }



    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            RoomInfo info = roomList[i];
            if (info.RemovedFromList)
            {
                cachedRoomList.Remove(info.Name);
            }
            else
            {
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnJoinedLobby()
    {
        cachedRoomList.Clear();
    }

    public bool InLobby()
    {
        return PhotonNetwork.InLobby && PhotonNetwork.CurrentLobby == mainLobby;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
        if (MainMenuUIManager.manager)
        {
            MainMenuUIManager.manager.UpdateSessionListView(Lobby.availableSessions);
        }
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        cachedRoomList.Clear();
    }

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to server.");
        PhotonNetwork.JoinLobby(mainLobby);
    }


    public void CreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady && !PhotonNetwork.InRoom)
        {
            Debug.Log("Creating room: " + roomName);
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 6, IsVisible = true, PublishUserId = true });
        }
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room created successfully.");
        string sessionId = PhotonNetwork.CurrentRoom.Name;
        if (!Lobby.activeGames.ContainsKey(sessionId))
        {
            Debug.LogError("No lobby session found for room with id " + sessionId);
            return;
        }
        if (Lobby.activeGames[sessionId].createdBy == GameConstants.username)
        {
            TaskRunner.runOnMainThread(() =>
            {
                MainMenuUIManager.manager.OnRoomCreated(sessionId);
            });
        }
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room {PhotonNetwork.CurrentRoom.Name}");
        if (MainMenuUIManager.manager != null)
        {
            MainMenuUIManager.manager.SwitchToCorrectView();

        }

        if (PhotonNetwork.IsMasterClient) return; // Don't sync player stats yet because game is not ready

        Player.GetLocalPlayer().SyncPlayerStats();

    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Player.GetLocalPlayer().SyncPlayerStats();
        // if (PhotonNetwork.IsMasterClient && MainMenuUIManager.manager && MainMenuUIManager.manager.GetLoadedOwner() == newPlayer.UserId)
        // {
        //     PhotonNetwork.SetMasterClient(newPlayer);
        //     return;
        // }

        if (PhotonNetwork.IsMasterClient)
        {
            Game.currentGame.SyncGameProperties();
        }
    }


    public override void OnLeftRoom()
    {
        ResetPlayerProperties();
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            SceneManager.LoadScene("MainMenu");
        }
        else if (MainMenuUIManager.manager != null)
        {
            MainMenuUIManager.manager.SwitchToCorrectView();
        }

        if (Game.currentGame.gameCreator == GameConstants.username)
        {
            Lobby.gameservice.DeleteSession(Game.currentGame.gameId); // Deletes session if launched
            Lobby.user.DeleteSession(Game.currentGame.gameId); // Deletes session if not launched
        }

        // Leave Game chat
        ChatManager.manager.LeaveGroup();
    }

    #endregion


}
