using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacle : MonoBehaviour
{
    public Transform obstacle;

    public bool clockwiseDirection;
    public float rotationSpeed = 100f;
    public Transform avoidCollider;
    public Transform rotateAvoidCollider;


    // Start is called before the first frame update
    void Start()
    {
        if (clockwiseDirection)
            rotationSpeed *= -1f;
    }

    public void UpdateAvoidAstarInfo(float time)
    {
        avoidCollider.position = GetFuturePosition(time);

        if(rotateAvoidCollider != null)
        {
            rotateAvoidCollider.rotation = transform.rotation;
            rotateAvoidCollider.Rotate(Vector3.forward * rotationSpeed * time);

        }
    }

    private void Update()
    {
        float deltaRotation = rotationSpeed * Time.deltaTime;

        transform.Rotate(Vector3.forward  * deltaRotation);
        avoidCollider.transform.position = Vector3.down * 99999f;

        if(rotateAvoidCollider != null)
            rotateAvoidCollider.transform.position = Vector3.down * 99999f;

        
    }



    public Vector2 GetFuturePosition(float time)
    {
        var prevRotation = transform.rotation;

        transform.Rotate(Vector3.forward * rotationSpeed * time);

        Vector2 result = obstacle.position;

        transform.rotation = prevRotation;

        return result;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(transform.position, obstacle.position);
        Gizmos.DrawWireSphere(obstacle.position, 0.3f);

    }
}
