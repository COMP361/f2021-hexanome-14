using System.Collections.Generic;
using UnityEngine;


public enum GamePhase : byte
{
    DrawCardsAndCounters,
    DrawCounters1,
    DrawCounters2,
    DrawCounters3,
    Auction,
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
                return GamePhase.PlaceCounter; // Skip auction for Elfenland
            }
        }

        return (GamePhase)((byte)phase + 1);
    }
}
