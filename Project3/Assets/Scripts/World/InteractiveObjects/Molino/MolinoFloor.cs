using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MolinoFloor : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var ai = collision.collider.GetComponent<AIController>();

        if(ai != null)
        {
            ai.EnterOnMolino();
        }

        var runner = collision.collider.GetComponent<Runner>();
        runner.SetParent(transform);


    }

}
