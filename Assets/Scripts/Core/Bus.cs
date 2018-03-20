﻿using UnityEngine;
using Mapbox.Unity.Location;
using UnityEditor;
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
        
        public enum BusStatus { InService, OffService, FinishedRoute, BrokenDown }
        /// <summary>
        /// Represents if the <see cref="Bus"/> is in service, off service, broken down, etc
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
        
        private BusTimeSlot servicingTimeSlot = null;
        /// <summary>
        /// The current <see cref="BusTimeSlot"/> that the <see cref="Bus"/> is servicing
        /// </summary>
        public BusTimeSlot ServicingTimeSlot
        {
            get { return servicingTimeSlot; }
        }

        /// <summary>
        /// The currently serviced <see cref="BusStop"/> in the <see cref="CurrentRoute"/>
        /// </summary>
        public BusStop CurrentStop
        {
            get { return ServicingTimeSlot.ScheduledBusStop; }
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
            servicingTimeSlot = service.ScheduledTimeSlot;

            // Set the current destination to the serviced time slot stop
            currentDestination = CurrentRoute.GetCoordinateLocationFromBusStop(CurrentStop);
            transform.position = currentDestination.AsUnityPosition(Map);

            // Get the index of the current path waypoint
            currentPathWaypointIndex = CurrentRoute.PathWaypoints.IndexOf(currentDestination);

            Status = BusStatus.InService;

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
            if (Status == BusStatus.InService)
            {
                DriveTowardsDestination();
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
            transform.Translate(Vector3.forward * 5 * Time.deltaTime);
            
            Debug.Log("Current Destination: " + destinationPosition + " --> Current Distance " + destinationDistance);

            // Once the bus reaches the destination, go to the next stop if it exists. Otherwise set the bus status to have finished the service.
            if (destinationDistance < 0.1f)
            {
                if (NextStop != null)
                {
                    currentPathWaypointIndex += 1;
                    currentDestination = CurrentRoute.PathWaypoints[currentPathWaypointIndex];
                }
                else
                {
                    Status = BusStatus.FinishedRoute;
                }
            }
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
}
