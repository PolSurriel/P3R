using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string baseSkin;
    public string suitSkin;
    public string accessory1;
    public string accessory2;
    public PerkData[] inventory;
    public PerkData[] equipped;
    public bool freePerkSlotUnlocked;
    public int premiumPerkSlotsUnlocked;
    public int freeCostUnlocked;
    public int premiumCostUnlocked;
    public int totalPerkCost;
    public int equippedPerkCost;

    public SaveData()
    {
        baseSkin = "Yellow";
        suitSkin = "Default";
        accessory1 = "Default";
        accessory2 = "Default";
        inventory = null;
        equipped = null;
        freePerkSlotUnlocked = false;
        premiumPerkSlotsUnlocked = 0;
        freeCostUnlocked = 0;
        premiumCostUnlocked = 0;
        totalPerkCost = 0;
        equippedPerkCost = 0;
    }

    public SaveData(GameInfo gameInfo)
    {
        // Coger las variables de gameInfo y asignarlas a las de MenuData
        baseSkin = GameInfo.playerSkin.baseSkinName;
        suitSkin = GameInfo.playerSkin.suitSkinName;
        accessory1 = GameInfo.playerSkin.accessory1SkinName;
        accessory2 = GameInfo.playerSkin.accessory2SkinName;
        inventory = null;
        equipped = null;
        freePerkSlotUnlocked = GameInfo.freePerkSlotUnlocked;
        premiumPerkSlotsUnlocked = GameInfo.premiumPerkSlotsUnlocked;
        freeCostUnlocked = GameInfo.freeCostUnlocked;
        premiumCostUnlocked = GameInfo.premiumCostUnlocked;
        totalPerkCost = GameInfo.totalPerkCost;
        equippedPerkCost = GameInfo.equippedPerkCost;
    }

    public void UpdateState(GameInfo gameInfo)
    {
        baseSkin = GameInfo.playerSkin.baseSkinName;
        suitSkin = GameInfo.playerSkin.suitSkinName;
        accessory1 = GameInfo.playerSkin.accessory1SkinName;
        accessory2 = GameInfo.playerSkin.accessory2SkinName;
        freePerkSlotUnlocked = GameInfo.freePerkSlotUnlocked;
        premiumPerkSlotsUnlocked = GameInfo.premiumPerkSlotsUnlocked;
        freeCostUnlocked = GameInfo.freeCostUnlocked;
        premiumCostUnlocked = GameInfo.premiumCostUnlocked;
        totalPerkCost = GameInfo.totalPerkCost;
        equippedPerkCost = GameInfo.equippedPerkCost;
    }

    public void SaveInventory(List<ScriptablePerk> _inventory)
    {
        inventory = ConvertInventoryToItemData(_inventory);
    }

    public PerkData[] ConvertInventoryToItemData(List<ScriptablePerk> inventory)
    {
        PerkData[] items = new PerkData[inventory.Count];
        int index = 0;

        foreach (var entry in inventory)
        {
            PerkData item = new PerkData(entry.name, entry.itemLvl, entry.fusionLvl, entry.cost, entry.maxFusionLvl, entry.sprite.ToString(), (int)entry.rarity);
            items[index] = item;
            index++;
        }

        return items;
    }
}
