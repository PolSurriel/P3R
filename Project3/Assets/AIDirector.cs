using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(MapController))]
public class AIDirector : MonoBehaviour
{

    MapController mapController;

    [Button("Precalculate AI Graph")]
    void PrecalculateAIGraph()
    {

        SetMapController();
        
        for (int i = 0; i < mapController.numberOfTilemaps; i++)
        {
            PrecalculateTilemapAIGraph(ProjectUtils.LoadTilemap(i));
        }

    }

    [Button("Precalculate AI Graph (Empty tilemaps only)")]
    void PrecalculateOnlyEmptyAIGraph()
    {
        SetMapController();

        for (int i = 0; i < mapController.numberOfTilemaps; i++)
        {
            var obj = ProjectUtils.LoadTilemap(i).GetComponent<TilemapAIInfo>();

            if (!obj.NodesPrecalculated())
            {
                PrecalculateTilemapAIGraph(obj.gameObject);
            }
        }

    }

    void SetMapController()
    {
        if (mapController == null)
            mapController = GetComponent<MapController>();

    }




    void PrecalculateTilemapAIGraph(GameObject tilemapGameObject)
    {

        
        var obj = PrefabUtility.InstantiatePrefab(tilemapGameObject) as GameObject;

        obj.transform.SetParent(transform);

        obj.GetComponent<TilemapAIInfo>().PrecalculateGraphNodes();

        bool success = false;
        PrefabUtility.SaveAsPrefabAsset(obj, "Assets/Resources/" + obj.name+".prefab", out success);

        DestroyImmediate(obj);
    }


}
