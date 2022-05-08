using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreadmillAnimation : MonoBehaviour
{

    public bool goesup;

    Material mat;

    float currentOffset = -1f;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
    }


    // Update is called once per frame
    void Update()
    {
        mat.SetFloat("_OffsetUvY", currentOffset += Time.deltaTime * (goesup?-1f:1f));   
    }
}
