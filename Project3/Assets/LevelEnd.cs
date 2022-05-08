using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelEnd : MonoBehaviour
{
    const int CurrencyMult = 10;
    float startingTime;
    [SerializeField]
    GameObject rankingMenu;
    [SerializeField]
    GameObject prefabRankingSlot;

    string[] names = { "Adam", "Max", "iancobox", "fee7d6", "levisito04", "perShun", "steffi<3", "bek44r" };
    private void Start()
    {
        if (GameInfo.instance == null || GameInfo.instance.levelID == 0 || GameInfo.instance.levelID == 3)
        {
            Destroy(gameObject);
        }
        startingTime = Time.time;
    }
    public void BackToMenu()
    {
        GameObject.FindObjectOfType<MapController>().ResetMap();
        MatchController.instance.Quit();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("end");

        if(collision.GetComponent<PlayerController>() != null || (collision.GetComponent<AIController>() != null && GameInfo.instance.levelID == 2))
        {
            rankingMenu.SetActive(true);
            // Add Soft Currency
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 0);
            int currencyToAdd = CurrencyMult * (int)Mathf.Round(multipier) + 100;
            GameObject rankingSlot = Instantiate(prefabRankingSlot, rankingMenu.transform.GetChild(0).GetChild(1));
            rankingSlot.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = currencyToAdd.ToString();
            GameInfo.AddSoftCurrency(currencyToAdd);
            Destroy(collision.GetComponent<PlayerController>());

            
        }
        if(collision.GetComponent<AIController>() != null)
        {
            GameObject rankingSlot = Instantiate(prefabRankingSlot, rankingMenu.transform.Find("PlayerList"));
            rankingSlot.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = names[Random.Range(0, 8)];
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 0);
            int currencyToAdd = CurrencyMult * (int)Mathf.Round(multipier) + 100;
            rankingSlot.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = currencyToAdd.ToString();
            Destroy(collision.GetComponent<AIController>());

        }
    }

}
