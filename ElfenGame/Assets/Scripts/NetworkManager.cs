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

    private void Update()
    {
        uiManager.SetConnectionStatus(PhotonNetwork.NetworkClientState.ToString());
    }

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.LogError($"Connected to server. Looking for random room");
        PhotonNetwork.JoinRandomRoom();
    }


    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.LogError($"Joining random room failed because of {message}. Creating a new one.");
        PhotonNetwork.CreateRoom(null);
    }


    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
    }


    public override void OnJoinedRoom()
    {
        Debug.LogError($"Player {PhotonNetwork.LocalPlayer.ActorNumber} joined the room");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.LogError($"Player {newPlayer.ActorNumber} entered the room.");

    }

    #endregion
}
