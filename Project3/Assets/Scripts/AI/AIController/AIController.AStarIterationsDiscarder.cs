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
        public bool swapXY;
        public Vector2 otherPortalNormal;
        public Vector2 otherPortalPosition;
        public Vector2 portalPosition;
    }

    public const int ITERATION_DISCARDER_BATCH = 3;
    public struct AStarIterationsDiscarder : IJobParallelFor
    {
        public static float DIST_REACH_TARGET = 0.9f;

        public static float maxX;
        public static float minX;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeArray<Vector2> m_precalculatedDirections;

        [NativeDisableParallelForRestriction]
        public static int lastAddedJumpPredictorIndex = 0;


        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeFIFO<NativeReboundWallInfo> m_reboundWalls;

        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeFIFO<NativePortalInfo> m_portals;


        public static int a;

        public int m_jumpPredictorIndex;
        public Vector2 nodePosition;
        public Vector2 m_portalSense;
        public float m_characterRadius;

        public int m_iterationsCount;
        public Vector2 m_goalPosition;

        public NativeArray<bool> m_result;


     

        void PortalCase(ref Vector2 portalSense, ref Vector2 lastNodePos, ref Vector2 nextNodePos, ref Vector2 origin, ref int directionIndex, ref int positionIndex, ref bool enterInPortalSwap)
        {
            var movDirection =  (nextNodePos - lastNodePos).normalized; 

            for (int i = 0; i < m_portals.Length; i++)
            {
                var portal = m_portals[i];

                if(Vector2.Distance(nextNodePos, portal.collisionInfo.pointB) > VALID_TARGET_AREA_RADIUS)
                {
                    continue;
                }

                //Si vamos muy paralelos al portal, mejor evitar entrar.
                if(Vector2.Dot(movDirection, -portal.otherPortalNormal) < 0.25f)
                {
                    continue;
                }

                var cast = SurrealBoost.Utils.Intersection2D.lineLine(portal.collisionInfo, new Line() { pointA = lastNodePos, pointB = nextNodePos });

                if (cast.result)
                {
                    
                    

                    // Change nextPos
                    Vector2 deltaMove = nextNodePos - lastNodePos;


                    
                    if (portal.swapXY)
                    {
                        //tmp
                        enterInPortalSwap = true;
                        return;


                        // 1) we get the swapped+inversed new velocity
                        deltaMove *= new Vector2(portal.inverseX ? -1f : 1f, portal.inverseY ? -1f : 1f);
                        var newVel = Portal.SmartSwap(true, portal.otherPortalNormal, deltaMove.normalized);

                        // 2) we get the new local position relative to other portal (local swapped!)
                        var relativePortal = lastNodePos - (Vector2)portal.portalPosition;
                        var relativeOtherPortal = Portal.SmartSwap(true, portal.otherPortalNormal, relativePortal);

                        // 3) we get the next node position
                        nextNodePos = (Vector2)portal.otherPortalPosition + relativeOtherPortal;

                        // 4) using new velocity we get the new simulation index
                        //directionIndex = m_jumpPredictors[m_jumpPredictorIndex].GetSimulationIndex(newVel);

                        // then we clean portal sense
                        //portalSense = Vector2.one;

                        // 5) we get the new origin
                        //origin = nextNodePos - (m_jumpPredictors[m_jumpPredictorIndex].precalculatedDirections[directionIndex * m_jumpPredictors[m_jumpPredictorIndex].iterationsCount + positionIndex]);

                    }
                    else
                    {

                        if (portal.inverseY)
                            portalSense.y *= -1;

                        if (portal.inverseX)
                            portalSense.x *= -1;

                        deltaMove *= portalSense;
                        nextNodePos = lastNodePos + (deltaMove) + portal.otherPortalNormal * m_characterRadius;

                        origin = nextNodePos - (portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + positionIndex]);


                    }


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
                var nextPosition = origin + (portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + pathIndex]);
                bool enterInPortalSwap = false;
                PortalCase(ref portalSense, ref lastPosition, ref nextPosition, ref origin, ref directionIndex, ref pathIndex, ref enterInPortalSwap);

                float distToGoal = Vector2.Distance(nextPosition, m_goalPosition);
                //ReboundWallCase(ref portalSense, ref lastPosition, ref nextPosition, ref origin);

                // Si pasa cerca del goal o un portal, lo valido.
                if (distToGoal <= DIST_REACH_TARGET || enterInPortalSwap) //(m_usePortal && Vector2.Distance(nextPosition, m_portalPosition) <= GOAL_MIN_DISTANCE))
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
