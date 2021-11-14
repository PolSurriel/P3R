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





    public List<GameObject> ignoring = new List<GameObject>();

    IEnumerator StopIgnoringAt(float time, GameObject obj)
    {
        float t = 0f;
        do { yield return null; } while ((t += Time.deltaTime) < time);


        otherPortal.ignoring.Remove(obj);

    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {

            if (ignoring.Contains(collision.gameObject))
                return;

            otherPortal.ignoring.Add(collision.gameObject);
            
            Vector3 localPos = collision.gameObject.transform.position - transform.position; 
            collision.gameObject.transform.position = otherPortal.transform.position + localPos;

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


        }
    }


}
