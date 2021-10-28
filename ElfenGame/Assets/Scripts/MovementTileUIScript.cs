using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovementTileUIScript : MonoBehaviour
{
    public MovementTileSO mTile;


    public void SetTileSO(MovementTileSO newTileSO)
    {
        mTile = newTileSO;

        GetComponent<Image>().sprite = mTile.mImage;
    }
}
