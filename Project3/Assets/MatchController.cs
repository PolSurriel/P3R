using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MatchController : MonoBehaviour
{
    public SceneReference mainMenu;

    public float startingTime;
    public static MatchController instance;

    private void Start()
    {
        startingTime = Time.time;
    }

    private void Awake()
    {
        instance = this;
    }

    public void Quit()
    {
        if(GameInfo.instance.levelID == 3)
        {
            float multipier = Mathf.Clamp((120.0f - (Time.time - startingTime)), 0, 120);
            int currencyToAdd = (int)Mathf.Round(Mathf.Clamp((Time.time - startingTime), 0, 300));
            GameInfo.AddSoftCurrency(currencyToAdd);
        }
        GoToMainMenu();
    }

    void GoToMainMenu()
    {
        SceneManager.LoadSceneAsync(mainMenu);
    }
}
