using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : MonoBehaviour
{

    const float TIME_TO_BE_DESTROYED = 3f;

    float tCount = 0f;

    private void Update()
    {
        tCount += Time.deltaTime;

        if(tCount >= TIME_TO_BE_DESTROYED)
        {
            Destroy(gameObject);
        }
    }

}
