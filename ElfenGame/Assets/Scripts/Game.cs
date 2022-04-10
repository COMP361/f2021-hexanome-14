using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

static public class ListExtension
{

    //TODO: Move to a better place
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

public class Game
{

    #region Fields

    public const string pDECK = "DECK", pDISCARD = "DISCARD", pVISIBLECARDS = "VISIBLE_CARDS", pPILE = "PILE",
    pPLAYERS = "PLAYERS", pCUR_PLAYER = "CUR_PLAYER", pCUR_ROUND = "CUR_ROUND", pCUR_PHASE = "CUR_PHASE",
    pMAX_ROUNDS = "MAX_ROUNDS", pGAME_MODE = "GAME_MODE", pEND_TOWN = "END_TOWN", pWITCH_CARD = "WITCH_CARD",
    pRAND_GOLD = "RAND_GOLD", pPASSED_PLAYERS = "PASSED_PLAYERS", pGAME_ID = "GAME_ID", pSAVE_ID = "SAVE_ID",
    pGOLD_VALUES = "GOLD_VALUES", pGAME_CREATOR = "GAME_CREATOR", pGOLD_PILE_VALUE = "GOLD_PILE_VALUE", pVISIBLE = "VISIBLE",
    pCUR_BID = "CUR_BID", pCUR_BID_PLAYER = "CUR_BID_PLAYER", pNUM_REM_BIDS = "NUM_REM_BIDS";
    public static string[] pGAME_PROPS = {
        pDECK, pDISCARD, pVISIBLECARDS, pPILE, pVISIBLE, pPLAYERS, pCUR_PLAYER, pCUR_ROUND, pCUR_PHASE, pMAX_ROUNDS,
        pPASSED_PLAYERS, pGAME_ID, pSAVE_ID,  pGAME_MODE, pEND_TOWN, pWITCH_CARD, pRAND_GOLD, pGOLD_VALUES, pGAME_CREATOR,
        pGOLD_PILE_VALUE, pCUR_BID, pCUR_BID_PLAYER, pNUM_REM_BIDS
    };
    private const string pCOLOR_AVAIL_PREFIX = "COLOR_AVAIL";

    #endregion

    private static Game _currentGame;
    public static Game currentGame
    {
        get
        {
            return _currentGame;
        }
        set
        {
            _currentGame = value;
        }
    }


    private ExitGames.Client.Photon.Hashtable _gameProperties;
    private ExitGames.Client.Photon.Hashtable _colorProperties;

    private bool gameIsOver = false;

    #region Properties
    private void PropertyNotFoundWarning(string property)
    {
        Debug.LogWarning("Property " + property + " not found in game properties");
    }

    private T GetP<T>(string property)
    {
        if (_gameProperties.ContainsKey(property))
        {
            return (T)_gameProperties[property];
        }
        else
        {
            PropertyNotFoundWarning(property);
            return default(T);
        }
    }
    private void SetP<T>(string property, T value)
    {
        _gameProperties[property] = value;
    }
    private List<T> GetL<T>(string property)
    {
        T[] array = GetP<T[]>(property);
        if (array == null)
        {
            return new List<T>();
        }
        return array.ToList();
    }
    private void SetL<T>(string property, List<T> list)
    {
        SetP<T[]>(property, list.ToArray());
    }
    public string gameId { get => GetP<string>(pGAME_ID); set => SetP<string>(pGAME_ID, value); }
    public string saveId { get => GetP<string>(pSAVE_ID); set => SetP<string>(pSAVE_ID, value); }
    public string gameCreator { get => GetP<string>(pGAME_CREATOR); set => SetP<string>(pGAME_CREATOR, value); }
    public int curPlayerIndex { get => GetP<int>(pCUR_PLAYER); set => SetP<int>(pCUR_PLAYER, value); }
    public int passedPlayers { get => GetP<int>(pPASSED_PLAYERS); set => SetP<int>(pPASSED_PLAYERS, value); }
    public int maxRounds { get => GetP<int>(pMAX_ROUNDS); set => SetP<int>(pMAX_ROUNDS, value); }
    public GamePhase curPhase { get => GetP<GamePhase>(pCUR_PHASE); set => SetP<GamePhase>(pCUR_PHASE, value); }
    public int curRound { get => GetP<int>(pCUR_ROUND); set => SetP<int>(pCUR_ROUND, value); }
    public int goldPileValue { get => GetP<int>(pGOLD_PILE_VALUE); set => SetP<int>(pGOLD_PILE_VALUE, value); }
    public GameMode gameMode { get => GetP<GameMode>(pGAME_MODE); set => SetP<GameMode>(pGAME_MODE, value); }
    public bool endTown { get => GetP<bool>(pEND_TOWN); set => SetP<bool>(pEND_TOWN, value); }
    public bool witchCard { get => GetP<bool>(pWITCH_CARD); set => SetP<bool>(pWITCH_CARD, value); }
    public bool randGold { get => GetP<bool>(pRAND_GOLD); set => SetP<bool>(pRAND_GOLD, value); }

