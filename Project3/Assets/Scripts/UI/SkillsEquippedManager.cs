using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsEquippedManager : MonoBehaviour
{
    [SerializeField] PerkDisplay[] allPerks = new PerkDisplay[4];
    [SerializeField] PerkDisplay freePerkSlot;
    [SerializeField] PerkDisplay[] premiumPerkSlots = new PerkDisplay[2];
    [SerializeField] ScriptablePerk freeblocked;
    [SerializeField] ScriptablePerk premiumblocked;

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
            freePerkSlot.RefreshCard();
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

    public void UnlockSlot()
    {
#if UNITY_EDITOR
        GameInfo.freePerkSlotUnlocked = !GameInfo.freePerkSlotUnlocked;
        Debug.Log(GameInfo.freePerkSlotUnlocked);

#endif
    }

}
