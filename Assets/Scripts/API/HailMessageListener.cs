using UnityEngine;
using AaronMeaney.BusStop.Core;
using System.Collections.Generic;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Listens for a hail message from the <see cref="BusStopAPI"/>.
    /// Hails the bus once a hail message is received.
    /// </summary>
    public class HailMessageListener : MonoBehaviour
    {
        private Bus bus;
        private BusStopAPI busStopAPI;

        private void Awake()
        {
            bus = GetComponent<Bus>();
            busStopAPI = FindObjectOfType<BusStopAPI>();

            busStopAPI.OnMessageReceived += (message) => HandleBusHail(message);
        }

        /// <summary>
        /// Performs a bus hail if the topic, bus and bus stop params are correct.
        /// </summary>
        private void HandleBusHail(Dictionary<string, object> message)
        {
            string topic = message["topic"].ToString();

            // Ensure topic is correct
            if (topic != "hail_bus")
                return;

            Dictionary<string, object> hailMessage = (Dictionary<string, object>)message["message"];
            string busRegistration = hailMessage["bus_reg"].ToString();
            string busStopInternalId = hailMessage["bus_stop"].ToString();

            // Ensure bus registration is correct
            if (!busRegistration.Equals(bus.RegistrationNumber))
                return;

            Debug.Log("Stopping bus with reg: " + bus.RegistrationNumber);
            bus.PrepareToStop();
        }
    }
}
