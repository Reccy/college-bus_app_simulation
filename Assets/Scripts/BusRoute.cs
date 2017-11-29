using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BusStop/BusRoute"), System.Serializable]
public class BusRoute {
    [SerializeField]
    private List<BusRouteNode> latLongNodes;
    public List<BusRouteNode> LatLongNodes
    {
        get { return latLongNodes; }
    }
}

[System.Serializable]
public class BusRouteNode
{
    [SerializeField]
    private double latitude;
    public double Latitude { get; set; }

    [SerializeField]
    private double longitude;
    public double Longitude { get; set; }
}
