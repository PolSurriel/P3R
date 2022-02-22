using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Runner))]
public class PlayerController : MonoBehaviour
{

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

    // Start is called before the first frame update
    void Start()
    {

        runner = GetComponent<Runner>();
    }

    const float MAX_INPUT_LENGHT = 1f;


    Vector2 tmpLastInputPos;
    bool firstDragInputIteration = true;


    Vector2 inputVector;


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
            if (!firstDragInputIteration)
            {
                Vector2 deltaMove = currentInput - tmpLastInputPos;
                inputVector += deltaMove*0.005f;
            }

            tmpLastInputPos = Input.mousePosition;
            firstDragInputIteration = false;

            


            if (inputVector.magnitude > MAX_INPUT_LENGHT)
            {
                inputVector = inputVector.normalized * MAX_INPUT_LENGHT;
            }

            inputLR.SetPosition(1, inputVector*2f);

            
        }

    }
}
