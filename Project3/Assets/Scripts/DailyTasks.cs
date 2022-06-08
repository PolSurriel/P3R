using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyTasks : MonoBehaviour
{

    [SerializeField] GameObject taskPlayGame;
    [SerializeField] GameObject taskEquipSkill;
    [SerializeField] GameObject taskHighScore;
	Color initialButtonColor;

	private void Start()
	{
		initialButtonColor = taskEquipSkill.GetComponent<Image>().color;
	}
	public void CheckTasks()
	{
		if (GameInfo.instance.taskPlayGame && !GameInfo.instance.taskPlayGameClaim)
		{
			taskPlayGame.GetComponent<Button>().interactable = true;
			taskPlayGame.GetComponent<Image>().color = Color.white;
			Color buttonColor = taskPlayGame.GetComponentInChildren<Text>().color;
			buttonColor.a = 1f;
			taskPlayGame.GetComponentInChildren<Text>().color = buttonColor;
		}

		if(!GameInfo.instance.taskEquipSkillClaim)
			foreach (ScriptablePerk perk in GameInfo.equippedPerks){
				if(perk.myName != "Default" && perk.myName != "Blocked")
				{
					taskEquipSkill.GetComponent<Button>().interactable = true;
					taskEquipSkill.GetComponent<Image>().color = Color.white;
					Color buttonColor = taskEquipSkill.GetComponentInChildren<Text>().color;
					buttonColor.a = 1f;
					taskEquipSkill.GetComponentInChildren<Text>().color = buttonColor;
				}
			}

		if(GameInfo.instance.maxScore > 200 && !GameInfo.instance.taskHighScoreClaim)
		{
			taskHighScore.GetComponent<Button>().interactable = true;
			taskHighScore.GetComponent<Image>().color = Color.white;
			Color buttonColor = taskHighScore.GetComponentInChildren<Text>().color;
			buttonColor.a = 1f;
			taskHighScore.GetComponentInChildren<Text>().color = buttonColor;
		}
	}

	public void ResetTaskPlayGame()
	{
		GameInfo.instance.taskPlayGameClaim = true;
		taskPlayGame.GetComponent<Button>().interactable = false;
		taskPlayGame.GetComponent<Image>().color = initialButtonColor;
		Color buttonColor = taskPlayGame.GetComponentInChildren<Text>().color;
		buttonColor.a = 0.7f;
		taskPlayGame.GetComponentInChildren<Text>().color = buttonColor;
		GameInfo.AddSoftCurrency(100);
	}
	public void ResetTaskEquipSkill()
	{
		GameInfo.instance.taskEquipSkillClaim = true;
		taskEquipSkill.GetComponent<Button>().interactable = false;
		taskEquipSkill.GetComponent<Image>().color = initialButtonColor;
		Color buttonColor = taskEquipSkill.GetComponentInChildren<Text>().color;
		buttonColor.a = 0.7f;
		taskEquipSkill.GetComponentInChildren<Text>().color = buttonColor;
		GameInfo.AddSoftCurrency(50);
	}
	public void ResetTaskHighScore()
	{
		GameInfo.instance.taskHighScoreClaim = true;
		taskHighScore.GetComponent<Button>().interactable = false;
		taskHighScore.GetComponent<Image>().color = initialButtonColor;
		Color buttonColor = taskHighScore.GetComponentInChildren<Text>().color;
		buttonColor.a = 0.7f;
		taskHighScore.GetComponentInChildren<Text>().color = buttonColor;
		GameInfo.AddSoftCurrency(350);
	}
}
