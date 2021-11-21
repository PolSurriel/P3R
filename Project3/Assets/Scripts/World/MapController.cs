using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class MapController : MonoBehaviour
{
    public int numberOfTilemaps;
    public Transform playerTransform;

    Queue<int> doNotRepeat;
    Queue<GameObject> tilemapInstances;
    const int MAX_DONOTREPEAT_ITEMS = 1;
    const int MAX_CONCURRENT_TILEMAPS = 5;

    GameObject[] tilemaps;
    float totalHeightAcumulated;
    float gridSize;
    int enqueuedCount;

    public float nodeDistance;


    public Vector2 nodeZeroPosition;

    public List<NodeListWrapper> nodesGlobalMatrix;

    // Start is called before the first frame update
    void Start()
    {
        gridSize = GetComponent<Grid>().cellSize.x;
        doNotRepeat = new Queue<int>();
        tilemapInstances = new Queue<GameObject>();

        nodeDistance = gridSize / 3f;


        LoadTilemaps();



        int numberOfTilemaps = 3;
        InstantiateTilemap(0);
        for (int i = 0; i < numberOfTilemaps; i++)
        {
            InstantiateTilemap(GetRandomTilemapIndex());
        }


    }

    


    int GetRandomTilemapIndex()
    {
        int result;

        do
        {
            result = Random.Range(1, numberOfTilemaps);

        } while (doNotRepeat.Contains(result));

        return result;
    }

    void LoadTilemaps()
    {
        tilemaps = new GameObject[numberOfTilemaps];
        for (int i = 0; i < numberOfTilemaps; i++)
        {
            ProjectUtils.LoadTilemap(i);

            tilemaps[i] = ProjectUtils.LoadTilemap(i);
        }
    }

    

    void InstantiateTilemap(int index)
    {
        if (doNotRepeat.Contains(index))
            throw new System.Exception("Tilemap repeated. You cannot instantiate a repeated tilemap until "+MAX_DONOTREPEAT_ITEMS+" (at least) new tilemaps have been instanced since it's last instantiation.)");

        doNotRepeat.Enqueue(index);
        enqueuedCount++;

        var obj = Instantiate(tilemaps[index]);

        tilemapInstances.Enqueue(obj);

        obj.transform.position = Vector3.up * totalHeightAcumulated;
        obj.transform.SetParent(this.transform);

        totalHeightAcumulated += obj.GetComponent<Tilemap>().size.y * gridSize;

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
            InstantiateTilemap(GetRandomTilemapIndex());

        }
    }
}


