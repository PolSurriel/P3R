using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;
using Sirenix.OdinInspector;



[RequireComponent(typeof(Grid))]
public class MapController : MonoBehaviour
{

    public bool debugMode;
    public List<GameObject> debugTilemaps = new List<GameObject>();


    public static int numberOfTilemaps = 29;

    [HideInInspector]
    public Transform playerTransform;
    public static List<int>[] tilemapsDifficulties;
    
    public static int MaxtilemapDifficulty = 8;
    public static int actualDifficulty = 0;

    public static GameObject instanceGameObject;

    Queue<int> doNotRepeat;
    Queue<GameObject> tilemapInstances;
    const int MAX_DONOTREPEAT_ITEMS = 1;
    const int MAX_CONCURRENT_TILEMAPS = 5;


    public static bool tilemapsInizializated = false;
    public static GameObject[] tilemaps;
    float totalHeightAcumulated;
    float gridSize;
    int enqueuedCount;

    GameObject lineaMeta;
    



    bool closedLevel = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameInfo.instance.levelID == 3)
            GameInfo.instance.platformsCountDown = GameInfo.INITIAL_TIME;
        lineaMeta = FindObjectOfType<LevelEnd>().gameObject;

        instanceGameObject = this.gameObject;
        gridSize = GetComponent<Grid>().cellSize.x;
        doNotRepeat = new Queue<int>();
        tilemapInstances = new Queue<GameObject>();

        LoadTilemaps();

        try
        {

            playerTransform = GameInfo.instance.player.transform;
        }catch(NullReferenceException e)
        {
            // THEN, WE ARE IN EDIT MODE
            //playerTransform = FindObjectOfType<AIController>().transform;
            //playerTransform = FindObjectOfType<PlayerController>().transform;
        }

        

        if (GameInfo.instance == null || GameInfo.instance.levelID == 0 || GameInfo.instance.levelID == 3 || GameInfo.instance.levelID == 4)
        {
            if (GameInfo.instance != null && GameInfo.instance.levelID == 3)
                lineaMeta.SetActive(false);

            int numberOfTilemaps = 3;
            InstantiateTilemap(0);

            if(!debugMode && GameInfo.instance == null)
                for (int i = 0; i < numberOfTilemaps; i++)
                    InstantiateTilemap(GetRandomTilemapIndex(actualDifficulty));
            else
            {
                foreach(var tm in debugTilemaps)
                {
                    InstantiateTilemap(tm);
                }
            }


        }else
        {
            closedLevel = true;
            // es cutre pero como el juego final usa randoms es temporal, despues de la entrega lo borramos.
            int[] level = null;

            if (GameInfo.instance.levelID == 1) {
                level = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            }

            InstantiateTilemap(0);
            if ( GameInfo.instance.levelID == 2)
            {
                level = new int[] { 0, 1, 8, 6, 4, 19, 3, 7, 8, 9 };
            }
            if (!debugMode && GameInfo.instance == null)
                foreach (var tilemapIndex in level)
                {
                    InstantiateTilemap(tilemapIndex);
                }
            else
            {
                foreach (var tm in debugTilemaps)
                {
                    InstantiateTilemap(tm);
                }
            }

        }


    }


    void InstantiateTilemap(GameObject tilemap)
    {
        int index = 0;
        foreach(var tm in tilemaps)
        {
            if (tm == tilemap)
            {
                InstantiateTilemap(index);
            }

            index++;
        }
    }

    int GetRandomTilemapIndex(int difficulty)
    {
        int result;
        do
        {
            // Gets an index from List tilemapsDifficulties
            result = UnityEngine.Random.Range(0, tilemapsDifficulties[difficulty].Count);

        } while (doNotRepeat.Contains(tilemapsDifficulties[difficulty][result]));

        return tilemapsDifficulties[difficulty][result];
    }

    public delegate void TilemapPathIteration(string path);
    public static void ForEachTilemapSRCPath(TilemapPathIteration method)
    {
        int i = 1;
#if UNITY_EDITOR
        do
        {
            string path = $"Assets/Resources/Tilemaps/Tilemap{i++}.prefab";
            
            if (AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) != null)
            {
                method(path);
            }
            else break;

        } while (true);

