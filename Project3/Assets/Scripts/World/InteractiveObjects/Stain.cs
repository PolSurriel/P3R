using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stain : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<Runner>().EnterOnStain();
        var ai = collision.GetComponent<AIController>();
    
        if(ai != null)
        {
            ai.EnterOnStain();
        }
    
    }


}
