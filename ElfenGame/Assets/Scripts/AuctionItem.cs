using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AuctionItem : MonoBehaviour
{
 
    [SerializeField]
    public Bid maxBid = null;

    [SerializeField]
    public MovementTileSO tile;

    [SerializeField]
    public Image tileImage;


 

    public List<Bid> bidsList = new List<Bid>();
        //model passes as a bid of 0 
        
    public AuctionItem(MovementTileSO pTile)
    {
        tile = pTile;
      
    }

    public void AddToBidsList(Bid bid)
    {
        bidsList.Add(bid);
        if (maxBid == null || bid.bidAmount > maxBid.bidAmount)
        {
            maxBid = bid;
        }
    }

    public List<Bid> GetBidsList() {
        return bidsList;
    }


    public void SetTile(MovementTileSO tile)
    {
        this.tile = tile;
        tileImage.sprite = tile.mImage;
    }


    public class Bid
    {
        [SerializeField]
        public String playerIdentifer;

        [SerializeField]
        public int bidAmount;

        public Bid(String pPlayerIdentifer, int pBidAmount )
        {
            playerIdentifer = pPlayerIdentifer;
            bidAmount = pBidAmount;
        }
    }

}


