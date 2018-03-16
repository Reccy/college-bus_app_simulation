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

                return timeSlots;
            }
        }

        public BusService(BusTimetable busTimetable)
        {
            this.busTimetable = busTimetable;
        }
    }
}
