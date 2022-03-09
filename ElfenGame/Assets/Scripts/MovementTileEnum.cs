using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MovementTile : byte
{
    Unicorn,
    TrollWagon,
    Elfcycle,
    MagicCloud,
    GiantPig,
    Dragon,
    RoadObstacle,
    WaterObstacle
//TODO: Add other tiles for elvengold
}

static class MovementTileExtension
{
   public static byte[] Serialize(object card)
    {
        var c = (MovementTile)card;
        return new byte[] { (byte)c};
    }

    public static object Deserialize(byte[] v)
    {
        return (MovementTile)v[0];
    }
}

