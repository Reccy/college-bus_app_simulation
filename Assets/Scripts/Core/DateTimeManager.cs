using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

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
        
        private void Update()
        {
            SetToNow();
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
