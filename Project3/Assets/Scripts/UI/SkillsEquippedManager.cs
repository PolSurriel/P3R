using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillsEquippedManager : MonoBehaviour
{
    [SerializeField] PerkDisplay[] allPerks = new PerkDisplay[4];
    [SerializeField] PerkDisplay[] allPerksEquip = new PerkDisplay[4];
    [SerializeField] PerkDisplay freePerkSlot;
    [SerializeField] PerkDisplay[] premiumPerkSlots = new PerkDisplay[2];
    [SerializeField] ScriptablePerk freeblocked;
    [SerializeField] ScriptablePerk premiumblocked;
    [SerializeField] GameObject[] costsImages = new GameObject[9];
    [SerializeField] Sprite freeLockSprite;
    [SerializeField] Sprite premiumLockSprite;
    public bool flagEquip = false;
    private ScriptablePerk perkSelected;

    Color colorCostEquipped = new Color(0.9f, 0.6f, 0.2f);
    Color colorCostNotEquipped = new Color(0.95f, 0.95f, 0.95f);

    int index = 2;
    int indexFree = 4;
    int indexPremium = 3;


    // Start is called before the first frame update
    void Awake()
    {
        //CheckPerks();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void CheckPerks()
    {
        for (int i = 0; i < allPerks.Length; i++)
        {
            allPerks[i].perk = GameInfo.equippedPerks[i];
            allPerksEquip[i].perk = GameInfo.equippedPerks[i];
            allPerks[i].RefreshCard();
            allPerksEquip[i].RefreshCard();
        }
        if (!GameInfo.freePerkSlotUnlocked)
            freePerkSlot.perk = freeblocked;
        freePerkSlot.RefreshCard();
        int counter = 2;
        while (GameInfo.premiumPerkSlotsUnlocked < counter)
        {
            counter--;
            premiumPerkSlots[counter].perk = premiumblocked;
            premiumPerkSlots[counter].RefreshCard();
        }
    }
    public void RefreshCosts()
    {
        index = 2;
        indexFree = 4;
        indexPremium = 3;
        GameInfo.totalPerkCost = 0;
        for(int i=0; i < 2; i++)
        {
            costsImages[i].transform.Find("Lock").localScale = new Vector3(0, 0, 0);
            GameInfo.totalPerkCost++;
        }
        for (int i=2; i<costsImages.Length; i++)
        {
            costsImages[i].transform.Find("Lock").localScale = new Vector3(1, 1, 1);
        }
        for(int i = 0; i<GameInfo.freeCostUnlocked; i++)
        {
            costsImages[index + i].transform.Find("Lock").localScale = new Vector3(0, 0, 0);
            indexFree--;
            GameInfo.totalPerkCost++;
        }
        index += GameInfo.freeCostUnlocked;
        for(int i=0; i<GameInfo.premiumCostUnlocked; i++)
        {
            costsImages[index +  i].transform.Find("Lock").localScale = new Vector3(0, 0, 0);
            indexPremium--;
            GameInfo.totalPerkCost++;

        }
        index += GameInfo.premiumCostUnlocked;
        ChangeCostIcons();
        GameInfo.equippedPerkCost = CalculateEquippedPerkCost();
        // Change color icons
        for(int i=0; i<GameInfo.equippedPerkCost; i++)
        {
            costsImages[i].transform.Find("Image").GetComponent<Image>().color = colorCostEquipped;
        }
        for(int i = GameInfo.equippedPerkCost; i<costsImages.Length; i++)
        {
            costsImages[i].transform.Find("Image").GetComponent<Image>().color = colorCostNotEquipped;
        }
        Debug.Log("Total Perk Cost: " + GameInfo.totalPerkCost);
    }

    public bool CheckCost(int _costPlus, int _costMinus)
    {
        return (GameInfo.totalPerkCost >= GameInfo.equippedPerkCost + _costPlus - _costMinus);
    }

    public int CalculateEquippedPerkCost()
    {
        int totalCost = 0;
        for(int i=0; i < allPerks.Length; i++)
        {
            totalCost += allPerks[i].perk.cost;
        }
        return totalCost;
    }

    public void ChangeCostIcons()
    {
        while(index < costsImages.Length)
        {
            if(0 < indexFree)
            {
                costsImages[index].transform.Find("Lock").GetComponent<Image>().sprite = freeLockSprite;
                indexFree--;
            }
            else if(0 < indexPremium)
            {
                costsImages[index].transform.Find("Lock").GetComponent<Image>().sprite = premiumLockSprite;
                indexPremium--;
            }
            index++;
        }

    }

    public void SelectPerk(ScriptablePerk perk)
    {

        flagEquip = true;
        perkSelected = perk;
    }

    public void UnequipPerk(int slot)
    {
        if (flagEquip)
        {
            if (CheckCost(perkSelected.cost, allPerks[slot].perk.cost))
            {
                GameInfo.equippedPerks[slot] = perkSelected;
                GameInfo.equippedPerkCost = CalculateEquippedPerkCost();
                allPerks[slot].perk = perkSelected;
                allPerks[slot].RefreshCard();
                flagEquip = false;
                perkSelected = null;
            }
        }
        else
        {
            GameInfo.equippedPerks[slot] = GameInfo.defaultPerk;
            GameInfo.equippedPerkCost = CalculateEquippedPerkCost();
            allPerks[slot].perk = GameInfo.defaultPerk;
            allPerks[slot].RefreshCard();
            foreach(ScriptablePerk perk in GameInfo.equippedPerks)
            {
                Debug.Log(perk.myName);
            }
        }
        
        RefreshCosts();

    }


    public void UnlockSlot()
    {
#if UNITY_EDITOR
        GameInfo.freePerkSlotUnlocked = !GameInfo.freePerkSlotUnlocked;
        GameInfo.freeCostUnlocked++;
        GameInfo.premiumCostUnlocked++;
        Debug.Log(GameInfo.freePerkSlotUnlocked);

#endif
    }


}
