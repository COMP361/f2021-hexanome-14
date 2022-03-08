using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    [SerializeField]
    public int nRows, nCols;

    [SerializeField]
    public float tileSize;

    private List<GameObject> elements;


    private void Awake()
    {
        elements = new List<GameObject>();
    }

    private void PositionElements()
    {
        int colsNeeded = Mathf.Min(nCols, elements.Count);
        int rowsNeeded = (elements.Count-1) / nCols + 1;
        int index = 0;
        foreach (GameObject gm in elements)
        {
            int row = index / nCols;
            int col = index % nCols;

            float posX = col * tileSize - ((colsNeeded-1) * tileSize / 2);
            float posY = row * -tileSize + ((rowsNeeded-1) * tileSize / 2);

            gm.transform.SetParent(gameObject.transform);
            gm.transform.localPosition = new Vector3(posX, posY, GameConstants.gridItemRelativeZ);

            //Debug.Log(gm.name + '@' + gm.transform.localPosition.ToString());

            index++;
        }
    }

    public bool AddElement(GameObject newGameObject)
    {
        if (elements.Count >= nRows * nCols)
        {
            return false;
        }

        elements.Add(newGameObject);
        PositionElements();

        return true;
    }

    public void RemoveElement(GameObject gameObject)
    {
        if (elements.Contains(gameObject))
        {
            elements.Remove(gameObject);
            PositionElements();
        }
    }

    /*
    public bool checkHasObstacle(PathScript p){
        foreach ( GameObject el in elements)
        {
            MovementTileSpriteScript mvts = el.GetComponent<Obstacle>();
            if (mvts != null){
                return true;
            }
        }
        return true;
    }
    */

    public int checkNumMovTile(PathScript p){
        int n = 0;
        foreach ( GameObject el in elements)
        {
            MovementTileSpriteScript mvts = el.GetComponent<MovementTileSpriteScript>();
            if (mvts != null){
                n++;
            } 
        }
        return n;
    }

    public bool checkHasSpecial(PathScript p){
        return false;
    }

    public bool checkHasDoubleTok(PathScript p){
        return false;
    }

    
}
