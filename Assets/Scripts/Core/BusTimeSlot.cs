using UnityEngine;
using AaronMeaney.BusStop.Scheduling;
using System;
using System.Collections.Generic;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a <see cref="BusStop"/> timeslot in a <see cref="BusService"/>
    /// </summary>
    [System.Serializable]
    public class BusTimeSlot
    {
        private bool isInitialized = false;
        private ScheduleTaskRunner taskRunner = null;
        private DateTimeManager dateTimeManager = null;

        private BusService service = null;
        /// <summary>
        /// The <see cref="BusService"/> that is the parent to this <see cref="BusTimeSlot"/>
        /// </summary>
        public BusService Service { get { return service; } }

        [SerializeField]
        private BusStop scheduledBusStop;
        /// <summary>
        /// The <see cref="BusStop"/> that the bus will stop at on this <see cref="BusTimeSlot"/>
        /// </summary>
        public BusStop ScheduledBusStop {
            get { return scheduledBusStop; }
            set { scheduledBusStop = value; }
        }

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

        /// <summary>
        /// Sets up references for this <see cref="BusTimeSlot"/>.
        /// </summary>
        /// <param name="taskRunner">Reference to the <see cref="ScheduleTaskRunner"/></param>
        public void Initialize(BusService service, DateTimeManager dateTimeManager, ScheduleTaskRunner taskRunner)
        {
            if (isInitialized)
                return;

            this.service = service;
            this.dateTimeManager = dateTimeManager;
            this.taskRunner = taskRunner;

            ScheduleTimeSlot();

            isInitialized = true;
        }

        /// <summary>
        /// Schedules this <see cref="BusTimeSlot"/>'s activation with the <see cref="ScheduleTaskRunner"/>.
        /// </summary>
        private void ScheduleTimeSlot()
        {
            DateTime currentDateTime = dateTimeManager.CurrentDateTime;
            List<DayOfWeek> runningDays = Service.ParentBusTimetable.DaysRunning();

            // Set scheduled date time
            DateTime scheduledDateTime = new DateTime(currentDateTime.Year, currentDateTime.Month, currentDateTime.Day, scheduledHour, scheduledMinute, 0);

            // If the scheduled time is in the past or does not take place on a scheduled day, advance day by 1 and check again
            while (DateTime.Compare(scheduledDateTime, currentDateTime) < 0 || !runningDays.Contains(scheduledDateTime.DayOfWeek))
            {
                scheduledDateTime = scheduledDateTime.AddDays(1);
            }

            // Create Scheduled Task
            ScheduledTask task = new ScheduledTask(() => { Debug.Log("Triggered Time Slot for Stop " + ScheduledBusStop.BusStopIdInternal + " at " + scheduledDateTime.ToShortTimeString()); }, scheduledDateTime);
            taskRunner.AddTask(task);
        }
    }
}
