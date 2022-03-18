using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class AIController: MonoBehaviour
{

    [HideInInspector]
    public float erraticBehaviourFactor; // aka EBF

    [HideInInspector]
    public float desiredPlayerEBFOffset;

    const float EBF_APROXIMATION_DURATION = 4f; // in seconds

    public void AproximateOwnEBFToPlayerEBF(float playerEBF)
    {
        float dif = playerEBF - erraticBehaviourFactor;
        erraticBehaviourFactor += dif * EBF_APROXIMATION_DURATION * Time.deltaTime;

        if (erraticBehaviourFactor > 1f) erraticBehaviourFactor = 1f;
        else if (erraticBehaviourFactor < 0f) erraticBehaviourFactor = 0f;
    }


}
