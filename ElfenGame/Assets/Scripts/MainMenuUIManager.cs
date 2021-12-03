using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MainMenuUIManager : MonoBehaviour, GameSessionsReceivedInterface
{
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject availableGamesView;
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private GameObject gameSelectView;
    [SerializeField] private GameObject homeView;


    public void OnStartClicked()
    {
        connectionStatusText.gameObject.SetActive(true);
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        networkManager.Connect();
        ForceUpdateList();
    }


    public async void OnCreateGameClicked()
    {
        await Lobby.CreateSession();
        GetComponent<PhotonView>().RPC(nameof(RPC_ListUpdated), RpcTarget.AllBuffered, new object[] { });
    }

    public void SetConnectionStatus(string status)
    {
        connectionStatusText.text = status;
    }

    public void OnUpdatedGameListReceived(List<Lobby.GameSession> gameSessions)
    {
        Debug.Log($"OnUpdatedGameListReceived with {gameSessions.Count} games");
        RemoveAllGameSessions();
        foreach (Lobby.GameSession game in gameSessions)
        {
            AddGameSession(game);
        }

    }

    public void ForceUpdateList()
    {
        OnUpdatedGameListReceived(Lobby.availableGames);
    }

    private void RemoveAllGameSessions()
    {
        foreach (Transform child in availableGamesView.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    private void AddGameSession(Lobby.GameSession gameSession)
    {
        GameObject newSession = Instantiate(sessionPrefab, availableGamesView.transform);

        GameSessionListItemScript sessionScript = newSession.GetComponent<GameSessionListItemScript>();
        sessionScript.SetFields(gameSession.createdBy, gameSession.players.Count);
    }

    //


    [PunRPC]
    public async void RPC_ListUpdated()
    {
        await Lobby.GetSessions(this);
    }
}
