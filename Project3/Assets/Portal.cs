using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal otherPortal;


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


        }
    }


}
