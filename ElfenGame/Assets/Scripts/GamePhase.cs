﻿using System.Collections.Generic;
using UnityEngine;


public enum GamePhase : byte { DrawCardsAndCounters, DrawCounters1, DrawCounters2, DrawCounters3, PlaceCounter, Travel, SelectTokenToKeep };
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
}