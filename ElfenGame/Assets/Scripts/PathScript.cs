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

    public GridManager GetGridManager()
    {
        return gridManager;
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
        if (MovementValidator.validWitchTeleport(cards, Player.GetLocalPlayer()))
        {
            // Can Teleport
            SetGreen();
            return;
        }

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

    public void SetGreen()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, GameConstants.pathColoringAlpha);
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

    public List<MovementTileSpriteScript> GetMovementTiles()
    {
        return gridManager.GetMovementTiles();
    }

    public bool isValid(MovementTileSO movementTileSO)
    {
        GridManager gm = GetComponentInChildren<GridManager>();
        //first check if there is double on board
        bool DoubleExists = false;
        foreach (PathScript pathScript in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            GridManager gManager = pathScript.GetComponentInChildren<GridManager>();
            if (gManager.HasDouble())DoubleExists = true;

        }
        if (DoubleExists && (movementTileSO.mTile == MovementTile.Double ||  movementTileSO.mTile == MovementTile.Bounce ||  movementTileSO.mTile == MovementTile.WaterObstacle ||  movementTileSO.mTile == MovementTile.RoadObstacle ||  movementTileSO.mTile == MovementTile.Gold || !gm.HasDouble()))
        {
            return false;
        }
        if (DoubleExists && gm.HasDouble())
        {
            foreach (MovementTileSpriteScript tile in gm.GetMovementTiles())
            {
                if (tile.mTile.mTile == movementTileSO.mTile) return false;
            }
        }

        if (gm == null)
        {
            throw new System.Exception("Paths must have GridManagers in a child Element");
        }

        if (!movementTileSO.mValidRoads.Contains(roadType))
        {
            return false;
        } 
         
        else if (movementTileSO.mTile != MovementTile.RoadObstacle && movementTileSO.mTile != MovementTile.Bounce && movementTileSO.mTile != MovementTile.Double && movementTileSO.mTile != MovementTile.Gold && gm.GetMovementTile() == null) //will also pass when path only has switch tile
        {
            return true;
        }
        else if (movementTileSO.mTile != MovementTile.RoadObstacle && movementTileSO.mTile != MovementTile.Bounce && movementTileSO.mTile != MovementTile.Double && movementTileSO.mTile != MovementTile.Gold && gm.GetMovementTiles().Count == 1 && gm.HasDouble()) 
        {
            return true;//if has double spell and only 1 movement tile on path and tile is a transportation tile
        }

        else if (movementTileSO.mTile == MovementTile.RoadObstacle && !gm.HasObstacle() && !HasGold() && gm.GetMovementTile() != null) 
        {
            return true; // Place obstacle on path with no other obstacle or gold, but contains transport tile
        }
        else if (movementTileSO.mTile == MovementTile.Double && !gm.HasDouble() &&  gm.GetMovementTile() != null)
        {
            return true; //place double spell tile on path with movement tile 
        } 
        else if (movementTileSO.mTile == MovementTile.Bounce && gm.GetMovementTile() != null) 
        {
            return true; //place bounce spell tile on path with movement tile
        }
        else if (movementTileSO.mTile == MovementTile.Gold && !gm.HasObstacle() && gm.GetMovementTile() != null && !gm.HasGold())
        {
            return true; //place gold tile on path if no obstacle and has transport tile and no other gold tile
        }
        return false;
    }

    public bool HasObstacle()
    {
        return gridManager.HasObstacle();


    }

    public bool HasDouble()
    {
        return gridManager.HasDouble();
    }

    public bool hasBounce()
    {
        return gridManager.HasBounce();
    }

    public bool HasGold(){
        return gridManager.HasGold();
    }

}
