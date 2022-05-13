using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameInfo : MonoBehaviour
{
    
    public class PreloadedSprites {


    }

    
    private const int AI_PLAYERS_COUNT = 3;


    public static List<ScriptablePerk> inventoryPerks = new List<ScriptablePerk>();
    public static List<ScriptablePerk> equippedPerks = new List<ScriptablePerk>();
    // Variables we need to save
    public static bool freePerkSlotUnlocked;
    public static int premiumPerkSlotsUnlocked;
    public static int freeCostUnlocked;
    public static int premiumCostUnlocked;
    public static int totalPerkCost;
    public static int equippedPerkCost;
    public static int softCurrency;

    public static bool showAIStatusInfo = false;

    public void ToggleAIStatusDebugInfo()
    {
        showAIStatusInfo = !showAIStatusInfo;
    }

    public void SetAppVolume(Slider vol){
        
    }

    public void OnMatchSceneClosed()
    {

        Destroy(player.gameObject);
        if(ai_players != null)
            foreach(var ai in ai_players)
                Destroy(ai.gameObject);
        
        Destroy(MapController.instanceGameObject);

        AudioController.instance.sounds.matchSong.Stop();
        StartMatchCountDown.matchStarted = false;

    }
   

    public class RunnerSkinInfo
    {
        public string baseSkinName;
        public string accessory1SkinName;
        public string accessory2SkinName;
        public string suitSkinName;
        public Color playerColor;
    }


    public static RunnerSkinInfo playerSkin = new RunnerSkinInfo() {
        baseSkinName = "Yellow",
        suitSkinName = "Default",
        accessory1SkinName = "Default",
        accessory2SkinName = "Default",

        // Placeholder color
        playerColor = new Color(1, 1, 1)
    };

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ai_playerPrefab;
    public static ScriptablePerk defaultPerk;

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject[] ai_players;

    public SaveData state;
    public static GameInfo instance;

    //Esto es temporal para la pre-alpha. De hecho los niveles dejaran de existir despues de la entrega.
    public int levelID;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }
        defaultPerk = Resources.Load<ScriptablePerk>("Perks/Default");
        instance = this;
        freePerkSlotUnlocked = false;
        premiumPerkSlotsUnlocked = 0;
        for (int i = 0; i < 4; i++)
        {
            equippedPerks.Add(defaultPerk);
        }
        LoadData();
        DontDestroyOnLoad(instance.gameObject);

        MapController.LoadTilemaps();


    }

    public void InitPlayers()
    {
        switch (levelID)
        {
            case 1:
            case 3:

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);

                RefreshSkin();

                break;
            case 2:

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);

                var ai = player.AddComponent<AIController>();

                // TODO: Set a based-in-something value
                ai.erraticBehaviourFactor = 0.5f;

                // TODO: Set selected skins
                RefreshSkin();

                break;
            case 4:

                ai_players = new GameObject[AI_PLAYERS_COUNT];
                /*
                    Doc:
                https://media.discordapp.net/attachments/905760062293811221/954412884895617044/unknown.png?width=1467&height=1467

                 */
                //TMP hardcoded desired
                float[] targetEBFOffsets = new float[] { -.1f, 0, .1f, -.05f }; 
                float[] initialEBFs = new float[] { .3f, .3f, .3f, .3f }; 

                for (int i = 0; i < AI_PLAYERS_COUNT; i++)
                {
                    GameObject ai_player = Instantiate(playerPrefab);
                    var aiController = ai_player.AddComponent<AIController>();
                    DontDestroyOnLoad(ai_player);
                    aiController.desiredPlayerEBFOffset = targetEBFOffsets[i];
                    aiController.erraticBehaviourFactor = initialEBFs[i];

                    Destroy(ai_player.gameObject.GetComponent<PlayerController>());

                    RefreshSkin(ai_player);
                    // TODO: Randomize skins
                    /*
                    var animController = ai_player.transform.GetChild(1).GetChild(0).GetComponent<PlayerAnimationController>();
                    animController.baseSkin = "Yellow";
                    animController.suitSkin = "MIBred";
                    animController.accessory1Skin = "Default";
                    animController.accessory2Skin = "Default";
                    */

                    ai_players[i] = ai_player;
                }

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);
                RefreshSkin();

                break;
            default:
                break;
        }

        
    }

    void RefreshSkin()
    {
        var playerAnimController = player.GetComponent<Runner>().aspect.GetComponent<PlayerAnimationController>();
        playerAnimController.baseSkin = playerSkin.baseSkinName;
        playerAnimController.suitSkin = playerSkin.suitSkinName;
        playerAnimController.accessory1Skin = playerSkin.accessory1SkinName;
        playerAnimController.accessory2Skin = playerSkin.accessory2SkinName;
        Material mat = playerAnimController.gameObject.GetComponent<SpriteRenderer>().material;
        if (playerSkin.playerColor != new Color(1, 1, 1))
        {
            mat.SetFloat("_ColorChangeTolerance", 0.2f);
            mat.SetColor("_ColorChangeNewCol", playerSkin.playerColor);
        }
        else
        {
            mat.SetFloat("_ColorChangeTolerance", 1.0f);

        }

    }

    void RefreshSkin(GameObject _player)
    {
        var playerAnimController = _player.transform.GetChild(1).GetChild(0).GetComponent<PlayerAnimationController>();
        playerAnimController.baseSkin = "Yellow";
        playerAnimController.suitSkin = "MIBred";
        playerAnimController.accessory1Skin = "Default";
        playerAnimController.accessory2Skin = "Default";
        Material mat = playerAnimController.gameObject.GetComponent<SpriteRenderer>().material;
        int skinIndx = UnityEngine.Random.Range(0, 4);
        mat.SetFloat("_ColorChangeTolerance", 0.2f);
        switch (UnityEngine.Random.Range(0, 4))
        {
            case 0:
                mat.SetColor("_ColorChangeNewCol", new Color(0.2f, 0.75f, 0.75f));
                break;
            case 1:
                mat.SetColor("_ColorChangeNewCol", new Color(0.3f, 0.8f, 0.3f));
                break;
            case 2:
                mat.SetColor("_ColorChangeNewCol", new Color(0.7f, 0.3f, 1.0f));
                break;
            case 3:
                mat.SetFloat("_ColorChangeTolerance", 1.0f);
                break;
        }

        
    }

    public void SaveData()
    {
        //state.SaveInventory(inventoryPerks);
        state.UpdateState(instance);
        SaveSys.SaveData(state);
    }

    public void LoadData()
    {
        SaveData data = SaveSys.LoadMenuData();
        if(data != null)
        {
            state = data;
            playerSkin.baseSkinName = data.baseSkin;
            playerSkin.suitSkinName = data.suitSkin;
            playerSkin.accessory1SkinName = data.accessory1;
            playerSkin.accessory2SkinName = data.accessory2;
            playerSkin.playerColor = new Color(data.colorBaseSkin[0], data.colorBaseSkin[1], data.colorBaseSkin[2]);

            equippedPerks.Clear();
            if(data.equipped != null)
                foreach(var perk in data.equipped)
                {
                    var aux = Resources.Load<ScriptablePerk>("Perks/" + perk.name);
                    equippedPerks.Add(aux);
                }
            inventoryPerks.Clear();
            if(data.inventory != null)
                foreach(var perk in data.inventory)
                {
                    var aux = Resources.Load<ScriptablePerk>("Perks/" + perk.name);
                    inventoryPerks.Add(aux);
                }
            
            freePerkSlotUnlocked = data.freePerkSlotUnlocked;
            premiumPerkSlotsUnlocked = data.premiumPerkSlotsUnlocked;
            freeCostUnlocked = data.freeCostUnlocked;
            premiumCostUnlocked = data.premiumCostUnlocked;
            totalPerkCost = data.totalPerkCost;
            equippedPerkCost = data.equippedPerkCost;
            softCurrency = data.softCurrency;

            Debug.Log(playerSkin.baseSkinName + " " + playerSkin.suitSkinName + " " + playerSkin.accessory1SkinName + " " + playerSkin.accessory2SkinName + "Unlocked: " + equippedPerkCost);
        }
        else
        {
            data = new SaveData();

            state = data;
            playerSkin.baseSkinName = data.baseSkin;
            playerSkin.suitSkinName = data.suitSkin;
            playerSkin.accessory1SkinName = data.accessory1;
            playerSkin.accessory2SkinName = data.accessory2;

            equippedPerks = new List<ScriptablePerk>();
            for (int i = 0; i < 4; i++)
            {
                var aux = Resources.Load<ScriptablePerk>("Perks/Default");
                equippedPerks.Add(aux);
            }
            inventoryPerks = new List<ScriptablePerk>();

            freePerkSlotUnlocked = data.freePerkSlotUnlocked;
            premiumPerkSlotsUnlocked = data.premiumPerkSlotsUnlocked;
            freeCostUnlocked = data.freeCostUnlocked;
            premiumCostUnlocked = data.premiumCostUnlocked;
            totalPerkCost = data.totalPerkCost;
            equippedPerkCost = data.equippedPerkCost;
        }
    }


    public static void AddSoftCurrency(int amount)
    {
        GameInfo.softCurrency += amount;
    }

    public static void AddPerkToInventory(ScriptablePerk perk)
    {
        inventoryPerks.Add(perk);
    }


    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveData();
    }
    private void OnApplicationQuit()
    {
        SaveData();
    }
}
