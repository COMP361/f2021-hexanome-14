using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class MainMenuUIManager : MonoBehaviour, GameSessionsReceivedInterface, OnGameSessionClickedHandler
{
    [SerializeField] private GameObject availableGamesView;
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private GameObject gameSelectView;
    [SerializeField] private GameObject homeView;

    [SerializeField] private GameObject gameOptionButtonsView;
    [SerializeField] private GameObject gameCreatorOptionsView;
    [SerializeField] private GameObject gameJoinedOptionsView;

    [SerializeField] private Dropdown variationDD;
    [SerializeField] private Dropdown numRounds;
    private List<int> numRoundOptions = new List<int> { 3, 4, 5 };


    private Lobby.GameSession currentSelectedSession;

    public void Update()
    {
    }

    public async void Start()
    {
        await Lobby.LongPollForUpdates(this);
    }

    public void OnStartClicked()
    {
        //connectionStatusText.gameObject.SetActive(true);
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        OnUpdatedGameListReceived(Lobby.availableGames);
        InGameSelectView();
    }

    public void InGameSelectView()
    {
        if (GameConstants.networkManager.inGameMaster())
        {
            gameCreatorOptionsView.SetActive(true);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(false);
            SetGameActive(false);
        } else if (GameConstants.networkManager.inGame())
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(true);
            SetGameActive(false);
        } else
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(true);
            gameJoinedOptionsView.SetActive(false);
            SetGameActive(true);
        }
    }


    public void OnStartGameClicked()
    {
        foreach (Player p in Player.GetAllPlayers())
        {
            p.Reset();
	    }
        // Debug.LogError($"num rounds: {numRounds.value}");
        Game.currentGame.Init(numRoundOptions[numRounds.value], variationDD.options[variationDD.value].text);
        GameConstants.networkManager.LoadArena();
    }

    public async void OnJoinGameClicked()
    {
        if (currentSelectedSession != null)
        {
            Debug.Log($"Attempting to join Game {currentSelectedSession.session_ID} as user {Lobby.myUsername}");
            await Lobby.JoinSession(currentSelectedSession.session_ID);
            GameConstants.networkManager.JoinRoom(currentSelectedSession.session_ID);
            //GetComponent<PhotonView>().RPC(nameof(RPC_ListUpdated), RpcTarget.AllBuffered, new object[] { });
        }

    }

    public async void OnCreateGameClicked()
    {
        gameOptionButtonsView.SetActive(false);
        gameCreatorOptionsView.SetActive(true);

        await Lobby.CreateSession();

        //GetComponent<PhotonView>().RPC(nameof(RPC_ListUpdated), RpcTarget.AllBuffered, new object[] { });
    }

    public void OnLeaveGameClicked()
    {
        GameConstants.networkManager.LeaveRoom();
        InGameSelectView();
    }

    public void OnDeleteGameClicked()
    {
        GameConstants.networkManager.LeaveRoom();
        InGameSelectView();
    }

    public void OnUpdatedGameListReceived(List<Lobby.GameSession> gameSessions)
    {
        Debug.Log($"OnUpdatedGameListReceived with {gameSessions.Count} games");
        RemoveAllGameSessions();
        foreach (Lobby.GameSession game in gameSessions)
        {
            AddGameSession(game);

            if (game.createdBy == Lobby.myUsername && !GameConstants.networkManager.inGame())
            {
                GameConstants.networkManager.CreateRoom(game.session_ID);
            }
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

    private void SetGameActive(bool active)
    {
        foreach (GameSessionListItemScript sessionScript in availableGamesView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            if (active)
            {
                sessionScript.activate();
            }else
            {
                sessionScript.deactivate();
            }
        }
    }

    private void AddGameSession(Lobby.GameSession gameSession)
    {
        GameObject newSession = Instantiate(sessionPrefab, availableGamesView.transform);

        GameSessionListItemScript sessionScript = newSession.GetComponent<GameSessionListItemScript>();
        sessionScript.SetFields(gameSession);
        sessionScript.SetOnGameSessionClickedHandler(this);
    }

    public void OnEscapePressed()
    {
        gameSelectView.gameObject.SetActive(false);
        homeView.gameObject.SetActive(true);
        GameConstants.networkManager.LeaveRoom();
    }


    private void resetColors()
    {
        foreach (GameSessionListItemScript sessionScript in availableGamesView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            sessionScript.SetToDefaultColor();
        }
    }

    public void OnGameSessionClicked(Lobby.GameSession gameSession)
    {
        resetColors();
        currentSelectedSession = gameSession;
    }
}
