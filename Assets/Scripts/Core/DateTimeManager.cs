using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Responsible for maintaining the current date/time in the simulation
    /// </summary>
    public class DateTimeManager : MonoBehaviour
    {
        /// <summary>
        /// The DateTime determined by the user/simulation.
        /// </summary>
        private DateTime currentDateTime;

        /// <summary>
        /// The current date time of the simulation
        /// </summary>
        public DateTime CurrentDateTime {
            get
            {
                return currentDateTime;
            }
            set
            {
                currentDateTime = value;
            }
        }
        
        /// <summary>
        /// Sets the current date time to now.
        /// </summary>
        public void SetToNow()
        {
            CurrentDateTime = DateTime.Now;
        }

        /// <summary>
        /// Adds the deltaTime to the current time, keeping it up to date.
        /// </summary>
        public void AdvanceTimeByDeltaTime()
        {
            CurrentDateTime = CurrentDateTime.AddSeconds(Time.deltaTime);
        }

        private void Awake()
        {
            SetToNow();
        }

        private void Update()
        {
            AdvanceTimeByDeltaTime();
        }
    }
}