using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

static public class ListExtension
{
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
    public const string pDECK = "DECK";
    public const string pDISCARD = "DISCARD";
    public const string pPILE = "PILE";
    public const string pVISIBLE = "VISIBLE";
    public const string pPLAYERS = "PLAYERS";
    public const string pCUR_PLAYER = "CUR_PLAYER";
    public const string pCUR_ROUND = "CUR_ROUND";
    public const string pCUR_PHASE = "CUR_PHASE";
    public const string pMAX_ROUNDS = "MAX_ROUNDS";
    public const string pGAME_MODE = "GAME_MODE";
    public const string pEND_TOWN = "END_TOWN";
    public const string pWITCH_CARD = "WITCH_CARD";
    public const string pRAND_GOLD = "RAND_GOLD";

    public const string pPASSED_PLAYERS = "PASSED_PLAYERS";

    public const string pGAME_ID = "GAME_ID";

    public const string pGAME_CREATOR = "GAME_CREATOR";

    public static string[] pGAME_PROPS = {
        pDECK, pDISCARD, pPILE, pVISIBLE, pPLAYERS, pCUR_PLAYER, pCUR_ROUND, pCUR_PHASE, pMAX_ROUNDS, pPASSED_PLAYERS, pGAME_ID, pGAME_MODE, pEND_TOWN, pWITCH_CARD, pRAND_GOLD
    };

    private const string pCOLOR_AVAIL_PREFIX = "COLOR_AVAIL";

    public static Game currentGame = new Game();


    private ExitGames.Client.Photon.Hashtable _gameProperties;
    private ExitGames.Client.Photon.Hashtable _colorProperties;

    private void PropertyNotFoundWarning(string property)
    {
        Debug.LogWarning("Property " + property + " not found in game properties");
    }

    #region Properties

    public ExitGames.Client.Photon.Hashtable gameProperties
    {
        get
        {
            return _gameProperties;
        }
    }

    public string gameId
    {
        get
        {
            if (!_gameProperties.ContainsKey(pGAME_ID))
            {
                PropertyNotFoundWarning(pGAME_ID);
                return "";
            }
            return (string)_gameProperties[pGAME_ID];
        }
        set
        {
            _gameProperties[pGAME_ID] = value;
        }
    }

    public string gameCreator
    {
        get
        {
            if (!_gameProperties.ContainsKey(pGAME_CREATOR))
            {
                PropertyNotFoundWarning(pGAME_CREATOR);
                return "";
            }
            return (string)_gameProperties[pGAME_CREATOR];
        }
        set
        {
            _gameProperties[pGAME_CREATOR] = value;
        }
    }

    public int curPlayerIndex
    {
        get
        {
            if (!_gameProperties.ContainsKey(pCUR_PLAYER))
            {
                PropertyNotFoundWarning(pCUR_PLAYER);
                return -1;
            }
            return (int)_gameProperties[pCUR_PLAYER];
        }
        set
        {
            _gameProperties[pCUR_PLAYER] = value;
        }
    }

    internal void SetFromGameData(SaveAndLoad.GameData data)
    {
        gameMode = data.gameMode;
        maxRounds = data.numRounds;
        endTown = data.endTown;
        witchCard = data.witch;
        randGold = data.randGold;
        mDeck = data.deck;
        mDiscardPile = data.discard;
        mVisibleTiles = data.visible;
        mPile = data.pile;
        curPlayerIndex = data.curPlayerIndex;
        mPlayers = data.players;
        gameId = data.gameId;
        curPhase = data.curPhase;
        curRound = data.curRound;
        passedPlayers = data.passedPlayers;

        SyncGameProperties();

    }



    public int passedPlayers
    {
        get
        {
            if (!_gameProperties.ContainsKey(pPASSED_PLAYERS))
            {
                PropertyNotFoundWarning(pPASSED_PLAYERS);
                return -1;
            }
            return (int)_gameProperties[pPASSED_PLAYERS];
        }
        set
        {
            _gameProperties[pPASSED_PLAYERS] = value;
        }
    }

