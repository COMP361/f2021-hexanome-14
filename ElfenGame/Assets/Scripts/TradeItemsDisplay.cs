using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TradeItemsDisplay : MonoBehaviour
{
    [SerializeField]
    public Text nameText, numHiddenText;

    [SerializeField]
    public GameObject tileGroup;
    

    [SerializeField]
    public GameObject tilePrefab;
    [SerializeField]
    public GameObject cardPrefab;
    
    

    public void SetName(string username)
    {
        nameText.text = $"{username}'s tokens";
    }

    public void SetItems(TradeOffer t, Dictionary<MovementTile, MovementTileSO>  mTileDic)
    {
        Dictionary<MovementTile, MovementTileSO> mTileDict = mTileDic;
        
        GridLayoutGroup gridGroup = gameObject.GetComponentInChildren<GridLayoutGroup>();

        foreach (Transform child in gridGroup.transform)
        {
            Destroy(child.gameObject);
        }


        foreach (GameObject item in t.chosenItems)
        {
            item.transform.parent = gridGroup.transform;
        }

        /*
        foreach (MovementTile tile in t.tilesWanted)
        {
            if (mTileDict.ContainsKey(tile)){
                GameObject g = Instantiate(tilePrefab, gridGroup.transform);

                TileHolderScript thscript = g.GetComponent<TileHolderScript>();
                thscript.trading = true;
            
                thscript.SetTile(mTileDict[tile]);
                thscript.SetIsSelectable(true);
                
            }
            
        }

        // instantiate all kinds of cards 
        foreach (CardEnum c in t.cardsWanted)
        {
            //Debug.Log("Cards being added !!!!!!!!");
            GameObject g = Instantiate(cardPrefab, gridGroup.transform);
            Card card = g.GetComponent<Card>();

            card.Initialize(c);

            
        }
        */
    }
    /*
(List<GameObject> chosenItems){
        tilesWanted = new List<MovementTile>();
        cardsWanted = new List<CardEnum>();
        foreach (GameObject g in chosenItems)
        {
            Card card = g.GetComponent<Card>();
            TileHolderScript tile = g.GetComponent<TileHolderScript>();
            if (card != null){
                cardsWanted.Add(card.cardType);
            }
            if (tile != null){
                tilesWanted.Add(tile.tile.mTile);
            }
        }

        // print them
        foreach (CardEnum c in cardsWanted)
        {
            Debug.Log("I want this card : " + System.Enum.GetName(typeof(CardEnum), c));
        }

        foreach (MovementTile t in tilesWanted)
        {
            Debug.Log("I want this tile : " + System.Enum.GetName(typeof(MovementTile), t));
        }
    */

    public void SetNumHidden(int numHidden)
    {
        numHiddenText.text = numHidden.ToString();
    }

    public void closeWindow()
    {
        gameObject.SetActive(false);
    }

    public void openWindow()
    {
        gameObject.SetActive(true);
    }

    public void counter()
    {
        gameObject.SetActive(false);
        
    }

}