    public int curBid { get => GetP<int>(pCUR_BID); set => SetP<int>(pCUR_BID, value); }
    public string curBidPlayer { get => GetP<string>(pCUR_BID_PLAYER); set => SetP<string>(pCUR_BID_PLAYER, value); }

    public int numRemainingAuctionItems { get => GetP<int>(pNUM_REM_BIDS); set => SetP<int>(pNUM_REM_BIDS, value); }

    // List properties
    public List<string> mPlayers { get => GetL<string>(pPLAYERS); set => SetL<string>(pPLAYERS, value); }
    public List<int> goldValues { get => GetL<int>(pGOLD_VALUES); set => SetL<int>(pGOLD_VALUES, value); }
    public List<CardEnum> mDeck { get => GetL<CardEnum>(pDECK); set => SetL<CardEnum>(pDECK, value); }
    public List<CardEnum> visibleCards { get => GetL<CardEnum>(pVISIBLECARDS); set => SetL<CardEnum>(pVISIBLECARDS, value); }
    public List<CardEnum> mDiscardPile { get => GetL<CardEnum>(pDISCARD); set => SetL<CardEnum>(pDISCARD, value); }
    public List<MovementTile> mPile { get => GetL<MovementTile>(pPILE); set => SetL<MovementTile>(pPILE, value); }
    public List<MovementTile> mVisibleTiles { get => GetL<MovementTile>(pVISIBLE); set => SetL<MovementTile>(pVISIBLE, value); }
    public List<PlayerColor> mAvailableColors
    {
        get
        {
            List<PlayerColor> colors = new List<PlayerColor>();
            for (int i = 0; i < 6; i++)
            {
                if ((string)_colorProperties[getColorKey((PlayerColor)i)] == "")
                {
                    colors.Add((PlayerColor)i);
                }
            }
            return colors;
        }
    }

    #endregion

    private string getColorKey(PlayerColor color)
    {
        return $"{pCOLOR_AVAIL_PREFIX}{Enum.GetName(typeof(PlayerColor), color)}";
    }

    public void ClaimColor(PlayerColor c)
    {
        string key = getColorKey(c);
        if (_colorProperties.ContainsKey(key) && (string)_colorProperties[key] == "")
        {
            //TODO: Attempt to claim color
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties[key] = Player.GetLocalPlayer().userName;

            ExitGames.Client.Photon.Hashtable oldProperties = new ExitGames.Client.Photon.Hashtable();
            oldProperties[key] = "";

            if (NetworkManager.manager)
            {
                NetworkManager.manager.SetGamePropertiesWithCheck(newProperties, oldProperties);
            }
        }
    }
    internal void SetFromGameData(SaveAndLoad.GameData data)
    {
        gameMode = data.gameMode;
        maxRounds = data.numRounds;
        endTown = data.endTown;
        witchCard = data.witch;
        randGold = data.randGold;
        goldPileValue = data.goldPileValue;
        mDeck = data.deck;
        mDiscardPile = data.discard;
        mVisibleTiles = data.visible;
        mPile = data.pile;
        curPlayerIndex = data.curPlayerIndex;
        mPlayers = data.players;
        // gameId = data.gameId; // TODO: This should not be set (new session id should be kept)
        visibleCards = data.visibleCards;
        curPhase = data.curPhase;
        curRound = data.curRound;
        passedPlayers = data.passedPlayers;
        saveId = data.saveId;
        goldValues = data.goldValues;

        SyncGameProperties();

    }

