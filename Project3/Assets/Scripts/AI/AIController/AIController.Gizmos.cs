using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    private void OnDrawGizmos()
    {


        var color = Color.yellow;
        color.a = 0.4f;
        //Handles.color = color;
        //Handles.DrawWireDisc(transform.position, transform.forward, VALID_TARGET_AREA_RADIUS);


        if (aStarSolver == null)
            return;


        if (aStarSolver.output != null && aStarGoal != null && aStarSolver.output.Count != 0)
        {

            Vector2 lastPos = aStarSolver.output[0].position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(aStarSolver.output[0].directionIndex, 0);
            foreach (var node in aStarSolver.output)
            {


                //Handles.Label(node.position, "" + i++);
                Debug.DrawLine(lastPos, node.position);

                lastPos = node.position;

            }

        }

        if (aStarGoal != null)
        {
            Gizmos.DrawWireSphere(aStarGoal.position, GOAL_MIN_DISTANCE);

        }
    }
}
