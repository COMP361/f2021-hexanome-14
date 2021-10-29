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
    }

    public void OnDragExit()
    {
        //GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
}
