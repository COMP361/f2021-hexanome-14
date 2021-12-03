using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MainMenuUIManager : MonoBehaviour, GameSessionsReceivedInterface, OnGameSessionClickedHandler
{
    [SerializeField] private TextMeshProUGUI connectionStatusText;
    [SerializeField] private NetworkManager networkManager;
    [SerializeField] private GameObject availableGamesView;
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private GameObject gameSelectView;
    [SerializeField] private GameObject homeView;

    [SerializeField] private GameObject gameOptionButtonsView;
    [SerializeField] private GameObject gameCreatorOptionsView;


    private Lobby.GameSession currentSelectedSession;

    public async void OnStartClicked()
    {
        connectionStatusText.gameObject.SetActive(true);
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        networkManager.Connect();
        await Lobby.GetSessions(this);
    }

    public void OnStartGameClicked()
    {
        networkManager.LoadArena();
    }

    public async void OnJoinGameClicked()
    {
        if (currentSelectedSession != null)
        {
            Debug.Log($"Attempting to join Game {currentSelectedSession.session_ID} as user {Lobby.myUsername}");
            await Lobby.JoinSession(currentSelectedSession.session_ID);
            GetComponent<PhotonView>().RPC(nameof(RPC_ListUpdated), RpcTarget.AllBuffered, new object[] { });
        }


    }


    public async void OnCreateGameClicked()
    {
        gameOptionButtonsView.SetActive(false);
        gameCreatorOptionsView.SetActive(true);

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
        sessionScript.SetFields(gameSession);
        sessionScript.SetOnGameSessionClickedHandler(this);
    }

    //


    [PunRPC]
    public async void RPC_ListUpdated()
    {
        await Lobby.GetSessions(this);
    }

    private void resetColors()
    {
        foreach (Image image in availableGamesView.GetComponentsInChildren<Image>())
        {
            image.color = new Color(238f / 255f, 100f / 255f, 100f / 255f, 74f / 255f);
        }
    }

    public void OnGameSessionClicked(Lobby.GameSession gameSession)
    {
        resetColors();
        currentSelectedSession = gameSession;
    }
}
