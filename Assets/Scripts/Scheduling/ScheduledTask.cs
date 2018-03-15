using System;
using UnityEngine;

namespace AaronMeaney.BusStop.Scheduling
{
    /// <summary>
    /// Represents a method to call and the <see cref="ScheduledDateTime"/> to run it at.
    /// </summary>
    public class ScheduledTask
    {
        private DateTime scheduledDateTime;
        /// <summary>
        /// When the <see cref="ExecuteTask"/> method should be called.
        /// </summary>
        public DateTime ScheduledDateTime
        {
            get { return scheduledDateTime; }
        }

        private Action callback;

        /// <summary>
        /// The method to call when <see cref="DateTimeManager.CurrentDateTime"/> reaches <see cref="ScheduledDateTime"/>
        /// Called by the <see cref="ScheduleTaskRunner"/>
        /// </summary>
        public void ExecuteTask()
        {
            callback();
        }

        public ScheduledTask(Action callback, DateTime scheduledDateTime)
        {
            this.callback = callback;
            this.scheduledDateTime = scheduledDateTime;
        }
    }
}
