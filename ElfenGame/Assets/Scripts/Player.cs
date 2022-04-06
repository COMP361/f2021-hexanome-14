using System;
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

    private String _endTown; // Not synced

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

        if (MainUIManager.manager)
        {
            MainUIManager.manager.UpdatePlayerPointDisplay();

            if (IsLocalPlayer())
            {
                MainUIManager.manager.UpdateMovementTileCounts();
                MainUIManager.manager.UpdateCardHand();
            }
        }

        if (!IsLocalPlayer())
            Game.currentGame.GameOver(check: true);
    }

    #region Private Update Methods


    public void SelfInitRound()
    {
        if (Game.currentGame.curRound <= lastInitializedround)
        {
            Debug.Log($"Already initialized round {Game.currentGame.curRound} for player {userName}");
            return;
        }
        if (Game.currentGame.curRound == 1)
        {
            SelfInitFirstRound();
        }
        List<CardEnum> cards = mCards;
        if (cards.Count < 8)
        {
            AddCards(Game.currentGame.Draw(8 - cards.Count));
        }

        AddHiddenTile(Game.currentGame.RemoveTileFromPile());

        if (Game.currentGame.gameMode == "Elfengold")
        {
            nCoins += 2;
        }

        lastInitializedround = Game.currentGame.curRound;
    }

    private void SelfInitFirstRound()
    {
        if (Game.currentGame.gameMode == "Elfengold")
        {
            nCoins = 10;
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

    public string endTown
    {
        get
        {
            if (_endTown == null)
            {
                Debug.LogWarning("End town is null");
                return "";
            }
            return _endTown;
        }
        set
        {
            _endTown = value;
            if (userName == GameConstants.username && MainUIManager.manager)
            {
                MainUIManager.manager.UpdateEndTown(value);
            }
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
        if (NetworkManager.manager)
        {
            NetworkManager.manager.SetLocalPlayerStats(_properties);
        }
        UpdateDisplay();
        if (IsLocalPlayer())
        {
            SaveAndLoad.SaveLocalPlayerState();
        }
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
        Game.currentGame.DiscardCards(cards);
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
        if (IsLocalPlayer()) return; // Don't update local player stats (they should already be up to date)
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

        Debug.LogError("Player created: " + userName);
    }

    public void DeductDistToEndTown()
    {
        Dictionary<string, List<string>> adj = new Dictionary<string, List<string>>();

        foreach (string townName in GameConstants.townNames)
        {
            adj[townName] = new List<string>();
        }

        foreach (PathScript path in GameConstants.roadDict.Values)
        {
            adj[path.town1.name].Add(path.town2.name);
            adj[path.town2.name].Add(path.town1.name);
        }

        Dictionary<string, int> dist = new Dictionary<string, int>();
        foreach (string town in adj.Keys)
        {
            dist[town] = -1;
        }

        Queue<string> q = new Queue<string>();
        q.Enqueue(curTown);
        dist[curTown] = 0;

        while (q.Count != 0)
        {
            string cTown = q.Dequeue();
            foreach (string nextTown in adj[cTown])
            {
                if (dist[nextTown] == -1)
                {
                    dist[nextTown] = dist[cTown] + 1;
                    q.Enqueue(nextTown);
                }
            }
        }

        _properties[pPOINTS] = nPoints - dist[endTown]; // Sketch but works (updates without sending update)

    }
    internal void SetFromPlayerData(SaveAndLoad.PlayerData data)
    {
        _properties[pNAME] = data.userName;
        _properties[pCOINS] = data.nCoins;
        _properties[pPOINTS] = data.nPoints;
        _properties[pCOLOR] = data.playerColor;
        _properties[pTOWN] = data.curTown;
        _properties[pCARDS] = data.mCards.ToArray();
        _properties[pVISIBLE_TILES] = data.mVisibleTiles.ToArray();
        _properties[pHIDDEN_TILES] = data.mHiddenTiles.ToArray();
        endTown = data.endTown;
        Dictionary<string, bool> visited = new Dictionary<string, bool>();
        foreach (string town in GameConstants.townNames)
        {
            if (data.mVisited.Contains(town))
            {
                visited[town] = true;
            }
            else
            {
                visited[town] = false;
            }
        }
        _properties[pVISITED] = visited;
        UpdateDisplay();
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

    public bool IsMyTurn()
    {
        return (Game.currentGame != null) && (Game.currentGame.GetCurPlayer() == userName);
    }

    public bool IsLocalPlayer()
    {
        return userName == GameConstants.username;
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
            Debug.LogWarning("Town " + townName + " not found in visitedTowns");
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
        return GetPlayer(GameConstants.username);
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

    public static void ResetPlayers()
    {
        _players = new Dictionary<string, Player>();
        _players[GameConstants.username] = new Player(GameConstants.username);
    }

    #endregion
}