    public int maxRounds
    {
        get
        {
            if (!_gameProperties.ContainsKey(pMAX_ROUNDS))
            {
                PropertyNotFoundWarning(pMAX_ROUNDS);
                return -1;
            }
            return (int)_gameProperties[pMAX_ROUNDS];
        }
        set
        {
            _gameProperties[pMAX_ROUNDS] = value;
        }
    }

    public GamePhase curPhase
    {
        get
        {
            if (!_gameProperties.ContainsKey(pCUR_PHASE))
            {
                PropertyNotFoundWarning(pCUR_PHASE);
                return GamePhase.DrawCardsAndCounters;
            }
            return (GamePhase)_gameProperties[pCUR_PHASE];
        }
        set
        {
            _gameProperties[pCUR_PHASE] = value;
        }
    }

    public int curRound
    {
        get
        {
            if (!_gameProperties.ContainsKey(pCUR_ROUND))
            {
                PropertyNotFoundWarning(pCUR_ROUND);
                return -1;
            }
            return (int)_gameProperties[pCUR_ROUND];
        }
        set
        {
            _gameProperties[pCUR_ROUND] = value;
        }
    }

    public string gameMode
    {
        get
        {
            if (!_gameProperties.ContainsKey(pGAME_MODE))
            {
                PropertyNotFoundWarning(pGAME_MODE);
                return "";
            }
            return (string)_gameProperties[pGAME_MODE];
        }
        set
        {
            _gameProperties[pGAME_MODE] = value;
        }
    }
    public bool endTown
    {
        get
        {
            if (!_gameProperties.ContainsKey(pEND_TOWN))
            {
                PropertyNotFoundWarning(pEND_TOWN);
                return false;
            }
            return (bool)_gameProperties[pEND_TOWN];
        }
        set
        {
            _gameProperties[pEND_TOWN] = value;
        }
    }
    public bool witchCard
    {
        get
        {
            if (!_gameProperties.ContainsKey(pWITCH_CARD))
            {
                PropertyNotFoundWarning(pWITCH_CARD);
                return false;
            }
            return (bool)_gameProperties[pWITCH_CARD];
        }
        set
        {
            _gameProperties[pWITCH_CARD] = value;
        }
    }

    public bool randGold
    {
        get
        {
            if (!_gameProperties.ContainsKey(pRAND_GOLD))
            {
                PropertyNotFoundWarning(pRAND_GOLD);
                return false;
            }
            return (bool)_gameProperties[pRAND_GOLD];
        }
        set
        {
            _gameProperties[pRAND_GOLD] = value;
        }
    }

    public List<string> mPlayers
    {
        get
        {
            if (!_gameProperties.ContainsKey(pPLAYERS))
            {
                PropertyNotFoundWarning(pPLAYERS);
                return new List<string>();
            }
            return new List<string>((string[])_gameProperties[pPLAYERS]);
        }
        set
        {
            _gameProperties[pPLAYERS] = value.ToArray();
        }
    }

    public List<CardEnum> mDeck
    {
        get
        {
            if (!_gameProperties.ContainsKey(pDECK))
            {
                PropertyNotFoundWarning(pDECK);
                return new List<CardEnum>();
            }
            return new List<CardEnum>((CardEnum[])_gameProperties[pDECK]);
        }

        set
        {
            _gameProperties[pDECK] = value.ToArray();
        }
    }

    public List<CardEnum> mDiscardPile
    {
        get
        {
            if (!_gameProperties.ContainsKey(pDISCARD))
            {
                PropertyNotFoundWarning(pDISCARD);
                return new List<CardEnum>();
            }
            return new List<CardEnum>((CardEnum[])_gameProperties[pDISCARD]);
        }
        set
        {
            _gameProperties[pDISCARD] = value.ToArray();
        }
    }
    public List<MovementTile> mPile
    {
        get
        {
            if (!_gameProperties.ContainsKey(pPILE))
            {
                PropertyNotFoundWarning(pPILE);
                return new List<MovementTile>();
            }
            return new List<MovementTile>((MovementTile[])_gameProperties[pPILE]);
        }
        set
        {
            _gameProperties[pPILE] = value.ToArray();
        }
    }

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

