using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusStop"/> on the side of the road
    /// </summary>
    [RequireComponent(typeof(PlaceAtCoordinates))]
    public class BusStop : MonoBehaviour
    {
        [SerializeField]
        private string busStopId;
        /// <summary>
        /// The ID of the <see cref="BusStop"/>. Shown to the end user.
        /// </summary>
        public string BusStopId { get { return busStopId; } }

        [SerializeField]
        private string busStopIdInternal;
        /// <summary>
        /// The ID of the <see cref="BusStop"/>. Only used in the Unity Editor. Must be unique.
        /// </summary>
        public string BusStopIdInternal { get { return busStopIdInternal; } }
        
        /// <summary>
        /// The <see cref="RouteWaypoint"/> belonging to this <see cref="BusStop"/>
        /// </summary>
        public RouteWaypoint LinkedRouteWaypoint
        {
            get { return GetComponentInParent<RouteWaypoint>(); }
        }

        private PlaceAtCoordinates busStopPlacer;

        private void Awake()
        {
            busStopPlacer = GetComponent<PlaceAtCoordinates>();

            // Rotate towards the waypoint once the coordinates are set
            busStopPlacer.OnPlacementFinished += () =>
            {
                RotateTowardsWaypoint();
            };

            // Set the coordinates
            SetCoordinatePositionWhenReady();
        }

        /// <summary>
        /// Places the <see cref="BusStop"/> using <see cref="PlaceAtCoordinates"/> when the <see cref="LinkedRouteWaypoint"/> is finished being placed.
        /// Has to be called after to prevent Transform errors.
        /// </summary>
        private void SetCoordinatePositionWhenReady()
        {
            PlaceAtCoordinates busWaypointPlacer = LinkedRouteWaypoint.GetComponent<PlaceAtCoordinates>();

            if (busWaypointPlacer.FinishedPlacement)
            {
                busStopPlacer.Execute();
            }
            else
            {
                busWaypointPlacer.OnPlacementFinished += () =>
                {
                    busStopPlacer.Execute();
                };
            }
        }

        /// <summary>
        /// Rotates the <see cref="BusStop"/> to face the <see cref="LinkedRouteWaypoint"/>
        /// </summary>
        private void RotateTowardsWaypoint()
        {
            transform.right = LinkedRouteWaypoint.transform.position - transform.position;
        }

        private void OnDrawGizmos()
        {
            Handles.Label(transform.position, name);
        }

        private void OnValidate()
        {
            name = BusStopIdInternal + "_BusStop";

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
