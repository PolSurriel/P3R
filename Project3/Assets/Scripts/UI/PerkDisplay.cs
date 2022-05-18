using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PerkDisplay : MonoBehaviour
{
    Color common = new Color(0.99f, 0.99f, 0.99f);
    Color uncommon = new Color(0f, 0.88f, 0.06f);
    Color rare = new Color(0.15f, 0.64f, 0.93f);
    Color epic = new Color(0.79f, 0.15f, 0.93f);
    Color legendary = new Color(0.93f, 0.75f, 0.15f);
    Color def = new Color(0.39f, 0.39f, 0.39f);
    public ScriptablePerk perk;
    public int perkIndex = 0;
    [SerializeField] ScriptablePerk defPerk;

    public Text itemLvl;
    public Text fusionLvl;
    public Image sprite;
    public Image rarity;
    public GameObject selectedMenu;
    SkillsEquippedManager perkManager;

    // Start is called before the first frame update
    void Start()
    {
        RefreshCard();
        perkManager = GameObject.FindObjectOfType<SkillsEquippedManager>();
    }

    public void RefreshCard()
    {
        if (perk == null)
            perk = defPerk;
        if (perk.myName != "Default" && perk.myName != "Blocked")
        {
            itemLvl.text = "Lvl." + perk.itemLvl.ToString();
            fusionLvl.text = perk.fusionLvl.ToString();
            this.transform.GetComponentInChildren<Button>().interactable = true;
        }
        else
        {
            fusionLvl.text = "";
            itemLvl.text = "";
            //this.transform.GetComponentInChildren<Button>().interactable = false;
        }
        sprite.sprite = perk.sprite;
        rarity.color = SetRarityColor();
    }

    

    public Color SetRarityColor()
    {
        switch (perk.rarity)
        {
            case Rarity.COMMON:
                return common;
            case Rarity.UNCOMMON:
                return uncommon;
            case Rarity.RARE:
                return rare;
            case Rarity.EPIC:
                return epic;
            case Rarity.LEGENDARY:
                return legendary;
            case Rarity.DEFAULT:
                return def;
            default:
                Debug.LogError("Color not found in PerkDisplay");
                return Color.white;

        }
    }

    public void PressButton()
    {
        foreach (PerkDisplay perk in GameObject.FindObjectsOfType<PerkDisplay>())
        {
            perk.selectedMenu.SetActive(false);
        }
        if (perk.myName != "Default" && !perkManager.flagEquip)
		{
            GameObject.FindObjectOfType<PerkSelectionUI>().SetPerkSelectionUI(this.perk);
            perkManager.flagEquip = true;
		}
        else if (perkManager.flagEquip)
		{
            EquipPerk();
		}
            //selectedMenu.SetActive(true);
    }

    public void InventoryEquipButton()
    {
        // Function called when pressed button "equip" of an Inventory's Perk
        if(perkManager != null)
        {
            perkManager.SelectPerk(perk);
        }
    }

    public void EquipPerk()
    {
        // Function called when pressed the selected position of
        // equippedPerks where you want to equip selectedPerk
        perkManager.UnequipPerk(perkIndex);
        perkManager.flagEquip = false;

    }
}
