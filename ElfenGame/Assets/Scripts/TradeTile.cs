using UnityEngine;
using UnityEngine.UI;

public class TradeTile : MonoBehaviour
{

    


    TradeOffer trade;

    public void SetTrade(TradeOffer trade)
    {
        this.trade = trade;
    }

    public void OnClickWanted()
    {
        //trade.OpenWantedDisplay();
    }

    public void OnClickGiving()
    {
        //trade.OpenGivingDisplay();
    }

  
    
}
