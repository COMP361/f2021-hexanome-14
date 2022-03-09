using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour, IDragOver
{
    [SerializeField]
    public RoadType roadType;

    [SerializeField]
    public NewTown town1, town2;

    private GridManager gridManager;
 
    
    void Start()
    {
        gridManager = GetComponentInChildren<GridManager>();
    }

    public void OnDragEnter()
    {
        //GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.65f);
        Color curColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(curColor.r, curColor.g, curColor.b, GameConstants.pathColoringHoverAlpha);
    }

    public void OnDragExit()
    {
        //GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        Color curColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(curColor.r, curColor.g, curColor.b, GameConstants.pathColoringAlpha);
    }

    public MovementTileSpriteScript GetMovementTile()
    {

        return gridManager.GetMovementTile();
    }

    public bool HasObstacle()
    {
        return gridManager.HasObstacle();


    }

}
