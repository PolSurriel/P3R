using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public SceneReference level1, level2, level3;

    public void LoadLevel1()
    {
        GameInfo.instance.levelID = 1;
        SceneManager.LoadScene(level1);
    }
    public void LoadLevel2()
    {
        GameInfo.instance.levelID = 2;
        SceneManager.LoadScene(level2);
    }
    public void LoadLevel3()
    {

        GameInfo.instance.levelID = 3;
        SceneManager.LoadScene(level3);
    }
    public void QuitGame()
    {
        Application.Quit();
    }

}

// Clase que re-estrctura una array para añadir un elemento
public static class Extensions
{
    public static T[] Append<T>(this T[] array, T item)
    {
        if (array == null)
        {
            return new T[] { item };
        }
        T[] result = new T[array.Length + 1];
        array.CopyTo(result, 0);
        result[array.Length] = item;
        return result;
    }
}
