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
    static Dictionary<CardEnum, String> resourceNames = new Dictionary<CardEnum, String>()
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
}
