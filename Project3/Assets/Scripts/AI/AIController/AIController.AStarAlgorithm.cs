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
        int layerMaskAvoidImaginary;

        public AStarSolver(float predictionPlayerRadius, JobyfablePrecalculatedPredictionSystem jumpPredictor)
        {
            layerMaskPrediction = (1 << LayerMask.NameToLayer("floor")) | 
                                  (1 << LayerMask.NameToLayer("Obstacle"));

            layerMaskAvoidImaginary = (1 << LayerMask.NameToLayer("ImaginaryAvoid"));

            this.jumpPredictor = jumpPredictor;
            this.predictionPlayerRadius = predictionPlayerRadius;

        }


        void PortalCase(ref AStarNode nextNode,  Vector2 prevPos)
        {

            int layerMaskPortal = 1 << LayerMask.NameToLayer("Portal");

            RaycastHit2D portalHit = Physics2D.Linecast(prevPos, nextNode.position, layerMaskPortal);

            if (portalHit)
            {
                var portal = portalHit.collider.GetComponent<Portal>();



                if (portal.inverseY)
                {
                    nextNode.portalSense.y *= -1;

                }

                if (portal.inverseX)
                {
                    nextNode.portalSense.x *= -1;
                }

                var prevNext = nextNode.position;

                // Change nextPos
                Vector2 deltaMove = nextNode.position - prevPos;
                deltaMove *= nextNode.portalSense;
                nextNode.position = prevPos + deltaMove;


                // Calculate offset
                Vector2 portalPos = portal.transform.position;
                Vector2 otherPortalpos = portal.otherPortal.transform.position;
                nextNode.portalOffset += (otherPortalpos - portalPos);

                
            }

            
        }

        public List<MovingObstacle> movingObstaclesToHandle = new List<MovingObstacle>(0);
        public List<RotatingObstacle> rotatingObstaclesToHandle = new List<RotatingObstacle>(0);


 

        bool CollidesWithDynamicObstacle(ref Vector2 nextPos, ref Vector2 prevPos, ref float time)
        {
            foreach (var obstacle in movingObstaclesToHandle)
            {
                obstacle.UpdateAvoidAstarInfo(time);
            }

            

            foreach (var obstacle in rotatingObstaclesToHandle)
            {
                obstacle.UpdateAvoidAstarInfo(time);

            }

            bool collides = Physics2D.Linecast(prevPos, nextPos, layerMaskAvoidImaginary);


            return collides;
        }

        int layerMaskPrediction;
        float CalculateCost(Vector2 from, ref Vector2 to, ref float time, bool checkCollision = true)
        {
            
        
            float cost;

            Vector2 perp = Vector2.Perpendicular(from-to).normalized * predictionPlayerRadius;

            bool collides = false;

            if (checkCollision)
            {
                collides = Physics2D.Linecast(from+perp, to+perp, layerMaskPrediction) || Physics2D.Linecast(from - perp, to - perp, layerMaskPrediction) || CollidesWithDynamicObstacle(ref to, ref from, ref time);

                

            }
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
        void ForEachNewJumpNeighbour(ref AStarNode node, DelegationNeighbour method)
        {
            for (int i = 0; i < DIRECTIONS_COUNT; i++)
            {
                method(node.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0) + node.position);
            }

        }


        delegate void ASDelegationNeighbour(AStarNode node);
        void ForeachNeighbour(ref AStarNode inNode, ASDelegationNeighbour method)
        {

            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                Vector2 origin = inNode.position - inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex);
                Vector2 nextPos = origin + inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT);


                

                float cost = CalculateCost(inNode.position, ref nextPos, ref inNode.time);


                if(cost >= 0)
                {
                    AStarNode next = new AStarNode(nextPos, inNode.secondJumpDone, inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT, cost, inNode.time + PRECALCULATION_INCREMENT_DELTATIME);
                    PortalCase(ref next, inNode.position);

                    method(next);
                }
            }

            if (!inNode.secondJumpDone)
            {

                int dirIndex = 0;

                for (int i = 0; i < DIRECTIONS_COUNT; i++)
                {
                    var nextNodePos = inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0) + inNode.position;
                    float cost = CalculateCost(inNode.position, ref nextNodePos, ref inNode.time);
                    if (cost >= 0)
                    {
                        AStarNode next = new AStarNode(nextNodePos, true, dirIndex++, 0, cost, inNode.time + PRECALCULATION_INCREMENT_DELTATIME);
                        PortalCase(ref next, inNode.position);

                        method(next);
                    }


                }

                //ForEachNewJumpNeighbour(inNode, (nextNodePos) => {


                //    float cost = CalculateCost(inNode.position, nextNodePos, inNode.time);
                //    if (cost >= 0)
                //    { 
                //        AStarNode next = new AStarNode(nextNodePos, true, dirIndex++, 0, cost, inNode.time + PRECALCULATION_INCREMENT_DELTATIME);
                //        PortalCase(ref next, inNode.position);

                //        method(next);
                //    }
                //});
            }


        }


        


        PriorityQueue<AStarNode, float> SetUpFrontierAstar(Vector2 goalPosition, float timeToStart)
        {

            var frontier = new PriorityQueue<AStarNode, float>(0f);

            // First node
            for (int i = 2; i < DIRECTIONS_COUNT-1; i++)
            {

                Vector2 position = originPosition + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0);

                var time = PRECALCULATION_DELTATIME + timeToStart;
                float cost = CalculateCost(originPosition, ref position, ref time, false);


                if(cost >= 0)
                {
                    var an = new AStarNode(position, false, i, 0, cost, PRECALCULATION_DELTATIME);
                    float priority = cost + an.H(goalPosition);
                    frontier.Insert(an, priority);
                }

            }

            return frontier;

        }


        Vector2 GetNextDeltaPos(ref AStarNode inNode)
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

        bool EarlyExit(ref AStarNode current, ref AstarGoal goal)
        {

            float distanceToGoal = (goal.position - current.position).magnitude;
            if (distanceToGoal <= GOAL_MIN_DISTANCE)
            {
                if (!goal.useIncisionConstrain)
                    return true;

                Vector2 deltaPos = GetNextDeltaPos(ref current);
                float dot = Vector2.Dot(goal.incisionDirection.normalized, deltaPos);
                
                if(dot > Random.Range(0.2f, 0.4f))
                {
                    return true;
                }
            }

            return false;
        }

        public List<AStarNode> AStar(Vector2 startPosition, AstarGoal goal, float timeToStart)
        {
            originPosition = startPosition;



            var frontier = SetUpFrontierAstar(goal.position, timeToStart);


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

                if (EarlyExit(ref current, ref goal))
                {
                    break;
                }


                ForeachNeighbour(ref current, (AStarNode neighbor) => {


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
