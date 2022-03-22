using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGridGenerator : MonoBehaviour
{
    public GameObject perkSlotPrefab;
    public ScriptablePerk def;
    // Start is called before the first frame update
    void Start()
    {
        int counter = 0;
        GameObject aux;

        // Load Perks from GameInfo
        if(GameInfo.inventoryPerks !=null)
            foreach (ScriptablePerk perk in GameInfo.inventoryPerks)
            {
                aux = Instantiate(perkSlotPrefab);
                aux.transform.SetParent(this.transform);
                aux.GetComponent<PerkDisplay>().perk = perk;
                aux.GetComponent<PerkDisplay>().RefreshCard();
                counter++;
            }
        
        // Fills remaining slots until the end of the line
        while(counter%5 !=0 || counter < 20){
            aux = Instantiate(perkSlotPrefab, this.transform);
            aux.transform.SetParent(this.transform);
            aux.GetComponent<PerkDisplay>().perk = def;
            aux.GetComponent<PerkDisplay>().RefreshCard();
            counter++;
        }


        //Resets Grid Vertical Position
        this.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }
}
