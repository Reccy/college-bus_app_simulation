using System;
using UnityEngine;
using UnityEditor;

namespace AaronMeaney.BusStop.Scheduling
{
    /// <summary>
    /// Responsible for maintaining the current date/time in the simulation
    /// </summary>
    public class DateTimeManager : MonoBehaviour
    {
        public enum DateTimeMode { Realtime, Simulated };
        private DateTimeMode mode;
        /// <summary>
        /// If the 
        /// </summary>
        public DateTimeMode Mode
        {
            get { return mode; }
            set { mode = value; }
        }

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
        
        private void Update()
        {
            switch (Mode)
            {
                case DateTimeMode.Realtime:
                    SetToNow();
                    break;
                case DateTimeMode.Simulated:
                    AdvanceTime();
                    break;
            }
        }

        /// <summary>
        /// Sets <see cref="CurrentDateTime"/> to now.
        /// </summary>
        private void SetToNow()
        {
            CurrentDateTime = DateTime.Now;
        }

        /// <summary>
        /// Advanced <see cref="CurrentDateTime"/> by <see cref="Time.deltaTime"/>
        /// </summary>
        private void AdvanceTime()
        {
            throw new NotImplementedException();
        }
    }

    [CustomEditor(typeof(DateTimeManager))]
    public class DateTimeManagerEditor : Editor
    {
        private DateTimeManager dateTimeManager;
        private string currentTime;
        private string currentDay;

        void OnEnable()
        {
            dateTimeManager = ((DateTimeManager)target);
            EditorApplication.update += Update;
        }

        void OnDisable()
        {
            EditorApplication.update -= Update;
        }

        void Update()
        {
            currentDay = dateTimeManager.CurrentDateTime.DayOfWeek.ToString();
            currentTime = dateTimeManager.CurrentDateTime.ToLongTimeString();
            Repaint();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DateTimeField();
        }
        
        private void DateTimeField()
        {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.LabelField("Current Day", dateTimeManager.CurrentDateTime.DayOfWeek.ToString());
            EditorGUILayout.LabelField("Current Time", dateTimeManager.CurrentDateTime.ToLongTimeString());
            EditorGUI.EndDisabledGroup();
        }
    }
}
