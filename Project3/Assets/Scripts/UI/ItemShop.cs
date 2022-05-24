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
        if(!(GameInfo.softCurrency >= cost))
		{
            ErrorBuyMessage();
            return;
        }
        AudioController.instance.sounds.buttonSound.Play();
        GameInfo.AddPerkToInventory(perk);
        GameInfo.AddSoftCurrency(-cost);
        GameObject.FindObjectOfType<BuyWindowUI>().SetBuyWindowUI(perk);
    }

    public void BuyFreePerkSlot()
    {
        if (!(GameInfo.softCurrency >= cost && !GameInfo.freePerkSlotUnlocked))
        {
            ErrorBuyMessage();
            return;
        }
        AudioController.instance.sounds.buttonSound.Play();
        GameInfo.freePerkSlotUnlocked = true;
        GameInfo.AddSoftCurrency(-cost);
        GameObject.FindObjectOfType<BuyWindowUI>().SetBuyWindowUI(perk);
    }
    
    public void BuyPremiumPerkSlot()
    {
        if (!(GameInfo.softCurrency >= cost && GameInfo.premiumPerkSlotsUnlocked < 2))
        {
            ErrorBuyMessage();
            return;
        }
        AudioController.instance.sounds.buttonSound.Play();
        GameInfo.premiumPerkSlotsUnlocked++;
        GameInfo.AddSoftCurrency(-cost);
        GameObject.FindObjectOfType<BuyWindowUI>().SetBuyWindowUI(perk);
    }

    public void BuyPerkCostSlot()
    {
        if (!(GameInfo.softCurrency >= cost && GameInfo.freeCostUnlocked < 4))
        {
            ErrorBuyMessage();
            return;
        }
        AudioController.instance.sounds.buttonSound.Play();
        GameInfo.freeCostUnlocked++;
        GameInfo.AddSoftCurrency(-cost);
        GameObject.FindObjectOfType<BuyWindowUI>().SetBuyWindowUI(perk);
    }

    void ErrorBuyMessage()
	{
        AudioController.instance.sounds.errorButtonSound.Play();
        GameInfo.instance.floatingText.ShowText("Could not buy this item", Color.red);
	}
}
