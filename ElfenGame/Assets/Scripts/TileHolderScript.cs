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
    private bool inVisible;
    private Image background;

    public void Awake()
    {
        isSelectable = true;
        selected = false;
        inVisible = false;
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

    public void SetInVisibleTokens(bool inVisible)
    {
        this.inVisible = inVisible;
        SetBackGroundColor();
    }

    public bool GetInVisibleTokens()
    {
        return inVisible;
    }

    public void SetBackGroundColor()
    {
        if (background == null) background = GetComponent<Image>();
        if (!isSelectable) return;

        if (selected)
        {
            background.color = GameConstants.green;
        }
        else if (inVisible)
        {
            background.color = GameConstants.red;
        }
        else if (!inVisible)
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
