using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMovementTile", menuName = "MovementTile")]
public class MovementTileSO : ScriptableObject
{
    public MovementTile mTile;
    public Sprite mImage;
    public List<RoadType> mValidRoads;//should spells have all roadtypes valid or none valid

    public void OnMouseDown()
    {
        Debug.Log("clicked on SO");
    }
}
