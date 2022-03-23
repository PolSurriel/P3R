using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public static Vector2 SmartSwap(bool swap, Vector2 outNormal, Vector2 toSwap)
    {

        if (!swap)
            return toSwap;

        var auxY = toSwap.y;
        toSwap.y = toSwap.x;
        toSwap.x = auxY;

        float dot = Vector2.Dot(toSwap, outNormal);

        if(dot < 0f)
        {
            toSwap *= -1f;
        }

        return toSwap;
    }


    public bool aiIgnore;
    public Portal otherPortal;

    [OnValueChanged("OnInverseXChanged")]
    public bool inverseX;
    [OnValueChanged("OnInverseYChanged")]
    public bool inverseY;
    [OnValueChanged("OnSwapValuesChanged")]
    public bool swapXY;
    [InfoBox("First, we invert velocity axis value, and then, we swap them", InfoMessageType.Warning)]

    public Vector2 normal;

    void OnInverseXChanged()
    {
        otherPortal.inverseX = inverseX;
    }
    
    void OnInverseYChanged()
    {
        otherPortal.inverseY = inverseY;
    }
    
    void OnSwapValuesChanged()
    {
        otherPortal.swapXY = swapXY;
    }

    AIController[] ais;

    public AIController.NativePortalInfo GeneratePortalNativeInfo() {

        AIController.NativePortalInfo result = new AIController.NativePortalInfo() {
            collisionInfo = CalculateLineCollisionInfo(),
            inverseX = inverseX,
            inverseY = inverseY,
            swapXY = swapXY,
            normal = normal,
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

    private void OnDrawGizmos()
    {
        var inVelocity = otherPortal.normal * -1f;
        inVelocity = Quaternion.AngleAxis(25f, Vector3.forward) * inVelocity;

        var outVelocity = inVelocity;

        if (inverseX) outVelocity.x *= -1f;
        if (inverseY) outVelocity.y *= -1f;

        outVelocity = SmartSwap(swapXY, normal, outVelocity);

        Debug.DrawLine(transform.position, (Vector2)transform.position + outVelocity, Color.red);
        Debug.DrawLine(transform.position, (Vector2)transform.position + inVelocity, Color.green);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere((Vector2)transform.position + inVelocity, 0.01f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position + outVelocity, 0.01f);

        // DESIRED OUTPUT IF NORMALS ARE OK (muy caro para usar este metodo pero bueno para mostrarse en gizmos)
        float angleBetweenNormals = Vector2.SignedAngle(normal, otherPortal.normal);
        Vector2 desiredOutput = Quaternion.AngleAxis(angleBetweenNormals, Vector3.forward) * inVelocity;    
        Debug.DrawLine(transform.position, (Vector2)transform.position + desiredOutput, Color.yellow);

        

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

            collision.GetComponent<Runner>().EnterOnAPortal();

            //collision.GetComponent<Runner>().EnterOnAPortal();

            if (ignoring.Contains(collision.gameObject))
                return;

            otherPortal.ignoring.Add(collision.gameObject);
            
            Vector3 localPos = collision.gameObject.transform.position - transform.position;

            if (swapXY)
            {
                var auxY = localPos.y;
                localPos.y = localPos.x;
                localPos.x = auxY;
            }

            Vector2 newPos = otherPortal.transform.position + localPos;

            StartCoroutine(StopIgnoringAt(0.5f, collision.gameObject));

            var rb = collision.gameObject.GetComponent<Rigidbody2D>();


            var vel = rb.velocity;

            if (inverseX)
            {
                vel.x = vel.x * -1f;
            }

            else if (inverseY)
            {
                vel.y = vel.y * -1f;
            }
            
            // we add a little position offset so player don't collision with walls
            newPos += otherPortal.normal * ((CircleCollider2D)collision).radius*2.2f;


            SmartSwap(swapXY, normal, vel);


            rb.velocity = vel;
            //Debug.LogError("Printed");
            Debug.DrawLine(newPos, newPos + vel.normalized * 100f, Color.blue, 10f);

            collision.gameObject.transform.position = newPos /* + vel.normalized * 0.5f*/;



        }
    }


}
