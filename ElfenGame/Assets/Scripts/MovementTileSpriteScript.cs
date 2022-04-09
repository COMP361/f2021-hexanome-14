using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class MovementTileSpriteScript : MonoBehaviour
{
    public MovementTileSO mTile;

    private MovementTileUIScript dragOrigin;
    private bool drag = false;


    public void Start()
    {
        if (mTile != null)
        {
            SetTileSO(mTile);
        }
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
                    tileScript.SetGreen();
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

            // Debug.Log(tile.mTile.mTile);
            // 



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
        // //return mTile.mValidRoads.Intersect(pTile.mTile.mValidRoads).Any(); not sure why this gives compilation error
        // foreach (RoadType road in mTile.mValidRoads)
        // {
        //     if (pTile.mTile.mValidRoads.Contains(road));
        //     {
        //         return true;
        //         break;
        //     }
        // }
        // return false;
        return true;
    
    }

     private void DoSwap(PathScript path)
    {
        Debug.Log("initiate swapping now");
        GridManager gm = path.GetComponentInChildren<GridManager>();
        Debug.Log(gm.GetNonObstacleTiles().Count);
        if (gm.GetNonObstacleTiles().Count == 1)
        {
            Debug.Log(gm.GetNonObstacleTiles()[0]);
        }
        ColorTilesByBounceValidity(path);

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
                    Debug.Log("THis POOP IS PRESSED");
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
                GridManager gm = path.GetComponentInChildren<GridManager>();
                if (gm == null)
                {
                    throw new System.Exception("Paths must have GridManagers in a child Element");
                }
                
                else
                {
                    if (mTile.mTile == MovementTile.Bounce){
                        DoSwap(path);
                        

                    
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
                            //dont think this is correct
                            if (mTile.mTile == MovementTile.Bounce) //when placing the bounce tile on path, remove the tile currently on it
                            {
                                Destroy(gm.GetMovementTile());
                            }
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
