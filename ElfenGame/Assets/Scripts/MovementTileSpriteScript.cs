using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementTileSpriteScript : MonoBehaviour
{
    public MovementTileSO mTile;

    public void Start()
    {
        if (mTile != null)
        {
            SetTileSO(mTile);
        }
    }

    public void SetTileSO(MovementTileSO newTileSO)
    {
        mTile = newTileSO;

        GetComponent<SpriteRenderer>().sprite = mTile.mImage;
    }
}
