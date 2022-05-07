using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEnd : MonoBehaviour
{
    const int CurrencyMult = 10;
    float startingTime;
    private void Start()
    {
        if (GameInfo.instance == null || GameInfo.instance.levelID == 0 || GameInfo.instance.levelID == 3)
        {
            Destroy(gameObject);
        }
        startingTime = Time.time;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("end");

        if(collision.GetComponent<PlayerController>() != null || (collision.GetComponent<AIController>() != null && GameInfo.instance.levelID == 2))
        {
            // Add Soft Currency
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 0);
            int currencyToAdd = CurrencyMult * (int)Mathf.Round(multipier) + 100;
            GameInfo.AddSoftCurrency(currencyToAdd);

            GameObject.FindObjectOfType<MapController>().ResetMap();
            MatchController.instance.Quit();
        }
    }

}
