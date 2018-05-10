using System;
using System.Collections.Generic;
using AaronMeaney.BusStop.Scheduling;
using UnityEngine;

namespace AaronMeaney.BusStop.API
{
    public class PublishBusRoutes : MonoBehaviour
    {
        private ScheduleTaskRunner taskRunner;
        private BusStopAPI busStopAPI;

        private void Awake()
        {
            busStopAPI = FindObjectOfType<BusStopAPI>();
            taskRunner = FindObjectOfType<ScheduleTaskRunner>();

            busStopAPI.OnAPIInitialized += () => busStopAPI.UpdateBusRoutes(new List<Core.BusRoute>(GetComponentsInChildren<Core.BusRoute>()));
        }
    }
}
