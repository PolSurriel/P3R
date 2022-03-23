using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugGame : MonoBehaviour
{
    public bool useAI;
    public GameObject playerPrefab;
    void Start()
    {
        if(FindObjectOfType<Runner>() == null)
        {
            var p = Instantiate(playerPrefab);
            if (useAI)
            {
                p.AddComponent<AIController>();
                Destroy(p.GetComponent<PlayerController>());

            }
            FindObjectOfType<MapController>().playerTransform = p.transform;
            FindObjectOfType<CameraController>().playerTransform = p.transform;
        }      
    }
#if UNITY_EDITOR

#endif

}
