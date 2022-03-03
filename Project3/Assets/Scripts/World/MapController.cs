﻿using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[RequireComponent(typeof(Grid))]
public class MapController : MonoBehaviour
{
    public int numberOfTilemaps;
    public Transform playerTransform;
    public List<int>[] tilemapsDifficulties;
    public static int MaxtilemapDifficulty = 6;
    public static int actualDifficulty = 0;

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

    bool closedLevel = false;

    // Start is called before the first frame update
    void Start()
    {
        gridSize = GetComponent<Grid>().cellSize.x;
        doNotRepeat = new Queue<int>();
        tilemapInstances = new Queue<GameObject>();

        nodeDistance = gridSize / 3f;

        InitializeArrayTilemaps();
        LoadTilemaps();

        try
        {

            playerTransform = GameInfo.instance.player.transform;
        }catch(NullReferenceException e)
        {
            // THEN, WE ARE IN EDIT MODE AND DEBUGGAME WILL SET THE PROPERTY FOR US
        }




        if (GameInfo.instance == null || GameInfo.instance.levelID == 0 || GameInfo.instance.levelID == 3)
        {

            int numberOfTilemaps = 3;
            InstantiateTilemap(0);
            for (int i = 0; i < numberOfTilemaps; i++)
            {
                InstantiateTilemap(GetRandomTilemapIndex(actualDifficulty));
            }

        }else
        {
            closedLevel = true;
            // es cutre pero como el juego final usa randoms es temporal, despues de la entrega lo borramos.
            int[] level = null;

            if (GameInfo.instance.levelID == 1) {
                level = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            }
            else if (GameInfo.instance.levelID == 2 || GameInfo.instance.levelID == 4) {

                level = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9};
            }
            
            foreach(var tilemapIndex in level)
            {
                InstantiateTilemap(tilemapIndex);
            }

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

        return result;
    }

    void LoadTilemaps()
    {
        tilemaps = new GameObject[numberOfTilemaps];
        for (int i = 0; i < numberOfTilemaps; i++)
        {
            ProjectUtils.LoadTilemap(i);
            tilemaps[i] = ProjectUtils.LoadTilemap(i);

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

        if(index == 1)
        {

            obj.transform.position = Vector3.up * (totalHeightAcumulated+2f);
        }
        else
        {
            obj.transform.position = Vector3.up * totalHeightAcumulated;

        }
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
            if(!closedLevel)
                InstantiateTilemap(GetRandomTilemapIndex(actualDifficulty));

        }
    }

    void InitializeArrayTilemaps()
    {
        tilemapsDifficulties = new List<int>[MaxtilemapDifficulty];
        for(int i=0; i<tilemapsDifficulties.Length; i++)
        {
            tilemapsDifficulties[i] = new List<int>();
        }
    }
}


