using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform playerTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        const float speedFactor = 3f;

        var yDif = (playerTransform.position.y + 2.5f) - transform.position.y;
        Vector2 deltaMove = Vector2.up * yDif;


        transform.Translate(deltaMove * Time.deltaTime * speedFactor, Space.World);

    }
}
