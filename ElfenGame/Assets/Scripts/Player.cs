using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    public Elf elf { set; private get; }
    public PlayerTile tile { set; private get; }
    public PlayerVisibleTokenDisplay tokenDisplay { set; private get; }
    public const string pCOINS = "COINS";
    public const string pPOINTS = "POINTS";
    public const string pNAME = "NAME";
    public const string pCARDS = "CARDS";
    public const string pVISIBLE_TILES = "VISIBLE_TILES";
    public const string pHIDDEN_TILES = "HIDDEN_TILES";
    public const string pCOLOR = "COLOR";
    public const string pTOWN = "TOWN";

    public const string pVISITED = "VISITED";
    private ExitGames.Client.Photon.Hashtable _properties;

    public Dictionary<MovementTile, int> mTiles;

    private int lastInitializedround = 0;


    public void UpdateDisplay()
    {
        UpdateTiles();
        if (tile != null)
            tile.UpdateStats(userName, nCoins, nPoints, mCards.Count, mVisibleTiles.Count + mHiddenTiles.Count, playerColor);

        if (elf != null)
        {
            elf.UpdateColor();
            elf.SetTown(curTown);
        }

        if (tokenDisplay != null)
        {
            tokenDisplay.SetVisible(mVisibleTiles);
            tokenDisplay.SetNumHidden(mHiddenTiles.Count);
        }

        if (GameConstants.mainUIManager)
        {
            GameConstants.mainUIManager.UpdatePlayerPointDisplay();

            if (IsLocalPlayer())
            {
                GameConstants.mainUIManager.UpdateMovementTileCounts();
                GameConstants.mainUIManager.UpdateCardHand();
            }
        }
    }

    #region Private Update Methods


    private void HandleTownChange()
    {
    }
    // private void UpdateTown(string value)
    // {
    //     if (elf != null) elf.MoveToTown(value, _curTown);
    //     _curTown = value;
    //     visitedTown[_curTown] = true;
    //     //Debug.LogError($"Updating town {value} for player {_userName}");
    //     if (GameConstants.townDict.ContainsKey(_curTown))
    //     {
    //         NewTown town = GameConstants.townDict[_curTown];
    //         if (town != null) town.DisplayVisited();
    //     }
    //     int nVisited = 0;
    //     foreach (bool b in visitedTown.Values)
    //     {
    //         if (b) nVisited++;
    //     }
    //     nPoints = nVisited - 1;
    // }

    // private void UpdateVisibleTiles(List<MovementTile> tiles)
    // {
    //     mVisibleTiles = tiles;
    //     UpdateTiles();
    //     if (tokenDisplay != null) tokenDisplay.SetVisible(tiles);
    // }

    // private void UpdateHiddenTiles(List<MovementTile> tiles)
    // {
    //     mHiddenTiles = tiles;
    //     UpdateTiles();
    //     if (tokenDisplay != null) tokenDisplay.SetNumHidden(tiles.Count);
    // }

    // private void UpdateCards(List<CardEnum> cards)
    // {
    //     _mCards = cards;
    //     if (Lobby.myUsername == _userName && GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateCardHand();
    //     if (tile != null) tile.SetCards(_mCards.Count);
    // }

    public void SelfInitRound()
    {
        if (Game.currentGame.curRound <= lastInitializedround)
        {
            Debug.Log($"Already initialized round {Game.currentGame.curRound} for player {userName}");
            return;
        }
        List<CardEnum> cards = mCards;
        if (cards.Count < 8)
        {
            AddCards(Game.currentGame.Draw(8 - cards.Count));
        }

        AddHiddenTile(Game.currentGame.RemoveTileFromPile());

        lastInitializedround = Game.currentGame.curRound;
    }

    private void UpdateTiles()
    {
        mTiles = new Dictionary<MovementTile, int>();
        foreach (MovementTile tile in mVisibleTiles)
        {
            if (!mTiles.ContainsKey(tile)) mTiles[tile] = 0;

            mTiles[tile]++;
        }

        foreach (MovementTile tile in mHiddenTiles)
        {
            if (!mTiles.ContainsKey(tile)) mTiles[tile] = 0;

            mTiles[tile]++;
        }
    }

    #endregion

    #region Public Member Definitions
    public int nCoins
    {
        get
        {
            return (int)_properties[pCOINS];
        }
        set
        {
            _properties[pCOINS] = value;
            SyncPlayerStats();
        }
    }
    public int nPoints
    {
        get
        {
            return (int)_properties[pPOINTS];
        }
        set
        {
            _properties[pPOINTS] = value;
            SyncPlayerStats();
        }
    }

    public PlayerColor playerColor
    {
        get
        {
            return (PlayerColor)_properties[pCOLOR];
        }
        set
        {
            _properties[pCOLOR] = value;
            SyncPlayerStats();
        }
    }

    public List<CardEnum> mCards
    {
        get
        {
            return new List<CardEnum>((CardEnum[])_properties[pCARDS]);
        }
    }

    public List<MovementTile> mVisibleTiles
    {
        get
        {
            return new List<MovementTile>((MovementTile[])_properties[pVISIBLE_TILES]);
        }
    }

    public List<MovementTile> mHiddenTiles
    {
        get
        {
            return new List<MovementTile>((MovementTile[])_properties[pHIDDEN_TILES]);
        }
    }

    public string userName
    {
        get
        {
            return (string)_properties[pNAME];
        }
        set
        {
            _properties[pNAME] = value;
            SyncPlayerStats();
        }
    }

    public string curTown
    {
        get
        {
            return (string)_properties[pTOWN];
        }
        set
        {
            // Update visited
            Dictionary<string, bool> visited = mVisited;
            visited[value] = true;
            _properties[pVISITED] = visited;

            // Update points
            int nVisited = 0;
            foreach (bool b in visited.Values)
            {
                if (b) nVisited++;
            }
            _properties[pPOINTS] = nVisited - 1;

            // Update town
            _properties[pTOWN] = value;
            SyncPlayerStats();
        }
    }

    public Dictionary<string, bool> mVisited
    {
        get
        {
            return _properties[pVISITED] as Dictionary<string, bool>;
        }
    }

    #endregion

    #region stat syncing

    public void SyncPlayerStats()
    {
        if (!IsLocalPlayer())
        {
            Debug.LogError("Trying to sync stats for non-local player");
            return;
        }
        if (GameConstants.networkManager)
        {
            GameConstants.networkManager.SetLocalPlayerStats(_properties);
        }
        UpdateDisplay();
    }

    public void AddCards(CardEnum[] cards)
    {
        List<CardEnum> hand = mCards;
        hand.AddRange(cards);
        _properties[pCARDS] = hand.ToArray();
        SyncPlayerStats();
    }

    public void RemoveCards(CardEnum[] cards)
    {
        List<CardEnum> hand = mCards;
        foreach (CardEnum card in cards)
        {
            hand.Remove(card);
        }
        _properties[pCARDS] = hand.ToArray();
        SyncPlayerStats();
    }

    public void AddVisibleTile(MovementTile tile)
    {
        List<MovementTile> visibleTiles = mVisibleTiles;
        visibleTiles.Add(tile);
        _properties[pVISIBLE_TILES] = visibleTiles.ToArray();
        SyncPlayerStats();
    }
    public void AddHiddenTile(MovementTile tile)
    {
        List<MovementTile> hiddenTiles = mHiddenTiles;
        hiddenTiles.Add(tile);
        _properties[pHIDDEN_TILES] = hiddenTiles.ToArray();
        SyncPlayerStats();
    }
    public void RemoveTile(MovementTile tile)
    {
        if (mVisibleTiles.Contains(tile))
        {
            RemoveVisibleTile(tile);
        }
        else
        {
            RemoveHiddenTile(tile);
        }
        SyncPlayerStats();
    }

    public void RemoveVisibleTile(MovementTile tile)
    {
        List<MovementTile> visibleTiles = mVisibleTiles;
        visibleTiles.Remove(tile);
        _properties[pVISIBLE_TILES] = visibleTiles.ToArray();
    }

    public void RemoveHiddenTile(MovementTile tile)
    {
        List<MovementTile> hiddenTiles = mHiddenTiles;
        hiddenTiles.Remove(tile);
        _properties[pHIDDEN_TILES] = hiddenTiles.ToArray();
    }

    public void SetOnlyTile(MovementTile tile, bool inVisibleTiles)
    {
        bool hasObstacle = mVisibleTiles.Contains(MovementTile.RoadObstacle);
        List<MovementTile> hiddenTiles = new List<MovementTile>();
        List<MovementTile> visibleTiles = new List<MovementTile>();
        if (inVisibleTiles)
        {
            visibleTiles.Add(tile);
        }
        else
        {
            hiddenTiles.Add(tile);
        }

        if (hasObstacle) visibleTiles.Add(MovementTile.RoadObstacle);

        _properties[pVISIBLE_TILES] = visibleTiles.ToArray();
        _properties[pHIDDEN_TILES] = hiddenTiles.ToArray();
        SyncPlayerStats();
    }

    public int GetNumTilesOfType(MovementTile tile)
    {
        if (!mTiles.ContainsKey(tile))
            return 0;
        else
            return mTiles[tile];
    }

    public void UpdatePlayerStats(ExitGames.Client.Photon.Hashtable hashtable)
    {
        foreach (string key in _properties.Keys)
        {
            if (!hashtable.ContainsKey(key))
            {
                return; // If hashtable not complete, don't update
            }
        }
        _properties = hashtable;
        UpdateDisplay();
    }

    // public void updatePropertiesCallback(string key, object value)
    // {
    //     if (key == pCOINS)
    //     {
    //         if (value == null) value = 0;
    //         UpdateCoins((int)value);
    //     }
    //     else if (key == pPOINTS)
    //     {
    //         if (value == null) value = 0;
    //         UpdatePoints((int)value);
    //     }
    //     else if (key == pCARDS)
    //     {
    //         if (value == null) value = (object)(new List<CardEnum>()).ToArray();
    //         UpdateCards(((CardEnum[])value).ToList());
    //     }
    //     else if (key == pVISIBLE_TILES)
    //     {
    //         if (value == null) value = (object)(new List<MovementTile>()).ToArray();
    //         UpdateVisibleTiles(((MovementTile[])value).ToList());
    //     }
    //     else if (key == pHIDDEN_TILES)
    //     {
    //         if (value == null) value = (object)(new List<MovementTile>()).ToArray();
    //         UpdateHiddenTiles(((MovementTile[])value).ToList());
    //     }
    //     else if (key == pNAME)
    //     {
    //         if (value != null) UpdateName((string)value);
    //     }
    //     else if (key == pCOLOR)
    //     {
    //         if (value == null) value = PlayerColor.Blue;
    //         UpdateColor((PlayerColor)value);
    //     }
    //     else if (key == pTOWN)
    //     {
    //         if (value == null) value = "TownElvenhold";
    //         UpdateTown((string)value);
    //     }
    // }


    #endregion

    public Player(string userName)
    {
        _properties = new ExitGames.Client.Photon.Hashtable();
        _properties[pNAME] = userName;
        _properties[pCOINS] = 0;
        _properties[pPOINTS] = 0;
        _properties[pCOLOR] = PlayerColor.None;
        _properties[pTOWN] = "TownElvenhold";
        _properties[pCARDS] = new CardEnum[] { };
        _properties[pVISIBLE_TILES] = new MovementTile[] { MovementTile.RoadObstacle }; // TODO: Update for Elvengold
        _properties[pHIDDEN_TILES] = new MovementTile[] { };
        _properties[pVISITED] = new Dictionary<string, bool>();

        InitVisited();

        lastInitializedround = 0;

    }

    private void InitVisited()
    {
        Dictionary<string, bool> visited = mVisited;
        foreach (string townName in GameConstants.townNames)
        {
            visited[townName] = false;
        }

        visited["TownElvenhold"] = true;
        _properties[pVISITED] = visited;
    }

    // public void UpdateDisplayer()
    // {
    //     UpdateTiles();
    //     UpdateVisibleTiles(mVisibleTiles);
    //     UpdateHiddenTiles(mHiddenTiles);
    //     if (elf != null) elf.MoveToTown(_curTown, _curTown);
    // }

    // public void Reset()
    // {
    //     Debug.Log($"Resetting Player {_userName}");
    //     nPoints = 0;
    //     nCoins = 0;
    //     _mCards = new List<CardEnum>();
    //     visitedTown = new Dictionary<string, bool>();
    //     foreach (string townName in GameConstants.townDict.Keys)
    //     {
    //         visitedTown[townName] = false;
    //     }
    //     curTown = "TownElvenhold";
    //     UpdateTiles();
    //     UpdateVisibleTiles(mVisibleTiles);
    //     UpdateHiddenTiles(mHiddenTiles);

    //     if (GameConstants.networkManager)
    //     {
    //         GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCARDS, mCards.ToArray());
    //         GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pVISIBLE_TILES, mVisibleTiles.ToArray());
    //         GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pHIDDEN_TILES, mHiddenTiles.ToArray());
    //     }
    // }

    public bool IsMyTurn()
    {
        return (Game.currentGame != null) && (Game.currentGame.GetCurPlayer() == userName);
    }

    public bool IsLocalPlayer()
    {
        return userName == Lobby.myUsername;
    }

    public bool isVisited(string townName)
    {
        Dictionary<string, bool> visitedTowns = _properties[pVISITED] as Dictionary<string, bool>;
        if (visitedTowns.ContainsKey(townName))
        {
            return visitedTowns[townName];
        }
        else
        {
            Debug.LogError("Town " + townName + " not found in visitedTowns");
        }
        return false;
    }

    public void OpenTokenDisplay()
    {
        if (tokenDisplay != null) tokenDisplay.openWindow();
    }

    #region static methods

    private static Dictionary<string, Player> _players;

    public static Player GetLocalPlayer()
    {
        return GetPlayer(Lobby.myUsername);
    }

    public static List<Player> GetAllPlayers()
    {
        // if (GameConstants.networkManager)
        // {
        //     List<string> toRemove = new List<string>();
        //     foreach (string pName in _players.Keys)
        //     {
        //         if (GameConstants.networkManager.GetPlayer(pName) == null)
        //         {
        //             toRemove.Add(pName);
        //         }
        //     }
        //     foreach (string pName in toRemove)
        //     {
        //         _players.Remove(pName);
        //     }
        // }
        return _players.Values.ToList();
    }

    public static Player GetOrCreatePlayer(string p)
    {
        if (!_players.ContainsKey(p))
        {
            Player newPlayer = new Player(p);
            _players[p] = newPlayer;
        }

        return _players[p];
    }

    public static Player GetPlayer(string p)
    {
        if (!_players.ContainsKey(p))
        {
            Debug.LogError($"Could not find player {p} in Dictionary: {_players.Keys.ToArray<string>()}");
            return null;
        }

        return _players[p];
    }

    public static void ResetPlayers()
    {
        _players = new Dictionary<string, Player>();
        _players[Lobby.myUsername] = new Player(Lobby.myUsername);
    }

    #endregion
}
