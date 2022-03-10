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
    private const string pPILE = "PILE";
    private const string pVISIBLE = "VISIBLE";
    private const string pPOINTER = "POINTER";
    private const string pPLAYERS = "PLAYERS";
    private const string pCUR_PLAYER = "CUR_PLAYER";
    private const string pCUR_ROUND = "CUR_ROUND";
    private const string pCUR_PHASE = "CUR_PHASE";
    private const string pMAX_ROUNDS = "MAX_ROUNDS";

    private const string pPASSED_PLAYERS = "PASSED_PLAYERS";
    private const string pPATH_TILE = "PATH_TILE";

    public static Game currentGame = new Game();

    private List<CardEnum> deck;
    private List<MovementTile> pile;
    private List<MovementTile> visibleTiles;
    private int curCardPointer;
    private Hashtable onRoad;

    private List<string> players;
    private int _curPlayerIndex;
    private int _curRound;
    private int _maxRounds;
    private int _passedPlayers;
    private GamePhase _curPhase;

    public int curPlayerIndex
    {
        get
        {
            return _curPlayerIndex;
        }
        set
        {
            if (_curPlayerIndex != value && GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pCUR_PLAYER, value);
            _curPlayerIndex = value;
        }
    }

    public int passedPlayers
    {
        get
        {
            return _passedPlayers;
        }
        set
        {
            if (_passedPlayers != value && GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pPASSED_PLAYERS, value);
            _passedPlayers = value;
        }
    }

    public int maxRounds
    {
        get
        {
            return _maxRounds;
        }
        set
        {
            if (_maxRounds != value && GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pMAX_ROUNDS, value);
        }
    }

    public GamePhase curPhase
    {
        get
        {
            return _curPhase;
        }
        set
        {
            if (_curPhase != value && GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pCUR_PHASE, value);
            _curPhase = value;
        }
    }

    public int curRound
    {
        get
        {
            return _curRound;
        }
        set
        {
            if (_curRound != value && GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pCUR_ROUND, value);
            _curRound = value;
        }
    }

    public void SyncPile()
    {
        if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pPILE, pile.ToArray());
    }

    public void SyncVisible()
    {
        if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pVISIBLE, visibleTiles.ToArray());
    }

    public List<MovementTile> GetVisible()
    {
        return visibleTiles;
    }

    public void UpdatePile(List<MovementTile> newPile)
    {
        pile = newPile;
        // TODO: Update visualization of pile 
    }

    public void UpdateVisible(List<MovementTile> newVisible)
    {
        visibleTiles = newVisible;
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateAvailableTokens();
        // TODO: Update visualization of Visible tiles
    }

    public void Init(int maxRnds)
    {
        Debug.Log("Game Init Called");
        onRoad = new Hashtable();
	    InitDeck();
        InitPlayersList();
        InitPile();

        curPhase = GamePhase.HiddenCounter;

        for (int i = 0; i < players.Count; i++)
        {
            Player p = Player.GetPlayer(players[i]);

            for (int j = 0; j < 8; j++)
            {
                p.AddCard(Draw());
            }
            
            // Set Player Colors
            // TODO: Remove Later
            p.playerColor = (PlayerColor)i;

            p.AddHiddenTile(RemoveTileFromPile());
            p.AddHiddenTile(RemoveTileFromPile());
            p.AddVisibleTile(RemoveTileFromPile());
            p.AddVisibleTile(RemoveTileFromPile());
        }
        _curPlayerIndex = -1;
        curPlayerIndex = 0;
        _curRound = -1;
        curRound = 1;
        _maxRounds = -1;
        maxRounds = maxRnds;
    }

    private void InitPile()
    {
        pile = new List<MovementTile>();
        visibleTiles = new List<MovementTile>();

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
            visibleTiles.Add(pile[0]);
            pile.RemoveAt(0);
	    }

        SyncPile();
        SyncVisible();
    }

    public void InitPlayersList()
    {
        players = new List<string>();
        foreach (Player p in Player.GetAllPlayers())
        {
            players.Add(p.userName);
        }
        players.Shuffle();
        //Debug.Log(players.ToString());
        if (GameConstants.networkManager)
        {
            GameConstants.networkManager.SetGameProperty(pPLAYERS, players.ToArray());
	    }
    }

    public List<string> GetPlayerList() { return players; }


    private void InitDeck()
    {
        // ElfenLand (might be different for elvengold)
        curCardPointer = 0;
        deck = new List<CardEnum>();
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
        if (GameConstants.networkManager)
        {
            GameConstants.networkManager.SetGameProperty(pPOINTER, curCardPointer);
            GameConstants.networkManager.SetGameProperty(pDECK, deck.ToArray());
        }
    }

    private void YourTurn()
    {

    }

    private void HandlePlayerUpdate(int idx)
    {
        _curPlayerIndex = idx;
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        if (Player.GetLocalPlayer().IsMyTurn()) YourTurn();
    }

    private void HandlePhaseUpdate(GamePhase phase)
    {
        _curPhase = phase;
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        if (curPhase == GamePhase.DrawCounters1 || curPhase == GamePhase.DrawCounters2 || curPhase == GamePhase.DrawCounters3)
        {
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.showTokenSelection();
        } else
        {
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.hideTokenSelection();
        }
        // Debug.LogError($"Cur Phase set to {Enum.GetName(typeof(GamePhase), curPhase)}");
    }

    public void UpdateProperties(string key, object data)
    {
        if (key == pDECK)
        {
            deck = ((CardEnum[])data).ToList();
        }
        else if (key == pPOINTER)
        {
            curCardPointer = (int)data;
        }
        else if (key == pPLAYERS)
        {
            players = ((string[])data).ToList();
        }
        else if (key == pCUR_PLAYER)
        {
            HandlePlayerUpdate((int)data);
            // Debug.LogError($"The current Player is {Game.currentGame.GetCurPlayer()}");
        }
        else if (key == pCUR_ROUND)
        {
            _curRound = (int)data;
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        }
        else if (key == pPASSED_PLAYERS)
        {
            _passedPlayers = (int)data;
        }
        else if (key == pCUR_PHASE)
        {
            HandlePhaseUpdate((GamePhase)data);
        }
        else if (key == pMAX_ROUNDS)
        {
            _maxRounds = (int)data;
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        }
        else if (key == pPILE)
        {
            UpdatePile(((MovementTile[])data).ToList());
	    }
        else if (key == pVISIBLE)
        {
            UpdateVisible(((MovementTile[])data).ToList());
	    }
    }

    public void RemoveVisibleTile(MovementTile movementTile)
    {
        int index = visibleTiles.IndexOf(movementTile);
        if (index == -1) return;

        pile.Add(movementTile);
        visibleTiles[index] = pile[0];
        pile.RemoveAt(0);
        SyncPile();
        SyncVisible();
    }

    public void AddTileToPile(MovementTile tile)
    {
        pile.Add(tile);
        SyncPile();
    }


    public MovementTile RemoveTileFromPile()
    {
        MovementTile ret = pile[0];
        pile.RemoveAt(0);
        SyncPile();
        return ret; 
    }

    public void nextPlayer(bool passed = false)
    {
        curPlayerIndex = (curPlayerIndex + 1) % players.Count;
        // Debug.LogError($"Current Player {GetCurPlayer()}");
        Debug.LogError($"Current Player Index {curPlayerIndex}");

        if (curPhase == GamePhase.PlaceCounter && passed) 
        {
            passedPlayers += 1;
        } else
        {
            passedPlayers = 0;
        }
        if ((curPlayerIndex == (curRound-1) % players.Count && curPhase != GamePhase.PlaceCounter) || (passedPlayers == players.Count))
        {
            if (curPhase == GamePhase.Travel)
            {
                if (curRound == maxRounds)
                {
                    // TODO: Game Over
                    Debug.LogError($"Game Over");
                }
                else
                {
                    if (GameConstants.mainUIManager) GameConstants.mainUIManager.ClearAllTiles();
                    curPhase = GamePhase.HiddenCounter;
                    curRound = curRound + 1;
                    curPlayerIndex = (curPlayerIndex + 1) % players.Count;
                }
            }
            else
            {
                curPhase++;
            }

            Debug.LogError($"Cur Round is: {curRound}"); 
        }
    }

    public CardEnum Draw()
    { 
       if (curCardPointer >= deck.Count)
        { 
            deck.Shuffle();
            if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pDECK, deck.ToArray());
            curCardPointer = 0;
	    }

        CardEnum ret = deck[curCardPointer];
        curCardPointer++;

        if (GameConstants.networkManager) GameConstants.networkManager.SetGameProperty(pPOINTER, curCardPointer);


        return ret;
    }

    public void AddTileToRoad(string roadName, MovementTile movementTile)
    { 
        if (onRoad.ContainsKey(roadName))
        {
            MovementTile[] tilesOnRoad = (MovementTile[]) onRoad[roadName];
            Array.Resize(ref tilesOnRoad, tilesOnRoad.Length + 1);
            tilesOnRoad[tilesOnRoad.Length - 1] = movementTile;
            onRoad[roadName] = tilesOnRoad;
	    } else
        {
            onRoad[roadName] = new MovementTile[] { movementTile };
	    }
    }

    public void ClearTilesOnAllRoads()
    {
        string[] toClear = new string[onRoad.Keys.Count];
        onRoad.Keys.CopyTo(toClear, 0); 
        foreach (string roadName in toClear)
        {
            onRoad.Remove(roadName);
        }
    }


    public string GetCurPlayer()
    {
        return players[_curPlayerIndex];
    }


}
