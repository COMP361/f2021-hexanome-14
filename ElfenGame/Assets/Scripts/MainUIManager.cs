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
    public GameObject tileGroup, tileWindow;

    [SerializeField]
    public GameObject cardPrefab;

    [SerializeField]
    public GameObject tilePrefab;

    [SerializeField]
    public GameObject tokenDisplayPrefab;

    [SerializeField]
    public GameObject endTurnButton;

    [SerializeField]
    private TextMeshProUGUI roundInfo;

    [SerializeField]
    public GameObject movementTileSpritePrefab;

    [SerializeField]
    public List<MovementTileSO> mTiles;

    [SerializeField]
    GameObject movementTileUIPrefab;

    [SerializeField]
    public GameObject mainCanvas;

    public Dictionary<MovementTile, MovementTileSO> mTileDict;

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
        UpdateAvailableTokens();
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
        tile.SetPlayer(p);

        GameObject tokenDisplayObject = Instantiate(tokenDisplayPrefab, mainCanvas.transform);
        PlayerVisibleTokenDisplay tokenDisplay = tokenDisplayObject.GetComponent<PlayerVisibleTokenDisplay>();
        tokenDisplay.SetName(username);

        tokenDisplayObject.transform.SetSiblingIndex(tokenDisplay.transform.GetSiblingIndex() - 1); // Ensure the pause menu is on top off view

        p.SetTokenDisplay(tokenDisplay);

        p.SetTile(tile);
        elf.LinkToPlayer(p);

        p.Reset();
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

    public void EndTurnTriggered()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        Game.currentGame.nextPlayer(passed : true);
    }

    public void UpdateRoundInfo()
    {
        roundInfo.text = $"Round: {Game.currentGame.curRound}/{Game.currentGame.maxRounds}\n" +
            $"Phase: {Enum.GetName(typeof(GamePhase), Game.currentGame.curPhase)}\n" +
            $"Turn: {Game.currentGame.GetCurPlayer()}";
    }

    public void SelectTokenPressed()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        { 
	        if (thscript.selected)
            {
                Game.currentGame.RemoveVisibleTile(thscript.tile.mTile);
                Player.GetLocalPlayer().AddVisibleTile(thscript.tile.mTile);
                break;
	        }
	    }
        Game.currentGame.nextPlayer();
    }

    public void SelectRandomTokenPressed()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        MovementTile tile = Game.currentGame.RemoveTileFromPile();
        Player.GetLocalPlayer().AddVisibleTile(tile);
        Game.currentGame.nextPlayer();
    }

    public void SelectCardsPressed()
    { 
        //TODO: Implement this
    }

    public void showTokenSelection()
    {
        tileWindow.SetActive(true);
        endTurnButton.interactable = false;
        endTurnButton.enabled = false;
    }

    public void hideTokenSelection()
    {
        tileWindow.SetActive(false);
        endTurnButton.interactable = true;
        endTurnButton.enabled = true;
    }

    public void UpdateAvailableTokens()
    {
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>() )
        {
            Destroy(thscript.gameObject);
	    }

        foreach (MovementTile tile in Game.currentGame.GetVisible())
        {
            GameObject g = Instantiate(tilePrefab, tileGroup.transform);

            TileHolderScript thscript = g.GetComponent<TileHolderScript>();
            thscript.SetTile(mTileDict[tile]);
	    }
    }

    public void SetTokensNotSelected()
    {
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        {
            thscript.selected = false;
            thscript.SetBackGroundColor();
	    }
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
            gm.AddNonObstacleTilesToDeck();
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
