using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public SceneReference level1, level2, level3;

    private ModeType modeType;

    [SerializeField] private Text raceText;

    private enum ModeType
    {
        PLAYER,
        AI,
        VERSUS
    }

    private void Awake()
    {
        //AudioController.instance.sounds.jump.Play();
    }

    private void Start()
    {
        modeType = ModeType.PLAYER;
        raceText.text = modeType.ToString();
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

    public void PlayMode()
    {
        switch (modeType)
        {
            case ModeType.PLAYER:
                LoadLevel1();
                break;
            case ModeType.AI:
                LoadLevel2();
                break;
            case ModeType.VERSUS:
                LoadLevel3();
                break;
            default:
                break;
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PreviousMode()
    {
        switch (modeType)
        {
            case ModeType.PLAYER:
                modeType = ModeType.VERSUS;
                break;
            case ModeType.AI:
                modeType = ModeType.PLAYER;
                break;
            case ModeType.VERSUS:
                modeType = ModeType.AI;
                break;
            default:
                break;
        }

        raceText.text = modeType.ToString();
    }

    public void NextMode()
    {
        switch (modeType)
        {
            case ModeType.PLAYER:
                modeType = ModeType.AI;
                break;
            case ModeType.AI:
                modeType = ModeType.VERSUS;
                break;
            case ModeType.VERSUS:
                modeType = ModeType.PLAYER;
                break;
            default:
                break;
        }

        raceText.text = modeType.ToString();
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
