using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingMessage : MonoBehaviour
{
    TextMeshProUGUI text;
    Animator myAnimator;
	private void Start()
	{
		text = this.GetComponent<TextMeshProUGUI>();
		myAnimator = this.GetComponent<Animator>();
	}

	public void ShowText(string _text, Color color)
	{
		this.transform.SetAsLastSibling();
		text.text = _text;
		text.color = color;
		myAnimator.Play("Base Layer.FloatingText");
	}
}
