using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player
{
    private int _nCoins = 0;
    private int _nPoints = 0;

    private PlayerColor _playerColor;
    private Elf elf;
    private PlayerTile tile;
    private PlayerVisibleTokenDisplay tokenDisplay;
    private List<CardEnum> _mCards;

    private Dictionary<MovementTile, int> mTiles;
    private List<MovementTile> mHiddenTiles, mVisibleTiles;

    private string _userName;
    private string _curTown;

    public const string pCOINS = "COINS";
    public const string pPOINTS = "POINTS";
    public const string pNAME = "NAME";
    public const string pCARDS = "CARDS";
    public const string pVISIBLE_TILES = "VISIBLE_TILES";
    public const string pHIDDEN_TILES = "HIDDEN_TILES";
    public const string pCOLOR = "COLOR";
    public const string pTOWN = "TOWN";

    private Dictionary<string, bool> visitedTown;


    #region Private Update Methods
    private void UpdateCoins(int value)
    {
        _nCoins = value;
        if (tile != null) tile.SetCoins(_nCoins);
    }

    private void UpdatePoints(int value)
    {
        _nPoints = value;
        if (tile != null) tile.SetPoints(_nPoints);
    }

    private void UpdateColor(PlayerColor value)
    {
        _playerColor = value;
        if (elf != null) elf.UpdateColor();
        if (tile != null) tile.SetColor(value);
        UpdateAllPlayerPoints();
    }

    private void UpdateName(string value)
    {
        _userName = value;
        if (tile != null) tile.SetName(_userName);
    }

    private void UpdateTown(string value)
    {
        if (elf != null) elf.MoveToTown(value, _curTown);
        _curTown = value;
        visitedTown[_curTown] = true;
        Debug.LogError($"Updating town {value} for player {_userName}");
        if (GameConstants.townDict.ContainsKey(_curTown))
        {
            NewTown town = GameConstants.townDict[_curTown];
            if (town != null) town.DisplayVisited();
        }
        int nVisited = 0;
        foreach (bool b in visitedTown.Values)
        {
            if (b) nVisited++;
        }
        nPoints = nVisited - 1;
    }

    private void UpdateVisibleTiles(List<MovementTile> tiles)
    {
        mVisibleTiles = tiles;
        UpdateTiles();
        if (tokenDisplay != null) tokenDisplay.SetVisible(tiles);
    }

    private void UpdateHiddenTiles(List<MovementTile> tiles)
    {
        mHiddenTiles = tiles;
        UpdateTiles();
        if (tokenDisplay != null) tokenDisplay.SetNumHidden(tiles.Count);
    }

    private void UpdateCards(List<CardEnum> cards)
    {
        _mCards = cards;
        if (Lobby.myUsername == _userName && GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateCardHand();
        if (tile != null) tile.SetCards(_mCards.Count);
    }

    private void UpdateAllPlayerPoints()
    { 
        foreach (NewTown town in GameConstants.townDict.Values)
        { 
            town.DisplayVisited();
	    }
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

        if (GameConstants.tileGroup != null)
        {
            foreach (MovementTileUIScript mtScript in GameConstants.tileGroup.GetComponentsInChildren<MovementTileUIScript>())
            {
                mtScript.UpdateText();
            }

            if (tile != null) tile.SetTiles(mVisibleTiles.Count + mHiddenTiles.Count);
        }
    }

    #endregion

    #region Public Member Definitions
    public int nCoins
    {
        get
        {
            return _nCoins;
        }
        set
        {
            if (_nCoins != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCOINS, value);
            UpdateCoins(value);
        }
    }
    public int nPoints
    {
        get
        {
            return _nPoints;
        }
        set
        {
            if (_nPoints != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pPOINTS, value);
            UpdatePoints(value);
        }
    }

    public PlayerColor playerColor
    {
        get
        {
            return _playerColor;
        }
        set
        {
            if (_playerColor != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCOLOR, value);
            UpdateColor(value);
        }
    }

    public List<CardEnum> mCards
    {
        get
        {
            return _mCards;
        }
    }

    public string userName
    {
        get
        {
            return _userName;
        }
        set
        {
            if (_userName != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pNAME, value);
            UpdateName(value);
        }
    }

    public string curTown
    {
        get { return _curTown; }
        set
        {
            if (_curTown != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTOWN, value);
            UpdateTown(value);
        }
    }

    #endregion

    #region stat syncing

    public void AddCard(CardEnum card)
    {
        _mCards.Add(card);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCARDS, mCards.ToArray());
    }

    public void RemoveCard(CardEnum card)
    {
        _mCards.Remove(card);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCARDS, mCards.ToArray());
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
    }

    public void AddVisibleTile(MovementTile tile)
    {
        mVisibleTiles.Add(tile);
        UpdateVisibleTiles(mVisibleTiles);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pVISIBLE_TILES, mVisibleTiles.ToArray());
	}

    public void RemoveVisibleTile(MovementTile tile)
    {
        mVisibleTiles.Remove(tile);
        UpdateVisibleTiles(mVisibleTiles);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pVISIBLE_TILES, mVisibleTiles.ToArray());
    }

    public void AddHiddenTile(MovementTile tile)
    {
        mHiddenTiles.Add(tile);
        UpdateHiddenTiles(mHiddenTiles);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pHIDDEN_TILES, mHiddenTiles.ToArray());
    }

    public void RemoveHiddenTile(MovementTile tile)
    {
        mHiddenTiles.Remove(tile);
        UpdateHiddenTiles(mHiddenTiles);
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pHIDDEN_TILES, mHiddenTiles.ToArray());
    }

    internal int GetNumTilesOfType(MovementTile tile)
    {
        if (!mTiles.ContainsKey(tile)) mTiles[tile] = 0;
        return mTiles[tile];
    }


    public void updatePropertiesCallback(string key, object value)
    {
        if (key == pCOINS)
        {
            UpdateCoins((int)value);
        }
        else if (key == pPOINTS)
        {
            UpdatePoints((int)value);
        }
        else if (key == pCARDS)
        {
            UpdateCards(((CardEnum[])value).ToList());
        }
        else if (key == pVISIBLE_TILES)
        {
            UpdateVisibleTiles(((MovementTile[])value).ToList());	
	    }
        else if (key == pHIDDEN_TILES)
        {
            UpdateHiddenTiles(((MovementTile[])value).ToList());	
	    }
        else if (key == pNAME)
        {
            UpdateName((string)value);
        }
        else if (key == pCOLOR)
        {
            UpdateColor((PlayerColor)value);
        }
        else if (key == pTOWN)
        {
            UpdateTown((string)value);
        }
    }


    #endregion

    public Player(string username)
    {
        _userName = username;
        mHiddenTiles = new List<MovementTile>();
        mVisibleTiles = new List<MovementTile>();
        mTiles = new Dictionary<MovementTile, int>();

        Reset();
        if (tile != null)
        {
            tile.UpdateStats(username, nCoins, nPoints, mCards.Count, mTiles.Count, playerColor);
            UpdateTiles();
	    }
    }

    public void Reset()
    {
        Debug.Log($"Resetting Player {_userName}");
        nPoints = 0;
        nCoins = 0;
        _mCards = new List<CardEnum>();
        visitedTown = new Dictionary<string, bool>();
        foreach (string townName in GameConstants.townDict.Keys)
        {
            visitedTown[townName] = false;
        }
        curTown = "TownElvenhold";
        UpdateTiles();
        UpdateVisibleTiles(mVisibleTiles);
        UpdateHiddenTiles(mHiddenTiles);
    }

    public bool IsMyTurn()
    {
        return (Game.currentGame != null) && (Game.currentGame.GetCurPlayer() == _userName);
    }

    public bool visited(string townName)
    {
        return visitedTown[townName];
    }

    public void OpenTokenDisplay()
    {
        if (tokenDisplay != null) tokenDisplay.openWindow();
    }

    public void SetElf(Elf elf)
    {
        this.elf = elf;
    }

    public void SetTokenDisplay(PlayerVisibleTokenDisplay tokenDisplay)
    {
        this.tokenDisplay = tokenDisplay;
    }

    public void SetTile(PlayerTile tile)
    {
        this.tile = tile;
        tile.UpdateStats(userName, nCoins, nPoints, mCards.Count, mTiles.Count, playerColor);
        UpdateTiles();
    }

    #region static methods

    private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

    public static Player GetLocalPlayer()
    {
        return GetPlayer(Lobby.myUsername);
    }

    public static List<Player> GetAllPlayers()
    {
        if (GameConstants.networkManager)
        {
            List<string> toRemove = new List<string>();
            foreach (string pName in _players.Keys)
            { 
	           if (GameConstants.networkManager.GetPlayer(pName) == null)
                {
                    toRemove.Add(pName);
                }
            }
            foreach (string pName in toRemove)
            {
                _players.Remove(pName);
            }
        }
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

    #endregion
}
