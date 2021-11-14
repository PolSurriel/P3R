using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{

    Runner[] players;
    float speed = 0.3f;

    // Start is called before the first frame update


    void Start()
    {
        players = FindObjectsOfType<Runner>();
    }
    private void FixedUpdate()
    {
        players = FindObjectsOfType<Runner>();

        float minDist = 99999f;

        foreach(var player in players)
        {

            float dist = Mathf.Abs(transform.position.y - player.transform.position.y);

            if (dist < minDist)
            {
                minDist = dist;
            }

        }

        if (minDist > 20f)
        {
            float newSpeed = minDist * 2f;
            transform.position = transform.position + Vector3.up * newSpeed * Time.fixedDeltaTime;

        }else
        {
            transform.position = transform.position + Vector3.up * speed * Time.fixedDeltaTime;
        }

        
        
    }

}
