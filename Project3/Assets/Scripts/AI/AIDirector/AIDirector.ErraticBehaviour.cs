using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

public partial class AIDirector: MonoBehaviour
{


    /*
        Doc:
    https://media.discordapp.net/attachments/905760062293811221/954412884895617044/unknown.png?width=1467&height=1467

     */

    [FoldoutGroup("Erratic behaviour configuration")]
    public AnimationCurve minWaitTimeBeforeJump;
    [FoldoutGroup("Erratic behaviour configuration")]
    public AnimationCurve maxWaitTimeBeforeJump;
    [FoldoutGroup("Erratic behaviour configuration")]
    public AnimationCurve minJumpVectorDeviation;
    [FoldoutGroup("Erratic behaviour configuration")]
    public AnimationCurve maxJumpVectorDeviation;
    [FoldoutGroup("Erratic behaviour configuration")]
    public AnimationCurve probabilityToApplyJumpDeviationScalar;
    [FoldoutGroup("Erratic behaviour configuration")]
    public float probabilityToApplyJumpDeviation = 20f;

    

    float playerEBF;

    [HideInInspector]
    public float desiredEBFDifference;

  


    AIController[] ais;
    
    // PERFORMANCE MEASURES
    const float performanceMeasurementLapse = 15f;
    float playerCumulativeTimeWithoutProgressInLastLapse = performanceMeasurementLapse;
    float playerMaxYReached = 0f;
    float[] aisYReached;
    float[] aisCumulativeTimeWithoutProgressInLastLapse;

    void SetupBehaviourData()
    {
        if (GameInfo.instance == null || GameInfo.instance.ai_players == null)
            return;

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

    [FoldoutGroup("Song volume curves")]
    public AnimationCurve songLayer0VolumePlayerPerformance;
    [FoldoutGroup("Song volume curves")]
    public AnimationCurve songLayer1VolumePlayerPerformance;
    [FoldoutGroup("Song volume curves")]
    public AnimationCurve songLayer2VolumePlayerPerformance;
    [FoldoutGroup("Song volume curves")]
    public AnimationCurve songLayer3VolumePlayerPerformance;

    void SetMusicVolumeFromPlayerPM()
    {

        var performancePercentage = 1f - ((playerCumulativeTimeWithoutProgressInLastLapse*0.5f) / performanceMeasurementLapse);

        AudioController.instance.SetSongLayer0Volume(StartMatchCountDown.matchStarted?songLayer0VolumePlayerPerformance.Evaluate(performancePercentage):0f);
        AudioController.instance.SetSongLayer1Volume(songLayer1VolumePlayerPerformance.Evaluate(performancePercentage));
        AudioController.instance.SetSongLayer2Volume(songLayer2VolumePlayerPerformance.Evaluate(performancePercentage));
        AudioController.instance.SetSongLayer3Volume(songLayer3VolumePlayerPerformance.Evaluate(performancePercentage));

    }


    void UpdateBehaviourParameters()
    {
       
        UpdatePerformanceMeasurementData();

        if (GameInfo.instance == null || GameInfo.instance.ai_players == null)
            return;


        playerEBF = CalculatePlayerErraticBehaviourFactor();

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
            ref float maxYReached, 
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

        float playerY;

#if UNITY_EDITOR
        if(GameInfo.instance == null)
        {
            playerY = FindObjectOfType<Runner>().transform.position.y;
        }
        else
#endif

        playerY = GameInfo.instance.player.transform.position.y;
        
        UpdateSinglePMData(ref playerMaxYReached, ref playerCumulativeTimeWithoutProgressInLastLapse, playerY);


        if (GameInfo.instance == null || GameInfo.instance.ai_players == null)
            return;


        for (int i = 0; i < ais.Length; i++)
            UpdateSinglePMData(ref aisYReached[i], 
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

    public void EBFGizmos()
    {

        if (GameInfo.instance == null || GameInfo.instance.ai_players == null || GameInfo.instance.ai_players.Length == 0)
            return;

#if UNITY_EDITOR
        Vector3 stageDimensions = Camera.main.ScreenToWorldPoint(new Vector3(10, Screen.height-40, 0));
        
        Handles.Label(stageDimensions, 
            $@"Player EBF: {playerEBF.ToString("0.00")} PM: {playerCumulativeTimeWithoutProgressInLastLapse.ToString("0.00")}
AI1 EBF: {ais[0].erraticBehaviourFactor.ToString("0.00")} TBF: {(ais[0].desiredPlayerEBFOffset+playerEBF).ToString("0.00")} PM: {aisCumulativeTimeWithoutProgressInLastLapse[0].ToString("0.00")}
AI2 EBF: {ais[1].erraticBehaviourFactor.ToString("0.00")} TBF: {(ais[1].desiredPlayerEBFOffset+playerEBF).ToString("0.00")} PM: {aisCumulativeTimeWithoutProgressInLastLapse[1].ToString("0.00")}
AI3 EBF: {ais[2].erraticBehaviourFactor.ToString("0.00")} TBF: {(ais[2].desiredPlayerEBFOffset+playerEBF).ToString("0.00")} PM: {aisCumulativeTimeWithoutProgressInLastLapse[2].ToString("0.00")}
");
#endif
    }
}
