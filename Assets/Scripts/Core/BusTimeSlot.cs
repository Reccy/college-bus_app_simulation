using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusStop"/> timeslot in a <see cref="BusService"/>
    /// </summary>
    [System.Serializable]
    public class BusTimeSlot
    {
        private BusService parentBusService;
        /// <summary>
        /// The <see cref="BusService"/> that owns this <see cref="BusTimeSlot"/>
        /// </summary>
        public BusService ParentBusService { get { return parentBusService; } }

        [SerializeField]
        private BusStop scheduledBusStop;
        /// <summary>
        /// The <see cref="BusStop"/> that the bus will stop at on this <see cref="BusTimeSlot"/>
        /// </summary>
        public BusStop ScheduledBusStop;

        [SerializeField]
        private int scheduledHour;
        /// <summary>
        /// The hour when the bus should be at the <see cref="BusStop"/>
        /// </summary>
        public int ScheduledHour
        {
            get { return scheduledHour; }
            set
            {
                if (value < 0 || value > 23)
                {
                    Debug.LogError("Scheduled Hour must be between 0 and 23 inclusive. Actual value was " + value);
                    return;
                }

                scheduledHour = value;
            }
        }

        [SerializeField]
        private int scheduledMinute;
        /// <summary>
        /// The minute when the bus should be at the <see cref="BusStop"/>
        /// </summary>
        public int ScheduledMinute
        {
            get { return scheduledMinute; }
            set
            {
                if (value < 0 || value > 59)
                {
                    Debug.LogError("Scheduled Minute must be between 0 and 59 inclusive. Actual value was " + value);
                    return;
                }

                scheduledMinute = value;
            }
        }
    }
}
