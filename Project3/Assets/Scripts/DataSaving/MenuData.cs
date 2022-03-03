using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MenuData
{
    public string baseSkin;
    public string suitSkin;
    public string accessory1;
    public string accessory2;

    public MenuData(GameInfo gameInfo)
    {
        // Coger las variables de gameInfo y asignarlas a las de MenuData
        baseSkin = GameInfo.playerSkin.baseSkinName;
        suitSkin = GameInfo.playerSkin.suitSkinName;
        accessory1 = GameInfo.playerSkin.accessory1SkinName;
        accessory2 = GameInfo.playerSkin.accessory2SkinName;
    }
}
