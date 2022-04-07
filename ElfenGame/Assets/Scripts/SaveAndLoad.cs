using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveAndLoad
{
    [System.Serializable]
    public class GameData
    {
        public GameMode gameMode;
        public int numRounds;
        public bool endTown;
        public bool witch;
        public bool randGold;
        public List<CardEnum> deck;

        public List<CardEnum> visibleCards;
        public List<CardEnum> discard;
        public List<MovementTile> visible;
        public List<MovementTile> pile;
        public int curPlayerIndex;
        public List<string> players;
        // public string gameId;
        public string saveId;
        public GamePhase curPhase;
        public int curRound;
        public int passedPlayers;

        public int goldPileValue;

        public List<string> tilePaths;
        public List<MovementTile> tileTypes;

        public List<int> goldValues;
        public GameData(Game g, Tuple<List<string>, List<MovementTile>> tilePositions)
        {
            gameMode = g.gameMode;
            numRounds = g.maxRounds;
            endTown = g.endTown;
            witch = g.witchCard;
            randGold = g.randGold;
            goldPileValue = g.goldPileValue;
            deck = new List<CardEnum>(g.mDeck);
            visibleCards = g.visibleCards;
            discard = new List<CardEnum>(g.mDiscardPile);
            visible = new List<MovementTile>(g.mVisibleTiles);
            pile = new List<MovementTile>(g.mPile);
            curPlayerIndex = g.curPlayerIndex;
            players = new List<string>(g.mPlayers);
            saveId = g.saveId;
            curPhase = g.curPhase;
            curRound = g.curRound;
            passedPlayers = g.passedPlayers;
            this.tilePaths = tilePositions.Item1;
            this.tileTypes = tilePositions.Item2;
            goldValues = g.goldValues;
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
        public string endTown;
        public List<string> mVisited;
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
            endTown = p.endTown;
            mVisited = new List<string>(); // Can't store dict (JsonUtility doesn't support it) so just store the keys of visited towns
            foreach (KeyValuePair<string, bool> entry in p.mVisited)
            {
                if (entry.Value)
                {
                    mVisited.Add(entry.Key);
                }
            }
        }
    }

    public static HashSet<string> usedSaveIds = new HashSet<string>();
    public static Dictionary<string, HashSet<string>> localSavedIds = new Dictionary<string, HashSet<string>>();

    public static System.Random rng = new System.Random();

    public static string InitPlayerDir()
    {
        string path = Application.persistentDataPath + "/" + GameConstants.username + "/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return path;
    }

    public static void UpdateLocalSavedIds()
    {
        //TODO: Call this more often
        if (GameConstants.username == "") return;

        string path = InitPlayerDir();
        string[] files = Directory.GetFiles(path);
        HashSet<string> ids = new HashSet<string>();
        foreach (string file in files)
        {
            string id = Path.GetFileNameWithoutExtension(file);
            if (id.EndsWith("_player")) continue;
            ids.Add(id);
        }
        localSavedIds[GameConstants.username] = ids;
    }

    public static bool SaveAvail(string saveid)
    {
        if (GameConstants.username == "" || saveid == "") return false;
        return localSavedIds[GameConstants.username].Contains(saveid);
    }

    public static string GenerateSaveId()
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
        var stringChars = new char[6];

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[rng.Next(chars.Length)];
        }

        if (usedSaveIds.Contains(new string(stringChars)))
        {
            return GenerateSaveId();
        }
        else
        {
            usedSaveIds.Add(new string(stringChars));
            return new string(stringChars);
        }
    }

    public static void SaveGameState()
    {
        if (Game.currentGame.saveId == "")
        {
            Debug.LogError("SaveGameState called without a saveId");
            return;
        }

        string player_save_dir = InitPlayerDir();

        Tuple<List<string>, List<MovementTile>> tilePositions;
        if (MainUIManager.manager)
        {
            tilePositions = MainUIManager.manager.GetTilePositions();
        }
        else
        {
            tilePositions = new Tuple<List<string>, List<MovementTile>>(new List<string>(), new List<MovementTile>());
        }
        string gameJson = JsonUtility.ToJson(new GameData(Game.currentGame, tilePositions), false);

        string gamePath = player_save_dir + Game.currentGame.saveId + ".json";
        Debug.Log("Saving game to " + gamePath);
        File.WriteAllText(gamePath, gameJson);
    }

    public static void SaveLocalPlayerState()
    {
        if (Game.currentGame.saveId == "")
        {
            Debug.LogError("SaveLocalPlayerState called without a saveId");
            return;
        }
        string path = InitPlayerDir();
        string playerJson = JsonUtility.ToJson(new PlayerData(Player.GetLocalPlayer()), false);
        string playerPath = path + Game.currentGame.saveId + "_player.json";
        Debug.Log("Saving player to " + playerPath);
        File.WriteAllText(playerPath, playerJson);
    }

    public static GameData LoadGameState(string saveid)
    {
        string player_save_dir = InitPlayerDir();
        string gamePath = player_save_dir + saveid + ".json";
        Debug.Log("Loading game from " + gamePath);
        if (!File.Exists(gamePath))
        {
            Debug.LogError("No game found at " + gamePath);
            return null;
        }
        string gameJson = File.ReadAllText(gamePath);
        Debug.Log("Game JSON: " + gameJson);
        GameData data = JsonUtility.FromJson<GameData>(gameJson);
        Debug.Log("Done loading game from " + gamePath);
        return data;
    }

    public static void LoadLocalPlayerState(string saveid)
    {
        string player_save_dir = InitPlayerDir();
        string playerPath = player_save_dir + saveid + "_player.json";
        Debug.Log("Loading player from " + playerPath);
        if (!File.Exists(playerPath))
        {
            Debug.LogError("No player found at " + playerPath);
            return;
        }
        string playerJson = File.ReadAllText(playerPath);
        PlayerData data = JsonUtility.FromJson<PlayerData>(playerJson);
        Player.GetLocalPlayer().SetFromPlayerData(data);
        Debug.Log("Done loading player from " + playerPath);
    }

}