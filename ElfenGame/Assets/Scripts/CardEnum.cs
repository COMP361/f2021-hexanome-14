using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum CardEnum : byte
{
    Dragon,
    ElfCycle,
    GiantPig,
    MagicCloud,
    Raft,
    TrollWagon,
    Unicorn,
    Witch
}

static class CardEnumExtension
{
    static Dictionary<CardEnum, string> resourceNames = new Dictionary<CardEnum, string>()
    {
        {CardEnum.Dragon, "Dragon" },
        {CardEnum.ElfCycle, "ElfCycle" },
        {CardEnum.GiantPig, "GiantPig" },
        {CardEnum.MagicCloud, "MagicCloud" },
        {CardEnum.Raft, "Raft" },
        {CardEnum.TrollWagon, "TrollWagon" },
        {CardEnum.Unicorn, "Unicorn" },
        {CardEnum.Witch, "Witch" },
    };
     
    public static Sprite GetSprite(this CardEnum card)
    { 
         
        return Resources.Load<Sprite>(resourceNames[card]);
    }

    public static byte[] Serialize(object card)
    {
        var c = (CardEnum)card;
        return new byte[] { (byte)c};
    }

    public static object Deserialize(byte[] v)
    {
        return (CardEnum)v[0];
    }
}
