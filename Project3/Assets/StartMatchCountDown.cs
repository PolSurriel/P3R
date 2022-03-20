﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StartMatchCountDown : MonoBehaviour
{


    TextMeshPro text;
    
    float timeCounter = 0f;
    float duration = 5;


    private void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    public static bool matchStarted = false;

    // Update is called once per frame
    void Update()
    {
        timeCounter += Time.deltaTime;

        if(timeCounter <= (duration - 0.5f))
        {
            text.text = (duration - timeCounter).ToString("0");
        }else
        {
            text.fontSize = 25f;
            text.text = "GO!";
            matchStarted = true;

        }
    }
}
