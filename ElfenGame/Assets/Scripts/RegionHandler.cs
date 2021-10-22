using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class RegionHandler : MonoBehaviour
{

    private SpriteRenderer sprite;

    public Color32 oldColor;
    public Color32 hoverColor;
    public Color32 startColor;

    void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        sprite.color = startColor;
    }

    void OnMouseEnter()
    {
        oldColor = sprite.color;
        sprite.color = hoverColor;
    }

    void OnMouseExit()
    {
        sprite.color = oldColor;
    }
/*
    private void OnMouseOver()
    {
        oldColor = sprite.color;
        sprite.color = hoverColor;
    }*/
}
