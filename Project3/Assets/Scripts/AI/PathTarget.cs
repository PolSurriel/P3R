using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathTarget : ComplexMonoBehaviour
{
    protected override string GetDocsLink() => "https://docs.google.com/document/d/1xOc4dZQYG8v7FqNdOv_oYREJqseOEOXbCb-CxMyNqqI/edit?usp=sharing";

    [Title("Evaluable position offsets")]
    [InfoBox("This offsets are applied in the pathfinding algorithm scope, NOT when the target is chosen.")]
    [OnValueChanged("OnRandomVerticalOffsetActivated")]
    public bool useRandomVerticalOffset;
    public float maxVerticalOffset;
    [OnValueChanged("OnRandomHorizontalOffsetActivated")]
    public bool useRandomHorizontalOffset;
    public float maxHorizontalOffset;

    [Title("Dot constrain")]
    [InfoBox("Defines if the target can be discarded by the dot product of the vector [Player to Target] and the setted [docContrai]. Unless the boolean forceDotConstraint is activated, the chooseTarget algorithm will interpret this as a preference, not as a requirement. The dotConstrainThreshold defines how much the dot constrain can reach.")]
    [LabelText("Activated")]
    public bool useDotConstrainToChoose = false;
    public bool forceDotConstraint;
    public float dotConstrainThreshold;
    public Vector2 dotConstrain;

    [Title("Fake position")]
    [InfoBox("Fake position for the choosing target algorithm, NOT THE PATHFINDING. If null, not used.")]
    public Transform fakePosition;

    [Title("Incision vector")]
    [InfoBox("Defines a required velocity direction the AIs need to consider in the pathfinding algorithm. The angle difference threshold depends on each AI and is defined out of this script. NOT used in the choose target algrithm.")]

    public bool useIncisionConstrain = true;
    public Vector2 incisionVector;

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




    void OnRandomVerticalOffsetActivated()
    {
        if (useRandomVerticalOffset)
            useRandomHorizontalOffset = false;
    }
    void OnRandomHorizontalOffsetActivated()
    {
        if (useRandomHorizontalOffset)
            useRandomVerticalOffset = false;
    }

    public Vector2 GetEvaluablePosition()
    {

        if (fakePosition == null)
            return transform.position;

        return fakePosition.position;
    }


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


        if (useRandomVerticalOffset)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * maxVerticalOffset * 0.5f, Color.red);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * maxVerticalOffset * 0.5f, Color.red);
        }

        if (useRandomHorizontalOffset)
        {
            Debug.DrawLine(transform.position, transform.position + Vector3.up * maxHorizontalOffset * 0.5f);
            Debug.DrawLine(transform.position, transform.position + Vector3.down * maxHorizontalOffset * 0.5f);
        }

    }
}
