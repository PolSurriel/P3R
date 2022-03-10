using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenuManager : MonoBehaviour
{
    const int PIXELS_BETWEEN_MENUS = -1471;

    public SceneReference level1, level2, level3;
    public RectTransform allMenusContainer;

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

    public void SwipeToShop()
    {
        allMenusContainer.DOAnchorPos(new Vector2(0, 0), 0.25f);
    }
    public void SwipeToSkins()
    {
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 1, 0), 0.25f);
    }
    public void SwipeToMain()
    {
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 2, 0), 0.25f);
    }
    public void SwipeToSkills()
    {
        SkillsEquippedManager aux = GameObject.FindObjectOfType<SkillsEquippedManager>();
        if (aux != null)
            aux.CheckPerks();
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 3, 0), 0.25f);
    }
    public void SwipeToSettings()
    {
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 4, 0), 0.25f);
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
        modeType = (ModeType)(((int)modeType - 1) % 3);
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
        modeType = (ModeType)(((int)modeType + 1) % 3);
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

    public void SelectSkin(string _name)
    {
        GameInfo.playerSkin.baseSkinName = _name;
        Debug.Log(_name + " Skin selected");
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
