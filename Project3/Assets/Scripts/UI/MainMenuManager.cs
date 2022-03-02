using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public SceneReference level1, level2, level3;

    private LevelType levelType;

    private enum LevelType
    {
        PLAYER,
        AI,
        VERSUS
    }

    private void Awake()
    {
        //AudioController.instance.sounds.jump.Play();
    }

    public void LoadLevel1()
    {
        GameInfo.instance.levelID = 1;
        SceneManager.LoadScene(level1);

        GameInfo.instance.InitPlayers();
    }
    public void LoadLevel2()
    {
        GameInfo.instance.levelID = 2;
        SceneManager.LoadScene(level2);

        GameInfo.instance.InitPlayers();
    }
    public void LoadLevel3()
    {
        // TODO: to debug has changed levelID to 2 instead of 3
        GameInfo.instance.levelID = 4;
        SceneManager.LoadScene(level3);

        GameInfo.instance.InitPlayers();
    }
    public void QuitGame()
    {
        Application.Quit();
    }

    public void PreviousLevel()
    {
        switch (levelType)
        {
            case LevelType.PLAYER:
                levelType = LevelType.VERSUS;
                break;
            case LevelType.AI:
                levelType = LevelType.PLAYER;
                break;
            case LevelType.VERSUS:
                levelType = LevelType.AI;
                break;
            default:
                break;
        }
    }

    public void NextLevel()
    {
        switch (levelType)
        {
            case LevelType.PLAYER:
                levelType = LevelType.AI;
                break;
            case LevelType.AI:
                levelType = LevelType.VERSUS;
                break;
            case LevelType.VERSUS:
                levelType = LevelType.PLAYER;
                break;
            default:
                break;
        }
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
