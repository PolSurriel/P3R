using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Runner))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public bool usingPrecalculatedUI = true;

    public AnimationCurve reduceInputMagnitude;

    bool pressingMouse;
    Vector2 mousePressFirstPos;

    Runner runner;

    public LineRenderer inputLR;

    public bool ignoreLevel = false;
    
    private void Awake()
    {
        // cutre pero es para la alpha

       
        
        if(GameInfo.instance != null)
        {
            if(GameInfo.instance.levelID == 2)
            {
                this.enabled = false;
                Destroy(this);
            }

            else if (!ignoreLevel && GameInfo.instance.levelID == 3)
            {
                this.enabled = false;
                Destroy(this);

            }
        }
    }

    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        //precalculator = new ClassicPredictionSystem(rb, true);

        float scale = 0.07f;

        for (int i = 0; i < inputLR.transform.childCount; i++)
        {
            var toAdd = inputLR.transform.GetChild(i).GetComponent<SpriteRenderer>();

            toAdd.enabled = false;
            toAdd.transform.localScale = new Vector3(scale, scale, 1f);
            scale -= 0.005f;
            uipoints.Add(toAdd);
        }

        uipoints.Reverse();

        runner = GetComponent<Runner>();
    }

    const float MAX_INPUT_LENGHT = 1f;


    Vector2 tmpLastInputPos;
    bool firstDragInputIteration = true;


    Vector2 inputVector;

    List<SpriteRenderer> uipoints = new List<SpriteRenderer>();

    ClassicPredictionSystem precalculator;

    [HideInInspector]
    public float uiSimulationTime = 0.8f;
    void UpdateCurveUI()
    {

        Vector2 currentPos = transform.position;

        float totalTime = uiSimulationTime;
        float dt = totalTime / (float)uipoints.Count;
        float itdt = itdt = dt / 10f;

        float magnitude = Mathf.Min(inputVector.magnitude, MAX_INPUT_LENGHT) / MAX_INPUT_LENGHT;
        magnitude = runner.jumpMagnitude * magnitude;
        Vector2 currentVelocity = inputVector.normalized * magnitude;
      
        foreach(var point in uipoints)
        {
            for (int i = 0; i < 10; i++)
            {
                currentVelocity += (Vector2)Physics.gravity * itdt;
                currentPos += currentVelocity * itdt;
            }
            point.transform.position = currentPos;

            point.enabled = true;
        }
    }

    void HideCurveUI()
    {
        foreach (var point in uipoints)
        {
            point.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {

      

        if (Input.GetMouseButtonUp(0))
        {

            GetComponent<AIController>().enabled = false;

            Vector2 worldPosition1 = Camera.main.ScreenToWorldPoint(mousePressFirstPos);
            Vector2 worldPosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 dif = inputVector;
            float magnitude = Mathf.Min(dif.magnitude, MAX_INPUT_LENGHT);

            pressingMouse = false;
            runner.Jump(inputVector, magnitude / MAX_INPUT_LENGHT);
            inputLR.SetPosition(1, Vector2.zero);

            HideCurveUI();
        }

        if (!pressingMouse && Input.GetMouseButtonDown(0))
        {
            firstDragInputIteration = true;
            pressingMouse = true;
            mousePressFirstPos = Input.mousePosition;
            inputVector = Vector2.zero;
            
        }

        Vector2 currentInput = Input.mousePosition;

        if (pressingMouse)
        {
            if(usingPrecalculatedUI)
                UpdateCurveUI();

            if (!firstDragInputIteration)
            {
                Vector2 deltaMove = currentInput - tmpLastInputPos;
                inputVector += deltaMove*0.003f;
                //inputVector += deltaMove*0.005f;
            }

            tmpLastInputPos = Input.mousePosition;
            firstDragInputIteration = false;

            


            if (inputVector.magnitude > MAX_INPUT_LENGHT)
            {
                inputVector = inputVector.normalized * MAX_INPUT_LENGHT;
            }

            if(!usingPrecalculatedUI)
                inputLR.SetPosition(1, inputVector*2f);

            
        }

    }
}
