using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{


    public bool aiIgnore;
    public Portal otherPortal;

    [OnValueChanged("OnInverseXChanged")]
    public bool inverseX;
    [OnValueChanged("OnInverseYChanged")]
    public bool inverseY;

    public Vector2 normal;

    void OnInverseXChanged()
    {
        otherPortal.inverseX = inverseX;
    }
    
    void OnInverseYChanged()
    {
        otherPortal.inverseY = inverseY;
    }

    AIController[] ais;

    

    public AIController.NativePortalInfo GeneratePortalNativeInfo() {

        AIController.NativePortalInfo result = new AIController.NativePortalInfo() {
            collisionInfo = CalculateLineCollisionInfo(),
            inverseX = inverseX,
            inverseY = inverseY,
            otherPortalPosition = otherPortal.transform.position,
            portalPosition = transform.position

        };

        return result;
    }

    SurrealBoost.Types.Line CalculateLineCollisionInfo()
    {

        var collider = GetComponent<BoxCollider2D>();

        var halfSize = collider.bounds.size * 0.5f;
        Vector2 offset = transform.position;

        if (normal == Vector2.left)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(-halfSize.x, halfSize.y)+offset,
                pointB = new Vector2(-halfSize.x, -halfSize.y) + offset
            };

        }
        else if (normal == Vector2.right)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(halfSize.x, halfSize.y) + offset,
                pointB = new Vector2(halfSize.x, -halfSize.y) + offset
            };
        }
        else if (normal == Vector2.down)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(-halfSize.x, -halfSize.y) + offset,
                pointB = new Vector2(halfSize.x, -halfSize.y) + offset
            };
        }
        else if (normal == Vector2.up)
        {
            return new SurrealBoost.Types.Line()
            {
                pointA = new Vector2(-halfSize.x, halfSize.y) + offset,
                pointB = new Vector2(halfSize.x, halfSize.y) + offset
            };
        }

        return new SurrealBoost.Types.Line();
    }

    private void Start()
    {
        if(!aiIgnore)
            AIDirector.AddPortal(this);

    }

    private void OnDestroy()
    {
        if (!aiIgnore)
            AIDirector.RemovePortal(this);
    }
    private void Update()
    {
        ais = FindObjectsOfType<AIController>();


        if (aiIgnore)
            return;

        foreach (var ai in ais)
        {
            float dist = Vector2.Distance(transform.position, ai.transform.position);


            if (dist < ai.closestPortalDistance)
            {
                ai.closestPortalDistance = dist;
                ai.closestPortal = transform.position;
               
            }

        }
    }




    public List<GameObject> ignoring = new List<GameObject>();

    IEnumerator StopIgnoringAt(float time, GameObject obj)
    {
        float t = 0f;
        do { yield return null; } while ((t += Time.deltaTime) < time);


        otherPortal.ignoring.Remove(obj);

    }


    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.tag == "AI_Player")
        {

            //collision.GetComponent<Runner>().EnterOnAPortal();

            if (ignoring.Contains(collision.gameObject))
                return;

            otherPortal.ignoring.Add(collision.gameObject);
            
            Vector3 localPos = collision.gameObject.transform.position - transform.position; 
            Vector2 newPos = otherPortal.transform.position + localPos;

            StartCoroutine(StopIgnoringAt(0.5f, collision.gameObject));

            var rb = collision.gameObject.GetComponent<Rigidbody2D>();


            var vel = rb.velocity;

            if (inverseX)
            {
                vel.x = vel.x * -1f;
            }else
            {
                // we add a little position offset so player don't collision with walls
                newPos.x += otherPortal.normal.x * ((CircleCollider2D)collision).radius;
            }

            if (inverseY)
            {
                vel.y = vel.y * -1f;
            }else
            {
                // we add a little position offset so player don't collision with walls
                newPos.y += otherPortal.normal.y * ((CircleCollider2D)collision).radius;
            }

            rb.velocity = vel;

            collision.gameObject.transform.position = newPos /* + vel.normalized * 0.5f*/;



        }
    }


}
