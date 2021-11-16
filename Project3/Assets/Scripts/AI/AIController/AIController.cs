using DataStructures.PriorityQueue;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public partial class AIController : MonoBehaviour
{

    AstarGoal aStarGoal;




    public class AstarGoal
    {
        public bool useIncisionConstrain;
        public Vector2 position;
        public Vector2 incisionDirection;

        public AstarGoal(Vector2 position, Vector2 incisionDirection, bool useIncisionConstrain)
        {
            this.position = position;
            this.incisionDirection = incisionDirection;
            this.useIncisionConstrain = useIncisionConstrain;
        }


    }


    const float HIGHT_FLOAT = (float.MaxValue * 0.5f);
    const float CAPSULE_CAST_RADIUS = 0.2f;

    Runner runner;
    Rigidbody2D rb;
    JobyfablePrecalculatedPredictionSystem jumpPredictor;

    MapController mapController;


    const int DIRECTIONS_COUNT = 36;
    const int NUMBER_OF_PRECALCULATED_POINTS = 300;
    const int PRECALCULATED_POINTS_INCREMENT = 30;
    const float DELTA_DEGREE = 360f / (float)DIRECTIONS_COUNT;
    const float PRECALCULATION_DURATION = 1.5f;
    const float PRECALCULATION_DELTATIME = (float)PRECALCULATION_DURATION / (float)NUMBER_OF_PRECALCULATED_POINTS;
    const float PRECALCULATION_INCREMENT_DELTATIME = PRECALCULATION_DELTATIME * (float)PRECALCULATED_POINTS_INCREMENT;



    float predictionPlayerRadius;

    public AStarSolver aStarSolver;

    // Start is called before the first frame update
    void Start()
    {

        predictionPlayerRadius = GetComponent<CircleCollider2D>().radius * 1.15f;
        runner = GetComponent<Runner>();
        rb = GetComponent<Rigidbody2D>();
        var tmpJumpPredictor = new PrecalculatedPredictionSystem(rb, PRECALCULATION_DURATION, NUMBER_OF_PRECALCULATED_POINTS, DIRECTIONS_COUNT, runner.GetImpulseMagnitude());
        jumpPredictor = tmpJumpPredictor.GetJobyfable();

        aStarSolver = new AStarSolver(predictionPlayerRadius, jumpPredictor);


        mapController = FindObjectOfType<MapController>();
    }

    

    private void FixedUpdate()
    {

        AstarExecutionFixedUpdate();

        jumpPredictor.rb.velocity = rb.velocity;
        jumpPredictor.rb.position = rb.position;
    }


    public JobyfablePrecalculatedPredictionSystem GenerateVirtualJumpPredictor()
    {
        var jumpPredictor = new PrecalculatedPredictionSystem(GetComponent<Rigidbody2D>(), PRECALCULATION_DURATION, NUMBER_OF_PRECALCULATED_POINTS, DIRECTIONS_COUNT, GetComponent<Runner>().GetImpulseMagnitude());
        return jumpPredictor.GetJobyfable();
    
    }





    const float GOAL_MIN_DISTANCE = 0.9f;

    
    List<AStarNode> currentPath;


    AStarNode currentNode;

    

    bool executingAstarSeek = false;
    float astarSeekTimeCounter = 0f;
    bool astarSeeklastSecondJumpDone = true;
    bool astarSeekFirstIteration = true;
    int astarSeekNodeIndex;
    List<AStarNode> astarSeekNodes;

    void AstarExecutionFixedUpdate()
    {
        if (executingAstarSeek)
        {
            currentNode = astarSeekNodes[astarSeekNodeIndex];

            if (currentNode.secondJumpDone != astarSeeklastSecondJumpDone || astarSeekFirstIteration)
            {

                astarSeeklastSecondJumpDone = !astarSeeklastSecondJumpDone;
                astarSeekFirstIteration = false;
                runner.Jump(jumpPredictor.GetForce(currentNode.directionIndex));


            }


            astarSeekTimeCounter += Time.fixedDeltaTime;

            if ( ! (astarSeekTimeCounter < currentNode.time))
            {
                astarSeekNodeIndex++;
                if(astarSeekNodeIndex == astarSeekNodes.Count)
                {
                    executingAstarSeek = false;
                }
            }
        }
    }


    void StartAstartExecution(ref List<AStarNode> nodes)
    {
        astarSeekNodes = nodes;
        executingAstarSeek = true;
        astarSeeklastSecondJumpDone = true;
        astarSeekFirstIteration = true;
        astarSeekNodeIndex = 0;
        astarSeekTimeCounter = 0f;
    }
   
    const float VALID_TARGET_AREA_RADIUS = 7f;
    Vector2 lastTargetPos = Vector2.zero;
   

    bool DotConstrain(PathTarget target)
    {
        if (!target.useDotConstrainToChoose)
            return true;

        Vector2 toTarget = target.GetEvaluablePosition() - (Vector2)transform.position;
        toTarget.Normalize();

        float dot = Vector2.Dot(toTarget, target.dotConstrain);

        return dot < target.dotConstrainThreshold;
    }

    bool ChooseTarget(bool useDotConstrain = true)
    {

        // choose a target
        var targets = FindObjectsOfType<PathTarget>();

        var validTargets = new List<PathTarget>();

        foreach (var target in targets)
        {

            if (targetsToIgnore.Contains(target))
                continue;

            var targetPos = target.GetEvaluablePosition();

            if(lastTargetPos != targetPos)

            if ((!(targetPos.y < transform.position.y + GOAL_MIN_DISTANCE)) || onBackupPlanZone)
            {

                if (Vector2.Distance(targetPos, transform.position) < VALID_TARGET_AREA_RADIUS)
                {
                        if (!useDotConstrain || DotConstrain(target))
                        {
                            validTargets.Add(target);
                            continue;
                        }
                }
            }
            
        }

        if (validTargets.Count == 0)
        {
            if (useDotConstrain)
            {
                return ChooseTarget(false);

            }else
            {
                // no valid target found
                return false;
            }
        }

        currentPathTargetObject = validTargets[Random.Range(0, validTargets.Count)];

        aStarGoal = new AstarGoal(currentPathTargetObject.transform.position, currentPathTargetObject.incisionVector, currentPathTargetObject.useIncisionConstrain);
        lastTargetPos = aStarGoal.position;

        // success!
        return true;

    }


    bool start = false;
    bool pendingToStart = true;


    float timeToStartCount;
    float timeToStart = 1f;
    // Update is called once per frame
    void Update()
    {

        if (onATreadmill)
        {
            TreadmilleUpdate();
            return;
        }
        
        if (onStain)
        {
            StainUpdate();
            return;
        }

        
        if(timeToStartCount < timeToStart)
        {
            timeToStartCount += Time.deltaTime;
        }else {
            start = true;
        }

        timeToStartCount += Time.deltaTime;

        if (start && pendingToStart && !onAMolino)
        {
            StartAStarPipeline();
        }



    }

    PathTarget currentPathTargetObject;
    List<PathTarget> targetsToIgnore = new List<PathTarget>();

    void StartAStarPipeline()
    {
        executingAstarSeek = false;
        pendingToStart = false;

        if (ChooseTarget())
        {
            // Go to the target

            currentPath = aStarSolver.AStar(transform.position, aStarGoal, timeBeforeJump);
            if(currentPath == null)
            {
                //at next frame, we'll try with another target!
                targetsToIgnore.Add(currentPathTargetObject);
                pendingToStart = true;
                return;
            
            }else
            {
                StartAstartExecution(ref currentPath);
                //executingAstarRoutine = ExecuteAstar(currentPath);
                //StartCoroutine(executingAstarRoutine);
                targetsToIgnore = new List<PathTarget>();

            }


        }else
        {

            pendingToStart = true;
            targetsToIgnore = new List<PathTarget>();
            //Si el tilemap requiere esperar a un momento del tiempo en
            //concreto para saltar, el backup plan no debería ser generico.
            //de algun modo deberiamos tener cuidado con esto. Reiniciar
            //la busqueda de target en el momento apropiado vs ejecutar 
            //un comportamientor reactivo
            Debug.LogError("No valid target found! We need to code some backup plan here.");
        }
    }



    IEnumerator WaitAndRestartAstar(float time)
    {
        float t = 0;
        while ( t < time)
        {
            t += Time.deltaTime;
            yield return null;
        }

        pendingToStart = true;
    }


    float timeBeforeJump = 0.1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {


        if (collision.collider.tag == "floor")
        {
            lastTargetPos = Vector2.down;
            executingAstarSeek = false;
            StartCoroutine(WaitAndRestartAstar(timeBeforeJump));


        }
    }


    bool onBackupPlanZone = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "backupPlanZone")
        {
            onBackupPlanZone = true;
        }
        else if (collision.tag == "extraJumpZone")
        {
            executingAstarSeek = false;
            StartCoroutine(WaitAndRestartAstar(timeBeforeJump));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "backupPlanZone")
        {
            onBackupPlanZone = false;
        }
    }

}


