using System.Collections.Generic;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusRoute"/> that's in service with a schedule determined by a list of <see cref="BusTimeSlot"/>s.
    /// </summary>
    [System.Serializable]
    public class BusService
    {
        private BusTimetable busTimetable = null;
        /// <summary>
        /// The <see cref="BusTimetable"/> that owns this <see cref="BusService"/>
        /// </summary>
        public BusTimetable ParentBusTimetable {
            get
            {
                if (busTimetable == null)
                {
                    List<BusTimetable> possibleTimetables = ServicedBusRoute.gameObject.transform.parent.GetComponentInParent<BusCompany>().BusTimetables;

                    foreach (BusTimetable timetable in possibleTimetables)
                    {
                        if (timetable.BusServices.Contains(this))
                        {
                            busTimetable = timetable;
                            return busTimetable;
                        }
                    }
                }
                
                return busTimetable;
            }
        }
        
        [SerializeField]
        private BusRoute servicedBusRoute = null;
        /// <summary>
        /// The <see cref="BusRoute"/> that this service is running on.
        /// </summary>
        public BusRoute ServicedBusRoute {
            get { return servicedBusRoute; }
            set { servicedBusRoute = value; }
        }

        [SerializeField]
        private List<BusTimeSlot> timeSlots = null;
        /// <summary>
        /// The list of <see cref="BusTimeSlot"/>s that define the schedule of this <see cref="BusService"/>.
        /// </summary>
        public List<BusTimeSlot> TimeSlots {
            get
            {
                if (timeSlots == null)
                    timeSlots = new List<BusTimeSlot>();

                if (Application.isPlaying && ServicedBusRoute != null)
                {
                    // Runtime cleanup
                    // Ensure that each TimeSlot belongs to the ServicedBusRoute
                    for (int timeSlotIndex = 0; timeSlotIndex < timeSlots.Count; timeSlotIndex ++)
                    {
                        if (!ServicedBusRoute.BusStops.Contains(timeSlots[timeSlotIndex].ScheduledBusStop))
                        {
                            timeSlots.Remove(timeSlots[timeSlotIndex]);
                        }
                    }
                }

                return timeSlots;
            }
        }

        private Bus servicingBus = null;
        /// <summary>
        /// The <see cref="Bus"/> that is servicing this <see cref="BusService"/>
        /// </summary>
        public Bus ServicingBus { get { return servicingBus; } }
        
        public BusService(BusTimetable busTimetable)
        {
            this.busTimetable = busTimetable;
        }

        private BusTimeSlot scheduledTimeSlot = null;
        /// <summary>
        /// The <see cref="BusTimeSlot"/> that should be in service at the current time.
        /// </summary>
        public BusTimeSlot ScheduledTimeSlot
        {
            get { return scheduledTimeSlot; }
            set
            {
                scheduledTimeSlot = value;

                if (!IsInService())
                {
                    Debug.Log(ServicedBusRoute.RouteIdInternal + " is starting to be serviced...");
                    Debug.Log("The first stop is " + ServicedBusRoute.BusStops[0]);
                    Debug.Log("The first time slot's bus is " + TimeSlots[0].ScheduledBusStop);
                    servicingBus = ParentBusTimetable.ParentBusCompany.DeployBus(this);
                }
            }
        }
        
        /// <summary>
        /// The <see cref="BusTimeSlot"/> that is currently being serviced.
        /// </summary>
        public BusTimeSlot CurrentTimeSlot
        {
            get
            {
                if (ServicingBus == null)
                    return null;

                return ServicingBus.CurrentTimeSlot;
            }
        }
        
        public enum Schedule { NotInService, BehindSchedule, OnSchedule, AheadOfSchedule }
        /// <summary>
        /// Represents if the <see cref="BusService"/> is in service and if it's behind, ahead or on schedule.
        /// </summary>
        public Schedule ScheduleState
        {
            get
            {
                if (ScheduledTimeSlot == null || CurrentTimeSlot == null)
                {
                    return Schedule.NotInService;
                }

                int scheduledTimeSlotIndex = TimeSlots.IndexOf(scheduledTimeSlot);
                int currentTimeSlotIndex = TimeSlots.IndexOf(CurrentTimeSlot);

                if (scheduledTimeSlotIndex < currentTimeSlotIndex)
                {
                    return Schedule.AheadOfSchedule;
                }
                else if (scheduledTimeSlot == CurrentTimeSlot)
                {
                    return Schedule.OnSchedule;
                }
                else
                {
                    return Schedule.BehindSchedule;
                }
            }
        }

        public bool IsInService()
        {
            return ScheduleState != Schedule.NotInService;
        }

        public bool IsBehindSchedule()
        {
            return ScheduleState == Schedule.BehindSchedule;
        }

        public bool IsOnSchedule()
        {
            return ScheduleState == Schedule.OnSchedule;
        }

        public bool IsAheadOfSchedule()
        {
            return ScheduleState == Schedule.AheadOfSchedule;
        }
    }
}
