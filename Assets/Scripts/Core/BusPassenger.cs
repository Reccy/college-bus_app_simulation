using UnityEngine;
using AaronMeaney.BusStop.Utilities;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a bus passenger
    /// </summary>
    public class BusPassenger
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

        /// <summary>
        /// The passenger's full name
        /// </summary>
        public string FullName
        {
            get { return (firstName + " " + lastName); }
        }

        private BusStop originBusStop = null;
        /// <summary>
        /// The <see cref="BusStop"/> that the passenger started waiting at.
        /// </summary>
        public BusStop OriginBusStop { get { return originBusStop; } }

        private BusStop destinationBusStop = null;
        /// <summary>
        /// The <see cref="BusStop"/> that the passenger wants to go to.
        /// </summary>
        public BusStop DestinationBusStop { get { return destinationBusStop; } }

        private Bus boardedBus = null;

        public BusPassenger(BusStop originBusStop, BusStop destinationBusStop)
        {
            this.firstName = RandomNameGenerator.GetFirstName();
            this.lastName = RandomNameGenerator.GetLastName();

            this.originBusStop = originBusStop;
            this.destinationBusStop = destinationBusStop;
        }

        /// <summary>
        /// Has the <see cref="BusPassenger"/> board the <see cref="Bus"/>.
        /// </summary>
        public void BoardBus(Bus bus)
        {
            boardedBus = bus;

            boardedBus.OnNearStop += (stop) =>
            {
                Debug.Log("OnNearStop called for " + stop.BusStopIdInternal);
                if (stop == destinationBusStop && boardedBus.IsStopping == false)
                {
                    Debug.Log(FullName + " pressed hailed the bus " + boardedBus.RegistrationNumber);
                    boardedBus.Hail(stop);
                }
            };
        }
    }
}
