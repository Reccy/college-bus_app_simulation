using UnityEngine;
using AaronMeaney.BusStop.Core;
using System.Collections.Generic;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Responsible for updating the <see cref="BusStopAPI"/> record of all <see cref="Core.BusStop"/>s.
    /// </summary>
    public class PublishBusStops : MonoBehaviour
    {
        private BusStopAPI busStopAPI;

        private void Awake()
        {
            busStopAPI = FindObjectOfType<BusStopAPI>();

            busStopAPI.OnAPIInitialized += () => busStopAPI.UpdateBusStops(new List<Core.BusStop>(GetComponentsInChildren<Core.BusStop>()));
        }
    }
}
