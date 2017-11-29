using System.Collections.Generic;
using Mapbox.Unity.Utilities;
using UnityEngine;
using Mapbox.Unity.Map;

[System.Serializable]
public class BusRoute {
    [SerializeField]
    private List<BusRouteNode> latLongNodes;
    public List<BusRouteNode> LatLongNodes
    {
        get { return latLongNodes; }
    }

    public int Size
    {
        get { return latLongNodes.Count; }
    }
}

[System.Serializable]
public class BusRouteNode
{
    [SerializeField]
    private float latitude;
    public float Latitude { get; set; }

    [SerializeField]
    private float longitude;
    public float Longitude { get; set; }

    public Vector3 AsUnityPosition(AbstractMap map)
    {
        return new Vector2(latitude, longitude).AsUnityPosition(map.CenterMercator, map.WorldRelativeScale);
    }
}
