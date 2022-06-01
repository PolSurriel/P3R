using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerVFXController : MonoBehaviour
{
    const float DEFAULT_TIME_DESTROY_PARTICLE = 1f;

    public GameObject wallCollisionParticle;

    public void OnCollisionWithWall(Vector2 contactPoint)
    {
        var obj = Instantiate(wallCollisionParticle);
        obj.transform.position = contactPoint;
        Destroy(obj, DEFAULT_TIME_DESTROY_PARTICLE);
    }

}
