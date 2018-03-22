using UnityEngine;
using AaronMeaney.BusStop.Utilities;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a bus passenger
    /// </summary>
    public class BusPassenger : MonoBehaviour
    {
        private string firstName;
        /// <summary>
        /// The passenger's first name
        /// </summary>
        public string FirstName
        {
            get { return firstName; }
        }

        private string lastName;
        /// <summary>
        ///  The passegner's last name
        /// </summary>
        public string LastName
        {
            get { return lastName; }
        }

        private BusStop destinationBusStop = null;
        public BusStop DestinationBusStop
        { get { return destinationBusStop; } }

        private void Awake()
        {
            firstName = RandomNameGenerator.GetFirstName();
            lastName = RandomNameGenerator.GetLastName();
            gameObject.name = "Passenger (" + firstName + " " + lastName + ")";
        }
    }
}
