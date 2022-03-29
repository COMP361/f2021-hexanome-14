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
        Color curColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(curColor.r, curColor.g, curColor.b, GameConstants.pathColoringHoverAlpha);
    }

    public void OnDragExit()
    {
        Color curColor = GetComponent<SpriteRenderer>().color;
        GetComponent<SpriteRenderer>().color = new Color(curColor.r, curColor.g, curColor.b, GameConstants.pathColoringAlpha);
    }

    public void ColorByMoveValidity(NewTown startTown, List<CardEnum> cards)
    {
        if (startTown.name != town1.name && startTown.name != town2.name) return;
        bool isValid = MovementValidator.IsMoveValid(startTown, this, cards);
        if (isValid)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, GameConstants.pathColoringAlpha);
        }
        else
        {
            GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GameConstants.pathColoringAlpha);
        }
    }

    public bool CanMoveOnPath(NewTown startTown, NewTown endTown, List<CardEnum> cards)
    {
        if (startTown.name == endTown.name) return false;
        if (startTown.name != town1.name && startTown.name != town2.name) return false;
        if (endTown.name != town1.name && endTown.name != town2.name) return false;

        return MovementValidator.IsMoveValid(startTown, this, cards);
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public MovementTileSpriteScript GetMovementTile()
    {

        return gridManager.GetMovementTile();
    }

    public bool isValid(MovementTileSO movementTileSO)
    {
        GridManager gm = GetComponentInChildren<GridManager>();
        if (gm == null)
        {
            throw new System.Exception("Paths must have GridManagers in a child Element");
        }

        if (!movementTileSO.mValidRoads.Contains(roadType)) return false;

        if (movementTileSO.mTile != MovementTile.RoadObstacle && gm.GetMovementTile() == null)
        {
            return true;
        }

        if (movementTileSO.mTile == MovementTile.RoadObstacle && !gm.HasObstacle() && gm.GetMovementTile() != null) return true; // Place obstacle on path with 
        return false;
    }

    public bool HasObstacle()
    {
        return gridManager.HasObstacle();


    }

}
