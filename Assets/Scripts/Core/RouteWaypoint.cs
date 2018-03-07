﻿using Mapbox.Unity.Map;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents a waypoint along a <see cref="BusPathfinder"/>
    /// </summary>
    [ExecuteInEditMode] // For Editor Validation
    [RequireComponent(typeof(SnapToTerrain), typeof(PlaceAtCoordinates))]
    public class RouteWaypoint : MonoBehaviour
    {
        private SnapToTerrain snapToTerrain;
        private AbstractMap map;

        private PlaceAtCoordinates placeAtCoordinates;
        public PlaceAtCoordinates PlactAtCoordinates { get { return placeAtCoordinates; } }

        /// <summary>
        /// The <see cref="BusStop"/> belonging to this <see cref="RouteWaypoint"/>
        /// </summary>
        public BusStop LinkedBusStop
        { get { return GetComponentInChildren<BusStop>(); } }

        private void Awake()
        {
            snapToTerrain = GetComponent<SnapToTerrain>();
            placeAtCoordinates = GetComponent<PlaceAtCoordinates>();
            map = FindObjectOfType<AbstractMap>();
            
            if (Application.isPlaying)
            {
                map.MapVisualizer.OnMapVisualizerStateChanged += OnMapVisualizerStateChanged;
            }
        }

        /// <summary>
        /// Handles what to do when the <see cref="MapVisualizer"/> state changes
        /// </summary>
        /// <param name="state">The new <see cref="ModuleState"/></param>
        private void OnMapVisualizerStateChanged(ModuleState state)
        {
            if (state == ModuleState.Finished)
            {
                Debug.Log("Placing at coordinates");
                placeAtCoordinates.Execute();

                Debug.Log("Performing snap");
                snapToTerrain.PerformSnap();
            }
        }

        #region Editor Validation

        private void OnTransformParentChanged()
        {
            ValidateAllWaypoints();
        }

        /// <summary>
        /// Calls <see cref="UpdateUniqueName"/> for all instances of <see cref="RouteWaypoint"/> and ensures that they are in a "Routes" game object in the hierarchy.
        /// </summary>
        protected internal static void ValidateAllWaypoints()
        {
            foreach (RouteWaypoint routeWaypoint in GameObject.FindObjectsOfType<RouteWaypoint>())
            {
                routeWaypoint.UpdateUniqueName();
                routeWaypoint.ValidateHierarchyPosition();
            }
        }

        /// <summary>
        /// Sets the <see cref="RouteWaypoint"/> name to "RouteWaypoint_" and the result of <see cref="GetInstanceNumber"/>.
        /// If there is a <see cref="LinkedBusStop"/>, then its set to the <see cref="BusStop"/> name.
        /// </summary>
        private void UpdateUniqueName()
        {
            if (LinkedBusStop != null)
            {
                name = LinkedBusStop.BusStopId + "_Waypoint";
            }
            else
            {
                name = "RouteWaypoint_" + GetInstanceNumber();
            }
        }

        /// <summary>
        /// Ensures that this object's parent is the RouteWaypointContainer.
        /// If it doesn't exist, it creates it and then makes it the parent.
        /// </summary>
        private void ValidateHierarchyPosition()
        {
            GameObject container = GameObject.Find("RouteWaypointContainer");

            if (container != null)
            {
                transform.parent = container.transform;
            }
            else
            {
                container = Instantiate<GameObject>(new GameObject(), Vector3.zero, Quaternion.identity);
                container.name = "RouteWaypointContainer";
                transform.parent = container.transform;
            }
        }
        
        /// <summary>
        /// Returns an number representing what order this object was instantiated at starting from the first instance to the newest instance.
        /// </summary>
        /// <returns>Number representing the order this object was instantiated at.</returns>
        private int GetInstanceNumber()
        {
            int instanceNumber = 0;
            List<RouteWaypoint> routeWaypoints = new List<RouteWaypoint>(FindObjectsOfType<RouteWaypoint>());
            routeWaypoints.Reverse(); // Reverse so that the newest instance is not '1' and is the actual count of 'instances + 1'
            foreach (RouteWaypoint routeWaypoint in routeWaypoints)
            {
                instanceNumber++;
                if (routeWaypoint.Equals(this))
                {
                    return instanceNumber;
                }
            }
            
            throw new MissingReferenceException();
        }
        
        #endregion

        #region Gizmos

        private void OnDrawGizmos()
        {
            // Draw line to Bus Stop
            Gizmos.color = Color.yellow;

            if (LinkedBusStop)
            {
                Gizmos.DrawLine(transform.position, LinkedBusStop.transform.position);
            }
            
            // Display name of node
            Handles.Label(transform.position, name);
        }

        #endregion
    }

    [CustomEditor(typeof(RouteWaypoint))]
    public class RouteWaypointEditor : Editor
    {
        void OnEnable()
        {
            RouteWaypoint.ValidateAllWaypoints();
        }
        
        void OnDisable()
        {
            RouteWaypoint.ValidateAllWaypoints();
        }
    }
}
