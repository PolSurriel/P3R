using SurrealBoost.Types;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionLine : MonoBehaviour
{

    public bool fillsAllXAxis;

    public static List<TransitionLine> lines = new List<TransitionLine>();

    float xA;
    float xB;
    float y;

    [HideInInspector]
    public Vector2 pointA;
    [HideInInspector]
    public Vector2 pointB;

    [HideInInspector]
    public Line line;

    // Start is called before the first frame update
    void Start()
    {
        var collider = GetComponent<BoxCollider2D>();

        xA = collider.bounds.min.x;
        xB = collider.bounds.max.x;
        y = collider.bounds.max.y;

        pointA = new Vector2(xA, y);
        pointB = new Vector2(xB, y);

        line = new Line();
        line.pointA = pointA;
        line.pointB = pointB;


        lines.Add(this);
    }


    private void OnDrawGizmos()
    {
        Debug.DrawLine(new Vector2(xA, y), new Vector2(xB, y), Color.red);
    }

    private void OnDestroy()
    {
        lines.Remove(this);
    }
}
