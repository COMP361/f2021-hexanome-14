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

    private MovementTileSpriteScript draggingSprite;


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
        GameObject newTileSprite = Instantiate(movementTileSpritePrefab);
        draggingSprite = newTileSprite.GetComponent<MovementTileSpriteScript>();
        draggingSprite.SetTileSO(mTile);
        draggingSprite.BeginDrag(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        draggingSprite.EndDrag();
        draggingSprite = null;
    }

    private void UpdateText()
    {
        //TODO: Implement this
    }

    public void IncrementCounter()
    {
        nTiles++;
        UpdateText();
    }

    public void DecrementCounter()
    {
        nTiles--;
        UpdateText();
    }
}