    public Game()
    {   // Used for non-master clients that receive rest of game state from master client
        Debug.Log("Creating new game");
        _gameProperties = new ExitGames.Client.Photon.Hashtable();
        _colorProperties = new ExitGames.Client.Photon.Hashtable();

        // Default gold values until updated by master client
        goldValues = new List<int> { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 6; i++)
        {
            _colorProperties[getColorKey((PlayerColor)i)] = "";
        }
    }
    public Game(string sessionId, string saveId, string creator, int maxRnds, GameMode gameMode, bool endTown, bool witchVar, bool randGoldVar) : this()
    {
        // Create new game constructor
        // Note no-param constructor is called first
        Debug.Log($"Creating new game with sessionId: {sessionId}");

        this.curPlayerIndex = 0;
        this.curRound = 1;
        this.curPhase = GamePhase.DrawCardsAndCounters;
        this.maxRounds = maxRnds;
        this.gameMode = gameMode;
        this.endTown = endTown;
        this.witchCard = witchVar;
        this.randGold = randGoldVar;
        this.passedPlayers = 0;
        this.saveId = saveId;
        this.gameId = sessionId;
        this.gameCreator = creator;

        this.mPlayers = new List<string>();
        this.mPile = new List<MovementTile>();
        this.mVisibleTiles = new List<MovementTile>();
        this.mDeck = new List<CardEnum>();
        this.mDiscardPile = new List<CardEnum>();
        this.visibleCards = new List<CardEnum>();
        this.goldPileValue = 0;
        this.curBid = 0;
        this.curBidPlayer = "";
        this.numRemainingAuctionItems = 0;

        InitPile(gameMode);
        InitDeck(gameMode, witchVar);

        if (gameMode == GameMode.Elfengold)
        {
            List<int> tempGoldValues = GameConstants.goldValues;
            if (randGoldVar)
            {
                tempGoldValues.Shuffle(); // doesn't matter if original list is modified (since only use once)
            }
            this.goldValues = tempGoldValues; // Set gold values if elfengold (already defaults to 0 for elvenland)
        }


    }

    public Game(string sessionId, string saveId, string creator) : this()
    {
        // Load Game Constructor
        Debug.Log($"Loading game with saveId: {saveId}");

        SaveAndLoad.GameData data = SaveAndLoad.LoadGameState(saveId);
        SetFromGameData(data);

        if (MainUIManager.manager)
        {
            MainUIManager.manager.SetTiles(data.tilePaths, data.tileTypes);
        }
    }


    public void SyncGameProperties()
    {
        UpdateDisplay();
        if (NetworkManager.manager) NetworkManager.manager.SetGameProperties(_gameProperties);
    }

    private void HandleColorUpdate(PlayerColor color, string value)
    {
        string key = getColorKey(color);
        _colorProperties[key] = value;
        if (Player.GetLocalPlayer().userName == value)
        {
            Player.GetLocalPlayer().playerColor = color;
            if (MainUIManager.manager)
                MainUIManager.manager.CloseColorSelection();
        }
        if (MainUIManager.manager)
        {
            MainUIManager.manager.UpdateColorOptions();
        }

        // Debug.LogError($"{value} claimed {color}");
    }

