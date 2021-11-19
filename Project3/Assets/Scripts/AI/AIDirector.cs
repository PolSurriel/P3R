using UnityEngine;

[RequireComponent(typeof(MapController))]
public class AIDirector : MonoBehaviour
{

    public AnimationCurve minWaitTimeBeforeJump;
    public AnimationCurve maxWaitTimeBeforeJump;
    
    public AnimationCurve minJumpVectorDeviation;
    public AnimationCurve maxJumpVectorDeviation;

    static AIDirector instance;

    public static float GetTimeBeforeJump(float humanityFactor)
    {
        float min = instance.minWaitTimeBeforeJump.Evaluate(humanityFactor);
        float max = instance.maxWaitTimeBeforeJump.Evaluate(humanityFactor);

        return Random.Range(min, max);
    }

    public static float GetVectorBeforeJump(float humanityFactor)
    {
        float min = instance.minJumpVectorDeviation.Evaluate(humanityFactor);
        float max = instance.maxJumpVectorDeviation.Evaluate(humanityFactor);

        float deviationDegrees = Random.Range(min, max);

        return deviationDegrees;
    }

    private void Awake()
    {
        instance = this;
    }


    void Start()
    {
        SetMapController();
    }

    MapController mapController;

    void SetMapController()
    {
        if (mapController == null)
            mapController = GetComponent<MapController>();

    }

}
