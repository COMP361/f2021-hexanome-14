using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;

using UnityEngine.EventSystems;


public static class MoveValidator
{

    // map roadtype to dictionary of movement card type, and their required counts
    // in for loop, count number of cards that have the right movementtile type, then lookup the amount u need in dictionary, then check if u have the
    // right amount

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

        // FIGURE OUT HOW TO IMPLEMENT RIVER/LAKE/RAFT movements
        //{RoadType.River, new Dictionary<MovementTile, int>
    };

   
    public static bool IsMoveValid(PathScript path, List<CardEnum> cards) 
    {
        MovementTileSpriteScript movementTile = path.GetMovementTile();

        // There's no movementTile on path
        if (movementTile == null)
        {
            return false;
        }

        // count number of cards that match the movementTile type
        int numOfType = 0;
        MovementTile mEnum = movementTile.mTile.mTile;

        foreach (CardEnum cEnum in cards)
        {
            if (cEnum.ToString() == mEnum.ToString())
            {
                numOfType++;
            }
        }

        if (numOfType >= transportationChart[path.roadType][mEnum])
        {
            return true;
        }
        return false;

        // implement CARAVAN check
    }
}
