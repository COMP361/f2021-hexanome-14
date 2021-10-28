using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTileScript : MonoBehaviour
{
    public MovementTileSO mTile;

    public void SetTileSO(MovementTileSO newTileSO)
    {
        mTile = newTileSO;

        GetComponent<Image>().sprite = mTile.mImage;
    }
}
