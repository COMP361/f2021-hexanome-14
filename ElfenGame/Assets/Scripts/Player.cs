using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private int _nCoins = 0;
    private int _nPoints = 0;


    private List<CardEnum> _mCards;
	
    private List<MovementTile> _mTiles;

    private string _userName;

    public const string pCOINS = "COINS";
    public const string pPOINTS = "POINTS";
    public const string pNAME = "NAME";
    public const string pCARDS = "CARDS";
    public const string pTILES = "TILES";

    public int nCoins
    {
        get
        {
            return _nCoins;
        }
        set
        {
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCOINS, value);
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
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pPOINTS, value);
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
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pNAME, value);
        }
    }

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


    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;

    public void updatePropertiesCallback(string key, object value)
    {
        if (key == pCOINS)
        {
            _nCoins = (int)value;
        } else if (key == pPOINTS)
        {
            _nPoints = (int)value;
        }
        else if (key == pCARDS)
        {
            _mCards = ((CardEnum[])value).ToList();
        } else if (key == pNAME)
        {
            _userName = (string)value;
        }
        updateStats();
    }

    public void updateStats()
    {
        mNameText.text = userName;
        mCoinText.text = nCoins.ToString();
        mPointText.text = nPoints.ToString();
        mCardText.text = mCards.Count.ToString();
        mTileText.text = mTiles.Count.ToString();
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateCardHand();
    }

    public void reset()
    {
        nPoints = 0;
        nCoins = 0;
        _mCards = new List<CardEnum>();
        _mTiles = new List<MovementTile>();

        if (GameConstants.networkManager)
        {
            GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pCARDS, mCards.ToArray());
            //GameConstants.networkManager.SetPlayerPropertyByPlayerName(_userName, pTILES, mTiles.ToArray());
        }
    }

    public void Initialize(string name)
    {
        _userName = name;
        reset();
    }


    private static Dictionary<string, Player> _players = new Dictionary<string, Player>();

    public static Player GetLocalPlayer()
    {
        return GetPlayer(Lobby.myUsername);
    }

    public static List<Player> GetAllPlayers()
    {

        _players = new Dictionary<string, Player>();
        foreach (Player pm in FindObjectsOfType<Player>())
        {
            _players.Add(pm.userName, pm);
        }

        return _players.Values.ToList();
    }

    public static Player GetPlayer(string p)
    {
        if (!_players.ContainsKey(p))
        {
            _players = new Dictionary<string, Player>();
            foreach (Player pm in FindObjectsOfType<Player>())
            {
                _players.Add(pm.userName, pm);
            }
        }
        return _players[p];
    }
}
