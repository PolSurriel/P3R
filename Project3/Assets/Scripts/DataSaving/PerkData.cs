using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerkData
{
    public string name;
    public int itemLvl;
    public int fusionLvl;
    public int cost;
    public int maxFusionLvl;
    public string imagePath;
    public int rarity;

    public PerkData(string _name, int _itemlvl, int _fusionlvl, int _cost, int _maxfusionlvl, string _imagepath, int _rarity)
    {
        name = _name;
        itemLvl = _itemlvl;
        fusionLvl = _fusionlvl;
        cost = _cost;
        maxFusionLvl = _maxfusionlvl;
        imagePath = _imagepath;
        rarity = _rarity;
    }

    public PerkData(ScriptablePerk _perk)
    {
        name = _perk.name;
        itemLvl = _perk.itemLvl;
        fusionLvl = _perk.fusionLvl;
        cost = _perk.cost;
        maxFusionLvl = _perk.maxFusionLvl;
        imagePath = _perk.sprite.ToString();
        rarity = (int)_perk.rarity;
    }

}
