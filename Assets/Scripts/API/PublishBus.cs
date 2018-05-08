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
    [RequireComponent(typeof(Bus))]
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
                bus.OnBusHailed += (stop) => PublishBusState();
                bus.OnStartService += () => PublishBusStateRecursively();
                bus.OnEndService += () => PublishEndService();
            };
        }
        
        private void PublishBusStateRecursively()
        {
            if (PublishBusState())
                scheduleTaskRunner.AddTask(new ScheduledTask(() => PublishBusStateRecursively(), DateTime.Now.AddSeconds(2)));
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

        /// <summary>
        /// Publishes the <see cref="Bus"/>'s state.
        /// Returns true if successful. False otherwise.
        /// <returns></returns>
        private bool PublishBusState()
        {
            if (!bus.gameObject.activeInHierarchy)
                return false;

            Dictionary<string, object> timeslots = new Dictionary<string, object>();

            List<DayOfWeek> daysOfWeek = bus.CurrentService.ParentBusTimetable.DaysRunning();

            List<string> hailedBusStops = new List<string>();
            foreach (Core.BusStop stop in bus.HailedStops)
            {
                hailedBusStops.Add(stop.BusStopIdInternal);
            }
            
            foreach (Core.BusStop stop in bus.CurrentRoute.BusStops)
            {
                foreach (BusTimeSlot slot in bus.CurrentService.TimeSlots)
                {
                    if (slot.ScheduledBusStop.Equals(stop))
                    {
                        string scheduledHour = slot.ScheduledHour.ToString();
                        if (scheduledHour.Length == 1)
                            scheduledHour = "0" + scheduledHour;

                        string scheduledMinute = slot.ScheduledMinute.ToString();
                        if (scheduledMinute.Length == 1)
                            scheduledMinute = "0" + scheduledMinute;

                        timeslots.Add(slot.ScheduledBusStop.BusStopIdInternal, scheduledHour + ":" + scheduledMinute);
                    }
                }
            }

            List<Dictionary<string, object>> waypoints = new List<Dictionary<string, object>>();
            foreach (RouteWaypoint waypoint in bus.CurrentRoute.RouteWaypoints)
            {
                Dictionary<string, object> latlng = new Dictionary<string, object>();
                latlng.Add("latitude", waypoint.Latitude);
                latlng.Add("longitude", waypoint.Longitude);
                waypoints.Add(latlng);
            }
            
            Dictionary<string, object> publishDict = new Dictionary<string, object>();
            publishDict.Add("bus_name", bus.name);
            publishDict.Add("latitude", bus.Latitude);
            publishDict.Add("longitude", bus.Longitude);
            publishDict.Add("registration_number", bus.RegistrationNumber);
            publishDict.Add("model", bus.BusModel);
            publishDict.Add("company", bus.Company.CompanyName);
            publishDict.Add("route_id_internal", bus.CurrentRoute.RouteIdInternal);
            publishDict.Add("current_stop_id_internal", bus.CurrentStop.BusStopIdInternal);
            publishDict.Add("current_capacity", bus.CurrentCapacity);
            publishDict.Add("maximum_capacity", bus.MaximumCapacity);
            publishDict.Add("hailed_stops", hailedBusStops);
            publishDict.Add("timeslots", timeslots);
            publishDict.Add("waypoints", waypoints);
            publishDict.Add("days", daysOfWeek);
            
            busStopAPI.PublishMessage("bus_data", publishDict);

            return true;
        }
    }
}
