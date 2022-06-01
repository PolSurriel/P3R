using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGridGenerator : MonoBehaviour
{
    public GameObject perkSlotPrefab;
    public ScriptablePerk def;
    int counter = 0;

    // Start is called before the first frame update
    void Start()
    {
        ReloadInventory();
        //Resets Grid Vertical Position
        this.transform.parent.GetComponent<ScrollRect>().verticalNormalizedPosition = 0;
    }

    public void ReloadInventory()
    {

        // Load Perks from GameInfo
        if (GameInfo.inventoryPerks != null)
        {
            GenerateNewSlots();
            int inventoryIndex = 0;
            foreach (Transform child in this.transform)
            {
                if(inventoryIndex < GameInfo.inventoryPerks.Count)
                {
                    child.GetComponent<PerkDisplay>().perk = GameInfo.inventoryPerks[inventoryIndex];
                    child.GetComponent<PerkDisplay>().perkIndex = inventoryIndex;
                    child.GetComponent<PerkDisplay>().RefreshCard();
                    inventoryIndex++;
				}
				else
				{
                    child.GetComponent<PerkDisplay>().perk = def;
                    child.GetComponent<PerkDisplay>().perkIndex = inventoryIndex;
                    child.GetComponent<PerkDisplay>().RefreshCard();
                    inventoryIndex++;
                }
            }
        }
    }

    void GenerateNewSlots()
    {
        GameObject _aux;
        while (counter < GameInfo.inventoryPerks.Count || counter % 5 != 0 || counter < 20)
        {
            _aux = Instantiate(perkSlotPrefab, this.transform);
            _aux.transform.SetParent(this.transform);
            _aux.GetComponent<PerkDisplay>().perk = def;
            _aux.GetComponent<PerkDisplay>().RefreshCard();
            counter++;
        }
    }
}
