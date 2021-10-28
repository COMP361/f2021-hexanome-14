using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "PlayerStats", order = 1)]

public class PlayerStats : ScriptableObject
{
    public int nCoins;
    public int nPoints;

    public List<Card> mCards;
    public List<MovementTile> mTiles;


}
