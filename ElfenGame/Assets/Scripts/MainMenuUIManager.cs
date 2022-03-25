using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using System;

public class MainMenuUIManager : MonoBehaviour, OnGameSessionClickedHandler
{
    #region singleton 

    private static MainMenuUIManager _instance;

    public static MainMenuUIManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainMenuUIManager>();
            }
            return _instance;
        }
    }

    #endregion   

    [SerializeField] private GameObject availableGamesView;
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private GameObject savedGamePrefab;

    [SerializeField] private Text gameSessionsText;


    [Header("Views")]
    [SerializeField] private GameObject gameSelectView;
    [SerializeField] private GameObject homeView;

    [SerializeField] private GameObject gameOptionButtonsView;
    [SerializeField] private GameObject gameCreatorOptionsView;
    [SerializeField] private GameObject gameJoinedOptionsView;
    [SerializeField] private GameObject gameCreationMenu;
    [SerializeField] private GameObject gameLoadOptionsView;

    [Header("Create Game UI")]

    [SerializeField] private Dropdown gameModeDD;
    [SerializeField] private Dropdown variationDD;
    [SerializeField] private Dropdown numRounds;
    [SerializeField] private Button endTownButton;
    [SerializeField] private Button witchButton;
    [SerializeField] private Button randGoldButton;
    private List<int> numRoundOptions = new List<int> { 3, 4, 5 };


    private Lobby.GameSession currentSelectedSession;
    private string savedGameID = "";

    public bool inLoadGameView = false;


    /// <summary>
    /// Called when MainMenuUIManager is created
    /// </summary>
    public void Start()
    {
        Game.currentGame = new Game();
        Lobby.user.GetSavedGames();
    }


    public string GetLoadedOwner()
    {
        return Game.currentGame.gameCreator;
    }

    /// <summary>
    /// Called when Start Button is clicked
    /// </summary>
    public void OnStartClicked()
    {
        Player.ResetPlayers();
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        UpdateSessionListView(Lobby.availableGames);
        InGameSelectView();
    }

    public void InGameSelectView()
    {
        if (NetworkManager.manager.inGame() && GetLoadedOwner() == GameConstants.username)
        {
            gameCreatorOptionsView.SetActive(true);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(false);
            gameLoadOptionsView.SetActive(false);
            SetGameActive(false);
        }
        else if (NetworkManager.manager.inGame())
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(true);
            gameLoadOptionsView.SetActive(false);
            SetGameActive(false);
        }
        else if (inLoadGameView)
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(false);
            gameJoinedOptionsView.SetActive(false);
            gameLoadOptionsView.SetActive(true);
            SetGameActive(false);
            UpdateSavedGameListView(Lobby.user.savedGames);
        }
        else
        {
            gameCreatorOptionsView.SetActive(false);
            gameOptionButtonsView.SetActive(true);
            gameJoinedOptionsView.SetActive(false);
            gameLoadOptionsView.SetActive(false);
            SetGameActive(true);
            UpdateSessionListView(Lobby.availableGames);
        }
    }

    public void OnGameModeChange()
    {
        witchButton.gameObject.SetActive((gameModeDD.options[gameModeDD.value].text == "Elfengold"));
        randGoldButton.gameObject.SetActive((gameModeDD.options[gameModeDD.value].text == "Elfengold"));
    }

    public void OnLoadGameViewClicked()
    {
        inLoadGameView = true;
        InGameSelectView();
        gameSessionsText.text = "Saved Games:";
    }

    public void OnReturnToLobbyViewClicked()
    {
        inLoadGameView = false;
        InGameSelectView();
        gameSessionsText.text = "Game Sessions:";
    }

    public void OnLoadSavedGameClicked()
    {
        //TODO: load game
        Debug.Log("Load Saved Game Clicked");
    }

    public void OnDeleteSavedGameClicked()
    {
        //TODO: delete game
        Debug.Log("Delete Saved Game Clicked");
    }

    public void OnStartGameClicked()
    {
        //Debug.LogError($"num rounds: {numRoundOptions[numRounds.value]}");
        //Debug.LogError($"variation: {variationDD.options[variationDD.value].text}");
        Lobby.user.LaunchSession(Game.currentGame.gameId);
    }

    public void OnJoinGameClicked()
    {
        if (currentSelectedSession != null)
        {
            Debug.Log($"Attempting to join Game {currentSelectedSession.session_ID} as user {GameConstants.username}");
            Lobby.user.JoinSession(currentSelectedSession.session_ID);
            Game.currentGame.SetSession(currentSelectedSession.createdBy, currentSelectedSession.session_ID);

            NetworkManager.manager.JoinOrCreateRoom(currentSelectedSession.session_ID);
        }
    }

    public void OnCreateGameClicked()
    {
        gameOptionButtonsView.SetActive(false);
        gameCreationMenu.SetActive(true);

    }

    public void OnCancelCreateClicked()
    {
        gameOptionButtonsView.SetActive(true);
        gameCreationMenu.SetActive(false);
    }

    public void OnConfirmCreateClicked()
    {

        gameCreationMenu.SetActive(false);
        gameCreatorOptionsView.SetActive(true);

        Lobby.user.CreateSession();
    }

    public void OnLeaveGameClicked()
    {
        NetworkManager.manager.LeaveRoom();
        Lobby.user.LeaveSession(Game.currentGame.gameId);
        InGameSelectView();
    }

    public void OnDeleteGameClicked()
    {
        Debug.Log("Delete Game!!!!!");
        Lobby.user.DeleteSession(Game.currentGame.gameId);
        NetworkManager.manager.LeaveRoom();
        currentSelectedSession = null;
        InGameSelectView();
    }

    public void UpdateSessionListView(List<Lobby.GameSession> gameSessions)
    {
        RemoveAllGameSessions();
        foreach (Lobby.GameSession game in gameSessions)
        {
            AddGameSession(game);
        }
    }

    public void UpdateSavedGameListView(List<Lobby.SavedGame> savedGames)
    {
        RemoveAllGameSessions();
        foreach (Lobby.SavedGame game in savedGames)
        {
            AddSavedGame(game);
        }
    }

    internal void CreateGameWithOptions()
    {
        Game.currentGame.Init(
            numRoundOptions[numRounds.value],
            gameModeDD.options[gameModeDD.value].text,
            endTownButton.GetComponent<VariationButton>().isSelected,
            witchButton.GetComponent<VariationButton>().isSelected,
            randGoldButton.GetComponent<VariationButton>().isSelected
        );
        Lobby.gameservice.PutSavedGame(Game.currentGame.gameId, Game.currentGame.mPlayers);
        NetworkManager.manager.LoadArena();
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
        NetworkManager.manager.LeaveRoom();
    }

    public void OnGameSessionClicked(Lobby.GameSession gameSession)
    {
        foreach (GameSessionListItemScript sessionScript in availableGamesView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            sessionScript.SetToDefaultColor();
        }
        currentSelectedSession = gameSession;
    }

    private void AddSavedGame(Lobby.SavedGame savedGame)
    {
        GameObject savedGameObject = Instantiate(savedGamePrefab, availableGamesView.transform);

        SavedGameListItemScript sessionScript = savedGameObject.GetComponent<SavedGameListItemScript>();
        sessionScript.SetFields(savedGame.savegameid, savedGame.players);
    }

    internal void LoadGameItemSelected(string saveid)
    {
        foreach (SavedGameListItemScript savedScript in availableGamesView.GetComponentsInChildren<SavedGameListItemScript>())
        {
            savedScript.SetToDefaultColor();
        }

        savedGameID = saveid;
    }


    internal void OnGameCreated(string sessionID)
    {
        Debug.Log($"Game created with ID {sessionID}");
        Game.currentGame.SetSession(GameConstants.username, sessionID);
        NetworkManager.manager.JoinOrCreateRoom(sessionID);
    }
}
