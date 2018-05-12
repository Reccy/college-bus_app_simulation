using UnityEngine;
using AaronMeaney.BusStop.Utilities;
using System;

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

            // Hail the bus once it approaches and it is going to the passenger's destination
            originBusStop.OnBusApproach += HailBus;
        }

        /// <summary>
        /// The passenger will try to hail the bus.
        /// </summary>
        private void HailBus(Bus bus)
        {
            Debug.Log("OnBusApproach called for " + bus.RegistrationNumber);
            if (bus.CurrentRoute.BusStops.Contains(destinationBusStop) && !bus.HailedStops.Contains(originBusStop))
            {
                Debug.Log(FullName + " hailed " + bus.RegistrationNumber + " to " + originBusStop.BusStopIdInternal + " because it is going to " + destinationBusStop.BusStopIdInternal);
                bus.Hail(originBusStop);
                originBusStop.OnBusApproach -= HailBus;
            }
        }

        /// <summary>
        /// Has the <see cref="BusPassenger"/> board the <see cref="Bus"/>.
        /// </summary>
        public void BoardBus(Bus bus)
        {
            originBusStop.OnBusApproach -= HailBus;

            boardedBus = bus;

            boardedBus.OnNearStop += (stop) =>
            {
                Debug.Log("OnNearStop called for " + stop.BusStopIdInternal);
                if (stop == destinationBusStop && boardedBus.IsStopping == false)
                {
                    Debug.Log(FullName + " pressed hail for the bus " + boardedBus.RegistrationNumber + " because they are getting off at " + stop.BusStopIdInternal);
                    boardedBus.Hail(stop);
                }
            };
        }
    }
}
