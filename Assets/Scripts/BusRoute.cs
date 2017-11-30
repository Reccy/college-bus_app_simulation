using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using UnityEngine;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using Mapbox.Directions;
using Mapbox.Unity;
using System;

[Serializable]
public class BusRoute {

    public delegate void OnBusRoutePopulated(BusRoute thisRoute);
    /// <summary>
    /// Delegate is called when the Bus Route has been fully populated with BusRouteNodes.
    /// </summary>
    public OnBusRoutePopulated onBusRoutePopulated;

    private List<BusRouteNode> latLongNodes = new List<BusRouteNode>();
    public List<BusRouteNode> LatLongNodes
    {
        get { return latLongNodes; }
    }

    public int Size
    {
        get { return latLongNodes.Count; }
    }

    Directions directions;

    /// <summary>
    /// Replaces the bus route with direction nodes from Mapbox.
    /// The position is translated to coordinates and then the directions are generated.
    /// </summary>
    /// <param name="fromPosition">The world space position to begin navigating from.</param>
    /// <param name="toPosition">The world space position to navigate to.</param>
    public void SetDirectionsToPosition(AbstractMap map, Vector3 fromPosition, Vector3 toPosition)
    {
        // Get the geo positions from the world Vector3 positions
        Vector2d fromGeoPosition = fromPosition.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);
        Vector2d toGeoPosition = toPosition.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);

        // Query the Mapbox Directions API, the response is handled by the HandleDirectionsResponse method
        directions = MapboxAccess.Instance.Directions;
        DirectionResource directionResource = new DirectionResource(new Vector2d[] { fromGeoPosition, toGeoPosition }, RoutingProfile.Driving);
        directionResource.Steps = true;
        directions.Query(directionResource, HandleDirectionsResponse);
    }

    /// <summary>
    /// Handles the response called from SetDirectionsToPosition by populating the LatLongNodes
    /// </summary>
    /// <param name="response"></param>
    private void HandleDirectionsResponse(DirectionsResponse response)
    {
        List<BusRouteNode> newNodes = new List<BusRouteNode>();

        foreach (Step step in response.Routes[0].Legs[0].Steps)
        {
            foreach (Vector2d point in step.Geometry)
            {
                newNodes.Add(new BusRouteNode((float)point.x, (float)point.y));
            }
        }

        LatLongNodes.Clear();
        LatLongNodes.AddRange(newNodes);

        if (onBusRoutePopulated != null)
            onBusRoutePopulated(this);
    }
}

[Serializable]
public class BusRouteNode
{
    [SerializeField]
    private float latitude;
    public float Latitude { get { return latitude; } }

    [SerializeField]
    private float longitude;
    public float Longitude { get { return longitude; } }

    public BusRouteNode(float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
    }

    public BusRouteNode(Vector2 latLong)
    {
        this.latitude = latLong.x;
        this.longitude = latLong.y;
    }

    public Vector3 AsUnityPosition(AbstractMap map)
    {
        Vector3 unityPosition = new Vector2(latitude, longitude).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
        return unityPosition;
    }
}
