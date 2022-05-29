using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class MatchController : MonoBehaviour
{
    public SceneReference mainMenu;

    public float startingTime;
    public static MatchController instance;
    public Slider loadingSlider;

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
        StartCoroutine(LoadAsyncScene(mainMenu));
    }

    IEnumerator LoadAsyncScene(SceneReference scene)
    {
        yield return new WaitForSeconds(0.1f);

        GameInfo.instance.InitPlayers();
        AsyncOperation ao = SceneManager.LoadSceneAsync(scene);
        ao.allowSceneActivation = false;

        while (!ao.isDone)
        {
            float progress = Mathf.Clamp01(ao.progress / 0.9f);
            loadingSlider.value = progress;
            // Loading completed
            if (ao.progress == 0.9f)
            {
                ao.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
