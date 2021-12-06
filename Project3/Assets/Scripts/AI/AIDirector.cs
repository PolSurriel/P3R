using SurrealBoost.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public class AIDirector : MonoBehaviour
{
    public Transform maxX;
    public Transform minX;

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
        AIController.AStarIterationsDiscarder.maxX = maxX.position.x;
        AIController.AStarIterationsDiscarder.minX = minX.position.x;

        AIController.AStarIterationsDiscarder.m_reboundWalls = new NativeFIFO<AIController.NativeReboundWallInfo>();
        AIController.AStarIterationsDiscarder.m_reboundWalls.Init(100);

        AIController.AStarIterationsDiscarder.m_portals = new NativeFIFO<AIController.NativePortalInfo>();
        AIController.AStarIterationsDiscarder.m_portals.Init(100);
        instance = this;
    }


    private void OnDestroy()
    {
        AIController.AStarIterationsDiscarder.m_reboundWalls.Dispose();
        AIController.AStarIterationsDiscarder.m_portals.Dispose();
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

    public static void AddPortal(Portal portal)
    {
        AIController.AStarIterationsDiscarder.m_portals.Add(portal.GeneratePortalNativeInfo());
    }

    public static void RemovePortal(Portal portal)
    {
        AIController.AStarIterationsDiscarder.m_portals.Pop();
    }

    public static void AddReboundSurface(ReboundSurface toAdd)
    {
        AIController.NativeReboundWallInfo wallInfo = new AIController.NativeReboundWallInfo()
        {
            inverseX = toAdd.inverseX,
            inverseY = toAdd.inverseY,
            collisionInfo = toAdd.collisionInfo

        };

        AIController.AStarIterationsDiscarder.m_reboundWalls.Add(wallInfo);

    }

    public static void RemoveReboundSurface(ReboundSurface toRemove)
    {
        AIController.AStarIterationsDiscarder.m_reboundWalls.Pop();
    }

    static List<ReboundSurface> reboundSurfaces = new List<ReboundSurface>();

}
