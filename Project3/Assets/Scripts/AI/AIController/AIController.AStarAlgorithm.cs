using DataStructures.PriorityQueue;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 En este archivo se encuentran todas las funciones
 que se ocupan de ejecutar el algoritmo de búsqueda de caminos.

 Recuerda que algunas de las constantes usadas se definen en el 
 archivo principal de AIController.

 Todas las funcionalidades de este archivo están encapsuladas en
 la classe AStar Solver.
 
 */

public partial class AIController : MonoBehaviour
{
    float predictionPlayerRadius;

    public class AStarSolver
    {
        public MovingObstacle[] movingObstaclesToHandle;
        public RotatingObstacle[] rotatingObstaclesToHandle;
        public Vector2 portalPosition;
        public bool usePortal;

        Vector2 originPosition;
        float predictionPlayerRadius;
        JobyfablePrecalculatedPredictionSystem jumpPredictor;
        int layerMaskAvoidImaginary;
        int layerMaskRaycastNOT;
        int layerMaskPortal;
        int layerMaskPrediction;
        Vector2 goalPosition;


        public AStarSolver(float predictionPlayerRadius, JobyfablePrecalculatedPredictionSystem jumpPredictor)
        {

            layerMaskPortal = 1 << LayerMask.NameToLayer("Portal");
            layerMaskPrediction = (1 << LayerMask.NameToLayer("floor")) | 
                                  (1 << LayerMask.NameToLayer("Obstacle"));
            layerMaskAvoidImaginary = (1 << LayerMask.NameToLayer("ImaginaryAvoid"));
            layerMaskRaycastNOT = (1 << LayerMask.NameToLayer("RaycastNOT"));

            this.jumpPredictor = jumpPredictor;
            this.predictionPlayerRadius = predictionPlayerRadius;

        }


