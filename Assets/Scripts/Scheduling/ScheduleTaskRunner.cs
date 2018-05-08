using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

namespace AaronMeaney.BusStop.Scheduling
{
    /// <summary>
    /// Contains a list of <see cref="ScheduledTask"/>s to run and executes their <see cref="ScheduledTask.ExecuteTask"/> method when their 
    /// </summary>
    public class ScheduleTaskRunner : MonoBehaviour
    {
        private DateTimeManager dateTimeManager;

        /// <summary>
        /// <see cref="List{T}"/> of <see cref="ScheduledTask"/>s in order of <see cref="ScheduledTask.ScheduledDateTime"/>
        /// </summary>
        private List<ScheduledTask> taskList = new List<ScheduledTask>();

        private void Awake()
        {
            dateTimeManager = FindObjectOfType<DateTimeManager>();
        }

        private void Update()
        {
            ExecuteReadyTasks();
        }

        /// <summary>
        /// Checks the <see cref="taskList"/> for <see cref="ScheduledTask"/>s that are ready to be executed and then calls <see cref="ScheduledTask.ExecuteTask"/>
        /// </summary>
        private void ExecuteReadyTasks()
        {
            while (taskList.Count > 0 && DateTime.Compare(taskList[0].ScheduledDateTime, dateTimeManager.CurrentDateTime) < 0)
            {
                taskList[0].ExecuteTask();
                taskList.Remove(taskList[0]);
            }
        }

        /// <summary>
        /// Adds a new <see cref="ScheduledTask"/> to the <see cref="taskList"/>
        /// </summary>
        /// <param name="newTask"></param>
        public void AddTask(ScheduledTask newTask)
        {
            // Add task in correct ordered position in the list
            for (int i = 0; i < taskList.Count; i++)
            {
                if (DateTime.Compare(taskList[i].ScheduledDateTime, newTask.ScheduledDateTime) > 0)
                {
                    taskList.Insert(i, newTask);
                    return;
                }
            }

            taskList.Add(newTask);
        }
    }
}
