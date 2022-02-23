using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;



public class MatchController : MonoBehaviour
{
    public SceneReference mainMenu;

    public static MatchController instance;

    private void Awake()
    {
        instance = this;
    }

    public void Quit()
    {
        GoToMainMenu();
    }

    void GoToMainMenu()
    {
        SceneManager.LoadScene(mainMenu);
    }
}
