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
    

    void FixVelocityToCurrentNode(Vector2 currentNodePosition)
    {
        // Posiblemente calcular aquí la desviación de la trayectoria

        Vector2 targetVelocity = (currentNodePosition - (Vector2)transform.position).normalized * rb.velocity.magnitude;

        const float CHANGE_SPEED = 2f;
        float scalar = 1f- Mathf.Min((CHANGE_SPEED * Time.deltaTime), 1f);

        // Add humanization angle variation
        // Disabled for pre-alpha
        //float angleVariation = AIDirector.GetVectorBeforeJump(HumanizationVariationFactor) * angleVariationAI;
        //targetVelocity = targetVelocity.Rotate(angleVariation);

        // apply
        rb.velocity = rb.velocity * scalar + (1f-scalar) * targetVelocity;

    }


    float timeExecuting;
    float suposedTimeNow;
    void AstarExecutionFixedUpdate()
    {


        // Si estoy siguiendo un camino
        if (executingAstarSeek)
        {
            if (onStain)
            {
                executingAstarSeek = false;
                return;
            }

            timeExecuting += Time.fixedDeltaTime;
            // Si el camino no es válido aborto
            if (astarSeekNodes.Count == 0)
            {
                executingAstarSeek = false;
                return;
            }

            // obtengo la información del tramo actual
            currentNode = astarSeekNodes[astarSeekNodeIndex];

            FixVelocityToCurrentNode(currentNode.position);


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


            float tdtimeExecuting = Mathf.Round(timeExecuting * 100f) / 100f;
            float tdsuposedTimeNow = Mathf.Round(suposedTimeNow * 100f) / 100f;

            //Debug.Log("" + tdtimeExecuting + " / " + tdsuposedTimeNow);

            // Compruebo si ya me toca pasar al siguiente nodo
            if (!(astarSeekTimeCounter < currentNode.time))
            {
                suposedTimeNow = currentNode.time;
                astarSeekNodeIndex++;
                if (astarSeekNodeIndex == astarSeekNodes.Count)
                {
                    suposedTimeNow = 0f;
                    timeExecuting = 0f;
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
        if (onStain)
            return;
        StartCoroutine(AStarRoutine());
    }


    IEnumerator AStarRoutine()
    {

        // Aquí es donde se calcula el timeBeforeJump
        timeBeforeJump = AIDirector.GetTimeBeforeJump(HumanizationVariationFactor) * timeVariationAI;

        aStarSolver.output = null;
        executingAstarSeek = false;
        pendingToStartAStarPipeline = false;

        // Si la eleección de target fue exitosa
        if (ChooseTarget())
        {
            // Generamos infomración para ejecutar el Astar a hacia ese target

            // Calculamos el camino
            yield return aStarSolver.AStar(transform.position, aStarGoal, timeBeforeJump);

            while(aStarSolver.timeSinceCalculationStarded < timeBeforeJump)
            {
                yield return null;
                aStarSolver.timeSinceCalculationStarded += Time.deltaTime;
            }
            
            // Si el caminno no fue encontrado
            if (aStarSolver.output == null)
            {

                // Avisamos a los logs
                //Debug.LogError("No path found.");

                // PERO:
                //at next frame, we'll try with another target!
                targetsToIgnore.Add(currentPathTargetObject);

                //TMP RESTAURAR:
                pendingToStartAStarPipeline = true;
                // TMP:
                //StartCoroutine(WaitAndRestartAstar(1f));

                

            }
            else // Si el camino es válido
            {
                // Iniciamos el seek al resultado del AStar
                StartAStartSeekPathResult(ref aStarSolver.output);
                targetsToIgnore = new List<PathTarget>();

            }

        }
        else
        {// Si no encontramos ningún target valido:

            // Indicamos al controlador que queremos reiniciar proceso de búsqueda de target
            // en el siguiente frame.

            // TMP RESTAURAR
            pendingToStartAStarPipeline = true;

            // TMP
            //StartCoroutine(WaitAndRestartAstar(1f));


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
            //Debug.LogError("No valid target found! We need to code some backup plan here.");
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
