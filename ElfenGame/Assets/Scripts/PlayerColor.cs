using System.Collections.Generic;
using UnityEngine;


public enum PlayerColor : byte { Blue, Cyan, Red, Orange, Pink, Green };
static class PlayerColorExtension
{
    static Dictionary<PlayerColor, string> resourceNames = new Dictionary<PlayerColor, string>()
    {
            {PlayerColor.Blue, "blue_elf" },
            {PlayerColor.Green, "green_elf" },
            {PlayerColor.Red, "red_elf" },
            {PlayerColor.Pink, "pink_elf" },
            {PlayerColor.Orange, "orange_elf" },
            {PlayerColor.Cyan, "turq_elf" },

    };

    public static Sprite GetSprite(this PlayerColor playerColor)
    {

        return Resources.Load<Sprite>(resourceNames[playerColor]);
    }
    
    public static byte[] Serialize(object color)
    {
        var c = (PlayerColor)color;
        return new byte[] { (byte)c};
    }

    public static object Deserialize(byte[] v)
    {
        return (PlayerColor)v[0];
    }
}