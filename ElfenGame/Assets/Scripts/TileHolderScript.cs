using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileHolderScript : MonoBehaviour
{

    [SerializeField]
    public Image tileImage;


    public MovementTileSO tile;
    public bool selected;
    public bool isSelectable;
    private Image background;

    public void Start()
    {
        selected = false;
        background = GetComponent<Image>();
        SetBackGroundColor();
    }

    public void SetTile(MovementTileSO tile)
    {
        this.tile = tile;
        tileImage.sprite = tile.mImage; 
    }

    public void SetIsSelectable(bool isSelectable)
    {
        this.isSelectable = isSelectable;
    }

    public void SetBackGroundColor()
    {
        if (!isSelectable) return;
        if (selected)
        {
            background.color = GameConstants.green;
        }
        else
        {
            background.color = GameConstants.blue;
        }
    }

    public void ClickedOn()
    {
        if (!isSelectable) return;
        if (GameConstants.mainUIManager) GameConstants.mainUIManager.SetTokensNotSelected();
        selected = true;
        SetBackGroundColor();
    }
}
