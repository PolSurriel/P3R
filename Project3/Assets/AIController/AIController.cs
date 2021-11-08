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


    const int DIRECTIONS_COUNT = 35;
    const int NUMBER_OF_PRECALCULATED_POINTS = 300;
    const int PRECALCULATED_POINTS_INCREMENT = 30;
    const float DELTA_DEGREE = 360f / (float)DIRECTIONS_COUNT;
    const float PRECALCULATION_DURATION = 1.5f;
    const float PRECALCULATION_DELTATIME = (float)PRECALCULATION_DURATION / (float)NUMBER_OF_PRECALCULATED_POINTS;
    const float PRECALCULATION_INCREMENT_DELTATIME = PRECALCULATION_DURATION * PRECALCULATED_POINTS_INCREMENT;



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

    
    IEnumerator executingAstarRoutine;

    IEnumerator ExecuteAstar(List<AStarNode> nodes)
    {

        bool lastSecondJumpDone = true;
        bool first = true;
        foreach(var node in nodes)
        {
            currentNode = node;

            if (node.secondJumpDone != lastSecondJumpDone || first)
            {

                lastSecondJumpDone = !lastSecondJumpDone;
                first = false;
                runner.Jump(jumpPredictor.GetForce(node.directionIndex));

            }

            //do
            //{
            //    timeCounter += Time.deltaTime;
            //    yield return null;

            //} while (timeCounter < node.time);
            float time = 0f;
            const float MAX_TIME_SEEKING_NEXTNODE = 1f;

            float distanceToNext;
            do
            {
                distanceToNext = Vector2.Distance(node.position, transform.position);
                yield return null;
                time += Time.deltaTime;
                if(time > MAX_TIME_SEEKING_NEXTNODE)
                {
                    lastTargetPos = Vector2.zero;
                    StartCoroutine(WaitAndRestartAstar(0f));
                    StopCoroutine(executingAstarRoutine);
                    yield return null;
                }
              

            } while (distanceToNext > 0.15f);



        }

        //pendingToStart = true;


    }


    const float VALID_TARGET_AREA_RADIUS = 7f;
    Vector2 lastTargetPos = Vector2.zero;
    
    AINode FindCurrentNode()
    {

        float localY = transform.position.y - mapController.nodeZeroPosition.y;

        int myPositioniIndex = (int)(localY / mapController.nodeDistance);

        if(myPositioniIndex > 0)
            myPositioniIndex--; 

        AINode currenNode = null;

        float shortestDistance = 99999999f;

        for (int i = myPositioniIndex; i < myPositioniIndex + 3; i++)
        {
            if (i < mapController.nodesGlobalMatrix.Count && i >= 0)
            {
                foreach (var node in mapController.nodesGlobalMatrix[i].list)
                {
                    float dist = Vector2.Distance(transform.position, node.GetWorldPosition(node.tilemapWorldPos));

                    if (dist < shortestDistance)
                    {
                        shortestDistance = dist;
                        currenNode = node;
                    }

                }

            }
        }

        return currenNode;

    }

    void ChooseTarget()
    {

        // choose a target
        var targets = FindObjectsOfType<PathTarget>();

        var validTargets = new List<PathTarget>();

        foreach (var target in targets)
        {
            
            if(lastTargetPos != (Vector2)target.transform.position)

            if (! (target.transform.position.y < transform.position.y + GOAL_MIN_DISTANCE))
            {

                if (Vector2.Distance(target.transform.position, transform.position) < VALID_TARGET_AREA_RADIUS)
                {
                    validTargets.Add(target);
                    continue;
                }
            }
            
        }

        if (validTargets.Count == 0)
        {
            Debug.LogError("No valid target found");
            return;
        }

        var finalTarget = validTargets[Random.Range(0, validTargets.Count)];

        aStarGoal = new AstarGoal(finalTarget.transform.position, finalTarget.incisionVector, finalTarget.useIncisionConstrain);
        lastTargetPos = aStarGoal.position;


    }


    bool start = false;
    bool pendingToStart = true;


    float timeToStartCount;
    float timeToStart = 1f;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.R))
        {
            pendingToStart = true;
            lastTargetPos = Vector2.zero;
        }

        if(timeToStartCount < timeToStart)
        {
            timeToStartCount += Time.deltaTime;
        }else {
            start = true;
        }

        timeToStartCount += Time.deltaTime;

        if (Input.GetKeyUp(KeyCode.E))
        {
            start = true;

            //for (int i = 0; i < 10; i++)
            //{
            //    ChooseTarget();
            //    Debug.DrawLine(transform.position, aStarGoal.position, Color.red, 120f);
            //    transform.position = aStarGoal.position;

            //    Debug.Log("" + i + ": " + aStarGoal.position.x + ", " + aStarGoal.position.y);
            //}



        }



        if (start && pendingToStart)
        {
            pendingToStart = false;

            ChooseTarget();

            // Go to the target
            
            currentPath = aStarSolver.AStar(transform.position, aStarGoal, timeBeforeJump);
            executingAstarRoutine = ExecuteAstar(currentPath);
            StartCoroutine(executingAstarRoutine);

            //rb.gravityScale = 0f;

        }

        if(start && !pendingToStart)
        {
            //Vector2 direction = (aStarGoal.position - transform.position).normalized;
            //Vector2 nextP = transform.position;
            //nextP += direction * Time.deltaTime * 10f;

            //transform.position= nextP;

            if(Vector2.Distance(aStarGoal.position, transform.position) < 0.4f){
                //pendingToStart = !pendingToStart;
            }
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
            if(executingAstarRoutine != null)
            {
                StopCoroutine(executingAstarRoutine);
            }
            StartCoroutine(WaitAndRestartAstar(timeBeforeJump));


        }
    }


    


}