    public void UpdateGameProperties(ExitGames.Client.Photon.Hashtable properties)
    {
        for (int i = 0; i < 6; i++)
        {
            string key = getColorKey((PlayerColor)i);
            if (properties.ContainsKey(key))
            {
                HandleColorUpdate((PlayerColor)i, (string)properties[key]);
            }
        }

        bool updatedProps = false;
        foreach (string key in pGAME_PROPS)
        {
            if (properties.ContainsKey(key))
            {
                _gameProperties[key] = properties[key];
                updatedProps = true;
            }
        }
        if (updatedProps)
        {
            Debug.Log("Game properties updated");
        }

        foreach (string playerName in mPlayers)
        {
            string playerEndTownKey = getEndTownKey(playerName);
            if (properties.ContainsKey(playerEndTownKey))
            {
                Player.GetPlayer(playerName).endTown = properties[playerEndTownKey].ToString();
            }
        }
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (MainUIManager.manager)
        {
            HandlePhaseUpdate();
            MainUIManager.manager.UpdateAvailableTokens();
            MainUIManager.manager.UpdateRoundInfo(); // TODO: pass info as argument?
            MainUIManager.manager.UpdateGoldValues();
            MainUIManager.manager.UpdateAvailableCards();
        }

        SaveAndLoad.SaveGameState();

    }


    public void SetInitialColorValues()
    {

        if (NetworkManager.manager)
        {
            NetworkManager.manager.SetGameProperties(_colorProperties);
        }
    }
    private void InitPile(GameMode gameMode)
    {
        List<MovementTile> pile = mPile;
        List<MovementTile> visible = mVisibleTiles;

        if (gameMode == GameMode.Elfenland){
             for (int i = 0; i < 4; ++i)
            {
                pile.Add(MovementTile.Dragon);
                pile.Add(MovementTile.Elfcycle);
                pile.Add(MovementTile.GiantPig);
                pile.Add(MovementTile.MagicCloud);
                pile.Add(MovementTile.TrollWagon);
                pile.Add(MovementTile.Unicorn);
                
            }

        }
        else
        {
            for (int i= 0; i<4;i++)//change to 8
            {
                pile.Add(MovementTile.Dragon);
                pile.Add(MovementTile.MagicCloud);
            }

            for (int i= 0; i<5;i++)
            {
                pile.Add(MovementTile.Unicorn);
                
            }
            for (int i= 0; i<8;i++)
            {
                pile.Add(MovementTile.Elfcycle);
                pile.Add(MovementTile.TrollWagon);
            }

            for (int i= 0; i<9;i++)
            {
                pile.Add(MovementTile.GiantPig);
                
            }

            for (int i= 0; i<15;i++) //just for testing purposes
            {
                pile.Add(MovementTile.Double);
                pile.Add(MovementTile.Bounce);
                pile.Add(MovementTile.RoadObstacle);
                pile.Add(MovementTile.WaterObstacle);
                pile.Add(MovementTile.Gold);
            }
        }

        pile.Shuffle();

        for (int i = 0; i < 5; ++i)
        {
            visible.Add(pile[0]);
            pile.RemoveAt(0);
        }
        
        mPile = pile;
        mVisibleTiles = visible;
    }

    public void InitPlayersList()
    {
        if (mPlayers.Count > 0)
        {
            // Already initialized (probably from loading a game)
            return;
        }
        List<string> playersInGame = new List<string>();
        foreach (Photon.Realtime.Player p in NetworkManager.manager.GetPlayers())
        {
            playersInGame.Add(p.UserId);
        }
        playersInGame.Shuffle();
        mPlayers = playersInGame;

        if (endTown)
        {
            HashSet<string> claimedTowns = new HashSet<string>();
            claimedTowns.Add("TownElvenhold");
            var random = new System.Random();
            foreach (string player in playersInGame)
            {
                string playerEndTown = GameConstants.townNames[random.Next(0, GameConstants.townNames.Count)];
                while (claimedTowns.Contains(playerEndTown))
                {
                    playerEndTown = GameConstants.townNames[random.Next(0, GameConstants.townNames.Count)];
                }
                claimedTowns.Add(playerEndTown);
                _gameProperties[getEndTownKey(player)] = playerEndTown;
            }
        }

        numRemainingAuctionItems = 2 * mPlayers.Count; // Set num Auction Items
    }

