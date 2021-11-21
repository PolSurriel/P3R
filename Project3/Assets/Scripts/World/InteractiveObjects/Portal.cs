using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{

    public Portal otherPortal;

    [OnValueChanged("OnInverseXChanged")]
    public bool inverseX;
    [OnValueChanged("OnInverseYChanged")]
    public bool inverseY;

    void OnInverseXChanged()
    {
        otherPortal.inverseX = inverseX;
    }
    
    void OnInverseYChanged()
    {
        otherPortal.inverseY = inverseY;
    }

    AIController[] ais;

    private void Start()
    {
        ais = FindObjectsOfType<AIController>();

        foreach(var ai in ais)
        {
            float dist = Vector2.Distance(transform.position, ai.transform.position);

            if(dist < ai.closestPortalDistance)
            {
                ai.closestPortalDistance = dist;
                ai.closestPortal = transform.position;
                if(dist <= AIController.VALID_TARGET_AREA_RADIUS)
                {
                    ai.usePortal = true;
                }
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
        if (collision.tag == "Player")
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
            }

            if (inverseY)
            {
                vel.y = vel.y * -1f;
            }

            rb.velocity = vel;

            collision.gameObject.transform.position = newPos /* + vel.normalized * 0.5f*/;



        }
    }


}
