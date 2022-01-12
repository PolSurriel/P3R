using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesTilemap : MonoBehaviour
{
    [SerializeField] private float angleVariationTrigger;
    [SerializeField] private float timeVariationTrigger;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            other.GetComponent<AIController>().SetErrorTriggerVariables(angleVariationTrigger, timeVariationTrigger);
    }

}
