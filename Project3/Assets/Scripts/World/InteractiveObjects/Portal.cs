using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Portal : ComplexMonoBehaviour
{
    protected override string GetDocsLink() => "https://docs.google.com/document/d/1TtdxIMOyhyy9kL0Gu0x5pjhhjCxQG3gG2YF3BIHFiSE/edit?usp=sharing";

    public static Vector2 SmartSwap(bool swap, Vector2 outNormal, Vector2 toSwap)
    {

        if (!swap)
            return toSwap;

        var auxY = toSwap.y;
        toSwap.y = toSwap.x;
        toSwap.x = auxY;

        float dot = Vector2.Dot(toSwap, outNormal);

        if (dot < 0f)
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
            otherPortalNormal = otherPortal.normal,
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

    float gizmosSelectedTimeCounter = 0f;
    bool gizmosSelectedTimeIncresePositive = true;
    private void OnDrawGizmosSelected()
    {


        gizmosSelectedTimeCounter += 0.1f* Time.deltaTime * (gizmosSelectedTimeIncresePositive ? 1f:-1f);
        otherPortal.gizmosSelectedTimeCounter = gizmosSelectedTimeCounter;

        if(
            (gizmosSelectedTimeIncresePositive && gizmosSelectedTimeCounter > 1f)
            ||
            (!gizmosSelectedTimeIncresePositive && gizmosSelectedTimeCounter < -1f))
        {
            gizmosSelectedTimeIncresePositive = !gizmosSelectedTimeIncresePositive;
            otherPortal.gizmosSelectedTimeIncresePositive = gizmosSelectedTimeIncresePositive;
        }

        DrawPortalsEnterExitInfo();
        otherPortal.DrawPortalsEnterExitInfo();
    }


    public void DrawPortalsEnterExitInfo()
    {
        if (!GizmosCustomMenu.instance.portalsConfiguration)
            return;

        SurrealBoost.GizmosTools.Draw2D.ArrowedLine(transform.position, (Vector2)transform.position + normal*0.7f, 0.02f, Color.white);
        #if UNITY_EDITOR
            Handles.Label((Vector2)transform.position + normal * 0.7f, "normal");
        #endif

        var inVelocity = otherPortal.normal * -1f;
        inVelocity = Quaternion.AngleAxis(25f * gizmosSelectedTimeCounter, Vector3.forward) * inVelocity;

        var outVelocity = inVelocity;

        if (inverseX) outVelocity.x *= -1f;
        if (inverseY) outVelocity.y *= -1f;

        outVelocity = SmartSwap(swapXY, normal, outVelocity);

        const float lineWidth = 0.05f;

        SurrealBoost.GizmosTools.Draw2D.ArrowedLine(transform.position, (Vector2)transform.position + outVelocity, lineWidth, Color.red);
        SurrealBoost.GizmosTools.Draw2D.ArrowedLine(transform.position, (Vector2)transform.position + inVelocity, lineWidth, Color.green);

#if UNITY_EDITOR
        Handles.color = Color.green;
        Handles.Label((Vector2)transform.position + inVelocity, "INPUT VELOCITY");
        Handles.color = Color.red;
        Handles.Label((Vector2)transform.position + outVelocity, "OUTPUT VELOCITY");
#endif


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


    public static Vector2 GetCrossingPortalPosition( Vector2 characterPosition, Vector2 portalPosition, Vector2 otherPortalPosition, Vector2 otherPortalNormal, bool portalSwapXY, float characterRadius)
    {
        Vector2 localPosRelativeToPortal = characterPosition - portalPosition;
        localPosRelativeToPortal = SmartSwap(portalSwapXY, otherPortalNormal, localPosRelativeToPortal);

        return otherPortalPosition + localPosRelativeToPortal + otherPortalNormal * characterRadius;
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
            localPos = SmartSwap(swapXY, otherPortal.normal, localPos);

            float dot = Vector2.Dot(localPos.normalized, otherPortal.normal);

            if (dot < 0f)
            {
                localPos *= -1f;
            }



            Vector2 newPos = otherPortal.transform.position + localPos;

            StartCoroutine(StopIgnoringAt(0.5f, collision.gameObject));

            var rb = collision.gameObject.GetComponent<Rigidbody2D>();


            var vel = rb.velocity;

            if (inverseX)
                vel.x = vel.x * -1f;

            if (inverseY)
                vel.y = vel.y * -1f;

            
            vel = SmartSwap(swapXY, otherPortal.normal, vel);
            dot = Vector2.Dot(vel, otherPortal.normal);

            if (dot < 0f)
            {
                vel *= -1f;
            }


            rb.velocity = vel;
            //Debug.DrawLine(newPos, newPos+ otherPortal.normal * ((CircleCollider2D)collision).radius, Color.green, 10f);
            //Debug.DrawLine(newPos, newPos+ otherPortal.normal * ((CircleCollider2D)collision).radius * 0.7f, Color.red, 10f);

#if UNITY_EDITOR
            if (GizmosCustomMenu.instance.portalsRuntimeInfo)
            {
                
                Debug.DrawLine(collision.gameObject.transform.position, newPos, Color.green, 99999f);
                Debug.DrawLine(newPos, newPos + otherPortal.normal * ((CircleCollider2D)collision).radius * 1.1f, Color.red, 99999f);

            }
#endif


            collision.gameObject.transform.position = newPos  + otherPortal.normal * ((CircleCollider2D)collision).radius;
#if UNITY_EDITOR
            if (GizmosCustomMenu.instance.portalsRuntimeInfo)
                Debug.DrawLine(collision.gameObject.transform.position, (Vector2)collision.gameObject.transform.position + vel, Color.blue, 99999f);
#endif
        }
    }


}
