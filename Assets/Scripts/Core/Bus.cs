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

        private BusStop nextStop = null;
        /// <summary>
        /// The current destination <see cref="BusStop"/> in the <see cref="CurrentRoute"/>
        /// </summary>
        public BusStop NextStop
        {
            get { return nextStop; }
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

        // TODO: Implement StartService and EndService

        /// <summary>
        /// Assigns the <see cref="CurrentService"/> and puts this <see cref="Bus"/> in service.
        /// </summary>
        /// <param name="service"></param>
        public void StartService(BusService service)
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// Removes this <see cref="Bus"/> from service.
        /// </summary>
        public void EndService()
        {
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