        /* Este metodo modifica la información de un nodo si este cruza un portal.
           Modifica el sentido de su x/y si es necesario y añade un offset a su
           posicion.
         */
        void PortalCase(ref AStarNode nextNode,  Vector2 prevPos)
        {

            if (!usePortal)
                return;

            RaycastHit2D portalHit = Physics2D.Linecast(prevPos, nextNode.position, layerMaskPortal);

            if (portalHit)
            {
                var portal = portalHit.collider.GetComponent<Portal>();

                nextNode.iterationsSincePortalCrossed = 2;

                if (portal.inverseY)
                {
                    nextNode.portalSense.y *= -1;

                }

                if (portal.inverseX)
                {
                    nextNode.portalSense.x *= -1;
                }


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

        /*
         Comprueba si el nodo colisiona con objetos en movimiento.

         Es un poco tricky. Modifica el offset de los colliders y hace un cast con ellos después.
         Estos colliders son sólo para este cast, de modo q no hace falta volver a moverlos.

         Por temas de precisión, toma en cuenta posiciones pasadas y futuras.
         (Por eso hay un for que incrementa el tiempo)

         */
        bool CollidesWithDynamicObstacle(ref Vector2 nextPos, ref Vector2 prevPos, ref float time)
        {

            const float TIME_INCREMENT = PRECALCULATION_INCREMENT_DELTATIME;
            float timeCheck = time - PRECALCULATION_INCREMENT_DELTATIME*2;

            bool collides = false;

            for (int i = 0; i < 4; i++)
            {

                // Actualizamos la info de los colliders para que correspondan
                // al momento del tiempo simulado
                foreach (var obstacle in movingObstaclesToHandle)
                    obstacle.UpdateAvoidAstarInfo(timeCheck);

                foreach (var obstacle in rotatingObstaclesToHandle)
                    obstacle.UpdateAvoidAstarInfo(timeCheck);


                // Hacemos el raycast teniendo en consideración el radio del player
                Vector2 perp = Vector2.Perpendicular(prevPos - nextPos).normalized * predictionPlayerRadius;
                collides = Physics2D.Linecast(prevPos + perp, nextPos + perp, layerMaskAvoidImaginary) || Physics2D.Linecast(prevPos - perp, nextPos - perp, layerMaskAvoidImaginary);
                

                //Early exit
                if (collides)
                    break;

                timeCheck += TIME_INCREMENT;
            }

            

            return collides;
        }

        /*
        
        Calcula el coste entre un nodo y el siguiente.

        Si el nodo no es accesible tiene un coste = -1.

        El cast se hace teniendo en consideracion el radio del player.

         */
        float CalculateCost(Vector2 from, ref Vector2 to, ref float time, bool checkCollision = true)
        {
            
        
            bool collides = false;

            if (checkCollision)
            {
                // Por temas de portales, existe un trigger que se usa para ignorar el cast en una zona en concreto.
                // Esto es por si mi cast cruza el portal y da a la pared. Pues esa zona negará el cast a la pared.
                bool raycastNot = Physics2D.Linecast(from, to, layerMaskRaycastNOT);


                if (!raycastNot) // En caso de que SÍ debamos comprobar el cast:
                {
                    // Realizamos el cast teniendo en cuenta el radio del player
                    Vector2 perp = Vector2.Perpendicular(from-to).normalized * predictionPlayerRadius;
                    collides = Physics2D.Linecast(from+perp, to+perp, layerMaskPrediction) || Physics2D.Linecast(from - perp, to - perp, layerMaskPrediction) || CollidesWithDynamicObstacle(ref to, ref from, ref time);
                }
            }

            //IMPORTANTE: Las siguientes float lineas dibujan los pasos
            //que se tienen en consideración.
            //Rojo: colisiona, Verde: ok.
            //Son MUY útiles para visualizar los nodos visitados por el algoritmo. 

            //float alpha = 0.4f;
            //Color c = collides ? new Color(1f, 0f, 0f, alpha) : new Color(0f, 1f, 0f, alpha);
            //Debug.DrawLine(from, to, c, 1.2f);

            // Cálculo final del coste:
            float cost = collides ? -1f : (from - to).magnitude;

            return cost;
        }

        /*
         Determina si un salto a partir de un nodo es válido para encontrar el camino.

         Un salto es válido si:
           A) pasa cerca del astarGoal
           B) pasa cerca del portal más cercano

        IMPORTANTE: El objetivo de esta función es descartar casos de manera eficiente.
        NO debe usarse ningún raycast aquí.
         
         */

        bool JumpIsValid(ref AStarNode inNode, int directionIndex)
        {

            bool valid = false;
            const int INCREMENT = PRECALCULATED_POINTS_INCREMENT;

            // por cada punto del salto
            for (int pathIndex = 0; pathIndex < NUMBER_OF_PRECALCULATED_POINTS; pathIndex += INCREMENT)
            {
                // calculo su posicion
                var nextPosition = inNode.position + inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(directionIndex, pathIndex);

                // Si pasa cerca del goal o un portal, lo valido.
                if (Vector2.Distance(nextPosition, goalPosition) <= GOAL_MIN_DISTANCE || (usePortal && Vector2.Distance(nextPosition, portalPosition) <= GOAL_MIN_DISTANCE))
                {
                    valid = true;
                    break;
                }

            }

            return valid;
        }

        
        /*
         Éste método es el núcleo del Astar. Es el iterador de nodos vecinos.
         
         Calcula todos los posibles nodos vecinos de inNode y ejecuta el método "method"
         con ellos.

         Ésta función tiene 2 partes importants:
           a) Primero evalua el nodo que representa continuar la trayectoria sin saltar
           b) Evalua los nodos que representan saltar interrumpiendo la trayectoria

         */
        delegate void ASDelegationNeighbour(AStarNode node);
        void ForeachNeighbour(ref AStarNode inNode, ASDelegationNeighbour method)
        {
            // CASO A: CONTINUAR LA TRAYECTORIA

            // Si quedan puntos por explorar en la trayectoria:
            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                Vector2 origin = inNode.position - inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex);
                Vector2 nextPos = origin + inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(inNode.directionIndex, inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT);

                //extraemos su coste.
                float cost = CalculateCost(inNode.position, ref nextPos, ref inNode.time);

                // Si el coste nos indica que el nodo no colisiona:
                if(cost >= 0)
                {
                    // Generamos el siguiente nodo
                    AStarNode next = new AStarNode(
                        nextPos, 
                        inNode.secondJumpDone, 
                        inNode.directionIndex, 
                        inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT, 
                        cost, 
                        inNode.time + PRECALCULATION_INCREMENT_DELTATIME, 
                        inNode.iterationsSincePortalCrossed-1
                    );
                    // Corregimos su infomración si pasa por un portal
                    PortalCase(ref next, inNode.position);

                    // Iteramos con el nodo resultante
                    method(next);
                }
            }


            // CASO B: SALTAR EN MITAD DE LA TRAYECTORIA
            
            // Si el segundo salto todavía no ha sido dado
            // Y no acabamos de cruzar un portal 
            if (!inNode.secondJumpDone && inNode.iterationsSincePortalCrossed <= 0)
            {

                int dirIndex = 0;

                // Por cada posible dirección hacia la que podriamos saltar
                for (int i = 0; i < DIRECTIONS_COUNT; i++)
                {
                    // Descartamos los nodos vecinos que no sean válidos.
                    // Ésto reduce las iteraciones en 10x.
                    // Es mucho más barato predecir si una iteración es válida que ejecutarla.
                    if (!JumpIsValid(ref inNode, i))
                        continue;

                    // Calculamos la primera posición del salto.
                    var nextNodePos = inNode.portalSense * jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0) + inNode.position;

                    // Evaluamos el coste del nodo
                    float cost = CalculateCost(inNode.position, ref nextNodePos, ref inNode.time);
                    
                    // Si el coste nos indica que no colisiona con ninguna pared ni obstáculo
                    if (cost >= 0)
                    {
                        // Generamos la información del nodo
                        AStarNode next = new AStarNode(
                            nextNodePos, 
                            true, 
                            dirIndex++, 
                            0, 
                            cost, 
                            inNode.time + PRECALCULATION_INCREMENT_DELTATIME,
                            0);
                        // Modificamos la información en el caso de que pase por algún portal
                        PortalCase(ref next, inNode.position);

                        // iteramos con el nodo creado
                        method(next);
                    }
                }
            }
        }

