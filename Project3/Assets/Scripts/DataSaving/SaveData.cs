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
    public float[] colorBaseSkin;
    public PerkData[] inventory;
    public PerkData[] equipped;
    public bool freePerkSlotUnlocked;
    public int premiumPerkSlotsUnlocked;
    public int freeCostUnlocked;
    public int premiumCostUnlocked;
    public int totalPerkCost;
    public int equippedPerkCost;
    public int softCurrency;
    public int maxScore;

    public bool sfxEnable;
    public bool musicEnable;

    public SaveData()
    {
        baseSkin = "Yellow";
        suitSkin = "Default";
        accessory1 = "Default";
        accessory2 = "Default";
        colorBaseSkin = new float[3];
        colorBaseSkin[0] = 1.0f;
        colorBaseSkin[1] = 1.0f;
        colorBaseSkin[2] = 1.0f;
        inventory = null;
        equipped = null;
        freePerkSlotUnlocked = false;
        premiumPerkSlotsUnlocked = 0;
        freeCostUnlocked = 0;
        premiumCostUnlocked = 0;
        totalPerkCost = 0;
        equippedPerkCost = 0;
        softCurrency = 0;
        sfxEnable = false;
        musicEnable = false;
        maxScore = 0;
    }

    public SaveData(GameInfo gameInfo)
    {
        // Coger las variables de gameInfo y asignarlas a las de MenuData
        baseSkin = GameInfo.playerSkin.baseSkinName;
        suitSkin = GameInfo.playerSkin.suitSkinName;
        accessory1 = GameInfo.playerSkin.accessory1SkinName;
        accessory2 = GameInfo.playerSkin.accessory2SkinName;
        colorBaseSkin = new float[3];
        colorBaseSkin[0] = GameInfo.playerSkin.playerColor.r;
        colorBaseSkin[1] = GameInfo.playerSkin.playerColor.g;
        colorBaseSkin[2] = GameInfo.playerSkin.playerColor.b;
        inventory = ConvertInventoryToItemData(GameInfo.equippedPerks);
        equipped = ConvertInventoryToItemData(GameInfo.inventoryPerks);
        freePerkSlotUnlocked = GameInfo.freePerkSlotUnlocked;
        premiumPerkSlotsUnlocked = GameInfo.premiumPerkSlotsUnlocked;
        freeCostUnlocked = GameInfo.freeCostUnlocked;
        premiumCostUnlocked = GameInfo.premiumCostUnlocked;
        totalPerkCost = GameInfo.totalPerkCost;
        equippedPerkCost = GameInfo.equippedPerkCost;
        softCurrency = GameInfo.softCurrency;
        sfxEnable = gameInfo.sfxEnable;
        musicEnable = gameInfo.musicEnable;
        maxScore = gameInfo.maxScore;
    }

    public void UpdateState(GameInfo gameInfo)
    {
        baseSkin = GameInfo.playerSkin.baseSkinName;
        suitSkin = GameInfo.playerSkin.suitSkinName;
        accessory1 = GameInfo.playerSkin.accessory1SkinName;
        accessory2 = GameInfo.playerSkin.accessory2SkinName;
        colorBaseSkin = new float[3];
        colorBaseSkin[0] = GameInfo.playerSkin.playerColor.r;
        colorBaseSkin[1] = GameInfo.playerSkin.playerColor.g;
        colorBaseSkin[2] = GameInfo.playerSkin.playerColor.b;
        equipped = ConvertInventoryToItemData(GameInfo.equippedPerks);
        inventory = ConvertInventoryToItemData(GameInfo.inventoryPerks);
        freePerkSlotUnlocked = GameInfo.freePerkSlotUnlocked;
        premiumPerkSlotsUnlocked = GameInfo.premiumPerkSlotsUnlocked;
        freeCostUnlocked = GameInfo.freeCostUnlocked;
        premiumCostUnlocked = GameInfo.premiumCostUnlocked;
        totalPerkCost = GameInfo.totalPerkCost;
        equippedPerkCost = GameInfo.equippedPerkCost;
        softCurrency = GameInfo.softCurrency;
        sfxEnable = gameInfo.sfxEnable;
        musicEnable = gameInfo.musicEnable;
        maxScore = gameInfo.maxScore;
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
            PerkData item = new PerkData(entry);
            items[index] = item;
            index++;
        }

        return items;
    }
}
