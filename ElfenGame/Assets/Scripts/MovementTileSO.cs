using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementTile", menuName = "MovementTile")]
public class MovementTileSO : ScriptableObject
{
    public MovementTile mTile;
    public Sprite mImage;
    public List<RoadType> mValidRoads;
}
