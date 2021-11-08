using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleZone : MonoBehaviour
{

    public List<MovingObstacle> movingObstacles = new List<MovingObstacle>();
    public List<RotatingObstacle> rotatingObstacles = new List<RotatingObstacle>();


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var ai = collision.GetComponent<AIController>();

        if (ai != null)
        {
            ai.aStarSolver.movingObstaclesToHandle = movingObstacles;
            ai.aStarSolver.rotatingObstaclesToHandle = rotatingObstacles;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var ai = collision.GetComponent<AIController>();

        if (ai != null)
        {
            ai.aStarSolver.movingObstaclesToHandle = new List<MovingObstacle>(0);
            ai.aStarSolver.rotatingObstaclesToHandle = new List<RotatingObstacle>(0);
        }

    }

}


