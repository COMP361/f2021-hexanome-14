using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveAndLoad
{
    [System.Serializable]
    public struct GameData
    {
        public string gameMode;
        public int numRounds;
        public bool endTown;
        public bool witch;
        public bool randGold;

        public List<CardEnum> deck;

        public List<CardEnum> discard;

        public List<MovementTile> visible;

        public List<MovementTile> pile;

        public int curPlayerIndex;

        public List<string> players;

        public string gameId;

        public GamePhase curPhase;

        public int curRound;

        public int passedPlayers;


        public GameData(Game g)
        {
            gameMode = g.gameMode;
            numRounds = g.maxRounds;
            endTown = g.endTown;
            witch = g.witchCard;
            randGold = g.randGold;

            deck = new List<CardEnum>(g.mDeck);

            discard = new List<CardEnum>(g.mDiscardPile);

            visible = new List<MovementTile>(g.mVisibleTiles);

            pile = new List<MovementTile>(g.mPile);

            curPlayerIndex = g.curPlayerIndex;

            players = new List<string>(g.mPlayers);

            gameId = g.gameId;

            curPhase = g.curPhase;

            curRound = g.curRound;

            passedPlayers = g.passedPlayers;
        }
    }

    [System.Serializable]
    public struct PlayerData
    {
        // store data from Player Class

        public int nCoins;

        public int nPoints;

        public PlayerColor playerColor;

        public List<CardEnum> mCards;

        public List<MovementTile> mVisibleTiles;

        public List<MovementTile> mHiddenTiles;

        public string userName;

        public string curTown;

        public Dictionary<string, bool> mVisited;

        public PlayerData(Player p)
        {
            nCoins = p.nCoins;
            nPoints = p.nPoints;
            playerColor = p.playerColor;
            mCards = p.mCards;
            mVisibleTiles = p.mVisibleTiles;
            mHiddenTiles = p.mHiddenTiles;
            userName = p.userName;
            curTown = p.curTown;
            mVisited = p.mVisited;
        }
    }


    public static void SetFromJSON(string jsonString)
    {
        GameData data = JsonUtility.FromJson<GameData>(jsonString);

        Game.currentGame.SetFromGameData(data);
    }

    public static void SaveGame()
    {
        string gameJson = JsonUtility.ToJson(new GameData(Game.currentGame), false);
        string playerJson = JsonUtility.ToJson(new PlayerData(Player.GetLocalPlayer()), false);

        string gamePath = Application.persistentDataPath + "/" + Lobby.myUsername + "/" + Game.currentGame.gameId + ".json";
        string playerPath = Application.persistentDataPath + "/" + Lobby.myUsername + "/" + Game.currentGame.gameId + "_player.json";
        if (!Directory.Exists(Application.persistentDataPath + "/" + Lobby.myUsername))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/" + Lobby.myUsername);
        }
        Debug.Log("Saving game to " + gamePath);
        File.WriteAllText(gamePath, gameJson);
        Debug.Log("Saving player to " + playerPath);
        File.WriteAllText(playerPath, playerJson);
    }

}