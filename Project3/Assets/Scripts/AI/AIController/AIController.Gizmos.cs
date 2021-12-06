using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    private void OnDrawGizmos()
    {


        //var color = Color.black;
        //color.a = 0.4f;
        //Handles.color = color;
        //Handles.DrawWireDisc(transform.position, transform.forward, VALID_TARGET_AREA_RADIUS);

        
        if (aStarSolver == null)
            return;


        if (aStarSolver.output != null && aStarGoal != null && aStarSolver.output.Count != 0)
        {

            Vector2 lastPos = aStarSolver.output[0].position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(aStarSolver.output[0].directionIndex, 0);
            int i = 0;
            foreach (var node in aStarSolver.output)
            {
                Handles.Label(node.position, "" + i);
                Debug.DrawLine(lastPos, node.position);

                lastPos = node.position;


                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;

                try
                {
                    foreach (var obj in aStarSolver.movingObstaclesToHandle)
                    {
                        var futurePos = obj.GetFuturePosition(node.time - astarSeekTimeCounter);
                        Handles.Label(futurePos, "" + i, style);
                    }

                    foreach (var obj in aStarSolver.rotatingObstaclesToHandle)
                    {
                        var futurePos = obj.GetFuturePosition(node.time-astarSeekTimeCounter);
                        Handles.Label(futurePos, "" + i, style);
                    }
                }catch(MissingReferenceException e)
                {

                }

                i++;
            }

        }

        if (aStarGoal != null)
        {
            Gizmos.DrawWireSphere(aStarGoal.position, GOAL_MIN_DISTANCE);

        }
    }
}
