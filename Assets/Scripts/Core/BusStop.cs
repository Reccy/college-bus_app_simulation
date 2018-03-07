﻿using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusStop"/> on the side of the road
    /// </summary>
    public class BusStop : MonoBehaviour
    {
        [SerializeField]
        private string busStopId;
        /// <summary>
        /// The ID of the <see cref="BusStop"/>. Must be unique.
        /// </summary>
        public string BusStopId { get { return busStopId; } }
        
        /// <summary>
        /// The <see cref="RouteWaypoint"/> belonging to this <see cref="BusStop"/>
        /// </summary>
        public RouteWaypoint LinkedRouteWaypoint
        {
            get { return GetComponentInParent<RouteWaypoint>(); }
        }
        
        private void OnDrawGizmos()
        {
            Handles.Label(transform.position, name);
        }

        private void OnValidate()
        {
            name = BusStopId + "_BusStop";

            // Validate waypoints to update BusStopId names in Waypoint
            RouteWaypoint.ValidateAllWaypoints();
        }
    }

    [CustomEditor(typeof(BusStop))]
    public class BusStopEditor : Editor
    {
        private BusStop busStop;

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            busStop = (BusStop)target;

            if (busStop.LinkedRouteWaypoint)
            {
                if (GUILayout.Button("Select Waypoint"))
                {
                    Selection.activeGameObject = busStop.LinkedRouteWaypoint.gameObject;
                }
            }
        }
    }
}
