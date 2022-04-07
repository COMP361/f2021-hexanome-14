using System.Collections.Generic;
using UnityEngine;


public enum GamePhase : byte
{
    DrawCardsAndCounters,
    DrawCounters1,
    DrawCounters2,
    DrawCounters3,
    Auction,
    Trading,
    PlaceCounter,
    Travel,
    SelectTokenToKeep
};
static class GamePhaseExtension
{
    public static byte[] Serialize(object phase)
    {
        var ph = (GamePhase)phase;
        return new byte[] { (byte)ph };
    }

    public static object Deserialize(byte[] v)
    {
        return (GamePhase)v[0];
    }
    public static GamePhase NextPhase(this GamePhase phase)
    {
        if (phase == GamePhase.SelectTokenToKeep)
        {
            // Last phase (reset)
            return GamePhase.DrawCardsAndCounters;
        }


        if (Game.currentGame.gameMode == "Elfengold")
        {
            if (phase == GamePhase.DrawCounters1)
            {
                return GamePhase.Auction; // Only one round of draw counter in elfengold
            }
            
        }
        else
        {
            if (phase == GamePhase.DrawCounters3)
            {
                // TODO: If for trading 
                if (Game.currentGame.tradingPhase){
                    return GamePhase.Trading;
                } else {
                    return GamePhase.PlaceCounter; // Skip auction for Elfenland
                }
                
            }
        }

        // if elfengold and trading phase
        if (phase == GamePhase.Auction && Game.currentGame.tradingPhase)
        {
            return GamePhase.Trading;
        }

        if (phase == GamePhase.Trading){
            Debug.Log("DEBUGGER4");
            return GamePhase.PlaceCounter;
        }

        // if auction -> trading

        return (GamePhase)((byte)phase + 1);
    }
}
