﻿using UnityEngine;
using UnityEditor;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the information belonging to a single <see cref="Bus"/> instance.
    /// This data will be sent back to the API.
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(TransformLocationProvider))]
    public class Bus : MonoBehaviour
    {
        // Miscellaneous Information
        [SerializeField]
        private string registrationNumber;
        /// <summary>
        /// The registration number of this <see cref="Bus"/>
        /// </summary>
        public string RegistrationNumber
        {
            get { return registrationNumber; }
        }

        [SerializeField]
        private string busModel;
        /// <summary>
        /// The model of this <see cref="Bus"/>
        /// </summary>
        public string BusModel
        {
            get { return busModel; }
        }

        /// <summary>
        /// The <see cref="BusCompany"/> that this bus belongs to
        /// </summary>
        public BusCompany Company
        {
            get
            {
                if (GetComponentInParent<BusCompany>() == null)
                    return null;

                return GetComponentInParent<BusCompany>();
            }
        }

        // Runtime data
        private AbstractMap map = null;
        private AbstractMap Map
        {
            get
            {
                if (map == null)
                    map = FindObjectOfType<AbstractMap>();

                return map;
            }
        }

        public enum BusStatus { WaitingAtStop, Driving, OffService, BrokenDown }
        /// <summary>
        /// Represents the <see cref="Bus"/> status / state
        /// </summary>
        public BusStatus Status;

        private BusService currentService = null;
        /// <summary>
        /// The current <see cref="BusService"/> that this <see cref="Bus"/> is assigned to
        /// </summary>
        public BusService CurrentService
        {
            get { return currentService; }
        }

        /// <summary>
        /// The current <see cref="BusRoute"/> that this <see cref="Bus"/> is servicing
        /// </summary>
        public BusRoute CurrentRoute
        {
            get
            {
                if (CurrentService == null)
                    return null;

                return CurrentService.ServicedBusRoute;
            }
        }

        private BusTimeSlot currentTimeSlot = null;
        /// <summary>
        /// The current <see cref="BusTimeSlot"/> that the <see cref="Bus"/> is servicing
        /// </summary>
        public BusTimeSlot CurrentTimeSlot
        {
            get { return currentTimeSlot; }
            set
            {
                currentTimeSlot = value;

                // If this is the final stop, make sure the bus stops there
                if (CurrentRoute.IsFinalStop(CurrentStop))
                {
                    isStopping = true;
                }
            }
        }

        public BusTimeSlot NextTimeSlot
        {
            get
            {
                int currentTimeSlotIndex = CurrentService.TimeSlots.IndexOf(CurrentTimeSlot);
                int nextTimeSlotIndex = currentTimeSlotIndex + 1;

                if (nextTimeSlotIndex >= CurrentService.TimeSlots.Count)
                    return null;

                return CurrentService.TimeSlots[nextTimeSlotIndex];
            }
        }

        /// <summary>
        /// The currently serviced <see cref="BusStop"/> in the <see cref="CurrentRoute"/>
        /// </summary>
        public BusStop CurrentStop
        {
            get { return CurrentTimeSlot.ScheduledBusStop; }
        }

        /// <summary>
        /// The next <see cref="BusStop"/> to be serviced in the <see cref="CurrentRoute"/>
        /// </summary>
        public BusStop NextStop
        {
            get
            {
                int currentStopIndex = CurrentRoute.BusStops.IndexOf(CurrentStop);
                int nextStopIndex = currentStopIndex + 1;

                if (nextStopIndex >= CurrentRoute.BusStops.Count)
                    return null;

                return CurrentRoute.BusStops[nextStopIndex];
            }
        }

        /// <summary>
        /// The current <see cref="CoordinateLocation"/> to drive directly towards
        /// </summary>
        private CoordinateLocation currentDestination = null;

        /// <summary>
        /// The index of the current <see cref="BusRoute.PathWaypoints"/> that is being followed.
        /// </summary>
        private int currentPathWaypointIndex = 0;
        
        private bool isStopping = false;
        /// <summary>
        /// If the <see cref="Bus"/> is prepared to stop at the next <see cref="BusStop"/>
        /// </summary>
        public bool IsStopping { get { return isStopping; } }

        /// <summary>
        /// Stops the <see cref="Bus"/> at the <see cref="CurrentStop"/>
        /// </summary>
        public void PrepareToStop()
        {
            if (Status == BusStatus.Driving)
            {
                Debug.Log("Preparing to stop at: " + CurrentStop);
                isStopping = true;
            }
        }

        /// <summary>
        /// The <see cref="Latitude"/> of the <see cref="Bus"/>
        /// </summary>
        public double Latitude
        {
            get { return GetComponent<TransformLocationProvider>().Location.x; }
        }

        /// <summary>
        /// The <see cref="Longitude"/> of the <see cref="Bus"/>
        /// </summary>
        public double Longitude
        {
            get { return GetComponent<TransformLocationProvider>().Location.y; }
        }

        /// <summary>
        /// Assigns the <see cref="CurrentService"/> and puts this <see cref="Bus"/> in service.
        /// </summary>
        public void StartService(BusService service)
        {
            if (!Company.BussesOnRoad.Contains(this))
            {
                Company.BussesOnRoad.Add(this);
                Company.BussesInDepot.Remove(this);
            }

            // Assign the bus to the service
            currentService = service;

            // Set the time slot that the bus is servicing
            CurrentTimeSlot = service.ScheduledTimeSlot;

            // Set the current destination to the serviced time slot stop
            currentDestination = CurrentRoute.GetCoordinateLocationFromBusStop(CurrentStop);
            transform.position = currentDestination.AsUnityPosition(Map);

            // Get the index of the current path waypoint
            currentPathWaypointIndex = CurrentRoute.PathWaypoints.IndexOf(currentDestination);

            // Service the first stop that the Bus is assigned to
            CurrentStop.ServiceStop(this);

            gameObject.SetActive(true);

            Debug.Log(RegistrationNumber + " entered service for route " + CurrentRoute.RouteIdInternal);
        }

        /// <summary>
        /// Removes this <see cref="Bus"/> from service.
        /// </summary>
        public void EndService()
        {
            if (!Company.BussesInDepot.Contains(this))
            {
                Company.BussesInDepot.Add(this);
                Company.BussesOnRoad.Remove(this);
            }

            currentService = null;
            transform.position = Vector3.zero;
            gameObject.SetActive(false);

            Status = BusStatus.OffService;
        }

        private void Update()
        {
            switch (Status)
            {
                case BusStatus.Driving:
                    DriveTowardsDestination();
                    break;
            }
        }

        /// <summary>
        /// Drives the <see cref="Bus"/> towards the <see cref="currentDestination"/>
        /// </summary>
        private void DriveTowardsDestination()
        {
            // Get the distance between the bus and the destination (Exclude Y axis)
            Vector3 destinationPosition = currentDestination.AsUnityPosition(Map);
            destinationPosition = new Vector3(destinationPosition.x, transform.position.y, destinationPosition.z);

            float destinationDistance = Vector3.Distance(transform.position, destinationPosition);

            // Drive towards the destination
            transform.LookAt(destinationPosition);
            transform.position = Vector3.MoveTowards(transform.position, destinationPosition, 15 * Time.deltaTime);

            // Logic for when the bus reaches the destination
            if (destinationDistance < 0.1f)
            {
                bool isNextStop = currentDestination == CurrentRoute.GetCoordinateLocationFromBusStop(CurrentStop);
                
                if (isNextStop && isStopping)
                {
                    CurrentStop.ServiceStop(this);
                    return;
                }
                else if (isNextStop)
                {
                    CurrentTimeSlot = NextTimeSlot;
                }

                // Go the next destination
                currentPathWaypointIndex += 1;
                currentDestination = CurrentRoute.PathWaypoints[currentPathWaypointIndex];
            }
        }

        /// <summary>
        /// Called by the <see cref="BusStop"/> when the passengers have finished embarking and disembarking.
        /// </summary>
        public void FinishWaitingAtStop()
        {
            Debug.Log("Finished waiting at " + CurrentStop);

            Status = BusStatus.Driving;

            isStopping = false;

            // Service the next time slot
            CurrentTimeSlot = NextTimeSlot;
        }

        /// <summary>
        /// Sets the name of the <see cref="GameObject"/>
        /// </summary>
        public void ValidateBusName()
        {
            string busName = "";

            if (Company == null)
            {
                busName += "[BAD_COMPANY]";
                Debug.LogError("Bus [" + RegistrationNumber + "] has no company!", this);
            }
            else
            {
                busName += "[" + Company.CompanyName + "]";
            }

            busName += " Bus - " + RegistrationNumber;

            name = busName;
        }

        // Need to use OnDrawGizmos since it's the closest thing to Update() in Edit Mode
        private void OnDrawGizmosSelected()
        {
            ValidateBusName();
        }
    }

    [CustomEditor(typeof(Bus))]
    public class BusEditor : Editor
    {
        private Bus bus;

        public override void OnInspectorGUI()
        {
            base.DrawDefaultInspector();

            bus = ((Bus)target);

            if (Application.isPlaying)
            {
                EditorGUI.BeginDisabledGroup((bus.IsStopping && bus.Status == Bus.BusStatus.Driving) || bus.Status == Bus.BusStatus.WaitingAtStop);

                if (GUILayout.Button("Hail Bus"))
                {
                    bus.PrepareToStop();
                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
