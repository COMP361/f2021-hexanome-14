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
    private List<CardEnum> _mCards;

    private Dictionary<MovementTile, int> _mTiles;

    private string _userName;
    private string _curTown;

    public const string pCOINS = "COINS";
    public const string pPOINTS = "POINTS";
    public const string pNAME = "NAME";
    public const string pCARDS = "CARDS";
    public const string pTILES = "TILES";
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
        nPoints = nVisited-1;
    }

    private void UpdateCards(List<CardEnum> cards)
    {
        _mCards = cards;
        if (Lobby.myUsername == _userName && GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateCardHand();
        if (tile != null) tile.SetCards(_mCards.Count);
    }

    private void DisplayTiles()
    { 
        if (GameConstants.tileGroup != null)
        {
            foreach (MovementTileUIScript mtScript in GameConstants.tileGroup.GetComponentsInChildren<MovementTileUIScript>())
            {
                mtScript.UpdateText();
            }

            int nTiles = 0;
            foreach (int v in _mTiles.Values) nTiles += v;

            if (tile != null) tile.SetTiles(nTiles);
        }
    }

    private void UpdateTiles(MovementTile movementTile, int newVal)
    {
        _mTiles[movementTile] = newVal;
        DisplayTiles();  
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

    public Dictionary<MovementTile, int> mTiles
    {
        get
        {
            return _mTiles;
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
        if (!_mTiles.ContainsKey(tile)) _mTiles[tile] = 0;
        if (_mTiles[tile] > 0)
        {
            _mTiles[tile]--;
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTILES, new object[] { tile, _mTiles[tile] });
        }
    }

    public void AddTile(MovementTile tile)
    {
        if (!_mTiles.ContainsKey(tile)) _mTiles[tile] = 0;
        _mTiles[tile]++;
        if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTILES, new object[] { tile, _mTiles[tile] });
    }

    internal int GetNumTilesOfType(MovementTile tile)
    {
        if (!_mTiles.ContainsKey(tile)) _mTiles[tile] = 0;
        return _mTiles[tile];
    }

    public void ResetTiles()
    { 
        foreach (MovementTile movementTile in mTiles.Keys)
        {
            _mTiles[movementTile] = 0;
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTILES, new object[] { _mTiles[movementTile] });
	    }
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
        else if (key == pTILES)
        {
            object[] vals = (object[]) value; 
	        UpdateTiles((MovementTile) vals[0], (int) vals[1]);
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
        Reset();
        curTown = "TownElvenhold";
        if (tile != null)
        {
            tile.UpdateStats(username, nCoins, nPoints, mCards.Count, mTiles.Count, playerColor);
            DisplayTiles();
	    }
    }

    public void Reset()
    {
        Debug.Log($"Resetting Player {_userName}");
        nPoints = 0;
        nCoins = 0;
        _mCards = new List<CardEnum>();
        _mTiles = new Dictionary<MovementTile, int>();
        ResetTiles();
        visitedTown = new Dictionary<string, bool>();
        foreach (string townName in GameConstants.townDict.Keys)
        {
            visitedTown[townName] = false;
        }
        
    }

    public bool IsMyTurn()
    {
        return (Game.currentGame != null) && (Game.currentGame.GetCurPlayer() == _userName);
    }

    public bool visited(string townName)
    {
        if (!visitedTown.ContainsKey(townName))
        {
            foreach (string tName in GameConstants.townDict.Keys)
            {
                visitedTown[tName] = false;
            }
        }
        return visitedTown[townName];
    }

    public void SetElf(Elf elf)
    {
        this.elf = elf;
    }

    public void SetTile(PlayerTile tile)
    {
        this.tile = tile;
        tile.UpdateStats(userName, nCoins, nPoints, mCards.Count, mTiles.Count, playerColor);
        DisplayTiles();
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
