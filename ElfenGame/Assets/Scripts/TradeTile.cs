using UnityEngine;
using UnityEngine.UI;

public class TradeTile : MonoBehaviour
{

    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;

    [SerializeField]
    Image mPointImage;


    TradeOffer trade;

    public void SetTrade(TradeOffer trade)
    {
        this.trade = trade;
    }

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

    public void OnClickToken()
    {
        player.OpenTokenDisplay();
    }

}
