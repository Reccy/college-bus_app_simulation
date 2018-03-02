using System;
using Mapbox.Unity.Map;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a waypoint for each leg in the journey of a <see cref="BusRoute"/>
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(SnapToTerrain))]
    public class RouteWaypoint : MonoBehaviour
    {
        private AbstractMap simAbstractMap;

        public RouteWaypointData routeWaypointData;

        /// <summary>
        /// Sets the <see cref="RouteWaypointData"/> and places an instance of the <see cref="RouteWaypoint"/> in the simulation
        /// </summary>
        internal void Initialize(RouteWaypointData routeWaypointData, AbstractMap simAbstractMap)
        {
            this.simAbstractMap = simAbstractMap;
            this.routeWaypointData = routeWaypointData;

            name = "Route Waypoint (" + routeWaypointData.coordinateLocation.AsUnityPosition(simAbstractMap).ToString() + ")";
            transform.position = routeWaypointData.coordinateLocation.AsUnityPosition(simAbstractMap);

            UpdatePosition();
        }

        /// <summary>
        /// Updates the position based on the latitude and longitude of the <see cref="BusStopData"/>
        /// </summary>
        public void UpdatePosition()
        {
            Debug.Log("Updating position of " + name);
            transform.position = routeWaypointData.coordinateLocation.AsUnityPosition(simAbstractMap);

            // Snap position to terrain
            Debug.Log("Snapping " + name + " to terrain");
            GetComponent<SnapToTerrain>().PerformSnap();
        }
    }

    /// <summary>
    /// Contains data used to drive the functionality of <see cref="RouteWaypoint"/>
    /// </summary>
    [System.Serializable]
    public class RouteWaypointData
    {
        [SerializeField]
        private bool hasTrafficLights;
        /// <summary>
        /// If the waypoint has traffic lights, the bus will have to stop for some time at this waypoint.
        /// </summary>
        public bool HasTrafficLights
        {
            get { return hasTrafficLights; }
        }

        /// <summary>
        /// The coordinates of the route waypoint
        /// </summary>
        [SerializeField]
        public CoordinateLocation coordinateLocation;
    }
}
