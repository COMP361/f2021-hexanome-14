using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    const byte SPAWN_PLAYER_CODE = 12;

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = Lobby.myUsername;
            _ = PhotonNetwork.ConnectUsingSettings();
            PhotonPeer.RegisterType(typeof(CardEnum), 255, CardEnumExtension.Serialize, CardEnumExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(MovementTile), 252, MovementTileExtension.Serialize, MovementTileExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(PlayerColor), 254, PlayerColorExtension.Serialize, PlayerColorExtension.Deserialize);
            PhotonPeer.RegisterType(typeof(GamePhase), 253, GamePhaseExtension.Serialize, GamePhaseExtension.Deserialize);
            networkPlayers = new Dictionary<string, Photon.Realtime.Player>();
        }
    }
   
     
    private Dictionary<string, Photon.Realtime.Player> networkPlayers;
    private Photon.Realtime.Player GetPlayer(string playerId)
    { 
        if (!networkPlayers.ContainsKey(playerId))
        { 
	        foreach (Photon.Realtime.Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                networkPlayers[p.UserId] = p;
	        }
	    }

        return networkPlayers[playerId];
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



    public void JoinRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        }
    }

    public List<Photon.Realtime.Player> GetPlayers()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            List<Photon.Realtime.Player> players = new List<Photon.Realtime.Player>();

            foreach(Photon.Realtime.Player p in PhotonNetwork.CurrentRoom.Players.Values)
            {
                players.Add(p);
            }
            return players;
        }
        return null;
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
        if (otherPlayer != PhotonNetwork.LocalPlayer)
        {
            LeaveRoom();
        }

    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        Debug.Log("Room properties updated");
        if (Game.currentGame != null)
        { 
	        foreach (DictionaryEntry entry in propertiesThatChanged)
            {
                Game.currentGame.UpdateProperties((string)entry.Key, entry.Value);
	        }
	    }
    }

    public void SetGameProperty(object key, object value)
    {
        ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add(key, value);
        _ = PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
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
        SetPlayerPropertyByPlayerName(Lobby.myUsername, key, value);
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

        Debug.Log($"{targetPlayer.UserId} properties updated");
        Player pm = Player.GetOrCreatePlayer(targetPlayer.UserId);
        if (pm != null)
        {
            foreach (DictionaryEntry entry in changedProps)
            {
                pm.updatePropertiesCallback((string) entry.Key, entry.Value);
            }
        }
    }


    //public void SpawnPlayer(string username, PhotonView photonView)
    //{
    //    if (PhotonNetwork.AllocateRoomViewID(photonView))
    //    {
    //        object[] data = new object[] { username, photonView.ViewID };

    //        RaiseEventOptions raiseEventOptions = new RaiseEventOptions
    //        {
    //            Receivers = ReceiverGroup.Others,
    //            CachingOption = EventCaching.AddToRoomCache
    //        };

    //        SendOptions sendOptions = new SendOptions
    //        {
    //            Reliability = true
    //        };

    //        PhotonNetwork.RaiseEvent(SPAWN_PLAYER_CODE, data, raiseEventOptions, sendOptions);

    //    }
    //    else
    //    {
    //        Debug.LogError("Failed to allocate ViewId");
    //    }
    //}

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

    public bool IsMasterClient()
    {
        return PhotonNetwork.IsMasterClient;
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log($"Connected to server.");
    }

    //public override void OnJoinRandomFailed(short returnCode, string message)
    //{
    //    Debug.Log($"Joining random room failed because of {message}. Creating a new one.");
    //    PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6, IsVisible = true });
    //}


    public void CreateRoom(string roomName)
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 6, IsVisible = true, PublishUserId = true });
        }
    }

    //public override void OnRoomListUpdate(List<RoomInfo> roomList)
    //{
    //    List<string> roomNames = new List<string>();

    //    foreach (RoomInfo roomInfo in roomList)
    //    {
    //        roomNames.Add(roomInfo.Name);
    //    }

    //    uiManager.UpdateAvailableRoomList(roomNames);
    //}

    //public void OnEvent(EventData photonEvent)
    //{
    //    if (photonEvent.Code == SPAWN_PLAYER_CODE && GameConstants.mainUIManager)
    //    {
    //        object[] data = (object[])photonEvent.CustomData;
    //        string username = (string)data[0];
    //        int viewId = (int)data[1];

    //        GameConstants.mainUIManager.InitPlayer(username, viewId);
	   // }
    //}

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }


    public override void OnJoinedRoom()
    {
        //Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room");

        if (GameConstants.mainMenuUIManager != null)
        {
            GameConstants.mainMenuUIManager.InGameSelectView();
        }

        _ = Player.GetOrCreatePlayer(Lobby.myUsername);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room.");

        Player.GetOrCreatePlayer(newPlayer.UserId);

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("OnPlayerEnteredRoom IsMasterClient");

            //LoadArena();
        }

    }


    public override void OnLeftRoom()
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Main"))
        {
            SceneManager.LoadScene("MainMenu");
        } else if (GameConstants.mainMenuUIManager != null)
        {
            GameConstants.mainMenuUIManager.InGameSelectView();
        }
    }

    #endregion
}
