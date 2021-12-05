using SurrealBoost.Types;
using System;
using System.Collections.Generic;
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

        return UnityEngine.Random.Range(min, max);
    }

    public static float GetVectorBeforeJump(float humanityFactor)
    {
        float min = instance.minJumpVectorDeviation.Evaluate(humanityFactor);
        float max = instance.maxJumpVectorDeviation.Evaluate(humanityFactor);

        float deviationDegrees = UnityEngine.Random.Range(min, max);

        return deviationDegrees;
    }

    private void Awake()
    {
        AIController.AStarIterationsDiscarder.m_reboundWalls = new NativeFIFO<AIController.ReboundWallInfo>();
        AIController.AStarIterationsDiscarder.m_reboundWalls.Init(100);
        instance = this;
    }


    private void OnDestroy()
    {
        AIController.AStarIterationsDiscarder.m_reboundWalls.Dispose();
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

    public static void AddReboundSurface(ReboundSurface toAdd)
    {
        reboundSurfaces.Add(toAdd);

        AIController.AStarIterationsDiscarder.m_reboundWallsLenght = reboundSurfaces.Count;
        for (int i = 0; i < AIController.AStarIterationsDiscarder.m_reboundWallsLenght; i++)
        {

            AIController.ReboundWallInfo wallInfo = new AIController.ReboundWallInfo()
            {
                inverseX = reboundSurfaces[i].inverseX,
                inverseY = reboundSurfaces[i].inverseY,
                collisionInfo = reboundSurfaces[i].collisionInfo

            };

            AIController.AStarIterationsDiscarder.m_reboundWalls.Add(wallInfo);

        }

    }

    public static void RemoveReboundSurface(ReboundSurface toRemove)
    {
        reboundSurfaces.Remove(toRemove);


        AIController.AStarIterationsDiscarder.m_reboundWallsLenght = reboundSurfaces.Count;
        for (int i = 0; i < AIController.AStarIterationsDiscarder.m_reboundWallsLenght; i++)
        {
            AIController.ReboundWallInfo wallInfo = new AIController.ReboundWallInfo()
            {
                inverseX = reboundSurfaces[i].inverseX,
                inverseY = reboundSurfaces[i].inverseY,
                collisionInfo = reboundSurfaces[i].collisionInfo

            };
            AIController.AStarIterationsDiscarder.m_reboundWalls.Add(wallInfo);

        }

    }

    static List<ReboundSurface> reboundSurfaces = new List<ReboundSurface>();

}
