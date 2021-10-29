using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GameConstants.mouseActivityManager.BeginDrag<PathScript>();
        ColorPathsByValidity();
    }

    public void ColorPathsByValidity()
    {
        foreach (PathScript path in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            if (mTile.mValidRoads.Contains(path.roadType))
            {
                path.GetComponent<SpriteRenderer>().color = new Color(0.0f, 1.0f, 0.0f, 0.65f);
            } else
            {
                path.GetComponent<SpriteRenderer>().color = new Color(1.0f, 0.0f, 0.0f, 0.65f);
            }
        }
    }

    public void ResetPathColor()
    {
        foreach(PathScript path in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            path.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }


    public void OnMouseDrag()
    {
        
        if (drag)
        {
            Vector2 MousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 objPosition = GameConstants.mainCamera.ScreenToWorldPoint(MousePosition);
            transform.position = new Vector3(objPosition.x, objPosition.y, GameConstants.dragZ);
            GameConstants.mouseActivityManager.WhileDrag<PathScript>();
        }
    }

    public void EndDrag()
    {
        if (drag)
        {
            drag = false;

            Debug.Log("Dragging Tile Sprite. Done.");
            PathScript path = GameConstants.mouseActivityManager.EndDrag<PathScript>();

            if (path == null)
            {
                dragOrigin.IncrementCounter();
                Destroy(gameObject);
            } else
            {
                GridManager gm = path.GetComponentInChildren<GridManager>();
                if (gm == null)
                {
                    throw new System.Exception("Paths must have GridManagers in a child Element");
                }
                bool added = gm.AddElement(gameObject);

                if (!added)
                {
                    Destroy(gameObject);
                }
            }
            ResetPathColor();
        }    
        

    }    
}
