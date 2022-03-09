using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUIManager : MonoBehaviour
{
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject elfPrefab;

    [SerializeField]
    public GameObject leftPane;

    [SerializeField]
    public GameObject cardPanel;

    [SerializeField]
    public GameObject cardPrefab;

    [SerializeField]
    public GameObject confirmButton;

    [SerializeField]
    private TextMeshProUGUI roundInfo;

    [SerializeField]
    public GameObject movementTileSpritePrefab;

    [SerializeField]
    public List<MovementTileSO> mTiles;

    [SerializeField]
    GameObject movementTileUIPrefab;

    private Dictionary<MovementTile, MovementTileSO> mTileDict;

    private bool isPaused = false;
    private bool isViewingCards = false;
    // Start is called before the first frame update
    void Start()
    {
        mTileDict = new Dictionary<MovementTile, MovementTileSO>();
        foreach (MovementTileSO tile in mTiles)
        {
            mTileDict[tile.mTile] = tile;
            GameObject newTile = Instantiate(movementTileUIPrefab, GameConstants.tileGroup.transform);
            newTile.GetComponent<MovementTileUIScript>().SetTileSO(tile);
        }
        UpdateCardHand();

        if (GameConstants.networkManager) GameConstants.networkManager.verifyAllPlayersExist();
        GameConstants.townDict = null; // Force reset of town Dict
        GameConstants.roadDict = null;
        foreach (string playerName in Game.currentGame.GetPlayerList())
        {
            InitPlayer(playerName);
        }

        UpdateRoundInfo();
        foreach (NewTown town in GameConstants.townDict.Values)
        {
            town.DisplayVisited();
	    }
    }

    public void InitPlayer(string username)
    {
        Debug.LogError($"Creating Player {username}, Local Username = {Lobby.myUsername}");
        GameObject elfObject = Instantiate(elfPrefab);

        Player p = Player.GetOrCreatePlayer(username);
        Elf elf = elfObject.GetComponent<Elf>();

        GameObject g = Instantiate(playerPrefab, leftPane.transform);
        PlayerTile tile = g.GetComponent<PlayerTile>();

        p.SetTile(tile);
        elf.LinkToPlayer(p);

        p.curTown = "TownElvenhold";

    }

    public void OnPausePressed()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
    }

    public void exitGameClicked()
    {
        GameConstants.networkManager.LeaveRoom();
    }

    public void OnShowCardHandPressed()
    {
        isViewingCards = !isViewingCards;
        cardPanel.SetActive(isViewingCards);
    }

    public void DoneMove()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        Game.currentGame.nextPlayer(Game.currentGame.curPhase == GamePhase.PlaceCounter);
    }

    public void UpdateRoundInfo()
    {
        roundInfo.text = $"Round: {Game.currentGame.curRound}/{Game.currentGame.maxRounds}\n" +
            $"Phase: {Enum.GetName(typeof(GamePhase), Game.currentGame.curPhase)}\n" +
            $"Turn: {Game.currentGame.GetCurPlayer()}";
    }


    public void UpdateCardHand()
    { 
   
	    foreach (Card card in cardPanel.GetComponentsInChildren<Card>())
        {
            Destroy(card.gameObject);
		}

        Transform cardGroup = cardPanel.GetComponentInChildren<GridLayoutGroup>().transform;	
	    foreach (CardEnum c in Player.GetLocalPlayer().mCards)
        {
            GameObject g = Instantiate(cardPrefab, cardGroup);
            Card card = g.GetComponent<Card>();

            card.Initialize(c);

        }
    }


    public void ClearAllTiles()
    {
        if (GameConstants.roadGroup == null) return;
        foreach (GridManager gm in GameConstants.roadGroup.GetComponentsInChildren<GridManager>())
        {
            gm.Clear();
        }
    }

    internal void AddTile(string roadName, MovementTile movementTile)
    {
        PathScript pathScript = GameConstants.roadDict[roadName];
        GameObject newTileSprite = Instantiate(movementTileSpritePrefab);
        MovementTileSpriteScript spriteScript = newTileSprite.GetComponent<MovementTileSpriteScript>();
        spriteScript.SetTileSO(mTileDict[movementTile]);

        _ = pathScript.GetComponentInChildren<GridManager>().AddElement(newTileSprite);
    }
}
