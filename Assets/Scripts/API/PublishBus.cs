using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AaronMeaney.BusStop.Core;
using AaronMeaney.BusStop.Scheduling;
using AaronMeaney.BusStop.Utilities;
using System;

namespace AaronMeaney.BusStop.API
{
    /// <summary>
    /// Responsible for pushing the <see cref="Bus"/> current state to <see cref="PubNubAPI"/>
    /// </summary>
    public class PublishBus : MonoBehaviour
    {
        private ScheduleTaskRunner scheduleTaskRunner;
        private BusStopAPI busStopAPI;
        private Bus bus;

        private void Awake()
        {
            scheduleTaskRunner = FindObjectOfType<ScheduleTaskRunner>();
            busStopAPI = FindObjectOfType<BusStopAPI>();
            bus = GetComponent<Bus>();

            busStopAPI.OnAPIInitialized += () =>
            {
                bus.OnStartService += () => PublishStartService();
                bus.OnStartService += () => PublishBusState();
                bus.OnEndService += () => PublishEndService();
            };
        }
        
        private void PublishStartService()
        {
            Dictionary<string, object> publishDict = new Dictionary<string, object>();
            publishDict.Add("bus_name", bus.name);
            busStopAPI.PublishMessage("bus_start_service", publishDict);
        }

        private void PublishEndService()
        {
            Dictionary<string, object> publishDict = new Dictionary<string, object>();
            publishDict.Add("bus_name", bus.name);
            busStopAPI.PublishMessage("bus_end_service", publishDict);
        }

        private void PublishBusState()
        {
            if (!bus.gameObject.activeInHierarchy)
                return;

            Dictionary<string, object> publishDict = new Dictionary<string, object>();
            publishDict.Add("bus_name", bus.name);
            publishDict.Add("latitude", bus.Latitude);
            publishDict.Add("longitude", bus.Longitude);
            busStopAPI.PublishMessage("bus_data", publishDict);

            scheduleTaskRunner.AddTask(new ScheduledTask(() => PublishBusState(), DateTime.Now.AddSeconds(2)));
        }
    }
}