using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wiggle : MonoBehaviour
{


    public AnimationCurve yCurve;
    public AnimationCurve xCurve;
    public float xMovementMagnitude;
    public float yMovementMagnitude;
    public float duration;

    float timeCounter = 0f;

    // Update is called once per frame
    void Update()
    {
        timeCounter = (timeCounter + Time.deltaTime) % duration;

        float t = timeCounter / duration;

        transform.localPosition = new Vector3(
            xCurve.Evaluate(t) * xMovementMagnitude,
            yCurve.Evaluate(t) * yMovementMagnitude,
            0
            );
    
    }
}
