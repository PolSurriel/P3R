using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
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

    public class RunnerSkinInfo
    {
        public string baseSkinName;
        public string accessory1SkinName;
        public string accessory2SkinName;
        public string suitSkinName;
    }

    
    public static RunnerSkinInfo playerSkin = new RunnerSkinInfo (){
        baseSkinName = "Yellow",
        suitSkinName = "Default",
        accessory1SkinName = "Default",
        accessory2SkinName = "Default"
    };


    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ai_playerPrefab;
    [SerializeField] private ScriptablePerk def;

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
        instance = this;
        freePerkSlotUnlocked = false;
        premiumPerkSlotsUnlocked = 0;
        for (int i = 0; i < 4; i++)
        {
            equippedPerks.Add(def);
        }
        LoadData();
        DontDestroyOnLoad(instance.gameObject);

        ai_players = new GameObject[AI_PLAYERS_COUNT];

    }

    public void InitPlayers()
    {
        switch (levelID)
        {
            case 1:

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);

                break;
            case 2:

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);

                var ai = player.AddComponent<AIController>();

                // TODO: Set a based-in-something value
                ai.erraticBehaviourFactor = 0.5f;

                // TODO: Set selected skins
                var playerAnimController = player.GetComponent<Runner>().aspect.GetComponent<PlayerAnimationController>();
                playerAnimController.baseSkin = "Yellow";
                playerAnimController.suitSkin = "Default";
                playerAnimController.accessory1Skin = "Default";
                playerAnimController.accessory2Skin = "Default";

                break;
            case 4:

                for (int i = 0; i < AI_PLAYERS_COUNT; i++)
                {
                    GameObject ai_player = Instantiate(ai_playerPrefab);
                    DontDestroyOnLoad(ai_player);
                    
                    // TODO: Randomize skins
                    var animController = ai_player.GetComponent<PlayerAnimationController>();
                    animController.baseSkin = "Yellow";
                    animController.suitSkin = "Default";
                    animController.accessory1Skin = "Default";
                    animController.accessory2Skin = "Default";

                    ai_players[i] = ai_player;
                }

                player = Instantiate(playerPrefab);
                DontDestroyOnLoad(player);

                break;
            default:
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

            
            if(data.equipped != null)
                foreach(var perk in data.equipped)
                {
                    var aux = Resources.Load<ScriptablePerk>("Perks/" + perk.name);
                    equippedPerks.Add(aux);
                }
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

            Debug.Log(playerSkin.baseSkinName + " " + playerSkin.suitSkinName + " " + playerSkin.accessory1SkinName + " " + playerSkin.accessory2SkinName + "Unlocked: " + equippedPerkCost);
        }
    }

    public void AddPerk()
    {
        inventoryPerks.Add(Resources.Load<ScriptablePerk>("Perks/Ligther"));
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
