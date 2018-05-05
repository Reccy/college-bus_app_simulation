using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.API
{
    public class PublishBusRoutes : MonoBehaviour
    {
        private BusStopAPI busStopAPI;

        private void Awake()
        {
            busStopAPI = FindObjectOfType<BusStopAPI>();

            busStopAPI.OnAPIInitialized += () => busStopAPI.UpdateBusRoutes(new List<Core.BusRoute>(GetComponentsInChildren<Core.BusRoute>()));
        }
    }
}
