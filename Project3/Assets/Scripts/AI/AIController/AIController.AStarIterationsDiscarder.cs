using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using SurrealBoost.Types;
using Unity.Burst;


/*
 
Las iteraciones de AStar son extremadamente caras.
Principalmente debido a la cantidad de raycast que se hacen.

Por ello, descartar iteraciones que no llegan al punto final
mejora el rendimiento de la búsqueda de caminos en más de 10x.

La clave de ésta optimización está en descartar las iteraciones
SIN raycast y de la manera más eficiente posible.
 
 */

[BurstCompile]
public partial class AIController : MonoBehaviour
{
    public struct NativeReboundWallInfo
    {
        public Line collisionInfo;
        public bool inverseX;
        public bool inverseY;
    }


    public struct NativePortalInfo
    {
        public Line collisionInfo;
        public bool inverseX;
        public bool inverseY;
        public Vector2 otherPortalPosition;
        public Vector2 portalPosition;
    }

    public const int ITERATION_DISCARDER_BATCH = 3;
    public struct AStarIterationsDiscarder : IJobParallelFor
    {
        public static float maxX;
        public static float minX;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeArray<Vector2> m_precalculatedDirections;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeFIFO<NativeReboundWallInfo> m_reboundWalls;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeFIFO<NativePortalInfo> m_portals;





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

            for (int i = 0; i < m_reboundWalls.Length; i++)
            {
                var wall = m_reboundWalls[i];
                var cast = SurrealBoost.Utils.Intersection2D.lineLine(wall.collisionInfo, new Line() { pointA = from, pointB = to });

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

        void PortalCase(ref Vector2 portalSense, ref Vector2 lastNodePos, ref Vector2 nextNodePos, ref Vector2 origin, ref Vector2 portalOffset, ref int directionIndex, ref int positionIndex)
        {
            for (int i = 0; i < m_portals.Length; i++)
            {
                var portal = m_portals[i];
                var cast = SurrealBoost.Utils.Intersection2D.lineLine(portal.collisionInfo, new Line() { pointA = lastNodePos, pointB = nextNodePos });

                if (cast.result)
                {
                    if (portal.inverseY)
                        portalSense.y *= -1;

                     if (portal.inverseX)
                        portalSense.x *= -1;

                    // Change nextPos
                    Vector2 deltaMove = nextNodePos - lastNodePos;
                    deltaMove *= portalSense;
                    nextNodePos = lastNodePos + deltaMove;


                    // Calculate offset
                    Vector2 portalPos = portal.portalPosition;
                    Vector2 otherPortalpos = portal.otherPortalPosition;
                    portalOffset += (otherPortalpos - portalPos);

                    origin = nextNodePos - portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + positionIndex];


                }
            }

        }

        public void Execute(int directionIndex)
        {

            const int INCREMENT = PRECALCULATED_POINTS_INCREMENT;

            m_result[directionIndex] = false;


            Vector2 portalSense = m_portalSense;
            Vector2 portalOffset = Vector2.zero;

            Vector2 origin = nodePosition;
            Vector2 lastPosition = nodePosition;

            // por cada punto del salto
            for (int pathIndex = 0; pathIndex < NUMBER_OF_PRECALCULATED_POINTS; pathIndex += INCREMENT)
            {
                // calculo su posicion
                var nextPosition = portalOffset + origin + portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + pathIndex];
                PortalCase(ref portalSense, ref lastPosition, ref nextPosition, ref origin, ref portalOffset, ref directionIndex, ref pathIndex);

                float distToGoal = Vector2.Distance(nextPosition, m_goalPosition);
                //ReboundWallCase(ref portalSense, ref lastPosition, ref nextPosition, ref origin);

                // Si pasa cerca del goal o un portal, lo valido.
                if (distToGoal <= GOAL_MIN_DISTANCE) //(m_usePortal && Vector2.Distance(nextPosition, m_portalPosition) <= GOAL_MIN_DISTANCE))
                {
                    m_result[directionIndex] = true;
                    return;
                }

                // Descartamos la iteracion si se sale de los bounds del mapa
                else if (nextPosition.x > maxX || nextPosition.x < minX)
                {
                    return;
                }

                lastPosition = nextPosition;

            }

        }
    }
}
