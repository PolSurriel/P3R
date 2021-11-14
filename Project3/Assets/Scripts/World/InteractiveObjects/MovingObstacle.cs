using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{

    public Transform obstacle;
    public List<Transform> points = new List<Transform>();
    public float speed = 6f;
    int currentTargetIndex = 1;

    public Transform avoidCollider;


    public void UpdateAvoidAstarInfo(float time)
    {
        avoidCollider.position = GetFuturePosition(time);
    }

    private void FixedUpdate()
    {

        Vector2 dir = points[currentTargetIndex].position - obstacle.position;
        float dist = dir.magnitude;
        dir.Normalize();

        Vector2 deltaMove = (Vector3)dir * speed * Time.fixedDeltaTime;

        if(deltaMove.magnitude > dist || dir == Vector2.zero)
        {

            currentTargetIndex = ++currentTargetIndex % points.Count;

        }
            
        

        obstacle.Translate(deltaMove, Space.World);

    }

    public float simT;

    public Vector2 GetFuturePosition(float time)
    {

        float totalDeltaMove = time * speed;
        Vector2 obstaclePos = obstacle.position;
        int virtualTarget = currentTargetIndex;

        Vector2 dir = (Vector2)points[virtualTarget].position - obstaclePos;
        float dist = dir.magnitude;

        //si nos pasamos
        while (totalDeltaMove > dist)
        {
            // partimos de la siguiente posicion
            obstaclePos = (Vector2)points[virtualTarget].position;
            // cambiamos nuestro target al siguiente
            virtualTarget = ++virtualTarget % points.Count;

            // nos hemos movido dist
            totalDeltaMove -= dist;

            dir = (Vector2)points[virtualTarget].position - obstaclePos;
            dist = dir.magnitude;

        }

        dir.Normalize();
        Vector2 deltaMove = (Vector3)dir * totalDeltaMove;

      


        Vector2 result = obstaclePos + deltaMove;

        return result;
    }


    private void Update()
    {
        Debug.DrawLine(transform.position, GetFuturePosition(simT));
    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawWireSphere(obstacle.position, 1.3f);
        Transform lastPoint = null;
        Transform firstPoint = null;
        int i = 0;
        foreach(var point in points)
        {

            //Handles.Label(point.position, "     Point "+(i++));
            Gizmos.DrawWireSphere(point.position, 0.3f);

            if(lastPoint != null)
            {
                Debug.DrawLine(lastPoint.position, point.position);

                if(i == points.Count)
                {
                    Debug.DrawLine(firstPoint.position, point.position);
                }
            }else
            {
                firstPoint = point;
            }

            lastPoint = point;
        }
    }


}
