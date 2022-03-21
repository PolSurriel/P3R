using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReboundSurface : MonoBehaviour
{

    public bool inverseX;
    public bool inverseY;

    Vector2 senseVector;

    public Vector2 normal;

    BoxCollider2D collider;
    public SurrealBoost.Types.Line collisionInfo;

    SurrealBoost.Types.Line CalculateLineCollisionInfo()
    {
        
        if(normal == Vector2.left)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = collider.bounds.min,
                pointB = new Vector2(collider.bounds.min.x, collider.bounds.max.y)
            };

        }
        else if (normal == Vector2.right)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(collider.bounds.max.x, collider.bounds.min.y),
                pointB = new Vector2(collider.bounds.max.x, collider.bounds.max.y)
            };
        }
        else if (normal == Vector2.down)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(collider.bounds.max.x, collider.bounds.min.y),
                pointB = new Vector2(collider.bounds.min.x, collider.bounds.min.y)
            };
        }
        else if (normal == Vector2.up)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(collider.bounds.max.x, collider.bounds.max.y),
                pointB = new Vector2(collider.bounds.min.x, collider.bounds.max.y)
            };
        }

        return new SurrealBoost.Types.Line();
    }

    private void Start()
    {
        senseVector = Vector2.one;
        if (inverseX) senseVector.x *= -1f;
        if (inverseY) senseVector.y *= -1f;

        collider = GetComponent<BoxCollider2D>();
        collisionInfo = CalculateLineCollisionInfo();
        AIDirector.AddReboundSurface(this);
    }


    private void OnDestroy()
    {
        AIDirector.RemoveReboundSurface(this);
    }

    float maxUpMagnitude = 16f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var runner = collision.collider.GetComponent<Runner>();

        runner.rb.velocity = (runner.lastVelocity * senseVector + Vector2.up* Mathf.Min(runner.lastVelocity.magnitude*2.5f, maxUpMagnitude)).normalized * runner.lastVelocity.magnitude;
    }

}
