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
            _nCoins = value;
            if (tile != null) tile.SetCoins(_nCoins);
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
            _nPoints = value;
            if (tile != null) tile.SetPoints(_nPoints);
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
            _playerColor = value;
            if (elf != null) elf.UpdateColor();
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
            _userName = value;
            if (tile != null) tile.SetName(_userName);
        }
    }

    public string curTown
    {
        get { return _curTown; }
        set
        {
            if (_curTown != value && GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTOWN, value);
            if (elf != null) elf.MoveToTown(value, _curTown);
            _curTown = value;
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
            _nCoins = (int)value;
            if (tile != null) tile.SetCards(_nCoins);
        } else if (key == pPOINTS)
        {
            _nPoints = (int)value;
            if (tile != null) tile.SetPoints(_nPoints);
        }
        else if (key == pCARDS)
        {
            _mCards = ((CardEnum[])value).ToList();
            if (Lobby.myUsername == _userName && GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateCardHand();
            if (tile != null) tile.SetCards(_mCards.Count);
        }
        else if (key == pNAME)
        {
            _userName = (string)value;
            if (tile != null) tile.SetName(_userName);
        } else if (key == pCOLOR)
        {
            _playerColor = (PlayerColor)value;
            if (elf != null) elf.UpdateColor();
	    } else if (key == pTOWN)
        {
            if (elf != null) elf.MoveToTown((string)value, _curTown);
            _curTown = (string)value;
	    }
    }

    #endregion

    public Player(string username)
    {
        _userName = username;
        curTown = "TownElvenhold";
        Reset();
        if (tile != null) tile.UpdateStats(username, nCoins, nPoints, mCards.Count, mTiles.Count);
    }

    public void Reset()
    {
        nPoints = 0;
        nCoins = 0;
        _mCards = new List<CardEnum>();
        _mTiles = new List<MovementTile>();
    }

    public bool IsMyTurn()
    {
        return (Game.currentGame != null) && (Game.currentGame.GetCurPlayer() == _userName);
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
