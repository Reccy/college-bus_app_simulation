using UnityEngine;
using Mapbox.Unity.Location;
using UnityEditor;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the information belonging to a single <see cref="Bus"/> instance.
    /// This data will be sent back to the API.
    /// </summary>
    [System.Serializable]
    [RequireComponent(typeof(BusDriver), typeof(TransformLocationProvider))]
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
        public enum BusStatus { InService, OffService, BrokenDown }
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
        
        private BusTimeSlot currentTimeSlot = null;
        /// <summary>
        /// The current <see cref="BusTimeSlot"/> that the <see cref="Bus"/> is servicing
        /// </summary>
        public BusTimeSlot CurrentTimeSlot
        {
            get { return currentTimeSlot; }
        }

        /// <summary>
        /// The current destination <see cref="BusStop"/> in the <see cref="CurrentRoute"/>
        /// </summary>
        public BusStop NextStop
        {
            get { return CurrentTimeSlot.ScheduledBusStop; }
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

            currentService = service;
            
            Debug.Log("0th Time Slot is " + service.TimeSlots[0].ScheduledBusStop.BusStopIdInternal);

            currentTimeSlot = service.TimeSlots[0];
            transform.position = NextStop.LinkedRouteWaypoint.transform.position;
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
