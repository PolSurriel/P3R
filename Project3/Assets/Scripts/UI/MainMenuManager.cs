using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public SceneReference level1, level2, level3;

    public void LoadLevel1()
    {
        SceneManager.LoadScene(level1);
    }
    public void LoadLevel2()
    {
        SceneManager.LoadScene(level2);
    }
    public void LoadLevel3()
    {
        SceneManager.LoadScene(level3);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}
