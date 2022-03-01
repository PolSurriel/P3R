using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerVFXController : MonoBehaviour
{
    const float DEFAULT_TIME_DESTROY_PARTICLE = 1f;
    IEnumerator DestroyObjectIn(float seconds, GameObject obj)
    {
        float t = 0;

        do
        {
            yield return null;

        } while ((t += Time.deltaTime) < seconds);

        Destroy(obj);
    }

    public GameObject wallCollisionParticle;

    public void OnCollisionWithWall(Vector2 contactPoint)
    {
        var obj = Instantiate(wallCollisionParticle);
        obj.transform.position = contactPoint;
        StartCoroutine(DestroyObjectIn(DEFAULT_TIME_DESTROY_PARTICLE, obj));
    }

}
