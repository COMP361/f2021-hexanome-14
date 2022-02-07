using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private int _nCoins = 0;
    private int _nPoints = 0;

    private List<Card> _mCards;
    private List<MovementTile> _mTiles;

    private string _userName;

    public const string pCOINS = "COINS";
    public const string pPOINTS = "POINTS";
    public const string pNAME = "NAME";

    public int nCoins
    {
        get
        {
            return _nCoins;
        }
        set
        {
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerProperty(pCOINS, value);
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
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerProperty(pPOINTS, value);
        }
    }

    public List<Card> mCards;
    public List<MovementTile> mTiles;
    public string userName
    {
        get
        {
            return _userName;
        }
        set
        {
            if (GameConstants.networkManager) GameConstants.networkManager.SetPlayerProperty(pNAME, value);
        }
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
        //} else if (p == CARDS)
        //{
        //    _mCards = (List<Card>)value;
        //} else if (p == playerProperties.TILES)
        //{
        //    _mTiles = (List<MovementTile>)value;
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
    }

    public void initialize(string name)
    {
        _userName = name;
        _nPoints = 0;
        _nCoins = 0;
    }


    private static Dictionary<string, PlayerManager> _players = new Dictionary<string, PlayerManager>();

    public static PlayerManager GetPlayer(string p)
    {
        if (!_players.ContainsKey(p))
        {
            _players = new Dictionary<string, PlayerManager>();
            foreach (PlayerManager pm in FindObjectsOfType<PlayerManager>())
            {
                _players.Add(pm.userName, pm);
            }
        }
        return _players[p];
    }
}
