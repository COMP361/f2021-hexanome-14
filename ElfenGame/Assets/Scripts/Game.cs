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
    private const string pDECK = "DECK";
    private const string pDISCARD = "DISCARD";
    private const string pPILE = "PILE";
    private const string pVISIBLE = "VISIBLE";
    private const string pPOINTER = "POINTER";
    private const string pPLAYERS = "PLAYERS";
    private const string pCUR_PLAYER = "CUR_PLAYER";
    private const string pCUR_ROUND = "CUR_ROUND";
    private const string pCUR_PHASE = "CUR_PHASE";
    private const string pMAX_ROUNDS = "MAX_ROUNDS";
    private const string pCOLOR_AVAIL_PREFIX = "COLOR_AVAIL";
    private const string pGAME_VARIATION = "GAME_VARIATION";

    private const string pPASSED_PLAYERS = "PASSED_PLAYERS";

    public static Game currentGame = new Game();

    private int curCardPointer;


    private ExitGames.Client.Photon.Hashtable _gameProperties;

    #region Properties
    public string gameVariation
    {
        get
        {
            return (string)_gameProperties[pGAME_VARIATION];
        }
        set
        {
            _gameProperties[pGAME_VARIATION] = value;
            SyncGameProperties();
        }
    }

    public int curPlayerIndex
    {
        get
        {
            return (int)_gameProperties[pCUR_PLAYER];
        }
        set
        {
            _gameProperties[pCUR_PLAYER] = value;
            SyncGameProperties();
        }
    }

    public int passedPlayers
    {
        get
        {
            return (int)_gameProperties[pPASSED_PLAYERS];
        }
        set
        {
            _gameProperties[pPASSED_PLAYERS] = value;
            SyncGameProperties();
        }
    }

    public int maxRounds
    {
        get
        {
            return (int)_gameProperties[pMAX_ROUNDS];
        }
        set
        {
            _gameProperties[pMAX_ROUNDS] = value;
            SyncGameProperties();
        }
    }

    public GamePhase curPhase
    {
        get
        {
            return (GamePhase)_gameProperties[pCUR_PHASE];
        }
        set
        {
            _gameProperties[pCUR_PHASE] = value;
            SyncGameProperties();
        }
    }

    public int curRound
    {
        get
        {
            return (int)_gameProperties[pCUR_ROUND];
        }
        set
        {
            _gameProperties[pCUR_ROUND] = value;
            SyncGameProperties();
        }
    }

    public List<string> mPlayers
    {
        get
        {
            return new List<string>((string[])_gameProperties[pPLAYERS]);
        }
    }

    public List<CardEnum> mDeck
    {
        get
        {
            return new List<CardEnum>((CardEnum[])_gameProperties[pDECK]);
        }
    }

    public List<CardEnum> mDiscardPile
    {
        get
        {
            return new List<CardEnum>((CardEnum[])_gameProperties[pDISCARD]);
        }
    }
    public List<MovementTile> mPile
    {
        get
        {
            return new List<MovementTile>((MovementTile[])_gameProperties[pPILE]);
        }
    }

    public List<PlayerColor> mAvailableColors
    {
        get
        {
            List<PlayerColor> colors = new List<PlayerColor>();
            for (int i = 0; i < Enum.GetNames(typeof(PlayerColor)).Length; i++)
            {
                if ((string)_gameProperties[getColorKey((PlayerColor)i)] == "")
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
            return new List<MovementTile>((MovementTile[])_gameProperties[pVISIBLE]);
        }
    }


    #endregion

    // public void SyncPile()
    // {
    //     if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pPILE, pile.ToArray());
    // }

    // public void SyncVisible()
    // {
    //     if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pVISIBLE, visibleTiles.ToArray());
    // }

    // public void UpdatePile(List<MovementTile> newPile)
    // {
    //     pile = newPile;
    //     // TODO: Update visualization of pile 
    // }

    // public void UpdateVisible(List<MovementTile> newVisible)
    // {
    //     visibleTiles = newVisible;
    //     if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateAvailableTokens();
    //     // TODO: Update visualization of Visible tiles
    // }

    private string getColorKey(PlayerColor color)
    {
        return $"{pCOLOR_AVAIL_PREFIX}{Enum.GetName(typeof(PlayerColor), color)}";
    }

    public void ClaimColor(PlayerColor c)
    {
        string key = getColorKey(c);
        if (_gameProperties.ContainsKey(key) && (string)_gameProperties[key] == "")
        {
            //TODO: Attempt to claim color
            ExitGames.Client.Photon.Hashtable newProperties = new ExitGames.Client.Photon.Hashtable();
            newProperties[key] = Player.GetLocalPlayer().userName;

            ExitGames.Client.Photon.Hashtable oldProperties = new ExitGames.Client.Photon.Hashtable();
            oldProperties[key] = "";

            if (GameConstants.networkManager)
            {
                GameConstants.networkManager.SetGamePropertiesWithCheck(newProperties, oldProperties);
            }
        }
    }

    private Game()
    {
        _gameProperties = new ExitGames.Client.Photon.Hashtable();
    }


    public void SyncGameProperties()
    {
        UpdateDisplay();
        if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperties(_gameProperties);
        HandlePhaseUpdate();
        HandlePlayerUpdate();
    }

    private void CheckForColorUpdates(ExitGames.Client.Photon.Hashtable properties)
    {
        for (int i = 0; i < 6; ++i)
        {
            string key = getColorKey((PlayerColor)i);
            if (properties.ContainsKey(key))
            {
                string player_name = (string)properties[key];
                _gameProperties[key] = player_name;
                if (Player.GetLocalPlayer().userName == player_name)
                {
                    Player.GetLocalPlayer().playerColor = (PlayerColor)i;
                }
                if (GameConstants.mainUIManager)
                {
                    GameConstants.mainUIManager.UpdateColorOptions();
                }
            }
        }
    }

    public void UpdateGameProperties(ExitGames.Client.Photon.Hashtable properties)
    {
        foreach (string key in _gameProperties.Keys)
        {
            if (!properties.ContainsKey(key))
            {
                CheckForColorUpdates(properties);
                return; // If hashtable not complete, don't update
            }
        }
        _gameProperties = properties;
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (GameConstants.mainUIManager)
        {
            GameConstants.mainUIManager.UpdateAvailableTokens();
            // GameConstants.mainUIManager.UpdateColorOptions(); TODO: Handle separately
            GameConstants.mainUIManager.UpdateRoundInfo(); // TODO: pass info as argument?
        }
    }

    public void Init(int maxRnds, string variation)
    {

        Debug.Log("Game Init Called");
        _gameProperties[pCUR_PLAYER] = 0;
        _gameProperties[pCUR_ROUND] = 0;
        _gameProperties[pCUR_PHASE] = GamePhase.HiddenCounter;
        _gameProperties[pMAX_ROUNDS] = maxRnds;
        _gameProperties[pGAME_VARIATION] = variation;
        _gameProperties[pPASSED_PLAYERS] = 0;

        _gameProperties[pPLAYERS] = new string[] { };
        _gameProperties[pPILE] = new MovementTile[0];
        _gameProperties[pVISIBLE] = new MovementTile[0];
        _gameProperties[pDISCARD] = new CardEnum[0];
        _gameProperties[pDECK] = new CardEnum[0];


        InitPlayersList();
        InitPile();
        InitDeck();

        for (int i = 0; i < 6; ++i)
        {
            _gameProperties[getColorKey((PlayerColor)i)] = "";
        }

        InitRound();

        SyncGameProperties();
    }

    public void InitRound()
    {
        //TODO: Stop using this function (players should get their own cards)
        for (int i = 0; i < mPlayers.Count; i++)
        {
            Player p = Player.GetPlayer(mPlayers[i]);


            if (p.mCards.Count < 8)
            {
                p.AddCards(Draw(8 - p.mCards.Count));
            }

            p.AddHiddenTile(RemoveTileFromPile());
        }
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
        List<string> playersInGame = mPlayers;
        foreach (Photon.Realtime.Player p in GameConstants.networkManager.GetPlayers())
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

    private void YourTurn()
    {
        Player local = Player.GetLocalPlayer();
        if (curPhase == GamePhase.SelectTokenToKeep)
        {
            if (local.mHiddenTiles.Count == 0 && (local.mVisibleTiles.Count == 0 ||
             (local.mVisibleTiles.Count == 1 && local.mVisibleTiles[0] == MovementTile.RoadObstacle)))
            {
                nextPlayer();
            }
            else if (GameConstants.mainUIManager)
            {
                GameConstants.mainUIManager.UpdateAvailableTokens();
                GameConstants.mainUIManager.showAvailableTokensToKeep();
            }
            // SelectTokenToKeep but no tokens remain
        }
    }

    private void NotYourTurn()
    {
        if (GameConstants.mainUIManager)
        {
            GameConstants.mainUIManager.hideAvailableTokensToKeep();
        }
    }

    private void HandlePlayerUpdate()
    {
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        if (Player.GetLocalPlayer().IsMyTurn())
        {
            YourTurn();
        }
        else
        {
            NotYourTurn();
        }
    }

    private void HandlePhaseUpdate()
    {
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        if (curPhase == GamePhase.DrawCounters1 || curPhase == GamePhase.DrawCounters2 || curPhase == GamePhase.DrawCounters3)
        {
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.showTokenSelection();
        }
        else
        {
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.hideTokenSelection();
        }
        // Debug.LogError($"Cur Phase set to {Enum.GetName(typeof(GamePhase), curPhase)}");
    }


    // public void UpdateProperties(string key, object data)
    // {
    //     if (key == pDECK)
    //     {
    //         deck = ((CardEnum[])data).ToList();
    //     }
    //     else if (key == pPOINTER)
    //     {
    //         curCardPointer = (int)data;
    //     }
    //     else if (key == pPLAYERS)
    //     {
    //         players = ((string[])data).ToList();
    //     }
    //     else if (key == pCUR_PLAYER)
    //     {
    //         HandlePlayerUpdate((int)data);
    //         // Debug.LogError($"The current Player is {Game.currentGame.GetCurPlayer()}");
    //     }
    //     else if (key == pCUR_ROUND)
    //     {
    //         _curRound = (int)data;
    //         if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
    //     }
    //     else if (key == pPASSED_PLAYERS)
    //     {
    //         _passedPlayers = (int)data;
    //     }
    //     else if (key == pCUR_PHASE)
    //     {
    //         HandlePhaseUpdate((GamePhase)data);
    //     }
    //     else if (key == pMAX_ROUNDS)
    //     {
    //         _maxRounds = (int)data;
    //         if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
    //     }
    //     else if (key == pPILE)
    //     {
    //         UpdatePile(((MovementTile[])data).ToList());
    //     }
    //     else if (key == pVISIBLE)
    //     {
    //         UpdateVisible(((MovementTile[])data).ToList());
    //     }
    //     else if (key == pGAME_VARIATION)
    //     {
    //         _gameVariation = (string)data;
    //     }
    //     else
    //     {
    //         for (int i = 0; i < 6; ++i)
    //         {
    //             PlayerColor c = (PlayerColor)i;
    //             if (key == $"{pCOLOR_AVAIL_PREFIX}{Enum.GetName(typeof(PlayerColor), c)}")
    //             {
    //                 availableColors[c] = (string)data;
    //                 if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateColorOptions();
    //             }
    //         }
    //     }
    // }

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
        if (gameVariation == "Standard")
        {
            Debug.Log("In Standard");
            winners = winners.OrderByDescending(o => o.nPoints * 1000 + o.mCards.Count).ToList();
            foreach (Player p in winners)
            {
                scores.Add(p.nPoints);
            }
        }
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.GameOverTriggered(winners, scores);
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
            GameConstants.networkManager.GameOver();
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
                GameConstants.networkManager.GameOver();
                return;
            }
            else if (curPhase == GamePhase.SelectTokenToKeep)
            {
                if (GameConstants.mainUIManager) GameConstants.mainUIManager.ClearAllTiles();
                if (GameConstants.networkManager) GameConstants.networkManager.ClearAllTiles();
                curPhase = GamePhase.HiddenCounter;
                InitRound();
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
    }

    public CardEnum[] Draw(int n)
    {
        List<CardEnum> deck = mDeck;
        CardEnum[] ret = new CardEnum[n];
        for (int i = 0; i < n; ++i)
        {
            ret[i] = deck[0];
            deck.RemoveAt(0);
        } //TODO: Synce updates across clients
        _gameProperties[pDECK] = deck.ToArray();
        return ret;
    }

    // public CardEnum Draw()
    // {
    //     if (curCardPointer >= deck.Count)
    //     {
    //         deck.Shuffle();
    //         if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pDECK, deck.ToArray());
    //         curCardPointer = 0;
    //     }

    //     CardEnum ret = deck[curCardPointer];
    //     curCardPointer++;

    //     if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pPOINTER, curCardPointer);


    //     return ret;
    // }

    // public void AddTileToRoad(string roadName, MovementTile movementTile)
    // {
    //     if (onRoad.ContainsKey(roadName))
    //     {
    //         MovementTile[] tilesOnRoad = (MovementTile[])onRoad[roadName];
    //         Array.Resize(ref tilesOnRoad, tilesOnRoad.Length + 1);
    //         tilesOnRoad[tilesOnRoad.Length - 1] = movementTile;
    //         onRoad[roadName] = tilesOnRoad;
    //     }
    //     else
    //     {
    //         onRoad[roadName] = new MovementTile[] { movementTile };
    //     }
    // }

    // public void ClearTilesOnAllRoads()
    // {
    //     string[] toClear = new string[onRoad.Keys.Count];
    //     onRoad.Keys.CopyTo(toClear, 0);
    //     foreach (string roadName in toClear)
    //     {
    //         onRoad.Remove(roadName);
    //     }
    // }


    public string GetCurPlayer()
    {
        return mPlayers[curPlayerIndex];
    }


}
