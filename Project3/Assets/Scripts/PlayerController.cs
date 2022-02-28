using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Runner))]
public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    bool usingPrecalculatedUI = true;

    public AnimationCurve reduceInputMagnitude;

    bool pressingMouse;
    Vector2 mousePressFirstPos;

    Runner runner;

    public LineRenderer inputLR;

    public bool ignoreLevel = false;
    
    private void Awake()
    {
        // cutre pero es para la alpha

        return;
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

    void SetupInitialInputVector()
    {
        initialInputVector = Vector2.zero;

        if (usingPrecalculatedUI && !runner.wallup)
        {
            int its = 0;
            while (Vector2.Distance(FindEndPosWiht(initialInputVector), transform.position) > 0.1f)
            {
                if (its++ > 99999)
                {
                    Debug.LogError("infiniteLoop");
                    break;
                }

                initialInputVector += Vector2.up * 0.1f;
            }

        }
    }

    Vector2 initialInputVector;

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

        float totalTime = usingPrecalculatedUI && !runner.wallup ? uiSimulationTime : 0.6f;
        float dt = totalTime / (float)uipoints.Count;
        float itdt = itdt = dt / 10f;

        float magnitude = Mathf.Min(inputVector.magnitude, MAX_INPUT_LENGHT) / MAX_INPUT_LENGHT;
        magnitude = runner.jumpMagnitude * magnitude;
        Vector2 currentVelocity = inputVector.normalized * magnitude;

        float inputmag = (inputVector - initialInputVector).magnitude;
        float alpha = Mathf.Min(inputmag * 3f, 0.7f);

        foreach (var point in uipoints)
        {
            for (int i = 0; i < 10; i++)
            {
                if (usingPrecalculatedUI && !runner.wallup)
                {
                    currentVelocity += (Vector2)Physics.gravity * itdt;
                }
                currentPos += currentVelocity * itdt;
            }
            point.transform.position = currentPos;

            point.color = new Color(1f, 1f, 1f, alpha);
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

    Vector2 FindEndPosWiht(Vector2 inputVector)
    {

        Vector2 currentPos = transform.position;

        float totalTime = usingPrecalculatedUI && !runner.wallup ? uiSimulationTime : 0.6f;
        float dt = totalTime / (float)uipoints.Count;
        float itdt = itdt = dt / 10f;

        float magnitude = Mathf.Min(inputVector.magnitude, MAX_INPUT_LENGHT) / MAX_INPUT_LENGHT;
        magnitude = runner.jumpMagnitude * magnitude;
        Vector2 currentVelocity = inputVector.normalized * magnitude;

        foreach (var point in uipoints)
        {
            for (int i = 0; i < 10; i++)
            {
                if (usingPrecalculatedUI && !runner.wallup)
                {
                    currentVelocity += (Vector2)Physics.gravity * itdt;
                }
                currentPos += currentVelocity * itdt;
            }

        }

        return currentPos;
    }

    // Update is called once per frame
    void Update()
    {




        if (Input.GetMouseButtonUp(0))
        {

            Vector2 worldPosition1 = Camera.main.ScreenToWorldPoint(mousePressFirstPos);
            Vector2 worldPosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Vector2 dif = inputVector;
            float magnitude = Mathf.Min(dif.magnitude, MAX_INPUT_LENGHT);

            pressingMouse = false;
            runner.Jump(inputVector, magnitude / MAX_INPUT_LENGHT);


            HideCurveUI();
        }

        if (!pressingMouse && Input.GetMouseButtonDown(0))
        {
            firstDragInputIteration = true;
            pressingMouse = true;
            mousePressFirstPos = Input.mousePosition;

            inputVector = initialInputVector;


            tmpLastInputPos = Vector2.zero;

        }

        Vector2 currentInput = Input.mousePosition;

        if (pressingMouse)
        {
            UpdateCurveUI();

            if (!firstDragInputIteration)
            {
                Vector2 deltaMove = currentInput - tmpLastInputPos;
                inputVector += deltaMove*0.003f;
                //inputVector += deltaMove*0.005f;
            }

            tmpLastInputPos = Input.mousePosition;
            

            


            if (inputVector.magnitude > MAX_INPUT_LENGHT)
            {
                inputVector = inputVector.normalized * MAX_INPUT_LENGHT;
            }

            if (firstDragInputIteration)
            {
                SetupInitialInputVector();
                inputVector = initialInputVector;
                
            }

            firstDragInputIteration = false;

        }

    }
}
