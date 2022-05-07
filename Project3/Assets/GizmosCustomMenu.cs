using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GizmosCustomMenu : MonoBehaviour
{

    [Title("Select the information you want to be drawn on gizmos.")]

    public bool pathTargets;
    public bool movingObstaclesFuture;
    public bool aiController;
    public bool portalsRuntimeInfo;
    public bool portalsConfiguration;

  
    public static GizmosCustomMenu m_instance;
    public static GizmosCustomMenu instance {
        get {
            if (m_instance == null)
            {
                var queryResult = FindObjectsOfType<GizmosCustomMenu>();
                if(queryResult.Length > 1)
                {
                    Debug.LogError("Only can be one GizmosCustomMenu component in the scene.");

                }else
                {
                    m_instance = queryResult[0];
                }
            }

            return m_instance;        
        }
    }

   

}
