using AaronMeaney.BusStop.Scheduling;
using System;
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

        /// <summary>
        /// The different bus queues for each <see cref="BusRoute"/>.
        /// </summary>
        private Dictionary<BusRoute, Queue<BusPassenger>> busQueues;

        private PlaceAtCoordinates busStopPlacer = null;
        private PlaceAtCoordinates BusStopPlacer
        {
            get
            {
                if (busStopPlacer == null)
                    busStopPlacer = GetComponent<PlaceAtCoordinates>();

                return busStopPlacer;
            }
        }

        private ScheduleTaskRunner taskRunner = null;
        private ScheduleTaskRunner TaskRunner
        {
            get
            {
                if (taskRunner == null)
                    taskRunner = FindObjectOfType<ScheduleTaskRunner>();

                return taskRunner;
            }
        }

        private void Awake()
        {
            busQueues = new Dictionary<BusRoute, Queue<BusPassenger>>();

            // Rotate towards the waypoint once the coordinates are set
            BusStopPlacer.OnPlacementFinished += () =>
            {
                RotateTowardsWaypoint();
            };

            // Set the coordinates
            SetCoordinatePositionWhenReady();
        }
        
        /// <summary>
        /// Allows passengers to board/unboard the <see cref="Bus"/> that is servicing this stop.
        /// </summary>
        /// <param name="bus">The <see cref="Bus"/> that is waiting at / servicing this stop.</param>
        public void ServiceStop(Bus bus)
        {
            bus.Status = Bus.BusStatus.WaitingAtStop;

            // Board passengers once the passengers on the bus finish unboarding
            bus.OnPassengersUnboarded = () => { BoardFrontPassenger(bus); };

            bus.UnboardPassenger(this);
        }
        
        /// <summary>
        /// Recursive call to board the passenger at the front of the <see cref="busQueues"/>.
        /// Returns once the queue for this <see cref="Bus"/>' <see cref="BusRoute"/> is empty.
        /// </summary>
        /// <param name="bus">The <see cref="Bus"/> the passengers are getting onto.</param>
        private void BoardFrontPassenger(Bus bus)
        {
            if (BusCanBeBoarded(bus) == false)
            {
                if (bus.CurrentRoute.IsFinalStop(bus.CurrentStop))
                {
                    bus.EndService();
                    return;
                }
                else
                {
                    bus.FinishWaitingAtStop();
                    return;
                }
            }

            Queue<BusPassenger> busQueue = busQueues[bus.CurrentRoute];
            
            bus.BoardPassenger(busQueue.Dequeue());

            // Wait for the next passenger to board
            DateTime nextBoardTime = DateTime.Now.AddSeconds(5);
            TaskRunner.AddTask(new ScheduledTask(() => { BoardFrontPassenger(bus); }, nextBoardTime));
        }

        /// <summary>
        /// Returns if the <see cref="Bus"/> can be boarded.
        /// </summary>
        private bool BusCanBeBoarded(Bus bus)
        {
            // Bus Queue doesn't exist
            if (busQueues.ContainsKey(bus.CurrentRoute) == false)
                return false;

            // Bus Queue is empty
            if (busQueues[bus.CurrentRoute].Count == 0)
                return false;

            // The bus is full
            if (bus.IsFull)
                return false;

            return true;
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
                BusStopPlacer.Execute();
            }
            else
            {
                busWaypointPlacer.OnPlacementFinished += () =>
                {
                    BusStopPlacer.Execute();
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
