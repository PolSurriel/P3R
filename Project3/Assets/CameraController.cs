using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;

    float highestY;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (highestY < playerTransform.position.y)
            highestY = playerTransform.position.y;

        transform.position = new Vector3(
            transform.position.x,
            Math.Max(playerTransform.position.y, highestY),
            transform.position.z
        );
    }
}
