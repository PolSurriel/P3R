using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTarget : MonoBehaviour
{

    public bool forceDotConstraint;
    public bool useDotConstrainToChoose = false;
    public float dotConstrainThreshold;
    public Vector2 dotConstrain;

    public Transform fakePosition;

    public Vector2 GetEvaluablePosition()
    {

        if (fakePosition == null)
            return transform.position;

        return fakePosition.position;
    }

    public bool useIncisionConstrain = true;

    [Button("SetLeft")]
    public void SetLeft()
    {
        incisionVector = Vector2.left;
        dotConstrain.Normalize();
    }
    [Button("SetRight")]
    public void SetRight()
    {
        incisionVector = Vector2.right;
    }
    
    [Button("SetDown")]
    public void SetDown()
    {
        incisionVector = Vector2.down;
    }
    
    [Button("SetUp")]
    public void SetUp()
    {
        incisionVector = Vector2.up;
    }

    public Vector2 incisionVector;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {

        const float RADIUS = 1f;

        //Handles.color = Color.yellow;
        //Handles.DrawWireDisc(transform.position, transform.forward, RADIUS);

        Debug.DrawLine((Vector2)transform.position + Vector2.left* RADIUS, (Vector2)transform.position + Vector2.right* RADIUS);
        Debug.DrawLine((Vector2)transform.position + Vector2.up* RADIUS, (Vector2)transform.position + Vector2.down* RADIUS);


    }
}
