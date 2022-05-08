using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIDirector : MonoBehaviour
{
    const string CONFIG_NAME = "Global tilemap configuration";
    const string CLAMP_CONFIG = CONFIG_NAME + "/Clamp values";




    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMinWaitTime;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float minWaitTimeClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMaxWaitTime;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float maxWaitTimeClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMinAngleDeviation;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float minAngleDeviationClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    public bool clampMaxAngleDeviation;
    [FoldoutGroup(CLAMP_CONFIG)]
    public float maxAngleDeviationClamp;

    [FoldoutGroup(CLAMP_CONFIG)]
    [Button("Apply clamp")]
    void ApplyClamp()
    {

        
        MapController.EditEachTilemapPrefab((ref GameObject prefab)=> {
            for (int i = 0; i < prefab.transform.childCount; i++)
            {
                VariablesTilemap vt = prefab.transform.GetChild(i).GetComponent<VariablesTilemap>();

                if (vt != null)
                {

                    if (clampMaxAngleDeviation)
                        if(vt.angleVariationTrigger > maxAngleDeviationClamp)
                            vt.angleVariationTrigger = maxAngleDeviationClamp; 

                    if (clampMinAngleDeviation)
                        if (vt.angleVariationTrigger < minAngleDeviationClamp)
                            vt.angleVariationTrigger = minAngleDeviationClamp;

                    if (clampMaxWaitTime)
                        if(vt.timeVariationTrigger > maxWaitTimeClamp)
                            vt.timeVariationTrigger = maxWaitTimeClamp;

                    if (clampMinWaitTime)
                        if(vt.timeVariationTrigger < minWaitTimeClamp)
                            vt.timeVariationTrigger = minWaitTimeClamp;

                    vt.timeVariationTrigger *= scalar;
                    break;
                }
            }
        });

        clampMaxWaitTime = false;
        clampMaxAngleDeviation = false;
        clampMinWaitTime = false;
        clampMinAngleDeviation = false;


    }

    [Title("Multiply all tilemaps max&min timebeforejump by scalar")]
    [FoldoutGroup(CONFIG_NAME)]
    public float scalar = 1f;

    [FoldoutGroup(CONFIG_NAME)]
    [Button("Apply multiplication")]
    void ApplyScalar()
    {
        MapController.EditEachTilemapPrefab((ref GameObject editablePrefab) =>
        {
            for (int i = 0; i < editablePrefab.transform.childCount; i++)
            {
                VariablesTilemap vt = editablePrefab.transform.GetChild(i).GetComponent<VariablesTilemap>();

                if (vt != null)
                {
                    vt.timeVariationTrigger *= scalar;
                    break;
                }
            }

        });

    }


    [Title("Add a number to all tilemaps max&min timebeforejump")]
    [FoldoutGroup(CONFIG_NAME)]
    public float number = 1f;

    [FoldoutGroup(CONFIG_NAME)]
    [Button("Apply sum")]
    void ApplyOffset()
    {
        MapController.EditEachTilemapPrefab((ref GameObject editablePrefab) =>
        {
            for (int i = 0; i < editablePrefab.transform.childCount; i++)
            {
                VariablesTilemap vt = editablePrefab.transform.GetChild(i).GetComponent<VariablesTilemap>();

                if (vt != null)
                {
                    vt.timeVariationTrigger += number;
                    break;
                }
            }

        });

    }


}
