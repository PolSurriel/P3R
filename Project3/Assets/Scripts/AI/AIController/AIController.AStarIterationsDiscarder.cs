using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public partial class AIController : MonoBehaviour
{

    public const int ITERATION_DISCARDER_BATCH = 3;
    public struct AStarIterationsDiscarder : IJobParallelFor
    {

        public static int a;

        public Vector2 nodePosition;
        public Vector2 portalSense;
        [ReadOnly]
        [NativeDisableParallelForRestriction]
        public static NativeArray<Vector2> m_precalculatedDirections;

        public int m_iterationsCount;
        public Vector2 m_goalPosition;
        public Vector2 m_portalPosition;
        public bool m_usePortal;

        public NativeArray<bool> m_result;


        public void Execute(int directionIndex)
        {

            const int INCREMENT = PRECALCULATED_POINTS_INCREMENT;

            m_result[directionIndex] = false;

            // por cada punto del salto
            for (int pathIndex = 0; pathIndex < NUMBER_OF_PRECALCULATED_POINTS; pathIndex += INCREMENT)
            {
                // calculo su posicion
                var nextPosition = nodePosition + portalSense * m_precalculatedDirections[directionIndex * m_iterationsCount + pathIndex];

                float distToGoal = Vector2.Distance(nextPosition, m_goalPosition);

                // Si pasa cerca del goal o un portal, lo valido.
                if (distToGoal <= GOAL_MIN_DISTANCE || (m_usePortal && Vector2.Distance(nextPosition, m_portalPosition) <= GOAL_MIN_DISTANCE))
                {
                    m_result[directionIndex] = true;
                    return;
                }

            }

        }
    }
}
