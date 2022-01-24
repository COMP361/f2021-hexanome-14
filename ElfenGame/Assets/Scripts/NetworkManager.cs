using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    [SerializeField] MainMenuUIManager uiManager;

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void ConnectToRoom(string roomName)
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRoom(roomName);
        } else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void Update()
    {
        if (uiManager != null)
            uiManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    public void LoadArena()
    {
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
        Debug.Log($"Connected to server. Looking for random room");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"Joining random room failed because of {message}. Creating a new one.");
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 6, IsVisible = true });
    }


    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions { MaxPlayers = 6, IsVisible = true });
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
        //SceneManager.LoadScene("MainMenu");

    }

    #endregion
}
