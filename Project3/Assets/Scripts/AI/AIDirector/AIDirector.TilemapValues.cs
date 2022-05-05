using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIDirector : MonoBehaviour
{
    const string CONFIG_NAME = "Global tilemap configuration";
    const string CLAMP_CONFIG = CONFIG_NAME + "/Apply clamp to all tilemaps";

  


    
    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMaxWaitTime;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float maxWaitTimeClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMinWaitTime;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float minWaitTimeClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMaxAngleDeviation;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float maxAngleDeviationClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMinAngleDeviation;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float minAngleDeviationClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    [Button("Apply clamp")]
    void ApplyClamp()
    {

        clampMaxWaitTime = false;
        clampMinWaitTime = false;
        clampMaxAngleDeviation = false;
        clampMinAngleDeviation = false;

    }

    [Title("Multiply all tilemaps max&min timebeforejump by scalar")]
    [FoldoutGroup(CONFIG_NAME)]
    public float scalar = 1f;

    [FoldoutGroup(CONFIG_NAME)]
    [Button("Apply")]
    void ApplyScalar()
    {

        int modifiedCount = 0; 
        MapController.ForEachTilemapSRCPath((string path)=> {
            // get a reference to the prefab itself, not a clone or instantiation: 
            GameObject editablePrefab = AssetDatabase.LoadAssetAtPath(path,
                 typeof(GameObject)) as GameObject;

            for (int i = 0; i < editablePrefab.transform.childCount; i++)
            {
                VariablesTilemap vt = editablePrefab.transform.GetChild(i).GetComponent<VariablesTilemap>();

                if(vt != null)
                {
                    vt.timeVariationTrigger *= scalar;
                    AssetDatabase.DeleteAsset(path);
                    AssetDatabase.CreateAsset(editablePrefab, path);
                    modifiedCount++;
                    
                }
            }


        });
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Modified {modifiedCount} tilemaps.");



    }

}
