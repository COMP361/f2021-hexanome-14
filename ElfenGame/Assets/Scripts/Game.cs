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

    public void Init(int maxRnds)
    {
        Debug.Log("Game Init Called");
        onRoad = new Hashtable();
	    InitDeck();
        InitPlayersList();

        curPhase = GamePhase.HideCounter;

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

            p.AddTile(MovementTile.GiantPig);
            p.AddTile(MovementTile.GiantPig);
            p.AddTile(MovementTile.GiantPig);
            p.AddTile(MovementTile.TrollWagon);
            p.AddTile(MovementTile.Unicorn);
        }
        _curPlayerIndex = -1;
        curPlayerIndex = 0;
        _curRound = -1;
        curRound = 1;
        _maxRounds = -1;
        maxRounds = maxRnds;
    }

    public void InitPlayersList()
    {
        players = new List<string>();
        foreach (Player p in Player.GetAllPlayers())
        {
            players.Add(p.userName);
        }
        players.Shuffle();
        Debug.Log(players.ToString());
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
            _curPlayerIndex = (int)data;
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
            Debug.LogError($"The current Player is {Game.currentGame.GetCurPlayer()}");
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
            curPhase = (GamePhase)data;
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
            Debug.LogError($"Cur Phase set to {Enum.GetName(typeof(GamePhase), curPhase)}");
        }
        else if (key == pMAX_ROUNDS)
        {
            _maxRounds = (int)data;
            if (GameConstants.mainUIManager) GameConstants.mainUIManager.UpdateRoundInfo();
        }
    }

    public void nextPlayer(bool pass)
    {
        curPlayerIndex = (curPlayerIndex + 1) % players.Count;
        // Debug.LogError($"Current Player {GetCurPlayer()}");
        Debug.LogError($"Current Player Index {curPlayerIndex}");
        if (pass) 
        {
            passedPlayers += 1;
        } else
        {
            passedPlayers = 0;
        }
        if ((curPlayerIndex == (curRound-1) % players.Count && curPhase != GamePhase.PlaceCounter) || (passedPlayers == players.Count))
        {
            passedPlayers = 0;
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
                    curPhase = GamePhase.HideCounter;
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
