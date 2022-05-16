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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPerkSelectionUI(ScriptablePerk perk)
	{
        this.GetComponent<Image>().enabled = true;
        window.SetActive(true);
        perkName.text = perk.name;
        perkDescription.text = perk.description;
        myPerk.perk = perk;
        myPerk.RefreshCard();
        myPerk.transform.GetComponentInChildren<Button>().interactable = false;
        perkCost.text = perk.cost.ToString();
        perkRarity.text = perk.rarity.ToString();
        perkRarity.colorGradient = new VertexGradient(myPerk.SetRarityColor(), myPerk.SetRarityColor(), Color.black, Color.black);
    }
}
