using DataStructures.PriorityQueue;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using SurrealBoost.Utils;
using UnityEditor;


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
    float realPlayerRadius;

    public class AStarSolver
    {

        public float goalMinDist;

        const int MIN_FIRST_ITERATIONS_TO_USE_SECONDJUMP = 3;
        public MovingObstacle[] movingObstaclesToHandle;
        public RotatingObstacle[] rotatingObstaclesToHandle;

        Vector2 originPosition;
        float predictionPlayerRadius;
        float realPlayerRadius;
        JobyfablePrecalculatedPredictionSystem jumpPredictor;
        int layerMaskRaycastNOT;
        int layerMaskPortal;
        int layerMaskPrediction;
        Vector2 goalPosition;
        int jumpPredictorIndex;


        public AStarSolver(float predictionPlayerRadius, float realPlayerRadius, JobyfablePrecalculatedPredictionSystem jumpPredictor)
        {

            layerMaskPortal = 1 << LayerMask.NameToLayer("Portal");
            layerMaskPrediction = (1 << LayerMask.NameToLayer("floor")) | 
                                  (1 << LayerMask.NameToLayer("Obstacle")) | (1 << LayerMask.NameToLayer("ImaginaryAvoid"));
            layerMaskRaycastNOT = (1 << LayerMask.NameToLayer("RaycastNOT"));

            this.jumpPredictor = jumpPredictor;
            this.predictionPlayerRadius = predictionPlayerRadius;
            this.realPlayerRadius = realPlayerRadius;

            SetupIterationDiscarder();

        }

        public void OnDestroy()
        {
            try
            {
                jumpPredictor.precalculatedDirections.Dispose();

            }catch(System.InvalidOperationException e) { }
        }

        void SetupIterationDiscarder()
        {

            AStarIterationsDiscarder.m_precalculatedDirections = jumpPredictor.precalculatedDirections;
            jumpPredictorIndex = AStarIterationsDiscarder.lastAddedJumpPredictorIndex;

        }


        /* Este metodo modifica la información de un nodo si este cruza un portal.
           Modifica el sentido de su x/y si es necesario y añade un offset a su
           posicion.
         */
        void PortalCase(ref AStarNode nextNode,  Vector2 prevPos, ref RaycastHit2D portalHit)
        {
            if (portalHit)
            {
                var portal = portalHit.collider.GetComponent<Portal>();

                nextNode.iterationsSincePortalCrossed = 1;

                Vector2 deltaMove = nextNode.position - prevPos; // value previous of crossing portal.
                
                
                if (portal.swapXY)
                {
                    // 1) we get the swapped+inversed new velocity
                    deltaMove *= new Vector2( portal.inverseX ? -1f:1f, portal.inverseY ? -1f : 1f);
                    var newVel = Portal.SmartSwap(true, portal.otherPortal.normal, deltaMove.normalized);

                    // 2) we get the new local position relative to other portal (local swapped!)
                    var relativePortal = prevPos - (Vector2)portal.transform.position;
                    var relativeOtherPortal = Portal.SmartSwap(true, portal.otherPortal.normal, relativePortal);

                    // 3) we get the next node position
                    nextNode.position = (Vector2)portal.otherPortal.transform.position + relativeOtherPortal + portal.otherPortal.normal * realPlayerRadius;

                    // 4) using new velocity we get the new simulation index
                    //nextNode.directionIndex = jumpPredictor.GetSimulationIndex(newVel);

                    // then we clean portal sense
                    //nextNode.portalSense = Vector2.one;

                    // 5) we get the new origin
                    //nextNode.origin = nextNode.position - (jumpPredictor.precalculatedDirections[nextNode.directionIndex * jumpPredictor.iterationsCount + nextNode.positionIndex]);

                    //tmp
                    nextNode.forceEndOfPath = true;

                }
                else
                {

                    if (portal.inverseY)
                        nextNode.portalSense.y *= -1;

                    if (portal.inverseX)
                        nextNode.portalSense.x *= -1;


                    deltaMove *= nextNode.portalSense;
                    nextNode.position = (Vector2)portal.otherPortal.transform.position + (deltaMove);


                    // Calculate new origin
                    var currentPosToOrigin = jumpPredictor.precalculatedDirections[nextNode.directionIndex * jumpPredictor.iterationsCount + nextNode.positionIndex];
                    nextNode.origin = nextNode.position - nextNode.portalSense * currentPosToOrigin;

                    Debug.DrawLine(prevPos, nextNode.position, Color.yellow, 999999f);


                    
                }

            }


        }

        /*
         Comprueba si el nodo colisiona con objetos en movimiento.

         Es un poco tricky. Modifica el offset de los colliders y hace un cast con ellos después.
         Estos colliders son sólo para este cast, de modo q no hace falta volver a moverlos.

         Por temas de precisión, toma en cuenta posiciones pasadas y futuras.
         (Por eso hay un for que incrementa el tiempo)

         */
        bool CollidesWithDynamicObstacle(ref Vector2 nextPos, ref Vector2 prevPos, float time
#if UNITY_EDITOR
, AStarNode node
#endif
        )
        {

            time += timeBeforeJump - timeSinceCalculationStarded + PRECALCULATION_INCREMENT_DELTATIME;

            const int steps = 2;
            const float TIME_INCREMENT = PRECALCULATION_INCREMENT_DELTATIME;
            float timeCheck = time;


#if UNITY_EDITOR

            var dbprevPos = prevPos;
            var dbnextPos = nextPos;
            node.ifChoosenDoOnGizmos.Add(()=> {
                //Debug.DrawLine(dbprevPos, dbnextPos, Color.red);

            });
#endif

            Vector2 deltaPos = nextPos - prevPos;
            var deltaMovementLine = new SurrealBoost.Types.Line() { pointA = prevPos, pointB = nextPos };

            if (deltaPos.y < 0f)
            {
                foreach (var transitionline in TransitionLine.lines)
                {
                    if (transitionline.fillsAllXAxis)
                    {
                        if (transitionline.pointA.y > nextPos.y)
                        {
                            return true;
                        }

                    }else
                    {
                        if (Intersection2D.lineLine(transitionline.line, deltaMovementLine).result)
                        {
                            return true;
                        }

                    }

                }
            }

            for (int i = 0; i < steps; i++)
            {

                // Actualizamos la info de los colliders para que correspondan
                // al momento del tiempo simulado
                foreach (var obstacle in rotatingObstaclesToHandle)
                {
                    var opos = obstacle.GetFuturePosition(timeCheck);
#if UNITY_EDITOR
                    node.ifChoosenDoOnGizmos.Add(() => {
                        Debug.DrawLine(opos, opos + Vector2.up * 0.2f, Color.green);
                    });
#endif


                    if (Intersection2D.LineCircle(prevPos, nextPos, obstacle.GetFuturePosition(timeCheck), obstacle.colliderRadius))
                    {
                        return true;
                    }
                }


                foreach (var obstacle in movingObstaclesToHandle)
                {
                    if(Intersection2D.LineCircle(prevPos, nextPos, obstacle.GetFuturePosition(timeCheck), obstacle.colliderRadius))
                    {
                        return true;
                    }

                }

                timeCheck += TIME_INCREMENT;
            }

            return false;
        }

        /*
        
        Calcula el coste entre un nodo y el siguiente.

        Si el nodo no es accesible tiene un coste = -1.

        El cast se hace teniendo en consideracion el radio del player.

         */
        float CalculateCost(Vector2 from, ref Vector2 to, float time, ref RaycastHit2D portalHit,
#if UNITY_EDITOR
            AStarNode node,
#endif
            bool checkCollision = true)
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

                    RaycastHit2D wallCast1 = Physics2D.Linecast(from + perp, to + perp, layerMaskPrediction);
                    RaycastHit2D wallCast2 = Physics2D.Linecast(from - perp, to - perp, layerMaskPrediction);
                    collides = wallCast1 || wallCast2 || CollidesWithDynamicObstacle(ref to, ref from, time

#if UNITY_EDITOR
                        , node
#endif
                        );



                    /*
                     Si hay cast positivo con la pared, PERO también hay cast positivo con el goalPosition o un portal,
                    Y el punto de intersección está más cerca del inicio de la línea, significa que el player llega antes
                    al portal/goal que a la pared y el cast debe ser anulado.
                     */
                    if (collides)
                    {
                        var closestWall = wallCast1 ? wallCast1.point : wallCast2.point;
                        var wallDist = Vector2.Distance(from, closestWall);

                        if( Vector2.Distance(to, goalPosition) <= goalMinDist  && Vector2.Distance(goalPosition, from) < wallDist)
                        {
                            collides = false;
                        }

                        // revisamos si hay que anular pq llega primero a portal
                        if (portalHit)
                        {
                            var closestPortal = portalHit.point;
                            var portalDist = Vector2.Distance(from, closestPortal);

                            if(portalDist < wallDist)
                            {
                                collides = false;
                            }
                        }

                        // revisamos si hay que anular pq llega primero a goal
                        float distanceToGoal = (goalPosition - to).magnitude;
                        if (distanceToGoal <= goalMinDist)
                        {
                            var closestGoal = SurrealBoost.Utils.Intersection2D.ClosestLineCircle(from, to,goalPosition, goalMinDist);
                            var goalDist = Vector2.Distance(from, closestGoal);
                            if(goalDist < wallDist)
                            {
                                collides = false;
                            }
                        }

                    }
                
                }
            }

