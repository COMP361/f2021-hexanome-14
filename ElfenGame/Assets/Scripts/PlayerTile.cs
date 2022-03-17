using UnityEngine;
using UnityEngine.UI;

public class PlayerTile : MonoBehaviour
{

    [SerializeField]
    Text mNameText, mCoinText, mCardText, mTileText, mPointText;

    [SerializeField]
    Image mPointImage;


    Player player;

    public void SetPlayer(Player player)
    {
        this.player = player;
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
