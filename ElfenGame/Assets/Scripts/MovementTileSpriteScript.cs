using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;

public class MovementTileSpriteScript : MonoBehaviour
{
    public MovementTileSO mTile;

    private MovementTileUIScript dragOrigin;

    private bool drag = false;

    private bool lookingForSwap = false;
    private MovementTileSpriteScript swap; 
    private PathScript aPath;
    


    public void Start()
    {
        if (mTile != null)
        {
            SetTileSO(mTile);
        }
    }
    public bool GetLookingForSwap()
    {
        return lookingForSwap;
    }

    public void SetLookingForSwap(bool pBool)
    {
        lookingForSwap = pBool;
    }

    public MovementTileSpriteScript GetSwap()
    {
        return swap;
    }

    public void SetSwap(MovementTileSpriteScript pSwap)
    {
        swap = pSwap;
    }

    public PathScript GetPath()
    {
        return aPath;
    }

    public void SetTileSO(MovementTileSO newTileSO)
    {
        mTile = newTileSO;

        GetComponent<SpriteRenderer>().sprite = mTile.mImage;
    }

    public void BeginDrag(MovementTileUIScript dragOrigin)
    {
        // Custom Function
        Debug.Log("Dragging Tile Sprite.");
        this.dragOrigin = dragOrigin;
        drag = true;
        MouseActivityManager.manager.BeginDrag<PathScript>();
        ColorPathsByValidity();
    }