    private string getEndTownKey(string player)
    {
        return $"{pEND_TOWN}_{player}";
    }

    private void InitDeck(GameMode gameMode, bool witchVar)
    {
        List<CardEnum> deck = mDeck;
         
        if (gameMode == GameMode.Elfenland)
        {
            for (int i = 0; i < 10; i++)
            {
                deck.Add(CardEnum.Dragon);
                deck.Add(CardEnum.ElfCycle);
                deck.Add(CardEnum.GiantPig);
                deck.Add(CardEnum.MagicCloud);
                deck.Add(CardEnum.Raft);
                deck.Add(CardEnum.TrollWagon);
                deck.Add(CardEnum.Unicorn);
            }

            deck.Add(CardEnum.Raft);
            deck.Add(CardEnum.Raft);

            deck.Shuffle();
            mDeck = deck;
        }
        else // Elfengold
        {
            for (int i = 0; i < 9; i++)
            {
                deck.Add(CardEnum.Dragon);
                deck.Add(CardEnum.ElfCycle);
                deck.Add(CardEnum.TrollWagon);
                deck.Add(CardEnum.MagicCloud);
                deck.Add(CardEnum.Unicorn);
                deck.Add(CardEnum.GiantPig);
                deck.Add(CardEnum.Raft);
            }

            // add 6 witch cards if witchVar is true
            if (witchVar)
            {
                for (int i = 0; i < 6; i++)
                {
                    deck.Add(CardEnum.Witch);
                }
            }
            deck.Shuffle();
            mDeck = deck;

            List<CardEnum> tempVisibleCards = visibleCards;
            for (int i = 0; i < 3; i++)
            {
                tempVisibleCards.Add(Draw(1)[0]);
            }
            visibleCards = tempVisibleCards;
            
        }

    }

    public void AddGoldCards() 
    {
        List<CardEnum> deck = mDeck;
        for (int i = 0; i < 6; i++)
        {
            deck.Add(CardEnum.Gold);
        }
        deck.Shuffle();
        mDeck = deck;
    }

    private void HandlePhaseUpdate()
    {
        if (!MainUIManager.manager)
        {
            Debug.LogError("No mainUIManager");
            return;
        }
        MainUIManager.manager.UpdateRoundInfo();
        if (curPhase == GamePhase.DrawCounters1 || curPhase == GamePhase.DrawCounters2 || curPhase == GamePhase.DrawCounters3)
        {
            MainUIManager.manager.showTokenSelection();
        }
        else
        {
            MainUIManager.manager.hideTokenSelection();
        }

        Player local = Player.GetLocalPlayer();
        if (curPhase == GamePhase.SelectTokenToKeep && local.IsMyTurn())
        {
            if (local.mHiddenTiles.Count == 0 && (local.mVisibleTiles.Count == 0 ||
                         (local.mVisibleTiles.Count == 1 && local.mVisibleTiles[0] == MovementTile.RoadObstacle)))
            {
                nextPlayer();
            }
            else
            {
                MainUIManager.manager.showAvailableTokensToKeep();
            }
            if (local.userName == mPlayers[0] && curRound == 1 && gameMode == GameMode.Elfengold) // Only do this once (for one player) doesn't matter which
            {
                AddGoldCards();
            }
        }
        else
        {
            MainUIManager.manager.hideAvailableTokensToKeep();
        }

        if (curPhase == GamePhase.DrawCardsAndCounters && local.IsMyTurn())
        {
            local.SelfInitRound();
        }

        if (gameMode == GameMode.Elfengold && curPhase == GamePhase.DrawCardsAndCounters && curRound > 1)
        {
            MainUIManager.manager.DrawCardPanelToggle(true);
        }
        else
        {
            MainUIManager.manager.DrawCardPanelToggle(false);
        }

        if (curPhase == GamePhase.Auction)
        {
            MainUIManager.manager.ShowAuctionScreen();
            MainUIManager.manager.UpdateAuctionItems(mPile.GetRange(0, numRemainingAuctionItems));
            MainUIManager.manager.UpdateAuctionCurrentBestBid(curBid, curBidPlayer);
        }
        else
        {
            MainUIManager.manager.HideAuctionScreen();
        }
    }

