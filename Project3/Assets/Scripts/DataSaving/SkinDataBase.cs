using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterDataBase : ScriptableObject
{
    public SkinScriptable[] skins;

    public int skinCount
    {
        get
        {
            return skins.Length;
        }
    }

    public SkinScriptable GetSkin(int index)
    {
        return skins[index];
    }
}
