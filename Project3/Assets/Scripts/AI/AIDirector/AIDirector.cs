using SurrealBoost.Types;
using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapController))]
public partial class AIDirector : MonoBehaviour
{
    public Transform astarMaxX;
    public Transform astarMinX;

    static AIDirector instance;



    private void OnDrawGizmos()
    {
        EBFGizmos();
    }

    private void Awake()
    {
        AIController.AStarIterationsDiscarder.maxX = astarMaxX.position.x;
        AIController.AStarIterationsDiscarder.minX = astarMinX.position.x;

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
        SetupBehaviourData();
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


    private void Update()
    {
        UpdateBehaviourParameters();
    }

    public static void RemoveReboundSurface(ReboundSurface toRemove)
    {
        AIController.AStarIterationsDiscarder.m_reboundWalls.Pop();
    }

    static List<ReboundSurface> reboundSurfaces = new List<ReboundSurface>();

}