        /*
         Genera la frontier inicial para el Astar.
         
         Rellena la frontera con todos los posibles saltos estando en el suelo.
       
         
         */
        PriorityQueue<AStarNode, float> SetUpFrontierAstar(Vector2 goalPosition, float timeToStart)
        {

            var frontier = new PriorityQueue<AStarNode, float>(0f);

            // Por cada posible dirección de salto 
            for (int i = 2; i < DIRECTIONS_COUNT-1; i++)
            {
                // Genero la la primera posición de la trayectoria. 
                Vector2 position = originPosition + jumpPredictor.ReadLocalSimulationPositionIgnoringVelocity(i, 0);

                // Genero información de nodo
                var time = PRECALCULATION_DELTATIME + timeToStart;
                float cost = CalculateCost(originPosition, ref position, ref time, false);

                // Si el coste nos indica que no colisiona con pared/obstaculos
                if(cost >= 0)
                {
                    // termino de generar el nodo y lo añado a la frontera
                    var an = new AStarNode(position, false, i, 0, cost, PRECALCULATION_DELTATIME, 0);
                    float priority = cost + an.H(goalPosition);
                    frontier.Insert(an, priority);
                }

            }

            return frontier;

        }

        /*
         Devuelve el delta position respecto a la anterior posición.
         */
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

        /*
         Determina si se cumplen las condiciones para hacer early exit.

        Incision constraint:
         En ocasiones un astarGoal requiere que la velocidad de incisión sea en 
         una dirección en concreto para asegurar que choca con la pared.
         
         */
        bool EarlyExit(ref AStarNode current, ref AstarGoal goal)
        {

            float distanceToGoal = (goal.position - current.position).magnitude;
            if (distanceToGoal <= GOAL_MIN_DISTANCE)
            {
                if (!goal.useIncisionConstrain)
                    return true;

                Vector2 deltaPos = GetNextDeltaPos(ref current);
                float dot = Vector2.Dot(goal.incisionDirection.normalized, deltaPos);
                
                // IMPORTANTE: Usamos un random para la comprobación para dar un comportamiento más humano.
                // De no ser por el random, todos los saltos de un nodo A al nodo B usarían la misma inclinación.
                // NO TOCAR
                if(dot > Random.Range(0.2f, 0.4f))
                {
                    return true;
                }
            }

            return false;
        }



        /*
         
         Ejecuta el algoritmo de AStar de manera literal a la definición academécia del algoritmo.

         Si hay problemas para entenderlo, revisar las diapositivas de LLuís.
         Preguntar por ellas a Pol Surriel.
         
         */
        public List<AStarNode> AStar(Vector2 startPosition, AstarGoal goal, float timeToStart)
        {

            // SETUP
            originPosition = startPosition;
            goalPosition = goal.position;
            var frontier = SetUpFrontierAstar(goal.position, timeToStart);
            Dictionary<AStarNode, AStarNode> cameFrom = new Dictionary<AStarNode, AStarNode>();
            Dictionary<AStarNode, float> costSoFar = new Dictionary<AStarNode, float>();


            // Si no podemos saltar, devolvemos path not found
            if (frontier.Empty())
                return null;


            // Iniciamos algoritmo AStar
            AStarNode current = null;
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



            // BACKTRACKING

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
