using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Perk", menuName ="Perk")]
public class ScriptablePerk : ScriptableObject
{
    public string myName;
    public int itemLvl;
    public int fusionLvl;
    public int cost;
    public int maxFusionLvl;
    public Sprite sprite;
    public Rarity rarity;
    
}
public enum Rarity { COMMON, UNCOMMON, RARE, EPIC, LEGENDARY, DEFAULT }
