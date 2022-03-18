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

    [HideInInspector]
    public float desiredEBFDifference;

    [Button("Apply clamp")]
    void ApplyClamp()
    {

        clampMaxWaitTime = false;
        clampMinWaitTime = false;
        clampMaxAngleDeviation = false;
        clampMinAngleDeviation = false;

    }


    AIController[] ais;
    
    // PERFORMANCE MEASURES
    const float performanceMeasurementLapse = 15f;
    float playerCumulativeTimeWithoutProgressInLastLapse = performanceMeasurementLapse;
    float playerMaxYReached = 0f;
    float[] aisYReached;
    float[] aisCumulativeTimeWithoutProgressInLastLapse;

    void SetupBehaviourData()
    {
        ais = new AIController[GameInfo.instance.ai_players.Length];
        aisYReached = new float[GameInfo.instance.ai_players.Length];
        aisCumulativeTimeWithoutProgressInLastLapse = new float[GameInfo.instance.ai_players.Length];

        for (int i = 0; i < GameInfo.instance.ai_players.Length; i++)
        {
            ais[i] = GameInfo.instance.ai_players[i].GetComponent<AIController>();
            aisCumulativeTimeWithoutProgressInLastLapse[i] = performanceMeasurementLapse;
            aisYReached[i] = 0f;
        }

    }



    void UpdateBehaviourParameters()
    {
        UpdatePerformanceMeasurementData();

        float playerEBF = CalculatePlayerErraticBehaviourFactor();

        foreach(var ai in ais)
        {
            ai.AproximateOwnEBFToPlayerEBF(playerEBF);
        }
    }



    float CalculatePlayerErraticBehaviourFactor()
    {
        float acumulatedRuleOfThree = 0f;
        for(int i = 0; i < ais.Length; i++)
        {
            acumulatedRuleOfThree += 
                (ais[i].erraticBehaviourFactor * playerCumulativeTimeWithoutProgressInLastLapse) 
                / aisCumulativeTimeWithoutProgressInLastLapse[i];
        }

        // AVG
        acumulatedRuleOfThree /= (float)ais.Length;

        return acumulatedRuleOfThree;
    }


    void UpdateSinglePMData
        (
            float maxYReached, 
            ref float cumulativeTimeWithoutProgressInLastLapse,
            float currentY
        )
    {
        const float limit = 0.0000001f;

        if (currentY > maxYReached)
        {
            maxYReached = currentY;
            cumulativeTimeWithoutProgressInLastLapse -= Time.deltaTime;
            if (cumulativeTimeWithoutProgressInLastLapse < limit)
                cumulativeTimeWithoutProgressInLastLapse = limit;
        }
        else
        {
            cumulativeTimeWithoutProgressInLastLapse += Time.deltaTime;
            if (cumulativeTimeWithoutProgressInLastLapse > performanceMeasurementLapse)
                cumulativeTimeWithoutProgressInLastLapse = performanceMeasurementLapse;
        }
    }

    void UpdatePerformanceMeasurementData()
    {
        float playerY = GameInfo.instance.player.transform.position.y;
        UpdateSinglePMData(playerMaxYReached, ref playerCumulativeTimeWithoutProgressInLastLapse, playerY);

        for(int i = 0; i < ais.Length; i++)
            UpdateSinglePMData(aisYReached[i], 
                ref aisCumulativeTimeWithoutProgressInLastLapse[i], 
                ais[i].transform.position.y);
           
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
