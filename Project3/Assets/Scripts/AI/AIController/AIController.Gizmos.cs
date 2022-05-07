using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIController : MonoBehaviour
{



    TextMesh debugTestMesh;
    Transform debugBG;

    private void OnDrawGizmos()
    {

#if UNITY_EDITOR
        if (GizmosCustomMenu.instance.aiStatus)
        {
            
            switch (state)
            {
                case AStarExecutionState.STOPPED:
                    debugTestMesh.fontSize = 20;
                    debugTestMesh.color = Color.yellow;
                    debugTestMesh.text = "STOPPED";
                    break;
                case AStarExecutionState.CHOOSING_TARGET:
                    debugTestMesh.fontSize = 20;
                    debugTestMesh.color = Color.white;
                    debugTestMesh.text = "CHOOSING\nTARGET";
                    break;
                case AStarExecutionState.EXECUTING_ASTAR:
                    Handles.Label(aStarGoal.position, $"Choosen target by {gameObject.name}");

                    debugTestMesh.fontSize = 50;
                    debugTestMesh.color = Color.red;
                    debugTestMesh.text = (timeBeforeJump - aStarSolver.timeSinceCalculationStarded).ToString(".0")+"s";
                    break;
                case AStarExecutionState.WAITING_TO_JUMP:
                    Handles.Label(aStarGoal.position, $"Choosen target by {gameObject.name}");

                    debugTestMesh.fontSize = 50;
                    debugTestMesh.color = Color.green;
                    debugTestMesh.text = (timeBeforeJump - aStarSolver.timeSinceCalculationStarded).ToString(".0") + "s";
                    break;
                case AStarExecutionState.JUMPING:
                    debugTestMesh.fontSize = 16;
                    debugTestMesh.color = Color.green;
                    debugTestMesh.text = "JUMPING";
                    break;
            }

            
            debugBG.localPosition = new Vector3(0.61f, 0.45f, -0.1f);

        }
        else
        {
            debugBG.position = new Vector3(9999f, 0.71f, 0f);

        }
#endif


        if (!GizmosCustomMenu.instance.aiController)
            return;

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
#if UNITY_EDITOR
                Handles.Label(node.position, "" + i);
#endif
                Debug.DrawLine(lastPos, node.position, Color.red);

                lastPos = node.position;


                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.black;

                try
                {
                    foreach (var obj in aStarSolver.movingObstaclesToHandle)
                    {
                        var futurePos = obj.GetFuturePosition(node.time - astarSeekTimeCounter);
#if UNITY_EDITOR
                        Handles.Label(futurePos, "" + i, style);
#endif
                    }

                    foreach (var obj in aStarSolver.rotatingObstaclesToHandle)
                    {
                        var futurePos = obj.GetFuturePosition(node.time-astarSeekTimeCounter);
#if UNITY_EDITOR
                        Handles.Label(futurePos, "" + i, style);
#endif
                    }
                }catch(MissingReferenceException e)
                {

                }

                i++;
            }

        }

        if (aStarGoal != null)
        {
            Gizmos.DrawWireSphere(aStarGoal.position, goalMinDist);

        }


#if UNITY_EDITOR
        if (aStarSolver.output != null)
            foreach (var node in aStarSolver.output)
            {
                foreach (var callback in node.ifChoosenDoOnGizmos)
                {
                    callback();
                }
            }

        if (currentNode != null)
            foreach (var callback in currentNode.ifChoosenAndCurrentDoOnGizmos)
            {
                callback();
            }
#endif
    }
}
