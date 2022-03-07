using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsTile : MonoBehaviour
{
    [SerializeField]
    PlayerStats mPlayerStats;

    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;

    void Start()
    {
        if (mPlayerStats != null)
        {
            mNameText.text = mPlayerStats.name;
            mCoinText.text = mPlayerStats.nCoins.ToString();
            mPointText.text = mPlayerStats.nPoints.ToString();
            mCardText.text = mPlayerStats.mCards.Count.ToString();
            mTileText.text = mPlayerStats.mTiles.Count.ToString();
        }
    }


}
