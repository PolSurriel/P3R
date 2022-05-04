using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField] int cost;
    ScriptablePerk perk;

    private void Start()
    {
        perk = this.transform.GetComponentInChildren<PerkDisplay>().perk;
        this.transform.GetComponentInChildren<Text>().text = "Cost: " + cost.ToString();
    }
    public void BuyPerk()
    {
        if(GameInfo.softCurrency >= cost)
        {
            GameInfo.AddPerkToInventory(perk);
            GameInfo.AddSoftCurrency(-cost);
        }
    }

    public void BuyFreePerkSlot()
    {
        if (GameInfo.softCurrency >= cost && !GameInfo.freePerkSlotUnlocked)
        {
            GameInfo.freePerkSlotUnlocked = true;
            GameInfo.AddSoftCurrency(-cost);
        }
    }
    
    public void BuyPremiumPerkSlot()
    {
        if (GameInfo.softCurrency >= cost && GameInfo.premiumPerkSlotsUnlocked < 2)
        {
            GameInfo.premiumPerkSlotsUnlocked++;
            GameInfo.AddSoftCurrency(-cost);
        }
    }

    public void BuyPerkCostSlot()
    {
        if (GameInfo.softCurrency >= cost && GameInfo.freeCostUnlocked < 4)
        {
            GameInfo.freeCostUnlocked++;
            GameInfo.AddSoftCurrency(-cost);
        }
    }
}
