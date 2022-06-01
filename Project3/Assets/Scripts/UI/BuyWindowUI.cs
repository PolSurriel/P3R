using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyWindowUI : MonoBehaviour
{
	[SerializeField] GameObject window;
	[SerializeField] PerkDisplay myPerk;

	// Sets all the variables needed to show BuyWindow
    public void SetBuyWindowUI(ScriptablePerk perk)
	{
		AudioController.instance.sounds.buttonSound.Play();
		window.SetActive(true);
		this.GetComponent<Image>().enabled = true;
		myPerk.perk = perk;
		myPerk.RefreshCard();
		myPerk.transform.GetComponentInChildren<Button>().interactable = false;
	}
}
