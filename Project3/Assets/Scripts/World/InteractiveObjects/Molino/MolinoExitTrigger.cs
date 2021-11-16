using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolinoExitTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var ai = collision.GetComponent<AIController>();

        if(ai != null)
        {
            ai.ExitMolino();
        }
    }

}
