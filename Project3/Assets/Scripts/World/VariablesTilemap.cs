using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariablesTilemap : MonoBehaviour
{
    [SerializeField] private float angleVariationTrigger;
    [SerializeField] private float timeVariationTrigger;
    [SerializeField] private int[] difficulties = { 0 };


    private void OnTriggerEnter2D(Collider2D other)
    {
        var ai = other.GetComponent<AIController>();

        if (ai != null)
            other.GetComponent<AIController>().SetErrorTriggerVariables(angleVariationTrigger, timeVariationTrigger);
    }
    public int[] GetDifficulties()
    {
        return difficulties;
    }
}
