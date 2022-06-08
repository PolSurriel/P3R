using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelEnd : MonoBehaviour
{
    const int CurrencyMult = 2;
    float startingTime;
    public GameObject rankingMenu;
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
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 120);
            int currencyToAdd = CurrencyMult * (int)Mathf.Round(multipier) + 100;
            SetRankingMenuPlayer(currencyToAdd);
            if (GameInfo.instance.levelID == 1)
                GameInfo.instance.tutorialDone = true;
            else
                GameInfo.instance.taskPlayGame = true;
        }
        if(collision.GetComponent<AIController>() != null)
        {
            GameObject rankingSlot = Instantiate(prefabRankingSlot, rankingMenu.transform.GetChild(0).GetChild(1));
            rankingSlot.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = names[Random.Range(0, 8)];
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 120);
            int currencyToAdd = CurrencyMult * (int)Mathf.Round(multipier) + 100;
            rankingSlot.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = currencyToAdd.ToString();
            collision.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            //Destroy(collision.GetComponent<AIController>());

        }
    }

    public void SetRankingMenuPlayer(int currencyToAdd)
	{
        rankingMenu.SetActive(true);
        // Add Soft Currency
        GameObject rankingSlot = Instantiate(prefabRankingSlot, rankingMenu.transform.GetChild(0).GetChild(1));
        rankingSlot.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = currencyToAdd.ToString();
        GameInfo.AddSoftCurrency(currencyToAdd);
        GameObject.FindObjectOfType<PlayerController>().GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        Destroy(GameObject.FindObjectOfType<PlayerController>().GetComponent<PlayerController>());
    }

}
