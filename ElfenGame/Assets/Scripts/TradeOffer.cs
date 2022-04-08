using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TradeOffer
{
    
    [SerializeField]
    private GameObject tooltip;
    public List<CardEnum> cardsWanted;
    public List<MovementTile> tilesWanted;
    public bool available = true;

    public List<GameObject> chosenItems;

    public TradeOffer(List<GameObject> chosenItems){
        this.chosenItems = chosenItems;
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

    }
}
