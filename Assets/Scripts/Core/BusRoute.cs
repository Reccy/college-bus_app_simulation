﻿using Mapbox.Directions;
using Mapbox.Unity.Map;
using Mapbox.Unity.Utilities;
using Mapbox.Utils;
using Rotorz.ReorderableList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AaronMeaney.BusStop.Core
{
    /// <summary>
    /// Represents the list of <see cref="RouteWaypoint"/>s in a <see cref="BusRoute"/>.
    /// Does NOT contain any timetable/time slot data.
    /// </summary>
    public class BusRoute : MonoBehaviour
    {
        /// <summary>
        /// Called when <see cref="SetPathWaypoints"/> is finished.
        /// </summary>
        public Action OnBusPathReady;

        [SerializeField]
        [HideInInspector]
        private AbstractMap map;

        [SerializeField]
        private string routeId;
        /// <summary>
        /// The ID of this Route as displayed to the end user.
        /// </summary>
        public string RouteId { get { return routeId; } }

        [SerializeField]
        private string routeIdInternal;
        /// <summary>
        /// The ID of this Route for use in the Unity Engine only.
        /// </summary>
        public string RouteIdInternal { get { return routeIdInternal; } }
        
        [SerializeField]
        [HideInInspector]
        private List<RouteWaypoint> routeWaypoints;
        /// <summary>
        /// The list of <see cref="RouteWaypoint"/>s in the <see cref="BusRoute"/> 
        /// </summary>
        public List<RouteWaypoint> RouteWaypoints { get { return routeWaypoints; } }
        
        /// <summary>
        /// The list of all <see cref="BusStop"/>s in a <see cref="BusRoute"/>
        /// </summary>
        public List<BusStop> BusStops
        {
            get
            {
                List<BusStop> stops = new List<BusStop>();

                foreach (RouteWaypoint routeWaypoint in RouteWaypoints)
                {
                    if (routeWaypoint.LinkedBusStop != null)
                    {
                        stops.Add(routeWaypoint.LinkedBusStop);
                    }
                }

                return stops;
            }
        }
        
        private List<CoordinateLocation> pathWaypoints;
        /// <summary>
        /// The list of <see cref="CoordinateLocation"/>s that the bus has to follow. Generated by the <see cref="BusPathfinder"/>.
        /// </summary>
        public List<CoordinateLocation> PathWaypoints
        { get { return pathWaypoints; } }

        /// <summary>
        /// Used to determine the closest <see cref="CoordinateLocation"/> in <see cref="pathWaypoints"/> to a <see cref="RouteWaypoint"/> in <see cref="routeWaypoints"/>.
        /// </summary>
        private Dictionary<CoordinateLocation, RouteWaypoint> pathWaypointToBusWaypointDictionary = null;
        
        /// <summary>
        /// Gets the <see cref="CoordinateLocation"/> linked to the <see cref="BusStop.LinkedRouteWaypoint"/>.
        /// </summary>
        /// <param name="busStop">The <see cref="BusStop"/> that is being checked for the linked <see cref="CoordinateLocation"/></param>
        /// <returns>The linked <see cref="CoordinateLocation"/> of the <see cref="BusStop.LinkedRouteWaypoint"/></returns>
        public CoordinateLocation GetCoordinateLocationFromBusStop(BusStop busStop)
        {
            CoordinateLocation coordinateLocation = null;
            RouteWaypoint routeWaypoint = busStop.LinkedRouteWaypoint;

            if (pathWaypointToBusWaypointDictionary == null)
                return null;

            if (routeWaypoint == null)
                return null;

            if (!pathWaypointToBusWaypointDictionary.ContainsValue(routeWaypoint))
                return null;
            
            foreach (KeyValuePair<CoordinateLocation, RouteWaypoint> pair in pathWaypointToBusWaypointDictionary)
            {
                if (pair.Value == routeWaypoint)
                {
                    coordinateLocation = pair.Key;
                    break;
                }
            }

            return coordinateLocation;
        }

        private void OnEnable()
        {
            pathWaypoints = new List<CoordinateLocation>();
        }

        private void Awake()
        {
            map = FindObjectOfType<AbstractMap>();
        }

        /// <summary>
        /// Gets a <see cref="BusPathfinder"/> for each <see cref="RouteWaypoint"/> pair and appends the <see cref="CoordinateLocation"/>s to <see cref="PathWaypoints"/>.
        /// Logic is finished in <see cref="OnPathWaypointsPopulated(BusPathfinder)"/>
        /// </summary>
        public void SetPathWaypoints()
        {
            BusPathfinder pathfinder = new BusPathfinder();
            pathfinder.onBusPathPopulated += OnPathWaypointsPopulated;

            List<Vector2d> positionList = new List<Vector2d>();

            // Convert each Route Waypoint into a Vector2d position for Mapbox
            foreach (RouteWaypoint waypoint in RouteWaypoints)
            {
                Vector2d waypointAsVector = waypoint.transform.position.GetGeoPosition(map.CenterMercator, map.WorldRelativeScale);
                positionList.Add(waypointAsVector);
            }
            
            Vector2d[] positions = positionList.ToArray();

            pathfinder.SetDirectionsToPosition(map, positions);
        }

        /// <summary>
        /// Handles logic after the <see cref="BusPathfinder"/> has generated the <see cref="PathWaypoints"/>
        /// </summary>
        /// <param name="pathfinder"></param>
        public void OnPathWaypointsPopulated(BusPathfinder pathfinder)
        {
            pathWaypoints.Clear();

            pathWaypoints.AddRange(pathfinder.CoordinateLocations);

            // Populate pathWaypointToBusWaypointDictionary
            pathWaypointToBusWaypointDictionary = new Dictionary<CoordinateLocation, RouteWaypoint>();

            foreach (RouteWaypoint routeWaypoint in RouteWaypoints)
            {
                Vector3 routeWaypointPosition = routeWaypoint.transform.position;
                CoordinateLocation closestPathWaypoint = null;
                float currentClosestDistance = float.MaxValue;

                // Get current closest path waypoint for the routeWaypoint
                foreach (CoordinateLocation pathWaypoint in pathWaypoints)
                {
                    Vector3 pathWaypointPosition = pathWaypoint.AsUnityPosition(map);
                    float currentDistance = Vector3.Distance(routeWaypointPosition, pathWaypointPosition);

                    if (currentDistance < currentClosestDistance)
                    {
                        closestPathWaypoint = pathWaypoint;
                        currentClosestDistance = currentDistance;
                    }
                }

                pathWaypointToBusWaypointDictionary.Add(closestPathWaypoint, routeWaypoint);
            }

            if (OnBusPathReady != null)
                OnBusPathReady();
        }

        #region Topological Sort
        /// <summary>
        /// Sorts the <see cref="BusStop"/>s in the list of <see cref="BusRoute"/>s by using Topological Sort.
        /// Adapted from Wikipedia: https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
        /// </summary>
        /// <param name="busRoutes">List of <see cref="BusRoute"/>s to sort</param>
        /// <returns>An ordered topological list of <see cref="BusStop"/>s from each <see cref="BusRoute"/></returns>
        public static List<BusStop> TopologicalSort(List<BusRoute> busRoutes)
        {
            // Sorted bus stops to return at end of method
            List<BusStop> sortedBusStops = new List<BusStop>();

            // Dict of Bus Stop edges for graph
            Dictionary<BusStop, List<BusStop>> busStopEdges = new Dictionary<BusStop, List<BusStop>>();

            // List of unvisited Bus Stops for the sort
            List<BusStop> unvisitedBusStops = new List<BusStop>();
            
            // Create Graph of Bus Stops
            foreach (BusRoute route in busRoutes)
            {
                // Create Vertex for each pair of Bus Stops
                for (int busStopIndex = 1; busStopIndex < route.BusStops.Count; busStopIndex++)
                {
                    BusStop stop = route.BusStops[busStopIndex - 1];
                    BusStop nextStop = route.BusStops[busStopIndex];
                    
                    // Add Vertex if it doesn't already exist
                    if (!busStopEdges.ContainsKey(stop))
                        busStopEdges[stop] = new List<BusStop>();

                    // Add next stop to Vertex if it doesn't already exist
                    if (!busStopEdges[stop].Contains(nextStop))
                        busStopEdges[stop].Add(nextStop);

                    // Add stops list of unvisited Bus Stops for Topological Sort
                    if (!unvisitedBusStops.Contains(stop))
                    {
                        unvisitedBusStops.Add(stop);
                    }

                    if (!unvisitedBusStops.Contains(nextStop))
                    {
                        unvisitedBusStops.Add(nextStop);
                    }
                }
            }

            // Perform Topological Sort
            List<BusStop> beingVisitedBusStops = new List<BusStop>();

            while (unvisitedBusStops.Count != 0)
            {
                BusStop selectedStop = unvisitedBusStops[0];
                
                Visit(selectedStop, unvisitedBusStops, busStopEdges, sortedBusStops, beingVisitedBusStops);
            }

            sortedBusStops.Reverse();
            return sortedBusStops;
        }

        /// <summary>
        /// Visit helper method for <see cref="TopologicalSort(List{BusRoute})"/>
        /// Adapted from Wikipedia: https://en.wikipedia.org/wiki/Topological_sorting#Depth-first_search
        /// </summary>
        private static void Visit(BusStop busStop,
            List<BusStop> unvisitedBusStops,
            Dictionary<BusStop, List<BusStop>> busStopEdges,
            List<BusStop> sortedBusStops,
            List<BusStop> beingVisitedBusStops)
        {
            if (sortedBusStops.Contains(busStop))
            {
                return;
            }

            if (beingVisitedBusStops.Contains(busStop))
            {
                Debug.LogError("Cycle detected in DAG at: " + busStop.BusStopIdInternal);
                unvisitedBusStops.Remove(busStop);
                return;
            }

            beingVisitedBusStops.Add(busStop);
            
            if (busStopEdges.ContainsKey(busStop))
            {
                foreach (BusStop stop in busStopEdges[busStop])
                {
                    Visit(stop, unvisitedBusStops, busStopEdges, sortedBusStops, beingVisitedBusStops);
                }
            }
            
            unvisitedBusStops.Remove(busStop);
            sortedBusStops.Add(busStop);
        }
        #endregion

        #region Gizmos
        [SerializeField]
        [HideInInspector]
        private Color gizmoColor;

        [SerializeField]
        [HideInInspector]
        private bool gizmoEnabled;

        private void OnDrawGizmos()
        {
            if (gizmoEnabled)
            {
                DrawGizmos();
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos();
        }

        private void DrawGizmos()
        {
            // Show arrows between Route Waypoints
            Gizmos.color = gizmoColor;

            for (int i = 1; i < routeWaypoints.Count; i++)
            {
                if (routeWaypoints[i - 1] != null && routeWaypoints[i] != null)
                {
                    Vector3 dir = routeWaypoints[i].transform.position - routeWaypoints[i - 1].transform.position;
                    DrawArrow.ForGizmo(routeWaypoints[i - 1].transform.position, dir, Gizmos.color, 0.4f, arrowPosition: 0.5f);
                }
            }

            // Show arrows between Path Waypoints
            Gizmos.color = Color.blue;

            if (pathWaypoints == null)
                pathWaypoints = new List<CoordinateLocation>();

            for (int i = 1; i < pathWaypoints.Count; i++)
            {
                if (pathWaypoints[i - 1] != null && pathWaypoints[i] != null)
                {
                    Vector3 dir = pathWaypoints[i].AsUnityPosition(map) - pathWaypoints[i - 1].AsUnityPosition(map);
                    Gizmos.DrawWireSphere(pathWaypoints[i - 1].AsUnityPosition(map), 0.2f);
                    DrawArrow.ForGizmo(pathWaypoints[i - 1].AsUnityPosition(map), dir, Gizmos.color, 0.4f, arrowPosition: 0.5f);
                }
            }

            // Show links between path waypoint and bus waypoints
            Gizmos.color = Color.green;

            if (pathWaypointToBusWaypointDictionary != null)
            {
                foreach (KeyValuePair<CoordinateLocation, RouteWaypoint> pair in pathWaypointToBusWaypointDictionary)
                {
                    Vector3 fromPosition = pair.Key.AsUnityPosition(map);
                    Vector3 toPosition = pathWaypointToBusWaypointDictionary[pair.Key].transform.position;

                    Gizmos.DrawLine(fromPosition, toPosition);
                }
            }
        }
        #endregion

        private void OnValidate()
        {
            name = "Bus Route " + RouteIdInternal;
        }
    }

    [CustomEditor(typeof(BusRoute))]
    public class BusRouteEditor : Editor
    {
        private bool isGizmoEditorShown = true;
        private SerializedProperty _map;
        private SerializedProperty _gizmoColor;
        private SerializedProperty _gizmoEnabled;
        private SerializedProperty _routeId;
        private SerializedProperty _routeWaypoints;

        private void OnEnable()
        {
            _map = serializedObject.FindProperty("map");
            _gizmoColor = serializedObject.FindProperty("gizmoColor");
            _gizmoEnabled = serializedObject.FindProperty("gizmoEnabled");
            _routeId = serializedObject.FindProperty("routeId");
            _routeWaypoints = serializedObject.FindProperty("routeWaypoints");
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            serializedObject.Update();
            
            // Waypoint List
            ReorderableListGUI.Title(typeof(RouteWaypoint).Name + " List");
            ReorderableListGUI.ListField(_routeWaypoints);

            // Gizmos
            isGizmoEditorShown = EditorGUILayout.Foldout(isGizmoEditorShown, "Gizmo Visualisation");

            if (isGizmoEditorShown)
            {
                _gizmoEnabled.boolValue = EditorGUILayout.Toggle("Gizmo Enabled", _gizmoEnabled.boolValue);
                _gizmoColor.colorValue = EditorGUILayout.ColorField("Gizmo Color", _gizmoColor.colorValue);
            }

            // Show the map route as Gizmos
            if (Application.isPlaying && _map.objectReferenceValue != null)
            {
                if (GUILayout.Button("Show Bus Route"))
                {
                    ((BusRoute)target).SetPathWaypoints();
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }
    }
}
