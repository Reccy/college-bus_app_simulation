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
    [System.Serializable]
    [RequireComponent(typeof(PlaceAtCoordinates))]
    public class BusStop : MonoBehaviour
    {
        /// <summary>
        /// Called when a <see cref="Bus"/> approaches the <see cref="BusStop"/>
        /// </summary>
        public Action<Bus> OnBusApproach;

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
        private Queue<BusPassenger> busQueue = null;
        public Queue<BusPassenger> BusQueue
        {
            get
            {
                if (busQueue == null)
                    busQueue = new Queue<BusPassenger>();

                return busQueue;
            }
        }

        /// <summary>
        /// <see cref="HashSet"/> of <see cref="BusStop"/>s that are downstream to this <see cref="BusStop"/>
        /// </summary>
        private List<BusStop> downstreamStops = null;
        public List<BusStop> DownstreamStops
        {
            get
            {
                if (downstreamStops == null)
                    downstreamStops = new List<BusStop>();

                return downstreamStops;
            }
        }

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
            // Start making passengers queue here
            PopulateBusStop();
            
            // Rotate towards the waypoint once the coordinates are set
            BusStopPlacer.OnPlacementFinished += () =>
            {
                RotateTowardsWaypoint();
            };

            // Set the coordinates
            SetCoordinatePositionWhenReady();
        }

        /// <summary>
        /// Populates this <see cref="BusStop"/> with passengers who are going to <see cref="BusStop"/>s
        /// downstream of the routes that contain this <see cref="BusStop"/>.
        /// </summary>
        private void PopulateBusStop()
        {
            List<BusRoute> routes = new List<BusRoute>(FindObjectsOfType<BusRoute>());

            // Get a list of all the bus routes that are after this one
            foreach (BusRoute route in routes)
            {
                if (route.BusStops.Contains(this))
                {
                    int startIndex = route.BusStops.IndexOf(this) + 1;

                    for (int busStopIndex = startIndex; busStopIndex < route.BusStops.Count; busStopIndex++)
                    {
                        BusStop newBusStop = route.BusStops[busStopIndex];

                        if (!DownstreamStops.Contains(newBusStop))
                            DownstreamStops.Add(newBusStop);
                    }
                }
            }

            // Don't add passengers to the bus stop if this is the last stop for all routes
            if (DownstreamStops.Count == 0)
                return;

            // Create 3 random passengers
            System.Random r = new System.Random();
            for (int i = 1; i <= 3; i++)
            {
                int busStopIndex = r.Next(0, DownstreamStops.Count);
                BusStop destinationStop = DownstreamStops[busStopIndex];

                AddPassengerToQueue(new BusPassenger(this, destinationStop));
            }
        }

        /// <summary>
        /// Adds a <see cref="BusPassenger"/> to the <see cref="BusQueue"/>
        /// </summary>
        private void AddPassengerToQueue(BusPassenger fellowPassenger)
        {
            BusQueue.Enqueue(fellowPassenger);

            Debug.Log(fellowPassenger.FullName
                + " joined the queue at "
                + fellowPassenger.OriginBusStop.busStopIdInternal
                + " to go to "
                + fellowPassenger.DestinationBusStop.busStopIdInternal);
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
        /// Recursive call to board the passenger at the front of the <see cref="BusQueues"/>.
        /// Returns once the queue for this <see cref="Bus"/>' <see cref="BusRoute"/> is empty.
        /// </summary>
        /// <param name="bus">The <see cref="Bus"/> the passengers are getting onto.</param>
        private void BoardFrontPassenger(Bus bus)
        {
            Debug.Log(bus.RegistrationNumber + " is being boarded by " + BusStopIdInternal);

            if (BusCanBeBoarded(bus) == false)
            {
                Debug.Log(bus.RegistrationNumber + " is finished being boarded by " + BusStopIdInternal);

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
            
            Debug.Log(BusQueue.Peek().FullName + " is boarding " + bus.RegistrationNumber + ". Destination: " + BusQueue.Peek().DestinationBusStop.BusStopIdInternal);

            bus.BoardPassenger(BusQueue.Dequeue());

            // Wait for the next passenger to board
            DateTime nextBoardTime = DateTime.Now.AddSeconds(1);
            TaskRunner.AddTask(new ScheduledTask(() => { BoardFrontPassenger(bus); }, nextBoardTime));
        }

        /// <summary>
        /// Returns if the <see cref="Bus"/> can be boarded.
        /// </summary>
        private bool BusCanBeBoarded(Bus bus)
        {
            // Bus Queue is empty
            if (BusQueue.Count == 0)
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
            // Display name of bus stop (Frustum culling source: https://forum.unity.com/threads/handles-label-fail-when-point-behind-camera.79217/ - Barry Northern)
            Plane[] frustum = GeometryUtility.CalculateFrustumPlanes(SceneView.currentDrawingSceneView.camera);
            if (GeometryUtility.TestPlanesAABB(frustum, new Bounds(transform.position, Vector3.one)))
            {
                Handles.Label(transform.position, name);
            }
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
