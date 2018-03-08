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
        private BusTimetable busTimetable;
        /// <summary>
        /// The <see cref="BusTimetable"/> that owns this <see cref="BusService"/>
        /// </summary>
        public BusTimetable ParentBusTimetable { get { return busTimetable; } }
        
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
        public List<BusTimeSlot> TimeSlots { get { return timeSlots; } }

        public BusService(BusTimetable busTimetable)
        {
            this.busTimetable = busTimetable;
        }
    }
}
