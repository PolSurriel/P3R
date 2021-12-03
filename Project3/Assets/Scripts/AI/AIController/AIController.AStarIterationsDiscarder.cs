using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using SurrealBoost.Types;


/*
 
Las iteraciones de AStar son extremadamente caras.
Principalmente debido a la cantidad de raycast que se hacen.

Por ello, descartar iteraciones que no llegan al punto final
mejora el rendimiento de la búsqueda de caminos en más de 10x.

La clave de ésta optimización está en descartar las iteraciones
SIN raycast y de la manera más eficiente posible.
 
 */
public partial class AIController : MonoBehaviour
{
    public struct ReboundWallInfo
    {
        public Line collisionInfo;
        public bool inverseX;
        public bool inverseY;
    }

    public const int ITERATION_DISCARDER_BATCH = 3;
    public struct AStarIterationsDiscarder : IJobParallelFor
    {
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeArray<Vector2> m_precalculatedDirections;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeArray<ReboundWallInfo> m_reboundWalls;
        [ReadOnly]
        public static int m_reboundWallsLenght;




        public static int a;

        public Vector2 nodePosition;
        public Vector2 m_portalSense;

        public int m_iterationsCount;
        public Vector2 m_goalPosition;
        public Vector2 m_portalPosition;
        public bool m_usePortal;

        public NativeArray<bool> m_result;


        void ReboundWallCase(ref Vector2 portalSense, ref Vector2 from, ref Vector2 to, ref Vector2 origin)
        {

            for (int i = 0; i < m_reboundWallsLenght; i++)
            {
                var wall = m_reboundWalls[i];
                var cast = SurrealBoost.Utils.Intersection.lineLine(wall.collisionInfo, new Line() { pointA = from, pointB = to });

                if (cast.result)
                {
                    if (wall.inverseX) portalSense.x *= -1;
                    if (wall.inverseY) portalSense.y *= -1;

                    var ipToOrigin = origin - cast.intersectionPoint;
                    origin = cast.intersectionPoint + ipToOrigin * portalSense;
                    return;

                }
            }
        }

        public void Execute(int directionIndex)
        {

            const int INCREMENT = PRECALCULATED_POINTS_INCREMENT;

            m_result[directionIndex] = false;


            Vector2 portalSense = m_portalSense;

            Vector2 origin = nodePosition;
            Vector2 lastPosition = nodePosition;

            // por cada punto del salto
            for (int pathIndex = 0; pathIndex < NUMBER_OF_PRECALCULATED_POINTS; pathIndex += INCREMENT)
            {
                // calculo su posicion
                var nextPosition = origin + portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + pathIndex];

                float distToGoal = Vector2.Distance(nextPosition, m_goalPosition);
                ReboundWallCase(ref portalSense, ref lastPosition, ref nextPosition, ref origin);

                // Si pasa cerca del goal o un portal, lo valido.
                if (distToGoal <= GOAL_MIN_DISTANCE || (m_usePortal && Vector2.Distance(nextPosition, m_portalPosition) <= GOAL_MIN_DISTANCE))
                {
                    m_result[directionIndex] = true;
                    return;
                }

                lastPosition = nextPosition;

            }

        }
    }
}
