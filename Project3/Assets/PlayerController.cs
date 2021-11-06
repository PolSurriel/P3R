using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Runner))]
public class PlayerController : MonoBehaviour
{

    bool pressingMouse;
    Vector2 mousePressFirstPos;

    Runner runner;

    // Start is called before the first frame update
    void Start()
    {
        runner = GetComponent<Runner>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            
            pressingMouse = false;
            runner.Jump((Vector2)Input.mousePosition - mousePressFirstPos);
            
        }

        if (!pressingMouse && Input.GetMouseButtonDown(0))
        {
            pressingMouse = true;
            mousePressFirstPos = Input.mousePosition;

        }


        if (pressingMouse)
        {
            Vector2 worldPosition1 = Camera.main.ScreenToWorldPoint(mousePressFirstPos);
            Vector2 worldPosition2 = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Debug.DrawLine(worldPosition1, worldPosition2, Color.yellow);
        }
    }
}
