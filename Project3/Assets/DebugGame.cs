using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGame : MonoBehaviour
{

    public GameObject playerPrefab;
    void Start()
    {
        if(FindObjectOfType<Runner>() == null)
        {
            var p = Instantiate(playerPrefab);
            FindObjectOfType<MapController>().playerTransform = p.transform;
            FindObjectOfType<CameraController>().playerTransform = p.transform;
        }      
    }
#if UNITY_EDITOR

#endif

}
