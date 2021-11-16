using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Molino : MonoBehaviour
{

    [OnValueChanged("OnClockWiseChange")]
    public bool clockWise;
    [OnValueChanged("OnRotationSpeedChange")]
    public float rotationSpeed;
    [OnValueChanged("OnEsgeAvoidanceRadiuseChange")]
    public float edgeAvoidanceRadius;

    void OnClockWiseChange()
    {
        foreach(var edge in edges)
        {
            edge.clockwiseDirection = clockWise;
        }
    }

    void OnRotationSpeedChange()
    {
        foreach (var edge in edges)
        {
            edge.rotationSpeed = rotationSpeed;
        }
    }

    void OnEsgeAvoidanceRadiuseChange()
    {
        foreach (var edge in edges)
        {
            edge.avoidCollider.GetComponent<CircleCollider2D>().radius = edgeAvoidanceRadius;
        }
    }

    public List<RotatingObstacle> edges = new List<RotatingObstacle>();

    private void Start()
    {
        if (clockWise)
        {
            rotationSpeed *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0f,0f,1f) * rotationSpeed * Time.deltaTime, Space.World);
    }
}
