using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo : MonoBehaviour
{
    private const int AI_PLAYERS_COUNT = 3;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject ai_playerPrefab;

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

                player = Instantiate(ai_playerPrefab);
                DontDestroyOnLoad(player);

                break;
            case 4:

                for (int i = 0; i < AI_PLAYERS_COUNT; i++)
                {
                    GameObject ai_player = Instantiate(ai_playerPrefab);
                    DontDestroyOnLoad(ai_player);

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
