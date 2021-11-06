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
        Handles.color = color;
        Handles.DrawWireDisc(transform.position, transform.forward, VALID_TARGET_AREA_RADIUS);




        if (currentPath != null && aStarGoal != null && currentPath.Count != 0)
        {

            int i = 0;
            Vector2 lastPos = currentPath[0].position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(currentPath[0].directionIndex, 0);
            foreach (var node in currentPath)
            {


                Handles.Label(node.position, "" + i++);

                Debug.DrawLine(lastPos, node.position);

                lastPos = node.position;

            }

        }

        if (aStarGoal != null)
        {
            Handles.color = Color.red;
            Handles.DrawWireDisc(aStarGoal.position, transform.forward, GOAL_MIN_DISTANCE);
        }
    }
}
