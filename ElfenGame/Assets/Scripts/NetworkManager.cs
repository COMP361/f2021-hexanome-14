using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    public void Connect()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AuthValues = new AuthenticationValues();
            PhotonNetwork.AuthValues.UserId = Lobby.myUsername;
            PhotonNetwork.ConnectUsingSettings();
        }
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
            PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 6, IsVisible = true });
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
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log($"Player {newPlayer.ActorNumber} entered the room.");

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