    public MovementTile RemoveVisibleTile(int index)
    {
        List<MovementTile> pile = mPile;
        List<MovementTile> visible = mVisibleTiles;
        MovementTile ret = visible[index];
        visible[index] = pile[0];
        pile.RemoveAt(0);
        mPile = pile;
        mVisibleTiles = visible;
        return ret;
    }

    public CardEnum RemoveVisibleCard(int index)
    {
        List<CardEnum> deck = mDeck;
        List<CardEnum> visible = visibleCards;
        CardEnum ret = visible[index];
        CardEnum drawn = Draw(1)[0];
        bool needValidCard = false;
        do
        {
            if (drawn == CardEnum.Gold)
            {
                needValidCard = true;
                Debug.Log("Gold card");
                Game.currentGame.goldPileValue += 3;
                Game.currentGame.SyncGameProperties();
                drawn = Draw(1)[0];
            }
            else
            {
                needValidCard = false;
            }
        } while (needValidCard);

        visible[index] = drawn;
        visibleCards = visible;
        return ret;
    }

    public void AddTilesToPile(MovementTile[] tiles)
    {
        List<MovementTile> pile = mPile;
        pile.AddRange(tiles);
        mPile = pile;
    }


    public MovementTile RemoveTileFromPile()
    {
        List<MovementTile> pile = mPile;
        MovementTile ret = pile[0];
        pile.RemoveAt(0);
        mPile = pile;
        return ret;
    }


    public void ReturnTilesToPile(MovementTile[] tiles)
    {
        List<MovementTile> pile = mPile;
        pile.AddRange(tiles);
        mPile = pile;
    }

    public void GameOver(bool check = false)
    {
        if (check && !gameIsOver)
        {
            return;
        }
        gameIsOver = true;

        List<Player> winners = new List<Player>();
        List<int> scores = new List<int>();
        foreach (Player p in Player.GetAllPlayers())
        {
            winners.Add(p);
            p.DeductDistToEndTown();
        }
        if (gameMode == GameMode.Elfenland)
        {
            Debug.Log("In Elfenland");
            winners = winners.OrderByDescending(o => o.nPoints * 1000 + o.mCards.Count).ToList();
        }
        else if (gameMode == GameMode.Elfengold)
        {
            Debug.Log("In Elfengold");
            winners = winners.OrderByDescending(o => o.nPoints * 1000 + o.nCoins).ToList();
        }
        foreach (Player p in winners)
        {
            scores.Add(p.nPoints);
        }
        if (MainUIManager.manager) MainUIManager.manager.GameOverTriggered(winners, scores);
    }

    public bool checkAnyPlayerDone()
    {
        foreach (Player p in Player.GetAllPlayers())
        {
            if (p.nPoints == 20)
            {
                return true;
            }
        }
        return false;
    }

