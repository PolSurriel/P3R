using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
    private const int AI_PLAYERS_COUNT = 3;


    public void OnMatchSceneClosed()
    {

        Destroy(player.gameObject);
        if(ai_players != null)
            foreach(var ai in ai_players)
                Destroy(ai.gameObject);
        
        Destroy(MapController.instanceGameObject);

    }
   

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

    [HideInInspector] public GameObject player;
    [HideInInspector] public GameObject[] ai_players;

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
        DontDestroyOnLoad(instance.gameObject);


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

                ai_players = new GameObject[AI_PLAYERS_COUNT];
                /*
                    Doc:
                https://media.discordapp.net/attachments/905760062293811221/954412884895617044/unknown.png?width=1467&height=1467

                 */
                //TMP hardcoded desired
                float[] targetEBFOffsets = new float[] { -.1f, 0, .1f, -.05f }; 
                float[] initialEBFs = new float[] { .5f, .5f, .5f, .5f }; 

                for (int i = 0; i < AI_PLAYERS_COUNT; i++)
                {
                    GameObject ai_player = Instantiate(playerPrefab);
                    var aiController = ai_player.AddComponent<AIController>();
                    DontDestroyOnLoad(ai_player);
                    aiController.desiredPlayerEBFOffset = targetEBFOffsets[i];
                    aiController.erraticBehaviourFactor = initialEBFs[i];

                    Destroy(ai_player.gameObject.GetComponent<PlayerController>());

                    // TODO: Randomize skins
                    var animController = ai_player.transform.GetChild(1).GetChild(0).GetComponent<PlayerAnimationController>();
                    animController.baseSkin = "Yellow";
                    animController.suitSkin = "MIBred";
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



}
