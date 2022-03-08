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

    private List<MovementTile> _mTiles;

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
            town.DisplayVisited();
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

    public List<MovementTile> mTiles
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
        if (tile != null) tile.UpdateStats(username, nCoins, nPoints, mCards.Count, mTiles.Count);
    }

    public void Reset()
    {
        nPoints = 0;
        nCoins = 0;
        _mCards = new List<CardEnum>();
        _mTiles = new List<MovementTile>();
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
        return visitedTown[townName];
    }

    public void SetElf(Elf elf)
    {
        this.elf = elf;
    }

    public void SetTile(PlayerTile tile)
    {
        this.tile = tile;
        tile.UpdateStats(userName, nCoins, nPoints, mCards.Count, mTiles.Count);
    }

    #region static methods

    private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

    public static Player GetLocalPlayer()
    {
        return GetPlayer(Lobby.myUsername);
    }

    public static List<Player> GetAllPlayers()
    {
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
