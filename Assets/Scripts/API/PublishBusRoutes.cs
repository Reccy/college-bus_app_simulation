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
        private List<Core.BusRoute> busRoutes;

        private void Awake()
        {
            busStopAPI = FindObjectOfType<BusStopAPI>();
            taskRunner = FindObjectOfType<ScheduleTaskRunner>();
            busRoutes = new List<Core.BusRoute>(GetComponentsInChildren<Core.BusRoute>());

            busStopAPI.OnAPIInitialized += () =>
            {
                foreach (Core.BusRoute route in busRoutes)
                {
                    route.GetPathWaypointsDebug((waypoints) => busStopAPI.UpdateRouteWaypoints(route.RouteIdInternal, waypoints));
                }

                busStopAPI.UpdateBusRoutes(new List<Core.BusRoute>(GetComponentsInChildren<Core.BusRoute>()));
            };
        }
    }
}
