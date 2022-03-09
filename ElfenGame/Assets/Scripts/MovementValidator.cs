using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine.EventSystems;


public static class MoveValidator
{
    // dictionary maps roadType to movementTile type and their required counts
    // iterate over cards, and check if there are enough cards that validate this move

    static Dictionary<RoadType, Dictionary<MovementTile, int>> transportationChart = new Dictionary<RoadType, Dictionary<MovementTile, int>>()
    {
        {RoadType.Plain, new Dictionary<MovementTile, int>
        {
            {MovementTile.GiantPig, 1},
            {MovementTile.Elfcycle, 1},
            {MovementTile.MagicCloud, 2},
            {MovementTile.TrollWagon, 1},
            {MovementTile.Dragon, 1}
        }
        },

        {RoadType.Forest, new Dictionary<MovementTile, int>
        {
            {MovementTile.GiantPig, 1},
            {MovementTile.Elfcycle, 1},
            {MovementTile.MagicCloud, 2},
            {MovementTile.Unicorn, 1},
            {MovementTile.TrollWagon, 2},
            {MovementTile.Dragon, 2}
        }
        },

        {RoadType.Desert, new Dictionary<MovementTile, int>
        {
            {MovementTile.Unicorn, 2},
            {MovementTile.TrollWagon, 2},
            {MovementTile.Dragon, 1}
        }
        },

        {RoadType.Mountain, new Dictionary<MovementTile, int>
        {
            {MovementTile.Elfcycle, 2},
            {MovementTile.MagicCloud, 1},
            {MovementTile.Unicorn, 1},
            {MovementTile.TrollWagon, 2},
            {MovementTile.Dragon, 1}
        }
        },
        
    };

    public static bool IsMoveValid(PathScript path, List<CardEnum> cards)
    {
        MovementTileSpriteScript movementTileWrapper = path.GetMovementTile();
        MovementTile movementTile = movementTileWrapper.mTile.mTile;

        // There's no movementTile on path
        if (movementTileWrapper == null)
        {
            return false;
        }


        // returns true if path is river and theres valid num of raft cards
        if (path.roadType == RoadType.River)
        {
            int numOfRaftCards = NumOfRaftCards(cards);
            return (numOfRaftCards >= 1);
        }


        // Getting num of cards from dictionary
        int requiredCount = transportationChart[path.roadType][movementTile];


        // adding 1 to required num of cards if theres an obstacle
        if (path.HasObstacle())
        {
            requiredCount++;
        }


        // count number of cards that match the movementTile type
        int actualCount = NumberOfMovementType(cards, movementTile);

        // return true if there are valid cards to make a move
        if (actualCount >= requiredCount)
        {
            return true;
        }


        //// caravan check
        //if (!CardsAreSame(cards) && cards.Count >= 3)
        //{
        //    if (!path.HasObstacle())
        //    {
        //        return true;
        //    }
        //    return (cards.Count >= 4);
        //}

        return false;

    }


    // returns true if all cards in list are of same type
    private static bool CardsAreSame(List<CardEnum> cards)
    {
        String firstCardName = Enum.GetName(typeof(CardEnum), cards[0]);
        foreach(CardEnum cEnum in cards)
        {
            String cName = Enum.GetName(typeof(CardEnum), cEnum);
            if (cName != firstCardName)
            {
                return false;
            }
        }
        return true;
    }



    private static int NumOfRaftCards(List<CardEnum> cards)
    {
        int numOfRaftCard = 0;
        foreach(CardEnum cEnum in cards)
        {
            if (cEnum == CardEnum.Raft)
            {
                numOfRaftCard++;
            }
        }
        return numOfRaftCard;
    }




     // Counts the number of cards that have the same type as movement tile
     private static int NumberOfMovementType(List<CardEnum> cards, MovementTile movementTile)
     {
        int countOfCards = 0;
        String tileName = Enum.GetName(typeof(MovementTile), movementTile);

        foreach (CardEnum cEnum in cards)
        {
            String cardName = Enum.GetName(typeof(CardEnum), cEnum);

            if (cardName == tileName)
            {
                countOfCards++;
            }
        }
        return countOfCards;
    }
     




    
}
