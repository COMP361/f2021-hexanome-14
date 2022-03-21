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

    private Lobby.GameSession loadedSession;

    public void Update()
    {
    }

    /// <summary>
    /// Called when MainMenuUIManager is created
    /// </summary>
    public async void Start()
    {
        await Lobby.LongPollForUpdates(this);

        Game.currentGame = new Game();
    }


    public string GetLoadedOwner()
    {
        return loadedSession.createdBy;
    }

    /// <summary>
    /// Called when Start Button is clicked
    /// </summary>
    public void OnStartClicked()
    {
        Player.ResetPlayers();
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        OnUpdatedGameListReceived(Lobby.availableGames);
        InGameSelectView();
        // if (GameConstants.networkManager.isConnected())
        // {
        //     GameConstants.networkManager.ResetPlayerProperties();
        // }
    }

    public void InGameSelectView()
    {
        if (GameConstants.networkManager.inGame() && GetLoadedOwner() == Lobby.myUsername)
        {
            gameCreatorOptionsView.SetActive(true);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(false);
            SetGameActive(false);
        }
        else if (GameConstants.networkManager.inGame())
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(true);
            SetGameActive(false);
        }
        else
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(true);
            gameJoinedOptionsView.SetActive(false);
            SetGameActive(true);
        }
    }


    public void OnStartGameClicked()
    {
        //Debug.LogError($"num rounds: {numRoundOptions[numRounds.value]}");
        //Debug.LogError($"variation: {variationDD.options[variationDD.value].text}");
        Game.currentGame.Init(numRoundOptions[numRounds.value], variationDD.options[variationDD.value].text, loadedSession.session_ID, GameConstants.playfabManager.GetGroupId());
        GameConstants.networkManager.LoadArena();
    }

    public async void OnJoinGameClicked()
    {
        if (currentSelectedSession != null)
        {
            Debug.Log($"Attempting to join Game {currentSelectedSession.session_ID} as user {Lobby.myUsername}");
            await Lobby.JoinSession(currentSelectedSession.session_ID);
            loadedSession = currentSelectedSession;

            // if (GameConstants.playfabManager)
            // {
            //     GameConstants.playfabManager.CheckInGroup(loadedSession.saveID);
            // }
            GameConstants.networkManager.JoinOrCreateRoom(currentSelectedSession.session_ID);
        }
    }

    public async void OnCreateGameClicked()
    {
        await Lobby.CreateSession(savegameID: "");
    }

    public void OnLeaveGameClicked()
    {
        GameConstants.networkManager.LeaveRoom();
        _ = Lobby.LeaveSession(currentSelectedSession.session_ID);
        InGameSelectView();
    }

    public void OnDeleteGameClicked()
    {
        Debug.Log("Delete Game!!!!!");
        _ = Lobby.DeleteSession(currentSelectedSession.session_ID);
        GameConstants.networkManager.LeaveRoom();
        currentSelectedSession = null;
        InGameSelectView();
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

    private void SetGameActive(bool active)
    {
        foreach (GameSessionListItemScript sessionScript in availableGamesView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            if (active)
            {
                sessionScript.activate();
            }
            else
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
