using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    bool onATreadmill = false;
    Vector2 treadMillTarget;

    void TreadmilleUpdate()
    {
        var dist = Vector2.Distance(treadMillTarget, transform.position);


        if(dist < 0.5f)
        {
            

            onATreadmill = false;
            executingAstarSeek = false;
            pendingToStart = false;
            StartAStarPipeline();
        }
    }


    public void EnterOnATreadMille(ref List<Transform> exits, Vector2 direction)
    {

        onATreadmill = true;

        List<Vector2> candidates = new List<Vector2>(10);

        direction.Normalize();

        // if dot pos -> exit, dir is ok, then choose

        //treadMillTarget = exits[0].transform.position;
        //return;

        foreach (var exit in exits)
        {
            Vector2 posToExit = exit.position - transform.position;
            posToExit.Normalize();

            if (Vector2.Dot(posToExit, direction.normalized) > 0.9f)
            {
                candidates.Add(exit.position);
            }
        }

        treadMillTarget = candidates[Random.Range(0, candidates.Count)];
        
        
    }

}
