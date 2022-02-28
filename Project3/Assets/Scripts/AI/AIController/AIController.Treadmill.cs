using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 
Cuando entramos en un treadmill, se decide un punto (posición)
del futuro a la que el treadmill nos terminará llevando.

Una vez la ia llega a ese punto, saltará.

 */

public partial class AIController : MonoBehaviour
{
    [HideInInspector]
    public bool onATreadmill = false;
    Vector2 treadMillTarget;

    void TreadmilleUpdate()
    {
        var distToDestination = Vector2.Distance(treadMillTarget, transform.position);
        if(distToDestination < 0.5f)
        {
            onATreadmill = false;
            executingAstarSeek = false;
            pendingToStartAStarPipeline = false;
            StartAStarPipeline();
        }
    }


    /*
     * Decidimos el punto (posición) que triggea la salida del treadmill
     * dentro de una lista de posibles salidas que nos brinda el propio 
     * treadmill (entidad que llama a este metodo)
     */
    public void EnterOnATreadMille(ref List<Transform> exits, Vector2 direction)
    {
        // Actualizamos el estado
        onATreadmill = true;

        // Evaluamos las posibles salidas
        // (si caemos en mitad del treadmill y éste sube, todos los exits que esten por debajo no sirven)
        List<Vector2> candidates = new List<Vector2>(10);

        direction.Normalize();

        foreach (var exit in exits)
        {
            Vector2 posToExit = exit.position - transform.position;
            posToExit.Normalize();

            if (Vector2.Dot(posToExit, direction.normalized) > 0.9f)
            {
                candidates.Add(exit.position);
            }
        }

        try
        {
            treadMillTarget = candidates[UnityEngine.Random.Range(0, candidates.Count)];

        }catch(ArgumentOutOfRangeException e)
        {

        }
        
        
    }

}
