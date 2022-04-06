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

    public void ResetPathColor()
    {
        foreach (PathScript path in GameConstants.roadGroup.GetComponentsInChildren<PathScript>())
        {
            path.ResetColor();
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
            ResetPathColor();
        }
        return added;
    }
}
