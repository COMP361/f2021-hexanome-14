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

    public static Game currentGame = new Game();

    private List<CardEnum> deck;
    private int curCardPointer; 

    private List<String> players;
    private int curPlayerIndex;

    public void Init()
    {
        Debug.Log("Game Init Called");
        InitDeck();
        InitPlayers();


        for (int i = 0; i < players.Count; i++)
        {
            Player p = Player.GetPlayer(players[i]);

            for (int j = 0; j < 8; j++)
            {
                p.AddCard(Draw());
            }
	    }
    }

    private void InitPlayers()
    {
        players = new List<string>();
	    
	    foreach(Player p in Player.GetAllPlayers())
        {
            players.Add(p.userName);
	    }

        players.Shuffle();
        Debug.Log(players.ToString());
        curPlayerIndex = 0;
    }


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
        } else if (key == pPOINTER)
        {
            curCardPointer = (int)data;
	    } else if (key == pPLAYERS)
        {
            players = ((string[])data).ToList();
	    } else if (key == pCUR_PLAYER)
        {
            curPlayerIndex = (int)data;
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



}
