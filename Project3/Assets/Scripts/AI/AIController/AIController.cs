using DataStructures.PriorityQueue;
using Sirenix.OdinInspector;
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

    //Variables
    private float angleVariationAI;
    private float timeVariationAI;

    
    [Button]
    void DebugStartAstarPipeline()
    {
        lastTargetPos = Vector2.zero;
        executingAstarSeek = false;
        aStarSolver.output = null;
        StartAStarPipeline();
    }

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


    private void Awake()
    {
        if (GameInfo.instance != null)
        {
            if (GameInfo.instance.levelID == 1)
            {
                this.enabled = false;
                Destroy(this);
                
            }
        }
    }

    

    // Start is called before the first frame update
    void Start()
    {
        debugBG =  transform.GetChild(3);
        debugTestMesh = transform.GetChild(2).GetComponent<TextMesh>();

        if (!this.enabled)
            return;

        predictionPlayerRadius = GetComponent<CircleCollider2D>().radius * 1.15f;
        realPlayerRadius = GetComponent<CircleCollider2D>().radius;
        runner = GetComponent<Runner>();
        rb = GetComponent<Rigidbody2D>();
        var tmpJumpPredictor = new PrecalculatedPredictionSystem(rb, PRECALCULATION_DURATION, NUMBER_OF_PRECALCULATED_POINTS, DIRECTIONS_COUNT, runner.GetImpulseMagnitude());
        jumpPredictor = tmpJumpPredictor.GetJobyfable();

        aStarSolver = new AStarSolver(predictionPlayerRadius, realPlayerRadius, jumpPredictor);


    }

    private void OnDestroy()
    {
        try
        {
            jumpPredictor.precalculatedDirections.Dispose();
            
        }catch(System.InvalidOperationException e) { }

        if(aStarSolver != null)
            aStarSolver.OnDestroy();
    }

    public void OnMatchStarts()
    {
        //StartAStarPipeline();
    }

    private void FixedUpdate()
    {
        if (!StartMatchCountDown.matchStarted) return;


        AstarExecutionFixedUpdate();

        jumpPredictor.rb.velocity = rb.velocity;
        jumpPredictor.rb.position = rb.position;
    }

    
    bool controllingPlayer = false;
    bool pendingToStartAStarPipeline = true;

    float timeToStartCount;
    const float TIME_TO_START_CONTROLLING_PLAYER = 1f;
    float timeBeforeJump = 0.15f;

    float timeStuckedCount = 0f;
    const float MAX_TIME_STUCKED = 4f;
    Vector2 lastStuckedPosition;
    

    void Update()
    {
        if (!StartMatchCountDown.matchStarted) return;

        if(timeStuckedCount > MAX_TIME_STUCKED && aStarSolver.timeSinceCalculationStarded >= timeBeforeJump + MAX_TIME_STUCKED)
        {
            Debug.Log("stucked");
            runner.jumpCounter = 0;
            timeStuckedCount = 0f;
            executingAstarSeek = false;
            aStarSolver.output = null;
            StartAStarPipeline();
        }

        
        if(lastStuckedPosition == (Vector2)transform.position || (onATreadmill && lastStuckedPosition.x == transform.position.x))
        {
            if(onStain)
                timeStuckedCount += Time.deltaTime*0.5f;
            else
                timeStuckedCount += Time.deltaTime;
        }else
        {
            lastStuckedPosition = transform.position;
            timeStuckedCount = 0f;
        }



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


        if (pendingToStartAStarPipeline && !onAMolino && !onStain)
        {
            StartAStarPipeline();
        }



    }

    void OnCollisionWithFloor()
    {
        if (executingAstarSeek)
        {
            if (Vector2.Distance(lastTargetPos, transform.position) > MIN_DIST_TO_CHOOSE_TARGET)
            {
                lastTargetPos = Vector2.zero;
                lastTargetLandedPosition = Vector2.zero;
            }
            else
            {
                lastTargetLandedPosition = lastTargetPos;
            }
        }

        lastTargetPos = Vector2.down;
        executingAstarSeek = false;


        StartAStarPipeline();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!StartMatchCountDown.matchStarted)
            return;

        if (collision.collider.tag == "floor")
        {

            OnCollisionWithFloor();
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!StartMatchCountDown.matchStarted)
            return;


        if (collision.tag == "backupPlanZone")
        {
            onBackupPlanZone = true;
        }
        else if (collision.tag == "extraJumpZone" && !collision.GetComponent<ExtraJumpZone>().ignoring.Contains(runner))
        {
            executingAstarSeek = false;
            aStarSolver.output = null;
            StartAStarPipeline();
        }

        if(collision.tag == "transitionLine")
        {
            OnCollisionWithFloor();
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

    public void SetErrorTriggerVariables(float _angle, float _time )
    {
        angleVariationAI = _angle;
        timeVariationAI = _time;
    }

}


