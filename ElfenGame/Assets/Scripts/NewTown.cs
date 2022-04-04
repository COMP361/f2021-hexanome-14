using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewTown : MonoBehaviour, IDragOver
{
    private GameObject pointsHolder;
    private int goldValue;

    public void Awake()
    {
        pointsHolder = (GameObject)Instantiate(Resources.Load("PointsHolder"), transform);
    }

    public GameObject pointPrefab;
    public void OnDragEnter()
    {
        //Not a built in method
        transform.localScale = new Vector3(1.2f, 1.2f, 1.0f);
    }

    public void OnDragExit()
    {
        //Not a built in method
        transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
    }

    public int getGoldValue()
    {
        return goldValue;
    }

    public void DisplayVisited()
    {
        GridManager pointsManager = pointsHolder.GetComponent<GridManager>();
        pointsManager.Clear();

        foreach (Player p in Player.GetAllPlayers())
        {
            bool isVisited = p.isVisited(name);
            if (!isVisited)
            {
                GameObject g = Instantiate(pointPrefab);
                SpriteRenderer gs = g.transform.GetComponent<SpriteRenderer>();
                gs.color = p.playerColor.GetColor();
                g.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                pointsManager.AddElement(g);
            }
        }
    }

    internal void SetEndTown()
    {
        GameObject g = (GameObject)Instantiate(Resources.Load("RedX"), transform);
        g.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
    }

    internal void SetGold(int v)
    {
        goldValue = v;
        if (v == 0) return; // Don't display anything for elvenhold
        GameObject g = (GameObject)Instantiate(Resources.Load("GoldValue"), transform);
        g.transform.localPosition = new Vector3(0.0f, 0.0f, -1.0f);
        g.GetComponentInChildren<TextMesh>().text = v.ToString();
    }
}
