using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stain : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var runner = collision.GetComponent<Runner>();

        if (runner == null)
            return;

        runner.EnterOnStain();
        var ai = collision.GetComponent<AIController>();
    
        if(ai != null)
        {
            ai.EnterOnStain();
        }
    
    }


}
