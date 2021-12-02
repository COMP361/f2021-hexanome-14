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


    private void Start()
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
            gm.transform.localPosition = new Vector3(posX, posY, GameManager.gridItemRelativeZ);

            Debug.Log(gm.name + '@' + gm.transform.localPosition.ToString());

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
        elements.Remove(gameObject);
        PositionElements();
    }

}