    public List<MovementTile> mVisibleTiles
    {
        get
        {
            if (!_gameProperties.ContainsKey(pVISIBLE))
            {
                PropertyNotFoundWarning(pVISIBLE);
                return new List<MovementTile>();
            }
            return new List<MovementTile>((MovementTile[])_gameProperties[pVISIBLE]);
        }
        set
        {
            _gameProperties[pVISIBLE] = value.ToArray();
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

    public Game()
    {
        _gameProperties = new ExitGames.Client.Photon.Hashtable();
        _colorProperties = new ExitGames.Client.Photon.Hashtable();
        for (int i = 0; i < 6; i++)
        {
            _colorProperties[getColorKey((PlayerColor)i)] = "";
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
            Debug.LogError("Game properties updated");
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
        }

        SaveAndLoad.SaveGame();

    }

    public void Init(int maxRnds, string gameMode, bool endTown, bool witchVar, bool randGoldVar)
    {
        // TODO: sync endTown, whitchVar, randGoldVar
        Debug.Log($"max rnds {maxRnds}, endTown {endTown}, whitchVar {witchVar}, randGoldVar {randGoldVar}");
        Debug.Log("Game Init Called");
        _gameProperties[pCUR_PLAYER] = 0;
        _gameProperties[pCUR_ROUND] = 1;
        _gameProperties[pCUR_PHASE] = GamePhase.DrawCardsAndCounters;
        _gameProperties[pMAX_ROUNDS] = maxRnds;
        _gameProperties[pGAME_MODE] = gameMode;
        _gameProperties[pEND_TOWN] = endTown;
        _gameProperties[pWITCH_CARD] = witchVar;
        _gameProperties[pRAND_GOLD] = randGoldVar;
        _gameProperties[pPASSED_PLAYERS] = 0;
        _gameProperties[pGAME_ID] = gameId;
        _gameProperties[pGAME_CREATOR] = gameCreator;

        _gameProperties[pPLAYERS] = new string[] { };
        _gameProperties[pPILE] = new MovementTile[0];
        _gameProperties[pVISIBLE] = new MovementTile[0];
        _gameProperties[pDISCARD] = new CardEnum[0];
        _gameProperties[pDECK] = new CardEnum[0];

        InitPlayersList();
        InitPile();
        InitDeck();

        if (NetworkManager.manager)
        {
            NetworkManager.manager.SetGameProperties(_colorProperties);
        }

        SyncGameProperties();

    }
    private void InitPile()
    {
        List<MovementTile> pile = mPile;
        List<MovementTile> visible = mVisibleTiles;
        for (int i = 0; i < 8; ++i)
        {
            pile.Add(MovementTile.Dragon);
            pile.Add(MovementTile.Elfcycle);
            pile.Add(MovementTile.GiantPig);
            pile.Add(MovementTile.MagicCloud);
            pile.Add(MovementTile.TrollWagon);
            pile.Add(MovementTile.Unicorn);
        }

        pile.Shuffle();

        for (int i = 0; i < 5; ++i)
        {
            visible.Add(pile[0]);
            pile.RemoveAt(0);
        }
        _gameProperties[pPILE] = pile.ToArray();
        _gameProperties[pVISIBLE] = visible.ToArray();
    }

    public void InitPlayersList()
    {
        List<string> playersInGame = new List<string>();
        foreach (Photon.Realtime.Player p in NetworkManager.manager.GetPlayers())
        {
            playersInGame.Add(p.UserId);
        }
        playersInGame.Shuffle();
        _gameProperties[pPLAYERS] = playersInGame.ToArray();
    }

    private void InitDeck()
    {
        List<CardEnum> deck = mDeck;
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
        _gameProperties[pDECK] = deck.ToArray();
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
        }
        else
        {
            MainUIManager.manager.hideAvailableTokensToKeep();
        }

        if (curPhase == GamePhase.DrawCardsAndCounters && local.IsMyTurn())
        {
            local.SelfInitRound();
        }
        // Debug.LogError($"Cur Phase set to {Enum.GetName(typeof(GamePhase), curPhase)}");
    }

    public MovementTile RemoveVisibleTile(int index)
    {
        List<MovementTile> pile = mPile;
        List<MovementTile> visible = mVisibleTiles;
        MovementTile ret = visible[index];
        visible[index] = pile[0];
        pile.RemoveAt(0);
        _gameProperties[pVISIBLE] = visible.ToArray();
        _gameProperties[pPILE] = pile.ToArray();
        //TODO: Sync pile
        return ret;
    }

    public void AddTilesToPile(MovementTile[] tiles)
    {
        List<MovementTile> pile = mPile;
        pile.AddRange(tiles);
        _gameProperties[pPILE] = pile.ToArray();
        //TODO: Sync pile
    }


    public MovementTile RemoveTileFromPile()
    {
        List<MovementTile> pile = mPile;
        MovementTile ret = pile[0];
        pile.RemoveAt(0);
        _gameProperties[pPILE] = pile.ToArray();
        //TODO: Sync pile
        return ret;
    }

    public void GameOver()
    {
        List<Player> winners = new List<Player>();
        List<int> scores = new List<int>();
        foreach (Player p in Player.GetAllPlayers())
        {
            winners.Add(p);
        }
        if (gameMode == "Elfenland")
        {
            Debug.Log("In Elfenland");
            winners = winners.OrderByDescending(o => o.nPoints * 1000 + o.mCards.Count).ToList();
            foreach (Player p in winners)
            {
                scores.Add(p.nPoints);
            }
        }
        else if (gameMode == "Elfengold")
        {
            Debug.Log("In Elfengold");
            //TODO: Implement Elfengold Ending
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

        if (curPhase == GamePhase.PlaceCounter && passed)
        {
            passedPlayers += 1;
        }
        else
        {
            passedPlayers = 0;
        }
        if ((curPlayerIndex == (curRound - 1) % mPlayers.Count && curPhase != GamePhase.PlaceCounter) || (passedPlayers == mPlayers.Count))
        {
            if (curPhase == GamePhase.Travel && curRound == maxRounds)
            {
                GameOver();
                NetworkManager.manager.GameOver();
                return;
            }
            else if (curPhase == GamePhase.SelectTokenToKeep)
            {
                if (MainUIManager.manager) MainUIManager.manager.ClearAllTiles();
                if (NetworkManager.manager) NetworkManager.manager.ClearAllTiles();
                curPhase = GamePhase.DrawCardsAndCounters;
                curRound = curRound + 1;
                curPlayerIndex = (curRound - 1) % mPlayers.Count;
            }
            else
            {
                curPhase++;
                curPlayerIndex = (curRound - 1) % mPlayers.Count;
            }

            //Debug.LogError($"Cur Round is: {curRound}"); 
        }
        SyncGameProperties();
    }

    public CardEnum[] Draw(int n)
    {
        //TODO: reshuffle deck with empty (use discard pile)
        List<CardEnum> deck = mDeck;
        CardEnum[] ret = new CardEnum[n];
        for (int i = 0; i < n; ++i)
        {
            ret[i] = deck[0];
            deck.RemoveAt(0);
        } //TODO: Synce updates across clients (this might be covered now by sync at end of turn)
        _gameProperties[pDECK] = deck.ToArray();
        return ret;
    }
    internal void SetSession(Lobby.GameSession currentSelectedSession)
    {
        gameCreator = currentSelectedSession.createdBy;
        gameId = currentSelectedSession.session_ID;
    }

    public string GetCurPlayer()
    {
        return mPlayers[curPlayerIndex];
    }


}
