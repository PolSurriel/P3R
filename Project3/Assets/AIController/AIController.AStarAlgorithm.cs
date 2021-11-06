using DataStructures.PriorityQueue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    public class AStarSolver
    {
        Vector2 originPosition;
        float predictionPlayerRadius;
        JobyfablePrecalculatedPredictionSystem jumpPredictor;
        
        public AStarSolver(float predictionPlayerRadius, JobyfablePrecalculatedPredictionSystem jumpPredictor)
        {
            this.jumpPredictor = jumpPredictor;
            this.predictionPlayerRadius = predictionPlayerRadius;

        }



        float CalculateCost(Vector2 from, Vector2 to, bool checkCollision = true)
        {
            int layerMaskPrediction = 1 << LayerMask.NameToLayer("floor");
        
            float cost;

            Vector2 perp = Vector2.Perpendicular(from-to).normalized * predictionPlayerRadius;

            bool collides = false;

            if (checkCollision)
                collides = Physics2D.Linecast(from+perp, to+perp, layerMaskPrediction) || Physics2D.Linecast(from - perp, to - perp, layerMaskPrediction);
            //bool collides = Physics2D.Linecast(from, to, layerMaskPrediction);

            //Color c = collides ? new Color(1f, 0f, 0f, 0.1f) : new Color(0f, 1f, 0f, 0.1f);
            //Debug.DrawLine(from, to, c, 0.2f);



            if (collides)
            {
                cost = -1;
            }
            else
            {
                cost = (from - to).magnitude;

            }

            return cost;
        }


        public delegate void DelegationNeighbour(Vector2 node);
        void ForEachNewJumpNeighbour(Vector2 node, DelegationNeighbour method)
        {
            for (int i = 0; i < DIRECTIONS_COUNT; i++)
            {
                method(jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0) + node);
            }

        }


        delegate void ASDelegationNeighbour(AStarNode node);
        void ForeachNeighbour(AStarNode inNode, ASDelegationNeighbour method)
        {

            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                Vector2 origin = inNode.position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex);
                Vector2 nextPos = origin + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT);



                float cost = CalculateCost(inNode.position, nextPos);


                if(cost >= 0)
                {
                    AStarNode next = new AStarNode(nextPos, inNode.secondJumpDone, inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT, cost, inNode.time + PRECALCULATION_INCREMENT_DELTATIME);
                    method(next);
                }
            }

            if (!inNode.secondJumpDone)
            {

                int dirIndex = 0;
                ForEachNewJumpNeighbour(inNode.position, (nextNodePos) => {


                    float cost = CalculateCost(inNode.position, nextNodePos);
                    if (cost >= 0)
                    { 
                        AStarNode next = new AStarNode(nextNodePos, true, dirIndex++, 0, cost, inNode.time + PRECALCULATION_INCREMENT_DELTATIME);
                        method(next);
                    }
                });
            }


        }



        PriorityQueue<AStarNode, float> SetUpFrontierAstar(Vector2 goalPosition)
        {

            var frontier = new PriorityQueue<AStarNode, float>(0f);

            // First node
            for (int i = 2; i < DIRECTIONS_COUNT-1; i++)
            {

                Vector2 position = originPosition + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0);
                float cost = CalculateCost(originPosition, position, false);


                if(cost >= 0)
                {
                    var an = new AStarNode(position, false, i, 0, cost, PRECALCULATION_DELTATIME);
                    float priority = cost + an.H(goalPosition);
                    frontier.Insert(an, priority);
                }

            }

            return frontier;

        }


        Vector2 GetNextDeltaPos(AStarNode inNode)
        {
            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                Vector2 origin = inNode.position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex);
                Vector2 nextPos = origin + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT);

                return nextPos - inNode.position;

            }else
            {
                Vector2 origin = inNode.position - jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex);
                Vector2 lastPos = origin + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex - PRECALCULATED_POINTS_INCREMENT);

                return inNode.position - lastPos;
            }
        }

        bool EarlyExit(AStarNode current, AstarGoal goal)
        {

            float distanceToGoal = (goal.position - current.position).magnitude;
            if (distanceToGoal <= GOAL_MIN_DISTANCE)
            {
                if (!goal.useIncisionConstrain)
                    return true;

                Vector2 deltaPos = GetNextDeltaPos(current);
                float dot = Vector2.Dot(goal.incisionDirection.normalized, deltaPos);
                
                if(dot > Random.Range(0.2f, 0.4f))
                {
                    return true;
                }
            }

            return false;
        }

        public List<AStarNode> AStar(Vector2 startPosition, AstarGoal goal)
        {
            originPosition = startPosition;



            var frontier = SetUpFrontierAstar(goal.position);


            Dictionary<AStarNode, AStarNode> cameFrom = new Dictionary<AStarNode, AStarNode>();
            Dictionary<AStarNode, float> costSoFar = new Dictionary<AStarNode, float>();


            if (frontier.Empty())
                return null;

            AStarNode current = frontier.Top();


            // loop
            do
            {
                current = frontier.Top();
                frontier.Pop();

                if (EarlyExit(current, goal))
                {
                    break;
                }


                ForeachNeighbour(current, (AStarNode neighbor) => {


                    float currentCostSoFar = costSoFar.ContainsKey(current) ? costSoFar[current] : current.coste;
                    float newCost = currentCostSoFar + neighbor.coste;

                    // Si el coste current --> neighbor no está contemplado o es menor al ya contemplado
                    if (!costSoFar.ContainsKey(neighbor) || costSoFar[neighbor] > newCost)
                    {
                        // Sobreescribir coste cotemplado en current --> neighbor
                        costSoFar[neighbor] = newCost;
                        // Actualizar el came from de next para setearlo en current
                        cameFrom[neighbor] = current;

                        float priority = newCost + neighbor.H(goal.position);
                        // Insertar en frontera neighbor con su weight actualizado.
                        frontier.Insert(neighbor, priority);
                    }


                });


            } while (!frontier.Empty());



            List<AStarNode> result = new List<AStarNode>();
            if (frontier.Empty())
            {
                //PATH NOT FOUND
                return null;
            }

            // full backtrack
            while (cameFrom.ContainsKey(current))
            {
                if (cameFrom[current].secondJumpDone == current.secondJumpDone)
                    result.Insert(0, current);
                current = cameFrom[current];

            }


            return result;


        }

    }

}
