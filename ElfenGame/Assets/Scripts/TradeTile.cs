using UnityEngine;
using UnityEngine.UI;

public class TradeTile : MonoBehaviour
{


    


    TradeOffer trade;
    TradeItemsDisplay tradeDisplay;
    

    public void SetTrade(TradeOffer trade, TradeItemsDisplay td)
    {
        this.trade = trade;
        this.tradeDisplay = td;

        Debug.Log(" trade is null ?: " + this.trade == null);
        Debug.Log(" td is null ?: " + this.tradeDisplay == null);
    }


    public void OnClickWanted()
    {
        if ( tradeDisplay != null){
            Debug.Log("Hey 1");
        }
        if ( trade != null){
            Debug.Log("Hey 2");
        }
        tradeDisplay.openWindow();
        //trade.OpenWantedDisplay();
    }

    public void OnClickGiving()
    {
        //trade.OpenGivingDisplay();
    }


  
    
}
