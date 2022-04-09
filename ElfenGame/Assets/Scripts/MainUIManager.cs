using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static AuctionItem;

public class MainUIManager : MonoBehaviour
{
    #region singleton 

    private static MainUIManager _instance;

    public static MainUIManager manager
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainUIManager>();
            }
            return _instance;
        }
    }

    #endregion   

    #region Serialized Fields
    [SerializeField]
    private GameObject pausePanel;

    [SerializeField]
    private GameObject chooseColorPanel;

    [SerializeField]
    private GameObject gameOverScreen;

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GameObject elfPrefab;

    [SerializeField]
    public GameObject leftPane;

    [SerializeField]
    public GameObject cardPanel;

    [SerializeField]
    public GameObject drawCardPanel;

    [SerializeField]
    public GameObject tileGroup, tileWindow;

    [SerializeField]
    public GameObject cardPrefab;

    [SerializeField]
    public GameObject availableCardPrefab;

    [SerializeField]
    public Button claimGoldButton;

    [SerializeField]
    public GameObject availableCardGroup;

    [SerializeField]
    public GameObject tilePrefab;

    [SerializeField]
    public GameObject tokenDisplayPrefab;

    [SerializeField]
    public GameObject tokenToKeepSelectionWindow;

    [SerializeField]
    public Button endTurnButton;

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

    [SerializeField]
    public TMP_Dropdown colorSelectionDD;

    [SerializeField]
    public GameObject firstPlace, secondPlace, thirdPlace;

    [SerializeField]
    public TMP_Text firstPlaceText, secondPlaceText, thirdPlaceText;

    [SerializeField]
    public Image firstPlaceSprite, secondPlaceSprite, thirdPlaceSprite;

    [SerializeField]
    public Text goldPileValue;

    public Image volumeHandleImage;

    public Slider volumeSlider2;


    [Header("Auction Fields")]

    public GameObject auctionGroup;
    public GameObject auctionPanel;
    public GameObject auctionItemPrefab;

    public Text curBestBidText;
    public Image curBestBidSprite;
    public Text curBestBidderText;
    public Text curBidText;
    public Text bidStatusText;

    #endregion


    public Dictionary<MovementTile, MovementTileSO> mTileDict;

    private bool isPaused = false;
    private List<string> elfNames = new List<string> { "Blue", "Cyan", "Red", "Orange", "Pink", "Green" };
    private List<AuctionItem> auctionItems = new List<AuctionItem>();
    private AuctionItem currentAuctionItem;
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

        if (NetworkManager.manager) NetworkManager.manager.verifyAllPlayersExist(); // TODO: Remove this line
        GameConstants.townDict = null; // Force reset of town Dict
        GameConstants.roadDict = null;
        foreach (string playerName in Game.currentGame.mPlayers)
        {
            InitPlayer(playerName);
        }

        Game.currentGame.UpdateDisplay();

        foreach (NewTown town in GameConstants.townDict.Values)
        {
            town.DisplayVisited();
        }

        if (Player.GetLocalPlayer().playerColor == PlayerColor.None)
            chooseColorPanel.SetActive(true);
        UpdateColorOptions();

        SaveAndLoad.GameData data = SaveAndLoad.LoadGameState(Game.currentGame.saveId);
        if (data != null)
        {
            SetTiles(data.tilePaths, data.tileTypes);
        }
        float volume = PlayerPrefs.GetFloat("volume");
        volumeSlider2.value = volume;

        UpdateEndTown(Player.GetLocalPlayer().endTown);
        UpdateGoldValues();
    }

    public void UpdateColorOptions()
    {
        if (!chooseColorPanel.activeSelf) return;
        colorSelectionDD.ClearOptions();
        List<TMP_Dropdown.OptionData> newOptions = new List<TMP_Dropdown.OptionData>();

        foreach (PlayerColor c in Game.currentGame.mAvailableColors)
        {
            newOptions.Add(new TMP_Dropdown.OptionData(elfNames[(int)c], c.GetSprite()));
        }
        colorSelectionDD.AddOptions(newOptions);

        OnColorSelectionChanged();
    }

    public void UpdatePlayerPointDisplay()
    {
        foreach (NewTown town in GameConstants.townDict.Values)
        {
            town.DisplayVisited();
        }
    }


    public void InitPlayer(string username)
    {
        //Debug.LogError($"Creating Player {username}, Local Username = {GameConstants.username}");

        Player p = Player.GetOrCreatePlayer(username); //TODO: remove this line

        //Elf
        GameObject elfObject = Instantiate(elfPrefab);
        Elf elf = elfObject.GetComponent<Elf>();
        elf.LinkToPlayer(p);
        p.elf = elf;

        //Player Tile
        GameObject g = Instantiate(playerPrefab, leftPane.transform);
        PlayerTile tile = g.GetComponent<PlayerTile>();
        tile.SetPlayer(p);
        p.tile = tile;

        // Token Display
        GameObject tokenDisplayObject = Instantiate(tokenDisplayPrefab, mainCanvas.transform);
        PlayerVisibleTokenDisplay tokenDisplay = tokenDisplayObject.GetComponent<PlayerVisibleTokenDisplay>();
        tokenDisplay.SetName(username);
        tokenDisplayObject.transform.SetSiblingIndex(tokenDisplay.transform.GetSiblingIndex() - 1); // Ensure the pause menu is on top of view
        p.tokenDisplay = tokenDisplay;

        // Update ALL UI with current player stats
        p.UpdateDisplay();
    }

    internal void UpdateEndTown(string value)
    {
        if (value == null || value == "") return;
        GameConstants.townDict[value].SetEndTown();
    }

    internal Tuple<List<string>, List<MovementTile>> GetTilePositions()
    {
        List<string> tilePaths = new List<string>();
        List<MovementTile> tiles = new List<MovementTile>();
        foreach (PathScript path in GameConstants.roadDict.Values)
        {
            GridManager grid = path.GetComponentInChildren<GridManager>();
            foreach (MovementTile tile in grid.GetNonObstacleTiles())
            {
                tilePaths.Add(path.name);
                tiles.Add(tile);
            }

            if (grid.HasObstacle())
            {
                if (path.roadType == RoadType.River || path.roadType == RoadType.Lake)
                {
                    tilePaths.Add(path.name);
                    tiles.Add(MovementTile.WaterObstacle);

                }
                else
                {
                    tilePaths.Add(path.name);
                    tiles.Add(MovementTile.RoadObstacle);
                }
            }
        }

        return new Tuple<List<string>, List<MovementTile>>(tilePaths, tiles);
    }

    internal void SetTiles(List<string> tilePaths, List<MovementTile> tileTypes)
    {
        for (int i = 0; i < tilePaths.Count; i++)
        {
            GameObject g = Instantiate(movementTileSpritePrefab);
            MovementTileSpriteScript sprite = g.GetComponent<MovementTileSpriteScript>();
            sprite.SetTileSO(mTileDict[tileTypes[i]]);

            GridManager grid = GameConstants.roadDict[tilePaths[i]].GetComponent<GridManager>();
            grid.AddElement(g);
        }
    }

    public void OnColorSelectionChanged()
    {
        int index = elfNames.IndexOf(colorSelectionDD.options[colorSelectionDD.value].text);
        HelpElfManager.elf.SetSprite(colorSelectionDD.options[colorSelectionDD.value].image);
    }

    public void OnPausePressed()
    {
        isPaused = !isPaused;

        pausePanel.SetActive(isPaused);
    }

    public void OnConfirmColor()
    {
        int index = elfNames.IndexOf(colorSelectionDD.options[colorSelectionDD.value].text);
        Game.currentGame.ClaimColor((PlayerColor)index);
    }

    public void CloseColorSelection()
    {
        chooseColorPanel.SetActive(false);
        HelpElfManager.elf.SetSprite(Player.GetLocalPlayer().playerColor.GetSprite());
    }

    public void exitGameClicked()
    {
        NetworkManager.manager.LeaveRoom();
    }

    public void OnShowCardHandPressed()
    {
        cardPanel.SetActive(!cardPanel.activeSelf);
    }

    public void EndTurnTriggered()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        if (Game.currentGame.curPhase == GamePhase.SelectTokenToKeep) return;
        Game.currentGame.nextPlayer(passed: true);
    }

    public void UpdateRoundInfo()
    {
        roundInfo.text = $"Round: {Game.currentGame.curRound}/{Game.currentGame.maxRounds}\n" +
            $"Phase: {Enum.GetName(typeof(GamePhase), Game.currentGame.curPhase)}\n" +
            $"Turn: {Game.currentGame.GetCurPlayer()}";
    }

    public void SelectTokenPressed()
    {
        bool foundSelected = false;
        int index = 0;
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        {
            if (thscript.selected)
            {
                MovementTile movementTile = Game.currentGame.RemoveVisibleTile(index);
                Player.GetLocalPlayer().AddVisibleTile(movementTile);
                foundSelected = true;
                break;
            }
            index++;
        }
        if (foundSelected) Game.currentGame.nextPlayer();
    }

    public void DrawFromDeckPressed()
    {
        if (!Player.GetLocalPlayer().IsMyTurn() || Player.GetLocalPlayer().cardsToDraw == 0) return;
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        CardEnum[] cards = Game.currentGame.Draw(1);

        // check if card is golold card
        if (cards[0] == CardEnum.Gold)
        {
            Game.currentGame.goldPileValue += 3;
            Game.currentGame.SyncGameProperties();
        }
        else
        {
            Player.GetLocalPlayer().AddCards(cards);
            CardDrawn();
        }
    }

    public void SelectRandomTokenPressed()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;
        MovementTile tile = Game.currentGame.RemoveTileFromPile();
        Player.GetLocalPlayer().AddVisibleTile(tile);
        Game.currentGame.nextPlayer();
    }

    private void CardDrawn()
    {
        Player.GetLocalPlayer().cardsToDraw--;

        if (Player.GetLocalPlayer().cardsToDraw == 0)
        {
            Player.GetLocalPlayer().cardsToDraw = 3;
            Game.currentGame.nextPlayer();
        }
    }

    public void OnClaimGoldSelected()
    {
        if (!Player.GetLocalPlayer().IsMyTurn() || Player.GetLocalPlayer().cardsToDraw == 0) return;
        Player.GetLocalPlayer().nCoins += Game.currentGame.goldPileValue;
        // Shuffle one gold card for every three coins into the discardPile
        for (int i = 0; i < Game.currentGame.goldPileValue / 3; i++)
        {
            Game.currentGame.mDiscardPile.Add(CardEnum.Gold);
        }
        Game.currentGame.goldPileValue = 0;
        Game.currentGame.SyncGameProperties();
        CardDrawn();
    }

    private TileHolderScript GetSelectedTokenToKeep()
    {
        foreach (TileHolderScript thscript in tokenToKeepSelectionWindow.GetComponentsInChildren<TileHolderScript>())
        {
            if (thscript.selected) return thscript;
        }
        return null;
    }

    // for draw card
    private int GetSelectedCard()
    {
        int index = 0;
        foreach (Card card in availableCardGroup.GetComponentsInChildren<Card>())
        {
            if (card.selected) return index;
            index++;
        }
        return -1;
    }

    public void SelectTokenToKeepPressed()
    {
        Player localPlayer = Player.GetLocalPlayer();
        if (!localPlayer.IsMyTurn()) return;

        TileHolderScript thscript = GetSelectedTokenToKeep();
        if (thscript == null) return;

        if (thscript.tile.mTile == MovementTile.RoadObstacle) return; // Select non obstacle tile
        localPlayer.SetOnlyTile(thscript.tile.mTile, thscript.GetInVisibleTokens());
        Game.currentGame.nextPlayer();
    }

    // for draw cards
    public void OnSelectCardPressed()
    {
        // check if it's your turn
        if (!Player.GetLocalPlayer().IsMyTurn() || Player.GetLocalPlayer().cardsToDraw == 0) return;
        int index = GetSelectedCard();
        if (index == -1) return;

        CardEnum card = Game.currentGame.RemoveVisibleCard(index);

        Player.GetLocalPlayer().AddCards(new CardEnum[] { card });
        CardDrawn();
        Game.currentGame.SyncGameProperties();
    }

    public void SelectCardsPressed()
    {
        cardPanel.SetActive(false);

        if (Game.currentGame.curPhase != GamePhase.Travel) return;

        foreach (PathScript path in GameConstants.roadDict.Values)
        {
            path.ColorByMoveValidity(GameConstants.townDict[Player.GetLocalPlayer().curTown], GetSelectedCards());
        }
    }

    public void ResetRoadColors()
    {
        foreach (PathScript path in GameConstants.roadDict.Values)
        {
            path.ResetColor();
        }
    }

    public List<CardEnum> GetSelectedCards()
    {
        List<CardEnum> cards = new List<CardEnum>();
        foreach (Card cardScript in cardPanel.GetComponentsInChildren<Card>())
        {
            if (cardScript.selected) cards.Add(cardScript.cardType);
        }

        return cards;
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

    public void showAvailableTokensToKeep()
    {
        UpdateTokenToKeep();
        tokenToKeepSelectionWindow.SetActive(true);
    }

    public void hideAvailableTokensToKeep()
    {
        tokenToKeepSelectionWindow.SetActive(false);
    }

    public void UpdateTokenToKeep()
    {
        GridLayoutGroup gridGroup = tokenToKeepSelectionWindow.GetComponentInChildren<GridLayoutGroup>();
        foreach (TileHolderScript thscript in gridGroup.GetComponentsInChildren<TileHolderScript>())
        {
            Destroy(thscript.gameObject);
        }

        foreach (MovementTile tile in Player.GetLocalPlayer().mVisibleTiles)
        {
            // For elfenland, only show tokens that are not obstacles
            if (tile == MovementTile.RoadObstacle) continue;

            GameObject g = Instantiate(tilePrefab, gridGroup.transform);

            TileHolderScript thscript = g.GetComponent<TileHolderScript>();
            thscript.SetTile(mTileDict[tile]);
            thscript.SetIsSelectable(true);
            thscript.SetInVisibleTokens(true);
        }

        foreach (MovementTile tile in Player.GetLocalPlayer().mHiddenTiles)
        {
            GameObject g = Instantiate(tilePrefab, gridGroup.transform);

            TileHolderScript thscript = g.GetComponent<TileHolderScript>();
            thscript.SetTile(mTileDict[tile]);
            thscript.SetIsSelectable(true);
            thscript.SetInVisibleTokens(false);
        }
    }

    public void UpdateMovementTileCounts()
    {
        foreach (MovementTileUIScript mtScript in GameConstants.tileGroup.GetComponentsInChildren<MovementTileUIScript>())
        {
            mtScript.UpdateText();
        }
    }

    public void UpdateAvailableTokens()
    {
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        {
            Destroy(thscript.gameObject);
        }

        foreach (MovementTile tile in Game.currentGame.mVisibleTiles)
        {
            GameObject g = Instantiate(tilePrefab, tileGroup.transform);

            TileHolderScript thscript = g.GetComponent<TileHolderScript>();
            thscript.SetTile(mTileDict[tile]);
            thscript.SetIsSelectable(true);
        }
    }

    public void DrawCardPanelToggle(bool active)
    {
        drawCardPanel.SetActive(active);
    }

    public void UpdateAvailableCards()
    {
        foreach (Card cardScript in availableCardGroup.GetComponentsInChildren<Card>())
        {
            Destroy(cardScript.gameObject);
        }

        foreach (CardEnum card in Game.currentGame.visibleCards)
        {
            GameObject g = Instantiate(availableCardPrefab, availableCardGroup.transform);

            Card cardScript = g.GetComponent<Card>();
            cardScript.Initialize(card);
            if (cardScript.cardType == CardEnum.Witch){
                HelpMessage hm = g.AddComponent(typeof(HelpMessage)) as HelpMessage;
                hm.helpMessage = "Witch is used to jump to a town selected, by using 3 gold coins or skip an obstacle for 1 coin";
            }
        }
    }

    public void SetTokensNotSelected()
    {
        foreach (TileHolderScript thscript in tileGroup.GetComponentsInChildren<TileHolderScript>())
        {
            thscript.selected = false;
            thscript.SetBackGroundColor();
        }

        foreach (TileHolderScript thscript in tokenToKeepSelectionWindow.GetComponentsInChildren<TileHolderScript>())
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
            if (card.cardType == CardEnum.Witch){
                HelpMessage hm = g.AddComponent(typeof(HelpMessage)) as HelpMessage;
                hm.helpMessage = "Witch is used to jump to a town selected, by using 3 gold coins or skip an obstacle for 1 coin";
            }
        }
    }


    public List<MovementTile> ClearAllTiles()
    {
        if (GameConstants.roadGroup == null) return null;
        List<MovementTile> tiles = new List<MovementTile>();
        foreach (GridManager gm in GameConstants.roadGroup.GetComponentsInChildren<GridManager>())
        {
            tiles.AddRange(gm.GetNonObstacleTiles());
            gm.Clear();
        }

        return tiles;
    }

    internal void UpdateGoldValues()
    {
        Debug.Log($"UpdateGoldValues called with gamemode: {Enum.GetName(typeof(GameMode), Game.currentGame.gameMode)}");
        if (Game.currentGame.gameMode != GameMode.Elfengold) return; // Only display gold for elvenhold
        List<int> goldValues = Game.currentGame.goldValues;
        if (goldValues.Count != GameConstants.townNames.Count - 1) return;

        int index = 0;
        foreach (string townName in GameConstants.townNames)
        {
            NewTown town = GameConstants.townDict[townName];
            if (town == null) return;

            if (townName == "TownElvenhold")
            {
                town.SetGold(0);
                continue;
            }
            town.SetGold(goldValues[index]);

            index++;
        }

        goldPileValue.text = $"Gold: {Game.currentGame.goldPileValue}";
        if (Game.currentGame.goldPileValue > 0)
        {
            goldPileValue.color = Color.yellow;
            claimGoldButton.interactable = true;
        }
        else
        {
            goldPileValue.color = Color.grey;
            claimGoldButton.interactable = false;
        }
    }

    public void SetVolume(float volume)
    {
        float curVolume = AudioManager.manager.GetVolume();
        if (volume <= -30)
        {
            volume = -80; // -80 is the minimum value for the audio mixer
            volumeHandleImage.sprite = Resources.Load<Sprite>("SoundOff");
        }
        else if (curVolume == -80)
        {
            // Currently set to -80 and being changed to something higher
            volumeHandleImage.sprite = Resources.Load<Sprite>("SoundOn");
        }
        AudioManager.manager.SetVolume(volume);
        PlayerPrefs.SetFloat("volume", volume);
        PlayerPrefs.Save();
    }

    public void GameOverTriggered(List<Player> winners, List<int> scores)
    {
        Debug.Log($"Num winners: {winners.Count}");
        Debug.Log($"Num scores: {scores.Count}");
        gameOverScreen.SetActive(true);

        firstPlace.SetActive(true);
        firstPlaceSprite.sprite = winners[0].playerColor.GetSprite();
        firstPlaceText.text = $"{winners[0].userName}\nScore: {scores[0]}";

        if (winners.Count > 1)
        {
            secondPlace.SetActive(true);
            secondPlaceSprite.sprite = winners[1].playerColor.GetSprite();
            secondPlaceText.text = $"{winners[1].userName}\nScore: {scores[1]}";
        }

        if (winners.Count > 2)
        {
            thirdPlace.SetActive(true);
            thirdPlaceSprite.sprite = winners[2].playerColor.GetSprite();
            thirdPlaceText.text = $"{winners[2].userName}\nScore: {scores[2]}";

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







    #region Auction Screen

    public void OnAuctionBidUpArrowClicked()
    {
        int curBidAmount = int.Parse(curBidText.text);
        curBidText.text = (1 + curBidAmount).ToString();
    }

    public void OnAuctionBidDownArrowClicked()
    {
        int curBidAmount = int.Parse(curBidText.text);
        if (curBidAmount > 0)
            curBidText.text = (curBidAmount - 1).ToString();
    }

    //AUCTION SCREEN
    // Show auction screen that displays all tiles that will be up for auction.

    // TODO call this from somewhere
    public void ShowAuctionScreen()
    {
        auctionPanel.SetActive(true);
        endTurnButton.interactable = false;
        endTurnButton.enabled = false;
    }

    // Go to the next item in the auction
    private void GoToNextAuctionItem()
    {
        // get the next item
        currentAuctionItem = auctionItems[0];
        // remove it from the list
        auctionItems.RemoveAt(0);
        // update the tile image in the UI
        auctionPanel.GetComponent<SpriteRenderer>().sprite = currentAuctionItem.tile.mImage;
        // reset the user's bid amount
        auctionPanel.GetComponentInChildren<TextMeshPro>().SetText("0");

    }

    public void HideAuctionScreen()
    {
        auctionPanel.SetActive(false);
        endTurnButton.interactable = true;
        endTurnButton.enabled = true;
    }




    public void OnBidButtonClicked()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;

        int bidAmount = int.Parse(curBidText.text);

        //bid amount can't be greater than the players number of coins
        //USER INPUT
        if (bidAmount > Player.GetLocalPlayer().nCoins)
        {
            bidStatusText.gameObject.SetActive(true);
            bidStatusText.text = "Not Enough Gold!";
            return;
        }
        else if (bidAmount <= Game.currentGame.curBid)
        {
            bidStatusText.gameObject.SetActive(true);
            bidStatusText.text = "Bid Too Low!";
            return;
        }

        bidStatusText.gameObject.SetActive(false);


        Game.currentGame.curBid = bidAmount;
        Game.currentGame.curBidPlayer = Player.GetLocalPlayer().userName;
        Game.currentGame.nextPlayer();
    }

    public void OnPassButtonClicked()
    {
        if (!Player.GetLocalPlayer().IsMyTurn()) return;

        bidStatusText.gameObject.SetActive(false);
        Game.currentGame.nextPlayer(passed: true);
    }

    private void GoToNextAuctionState()
    {

        // check if auctionItem has gone through all players

        if (!IsAuctionItemDone())
        {
            Game.currentGame.nextPlayer();
            return;
        }

        // give the winner the tile. if no one bidded, no one gets it
        if (currentAuctionItem.maxBid.bidAmount != 0)
        {
            String winnerId = currentAuctionItem.maxBid.playerIdentifer;
            Player winner = Player.GetPlayer(winnerId);
            winner.nCoins -= currentAuctionItem.maxBid.bidAmount;
            winner.AddVisibleTile(currentAuctionItem.tile.mTile);
            Game.currentGame.RemoveTileFromPile();
        }

        // if auction is done, hide the auction screen
        if (IsAuctionDone())
        {
            HideAuctionScreen();
            return;
        }

        // Go to next auction item
        GoToNextAuctionItem();
    }

    // Returns true once all items have been auctioned
    private bool IsAuctionDone()
    {
        return auctionItems.Count == 0 && IsAuctionItemDone();
    }

    // Returns true once every player has bid (note that a pass is equivalent to a bid of 0)
    private bool IsAuctionItemDone()
    {
        return currentAuctionItem.GetBidsList().Count == Player.GetAllPlayers().Count;
    }

    public void UpdateAuctionCurrentBestBid(int bidAmount, string playername)
    {
        curBestBidText.text = bidAmount.ToString();
        curBestBidderText.text = playername;
        if (playername != "")
        {
            curBestBidSprite.sprite = Player.GetPlayer(playername).playerColor.GetSprite();
        }
        else
        {
            curBestBidSprite.sprite = PlayerColor.None.GetSprite();
        }
    }

    // Instantiate prefab and add prefab to its parent: auctionGroup
    //ntiles is the remaining tiles up for display
    public void UpdateAuctionItems(List<MovementTile> tilesForAuction)
    {

        // auction twice as many tiles as num of players
        foreach (AuctionItem item in auctionGroup.GetComponentsInChildren<AuctionItem>())
        {
            Destroy(item.gameObject);
        }

        bool first = true;

        foreach (MovementTile tile in tilesForAuction)
        {

            GameObject g = Instantiate(auctionItemPrefab, auctionGroup.transform);

            AuctionItem auctionItem = g.GetComponent<AuctionItem>();
            auctionItem.SetTile(mTileDict[tile]);

            if (first)
            {
                first = false;
                g.GetComponent<Image>().color = GameConstants.greenFaded;
            }

        }
    }

    #endregion



}