    public void ColorPathsByValidity()
    {
        foreach (PathScript path in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            if (path.isValid(mTile))
            {
                path.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, GameConstants.pathColoringAlpha);
            }
            else
            {
                path.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GameConstants.pathColoringAlpha);
            }
        }
    }

     public void ColorTilesByBounceValidity(PathScript path)
    {
        GridManager gm = path.GetComponentInChildren<GridManager>();
        //search in each path
        foreach ( PathScript path2 in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            GridManager gm2 = path2.GetComponentInChildren<GridManager>();
            List<MovementTileSpriteScript> path2Tiles = gm2.GetAllTiles(); //since want to highlight red/green all tiles including special/obstacles
            foreach (MovementTileSpriteScript tileScript in path2Tiles)
            {
                Debug.Log(tileScript.mTile.mTile);
                if (mTile.mTile == tileScript.mTile.mTile)
                {
                    //dont colour tile
                    //tileScript.SetGreen();
                    Debug.Log(mTile.mTile);
                }
                else if (tileScript.mTile.mTile == MovementTile.Bounce || tileScript.mTile.mTile == MovementTile.RoadObstacle || tileScript.mTile.mTile == MovementTile.WaterObstacle || tileScript.mTile.mTile == MovementTile.Double || tileScript.mTile.mTile == MovementTile.Gold)
                {
                    tileScript.SetRed();
                }
                else if (this.CanSwap(tileScript))
                {
                    tileScript.SetGreen();
                }
                else
                {
                    tileScript.SetRed();
                }

            }

        }
        if (gm.GetMovementTiles().Count == 1)
        {
            gm.GetMovementTiles()[0].SetBlue();

        }
        
    }

    public void SetGreen()
    {
        GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, GameConstants.tileColoringAlpha);
    }

    public void SetRed()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, GameConstants.tileColoringAlpha);
    }

    public void SetBlue()
    {
         GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 1.0f, GameConstants.tileColoringAlpha);
    }

    public void ResetColor()
    {
        GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }

    public void ResetPathColor()
    {
        foreach (PathScript path in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            path.ResetColor();
        }
    }

    public bool CanSwap(MovementTileSpriteScript pTile)
    {
        //return mTile.mValidRoads.Intersect(pTile.mTile.mValidRoads).Any(); not sure why cant find intersect
        if (mTile.mTile != MovementTile.Bounce && mTile.mTile != MovementTile.Double && mTile.mTile != MovementTile.RoadObstacle && mTile.mTile != MovementTile.WaterObstacle && mTile.mTile != MovementTile.Gold)
        {
            foreach (RoadType road in mTile.mValidRoads)
            {
                if (pTile.mTile.mValidRoads.Contains(road));
                {
                    return true;
                    break;
                }
            }
            return false;

        }
        return false;
        
        
    
    }

     private void DoSwap(PathScript path)
    {
        Debug.Log("initiate swapping now");
        GridManager gm = path.GetComponentInChildren<GridManager>();
        
        
        if (gm.GetMovementTiles().Count == 1)
        {
            MovementTileSpriteScript tile1 = gm.GetMovementTiles()[0];
            tile1.SetLookingForSwap(true);
            tile1.ColorTilesByBounceValidity(path);
            tile1.SetBlue();
        }
        else 
        {
             //since want to highlight red/green all tiles including special/obstacles
            foreach (MovementTileSpriteScript tileScript in gm.GetMovementTiles())
            {
                tileScript.SetGreen();
            }
            //ColorTilesByBounceValidity(path);

        }
       

    }

    public void Swap(MovementTileSpriteScript m1, MovementTileSpriteScript m2)
    {
         if (NetworkManager.manager)
         {
             NetworkManager.manager.RemoveTileFromRoad(m1.mTile.mTile,m1.GetPath());
             NetworkManager.manager.RemoveTileFromRoad(m2.mTile.mTile, m2.GetPath());
             NetworkManager.manager.AddTileToRoad(m1.GetPath().name, m2.mTile.mTile);
             NetworkManager.manager.AddTileToRoad(m2.GetPath().name, m1.mTile.mTile);

         } 
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("MouseDown on: UI Element");
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(GameConstants.mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                Debug.Log("MouseDown on: " + hit.collider.gameObject.name);
                
                if (hit.collider.gameObject == gameObject)
                {
                    //check if there are tiles looking for a swap 
                    Debug.Log("this object is " + mTile.mTile);
                    if (lookingForSwap)
                    {
                        return; //if tile is already blue dont do anything
                    }
                    foreach ( PathScript path2 in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
                    {
                        GridManager gm2 = path2.GetComponentInChildren<GridManager>();
                        List<MovementTileSpriteScript> path2Tiles = gm2.GetMovementTiles(); //since want to highlight red/green all tiles including special/obstacles
                        foreach (MovementTileSpriteScript tileScript in path2Tiles)
                        {
                            if (tileScript.GetLookingForSwap())
                            {
                                if (CanSwap(tileScript))
                                {
                                    SetBlue();
                                    SetLookingForSwap(true);
                                    SetSwap(tileScript);
                                    tileScript.SetSwap(this);
                                    Swap(this,tileScript);
                                    return;
                                }
                                else
                                {
                                    return;
                                }
                                
                            }
                        }
                    }
                    //since no tiles looking for swap, check if bounce is on the board -> in play

                    GridManager gm = aPath.GetComponentInChildren<GridManager>(); //since instance exists, aPath !=null
                    if ( gm.HasBounce() && mTile.mTile != MovementTile.Bounce && mTile.mTile != MovementTile.Double && mTile.mTile != MovementTile.RoadObstacle && mTile.mTile != MovementTile.WaterObstacle && mTile.mTile != MovementTile.Gold)
                    {
                        SetBlue();
                        SetLookingForSwap(true);
                    }
                        //if yes, check if valid
                            //if valid, set colour to blue and swap
                            //if not valid, ignore
                        //if no, check if there is a bounce tile on the board
                            //if yes, set colour to blue
                            //if no, ignore
                }
            }
        }
    }


    public void OnMouseDrag()
    {

        if (drag)
        {
            Vector2 MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 objPosition = GameConstants.mainCamera.ScreenToWorldPoint(MousePosition);
            transform.position = new Vector3(objPosition.x, objPosition.y, GameConstants.dragZ);
            MouseActivityManager.manager.WhileDrag<PathScript>();
        }
    }

   

    public bool EndDrag()
    {
        bool added = false;
        if (drag)
        {
            drag = false;

            Debug.Log("Dragging Tile Sprite. Done.");
            PathScript path = MouseActivityManager.manager.EndDrag<PathScript>();

            if (path == null || !path.isValid(mTile))
            {
                Destroy(gameObject);
            }
            else
            {
                aPath = path;
                GridManager gm = path.GetComponentInChildren<GridManager>();
                if (gm == null)
                {
                    throw new System.Exception("Paths must have GridManagers in a child Element");
                }
                
                else
                {
                    if (mTile.mTile == MovementTile.Bounce){
                        DoSwap(path);
                        Destroy(gameObject);                    
                    }
                    else 
                    {
                        added = gm.AddElement(gameObject);

                        if (!added)
                        {
                            Destroy(gameObject);
                        }
                        else
                        {
                            if (NetworkManager.manager) NetworkManager.manager.AddTileToRoad(path.name, mTile.mTile);
                            
                        }

                    }
                    
                    
                }
                
            }
            ResetPathColor();
        }
        return added;
    }
}
