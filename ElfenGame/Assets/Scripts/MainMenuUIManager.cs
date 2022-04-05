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

    #region Serialized Fields
    [SerializeField] private GameObject sessionListView, savedGameListView;
    [SerializeField] private GameObject sessionPrefab;
    [SerializeField] private GameObject savedGamePrefab;

    [SerializeField] private Text gameSessionsText;
    [SerializeField] private Text statusText;

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
    [SerializeField] private Button tradingButton;

    #endregion
    private List<int> numRoundOptions = new List<int> { 3, 4, 5 };
    private string selectedSaveId = "";

    private string selectedSessionId = "";
    public bool inLoadGameView = false;

    public void Start()
    {
    }

    public string GetLoadedOwner()
    {
        if (selectedSessionId == "" || !Lobby.activeGames.ContainsKey(selectedSessionId))
        {
            return "";
        }
        return Lobby.activeGames[selectedSessionId].createdBy;
    }

    #region UI Click Handlers
    /// <summary>
    /// Called when Start Button is clicked
    /// </summary>
    public void OnStartClicked()
    {
        Player.ResetPlayers(); // TODO: Move this elsewhere
        gameSelectView.gameObject.SetActive(true);
        homeView.gameObject.SetActive(false);
        UpdateSessionListView(Lobby.availableSessions);
        SwitchToCorrectView();
    }

    /// <summary>
    /// Called when the Load Game button is clicked.
    /// Switches to the load game view and display all found saved games
    /// </summary>
    public void OnLoadGameViewClicked()
    {
        SaveAndLoad.UpdateLocalSavedIds();
        Lobby.user.GetSavedGames();
        inLoadGameView = true;
        gameSessionsText.text = "Saved Games:";
        SwitchToCorrectView();
    }

    /// <summary>
    /// Called when the Return to Lobby button is clicked.
    /// Switches to the home view and switches the saved game list with available sessions list
    /// </summary>
    public void OnReturnToLobbyViewClicked()
    {
        inLoadGameView = false;
        gameSessionsText.text = "Game Sessions:";
        SwitchToCorrectView();
    }

    /// <summary>
    /// Called when the Load Game button (in the load game view) is clicked.
    /// If the selected saved game has already been loaded, joins the game.
    /// If the selected saved game has not been loaded, creates and new session and loads the game.
    /// </summary>
    public void OnLoadSavedGameClicked()
    {
        Debug.Log("Load Saved Game Clicked");
        if (selectedSaveId == "") return;
        if (Lobby.activeSavedGames.ContainsKey(selectedSaveId))
        {
            Lobby.GameSession session = Lobby.activeSavedGames[selectedSaveId];
            Debug.Log("Session for saved game found");
            Lobby.user.JoinSession(session.session_ID);
        }
        else
        {
            Lobby.user.CreateSession(selectedSaveId);
        }
        OnReturnToLobbyViewClicked();
    }

    /// <summary>
    /// Called when the Delete Saved Game button (in the load game view) is clicked.
    /// Deletes the selected saved game from the lobby's saved games list
    /// </summary>
    public void OnDeleteSavedGameClicked()
    {
        if (selectedSaveId == "") return;
        Lobby.gameservice.DeleteSavedGame(selectedSaveId);
        selectedSaveId = "";
    }

    /// <summary>
    /// Called when the Start Game button (in the game creator options view) is clicked.
    /// Attempts to launch game session
    /// </summary>
    public void OnStartGameClicked()
    {
        Lobby.user.LaunchSession(selectedSessionId);
    }

    /// <summary>
    /// Called when the Join Game button (in the game options view) is clicked.
    /// Attempts to join the selected game session
    /// if the selected session is from a loaded game, attempts to load the player's state for the game
    /// </summary>
    public void OnJoinSessionClicked()
    {
        if (selectedSessionId == "") return;

        Debug.Log($"Attempting to join Game {selectedSessionId} as user {GameConstants.username}");
        Lobby.user.JoinSession(selectedSessionId);
    }

    /// <summary>
    /// Called when the Create Game button (in the game options view) is clicked.
    /// Switches to the game creator options view
    /// </summary>
    public void OnCreateSessionClicked()
    {
        gameOptionButtonsView.SetActive(false);
        gameCreationMenu.SetActive(true);

    }

    /// <summary>
    /// Called when the cancel create game button (in the game creator options view) is clicked.
    /// Switches to the game options view
    /// </summary>
    public void OnCancelCreateClicked()
    {
        gameOptionButtonsView.SetActive(true);
        gameCreationMenu.SetActive(false);
    }

    /// <summary>
    /// Called when the confirm create game button (in the game creator options view) is clicked.
    /// Attempts to create a new game session
    /// </summary>
    public void OnConfirmCreateClicked()
    {

        gameCreationMenu.SetActive(false);
        gameCreatorOptionsView.SetActive(true);

        Lobby.user.CreateSession();
    }

    /// <summary>
    /// Called when the leave game button (in the game joined options view) is clicked.
    /// Attempts to leave the current game session
    /// </summary>
    public void OnLeaveGameClicked()
    {
        Lobby.user.LeaveSession(selectedSessionId);
        NetworkManager.manager.LeaveRoom();
        selectedSessionId = "";
        SwitchToCorrectView();
    }

    /// <summary>
    /// Called when the Delete Game button (in the game creator options view) is clicked.
    /// Attempts to delete the current game session
    /// </summary>
    public void OnDeleteGameClicked()
    {
        Debug.Log("Delete Game!!!!!");
        Lobby.user.DeleteSession(selectedSessionId);
        NetworkManager.manager.LeaveRoom();
        selectedSessionId = "";
        SwitchToCorrectView();
    }


    #endregion

    #region List View Management

    /// <summary>
    /// Updates the game session list view with the given list of sessions
    /// </summary>
    /// <param name="gameSessions">List of sessions to display</param>
    public void UpdateSessionListView(List<Lobby.GameSession> gameSessions)
    {
        DestroyAllChildren(sessionListView.transform);
        foreach (Lobby.GameSession game in gameSessions)
        {
            AddGameSession(game);
        }
    }

    /// <summary>
    /// Updates the saved game list view with the given list of saved games
    /// </summary>
    /// <param name="savedGames">List of saved games to display</param>
    public void UpdateSavedGameListView(List<Lobby.SavedGame> savedGames)
    {
        DestroyAllChildren(savedGameListView.transform);
        foreach (Lobby.SavedGame game in savedGames)
        {
            AddSavedGame(game);
        }
    }

    /// <summary>
    /// Removes all game sessions/saved games from the list view
    /// </summary>
    /// <param name="transform">Transform of the list view</param>
    private void DestroyAllChildren(Transform transform)
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

    }

    /// <summary>
    /// Sets the session list items to be interactable or not
    /// </summary>
    /// <param name="active">Whether the items should be interactable or not</param>
    private void SetSessionClickEnabled(bool active)
    {
        foreach (GameSessionListItemScript sessionScript in sessionListView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            if (active)
            {
                sessionScript.activate();
            }
            else
            {
                sessionScript.deactivate();
                if (sessionScript.gameSession.session_ID != selectedSessionId)
                {
                    sessionScript.SetUnselectedColor();
                }
            }
        }
    }

    /// <summary>
    /// Instantiates a new game session list item and adds it to the list view
    /// </summary>
    /// <param name="gameSession">Session to add to the list</param>
    private void AddGameSession(Lobby.GameSession gameSession)
    {
        GameObject newSession = Instantiate(sessionPrefab, sessionListView.transform);

        GameSessionListItemScript sessionScript = newSession.GetComponent<GameSessionListItemScript>();
        sessionScript.SetFields(gameSession);
        sessionScript.SetOnGameSessionClickedHandler(this);
        if (selectedSessionId == gameSession.session_ID)
        {
            sessionScript.SetSelectedColor();
        }
        else
        {
            sessionScript.SetToDefaultColor();
        }

    }

    /// <summary>
    /// Adds a new saved game to the list view
    /// </summary>
    private void AddSavedGame(Lobby.SavedGame savedGame)
    {
        GameObject savedGameObject = Instantiate(savedGamePrefab, savedGameListView.transform);

        SavedGameListItemScript sessionScript = savedGameObject.GetComponent<SavedGameListItemScript>();
        sessionScript.SetFields(savedGame.savegameid, savedGame.players);
    }

    /// <summary>
    /// Called when an item is clicked on in the list of saved games
    /// </summary>
    internal void LoadGameItemSelected(string saveid)
    {
        foreach (SavedGameListItemScript savedScript in savedGameListView.GetComponentsInChildren<SavedGameListItemScript>())
        {
            savedScript.SetToDefaultColor();
        }

        selectedSaveId = saveid;
    }

    /// <summary>
    /// Called when one of the gameSessions in the list view is clicked
    /// Resets the color of all game session list items and sets the clicked item as selected
    /// </summary>
    /// <param name="gameSession">Session that was clicked</param>
    public void OnGameSessionClicked(Lobby.GameSession gameSession)
    {
        foreach (GameSessionListItemScript sessionScript in sessionListView.GetComponentsInChildren<GameSessionListItemScript>())
        {
            sessionScript.SetToDefaultColor();
        }
        selectedSessionId = gameSession.session_ID;
    }

    #endregion

    /// <summary>
    /// Determines which menu view to display and activates it
    /// </summary>
    public void SwitchToCorrectView()
    {
        gameCreatorOptionsView.SetActive(false);
        gameJoinedOptionsView.SetActive(false);
        gameLoadOptionsView.SetActive(false);
        gameOptionButtonsView.SetActive(false);
        SetSessionClickEnabled(false);
        sessionListView.SetActive(false);
        savedGameListView.SetActive(false);
        bool loadedOwner = false;
        if (selectedSessionId != "" && Lobby.activeGames.ContainsKey(selectedSessionId))
        {
            Lobby.GameSession session = Lobby.activeGames[selectedSessionId];
            loadedOwner = session.createdBy == GameConstants.username;
        }
        if (NetworkManager.manager.inGame() && loadedOwner)
        {
            // If in game and you are the game creator, show the game creator options (start game, delete game)
            gameCreatorOptionsView.SetActive(true);
            sessionListView.SetActive(true);
        }
        else if (NetworkManager.manager.inGame())
        {
            // If in game and you are not the game creator, show the game joined options (leave game)
            gameJoinedOptionsView.SetActive(true);
            sessionListView.SetActive(true);
        }
        else if (inLoadGameView)
        {
            // If in load game view, show the game load options (load game, delete saved game, return to lobby)
            selectedSaveId = ""; // Currently selected saved game
            gameLoadOptionsView.SetActive(true);
            savedGameListView.SetActive(true);

        }
        else
        {
            // If not in game && not in load game view, show the game options (create game, join game, load game)
            selectedSessionId = ""; //TODO: This might clear the session on session list updates
            gameOptionButtonsView.SetActive(true);
            sessionListView.SetActive(true);
            SetSessionClickEnabled(true);
        }
    }

    /// <summary>
    /// Enables/Disables the Witch/Gold buttons in the game creation menu
    /// </summary>
    public void OnGameModeChange()
    {
        witchButton.gameObject.SetActive((gameModeDD.options[gameModeDD.value].text == "Elfengold"));
        randGoldButton.gameObject.SetActive((gameModeDD.options[gameModeDD.value].text == "Elfengold"));
    }

    /// <summary>
    /// Called when escape key pressed
    /// Leaves Room (if in one) and returns to main menu
    /// </summary>
    public void OnEscapePressed()
    {
        gameSelectView.gameObject.SetActive(false);
        homeView.gameObject.SetActive(true);
        NetworkManager.manager.LeaveRoom();
        // FIXME: Update so that it leaves lobby session and handles deleting lobby session
    }

    public void Update()
    {
        // For debugging purposes
        // statusText.text = NetworkManager.manager.IsMasterClient().ToString();
    }

    #region Callbacks

    public void OnSessionJoined(Lobby.GameSession session)
    {
        //TODO: Summary
        Game.currentGame = new Game();
        if (session.saveID != "")
        {
            SaveAndLoad.LoadLocalPlayerState(session.saveID);
        }

        NetworkManager.manager.JoinOrCreateRoom(session.session_ID);
        UpdateSessionListView(Lobby.availableSessions);
    }

    /// <summary>
    /// Called once the Lobby has sucessfully launched the current session
    /// Handles some Game setup and tells NetworkManager to load Main Scene
    /// </summary>
    public void OnSessionLaunched()
    {
        Debug.Assert(NetworkManager.manager.inGame());
        Game.currentGame.InitPlayersList();
        Game.currentGame.SyncGameProperties();
        Lobby.gameservice.PutSavedGame(Game.currentGame.saveId, Game.currentGame.mPlayers);
        NetworkManager.manager.LoadArena();
    }


    /// <summary>
    /// Callback from Lobby Session Created
    /// </summary>
    internal void OnSessionCreated(string sessionID, string saveId)
    {
        selectedSessionId = sessionID;
        Debug.Log($"Game created with ID {sessionID}");
        NetworkManager.manager.CreateRoom(sessionID);
    }

    /// <summary>
    /// Called from Network Manager when a room is joined by its session's creator
    /// Should only be called on a single client
    /// </summary>
    internal void OnRoomCreated(string sessionID)
    {
        Debug.Log($"OnRoomCreated: {sessionID}");
        Lobby.GameSession session = Lobby.activeGames[sessionID];
        if (session.saveID == "")
        {
            // Game.currentGame.Init(
            //     numRoundOptions[numRounds.value],
            //     gameModeDD.options[gameModeDD.value].text,
            //     endTownButton.GetComponent<VariationButton>().isSelected,
            //     witchButton.GetComponent<VariationButton>().isSelected,
            //     randGoldButton.GetComponent<VariationButton>().isSelected
            // );

            Game.currentGame = new Game(
                sessionId: sessionID,
                saveId: SaveAndLoad.GenerateSaveId(),
                creator: GameConstants.username,
                maxRnds: numRoundOptions[numRounds.value],
                gameMode: gameModeDD.options[gameModeDD.value].text,
                endTown: endTownButton.GetComponent<VariationButton>().isSelected,
                witchVar: witchButton.GetComponent<VariationButton>().isSelected,
                randGoldVar: randGoldButton.GetComponent<VariationButton>().isSelected,
                tradingVar: tradingButton.GetComponent<VariationButton>().isSelected
            );
        }
        else
        {
            Game.currentGame = new Game(sessionId: sessionID,
                saveId: session.saveID,
                creator: session.createdBy);

            SaveAndLoad.LoadLocalPlayerState(Game.currentGame.saveId);
        }
        Game.currentGame.SetInitialColorValues();
        Game.currentGame.SyncGameProperties();
        Player.GetLocalPlayer().SyncPlayerStats();
        UpdateSessionListView(Lobby.availableSessions);
    }

    #endregion
}
