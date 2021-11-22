using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 Este archivo contiene todas las funcionalidades que USAN el Astar.

 Existen 3 métodos importantes aquí:

  1) StartAStartPipeline: Inicia todo el proceso del AStar.
     - Si hay un camino activo al que le estamos haciendo seek lo cancela
     - Decide un target hacia el que saltar
     - Ejecuta el Astar para encontrar un camino hacia ese target.
     - Llama al método StartAStartSeekPathResult()

  2) StartAStartSeekPathResult: Prepara los atributos de estado 
     para que el fixedUpdate ejecute el resultado del Astar

  3) AstarExecutionFixedUpdate: En caso de estar haciendo seek a un
     resultado de Astar, lo aplica.

 */
public partial class AIController : MonoBehaviour
{

    List<AStarNode> currentPath;
    AStarNode currentNode;

    const float GOAL_MIN_DISTANCE = 0.9f;

    bool onBackupPlanZone = false;

    PathTarget currentPathTargetObject;
    List<PathTarget> targetsToIgnore = new List<PathTarget>();

    bool executingAstarSeek = false;
    float astarSeekTimeCounter = 0f;
    bool astarSeeklastSecondJumpDone = true;
    bool astarSeekFirstIteration = true;
    int astarSeekNodeIndex;
    List<AStarNode> astarSeekNodes;

    Vector2 lastTargetPos = Vector2.zero;


    void AstarExecutionFixedUpdate()
    {
        // Si estoy siguiendo un camino
        if (executingAstarSeek)
        {

            // Si el camino no es válido aborto
            if (astarSeekNodes.Count == 0)
            {
                executingAstarSeek = false;
                return;
            }

            // obtengo la información del tramo actual
            currentNode = astarSeekNodes[astarSeekNodeIndex];

            // Compruebo si debo saltar
            if (currentNode.secondJumpDone != astarSeeklastSecondJumpDone || astarSeekFirstIteration)
            {
                // Salto
                astarSeeklastSecondJumpDone = !astarSeeklastSecondJumpDone;
                astarSeekFirstIteration = false;
                runner.Jump(jumpPredictor.GetForce(currentNode.directionIndex));

            }

            // Incremento el contador de tiempo,
            // ya que paso de un nodo al siguiente en función del tiempo.
            astarSeekTimeCounter += Time.fixedDeltaTime;

            // Compruebo si ya me toca pasar al siguiente nodo
            if (!(astarSeekTimeCounter < currentNode.time))
            {
                astarSeekNodeIndex++;
                if (astarSeekNodeIndex == astarSeekNodes.Count)
                {
                    executingAstarSeek = false;
                }
            }
        }
    }


    /*
     Prepara los atributos de estado 
     para que el fixedUpdate ejecute el resultado del Astar
     */
    void StartAStartSeekPathResult(ref List<AStarNode> nodes)
    {

        astarSeekNodes = nodes;
        executingAstarSeek = true;
        astarSeeklastSecondJumpDone = true;
        astarSeekFirstIteration = true;
        astarSeekNodeIndex = 0;
        astarSeekTimeCounter = 0f;
    }



    void StartAStarPipeline()
    {

        executingAstarSeek = false;
        pendingToStartAStarPipeline = false;

        // Si la eleección de target fue exitosa
        if (ChooseTarget())
        {
            // Generamos infomración para ejecutar el Astar a hacia ese target
            aStarSolver.movingObstaclesToHandle = FindObjectsOfType<MovingObstacle>();
            aStarSolver.rotatingObstaclesToHandle = FindObjectsOfType<RotatingObstacle>();

            aStarSolver.portalPosition = closestPortal;
            aStarSolver.usePortal = usePortal;

            // Calculamos el camino
            currentPath = aStarSolver.AStar(transform.position, aStarGoal, timeBeforeJump);
            
            // Si el caminno no fue encontrado
            if (currentPath == null)
            {

                // Avisamos a los logs
                Debug.LogError("No path found.");

                // PERO:
                //at next frame, we'll try with another target!
                targetsToIgnore.Add(currentPathTargetObject);
                pendingToStartAStarPipeline = true;
                return;

            }
            else // Si el camino es válido
            {
                // Iniciamos el seek al resultado del AStar
                StartAStartSeekPathResult(ref currentPath);
                targetsToIgnore = new List<PathTarget>();

            }

        }
        else
        {// Si no encontramos ningún target valido:

            // Indicamos al controlador que queremos reiniciar proceso de búsqueda de target
            // en el siguiente frame.
            pendingToStartAStarPipeline = true;
            targetsToIgnore = new List<PathTarget>();
            
            //Si el tilemap requiere esperar a un momento del tiempo en
            //concreto para saltar, el backup plan no debería ser generico.
            //de algun modo deberiamos tener cuidado con esto. Reiniciar
            //la busqueda de target en el momento apropiado vs ejecutar 
            //un comportamientor reactivo
            // Problema: En ocasiones nos interesa dejar q esta zona del código no haga nada
            // y reanude el proceso infinitamente. Ej: Cuando un obstaculo dinamico se mueve
            // y la ia debe esperar para saltar. Nos intersa estar calculando todo el tiempo
            // el camino entre los diferentes targets hasta que sea posible saltar.
            // De momento no tocamos nada.
            Debug.LogError("No valid target found! We need to code some backup plan here.");
        }
    }

    
    IEnumerator WaitAndRestartAstar(float time)
    {
        float t = 0;
        while (t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }

        pendingToStartAStarPipeline = true;
    }




}
