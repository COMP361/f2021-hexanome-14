using UnityEngine;
using UnityEngine.UI;

public class PlayerTile : MonoBehaviour
{
    
    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;

    [SerializeField]
    Image mPointImage;


    public void UpdateStats(string userName, int nCoins, int nPoints, int nCards, int nTiles, PlayerColor color)
    {   
        if (Player.GetLocalPlayer().userName == userName)
        {
            mNameText.text = userName + " (me)";
        }
        else
        {
            mNameText.text = userName;
        }
        mCoinText.text = nCoins.ToString();
        mPointText.text = nPoints.ToString();
        mCardText.text = nCards.ToString();
        mTileText.text = nTiles.ToString();
        mPointImage.color = color.GetColor(); 
    }
  
    public void SetColor(PlayerColor color) { mPointImage.color = color.GetColor(); } 

    public void SetName(string username) { mNameText.text = username; } 

    public void SetCoins(int nCoins) { mCoinText.text = nCoins.ToString(); }

    public void SetPoints(int nPoints) { mPointText.text = nPoints.ToString(); }

    public void SetCards(int nCards) { mCardText.text = nCards.ToString(); }

    public void SetTiles(int nTiles) { mTileText.text = nTiles.ToString(); }
}