#endif
    }


    public delegate void EditTilemapPrefab(ref GameObject tilemapPrefab);
    public static void EditEachTilemapPrefab(EditTilemapPrefab method)
    {
#if UNITY_EDITOR
        int modifiedCount = 0;
        MapController.ForEachTilemapSRCPath((string path) => {
            // get a reference to the prefab itself, not a clone or instantiation: 
            GameObject editablePrefab = AssetDatabase.LoadAssetAtPath(path,
                 typeof(GameObject)) as GameObject;

            method(ref editablePrefab);
            PrefabUtility.SavePrefabAsset(editablePrefab);
            modifiedCount++;

        });

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Modified {modifiedCount} tilemaps.");
#endif
    }



    public static void LoadTilemaps()
    {
        if (tilemapsInizializated)
            return;

        tilemapsInizializated = true;

        InitializeArrayTilemaps();

        tilemaps = new GameObject[numberOfTilemaps];
        for (int i = 0; i < numberOfTilemaps; i++)
        {
            ProjectUtils.LoadTilemap(i);
            tilemaps[i] = ProjectUtils.LoadTilemap(i);
            tilemaps[i].GetComponent<Tilemap>().CompressBounds();

            //Lee la array de que dificultades se tiene que poner este tilemap
            int[] aux = tilemaps[i].GetComponentInChildren<VariablesTilemap>().GetDifficulties();
            for (int j = 0; j < aux.Length; j++)
            {
                // i =  num. tilemap aux[j] = dificultad
                tilemapsDifficulties[aux[j]].Add(i);
            }
        }
    }

    void InstantiateTilemap(int index)
    {
        if (doNotRepeat.Contains(index) && !closedLevel)
            throw new System.Exception("Tilemap repeated. You cannot instantiate a repeated tilemap until "+MAX_DONOTREPEAT_ITEMS+" (at least) new tilemaps have been instanced since it's last instantiation.)");
        
        doNotRepeat.Enqueue(index);
        enqueuedCount++;

        var obj = Instantiate(tilemaps[index]);

        tilemapInstances.Enqueue(obj);

      
        obj.transform.position = Vector3.up * totalHeightAcumulated;
        obj.transform.SetParent(this.transform);
        
        totalHeightAcumulated += obj.GetComponent<Tilemap>().size.y * gridSize;

        if (closedLevel)
            return;

        if(enqueuedCount > MAX_DONOTREPEAT_ITEMS)
        {
            doNotRepeat.Dequeue();
        }

        if (enqueuedCount > MAX_CONCURRENT_TILEMAPS)
        {
            Destroy(tilemapInstances.Peek());
            tilemapInstances.Dequeue();
        }

    }


    // Update is called once per frame
    void Update()
    {
        

        if (playerTransform.position.y > totalHeightAcumulated - Camera.main.orthographicSize * 2)
        {
            if(!closedLevel || (lineaMeta.activeSelf && totalHeightAcumulated < lineaMeta.transform.position.y))
                InstantiateTilemap(GetRandomTilemapIndex(actualDifficulty));

        }
    }

    static void InitializeArrayTilemaps()
    {
        tilemapsDifficulties = new List<int>[MaxtilemapDifficulty];
        for(int i=0; i<tilemapsDifficulties.Length; i++)
        {
            tilemapsDifficulties[i] = new List<int>();
        }
    }

    public void ResetMap()
    {
        if (GameObject.FindObjectOfType<StartMatchCountDown>() != null)
            GameObject.FindObjectOfType<StartMatchCountDown>().ResetCountDown();
        doNotRepeat.Clear();
        tilemapInstances.Clear();
        lineaMeta.SetActive(true);
    }
}