    public void nextPlayer(bool passed = false)
    {
        curPlayerIndex = (curPlayerIndex + 1) % mPlayers.Count;
        // Debug.LogError($"Current Player {GetCurPlayer()}");
        //Debug.LogError($"Current Player Index {curPlayerIndex}");

        if (checkAnyPlayerDone())
        {
            GameOver();
            NetworkManager.manager.GameOver();
            return;
        }

        if ((curPhase == GamePhase.PlaceCounter || curPhase == GamePhase.Auction) && passed)
        {
            passedPlayers += 1;
        }
        else
        {
            passedPlayers = 0;
        }

        if (curPhase == GamePhase.Auction && passedPlayers >= mPlayers.Count - 1)
        {

            // Auction Round done
            curPlayerIndex = (curRound - 1) % mPlayers.Count; // Switch back to starting player

            MovementTile auctionTile = RemoveTileFromPile();

            Player local = Player.GetLocalPlayer();
            if (local.userName == curBidPlayer)
            {
                local.nCoins -= curBid;
                local.AddVisibleTile(auctionTile);
            }
            else
            {
                NetworkManager.manager.SignalPlayerWonAuction(curBidPlayer, curBid, auctionTile);
            }

            curBid = 0;
            curBidPlayer = "";
            numRemainingAuctionItems -= 1;

            if (numRemainingAuctionItems != 0)
            {
                SyncGameProperties();
                return; // Don't move to next round if there are still items to auction
            }
            else
            {
                // Set num remaining items to max for next round
                numRemainingAuctionItems = 2 * mPlayers.Count;
                passedPlayers = 0; // Set passed players for next phase to 0
                curPhase = curPhase.NextPhase();
            }
        }
        else if ((curPlayerIndex == (curRound - 1) % mPlayers.Count && curPhase != GamePhase.PlaceCounter && curPhase != GamePhase.Auction) || (passedPlayers == mPlayers.Count))
        {
            if (curPhase == GamePhase.Travel && curRound == maxRounds)
            {
                GameOver();
                NetworkManager.manager.GameOver();
                return;
            }
            else if (curPhase == GamePhase.SelectTokenToKeep)
            {
                if (MainUIManager.manager)
                {
                    List<MovementTile> cleared = MainUIManager.manager.ClearAllTiles(); // Local UI update
                    if (cleared != null)
                    {
                        if (gameMode == GameMode.Elfenland)
                        {
                            // Remove used obstacles
                            cleared.RemoveAll(o => o == MovementTile.RoadObstacle);
                        }
                        ReturnTilesToPile(cleared.ToArray());
                    }
                }
                if (NetworkManager.manager) NetworkManager.manager.ClearAllTiles(); // Other client UI Update
                curRound = curRound + 1;
                curPlayerIndex = (curRound - 1) % mPlayers.Count;
            }
            else
            {
                curPlayerIndex = (curRound - 1) % mPlayers.Count;
            }
            curPhase = curPhase.NextPhase();

            //Debug.LogError($"Cur Round is: {curRound}"); 
        }

        HelpElfManager helper = GameObject.FindObjectOfType<HelpElfManager>();
        HelpMessage hm = helper.gameObject.GetComponent<HelpMessage>();
        switch (curPhase)
        {
            case GamePhase.Auction:

                hm.helpMessage = "Auction is for bidding on tokens the highest bidder will take it ";
                break;
            case GamePhase.DrawCardsAndCounters:

                hm.helpMessage = "Draw Cards and Counters Phase ! ";
                break;

            case GamePhase.PlaceCounter:
                hm.helpMessage = "Place counter on the roads to prepare for the traveling phase ";
                break;
            case GamePhase.SelectTokenToKeep:
                hm.helpMessage = "Select a token to keep ";
                break;

            case GamePhase.Travel:
                hm.helpMessage = "Start Traveling, by choosing cards from the CardHand on the top right and dragging to the town of your choice, remember to visit new towns and try to get as many tokens as possible! ";
                break;
            default:
                hm.helpMessage = "choose a token from the selection or get a random token";
                break;
        }
        SyncGameProperties();
    }

    public CardEnum[] Draw(int n)
    {
        List<CardEnum> deck = mDeck;
        CardEnum[] ret = new CardEnum[n];
        for (int i = 0; i < n; ++i)
        {
            if (deck.Count == 0)
            {
                deck.AddRange(mDiscardPile);
                mDiscardPile.Clear();
                deck.Shuffle();
            }
            ret[i] = deck[0];
            deck.RemoveAt(0);
        } //TODO: Synce updates across clients (this might be covered now by sync at end of turn)
        this.mDeck = deck;
        return ret;
    }

    public void DiscardCards(CardEnum[] cards)
    {
        List<CardEnum> discard = mDiscardPile;
        discard.AddRange(cards);
        this.mDiscardPile = discard;
    }

    internal void SetSession(string createdBy, string sessionId)
    {
        gameCreator = createdBy;
        gameId = sessionId;
    }

    public string GetCurPlayer()
    {
        return mPlayers[curPlayerIndex];
    }


}