#if UNITY_EDITOR
            //IMPORTANTE: Las siguientes float lineas dibujan los pasos
            //que se tienen en consideración.
            //Rojo: colisiona, Verde: ok.
            //Son MUY útiles para visualizar los nodos visitados por el algoritmo. 

            if (GizmosCustomMenu.instance.aiEvaluatedPaths)
            {
                float alpha = 0.999f;
                Color c = collides ? new Color(1f, 0f, 0f, alpha) : new Color(0f, 1f, 0f, alpha);
                Debug.DrawLine(from, to, c, 1.2f);
            }

#endif

            // Cálculo final del coste:
            float cost = collides ? -1f : (from - to).magnitude;

            return cost;
        }


        void DebugDrawJump(ref AStarNode inNode, int directionIndex, int endIndex, float duration = 1f)
        {
            Color c = Color.red;
            c.a = 0.3f;
            
            const int INCREMENT = PRECALCULATED_POINTS_INCREMENT;

            // por cada punto del salto
            for (int pathIndex = 0; pathIndex < NUMBER_OF_PRECALCULATED_POINTS && pathIndex < endIndex; pathIndex += INCREMENT)
            {
                // calculo su posicion
                var nextPosition = inNode.position + (inNode.portalSense * jumpPredictor.precalculatedDirections[directionIndex* jumpPredictor.iterationsCount + pathIndex]);
                var lastPos = inNode.position + (inNode.portalSense * jumpPredictor.precalculatedDirections[directionIndex* jumpPredictor.iterationsCount + pathIndex>0 ? pathIndex-INCREMENT : 0]);

                Debug.DrawLine(nextPosition, lastPos, c, duration);


            }



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

            if (inNode.forceEndOfPath)
                return;

            
            // CASO A: CONTINUAR LA TRAYECTORIA

            // Si quedan puntos por explorar en la trayectoria:
            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                //Vector2 origin = inNode.position - inNode.portalSense * jumpPredictor.precalculatedDirections[inNode.directionIndex * jumpPredictor.iterationsCount + inNode.positionIndex];


                var localRelativeToOrigin = jumpPredictor.precalculatedDirections[inNode.directionIndex * jumpPredictor.iterationsCount + inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT];
                Vector2 nextPos = inNode.origin + (inNode.portalSense * localRelativeToOrigin);

                //extraemos su coste.
                RaycastHit2D portalHit = Physics2D.Linecast(inNode.position, nextPos, layerMaskPortal);



                float cost = CalculateCost(inNode.position, ref nextPos, inNode.time, ref portalHit
#if UNITY_EDITOR
                            ,inNode
#endif
                    );

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
                    next.origin = inNode.origin;
                    // Corregimos su infomración si pasa por un portal
                    PortalCase(ref next, inNode.position, ref portalHit);

                   
                    
                    // Iteramos con el nodo resultante
                    method(next);
                }
            }

            
            

            // CASO B: SALTAR EN MITAD DE LA TRAYECTORIA

            // Si el segundo salto todavía no ha sido dado
            // Y no acabamos de cruzar un portal 
            if (!inNode.secondJumpDone /*&& inNode.iterationsSincePortalCrossed <= 0 TMF FIX */)
            {

                // VALIDACION DIRECCIONES - Descartamos iteraciones no-necesarias
                var directionValidation = new Unity.Collections.NativeArray<bool>(DIRECTIONS_COUNT, Unity.Collections.Allocator.Persistent);

                AStarIterationsDiscarder iterationDiscarder = new AStarIterationsDiscarder()
                {
                    m_result = directionValidation,
                    nodePosition = inNode.position,
                    m_goalPosition = goalPosition,
                    m_portalSense = inNode.portalSense,
                    m_iterationsCount = jumpPredictor.iterationsCount,
                    m_characterRadius = predictionPlayerRadius,
                    m_jumpPredictorIndex = jumpPredictorIndex

                };

                JobHandle jobHandle = iterationDiscarder.Schedule(DIRECTIONS_COUNT, ITERATION_DISCARDER_BATCH);
                jobHandle.Complete();
                

                // Por cada posible dirección hacia la que podriamos saltar
                for (int i = 0; i < DIRECTIONS_COUNT; i++)
                {
                    // Descartamos los nodos vecinos que no sean válidos.
                    // Ésto reduce las iteraciones en 10x.
                    // Es mucho más barato predecir si una iteración es válida que ejecutarla.
                    if (!directionValidation[i])
                        continue;

                    // Calculamos la primera posición del salto.
                    var nextNodePos = inNode.position + (inNode.portalSense * jumpPredictor.precalculatedDirections[i * jumpPredictor.iterationsCount + PRECALCULATED_POINTS_INCREMENT]);

                    // Evaluamos el coste del nodo
                    RaycastHit2D portalHit = Physics2D.Linecast(inNode.position, nextNodePos, layerMaskPortal);


                    float cost = CalculateCost(inNode.position, ref nextNodePos, inNode.time, ref portalHit
#if UNITY_EDITOR
                            , inNode
#endif
                    );


                    // Si el coste nos indica que no colisiona con ninguna pared ni obstáculo
                    if (cost >= 0)
                    {
                        // Generamos la información del nodo
                        AStarNode next = new AStarNode(
                            nextNodePos, 
                            true, 
                            i, 
                            0, 
                            cost * jumpCostScalar, 
                            inNode.time,
                            0);
                        next.origin = inNode.position;
                        // Modificamos la información en el caso de que pase por algún portal
                        PortalCase(ref next, inNode.position, ref portalHit);

                        // iteramos con el nodo creado
                        method(next);
                    }
                }

                directionValidation.Dispose();

            }
        }

        /*
         Genera la frontier inicial para el Astar.
         
         Rellena la frontera con todos los posibles saltos estando en el suelo.
       
         
         */
        PriorityQueue<AStarNode, float> SetUpFrontierAstar(Vector2 goalPosition)
        {

            var frontier = new PriorityQueue<AStarNode, float>(0f);

            // Por cada posible dirección de salto 
            for (int i = 2; i < DIRECTIONS_COUNT-1; i++)
            {
                // Genero la la primera posición de la trayectoria. 
                Vector2 position = originPosition + jumpPredictor.precalculatedDirections[i * jumpPredictor.iterationsCount + 0];

                // Genero información de nodo
                var time = 0f;
                RaycastHit2D portalHit = Physics2D.Linecast(originPosition, position, layerMaskPortal);
                float cost = CalculateCost(originPosition, ref position, time, ref portalHit,
#if UNITY_EDITOR
                        null, // ONLY NULL IF IS FIRST NODE
#endif
                        false
                    );

                // Si el coste nos indica que no colisiona con pared/obstaculos
                if(cost >= 0)
                {
                    // termino de generar el nodo y lo añado a la frontera
                    var an = new AStarNode(position, false, i, 0, cost, time, MIN_FIRST_ITERATIONS_TO_USE_SECONDJUMP);
                    an.origin = originPosition;

                    float priority = cost + an.H(goalPosition);
                    frontier.Insert(an, priority);
                }

            }

            return frontier;

        }

        void DebugDrawJumpFromOrigin(ref AStarNode inNode, Vector2 origin, Color c)
        {
            var currentPos = origin;
            for (int i = 0; i < inNode.positionIndex; i += PRECALCULATED_POINTS_INCREMENT)
            {
                var nextPos = origin + (inNode.portalSense * jumpPredictor.precalculatedDirections[inNode.directionIndex* jumpPredictor.iterationsCount + i + PRECALCULATED_POINTS_INCREMENT]);
                Debug.DrawLine(currentPos, nextPos, c, 1f);
                currentPos = nextPos;
            }
        }

        /*
         Devuelve el delta position respecto a la anterior posición.
         */
        Vector2 GetNextDeltaPos(ref AStarNode inNode)
        {

            //Vector2 origin = inNode.position - inNode.portalSense * jumpPredictor.precalculatedDirections[inNode.directionIndex* jumpPredictor.iterationsCount + inNode.positionIndex];

            //DebugDrawJumpFromOrigin(ref inNode, origin, Color.green);

            if (inNode.positionIndex < NUMBER_OF_PRECALCULATED_POINTS - PRECALCULATED_POINTS_INCREMENT)
            {
                // Calculamos la posición inmediatamente siguiente en la trayectoria actual
                Vector2 nextPos = inNode.origin + (inNode.portalSense * jumpPredictor.precalculatedDirections[inNode.directionIndex* jumpPredictor.iterationsCount + inNode.positionIndex + PRECALCULATED_POINTS_INCREMENT]);
                //if (inNode.secondJumpDone)
                //{
                //    Debug.DrawLine(inNode.position, nextPos, Color.magenta, 1f);
                //}
                return nextPos - inNode.position;

            }else
            {
                Vector2 lastPos = inNode.origin + (inNode.portalSense * jumpPredictor.precalculatedDirections[inNode.directionIndex* jumpPredictor.iterationsCount + inNode.positionIndex - PRECALCULATED_POINTS_INCREMENT]);
                //if (inNode.secondJumpDone)
                //{ 
                //    Debug.DrawLine(inNode.position, lastPos, Color.magenta, 1f);
                //}
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
            if (distanceToGoal <= goalMinDist)
            {

                
                if (!goal.useIncisionConstrain)
                    return true;

                Vector2 deltaPos = GetNextDeltaPos(ref current);
                float dot = Vector2.Dot(goal.incisionDirection.normalized, deltaPos.normalized);
                //Debug.DrawLine(current.position, current.position + deltaPos.normalized * 1f, testColor, 1f);
                //Debug.DrawLine(current.position + deltaPos.normalized * 1f + Vector2.right * 0.1f, current.position + deltaPos.normalized * 1f + Vector2.left * 0.1f, Color.yellow, 1f);
                //Debug.DrawLine(current.position + deltaPos.normalized * 1f + Vector2.up * 0.1f, current.position + deltaPos.normalized * 1f + Vector2.down * 0.1f, Color.yellow, 1f);
                //Vector2 origin = current.position - current.portalSense * jumpPredictor.precalculatedDirections[current.directionIndex* jumpPredictor.iterationsCount + current.positionIndex];
                //DebugDrawJumpFromOrigin(ref current, origin, testColor);


                // IMPORTANTE: Usamos un random para la comprobación para dar un comportamiento más humano.
                // De no ser por el random, todos los saltos de un nodo A al nodo B usarían la misma inclinación.
                // NO TOCAR
                if (dot > Random.Range(0.2f, 0.4f))
                {
                    return true;
                }
            }

            return false;
        }



        const int MAX_FRAME_ITERATIONS = 43;
        public float timeBeforeJump = 0f;
        public float timeSinceCalculationStarded = 0f;

        /*
         
         Ejecuta el algoritmo de AStar de manera literal a la definición academécia del algoritmo.

         Si hay problemas para entenderlo, revisar las diapositivas de LLuís.
         Preguntar por ellas a Pol Surriel.
         
         */

        public List<AStarNode> output;

        float jumpCostScalar;

        public IEnumerator AStar(Vector2 startPosition, AstarGoal goal, float timeToStart)
        {

            jumpCostScalar = Random.Range(1f, 2.5f);

            // SETUP
            timeBeforeJump = timeToStart;
            timeSinceCalculationStarded = 0f;
            originPosition = startPosition;
            goalPosition = goal.position;
            var frontier = SetUpFrontierAstar(goal.position);
            Dictionary<AStarNode, AStarNode> cameFrom = new Dictionary<AStarNode, AStarNode>();
            Dictionary<AStarNode, float> costSoFar = new Dictionary<AStarNode, float>();


            const int INTERATIONS_NEEDED_AVG = 1500;
            float fps = (1.0f / Time.smoothDeltaTime);
            int totalFrames = (int)(timeBeforeJump * fps) +1;
            int iterationBatch = INTERATIONS_NEEDED_AVG / totalFrames;

            //int iterationsBatch = 

            // Si no podemos saltar, devolvemos path not found
            if (frontier.Empty())
            {
                output = null;

            }
            else
            {
                int iterationsCount = 0;

                // Iniciamos algoritmo AStar
                AStarNode current = null;
                do
                {

                    if ((iterationsCount++ >= MAX_FRAME_ITERATIONS) && timeBeforeJump != 0f)
                    {
                        iterationsCount = 0;
                        yield return null;
                        timeSinceCalculationStarded += Time.deltaTime;
                    }

                    current = frontier.Top();
                    frontier.Pop();
                    if (EarlyExit(ref current, ref goal))
                    {
                        break;
                    }


                    ForeachNeighbour(ref current, (AStarNode neighbor) =>
                    {

                        float currentCostSoFar = costSoFar.ContainsKey(current) ? costSoFar[current] : 0f;
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

                            //if (neighbor.secondJumpDone && EarlyExit(ref neighbor, ref goal))
                            //{
                            //    //Vector2 origin = neighbor.position - neighbor.portalSense * jumpPredictor.precalculatedDirections[neighbor.directionIndex* jumpPredictor.iterationsCount + neighbor.positionIndex];
                            //    //DebugDrawJumpFromOrigin(ref neighbor, origin, Color.red);
                            //}

                        }


                    });


                } while (!frontier.Empty());



                // BACKTRACKING

                if (!frontier.Empty())
                {
                    output = new List<AStarNode>(40);
                    // full backtrack
                    while (cameFrom.ContainsKey(current))
                    {
                        if (cameFrom[current].secondJumpDone == current.secondJumpDone)
                            output.Insert(0, current);
                        current = cameFrom[current];

                    }
                }

            }

        }

    }

}
