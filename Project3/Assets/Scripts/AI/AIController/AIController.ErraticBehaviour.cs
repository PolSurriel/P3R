using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public partial class AIController: MonoBehaviour
{

    /*
        Doc:
    https://media.discordapp.net/attachments/905760062293811221/954412884895617044/unknown.png?width=1467&height=1467

     */

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
