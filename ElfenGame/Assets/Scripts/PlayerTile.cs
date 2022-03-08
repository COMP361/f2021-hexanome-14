using UnityEngine;
using UnityEngine.UI;

public class PlayerTile : MonoBehaviour
{
    
    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;


    public void UpdateStats(string userName, int nCoins, int nPoints, int nCards, int nTiles)
    {
        mNameText.text = userName;
        mCoinText.text = nCoins.ToString();
        mPointText.text = nPoints.ToString();
        mCardText.text = nCards.ToString();
        mTileText.text = nTiles.ToString();
    }
   
    public void SetName(string username) { mNameText.text = username; } 

    public void SetCoins(int nCoins) { mCoinText.text = nCoins.ToString(); }

    public void SetPoints(int nPoints) { mPointText.text = nPoints.ToString(); }

    public void SetCards(int nCards) { mCardText.text = nCards.ToString(); }

    public void SetTiles(int nTiles) { mTileText.text = nTiles.ToString(); }
}