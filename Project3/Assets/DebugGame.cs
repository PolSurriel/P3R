﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGame : MonoBehaviour
{

    public GameObject playerPrefab;
#if UNITY_EDITOR
    void Start()
    {
        if(FindObjectOfType<Runner>() == null)
        {
            var p = Instantiate(playerPrefab);
            FindObjectOfType<MapController>().playerTransform = p.transform;
            FindObjectOfType<CameraController>().playerTransform = p.transform;
        }      
    }

#endif

}
