using System;
using System.Collections.Generic;

using UnityEngine.EventSystems;


public static class MovementValidator
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

    static Dictionary<MovementTile, CardEnum> tileToCard = new Dictionary<MovementTile, CardEnum>()
    {
        {MovementTile.Dragon, CardEnum.Dragon },
        {MovementTile.Elfcycle, CardEnum.ElfCycle },
        {MovementTile.GiantPig, CardEnum.GiantPig },
        {MovementTile.MagicCloud, CardEnum.MagicCloud },
        {MovementTile.TrollWagon, CardEnum.TrollWagon },
        {MovementTile.Unicorn, CardEnum.Unicorn }
    };

    public static bool IsMoveValid(NewTown startTown, PathScript path, List<CardEnum> cards)
    {
        
        // No cards given
        if (cards.Count == 0)
        {
            return false;
        }

        // returns true if path is river and theres valid num of raft cards
        if (path.roadType == RoadType.River)
        {
            if (CardsAreSame(cards) && cards[0] == CardEnum.Raft)
            {
                if (startTown.name == path.town1.name) // Downstream
                {
                    return (cards.Count == 1);
                }
                else
                { //Upstream
                    return (cards.Count == 2);
                }
            }
            else
            {
                return false;
            }
        }

        if (path.roadType == RoadType.Lake)
        {
            if (CardsAreSame(cards) && cards[0] == CardEnum.Raft)
            {
                return cards.Count == 2;
            }
            else
            {
                return false;
            }
        }
        
        List<MovementTileSpriteScript> movementTileWrappers = path.GetMovementTiles();

        if (movementTileWrappers == null) return false;

        foreach (var movementTileWrapper in movementTileWrappers)
        {
            if (movementTileWrapper == null) return false;

            MovementTile movementTile = movementTileWrapper.mTile.mTile;

            if (transportationChart[path.roadType].ContainsKey(movementTile))
            {
                // Getting num of cards from dictionary
                int requiredCount = transportationChart[path.roadType][movementTile];

                if (CardsAreSame(cards) && tileToCard[movementTile] == cards[0])
                {
                    // adding 1 to required num of cards if theres an obstacle
                    if (path.HasObstacle())
                    {
                        requiredCount++;
                    }

                    return cards.Count == requiredCount;
                }
            }
           
        }
         
        


        // Caravaning
        if (path.HasObstacle())
        {
            return cards.Count == 4;
        }
        else
        {
            return cards.Count == 3;
        }
    }


    // returns true if all cards in list are of same type
    private static bool CardsAreSame(List<CardEnum> cards)
    {
        CardEnum firstCard = cards[0];
        foreach (CardEnum cEnum in cards)
        {
            if (cEnum != firstCard)
            {
                return false;
            }
        }
        return true;
    }



    private static int NumOfRaftCards(List<CardEnum> cards)
    {
        int numOfRaftCard = 0;
        foreach (CardEnum cEnum in cards)
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

    public static bool validWitchTeleport(List<CardEnum> cards, Player p)
    {
        if (cards.Count != 1) return false;
        if (cards[0] != CardEnum.Witch) return false;
        if (p.nCoins < GameConstants.COST_OF_TELEPORT) return false;

        return true;
    }




}
