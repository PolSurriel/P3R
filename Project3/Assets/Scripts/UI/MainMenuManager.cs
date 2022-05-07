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
    public Slider loadingSlider;
    [SerializeField] Text softCurrenyTxt;
    [SerializeField] Sprite MIBSuitPrev;
    [SerializeField] Sprite defSuitPrev;


    private ModeType modeType;

    [SerializeField] private Text raceText;

    private enum ModeType
    {
        PLAYER,
        AI,
        VERSUS,
        INFINITE

    }


    private void Awake()
    {
        //AudioController.instance.sounds.jump.Play();
        RefreshCurrencies();
        StartCoroutine(InitaliceMainMenu(1.0f));
    }

    private void Start()
    {
        modeType = ModeType.PLAYER;
        raceText.text = modeType.ToString();
    }

    private void Update()
    {

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
        {
            aux.CheckPerks();
            aux.RefreshCosts();
        }
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 3, 0), 0.25f);
    }
    public void SwipeToSettings()
    {
        allMenusContainer.DOAnchorPos(new Vector2(PIXELS_BETWEEN_MENUS * 4, 0), 0.25f);
    }

    public void LoadLevel1()
    {
        GameInfo.instance.levelID = 1;
        StartCoroutine(LoadAsyncScene(level1));

        GameInfo.instance.InitPlayers();
    }
    public void LoadLevel2()
    {
        GameInfo.instance.levelID = 2;
        StartCoroutine(LoadAsyncScene(level2));

        GameInfo.instance.InitPlayers();
    }
    public void LoadLevel3()
    {
        GameInfo.instance.levelID = 3;
        StartCoroutine(LoadAsyncScene(level3));

        GameInfo.instance.InitPlayers();
    }
    public void LoadLevel4()
    {
        // TODO: to debug has changed levelID to 2 instead of 3
        GameInfo.instance.levelID = 4;
        StartCoroutine(LoadAsyncScene(level3));

        GameInfo.instance.InitPlayers();
    }

    public void PlayMode()
    {
        switch (modeType)
        {
            case ModeType.PLAYER:
                LoadLevel1();
                break;
            case ModeType.VERSUS:
                LoadLevel4();
                break;
            case ModeType.INFINITE:
                LoadLevel3();
                break;
            case ModeType.AI:
                LoadLevel2();
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
        //modeType = (ModeType)(((int)modeType - 1) % 3);
        switch (modeType)
        {
            case ModeType.PLAYER:
                modeType = ModeType.AI;
                break;
            case ModeType.VERSUS:
                modeType = ModeType.PLAYER;
                break;
            case ModeType.INFINITE:
                modeType = ModeType.VERSUS;
                break;
            case ModeType.AI:
                modeType = ModeType.INFINITE;
                break;
            default:
                break;
        }

        raceText.text = modeType.ToString();
    }

    public void NextMode()
    {
        //modeType = (ModeType)(((int)modeType + 1) % 3);
        switch (modeType)
        {
            case ModeType.PLAYER:
                modeType = ModeType.VERSUS;
                break;
            case ModeType.VERSUS:
                modeType = ModeType.INFINITE;
                break;
            case ModeType.INFINITE:
                modeType = ModeType.AI;
                break;
            case ModeType.AI:
                modeType = ModeType.PLAYER;
                break;
            default:
                break;
        }

        raceText.text = modeType.ToString();
    }

    public void SelectSkin(GameObject _sprite)
    {
        GameInfo.playerSkin.playerColor = _sprite.GetComponent<Image>().color;
        StartCoroutine(InitaliceMainMenu(0.1f));
    }

    public void SelectSuit(string _name)
    {
        GameInfo.playerSkin.suitSkinName = _name;
        Debug.Log(_name + " suit selected");
        StartCoroutine(InitaliceMainMenu(0.1f));
    }

    public void RefreshCurrencies()
    {
        softCurrenyTxt.text = GameInfo.softCurrency.ToString();
    }

    public void RefreshSkinPrev()
    {
        if(GameInfo.playerSkin.playerColor != new Color(1.0f, 1.0f, 1.0f))
        {
            GameObject.Find("SkinTexture").GetComponent<Image>().material.SetFloat("_ColorChangeTolerance", 0.2f);
            GameObject.Find("SkinTexture").GetComponent<Image>().material.SetColor("_ColorChangeNewCol", GameInfo.playerSkin.playerColor);
        }
        else
        {
            GameObject.Find("SkinTexture").GetComponent<Image>().material.SetFloat("_ColorChangeTolerance", 1.0f);
        }

        if(GameInfo.playerSkin.suitSkinName == "MIBred")
        {
            GameObject.Find("SuitTexturePrev").GetComponent<Image>().sprite = MIBSuitPrev;
        }
        else
        {
            GameObject.Find("SuitTexturePrev").GetComponent<Image>().sprite = defSuitPrev;
        }
        
    }

    public void AddSoftCurrency(int amount)
    {
        GameInfo.softCurrency += amount;
        RefreshCurrencies();
    }

    IEnumerator InitaliceMainMenu(float _waitTime)
    {
        // Initialice and refresh all variables needed in menu
        // Ex: Currency amount, skin selected previsualizer, etc.
        yield return new WaitForSeconds(_waitTime);

        RefreshCurrencies();
        RefreshSkinPrev();

    }

    IEnumerator LoadAsyncScene(SceneReference scene)
    {
        yield return null;

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


