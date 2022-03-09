using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MovementTileUIScript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public MovementTileSO mTile;

    private int nTiles = 0;

    [SerializeField]
    public GameObject movementTileSpritePrefab;

    [SerializeField]
    public Text countText;

    private MovementTileSpriteScript draggingSprite;

    public void Start()
    {
        UpdateText();
    }


    public void SetTileSO(MovementTileSO newTileSO)
    {
        mTile = newTileSO;

        GetComponent<Image>().sprite = mTile.mImage;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (draggingSprite != null)
        {
            draggingSprite.OnMouseDrag();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (nTiles <= 0) return;
        GameObject newTileSprite = Instantiate(movementTileSpritePrefab);
        draggingSprite = newTileSprite.GetComponent<MovementTileSpriteScript>();
        draggingSprite.SetTileSO(mTile);
        draggingSprite.BeginDrag(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (draggingSprite == null) return;
        bool placed = draggingSprite.EndDrag();
        if (placed) Player.GetLocalPlayer().RemoveTile(mTile.mTile);
        draggingSprite = null;
    }

    public void UpdateText()
    {
        nTiles = Player.GetLocalPlayer().GetNumTilesOfType(mTile.mTile);
        countText.text = nTiles.ToString();
        if (nTiles == 0)
        {
            GetComponent<Image>().color = GameConstants.grey;
	    } else
        {
            GetComponent<Image>().color = GameConstants.white;
	    }
    }
}
