using DataStructures.PriorityQueue;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIController : MonoBehaviour
{
    // Portal attributes, modified from Portal.cs
    public bool usePortal;
    public Vector2 closestPortal;
    public float closestPortalDistance;


    Runner runner;
    Rigidbody2D rb;

    // Jump predictor
    JobyfablePrecalculatedPredictionSystem jumpPredictor;
    const int DIRECTIONS_COUNT = 36;
    const int NUMBER_OF_PRECALCULATED_POINTS = 300;
    const int PRECALCULATED_POINTS_INCREMENT = 30;
    const float DELTA_DEGREE = 360f / (float)DIRECTIONS_COUNT;
    const float PRECALCULATION_DURATION = 1.5f;
    const float PRECALCULATION_DELTATIME = (float)PRECALCULATION_DURATION / (float)NUMBER_OF_PRECALCULATED_POINTS;
    const float PRECALCULATION_INCREMENT_DELTATIME = PRECALCULATION_DELTATIME * (float)PRECALCULATED_POINTS_INCREMENT;

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

    }

    

    private void FixedUpdate()
    {

        AstarExecutionFixedUpdate();

        jumpPredictor.rb.velocity = rb.velocity;
        jumpPredictor.rb.position = rb.position;
    }

    
    bool controllingPlayer = false;
    bool pendingToStartAStarPipeline = true;

    float timeToStartCount;
    const float TIME_TO_START_CONTROLLING_PLAYER = 1f;
    float timeBeforeJump = 0.15f;

    void Update()
    {
        aStarSolver.movingObstaclesToHandle = FindObjectsOfType<MovingObstacle>();
        aStarSolver.rotatingObstaclesToHandle = FindObjectsOfType<RotatingObstacle>();


        if (Input.GetKeyUp(KeyCode.T))
        {
            Time.timeScale = Time.timeScale == 1f ? 0.2f : 1f;
        }

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


        if (timeToStartCount < TIME_TO_START_CONTROLLING_PLAYER)
        {
            timeToStartCount += Time.deltaTime;
        }
        else
        {
            controllingPlayer = true;
        }


        if (controllingPlayer && pendingToStartAStarPipeline && !onAMolino && !onStain)
        {
            StartAStarPipeline();
        }



    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "floor")
        {
            lastTargetPos = Vector2.down;

            if (executingAstarSeek)
            {
                if(Vector2.Distance(lastTargetPos, transform.position) > MIN_DIST_TO_CHOOSE_TARGET)
                {
                    lastTargetPos = Vector2.zero;
                }
            }

            executingAstarSeek = false;


            StartAStarPipeline();


        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "backupPlanZone")
        {
            onBackupPlanZone = true;
        }
        else if (collision.tag == "extraJumpZone")
        {
            executingAstarSeek = false;
            aStarSolver.output = null;
            StartAStarPipeline();
        }


    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "backupPlanZone")
        {
            onBackupPlanZone = false;
        }
    }

    private void LateUpdate()
    {

        if (closestPortalDistance <= AIController.VALID_TARGET_AREA_RADIUS)
        {
            usePortal = true;
        }
        closestPortalDistance = 999999f;

    }

}


