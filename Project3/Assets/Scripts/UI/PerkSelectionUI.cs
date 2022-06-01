using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkSelectionUI : MonoBehaviour
{
    [SerializeField] Text perkName;
    [SerializeField] TextMeshProUGUI perkDescription;
    [SerializeField] TextMeshProUGUI perkRarity;
    [SerializeField] TextMeshProUGUI perkCost;
    [SerializeField] GameObject window;
    [SerializeField] PerkDisplay myPerk;

    public void SetPerkSelectionUI(PerkDisplay perk)
	{
        this.GetComponent<Image>().enabled = true;
        window.SetActive(true);
        perkName.text = perk.perk.name;
        perkDescription.text = perk.perk.description;
        myPerk.perk = perk.perk;
        myPerk.RefreshCard();
        myPerk.perkIndex = perk.perkIndex;
        myPerk.transform.GetComponentInChildren<Button>().interactable = false;
        perkCost.text = perk.perk.cost.ToString();
        perkRarity.text = perk.perk.rarity.ToString();
        perkRarity.colorGradient = new VertexGradient(myPerk.SetRarityColor(), myPerk.SetRarityColor(), Color.black, Color.black);
    }
    public void UnequipPerk()
	{
        myPerk.perkManager.flagEquip = false;
        myPerk.EquipPerk();
	}
}
