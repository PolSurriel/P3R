using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public partial class AIDirector: MonoBehaviour
{

    public AnimationCurve minWaitTimeBeforeJump;
    public AnimationCurve maxWaitTimeBeforeJump;

    public AnimationCurve minJumpVectorDeviation;
    public AnimationCurve maxJumpVectorDeviation;


    public AnimationCurve probabilityToApplyJumpDeviationScalar;
    public float probabilityToApplyJumpDeviation = 20f;

    [TitleGroup("---- GlobalTilemapConfiguration ----")]
    public bool clampMaxWaitTime;
    public float maxWaitTimeClamp;

    public bool clampMinWaitTime;
    public float minWaitTimeClamp;

    public bool clampMaxAngleDeviation;
    public float maxAngleDeviationClamp;

    public bool clampMinAngleDeviation;
    public float minAngleDeviationClamp;

    [Button("Apply clamp")]
    void ApplyClamp()
    {

        clampMaxWaitTime = false;
        clampMinWaitTime = false;
        clampMaxAngleDeviation = false;
        clampMinAngleDeviation = false;

    }



    public static float GetTimeBeforeJump(float erraticBehaviourFactor)
    {
        float min = instance.minWaitTimeBeforeJump.Evaluate(erraticBehaviourFactor);
        float max = instance.maxWaitTimeBeforeJump.Evaluate(erraticBehaviourFactor);


        return UnityEngine.Random.Range(min, max);
    }

    public static bool ApplyVectorDeviation(float erraticBehaviourFactor)
    {
        return Random.Range(0f, 100f) > (instance.probabilityToApplyJumpDeviation * instance.probabilityToApplyJumpDeviationScalar.Evaluate(erraticBehaviourFactor));
    }

    public static float GetJumpAngleDeviation(float erraticBehaviourFactor)
    {
        float min = instance.minJumpVectorDeviation.Evaluate(erraticBehaviourFactor)*0.1f;
        float max = instance.maxJumpVectorDeviation.Evaluate(erraticBehaviourFactor)*0.1f;

        return UnityEngine.Random.Range(min, max);
    }
}
