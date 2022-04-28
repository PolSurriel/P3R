using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Perk Database", menuName = "Save System/Perks/Database")]
public class PerkDataBase : ScriptableObject, ISerializationCallbackReceiver
{
    public ScriptablePerk[] perks;
    public Dictionary<ScriptablePerk, int> GetId = new Dictionary<ScriptablePerk, int>();

    public void OnAfterDeserialize()
    {
        GetId = new Dictionary<ScriptablePerk, int>();
        for(int i=0; i < perks.Length; i++)
        {
            GetId.Add(perks[i], i);
        }
    }

    public void OnBeforeSerialize()
    {
    }
}
