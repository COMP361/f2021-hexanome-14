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

            float posX = col * tileSize - (colsNeeded * tileSize / 2);
            float posY = row * -tileSize + (rowsNeeded * tileSize / 2);

            gm.transform.SetParent(gameObject.transform);
            gm.transform.localPosition = new Vector3(posX, posY, -1);

            Debug.Log(gm.name + '@' + gm.transform.localPosition.ToString());

            index++;
        }
    }

    public void AddElement(GameObject newGameObject)
    {
        if (elements.Count >= nRows * nCols)
        {
            throw new System.Exception("Too many Elements in GridManager");
        }

        elements.Add(Instantiate(newGameObject));
        PositionElements();
    }

    public void RemoveElement(GameObject gameObject)
    {
        elements.Remove(gameObject);
        PositionElements();
    }

}
